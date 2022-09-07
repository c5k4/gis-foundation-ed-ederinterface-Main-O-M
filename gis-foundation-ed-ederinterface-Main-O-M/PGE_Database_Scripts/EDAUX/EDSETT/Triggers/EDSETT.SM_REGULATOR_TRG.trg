Prompt drop Trigger SM_REGULATOR_TRG;
DROP TRIGGER EDSETT.SM_REGULATOR_TRG
/

Prompt Trigger SM_REGULATOR_TRG;
--
-- SM_REGULATOR_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_REGULATOR_TRG
BEFORE INSERT ON EDSETT.SM_REGULATOR
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_REGULATOR_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/