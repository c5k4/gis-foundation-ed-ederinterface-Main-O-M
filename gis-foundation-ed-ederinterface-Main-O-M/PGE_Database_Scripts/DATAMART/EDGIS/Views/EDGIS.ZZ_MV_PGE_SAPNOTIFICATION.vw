Prompt drop View ZZ_MV_PGE_SAPNOTIFICATION;
DROP VIEW EDGIS.ZZ_MV_PGE_SAPNOTIFICATION
/

/* Formatted on 7/2/2019 01:19:37 PM (QP5 v5.313) */
PROMPT View ZZ_MV_PGE_SAPNOTIFICATION;
--
-- ZZ_MV_PGE_SAPNOTIFICATION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_PGE_SAPNOTIFICATION
(
    OBJECTID,
    NOTIFICATIONNUM,
    NOTIFICATIONTYPE,
    NOTIFICATIONSTATUS,
    DESCRIPTION,
    SAPEQUIPMENTID,
    GUID,
    EQUIPMENTTYPE,
    FUNCTIONALLOCATION,
    WORKTYPECATEGORY,
    WORKTYPECODE,
    NOTIFICATIONCREATION,
    NOTIFICATIONDATE,
    COMPLETIONDATE,
    DUEDATE,
    PRIORITY,
    PMORDERNUM,
    MAT,
    FACILITY,
    PROBLEMSHORTTEXT,
    CAUSETEXT,
    ACTIVITY,
    SUBTYPECD,
    SHAPE,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.NOTIFICATIONNUM,
           b.NOTIFICATIONTYPE,
           b.NOTIFICATIONSTATUS,
           b.DESCRIPTION,
           b.SAPEQUIPMENTID,
           b.GUID,
           b.EQUIPMENTTYPE,
           b.FUNCTIONALLOCATION,
           b.WORKTYPECATEGORY,
           b.WORKTYPECODE,
           b.NOTIFICATIONCREATION,
           b.NOTIFICATIONDATE,
           b.COMPLETIONDATE,
           b.DUEDATE,
           b.PRIORITY,
           b.PMORDERNUM,
           b.MAT,
           b.FACILITY,
           b.PROBLEMSHORTTEXT,
           b.CAUSETEXT,
           b.ACTIVITY,
           b.SUBTYPECD,
           b.SHAPE,
           0 SDE_STATE_ID
      FROM EDGIS.PGE_SAPNOTIFICATION  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D1165195
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.NOTIFICATIONNUM,
           a.NOTIFICATIONTYPE,
           a.NOTIFICATIONSTATUS,
           a.DESCRIPTION,
           a.SAPEQUIPMENTID,
           a.GUID,
           a.EQUIPMENTTYPE,
           a.FUNCTIONALLOCATION,
           a.WORKTYPECATEGORY,
           a.WORKTYPECODE,
           a.NOTIFICATIONCREATION,
           a.NOTIFICATIONDATE,
           a.COMPLETIONDATE,
           a.DUEDATE,
           a.PRIORITY,
           a.PMORDERNUM,
           a.MAT,
           a.FACILITY,
           a.PROBLEMSHORTTEXT,
           a.CAUSETEXT,
           a.ACTIVITY,
           a.SUBTYPECD,
           a.SHAPE,
           a.SDE_STATE_ID
      FROM EDGIS.A1165195  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D1165195
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V1165195_DELETE;
--
-- V1165195_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1165195_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A1165195 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d1165195 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d1165195 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.PGE_SAPNOTIFICATION WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A1165195 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1165195,current_state); END IF;END;
/


Prompt Trigger V1165195_INSERT;
--
-- V1165195_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1165195_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',1165195); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A1165195 VALUES (next_rowid,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.PGE_SAPNOTIFICATION VALUES (next_rowid,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE);  ELSE INSERT INTO EDGIS.A1165195  VALUES (next_rowid,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1165195,current_state);  END IF;END;
/


