Prompt drop Trigger PGE_OUTAGENOTIFICATION_INSERT;
DROP TRIGGER WEBR.PGE_OUTAGENOTIFICATION_INSERT
/

Prompt Trigger PGE_OUTAGENOTIFICATION_INSERT;
--
-- PGE_OUTAGENOTIFICATION_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.PGE_OUTAGENOTIFICATION_INSERT
BEFORE INSERT ON WEBR.PGE_OUTAGENOTIFICATION
FOR EACH ROW
DECLARE OBJECTID PGE_OUTAGENOTIFICATION.objectid%TYPE;
  regid number;
BEGIN
  select registration_id into regid from sde.table_registry where table_name='PGE_OUTAGENOTIFICATION';
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',regid);
  :new.objectid := objectid;
  if :new.latitude is not null and :new.longitude is not null then
    :new.shape := SDE.ST_POINT(:new.longitude,:new.latitude,4326);
  end if;
  if :new.etor is not null then
    :new.etor_txt := to_char(:new.etor, 'MM-DD-YYYY HH:MM:SS AM');
  end if;
  if :new.outage_start_time is not null then
    :new.outage_start_time_txt := to_char(:new.outage_start_time, 'MM-DD-YYYY HH:MM:SS AM');
  end if;
END;
/
