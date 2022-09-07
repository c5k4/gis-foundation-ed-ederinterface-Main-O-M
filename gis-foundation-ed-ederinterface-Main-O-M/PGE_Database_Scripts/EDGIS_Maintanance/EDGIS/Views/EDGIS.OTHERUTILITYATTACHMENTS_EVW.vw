Prompt drop View OTHERUTILITYATTACHMENTS_EVW;
DROP VIEW EDGIS.OTHERUTILITYATTACHMENTS_EVW
/

/* Formatted on 6/27/2019 02:59:23 PM (QP5 v5.313) */
PROMPT View OTHERUTILITYATTACHMENTS_EVW;
--
-- OTHERUTILITYATTACHMENTS_EVW  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.OTHERUTILITYATTACHMENTS_EVW
(
    OBJECTID,
    OUNAME,
    ATTACHMENTTYPE,
    ATTACHMENTHEIGHT,
    ATTACHMENTOFFSET,
    SERVICEDROPALIGNMENT,
    STRUCTUREGUID,
    CREATIONUSER,
    LASTUSER,
    DATECREATED,
    DATEMODIFIED,
    SDE_STATE_ID,
    CABLESIZE,
    SERVICEDROPCOUNT,
    GLOBALID
)
AS
    SELECT b.OBJECTID,
           b.OUNAME,
           b.ATTACHMENTTYPE,
           b.ATTACHMENTHEIGHT,
           b.ATTACHMENTOFFSET,
           b.SERVICEDROPALIGNMENT,
           b.STRUCTUREGUID,
           b.CREATIONUSER,
           b.LASTUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           0 SDE_STATE_ID,
           b.CABLESIZE,
           b.SERVICEDROPCOUNT,
           b.GLOBALID
      FROM EDGIS.OTHERUTILITYATTACHMENTS  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D13022524
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.OUNAME,
           a.ATTACHMENTTYPE,
           a.ATTACHMENTHEIGHT,
           a.ATTACHMENTOFFSET,
           a.SERVICEDROPALIGNMENT,
           a.STRUCTUREGUID,
           a.CREATIONUSER,
           a.LASTUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.SDE_STATE_ID,
           a.CABLESIZE,
           a.SERVICEDROPCOUNT,
           a.GLOBALID
      FROM EDGIS.A13022524  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D13022524
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V13022524_DELETE;
--
-- V13022524_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V13022524_DELETE INSTEAD OF DELETE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A13022524 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d13022524 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d13022524 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.OTHERUTILITYATTACHMENTS WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A13022524 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (13022524,current_state); END IF;END;
/


Prompt Trigger V13022524_INSERT;
--
-- V13022524_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V13022524_INSERT INSTEAD OF INSERT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',13022524); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A13022524 VALUES (next_rowid,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,current_state,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.OTHERUTILITYATTACHMENTS VALUES (next_rowid,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID);  ELSE INSERT INTO EDGIS.A13022524  VALUES (next_rowid,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,current_state,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (13022524,current_state);  END IF;END;
/


Prompt Trigger V13022524_UPDATE;
--
-- V13022524_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V13022524_UPDATE INSTEAD OF UPDATE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A13022524 VALUES (:old.OBJECTID,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,current_state,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID); INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A13022524 SET OUNAME = :new.OUNAME,ATTACHMENTTYPE = :new.ATTACHMENTTYPE,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,ATTACHMENTOFFSET = :new.ATTACHMENTOFFSET,SERVICEDROPALIGNMENT = :new.SERVICEDROPALIGNMENT,STRUCTUREGUID = :new.STRUCTUREGUID,CREATIONUSER = :new.CREATIONUSER,LASTUSER = :new.LASTUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,CABLESIZE = :new.CABLESIZE,SERVICEDROPCOUNT = :new.SERVICEDROPCOUNT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d13022524 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d13022524 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A13022524 VALUES (:old.OBJECTID,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,current_state,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID); INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.OTHERUTILITYATTACHMENTS SET OUNAME = :new.OUNAME,ATTACHMENTTYPE = :new.ATTACHMENTTYPE,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,ATTACHMENTOFFSET = :new.ATTACHMENTOFFSET,SERVICEDROPALIGNMENT = :new.SERVICEDROPALIGNMENT,STRUCTUREGUID = :new.STRUCTUREGUID,CREATIONUSER = :new.CREATIONUSER,LASTUSER = :new.LASTUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,CABLESIZE = :new.CABLESIZE,SERVICEDROPCOUNT = :new.SERVICEDROPCOUNT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A13022524 VALUES (:old.OBJECTID,:new.OUNAME,:new.ATTACHMENTTYPE,:new.ATTACHMENTHEIGHT,:new.ATTACHMENTOFFSET,:new.SERVICEDROPALIGNMENT,:new.STRUCTUREGUID,:new.CREATIONUSER,:new.LASTUSER,:new.DATECREATED,:new.DATEMODIFIED,current_state,:new.CABLESIZE,:new.SERVICEDROPCOUNT,:new.GLOBALID); INSERT INTO EDGIS.D13022524 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A13022524 SET OUNAME = :new.OUNAME,ATTACHMENTTYPE = :new.ATTACHMENTTYPE,ATTACHMENTHEIGHT = :new.ATTACHMENTHEIGHT,ATTACHMENTOFFSET = :new.ATTACHMENTOFFSET,SERVICEDROPALIGNMENT = :new.SERVICEDROPALIGNMENT,STRUCTUREGUID = :new.STRUCTUREGUID,CREATIONUSER = :new.CREATIONUSER,LASTUSER = :new.LASTUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,CABLESIZE = :new.CABLESIZE,SERVICEDROPCOUNT = :new.SERVICEDROPCOUNT,GLOBALID = :new.GLOBALID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (13022524,current_state);  END IF; END;
/


Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO DAT_EDITOR
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO DMSSTAGING
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO GISINTERFACE
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO GIS_INTERFACE
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO GIS_I_WRITE
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO PUBLIC
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO SDE
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO SDE_EDITOR
/

Prompt Grants on VIEW OTHERUTILITYATTACHMENTS_EVW TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.OTHERUTILITYATTACHMENTS_EVW TO SDE_VIEWER
/
