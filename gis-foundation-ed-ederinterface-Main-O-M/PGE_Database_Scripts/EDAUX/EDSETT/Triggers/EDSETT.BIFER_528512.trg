Prompt drop Trigger BIFER_528512;
DROP TRIGGER EDSETT.BIFER_528512
/

Prompt Trigger BIFER_528512;
--
-- BIFER_528512  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.BIFER_528512 BEFORE INSERT ON EDSETT.PGEDATA_SM_RELAY_STAGE FOR EACH ROW
DECLARE BEGIN IF :NEW.OBJECTID IS NULL THEN :NEW.OBJECTID := EDSETT.SEQ_528512.NEXTVAL; END IF; EXCEPTION WHEN OTHERS THEN RAISE; END;
/
