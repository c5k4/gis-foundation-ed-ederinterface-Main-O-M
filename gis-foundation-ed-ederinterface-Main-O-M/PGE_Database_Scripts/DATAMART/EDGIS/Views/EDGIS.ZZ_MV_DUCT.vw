Prompt drop View ZZ_MV_DUCT;
DROP VIEW EDGIS.ZZ_MV_DUCT
/

/* Formatted on 7/2/2019 01:18:58 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DUCT;
--
-- ZZ_MV_DUCT  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DUCT
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    LASTUSER,
    DATEMODIFIED,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    PHASEDESIGNATION,
    HASSUBDUCTS,
    DUCTPOSITION,
    DUCTNAME,
    DUCTSIZE,
    OCCUPIED,
    MATERIAL,
    CUSTOMEROWNED,
    FIBEROPTICIDC,
    VENTIDC,
    DUCTSHAPE,
    BYPASS,
    CONDITION,
    AVAILABLE,
    FACILITYID,
    SDE_STATE_ID,
    TELCOIDC,
    ELECTRICTRANSMISSIONIDC,
    GASLINEIDC,
    SCADAIDC,
    SFDTIDC,
    MUNICABLEIDC,
    TEMPERATUREIDC,
    MISCIDC1,
    MISCIDC2,
    MISCIDC3,
    SHAPE,
    ACTUALSIZE
)
AS
    SELECT b.OBJECTID,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.LASTUSER,
           b.DATEMODIFIED,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.PHASEDESIGNATION,
           b.HASSUBDUCTS,
           b.DUCTPOSITION,
           b.DUCTNAME,
           b.DUCTSIZE,
           b.OCCUPIED,
           b.MATERIAL,
           b.CUSTOMEROWNED,
           b.FIBEROPTICIDC,
           b.VENTIDC,
           b.DUCTSHAPE,
           b.BYPASS,
           b.CONDITION,
           b.AVAILABLE,
           b.FACILITYID,
           0 SDE_STATE_ID,
           b.TELCOIDC,
           b.ELECTRICTRANSMISSIONIDC,
           b.GASLINEIDC,
           b.SCADAIDC,
           b.SFDTIDC,
           b.MUNICABLEIDC,
           b.TEMPERATUREIDC,
           b.MISCIDC1,
           b.MISCIDC2,
           b.MISCIDC3,
           b.SHAPE,
           b.ACTUALSIZE
      FROM EDGIS.DUCT  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D192
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
           a.LASTUSER,
           a.DATEMODIFIED,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.PHASEDESIGNATION,
           a.HASSUBDUCTS,
           a.DUCTPOSITION,
           a.DUCTNAME,
           a.DUCTSIZE,
           a.OCCUPIED,
           a.MATERIAL,
           a.CUSTOMEROWNED,
           a.FIBEROPTICIDC,
           a.VENTIDC,
           a.DUCTSHAPE,
           a.BYPASS,
           a.CONDITION,
           a.AVAILABLE,
           a.FACILITYID,
           a.SDE_STATE_ID,
           a.TELCOIDC,
           a.ELECTRICTRANSMISSIONIDC,
           a.GASLINEIDC,
           a.SCADAIDC,
           a.SFDTIDC,
           a.MUNICABLEIDC,
           a.TEMPERATUREIDC,
           a.MISCIDC1,
           a.MISCIDC2,
           a.MISCIDC3,
           a.SHAPE,
           a.ACTUALSIZE
      FROM EDGIS.A192  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D192
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V192_DELETE;
--
-- V192_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V192_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DUCT REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A192 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d192 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d192 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DUCT WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A192 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (192,current_state); END IF;END;
/


Prompt Trigger V192_INSERT;
--
-- V192_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V192_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DUCT REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',192); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A192 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,current_state,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DUCT VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE);  ELSE INSERT INTO EDGIS.A192  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,current_state,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (192,current_state);  END IF;END;
/


Prompt Trigger V192_UPDATE;
--
-- V192_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V192_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DUCT REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A192 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,current_state,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE); INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A192 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,PHASEDESIGNATION = :new.PHASEDESIGNATION,HASSUBDUCTS = :new.HASSUBDUCTS,DUCTPOSITION = :new.DUCTPOSITION,DUCTNAME = :new.DUCTNAME,DUCTSIZE = :new.DUCTSIZE,OCCUPIED = :new.OCCUPIED,MATERIAL = :new.MATERIAL,CUSTOMEROWNED = :new.CUSTOMEROWNED,FIBEROPTICIDC = :new.FIBEROPTICIDC,VENTIDC = :new.VENTIDC,DUCTSHAPE = :new.DUCTSHAPE,BYPASS = :new.BYPASS,CONDITION = :new.CONDITION,AVAILABLE = :new.AVAILABLE,FACILITYID = :new.FACILITYID,TELCOIDC = :new.TELCOIDC,ELECTRICTRANSMISSIONIDC = :new.ELECTRICTRANSMISSIONIDC,GASLINEIDC = :new.GASLINEIDC,SCADAIDC = :new.SCADAIDC,SFDTIDC = :new.SFDTIDC,MUNICABLEIDC = :new.MUNICABLEIDC,TEMPERATUREIDC = :new.TEMPERATUREIDC,MISCIDC1 = :new.MISCIDC1,MISCIDC2 = :new.MISCIDC2,MISCIDC3 = :new.MISCIDC3,SHAPE = :new.SHAPE,ACTUALSIZE = :new.ACTUALSIZE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d192 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d192 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A192 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,current_state,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE); INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DUCT SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,PHASEDESIGNATION = :new.PHASEDESIGNATION,HASSUBDUCTS = :new.HASSUBDUCTS,DUCTPOSITION = :new.DUCTPOSITION,DUCTNAME = :new.DUCTNAME,DUCTSIZE = :new.DUCTSIZE,OCCUPIED = :new.OCCUPIED,MATERIAL = :new.MATERIAL,CUSTOMEROWNED = :new.CUSTOMEROWNED,FIBEROPTICIDC = :new.FIBEROPTICIDC,VENTIDC = :new.VENTIDC,DUCTSHAPE = :new.DUCTSHAPE,BYPASS = :new.BYPASS,CONDITION = :new.CONDITION,AVAILABLE = :new.AVAILABLE,FACILITYID = :new.FACILITYID,TELCOIDC = :new.TELCOIDC,ELECTRICTRANSMISSIONIDC = :new.ELECTRICTRANSMISSIONIDC,GASLINEIDC = :new.GASLINEIDC,SCADAIDC = :new.SCADAIDC,SFDTIDC = :new.SFDTIDC,MUNICABLEIDC = :new.MUNICABLEIDC,TEMPERATUREIDC = :new.TEMPERATUREIDC,MISCIDC1 = :new.MISCIDC1,MISCIDC2 = :new.MISCIDC2,MISCIDC3 = :new.MISCIDC3,SHAPE = :new.SHAPE,ACTUALSIZE = :new.ACTUALSIZE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A192 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.LASTUSER,:new.DATEMODIFIED,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.PHASEDESIGNATION,:new.HASSUBDUCTS,:new.DUCTPOSITION,:new.DUCTNAME,:new.DUCTSIZE,:new.OCCUPIED,:new.MATERIAL,:new.CUSTOMEROWNED,:new.FIBEROPTICIDC,:new.VENTIDC,:new.DUCTSHAPE,:new.BYPASS,:new.CONDITION,:new.AVAILABLE,:new.FACILITYID,current_state,:new.TELCOIDC,:new.ELECTRICTRANSMISSIONIDC,:new.GASLINEIDC,:new.SCADAIDC,:new.SFDTIDC,:new.MUNICABLEIDC,:new.TEMPERATUREIDC,:new.MISCIDC1,:new.MISCIDC2,:new.MISCIDC3,:new.SHAPE,:new.ACTUALSIZE); INSERT INTO EDGIS.D192 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A192 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,LASTUSER = :new.LASTUSER,DATEMODIFIED = :new.DATEMODIFIED,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,PHASEDESIGNATION = :new.PHASEDESIGNATION,HASSUBDUCTS = :new.HASSUBDUCTS,DUCTPOSITION = :new.DUCTPOSITION,DUCTNAME = :new.DUCTNAME,DUCTSIZE = :new.DUCTSIZE,OCCUPIED = :new.OCCUPIED,MATERIAL = :new.MATERIAL,CUSTOMEROWNED = :new.CUSTOMEROWNED,FIBEROPTICIDC = :new.FIBEROPTICIDC,VENTIDC = :new.VENTIDC,DUCTSHAPE = :new.DUCTSHAPE,BYPASS = :new.BYPASS,CONDITION = :new.CONDITION,AVAILABLE = :new.AVAILABLE,FACILITYID = :new.FACILITYID,TELCOIDC = :new.TELCOIDC,ELECTRICTRANSMISSIONIDC = :new.ELECTRICTRANSMISSIONIDC,GASLINEIDC = :new.GASLINEIDC,SCADAIDC = :new.SCADAIDC,SFDTIDC = :new.SFDTIDC,MUNICABLEIDC = :new.MUNICABLEIDC,TEMPERATUREIDC = :new.TEMPERATUREIDC,MISCIDC1 = :new.MISCIDC1,MISCIDC2 = :new.MISCIDC2,MISCIDC3 = :new.MISCIDC3,SHAPE = :new.SHAPE,ACTUALSIZE = :new.ACTUALSIZE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (192,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DUCT TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_DUCT TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_DUCT TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DUCT TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DUCT TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCT TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_DUCT TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DUCT TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DUCT TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCT TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DUCT TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCT TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DUCT TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCT TO SDE
/

Prompt Grants on VIEW ZZ_MV_DUCT TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCT TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DUCT TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DUCT TO SDE_VIEWER
/
