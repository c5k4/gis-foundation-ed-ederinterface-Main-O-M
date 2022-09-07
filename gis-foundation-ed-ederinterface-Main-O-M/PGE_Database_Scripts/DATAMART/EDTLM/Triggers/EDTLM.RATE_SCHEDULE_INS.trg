Prompt drop Trigger RATE_SCHEDULE_INS;
DROP TRIGGER EDTLM.RATE_SCHEDULE_INS
/

Prompt Trigger RATE_SCHEDULE_INS;
--
-- RATE_SCHEDULE_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.RATE_SCHEDULE_INS BEFORE
  INSERT ON EDTLM.RATE_SCHEDULE FOR EACH ROW
BEGIN <<COLUMN_SEQUENCES>>
    DECLARE clid VARCHAR2(30);
  BEGIN
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
