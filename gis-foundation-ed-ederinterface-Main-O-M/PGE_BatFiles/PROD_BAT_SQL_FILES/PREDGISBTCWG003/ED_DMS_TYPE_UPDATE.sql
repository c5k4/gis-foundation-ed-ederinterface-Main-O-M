SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0    
spool D:\edgisdbmaint\log\ED_DMS_TYPE_UPDATE.log 

UPDATE dmsstaging.dmsstarttime SET EXTRACTTYPE = 'Bulk';
commit;


spool off;

exit;       
