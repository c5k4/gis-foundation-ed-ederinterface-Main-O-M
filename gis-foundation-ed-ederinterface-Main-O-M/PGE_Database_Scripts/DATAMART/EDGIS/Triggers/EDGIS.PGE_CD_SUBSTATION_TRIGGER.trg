Prompt drop Trigger PGE_CD_SUBSTATION_TRIGGER;
DROP TRIGGER EDGIS.PGE_CD_SUBSTATION_TRIGGER
/

Prompt Trigger PGE_CD_SUBSTATION_TRIGGER;
--
-- PGE_CD_SUBSTATION_TRIGGER  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.PGE_CD_SUBSTATION_TRIGGER
BEFORE DELETE
 ON EDGIS.PGE_CHANGED_SUBSTATION
 FOR EACH ROW
DECLARE

BEGIN
 -- Insert the row being deleted from PGE_CHANGED_CIRCUIT table into PGE_CHANGED_CIRCUIT_ARCHIVE table for permanent history
INSERT INTO EDGIS.PGE_CHANGED_SUBSTATION_ARCHIVE
(ID,
SUBSTATIONID,
USERID,
POSTDATE,
CHANGED_ACTION,
DELETED_ON
)
VALUES(
 :old.ID,
 :old.SUBSTATIONID,
 :old.USERID,
 :old.POSTDATE,
 :old.CHANGED_ACTION,
 CURRENT_DATE
 );
END;
/