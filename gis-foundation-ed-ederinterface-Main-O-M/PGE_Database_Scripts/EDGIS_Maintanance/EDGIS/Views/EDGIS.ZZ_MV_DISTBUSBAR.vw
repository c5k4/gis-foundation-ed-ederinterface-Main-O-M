Prompt drop View ZZ_MV_DISTBUSBAR;
DROP VIEW EDGIS.ZZ_MV_DISTBUSBAR
/

/* Formatted on 6/27/2019 02:56:32 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DISTBUSBAR;
--
-- ZZ_MV_DISTBUSBAR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DISTBUSBAR
(
    OBJECTID,
    ENABLED,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    COMMENTS,
    STATUS,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    ELECTRICTRACEWEIGHT,
    TRACEABLE,
    CIRCUITID,
    CIRCUITID2,
    CONVCIRCUITID,
    CONVCIRCUITID2,
    FEEDERINFO,
    CEDSANUMBEROFPHASES,
    COUNTY,
    ZIP,
    REPLACEGUID,
    SUBTYPECD,
    PHASEDESIGNATION,
    PHASINGVERIFIEDSTATUS,
    OPERATINGVOLTAGE,
    STRUCTUREGUID,
    STRUCTURECONVID,
    CEDSALINESECTIONID,
    CUSTOMEROWNED,
    LOCALOFFICEID,
    DISTRICT,
    DIVISION,
    REGION,
    INSTALLJOBNUMBER,
    INSTALLATIONDATE,
    CITY,
    VERSIONNAME,
    OPERATINGNUMBER,
    SHAPE,
    FEEDERTYPE,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.ENABLED,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.COMMENTS,
           b.STATUS,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.ELECTRICTRACEWEIGHT,
           b.TRACEABLE,
           b.CIRCUITID,
           b.CIRCUITID2,
           b.CONVCIRCUITID,
           b.CONVCIRCUITID2,
           b.FEEDERINFO,
           b.CEDSANUMBEROFPHASES,
           b.COUNTY,
           b.ZIP,
           b.REPLACEGUID,
           b.SUBTYPECD,
           b.PHASEDESIGNATION,
           b.PHASINGVERIFIEDSTATUS,
           b.OPERATINGVOLTAGE,
           b.STRUCTUREGUID,
           b.STRUCTURECONVID,
           b.CEDSALINESECTIONID,
           b.CUSTOMEROWNED,
           b.LOCALOFFICEID,
           b.DISTRICT,
           b.DIVISION,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.INSTALLATIONDATE,
           b.CITY,
           b.VERSIONNAME,
           b.OPERATINGNUMBER,
           b.SHAPE,
           b.FEEDERTYPE,
           0 SDE_STATE_ID
      FROM EDGIS.DISTBUSBAR  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D135
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.ENABLED,
           a.GLOBALID,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.COMMENTS,
           a.STATUS,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.ELECTRICTRACEWEIGHT,
           a.TRACEABLE,
           a.CIRCUITID,
           a.CIRCUITID2,
           a.CONVCIRCUITID,
           a.CONVCIRCUITID2,
           a.FEEDERINFO,
           a.CEDSANUMBEROFPHASES,
           a.COUNTY,
           a.ZIP,
           a.REPLACEGUID,
           a.SUBTYPECD,
           a.PHASEDESIGNATION,
           a.PHASINGVERIFIEDSTATUS,
           a.OPERATINGVOLTAGE,
           a.STRUCTUREGUID,
           a.STRUCTURECONVID,
           a.CEDSALINESECTIONID,
           a.CUSTOMEROWNED,
           a.LOCALOFFICEID,
           a.DISTRICT,
           a.DIVISION,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.INSTALLATIONDATE,
           a.CITY,
           a.VERSIONNAME,
           a.OPERATINGNUMBER,
           a.SHAPE,
           a.FEEDERTYPE,
           a.SDE_STATE_ID
      FROM EDGIS.A135  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D135
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V135_DELETE;
--
-- V135_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V135_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DISTBUSBAR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A135 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d135 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d135 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DISTBUSBAR WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A135 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (135,current_state); END IF;END;
/


Prompt Trigger V135_INSERT;
--
-- V135_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V135_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DISTBUSBAR REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',135); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A135 VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DISTBUSBAR VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A135  VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (135,current_state);  END IF;END;
/


Prompt Trigger V135_UPDATE;
--
-- V135_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V135_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A135 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A135 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,INSTALLATIONDATE = :new.INSTALLATIONDATE,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d135 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d135 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A135 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DISTBUSBAR SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,INSTALLATIONDATE = :new.INSTALLATIONDATE,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A135 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CEDSALINESECTIONID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.INSTALLATIONDATE,:new.CITY,:new.VERSIONNAME,:new.OPERATINGNUMBER,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D135 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A135 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,INSTALLATIONDATE = :new.INSTALLATIONDATE,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,OPERATINGNUMBER = :new.OPERATINGNUMBER,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (135,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_DISTBUSBAR TO A0SW
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DISTBUSBAR TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DISTBUSBAR TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DISTBUSBAR TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_DISTBUSBAR TO M4AB
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DISTBUSBAR TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DISTBUSBAR TO RSDH
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_DISTBUSBAR TO S7MA
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DISTBUSBAR TO SDE
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DISTBUSBAR TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DISTBUSBAR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DISTBUSBAR TO SDE_VIEWER
/
