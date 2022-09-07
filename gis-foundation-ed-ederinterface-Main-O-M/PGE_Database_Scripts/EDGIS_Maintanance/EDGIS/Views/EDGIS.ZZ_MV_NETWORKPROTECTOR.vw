Prompt drop View ZZ_MV_NETWORKPROTECTOR;
DROP VIEW EDGIS.ZZ_MV_NETWORKPROTECTOR
/

/* Formatted on 6/27/2019 02:55:27 PM (QP5 v5.313) */
PROMPT View ZZ_MV_NETWORKPROTECTOR;
--
-- ZZ_MV_NETWORKPROTECTOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_NETWORKPROTECTOR
(
    OBJECTID,
    SUBTYPECD,
    LABELTEXT,
    COMMENTS,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    SYMBOLROTATION,
    LISTNAME,
    INSTALLATIONDATE,
    RETIREDATE,
    STATUS,
    INSTALLJOBPREFIX,
    INSTALLJOBYEAR,
    SAPEQUIPID,
    TEMPEQUIPID,
    PROTECTORTYPE,
    RELAYTYPE,
    INSTALLJOBNUMBER,
    OPERATINGNUMBER,
    SAPSORTFIELD,
    STREET,
    CITY,
    FUNCTIONALLOC,
    TECHIDENTNO,
    CHANGEDON,
    CHANGEDBY,
    INSTALLCODE,
    NAMEPLATEAMPERAGE,
    SPOTORGRID,
    DESCRIPTION,
    DESCRIPTION2,
    UGMAP,
    PROTECTORVOLTAGE,
    VAULTNUMBER,
    MANUFACTURER,
    MANUFACTURERSERIALNO,
    NETWORKGROUPNUMBER,
    YEAROFMANUFACTURE,
    MATERIALSUPPLYCODE,
    FUSETYPE,
    STRUCTUREGUID,
    STRUCTURECONVID,
    CIRCUITID,
    CIRCUITID2,
    FEEDERINFO,
    ELECTRICTRACEWEIGHT,
    ANCILLARYROLE,
    ENABLED,
    PHASEDESIGNATION,
    PHASINGVERIFIEDSTATUS,
    GLOBALID,
    GEMSDISTMAPNUM,
    GEMSCIRCUITMAPNUM,
    GEMSOTHERMAPNUM,
    SHAPE,
    LOCALOFFICEID,
    SDE_STATE_ID,
    DIVISION,
    DISTRICT,
    LINKLOCATION,
    SUPERVISORYCONTROL,
    ENCLOSURETYPE,
    VERSIONNAME,
    FEEDERTYPE,
    COUNTY
)
AS
    SELECT b.OBJECTID,
           b.SUBTYPECD,
           b.LABELTEXT,
           b.COMMENTS,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.SYMBOLROTATION,
           b.LISTNAME,
           b.INSTALLATIONDATE,
           b.RETIREDATE,
           b.STATUS,
           b.INSTALLJOBPREFIX,
           b.INSTALLJOBYEAR,
           b.SAPEQUIPID,
           b.TEMPEQUIPID,
           b.PROTECTORTYPE,
           b.RELAYTYPE,
           b.INSTALLJOBNUMBER,
           b.OPERATINGNUMBER,
           b.SAPSORTFIELD,
           b.STREET,
           b.CITY,
           b.FUNCTIONALLOC,
           b.TECHIDENTNO,
           b.CHANGEDON,
           b.CHANGEDBY,
           b.INSTALLCODE,
           b.NAMEPLATEAMPERAGE,
           b.SPOTORGRID,
           b.DESCRIPTION,
           b.DESCRIPTION2,
           b.UGMAP,
           b.PROTECTORVOLTAGE,
           b.VAULTNUMBER,
           b.MANUFACTURER,
           b.MANUFACTURERSERIALNO,
           b.NETWORKGROUPNUMBER,
           b.YEAROFMANUFACTURE,
           b.MATERIALSUPPLYCODE,
           b.FUSETYPE,
           b.STRUCTUREGUID,
           b.STRUCTURECONVID,
           b.CIRCUITID,
           b.CIRCUITID2,
           b.FEEDERINFO,
           b.ELECTRICTRACEWEIGHT,
           b.ANCILLARYROLE,
           b.ENABLED,
           b.PHASEDESIGNATION,
           b.PHASINGVERIFIEDSTATUS,
           b.GLOBALID,
           b.GEMSDISTMAPNUM,
           b.GEMSCIRCUITMAPNUM,
           b.GEMSOTHERMAPNUM,
           b.SHAPE,
           b.LOCALOFFICEID,
           0 SDE_STATE_ID,
           b.DIVISION,
           b.DISTRICT,
           b.LINKLOCATION,
           b.SUPERVISORYCONTROL,
           b.ENCLOSURETYPE,
           b.VERSIONNAME,
           b.FEEDERTYPE,
           b.COUNTY
      FROM EDGIS.NETWORKPROTECTOR  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7727
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.SUBTYPECD,
           a.LABELTEXT,
           a.COMMENTS,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.SYMBOLROTATION,
           a.LISTNAME,
           a.INSTALLATIONDATE,
           a.RETIREDATE,
           a.STATUS,
           a.INSTALLJOBPREFIX,
           a.INSTALLJOBYEAR,
           a.SAPEQUIPID,
           a.TEMPEQUIPID,
           a.PROTECTORTYPE,
           a.RELAYTYPE,
           a.INSTALLJOBNUMBER,
           a.OPERATINGNUMBER,
           a.SAPSORTFIELD,
           a.STREET,
           a.CITY,
           a.FUNCTIONALLOC,
           a.TECHIDENTNO,
           a.CHANGEDON,
           a.CHANGEDBY,
           a.INSTALLCODE,
           a.NAMEPLATEAMPERAGE,
           a.SPOTORGRID,
           a.DESCRIPTION,
           a.DESCRIPTION2,
           a.UGMAP,
           a.PROTECTORVOLTAGE,
           a.VAULTNUMBER,
           a.MANUFACTURER,
           a.MANUFACTURERSERIALNO,
           a.NETWORKGROUPNUMBER,
           a.YEAROFMANUFACTURE,
           a.MATERIALSUPPLYCODE,
           a.FUSETYPE,
           a.STRUCTUREGUID,
           a.STRUCTURECONVID,
           a.CIRCUITID,
           a.CIRCUITID2,
           a.FEEDERINFO,
           a.ELECTRICTRACEWEIGHT,
           a.ANCILLARYROLE,
           a.ENABLED,
           a.PHASEDESIGNATION,
           a.PHASINGVERIFIEDSTATUS,
           a.GLOBALID,
           a.GEMSDISTMAPNUM,
           a.GEMSCIRCUITMAPNUM,
           a.GEMSOTHERMAPNUM,
           a.SHAPE,
           a.LOCALOFFICEID,
           a.SDE_STATE_ID,
           a.DIVISION,
           a.DISTRICT,
           a.LINKLOCATION,
           a.SUPERVISORYCONTROL,
           a.ENCLOSURETYPE,
           a.VERSIONNAME,
           a.FEEDERTYPE,
           a.COUNTY
      FROM EDGIS.A7727  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D7727
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V7727_DELETE;
--
-- V7727_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7727_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_NETWORKPROTECTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.NETWORKPROTECTOR WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A7727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7727,current_state); END IF;END;
/


