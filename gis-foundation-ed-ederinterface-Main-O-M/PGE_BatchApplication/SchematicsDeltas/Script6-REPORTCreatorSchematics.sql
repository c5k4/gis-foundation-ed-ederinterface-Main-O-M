/* ----------------------------------------------
-- SCHEMATICS DELTA EXPORTER SCRIPT
-- Version 1.0 first relese Rob Rader 11/19/2014
------------------------------------------------- */
set trimspool on
SET TRIMOUT ON
set pagesize 0
set linesize 5000
spool c:\temp\schem_report_output_12-12-2014.csv
select 'ACTION,DIAGRAM_NAME,UGUID,UOID,TABLE,ORIG_UCID,ORIG_MinX,ORIG_MinY,ORIG_MaxX,ORIG_MaxY,ORIG_USID,NEW_UCID,NEW_MinX,NEW_MinY,NEW_MaxX,NEW_MaxY,NEW_USID' from dual;
-- Deletes
select 
   'DELETED,'||
   cm.NAME||','||
   tab1.UGUID||','||
   tab1.UOID||','||tab1.SCH_TABLE||','||
   tab1.UCID||','||tab1.EMINX||','||
   tab1.EMINY||','||tab1.EMAXX||','||
   tab1.EMAXY||','||tab1.USID||','||',"","","","","",""'
from  sde.SCHEM_GUID_PRE_DROP_TEMP tab1 left outer join edgis.sch1284D_circuitmap cm on tab1.DIAGRAMCLASSID=cm.ID
where  tab1.UGUID in (select dt1.UGUID from sde.SCHEM_GUID_DELTA_TEMP dt1 where action='D');

-- INSERTS
select 
   'INSERTS,'||
   cm.NAME||','||
   tab1.UGUID||','||
   tab1.UOID||','||tab1.SCH_TABLE||','||
   tab1.UCID||','||tab1.EMINX||','||
   tab1.EMINY||','||tab1.EMAXX||','||
   tab1.EMAXY||','||tab1.USID||','||',"","","","","",""'
from  sde.SCHEM_GUID_POST_DROP_TEMP tab1 left outer join edgis.sch1284D_circuitmap cm on tab1.DIAGRAMCLASSID=cm.ID
where  tab1.UGUID in (select dt1.UGUID from sde.SCHEM_GUID_DELTA_TEMP dt1 where action='I');

-- updates
select 
   'UPDATED,'||
   cm.NAME||','||
   tab1.UGUID||','||
   tab1.UOID||','||tab1.SCH_TABLE||','||
   tab1.UCID||','||tab1.EMINX||','||
   tab1.EMINY||','||tab1.EMAXX||','||
   tab1.EMAXY||','||tab1.USID||','||
   tab2.UCID||','||tab2.EMINX||','||
   tab2.EMINY||','||tab2.EMAXX||','||
   tab2.EMAXY||','||tab2.USID
from (select * from  sde.SCHEM_GUID_PRE_DROP_TEMP pre_tab1
where  pre_tab1.UGUID in (select dt11.UGUID from sde.SCHEM_GUID_DELTA_TEMP dt11 where dt11.action='U')
) TAB1 left outer join (
select * from  sde.SCHEM_GUID_POST_DROP_TEMP post_tab1
where  post_tab1.UGUID in (select dt12.UGUID from sde.SCHEM_GUID_DELTA_TEMP dt12 where dt12.action='U')
) TAB2 on TAB1.UGUID=TAB2.UGUID
left outer join edgis.sch1284D_circuitmap cm on tab1.DIAGRAMCLASSID=cm.ID;
spool off;

