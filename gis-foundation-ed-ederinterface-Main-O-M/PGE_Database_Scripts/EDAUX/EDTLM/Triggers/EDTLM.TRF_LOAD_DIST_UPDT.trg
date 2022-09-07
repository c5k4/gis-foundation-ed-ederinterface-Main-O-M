Prompt drop Trigger TRF_LOAD_DIST_UPDT;
DROP TRIGGER EDTLM.TRF_LOAD_DIST_UPDT
/

Prompt Trigger TRF_LOAD_DIST_UPDT;
--
-- TRF_LOAD_DIST_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDTLM.TRF_LOAD_DIST_UPDT
    BEFORE UPDATE
    ON EDTLM.TRF_LOAD_DIST

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
