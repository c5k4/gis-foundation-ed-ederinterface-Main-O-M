Prompt drop View ZZ_MV_DUCTANNOTATION;
DROP VIEW EDGIS.ZZ_MV_DUCTANNOTATION
/

/* Formatted on 6/27/2019 02:56:28 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DUCTANNOTATION;
--
-- ZZ_MV_DUCTANNOTATION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DUCTANNOTATION
(
    OBJECTID,
    FEATUREID,
    ZORDER,
    ANNOTATIONCLASSID,
    ELEMENT,
    SYMBOLID,
    STATUS,
    TEXTSTRING,
    FONTNAME,
    FONTSIZE,
    BOLD,
    ITALIC,
    UNDERLINE,
    VERTICALALIGNMENT,
    HORIZONTALALIGNMENT,
    XOFFSET,
    YOFFSET,
    ANGLE,
    FONTLEADING,
    WORDSPACING,
    CHARACTERWIDTH,
    CHARACTERSPACING,
    FLIPANGLE,
    OVERRIDE,
    FEATURECONVERSIONID,
    CONVERSIONWORKPACKAGE,
    GLOBALID,
    FACILITYID,
    SDE_STATE_ID,
    SHAPE,
    PARENTGUID
)
AS
    SELECT b.OBJECTID,
           b.FEATUREID,
           b.ZORDER,
           b.ANNOTATIONCLASSID,
           b.ELEMENT,
           b.SYMBOLID,
           b.STATUS,
           b.TEXTSTRING,
           b.FONTNAME,
           b.FONTSIZE,
           b.BOLD,
           b.ITALIC,
           b.UNDERLINE,
           b.VERTICALALIGNMENT,
           b.HORIZONTALALIGNMENT,
           b.XOFFSET,
           b.YOFFSET,
           b.ANGLE,
           b.FONTLEADING,
           b.WORDSPACING,
           b.CHARACTERWIDTH,
           b.CHARACTERSPACING,
           b.FLIPANGLE,
           b.OVERRIDE,
           b.FEATURECONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.GLOBALID,
           b.FACILITYID,
           0 SDE_STATE_ID,
           b.SHAPE,
           b.PARENTGUID
      FROM EDGIS.DUCTANNOTATION  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D9727
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.FEATUREID,
           a.ZORDER,
           a.ANNOTATIONCLASSID,
           a.ELEMENT,
           a.SYMBOLID,
           a.STATUS,
           a.TEXTSTRING,
           a.FONTNAME,
           a.FONTSIZE,
           a.BOLD,
           a.ITALIC,
           a.UNDERLINE,
           a.VERTICALALIGNMENT,
           a.HORIZONTALALIGNMENT,
           a.XOFFSET,
           a.YOFFSET,
           a.ANGLE,
           a.FONTLEADING,
           a.WORDSPACING,
           a.CHARACTERWIDTH,
           a.CHARACTERSPACING,
           a.FLIPANGLE,
           a.OVERRIDE,
           a.FEATURECONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.GLOBALID,
           a.FACILITYID,
           a.SDE_STATE_ID,
           a.SHAPE,
           a.PARENTGUID
      FROM EDGIS.A9727  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D9727
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V9727_DELETE;
--
-- V9727_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9727_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DUCTANNOTATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A9727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d9727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d9727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DUCTANNOTATION WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A9727 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9727,current_state); END IF;END;
/


Prompt Trigger V9727_INSERT;
--
-- V9727_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9727_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DUCTANNOTATION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',9727); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A9727 VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE,:new.PARENTGUID);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DUCTANNOTATION VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,:new.SHAPE,:new.PARENTGUID);  ELSE INSERT INTO EDGIS.A9727  VALUES (next_rowid,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE,:new.PARENTGUID);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9727,current_state);  END IF;END;
/


Prompt Trigger V9727_UPDATE;
--
-- V9727_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9727_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DUCTANNOTATION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A9727 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE,:new.PARENTGUID); INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A9727 SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE,PARENTGUID = :new.PARENTGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d9727 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d9727 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A9727 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE,:new.PARENTGUID); INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DUCTANNOTATION SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE,PARENTGUID = :new.PARENTGUID WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A9727 VALUES (:old.OBJECTID,:new.FEATUREID,:new.ZORDER,:new.ANNOTATIONCLASSID,:new.ELEMENT,:new.SYMBOLID,:new.STATUS,:new.TEXTSTRING,:new.FONTNAME,:new.FONTSIZE,:new.BOLD,:new.ITALIC,:new.UNDERLINE,:new.VERTICALALIGNMENT,:new.HORIZONTALALIGNMENT,:new.XOFFSET,:new.YOFFSET,:new.ANGLE,:new.FONTLEADING,:new.WORDSPACING,:new.CHARACTERWIDTH,:new.CHARACTERSPACING,:new.FLIPANGLE,:new.OVERRIDE,:new.FEATURECONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE,:new.PARENTGUID); INSERT INTO EDGIS.D9727 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A9727 SET FEATUREID = :new.FEATUREID,ZORDER = :new.ZORDER,ANNOTATIONCLASSID = :new.ANNOTATIONCLASSID,ELEMENT = :new.ELEMENT,SYMBOLID = :new.SYMBOLID,STATUS = :new.STATUS,TEXTSTRING = :new.TEXTSTRING,FONTNAME = :new.FONTNAME,FONTSIZE = :new.FONTSIZE,BOLD = :new.BOLD,ITALIC = :new.ITALIC,UNDERLINE = :new.UNDERLINE,VERTICALALIGNMENT = :new.VERTICALALIGNMENT,HORIZONTALALIGNMENT = :new.HORIZONTALALIGNMENT,XOFFSET = :new.XOFFSET,YOFFSET = :new.YOFFSET,ANGLE = :new.ANGLE,FONTLEADING = :new.FONTLEADING,WORDSPACING = :new.WORDSPACING,CHARACTERWIDTH = :new.CHARACTERWIDTH,CHARACTERSPACING = :new.CHARACTERSPACING,FLIPANGLE = :new.FLIPANGLE,OVERRIDE = :new.OVERRIDE,FEATURECONVERSIONID = :new.FEATURECONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE,PARENTGUID = :new.PARENTGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9727,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO A0SW to A0SW;
GRANT SELECT ON EDGIS.ZZ_MV_DUCTANNOTATION TO A0SW
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DUCTANNOTATION TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCTANNOTATION TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DUCTANNOTATION TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCTANNOTATION TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCTANNOTATION TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO S7MA to S7MA;
GRANT SELECT ON EDGIS.ZZ_MV_DUCTANNOTATION TO S7MA
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCTANNOTATION TO SDE
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DUCTANNOTATION TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DUCTANNOTATION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DUCTANNOTATION TO SDE_VIEWER
/
