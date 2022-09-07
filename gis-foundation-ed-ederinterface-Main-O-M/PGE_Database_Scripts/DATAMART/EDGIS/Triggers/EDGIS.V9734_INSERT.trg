Prompt drop Trigger V9734_INSERT;
DROP TRIGGER EDGIS.V9734_INSERT
/

Prompt Trigger V9734_INSERT;
--
-- V9734_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V9734_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_DIMENSION REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',9734); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A9734 VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.DIMENSION VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,:new.SHAPE);  ELSE INSERT INTO EDGIS.A9734  VALUES (next_rowid,:new.DIMLENGTH,:new.BEGINX,:new.BEGINY,:new.ENDX,:new.ENDY,:new.DIMX,:new.DIMY,:new.TEXTX,:new.TEXTY,:new.DIMTYPE,:new.EXTANGLE,:new.STYLEID,:new.USECUSTOMLENGTH,:new.CUSTOMLENGTH,:new.DIMDISPLAY,:new.EXTDISPLAY,:new.MARKERDISPLAY,:new.TEXTANGLE,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.LOB,:new.OWNERUSERID,:new.EXPIREDATE,:new.COMMENTS,:new.GLOBALID,:new.FACILITYID,current_state,:new.SHAPE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (9734,current_state);  END IF;END;
/