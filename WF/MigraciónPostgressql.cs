using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ENV.Data
{
    public class MigracionPostgresql
    {
        public string ConvertirPost(string sql)
        {
            int indice = 0;
            string palabra = string.Empty;

            if (sql.IndexOf("Ñ") > 0 || sql.IndexOf("ñ") > 0)
            {
                string[] results = sql.Split(new char[] { ' ', '=', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string elemento in results)
                {
                    if (elemento.IndexOf("Ñ") > 0)
                    {
                        palabra = "\"" + elemento + "\"";
                        sql = sql.Replace(elemento, palabra);

                    }
                    if (elemento.IndexOf("ñ") > 0)
                    {
                        palabra = "\"" + elemento.ToUpper() + "\"";
                        sql = sql.Replace(elemento, palabra);
                    }
                }
            }

            sql = CambiarBool(sql);

            sql = ChangeHourFormat(sql);

            sql = PostgresHelper.ReplaceConcatOperator(sql);

            if (sql.IndexOf("GLOBAL TEMPORARY") > 0)
            {
                sql = TablasTemporales(sql);
            }

            if (sql.IndexOf("TIMESTAMP") > 0)
            {
                sql = GetTimestamp(sql);
            }

            if (sql.IndexOf(" DAY") > 0)
            {
                sql = ConvertAddDay(sql);
            }

            if (sql.IndexOf("LEFT JOIN CBTIPOSRESPBOLETOS AS CTB0") > 0)
            {
                sql = ProcesarConsultaBoletos(sql);
            }

            if (sql.IndexOf("INSERT INTO  SESSION.AUXILIAR_SECUENCIA VALUES") > 0)
            {
                sql = Atomic();
            }

            if (sql.IndexOf("DAYOFWEEK") > 0)
            {
                sql = CambiarDAYOFWEEK(sql);
            }

            if (sql.IndexOf("FOR FETCH ONLY") > 0)
            {
                sql = CambiarFORFETCH(sql);
            }

            if (sql.StartsWith("CALL "))
            {
                sql = Stored(sql);
            }

            if (sql.Contains("LOCATE") && sql.Contains("OCTETS") && sql.Contains("WITH"))
            {
                sql = locate(sql);
            }

            if (sql.Contains("LISTAGG"))
            {
                sql = PostgresHelper.ReplaceListAgg(sql, true);
            }

            if (sql.Contains("DATE("))
            {
                sql = Remplazar(sql, "DATE(", "Fecha_texto(");
            }

            if (sql.Contains("NVL("))
            {
                sql = Reemplazar(sql, "NVL(", "COALESCE(");
            }

            if (sql.Contains("VALUE("))
            {
                sql = Remplazar(sql, "VALUE(", "COALESCE(");
            }

            if (sql.Contains(")CHAR("))
            {
                sql = CambiarChar(sql);
            }

            if (sql.Contains("SET PT.HTIEMPOESTANCIA = PT.HTIEMPOESTANCIA + 1 SECOND "))
            {
                sql = Remplazar(sql, "SET PT.HTIEMPOESTANCIA = PT.HTIEMPOESTANCIA + 1 SECOND ", "SET HTIEMPOESTANCIA = HTIEMPOESTANCIA + interval ' 1  SECOND' ");
            }

            if (sql.Contains("CAST (CONVERT (CHAR, GETDATE(), 112) AS DATETIME)"))
            {
                sql = Remplazar(sql, "CAST (CONVERT (CHAR, GETDATE(), 112) AS DATETIME)", "current_date");
            }

            if (sql.StartsWith("SELECT COUNT(*) FROM SYSCAT.TABLES"))
            {
                sql = NormalizeTablesSchema(sql);
            }

            if (sql.StartsWith("UPDATE GCPERSONAS PER"))
            {
                sql = PostgresHelper.RemoveAliasFromSet(sql, "PER");

                sql = PostgresHelper.ReplaceNVL(sql);
            }

            return sql;
        }

        /// <summary>
        /// Convierte valores 0/1 a False/True en INSERT statements para columnas que parecen booleanas
        /// basándose en el nombre de la columna (prefijos L, IS_, HAS_, ENABLE_, etc.)
        /// </summary>
        /// <param name="sql">SQL con INSERT statement</param>
        /// <returns>SQL con valores booleanos convertidos</returns>
        public string ConvertirBooleanosEnInsert(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            try
            {
                // Detectar la sección de columnas
                var regexColumnas = new Regex(
                    @"INSERT\s+INTO\s+[\w.]+\s*\((.*?)\)\s*VALUES\s*\((.*?)\)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline
                );

                var match = regexColumnas.Match(sql);
                if (!match.Success)
                    return sql;

                string columnasStr = match.Groups[1].Value;
                string valoresStr = match.Groups[2].Value;

                // Separar columnas y valores
                var columnas = columnasStr.Split(',')
                    .Select(c => c.Trim())
                    .ToList();

                var valores = SepararValores(valoresStr);

                if (columnas.Count != valores.Count)
                    return sql; // No coinciden, no modificar

                // Detectar columnas booleanas y convertir sus valores
                List<string> valoresConvertidos = new List<string>();
                for (int i = 0; i < columnas.Count; i++)
                {
                    string columna = columnas[i].ToUpper();
                    string valorOriginal = valores[i]; // Guardar el valor original con espacios
                    string valorLimpio = valorOriginal.Trim(); // Limpiar para comparar

                    // Detectar si la columna es booleana por su nombre
                    bool esColumnaBooleana =
                        columna.StartsWith("L");         // Ejemplo: LVISIBLETERMINAR

                    // Si es columna booleana y el valor (limpio) es 0 o 1, convertir
                    if (esColumnaBooleana && (valorLimpio == "0" || valorLimpio == "1"))
                    {
                        // Preservar espacios en blanco que pudieran existir
                        string nuevoValor = valorLimpio == "1" ? "True" : "False";

                        // Si el valor original tenía espacios al final, preservar la estructura
                        if (valorOriginal.EndsWith(" ") && !valorOriginal.Trim().Equals(valorOriginal))
                        {
                            // Mantener espacios después del valor si existían
                            int espaciosFinales = valorOriginal.Length - valorOriginal.TrimEnd().Length;
                            nuevoValor = nuevoValor + new string(' ', espaciosFinales);
                        }

                        valoresConvertidos.Add(nuevoValor);
                    }
                    else
                    {
                        valoresConvertidos.Add(valorOriginal);
                    }
                }

                // Reconstruir el SQL
                string nuevosValores = string.Join(",", valoresConvertidos);
                //string sqlModificado = sql.Replace(
                //    $"VALUES({valoresStr})",
                //    $"VALUES({nuevosValores})"
                //);
                string sqlModificado = SustitucionCambiosInsert(sql, match, nuevosValores);
                return sqlModificado;
            }
            catch (Exception)
            {
                // Si hay algún error, devolver el SQL original
                return sql;
            }
        }

        /// <summary>
        /// Separa los valores de un INSERT VALUES respetando comillas y paréntesis
        /// </summary>
        private List<string> SepararValores(string valoresStr)
        {
            List<string> valores = new List<string>();
            StringBuilder valorActual = new StringBuilder();
            bool dentroComillas = false;
            int nivelParentesis = 0;

            for (int i = 0; i < valoresStr.Length; i++)
            {
                char c = valoresStr[i];

                switch (c)
                {
                    case '\'':
                        valorActual.Append(c);
                        dentroComillas = !dentroComillas;
                        break;

                    case '(':
                        valorActual.Append(c);
                        if (!dentroComillas)
                            nivelParentesis++;
                        break;

                    case ')':
                        valorActual.Append(c);
                        if (!dentroComillas)
                            nivelParentesis--;
                        break;

                    case ',':
                        if (!dentroComillas && nivelParentesis == 0)
                        {
                            valores.Add(valorActual.ToString());
                            valorActual.Clear();
                        }
                        else
                        {
                            valorActual.Append(c);
                        }
                        break;

                    default:
                        valorActual.Append(c);
                        break;
                }
            }

            // Agregar el último valor
            if (valorActual.Length > 0)
            {
                valores.Add(valorActual.ToString());
            }

            return valores;
        }
        public string TablasTemporales(string sql)
        {
            string Salida = sql.ToUpper();
            string Muestra;
            string Tabla;
            int inicio = 0;

            Salida = Salida.Replace("DECLARE GLOBAL", "CREATE");
            Salida = Salida.Replace(" CHAR(", " CHARACTER(");

            //se trabaja con la parte de indices
            if (Salida.IndexOf("INDEX") > 0)
            {
                int i = Salida.IndexOf("TABLE");
                int indice = Salida.IndexOf("(");

                Tabla = Salida.Substring(i + 6, indice - i - 6).Trim(' ');

                int Indices = Salida.IndexOf(Tabla, indice);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, Tabla.Length + 10);

                    int x = Muestra.IndexOf("(");

                    if (x > 0)
                    {
                        Salida = Salida.Insert(Indices + Tabla.Length, " USING BTREE ");
                        Indices = Salida.IndexOf(Tabla, Indices + Tabla.Length);
                    }
                    else
                    {
                        Salida = Salida.Insert(Indices, "IDX_");
                        Indices = Salida.IndexOf(Tabla, Indices + Tabla.Length);
                    }

                }
                //Si ya viene con pg_temp. lo cambiamos a SESSION. y si viene con SESSION. lo cambiamos a pg_temp.
                //Salida = Salida.Replace("pg_temp.", "SESSION.").Replace("SESSION.", "pg_temp.");
                Salida = NormalizarSession(Salida);

                Indices = Salida.IndexOf("INDEX");

                while (Indices > 0)
                {
                    Salida = Salida.Insert(Indices + 5, " IF NOT EXISTS ");
                    Indices = Salida.IndexOf("INDEX", Indices + 5);
                }
            }

            //Se trabaja con la parte de booleanos
            if (Salida.IndexOf("CHARACTER(1)") > 0)
            {
                int Indices = Salida.IndexOf("CHARACTER(1)", 0);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT X'00',");

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        Muestra = Muestra.Replace("CHARACTER(1)", "BOOLEAN");
                        Muestra = Muestra.Replace("X'00'", "FALSE");
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHARACTER(1)", Indices + 12);
                }
            }

            if (Salida.IndexOf("CHARACTER(1)") > 0)
            {
                int Indices = Salida.IndexOf("CHARACTER(1)", 0);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT FALSE,");

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        Muestra = Muestra.Replace("CHARACTER(1)", "BOOLEAN");
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHARACTER(1)", Indices + 12);
                }
            }

            //Se trabaja con la parte de booleanos
            if (Salida.IndexOf("CHAR(1)") > 0)
            {
                int Indices = Salida.IndexOf("CHAR(1)", 0);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT X'00',");

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        Muestra = Muestra.Replace("CHAR(1)", "BOOLEAN");
                        Muestra = Muestra.Replace("X'00'", "FALSE");
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHAR(1)", Indices + 9);
                }
            }

            Salida = Salida.Replace("'00.00.00'", "'00:00:00'");
            Salida = Salida.Replace("INTEGER", "INT");
            Salida = Salida.Replace("DECIMAL", "NUMERIC");
            Salida = Salida.Replace("ON COMMIT PRESERVE ROWS NOT LOGGED WITH REPLACE", "");
            Salida = Salida.Replace("ON COMMIT PRESERVE ROWS NOT LOGGED", "");
            Salida = Salida.Replace("WITH", "");

            inicio = Salida.IndexOf("TABLE");

            Salida = Salida.Insert(inicio + 5, " IF NOT EXISTS ");

            return Salida;
        }

        public string GetTimestamp(string sql)
        {
            try
            {
                string Muestra;
                string tabla1, tabla2;
                string Sustitucion;
                int paren;

                List<int> Indices = AllIndexesOf(sql, "TIMESTAMP", 0);

                int j = 0;

                while (j < Indices.Count())
                {
                    paren = sql.IndexOf(")", Indices[j]);

                    Muestra = sql.Substring(Indices[j] + 10, paren - Indices[j] - 10);

                    if (!Muestra.Contains("("))
                    {
                        Muestra = Muestra.Trim();

                        int x = Muestra.IndexOf(",");

                        tabla1 = sql.Substring(Indices[j] + 10, x);

                        tabla2 = sql.Substring(Indices[j] + 11 + x, paren - Indices[j] - 11 - x);

                        Sustitucion = "(CAST(" + tabla1 + " AS TEXT) || ' '|| CAST(" + tabla2 + " AS TEXT)):: TIMESTAMP";

                        sql = sql.Remove(Indices[j], paren - Indices[j] + 1);

                        sql = sql.Insert(Indices[j], Sustitucion);

                        if (x > 0)
                        {
                            Indices = AllIndexesOf(sql, "TIMESTAMP", Indices[j] + Sustitucion.Length);
                            j = 0;
                        }
                        else
                        {
                            j++;
                        }
                    }
                    else
                    {
                        paren = sql.IndexOf(")", paren + 1);

                        Muestra = sql.Substring(Indices[j] + 10, paren - Indices[j] - 10);

                        Muestra = Muestra.Trim();

                        int x = Muestra.IndexOf(",");

                        tabla1 = sql.Substring(Indices[j] + 10, x);

                        tabla2 = sql.Substring(Indices[j] + 11 + x, paren - Indices[j] - 11 - x);

                        Sustitucion = "(CAST(" + tabla1 + " AS TEXT) || ' '|| CAST(" + tabla2 + " AS TEXT)):: TIMESTAMP";

                        sql = sql.Remove(Indices[j], paren - Indices[j] + 1);

                        sql = sql.Insert(Indices[j], Sustitucion);

                        if (x > 0)
                        {
                            Indices = AllIndexesOf(sql, "TIMESTAMP", Indices[j] + Sustitucion.Length);
                            j = 0;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }

                return sql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConvertAddDay(string sql)
        {
            string Muestra;
            string Numero;
            int Mas;

            List<int> Indices = AllIndexesOf(sql, " DAY", 0);

            int j = 0;

            while (j < Indices.Count())
            {
                Muestra = sql.Substring(Indices[j] - 8, 12);
                Mas = Muestra.IndexOf("+");

                if (Mas > 0)
                {
                    Numero = Muestra.Substring(Mas + 1, 7 - Mas).Trim();

                    if (int.TryParse(Numero, out int num))
                    {
                        sql = sql.Remove(Indices[j], 4);
                    }
                }
                else
                {
                    Indices = AllIndexesOf(sql, " DAY", Indices[j]);
                }

                j++;
            }

            return sql;
        }

        public string ProcesarConsultaBoletos(string sql)
        {
            sql = EliminarTrim(sql, ".NCLAVEALMACEN", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVEAREAVENTA", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVEAGENCIAINTERNA", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVEDEPARTAMENTO", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVEPERSONA", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVEAGENCIA", "TRIM");

            sql = EliminarTrim(sql, ".NTIPORESPONSABLE", "TRIM");

            sql = EliminarTrim(sql, ".NCLAVESUCURSALEXTERNA", "TRIM");

            sql = EliminarTrim(sql, ".LCAPTURAORIGENBOLMANUAL", "TRIM");

            sql = CambiarHex(sql, ".LCAPTURAORIGENBOLMANUAL");

            return sql;
        }

        public string EliminarTrim(string sql, string Cadena, string funcion)
        {
            string Muestra;
            string uno;
            int Mas, Pare;

            List<int> Indices = AllIndexesOf(sql, Cadena, 0);

            int j = 0;

            while (j < Indices.Count())
            {
                Muestra = sql.Substring(Indices[j] - 23, 42);
                Mas = Muestra.IndexOf(funcion);

                if (Mas > 0)
                {
                    Pare = Muestra.IndexOf("(", Mas);

                    uno = sql.Substring(Indices[j] - 23 + Mas, Pare - Mas + 1);

                    sql = sql.Remove(Indices[j] - 23 + Mas, Pare - Mas + 1);

                    Pare = sql.IndexOf(")", Indices[j]);

                    sql = sql.Remove(Pare, 1);

                    Indices = AllIndexesOf(sql, Cadena, Indices[j]);

                    j = 0;
                }
                else
                {
                    j++;
                }
            }

            return sql;
        }

        public string CambiarHex(string sql, string Cadena)
        {
            string Muestra;
            string uno;
            int Mas, Pare;

            List<int> Indices = AllIndexesOf(sql, Cadena, 0);

            int j = 0;

            while (j < Indices.Count())
            {
                Muestra = sql.Substring(Indices[j] - 15, 42);
                Mas = Muestra.IndexOf("HEX");

                if (Mas > 0)
                {
                    Pare = Muestra.IndexOf(")", Mas);

                    uno = sql.Substring(Indices[j] - 15 + Mas, Pare - Mas + 1);

                    sql = sql.Remove(Indices[j] - 15 + Mas, Pare - Mas + 1);

                    sql = sql.Insert(Indices[j] - 15 + Mas, " CASE WHEN TIPOB" + Cadena + " = false THEN '0' WHEN TIPOB" + Cadena + " = true THEN '1' ELSE '0' END ");

                    Indices = AllIndexesOf(sql, Cadena, Indices[j]);

                    j = 0;
                }
                else
                {
                    j++;
                }
            }

            return sql;
        }

        public List<int> AllIndexesOf(string cadena, string caracter, int inicio)
        {
            try
            {
                List<int> indexes = new List<int>();
                for (int index = inicio; ; index += caracter.Length)
                {
                    index = cadena.IndexOf(caracter, index);
                    if (index == -1)
                        return indexes;
                    indexes.Add(index);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string Atomic()
        {
            return @"BEGIN;
                    DELETE FROM AUXILIAR_SECUENCIA;
                    INSERT INTO AUXILIAR_SECUENCIA VALUES (NEXTVAL (':1'));
                    COMMIT;";
        }

        public string Eliminar(string sql, string Cadena)
        {
            Cadena = Cadena.ToUpper();
            int lognth = Cadena.Length;

            List<int> Indices = AllIndexesOf(sql, Cadena, 0);

            int j = 0;

            while (j < Indices.Count())
            {
                sql = sql.Remove(Indices[j], lognth);

                Indices = AllIndexesOf(sql, Cadena, Indices[j]);

                j = 0;
            }

            return sql;
        }
        public string Reemplazar(string cadenaOriginal, string valorbuscar, string valorReemplazo)
        {
            if (string.IsNullOrEmpty(cadenaOriginal))
                return cadenaOriginal;

            return cadenaOriginal.ToLower().Replace(valorbuscar.ToLower(), valorReemplazo ?? string.Empty);
        }
        public string NormalizarSession(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Parte por sentencias (conserva ;)
            var stmts = Regex.Split(sql, @"(?<=;)");

            for (int i = 0; i < stmts.Length; i++)
            {
                var s = stmts[i];
                var sUpper = s.ToUpperInvariant();

                bool esCreateIndex = Regex.IsMatch(
                    sUpper,
                    @"^\s*CREATE\s+(UNIQUE\s+)?INDEX\b",
                    RegexOptions.Singleline);

                if (esCreateIndex)
                {
                    // En CREATE INDEX: quitar SESSION./pg_temp.
                    s = Regex.Replace(s, @"\b(?:SESSION|PG_TEMP)\.", "", RegexOptions.IgnoreCase);
                }
                else
                {
                    // En cualquier otra sentencia: SESSION. -> pg_temp.
                    s = Regex.Replace(s, @"\bSESSION\.", "pg_temp.", RegexOptions.IgnoreCase);
                }

                stmts[i] = s;
            }

            return string.Concat(stmts);
        }

        public string NormalizeCurrentDateTime(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // current date  -> current_date
            sql = Regex.Replace(
                sql,
                @"\bcurrent\s+date\b",
                "current_date",
                RegexOptions.IgnoreCase);

            // current time  -> current_time
            sql = Regex.Replace(
                sql,
                @"\bcurrent\s+time\b",
                "current_time",
                RegexOptions.IgnoreCase);

            return sql;
        }
        public string ConvertDays(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return sql;
            Regex reg = new Regex(@"(?<signo>[+-])\s*(?<cantidad>\d+)\s+DAYS?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //{0} DAYS -> INTERVAL '{0} DAYS'
            sql = reg.Replace(sql, match =>
            {
                string signo = match.Groups["signo"].Value;
                string candidad = match.Groups["cantidad"].Value;
                return $"{signo} INTERVAL '{candidad} DAYS'";
            });

            return sql;
        }
        public string CambiarDAYOFWEEK(string sql)
        {
            string Parametro;
            string AS;
            int Fin;
            int Date;
            int parametro1, parametro2;

            List<int> Indices = AllIndexesOf(sql, "DAYOFWEEK", 0);

            int j = 0;

            while (j < Indices.Count())
            {
                parametro1 = sql.IndexOf(":", Indices[j]);

                parametro2 = sql.IndexOf("'", parametro1);

                Parametro = sql.Substring(parametro1 - 1, parametro2 - parametro1 + 2);

                Date = sql.IndexOf("DATE", parametro2);

                parametro1 = sql.IndexOf("AS", Date);

                parametro2 = sql.IndexOf("FROM", parametro1);

                AS = sql.Substring(parametro1 + 2, parametro2 - parametro1 - 2).Trim();

                Fin = sql.IndexOf(".SYSDUMMY1", Indices[j]) + 10;

                sql = sql.Remove(Indices[j], Fin - Indices[j]);

                sql = sql.Insert(Indices[j], " extract(dow from date " + Parametro + ") + 1 AS " + AS);

                Indices = AllIndexesOf(sql, "DAYOFWEEK", 0);

                j = 0;

            }

            return sql;
        }

        public string CambiarBool(string sql)
        {
            if (sql.IndexOf("x'01'") > 0)
            {
                sql = sql.Replace("x'01'", "True");
            }

            if (sql.IndexOf("x'00'") > 0)
            {
                sql = sql.Replace("x'00'", "False");
            }

            if (sql.IndexOf("X'01'") > 0)
            {
                sql = sql.Replace("X'01'", "True");
            }

            if (sql.IndexOf("X'00'") > 0)
            {
                sql = sql.Replace("X'00'", "False");
            }

            return sql;
        }

        /// <summary>
        /// Cambiar formato de hora HH.mm.ss(DB2) a HH:mm:ss(Postgres)
        /// Solo convierte valores que sean claramente horas (00-23 para horas)
        /// y solo si la columna correspondiente comienza con H en un INSERT
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string ChangeHourFormat(string sql)
        {

            // Si NO es un INSERT, aplicar conversión normal (comportamiento anterior)
            if (!sql.ToUpper().Contains("INSERT INTO"))
            {
                // Patrón 1: Formato completo dentro de comillas '18.51.30' -> '18:51:30'
                Regex regHourFull = new Regex(
                    @"'([01]\d|2[0-3])\.([0-5]\d)\.([0-5]\d)'",
                    RegexOptions.Compiled
                );
                sql = regHourFull.Replace(sql, "'$1:$2:$3'");

                // Patrón 2: Formato corto dentro de comillas '18.51' -> '18:51:00'
                Regex regHourShort = new Regex(
                    @"'([01]\d|2[0-3])\.([0-5]\d)'",
                    RegexOptions.Compiled
                );
                sql = regHourShort.Replace(sql, "'$1:$2:00'");

                return sql;
            }

            // Si ES un INSERT, validar columnas que empiecen con H
            try
            {
                // Detectar la sección de columnas y valores
                var regexInsert = new Regex(
                    @"INSERT\s+INTO\s+[\w.]+\s*\((.*?)\)\s*VALUES\s*\((.*?)\)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline
                );

                var match = regexInsert.Match(sql);
                if (!match.Success)
                    return sql; // No se puede parsear, devolver sin cambios

                string columnasStr = match.Groups[1].Value;
                string valoresStr = match.Groups[2].Value;

                // Separar columnas
                var columnas = columnasStr.Split(',')
                    .Select(c => c.Trim().ToUpper())
                    .ToList();

                // Separar valores
                var valores = SepararValores(valoresStr);

                if (columnas.Count != valores.Count)
                    return sql; // No coinciden, no modificar

                // Convertir valores de hora solo para columnas que empiecen con H
                List<string> valoresConvertidos = new List<string>();
                for (int i = 0; i < columnas.Count; i++)
                {
                    string columna = columnas[i];
                    string valorOriginal = valores[i];
                    string valorConvertido = valorOriginal;

                    // Solo convertir si la columna empieza con H
                    if (columna.StartsWith("H"))
                    {
                        // Aplicar conversión de formato de hora
                        // Patrón 1: '18.51.30' -> '18:51:30'
                        Regex regHourFull = new Regex(
                            @"'([01]\d|2[0-3])\.([0-5]\d)\.([0-5]\d)'",
                            RegexOptions.Compiled
                        );
                        valorConvertido = regHourFull.Replace(valorConvertido, "'$1:$2:$3'");

                        // Patrón 2: '18.51' -> '18:51:00'
                        Regex regHourShort = new Regex(
                            @"'([01]\d|2[0-3])\.([0-5]\d)'",
                            RegexOptions.Compiled
                        );
                        valorConvertido = regHourShort.Replace(valorConvertido, "'$1:$2:00'");
                    }

                    valoresConvertidos.Add(valorConvertido);
                }

                // Reconstruir el SQL solo si hubo cambios
                string nuevosValores = string.Join(",", valoresConvertidos);
                if (nuevosValores != valoresStr)
                {
                    sql=SustitucionCambiosInsert(sql, match, nuevosValores);
                }

                return sql;
            }
            catch (Exception)
            {
                // Si hay algún error en el parsing, devolver SQL original sin cambios
                return sql;
            }
        }

        public string SustitucionCambiosInsert(string sql,Match match,string nuevosValores)
        {
            try
            {
                // Encontrar "VALUES" y luego el primer "(" después de él
                int posValues = sql.IndexOf("VALUES", match.Index, StringComparison.OrdinalIgnoreCase);
                if (posValues >= 0)
                {
                    // Buscar el paréntesis de apertura después de VALUES
                    int posParenApertura = sql.IndexOf('(', posValues);
                    if (posParenApertura >= 0)
                    {
                        // Inicio = posición después del (
                        int inicioValues = posParenApertura + 1;

                        // Fin = posición del ) de cierre (desde el match)
                        int finValues = match.Index + match.Length - 1;

                        // Reconstruir: parte antes de ( + nuevosValores + ) + resto
                        sql = sql.Substring(0, inicioValues) + nuevosValores + sql.Substring(finValues);
                    }
                }
                return sql;
            }
            catch(Exception)
            {
                return sql;
            }
        }

        public string CambiarBoolEspacio(string sql)
        {
            if (sql.IndexOf("x'01 '") > 0)
            {
                sql = sql.Replace("x'01 '", "True");
            }

            if (sql.IndexOf("x'00 '") > 0)
            {
                sql = sql.Replace("x'00 '", "False");
            }

            if (sql.IndexOf("X'01 '") > 0)
            {
                sql = sql.Replace("X'01 '", "True");
            }

            if (sql.IndexOf("X'00 '") > 0)
            {
                sql = sql.Replace("X'00 '", "False");
            }
            // Convertir valores booleanos en INSERT basándose en nombres de columnas
            if (sql.ToUpper().Contains("INSERT INTO") && sql.ToUpper().Contains("VALUES"))
            {
                sql = ConvertirBooleanosEnInsert(sql);
            }
            return sql;
        }

        public string CambiarFORFETCH(string sql)
        {
            int indice;

            indice = sql.IndexOf("FOR", 0);

            sql = sql.Remove(indice, sql.Length - indice);

            return sql;
        }

        public string Stored(string sql)
        {
            int parametro1;
            int j = 0;
            int indice;

            //SP por probar
            if (sql.Contains("SP_MASTARIFA"))
            {
                parametro1 = sql.IndexOf(",", 0);

                sql = sql.Insert(parametro1, "::SMALLINT");

                indice = sql.IndexOf("?", 0);

                while (indice > 0)
                {
                    sql = sql.Remove(indice, 1);

                    sql = sql.Insert(indice, "NULL");

                    indice = sql.IndexOf("?", 0);
                }

                return sql;
            }

            else if (sql.Contains("SPRECCORPLANEACION"))
            {
                parametro1 = sql.IndexOf(",", 0);

                sql = sql.Insert(parametro1, "::SMALLINT");

                List<int> Indices = AllIndexesOf(sql, ",", 0);

                sql = sql.Insert(sql.Length - 1, "::INTEGER");

                return sql;
            }

            else
            {
                //PROC_VERIFICAAPARTADOS1INFO, SP_OBTIENEPROMOCIONES, SP_CONSULTACORRIDASMULTI, SP_CORRIDASOPTV1, PROC_VERIFICAAPARTADOS1, SP_RECCORRID170714, SP_SECCIERRASESIONCB
                sql = ProcedureGenerator.BuildStoreProcedureSql(sql);

                return sql;
            }
        }

        public string locate(string sql)
        {
            int parametro1;
            int j = 0;
            int indice;

            indice = sql.IndexOf("LOCATE", 0);

            while (indice > 0)
            {
                parametro1 = sql.IndexOf(",", indice);

                sql = sql.Remove(parametro1, 1);

                sql = sql.Insert(parametro1, " IN");

                sql = sql.Remove(indice, 6);

                sql = sql.Insert(indice, "POSITION");

                indice = sql.IndexOf("LOCATE", 0);
            }

            indice = sql.IndexOf("OCTETS", 0);

            while (indice > 0)
            {
                sql = sql.Remove(indice - 1, 7);

                indice = sql.IndexOf("OCTETS", 0);
            }

            return sql;
        }

        public string Second(string sql)
        {

            int indice, mas;
            string cantidad;

            indice = sql.IndexOf("SECOND", 0);

            while (indice > 0)
            {
                mas = sql.IndexOf('+', indice - 5);

                cantidad = sql.Substring(mas + 1, indice - mas - 1);

                sql = sql.Remove(mas + 1, indice - mas + 5);

                sql = sql.Insert(mas + 1, " interval '" + cantidad + " SECOND' ");

                indice = sql.IndexOf("SECOND", mas + cantidad.Length + 18);
            }

            return sql;
        }

        public string Remplazar(string sql, string original, string remplazo)
        {
            int indice;

            indice = sql.IndexOf(original, 0);

            while (indice > 0)
            {
                sql = sql.Remove(indice, original.Length);

                sql = sql.Insert(indice, remplazo);

                indice = sql.IndexOf(original, indice);
            }

            return sql;
        }

        public string Information_schema(string sql)
        {

            int indice;

            string tabla;

            indice = sql.IndexOf("=", 0);

            tabla = sql.Substring(indice + 1, sql.Length - indice - 1).Trim();

            sql = "SELECT table_name AS NAME FROM information_schema.tables where table_name = lower(" + tabla + ")";

            return sql;
        }

        public string CambiarChar(string sql)
        {

            int indice, parentesis;

            indice = sql.IndexOf("(CHAR(", 0);

            while (indice > 0)
            {
                sql = sql.Remove(indice + 1, 4);

                sql = sql.Insert(indice + 1, "CAST");

                parentesis = sql.IndexOf(')', indice + 1);

                sql = sql.Insert(parentesis, " AS CHAR");

                indice = sql.IndexOf("(CHAR(", 0);
            }

            return sql;
        }
        public string MDYToDMY(string input)
        {
            try
            {             
                          return Regex.Replace(input,
                       @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
                      "${year}-${month}-${day}", RegexOptions.None,
                      TimeSpan.FromMilliseconds(150));
            }
            catch (RegexMatchTimeoutException)
            {
                return input;
            }
        }

        /// <summary>
        /// Convertir consulta de catalogo de tablas DB2 a su equivalente de Postgres
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string NormalizeTablesSchema(string sql)
        {
            var dic = new Dictionary<string, string>
            {
                { @"\bSYSCAT\.TABLES\b", "information_schema.tables" },
                { @"\bTABSCHEMA\b", "table_schema" },
                { @"\bTABNAME\b", "table_name" }
            };

            foreach (var kvp in dic)
            {
                sql = Regex.Replace(sql, kvp.Key, kvp.Value, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            return PostgresHelper.ValuesToLower(sql);
        }

        
    }
}
