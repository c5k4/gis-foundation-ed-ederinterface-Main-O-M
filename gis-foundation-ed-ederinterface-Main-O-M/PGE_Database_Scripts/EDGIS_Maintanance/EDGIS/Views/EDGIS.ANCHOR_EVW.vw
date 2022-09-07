Prompt drop View ANCHOR_EVW;
DROP VIEW EDGIS.ANCHOR_EVW
/

/* Formatted on 6/27/2019 02:59:27 PM (QP5 v5.313) */
PROMPT View ANCHOR_EVW;
--
-- ANCHOR_EVW  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ANCHOR_EVW
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CUSTOMEROWNED,
    STATUS,
    INSTALLATIONDATE,
    RETIREDATE,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    SYMBOLNUMBER,
    SUBTYPECD,
    SYMBOLROTATION,
    JPANUMBER,
    JPASEQUENCE,
    JPADATE,
    LEAD,
    ANCHORSIZE,
    ANCHORTYPE,
    LABELTEXT,
    STRUCTUREGUID,
    STRUCTURECONVID,
    DISTRICT,
    DIVISION,
    REGION,
    INSTALLJOBNUMBER,
    JOINTOWNED,
    MANUFACTURER,
    REPLACEGUID,
    CITY,
    COUNTY,
    ZIP,
    FREEATTACHMENTIDC,
    LOCATIONDESCRIPTION,
    DIRECTION,
    COMMENTS,
    CONVERSIONWORKPACKAGE,
    CONVERSIONID,
    SHAPE,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CUSTOMEROWNED,
           b.STATUS,
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.SYMBOLNUMBER,
           b.SUBTYPECD,
           b.SYMBOLROTATION,
           b.JPANUMBER,
           b.JPASEQUENCE,
           b.JPADATE,
           b.LEAD,
           b.ANCHORSIZE,
           b.ANCHORTYPE,
           b.LABELTEXT,
           b.STRUCTUREGUID,
           b.STRUCTURECONVID,
           b.DISTRICT,
           b.DIVISION,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.JOINTOWNED,
           b.MANUFACTURER,
           b.REPLACEGUID,
           b.CITY,
           b.COUNTY,
           b.ZIP,
           b.FREEATTACHMENTIDC,
           b.LOCATIONDESCRIPTION,
           b.DIRECTION,
           b.COMMENTS,
           b.CONVERSIONWORKPACKAGE,
           b.CONVERSIONID,
           b.SHAPE,
           0 SDE_STATE_ID
      FROM EDGIS.ANCHOR  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7366252
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
           a.CUSTOMEROWNED,
           a.STATUS,
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.SYMBOLNUMBER,
           a.SUBTYPECD,
           a.SYMBOLROTATION,
           a.JPANUMBER,
           a.JPASEQUENCE,
           a.JPADATE,
           a.LEAD,
           a.ANCHORSIZE,
           a.ANCHORTYPE,
           a.LABELTEXT,
           a.STRUCTUREGUID,
           a.STRUCTURECONVID,
           a.DISTRICT,
           a.DIVISION,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.JOINTOWNED,
           a.MANUFACTURER,
           a.REPLACEGUID,
           a.CITY,
           a.COUNTY,
           a.ZIP,
           a.FREEATTACHMENTIDC,
           a.LOCATIONDESCRIPTION,
           a.DIRECTION,
           a.COMMENTS,
           a.CONVERSIONWORKPACKAGE,
           a.CONVERSIONID,
           a.SHAPE,
           a.SDE_STATE_ID
      FROM EDGIS.A7366252  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7366252
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7366252_DELETE;
--
-- V7366252_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366252_DELETE INSTEAD OF DELETE ON EDGIS.ANCHOR_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7366252 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7366252 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7366252 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.ANCHOR WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7366252 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366252,current_state); END IF;END;
/


Prompt Trigger V7366252_INSERT;
--
-- V7366252_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366252_INSERT INSTEAD OF INSERT ON EDGIS.ANCHOR_EVW REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',7366252); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A7366252 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.ANCHOR VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE);  ELSE INSERT INTO EDGIS.A7366252  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366252,current_state);  END IF;END;
/