Prompt Trigger V7727_INSERT;
--
-- V7727_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7727_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_NETWORKPROTECTOR REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',7727); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A7727 VALUES (next_rowid,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,current_state,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.NETWORKPROTECTOR VALUES (next_rowid,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY);  ELSE INSERT INTO EDGIS.A7727  VALUES (next_rowid,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,current_state,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7727,current_state);  END IF;END;
/


Prompt Trigger V7727_UPDATE;
--
-- V7727_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V7727_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A7727 VALUES (:old.OBJECTID,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,current_state,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY); INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A7727 SET SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COMMENTS = :new.COMMENTS,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SYMBOLROTATION = :new.SYMBOLROTATION,LISTNAME = :new.LISTNAME,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,PROTECTORTYPE = :new.PROTECTORTYPE,RELAYTYPE = :new.RELAYTYPE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,OPERATINGNUMBER = :new.OPERATINGNUMBER,SAPSORTFIELD = :new.SAPSORTFIELD,STREET = :new.STREET,CITY = :new.CITY,FUNCTIONALLOC = :new.FUNCTIONALLOC,TECHIDENTNO = :new.TECHIDENTNO,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,INSTALLCODE = :new.INSTALLCODE,NAMEPLATEAMPERAGE = :new.NAMEPLATEAMPERAGE,SPOTORGRID = :new.SPOTORGRID,DESCRIPTION = :new.DESCRIPTION,DESCRIPTION2 = :new.DESCRIPTION2,UGMAP = :new.UGMAP,PROTECTORVOLTAGE = :new.PROTECTORVOLTAGE,VAULTNUMBER = :new.VAULTNUMBER,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIALNO = :new.MANUFACTURERSERIALNO,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,YEAROFMANUFACTURE = :new.YEAROFMANUFACTURE,MATERIALSUPPLYCODE = :new.MATERIALSUPPLYCODE,FUSETYPE = :new.FUSETYPE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,FEEDERINFO = :new.FEEDERINFO,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,GLOBALID = :new.GLOBALID,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,SHAPE = :new.SHAPE,LOCALOFFICEID = :new.LOCALOFFICEID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,LINKLOCATION = :new.LINKLOCATION,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,ENCLOSURETYPE = :new.ENCLOSURETYPE,VERSIONNAME = :new.VERSIONNAME,FEEDERTYPE = :new.FEEDERTYPE,COUNTY = :new.COUNTY WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d7727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d7727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A7727 VALUES (:old.OBJECTID,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,current_state,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY); INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.NETWORKPROTECTOR SET SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COMMENTS = :new.COMMENTS,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SYMBOLROTATION = :new.SYMBOLROTATION,LISTNAME = :new.LISTNAME,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,PROTECTORTYPE = :new.PROTECTORTYPE,RELAYTYPE = :new.RELAYTYPE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,OPERATINGNUMBER = :new.OPERATINGNUMBER,SAPSORTFIELD = :new.SAPSORTFIELD,STREET = :new.STREET,CITY = :new.CITY,FUNCTIONALLOC = :new.FUNCTIONALLOC,TECHIDENTNO = :new.TECHIDENTNO,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,INSTALLCODE = :new.INSTALLCODE,NAMEPLATEAMPERAGE = :new.NAMEPLATEAMPERAGE,SPOTORGRID = :new.SPOTORGRID,DESCRIPTION = :new.DESCRIPTION,DESCRIPTION2 = :new.DESCRIPTION2,UGMAP = :new.UGMAP,PROTECTORVOLTAGE = :new.PROTECTORVOLTAGE,VAULTNUMBER = :new.VAULTNUMBER,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIALNO = :new.MANUFACTURERSERIALNO,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,YEAROFMANUFACTURE = :new.YEAROFMANUFACTURE,MATERIALSUPPLYCODE = :new.MATERIALSUPPLYCODE,FUSETYPE = :new.FUSETYPE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,FEEDERINFO = :new.FEEDERINFO,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,GLOBALID = :new.GLOBALID,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,SHAPE = :new.SHAPE,LOCALOFFICEID = :new.LOCALOFFICEID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,LINKLOCATION = :new.LINKLOCATION,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,ENCLOSURETYPE = :new.ENCLOSURETYPE,VERSIONNAME = :new.VERSIONNAME,FEEDERTYPE = :new.FEEDERTYPE,COUNTY = :new.COUNTY WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A7727 VALUES (:old.OBJECTID,:new.SUBTYPECD,:new.LABELTEXT,:new.COMMENTS,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.SYMBOLROTATION,:new.LISTNAME,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.STATUS,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.PROTECTORTYPE,:new.RELAYTYPE,:new.INSTALLJOBNUMBER,:new.OPERATINGNUMBER,:new.SAPSORTFIELD,:new.STREET,:new.CITY,:new.FUNCTIONALLOC,:new.TECHIDENTNO,:new.CHANGEDON,:new.CHANGEDBY,:new.INSTALLCODE,:new.NAMEPLATEAMPERAGE,:new.SPOTORGRID,:new.DESCRIPTION,:new.DESCRIPTION2,:new.UGMAP,:new.PROTECTORVOLTAGE,:new.VAULTNUMBER,:new.MANUFACTURER,:new.MANUFACTURERSERIALNO,:new.NETWORKGROUPNUMBER,:new.YEAROFMANUFACTURE,:new.MATERIALSUPPLYCODE,:new.FUSETYPE,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CIRCUITID,:new.CIRCUITID2,:new.FEEDERINFO,:new.ELECTRICTRACEWEIGHT,:new.ANCILLARYROLE,:new.ENABLED,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.GLOBALID,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.SHAPE,:new.LOCALOFFICEID,current_state,:new.DIVISION,:new.DISTRICT,:new.LINKLOCATION,:new.SUPERVISORYCONTROL,:new.ENCLOSURETYPE,:new.VERSIONNAME,:new.FEEDERTYPE,:new.COUNTY); INSERT INTO EDGIS.D7727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A7727 SET SUBTYPECD = :new.SUBTYPECD,LABELTEXT = :new.LABELTEXT,COMMENTS = :new.COMMENTS,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,SYMBOLROTATION = :new.SYMBOLROTATION,LISTNAME = :new.LISTNAME,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,STATUS = :new.STATUS,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,PROTECTORTYPE = :new.PROTECTORTYPE,RELAYTYPE = :new.RELAYTYPE,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,OPERATINGNUMBER = :new.OPERATINGNUMBER,SAPSORTFIELD = :new.SAPSORTFIELD,STREET = :new.STREET,CITY = :new.CITY,FUNCTIONALLOC = :new.FUNCTIONALLOC,TECHIDENTNO = :new.TECHIDENTNO,CHANGEDON = :new.CHANGEDON,CHANGEDBY = :new.CHANGEDBY,INSTALLCODE = :new.INSTALLCODE,NAMEPLATEAMPERAGE = :new.NAMEPLATEAMPERAGE,SPOTORGRID = :new.SPOTORGRID,DESCRIPTION = :new.DESCRIPTION,DESCRIPTION2 = :new.DESCRIPTION2,UGMAP = :new.UGMAP,PROTECTORVOLTAGE = :new.PROTECTORVOLTAGE,VAULTNUMBER = :new.VAULTNUMBER,MANUFACTURER = :new.MANUFACTURER,MANUFACTURERSERIALNO = :new.MANUFACTURERSERIALNO,NETWORKGROUPNUMBER = :new.NETWORKGROUPNUMBER,YEAROFMANUFACTURE = :new.YEAROFMANUFACTURE,MATERIALSUPPLYCODE = :new.MATERIALSUPPLYCODE,FUSETYPE = :new.FUSETYPE,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,FEEDERINFO = :new.FEEDERINFO,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,GLOBALID = :new.GLOBALID,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,SHAPE = :new.SHAPE,LOCALOFFICEID = :new.LOCALOFFICEID,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,LINKLOCATION = :new.LINKLOCATION,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,ENCLOSURETYPE = :new.ENCLOSURETYPE,VERSIONNAME = :new.VERSIONNAME,FEEDERTYPE = :new.FEEDERTYPE,COUNTY = :new.COUNTY WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (7727,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO A0SW
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO DATACONV
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO DMSSTAGING to DMSSTAGING;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO DMSSTAGING
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO GIS_INTERFACE
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO GIS_I_WRITE to GIS_I_WRITE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO GIS_I_WRITE
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO IGPCITEDITOR to IGPCITEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO IGPCITEDITOR
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO IGPEDITOR to IGPEDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO IGPEDITOR
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO M4AB to M4AB;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO M4AB
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO RSDH to RSDH;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO RSDH
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO S7MA
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO SDE to SDE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO SDE
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_NETWORKPROTECTOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_NETWORKPROTECTOR TO SDE_VIEWER
/
