Prompt drop View ZZ_MV_SUPPORTSTRUCTURE;
DROP VIEW EDGIS.ZZ_MV_SUPPORTSTRUCTURE
/

/* Formatted on 7/2/2019 01:20:09 PM (QP5 v5.313) */
PROMPT View ZZ_MV_SUPPORTSTRUCTURE;
--
-- ZZ_MV_SUPPORTSTRUCTURE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_SUPPORTSTRUCTURE
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
    POLECOUNT,
    STRUCTURECODE,
    HEIGHT,
    MATERIAL,
    MAXVOLTAGELEVEL,
    STATEREGAREA,
    VC_NUM,
    LABELTEXT,
    POLENUMBER,
    SMARTMETERNETWORKDEVICEOID,
    DEVICEGROUPGUID,
    LOCDESC3,
    CEDSAID,
    TECHIDENTNO,
    JP_DISTRICT,
    MANUFACTUREDYEAR,
    SAPEQUIPID,
    TEMPEQUIPID,
    CLASS,
    GAUGE,
    ORIGINALCIRCUMFERENCE,
    CURRENTCIRCUMFERENCE,
    MANUFACTURER,
    SPECIES,
    ORIGINALTREATMENTTYPE,
    POLETYPE,
    POLETOPEXTIDC,
    FOUNDATIONTYPE,
    GROUNDEDIDC,
    EXISTINGREINFORCEMENT,
    JPSEQUENCE,
    JPAGREEMENTDATE,
    SERIALNUMBER,
    CUSTAGREEMENTGUID,
    PTTDIDC,
    REPLACEGUID,
    STRUCTUREGUID,
    TRANSMISSIONMILENUMBER,
    LOCALOFFICEID,
    TRANSMISSIONSTRUCTNUMBER,
    POLEUSE,
    JOINTCOUNT,
    FOREIGNATTACHIDC,
    CITY,
    METEREDIDC,
    SHAPE,
    SDE_STATE_ID,
    PLDBID,
    BARCODE,
    INSTALLJOBNUMBER,
    JPNUMBER,
    HFTD
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
           b.POLECOUNT,
           b.STRUCTURECODE,
           b.HEIGHT,
           b.MATERIAL,
           b.MAXVOLTAGELEVEL,
           b.STATEREGAREA,
           b.VC_NUM,
           b.LABELTEXT,
           b.POLENUMBER,
           b.SMARTMETERNETWORKDEVICEOID,
           b.DEVICEGROUPGUID,
           b.LOCDESC3,
           b.CEDSAID,
           b.TECHIDENTNO,
           b.JP_DISTRICT,
           b.MANUFACTUREDYEAR,
           b.SAPEQUIPID,
           b.TEMPEQUIPID,
           b.CLASS,
           b.GAUGE,
           b.ORIGINALCIRCUMFERENCE,
           b.CURRENTCIRCUMFERENCE,
           b.MANUFACTURER,
           b.SPECIES,
           b.ORIGINALTREATMENTTYPE,
           b.POLETYPE,
           b.POLETOPEXTIDC,
           b.FOUNDATIONTYPE,
           b.GROUNDEDIDC,
           b.EXISTINGREINFORCEMENT,
           b.JPSEQUENCE,
           b.JPAGREEMENTDATE,
           b.SERIALNUMBER,
           b.CUSTAGREEMENTGUID,
           b.PTTDIDC,
           b.REPLACEGUID,
           b.STRUCTUREGUID,
           b.TRANSMISSIONMILENUMBER,
           b.LOCALOFFICEID,
           b.TRANSMISSIONSTRUCTNUMBER,
           b.POLEUSE,
           b.JOINTCOUNT,
           b.FOREIGNATTACHIDC,
           b.CITY,
           b.METEREDIDC,
           b.SHAPE,
           0 SDE_STATE_ID,
           b.PLDBID,
           b.BARCODE,
           b.INSTALLJOBNUMBER,
           b.JPNUMBER,
           b.HFTD
      FROM EDGIS.SUPPORTSTRUCTURE  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D144
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
           a.POLECOUNT,
           a.STRUCTURECODE,
           a.HEIGHT,
           a.MATERIAL,
           a.MAXVOLTAGELEVEL,
           a.STATEREGAREA,
           a.VC_NUM,
           a.LABELTEXT,
           a.POLENUMBER,
           a.SMARTMETERNETWORKDEVICEOID,
           a.DEVICEGROUPGUID,
           a.LOCDESC3,
           a.CEDSAID,
           a.TECHIDENTNO,
           a.JP_DISTRICT,
           a.MANUFACTUREDYEAR,
           a.SAPEQUIPID,
           a.TEMPEQUIPID,
           a.CLASS,
           a.GAUGE,
           a.ORIGINALCIRCUMFERENCE,
           a.CURRENTCIRCUMFERENCE,
           a.MANUFACTURER,
           a.SPECIES,
           a.ORIGINALTREATMENTTYPE,
           a.POLETYPE,
           a.POLETOPEXTIDC,
           a.FOUNDATIONTYPE,
           a.GROUNDEDIDC,
           a.EXISTINGREINFORCEMENT,
           a.JPSEQUENCE,
           a.JPAGREEMENTDATE,
           a.SERIALNUMBER,
           a.CUSTAGREEMENTGUID,
           a.PTTDIDC,
           a.REPLACEGUID,
           a.STRUCTUREGUID,
           a.TRANSMISSIONMILENUMBER,
           a.LOCALOFFICEID,
           a.TRANSMISSIONSTRUCTNUMBER,
           a.POLEUSE,
           a.JOINTCOUNT,
           a.FOREIGNATTACHIDC,
           a.CITY,
           a.METEREDIDC,
           a.SHAPE,
           a.SDE_STATE_ID,
           a.PLDBID,
           a.BARCODE,
           a.INSTALLJOBNUMBER,
           a.JPNUMBER,
           a.HFTD
      FROM EDGIS.A144  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D144
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V144_DELETE;
--
-- V144_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V144_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A144 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d144 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d144 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.SUPPORTSTRUCTURE WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A144 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (144,current_state); END IF;END;
/


Prompt Trigger V144_INSERT;
--
-- V144_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V144_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',144); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A144 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.SUPPORTSTRUCTURE VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD);  ELSE INSERT INTO EDGIS.A144  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (144,current_state);  END IF;END;
/


Prompt Trigger V144_UPDATE;
--
-- V144_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V144_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A144 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d144 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d144 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.SUPPORTSTRUCTURE SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A144 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (144,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO SDE
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_SUPPORTSTRUCTURE TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE TO SELECT_CATALOG_ROLE
/
