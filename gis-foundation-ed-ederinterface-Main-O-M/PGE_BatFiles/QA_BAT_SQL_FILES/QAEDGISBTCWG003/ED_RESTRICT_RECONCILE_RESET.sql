SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0    
spool D:\edgisdbmaint\log\ED_RESTRICT_RECONCILE_RESET.log 

delete from SDE.gdbm_no_reconcile_versions where objectid>15 and length(owner)=4 ;
commit;


spool off;

exit;       
