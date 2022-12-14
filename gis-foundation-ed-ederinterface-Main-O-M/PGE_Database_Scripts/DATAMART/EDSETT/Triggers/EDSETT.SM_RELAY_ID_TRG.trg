Prompt drop Trigger SM_RELAY_ID_TRG;
DROP TRIGGER EDSETT.SM_RELAY_ID_TRG
/

Prompt Trigger SM_RELAY_ID_TRG;
--
-- SM_RELAY_ID_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_RELAY_ID_TRG
BEFORE INSERT ON EDSETT.SM_RELAY
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_RELAY_ID_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
