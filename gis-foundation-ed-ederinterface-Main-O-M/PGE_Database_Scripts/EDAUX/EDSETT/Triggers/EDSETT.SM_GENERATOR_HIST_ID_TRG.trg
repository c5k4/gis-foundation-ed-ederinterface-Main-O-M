Prompt drop Trigger SM_GENERATOR_HIST_ID_TRG;
DROP TRIGGER EDSETT.SM_GENERATOR_HIST_ID_TRG
/

Prompt Trigger SM_GENERATOR_HIST_ID_TRG;
--
-- SM_GENERATOR_HIST_ID_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_GENERATOR_HIST_ID_TRG BEFORE INSERT ON EDSETT.SM_GENERATOR_HIST
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_GENERATOR_HIST_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
