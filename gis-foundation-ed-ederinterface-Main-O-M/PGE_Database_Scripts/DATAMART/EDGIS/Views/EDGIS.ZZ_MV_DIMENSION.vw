Prompt drop View ZZ_MV_DIMENSION;
DROP VIEW EDGIS.ZZ_MV_DIMENSION
/

/* Formatted on 7/2/2019 01:18:57 PM (QP5 v5.313) */
PROMPT View ZZ_MV_DIMENSION;
--
-- ZZ_MV_DIMENSION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZZ_MV_DIMENSION
(
    OBJECTID,
    DIMLENGTH,
    BEGINX,
    BEGINY,
    ENDX,
    ENDY,
    DIMX,
    DIMY,
    TEXTX,
    TEXTY,
    DIMTYPE,
    EXTANGLE,
    STYLEID,
    USECUSTOMLENGTH,
    CUSTOMLENGTH,
    DIMDISPLAY,
    EXTDISPLAY,
    MARKERDISPLAY,
    TEXTANGLE,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    LOB,
    OWNERUSERID,
    EXPIREDATE,
    COMMENTS,
    GLOBALID,
    FACILITYID,
    SDE_STATE_ID,
    SHAPE
)
AS
    SELECT b.OBJECTID,
           b.DIMLENGTH,
           b.BEGINX,
           b.BEGINY,
           b.ENDX,
           b.ENDY,
           b.DIMX,
           b.DIMY,
           b.TEXTX,
           b.TEXTY,
           b.DIMTYPE,
           b.EXTANGLE,
           b.STYLEID,
           b.USECUSTOMLENGTH,
           b.CUSTOMLENGTH,
           b.DIMDISPLAY,
           b.EXTDISPLAY,
           b.MARKERDISPLAY,
           b.TEXTANGLE,
           b.CREATIONUSER,
           b.DATECREATED,
           b.DATEMODIFIED,
           b.LASTUSER,
           b.CONVERSIONID,
           b.CONVERSIONWORKPACKAGE,
           b.LOB,
           b.OWNERUSERID,
           b.EXPIREDATE,
           b.COMMENTS,
           b.GLOBALID,
           b.FACILITYID,
           0 SDE_STATE_ID,
           b.SHAPE
      FROM EDGIS.DIMENSION  b,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D9734
             WHERE     SDE_STATE_ID = 0
                   AND SDE.version_util.in_current_lineage (DELETED_AT) > 0)
           d
     WHERE     b.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND d.SDE_STATE_ID IS NULL
           AND SDE.version_util.get_lineage_list > 0
    UNION ALL
    SELECT a.OBJECTID,
           a.DIMLENGTH,
           a.BEGINX,
           a.BEGINY,
           a.ENDX,
           a.ENDY,
           a.DIMX,
           a.DIMY,
           a.TEXTX,
           a.TEXTY,
           a.DIMTYPE,
           a.EXTANGLE,
           a.STYLEID,
           a.USECUSTOMLENGTH,
           a.CUSTOMLENGTH,
           a.DIMDISPLAY,
           a.EXTDISPLAY,
           a.MARKERDISPLAY,
           a.TEXTANGLE,
           a.CREATIONUSER,
           a.DATECREATED,
           a.DATEMODIFIED,
           a.LASTUSER,
           a.CONVERSIONID,
           a.CONVERSIONWORKPACKAGE,
           a.LOB,
           a.OWNERUSERID,
           a.EXPIREDATE,
           a.COMMENTS,
           a.GLOBALID,
           a.FACILITYID,
           a.SDE_STATE_ID,
           a.SHAPE
      FROM EDGIS.A9734  a,
           (SELECT SDE_DELETES_ROW_ID, SDE_STATE_ID
              FROM EDGIS.D9734
             WHERE SDE.version_util.in_current_lineage (DELETED_AT) > 0) d
     WHERE     a.OBJECTID = d.SDE_DELETES_ROW_ID(+)
           AND a.SDE_STATE_ID = d.SDE_STATE_ID(+)
           AND SDE.version_util.in_current_lineage (a.SDE_STATE_ID) > 0
           AND d.SDE_STATE_ID IS NULL
/


Prompt Trigger V9734_DELETE;
--
-- V9734_DELETE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9734_DELETE INSTEAD OF DELETE ON EDGIS.ZZ_MV_DIMENSION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A9734 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d9734 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d9734 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else DELETE FROM EDGIS.DIMENSION WHERE OBJECTID = :old.OBJECTID; END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); Else IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE DELETE FROM EDGIS.A9734 WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF; END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9734,current_state); END IF;END;
/


