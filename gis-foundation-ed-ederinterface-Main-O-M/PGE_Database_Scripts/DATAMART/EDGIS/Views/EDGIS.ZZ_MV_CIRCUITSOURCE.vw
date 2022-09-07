Prompt drop View ZZ_MV_CIRCUITSOURCE;
DROP VIEW EDGIS.ZZ_MV_CIRCUITSOURCE
/

/* Formatted on 7/2/2019 01:18:36 PM (QP5 v5.313) */
PROMPT View ZZ_MV_CIRCUITSOURCE;
--
-- ZZ_MV_CIRCUITSOURCE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_CIRCUITSOURCE
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    SUBTYPECD,
    INSTALLATIONDATE,
    MAXPOSITIVESEQUENCEREACTANCE,
    MAXPOSITIVESEQUENCERESISTANCE,
    MAXZEROSEQUENCEREACTANCE,
    MAXZEROSEQUENCERESISTANCE,
    FEEDERID,
    CIRCUITID,
    CIRCUITNAME,
    SUBSTATIONID,
    SUBSTATIONNAME,
    FEEDERSOURCEINFO,
    NOMINALVOLTAGE,
    UNDERFREQUENCYRELAYIDC,
    MINIMUMNORMALVOLTAGE,
    ANNUALLOADFACTOR,
    NETWORKIDC,
    DEVICEGUID,
    DEVICECONVID,
    DIVISION,
    DISTRICT,
    STATUS,
    NETWORKGROUPNAME,
    FEEDERTYPE,
    CIRCUITCOLOR,
    CIRCUITABBREVNAME,
    SDE_STATE_ID,
    FEEDER_CONFIG
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
           b.SUBTYPECD,
           b.INSTALLATIONDATE,
           b.MAXPOSITIVESEQUENCEREACTANCE,
           b.MAXPOSITIVESEQUENCERESISTANCE,
           b.MAXZEROSEQUENCEREACTANCE,
           b.MAXZEROSEQUENCERESISTANCE,
           b.FEEDERID,
           b.CIRCUITID,
           b.CIRCUITNAME,
           b.SUBSTATIONID,
           b.SUBSTATIONNAME,
           b.FEEDERSOURCEINFO,
           b.NOMINALVOLTAGE,
           b.UNDERFREQUENCYRELAYIDC,
           b.MINIMUMNORMALVOLTAGE,
           b.ANNUALLOADFACTOR,
           b.NETWORKIDC,
           b.DEVICEGUID,
           b.DEVICECONVID,
           b.DIVISION,
           b.DISTRICT,
           b.STATUS,
           b.NETWORKGROUPNAME,
           b.FEEDERTYPE,
           b.CIRCUITCOLOR,
           b.CIRCUITABBREVNAME,
           0 SDE_STATE_ID,
           b.FEEDER_CONFIG
      FROM EDGIS.CIRCUITSOURCE  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D98
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
           a.SUBTYPECD,
           a.INSTALLATIONDATE,
           a.MAXPOSITIVESEQUENCEREACTANCE,
           a.MAXPOSITIVESEQUENCERESISTANCE,
           a.MAXZEROSEQUENCEREACTANCE,
           a.MAXZEROSEQUENCERESISTANCE,
           a.FEEDERID,
           a.CIRCUITID,
           a.CIRCUITNAME,
           a.SUBSTATIONID,
           a.SUBSTATIONNAME,
           a.FEEDERSOURCEINFO,
           a.NOMINALVOLTAGE,
           a.UNDERFREQUENCYRELAYIDC,
           a.MINIMUMNORMALVOLTAGE,
           a.ANNUALLOADFACTOR,
           a.NETWORKIDC,
           a.DEVICEGUID,
           a.DEVICECONVID,
           a.DIVISION,
           a.DISTRICT,
           a.STATUS,
           a.NETWORKGROUPNAME,
           a.FEEDERTYPE,
           a.CIRCUITCOLOR,
           a.CIRCUITABBREVNAME,
           a.SDE_STATE_ID,
           a.FEEDER_CONFIG
      FROM EDGIS.A98  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D98
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V98_DELETE;
--
-- V98_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V98_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_CIRCUITSOURCE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A98 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d98 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d98 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.CIRCUITSOURCE WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A98 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (98,current_state); END IF;END;
/


Prompt Trigger V98_INSERT;
--
-- V98_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V98_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_CIRCUITSOURCE REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',98); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A98 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,current_state,:new.FEEDER_CONFIG);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.CIRCUITSOURCE VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,:new.FEEDER_CONFIG);  ELSE INSERT INTO EDGIS.A98  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,current_state,:new.FEEDER_CONFIG);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (98,current_state);  END IF;END;
/