Prompt Trigger V7366252_UPDATE;
--
-- V7366252_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7366252_UPDATE INSTEAD OF UPDATE ON EDGIS.ANCHOR_EVW REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A7366252 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE,current_state); INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A7366252 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CUSTOMEROWNED = :new.CUSTOMEROWNED,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,JPANUMBER = :new.JPANUMBER,JPASEQUENCE = :new.JPASEQUENCE,JPADATE = :new.JPADATE,LEAD = :new.LEAD,ANCHORSIZE = :new.ANCHORSIZE,ANCHORTYPE = :new.ANCHORTYPE,LABELTEXT = :new.LABELTEXT,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JOINTOWNED = :new.JOINTOWNED,MANUFACTURER = :new.MANUFACTURER,REPLACEGUID = :new.REPLACEGUID,CITY = :new.CITY,COUNTY = :new.COUNTY,ZIP = :new.ZIP,FREEATTACHMENTIDC = :new.FREEATTACHMENTIDC,LOCATIONDESCRIPTION = :new.LOCATIONDESCRIPTION,DIRECTION = :new.DIRECTION,COMMENTS = :new.COMMENTS,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CONVERSIONID = :new.CONVERSIONID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7366252 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7366252 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A7366252 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE,current_state); INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.ANCHOR SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CUSTOMEROWNED = :new.CUSTOMEROWNED,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,JPANUMBER = :new.JPANUMBER,JPASEQUENCE = :new.JPASEQUENCE,JPADATE = :new.JPADATE,LEAD = :new.LEAD,ANCHORSIZE = :new.ANCHORSIZE,ANCHORTYPE = :new.ANCHORTYPE,LABELTEXT = :new.LABELTEXT,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JOINTOWNED = :new.JOINTOWNED,MANUFACTURER = :new.MANUFACTURER,REPLACEGUID = :new.REPLACEGUID,CITY = :new.CITY,COUNTY = :new.COUNTY,ZIP = :new.ZIP,FREEATTACHMENTIDC = :new.FREEATTACHMENTIDC,LOCATIONDESCRIPTION = :new.LOCATIONDESCRIPTION,DIRECTION = :new.DIRECTION,COMMENTS = :new.COMMENTS,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CONVERSIONID = :new.CONVERSIONID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A7366252 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CUSTOMEROWNED,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.SUBTYPECD,:new.SYMBOLROTATION,:new.JPANUMBER,:new.JPASEQUENCE,:new.JPADATE,:new.LEAD,:new.ANCHORSIZE,:new.ANCHORTYPE,:new.LABELTEXT,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.JOINTOWNED,:new.MANUFACTURER,:new.REPLACEGUID,:new.CITY,:new.COUNTY,:new.ZIP,:new.FREEATTACHMENTIDC,:new.LOCATIONDESCRIPTION,:new.DIRECTION,:new.COMMENTS,:new.CONVERSIONWORKPACKAGE,:new.CONVERSIONID,:new.SHAPE,current_state); INSERT INTO EDGIS.D7366252 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A7366252 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CUSTOMEROWNED = :new.CUSTOMEROWNED,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,SUBTYPECD = :new.SUBTYPECD,SYMBOLROTATION = :new.SYMBOLROTATION,JPANUMBER = :new.JPANUMBER,JPASEQUENCE = :new.JPASEQUENCE,JPADATE = :new.JPADATE,LEAD = :new.LEAD,ANCHORSIZE = :new.ANCHORSIZE,ANCHORTYPE = :new.ANCHORTYPE,LABELTEXT = :new.LABELTEXT,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JOINTOWNED = :new.JOINTOWNED,MANUFACTURER = :new.MANUFACTURER,REPLACEGUID = :new.REPLACEGUID,CITY = :new.CITY,COUNTY = :new.COUNTY,ZIP = :new.ZIP,FREEATTACHMENTIDC = :new.FREEATTACHMENTIDC,LOCATIONDESCRIPTION = :new.LOCATIONDESCRIPTION,DIRECTION = :new.DIRECTION,COMMENTS = :new.COMMENTS,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CONVERSIONID = :new.CONVERSIONID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7366252,current_state);  END IF; END;
/


Prompt Grants on VIEW ANCHOR_EVW TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO A0SW
/

Prompt Grants on VIEW ANCHOR_EVW TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO DATACONV
/

Prompt Grants on VIEW ANCHOR_EVW TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO DAT_EDITOR
/

Prompt Grants on VIEW ANCHOR_EVW TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO DMSSTAGING
/

Prompt Grants on VIEW ANCHOR_EVW TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO GISINTERFACE
/

Prompt Grants on VIEW ANCHOR_EVW TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO GIS_I
/

Prompt Grants on VIEW ANCHOR_EVW TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO GIS_INTERFACE
/

Prompt Grants on VIEW ANCHOR_EVW TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO GIS_I_WRITE
/

Prompt Grants on VIEW ANCHOR_EVW TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO IGPCITEDITOR
/

Prompt Grants on VIEW ANCHOR_EVW TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO IGPEDITOR
/

Prompt Grants on VIEW ANCHOR_EVW TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO M4AB
/

Prompt Grants on VIEW ANCHOR_EVW TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO MM_ADMIN
/

Prompt Grants on VIEW ANCHOR_EVW TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO RSDH
/

Prompt Grants on VIEW ANCHOR_EVW TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO S7MA
/

Prompt Grants on VIEW ANCHOR_EVW TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO SDE
/

Prompt Grants on VIEW ANCHOR_EVW TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ANCHOR_EVW TO SDE_EDITOR
/

Prompt Grants on VIEW ANCHOR_EVW TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO SDE_VIEWER
/

Prompt Grants on VIEW ANCHOR_EVW TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ANCHOR_EVW TO SELECT_CATALOG_ROLE
/
