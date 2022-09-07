SET HEADING OFF
SET FEEDBACK OFF 
SET ECHO OFF 
SET PAGESIZE 0 
spool D:\edgisdbmaint\log\ADMS_CD.log 
grant SELECT on "EDSETT"."SM_GENERATION" to "ADMS" WITH grant option;
grant SELECT on "EDSETT"."SM_GENERATOR" to "ADMS" WITH grant option;
grant SELECT on "EDSETT"."SM_PROTECTION" to "ADMS" WITH grant option;
grant EXECUTE on "EDSETT"."SM_EXPOSE_DATA_PKG" to "ADMS" WITH grant option;
spool off;

exit;