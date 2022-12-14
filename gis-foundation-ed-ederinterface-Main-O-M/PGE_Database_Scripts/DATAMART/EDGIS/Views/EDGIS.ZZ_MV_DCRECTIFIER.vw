Prompt drop View ZZ_MV_DCRECTIFIER;
DROP VIEW EDGIS.ZZ_MV_DCRECTIFIER
/

/* Formatted on 7/2/2019 01:18:51 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DCRECTIFIER;
--
-- ZZ_MV_DCRECTIFIER  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DCRECTIFIER
(
    OBJECTID,
    ANCILLARYROLE,
    ENABLED,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    STATUS,
    ELECTRICTRACEWEIGHT,
    CIRCUITID,
    CIRCUITID2,
    CONVCIRCUITID,
    CONVCIRCUITID2,
    FEEDERINFO,
    INSTALLATIONDATE,
    RETIREDATE,
    NUMBEROFPHASES,
    PHASEDESIGNATION,
    PHASINGVERIFIEDSTATUS,
    SYMBOLROTATION,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    COMMENTS,
    CEDSADEVICEID,
    LOCALOPOFFICE,
    SOURCESIDEDEVICEID,
    COMPLEXDEVICEIDC,
    CUSTOMEROWNED,
    SYMBOLNUMBER,
    LOCDESC,
    COUNTY,
    ZIP,
    GEMSDISTMAPNUM,
    GEMSCIRCUITMAPNUM,
    GEMSOTHERMAPNUM,
    ANIMALGUARDTYPE,
    DIVISION,
    DISTRICT,
    VAULT,
    REPLACEGUID,
    SUBTYPECD,
    RECTIFIERTYPE,
    OPERATINGVOLTAGE,
    OUTPUTVOLTAGE,
    AMPRATING,
    STRUCTUREGUID,
    STRUCTURECONVID,
    INSTALLATIONTYPE,
    LOCALOFFICEID,
    REGION,
    INSTALLJOBNUMBER,
    CITY,
    OPERATINGNUMBER,
    DCRECTIFIERSIZE,
    RECTIFIERNUMBER,
    TRANSFORMERGUID,
    SHAPE,
    PROTECTIVESSD,
    AUTOPROTECTIVESSD,
    SDE_STATE_ID,
    SSDGUID,
    FEEDERTYPE
)
AS
    SELECT b.OBJECTID,
           b.ANCILLARYROLE,
           b.ENABLED,
           b.GLOBALID,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.STATUS,
           b.ELECTRICTRACEWEIGHT,
           b.CIRCUITID,
           b.CIRCUITID2,
           b.CONVCIRCUITID,
           b.CONVCIRCUITID2,
           b.FEEDERINFO,
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.NUMBEROFPHASES,
           b.PHASEDESIGNATION,
           b.PHASINGVERIFIEDSTATUS,
           b.SYMBOLROTATION,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.COMMENTS,
           b.CEDSADEVICEID,
           b.LOCALOPOFFICE,
           b.SOURCESIDEDEVICEID,
           b.COMPLEXDEVICEIDC,
           b.CUSTOMEROWNED,
           b.SYMBOLNUMBER,
           b.LOCDESC,
           b.COUNTY,
           b.ZIP,
           b.GEMSDISTMAPNUM,
           b.GEMSCIRCUITMAPNUM,
           b.GEMSOTHERMAPNUM,
           b.ANIMALGUARDTYPE,
           b.DIVISION,
           b.DISTRICT,
           b.VAULT,
           b.REPLACEGUID,
           b.SUBTYPECD,
           b.RECTIFIERTYPE,
           b.OPERATINGVOLTAGE,
           b.OUTPUTVOLTAGE,
           b.AMPRATING,
           b.STRUCTUREGUID,
           b.STRUCTURECONVID,
           b.INSTALLATIONTYPE,
           b.LOCALOFFICEID,
           b.REGION,
           b.INSTALLJOBNUMBER,
           b.CITY,
           b.OPERATINGNUMBER,
           b.DCRECTIFIERSIZE,
           b.RECTIFIERNUMBER,
           b.TRANSFORMERGUID,
           b.SHAPE,
           b.PROTECTIVESSD,
           b.AUTOPROTECTIVESSD,
           0 SDE_STATE_ID,
           b.SSDGUID,
           b.FEEDERTYPE
      FROM EDGIS.DCRECTIFIER  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D123
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.ANCILLARYROLE,
           a.ENABLED,
           a.GLOBALID,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.STATUS,
           a.ELECTRICTRACEWEIGHT,
           a.CIRCUITID,
           a.CIRCUITID2,
           a.CONVCIRCUITID,
           a.CONVCIRCUITID2,
           a.FEEDERINFO,
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.NUMBEROFPHASES,
           a.PHASEDESIGNATION,
           a.PHASINGVERIFIEDSTATUS,
           a.SYMBOLROTATION,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.COMMENTS,
           a.CEDSADEVICEID,
           a.LOCALOPOFFICE,
           a.SOURCESIDEDEVICEID,
           a.COMPLEXDEVICEIDC,
           a.CUSTOMEROWNED,
           a.SYMBOLNUMBER,
           a.LOCDESC,
           a.COUNTY,
           a.ZIP,
           a.GEMSDISTMAPNUM,
           a.GEMSCIRCUITMAPNUM,
           a.GEMSOTHERMAPNUM,
           a.ANIMALGUARDTYPE,
           a.DIVISION,
           a.DISTRICT,
           a.VAULT,
           a.REPLACEGUID,
           a.SUBTYPECD,
           a.RECTIFIERTYPE,
           a.OPERATINGVOLTAGE,
           a.OUTPUTVOLTAGE,
           a.AMPRATING,
           a.STRUCTUREGUID,
           a.STRUCTURECONVID,
           a.INSTALLATIONTYPE,
           a.LOCALOFFICEID,
           a.REGION,
           a.INSTALLJOBNUMBER,
           a.CITY,
           a.OPERATINGNUMBER,
           a.DCRECTIFIERSIZE,
           a.RECTIFIERNUMBER,
           a.TRANSFORMERGUID,
           a.SHAPE,
           a.PROTECTIVESSD,
           a.AUTOPROTECTIVESSD,
           a.SDE_STATE_ID,
           a.SSDGUID,
           a.FEEDERTYPE
      FROM EDGIS.A123  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D123
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V123_DELETE;
--
-- V123_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V123_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DCRECTIFIER REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A123 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d123 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d123 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DCRECTIFIER WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A123 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (123,current_state); END IF;END;
/


Prompt Trigger V123_INSERT;
--
-- V123_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V123_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DCRECTIFIER REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',123); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A123 VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,current_state,:new.SSDGUID,:new.FEEDERTYPE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DCRECTIFIER VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.SSDGUID,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A123  VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,current_state,:new.SSDGUID,:new.FEEDERTYPE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (123,current_state);  END IF;END;
/


Prompt Trigger V123_UPDATE;
--
-- V123_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V123_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A123 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,current_state,:new.SSDGUID,:new.FEEDERTYPE); INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A123 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,RECTIFIERTYPE = :new.RECTIFIERTYPE,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,OUTPUTVOLTAGE = :new.OUTPUTVOLTAGE,AMPRATING = :new.AMPRATING,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,DCRECTIFIERSIZE = :new.DCRECTIFIERSIZE,RECTIFIERNUMBER = :new.RECTIFIERNUMBER,TRANSFORMERGUID = :new.TRANSFORMERGUID,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,SSDGUID = :new.SSDGUID,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d123 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d123 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A123 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,current_state,:new.SSDGUID,:new.FEEDERTYPE); INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DCRECTIFIER SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,RECTIFIERTYPE = :new.RECTIFIERTYPE,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,OUTPUTVOLTAGE = :new.OUTPUTVOLTAGE,AMPRATING = :new.AMPRATING,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,DCRECTIFIERSIZE = :new.DCRECTIFIERSIZE,RECTIFIERNUMBER = :new.RECTIFIERNUMBER,TRANSFORMERGUID = :new.TRANSFORMERGUID,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,SSDGUID = :new.SSDGUID,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A123 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.RECTIFIERTYPE,:new.OPERATINGVOLTAGE,:new.OUTPUTVOLTAGE,:new.AMPRATING,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.DCRECTIFIERSIZE,:new.RECTIFIERNUMBER,:new.TRANSFORMERGUID,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,current_state,:new.SSDGUID,:new.FEEDERTYPE); INSERT INTO EDGIS.D123 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A123 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,RECTIFIERTYPE = :new.RECTIFIERTYPE,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,OUTPUTVOLTAGE = :new.OUTPUTVOLTAGE,AMPRATING = :new.AMPRATING,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,LOCALOFFICEID = :new.LOCALOFFICEID,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,DCRECTIFIERSIZE = :new.DCRECTIFIERSIZE,RECTIFIERNUMBER = :new.RECTIFIERNUMBER,TRANSFORMERGUID = :new.TRANSFORMERGUID,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,SSDGUID = :new.SSDGUID,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (123,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER TO SDE
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DCRECTIFIER TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_DCRECTIFIER TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_DCRECTIFIER TO SELECT_CATALOG_ROLE
/
