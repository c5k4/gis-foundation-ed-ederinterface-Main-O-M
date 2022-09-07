Prompt drop Trigger ECTPSSD_REPORT_INS;
DROP TRIGGER ROBCAPP.ECTPSSD_REPORT_INS
/

Prompt Trigger ECTPSSD_REPORT_INS;
--
-- ECTPSSD_REPORT_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER ROBCAPP.ECTPSSD_REPORT_INS
BEFORE INSERT ON ROBCAPP.ECTPSSD_REPORT
FOR EACH ROW
BEGIN
    DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT ECTPSSD_REPORT_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
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
