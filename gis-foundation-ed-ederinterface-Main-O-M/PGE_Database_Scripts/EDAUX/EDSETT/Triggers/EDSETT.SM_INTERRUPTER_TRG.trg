Prompt drop Trigger SM_INTERRUPTER_TRG;
DROP TRIGGER EDSETT.SM_INTERRUPTER_TRG
/

Prompt Trigger SM_INTERRUPTER_TRG;
--
-- SM_INTERRUPTER_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_INTERRUPTER_TRG
BEFORE INSERT ON EDSETT.SM_INTERRUPTER
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_INTERRUPTER_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/