Prompt drop View ZZ_MV_SUBSTATION;
DROP VIEW EDGIS.ZZ_MV_SUBSTATION
/

/* Formatted on 6/27/2019 02:53:11 PM (QP5 v5.313) */
PROMPT View ZZ_MV_SUBSTATION;
--
-- ZZ_MV_SUBSTATION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_SUBSTATION
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    CUSTOMEROWNED,
    COMMENTS,
    STATUS,
    INSTALLATIONDATE,
    RETIREDATE,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    SYMBOLNUMBER,
    GPSLATITUDE,
    GPSLONGITUDE,
    GPSSOURCE,
    SOURCEACCURACY,
    MAPOFFICE,
    DISTMAP,
    OTHERMAP,
    LOCDESC1,
    LOCDESC2,
    ACCESSINFO,
    URBANRURALCODE,
    DISTRICT,
    REGION,
    DIVISION,
    SYMBOLROTATION,
    FUNCTIONALLOCATIONID,
    COUNTY,
    ZIP,
    SUBTYPECD,
    NAME,
    STATIONNUMBER,
    STATIONTYPE,
    SUBSTATIONID,
    SINGLELINEDRAWINGNUM,
    LOCALOFFICEID,
    INSTALLJOBNUMBER,
    CEDSADEVICEID,
    CONVCIRCUITID,
    CONVCIRCUITID2,
    HYPERLINK,
    CITY,
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
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.CUSTOMEROWNED,
           b.COMMENTS,
           b.STATUS,
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.SYMBOLNUMBER,
           b.GPSLATITUDE,
           b.GPSLONGITUDE,
           b.GPSSOURCE,
           b.SOURCEACCURACY,
           b.MAPOFFICE,
           b.DISTMAP,
           b.OTHERMAP,
           b.LOCDESC1,
           b.LOCDESC2,
           b.ACCESSINFO,
           b.URBANRURALCODE,
           b.DISTRICT,
           b.REGION,
           b.DIVISION,
           b.SYMBOLROTATION,
           b.FUNCTIONALLOCATIONID,
           b.COUNTY,
           b.ZIP,
           b.SUBTYPECD,
           b.NAME,
           b.STATIONNUMBER,
           b.STATIONTYPE,
           b.SUBSTATIONID,
           b.SINGLELINEDRAWINGNUM,
           b.LOCALOFFICEID,
           b.INSTALLJOBNUMBER,
           b.CEDSADEVICEID,
           b.CONVCIRCUITID,
           b.CONVCIRCUITID2,
           b.HYPERLINK,
           b.CITY,
           b.SHAPE,
           0 SDE_STATE_ID
      FROM EDGIS.SUBSTATION  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D143
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
           a.CUSTOMEROWNED,
           a.COMMENTS,
           a.STATUS,
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.SYMBOLNUMBER,
           a.GPSLATITUDE,
           a.GPSLONGITUDE,
           a.GPSSOURCE,
           a.SOURCEACCURACY,
           a.MAPOFFICE,
           a.DISTMAP,
           a.OTHERMAP,
           a.LOCDESC1,
           a.LOCDESC2,
           a.ACCESSINFO,
           a.URBANRURALCODE,
           a.DISTRICT,
           a.REGION,
           a.DIVISION,
           a.SYMBOLROTATION,
           a.FUNCTIONALLOCATIONID,
           a.COUNTY,
           a.ZIP,
           a.SUBTYPECD,
           a.NAME,
           a.STATIONNUMBER,
           a.STATIONTYPE,
           a.SUBSTATIONID,
           a.SINGLELINEDRAWINGNUM,
           a.LOCALOFFICEID,
           a.INSTALLJOBNUMBER,
           a.CEDSADEVICEID,
           a.CONVCIRCUITID,
           a.CONVCIRCUITID2,
           a.HYPERLINK,
           a.CITY,
           a.SHAPE,
           a.SDE_STATE_ID
      FROM EDGIS.A143  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D143
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V143_DELETE;
--
-- V143_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V143_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_SUBSTATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A143 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d143 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d143 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.SUBSTATION WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A143 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (143,current_state); END IF;END;
/


Prompt Trigger V143_INSERT;
--
-- V143_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V143_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_SUBSTATION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',143); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A143 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.SUBSTATION VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE);  ELSE INSERT INTO EDGIS.A143  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (143,current_state);  END IF;END;
/


Prompt Trigger V143_UPDATE;
--
-- V143_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V143_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_SUBSTATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A143 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE,current_state); INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A143 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,NAME = :new.NAME,STATIONNUMBER = :new.STATIONNUMBER,STATIONTYPE = :new.STATIONTYPE,SUBSTATIONID = :new.SUBSTATIONID,SINGLELINEDRAWINGNUM = :new.SINGLELINEDRAWINGNUM,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CEDSADEVICEID = :new.CEDSADEVICEID,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,HYPERLINK = :new.HYPERLINK,CITY = :new.CITY,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d143 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d143 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A143 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE,current_state); INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.SUBSTATION SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,NAME = :new.NAME,STATIONNUMBER = :new.STATIONNUMBER,STATIONTYPE = :new.STATIONTYPE,SUBSTATIONID = :new.SUBSTATIONID,SINGLELINEDRAWINGNUM = :new.SINGLELINEDRAWINGNUM,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CEDSADEVICEID = :new.CEDSADEVICEID,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,HYPERLINK = :new.HYPERLINK,CITY = :new.CITY,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A143 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.NAME,:new.STATIONNUMBER,:new.STATIONTYPE,:new.SUBSTATIONID,:new.SINGLELINEDRAWINGNUM,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.CEDSADEVICEID,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.HYPERLINK,:new.CITY,:new.SHAPE,current_state); INSERT INTO EDGIS.D143 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A143 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,NAME = :new.NAME,STATIONNUMBER = :new.STATIONNUMBER,STATIONTYPE = :new.STATIONTYPE,SUBSTATIONID = :new.SUBSTATIONID,SINGLELINEDRAWINGNUM = :new.SINGLELINEDRAWINGNUM,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CEDSADEVICEID = :new.CEDSADEVICEID,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,HYPERLINK = :new.HYPERLINK,CITY = :new.CITY,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (143,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_SUBSTATION TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_SUBSTATION TO A0SW
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUBSTATION TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SUBSTATION TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_SUBSTATION TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SUBSTATION TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUBSTATION TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUBSTATION TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUBSTATION TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_SUBSTATION TO M4AB
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SUBSTATION TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUBSTATION TO RSDH
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_SUBSTATION TO S7MA
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SUBSTATION TO SDE
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_SUBSTATION TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SUBSTATION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_SUBSTATION TO SDE_VIEWER
/
