Prompt drop Function LOG_MIGRATION_ERROR;
DROP FUNCTION ROBCAPP.LOG_MIGRATION_ERROR
/

Prompt Function LOG_MIGRATION_ERROR;
--
-- LOG_MIGRATION_ERROR  (Function) 
--
CREATE OR REPLACE FUNCTION ROBCAPP."LOG_MIGRATION_ERROR" (wave_param VARCHAR2, table_name_param VARCHAR2, error_text_param VARCHAR2) RETURN VARCHAR2 AS
BEGIN
  update MIGRATION_LOG set ERROR_TEXT = error_text_param
  where
     WAVE = wave_param and
     TABLE_NAME = table_name_param;
  commit;
  return 'SUCCESS';
END LOG_MIGRATION_ERROR;

/
