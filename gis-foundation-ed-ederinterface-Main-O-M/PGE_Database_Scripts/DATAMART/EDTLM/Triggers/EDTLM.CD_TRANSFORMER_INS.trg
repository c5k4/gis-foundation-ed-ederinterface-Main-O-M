Prompt drop Trigger CD_TRANSFORMER_INS;
DROP TRIGGER EDTLM.CD_TRANSFORMER_INS
/

Prompt Trigger CD_TRANSFORMER_INS;
--
-- CD_TRANSFORMER_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.CD_TRANSFORMER_INS BEFORE INSERT ON EDTLM.CD_TRANSFORMER
FOR EACH ROW
BEGIN
  DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING THEN
      SELECT CD_TRANSFORMER_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
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
