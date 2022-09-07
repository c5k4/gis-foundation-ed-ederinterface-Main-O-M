Prompt drop Trigger PGE_CREWLOCATION_INSERT;
DROP TRIGGER WEBR.PGE_CREWLOCATION_INSERT
/

Prompt Trigger PGE_CREWLOCATION_INSERT;
--
-- PGE_CREWLOCATION_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.PGE_CREWLOCATION_INSERT
BEFORE INSERT ON WEBR.PGE_CREWLOCATION
FOR EACH ROW
DECLARE OBJECTID PGE_CREWLOCATION.objectid%TYPE;
  regid number;
BEGIN
  select registration_id into regid from sde.table_registry where table_name='PGE_CREWLOCATION';
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',regid);
  :new.objectid := objectid;
  if :new.longitude is not null and :new.latitude is not null then
	:new.shape := SDE.ST_POINT(:new.longitude,:new.latitude,4326);
  end if;
  :new.last_updated_txt := to_char(:new.last_updated, 'MM-DD-YYYY HH:MM:SS AM');
END;
/
