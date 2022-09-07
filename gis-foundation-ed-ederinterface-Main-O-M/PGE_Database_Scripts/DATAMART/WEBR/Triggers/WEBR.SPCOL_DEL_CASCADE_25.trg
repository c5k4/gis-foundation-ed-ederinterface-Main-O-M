Prompt drop Trigger SPCOL_DEL_CASCADE_25;
DROP TRIGGER WEBR.SPCOL_DEL_CASCADE_25
/

Prompt Trigger SPCOL_DEL_CASCADE_25;
--
-- SPCOL_DEL_CASCADE_25  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.SPCOL_DEL_CASCADE_25
           AFTER DELETE OR UPDATE OF SHAPE ON WEBR.PGE_BK_BOOKMARK
           FOR EACH ROW
DECLARE
             -- ArcSDE 9.2 --
             inv_spatial_col1 EXCEPTION;
           BEGIN
           IF DELETING THEN
           DELETE FROM WEBR.F25 WHERE WEBR.F25.fid = :old.SHAPE;
    DELETE FROM WEBR.S25 WHERE WEBR.S25.sp_fid = :old.SHAPE
;
           END IF;
          IF UPDATING AND (:new.SHAPE IS NULL AND :old.SHAPE IS NOT NULL) THEN
          DELETE FROM WEBR.F25 WHERE WEBR.F25.fid = :old.SHAPE;
    DELETE FROM WEBR.S25 WHERE WEBR.S25.sp_fid = :old.SHAPE
;
          END IF;

           IF UPDATING AND (:new.SHAPE != :old.SHAPE AND :old.SHAPE IS NOT NULL) THEN
             RAISE inv_spatial_col1;
           END IF;

          EXCEPTION
            WHEN inv_spatial_col1 THEN
            raise_application_error (-20013,'Invalid SDE spatial column UPDATE. Cannot update spatial column value '||TO_CHAR(:new.SHAPE)||' to non-NULL value.');END;
/
