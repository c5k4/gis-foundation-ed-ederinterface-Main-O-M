
--column dcol new_value mydate noprint
--select to_char(sysdate,'YYYYMMDDHH24MMSS') dcol from dual;
SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0 
SET LINE 200   
spool "EDGMCdeleteVersions.sql" 
select 'EXEC sde.version_user_ddl.delete_version ('||chr(39)||owner||'.'||name||chr(39)||')' from sde.versions where name <> 'DEFAULT' and name not like 'PGE_%' order by Parent_VERSION_ID desc;
select 'EXIT;' from dual;

spool off;

--@D:\edgisdbmaint\log\DELETEPROCESSEDCIRCUITS&mydate.sql;
exit;       
