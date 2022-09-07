/* ***************************************************************** */
/* SCRIPT for PL/SQL process to call SDE and Delete existing views   */
/* and generate the views using the PGE standard names of ZZ_MV_     */
/* written by Rob Rader as part of 10.2.1 delivery 5/14/2015         */
/* To use this script first find and replace the following:          */
/* Replace [INSTANCE] with the Oracle Serice Name to use             */
/* Replace [USER] with the Oracle Account to use                     */
/* Replace [PASSWORD] with the Oracle account password to use        */
/* Information provided for LBGISQ1Q, you may just uncomment to run  */
/* Information provided for LBGISS2Q, you may just uncomment to run  */
/* ***************************************************************** */
-- First we drop all existing views
set trimspool on
set linesize 500
set pagesize 50000
spool c:\temp\drop_mv_views.bat
select 'sdetable -o delete_mv_view -t '||table_name||' -N -i sde:oracle11g:[INSTANCE] -u [USER] -p [PASSWORD]' from sde.table_registry where imv_view_name is not null and bitand(object_flags,8)=8 order by table_name;
-- LBGISS2Q Information: select 'sdetable -o delete_mv_view -t '||table_name||' -N -i sde:oracle11g:lbgiss2q -u edgis -p edgis!S2Qi' from sde.table_registry where imv_view_name is not null and bitand(object_flags,8)=8 order by table_name;
-- LBGISQ1Q Information: select 'sdetable -o delete_mv_view -t '||table_name||' -N -i sde:oracle11g:lbgisq1q -u edgis -p edgis!Q1Qi' from sde.table_registry where imv_view_name is not null and bitand(object_flags,8)=8 order by table_name;
spool off;
host c:\temp\drop_mv_views.bat

set trimspool on
set linesize 500
set pagesize 50000
spool c:\temp\create_mv_views_1021.bat
select 'sdetable -o create_mv_view -T ZZ_MV_'||SUBSTR(table_name,0,24)||' -t '||table_name||' -i sde:oracle11g:[INSTANCE] -u [USER] -p [PASSWORD]' from sde.table_registry where imv_view_name is null and bitand(object_flags,8)=8 order by table_name;
-- LBGISS2Q Information: select 'sdetable -o create_mv_view -T zz_mv_'||SUBSTR(table_name,0,24)||' -t '||table_name||' -i sde:oracle11g:lbgiss2q -u edgis -p edgis!S2Qi' from sde.table_registry where imv_view_name is null and bitand(object_flags,8)=8 order by table_name;
-- LBGISQ1Q Information: select 'sdetable -o create_mv_view -T zz_mv_'||SUBSTR(table_name,0,24)||' -t '||table_name||' -i sde:oracle11g:lbgisq1q -u edgis -p edgis!Q1Qi' from sde.table_registry where imv_view_name is null and bitand(object_flags,8)=8 order by table_name;
spool off;
host c:\temp\create_mv_views_1021.bat

