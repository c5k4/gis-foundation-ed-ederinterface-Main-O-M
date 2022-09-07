--22769
insert into sde.mm_field_modelnames (oid,objectclassname,fieldname,modelname) values (sde.r659.nextval,'EDGIS.SECUGCONDUCTORINFO','CONDUCTORCOUNT','PGE_CONDUCTORCOUNT');
insert into sde.mm_field_modelnames (oid,objectclassname,fieldname,modelname) values (sde.r659.nextval,'EDGIS.SECUGCONDUCTORINFO','MATERIAL','PGE_CONDUCTORMATERIAL');
insert into sde.mm_field_modelnames (oid,objectclassname,fieldname,modelname) values (sde.r659.nextval,'EDGIS.SECUGCONDUCTORINFO','CONDUCTORSIZE','PGE_CONDUCTORSIZE');

--22109
delete from sde.mm_class_modelnames where objectclassname='EDGIS.CONDUITSYSTEM' and modelname='UFMVISIBLE';

--22799
insert into sde.mm_field_modelnames (oid,objectclassname,fieldname,modelname) values (sde.r659.nextval,'EDGIS.CIRCUITSOURCE','CIRCUITABBREVNAME','PGE_CIRCUITNAME');

exit;