Prompt drop Trigger TRF_NP_CAP_MULT_INS;
DROP TRIGGER EDTLM.TRF_NP_CAP_MULT_INS
/

Prompt Trigger TRF_NP_CAP_MULT_INS;
--
-- TRF_NP_CAP_MULT_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.TRF_NP_CAP_MULT_INS BEFORE
  INSERT ON EDTLM.TRF_NP_CAP_MULT FOR EACH ROW
BEGIN <<COLUMN_SEQUENCES>>
    DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT EDTLM.TRF_NP_CAP_MULT_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
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
