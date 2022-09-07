Prompt drop Trigger REV_ACCT_INS;
DROP TRIGGER EDTLM.REV_ACCT_INS
/

Prompt Trigger REV_ACCT_INS;
--
-- REV_ACCT_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.REV_ACCT_INS BEFORE
  INSERT ON EDTLM.REV_ACCT FOR EACH ROW
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
