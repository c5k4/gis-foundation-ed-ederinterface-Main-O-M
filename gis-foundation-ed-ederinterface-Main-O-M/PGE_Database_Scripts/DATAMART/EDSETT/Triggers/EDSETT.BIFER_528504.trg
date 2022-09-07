Prompt drop Trigger BIFER_528504;
DROP TRIGGER EDSETT.BIFER_528504
/

Prompt Trigger BIFER_528504;
--
-- BIFER_528504  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDSETT.BIFER_528504 BEFORE INSERT ON EDSETT.PGEDATA_SM_GENERATOR_STAGE FOR EACH ROW
DECLARE BEGIN IF :NEW.OBJECTID IS NULL THEN :NEW.OBJECTID := EDSETT.SEQ_528504.NEXTVAL; END IF; EXCEPTION WHEN OTHERS THEN RAISE; END;
/
