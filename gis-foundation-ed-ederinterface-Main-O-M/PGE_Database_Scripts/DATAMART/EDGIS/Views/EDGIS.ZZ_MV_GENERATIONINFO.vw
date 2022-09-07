Prompt drop View ZZ_MV_GENERATIONINFO;
DROP VIEW EDGIS.ZZ_MV_GENERATIONINFO
/

/* Formatted on 7/2/2019 01:19:12 PM (QP5 v5.313) */
PROMPT View ZZ_MV_GENERATIONINFO;
--
-- ZZ_MV_GENERATIONINFO  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_GENERATIONINFO
(
    SAPEGINOTIFICATION,
    PROJECTNAME,
    GENTYPE,
    PROGRAMTYPE,
    EFFRATINGMACHKW,
    EFFRATINGINVKW,
    EFFRATINGMACHKVA,
    EFFRATINGINVKVA,
    MAXSTORAGECAPACITY,
    CHARGEDEMANDKW,
    GENSYMBOLOGY,
    POWERSOURCE,
    BACKUPGEN,
    OBJECTID,
    GLOBALID,
    DATECREATED,
    CREATEDBY,
    DATEMODIFIED,
    MODIFIEDBY,
    SERVICEPOINTGUID,
    SDE_STATE_ID
)
AS
    SELECT b.SAPEGINOTIFICATION,
           b.PROJECTNAME,
           b.GENTYPE,
           b.PROGRAMTYPE,
           b.EFFRATINGMACHKW,
           b.EFFRATINGINVKW,
           b.EFFRATINGMACHKVA,
           b.EFFRATINGINVKVA,
           b.MAXSTORAGECAPACITY,
           b.CHARGEDEMANDKW,
           b.GENSYMBOLOGY,
           b.POWERSOURCE,
           b.BACKUPGEN,
           b.OBJECTID,
           b.GLOBALID,
           b.DATECREATED,
           b.CREATEDBY,
           b.DATEMODIFIED,
           b.MODIFIEDBY,
           b.SERVICEPOINTGUID,
           0 SDE_STATE_ID
      FROM EDGIS.GENERATIONINFO  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7420322
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.SAPEGINOTIFICATION,
           a.PROJECTNAME,
           a.GENTYPE,
           a.PROGRAMTYPE,
           a.EFFRATINGMACHKW,
           a.EFFRATINGINVKW,
           a.EFFRATINGMACHKVA,
           a.EFFRATINGINVKVA,
           a.MAXSTORAGECAPACITY,
           a.CHARGEDEMANDKW,
           a.GENSYMBOLOGY,
           a.POWERSOURCE,
           a.BACKUPGEN,
           a.OBJECTID,
           a.GLOBALID,
           a.DATECREATED,
           a.CREATEDBY,
           a.DATEMODIFIED,
           a.MODIFIEDBY,
           a.SERVICEPOINTGUID,
           a.SDE_STATE_ID
      FROM EDGIS.A7420322  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7420322
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7420322_DELETE;
--
-- V7420322_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420322_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_GENERATIONINFO REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7420322 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7420322 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7420322 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.GENERATIONINFO WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7420322 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420322,current_state); END IF;END;
/


Prompt Trigger V7420322_INSERT;
--
-- V7420322_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420322_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_GENERATIONINFO REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',7420322); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A7420322 VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,next_rowid,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.GENERATIONINFO VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,next_rowid,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID);  ELSE INSERT INTO EDGIS.A7420322  VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,next_rowid,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420322,current_state);  END IF;END;
/


