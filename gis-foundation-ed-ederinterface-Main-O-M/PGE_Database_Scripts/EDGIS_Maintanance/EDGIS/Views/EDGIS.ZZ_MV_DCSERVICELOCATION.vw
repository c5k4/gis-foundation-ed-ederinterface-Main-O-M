Prompt drop View ZZ_MV_DCSERVICELOCATION;
DROP VIEW EDGIS.ZZ_MV_DCSERVICELOCATION
/

/* Formatted on 6/27/2019 02:56:58 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DCSERVICELOCATION;
--
-- ZZ_MV_DCSERVICELOCATION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DCSERVICELOCATION
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    ANCILLARYROLE,
    ENABLED,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    STATUS,
    ELECTRICTRACEWEIGHT,
    PHASEDESIGNATION,
    PHASINGVERIFIEDSTATUS,
    CIRCUITID,
    CIRCUITID2,
    CONVCIRCUITID,
    CONVCIRCUITID2,
    FEEDERINFO,
    INSTALLATIONDATE,
    LOCATIONID,
    SYMBOLROTATION,
    ELEVATION,
    SUBTYPECD,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    LOCATIONDESC,
    COMMENTS,
    COUNTY,
    ZIP,
    REPLACEGUID,
    LOADSOURCEGUID,
    SERVICEPOINTID,
    SERVICEADDRESS1,
    SERVICEADDRESS2,
    STATE,
    STREETNUMBER,
    CGC12,
    CONVTRANSFORMERID,
    CUSTAGREEMENTGUID,
    GEMSOTHERMAPNUM,
    DIVISION,
    CUSTOMEROWNED,
    LOCALOFFICEID,
    REGION,
    INSTALLJOBNUMBER,
    DISTRICT,
    CITY,
    SECONDARYLOADPOINTGUID,
    SHAPE,
    SDE_STATE_ID,
    FEEDERTYPE
)
AS
    SELECT b.OBJECTID,
           b.GLOBALID,
           b.CREATIONUSER,
           b.ANCILLARYROLE,
           b.ENABLED,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.STATUS,
           b.ELECTRICTRACEWEIGHT,
           b.PHASEDESIGNATION,
           b.PHASINGVERIFIEDSTATUS,
           b.CIRCUITID,
           b.CIRCUITID2,
           b.CONVCIRCUITID,
           b.CONVCIRCUITID2,
           b.FEEDERINFO,
           b.INSTALLATIONDATE,
           b.LOCATIONID,
           b.SYMBOLROTATION,
           b.ELEVATION,
           b.SUBTYPECD,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.LOCATIONDESC,
           b.COMMENTS,
           b.COUNTY,
           b.ZIP,
           b.REPLACEGUID,
           b.LOADSOURCEGUID,
           b.SERVICEPOINTID,
           b.SERVICEADDRESS1,
           b.SERVICEADDRESS2,
           b.STATE,
           b.STREETNUMBER,
           b.CGC12,
           b.CONVTRANSFORMERID,
           b.CUSTAGREEMENTGUID,
           b.GEMSOTHERMAPNUM,
           b.DIVISION,
           b.CUSTOMEROWNED,
           b.LOCALOFFICEID,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.DISTRICT,
           b.CITY,
           b.SECONDARYLOADPOINTGUID,
           b.SHAPE,
           0 SDE_STATE_ID,
           b.FEEDERTYPE
      FROM EDGIS.DCSERVICELOCATION  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D16727
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
           a.ANCILLARYROLE,
           a.ENABLED,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.STATUS,
           a.ELECTRICTRACEWEIGHT,
           a.PHASEDESIGNATION,
           a.PHASINGVERIFIEDSTATUS,
           a.CIRCUITID,
           a.CIRCUITID2,
           a.CONVCIRCUITID,
           a.CONVCIRCUITID2,
           a.FEEDERINFO,
           a.INSTALLATIONDATE,
           a.LOCATIONID,
           a.SYMBOLROTATION,
           a.ELEVATION,
           a.SUBTYPECD,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.LOCATIONDESC,
           a.COMMENTS,
           a.COUNTY,
           a.ZIP,
           a.REPLACEGUID,
           a.LOADSOURCEGUID,
           a.SERVICEPOINTID,
           a.SERVICEADDRESS1,
           a.SERVICEADDRESS2,
           a.STATE,
           a.STREETNUMBER,
           a.CGC12,
           a.CONVTRANSFORMERID,
           a.CUSTAGREEMENTGUID,
           a.GEMSOTHERMAPNUM,
           a.DIVISION,
           a.CUSTOMEROWNED,
           a.LOCALOFFICEID,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.DISTRICT,
           a.CITY,
           a.SECONDARYLOADPOINTGUID,
           a.SHAPE,
           a.SDE_STATE_ID,
           a.FEEDERTYPE
      FROM EDGIS.A16727  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D16727
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V16727_DELETE;
--
-- V16727_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V16727_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DCSERVICELOCATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A16727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d16727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d16727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DCSERVICELOCATION WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A16727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (16727,current_state); END IF;END;
/


Prompt Trigger V16727_INSERT;
--
-- V16727_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V16727_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DCSERVICELOCATION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',16727); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A16727 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,current_state,:new.FEEDERTYPE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DCSERVICELOCATION VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A16727  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,current_state,:new.FEEDERTYPE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (16727,current_state);  END IF;END;
/


Prompt Trigger V16727_UPDATE;
--
-- V16727_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V16727_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A16727 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A16727 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,SUBTYPECD = :new.SUBTYPECD,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,LOADSOURCEGUID = :new.LOADSOURCEGUID,SERVICEPOINTID = :new.SERVICEPOINTID,SERVICEADDRESS1 = :new.SERVICEADDRESS1,SERVICEADDRESS2 = :new.SERVICEADDRESS2,STATE = :new.STATE,STREETNUMBER = :new.STREETNUMBER,CGC12 = :new.CGC12,CONVTRANSFORMERID = :new.CONVTRANSFORMERID,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,SECONDARYLOADPOINTGUID = :new.SECONDARYLOADPOINTGUID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d16727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d16727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A16727 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DCSERVICELOCATION SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,SUBTYPECD = :new.SUBTYPECD,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,LOADSOURCEGUID = :new.LOADSOURCEGUID,SERVICEPOINTID = :new.SERVICEPOINTID,SERVICEADDRESS1 = :new.SERVICEADDRESS1,SERVICEADDRESS2 = :new.SERVICEADDRESS2,STATE = :new.STATE,STREETNUMBER = :new.STREETNUMBER,CGC12 = :new.CGC12,CONVTRANSFORMERID = :new.CONVTRANSFORMERID,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,SECONDARYLOADPOINTGUID = :new.SECONDARYLOADPOINTGUID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A16727 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.ANCILLARYROLE,:new.ENABLED,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.ELEVATION,:new.SUBTYPECD,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.REPLACEGUID,:new.LOADSOURCEGUID,:new.SERVICEPOINTID,:new.SERVICEADDRESS1,:new.SERVICEADDRESS2,:new.STATE,:new.STREETNUMBER,:new.CGC12,:new.CONVTRANSFORMERID,:new.CUSTAGREEMENTGUID,:new.GEMSOTHERMAPNUM,:new.DIVISION,:new.CUSTOMEROWNED,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.DISTRICT,:new.CITY,:new.SECONDARYLOADPOINTGUID,:new.SHAPE,current_state,:new.FEEDERTYPE); INSERT INTO EDGIS.D16727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A16727 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,ELEVATION = :new.ELEVATION,SUBTYPECD = :new.SUBTYPECD,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,REPLACEGUID = :new.REPLACEGUID,LOADSOURCEGUID = :new.LOADSOURCEGUID,SERVICEPOINTID = :new.SERVICEPOINTID,SERVICEADDRESS1 = :new.SERVICEADDRESS1,SERVICEADDRESS2 = :new.SERVICEADDRESS2,STATE = :new.STATE,STREETNUMBER = :new.STREETNUMBER,CGC12 = :new.CGC12,CONVTRANSFORMERID = :new.CONVTRANSFORMERID,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,DIVISION = :new.DIVISION,CUSTOMEROWNED = :new.CUSTOMEROWNED,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,DISTRICT = :new.DISTRICT,CITY = :new.CITY,SECONDARYLOADPOINTGUID = :new.SECONDARYLOADPOINTGUID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (16727,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO A0SW
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO M4AB
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO RSDH
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO S7MA
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO SDE
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCSERVICELOCATION TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCSERVICELOCATION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DCSERVICELOCATION TO SDE_VIEWER
/