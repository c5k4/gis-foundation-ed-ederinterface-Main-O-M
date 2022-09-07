Prompt drop View ZZ_MV_PRIOHCONDUCTOR;
DROP VIEW EDGIS.ZZ_MV_PRIOHCONDUCTOR
/

/* Formatted on 6/27/2019 02:54:15 PM (QP5 v5.313) */
PROMPT View ZZ_MV_PRIOHCONDUCTOR;
--
-- ZZ_MV_PRIOHCONDUCTOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_PRIOHCONDUCTOR
(
    OBJECTID,
    ENABLED,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    COMMENTS,
    STATUS,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    ELECTRICTRACEWEIGHT,
    TRACEABLE,
    CIRCUITID,
    CIRCUITID2,
    CONVCIRCUITID,
    CONVCIRCUITID2,
    FEEDERINFO,
    CEDSANUMBEROFPHASES,
    COUNTY,
    ZIP,
    REPLACEGUID,
    SUBTYPECD,
    LABELTEXT,
    COASTALIDC,
    MEASUREDLENGTH,
    LENGTHSOURCE,
    JACKETTYPE,
    PHASEDESIGNATION,
    PHASINGVERIFIEDSTATUS,
    OPERATINGVOLTAGE,
    CONSTRUCTIONTYPE,
    CEDSALINESECTIONID,
    WINDSPEEDCODE,
    WINDSPEEDRERATEYEAR,
    MAINLINEIDC,
    NEUTRALIDC,
    SERVICEIDC,
    SPACING,
    LABELTEXT2,
    CUSTAGREEMENTGUID,
    CUSTOMEROWNED,
    LOCALOFFICEID,
    DISTRICT,
    DIVISION,
    REGION,
    INSTALLJOBNUMBER,
    CITY,
    VERSIONNAME,
    CIRCUITNAME,
    NOMINALVOLTAGE,
    SHAPE,
    CIRCUITCOLOR,
    FEEDERTYPE,
    SDE_STATE_ID
)
AS
    SELECT b.OBJECTID,
           b.ENABLED,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.COMMENTS,
           b.STATUS,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.ELECTRICTRACEWEIGHT,
           b.TRACEABLE,
           b.CIRCUITID,
           b.CIRCUITID2,
           b.CONVCIRCUITID,
           b.CONVCIRCUITID2,
           b.FEEDERINFO,
           b.CEDSANUMBEROFPHASES,
           b.COUNTY,
           b.ZIP,
           b.REPLACEGUID,
           b.SUBTYPECD,
           b.LABELTEXT,
           b.COASTALIDC,
           b.MEASUREDLENGTH,
           b.LENGTHSOURCE,
           b.JACKETTYPE,
           b.PHASEDESIGNATION,
           b.PHASINGVERIFIEDSTATUS,
           b.OPERATINGVOLTAGE,
           b.CONSTRUCTIONTYPE,
           b.CEDSALINESECTIONID,
           b.WINDSPEEDCODE,
           b.WINDSPEEDRERATEYEAR,
           b.MAINLINEIDC,
           b.NEUTRALIDC,
           b.SERVICEIDC,
           b.SPACING,
           b.LABELTEXT2,
           b.CUSTAGREEMENTGUID,
           b.CUSTOMEROWNED,
           b.LOCALOFFICEID,
           b.DISTRICT,
           b.DIVISION,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.CITY,
           b.VERSIONNAME,
           b.CIRCUITNAME,
           b.NOMINALVOLTAGE,
           b.SHAPE,
           b.CIRCUITCOLOR,
           b.FEEDERTYPE,
           0 SDE_STATE_ID
      FROM EDGIS.PRIOHCONDUCTOR  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D139
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.ENABLED,
           a.GLOBALID,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.COMMENTS,
           a.STATUS,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.ELECTRICTRACEWEIGHT,
           a.TRACEABLE,
           a.CIRCUITID,
           a.CIRCUITID2,
           a.CONVCIRCUITID,
           a.CONVCIRCUITID2,
           a.FEEDERINFO,
           a.CEDSANUMBEROFPHASES,
           a.COUNTY,
           a.ZIP,
           a.REPLACEGUID,
           a.SUBTYPECD,
           a.LABELTEXT,
           a.COASTALIDC,
           a.MEASUREDLENGTH,
           a.LENGTHSOURCE,
           a.JACKETTYPE,
           a.PHASEDESIGNATION,
           a.PHASINGVERIFIEDSTATUS,
           a.OPERATINGVOLTAGE,
           a.CONSTRUCTIONTYPE,
           a.CEDSALINESECTIONID,
           a.WINDSPEEDCODE,
           a.WINDSPEEDRERATEYEAR,
           a.MAINLINEIDC,
           a.NEUTRALIDC,
           a.SERVICEIDC,
           a.SPACING,
           a.LABELTEXT2,
           a.CUSTAGREEMENTGUID,
           a.CUSTOMEROWNED,
           a.LOCALOFFICEID,
           a.DISTRICT,
           a.DIVISION,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.CITY,
           a.VERSIONNAME,
           a.CIRCUITNAME,
           a.NOMINALVOLTAGE,
           a.SHAPE,
           a.CIRCUITCOLOR,
           a.FEEDERTYPE,
           a.SDE_STATE_ID
      FROM EDGIS.A139  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D139
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V139_DELETE;
--
-- V139_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V139_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A139 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d139 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d139 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.PRIOHCONDUCTOR WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A139 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (139,current_state); END IF;END;
/