Prompt Trigger V7420322_UPDATE;
--
-- V7420322_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7420322_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A7420322 VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,:old.OBJECTID,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID,current_state); INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A7420322 SET SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,PROJECTNAME = :new.PROJECTNAME,GENTYPE = :new.GENTYPE,PROGRAMTYPE = :new.PROGRAMTYPE,EFFRATINGMACHKW = :new.EFFRATINGMACHKW,EFFRATINGINVKW = :new.EFFRATINGINVKW,EFFRATINGMACHKVA = :new.EFFRATINGMACHKVA,EFFRATINGINVKVA = :new.EFFRATINGINVKVA,MAXSTORAGECAPACITY = :new.MAXSTORAGECAPACITY,CHARGEDEMANDKW = :new.CHARGEDEMANDKW,GENSYMBOLOGY = :new.GENSYMBOLOGY,POWERSOURCE = :new.POWERSOURCE,BACKUPGEN = :new.BACKUPGEN,GLOBALID = :new.GLOBALID,DATECREATED = :new.DATECREATED,CREATEDBY = :new.CREATEDBY,DATEMODIFIED = :new.DATEMODIFIED,MODIFIEDBY = :new.MODIFIEDBY,SERVICEPOINTGUID = :new.SERVICEPOINTGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7420322 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7420322 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A7420322 VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,:old.OBJECTID,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID,current_state); INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.GENERATIONINFO SET SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,PROJECTNAME = :new.PROJECTNAME,GENTYPE = :new.GENTYPE,PROGRAMTYPE = :new.PROGRAMTYPE,EFFRATINGMACHKW = :new.EFFRATINGMACHKW,EFFRATINGINVKW = :new.EFFRATINGINVKW,EFFRATINGMACHKVA = :new.EFFRATINGMACHKVA,EFFRATINGINVKVA = :new.EFFRATINGINVKVA,MAXSTORAGECAPACITY = :new.MAXSTORAGECAPACITY,CHARGEDEMANDKW = :new.CHARGEDEMANDKW,GENSYMBOLOGY = :new.GENSYMBOLOGY,POWERSOURCE = :new.POWERSOURCE,BACKUPGEN = :new.BACKUPGEN,GLOBALID = :new.GLOBALID,DATECREATED = :new.DATECREATED,CREATEDBY = :new.CREATEDBY,DATEMODIFIED = :new.DATEMODIFIED,MODIFIEDBY = :new.MODIFIEDBY,SERVICEPOINTGUID = :new.SERVICEPOINTGUID WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A7420322 VALUES (:new.SAPEGINOTIFICATION,:new.PROJECTNAME,:new.GENTYPE,:new.PROGRAMTYPE,:new.EFFRATINGMACHKW,:new.EFFRATINGINVKW,:new.EFFRATINGMACHKVA,:new.EFFRATINGINVKVA,:new.MAXSTORAGECAPACITY,:new.CHARGEDEMANDKW,:new.GENSYMBOLOGY,:new.POWERSOURCE,:new.BACKUPGEN,:old.OBJECTID,:new.GLOBALID,:new.DATECREATED,:new.CREATEDBY,:new.DATEMODIFIED,:new.MODIFIEDBY,:new.SERVICEPOINTGUID,current_state); INSERT INTO EDGIS.D7420322 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A7420322 SET SAPEGINOTIFICATION = :new.SAPEGINOTIFICATION,PROJECTNAME = :new.PROJECTNAME,GENTYPE = :new.GENTYPE,PROGRAMTYPE = :new.PROGRAMTYPE,EFFRATINGMACHKW = :new.EFFRATINGMACHKW,EFFRATINGINVKW = :new.EFFRATINGINVKW,EFFRATINGMACHKVA = :new.EFFRATINGMACHKVA,EFFRATINGINVKVA = :new.EFFRATINGINVKVA,MAXSTORAGECAPACITY = :new.MAXSTORAGECAPACITY,CHARGEDEMANDKW = :new.CHARGEDEMANDKW,GENSYMBOLOGY = :new.GENSYMBOLOGY,POWERSOURCE = :new.POWERSOURCE,BACKUPGEN = :new.BACKUPGEN,GLOBALID = :new.GLOBALID,DATECREATED = :new.DATECREATED,CREATEDBY = :new.CREATEDBY,DATEMODIFIED = :new.DATEMODIFIED,MODIFIEDBY = :new.MODIFIEDBY,SERVICEPOINTGUID = :new.SERVICEPOINTGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7420322,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO EDGISBO to EDGISBO;
GRANT SELECT ON EDGIS.ZZ_MV_GENERATIONINFO TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_GENERATIONINFO TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO PGEDATA to PGEDATA;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO PGEDATA
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZZ_MV_GENERATIONINFO TO PUBLIC
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO SDE
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_GENERATIONINFO TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_GENERATIONINFO TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_GENERATIONINFO TO SDE_VIEWER
/
