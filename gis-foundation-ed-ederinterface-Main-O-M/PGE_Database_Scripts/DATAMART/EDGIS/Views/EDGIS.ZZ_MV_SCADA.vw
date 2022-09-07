Prompt drop View ZZ_MV_SCADA;
DROP VIEW EDGIS.ZZ_MV_SCADA
/

/* Formatted on 7/2/2019 01:19:47 PM (QP5 v5.313) */
PROMPT View ZZ_MV_SCADA;
--
-- ZZ_MV_SCADA  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_SCADA
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    INSTALLATIONDATE,
    RETIREDATE,
    STATUS,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    SAPEQUIPID,
    TEMPEQUIPID,
    SCADATYPE,
    SCADACOMM,
    RTUTYPE,
    DEVICEGUID,
    DEVICECONVID,
    CEDSADEVICEID,
    RADIOMANUFACTURER,
    RADIOMODELNUM,
    RADIOSERIALNUM,
    LEASELINEACCOUNT,
    FLISRAUTOMATIONDEVICEIDC,
    INSTALLJOBNUMBER,
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
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.STATUS,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.SAPEQUIPID,
           b.TEMPEQUIPID,
           b.SCADATYPE,
           b.SCADACOMM,
           b.RTUTYPE,
           b.DEVICEGUID,
           b.DEVICECONVID,
           b.CEDSADEVICEID,
           b.RADIOMANUFACTURER,
           b.RADIOMODELNUM,
           b.RADIOSERIALNUM,
           b.LEASELINEACCOUNT,
           b.FLISRAUTOMATIONDEVICEIDC,
           b.INSTALLJOBNUMBER,
           0 SDE_STATE_ID
      FROM EDGIS.SCADA  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D85
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
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.STATUS,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.SAPEQUIPID,
           a.TEMPEQUIPID,
           a.SCADATYPE,
           a.SCADACOMM,
           a.RTUTYPE,
           a.DEVICEGUID,
           a.DEVICECONVID,
           a.CEDSADEVICEID,
           a.RADIOMANUFACTURER,
           a.RADIOMODELNUM,
           a.RADIOSERIALNUM,
           a.LEASELINEACCOUNT,
           a.FLISRAUTOMATIONDEVICEIDC,
           a.INSTALLJOBNUMBER,
           a.SDE_STATE_ID
      FROM EDGIS.A85  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D85
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V85_DELETE;
--
-- V85_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V85_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_SCADA REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A85 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d85 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d85 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.SCADA WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A85 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (85,current_state); END IF;END;
/


Prompt Trigger V85_INSERT;
--
-- V85_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V85_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_SCADA REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',85); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A85 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.SCADA VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER);  ELSE INSERT INTO EDGIS.A85  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (85,current_state);  END IF;END;
/


Prompt Trigger V85_UPDATE;
--
-- V85_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V85_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_SCADA REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A85 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER,current_state); INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A85 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SCADATYPE = :new.SCADATYPE,SCADACOMM = :new.SCADACOMM,RTUTYPE = :new.RTUTYPE,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,CEDSADEVICEID = :new.CEDSADEVICEID,RADIOMANUFACTURER = :new.RADIOMANUFACTURER,RADIOMODELNUM = :new.RADIOMODELNUM,RADIOSERIALNUM = :new.RADIOSERIALNUM,LEASELINEACCOUNT = :new.LEASELINEACCOUNT,FLISRAUTOMATIONDEVICEIDC = :new.FLISRAUTOMATIONDEVICEIDC,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d85 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d85 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A85 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER,current_state); INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.SCADA SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SCADATYPE = :new.SCADATYPE,SCADACOMM = :new.SCADACOMM,RTUTYPE = :new.RTUTYPE,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,CEDSADEVICEID = :new.CEDSADEVICEID,RADIOMANUFACTURER = :new.RADIOMANUFACTURER,RADIOMODELNUM = :new.RADIOMODELNUM,RADIOSERIALNUM = :new.RADIOSERIALNUM,LEASELINEACCOUNT = :new.LEASELINEACCOUNT,FLISRAUTOMATIONDEVICEIDC = :new.FLISRAUTOMATIONDEVICEIDC,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A85 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SCADATYPE,:new.SCADACOMM,:new.RTUTYPE,:new.DEVICEGUID,:new.DEVICECONVID,:new.CEDSADEVICEID,:new.RADIOMANUFACTURER,:new.RADIOMODELNUM,:new.RADIOSERIALNUM,:new.LEASELINEACCOUNT,:new.FLISRAUTOMATIONDEVICEIDC,:new.INSTALLJOBNUMBER,current_state); INSERT INTO EDGIS.D85 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A85 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SCADATYPE = :new.SCADATYPE,SCADACOMM = :new.SCADACOMM,RTUTYPE = :new.RTUTYPE,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,CEDSADEVICEID = :new.CEDSADEVICEID,RADIOMANUFACTURER = :new.RADIOMANUFACTURER,RADIOMODELNUM = :new.RADIOMODELNUM,RADIOSERIALNUM = :new.RADIOSERIALNUM,LEASELINEACCOUNT = :new.LEASELINEACCOUNT,FLISRAUTOMATIONDEVICEIDC = :new.FLISRAUTOMATIONDEVICEIDC,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (85,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_SCADA TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_SCADA TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_SCADA TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SCADA TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SCADA TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SCADA TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_SCADA TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_SCADA TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_SCADA TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SCADA TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_SCADA TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SCADA TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_SCADA TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SCADA TO SDE
/

Prompt Grants on VIEW ZZ_MV_SCADA TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SCADA TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SCADA TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_SCADA TO SDE_VIEWER
/
