Prompt drop View ZZ_MV_GWOC_STAGE;
DROP VIEW PGEDATA.ZZ_MV_GWOC_STAGE
/

/* Formatted on 6/27/2019 02:52:09 PM (QP5 v5.313) */
PROMPT View ZZ_MV_GWOC_STAGE;
--
-- ZZ_MV_GWOC_STAGE  (View)
--

CREATE OR REPLACE FORCE VIEW PGEDATA.ZZ_MV_GWOC_STAGE
(
    OBJECTID,
    SERVICEPOINTID,
    GENERATIONGUID,
    SAPEGINOTIFICATION,
    DATECREATED,
    METERNUMBER,
    STREETNAME1,
    STREETNAME2,
    CITY,
    STATE,
    ZIP,
    RESOLVED,
    DATEFIXED,
    STREETNUMBER,
    LOCALOFFICEID,
    CGC12,
    COMMENTS,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.SERVICEPOINTID,
           b.GENERATIONGUID,
           b.SAPEGINOTIFICATION,
           b.DATECREATED,
           b.METERNUMBER,
           b.STREETNAME1,
           b.STREETNAME2,
           b.CITY,
           b.STATE,
           b.ZIP,
           b.RESOLVED,
           b.DATEFIXED,
           b.STREETNUMBER,
           b.LOCALOFFICEID,
           b.CGC12,
           b.COMMENTS,
           0 SDE_STATE_ID
      FROM PGEDATA.GWOC_STAGE  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM PGEDATA.D7420639
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.SERVICEPOINTID,
           a.GENERATIONGUID,
           a.SAPEGINOTIFICATION,
           a.DATECREATED,
           a.METERNUMBER,
           a.STREETNAME1,
           a.STREETNAME2,
           a.CITY,
           a.STATE,
           a.ZIP,
           a.RESOLVED,
           a.DATEFIXED,
           a.STREETNUMBER,
           a.LOCALOFFICEID,
           a.CGC12,
           a.COMMENTS,
           a.SDE_STATE_ID
      FROM PGEDATA.A7420639  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM PGEDATA.D7420639
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7420639_DELETE;
--
-- V7420639_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.V7420639_DELETE INSTEAD OF DELETE ON PGEDATA.ZZ_MV_GWOC_STAGE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM PGEDATA.A7420639 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE PGEDATA.d7420639 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM PGEDATA.d7420639 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM PGEDATA.GWOC_STAGE WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM PGEDATA.A7420639 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420639,current_state); END IF;END;
/


Prompt Trigger V7420639_INSERT;
--
-- V7420639_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.V7420639_INSERT INSTEAD OF INSERT ON PGEDATA.ZZ_MV_GWOC_STAGE REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('PGEDATA',7420639); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO PGEDATA.A7420639 VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO PGEDATA.GWOC_STAGE VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS);  ELSE INSERT INTO PGEDATA.A7420639  VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420639,current_state);  END IF;END;
/


Prompt Trigger V7420639_UPDATE;
--
-- V7420639_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.V7420639_UPDATE INSTEAD OF UPDATE ON PGEDATA.ZZ_MV_GWOC_STAGE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO PGEDATA.A7420639 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE PGEDATA.A7420639 SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE PGEDATA.d7420639 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM PGEDATA.d7420639 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO PGEDATA.A7420639 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE PGEDATA.GWOC_STAGE SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO PGEDATA.A7420639 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO PGEDATA.D7420639 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE PGEDATA.A7420639 SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420639,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.ZZ_MV_GWOC_STAGE TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.ZZ_MV_GWOC_STAGE TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.ZZ_MV_GWOC_STAGE TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO PUBLIC to PUBLIC;
GRANT SELECT ON PGEDATA.ZZ_MV_GWOC_STAGE TO PUBLIC
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON PGEDATA.ZZ_MV_GWOC_STAGE TO SDE
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO SDE_EDITOR to SDE_EDITOR;
GRANT SELECT ON PGEDATA.ZZ_MV_GWOC_STAGE TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC_STAGE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON PGEDATA.ZZ_MV_GWOC_STAGE TO SDE_VIEWER
/
