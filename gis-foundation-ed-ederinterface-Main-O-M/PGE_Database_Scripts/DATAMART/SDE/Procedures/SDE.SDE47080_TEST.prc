Prompt drop Procedure SDE47080_TEST;
DROP PROCEDURE SDE.SDE47080_TEST
/

Prompt Procedure SDE47080_TEST;
--
-- SDE47080_TEST  (Procedure) 
--
CREATE OR REPLACE PROCEDURE SDE.SDE47080_TEST AS /* Test EXECUTE Access to DBMS_UTILITY.current_instance */ pvalue  INTEGER; BEGIN /* ArcSDE plsql */ pvalue := DBMS_UTILITY.current_instance; END;
/
