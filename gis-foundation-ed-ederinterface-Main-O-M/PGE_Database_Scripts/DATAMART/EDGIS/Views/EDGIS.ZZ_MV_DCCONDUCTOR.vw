Prompt drop View ZZ_MV_DCCONDUCTOR;
DROP VIEW EDGIS.ZZ_MV_DCCONDUCTOR
/

/* Formatted on 7/2/2019 01:18:47 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DCCONDUCTOR;
--
-- ZZ_MV_DCCONDUCTOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DCCONDUCTOR
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
    CONSTRUCTIONTYPE,
    PHASEDESIGNATION,
    FEEDERNAME,
    COASTALIDC,
    INCONDUITIDC,
    SERVICEIDC,
    MEASUREDLENGTH,
    LENGTHSOURCE,
    LABELTEXT,
    LOCALOFFICEID,
    DISTRICT,
    DIVISION,
    REGION,
    INSTALLJOBNUMBER,
    CITY,
    LABELTEXT2,
    SHAPE,
    SDE_STATE_ID,
    FEEDERTYPE
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
           b.CONSTRUCTIONTYPE,
           b.PHASEDESIGNATION,
           b.FEEDERNAME,
           b.COASTALIDC,
           b.INCONDUITIDC,
           b.SERVICEIDC,
           b.MEASUREDLENGTH,
           b.LENGTHSOURCE,
           b.LABELTEXT,
           b.LOCALOFFICEID,
           b.DISTRICT,
           b.DIVISION,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.CITY,
           b.LABELTEXT2,
           b.SHAPE,
           0 SDE_STATE_ID,
           b.FEEDERTYPE
      FROM EDGIS.DCCONDUCTOR  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D136
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
           a.CONSTRUCTIONTYPE,
           a.PHASEDESIGNATION,
           a.FEEDERNAME,
           a.COASTALIDC,
           a.INCONDUITIDC,
           a.SERVICEIDC,
           a.MEASUREDLENGTH,
           a.LENGTHSOURCE,
           a.LABELTEXT,
           a.LOCALOFFICEID,
           a.DISTRICT,
           a.DIVISION,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.CITY,
           a.LABELTEXT2,
           a.SHAPE,
           a.SDE_STATE_ID,
           a.FEEDERTYPE
      FROM EDGIS.A136  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D136
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V136_DELETE;
--
-- V136_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V136_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DCCONDUCTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A136 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d136 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d136 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DCCONDUCTOR WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A136 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (136,current_state); END IF;END;
/


Prompt Trigger V136_INSERT;
--
-- V136_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V136_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DCCONDUCTOR REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',136); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A136 VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DCCONDUCTOR VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A136  VALUES (next_rowid,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (136,current_state);  END IF;END;
/


Prompt Trigger V136_UPDATE;
--
-- V136_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V136_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A136 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d136 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d136 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DCCONDUCTOR SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A136 VALUES (:old.OBJECTID,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.COMMENTS,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.ELECTRICTRACEWEIGHT,:new.TRACEABLE,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.CEDSANUMBEROFPHASES,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.SUBTYPECD,:new.CONSTRUCTIONTYPE,:new.PHASEDESIGNATION,:new.FEEDERNAME,:new.COASTALIDC,:new.INCONDUITIDC,:new.SERVICEIDC,:new.MEASUREDLENGTH,:new.LENGTHSOURCE,:new.LABELTEXT,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.LABELTEXT2,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D136 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A136 SET ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,TRACEABLE = :new.TRACEABLE,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,CEDSANUMBEROFPHASES = :new.CEDSANUMBEROFPHASES,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,PHASEDESIGNATION = :new.PHASEDESIGNATION,FEEDERNAME = :new.FEEDERNAME,COASTALIDC = :new.COASTALIDC,INCONDUITIDC = :new.INCONDUITIDC,SERVICEIDC = :new.SERVICEIDC,MEASUREDLENGTH = :new.MEASUREDLENGTH,LENGTHSOURCE = :new.LENGTHSOURCE,LABELTEXT = :new.LABELTEXT,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,LABELTEXT2 = :new.LABELTEXT2,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (136,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR TO SDE
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCCONDUCTOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_DCCONDUCTOR TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_DCCONDUCTOR TO SELECT_CATALOG_ROLE
/
