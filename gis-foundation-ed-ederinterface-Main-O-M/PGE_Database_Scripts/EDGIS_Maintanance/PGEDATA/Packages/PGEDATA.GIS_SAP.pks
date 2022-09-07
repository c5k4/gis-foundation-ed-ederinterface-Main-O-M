Prompt drop Package GIS_SAP;
DROP PACKAGE PGEDATA.GIS_SAP
/

Prompt Package GIS_SAP;
--
-- GIS_SAP  (Package) 
--
CREATE OR REPLACE PACKAGE PGEDATA.GIS_SAP AS

  /* TODO enter package declarations (types, exceptions, methods etc) here */
    PROCEDURE MERGE_NOTIFICATIONHEADERS;

END GIS_SAP;

/


Prompt Grants on PACKAGE GIS_SAP TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO EDGIS
/

Prompt Grants on PACKAGE GIS_SAP TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO GIS_I
/

Prompt Grants on PACKAGE GIS_SAP TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO GIS_I_WRITE
/

Prompt Grants on PACKAGE GIS_SAP TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO IGPCITEDITOR
/

Prompt Grants on PACKAGE GIS_SAP TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO IGPEDITOR
/

Prompt Grants on PACKAGE GIS_SAP TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.GIS_SAP TO SDE
/
