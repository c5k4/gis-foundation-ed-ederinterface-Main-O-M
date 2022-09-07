Prompt drop Function LOG_MIGRATION_SUCCESS;
DROP FUNCTION ROBCAPP.LOG_MIGRATION_SUCCESS
/

Prompt Function LOG_MIGRATION_SUCCESS;
--
-- LOG_MIGRATION_SUCCESS  (Function) 
--
CREATE OR REPLACE FUNCTION ROBCAPP."LOG_MIGRATION_SUCCESS" (
    wave_param                 VARCHAR2,
    table_name_param           VARCHAR2,
    num_records_migrated_param NUMBER)
    RETURN VARCHAR2
AS
BEGIN
  DBMS_OUTPUT.PUT_LINE('DEBUG - Wave: '||wave_param||' Table Name: '||table_name_param||' Records updated: '||num_records_migrated_param);
  UPDATE MIGRATION_LOG
     SET NUM_RECORDS_MIGRATED = num_records_migrated_param,
         MIG_END_TS           = sysdate
   WHERE WAVE           = wave_param
     AND TABLE_NAME           = table_name_param;
  COMMIT;
  RETURN 'SUCCESS';

END LOG_MIGRATION_SUCCESS;
/