Prompt Trigger V1165195_UPDATE;
--
-- V1165195_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1165195_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A1165195 VALUES (:old.OBJECTID,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE,current_state); INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A1165195 SET NOTIFICATIONNUM = :new.NOTIFICATIONNUM,NOTIFICATIONTYPE = :new.NOTIFICATIONTYPE,NOTIFICATIONSTATUS = :new.NOTIFICATIONSTATUS,DESCRIPTION = :new.DESCRIPTION,SAPEQUIPMENTID = :new.SAPEQUIPMENTID,GUID = :new.GUID,EQUIPMENTTYPE = :new.EQUIPMENTTYPE,FUNCTIONALLOCATION = :new.FUNCTIONALLOCATION,WORKTYPECATEGORY = :new.WORKTYPECATEGORY,WORKTYPECODE = :new.WORKTYPECODE,NOTIFICATIONCREATION = :new.NOTIFICATIONCREATION,NOTIFICATIONDATE = :new.NOTIFICATIONDATE,COMPLETIONDATE = :new.COMPLETIONDATE,DUEDATE = :new.DUEDATE,PRIORITY = :new.PRIORITY,PMORDERNUM = :new.PMORDERNUM,MAT = :new.MAT,FACILITY = :new.FACILITY,PROBLEMSHORTTEXT = :new.PROBLEMSHORTTEXT,CAUSETEXT = :new.CAUSETEXT,ACTIVITY = :new.ACTIVITY,SUBTYPECD = :new.SUBTYPECD,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d1165195 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d1165195 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A1165195 VALUES (:old.OBJECTID,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE,current_state); INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.PGE_SAPNOTIFICATION SET NOTIFICATIONNUM = :new.NOTIFICATIONNUM,NOTIFICATIONTYPE = :new.NOTIFICATIONTYPE,NOTIFICATIONSTATUS = :new.NOTIFICATIONSTATUS,DESCRIPTION = :new.DESCRIPTION,SAPEQUIPMENTID = :new.SAPEQUIPMENTID,GUID = :new.GUID,EQUIPMENTTYPE = :new.EQUIPMENTTYPE,FUNCTIONALLOCATION = :new.FUNCTIONALLOCATION,WORKTYPECATEGORY = :new.WORKTYPECATEGORY,WORKTYPECODE = :new.WORKTYPECODE,NOTIFICATIONCREATION = :new.NOTIFICATIONCREATION,NOTIFICATIONDATE = :new.NOTIFICATIONDATE,COMPLETIONDATE = :new.COMPLETIONDATE,DUEDATE = :new.DUEDATE,PRIORITY = :new.PRIORITY,PMORDERNUM = :new.PMORDERNUM,MAT = :new.MAT,FACILITY = :new.FACILITY,PROBLEMSHORTTEXT = :new.PROBLEMSHORTTEXT,CAUSETEXT = :new.CAUSETEXT,ACTIVITY = :new.ACTIVITY,SUBTYPECD = :new.SUBTYPECD,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A1165195 VALUES (:old.OBJECTID,:new.NOTIFICATIONNUM,:new.NOTIFICATIONTYPE,:new.NOTIFICATIONSTATUS,:new.DESCRIPTION,:new.SAPEQUIPMENTID,:new.GUID,:new.EQUIPMENTTYPE,:new.FUNCTIONALLOCATION,:new.WORKTYPECATEGORY,:new.WORKTYPECODE,:new.NOTIFICATIONCREATION,:new.NOTIFICATIONDATE,:new.COMPLETIONDATE,:new.DUEDATE,:new.PRIORITY,:new.PMORDERNUM,:new.MAT,:new.FACILITY,:new.PROBLEMSHORTTEXT,:new.CAUSETEXT,:new.ACTIVITY,:new.SUBTYPECD,:new.SHAPE,current_state); INSERT INTO EDGIS.D1165195 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A1165195 SET NOTIFICATIONNUM = :new.NOTIFICATIONNUM,NOTIFICATIONTYPE = :new.NOTIFICATIONTYPE,NOTIFICATIONSTATUS = :new.NOTIFICATIONSTATUS,DESCRIPTION = :new.DESCRIPTION,SAPEQUIPMENTID = :new.SAPEQUIPMENTID,GUID = :new.GUID,EQUIPMENTTYPE = :new.EQUIPMENTTYPE,FUNCTIONALLOCATION = :new.FUNCTIONALLOCATION,WORKTYPECATEGORY = :new.WORKTYPECATEGORY,WORKTYPECODE = :new.WORKTYPECODE,NOTIFICATIONCREATION = :new.NOTIFICATIONCREATION,NOTIFICATIONDATE = :new.NOTIFICATIONDATE,COMPLETIONDATE = :new.COMPLETIONDATE,DUEDATE = :new.DUEDATE,PRIORITY = :new.PRIORITY,PMORDERNUM = :new.PMORDERNUM,MAT = :new.MAT,FACILITY = :new.FACILITY,PROBLEMSHORTTEXT = :new.PROBLEMSHORTTEXT,CAUSETEXT = :new.CAUSETEXT,ACTIVITY = :new.ACTIVITY,SUBTYPECD = :new.SUBTYPECD,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1165195,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO GIS_INTERFACE to GIS_INTERFACE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO SDE
/

Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_PGE_SAPNOTIFICATION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_PGE_SAPNOTIFICATION TO SDE_VIEWER
/
