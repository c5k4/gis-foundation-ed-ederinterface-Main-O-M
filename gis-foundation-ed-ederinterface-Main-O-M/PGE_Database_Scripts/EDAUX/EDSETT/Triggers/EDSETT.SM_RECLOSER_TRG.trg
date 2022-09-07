Prompt drop Trigger SM_RECLOSER_TRG;
DROP TRIGGER EDSETT.SM_RECLOSER_TRG
/

Prompt Trigger SM_RECLOSER_TRG;
--
-- SM_RECLOSER_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_RECLOSER_TRG
BEFORE INSERT ON EDSETT.SM_RECLOSER
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_RECLOSER_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
