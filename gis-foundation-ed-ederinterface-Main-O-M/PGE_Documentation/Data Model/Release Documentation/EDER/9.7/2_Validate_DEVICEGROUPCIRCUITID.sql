set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
set timing on
set linesize 160
set pagesize 50000
select sysdate from dual;
select count(*) from edgis.devicegroup where circuitid is null;
select sysdate from dual;
