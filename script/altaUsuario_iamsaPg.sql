--alta de usuario

INSERT INTO PLUGINS.PASS_USUARIOS(ID, "CONTRASEÑA") 
    VALUES('RMARTINEZ','')
    
INSERT INTO MAGICADM.GCTERMINALES(NCLAVETERMINAL, ADESCRIPCION, LRECIBEBOLETOS, LVENTAWEBSERVICES, LBAJA, NCLAVEEMPRESA, ACLAVEOFICINA, FAUFECHA, HAUHORA, NAUCLAVETERMINAL, AAUCLAVEOFICINA, AAUCLAVEUSUARIO, GCTER_LVENTADIRECTA, NAGRUPADORTAQUILLA, ACLAVECENTROCOSTOS, LJIOSKO, LKIOSKO, ATIPOTERMINAL, LAPLICABOLETERA, ATIPOBOLMOV, AOFICDEFAULT, ADIRECCIONIP, NTIPOIDENTIFICA) 
    VALUES(8097, 'RMARTINEZ', true, false, false, 4, 'SNJRCURRENT_TIME, 0, '', '', 0, 0, '', false, false, '', false, '', '', '', 0)
    
INSERT INTO gcpersonas (nclaveempresa,nclavepersona,aclaveusuario,aclaveempleado,anombres,aapellidos,arfc,ahomoclave,acurp,nimss,aestadocivil,nclaveescolaridad,acalle,acolonia,aciudad,aestado,ncodigopostal,atelefono1,atelefono2,acorreoelectronico,afoto,acontrasena,ffechacontrasena,aclaveoficinacontrasena,nclavedepartamento,nclavepuesto,aclaveoficina,ffechaingreso,lconductor,alicenciafederal,ffechalicenciafederal,alicenciaestatal,ffechalicenciaestatal,ntarjetacreditointerna,ffechatarjetacredito,lbajaempleado,lbajausuario,faucrfecha,haucrhora,naucrclaveterminal,aaucrclaveoficina,aaucrclaveusuario,faumofecha,haumohora,naumoclaveterminal,aaumoclaveoficina,aaumoclaveusuario,cnumerocliente,acuentaacredor,aclavesap,nempresaalterna,aregistropatronal,laplicaboleteras,aclavetarjeton,fvigenciatarjeton,lconductorturismo,ntipoempleadocanapat,ffechavigpsicofisico,tipooperador,nprocesodelsip,nsindicato,nclaveempleadosap,nsdi,nclaverolinterna,nllavemanejo,lejecutivocuenta,acedula,nidpuestosap,acontrasenaabordaje,aclaveine,lfof,aidivrusuario,laplicacortesia,lesinspector,ladministradorba,limppapeleta) VALUES
	 (4,(SELECT MAX(NCLAVEPERSONA) FROM GCPERSONAS WHERE NCLAVEEMPRESA=4)+1,'RMARTINEZ','RMARTINEZ','RUBEN','MARTINEZ','MARTINEZ ','   ','',0,' ',0,'','','  ','',0,' ',' ','','','','1901-01-01','    ',0,0,'MXST','1901-01-01',true,'  ','1901-01-01','  ','1901-01-01',0,'1901-01-01',false,false,'1901-01-01','12:00:00',0,'    ','','1901-01-01','12:00:00',0,'    ','','','  ','',4,'  ',true,'','1901-01-01',true,0,'1901-01-01','0    ',0,0,0,0.00,0,0,true,NULL,0,'','',false,'  ',false,false,false,false);

INSERT INTO GSUSUARIOSPERMISOS(NCLAVEEMPRESA,ACLAVEUSUARIOS,ACLAVEPERMISOS,ACONTRASENA) VALUES(4,'RMARTINEZ','SEGMEN','123')

INSERT INTO GSUSUARIOPERFIL(NCLAVEEMPRESA, ACLAVEUSUARIO, ACLAVEPERFIL, LFECHA, FFECHAINICIAL, FFECHAFINAL, FAUFECHA, HAUHORA, AAUCLAVEOFICINA, NAUCLAVETERMINAL, AAUCLAVEUSUARIO)
  VALUES(4, 'RMARTINEZ', 'COMPLE', false, '1901-01-01', '1901-01-01', '2016-06-17', '11:22:13', 'MEXP', 8097, 'RMARTINEZ')

--seleccionar un usuario
select * from PLUGINS.PASS_USUARIOS where ID='RMARTINEZ';  
select * from PLUGINS.PASS_USUARIOS where ID='41002986';  
select * from MAGICADM.GCPERSONAS where ACLAVEUSUARIO='RMARTINEZ';
select * from MAGICADM.GCPERSONAS where ACLAVEUSUARIO='41002986';

--departamentos
select *,nclavedepartamento  from magicadm.gcpersonas g where nclavedepartamento 

--tabla de queries
select * from magicadm.paquerydatosadicionales p ;
select * from magicadm.paquerydatosadicionales p where aquery_a_ejecutar like '%TRIM(ANOMBRES)%';

@claveusuario