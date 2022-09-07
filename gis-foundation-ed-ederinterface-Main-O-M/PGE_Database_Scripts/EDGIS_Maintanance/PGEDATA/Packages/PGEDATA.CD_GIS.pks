Prompt drop Package CD_GIS;
DROP PACKAGE PGEDATA.CD_GIS
/

Prompt Package CD_GIS;
--
-- CD_GIS  (Package) 
--
CREATE OR REPLACE PACKAGE PGEDATA.CD_GIS AS
	FUNCTION IS_FIELD_CD(t_name IN NVARCHAR2,field_name IN NVARCHAR2,cd_type NVARCHAR2) RETURN BOOLEAN;
    PROCEDURE SET_FIELD_CD(t_name IN NVARCHAR2, GLOBALID IN NVARCHAR2, cd_type NVARCHAR2) ;
    PROCEDURE UNSET_FIELD_CD(t_name IN NVARCHAR2, GLOBALID IN NVARCHAR2, cd_type NVARCHAR2) ;
END CD_GIS;
/


Prompt Grants on PACKAGE CD_GIS TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO EDGIS
/

Prompt Grants on PACKAGE CD_GIS TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO GIS_I
/

Prompt Grants on PACKAGE CD_GIS TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO GIS_I_WRITE
/

Prompt Grants on PACKAGE CD_GIS TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO IGPCITEDITOR
/

Prompt Grants on PACKAGE CD_GIS TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO IGPEDITOR
/

Prompt Grants on PACKAGE CD_GIS TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.CD_GIS TO SDE
/