Prompt drop Trigger TRANSFORMER_BANK_INS;
DROP TRIGGER EDTLM.TRANSFORMER_BANK_INS
/

Prompt Trigger TRANSFORMER_BANK_INS;
--
-- TRANSFORMER_BANK_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.TRANSFORMER_BANK_INS BEFORE
  INSERT ON EDTLM.TRANSFORMER_BANK FOR EACH ROW
BEGIN <<COLUMN_SEQUENCES>>
    DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT EDTLM.TRANSFORMER_BANK_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
    SELECT sysdate INTO :new.create_dtm FROM dual;
    SELECT sysdate INTO :new.update_dtm FROM dual;
    SELECT sys_context('userenv','client_identifier') INTO clid FROM dual;
    IF clid IS NULL THEN
      SELECT USER INTO :new.update_userid FROM dual;
      SELECT USER INTO :new.create_userid FROM dual;
    ELSE
      SELECT clid INTO :new.update_userid FROM dual;
      SELECT clid INTO :new.create_userid FROM dual;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