Prompt Trigger V9734_INSERT;
--
-- V9734_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9734_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DIMENSION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',9734); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A9734 VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DIMENSION VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,:new.SHAPE);  ELSE INSERT INTO EDGIS.A9734  VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9734,current_state);  END IF;END;
/


Prompt Trigger V9734_UPDATE;
--
-- V9734_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9734_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_DIMENSION REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A9734 VALUES (:old.OBJECTID,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE); INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A9734 SET DIMLENGTH = :new.DIMLENGTH,BEGINX = :new.BEGINX,BEGINY = :new.BEGINY,ENDX = :new.ENDX,ENDY = :new.ENDY,DIMX = :new.DIMX,DIMY = :new.DIMY,TEXTX = :new.TEXTX,TEXTY = :new.TEXTY,DIMTYPE = :new.DIMTYPE,EXTANGLE = :new.EXTANGLE,STYLEID = :new.STYLEID,USECUSTOMLENGTH = :new.USECUSTOMLENGTH,CUSTOMLENGTH = :new.CUSTOMLENGTH,DIMDISPLAY = :new.DIMDISPLAY,EXTDISPLAY = :new.EXTDISPLAY,MARKERDISPLAY = :new.MARKERDISPLAY,TEXTANGLE = :new.TEXTANGLE,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,LOB = :new.LOB,OWNERUSERID = :new.OWNERUSERID,EXPIREDATE = :new.EXPIREDATE,COMMENTS = :new.COMMENTS,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d9734 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d9734 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A9734 VALUES (:old.OBJECTID,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE); INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.DIMENSION SET DIMLENGTH = :new.DIMLENGTH,BEGINX = :new.BEGINX,BEGINY = :new.BEGINY,ENDX = :new.ENDX,ENDY = :new.ENDY,DIMX = :new.DIMX,DIMY = :new.DIMY,TEXTX = :new.TEXTX,TEXTY = :new.TEXTY,DIMTYPE = :new.DIMTYPE,EXTANGLE = :new.EXTANGLE,STYLEID = :new.STYLEID,USECUSTOMLENGTH = :new.USECUSTOMLENGTH,CUSTOMLENGTH = :new.CUSTOMLENGTH,DIMDISPLAY = :new.DIMDISPLAY,EXTDISPLAY = :new.EXTDISPLAY,MARKERDISPLAY = :new.MARKERDISPLAY,TEXTANGLE = :new.TEXTANGLE,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,LOB = :new.LOB,OWNERUSERID = :new.OWNERUSERID,EXPIREDATE = :new.EXPIREDATE,COMMENTS = :new.COMMENTS,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A9734 VALUES (:old.OBJECTID,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE); INSERT INTO EDGIS.D9734 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A9734 SET DIMLENGTH = :new.DIMLENGTH,BEGINX = :new.BEGINX,BEGINY = :new.BEGINY,ENDX = :new.ENDX,ENDY = :new.ENDY,DIMX = :new.DIMX,DIMY = :new.DIMY,TEXTX = :new.TEXTX,TEXTY = :new.TEXTY,DIMTYPE = :new.DIMTYPE,EXTANGLE = :new.EXTANGLE,STYLEID = :new.STYLEID,USECUSTOMLENGTH = :new.USECUSTOMLENGTH,CUSTOMLENGTH = :new.CUSTOMLENGTH,DIMDISPLAY = :new.DIMDISPLAY,EXTDISPLAY = :new.EXTDISPLAY,MARKERDISPLAY = :new.MARKERDISPLAY,TEXTANGLE = :new.TEXTANGLE,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,LOB = :new.LOB,OWNERUSERID = :new.OWNERUSERID,EXPIREDATE = :new.EXPIREDATE,COMMENTS = :new.COMMENTS,GLOBALID = :new.GLOBALID,FACILITYID = :new.FACILITYID,SHAPE = :new.SHAPE WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9734,current_state);  END IF; END;
/


Prompt Grants on VIEW ZZ_MV_DIMENSION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZZ_MV_DIMENSION TO DAT_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO EDGISBO to EDGISBO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DIMENSION TO EDGISBO
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZZ_MV_DIMENSION TO GISINTERFACE
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DIMENSION TO GIS_I
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO MM_ADMIN to MM_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DIMENSION TO MM_ADMIN
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DIMENSION TO SDE
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZZ_MV_DIMENSION TO SDE_EDITOR
/

Prompt Grants on VIEW ZZ_MV_DIMENSION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZZ_MV_DIMENSION TO SDE_VIEWER
/
