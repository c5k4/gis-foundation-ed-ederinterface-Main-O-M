Prompt drop Procedure CLEANUPDMS;
DROP PROCEDURE DMSSTAGING.CLEANUPDMS
/

Prompt Procedure CLEANUPDMS;
--
-- CLEANUPDMS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE DMSSTAGING.CLEANUPDMS AS
BEGIN
   execute immediate 'truncate table dmsstaging.line';
   execute immediate 'truncate table dmsstaging.path';
   execute immediate 'truncate table dmsstaging.device';
   execute immediate 'truncate table dmsstaging.site';
   execute immediate 'truncate table dmsstaging.capacitor';
   execute immediate 'truncate table dmsstaging.load';
   execute immediate 'truncate table dmsstaging.source';
   execute immediate 'truncate table dmsstaging.node';
   execute immediate 'truncate table dmsstaging.exported';
   execute immediate 'truncate table dmsstaging.PGE_DMS_PROCESSES_RUNNING';
   execute immediate 'truncate table dmsstaging.PGE_DMS_TO_PROCESS';
   execute immediate 'truncate table dmsstaging.PGE_DMS_FEATURE_ERRORS';
   execute immediate 'truncate table dmsstaging.PGE_DMS_SCHEMATICS_ERRORS';
   execute immediate 'truncate table dmsstaging.PGE_DMS_FEATURE_WARNINGS';
END CLEANUPDMS;
/


Prompt Grants on PROCEDURE CLEANUPDMS TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON DMSSTAGING.CLEANUPDMS TO GISINTERFACE
/

Prompt Grants on PROCEDURE CLEANUPDMS TO GIS_INTERFACE to GIS_INTERFACE;
GRANT EXECUTE ON DMSSTAGING.CLEANUPDMS TO GIS_INTERFACE
/
