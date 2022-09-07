Prompt drop Trigger PGE_OUTAGETRANSFORMER_INSERT;
DROP TRIGGER WEBR.PGE_OUTAGETRANSFORMER_INSERT
/

Prompt Trigger PGE_OUTAGETRANSFORMER_INSERT;
--
-- PGE_OUTAGETRANSFORMER_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER WEBR.PGE_OUTAGETRANSFORMER_INSERT
BEFORE INSERT ON WEBR.PGE_OUTAGETRANSFORMER
FOR EACH ROW
DECLARE OBJECTID PGE_OUTAGETRANSFORMER.objectid%TYPE;
  regid number;
BEGIN
  select registration_id into regid from sde.table_registry where table_name='PGE_OUTAGETRANSFORMER';
  objectid := SDE.VERSION_USER_DDL.NEXT_ROW_ID('WEBR',regid);
  :new.objectid := objectid;

  if :new.xfmr_guid is null then
    :new.xfmr_key := :new.xfmr_guid;
  elsif :new.xfmr_id is not null then
    :new.xfmr_key := :new.xfmr_id;
  end if;

  if :new.longitude is not null and :new.latitude is not null then
    :new.shape := sde.st_point(:new.longitude,:new.latitude,4326);
  end if;
END;
/
