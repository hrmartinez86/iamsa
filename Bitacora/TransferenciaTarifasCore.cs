using ENV;
using ENV.Data;
using Firefly.Box;
using Iamsa.MMTCC.Models;
using Iamsa.MMTCC.Types;
using Iamsa.Shared.ITrpServerGFA_WebReference;
using Iamsa.TarifasBase.Models.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Iamsa.Tarifas
{
    class TransferenciaTarifasCore : TransferenciaTarifas
    {
        #region Modelos
        readonly MMTCC.Models.GCTERMINALES GCTERMINALES = new MMTCC.Models.GCTERMINALES { Cached = false, ReadOnly = true };      
        readonly MMTCC.Models.GCPARAMETROS GCPARAMETROS_COWSPR = new MMTCC.Models.GCPARAMETROS { Cached = false, ReadOnly = true };
        readonly MMTCC.Models.GCPARAMETROS GCPARAMETROS_TRATVM = new MMTCC.Models.GCPARAMETROS { Cached = false, ReadOnly = true };
        readonly MMTCC.Models.GCPARAMETROS GCPARAMETROS_BOLIMP = new MMTCC.Models.GCPARAMETROS { Cached = false, ReadOnly = true };
        readonly MMTCC.Models.GCPARAMETROS GCPARAMETROS_RMSACT = new MMTCC.Models.GCPARAMETROS { Cached = false, ReadOnly = true };

        #endregion

        #region Columnas
        readonly MMTCC.Types.nFolio E_nClaveVersion = new MMTCC.Types.nFolio
        {
            Caption = "E_nClaveVersion"
        };
        readonly BoolColumn E_lSoloProductivo = new BoolColumn();
        readonly BoolColumn E_lSoloHistorico = new BoolColumn();
        public readonly NumberColumn v_NCONSECUTIVOHISTO = new NumberColumn("v_NCONSECUTIVOHISTO", "N9")
        {
            InputRange = @"\-2147483648-2147483647",
            AllowNull = false
        };
        public readonly NumberColumn v_NCLAVEVERSION = new NumberColumn("v_NCLAVEVERSION", "N9")
        {
            InputRange = @"\-2147483648-2147483647",
            AllowNull = false
        };
        private bool l_ExisteError = false;
        private bool l_ExisteRegistro = false;
        #endregion

        #region Columns Aux prod
        NumberColumn PRODNCONSECUTIVOHISTO = new NumberColumn("PRODNCONSECUTIVOHISTO", "N9");
        NumberColumn PRODNCONSECUTIVO = new NumberColumn("PRODNCONSECUTIVO", "N9");
        NumberColumn NCONSECUTIVOHISTORICO = new NumberColumn("NCONSECUTIVOHISTORICO", "N9");
        NumberColumn NCONSECUTIVO = new NumberColumn("NCONSECUTIVO", "N9");
        NumberColumn NCLAVEVERSION = new NumberColumn("NCLAVEVERSION", "N9");
        TextColumn ACLAVEOFICINAORIGEN = new TextColumn("ACLAVEOFICINAORIGEN");
        TextColumn ACLAVEOFICINADESTINO = new TextColumn("ACLAVEOFICINADESTINO");
        TextColumn NCLAVECLASESERVICIO = new TextColumn("NCLAVECLASESERVICIO");
        NumberColumn NCLAVERUTA = new NumberColumn("NCLAVERUTA", "N9");
        NumberColumn NTARIFA = new NumberColumn("NTARIFA", "N10.2");
        TextColumn ATIPOTARIFA = new TextColumn("ATIPOTARIFA");
        BoolColumn LAPLICAIVA = new BoolColumn("LAPLICAIVA");
        BoolColumn LLUNES = new BoolColumn("LLUNES");
        BoolColumn LMARTES = new BoolColumn("LMARTES");
        BoolColumn LMIERCOLES = new BoolColumn("LMIERCOLES");
        BoolColumn LJUEVES = new BoolColumn("LJUEVES");
        BoolColumn LVIERNES = new BoolColumn("LVIERNES");
        BoolColumn LSABADO = new BoolColumn("LSABADO");
        BoolColumn LDOMINGO = new BoolColumn("LDOMINGO");
        DateColumn FFECHAVENTAINICIO = new DateColumn("FFECHAVENTAINICIO", "DD/MM/YYYY")
        {
            AllowNull = false,
            Storage = new ENV.Data.Storage.DateDateStorage()
        };
        DateColumn FFECHAVENTAFIN = new DateColumn("FFECHAVENTAFIN", "DD/MM/YYYY")
        {
            AllowNull = false,
            Storage = new ENV.Data.Storage.DateDateStorage()
        };
        DateColumn FFECHAVIAJEINICIO = new DateColumn("FFECHAVIAJEINICIO", "DD/MM/YYYY")
        {
            AllowNull = false,
            Storage = new ENV.Data.Storage.DateDateStorage()
        };
        DateColumn FFECHAVIAJEFIN = new DateColumn("FFECHAVIAJEFIN", "DD/MM/YYYY")
        {
            AllowNull = false,
            Storage = new ENV.Data.Storage.DateDateStorage()
        };
        BoolColumn LTAQUILLA = new BoolColumn("LTAQUILLA");
        BoolColumn LWEB = new BoolColumn("LWEB");
        BoolColumn LAPP = new BoolColumn("LAPP");
        BoolColumn LCALLCENTER = new BoolColumn("LCALLCENTER");
        BoolColumn LMULTIEMPRESA = new BoolColumn("LMULTIEMPRESA");
        BoolColumn LKIOSKO = new BoolColumn("LKIOSKO");
        NumberColumn NAUCLAVETERMINAL = new NumberColumn("NAUCLAVETERMINAL", "N5");
        TextColumn AAUCLAVEOFICINA = new TextColumn("AAUCLAVEOFICINA", "4");
        TextColumn AAUCLAVEUSUARIO = new TextColumn("AAUCLAVEUSUARIO", "10");
        #endregion
        string v_aConsecutivoHistoricoCorrecto = "";
        string v_aConsecutivoHistoricoError = "";
        List<int> nVersionesTransferidas = new List<int>();
        int nContadorDeTarifasATransferir = 0;

        TransferenciaTarifasResponse S_Respuesta = new TransferenciaTarifasResponse();  
        Stopwatch stopwatch = new Stopwatch();

        public TransferenciaTarifasCore()
        {
            Title = "TransferenciaTarifasCore";
            InitializeDataView();
        }

        void InitializeDataView() 
        {
            #region Relaciones

            Relations.Add(GCTERMINALES,
            GCTERMINALES.NCLAVETERMINAL.IsEqualTo(() => u.Term()),
                GCTERMINALES.SortByIDX_0015_01);

            Relations.Add(GCPARAMETROS_COWSPR,
                    GCPARAMETROS_COWSPR.NCLAVEEMPRESA.IsEqualTo(GCTERMINALES.NCLAVEEMPRESA).And(
                    GCPARAMETROS_COWSPR.ACLAVEPARAMETRO.IsEqualTo("COWSPR")),
                GCPARAMETROS_COWSPR.SortByIDX_0011_01);

            Relations.Add(GCPARAMETROS_TRATVM,
                    GCPARAMETROS_TRATVM.NCLAVEEMPRESA.IsEqualTo(GCTERMINALES.NCLAVEEMPRESA).And(
                    GCPARAMETROS_TRATVM.ACLAVEPARAMETRO.IsEqualTo("TRATVM")),
                GCPARAMETROS_TRATVM.SortByIDX_0011_01);

            Relations.Add(GCPARAMETROS_BOLIMP,
                            GCPARAMETROS_BOLIMP.NCLAVEEMPRESA.IsEqualTo(GCTERMINALES.NCLAVEEMPRESA).And(
                            GCPARAMETROS_BOLIMP.ACLAVEPARAMETRO.IsEqualTo("BOLIMP")),
                            GCPARAMETROS_BOLIMP.SortByIDX_0011_01);

            Relations.Add(GCPARAMETROS_RMSACT,
                       GCPARAMETROS_RMSACT.NCLAVEEMPRESA.IsEqualTo(GCTERMINALES.NCLAVEEMPRESA).And(
                       GCPARAMETROS_RMSACT.ACLAVEPARAMETRO.IsEqualTo("RMSACT")),
                       GCPARAMETROS_RMSACT.SortByIDX_0011_01);


            #endregion

            #region Columnas

            Columns.Add(E_nClaveVersion);
            Columns.Add(E_lSoloProductivo);
            Columns.Add(E_lSoloHistorico);
            Columns.Add(v_NCONSECUTIVOHISTO);
            Columns.Add(v_NCLAVEVERSION);

            Columns.Add(GCTERMINALES.NCLAVETERMINAL);
            Columns.Add(GCTERMINALES.NCLAVEEMPRESA);
            Columns.Add(GCTERMINALES.ACLAVEOFICINA);
            Columns.Add(GCTERMINALES.AAUCLAVEUSUARIO);

            Columns.Add(GCPARAMETROS_COWSPR.LDATOLOGICO);
            Columns.Add(GCPARAMETROS_TRATVM.LDATOLOGICO);
            Columns.Add(GCPARAMETROS_BOLIMP.NDATONUMERICO);
            Columns.Add(GCPARAMETROS_RMSACT.LDATOLOGICO).Caption = "LDATOLOGICO_RMSACT";

            

            #endregion
        }

        public override TransferenciaTarifasResponse Run(NumberParameter pE_nClaveVersion, BoolParameter pE_lSoloProductivo, BoolParameter pE_lSoloHistorico)
        {
            BindParameter(E_nClaveVersion, pE_nClaveVersion);
            BindParameter(E_lSoloProductivo, pE_lSoloProductivo);
            BindParameter(E_lSoloHistorico, pE_lSoloHistorico);
            Execute();
            return S_Respuesta;
        }

        protected override void OnLoad()
        {
            Exit(ExitTiming.AfterRow);
            Activity = Activities.Browse;
        }

        protected override void OnLeaveRow()
        {
            if (!E_lSoloProductivo)
            {
                Cached<ActualizaTablaTrabajo>().Run();
                EliminaDuplicados();               
            }          

            if (E_lSoloHistorico)
            {
                l_ExisteError = true;
                Cached<VersionesTarifasHisto>().Run();                
            }
            if (!E_lSoloHistorico && !E_lSoloProductivo)
            {
                l_ExisteError = true;
                Cached<VersionesTarifasHisto>().Run();
                if (!l_ExisteError)
                {
                    stopwatch.Start();
                    ModificaTarifasProductivoSQL();
                    InsertaTarifasProductivoSQL();
                    stopwatch.Stop();
                    S_Respuesta.nCantidadTarifas = nContadorDeTarifasATransferir;
                    S_Respuesta.nTiempoTransferenciaTarifas = stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    GC.Collect();
                }
            }            
            if (E_lSoloProductivo)
            {
                stopwatch.Start();
                ModificaTarifasProductivoSQL();
                InsertaTarifasProductivoSQL();
                stopwatch.Stop();
                S_Respuesta.nCantidadTarifas = nContadorDeTarifasATransferir;
                S_Respuesta.nTiempoTransferenciaTarifas = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
                GC.Collect();                
            }
        }

        protected override void OnEnd()
        {
            if (v_aConsecutivoHistoricoError.Trim() != "")
            {
                ModificaHistoricoCorrectoIncorrecto(v_aConsecutivoHistoricoError, true);
            }
            else if (v_aConsecutivoHistoricoCorrecto.Trim() != "")
            {
                ModificaHistoricoCorrectoIncorrecto(v_aConsecutivoHistoricoCorrecto, false);
            }

            if (nVersionesTransferidas.Count != 0 && E_lSoloProductivo)
            {
                foreach (var item in nVersionesTransferidas)
                {
                    ModificaEstatusHistorico(item);
                }
            }
            else
            {
                ModificaEstatusHistorico(E_nClaveVersion);
            }


            LimpiaTablaProductivo();

            if (!l_ExisteError)
            {   
                ModificaTrabajoCorrecto();
                if (GCPARAMETROS_RMSACT.LDATOLOGICO)
                {
                    stopwatch.Start();
                    GeneraInformacionSarcan.CreateInstance().Run();
                    stopwatch.Stop();
                    S_Respuesta.nTiempoGeneracionInfoSarcan = stopwatch.ElapsedMilliseconds;
                }
                
            }
        }

        class VersionesTarifasHisto : BusinessProcessBase
        {

            #region Models
            readonly MMTCC.Models.CTTARIFASTRABAJOLAYOUT CTTARIFASTRABAJOLAYOUT = new MMTCC.Models.CTTARIFASTRABAJOLAYOUT { Cached = false, ReadOnly = true };
            #endregion           

            TransferenciaTarifasCore _parent;

            public VersionesTarifasHisto(TransferenciaTarifasCore parent)
            {
                _parent = parent;
                Title = "TarifasVersiones";
                InitializeDataView();
            }
            void InitializeDataView()
            {
                From = CTTARIFASTRABAJOLAYOUT;

                Where.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA.IsEqualTo(_parent.GCTERMINALES.NCLAVEEMPRESA));
                Where.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION.IsEqualTo(_parent.E_nClaveVersion));
                Where.Add(CTTARIFASTRABAJOLAYOUT.LMODIFICADO.IsEqualTo(true));
                Where.Add(CTTARIFASTRABAJOLAYOUT.LBAJA.IsEqualTo(false));

                OrderBy = CTTARIFASTRABAJOLAYOUT.CTTARIFASTRABAJOLAYOUT_01;

                
                #region Columns


                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCONSECUTIVO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINAORIGEN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINADESTINO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVECLASESERVICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVERUTA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NTARIFA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ATIPOTARIFA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICAIVA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICATARIFAREGRESO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NTARIFAREGRESO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LLUNES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMARTES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMIERCOLES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LJUEVES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LVIERNES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LSABADO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LDOMINGO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICAINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NPORCENTAJEINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NMONTOINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NREDONDEO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVENTAINICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVENTAFIN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEINICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEFIN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LTAQUILLA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LWEB);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPP);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LCALLCENTER);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMULTIEMPRESA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LKIOSKO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NAUCLAVETERMINAL);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.AAUCLAVEOFICINA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.AAUCLAVEUSUARIO);
                
                

                #endregion
            }

            internal void Run()
            {
                Execute();
            }
            
            protected override void OnLeaveRow()
            {
                Cached<Gc_BloqueoHisto>().Run();
                
            }

            class Gc_BloqueoHisto : BusinessProcessBase
            {

                #region Models
                readonly MMTCC.Models.GCBLOQUEOS GCBLOQUEOS = new MMTCC.Models.GCBLOQUEOS { Cached = false, AllowRowLocking = true };
                #endregion
                VersionesTarifasHisto _parent;

                public Gc_BloqueoHisto(VersionesTarifasHisto parent)
                {
                    _parent = parent;
                    Title = "Gc_Bloqueo";
                    InitializeDataView();
                }
                void InitializeDataView()
                {

                    Relations.Add(GCBLOQUEOS, RelationType.InsertIfNotFound,
                            GCBLOQUEOS.ACLAVETABLA.BindEqualTo("CTTarifasHistoLayout"),
                        GCBLOQUEOS.SortByIDX_911_01);



                    #region Columns

                    Columns.Add(GCBLOQUEOS.ACLAVETABLA);
                    Columns.Add(GCBLOQUEOS.FAUFECHA).BindValue(() => Date.Now);
                    Columns.Add(GCBLOQUEOS.HAUHORA).BindValue(() => Time.Now);
                    Columns.Add(GCBLOQUEOS.NAUCLAVETERMINAL).BindValue(_parent._parent.GCTERMINALES.NCLAVETERMINAL);
                    Columns.Add(GCBLOQUEOS.AAUCLAVEOFICINA).BindValue(_parent._parent.GCTERMINALES.ACLAVEOFICINA);
                    Columns.Add(GCBLOQUEOS.AAUCLAVEUSUARIO).BindValue(() => u.Upper(ENV.Security.UserManager.CurrentUser.Name));
                    #endregion
                }
                /// <summary>W_Bloqueos(P#26.1.1.1)</summary>
                internal void Run()
                {
                    Execute();
                }
                protected override void OnLoad()
                {
                    Exit(ExitTiming.AfterRow);
                    RowLocking = LockingStrategy.OnRowLoading;
                    TransactionScope = TransactionScopes.Task;
                }
                protected override void OnLeaveRow()
                {                    
                    GCBLOQUEOS.FAUFECHA.Value = Date.Now;
                    GCBLOQUEOS.HAUHORA.Value = Time.Now;
                    GCBLOQUEOS.NAUCLAVETERMINAL.Value = _parent._parent.GCTERMINALES.NCLAVETERMINAL;
                    GCBLOQUEOS.AAUCLAVEOFICINA.Value = _parent._parent.GCTERMINALES.ACLAVEOFICINA;
                    GCBLOQUEOS.AAUCLAVEUSUARIO.Value = u.Upper(ENV.Security.UserManager.CurrentUser.Name);
                    _parent._parent.l_ExisteError = true;
                    try
                    {
                        InsertaTarifasHistoricoSQL();
                    }
                    catch (Exception ex)
                    {
                        ENV.ErrorLog.WriteToLogFile($"Error al insertar la tarifa:  {_parent.CTTARIFASTRABAJOLAYOUT.NCONSECUTIVO} - en histórico. - {ex.Message} ");
                    }
                }

                /// <summary>
                /// InsertaTarifasProductivoSQL
                /// </summary>
                private void InsertaTarifasHistoricoSQL()
                {
                    MMTCC.Models.CTTARIFASHISTORICOLAYOUT1 CTTARIFASHISTORICOLAYOUT = new MMTCC.Models.CTTARIFASHISTORICOLAYOUT1 { Cached = false, AllowRowLocking = true };
                    var bp = new BusinessProcess { Activity = Activities.Insert, RowLocking = LockingStrategy.OnRowLoading, TransactionScope = TransactionScopes.Task };
                    bp.Exit(ExitTiming.AfterRow);
                    try
                    {
                        #region GUARDADO EN BD
                        bp.Relations.Add(CTTARIFASHISTORICOLAYOUT, RelationType.Insert);
                        bp.ForFirstRow(() =>
                        {
                            CTTARIFASHISTORICOLAYOUT.NCLAVEEMPRESA.Value = _parent.CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA;
                            CTTARIFASHISTORICOLAYOUT.NCONSECUTIVO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NCONSECUTIVO;
                            CTTARIFASHISTORICOLAYOUT.NCLAVEVERSION.Value = _parent.CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION;
                            CTTARIFASHISTORICOLAYOUT.ACLAVEOFICINAORIGEN.Value =  _parent.CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINAORIGEN;
                            CTTARIFASHISTORICOLAYOUT.ACLAVEOFICINADESTINO.Value = _parent.CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINADESTINO;
                            CTTARIFASHISTORICOLAYOUT.NCLAVECLASESERVICIO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NCLAVECLASESERVICIO;
                            CTTARIFASHISTORICOLAYOUT.NCLAVERUTA.Value = _parent.CTTARIFASTRABAJOLAYOUT.NCLAVERUTA;
                            CTTARIFASHISTORICOLAYOUT.NTARIFA.Value =  _parent.CTTARIFASTRABAJOLAYOUT.NTARIFA;
                            CTTARIFASHISTORICOLAYOUT.ATIPOTARIFA.Value = _parent.CTTARIFASTRABAJOLAYOUT.ATIPOTARIFA;
                            CTTARIFASHISTORICOLAYOUT.LAPLICAIVA.Value = _parent.CTTARIFASTRABAJOLAYOUT.LAPLICAIVA;
                            CTTARIFASHISTORICOLAYOUT.LAPLICATARIFAREGRESO.Value = _parent.CTTARIFASTRABAJOLAYOUT.LAPLICATARIFAREGRESO;
                            CTTARIFASHISTORICOLAYOUT.NTARIFAREGRESO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NTARIFAREGRESO;
                            CTTARIFASHISTORICOLAYOUT.LLUNES.Value = _parent.CTTARIFASTRABAJOLAYOUT.LLUNES;
                            CTTARIFASHISTORICOLAYOUT.LMARTES.Value = _parent.CTTARIFASTRABAJOLAYOUT.LMARTES;
                            CTTARIFASHISTORICOLAYOUT.LMIERCOLES.Value = _parent.CTTARIFASTRABAJOLAYOUT.LMIERCOLES;
                            CTTARIFASHISTORICOLAYOUT.LJUEVES.Value = _parent.CTTARIFASTRABAJOLAYOUT.LJUEVES;
                            CTTARIFASHISTORICOLAYOUT.LLVIERNES.Value = _parent.CTTARIFASTRABAJOLAYOUT.LVIERNES;
                            CTTARIFASHISTORICOLAYOUT.LSABADO.Value = _parent.CTTARIFASTRABAJOLAYOUT.LSABADO;
                            CTTARIFASHISTORICOLAYOUT.LDOMINGO.Value = _parent.CTTARIFASTRABAJOLAYOUT.LDOMINGO;
                            CTTARIFASHISTORICOLAYOUT.LAPLICAINCREMENTO.Value = _parent.CTTARIFASTRABAJOLAYOUT.LAPLICAINCREMENTO;
                            CTTARIFASHISTORICOLAYOUT.NPORCENTAJEINCREMENTO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NPORCENTAJEINCREMENTO;
                            CTTARIFASHISTORICOLAYOUT.NMONTOINCREMENTO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NMONTOINCREMENTO;
                            CTTARIFASHISTORICOLAYOUT.NREDONDEO.Value = _parent.CTTARIFASTRABAJOLAYOUT.NREDONDEO;
                            CTTARIFASHISTORICOLAYOUT.FFECHAVENTAINICIO.Value = _parent.CTTARIFASTRABAJOLAYOUT.FFECHAVENTAINICIO;
                            CTTARIFASHISTORICOLAYOUT.FFECHAVENTAFIN.Value = _parent.CTTARIFASTRABAJOLAYOUT.FFECHAVENTAFIN;
                            CTTARIFASHISTORICOLAYOUT.FFECHAVIAJEINICIO.Value = _parent.CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEINICIO;
                            CTTARIFASHISTORICOLAYOUT.FFECHAVIAJEFIN.Value = _parent.CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEFIN;
                            CTTARIFASHISTORICOLAYOUT.LTAQUILLA.Value = _parent.CTTARIFASTRABAJOLAYOUT.LTAQUILLA;
                            CTTARIFASHISTORICOLAYOUT.LWEB.Value = _parent.CTTARIFASTRABAJOLAYOUT.LWEB;
                            CTTARIFASHISTORICOLAYOUT.LAPP.Value = _parent.CTTARIFASTRABAJOLAYOUT.LAPP;
                            CTTARIFASHISTORICOLAYOUT.LCALLCENTER.Value = _parent.CTTARIFASTRABAJOLAYOUT.LCALLCENTER;
                            CTTARIFASHISTORICOLAYOUT.LMULTIEMPRESA.Value = _parent.CTTARIFASTRABAJOLAYOUT.LMULTIEMPRESA;
                            CTTARIFASHISTORICOLAYOUT.LKIOSKO.Value = _parent.CTTARIFASTRABAJOLAYOUT.LKIOSKO;
                            CTTARIFASHISTORICOLAYOUT.FAUFECHA.Value =  Date.Now;
                            CTTARIFASHISTORICOLAYOUT.HAUHORA.Value =  Time.Now;
                            CTTARIFASHISTORICOLAYOUT.FFECHATRANSFERENCIA.Value =  Date.Now;
                            CTTARIFASHISTORICOLAYOUT.HHORATRANSFERENCIA.Value =  Time.Now;
                            CTTARIFASHISTORICOLAYOUT.NAUCLAVETERMINAL.Value = _parent.CTTARIFASTRABAJOLAYOUT.NAUCLAVETERMINAL;
                            CTTARIFASHISTORICOLAYOUT.AAUCLAVEOFICINA.Value = _parent.CTTARIFASTRABAJOLAYOUT.AAUCLAVEOFICINA;
                            CTTARIFASHISTORICOLAYOUT.AAUCLAVEUSUARIO.Value = _parent.CTTARIFASTRABAJOLAYOUT.AAUCLAVEUSUARIO;
                            CTTARIFASHISTORICOLAYOUT.LPENDIENTETRANSFERENCIA.Value =  _parent._parent.E_lSoloHistorico && !_parent._parent.E_lSoloProductivo ? true : !_parent._parent.E_lSoloHistorico && !_parent._parent.E_lSoloProductivo ? true : false;
                            CTTARIFASHISTORICOLAYOUT.AMENSAJETRANSFERENCIA.Value =  _parent._parent.E_lSoloHistorico ? "PENDIENTE" : "";
                                                        
                            _parent._parent.l_ExisteError = false;
                        });
                        bp.Exit();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ENV.ErrorLog.WriteToLogFile($"Error al insertar la tarifa:  {_parent.CTTARIFASTRABAJOLAYOUT.NCONSECUTIVO} - en histórico. - {ex.Message} ");
                    }
                }
            }
        }
              
        class ActualizaTablaTrabajo : BusinessProcessBase
        {

            #region Models
            readonly MMTCC.Models.CTTARIFASTRABAJOLAYOUT CTTARIFASTRABAJOLAYOUT = new MMTCC.Models.CTTARIFASTRABAJOLAYOUT { Cached = false, ReadOnly = true };
            #endregion           
            private bool v_lValidaRegreso = false;
            TransferenciaTarifasCore _parent;

            public ActualizaTablaTrabajo(TransferenciaTarifasCore parent)
            {
                _parent = parent;
                Title = "TarifasVersiones";
                InitializeDataView();
            }
            void InitializeDataView()
            {
                From = CTTARIFASTRABAJOLAYOUT;

                Where.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA.IsEqualTo(_parent.GCTERMINALES.NCLAVEEMPRESA));
                Where.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION.IsEqualTo(_parent.E_nClaveVersion));
                Where.Add(CTTARIFASTRABAJOLAYOUT.LMODIFICADO.IsEqualTo(true));
                Where.Add(CTTARIFASTRABAJOLAYOUT.LBAJA.IsEqualTo(false));
                Where.Add(CTTARIFASTRABAJOLAYOUT.LAPLICATARIFAREGRESO.IsEqualTo(true));

                OrderBy = CTTARIFASTRABAJOLAYOUT.CTTARIFASTRABAJOLAYOUT_01;


                #region Columns


                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCONSECUTIVO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINAORIGEN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINADESTINO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVECLASESERVICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NCLAVERUTA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NTARIFA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.ATIPOTARIFA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICAIVA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICATARIFAREGRESO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NTARIFAREGRESO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LLUNES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMARTES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMIERCOLES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LJUEVES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LVIERNES);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LSABADO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LDOMINGO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPLICAINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NPORCENTAJEINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NMONTOINCREMENTO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NREDONDEO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVENTAINICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVENTAFIN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEINICIO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEFIN);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LTAQUILLA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LWEB);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LAPP);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LCALLCENTER);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LMULTIEMPRESA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.LKIOSKO);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.NAUCLAVETERMINAL);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.AAUCLAVEOFICINA);
                Columns.Add(CTTARIFASTRABAJOLAYOUT.AAUCLAVEUSUARIO);



                #endregion
            }

            internal void Run()
            {
                Execute();
            }

            protected override void OnLeaveRow()
            {               
                ActualizaTablaTrabajoSQL();
            }


            private void ActualizaTablaTrabajoSQL()
            {

                MMTCC.Models.CTTARIFASTRABAJOLAYOUT1 CTTARIFASTRABAJOLAYOUT1 = new MMTCC.Models.CTTARIFASTRABAJOLAYOUT1 { Cached = false, AllowRowLocking = true };
                var bp = new BusinessProcess { Activity = Activities.Insert, RowLocking = LockingStrategy.OnRowLoading, TransactionScope = TransactionScopes.Task };
                bp.Exit(ExitTiming.AfterRow);
                try
                {
                    #region GUARDADO EN BD
                    bp.Relations.Add(CTTARIFASTRABAJOLAYOUT1, RelationType.Insert);
                    bp.ForFirstRow(() =>
                    {
                        CTTARIFASTRABAJOLAYOUT1.NCLAVEEMPRESA.Value =  CTTARIFASTRABAJOLAYOUT.NCLAVEEMPRESA;
                        CTTARIFASTRABAJOLAYOUT1.NCLAVEVERSION.Value = CTTARIFASTRABAJOLAYOUT.NCLAVEVERSION;
                        CTTARIFASTRABAJOLAYOUT1.ACLAVEOFICINAORIGEN.Value = CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINADESTINO;
                        CTTARIFASTRABAJOLAYOUT1.ACLAVEOFICINADESTINO.Value = CTTARIFASTRABAJOLAYOUT.ACLAVEOFICINAORIGEN;
                        CTTARIFASTRABAJOLAYOUT1.NCLAVECLASESERVICIO.Value = CTTARIFASTRABAJOLAYOUT.NCLAVECLASESERVICIO;
                        CTTARIFASTRABAJOLAYOUT1.NCLAVERUTA.Value = CTTARIFASTRABAJOLAYOUT.NCLAVERUTA;
                        CTTARIFASTRABAJOLAYOUT1.NTARIFA.Value = CTTARIFASTRABAJOLAYOUT.NTARIFAREGRESO;
                        CTTARIFASTRABAJOLAYOUT1.ATIPOTARIFA.Value = CTTARIFASTRABAJOLAYOUT.ATIPOTARIFA;
                        CTTARIFASTRABAJOLAYOUT1.LAPLICAIVA.Value = CTTARIFASTRABAJOLAYOUT.LAPLICAIVA;
                        CTTARIFASTRABAJOLAYOUT1.LLUNES.Value = CTTARIFASTRABAJOLAYOUT.LLUNES;
                        CTTARIFASTRABAJOLAYOUT1.LMARTES.Value = CTTARIFASTRABAJOLAYOUT.LMARTES;
                        CTTARIFASTRABAJOLAYOUT1.LMIERCOLES.Value = CTTARIFASTRABAJOLAYOUT.LMIERCOLES;
                        CTTARIFASTRABAJOLAYOUT1.LJUEVES.Value = CTTARIFASTRABAJOLAYOUT.LJUEVES;
                        CTTARIFASTRABAJOLAYOUT1.LVIERNES.Value = CTTARIFASTRABAJOLAYOUT.LVIERNES;
                        CTTARIFASTRABAJOLAYOUT1.LSABADO.Value = CTTARIFASTRABAJOLAYOUT.LSABADO;
                        CTTARIFASTRABAJOLAYOUT1.LDOMINGO.Value = CTTARIFASTRABAJOLAYOUT.LDOMINGO;
                        CTTARIFASTRABAJOLAYOUT1.FFECHAVENTAINICIO.Value = CTTARIFASTRABAJOLAYOUT.FFECHAVENTAINICIO;
                        CTTARIFASTRABAJOLAYOUT1.FFECHAVENTAFIN.Value = CTTARIFASTRABAJOLAYOUT.FFECHAVENTAFIN;
                        CTTARIFASTRABAJOLAYOUT1.FFECHAVIAJEINICIO.Value = CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEINICIO;
                        CTTARIFASTRABAJOLAYOUT1.FFECHAVIAJEFIN.Value = CTTARIFASTRABAJOLAYOUT.FFECHAVIAJEFIN;
                        CTTARIFASTRABAJOLAYOUT1.LTAQUILLA.Value = CTTARIFASTRABAJOLAYOUT.LTAQUILLA;
                        CTTARIFASTRABAJOLAYOUT1.LWEB.Value = CTTARIFASTRABAJOLAYOUT.LWEB;
                        CTTARIFASTRABAJOLAYOUT1.LAPP.Value = CTTARIFASTRABAJOLAYOUT.LAPP;
                        CTTARIFASTRABAJOLAYOUT1.LCALLCENTER.Value = CTTARIFASTRABAJOLAYOUT.LCALLCENTER;
                        CTTARIFASTRABAJOLAYOUT1.LMULTIEMPRESA.Value = CTTARIFASTRABAJOLAYOUT.LMULTIEMPRESA;
                        CTTARIFASTRABAJOLAYOUT1.LKIOSKO.Value = CTTARIFASTRABAJOLAYOUT.LKIOSKO;
                        CTTARIFASTRABAJOLAYOUT1.FAUFECHA.Value = Date.Now;
                        CTTARIFASTRABAJOLAYOUT1.HAUHORA.Value = Time.Now;
                        CTTARIFASTRABAJOLAYOUT1.NAUCLAVETERMINAL.Value = CTTARIFASTRABAJOLAYOUT.NAUCLAVETERMINAL;
                        CTTARIFASTRABAJOLAYOUT1.AAUCLAVEOFICINA.Value = CTTARIFASTRABAJOLAYOUT.AAUCLAVEOFICINA;
                        CTTARIFASTRABAJOLAYOUT1.AAUCLAVEUSUARIO.Value = CTTARIFASTRABAJOLAYOUT.AAUCLAVEUSUARIO;
                        CTTARIFASTRABAJOLAYOUT1.LMODIFICADO.Value = true;
                    });
                    bp.Exit();
                    #endregion

                }
                catch (Exception ex)
                {                    
                    ENV.ErrorLog.WriteToLogFile($"ActualizaTablaTrabajoSQL - {ex.Message} ");
                }
               
            }
            
        }


        private void  ModificaTarifasProductivoSQL()
        {
            try
            {
                int v_ContadorModificados = 0;  
                string aQuery = string.Format(@"SELECT prod.NCONSECUTIVOHISTO,prod.NCONSECUTIVO, histo.NCONSECUTIVOHISTORICO,histo.NCONSECUTIVO,
                        histo.NCLAVEVERSION,histo.ACLAVEOFICINAORIGEN,histo.ACLAVEOFICINADESTINO,
                        histo.NCLAVECLASESERVICIO,histo.NCLAVERUTA,histo.NTARIFA,histo.ATIPOTARIFA,histo.LAPLICAIVA,histo.LLUNES,histo.LMARTES,
                        histo.LMIERCOLES,histo.LJUEVES,histo.LLVIERNES,histo.LSABADO,histo.LDOMINGO,histo.FFECHAVENTAINICIO,histo.FFECHAVENTAFIN,
                        histo.FFECHAVIAJEINICIO,histo.FFECHAVIAJEFIN,histo.LTAQUILLA,histo.LWEB,histo.LAPP,histo.LCALLCENTER,histo.LMULTIEMPRESA,
                        histo.LKIOSKO,histo.NAUCLAVETERMINAL,histo.AAUCLAVEOFICINA,histo.AAUCLAVEUSUARIO
                        FROM CTTARIFASHISTORICOLAYOUT histo 
                        JOIN CTTARIFASPRODUCCIONLAYOUT prod 
                        ON prod.NCLAVEEMPRESA = histo.NCLAVEEMPRESA
                        AND prod.NCLAVEVERSION = histo.NCLAVEVERSION 
                        AND prod.NCONSECUTIVO = histo.NCONSECUTIVO 
                        WHERE histo.NCLAVEEMPRESA = {0}  {1} AND LPENDIENTETRANSFERENCIA = X'01' ORDER BY histo.NCONSECUTIVO, histo.NCLAVEVERSION",
                    GCTERMINALES.NCLAVEEMPRESA, //:0;
                    !E_lSoloProductivo ? "AND histo.NCLAVEVERSION ="+E_nClaveVersion : "");//:1

                ENV.Data.DynamicSQLEntity SQL = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC, aQuery);
                SQL.Columns.Add(PRODNCONSECUTIVOHISTO, PRODNCONSECUTIVO, NCONSECUTIVOHISTORICO, NCONSECUTIVO, NCLAVEVERSION, ACLAVEOFICINAORIGEN, ACLAVEOFICINADESTINO,
                    NCLAVECLASESERVICIO, NCLAVERUTA, NTARIFA, ATIPOTARIFA, LAPLICAIVA, LLUNES, LMARTES, LMIERCOLES, LJUEVES,
                    LVIERNES, LSABADO, LDOMINGO, FFECHAVENTAINICIO, FFECHAVENTAFIN, FFECHAVIAJEINICIO, FFECHAVIAJEFIN,
                    LTAQUILLA, LWEB, LAPP, LCALLCENTER, LMULTIEMPRESA,LKIOSKO, NAUCLAVETERMINAL, AAUCLAVEOFICINA, AAUCLAVEUSUARIO);
                var bp = new BusinessProcess() { From = SQL, Activity = Activities.Browse };
                bp.ForEachRow(() =>
                {
                    try
                    {
                        ENV.Data.DynamicSQLEntity update = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                        @"UPDATE CTTARIFASPRODUCCIONLAYOUT SET NCONSECUTIVOHISTO = :1 ,NCONSECUTIVO = :2 ,NCLAVEVERSION = :3 ,
                                ACLAVEOFICINAORIGEN = ':4',ACLAVEOFICINADESTINO = ':5',NCLAVECLASESERVICIO = ':6',NCLAVERUTA = :7 ,NTARIFA = :8,
                                ATIPOTARIFA = ':9',LAPLICAIVA = X':10',LLUNES = X':11',LMARTES = X':12',LMIERCOLES = X':13',LJUEVES = X':14',
                                LVIERNES = X':15',LSABADO = X':16',LDOMINGO = X':17',FFECHAVENTAINICIO = ':18',FFECHAVENTAFIN = ':19',
                                FFECHAVIAJEINICIO = ':20',FFECHAVIAJEFIN = ':21',LTAQUILLA = X':22',LWEB = X':23',LAPP = X':24',LCALLCENTER = X':25',
                                LMULTIEMPRESA = X':26',LKIOSKO = X':27',NAUCLAVETERMINAL = :28,AAUCLAVEOFICINA = ':29',AAUCLAVEUSUARIO = ':30',
                                FAUFECHA = CURRENT date , HAUHORA = CURRENT time , FFECHATRANSFERENCIA = CURRENT date , 
                                HHORATRANSFERENCIA = CURRENT time, LENVIADOSARCAN = X'00'
                                WHERE NCLAVEEMPRESA = :31 AND NCLAVEVERSION = :32 AND NCONSECUTIVO = :33 AND NCONSECUTIVOHISTO = :34");

                        update.AddParameter(() => NCONSECUTIVOHISTORICO);//1
                        update.AddParameter(() => NCONSECUTIVO);//2
                        update.AddParameter(() => NCLAVEVERSION);//3
                        update.AddParameter(() => ACLAVEOFICINAORIGEN);//4
                        update.AddParameter(() => ACLAVEOFICINADESTINO);//5
                        update.AddParameter(() => NCLAVECLASESERVICIO);//6
                        update.AddParameter(() => NCLAVERUTA);//7
                        update.AddParameter(() => NTARIFA);//8
                        update.AddParameter(() => ATIPOTARIFA);//9
                        update.AddParameter(() => LAPLICAIVA ? "01" : "00");//10
                        update.AddParameter(() => LLUNES ? "01" : "00");//11
                        update.AddParameter(() => LMARTES ? "01" : "00");//12
                        update.AddParameter(() => LMIERCOLES ? "01" : "00");//13
                        update.AddParameter(() => LJUEVES ? "01" : "00");//14
                        update.AddParameter(() => LVIERNES ? "01" : "00");//15
                        update.AddParameter(() => LSABADO ? "01" : "00");//16
                        update.AddParameter(() => LDOMINGO ? "01" : "00");//17
                        update.AddParameter(() => FFECHAVENTAINICIO.ToString("DD/MM/YYYY"));//18
                        update.AddParameter(() => FFECHAVENTAFIN.ToString("DD/MM/YYYY"));//19
                        update.AddParameter(() => FFECHAVIAJEINICIO.ToString("DD/MM/YYYY"));//20
                        update.AddParameter(() => FFECHAVIAJEFIN.ToString("DD/MM/YYYY"));//21
                        update.AddParameter(() => LTAQUILLA ? "01" : "00");//22
                        update.AddParameter(() => LWEB ? "01" : "00");//23
                        update.AddParameter(() => LAPP ? "01" : "00");//24
                        update.AddParameter(() => LCALLCENTER ? "01" : "00");//25
                        update.AddParameter(() => LMULTIEMPRESA ? "01" : "00");//26
                        update.AddParameter(() => LKIOSKO ? "01" : "00");//27
                        update.AddParameter(() => NAUCLAVETERMINAL);//28
                        update.AddParameter(() => AAUCLAVEOFICINA);//29
                        update.AddParameter(() => AAUCLAVEUSUARIO);//30
                        update.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);//31
                        update.AddParameter(() => !E_lSoloProductivo ? E_nClaveVersion : NCLAVEVERSION);//32
                        update.AddParameter(() => PRODNCONSECUTIVO);//33
                        update.AddParameter(() => PRODNCONSECUTIVOHISTO);//34

                        var updatetabla = new BusinessProcess { From = update };
                        updatetabla.Exit(ExitTiming.AfterRow);
                        updatetabla.Run();
                        updatetabla.Exit();
                        nContadorDeTarifasATransferir++;

                        #region contador de versiones de tarea programada 
                        if (E_lSoloProductivo)
                        {
                            var v_nVersionModificada = nVersionesTransferidas.Find(x => x == NCLAVEVERSION);
                            if (v_nVersionModificada == 0)
                                nVersionesTransferidas.Add(NCLAVEVERSION);
                        }                        
                        #endregion                        

                        v_ContadorModificados++;
                        if (v_aConsecutivoHistoricoCorrecto != "")
                            v_aConsecutivoHistoricoCorrecto = v_aConsecutivoHistoricoCorrecto + ",";

                        v_aConsecutivoHistoricoCorrecto = v_aConsecutivoHistoricoCorrecto + NCONSECUTIVOHISTORICO;
                        if (v_ContadorModificados == 1000)
                        {
                            ModificaHistoricoCorrectoIncorrecto(v_aConsecutivoHistoricoCorrecto, false);
                            v_ContadorModificados = 0;
                            v_aConsecutivoHistoricoCorrecto = "";
                        }
                        S_Respuesta.nTarifasModificadas++;
                        S_Respuesta.lsTarifasTransferidas.Add(NCONSECUTIVOHISTORICO.Value);
                        l_ExisteError = false;
                    }
                    catch (Exception ex)
                    {

                        ENV.ErrorLog.WriteToLogFile($"Error al modificar el consecutivo:  {NCONSECUTIVOHISTORICO} - en productivo. - {ex.Message} ");
                        if (v_aConsecutivoHistoricoError != "")
                            v_aConsecutivoHistoricoError = v_aConsecutivoHistoricoError + ",";

                        v_aConsecutivoHistoricoError = v_aConsecutivoHistoricoError + NCONSECUTIVOHISTORICO;

                        S_Respuesta.nTarifasError++;
                        S_Respuesta.lsTarifasNoTransferidas.Add(NCONSECUTIVOHISTORICO);
                    }
                });
                bp.Exit();
            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile($"Error en InsertaModificaTarifasProductivoSQL. - {ex.Message}");
            }            
        }

        private void InsertaTarifasProductivoSQL()
        {
            int v_ContadorInsertados = 0;
            MMTCC.Models.CTTARIFASPRODUCCIONLAYOUT CTTARIFASPRODUCCIONLAYOUT = new MMTCC.Models.CTTARIFASPRODUCCIONLAYOUT { Cached = false, AllowRowLocking = true };
            try
            {
                string aQuery = string.Format(@"SELECT NCONSECUTIVOHISTORICO,NCONSECUTIVO,NCLAVEVERSION,ACLAVEOFICINAORIGEN,ACLAVEOFICINADESTINO,
            NCLAVECLASESERVICIO,NCLAVERUTA,NTARIFA,ATIPOTARIFA,LAPLICAIVA,LLUNES,LMARTES,LMIERCOLES,LJUEVES,
            LLVIERNES,LSABADO,LDOMINGO,FFECHAVENTAINICIO,FFECHAVENTAFIN,FFECHAVIAJEINICIO,FFECHAVIAJEFIN,
            LTAQUILLA,LWEB,LAPP,LCALLCENTER,LMULTIEMPRESA,LKIOSKO,NAUCLAVETERMINAL,AAUCLAVEOFICINA,AAUCLAVEUSUARIO
            FROM CTTARIFASHISTORICOLAYOUT histo WHERE histo.NCLAVEEMPRESA = {0} {1} AND LPENDIENTETRANSFERENCIA = X'01' AND histo.NCONSECUTIVO NOT IN  
            (SELECT prod.NCONSECUTIVO FROM CTTARIFASPRODUCCIONLAYOUT prod WHERE prod.NCLAVEEMPRESA = {0} {2})",
            GCTERMINALES.NCLAVEEMPRESA, //:0;
            !E_lSoloProductivo ? "AND histo.NCLAVEVERSION = " + E_nClaveVersion : "",//:1
            !E_lSoloProductivo ? "AND prod.NCLAVEVERSION = " + E_nClaveVersion : ""//:2
            );

                ENV.Data.DynamicSQLEntity SQL = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC, aQuery);
                SQL.Columns.Add(NCONSECUTIVOHISTORICO, NCONSECUTIVO, NCLAVEVERSION, ACLAVEOFICINAORIGEN, ACLAVEOFICINADESTINO,
                    NCLAVECLASESERVICIO, NCLAVERUTA, NTARIFA, ATIPOTARIFA, LAPLICAIVA, LLUNES, LMARTES, LMIERCOLES, LJUEVES,
                    LVIERNES, LSABADO, LDOMINGO, FFECHAVENTAINICIO, FFECHAVENTAFIN, FFECHAVIAJEINICIO, FFECHAVIAJEFIN,
                    LTAQUILLA, LWEB, LAPP, LCALLCENTER, LMULTIEMPRESA, LKIOSKO,NAUCLAVETERMINAL, AAUCLAVEOFICINA, AAUCLAVEUSUARIO);
                var bp = new BusinessProcess() { From = SQL, Activity = Activities.Browse };
                bp.ForEachRow(() =>
                {
                    var insert = new BusinessProcess { Activity = Activities.Insert, RowLocking = LockingStrategy.OnRowLoading, TransactionScope = TransactionScopes.Task };
                    insert.Exit(ExitTiming.AfterRow);
                    try
                    {
                        #region GUARDADO EN BD
                        insert.Relations.Add(CTTARIFASPRODUCCIONLAYOUT, RelationType.Insert);
                        insert.ForFirstRow(() =>
                        {
                            CTTARIFASPRODUCCIONLAYOUT.NCLAVEEMPRESA.Value = GCTERMINALES.NCLAVEEMPRESA;
                            CTTARIFASPRODUCCIONLAYOUT.NCONSECUTIVO.Value =NCONSECUTIVO;
                            CTTARIFASPRODUCCIONLAYOUT.NCONSECUTIVOHISTO.Value = NCONSECUTIVOHISTORICO;
                            CTTARIFASPRODUCCIONLAYOUT.NCLAVEVERSION.Value = NCLAVEVERSION;
                            CTTARIFASPRODUCCIONLAYOUT.ACLAVEOFICINAORIGEN.Value = ACLAVEOFICINAORIGEN;
                            CTTARIFASPRODUCCIONLAYOUT.ACLAVEOFICINADESTINO.Value = ACLAVEOFICINADESTINO;
                            CTTARIFASPRODUCCIONLAYOUT.NCLAVECLASESERVICIO.Value = NCLAVECLASESERVICIO;
                            CTTARIFASPRODUCCIONLAYOUT.NCLAVERUTA.Value = NCLAVERUTA;
                            CTTARIFASPRODUCCIONLAYOUT.NTARIFA.Value = NTARIFA;
                            CTTARIFASPRODUCCIONLAYOUT.ATIPOTARIFA.Value = ATIPOTARIFA;
                            CTTARIFASPRODUCCIONLAYOUT.LAPLICAIVA.Value = LAPLICAIVA;
                            CTTARIFASPRODUCCIONLAYOUT.LLUNES.Value = LLUNES;
                            CTTARIFASPRODUCCIONLAYOUT.LMARTES.Value = LMARTES;
                            CTTARIFASPRODUCCIONLAYOUT.LMIERCOLES.Value = LMIERCOLES;
                            CTTARIFASPRODUCCIONLAYOUT.LJUEVES.Value = LJUEVES;
                            CTTARIFASPRODUCCIONLAYOUT.LVIERNES.Value = LVIERNES;
                            CTTARIFASPRODUCCIONLAYOUT.LSABADO.Value = LSABADO;
                            CTTARIFASPRODUCCIONLAYOUT.LDOMINGO.Value = LDOMINGO;
                            CTTARIFASPRODUCCIONLAYOUT.FFECHAVENTAINICIO.Value = FFECHAVENTAINICIO;
                            CTTARIFASPRODUCCIONLAYOUT.FFECHAVENTAFIN.Value = FFECHAVENTAFIN;
                            CTTARIFASPRODUCCIONLAYOUT.FFECHAVIAJEINICIO.Value = FFECHAVIAJEINICIO;
                            CTTARIFASPRODUCCIONLAYOUT.FFECHAVIAJEFIN.Value = FFECHAVIAJEFIN;
                            CTTARIFASPRODUCCIONLAYOUT.LTAQUILLA.Value = LTAQUILLA;
                            CTTARIFASPRODUCCIONLAYOUT.LWEB.Value = LWEB;
                            CTTARIFASPRODUCCIONLAYOUT.LAPP.Value = LAPP;
                            CTTARIFASPRODUCCIONLAYOUT.LCALLCENTER.Value = LCALLCENTER;
                            CTTARIFASPRODUCCIONLAYOUT.LMULTIEMPRESA.Value = LMULTIEMPRESA;
                            CTTARIFASPRODUCCIONLAYOUT.LKIOSKO.Value = LKIOSKO;
                            CTTARIFASPRODUCCIONLAYOUT.FAUFECHA.Value = Date.Now;
                            CTTARIFASPRODUCCIONLAYOUT.HAUHORA.Value = Time.Now;
                            CTTARIFASPRODUCCIONLAYOUT.FFECHATRANSFERENCIA.Value = Date.Now;
                            CTTARIFASPRODUCCIONLAYOUT.HHORATRANSFERENCIA.Value = Time.Now;
                            CTTARIFASPRODUCCIONLAYOUT.NAUCLAVETERMINAL.Value = NAUCLAVETERMINAL;
                            CTTARIFASPRODUCCIONLAYOUT.AAUCLAVEOFICINA.Value = AAUCLAVEOFICINA;
                            CTTARIFASPRODUCCIONLAYOUT.AAUCLAVEUSUARIO.Value = AAUCLAVEUSUARIO;
                            CTTARIFASPRODUCCIONLAYOUT.LENVIADOSARCAN.Value = false;
                        });
                        insert.Exit();
                        nContadorDeTarifasATransferir++;
                        #endregion

                        #region contador de versiones de tarea programada 
                        if (E_lSoloProductivo)
                        {
                            var v_nVersionModificada = nVersionesTransferidas.Find(x => x == NCLAVEVERSION);
                            if (v_nVersionModificada == 0)
                                nVersionesTransferidas.Add(NCLAVEVERSION);
                        }
                        #endregion

                        v_ContadorInsertados++;
                        if (v_aConsecutivoHistoricoCorrecto != "")
                            v_aConsecutivoHistoricoCorrecto = v_aConsecutivoHistoricoCorrecto + ",";

                        v_aConsecutivoHistoricoCorrecto = v_aConsecutivoHistoricoCorrecto + NCONSECUTIVOHISTORICO;
                        if (v_ContadorInsertados == 1000)
                        {
                            ModificaHistoricoCorrectoIncorrecto(v_aConsecutivoHistoricoCorrecto, false);
                            v_ContadorInsertados = 0;
                            v_aConsecutivoHistoricoCorrecto = "";
                        }
                        S_Respuesta.nTarifasModificadas++;
                        S_Respuesta.lsTarifasTransferidas.Add(NCONSECUTIVOHISTORICO.Value);
                        l_ExisteError = false;
                    }
                    catch (Exception ex)
                    {
                        ENV.ErrorLog.WriteToLogFile($"Error al insertar el consecutivo:  {NCONSECUTIVOHISTORICO} - en productivo. - {ex.Message} ");
                        if (v_aConsecutivoHistoricoError != "")
                            v_aConsecutivoHistoricoError = v_aConsecutivoHistoricoError + ",";

                        v_aConsecutivoHistoricoError = v_aConsecutivoHistoricoError + NCONSECUTIVOHISTORICO;

                        S_Respuesta.nTarifasError++;
                        S_Respuesta.lsTarifasNoTransferidas.Add(NCONSECUTIVOHISTORICO);
                    }

                });
                bp.Exit();

            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile($"Error en InsertaTarifasProductivoSQL. - {ex.Message} ");
            }   
        }


        private void ModificaHistoricoCorrectoIncorrecto(string aConsecutivosHistorico, bool l_ExisteError)
        {
            try
            {
                ENV.Data.DynamicSQLEntity update = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                @"UPDATE MAGICADM.CTTARIFASHISTORICOLAYOUT 
                            SET AMENSAJETRANSFERENCIA = ':1',
                            LPENDIENTETRANSFERENCIA = X':2',
                            LERRORTRANSFERENCIA  = X':2',
                            FFECHATRANSFERENCIA = CURRENT DATE,
                            HHORATRANSFERENCIA =  CURRENT TIME
                            WHERE NCLAVEEMPRESA = :3 AND  NCONSECUTIVOHISTORICO  IN (:4)");

                update.AddParameter(() => l_ExisteError ? "ERROR AL TRANSFERIR" : "CORRECTO");
                update.AddParameter(() => l_ExisteError ? "01" : "00");
                update.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                update.AddParameter(() => aConsecutivosHistorico);

                var updatetabla = new BusinessProcess { From = update };
                updatetabla.Exit(ExitTiming.AfterRow);
                updatetabla.Run();
            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile($"Error al modificar los consecutivos:  {aConsecutivosHistorico} - en histórico. - {ex.Message} ");
            }
        }

        private void ModificaTrabajoCorrecto()
        {
            try
            {
                ENV.Data.DynamicSQLEntity update = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                    @"UPDATE MAGICADM.CTTARIFASTRABAJOLAYOUT SET NTARIFAREGRESO = 0,LAPLICATARIFAREGRESO = X'00',LMODIFICADO = X'00', LAPLICAINCREMENTO = X'00' WHERE NCLAVEEMPRESA = :1 AND NCLAVEVERSION = :2 AND LBAJA = X'00'");
                
                update.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                update.AddParameter(() => E_nClaveVersion);

                var updatetabla = new BusinessProcess { From = update };
                updatetabla.Exit(ExitTiming.AfterRow);
                updatetabla.Run();
            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile("ModificaTrabajoCorrecto: " + ex.Message);
            }
        }

        private void LimpiaTablaProductivo()
        {
            try
            {
                NumberColumn NCONSECUTIVO = new NumberColumn("NCONSECUTIVO", "N9");
                string aQuery = "";
                if (!E_lSoloProductivo)
                {
                     aQuery = string.Format(@"SELECT NCONSECUTIVO FROM MAGICADM.CTTARIFASTRABAJOLAYOUT WHERE 
                                                NCLAVEEMPRESA = {0} AND LBAJA = X'01'  AND NCLAVEVERSION = {1}",
                                                GCTERMINALES.NCLAVEEMPRESA, //:0;
                                                E_nClaveVersion //:1;
                                                );
                }
                else
                {
                     aQuery = string.Format(@"SELECT NCONSECUTIVO FROM CTTARIFASTRABAJOLAYOUT AS TRA
                                                JOIN CTTARIFASVERSIONESLAYOUT AS VER ON TRA.NCLAVEEMPRESA = VER.NCLAVEEMPRESA AND VER.LPRODUCTIVO = X'01' 
                                                AND TRA.NCLAVEVERSION = VER.NCLAVEVERSION 
                                                WHERE TRA.NCLAVEEMPRESA = {0} AND TRA.LBAJA = X'01'",
                                                GCTERMINALES.NCLAVEEMPRESA);
                }
                
                ENV.Data.DynamicSQLEntity SQL = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC, aQuery);
                SQL.Columns.Add(NCONSECUTIVO);
                var bp = new BusinessProcess() { From = SQL, Activity = Activities.Browse };
                bp.ForEachRow(() =>
                {
                    ENV.Data.DynamicSQLEntity delete = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                    @"DELETE FROM MAGICADM.CTTARIFASPRODUCCIONLAYOUT WHERE NCLAVEEMPRESA = :1 AND NCONSECUTIVO = :2 "+ 
                    (!E_lSoloProductivo ? " AND NCLAVEVERSION = :3" : ""));

                    delete.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                    delete.AddParameter(() => NCONSECUTIVO);
                    delete.AddParameter(() => E_nClaveVersion);

                    var deleteregistros = new BusinessProcess { From = delete };
                    deleteregistros.Exit(ExitTiming.AfterRow);
                    deleteregistros.Run();
                });
                bp.Exit();
                
            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile("LimpiaTablaProductivo: " + ex.Message);
            }
        }

        private void EliminaDuplicados()
        {

            string aQuery = string.Format(@"DELETE FROM CTTARIFASTRABAJOLAYOUT WHERE NCLAVEEMPRESA = {0} AND NCLAVEVERSION = {1} AND NCONSECUTIVO IN 
            (SELECT NCONSECUTIVO  
	        FROM CTTARIFASTRABAJOLAYOUT tar1
	        JOIN (
	        SELECT NCLAVEVERSION,ACLAVEOFICINAORIGEN,ACLAVEOFICINADESTINO,NCLAVECLASESERVICIO,NCLAVERUTA,ATIPOTARIFA,LAPLICAIVA,LAPLICATARIFAREGRESO,LLUNES,LMARTES,LMIERCOLES,LJUEVES,LVIERNES,LSABADO,LDOMINGO,FFECHAVENTAINICIO,FFECHAVENTAFIN,FFECHAVIAJEINICIO,FFECHAVIAJEFIN,LTAQUILLA,LWEB,LAPP,LCALLCENTER,LMULTIEMPRESA,LKIOSKO, LBAJA , MAX(NCONSECUTIVO) AS MaxID
	        FROM CTTARIFASTRABAJOLAYOUT WHERE NCLAVEEMPRESA = {0} AND NCLAVEVERSION = {1}
	        GROUP BY NCLAVEVERSION,ACLAVEOFICINAORIGEN,ACLAVEOFICINADESTINO,NCLAVECLASESERVICIO,NCLAVERUTA,ATIPOTARIFA,LAPLICAIVA,LAPLICATARIFAREGRESO,LLUNES,LMARTES,LMIERCOLES,LJUEVES,LVIERNES,LSABADO,LDOMINGO,FFECHAVENTAINICIO,FFECHAVENTAFIN,FFECHAVIAJEINICIO,FFECHAVIAJEFIN,LTAQUILLA,LWEB,LAPP,LCALLCENTER,LMULTIEMPRESA,LKIOSKO, LBAJA 
	        ) tar2
	        ON tar1.NCLAVEVERSION = tar2.NCLAVEVERSION
	        AND tar1.ACLAVEOFICINAORIGEN = tar2.ACLAVEOFICINAORIGEN 
	        AND tar1.ACLAVEOFICINADESTINO = tar2.ACLAVEOFICINADESTINO
	        AND tar1.NCLAVECLASESERVICIO = tar2.NCLAVECLASESERVICIO
	        AND tar1.NCLAVERUTA = tar2.NCLAVERUTA
	        AND tar1.ATIPOTARIFA = tar2.ATIPOTARIFA
	        AND tar1.LAPLICAIVA = tar2.LAPLICAIVA
	        AND tar1.LLUNES = tar2.LLUNES
	        AND tar1.LMARTES = tar2.LMARTES
	        AND tar1.LMIERCOLES = tar2.LMIERCOLES
	        AND tar1.LJUEVES = tar2.LJUEVES
	        AND tar1.LVIERNES = tar2.LVIERNES
	        AND tar1.LSABADO = tar2.LSABADO
	        AND tar1.LDOMINGO = tar2.LDOMINGO
	        AND tar1.FFECHAVENTAINICIO = tar2.FFECHAVENTAINICIO
	        AND tar1.FFECHAVENTAFIN = tar2.FFECHAVENTAFIN
	        AND tar1.FFECHAVIAJEINICIO = tar2.FFECHAVIAJEINICIO
	        AND tar1.FFECHAVIAJEFIN = tar2.FFECHAVIAJEFIN
	        AND tar1.LTAQUILLA = tar2.LTAQUILLA
	        AND tar1.LWEB = tar2.LWEB
	        AND tar1.LAPP = tar2.LAPP
	        AND tar1.LCALLCENTER = tar2.LCALLCENTER
	        AND tar1.LMULTIEMPRESA = tar2.LMULTIEMPRESA
	        AND tar1.LKIOSKO = tar2.LKIOSKO
            AND tar1.LBAJA = tar2.LBAJA
	        AND tar1.NCONSECUTIVO < tar2.MaxID 
	        WHERE tar1.NCLAVEEMPRESA = {0} AND tar1.NCLAVEVERSION = {1})", GCTERMINALES.NCLAVEEMPRESA, E_nClaveVersion);

            ENV.Data.DynamicSQLEntity SQL = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC, aQuery);            
            var bp = new BusinessProcess() { From = SQL};
            bp.Exit(ExitTiming.AfterRow);
            bp.Run();
            //bp.Exit();
        }

        private void ModificaEstatusHistorico(int nVersion)
        {
            int v_nContadorRegistros = 0;
            string v_aConsecutivosAmodificar = "";
            try
            {                
                NumberColumn nConsecutivoHisto = new NumberColumn("nConsecutivoHisto", "N9");
                ENV.Data.DynamicSQLEntity update = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                @"UPDATE MAGICADM.CTTARIFASHISTORICOLAYOUT 
                            SET LPRODUCCION = X'00'
                            WHERE NCLAVEEMPRESA = :1 AND  NCLAVEVERSION  = :2");

                update.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                update.AddParameter(() => nVersion);

                var updatetabla = new BusinessProcess { From = update };
                updatetabla.Exit(ExitTiming.AfterRow);
                updatetabla.Run();
                updatetabla.Exit();

                string buscar = string.Format(@"SELECT NCONSECUTIVOHISTO FROM CTTARIFASPRODUCCIONLAYOUT WHERE NCLAVEEMPRESA = {0} AND NCLAVEVERSION = {1}",
                                            GCTERMINALES.NCLAVEEMPRESA, nVersion);
                ENV.Data.DynamicSQLEntity SQLBuscar = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC, buscar);
                SQLBuscar.Columns.Add(nConsecutivoHisto);
                var bpbuscar = new BusinessProcess() { From = SQLBuscar, Activity = Activities.Browse };
                bpbuscar.ForEachRow(() =>
                {
                    v_nContadorRegistros++;
                    if (v_aConsecutivosAmodificar != "")
                        v_aConsecutivosAmodificar = v_aConsecutivosAmodificar + ",";

                    v_aConsecutivosAmodificar = v_aConsecutivosAmodificar + nConsecutivoHisto;
                    if (v_nContadorRegistros == 2000)
                    {
                        ENV.Data.DynamicSQLEntity updateprod = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                            @"UPDATE MAGICADM.CTTARIFASHISTORICOLAYOUT 
                            SET LPRODUCCION = X'01'
                            WHERE NCLAVEEMPRESA = :1 AND  NCONSECUTIVOHISTORICO  IN (:2)");

                        updateprod.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                        updateprod.AddParameter(() => v_aConsecutivosAmodificar);

                        var updatetablaprod = new BusinessProcess { From = updateprod };
                        updatetablaprod.Exit(ExitTiming.AfterRow);
                        updatetablaprod.Run();
                        updatetablaprod.Exit();
                        v_aConsecutivosAmodificar = "";
                        v_nContadorRegistros = 0;

                    }   
                });
                bpbuscar.Exit();

                if (v_nContadorRegistros != 0 && v_aConsecutivosAmodificar != "")
                {
                    ENV.Data.DynamicSQLEntity updateprod = new ENV.Data.DynamicSQLEntity(Iamsa.Shared.DataSources.CITEC,
                        @"UPDATE MAGICADM.CTTARIFASHISTORICOLAYOUT 
                            SET LPRODUCCION = X'01'
                            WHERE NCLAVEEMPRESA = :1 AND  NCONSECUTIVOHISTORICO  IN (:2)");

                    updateprod.AddParameter(() => GCTERMINALES.NCLAVEEMPRESA);
                    updateprod.AddParameter(() => v_aConsecutivosAmodificar);

                    var updatetablaprod = new BusinessProcess { From = updateprod };
                    updatetablaprod.Exit(ExitTiming.AfterRow);
                    updatetablaprod.Run();
                    updatetablaprod.Exit();
                    v_aConsecutivosAmodificar = "";
                    v_nContadorRegistros = 0;
                }

            }
            catch (Exception ex)
            {
                ENV.ErrorLog.WriteToLogFile($"Error al modificar Estatus de la version:  {nVersion} - con los consecutivos históricos: "+ v_aConsecutivosAmodificar  + $" - {ex.Message} ");
            }

        }

    }
}
