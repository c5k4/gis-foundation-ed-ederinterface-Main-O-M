Prompt drop Trigger PGE_OUTAGEXFMR_INSERT;
DROP TRIGGER WEBR.PGE_OUTAGEXFMR_INSERT
/

Prompt Trigger PGE_OUTAGEXFMR_INSERT;
--
-- PGE_OUTAGEXFMR_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.PGE_OUTAGEXFMR_INSERT BEFORE
  INSERT ON WEBR.PGE_OUTAGEXFMR FOR EACH ROW
DECLARE OBJECTID PGE_OUTAGEXFMR.objectid%TYPE;
  regid number;
BEGIN
  select registration_id into regid from sde.table_registry where table_name='PGE_OUTAGEXFMR';
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',regid);
  :new.objectid := objectid;
END;
/
