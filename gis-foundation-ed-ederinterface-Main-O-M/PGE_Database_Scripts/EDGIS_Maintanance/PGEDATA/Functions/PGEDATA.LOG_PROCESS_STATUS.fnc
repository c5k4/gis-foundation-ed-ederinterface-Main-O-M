Prompt drop Function LOG_PROCESS_STATUS;
DROP FUNCTION PGEDATA.LOG_PROCESS_STATUS
/

Prompt Function LOG_PROCESS_STATUS;
--
-- LOG_PROCESS_STATUS  (Function) 
--
CREATE OR REPLACE FUNCTION PGEDATA."LOG_PROCESS_STATUS" (
  V_Action_Type           Char,
  V_Action_name           VARCHAR2,
  V_Error_text            Varchar2,
  V_num_records_migrated  NUMBER
)
  RETURN VARCHAR2
AS
BEGIN
----------------------------------------------------------------------------
-- Initial VERSION 1.0
----------------------------------------------------------------------------
  if V_Action_Type ='I' then
     Insert into PROCESS_LOG (ACTION_NAME) values (V_Action_Name);
  End if;
  if V_Action_Type ='U' then
      UPDATE PROCESS_LOG
      SET NUM_RECORDS_MIGRATED = v_num_records_migrated,
          RUN_END              = sysdate
      WHERE ACTION_NAME         = V_Action_Name;
  End if;
  if V_Action_Type ='E' then
      update PROCESS_LOG set ERROR_TEXT = V_error_text
       where ACTION_NAME = V_Action_Name ;
  End if;
  RETURN 'SUCCESS';
END LOG_PROCESS_STATUS;
/


Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO EDGIS
/

Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO GIS_I
/

Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO GIS_I_WRITE
/

Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO IGPCITEDITOR
/

Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO IGPEDITOR
/

Prompt Grants on FUNCTION LOG_PROCESS_STATUS TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.LOG_PROCESS_STATUS TO SDE
/
