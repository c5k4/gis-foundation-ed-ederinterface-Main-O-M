Prompt drop Trigger V8741_INSERT;
DROP TRIGGER EDGIS.V8741_INSERT
/

Prompt Trigger V8741_INSERT;
--
-- V8741_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V8741_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DYNPROTDEVICE50ANNO REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',8741); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A8741 VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE,current_state);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DYNPROTDEVICE50ANNO VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE);  ELSE INSERT INTO EDGIS.A8741  VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE,current_state);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (8741,current_state);  END IF;END;
/
