Prompt drop Procedure TRUNCATETAXREPORTTABLES;
DROP PROCEDURE EDGISBO.TRUNCATETAXREPORTTABLES
/

Prompt Procedure TRUNCATETAXREPORTTABLES;
--
-- TRUNCATETAXREPORTTABLES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGISBO.TruncateTaxReportTables(tablename IN VARCHAR2)
AS
BEGIN
IF (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE') OR (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE_STAGING') THEN
   execute immediate 'TRUNCATE TABLE ' || tablename;
END IF;
END;
/


Prompt Grants on PROCEDURE TRUNCATETAXREPORTTABLES TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGISBO.TRUNCATETAXREPORTTABLES TO GIS_I
/
