Prompt drop Trigger V136_UPDATE;
DROP TRIGGER EDGIS.V136_UPDATE
/

Prompt Trigger V136_UPDATE;
--
-- V136_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V136_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A136 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d136 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d136 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DCCONDUCTOR SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A136 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (136,current_state);  END IF; END;
/