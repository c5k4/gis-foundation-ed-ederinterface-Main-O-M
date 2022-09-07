Prompt drop View ZZ_MV_N_2_JSTATUS;
DROP VIEW EDGIS.ZZ_MV_N_2_JSTATUS
/

/* Formatted on 6/27/2019 02:54:52 PM (QP5 v5.313) */
PROMPT View ZZ_MV_N_2_JSTATUS;
--
-- ZZ_MV_N_2_JSTATUS  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_N_2_JSTATUS
(
    OID,
    PAGENUMBER,
    PAGEBLOB,
    SDE_STATE_ID
)
AS
    SELECT b.OID,
           b.PAGENUMBER,
           b.PAGEBLOB,
           0 SDE_STATE_ID
      FROM EDGIS.N_2_JSTATUS  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D1166289
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OID,
           a.PAGENUMBER,
           a.PAGEBLOB,
           a.SDE_STATE_ID
      FROM EDGIS.A1166289  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D1166289
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V1166289_DELETE;
--
-- V1166289_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1166289_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_N_2_JSTATUS REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A1166289 WHERE OID = :old.OID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d1166289 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d1166289 WHERE sde_deletes_row_id = :OLD.OID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id); Else DELETE FROM EDGIS.N_2_JSTATUS WHERE OID = :old.OID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A1166289 WHERE OID = :old.OID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1166289,current_state); END IF;END;
/


Prompt Trigger V1166289_INSERT;
--
-- V1166289_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1166289_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_N_2_JSTATUS REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OID IS NOT NULL Then next_rowid := :new.OID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',1166289); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A1166289 VALUES (next_rowid,:new.PAGENUMBER,:new.PAGEBLOB,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.N_2_JSTATUS VALUES (next_rowid,:new.PAGENUMBER,:new.PAGEBLOB);  ELSE INSERT INTO EDGIS.A1166289  VALUES (next_rowid,:new.PAGENUMBER,:new.PAGEBLOB,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1166289,current_state);  END IF;END;
/


Prompt Trigger V1166289_UPDATE;
--
-- V1166289_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V1166289_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_N_2_JSTATUS REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OID <> :OLD.OID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A1166289 VALUES (:old.OID,:new.PAGENUMBER,:new.PAGEBLOB,current_state); INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id); ELSE UPDATE EDGIS.A1166289 SET PAGENUMBER = :new.PAGENUMBER,PAGEBLOB = :new.PAGEBLOB WHERE OID = :old.OID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d1166289 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d1166289 WHERE sde_deletes_row_id = :OLD.OID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A1166289 VALUES (:old.OID,:new.PAGENUMBER,:new.PAGEBLOB,current_state); INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id);  ELSE UPDATE EDGIS.N_2_JSTATUS SET PAGENUMBER = :new.PAGENUMBER,PAGEBLOB = :new.PAGEBLOB WHERE OID = :old.OID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A1166289 VALUES (:old.OID,:new.PAGENUMBER,:new.PAGEBLOB,current_state); INSERT INTO EDGIS.D1166289 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OID,:old.sde_state_id);  ELSE UPDATE EDGIS.A1166289 SET PAGENUMBER = :new.PAGENUMBER,PAGEBLOB = :new.PAGEBLOB WHERE OID = :old.OID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (1166289,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_N_2_JSTATUS TO A0SW
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_N_2_JSTATUS TO S7MA
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO SDE
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_N_2_JSTATUS TO SDE_VIEWER to SDE_VIEWER;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_N_2_JSTATUS TO SDE_VIEWER
/
