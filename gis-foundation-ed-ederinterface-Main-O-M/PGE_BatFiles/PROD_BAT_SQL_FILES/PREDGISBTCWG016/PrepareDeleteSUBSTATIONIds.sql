
--column dcol new_value mydate noprint
--select to_char(sysdate,'YYYYMMDDHH24MMSS') dcol from dual;
SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0    
spool D:\edgisdbmaint\log\DELETEPROCESSEDSUBSTATION.sql 
select 'Delete  from EDGIS.PGE_CHANGED_SUBSTATION where SUBSTATIONID ='''|| CIRCUITIDS ||''';' from DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS = 1 and SUBSTATIONORCIRCUIT = 'S';
select 'COMMIT;' from dual;
select 'EXIT;' from dual;

spool off;

--@D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS&mydate.sql;
exit;       
