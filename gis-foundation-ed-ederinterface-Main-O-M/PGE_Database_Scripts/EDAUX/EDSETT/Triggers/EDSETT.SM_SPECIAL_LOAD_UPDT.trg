Prompt drop Trigger SM_SPECIAL_LOAD_UPDT;
DROP TRIGGER EDSETT.SM_SPECIAL_LOAD_UPDT
/

Prompt Trigger SM_SPECIAL_LOAD_UPDT;
--
-- SM_SPECIAL_LOAD_UPDT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_SPECIAL_LOAD_UPDT
    BEFORE UPDATE
    ON EDSETT.SM_SPECIAL_LOAD

    FOR EACH ROW
DECLARE
        CLID VARCHAR2(30);
    BEGIN
        SELECT SYSDATE INTO :NEW.UPDATE_DTM FROM DUAL;
        IF :NEW.UPDATE_USERID IS NULL THEN
            SELECT SYS_CONTEXT('userenv','client_identifier') INTO CLID FROM DUAL;
            IF CLID IS NULL THEN
                SELECT USER INTO :NEW.UPDATE_USERID FROM DUAL;
            ELSE
                SELECT CLID INTO :NEW.UPDATE_USERID FROM DUAL;
            END IF;
        END IF;
    END;
/
