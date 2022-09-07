Prompt drop Trigger SP_PEAK_HIST_UPDT;
DROP TRIGGER EDTLM.SP_PEAK_HIST_UPDT
/

Prompt Trigger SP_PEAK_HIST_UPDT;
--
-- SP_PEAK_HIST_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.SP_PEAK_HIST_UPDT
    BEFORE UPDATE
    ON EDTLM.SP_PEAK_HIST

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
