--------------------------------------------------------
--  DDL for Function LOG_PROCESS_STATUS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "PGEDATA"."LOG_PROCESS_STATUS" (
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
