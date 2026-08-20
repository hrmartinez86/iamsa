## 18Ago2026
[x] agregar al método de hora la validacion del nombre de la columna del insert ,soloevaluar si el sql contiene insert, debe de ser una columna que comience con H, de lo contrario no elaborar el cambio de     regex del dato el cambio debe de contenerse dentro del metodo ChangeHourFormat
[x]cambio en el visor de vcel vrow, no esta reconociendo la columna nconsecutivo
    al cambiar en db2 muestra el error 
## 
    Error al intentar ejecutar la instrucción Select Query, Inner: ERROR [22018] [IBM][DB2/LINUXX8664] SQL0420N  Se ha encontrado un carácter no válido en un argumento de serie de caracteres de la función "BOOLEAN".
Error al intentar ejecutar la instrucción Select Query, Inner: ERROR [22018] [IBM][DB2/LINUXX8664] SQL0420N  Se ha encontrado un carácter no válido en un argumento de serie de caracteres de la función "BOOLEAN".
Task Crashed
Callstack:
   en Utilidades.ConsultasDirectas.DAL._ThrowException(String message, Exception innerException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 201
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Boolean throwException, String selectQuery, DbParameter[] parameters, Action`1 actionWithReader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 192
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Action`1 readEachRow, String sqlQuery, DbParameter[] parameters, Func`2 logicWhere, Boolean throwException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 111
   en AdicionesWorkFlowMPA.EventosDeEspera.Ejecucion.PS_HiloEventoEspera.ObtenerEventosDeEspera(SEventoDeEspera[]& eventos) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\AdicionesWorkFlowMPA\EventosDeEspera\Ejecucion\PS_HiloEventoEspera.cs:línea 140
   en AdicionesWorkFlowMPA.EventosDeEspera.Ejecucion.PS_HiloEventoEspera.<>c__DisplayClass10_0.<VerificarYEjecutar>b__0() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\AdicionesWorkFlowMPA\EventosDeEspera\Ejecucion\PS_HiloEventoEspera.cs:línea 68
   en Utilidades.ConsultasDirectas.ConsultasSQL.EjecutarAccionPostBloqueo() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\ConsultasSQL.cs:línea 713
   en Utilidades.ConsultasDirectas.ConsultasSQL.OnEnd() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\ConsultasSQL.cs:línea 497
   en ENV.BusinessProcessBase.<.ctor>b__3_3() en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\ENV\BusinessProcessBase.cs:línea 81
   en WizardOfOz.Witch.Engine.FlowEvent.Do()
   en Firefly.Box.BusinessProcess.TaskTypeBatch.RunTaskSection(FlowEvent flowEvent)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<>c__DisplayClass32.<Run>b__26()
   en Firefly.Box.RegularTaskRunner.RunTaskEnd(Action end)
   en Firefly.Box.Task.<>c__DisplayClass2b.<>c__DisplayClass2e.<>c__DisplayClass32.<Run>b__17()
   en Firefly.Box.DataAccess.Transactions.TaskTransactionContainer.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en WizardOfOz.Witch.Engine.CallStackClass.TaskInCallStack.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.Task.myTaskTransactionManager.RunMonitoredCommand(Action command, TransactionRollbackDelegate callMeIfYouRollbacked)
   en Firefly.Box.TransactionScopesStrategy.Task.RunMonitoredTaskLevelCommand(Action action)
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
Inner Error : ERROR [22018] [IBM][DB2/LINUXX8664] SQL0420N  Se ha encontrado un carácter no válido en un argumento de serie de caracteres de la función "BOOLEAN".
Inner Trace : 
   en IBM.Data.DB2.DB2DataReader.Read()
   en Utilidades.ConsultasDirectas.DAL.<>c__DisplayClass11_0.<_ExecuteSelectQuery>b__0(IDataReader reader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 129
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Boolean throwException, String selectQuery, DbParameter[] parameters, Action`1 actionWithReader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 184

Application Callstack:
Iamsa Version: 3.4.24.20499
Firefly Version: 3.4.24.20499
Callstack:
   en Utilidades.ConsultasDirectas.DAL._ThrowException(String message, Exception innerException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 201
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Boolean throwException, String selectQuery, DbParameter[] parameters, Action`1 actionWithReader) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 192
   en Utilidades.ConsultasDirectas.DAL._ExecuteSelectQuery(Action`1 readEachRow, String sqlQuery, DbParameter[] parameters, Func`2 logicWhere, Boolean throwException) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\Utilidades\ConsultasDirectas\DAL.cs:línea 111
   en AdicionesWorkFlowMPA.EventosDeEspera.Ejecucion.PS_HiloEventoEspera.ObtenerEventosDeEspera(SEventoDeEspera[]& eventos) en D:\Usuarios\c-hmartinez\SUCV80\Iamsa-postgress\AdicionesWorkFlowMPA\EventosDeEspera\Ejecucion\PS_HiloEventoEspera.cs:línea 140
-----------------
## 20 ago
[]74238 Generacion de flujos completos de WF,recomienda el conductor con registro en ultimo viaje
