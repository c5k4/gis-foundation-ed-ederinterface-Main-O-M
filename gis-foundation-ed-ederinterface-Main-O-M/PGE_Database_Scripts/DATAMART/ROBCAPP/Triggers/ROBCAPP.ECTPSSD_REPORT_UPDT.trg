Prompt drop Trigger ECTPSSD_REPORT_UPDT;
DROP TRIGGER ROBCAPP.ECTPSSD_REPORT_UPDT
/

Prompt Trigger ECTPSSD_REPORT_UPDT;
--
-- ECTPSSD_REPORT_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER ROBCAPP.ECTPSSD_REPORT_UPDT BEFORE UPDATE
    ON ROBCAPP.ECTPSSD_REPORT
FOR EACH ROW
DECLARE
        CLID VARCHAR2(30);
    BEGIN
        SELECT SYSDATE INTO :NEW.UPDATE_DTM FROM DUAL;
        SELECT SYS_CONTEXT('userenv','client_identifier') INTO CLID FROM DUAL;
        IF CLID IS NULL THEN
            SELECT USER INTO :NEW.UPDATE_USERID FROM DUAL;
        ELSE
            SELECT CLID INTO :NEW.UPDATE_USERID FROM DUAL;
        END IF;

END;
/
