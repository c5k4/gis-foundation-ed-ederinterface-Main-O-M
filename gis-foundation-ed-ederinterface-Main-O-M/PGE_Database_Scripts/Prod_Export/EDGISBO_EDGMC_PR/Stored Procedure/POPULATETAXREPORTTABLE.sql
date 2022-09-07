--------------------------------------------------------
--  DDL for Procedure POPULATETAXREPORTTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."POPULATETAXREPORTTABLE" 
IS
TYPE WorkStructureTableType IS TABLE OF workorderstructure_staging%ROWTYPE;
workstructureTable$ WorkStructureTableType;
BEGIN
   SELECT * BULK COLLECT INTO workstructureTable$
     FROM workorderstructure_staging;
     FORALL x in workstructureTable$.First..workstructureTable$.Last
      INSERT INTO workorderstructure VALUES workstructureTable$(x);
     commit;
END;
