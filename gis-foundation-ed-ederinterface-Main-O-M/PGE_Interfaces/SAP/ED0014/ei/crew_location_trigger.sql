  CREATE OR REPLACE TRIGGER "WEBR"."PGE_CREWLOCATION_INSERT" 
BEFORE INSERT ON PGE_CREWLOCATION
FOR EACH ROW 
  DECLARE OBJECTID PGE_CREWLOCATION.objectid%TYPE;
  
BEGIN
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',select registration_id from sde.table_registry where table_name='PGE_CREWLOCATION');
  :new.objectid := objectid;
  if :new.longitude is not null and :new.latitude is not null then 
	:new.shape := SDE.ST_POINT(:new.longitude,:new.latitude,4326);
  end if;
END;
/
ALTER TRIGGER "WEBR"."PGE_CREWLOCATION_INSERT" ENABLE;
