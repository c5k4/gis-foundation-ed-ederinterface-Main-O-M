Prompt drop Trigger V128_INSERT;
DROP TRIGGER EDGIS.V128_INSERT
/

Prompt Trigger V128_INSERT;
--
-- V128_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V128_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_SERVICELOCATION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',128); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A128 VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.GENCATEGORY,:new.SHAPE,:new.FEEDERTYPE,current_state,:new.LABELTEXT,:new.VERSIONNAME,:new.EVCN,:new.NUMOFCHARGERS,:new.OWNERSHIP,:new.SYMBOLNUMBER);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.SERVICELOCATION VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.GENCATEGORY,:new.SHAPE,:new.FEEDERTYPE,:new.LABELTEXT,:new.VERSIONNAME,:new.EVCN,:new.NUMOFCHARGERS,:new.OWNERSHIP,:new.SYMBOLNUMBER);  ELSE INSERT INTO EDGIS.A128  VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.GENCATEGORY,:new.SHAPE,:new.FEEDERTYPE,current_state,:new.LABELTEXT,:new.VERSIONNAME,:new.EVCN,:new.NUMOFCHARGERS,:new.OWNERSHIP,:new.SYMBOLNUMBER);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (128,current_state);  END IF;END;
/
