Prompt drop Trigger PGE_OUTAGEPOLY_INSERT;
DROP TRIGGER WEBR.PGE_OUTAGEPOLY_INSERT
/

Prompt Trigger PGE_OUTAGEPOLY_INSERT;
--
-- PGE_OUTAGEPOLY_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.PGE_OUTAGEPOLY_INSERT
BEFORE INSERT ON WEBR.PGE_OUTAGEPOLY
FOR EACH ROW
DECLARE OBJECTID PGE_OUTAGEPOLY.objectid%TYPE;
  regid number;
BEGIN
  select registration_id into regid from sde.table_registry where table_name='PGE_OUTAGEPOLY';
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',regid);
  :new.objectid := objectid;
END;
/
