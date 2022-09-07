Prompt drop Trigger V144_UPDATE;
DROP TRIGGER EDGIS.V144_UPDATE
/

Prompt Trigger V144_UPDATE;
--
-- V144_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V144_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_SUPPORTSTRUCTURE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A144 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d144 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d144 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.SUPPORTSTRUCTURE SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A144 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.CUSTOMEROWNED,:new.COMMENTS,:new.STATUS,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SYMBOLNUMBER,:new.GPSLATITUDE,:new.GPSLONGITUDE,:new.GPSSOURCE,:new.SOURCEACCURACY,:new.MAPOFFICE,:new.DISTMAP,:new.OTHERMAP,:new.LOCDESC1,:new.LOCDESC2,:new.ACCESSINFO,:new.URBANRURALCODE,:new.DISTRICT,:new.REGION,:new.DIVISION,:new.SYMBOLROTATION,:new.FUNCTIONALLOCATIONID,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.POLECOUNT,:new.STRUCTURECODE,:new.HEIGHT,:new.MATERIAL,:new.MAXVOLTAGELEVEL,:new.STATEREGAREA,:new.VC_NUM,:new.LABELTEXT,:new.POLENUMBER,:new.SMARTMETERNETWORKDEVICEOID,:new.DEVICEGROUPGUID,:new.LOCDESC3,:new.CEDSAID,:new.TECHIDENTNO,:new.JP_DISTRICT,:new.MANUFACTUREDYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.CLASS,:new.GAUGE,:new.ORIGINALCIRCUMFERENCE,:new.CURRENTCIRCUMFERENCE,:new.MANUFACTURER,:new.SPECIES,:new.ORIGINALTREATMENTTYPE,:new.POLETYPE,:new.POLETOPEXTIDC,:new.FOUNDATIONTYPE,:new.GROUNDEDIDC,:new.EXISTINGREINFORCEMENT,:new.JPSEQUENCE,:new.JPAGREEMENTDATE,:new.SERIALNUMBER,:new.CUSTAGREEMENTGUID,:new.PTTDIDC,:new.REPLACEGUID,:new.STRUCTUREGUID,:new.TRANSMISSIONMILENUMBER,:new.LOCALOFFICEID,:new.TRANSMISSIONSTRUCTNUMBER,:new.POLEUSE,:new.JOINTCOUNT,:new.FOREIGNATTACHIDC,:new.CITY,:new.METEREDIDC,:new.SHAPE,current_state,:new.PLDBID,:new.BARCODE,:new.INSTALLJOBNUMBER,:new.JPNUMBER,:new.HFTD); INSERT INTO EDGIS.D144 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A144 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,CUSTOMEROWNED = :new.CUSTOMEROWNED,COMMENTS = :new.COMMENTS,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SYMBOLNUMBER = :new.SYMBOLNUMBER,GPSLATITUDE = :new.GPSLATITUDE,GPSLONGITUDE = :new.GPSLONGITUDE,GPSSOURCE = :new.GPSSOURCE,SOURCEACCURACY = :new.SOURCEACCURACY,MAPOFFICE = :new.MAPOFFICE,DISTMAP = :new.DISTMAP,OTHERMAP = :new.OTHERMAP,LOCDESC1 = :new.LOCDESC1,LOCDESC2 = :new.LOCDESC2,ACCESSINFO = :new.ACCESSINFO,URBANRURALCODE = :new.URBANRURALCODE,DISTRICT = :new.DISTRICT,REGION = :new.REGION,DIVISION = :new.DIVISION,SYMBOLROTATION = :new.SYMBOLROTATION,FUNCTIONALLOCATIONID = :new.FUNCTIONALLOCATIONID,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,POLECOUNT = :new.POLECOUNT,STRUCTURECODE = :new.STRUCTURECODE,HEIGHT = :new.HEIGHT,MATERIAL = :new.MATERIAL,MAXVOLTAGELEVEL = :new.MAXVOLTAGELEVEL,STATEREGAREA = :new.STATEREGAREA,VC_NUM = :new.VC_NUM,LABELTEXT = :new.LABELTEXT,POLENUMBER = :new.POLENUMBER,SMARTMETERNETWORKDEVICEOID = :new.SMARTMETERNETWORKDEVICEOID,DEVICEGROUPGUID = :new.DEVICEGROUPGUID,LOCDESC3 = :new.LOCDESC3,CEDSAID = :new.CEDSAID,TECHIDENTNO = :new.TECHIDENTNO,JP_DISTRICT = :new.JP_DISTRICT,MANUFACTUREDYEAR = :new.MANUFACTUREDYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,CLASS = :new.CLASS,GAUGE = :new.GAUGE,ORIGINALCIRCUMFERENCE = :new.ORIGINALCIRCUMFERENCE,CURRENTCIRCUMFERENCE = :new.CURRENTCIRCUMFERENCE,MANUFACTURER = :new.MANUFACTURER,SPECIES = :new.SPECIES,ORIGINALTREATMENTTYPE = :new.ORIGINALTREATMENTTYPE,POLETYPE = :new.POLETYPE,POLETOPEXTIDC = :new.POLETOPEXTIDC,FOUNDATIONTYPE = :new.FOUNDATIONTYPE,GROUNDEDIDC = :new.GROUNDEDIDC,EXISTINGREINFORCEMENT = :new.EXISTINGREINFORCEMENT,JPSEQUENCE = :new.JPSEQUENCE,JPAGREEMENTDATE = :new.JPAGREEMENTDATE,SERIALNUMBER = :new.SERIALNUMBER,CUSTAGREEMENTGUID = :new.CUSTAGREEMENTGUID,PTTDIDC = :new.PTTDIDC,REPLACEGUID = :new.REPLACEGUID,STRUCTUREGUID = :new.STRUCTUREGUID,TRANSMISSIONMILENUMBER = :new.TRANSMISSIONMILENUMBER,LOCALOFFICEID = :new.LOCALOFFICEID,TRANSMISSIONSTRUCTNUMBER = :new.TRANSMISSIONSTRUCTNUMBER,POLEUSE = :new.POLEUSE,JOINTCOUNT = :new.JOINTCOUNT,FOREIGNATTACHIDC = :new.FOREIGNATTACHIDC,CITY = :new.CITY,METEREDIDC = :new.METEREDIDC,SHAPE = :new.SHAPE,PLDBID = :new.PLDBID,BARCODE = :new.BARCODE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,JPNUMBER = :new.JPNUMBER,HFTD = :new.HFTD WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (144,current_state);  END IF; END;
/
