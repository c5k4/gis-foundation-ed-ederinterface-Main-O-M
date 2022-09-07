Prompt drop Package TLM_CD_MGMT_MONTHLY_TEST;
DROP PACKAGE EDTLM.TLM_CD_MGMT_MONTHLY_TEST
/

Prompt Package TLM_CD_MGMT_MONTHLY_TEST;
--
-- TLM_CD_MGMT_MONTHLY_TEST  (Package) 
--
CREATE OR REPLACE PACKAGE EDTLM."TLM_CD_MGMT_MONTHLY_TEST"
AS
  /* TODO enter package declarations (types, exceptions, methods etc) here */
  PROCEDURE TRANSFORMER_MGMT(
      FromDate DATE,
      ToDate   DATE,
      ErrorMsg OUT VARCHAR2,
      ErrorCode OUT VARCHAR2 );
  PROCEDURE TRANSFORMER_BANK_MGMT(
      FromDate DATE,
      ToDate   DATE,
      ErrorMsg OUT VARCHAR2,
      ErrorCode OUT VARCHAR2 );
  PROCEDURE METER_MGMT(
      FromDate DATE,
      ToDate   DATE,
      ErrorMsg OUT VARCHAR2,
      ErrorCode OUT VARCHAR2 );
  PROCEDURE LOG_ERRORS(
      GLOBALID  VARCHAR2,
      ERRORCODE VARCHAR2,
      ERRORMSG  VARCHAR2,
      TRANSDATE TIMESTAMP,
      TRANSTYPE CHAR,
      APPTYPE   CHAR,
      PROCEDUREFOR VARCHAR2,ColumnId NUMBER );
  FUNCTION TLM_CD_CHECK_ISDUP (
      table_name varchar2,
      field_name varchar2,
      field_value varchar2)
      RETURN varchar2;
  FUNCTION TLM_CD_CHECK_ISNULL (
      val varchar2,
      rec_id number,
      global_id varchar2,
      trans_type varchar2,
      trans_date date,
      app_type varchar2,
      procedure_for varchar2,
      error_code varchar2)
      RETURN varchar2;
  FUNCTION TLM_CD_HAS_INSERT_RECORD (
      table_name varchar2,
      global_id varchar2,
      create_dt date)
      RETURN varchar2;
END TLM_CD_MGMT_MONTHLY_TEST;
/
