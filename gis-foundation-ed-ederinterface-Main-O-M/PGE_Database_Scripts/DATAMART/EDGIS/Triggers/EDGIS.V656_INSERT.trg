Prompt drop Trigger V656_INSERT;
DROP TRIGGER EDGIS.V656_INSERT
/

Prompt Trigger V656_INSERT;
--
-- V656_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V656_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_PGE_CONFIG REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',656); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A656 VALUES (next_rowid,:new.GLOBALID,:new.GROUP_ID,:new.FCID,:new.CLASS_NAME,:new.FIELD_NAME,:new.MODEL_NAME,:new.XML_CONFIG,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.PGE_CONFIG VALUES (next_rowid,:new.GLOBALID,:new.GROUP_ID,:new.FCID,:new.CLASS_NAME,:new.FIELD_NAME,:new.MODEL_NAME,:new.XML_CONFIG);  ELSE INSERT INTO EDGIS.A656  VALUES (next_rowid,:new.GLOBALID,:new.GROUP_ID,:new.FCID,:new.CLASS_NAME,:new.FIELD_NAME,:new.MODEL_NAME,:new.XML_CONFIG,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (656,current_state);  END IF;END;
/
