Prompt drop View ZZ_MV_GWOC;
DROP VIEW EDGIS.ZZ_MV_GWOC
/

/* Formatted on 6/27/2019 02:55:43 PM (QP5 v5.313) */
PROMPT View ZZ_MV_GWOC;
--
-- ZZ_MV_GWOC  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_GWOC
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
      FROM EDGIS.GWOC  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7420323
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
      FROM EDGIS.A7420323  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7420323
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7420323_DELETE;
--
-- V7420323_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420323_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_GWOC REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7420323 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7420323 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7420323 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.GWOC WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7420323 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420323,current_state); END IF;END;
/


Prompt Trigger V7420323_INSERT;
--
-- V7420323_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420323_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_GWOC REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',7420323); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A7420323 VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.GWOC VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS);  ELSE INSERT INTO EDGIS.A7420323  VALUES (next_rowid,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420323,current_state);  END IF;END;
/


Prompt Trigger V7420323_UPDATE;
--
-- V7420323_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420323_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_GWOC REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A7420323 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A7420323 SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7420323 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7420323 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A7420323 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.GWOC SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A7420323 VALUES (:old.OBJECTID,:new.SERVICEPOINTID,:new.GENERATIONGUID,:new.SAPEGINOTIFICATION,:new.DATECREATED,:new.METERNUMBER,:new.STREETNAME1,:new.STREETNAME2,:new.CITY,:new.STATE,:new.ZIP,:new.RESOLVED,:new.DATEFIXED,:new.STREETNUMBER,:new.LOCALOFFICEID,:new.CGC12,:new.COMMENTS,current_state); INSERT INTO EDGIS.D7420323 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A7420323 SET SERVICEPOINTID = :new.SERVICEPOINTID,GENERATIONGUID = :new.GENERATIONGUID,SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,DATECREATED = :new.DATECREATED,METERNUMBER = :new.METERNUMBER,STREETNAME1 = :new.STREETNAME1,STREETNAME2 = :new.STREETNAME2,CITY = :new.CITY,STATE = :new.STATE,ZIP = :new.ZIP,RESOLVED = :new.RESOLVED,DATEFIXED = :new.DATEFIXED,STREETNUMBER = :new.STREETNUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,CGC12 = :new.CGC12,COMMENTS = :new.COMMENTS WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420323,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_GWOC TO EDGISBO to EDGISBO;
GRANT SELECT ON EDGIS.ZZ_MV_GWOC TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_GWOC TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_GWOC TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC TO PGEDATA to PGEDATA;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO PGEDATA
/

Prompt Grants on VIEW ZZ_MV_GWOC TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZZ_MV_GWOC TO PUBLIC
/

Prompt Grants on VIEW ZZ_MV_GWOC TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO SDE
/

Prompt Grants on VIEW ZZ_MV_GWOC TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GWOC TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_GWOC TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_GWOC TO SDE_VIEWER
/
