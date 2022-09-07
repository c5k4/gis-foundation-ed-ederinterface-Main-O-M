Prompt drop View GUY_EVW;
DROP VIEW EDGIS.GUY_EVW
/

/* Formatted on 7/2/2019 01:18:03 PM (QP5 v5.313) */
PROMPT View GUY_EVW;
--
-- GUY_EVW  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.GUY_EVW
(
    OBJECTID,
    CREATIONUSER,
    DATECREATED,
    LASTUSER,
    DATEMODIFIED,
    CUSTOMEROWNED,
    GUYTYPE,
    GUYCOUNT,
    GUYSIZE,
    OWNERGRADE,
    OWNERSPACE,
    SPACETYPE,
    JONAME,
    ANCHORGUID,
    STATUS,
    ATTACHMENTHEIGHT,
    GLOBALID,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.LASTUSER,
           b.DATEMODIFIED,
           b.CUSTOMEROWNED,
           b.GUYTYPE,
           b.GUYCOUNT,
           b.GUYSIZE,
           b.OWNERGRADE,
           b.OWNERSPACE,
           b.SPACETYPE,
           b.JONAME,
           b.ANCHORGUID,
           b.STATUS,
           b.ATTACHMENTHEIGHT,
           b.GLOBALID,
           0 SDE_STATE_ID
      FROM EDGIS.GUY  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7366251
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.CREATIONUSER,
           a.DATECREATED,
           a.LASTUSER,
           a.DATEMODIFIED,
           a.CUSTOMEROWNED,
           a.GUYTYPE,
           a.GUYCOUNT,
           a.GUYSIZE,
           a.OWNERGRADE,
           a.OWNERSPACE,
           a.SPACETYPE,
           a.JONAME,
           a.ANCHORGUID,
           a.STATUS,
           a.ATTACHMENTHEIGHT,
           a.GLOBALID,
           a.SDE_STATE_ID
      FROM EDGIS.A7366251  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7366251
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7366251_DELETE;
--
-- V7366251_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366251_DELETE INSTEAD OF DELETE ON EDGIS.GUY_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7366251 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7366251 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7366251 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.GUY WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7366251 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366251,current_state); END IF;END;
/


Prompt Trigger V7366251_INSERT;
--
-- V7366251_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366251_INSERT INSTEAD OF INSERT ON EDGIS.GUY_EVW REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',7366251); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A7366251 VALUES (next_rowid,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.GUY VALUES (next_rowid,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID);  ELSE INSERT INTO EDGIS.A7366251  VALUES (next_rowid,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366251,current_state);  END IF;END;
/


Prompt Trigger V7366251_UPDATE;
--
-- V7366251_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366251_UPDATE INSTEAD OF UPDATE ON EDGIS.GUY_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A7366251 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID,current_state); INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A7366251 SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CUSTOMEROWNED = :new.CUSTOMEROWNED,GUYTYPE = :new.GUYTYPE,GUYCOUNT = :new.GUYCOUNT,GUYSIZE = :new.GUYSIZE,OWNERGRADE = :new.OWNERGRADE,OWNERSPACE = :new.OWNERSPACE,SPACETYPE = :new.SPACETYPE,JONAME = :new.JONAME,ANCHORGUID = :new.ANCHORGUID,STATUS = :new.STATUS,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7366251 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7366251 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A7366251 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID,current_state); INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.GUY SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CUSTOMEROWNED = :new.CUSTOMEROWNED,GUYTYPE = :new.GUYTYPE,GUYCOUNT = :new.GUYCOUNT,GUYSIZE = :new.GUYSIZE,OWNERGRADE = :new.OWNERGRADE,OWNERSPACE = :new.OWNERSPACE,SPACETYPE = :new.SPACETYPE,JONAME = :new.JONAME,ANCHORGUID = :new.ANCHORGUID,STATUS = :new.STATUS,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A7366251 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CUSTOMEROWNED,:new.GUYTYPE,:new.GUYCOUNT,:new.GUYSIZE,:new.OWNERGRADE,:new.OWNERSPACE,:new.SPACETYPE,:new.JONAME,:new.ANCHORGUID,:new.STATUS,:new.ATTACHMENTHEIGHT,:new.GLOBALID,current_state); INSERT INTO EDGIS.D7366251 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A7366251 SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CUSTOMEROWNED = :new.CUSTOMEROWNED,GUYTYPE = :new.GUYTYPE,GUYCOUNT = :new.GUYCOUNT,GUYSIZE = :new.GUYSIZE,OWNERGRADE = :new.OWNERGRADE,OWNERSPACE = :new.OWNERSPACE,SPACETYPE = :new.SPACETYPE,JONAME = :new.JONAME,ANCHORGUID = :new.ANCHORGUID,STATUS = :new.STATUS,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366251,current_state);  END IF; END;
/


Prompt Grants on VIEW GUY_EVW TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GUY_EVW TO DAT_EDITOR
/

Prompt Grants on VIEW GUY_EVW TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GUY_EVW TO DMSSTAGING
/

Prompt Grants on VIEW GUY_EVW TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.GUY_EVW TO GISINTERFACE
/

Prompt Grants on VIEW GUY_EVW TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.GUY_EVW TO GIS_INTERFACE
/

Prompt Grants on VIEW GUY_EVW TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GUY_EVW TO SDE
/

Prompt Grants on VIEW GUY_EVW TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GUY_EVW TO SDE_EDITOR
/

Prompt Grants on VIEW GUY_EVW TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.GUY_EVW TO SDE_VIEWER
/
