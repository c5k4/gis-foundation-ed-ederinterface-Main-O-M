Prompt drop Trigger BIFER_528500;
DROP TRIGGER EDSETT.BIFER_528500
/

Prompt Trigger BIFER_528500;
--
-- BIFER_528500  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.BIFER_528500 BEFORE INSERT ON EDSETT.PGEDATA_SM_GENERATION_STAGE FOR EACH ROW
DECLARE BEGIN IF :NEW.OBJECTID IS NULL THEN :NEW.OBJECTID := EDSETT.SEQ_528500.NEXTVAL; END IF; EXCEPTION WHEN OTHERS THEN RAISE; END;
/
