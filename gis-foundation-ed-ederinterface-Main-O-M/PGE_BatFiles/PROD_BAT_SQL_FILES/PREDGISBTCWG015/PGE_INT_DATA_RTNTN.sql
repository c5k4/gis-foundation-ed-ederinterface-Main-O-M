SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0    
spool D:\edgisdbmaint\log\INTERFACEDATARETENTION.log 

call PGE_INT_DATA_BACKUP('&1');


spool off;

exit;       
