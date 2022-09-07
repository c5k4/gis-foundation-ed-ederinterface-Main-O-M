Prompt drop Trigger V13727_UPDATE;
DROP TRIGGER EDGIS.V13727_UPDATE
/

Prompt Trigger V13727_UPDATE;
--
-- V13727_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V13727_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_ANTENNA REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A13727 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.STRUCTUREGUID,:new.NUM,:new.CUSTOMEROWNED,:new.GLOBALID,:new.SHAPE,current_state); INSERT INTO EDGIS.D13727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A13727 SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,STRUCTUREGUID = :new.STRUCTUREGUID,NUM = :new.NUM,CUSTOMEROWNED = :new.CUSTOMEROWNED,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d13727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d13727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A13727 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.STRUCTUREGUID,:new.NUM,:new.CUSTOMEROWNED,:new.GLOBALID,:new.SHAPE,current_state); INSERT INTO EDGIS.D13727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.ANTENNA SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,STRUCTUREGUID = :new.STRUCTUREGUID,NUM = :new.NUM,CUSTOMEROWNED = :new.CUSTOMEROWNED,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A13727 VALUES (:old.OBJECTID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.INSTALLATIONDATE,:new.LOCATIONID,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.LOCATIONDESC,:new.COMMENTS,:new.COUNTY,:new.ZIP,:new.SUBTYPECD,:new.LOCALOFFICEID,:new.DISTRICT,:new.DIVISION,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.STRUCTUREGUID,:new.NUM,:new.CUSTOMEROWNED,:new.GLOBALID,:new.SHAPE,current_state); INSERT INTO EDGIS.D13727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A13727 SET CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,INSTALLATIONDATE = :new.INSTALLATIONDATE,LOCATIONID = :new.LOCATIONID,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,LOCATIONDESC = :new.LOCATIONDESC,COMMENTS = :new.COMMENTS,COUNTY = :new.COUNTY,ZIP = :new.ZIP,SUBTYPECD = :new.SUBTYPECD,LOCALOFFICEID = :new.LOCALOFFICEID,DISTRICT = :new.DISTRICT,DIVISION = :new.DIVISION,REGION = :new.REGION,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,CITY = :new.CITY,STRUCTUREGUID = :new.STRUCTUREGUID,NUM = :new.NUM,CUSTOMEROWNED = :new.CUSTOMEROWNED,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (13727,current_state);  END IF; END;
/
