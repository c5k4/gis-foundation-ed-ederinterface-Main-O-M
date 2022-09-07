Prompt drop Trigger SM_SPECIAL_LOAD_INS;
DROP TRIGGER EDSETT.SM_SPECIAL_LOAD_INS
/

Prompt Trigger SM_SPECIAL_LOAD_INS;
--
-- SM_SPECIAL_LOAD_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.SM_SPECIAL_LOAD_INS
BEFORE INSERT ON EDSETT.SM_SPECIAL_LOAD
FOR EACH ROW
BEGIN
  <<COLUMN_SEQUENCES>>
  DECLARE clid VARCHAR2(30);
  BEGIN
    IF INSERTING AND :NEW.ID IS NULL THEN
      SELECT SM_SPECIAL_LOAD_SEQ.NEXTVAL INTO :NEW.ID FROM SYS.DUAL;
    END IF;
    SELECT sysdate INTO :new.create_dtm FROM dual;
    SELECT sysdate INTO :new.update_dtm FROM dual;
    IF :NEW.CREATE_USERID IS NULL THEN
        SELECT sys_context('userenv','client_identifier') INTO clid FROM dual;
        IF clid IS NULL THEN
          SELECT USER INTO :new.create_userid FROM dual;
        ELSE
          SELECT clid INTO :new.create_userid FROM dual;
        END IF;
    END IF;
    :new.update_userid := :new.create_userid;

  END COLUMN_SEQUENCES;
END;
/
