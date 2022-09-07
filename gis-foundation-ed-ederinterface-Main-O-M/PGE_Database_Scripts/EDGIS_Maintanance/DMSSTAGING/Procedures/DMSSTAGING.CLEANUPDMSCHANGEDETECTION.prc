Prompt drop Procedure CLEANUPDMSCHANGEDETECTION;
DROP PROCEDURE DMSSTAGING.CLEANUPDMSCHANGEDETECTION
/

Prompt Procedure CLEANUPDMSCHANGEDETECTION;
--
-- CLEANUPDMSCHANGEDETECTION  (Procedure) 
--
CREATE OR REPLACE PROCEDURE DMSSTAGING.CLEANUPDMSCHANGEDETECTION AS
BEGIN
   delete from edgis.pge_changed_circuit where circuitid in (select circuitids from dmsstaging.pge_dms_to_process where circuitstatus = 1);
   delete from edgis.pge_changed_substation where substationid in (select circuitids from dmsstaging.pge_dms_to_process where circuitstatus = 1);
   commit;
END CLEANUPDMSCHANGEDETECTION ;
/


Prompt Grants on PROCEDURE CLEANUPDMSCHANGEDETECTION TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON DMSSTAGING.CLEANUPDMSCHANGEDETECTION TO GISINTERFACE
/
