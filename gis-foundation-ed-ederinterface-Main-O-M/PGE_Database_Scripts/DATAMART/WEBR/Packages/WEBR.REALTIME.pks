Prompt drop Package REALTIME;
DROP PACKAGE WEBR.REALTIME
/

Prompt Package REALTIME;
--
-- REALTIME  (Package) 
--
CREATE OR REPLACE PACKAGE WEBR.REALTIME AS

  /* TODO enter package declarations (types, exceptions, methods etc) here */
    PROCEDURE PROCESS_OUTAGES;

END REALTIME;

/


Prompt Grants on PACKAGE REALTIME TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON WEBR.REALTIME TO GIS_I
/
