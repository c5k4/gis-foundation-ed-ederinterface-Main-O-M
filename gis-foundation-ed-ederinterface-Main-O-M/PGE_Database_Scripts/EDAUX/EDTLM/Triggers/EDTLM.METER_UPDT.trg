Prompt drop Trigger METER_UPDT;
DROP TRIGGER EDTLM.METER_UPDT
/

Prompt Trigger METER_UPDT;
--
-- METER_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.METER_UPDT
    BEFORE UPDATE
    ON EDTLM.METER

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