Prompt Trigger V98_UPDATE;
--
-- V98_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V98_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A98 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,current_state,:new.FEEDER_CONFIG); INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A98 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONDATE = :new.INSTALLATIONDATE,MAXPOSITIVESEQUENCEREACTANCE = :new.MAXPOSITIVESEQUENCEREACTANCE,MAXPOSITIVESEQUENCERESISTANCE = :new.MAXPOSITIVESEQUENCERESISTANCE,MAXZEROSEQUENCEREACTANCE = :new.MAXZEROSEQUENCEREACTANCE,MAXZEROSEQUENCERESISTANCE = :new.MAXZEROSEQUENCERESISTANCE,FEEDERID = :new.FEEDERID,CIRCUITID = :new.CIRCUITID,CIRCUITNAME = :new.CIRCUITNAME,SUBSTATIONID = :new.SUBSTATIONID,SUBSTATIONNAME = :new.SUBSTATIONNAME,FEEDERSOURCEINFO = :new.FEEDERSOURCEINFO,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,UNDERFREQUENCYRELAYIDC = :new.UNDERFREQUENCYRELAYIDC,MINIMUMNORMALVOLTAGE = :new.MINIMUMNORMALVOLTAGE,ANNUALLOADFACTOR = :new.ANNUALLOADFACTOR,NETWORKIDC = :new.NETWORKIDC,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,STATUS = :new.STATUS,NETWORKGROUPNAME = :new.NETWORKGROUPNAME,FEEDERTYPE = :new.FEEDERTYPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,CIRCUITABBREVNAME = :new.CIRCUITABBREVNAME,FEEDER_CONFIG = :new.FEEDER_CONFIG WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d98 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d98 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A98 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,current_state,:new.FEEDER_CONFIG); INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.CIRCUITSOURCE SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONDATE = :new.INSTALLATIONDATE,MAXPOSITIVESEQUENCEREACTANCE = :new.MAXPOSITIVESEQUENCEREACTANCE,MAXPOSITIVESEQUENCERESISTANCE = :new.MAXPOSITIVESEQUENCERESISTANCE,MAXZEROSEQUENCEREACTANCE = :new.MAXZEROSEQUENCEREACTANCE,MAXZEROSEQUENCERESISTANCE = :new.MAXZEROSEQUENCERESISTANCE,FEEDERID = :new.FEEDERID,CIRCUITID = :new.CIRCUITID,CIRCUITNAME = :new.CIRCUITNAME,SUBSTATIONID = :new.SUBSTATIONID,SUBSTATIONNAME = :new.SUBSTATIONNAME,FEEDERSOURCEINFO = :new.FEEDERSOURCEINFO,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,UNDERFREQUENCYRELAYIDC = :new.UNDERFREQUENCYRELAYIDC,MINIMUMNORMALVOLTAGE = :new.MINIMUMNORMALVOLTAGE,ANNUALLOADFACTOR = :new.ANNUALLOADFACTOR,NETWORKIDC = :new.NETWORKIDC,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,STATUS = :new.STATUS,NETWORKGROUPNAME = :new.NETWORKGROUPNAME,FEEDERTYPE = :new.FEEDERTYPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,CIRCUITABBREVNAME = :new.CIRCUITABBREVNAME,FEEDER_CONFIG = :new.FEEDER_CONFIG WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A98 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SUBTYPECD,:new.INSTALLATIONDATE,:new.MAXPOSITIVESEQUENCEREACTANCE,:new.MAXPOSITIVESEQUENCERESISTANCE,:new.MAXZEROSEQUENCEREACTANCE,:new.MAXZEROSEQUENCERESISTANCE,:new.FEEDERID,:new.CIRCUITID,:new.CIRCUITNAME,:new.SUBSTATIONID,:new.SUBSTATIONNAME,:new.FEEDERSOURCEINFO,:new.NOMINALVOLTAGE,:new.UNDERFREQUENCYRELAYIDC,:new.MINIMUMNORMALVOLTAGE,:new.ANNUALLOADFACTOR,:new.NETWORKIDC,:new.DEVICEGUID,:new.DEVICECONVID,:new.DIVISION,:new.DISTRICT,:new.STATUS,:new.NETWORKGROUPNAME,:new.FEEDERTYPE,:new.CIRCUITCOLOR,:new.CIRCUITABBREVNAME,current_state,:new.FEEDER_CONFIG); INSERT INTO EDGIS.D98 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A98 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SUBTYPECD = :new.SUBTYPECD,INSTALLATIONDATE = :new.INSTALLATIONDATE,MAXPOSITIVESEQUENCEREACTANCE = :new.MAXPOSITIVESEQUENCEREACTANCE,MAXPOSITIVESEQUENCERESISTANCE = :new.MAXPOSITIVESEQUENCERESISTANCE,MAXZEROSEQUENCEREACTANCE = :new.MAXZEROSEQUENCEREACTANCE,MAXZEROSEQUENCERESISTANCE = :new.MAXZEROSEQUENCERESISTANCE,FEEDERID = :new.FEEDERID,CIRCUITID = :new.CIRCUITID,CIRCUITNAME = :new.CIRCUITNAME,SUBSTATIONID = :new.SUBSTATIONID,SUBSTATIONNAME = :new.SUBSTATIONNAME,FEEDERSOURCEINFO = :new.FEEDERSOURCEINFO,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,UNDERFREQUENCYRELAYIDC = :new.UNDERFREQUENCYRELAYIDC,MINIMUMNORMALVOLTAGE = :new.MINIMUMNORMALVOLTAGE,ANNUALLOADFACTOR = :new.ANNUALLOADFACTOR,NETWORKIDC = :new.NETWORKIDC,DEVICEGUID = :new.DEVICEGUID,DEVICECONVID = :new.DEVICECONVID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,STATUS = :new.STATUS,NETWORKGROUPNAME = :new.NETWORKGROUPNAME,FEEDERTYPE = :new.FEEDERTYPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,CIRCUITABBREVNAME = :new.CIRCUITABBREVNAME,FEEDER_CONFIG = :new.FEEDER_CONFIG WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (98,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO GIS_I to GIS_I;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO PGEDATA to PGEDATA;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO PGEDATA
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO SDE
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_CIRCUITSOURCE TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_CIRCUITSOURCE TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_CIRCUITSOURCE TO SELECT_CATALOG_ROLE
/