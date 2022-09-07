Prompt drop Trigger SP_CCB_METER_LOAD_TRG;
DROP TRIGGER EDTLM.SP_CCB_METER_LOAD_TRG
/

Prompt Trigger SP_CCB_METER_LOAD_TRG;
--
-- SP_CCB_METER_LOAD_TRG  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.SP_CCB_METER_LOAD_TRG
BEFORE INSERT ON EDTLM.SP_CCB_METER_LOAD
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SP_CCB_METER_LOAD_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/