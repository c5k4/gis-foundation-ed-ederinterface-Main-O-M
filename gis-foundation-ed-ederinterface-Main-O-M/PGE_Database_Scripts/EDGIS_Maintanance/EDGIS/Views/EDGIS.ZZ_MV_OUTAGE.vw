Prompt drop View ZZ_MV_OUTAGE;
DROP VIEW EDGIS.ZZ_MV_OUTAGE
/

/* Formatted on 6/27/2019 02:54:42 PM (QP5 v5.313) */
PROMPT View ZZ_MV_OUTAGE;
--
-- ZZ_MV_OUTAGE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_OUTAGE
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    CUSTOMEROWNED,
    SUBTYPECD,
    SYMBOLROTATION,
    DISTRICT,
    DIVISION,
    REGION,
    OUTAGEID,
    OUTPUTREPORTNUM,
    OUTAGEDATE,
    BASICCAUSE,
    SUPPLEMENTALCAUSE,
    OPENPOINT,
    OPENQUIPCODE,
    MINUTETOTAL,
    TOTALCUSTOMERS,
    TOTALCUSTOMERSOUT,
    TOTALCUSTOMERMINUTES,
    SHAPE,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.CUSTOMEROWNED,
           b.SUBTYPECD,
           b.SYMBOLROTATION,
           b.DISTRICT,
           b.DIVISION,
           b.REGION,
           b.OUTAGEID,
           b.OUTPUTREPORTNUM,
           b.OUTAGEDATE,
           b.BASICCAUSE,
           b.SUPPLEMENTALCAUSE,
           b.OPENPOINT,
           b.OPENQUIPCODE,
           b.MINUTETOTAL,
           b.TOTALCUSTOMERS,
           b.TOTALCUSTOMERSOUT,
           b.TOTALCUSTOMERMINUTES,
           b.SHAPE,
           0 SDE_STATE_ID
      FROM EDGIS.OUTAGE  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D154
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.GLOBALID,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.CUSTOMEROWNED,
           a.SUBTYPECD,
           a.SYMBOLROTATION,
           a.DISTRICT,
           a.DIVISION,
           a.REGION,
           a.OUTAGEID,
           a.OUTPUTREPORTNUM,
           a.OUTAGEDATE,
           a.BASICCAUSE,
           a.SUPPLEMENTALCAUSE,
           a.OPENPOINT,
           a.OPENQUIPCODE,
           a.MINUTETOTAL,
           a.TOTALCUSTOMERS,
           a.TOTALCUSTOMERSOUT,
           a.TOTALCUSTOMERMINUTES,
           a.SHAPE,
           a.SDE_STATE_ID
      FROM EDGIS.A154  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D154
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V154_DELETE;
--
-- V154_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V154_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_OUTAGE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A154 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d154 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d154 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.OUTAGE WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A154 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (154,current_state); END IF;END;
/


Prompt Trigger V154_INSERT;
--
-- V154_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V154_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_OUTAGE REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',154); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A154 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.OUTAGE VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE);  ELSE INSERT INTO EDGIS.A154  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (154,current_state);  END IF;END;
/


Prompt Trigger V154_UPDATE;
--
-- V154_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V154_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_OUTAGE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A154 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE,current_state); INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A154 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,OUTAGEID = :new.OUTAGEID,OUTPUTREPORTNUM = :new.OUTPUTREPORTNUM,OUTAGEDATE = :new.OUTAGEDATE,BASICCAUSE = :new.BASICCAUSE,SUPPLEMENTALCAUSE = :new.SUPPLEMENTALCAUSE,OPENPOINT = :new.OPENPOINT,OPENQUIPCODE = :new.OPENQUIPCODE,MINUTETOTAL = :new.MINUTETOTAL,TOTALCUSTOMERS = :new.TOTALCUSTOMERS,TOTALCUSTOMERSOUT = :new.TOTALCUSTOMERSOUT,TOTALCUSTOMERMINUTES = :new.TOTALCUSTOMERMINUTES,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d154 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d154 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A154 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE,current_state); INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.OUTAGE SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,OUTAGEID = :new.OUTAGEID,OUTPUTREPORTNUM = :new.OUTPUTREPORTNUM,OUTAGEDATE = :new.OUTAGEDATE,BASICCAUSE = :new.BASICCAUSE,SUPPLEMENTALCAUSE = :new.SUPPLEMENTALCAUSE,OPENPOINT = :new.OPENPOINT,OPENQUIPCODE = :new.OPENQUIPCODE,MINUTETOTAL = :new.MINUTETOTAL,TOTALCUSTOMERS = :new.TOTALCUSTOMERS,TOTALCUSTOMERSOUT = :new.TOTALCUSTOMERSOUT,TOTALCUSTOMERMINUTES = :new.TOTALCUSTOMERMINUTES,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A154 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.OUTAGEID,:new.OUTPUTREPORTNUM,:new.OUTAGEDATE,:new.BASICCAUSE,:new.SUPPLEMENTALCAUSE,:new.OPENPOINT,:new.OPENQUIPCODE,:new.MINUTETOTAL,:new.TOTALCUSTOMERS,:new.TOTALCUSTOMERSOUT,:new.TOTALCUSTOMERMINUTES,:new.SHAPE,current_state); INSERT INTO EDGIS.D154 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A154 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,OUTAGEID = :new.OUTAGEID,OUTPUTREPORTNUM = :new.OUTPUTREPORTNUM,OUTAGEDATE = :new.OUTAGEDATE,BASICCAUSE = :new.BASICCAUSE,SUPPLEMENTALCAUSE = :new.SUPPLEMENTALCAUSE,OPENPOINT = :new.OPENPOINT,OPENQUIPCODE = :new.OPENQUIPCODE,MINUTETOTAL = :new.MINUTETOTAL,TOTALCUSTOMERS = :new.TOTALCUSTOMERS,TOTALCUSTOMERSOUT = :new.TOTALCUSTOMERSOUT,TOTALCUSTOMERMINUTES = :new.TOTALCUSTOMERMINUTES,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (154,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_OUTAGE TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_OUTAGE TO A0SW
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_OUTAGE TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_OUTAGE TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_OUTAGE TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_OUTAGE TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_OUTAGE TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_OUTAGE TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_OUTAGE TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_OUTAGE TO M4AB
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_OUTAGE TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_OUTAGE TO RSDH
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_OUTAGE TO S7MA
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_OUTAGE TO SDE
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_OUTAGE TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_OUTAGE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_OUTAGE TO SDE_VIEWER
/
