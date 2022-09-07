Prompt drop Trigger TRF_PEAK_UPDT;
DROP TRIGGER EDTLM.TRF_PEAK_UPDT
/

Prompt Trigger TRF_PEAK_UPDT;
--
-- TRF_PEAK_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.TRF_PEAK_UPDT
    BEFORE UPDATE
    ON EDTLM.TRF_PEAK

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
