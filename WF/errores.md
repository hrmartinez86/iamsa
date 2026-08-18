# 04/08/2026 - error al querer entrar a Catalogo de Departamentos MPA
    Error al intentar ejecutar la instrucción Select Query, Inner: 42703: column "claveusuario" does not exist
Application User: RMARTINEZ
Task Crashed
Callstack:
   en Utilidades.ConsultasDirectas.DAL._ThrowException(String message, Exception innerException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 201
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Boolean throwException, String selectQuery, DbParameter[] parameters, Action`1 actionWithReader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 175
   en Utilidades.ConsultasDirectas.DAL._ExcecuteSelectQuery(String sqlQuery, DbParameter[] parameters, Func`2 logicWhere, Boolean throwException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 85
   en Utilidades.QueriesAuxiliares.SQL_ObtenerAuditorias.GetValues.LoadUserSettings(String userID) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\QueriesAuxiliares\SQL_ObtenerAuditorias.cs:línea 145
   en Utilidades.QueriesAuxiliares.SQL_ObtenerAuditorias.GetValues.OnLeaveRow() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\QueriesAuxiliares\SQL_ObtenerAuditorias.cs:línea 125
   en ENV.BusinessProcessBase.<.ctor>b__3_1() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\BusinessProcessBase.cs:línea 61
   en WizardOfOz.Witch.Engine.FlowEvent.Do()
   en Firefly.Box.BusinessProcess.TaskTypeBatch.RunTaskSection(FlowEvent flowEvent)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.LeaveRow(LeaveRowTools tools, FlowToEndOfRow userFLowForLeaveRow, Boolean deleteRow)
   en Firefly.Box.BusinessProcess.TaskTypeBatch.<>c__DisplayClass25.<Run>b__20(RowCycleActions actions)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.<>c__DisplayClassc9.<DoRowCycle>b__c8()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.Task.myTaskTransactionManager.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredRowLevelCommand(Action command, Action callMeIfYouRollbacked)
   en Firefly.Box.Task.RunMonitoredRowLevelCommand(Action command, Boolean considerEndingTask)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.DoRowCycle(Action`1 rowCycle)
   en Firefly.Box.BusinessProcess.TaskTypeBatch.Run(RunToolsForTaskType options, EventHandlerBuilder builder)
   en Firefly.Box.Task.<Run>b__48(RunTools runTools)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<>c__DisplayClass32.<Run>b__17()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.Task.myTaskTransactionManager.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredTaskLevelCommand(Action action)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<Run>b__13(LoadTaskCommandDelegate loadTaskCommand, Boolean allowForm)
   en Firefly.Box.RegularTaskRunner.LoadTask(LoadTask load)
   en Firefly.Box.Task.<>c__DisplayClass2b.<Run>b__11(TaskRunContext context)
   en WizardOfOz.Witch.Engine.CallStackClass.RunTask(HostedItem task, Boolean isApplication, RunTask commandToExecute)
   en WizardOfOz.Witch.Engine.CallStackClass.<>c__DisplayClass11.<WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask>b__10()
   en WizardOfOz.Witch.Engine.CallStackClass.RunActionWithModuleController(ModuleController moduleController, Action action)
   en WizardOfOz.Witch.Engine.CallStackClass.WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask(HostedItem task, ModuleController module, RunTask cmd)
   en Firefly.Box.RegularTaskRunner.Execute(HostEnvironment host, HostedItem hostedItem, RunTask runTask, Action allowNestedRuns)
   en Firefly.Box.Task.Run(TaskRunner taskRunner)
   en Firefly.Box.Task.Run()
   en Firefly.Box.BusinessProcess.Run()
   en ENV.BusinessProcessBase.RunTheTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\BusinessProcessBase.cs:línea 478
   en ENV.ControllerBase.RunTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 658
   en ENV.ControllerBase.<>c.<.ctor>b__1_2(Action y) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 298
   en ENV.ControllerBase.Execute() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 347
Inner Error : 42703: column "claveusuario" does not exist
Inner Trace : 
   en Npgsql.NpgsqlConnector.<>c__DisplayClass158_0.<<DoReadMessage>g__ReadMessageLong|0>d.MoveNext()
--- Fin del seguimiento de la pila de la ubicación anterior donde se produjo la excepción ---
   en System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   en Npgsql.NpgsqlConnector.<>c__DisplayClass158_0.<<DoReadMessage>g__ReadMessageLong|0>d.MoveNext()
--- Fin del seguimiento de la pila de la ubicación anterior donde se produjo la excepción ---
   en System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   en System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   en System.Threading.Tasks.ValueTask`1.get_Result()
   en Npgsql.NpgsqlDataReader.<NextResult>d__44.MoveNext()
--- Fin del seguimiento de la pila de la ubicación anterior donde se produjo la excepción ---
   en System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   en System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   en Npgsql.NpgsqlDataReader.NextResult()
   en Npgsql.NpgsqlCommand.<ExecuteReaderAsync>d__97.MoveNext()
--- Fin del seguimiento de la pila de la ubicación anterior donde se produjo la excepción ---
   en System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   en System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   en Npgsql.NpgsqlCommand.ExecuteReader(CommandBehavior behavior)
   en Npgsql.NpgsqlCommand.ExecuteDbDataReader(CommandBehavior behavior)
   en System.Data.Common.DbCommand.System.Data.IDbCommand.ExecuteReader()
   en ENV.Data.DataProvider.LogDatabaseWrapper.TextLogWriter.ExecuteOperation[Type](String description, Func`1 whatToRun, IDbCommand command) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\LogDatabaseWrapper.cs:línea 120
   en ENV.Data.DataProvider.LogDatabaseWrapper.CommandWrapper.ExecuteReader() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\LogDatabaseWrapper.cs:línea 345
   en ENV.Data.DataProvider.LogDatabaseWrapper.TextLogWriter.ExecuteOperation[Type](String description, Func`1 whatToRun, IDbCommand command) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\LogDatabaseWrapper.cs:línea 120
   en ENV.Data.DataProvider.LogDatabaseWrapper.CommandWrapper.ExecuteReader() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\LogDatabaseWrapper.cs:línea 345
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Boolean throwException, String selectQuery, DbParameter[] parameters, Action`1 actionWithReader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 168

Application Callstack:
Ejecuta Programas - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas+EjecutaProgramas
UT_EjecutaProgramas (P#8) - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas
  Parameters: E_nClaveProgramaEjecutar = 1033

Programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas+Programas
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499

## Posible causa

El error principal es `42703: column "claveusuario" does not exist`. En PostgreSQL eso significa que la consulta ejecutada por `LoadUserSettings(String userID)` está intentando leer una columna que no existe con ese nombre exacto.

Por el contexto, el fallo ocurre antes de entrar al catálogo de departamentos, durante la carga de parámetros o preferencias del usuario. La evidencia del script de alta de usuario muestra columnas como `ACLAVEUSUARIO`, `AAUCLAVEUSUARIO` y `ACLAVEUSUARIOS`, no `claveusuario`.

## Qué revisar primero

1. Revisar la consulta en `Utilidades.QueriesAuxiliares.SQL_ObtenerAuditorias.GetValues.LoadUserSettings`.
2. Buscar si la consulta usa `claveusuario` en lugar de `ACLAVEUSUARIO` o `AAUCLAVEUSUARIO`.
3. Confirmar en la base de datos el nombre real de la columna en la tabla o vista que consulta el código.
4. Verificar si el esquema usa nombres sensibles a mayúsculas/minúsculas o columnas creadas con comillas.

## Pasos para solucionarlo

1. Abrir la instrucción SQL que falla y corregir el nombre de la columna al nombre real.
2. Si la consulta fue escrita para otro motor o una versión anterior, adaptar el SQL a PostgreSQL.
3. Validar que la tabla consultada tenga los datos del usuario `RMARTINEZ`.
4. Ejecutar la consulta manualmente en la base de datos antes de probar desde la aplicación.
5. Si la columna correcta existe pero la consulta sigue fallando, revisar aliases, joins y posibles vistas intermedias.

## Resultado esperado

Después de corregir el nombre de la columna, la carga de configuración del usuario debería completarse y el catálogo de departamentos debería abrir sin detener la tarea.



{

}
## query
@ SELECT ACLAVEOFICINA, NCLAVEDEPARTAMENTO, TRIM(ANOMBRES) || ' ' || TRIM(AAPELLIDOS) AS ANOMBREUSUARIO
FROM MAGICADM.GCPERSONAS
WHERE NCLAVEEMPRESA = 4 AND ACLAVEUSUARIO = @ClaveUsuario AND LBAJAUSUARIO = X'00'
FETCH FIRST ROW ONLY

hrmartinez86@gmail.com

## cambio en funcion
 if (sql.Contains("@ClaveUsuario"))
 {
     sql = Remplazar(sql, "@ClaveUsuario", "'RMARTINEZ'");
 }

## ERROR VTABLE
The column name was not found
Application User: RMARTINEZ
Task Crashed
Callstack:
   en ColleccionesSimples.ModeloDeTablas.V2_0.VTable.get_Item(Int32 rowIndex, String columnName) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ColleccionesSimples\ModeloDeTablas\V2_0\VTable.cs:línea 77
   en AdicionesWorkFlowMPA.CatalogoDepartamentos.PS_CatalogoDepartamentosMPA.get_DepartamentoSeleccionado() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\AdicionesWorkFlowMPA\CatalogoDepartamentos\PS_CatalogoDepartamentosMPA.cs:línea 22
   en AdicionesWorkFlowMPA.CatalogoDepartamentos.PS_CatalogoDepartamentosMPA.btn_Eventos_Click(Object sender, EventArgs e) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\AdicionesWorkFlowMPA\CatalogoDepartamentos\PS_CatalogoDepartamentosMPA.cs:línea 68
   en System.Windows.Forms.Control.OnClick(EventArgs e)
   en Utilidades.Controles.IconButton.lbl_Texto_Click(Object sender, EventArgs e) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\Controles\IconButton.cs:línea 308
   en System.Windows.Forms.Control.OnClick(EventArgs e)
   en System.Windows.Forms.Control.WmMouseUp(Message& m, MouseButtons button, Int32 clicks)
   en System.Windows.Forms.Control.WndProc(Message& m)
   en System.Windows.Forms.Label.WndProc(Message& m)
   en System.Windows.Forms.Control.ControlNativeWindow.OnMessage(Message& m)
   en System.Windows.Forms.Control.ControlNativeWindow.WndProc(Message& m)
   en System.Windows.Forms.NativeWindow.Callback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
   en WizardOfOz.Witch.Engine.CallStackClass.HandleUIException()
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.<>c__DisplayClass109.<RunMessageLoop>b__103()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.Task.myTaskTransactionManager.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredRowLevelCommand(Action command, Action callMeIfYouRollbacked)
   en Firefly.Box.Task.RunMonitoredRowLevelCommand(Action command, Boolean considerEndingTask)
   en Firefly.Box.Task.WrapEventProcessing(Action eventProcessing)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunMessageLoop(StopLoop stopLoop, MessageHandler messageHandler, Action`1 wrapEventProcessing)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.RunInteractiveLoop(Func`1 stop)
   en Firefly.Box.UIController.TaskTypeInteractive.Run(RunToolsForTaskType options, EventHandlerBuilder builder)
   en Firefly.Box.Task.<Run>b__48(RunTools runTools)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<>c__DisplayClass32.<Run>b__17()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.Task.myTaskTransactionManager.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredTaskLevelCommand(Action action)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<Run>b__13(LoadTaskCommandDelegate loadTaskCommand, Boolean allowForm)
   en Firefly.Box.RegularTaskRunner.LoadTask(LoadTask load)
   en Firefly.Box.Task.<>c__DisplayClass2b.<Run>b__11(TaskRunContext context)
   en WizardOfOz.Witch.Engine.CallStackClass.RunTask(HostedItem task, Boolean isApplication, RunTask commandToExecute)
   en WizardOfOz.Witch.Engine.CallStackClass.<>c__DisplayClass11.<WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask>b__10()
   en WizardOfOz.Witch.Engine.CallStackClass.RunActionWithModuleController(ModuleController moduleController, Action action)
   en WizardOfOz.Witch.Engine.CallStackClass.WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask(HostedItem task, ModuleController module, RunTask cmd)
   en Firefly.Box.RegularTaskRunner.Execute(HostEnvironment host, HostedItem hostedItem, RunTask runTask, Action allowNestedRuns)
   en Firefly.Box.Task.Run(TaskRunner taskRunner)
   en Firefly.Box.Task.Run()
   en Firefly.Box.UIController.Run()
   en ENV.AbstractUIController.RunTheTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\AbstractUIController.cs:línea 630
   en ENV.ControllerBase.RunTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 658
   en ENV.ControllerBase.<>c.<.ctor>b__1_2(Action y) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 298
   en ENV.ControllerBase.Execute() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 347

Application Callstack:
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499



## ubicacion de mensaje de eventos
D:\Usuarios\c-hmartinez\SUCV80\Iamsa\
AdicionesWorkFlowMPA\
   MttoEventos\
      PS_MantenimientoEventos_Principal.cs

## solucion
            private void LoadUserSettings(string userID)
            {
                var parametros = new IDbDataParameter[]
                {
        DAL.CreateParameter("@ClaveUsuario", DbType.String, 10, userID)
                };

                DAL.ExecuteSelectQuery(
                    $@"SELECT ACLAVEOFICINA, NCLAVEDEPARTAMENTO, TRIM(ANOMBRES) || ' ' || TRIM(AAPELLIDOS) AS ANOMBREUSUARIO
FROM MAGICADM.GCPERSONAS
WHERE NCLAVEEMPRESA = {NCLAVEEMPRESA.Value} AND ACLAVEUSUARIO = @ClaveUsuario AND LBAJAUSUARIO = X'00'
FETCH FIRST ROW ONLY",
                    parametros.Cast<DbParameter>().ToArray(),
                    new Action<VRow>(U =>
                    {
                        _parent.ClaveOficina = U["ACLAVEOFICINA"].ToString();
                        _parent.ClaveDepartamento = U["NCLAVEDEPARTAMENTO"].ToInt();
                        _parent.NombreUsuario = U["ANOMBREUSUARIO"].ToString();
                    })
                );
            }

## error de booleano
42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Application User: RMARTINEZ
DynamicSQLEntity: INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(:1,':2',':3',':4',:5,:6,:7,:8,:9,:10,:11,:12) 

ENV.Data.DynamicSQLEntity

SQL:
INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(    9077243.00000000 ,'REGISTRO DE INFORMACIÓN DE LA REPARACIÓN DEL AUTOBÚS                                                ','',' ',0,          5.00000000 ,0,0,False,      99623.00000000 ,0,0)


Callstack:
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1137
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.ProvideRowsSource(Entity entity) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 779
   en Firefly.Box.Data.Entity.StartTaskCommand.Run()
   en Firefly.Box.Data.MonitoredDatabaseRunnerClass.RunQuery(RunMonitoredCommand command, Boolean isUpdateOrInsert)
Inner Error : 42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Inner Trace : 
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1113

Application Callstack:
W_PaDetalleKardex - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales+W_PaDetalleKardex
DatosAdicionales - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales
W_PAKARDEX - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX
PS_EscribePAKARDEX (P#1551) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore
  Parameters: E_nClaveEmpresa = 4
              E_nClaveDepartamento = 7
              E_aDepartamento = WORK FLOW
              E_nIdTarea = 0
              E_lPostura = false
              E_lGuardia = false
              E_aClaveOficina = 
              E_nNumeroGuardia = 0
              E_nFolioViajeTurismo = 0
              E_nClaveAsignacion = 0
              E_nIdGeneralFlujo = 0
              E_nConsecutivo = 0
              E_aClaveEstado = 
              E_nClavePersona = 8081
              E_nClaveAutobus = 0
              E_lOperadorEstatus = false
              E_nClaveAutobusEstatus = 0
              E_nClaveOperadorEstatus = 0
              E_aClaveEstatus = 
              E_lDatosAdicionales = true
              S_nIdKardex = 0

W_Agregar - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore+W_Agregar
PS_Genera_Tarea/FlujodeTrabajo (P#1561) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore
  Parameters: E_nIdFlujo = 0
              E_nIdEvento = 7071
              E_nIdGeneralFlujo = 0
              E_lDesdeAdmonFlujos = false
              E_aNombreFlujo = 
              E_nClaveEmpleado = RMARTINEZ
              E_nNumeroEconomico = 0
              E_nIdTarea = 0
              S_nIdGeneralTarea = 0
              E_lGuardaAutomatico? = false
              E_fFechaInicial = 12/08/2026
              E_fFechaFinal = 12/08/2026
              E_hHorarioInicial = 00:00:00
              E_hHorarioFinal = 00:00:00
              E_lCreaTareasHorario = true
              E_nTipoEvento = 0
              E_aObservaciones = 
              E_nClaveEventoOrigen = 7071
              E_aClaveOficina = 
              E_lEntidadIndefinida = false
              E_aNombreEntidadIndefinida = 

W_Agregar - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore+W_Agregar
PS_MttoEventos (P#23) - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore
Ejecuta Programas - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas+EjecutaProgramas
UT_EjecutaProgramas (P#8) - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas
  Parameters: E_nClaveProgramaEjecutar = 1031

Programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas+Programas
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499

## error fecha
se tiene un nievo error con este script INSERT INTO MAGICADM.PAHISTORIALASIGNACIONES (NEMPRESA, NDEPARTAMENTO, NIDTAREA, NIDPUESTOORIGEN, NCONSECUTIVOPUESTOORIGEN, NIDGRUPOPUESTOSORIGEN, NIDPUESTODESTINO, NCONSECUTIVOPUESTODESTINO, NIDGRUPOPUESTOSDESTINO, FFECHAASIGNACION, HHORAASIGNACION, ACOMENTARIO, FAUFECHA, HAUHORA, NAUCLAVETERMINAL, AAUCLAVEOFICINA, AAUCLAVEUSUARIO) VALUES (          4.00000000 ,           7.00000000 , 0, 0, 0, 0, 0, 0, 0, '12/08/2026', '18.51', 'Asignación a Departamento - Creación de Tarea.                                                                                                                                                          ', '12/08/2026', '18.51',        8097.00000000 , 'SNJR', 'RMARTINEZ ')
   ## solucion
   El error ocurre porque PostgreSQL espera formato de hora HH:MM:SS pero está recibiendo 18.51 (formato DB2). Aunque ya tienes ChangeHourFormat en MigracionPostgresql.cs, no se está aplicando en el momento correcto. El problema es que ChangeHourFormat solo se llama después de que el SQL ya está completamente construido con los parámetros reemplazados.
      🔧 Solución: Aplicar Conversión de Hora en Múltiples Puntos
   Necesitas aplicar ChangeHourFormat en dos lugares:
   ## 1. En DynamicSQLEntity.cs - Constructor del SQLBuilder
      internal class SQLBuilder : ISQLBuilder
   {
    public SQLBuilder(string sql1)
    {
        string dbType = ENV.UserSettings.Get("MAGIC_DATABASES", "CITEC").Split(',')[1].Trim();               

        string sql = sql1.ToUpper();

        if (dbType == "22")
        {
            var x = new MigracionPostgresql();
            sql = x.ConvertirPost(sql);
        }
        else
        {
            var x = new MigracionPostgresql();
            sql = x.MDYToDMY(sql);
        }
       
        _sql = sql;

        var sb = new StringBuilder();
        bool inColon = false;
        int lastPos = 0;
        int pos = 0;
        foreach (var c in _sql)
        {
            if (inColon)
            {
                if ("1234567890".IndexOf(c) >= 0)
                {
                    sb.Append(c);
                }
                else
                {
                    _tokens.Add(new Token(sb.ToString(), lastPos, this));
                    inColon = false;
                }

            }
            if (c == ':' || c == '~')
            {
                sb = new StringBuilder();
                sb.Append(c);
                inColon = true;
                lastPos = pos;
            }
            pos++;
        }
        if (inColon)
        {
            _tokens.Add(new Token(sb.ToString(), lastPos, this));
        }

    }
   ## 2. Modificar AddValueParameter para Aplicar Formato de Hora DynamicSQLEntity.cs
   public void AddValueParameter(string s, bool isNull, Func<IDbDataParameter> createParamAndSetItsValue)
{
    _paramNumber++;
    
    // NUEVO: Aplicar conversión de formato de hora si es PostgreSQL
    string dbType = ENV.UserSettings.Get("MAGIC_DATABASES", "CITEC").Split(',')[1].Trim();
    if (dbType == "22" && !string.IsNullOrEmpty(s) && !isNull)
    {
        var x = new MigracionPostgresql();
        s = x.ChangeHourFormat(s);
    }
    
    //set Value Parameter
    {
        string lookFor = ":" + _paramNumber;
        foreach (var t in _tokens)
        {
            if (t._token == lookFor)
            {
                int lastIndex = t.Position;
                int delta = 0;

                _sql.Substring(lastIndex, lookFor.Length).ShouldBe(lookFor, "SQL Token parsing error, token was not where it should be");
                _sql = _sql.Remove(lastIndex, lookFor.Length);
                delta -= lookFor.Length;

                _sql = _sql.Insert(lastIndex, s);
                delta += s.Length;
                if (isNull)
                {
                    bool stopLoop = false;
                    for (int i = lastIndex - 1; i >= 0; i--)
                    {
                        switch (_sql[i])
                        {
                            case '\n':
                            case '\r':
                            case ' ':
                                break;
                            case '=':
                                _sql = _sql.Remove(i, 1);
                                delta -= 1;
                                _sql = _sql.Insert(i, " is ");
                                delta += 4;
                                break;
                            case '>':
                                if (i > 0 && _sql[i - 1] == '<')
                                {
                                    _sql = _sql.Remove(i - 1, 2);
                                    delta -= 2;
                                    _sql = _sql.Insert(i - 1, " is not ");
                                    delta += 8;
                                }
                                else
                                    stopLoop = true;
                                break;
                            default:
                                stopLoop = true;
                                break;
                        }
                        if (stopLoop)
                            break;
                    }
                }
                _deltas.Add(t._position, delta);
            }
        }
    }
    
    //set Real Parameter
    {
        string lookFor = "~" + _paramNumber;
        string paramName = ":p" + _paramNumber;
        bool _savedParam = false; 
        foreach (var t in _tokens)
        {
            if (t._token == lookFor)
            {
                int lastIndex = t.Position;
                int delta = 0;

                _sql.Substring(lastIndex, lookFor.Length).ShouldBe(lookFor, "SQL Token parsing error, token was not where it should be");
                _sql = _sql.Remove(lastIndex, lookFor.Length);
                delta -= lookFor.Length;
                _sql = _sql.Insert(lastIndex, paramName);
                delta += paramName.Length;
                if (!_savedParam)
                {
                    _savedParam = true;
                    var p = createParamAndSetItsValue();
                    p.ParameterName = paramName;
                }
                _deltas.Add(t._position, delta);
            }
        }
    }
}
   ## 3. Mejorar el Regex en ChangeHourFormat
   El regex actual solo busca dentro de comillas. Mejóralo para que funcione en más contextos:
   MigracionPostgresql.cs
   /// <summary>
/// Cambiar formato de hora HH.mm.ss(DB2) a HH:mm:ss(Postgres)
/// </summary>
/// <param name="sql"></param>
/// <returns></returns>
public string ChangeHourFormat(string sql)
{
    if (string.IsNullOrWhiteSpace(sql))
        return sql;

    // Patrón 1: Dentro de comillas simples '18.51.30' -> '18:51:30'
    Regex regHour = new Regex(
        @"(?<='[^']*)([01]\d|2[0-3])\.([0-5]\d)\.([0-5]\d)(?=[^']*')",
        RegexOptions.Compiled
    );
    sql = regHour.Replace(sql, "$1:$2:$3");

    // Patrón 2: Formato corto dentro de comillas '18.51' -> '18:51:00'
    Regex regHourShort = new Regex(
        @"(?<='[^']*)([01]\d|2[0-3])\.([0-5]\d)(?=[^']*')",
        RegexOptions.Compiled
    );
    sql = regHourShort.Replace(sql, "$1:$2:00");

    // Patrón 3: Fuera de comillas (valores literales) 18.51.30 -> '18:51:30'
    Regex regHourNoQuotes = new Regex(
        @"\b([01]\d|2[0-3])\.([0-5]\d)\.([0-5]\d)\b",
        RegexOptions.Compiled
    );
    sql = regHourNoQuotes.Replace(sql, "'$1:$2:$3'");

    // Patrón 4: Fuera de comillas formato corto 18.51 -> '18:51:00'
    Regex regHourShortNoQuotes = new Regex(
        @"\b([01]\d|2[0-3])\.([0-5]\d)\b",
        RegexOptions.Compiled
    );
    sql = regHourShortNoQuotes.Replace(sql, "'$1:$2:00'");

    return sql;
}
   ## El error ocurre porque los valores de hora en formato DB2 (18.51) no se convierten a formato PostgreSQL (18:51:00) antes de ejecutarse. La solución es aplicar ChangeHourFormat en el método AddValueParameter de DynamicSQLEntity.cs antes de insertar el valor en el SQL, detectando si es PostgreSQL mediante la verificación del tipo de base de datos (dbType == "22"), y además mejorar el regex en ChangeHourFormat para manejar tanto horas dentro de comillas como valores literales, convirtiendo formatos como 18.51 a 18:51:00 que PostgreSQL reconoce correctamente como tipo TIME.

## 42804: column "lvisibleterminar" is of type boolean but expression is of type integer
42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Application User: RMARTINEZ
DynamicSQLEntity: INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(:1,':2',':3',':4',:5,:6,:7,:8,:9,:10,:11,:12) 

ENV.Data.DynamicSQLEntity

SQL:
INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(    9077243.00000000 ,'REGISTRO DE INFORMACIÓN DE LA REPARACIÓN DEL AUTOBÚS                                                ','',' ',0,          5.00000000 ,0,0,False,      99623.00000000 ,0,0)


Callstack:
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1137
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.ProvideRowsSource(Entity entity) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 779
   en Firefly.Box.Data.Entity.StartTaskCommand.Run()
   en Firefly.Box.Data.MonitoredDatabaseRunnerClass.RunQuery(RunMonitoredCommand command, Boolean isUpdateOrInsert)
Inner Error : 42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Inner Trace : 
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1113

Application Callstack:
W_PaDetalleKardex - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales+W_PaDetalleKardex
DatosAdicionales - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales
W_PAKARDEX - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX
PS_EscribePAKARDEX (P#1551) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore
  Parameters: E_nClaveEmpresa = 4
              E_nClaveDepartamento = 7
              E_aDepartamento = WORK FLOW
              E_nIdTarea = 0
              E_lPostura = false
              E_lGuardia = false
              E_aClaveOficina = 
              E_nNumeroGuardia = 0
              E_nFolioViajeTurismo = 0
              E_nClaveAsignacion = 0
              E_nIdGeneralFlujo = 0
              E_nConsecutivo = 0
              E_aClaveEstado = 
              E_nClavePersona = 8081
              E_nClaveAutobus = 0
              E_lOperadorEstatus = false
              E_nClaveAutobusEstatus = 0
              E_nClaveOperadorEstatus = 0
              E_aClaveEstatus = 
              E_lDatosAdicionales = true
              S_nIdKardex = 0

W_Agregar - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore+W_Agregar
PS_Genera_Tarea/FlujodeTrabajo (P#1561) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore
  Parameters: E_nIdFlujo = 0
              E_nIdEvento = 7071
              E_nIdGeneralFlujo = 0
              E_lDesdeAdmonFlujos = false
              E_aNombreFlujo = 
              E_nClaveEmpleado = RMARTINEZ
              E_nNumeroEconomico = 0
              E_nIdTarea = 0
              S_nIdGeneralTarea = 0
              E_lGuardaAutomatico? = false
              E_fFechaInicial = 12/08/2026
              E_fFechaFinal = 12/08/2026
              E_hHorarioInicial = 00:00:00
              E_hHorarioFinal = 00:00:00
              E_lCreaTareasHorario = true
              E_nTipoEvento = 0
              E_aObservaciones = 
              E_nClaveEventoOrigen = 7071
              E_aClaveOficina = 
              E_lEntidadIndefinida = false
              E_aNombreEntidadIndefinida = 

W_Agregar - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore+W_Agregar
PS_MttoEventos (P#23) - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore
Ejecuta Programas - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas+EjecutaProgramas
UT_EjecutaProgramas (P#8) - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas
  Parameters: E_nClaveProgramaEjecutar = 1031

Programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas+Programas
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499
   ## 

Regex regHour = new Regex(@"(?<='[^']*)([01]\d|2[0-3])\.([0-5]\d)\.([0-5]\d)(?=[^']*')", RegexOptions.Compiled);

1-
      evento 7069
      INSERT INTO PAKARDEX
      sql.Contains("INSERT INTO PAKARDEX")
      con el cambio en fecha avanza en la tranformación

## 2-column "lvisibleterminar" is of type boolean but expression is of type integer
   INSERT INTO MAGICADM.PADETALLEKARDEX
   42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Application User: RMARTINEZ
DynamicSQLEntity: INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(:1,':2',':3',':4',:5,:6,:7,:8,:9,:10,:11,:12) 

ENV.Data.DynamicSQLEntity

SQL:
INSERT INTO MAGICADM.PADETALLEKARDEX
    (NIDKARDEX, ANOMBREDATOADICIONAL, AVALOR, ATIPODATO, NLONGITUDENTEROS,NTIPOSELECCION,NTIPO,NSUBTIPO,LOBLIGACAPTURA,NIDGENERALDA, NIDEVENTOSELECCION,LVISIBLETERMINAR) 
VALUES(    9135586.00000000 ,'Fecha del reporte                                                                                   ','',' ',0,          9.00000000 ,0,0,False,      99558.00000000 ,0,1 )
select * from PADETALLEKARDEX where NIDKARDEX=9135586;

Callstack:
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1137
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.ProvideRowsSource(Entity entity) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 779
   en Firefly.Box.Data.Entity.StartTaskCommand.Run()
   en Firefly.Box.Data.MonitoredDatabaseRunnerClass.RunQuery(RunMonitoredCommand command, Boolean isUpdateOrInsert)
Inner Error : 42804: column "lvisibleterminar" is of type boolean but expression is of type integer
Inner Trace : 
   en ENV.Data.DataProvider.DynamicSQLSupportingDataProvider.mySqlDataProviderForSqlEntity.InternalProvideSource(Entity entity, Boolean isParentInTransaction, Boolean isBatch, Func`1 commandFactory) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\Data\DataProvider\DynamicSQLSupportingDataProvider.cs:línea 1113

Application Callstack:
W_PaDetalleKardex - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales+W_PaDetalleKardex
DatosAdicionales - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX+DatosAdicionales
W_PAKARDEX - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore+W_PAKARDEX
PS_EscribePAKARDEX (P#1551) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_EscribePAKARDEXCore
  Parameters: E_nClaveEmpresa = 4
              E_nClaveDepartamento = 7
              E_aDepartamento = WORK FLOW
              E_nIdTarea = 7321324
              E_lPostura = false
              E_lGuardia = false
              E_aClaveOficina = 
              E_nNumeroGuardia = 0
              E_nFolioViajeTurismo = 0
              E_nClaveAsignacion = 0
              E_nIdGeneralFlujo = 0
              E_nConsecutivo = 0
              E_aClaveEstado = 
              E_nClavePersona = 8081
              E_nClaveAutobus = 0
              E_lOperadorEstatus = false
              E_nClaveAutobusEstatus = 0
              E_nClaveOperadorEstatus = 0
              E_aClaveEstatus = 
              E_lDatosAdicionales = true
              S_nIdKardex = 0

W_Agregar - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore+W_Agregar
PS_Genera_Tarea/FlujodeTrabajo (P#1561) - Iamsa.MMTCC.Part38.PersonalAbordo.PS_Genera_Tarea_FlujodeTrabajoCore
  Parameters: E_nIdFlujo = 0
              E_nIdEvento = 7069
              E_nIdGeneralFlujo = 0
              E_lDesdeAdmonFlujos = false
              E_aNombreFlujo = 
              E_nClaveEmpleado = RMARTINEZ
              E_nNumeroEconomico = 0
              E_nIdTarea = 0
              S_nIdGeneralTarea = 0
              E_lGuardaAutomatico? = false
              E_fFechaInicial = 13/08/2026
              E_fFechaFinal = 13/08/2026
              E_hHorarioInicial = 00:00:00
              E_hHorarioFinal = 00:00:00
              E_lCreaTareasHorario = true
              E_nTipoEvento = 0
              E_aObservaciones = 
              E_nClaveEventoOrigen = 7069
              E_aClaveOficina = 
              E_lEntidadIndefinida = false
              E_aNombreEntidadIndefinida = 

W_Agregar - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore+W_Agregar
PS_MttoEventos (P#23) - Iamsa.Mpa.Part1.PersonalAbordo.PS_MttoEventosCore
Ejecuta Programas - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas+EjecutaProgramas
UT_EjecutaProgramas (P#8) - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas
  Parameters: E_nClaveProgramaEjecutar = 1031

Programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas+Programas
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499

## Nconsecutivo no existe
El nombre de campo 'NCONSECUTIVO' no existe
Application User: RMARTINEZ
Task Crashed
Callstack:
   en ColleccionesSimples.ModeloDeTablas.V2_0.VRow.get_Item(String fieldName) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ColleccionesSimples\ModeloDeTablas\V2_0\VRow.cs:línea 47
   en Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.MemoriaYCalculos.AdminDetalleKardex.<>c__DisplayClass184_0.<ExisteConsecutivo>b__0(VRow d)
   en System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   en System.Linq.Enumerable.Count[TSource](IEnumerable`1 source)
   en Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.MemoriaYCalculos.AdminDetalleKardex.ExisteConsecutivo(Int32 nConsecutivo)
   en Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.MemoriaYCalculos.AdminDetalleKardex.AgregarFila(Int32 nConsecutivo, Int32 nIdKardex, String aNombreDatoAdicional, String aValor, String aTipoDato, Int32 nLongitudEnteros, Int32 nTipoSeleccion, Int32 nTipo, Int32 nSubTipo, Int32 nCatalogo, String aCatalogo, Int32 nEvento, Boolean lObligaCaptura, String aNombreRuta, Int32 nOrden, Int32 nOpcionEventoPredeterminado, Int32 nClaveDatoAdicional, Int32 nEventoProcesando, Boolean lVisibleTerminar, Int32 nEventoPredeterminado, Boolean lResultadoLogico, Int32 nJustificacion, String aColorLetra, String aFuenteLetra, Boolean lArchivoReferencia, Int32 nIDFlujoArchivoRef, Int32 nIDEventoArchivoRef, Int32 nIDDatoAdicionalArchivoRef, Int32 nIdTipoDocumentoReimp, Boolean receptorReferenciaCampo)
   en Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.MemoriaYCalculos.AdminDetalleKardex.PorCadaDatoAdicional(SimpleDictionary`2 D)
   en Utilidades.ConsultasDirectas.ConsultasSQL.OnLeaveRow() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\ConsultasSQL.cs:línea 482
   en ENV.BusinessProcessBase.<.ctor>b__3_1() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\BusinessProcessBase.cs:línea 61
   en WizardOfOz.Witch.Engine.FlowEvent.Do()
   en Firefly.Box.BusinessProcess.TaskTypeBatch.RunTaskSection(FlowEvent flowEvent)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.LeaveRow(LeaveRowTools tools, FlowToEndOfRow userFLowForLeaveRow, Boolean deleteRow)
   en Firefly.Box.BusinessProcess.TaskTypeBatch.<>c__DisplayClass25.<Run>b__20(RowCycleActions actions)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.<>c__DisplayClassc9.<DoRowCycle>b__c8()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.Task.myTaskTransactionManager.RunRecoverableMonitoredCommand(Action command, Action callMeIfYouRollbacked, Boolean throwExceptionIfNotRecover)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredRowLevelCommand(Action command, Action callMeIfYouRollbacked)
   en Firefly.Box.Task.RunMonitoredRowLevelCommand(Action command, Boolean considerEndingTask)
   en Firefly.Box.Task.RunToolsForTaskTypeClass.DoRowCycle(Action`1 rowCycle)
   en Firefly.Box.BusinessProcess.TaskTypeBatch.Run(RunToolsForTaskType options, EventHandlerBuilder builder)
   en Firefly.Box.Task.<Run>b__48(RunTools runTools)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<>c__DisplayClass32.<Run>b__17()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.Task.myTaskTransactionManager.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.TransactionScopesStrategy.None.RunMonitoredTaskLevelCommand(Action action)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<Run>b__13(LoadTaskCommandDelegate loadTaskCommand, Boolean allowForm)
   en Firefly.Box.RegularTaskRunner.LoadTask(LoadTask load)
   en Firefly.Box.Task.<>c__DisplayClass2b.<Run>b__11(TaskRunContext context)
   en WizardOfOz.Witch.Engine.CallStackClass.RunTask(HostedItem task, Boolean isApplication, RunTask commandToExecute)
   en WizardOfOz.Witch.Engine.CallStackClass.<>c__DisplayClass11.<WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask>b__10()
   en WizardOfOz.Witch.Engine.CallStackClass.RunActionWithModuleController(ModuleController moduleController, Action action)
   en WizardOfOz.Witch.Engine.CallStackClass.WizardOfOz.Witch.Engine.HostEnvironment.ExecuteTask(HostedItem task, ModuleController module, RunTask cmd)
   en Firefly.Box.RegularTaskRunner.Execute(HostEnvironment host, HostedItem hostedItem, RunTask runTask, Action allowNestedRuns)
   en Firefly.Box.Task.Run(TaskRunner taskRunner)
   en Firefly.Box.Task.Run()
   en Firefly.Box.BusinessProcess.Run()
   en ENV.BusinessProcessBase.RunTheTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\BusinessProcessBase.cs:línea 478
   en ENV.ControllerBase.RunTask() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 658
   en ENV.ControllerBase.<>c.<.ctor>b__1_2(Action y) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 298
   en ENV.ControllerBase.Execute() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\ControllerBase.cs:línea 347

Application Callstack:
Unnamed Task - Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.PS_CapturaDeDatosAdicionales+Controller_CapturaDeDatosAdicionales
Unnamed Task - Iamsa.Mpa.Part1.PersonalAbordo.CapturaDA.PS_CapturaDeDatosAdicionales
Ejecuta Programas - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas+EjecutaProgramas
UT_EjecutaProgramas (P#8) - Iamsa.MenúPrincipal.MenúPrincipal.UT_EjecutaProgramas
  Parameters: E_nClaveProgramaEjecutar = 1031

Programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas+Programas
Ciclo entre menú y programas - Iamsa.MenúPrincipal.MenuPrincipalCore+CicloEntreMenúY_programas
Menu Principal (P#3) - Iamsa.MenúPrincipal.MenuPrincipalCore

Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499




