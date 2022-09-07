--------------------------------------------------------
--  DDL for Procedure SDE30116_TEST
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "SDE"."SDE30116_TEST" AS /* Test EXECUTE Access to DBMS_LOCK.maxwait */ pvalue  INTEGER; BEGIN /* ArcSDE plsql */ pvalue := DBMS_LOCK.maxwait; END;