Prompt Trigger V139_INSERT;
--
-- V139_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V139_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',139); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A139 VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.PRIOHCONDUCTOR VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A139  VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (139,current_state);  END IF;END;
/


Prompt Trigger V139_UPDATE;
--
-- V139_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V139_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A139 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A139 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COASTALIDC = :new.COASTALIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,JACKETTYPE = :new.JACKETTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,WINDSPEEDCODE = :new.WINDSPEEDCODE,WINDSPEEDRERATEYEAR = :new.WINDSPEEDRERATEYEAR,MAINLINEIDC = :new.MAINLINEIDC,NEUTRALIDC = :new.NEUTRALIDC,SERVICEIDC = :new.SERVICEIDC,SPACING = :new.SPACING,LABELTEXT2 = :new.LABELTEXT2,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,CIRCUITNAME = :new.CIRCUITNAME,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,SHAPE = :new.SHAPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d139 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d139 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A139 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.PRIOHCONDUCTOR SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COASTALIDC = :new.COASTALIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,JACKETTYPE = :new.JACKETTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,WINDSPEEDCODE = :new.WINDSPEEDCODE,WINDSPEEDRERATEYEAR = :new.WINDSPEEDRERATEYEAR,MAINLINEIDC = :new.MAINLINEIDC,NEUTRALIDC = :new.NEUTRALIDC,SERVICEIDC = :new.SERVICEIDC,SPACING = :new.SPACING,LABELTEXT2 = :new.LABELTEXT2,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,CIRCUITNAME = :new.CIRCUITNAME,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,SHAPE = :new.SHAPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A139 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.LABELTEXT,:new.COASTALIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.JACKETTYPE,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.OPERATINGVOLTAGE,:new.CONSTRUCTIONTYPE,:new.CEDSALINESECTIONID,:new.WINDSPEEDCODE,:new.WINDSPEEDRERATEYEAR,:new.MAINLINEIDC,:new.NEUTRALIDC,:new.SERVICEIDC,:new.SPACING,:new.LABELTEXT2,:new.CUSTAGREEMENTGUID,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.VERSIONNAME,:new.CIRCUITNAME,:new.NOMINALVOLTAGE,:new.SHAPE,:new.CIRCUITCOLOR,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D139 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A139 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COASTALIDC = :new.COASTALIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,JACKETTYPE = :new.JACKETTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,CEDSALINESECTIONID = :new.CEDSALINESECTIONID,WINDSPEEDCODE = :new.WINDSPEEDCODE,WINDSPEEDRERATEYEAR = :new.WINDSPEEDRERATEYEAR,MAINLINEIDC = :new.MAINLINEIDC,NEUTRALIDC = :new.NEUTRALIDC,SERVICEIDC = :new.SERVICEIDC,SPACING = :new.SPACING,LABELTEXT2 = :new.LABELTEXT2,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,VERSIONNAME = :new.VERSIONNAME,CIRCUITNAME = :new.CIRCUITNAME,NOMINALVOLTAGE = :new.NOMINALVOLTAGE,SHAPE = :new.SHAPE,CIRCUITCOLOR = :new.CIRCUITCOLOR,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (139,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO A0SW
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO M4AB
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO RSDH
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO S7MA
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO SDE
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_PRIOHCONDUCTOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_PRIOHCONDUCTOR TO SDE_VIEWER
/
