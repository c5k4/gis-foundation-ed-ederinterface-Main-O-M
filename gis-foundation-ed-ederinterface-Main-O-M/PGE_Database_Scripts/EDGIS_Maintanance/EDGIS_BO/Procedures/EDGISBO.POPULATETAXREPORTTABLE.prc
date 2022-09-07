Prompt drop Procedure POPULATETAXREPORTTABLE;
DROP PROCEDURE EDGISBO.POPULATETAXREPORTTABLE
/

Prompt Procedure POPULATETAXREPORTTABLE;
--
-- POPULATETAXREPORTTABLE  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGISBO.PopulateTaxReportTable
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
/


Prompt Grants on PROCEDURE POPULATETAXREPORTTABLE TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGISBO.POPULATETAXREPORTTABLE TO GIS_I
/
