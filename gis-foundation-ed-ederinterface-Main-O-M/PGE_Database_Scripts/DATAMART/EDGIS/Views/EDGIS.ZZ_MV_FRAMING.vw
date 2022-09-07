Prompt drop View ZZ_MV_FRAMING;
DROP VIEW EDGIS.ZZ_MV_FRAMING
/

/* Formatted on 7/2/2019 01:19:09 PM (QP5 v5.313) */
PROMPT View ZZ_MV_FRAMING;
--
-- ZZ_MV_FRAMING  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_FRAMING
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    FRAMINGTYPE,
    FRAMINGPOSITION,
    STRUCTUREGUID,
    STRUCTURECONVID,
    SDE_STATE_ID,
    CONFIGURATION,
    FRAMEMATERIAL,
    FRAMELENGTH,
    ANIMALGUARDIDC,
    CROSSARMCOUNT,
    FRAMINGUSE,
    FRAMINGLEVEL,
    FRAMINGCONSTRUCTION,
    FRAMINGANGLE
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
           b.FRAMINGTYPE,
           b.FRAMINGPOSITION,
           b.STRUCTUREGUID,
           b.STRUCTURECONVID,
           0 SDE_STATE_ID,
           b.CONFIGURATION,
           b.FRAMEMATERIAL,
           b.FRAMELENGTH,
           b.ANIMALGUARDIDC,
           b.CROSSARMCOUNT,
           b.FRAMINGUSE,
           b.FRAMINGLEVEL,
           b.FRAMINGCONSTRUCTION,
           b.FRAMINGANGLE
      FROM EDGIS.FRAMING  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D65
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
           a.FRAMINGTYPE,
           a.FRAMINGPOSITION,
           a.STRUCTUREGUID,
           a.STRUCTURECONVID,
           a.SDE_STATE_ID,
           a.CONFIGURATION,
           a.FRAMEMATERIAL,
           a.FRAMELENGTH,
           a.ANIMALGUARDIDC,
           a.CROSSARMCOUNT,
           a.FRAMINGUSE,
           a.FRAMINGLEVEL,
           a.FRAMINGCONSTRUCTION,
           a.FRAMINGANGLE
      FROM EDGIS.A65  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D65
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V65_DELETE;
--
-- V65_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V65_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_FRAMING REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A65 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d65 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d65 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.FRAMING WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A65 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (65,current_state); END IF;END;
/


Prompt Trigger V65_INSERT;
--
-- V65_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V65_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_FRAMING REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',65); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A65 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,current_state,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.FRAMING VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE);  ELSE INSERT INTO EDGIS.A65  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,current_state,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (65,current_state);  END IF;END;
/


Prompt Trigger V65_UPDATE;
--
-- V65_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V65_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_FRAMING REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A65 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,current_state,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE); INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A65 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,FRAMINGTYPE = :new.FRAMINGTYPE,FRAMINGPOSITION = :new.FRAMINGPOSITION,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CONFIGURATION = :new.CONFIGURATION,FRAMEMATERIAL = :new.FRAMEMATERIAL,FRAMELENGTH = :new.FRAMELENGTH,ANIMALGUARDIDC = :new.ANIMALGUARDIDC,CROSSARMCOUNT = :new.CROSSARMCOUNT,FRAMINGUSE = :new.FRAMINGUSE,FRAMINGLEVEL = :new.FRAMINGLEVEL,FRAMINGCONSTRUCTION = :new.FRAMINGCONSTRUCTION,FRAMINGANGLE = :new.FRAMINGANGLE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d65 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d65 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A65 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,current_state,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE); INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.FRAMING SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,FRAMINGTYPE = :new.FRAMINGTYPE,FRAMINGPOSITION = :new.FRAMINGPOSITION,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CONFIGURATION = :new.CONFIGURATION,FRAMEMATERIAL = :new.FRAMEMATERIAL,FRAMELENGTH = :new.FRAMELENGTH,ANIMALGUARDIDC = :new.ANIMALGUARDIDC,CROSSARMCOUNT = :new.CROSSARMCOUNT,FRAMINGUSE = :new.FRAMINGUSE,FRAMINGLEVEL = :new.FRAMINGLEVEL,FRAMINGCONSTRUCTION = :new.FRAMINGCONSTRUCTION,FRAMINGANGLE = :new.FRAMINGANGLE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A65 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.FRAMINGTYPE,:new.FRAMINGPOSITION,:new.STRUCTUREGUID,:new.STRUCTURECONVID,current_state,:new.CONFIGURATION,:new.FRAMEMATERIAL,:new.FRAMELENGTH,:new.ANIMALGUARDIDC,:new.CROSSARMCOUNT,:new.FRAMINGUSE,:new.FRAMINGLEVEL,:new.FRAMINGCONSTRUCTION,:new.FRAMINGANGLE); INSERT INTO EDGIS.D65 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A65 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,FRAMINGTYPE = :new.FRAMINGTYPE,FRAMINGPOSITION = :new.FRAMINGPOSITION,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CONFIGURATION = :new.CONFIGURATION,FRAMEMATERIAL = :new.FRAMEMATERIAL,FRAMELENGTH = :new.FRAMELENGTH,ANIMALGUARDIDC = :new.ANIMALGUARDIDC,CROSSARMCOUNT = :new.CROSSARMCOUNT,FRAMINGUSE = :new.FRAMINGUSE,FRAMINGLEVEL = :new.FRAMINGLEVEL,FRAMINGCONSTRUCTION = :new.FRAMINGCONSTRUCTION,FRAMINGANGLE = :new.FRAMINGANGLE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (65,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_FRAMING TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_FRAMING TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_FRAMING TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_FRAMING TO SDE
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_FRAMING TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_FRAMING TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_FRAMING TO SELECT_CATALOG_ROLE
/
