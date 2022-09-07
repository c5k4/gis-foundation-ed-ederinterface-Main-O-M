Prompt drop Trigger TRANSFORMER_UPDT;
DROP TRIGGER EDTLM.TRANSFORMER_UPDT
/

Prompt Trigger TRANSFORMER_UPDT;
--
-- TRANSFORMER_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.TRANSFORMER_UPDT
    BEFORE UPDATE
    ON EDTLM.TRANSFORMER

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