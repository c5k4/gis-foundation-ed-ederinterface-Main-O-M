Prompt drop Procedure SDE44828_TEST;
DROP PROCEDURE SDE.SDE44828_TEST
/

Prompt Procedure SDE44828_TEST;
--
-- SDE44828_TEST  (Procedure) 
--
CREATE OR REPLACE PROCEDURE SDE.SDE44828_TEST AS /* Test EXECUTE Access to DBMS_PIPE.maxwait */ pvalue  INTEGER; BEGIN /* ArcSDE plsql */ pvalue := DBMS_PIPE.maxwait; END;
/
