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
            // Identificar si es creación de tabla (temporal o nativa)
            bool isCreateTable = Regex.IsMatch(sql, @"\bGLOBAL\s+TEMPORARY\b", RegexOptions.IgnoreCase) ||
                                 Regex.IsMatch(sql, @"^\s*CREATE\s+(TEMPORARY\s+)?TABLE\b", RegexOptions.IgnoreCase);

            if (Regex.IsMatch(sql, @"\bGLOBAL\s+TEMPORARY\b", RegexOptions.IgnoreCase))
            {
                sql = TablasTemporales(sql);
            }

            //if (sql.IndexOf("Ñ") > 0 || sql.IndexOf("ñ") > 0)
            if (Regex.IsMatch(sql, "Ñ", RegexOptions.IgnoreCase))
            {
                //Se modifica para hacerlo más general ya que con dos palabras con Ñ o ñ no se estaba reemplazando correctamente
                // Expresión regular: busca palabras con Ñ o ñ
                string patron = @"\b\w*ñ\w*|\b\w*Ñ\w*";

                sql = Regex.Replace(sql, patron, m =>
                {
                    // Convierte a mayúsculas y coloca entre comillas
                    return "\"" + m.Value.ToUpper() + "\"";
                });
            }

            sql = CambiarBool(sql);

            sql = ChangeHourFormat(sql);

            sql = NormalizeCurrentDateTime(sql);

            sql = PostgresHelper.ReplaceConcatOperator(sql);

			sql = PostgresHelper.ReplaceNVL(sql);

            sql = PostgresHelper.ReplaceValueFunction(sql);

			//Reemplazar DATE(CURRENT TIMESTAMP) por CURRENT_DATE
			sql = Regex.Replace(sql, @"\bDATE\s*\(\s*CURRENT\s+TIMESTAMP\s*\)", "CURRENT_DATE", RegexOptions.IgnoreCase);

			if (Regex.IsMatch(sql, @"\bTIMESTAMPDIFF\s*\(", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertTimestampDiff(sql);
			}

            if (Regex.IsMatch(sql, @"CURRENT\s+(TIMESTAMP|TIME|DATE)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                sql = NormalizeCurrentDateTime(sql);
            }

            if (Regex.IsMatch(sql, @"(?!AS\s)\bDATE\s*\(", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertDateInteger(sql);
            }

            //Detectar SQL con TIMESTAMP(arg1, arg2)
			if (Regex.IsMatch(sql, @"\bTIMESTAMP\s*\(\s*([^,]+)\s*,\s*([^)]+)\s*\)", RegexOptions.IgnoreCase))
			{
				sql = PostgresHelper.ConvertTimestampWithTwoArguments(sql);
			}

			if (Regex.IsMatch(sql, @"\bDATE\s*\(", RegexOptions.IgnoreCase))
			{
				sql = ConvertirFuncionDate(sql);
			}

			if (Regex.IsMatch(sql, @"\bTIMESTAMP\b", RegexOptions.IgnoreCase))
			{
				sql = GetTimestamp(sql);
			}

            sql = ConvertirIntervalos(sql);

            if (Regex.IsMatch(sql, @"\b(HOUR|MINUTE|MONTH|DAY|YEAR)\s*\(", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertToExtractFunction(sql);
            }

            if (!isCreateTable && !Regex.IsMatch(sql, @"\bAS\s+CHAR\s*\(", RegexOptions.IgnoreCase))
                sql = PostgresHelper.ReplaceCharFunction(sql); 
            
            sql = PostgresHelper.ReplaceCharTimeFormat(sql);

            if (Regex.IsMatch(sql, @"\bDECODE\s*\(", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertDecodeToCase(sql);
            }

            if (Regex.IsMatch(sql, @"\bCAST\s*\((.*?)\s+AS\s+BLOB\s*\)", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertBlobCast(sql);
            }

            if (Regex.IsMatch(sql, " DAY", RegexOptions.IgnoreCase))
            {
                sql = ConvertAddDay(sql);
            }

            if (Regex.IsMatch(sql, @"LEFT\s+JOIN\s+CBTIPOSRESPBOLETOS\s+AS\s+CTB0", RegexOptions.IgnoreCase))
            {               
                sql = ProcesarConsultaBoletos(sql);
            }

            // if (sql.IndexOf("INSERT INTO  SESSION.AUXILIAR_SECUENCIA VALUES") > 0)
            if (Regex.IsMatch(sql, @"INSERT\s+INTO\s+SESSION\.AUXILIAR_SECUENCIA\s+VALUES", RegexOptions.IgnoreCase))            
            {
                sql = Atomic();
            }

            // if (sql.IndexOf("DAYOFWEEK") > 0)
            if (Regex.IsMatch(sql, "DAYOFWEEK", RegexOptions.IgnoreCase))
            {
                sql = CambiarDAYOFWEEK(sql);
            }

            // if (sql.IndexOf("FOR FETCH ONLY") > 0)
            if (Regex.IsMatch(sql, @"FOR\s+FETCH\s+ONLY", RegexOptions.IgnoreCase))
            {
                sql = CambiarFORFETCH(sql);
            }

            // if (sql.StartsWith("CALL "))
            if (sql.StartsWith("CALL ", StringComparison.OrdinalIgnoreCase))
            {
                sql = Stored(sql);
            }

            //if (sql.Contains("LOCATE") && sql.Contains("OCTETS") && sql.Contains("WITH"))
            if (Regex.IsMatch(sql, @"LOCATE", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(sql, @"OCTETS", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(sql, @"WITH", RegexOptions.IgnoreCase))
            {
                sql = locate(sql);
            }

            // if (sql.Contains("LISTAGG"))
            if (Regex.IsMatch(sql, @"LISTAGG", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ReplaceListAgg(sql, true);
            }

            // if (sql.Contains("VALUE("))
            if (Regex.IsMatch(sql, @"VALUE\s*\(", RegexOptions.IgnoreCase))            
            {
                sql = Remplazar(sql, "VALUE(", "COALESCE(");
            }

            //if (sql.Contains(")CHAR("))
            if (Regex.IsMatch(sql, @"\)\s*CHAR\s*\(", RegexOptions.IgnoreCase))
            {
                sql = CambiarChar(sql);
            }

            // if (sql.Contains("SET PT.HTIEMPOESTANCIA = PT.HTIEMPOESTANCIA + 1 SECOND "))
            if (Regex.IsMatch(sql, @"SET\s+PT\.HTIEMPOESTANCIA\s*=\s*PT\.HTIEMPOESTANCIA\s*\+\s*1\s+SECOND", RegexOptions.IgnoreCase))
            {
                sql = Remplazar(sql, "SET PT.HTIEMPOESTANCIA = PT.HTIEMPOESTANCIA + 1 SECOND ", "SET HTIEMPOESTANCIA = HTIEMPOESTANCIA + interval ' 1  SECOND' ");
            }

            // if (sql.Contains("CAST (CONVERT (CHAR, GETDATE(), 112) AS DATETIME)"))
            if (Regex.IsMatch(sql, @"CAST\s*\(\s*CONVERT\s*\(\s*CHAR\s*,\s*GETDATE\(\)\s*,\s*112\s*\)\s*AS\s*DATETIME\s*\)", RegexOptions.IgnoreCase))
            {
                sql = Remplazar(sql, "CAST (CONVERT (CHAR, GETDATE(), 112) AS DATETIME)", "current_date");
            }

            //if (sql.StartsWith("SELECT COUNT(*) FROM SYSCAT.TABLES"))
            if (Regex.IsMatch(sql, @"^SELECT\s+COUNT\(\*\)\s+FROM\s+SYSCAT\.TABLES", RegexOptions.IgnoreCase))
            {
                sql = NormalizeTablesSchema(sql);
            }

            //if (sql.StartsWith("UPDATE"))
            if (Regex.IsMatch(sql, @"UPDATE", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.RemoveAliasFromSet(sql);
            }

            // Normalizar DELETE sin FROM: "DELETE TABLA WHERE ..." -> "DELETE FROM TABLA WHERE ..."
            // Esto permite que deletes como "DELETE PADATOSLIBERACION WHERE NIDGENERALDA = 99685" sean analizados correctamente
            if (Regex.IsMatch(sql, @"^\s*DELETE\s+[\w.]+(\s+WHERE\b|\s*;|\s*$)", RegexOptions.IgnoreCase))
            {
                sql = Regex.Replace(sql, @"^\s*DELETE\s+([\w.]+)", "DELETE FROM $1", RegexOptions.IgnoreCase);
            }

            if (sql.IndexOf("NEXTVAL", StringComparison.OrdinalIgnoreCase) > 0)
            {
                sql = ConvertirNextVal(sql);
            }
            if (Regex.IsMatch(sql, @"VARCHAR_FORMAT", RegexOptions.IgnoreCase))
            {
                sql = Remplazar(sql, "VARCHAR_FORMAT", "TO_CHAR");
            }

            if (Regex.IsMatch(sql,@"SUBSTR", RegexOptions.IgnoreCase) && Regex.IsMatch(sql, @"FCRFECHA", RegexOptions.IgnoreCase))
            {
                sql = ConvertirSubstrFecha(sql);
            }

            if (Regex.IsMatch(sql, @"SUBSTR", RegexOptions.IgnoreCase) && Regex.IsMatch(sql, @"HORA", RegexOptions.IgnoreCase))
            {
                sql = ConvertirSubstrHora(sql);
            }

            if ((Regex.IsMatch(sql, @"PER.TIPOOPERADOR", RegexOptions.IgnoreCase) && Regex.IsMatch(sql, @"PTO.NCLAVETIPO", RegexOptions.IgnoreCase))||
               ((Regex.IsMatch(sql, @"GCP.TIPOOPERADOR", RegexOptions.IgnoreCase) && Regex.IsMatch(sql, @"PST.NCLAVETIPO", RegexOptions.IgnoreCase))
               ))
            {
                sql = ConvertirComparacionTipoOperador(sql);
            }

            if (Regex.IsMatch(sql, @"TIME\(", RegexOptions.IgnoreCase))
            {
                sql = ConvertirFuncionTime(sql);
            }

            if (Regex.IsMatch(sql, "xmlagg", RegexOptions.IgnoreCase))
            {
                sql = ConvertirXmlAgg(sql);
            }

            if (Regex.IsMatch(sql, @"\bMONTH\s*\(", RegexOptions.IgnoreCase))
            {
                sql = ConvertirFuncionMonth(sql);
            }
            if (Regex.IsMatch(sql, @"SELECT\s+\w+\s+FROM\s+FINAL\s+TABLE\s*\(\s*INSERT", RegexOptions.IgnoreCase))
            {
                sql = PostgresHelper.ConvertirTodasLasFinalTable(sql);
            }

            //Se buscan las funciones hex() para convertirlas a la sintaxis de PostgreSQL
            if (Regex.IsMatch(sql, @"hex\s*\(\s*(?<campo>[a-zA-Z0-9_.]+)\s*\)", RegexOptions.IgnoreCase))
            {
                sql = CambiarHex(sql);
            }

            sql = EliminaWithUr(sql);

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

                var valores =  SepararValores(valoresStr);

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
                    bool esColumnaBooleana = columna.StartsWith("L", StringComparison.OrdinalIgnoreCase);

                   // bool esColumnaBooleana =  columna.StartsWith("L");         // Ejemplo: LVISIBLETERMINAR

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
            string Salida = sql;
            string Muestra;
            string Tabla;
            int inicio = 0;

            Salida = Regex.Replace(Salida, "DECLARE GLOBAL", "CREATE", RegexOptions.IgnoreCase);
            Salida = Regex.Replace(Salida, @"\sCHAR\(", " CHARACTER(", RegexOptions.IgnoreCase);

            //Salida = Salida.Replace("DECLARE GLOBAL", "CREATE");
            //Salida = Salida.Replace(" CHAR(", " CHARACTER(");

            //se trabaja con la parte de indices
            if (Regex.IsMatch(Salida, @"INDEX", RegexOptions.IgnoreCase))
            //if (Salida.IndexOf("INDEX") > 0)
            {
                int i = Salida.IndexOf("TABLE", StringComparison.OrdinalIgnoreCase);
                int indice = Salida.IndexOf("(");

                Tabla = Salida.Substring(i + 6, indice - i - 6).Trim(' ');

                int Indices = Salida.IndexOf(Tabla, indice, StringComparison.OrdinalIgnoreCase);

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

                Indices = Salida.IndexOf("INDEX", StringComparison.OrdinalIgnoreCase);

                while (Indices > 0)
                {
                    Salida = Salida.Insert(Indices + 5, " IF NOT EXISTS ");
                    Indices = Salida.IndexOf("INDEX", Indices + 5, StringComparison.OrdinalIgnoreCase);
                }
            }

            //Se trabaja con la parte de booleanos
            if (Regex.IsMatch(Salida, @"CHARACTER\(1\)", RegexOptions.IgnoreCase))
            //if (Salida.IndexOf("CHARACTER(1)") > 0) 
            {
                int Indices = Salida.IndexOf("CHARACTER(1)", 0, StringComparison.OrdinalIgnoreCase);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT X'00',", StringComparison.OrdinalIgnoreCase);

                    if (j < 1)
                    {
                        j = Muestra.IndexOf("DEFAULT X'00'\r\n", StringComparison.OrdinalIgnoreCase);
                    }

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        Muestra = Regex.Replace(Muestra, @"CHARACTER\(1\)", "BOOLEAN", RegexOptions.IgnoreCase);
                        //Muestra = Muestra.Replace("CHARACTER(1)", "BOOLEAN");
                        Muestra = Regex.Replace(Muestra, "X'00'", "FALSE", RegexOptions.IgnoreCase);
                        //Muestra = Muestra.Replace("X'00'", "FALSE");
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHARACTER(1)", Indices + 12, StringComparison.OrdinalIgnoreCase);
                }
            }

            if (Regex.IsMatch(sql, @"CHARACTER\(1\)", RegexOptions.IgnoreCase))
            {
                int Indices = Salida.IndexOf("CHARACTER(1)", 0, StringComparison.OrdinalIgnoreCase);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT FALSE,", StringComparison.OrdinalIgnoreCase);

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        Salida = Regex.Replace(Salida, @"CHARACTER\(1\)", "BOOLEAN", RegexOptions.IgnoreCase);
                        //Muestra = Muestra.Replace("CHARACTER(1)", "BOOLEAN");                        
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHARACTER(1)", Indices + 12, StringComparison.OrdinalIgnoreCase);
                }
            }

            //Se trabaja con la parte de booleanos
            if (Regex.IsMatch(sql, @"CHAR\(1\)", RegexOptions.IgnoreCase))
            // if (Salida.IndexOf("CHAR(1)") > 0)
            {
                int Indices = Salida.IndexOf("CHAR(1)", 0, StringComparison.OrdinalIgnoreCase);

                while (Indices > 0)
                {
                    Muestra = Salida.Substring(Indices, 70);

                    int j = Muestra.IndexOf("DEFAULT X'00',", StringComparison.OrdinalIgnoreCase);

                    if (j > 0)
                    {
                        Muestra = Muestra.Substring(0, j + 14);
                        //Muestra = Muestra.Replace("CHAR(1)", "BOOLEAN");
                        Salida = Regex.Replace(Salida, "CHAR\\(1\\)", "BOOLEAN", RegexOptions.IgnoreCase);
                        //Muestra = Muestra.Replace("X'00'", "FALSE");
                        Salida = Regex.Replace(Salida, "X'00'", "FALSE", RegexOptions.IgnoreCase);
                        Salida = Salida.Remove(Indices, j + 14);
                        Salida = Salida.Insert(Indices, Muestra);
                    }
                    Indices = Salida.IndexOf("CHAR(1)", Indices + 9, StringComparison.OrdinalIgnoreCase);
                }
            }

            Salida = Salida.Replace("'00.00.00'", "'00:00:00'");
            Salida = Regex.Replace(Salida, "INTEGER", "INT", RegexOptions.IgnoreCase);
            Salida = Regex.Replace(Salida, "DECIMAL", "NUMERIC", RegexOptions.IgnoreCase);
            Salida = Regex.Replace(Salida, @"\bON COMMIT PRESERVE ROWS NOT LOGGED WITH REPLACE\b", "", RegexOptions.IgnoreCase);
            Salida = Regex.Replace(Salida, @"\bON COMMIT PRESERVE ROWS NOT LOGGED\b", "", RegexOptions.IgnoreCase);
            Salida = Regex.Replace(Salida, "WITH", "", RegexOptions.IgnoreCase);
            //Salida = Salida.Replace("INTEGER", "INT");
            //Salida = Salida.Replace("DECIMAL", "NUMERIC");            
            //Salida = Salida.Replace("ON COMMIT PRESERVE ROWS NOT LOGGED WITH REPLACE", "");
            //Salida = Salida.Replace("ON COMMIT PRESERVE ROWS NOT LOGGED", "");
            //Salida = Salida.Replace("WITH", "");

            inicio = Salida.IndexOf("TABLE", StringComparison.OrdinalIgnoreCase);

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
                    // VALIDAR: Verificar si ya existe un CAST antes de TIMESTAMP
                    if (YaTieneCast(sql, Indices[j]))
                    {
                        j++; // Saltar esta ocurrencia, ya tiene CAST
                        continue;
                    }

                    paren = sql.IndexOf(")", Indices[j]);

                    // VALIDAR: Verificar que no sea una función de PostgreSQL como statement_timestamp()
                    if (EsFuncionPostgresTimestamp(sql, Indices[j]))
                    {
                        j++; // Saltar funciones de PostgreSQL
                        continue;
                    }

                    Muestra = sql.Substring(Indices[j] + 10, paren - Indices[j] - 10);

                    if (!Muestra.Contains("("))
                    {
                        Muestra = Muestra.Trim();

                        int x = Muestra.IndexOf(",");

                        // VALIDAR: Si no hay coma, no es el formato DB2 TIMESTAMP(fecha, hora)
                        if (x < 0)
                        {
                            j++;
                            continue;
                        }

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

                        // VALIDAR: Si no hay coma, no es el formato DB2 TIMESTAMP(fecha, hora)
                        if (x < 0)
                        {
                            j++;
                            continue;
                        }

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
        /// <summary>
        /// Verifica si ya existe un CAST antes de la posición de TIMESTAMP
        /// Ejemplo: CAST(statement_timestamp()::TIMESTAMP AS TEXT) -> retorna true
        /// </summary>
        /// <param name="sql">SQL completo</param>
        /// <param name="timestampIndex">Índice donde se encontró TIMESTAMP</param>
        /// <returns>True si ya tiene CAST, False si no</returns>
        private bool YaTieneCast(string sql, int timestampIndex)
        {
            // Buscar hacia atrás desde TIMESTAMP para encontrar un CAST(
            int buscarHasta = Math.Max(0, timestampIndex - 100); // Buscar hasta 100 caracteres atrás
            string fragmento = sql.Substring(buscarHasta, timestampIndex - buscarHasta);

            // Patrón: CAST( ... ::TIMESTAMP o CAST( ... AS TIMESTAMP
            var regexCast = new Regex(
                @"CAST\s*\([^)]*$",
                RegexOptions.IgnoreCase);

            return regexCast.IsMatch(fragmento);
        }

        /// <summary>
        /// Verifica si TIMESTAMP es parte de una función de PostgreSQL
        /// Ejemplos: statement_timestamp(), current_timestamp, transaction_timestamp()
        /// </summary>
        /// <param name="sql">SQL completo</param>
        /// <param name="timestampIndex">Índice donde se encontró TIMESTAMP</param>
        /// <returns>True si es una función de PostgreSQL, False si no</returns>
        private bool EsFuncionPostgresTimestamp(string sql, int timestampIndex)
        {
            // Buscar hacia atrás para ver si hay un nombre de función antes de TIMESTAMP
            int buscarDesde = Math.Max(0, timestampIndex - 30);
            string fragmento = sql.Substring(buscarDesde, timestampIndex - buscarDesde);

            // Funciones de PostgreSQL que terminan con _timestamp o contienen current_timestamp
            var funcionesPostgres = new[]
            {
        "statement_timestamp",
        "transaction_timestamp",
        "clock_timestamp",
        "current_timestamp",
        "timeofday_timestamp"
    };

            foreach (var funcion in funcionesPostgres)
            {
                if (fragmento.IndexOf(funcion, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            // Verificar patrón ::TIMESTAMP (cast de PostgreSQL)
            if (fragmento.TrimEnd().EndsWith("::"))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Convierte SUBSTR con columnas de fecha para agregar el cast ::text necesario en PostgreSQL
        /// Patrón: SUBSTR(TABLA.COLUMNA,pos,len) -> SUBSTR(TABLA.COLUMNA::text,pos,len)
        /// donde COLUMNA termina con 'FECHA'
        /// </summary>
        /// <param name="sql">SQL a convertir</param>
        /// <returns>SQL con las conversiones aplicadas</returns>
        public string ConvertirSubstrFecha(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Patrón que captura: SUBSTR(ALIAS.COLUMNA_FECHA, número, número)
            // donde COLUMNA termina con FECHA (case-insensitive)
            string pattern = @"SUBSTR\s*\(\s*(?<tabla>\w+)\.(?<columna>\w*FECHA)\s*,";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            sql = regex.Replace(sql, match =>
            {
                string tabla = match.Groups["tabla"].Value;
                string columna = match.Groups["columna"].Value;

                // Retorna con el cast ::text agregado
                return $"SUBSTR({tabla}.{columna}::text,";
            });

            return sql;
        }

        /// <summary>
        /// Convierte SUBSTR con columnas de hora para agregar el cast ::text necesario en PostgreSQL
        /// Patrón: SUBSTR(TABLA.COLUMNA,pos,len) -> SUBSTR(TABLA.COLUMNA::text,pos,len)
        /// donde COLUMNA contiene 'HORA' en cualquier parte del nombre
        /// Ejemplos: HHORASALIDAINICIO, HORASALIDA, TIEMPOHORA, etc.
        /// </summary>
        /// <param name="sql">SQL a convertir</param>
        /// <returns>SQL con las conversiones aplicadas</returns>
        public string ConvertirSubstrHora(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Patrón que captura: SUBSTR(ALIAS.COLUMNA, número, número)
            // donde COLUMNA contiene HORA en cualquier parte (case-insensitive)
            // Ejemplos: HHORASALIDAINICIO, HORASALIDA, TIEMPOHORA, HORA, etc.
            string pattern = @"SUBSTR\s*\(\s*(?<tabla>\w+)\.(?<columna>\w*HORA\w*)\s*,";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            sql = regex.Replace(sql, match =>
            {
                string tabla = match.Groups["tabla"].Value;
                string columna = match.Groups["columna"].Value;

                // Retorna con el cast ::text agregado
                return $"SUBSTR({tabla}.{columna}::text,";
            });

            return sql;
        }
        /// <summary>
        /// Convierte la comparación de TIPOOPERADOR con NCLAVETIPO agregando un CAST a INTEGER.
        /// Maneja ambos casos:
        /// - PER.TIPOOPERADOR = PTO.NCLAVETIPO
        /// - PTO.NCLAVETIPO = PER.TIPOOPERADOR
        /// También maneja variaciones con OR, WHERE, diferentes alias y espacios
        /// </summary>
        /// <param name="sql">SQL a convertir</param>
        /// <returns>SQL con la conversión de tipo aplicada</returns>
        public string ConvertirComparacionTipoOperador(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Patrón 1: TIPOOPERADOR en el lado IZQUIERDO
            // Ejemplo: AND PER.TIPOOPERADOR = PTO.NCLAVETIPO
            string pattern1 = @"(?<logico>\b(?:AND|OR|WHERE)\s+)?" +
                              @"(?<alias1>\w+)\.TIPOOPERADOR\s*" +
                              @"(?<comparador>=|<>|!=|<=?|>=?)\s*" +
                              @"(?<alias2>\w+)\.NCLAVETIPO\b";

            var regex1 = new Regex(pattern1, RegexOptions.IgnoreCase);
            
            sql = regex1.Replace(sql, match =>
            {
                string logico = match.Groups["logico"].Value;
                string alias1 = match.Groups["alias1"].Value;
                string comparador = match.Groups["comparador"].Value;
                string alias2 = match.Groups["alias2"].Value;

                // TIPOOPERADOR a la izquierda -> Aplicar CAST
                return $"{logico}CAST({alias1}.TIPOOPERADOR AS INTEGER) {comparador} {alias2}.NCLAVETIPO";
            });

            // Patrón 2: TIPOOPERADOR en el lado DERECHO
            // Ejemplo: AND PTO.NCLAVETIPO = PER.TIPOOPERADOR
            string pattern2 = @"(?<logico>\b(?:AND|OR|WHERE)\s+)?" +
                              @"(?<alias1>\w+)\.NCLAVETIPO\s*" +
                              @"(?<comparador>=|<>|!=|<=?|>=?)\s*" +
                              @"(?<alias2>\w+)\.TIPOOPERADOR\b";

            var regex2 = new Regex(pattern2, RegexOptions.IgnoreCase);
            
            sql = regex2.Replace(sql, match =>
            {
                string logico = match.Groups["logico"].Value;
                string alias1 = match.Groups["alias1"].Value;
                string comparador = match.Groups["comparador"].Value;
                string alias2 = match.Groups["alias2"].Value;

                // TIPOOPERADOR a la derecha -> Aplicar CAST
                return $"{logico}{alias1}.NCLAVETIPO {comparador} CAST({alias2}.TIPOOPERADOR AS INTEGER)";
            });

            return sql;
        }
        /// <summary>
        /// Convierte expresiones TIME() de DB2 a sintaxis compatible con PostgreSQL.
        /// Soporta literales y referencias a columnas.
        /// Ejemplos:
        /// TIME('00:00:00') -> TIME '00:00:00'
        /// TIME(TABLA.COLUMNA) -> CAST(TABLA.COLUMNA AS TIME)
        /// </summary>
        /// <param name="sql">SQL a convertir.</param>
        /// <returns>SQL con las expresiones TIME convertidas.</returns>
        public string ConvertirFuncionTime(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // TIME('12:30:00') -> TIME '12:30:00'
            sql = Regex.Replace(
                sql,
                @"\bTIME\s*\(\s*['""](?<hora>[^'""]+)['""]\s*\)",
                m => $"TIME '{m.Groups["hora"].Value}'",
                RegexOptions.IgnoreCase);

            // TIME(TABLA.COLUMNA) -> CAST(TABLA.COLUMNA AS TIME)
            sql = Regex.Replace(
                sql,
                @"\bTIME\s*\(\s*(?<expresion>[\w.]+)\s*\)",
                m => $"CAST({m.Groups["expresion"].Value} AS TIME)",
                RegexOptions.IgnoreCase);

            return sql;
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

            //Se comenta la función ya que se coloco en el metodo principal para validarla en todas las consultas y no solo en la consulta
           // sql = CambiarHex(sql, ".LCAPTURAORIGENBOLMANUAL");

            return sql;
        }

        public string EliminarTrim(string sql, string Cadena, string funcion)
        {
            string Muestra;           
            int Mas, Pare;

            List<int> Indices = AllIndexesOf(sql, Cadena, 0);

            int j = 0;

            while (j < Indices.Count())
            {
                Muestra = sql.Substring(Indices[j] - 23, 42);
                Mas = Muestra.IndexOf(funcion, StringComparison.OrdinalIgnoreCase);

                if (Mas > 0)
                {
                    Pare = Muestra.IndexOf("(", Mas, StringComparison.OrdinalIgnoreCase);                    

                    sql = sql.Remove(Indices[j] - 23 + Mas, Pare - Mas + 1);

                    Pare = sql.IndexOf(")", Indices[j], StringComparison.OrdinalIgnoreCase);

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

        //Se modifica la función hex() para hacerla genarl y cambiarla por instrucciones de Postgresql validas
        public string CambiarHex(string sql)
        {
            // Regex que captura hex(campo) incluyendo prefijos con puntos
            string pattern = @"hex\s*\(\s*(?<campo>[a-zA-Z0-9_.]+)\s*\)";

            return Regex.Replace(sql, pattern,
                m => $"(CASE WHEN {m.Groups["campo"].Value} THEN '01' ELSE '00' END)",
                RegexOptions.IgnoreCase);
        }

        public List<int> AllIndexesOf(string cadena, string caracter, int inicio)
        {
            try
            {
                List<int> indexes = new List<int>();
                for (int index = inicio; ; index += caracter.Length)
                {
                    index = cadena.IndexOf(caracter, index, StringComparison.OrdinalIgnoreCase);
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


			// current time  -> current_timestamp
			sql = Regex.Replace(
				sql,
				@"\bcurrent\s+timestamp\b",
				"current_timestamp",
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

                Date = sql.IndexOf("DATE", parametro2, StringComparison.OrdinalIgnoreCase);

                parametro1 = sql.IndexOf("AS", Date, StringComparison.OrdinalIgnoreCase);

                parametro2 = sql.IndexOf("FROM", parametro1, StringComparison.OrdinalIgnoreCase);

                AS = sql.Substring(parametro1 + 2, parametro2 - parametro1 - 2).Trim();

                Fin = sql.IndexOf(".SYSDUMMY1", Indices[j], StringComparison.OrdinalIgnoreCase) + 10;

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

            indice = sql.IndexOf("FOR", 0, StringComparison.OrdinalIgnoreCase);

            sql = sql.Remove(indice, sql.Length - indice);

            return sql;
        }

        public string Stored(string sql)
        {
            int parametro1;
            int j = 0;
            int indice;

            //SP por probar
            // if (sql.Contains("SP_MASTARIFA"))
            if (Regex.IsMatch(sql, @"SP_MASTARIFA", RegexOptions.IgnoreCase))            
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

            // else if (sql.Contains("SPRECCORPLANEACION"))
            else if (Regex.IsMatch(sql, @"SPRECCORPLANEACION", RegexOptions.IgnoreCase))
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

            indice = sql.IndexOf("LOCATE", 0, StringComparison.OrdinalIgnoreCase);

            while (indice > 0)
            {
                parametro1 = sql.IndexOf(",", indice);

                sql = sql.Remove(parametro1, 1);

                sql = sql.Insert(parametro1, " IN");

                sql = sql.Remove(indice, 6);

                sql = sql.Insert(indice, "POSITION");

                indice = sql.IndexOf("LOCATE", 0, StringComparison.OrdinalIgnoreCase);
            }

            indice = sql.IndexOf("OCTETS", 0, StringComparison.OrdinalIgnoreCase);

            while (indice > 0)
            {
                sql = sql.Remove(indice - 1, 7);

                indice = sql.IndexOf("OCTETS", 0, StringComparison.OrdinalIgnoreCase);
            }

            return sql;
        }

        public string Second(string sql)
        {

            int indice, mas;
            string cantidad;

            indice = sql.IndexOf("SECOND", 0, StringComparison.OrdinalIgnoreCase);

            while (indice > 0)
            {
                mas = sql.IndexOf('+', indice - 5);

                cantidad = sql.Substring(mas + 1, indice - mas - 1);

                sql = sql.Remove(mas + 1, indice - mas + 5);

                sql = sql.Insert(mas + 1, " interval '" + cantidad + " SECOND' ");

                indice = sql.IndexOf("SECOND", mas + cantidad.Length + 18, StringComparison.OrdinalIgnoreCase);
            }

            return sql;
        }

        public string Remplazar(string sql, string original, string remplazo)
        {
            int indice;

            indice = sql.IndexOf(original, 0, StringComparison.OrdinalIgnoreCase);

            while (indice > 0)
            {
                sql = sql.Remove(indice, original.Length);

                sql = sql.Insert(indice, remplazo);

                indice = sql.IndexOf(original, indice + 1, StringComparison.OrdinalIgnoreCase);
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

            indice = sql.IndexOf("(CHAR(", 0, StringComparison.OrdinalIgnoreCase);

            while (indice > 0)
            {
                sql = sql.Remove(indice + 1, 4);

                sql = sql.Insert(indice + 1, "CAST");

                parentesis = sql.IndexOf(')', indice + 1);

                sql = sql.Insert(parentesis, " AS CHAR");

                indice = sql.IndexOf("(CHAR(", 0, StringComparison.OrdinalIgnoreCase);
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

        public string ConvertirSecuencia(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            if (!Regex.IsMatch(sql, @"^\s*CREATE\s+SEQUENCE\b", RegexOptions.IgnoreCase))
                return sql;

            return Regex.Replace(
                sql,
                @"\bNO\s+CACHE\b",
                "CACHE 1",
                RegexOptions.IgnoreCase);
        }

        public string ConvertirNextVal(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Busca el patrón típico DB2: SELECT (ESQUEMA.SECUENCIA.NEXTVAL) AS ALIAS FROM SCHEMA.TABLA FETCH FIRST 1 ROW ONLY
            // O variaciones donde simplemente esté (SCHEMA.SEQ.NEXTVAL)
            string regexPattern = @"SELECT\s+\((?<schema>\w+)\.(?<sequence>\w+)\.NEXTVAL\)\s+AS\s+(?<alias>\w+)\s+FROM\s+\w+\.\w+\s+FETCH\s+FIRST\s+1\s+ROW\s+ONLY";

            if (Regex.IsMatch(sql, regexPattern, RegexOptions.IgnoreCase))
            {
                sql = Regex.Replace(
                    sql,
                    regexPattern,
                    "SELECT nextval('${schema}.${sequence}') AS ${alias}",
                    RegexOptions.IgnoreCase);
            }
            // Segunda validación por si la consulta viene sin el schema inicial en el FROM o de manera mas simple.
            // Para encontrar "(SCHEMA.SEQ.NEXTVAL)" suelto
            else if (sql.IndexOf(".NEXTVAL)", StringComparison.OrdinalIgnoreCase) > 0)
            {
                string simplePattern = @"\((?<schema>\w+)\.(?<sequence>\w+)\.NEXTVAL\)";
                sql = Regex.Replace(sql, simplePattern, "nextval('${schema}.${sequence}')", RegexOptions.IgnoreCase);

                // Si llegara a mantener el "FROM MAGICADM.TUCLIENTE FETCH FIRST 1 ROW ONLY" lo limpiamos
                // ya que Postgresql no requiere FROM para funciones escalares.
                string trimPattern = @"\s+FROM\s+\w+\.\w+\s+FETCH\s+FIRST\s+1\s+ROW\s+ONLY";
                sql = Regex.Replace(sql, trimPattern, "", RegexOptions.IgnoreCase);
            }

            return sql;
        }

        /// <summary>
        /// Convierte la instrucción xmlserialize(xmlagg(xmltext(CONCAT(... de DB2 
        /// a la función nativa string_agg() de PostgreSQL.
        /// Patrón: SUBSTR(xmlserialize(xmlagg(xmltext(CONCAT( ',',NCLAVEBASE))) as VARCHAR(1024)), 2)
        /// Resultado: string_agg(CAST(NCLAVEBASE AS TEXT), ',')
        /// </summary>
        /// <param name="sql">SQL a convertir</param>
        /// <returns>SQL con la sintaxis convertida</returns>
        public string ConvertirXmlAgg(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Busca el patrón exacto. Extraemos como variables el delimitador (ej. ',') y nombre de la columna (ej. NCLAVEBASE)
            string pattern = @"SUBSTR\s*\(\s*xmlserialize\s*\(\s*xmlagg\s*\(\s*xmltext\s*\(\s*CONCAT\s*\(\s*(?<delimiter>'[^']+')\s*,\s*(?<columna>[\w\.]+)\s*\)\s*\)\s*\)\s*AS\s+VARCHAR\s*\(\s*\d+\s*\)\s*\)\s*,\s*\d+\s*\)";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            sql = regex.Replace(sql, match =>
            {
                string delimiter = match.Groups["delimiter"].Value;
                string columna = match.Groups["columna"].Value;

                // Reconstruimos usando la sintaxis limpia de Postgres
                return $"string_agg(CAST({columna} AS TEXT), {delimiter})";
            });

            return sql;
        }

        /// <summary>
        /// Convierte la función MONTH() de DB2 a la sintaxis nativa EXTRACT de PostgreSQL.
        /// Patrón: MONTH(det.FFECHAINICIO) -> EXTRACT(MONTH FROM det.FFECHAINICIO)
        /// </summary>
        /// <param name="sql">SQL a convertir</param>
        /// <returns>SQL con la sintaxis de extracción de mes convertida</returns>
        public string ConvertirFuncionMonth(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Busca el patrón exacto MONTH(columna_o_variable)
            // \b asegura que busque la palabra completa, evitando reemplazar funciones que terminen en "MONTH"
            string pattern = @"\bMONTH\s*\(\s*(?<columna>[^)]+)\s*\)";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            sql = regex.Replace(sql, match =>
            {
                string columna = match.Groups["columna"].Value.Trim();
                // Reconstruimos usando la función de extracción de PostgreSQL
                return $"EXTRACT(MONTH FROM {columna})";
            });

            return sql;
        }

        /// <summary>
        /// Convierte la función DATE() de DB2 a un CAST oficial de PostgreSQL
        /// Patrón: DATE(':3') -> CAST(':3' AS DATE)
        /// Patrón: DATE(CAMPO) -> CAST(CAMPO AS DATE)
        /// </summary>
        public string ConvertirFuncionDate(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            // Busca el patrón nativo: la palabra DATE, un paréntesis, el valor, y el cierre.
            string pattern = @"\bDATE\s*\(\s*(?<valor>[^)]+)\s*\)";

            return Regex.Replace(sql, pattern, match =>
            {
                string valorInterior = match.Groups["valor"].Value.Trim();

                // Transforma DATE(':3') a CAST(':3' AS DATE)
                return $"CAST({valorInterior} AS DATE)";

            }, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Convierte operaciones con intervalos de tiempo de DB2 a sintaxis compatible con PostgreSQL.
        /// Soporta SECOND, MINUTE, HOUR, DAY, MONTH y YEAR.
        /// Ejemplos:
        /// - 20 HOUR -> - INTERVAL '20 hour'
        /// + 10 MINUTE -> + INTERVAL '10 minute'
        /// </summary>
        /// <param name="sql">SQL a convertir.</param>
        /// <returns>SQL con los intervalos convertidos a sintaxis de PostgreSQL.</returns>
        public string ConvertirIntervalos(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            return Regex.Replace(
                sql,
                @"(?<signo>[+-])\s*(?<cantidad>\d+)\s+(?<unidad>SECOND|MINUTE|HOUR|DAY|MONTH|YEAR)S?\b",
                m => $"{m.Groups["signo"].Value} INTERVAL '{m.Groups["cantidad"].Value} {m.Groups["unidad"].Value.ToLowerInvariant()}'",
                RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Elimina la cláusula WITH UR utilizada en consultas DB2,
        /// ya que esta sintaxis no es compatible con PostgreSQL.
        /// </summary>
        /// <param name="sql">Sentencia SQL a procesar.</param>
        /// <returns>Sentencia SQL sin la cláusula WITH UR.</returns>
        public string EliminaWithUr(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;

            return Regex.Replace(
                sql,
                @"\s+WITH\s+UR\s*(?=;?\s*$)",
                "",
                RegexOptions.IgnoreCase);
        }

    }
}
