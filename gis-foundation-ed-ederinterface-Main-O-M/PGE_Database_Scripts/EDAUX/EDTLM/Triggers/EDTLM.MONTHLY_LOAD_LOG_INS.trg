Prompt drop Trigger MONTHLY_LOAD_LOG_INS;
DROP TRIGGER EDTLM.MONTHLY_LOAD_LOG_INS
/

Prompt Trigger MONTHLY_LOAD_LOG_INS;
--
-- MONTHLY_LOAD_LOG_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.MONTHLY_LOAD_LOG_INS
BEFORE INSERT ON EDTLM.MONTHLY_LOAD_LOG
FOR EACH ROW
BEGIN
    DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT MONTHLY_LOAD_LOG_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;

    SELECT sysdate INTO :new.load_start_ts FROM dual;

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
