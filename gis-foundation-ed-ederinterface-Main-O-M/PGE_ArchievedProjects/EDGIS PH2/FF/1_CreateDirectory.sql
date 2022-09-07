set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on

-- Create Directory variable
--- create or replace directory UC4_EXPORT_DIR as  '/u02/uc4/&1/export' ;
create or replace directory UC4_EXPORT_DIR as  '&1' ;
GRANT READ, WRITE ON DIRECTORY UC4_EXPORT_DIR TO public;

set wrap off
set echo off term off
set serveroutput off
set time off
set timing off

exit;
