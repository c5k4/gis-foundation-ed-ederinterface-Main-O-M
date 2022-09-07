Prompt drop Trigger V131_UPDATE;
DROP TRIGGER EDGIS.V131_UPDATE
/

Prompt Trigger V131_UPDATE;
--
-- V131_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V131_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_SECONDARYGENERATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A131 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.INSTALLATIONTYPE,:new.GENERATORTYPE,:new.OWNERNAME,:new.CONNECTIONCONFIGURATION,:new.KW,:new.POWERFACTOR,:new.RATEDKVA,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.CEDSADEVICEID,:new.CAPACITY,:new.GENERATORNAME,:new.METERID,:new.GENERATIONID,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D131 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A131 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,GENERATORTYPE = :new.GENERATORTYPE,OWNERNAME = :new.OWNERNAME,CONNECTIONCONFIGURATION = :new.CONNECTIONCONFIGURATION,KW = :new.KW,POWERFACTOR = :new.POWERFACTOR,RATEDKVA = :new.RATEDKVA,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,CEDSADEVICEID = :new.CEDSADEVICEID,CAPACITY = :new.CAPACITY,GENERATORNAME = :new.GENERATORNAME,METERID = :new.METERID,GENERATIONID = :new.GENERATIONID,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d131 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d131 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A131 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.INSTALLATIONTYPE,:new.GENERATORTYPE,:new.OWNERNAME,:new.CONNECTIONCONFIGURATION,:new.KW,:new.POWERFACTOR,:new.RATEDKVA,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.CEDSADEVICEID,:new.CAPACITY,:new.GENERATORNAME,:new.METERID,:new.GENERATIONID,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D131 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.SECONDARYGENERATION SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,GENERATORTYPE = :new.GENERATORTYPE,OWNERNAME = :new.OWNERNAME,CONNECTIONCONFIGURATION = :new.CONNECTIONCONFIGURATION,KW = :new.KW,POWERFACTOR = :new.POWERFACTOR,RATEDKVA = :new.RATEDKVA,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,CEDSADEVICEID = :new.CEDSADEVICEID,CAPACITY = :new.CAPACITY,GENERATORNAME = :new.GENERATORNAME,METERID = :new.METERID,GENERATIONID = :new.GENERATIONID,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A131 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.INSTALLATIONTYPE,:new.GENERATORTYPE,:new.OWNERNAME,:new.CONNECTIONCONFIGURATION,:new.KW,:new.POWERFACTOR,:new.RATEDKVA,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.CEDSADEVICEID,:new.CAPACITY,:new.GENERATORNAME,:new.METERID,:new.GENERATIONID,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D131 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A131 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,GENERATORTYPE = :new.GENERATORTYPE,OWNERNAME = :new.OWNERNAME,CONNECTIONCONFIGURATION = :new.CONNECTIONCONFIGURATION,KW = :new.KW,POWERFACTOR = :new.POWERFACTOR,RATEDKVA = :new.RATEDKVA,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,CEDSADEVICEID = :new.CEDSADEVICEID,CAPACITY = :new.CAPACITY,GENERATORNAME = :new.GENERATORNAME,METERID = :new.METERID,GENERATIONID = :new.GENERATIONID,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (131,current_state);  END IF; END;
/
