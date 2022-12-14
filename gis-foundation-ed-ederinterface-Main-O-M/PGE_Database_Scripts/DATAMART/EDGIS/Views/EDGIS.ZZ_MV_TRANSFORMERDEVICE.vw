Prompt drop View ZZ_MV_TRANSFORMERDEVICE;
DROP VIEW EDGIS.ZZ_MV_TRANSFORMERDEVICE
/

/* Formatted on 7/2/2019 01:20:14 PM (QP5 v5.313) */
PROMPT View ZZ_MV_TRANSFORMERDEVICE;
--
-- ZZ_MV_TRANSFORMERDEVICE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_TRANSFORMERDEVICE
(
    OBJECTID,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    INSTALLATIONDATE,
    RETIREDATE,
    STATUS,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    SAPEQUIPID,
    TEMPEQUIPID,
    SUBTYPECD,
    DESCRIPTION,
    STREET,
    COMMENTS,
    INSULATINGFLUIDTYPE,
    MANUFACTURER,
    MANUFACTURERSERIAL,
    INSTALLCODE,
    NETWORKGROUPNUMBER,
    VAULTNUMBER,
    CIRCUITID,
    LATITUDE,
    LONGITUDE,
    GEMSDISTMAPNUM,
    GEMSCIRCUITMAPNUM,
    UGMAPNUM,
    INSULATINGMEDIUMQTY,
    CONSTRUCTIONTYPE,
    GROUNDSWITCHTYPE,
    MATERIALCODE,
    TRANSFORMERGUID,
    GEMSOTHERMAPNUM,
    COUNTY,
    ZIP,
    INSTALLJOBNUMBER,
    CITY,
    OPERATINGNUMBER,
    FUNCTIONALLOC,
    DESCRIPTION2,
    SAPSORTFIELD,
    SUPERORDEQUIP,
    CHANGEDON,
    CHANGEDBY,
    LISTNAME,
    MAINWORKCTR,
    LOCATION,
    TECHIDENTNO,
    FEEDER,
    SDE_STATE_ID,
    SUPERVISORYCONTROL
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
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.STATUS,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.SAPEQUIPID,
           b.TEMPEQUIPID,
           b.SUBTYPECD,
           b.DESCRIPTION,
           b.STREET,
           b.COMMENTS,
           b.INSULATINGFLUIDTYPE,
           b.MANUFACTURER,
           b.MANUFACTURERSERIAL,
           b.INSTALLCODE,
           b.NETWORKGROUPNUMBER,
           b.VAULTNUMBER,
           b.CIRCUITID,
           b.LATITUDE,
           b.LONGITUDE,
           b.GEMSDISTMAPNUM,
           b.GEMSCIRCUITMAPNUM,
           b.UGMAPNUM,
           b.INSULATINGMEDIUMQTY,
           b.CONSTRUCTIONTYPE,
           b.GROUNDSWITCHTYPE,
           b.MATERIALCODE,
           b.TRANSFORMERGUID,
           b.GEMSOTHERMAPNUM,
           b.COUNTY,
           b.ZIP,
           b.INSTALLJOBNUMBER,
           b.CITY,
           b.OPERATINGNUMBER,
           b.FUNCTIONALLOC,
           b.DESCRIPTION2,
           b.SAPSORTFIELD,
           b.SUPERORDEQUIP,
           b.CHANGEDON,
           b.CHANGEDBY,
           b.LISTNAME,
           b.MAINWORKCTR,
           b.LOCATION,
           b.TECHIDENTNO,
           b.FEEDER,
           0 SDE_STATE_ID,
           b.SUPERVISORYCONTROL
      FROM EDGIS.TRANSFORMERDEVICE  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D84
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
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.STATUS,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.SAPEQUIPID,
           a.TEMPEQUIPID,
           a.SUBTYPECD,
           a.DESCRIPTION,
           a.STREET,
           a.COMMENTS,
           a.INSULATINGFLUIDTYPE,
           a.MANUFACTURER,
           a.MANUFACTURERSERIAL,
           a.INSTALLCODE,
           a.NETWORKGROUPNUMBER,
           a.VAULTNUMBER,
           a.CIRCUITID,
           a.LATITUDE,
           a.LONGITUDE,
           a.GEMSDISTMAPNUM,
           a.GEMSCIRCUITMAPNUM,
           a.UGMAPNUM,
           a.INSULATINGMEDIUMQTY,
           a.CONSTRUCTIONTYPE,
           a.GROUNDSWITCHTYPE,
           a.MATERIALCODE,
           a.TRANSFORMERGUID,
           a.GEMSOTHERMAPNUM,
           a.COUNTY,
           a.ZIP,
           a.INSTALLJOBNUMBER,
           a.CITY,
           a.OPERATINGNUMBER,
           a.FUNCTIONALLOC,
           a.DESCRIPTION2,
           a.SAPSORTFIELD,
           a.SUPERORDEQUIP,
           a.CHANGEDON,
           a.CHANGEDBY,
           a.LISTNAME,
           a.MAINWORKCTR,
           a.LOCATION,
           a.TECHIDENTNO,
           a.FEEDER,
           a.SDE_STATE_ID,
           a.SUPERVISORYCONTROL
      FROM EDGIS.A84  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D84
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V84_DELETE;
--
-- V84_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V84_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A84 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d84 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d84 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.TRANSFORMERDEVICE WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A84 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (84,current_state); END IF;END;
/


Prompt Trigger V84_INSERT;
--
-- V84_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V84_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',84); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A84 VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,current_state,:new.SUPERVISORYCONTROL);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.TRANSFORMERDEVICE VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,:new.SUPERVISORYCONTROL);  ELSE INSERT INTO EDGIS.A84  VALUES (next_rowid,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,current_state,:new.SUPERVISORYCONTROL);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (84,current_state);  END IF;END;
/


Prompt Trigger V84_UPDATE;
--
-- V84_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V84_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A84 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,current_state,:new.SUPERVISORYCONTROL); INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A84 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SUBTYPECD = :new.SUBTYPECD,DESCRIPTION = :new.DESCRIPTION,STREET = :new.STREET,COMMENTS = :new.COMMENTS,INSULATINGFLUIDTYPE = :new.INSULATINGFLUIDTYPE,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIAL = :new.MANUFACTURERSERIAL,INSTALLCODE = :new.INSTALLCODE,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,VAULTNUMBER = :new.VAULTNUMBER,CIRCUITID = :new.CIRCUITID,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,UGMAPNUM = :new.UGMAPNUM,INSULATINGMEDIUMQTY = :new.INSULATINGMEDIUMQTY,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,GROUNDSWITCHTYPE = :new.GROUNDSWITCHTYPE,MATERIALCODE = :new.MATERIALCODE,TRANSFORMERGUID = :new.TRANSFORMERGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,COUNTY = :new.COUNTY,ZIP = :new.ZIP,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,FUNCTIONALLOC = :new.FUNCTIONALLOC,DESCRIPTION2 = :new.DESCRIPTION2,SAPSORTFIELD = :new.SAPSORTFIELD,SUPERORDEQUIP = :new.SUPERORDEQUIP,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,LISTNAME = :new.LISTNAME,MAINWORKCTR = :new.MAINWORKCTR,LOCATION = :new.LOCATION,TECHIDENTNO = :new.TECHIDENTNO,FEEDER = :new.FEEDER,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d84 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d84 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A84 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,current_state,:new.SUPERVISORYCONTROL); INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.TRANSFORMERDEVICE SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SUBTYPECD = :new.SUBTYPECD,DESCRIPTION = :new.DESCRIPTION,STREET = :new.STREET,COMMENTS = :new.COMMENTS,INSULATINGFLUIDTYPE = :new.INSULATINGFLUIDTYPE,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIAL = :new.MANUFACTURERSERIAL,INSTALLCODE = :new.INSTALLCODE,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,VAULTNUMBER = :new.VAULTNUMBER,CIRCUITID = :new.CIRCUITID,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,UGMAPNUM = :new.UGMAPNUM,INSULATINGMEDIUMQTY = :new.INSULATINGMEDIUMQTY,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,GROUNDSWITCHTYPE = :new.GROUNDSWITCHTYPE,MATERIALCODE = :new.MATERIALCODE,TRANSFORMERGUID = :new.TRANSFORMERGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,COUNTY = :new.COUNTY,ZIP = :new.ZIP,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,FUNCTIONALLOC = :new.FUNCTIONALLOC,DESCRIPTION2 = :new.DESCRIPTION2,SAPSORTFIELD = :new.SAPSORTFIELD,SUPERORDEQUIP = :new.SUPERORDEQUIP,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,LISTNAME = :new.LISTNAME,MAINWORKCTR = :new.MAINWORKCTR,LOCATION = :new.LOCATION,TECHIDENTNO = :new.TECHIDENTNO,FEEDER = :new.FEEDER,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A84 VALUES (:old.OBJECTID,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.SUBTYPECD,:new.DESCRIPTION,:new.STREET,:new.COMMENTS,:new.INSULATINGFLUIDTYPE,:new.MANUFACTURER,:new.MANUFACTURERSERIAL,:new.INSTALLCODE,:new.NETWORKGROUPNUMBER,:new.VAULTNUMBER,:new.CIRCUITID,:new.LATITUDE,:new.LONGITUDE,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.UGMAPNUM,:new.INSULATINGMEDIUMQTY,:new.CONSTRUCTIONTYPE,:new.GROUNDSWITCHTYPE,:new.MATERIALCODE,:new.TRANSFORMERGUID,:new.GEMSOTHERMAPNUM,:new.COUNTY,:new.ZIP,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.FUNCTIONALLOC,:new.DESCRIPTION2,:new.SAPSORTFIELD,:new.SUPERORDEQUIP,:new.CHANGEDON,:new.CHANGEDBY,:new.LISTNAME,:new.MAINWORKCTR,:new.LOCATION,:new.TECHIDENTNO,:new.FEEDER,current_state,:new.SUPERVISORYCONTROL); INSERT INTO EDGIS.D84 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A84 SET GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,SUBTYPECD = :new.SUBTYPECD,DESCRIPTION = :new.DESCRIPTION,STREET = :new.STREET,COMMENTS = :new.COMMENTS,INSULATINGFLUIDTYPE = :new.INSULATINGFLUIDTYPE,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIAL = :new.MANUFACTURERSERIAL,INSTALLCODE = :new.INSTALLCODE,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,VAULTNUMBER = :new.VAULTNUMBER,CIRCUITID = :new.CIRCUITID,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,UGMAPNUM = :new.UGMAPNUM,INSULATINGMEDIUMQTY = :new.INSULATINGMEDIUMQTY,CONSTRUCTIONTYPE = :new.CONSTRUCTIONTYPE,GROUNDSWITCHTYPE = :new.GROUNDSWITCHTYPE,MATERIALCODE = :new.MATERIALCODE,TRANSFORMERGUID = :new.TRANSFORMERGUID,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,COUNTY = :new.COUNTY,ZIP = :new.ZIP,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,FUNCTIONALLOC = :new.FUNCTIONALLOC,DESCRIPTION2 = :new.DESCRIPTION2,SAPSORTFIELD = :new.SAPSORTFIELD,SUPERORDEQUIP = :new.SUPERORDEQUIP,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,LISTNAME = :new.LISTNAME,MAINWORKCTR = :new.MAINWORKCTR,LOCATION = :new.LOCATION,TECHIDENTNO = :new.TECHIDENTNO,FEEDER = :new.FEEDER,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (84,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO BO_USER
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO SDE
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO SDE_VIEWER
/

Prompt Grants on VIEW ZZ_MV_TRANSFORMERDEVICE TO SELECT_CATALOG_ROLE to SELECT_CATALOG_ROLE;
GRANT SELECT ON EDGIS.ZZ_MV_TRANSFORMERDEVICE TO SELECT_CATALOG_ROLE
/
