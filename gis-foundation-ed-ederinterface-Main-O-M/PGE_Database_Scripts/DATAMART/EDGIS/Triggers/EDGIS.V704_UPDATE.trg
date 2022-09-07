Prompt drop Trigger V704_UPDATE;
DROP TRIGGER EDGIS.V704_UPDATE
/

Prompt Trigger V704_UPDATE;
--
-- V704_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V704_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DEVICEGROUPSCHEM500ANNO REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A704 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D704 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A704 SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d704 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d704 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A704 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D704 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DEVICEGROUPSCHEM500ANNO SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A704 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.GLOBALID,:new.SHAPE,:new.FEEDERTYPE,current_state); INSERT INTO EDGIS.D704 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A704 SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,GLOBALID = :new.GLOBALID,SHAPE = :new.SHAPE,FEEDERTYPE = :new.FEEDERTYPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (704,current_state);  END IF; END;
/
