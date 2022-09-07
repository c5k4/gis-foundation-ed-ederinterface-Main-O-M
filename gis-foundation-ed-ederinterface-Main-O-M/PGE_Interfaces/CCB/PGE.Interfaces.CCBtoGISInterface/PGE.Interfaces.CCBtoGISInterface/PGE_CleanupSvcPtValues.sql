
-- 	Script to cleanup Service Point Attribute Values.
--	This scipt will set values to NULL for the attributes not required by the GIS
--	or are not maintained by the CCB Interface GIS update process.
--	Note:  Both the base table and delta table (Add) will be processed.
--
--	Avery (RDI) - 2/10/2016
--
--

set serveroutput on
variable n number
exec :n := dbms_utility.get_time

set timing on;

spool c:\temp\SVC_PT_Cleanup_Log.txt;

prompt Cleanup Service Point Attribute Values

prompt ----------------;
prompt  LOG = c:\temp\SVC_PT_Cleanup_Log.txt;
prompt ----------------;


prompt ***SENSITIVECUSTOMERIDC***;
prompt non-null count ***SENSITIVECUSTOMERIDC***;
select count(*) from EDGIS.SERVICEPOINT where SENSITIVECUSTOMERIDC is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set SENSITIVECUSTOMERIDC = NULL where SENSITIVECUSTOMERIDC is not null;
update EDGIS.A71 set SENSITIVECUSTOMERIDC = NULL where SENSITIVECUSTOMERIDC is not null;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where SENSITIVECUSTOMERIDC is not NULL;
prompt ----------------;

prompt ***LIFESUPPORTIDC***;
prompt non-null count ***LIFESUPPORTIDC***;
select count(*) from EDGIS.SERVICEPOINT where LIFESUPPORTIDC is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set LIFESUPPORTIDC = NULL where LIFESUPPORTIDC is not NULL;
update EDGIS.A71 set LIFESUPPORTIDC = NULL where LIFESUPPORTIDC is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where LIFESUPPORTIDC is not NULL;
prompt ----------------;

prompt ***AREACODE***;
prompt non-null count ***AREACODE***;
select count(*) from EDGIS.SERVICEPOINT where AREACODE is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set AREACODE = NULL where AREACODE is not NULL;
update EDGIS.A71 set AREACODE = NULL where AREACODE is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where AREACODE is not NULL;
prompt ----------------;

prompt ***PHONENUM***;
prompt non-null count ***PHONENUM***;
select count(*) from EDGIS.SERVICEPOINT where PHONENUM is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set PHONENUM = NULL where PHONENUM is not NULL;
update EDGIS.A71 set PHONENUM = NULL where PHONENUM is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where PHONENUM is not NULL;
prompt ----------------;

prompt ***MAILNAME1***;
prompt non-null count ***MAILNAME1***;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME1 is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILNAME1 = NULL where MAILNAME1 is not NULL;
update EDGIS.A71 set MAILNAME1 = NULL where MAILNAME1 is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME1 is not NULL;
prompt ----------------;

prompt ***MAILNAME2***;
prompt non-null count ***MAILNAME2***;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME2 is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILNAME2 = NULL where MAILNAME2 is not NULL;
update EDGIS.A71 set MAILNAME2 = NULL where MAILNAME2 is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME2 is not NULL;
prompt ----------------;

prompt ***MAILSTREETNUM***;
prompt non-null count ***MAILSTREETNUM***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNUM is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILSTREETNUM = NULL where MAILSTREETNUM is not NULL;
update EDGIS.A71 set MAILSTREETNUM = NULL where MAILSTREETNUM is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNUM is not NULL;
prompt ----------------;

prompt ***MAILSTREETNAME1***;
prompt non-null count ***MAILSTREETNAME1***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME1 is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILSTREETNAME1 = NULL where MAILSTREETNAME1 is not NULL;
update EDGIS.A71 set MAILSTREETNAME1 = NULL where MAILSTREETNAME1 is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME1 is not NULL;
prompt ----------------;

prompt ***MAILSTREETNAME2***;
prompt non-null count ***MAILSTREETNAME2***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME2 is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILSTREETNAME2 = NULL where MAILSTREETNAME2 is not NULL;
update EDGIS.A71 set MAILSTREETNAME2 = NULL where MAILSTREETNAME2 is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME2 is not NULL;
prompt ----------------;

prompt ***MAILCITY***;
prompt non-null count ***MAILCITY***;
select count(*) from EDGIS.SERVICEPOINT where MAILCITY is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILCITY = NULL where MAILCITY is not NULL;
update EDGIS.A71 set MAILCITY = NULL where MAILCITY is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILCITY is not NULL;
prompt ----------------;

prompt ***MAILSTATE***;
prompt non-null count ***MAILSTATE***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTATE is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILSTATE = NULL where MAILSTATE is not NULL;
update EDGIS.A71 set MAILSTATE = NULL where MAILSTATE is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILSTATE is not NULL;
prompt ----------------;

prompt ***MAILZIPCODE***;
prompt non-null count ***MAILZIPCODE***;
select count(*) from EDGIS.SERVICEPOINT where MAILZIPCODE is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set MAILZIPCODE = NULL where MAILZIPCODE is not NULL;
update EDGIS.A71 set MAILZIPCODE = NULL where MAILZIPCODE is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where MAILZIPCODE is not NULL;
prompt ----------------;

prompt ***DISTRICT***;
prompt non-null count ***DISTRICT***;
select count(*) from EDGIS.SERVICEPOINT where DISTRICT is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set DISTRICT = NULL where DISTRICT is not NULL;
update EDGIS.A71 set DISTRICT = NULL where DISTRICT is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where DISTRICT is not NULL;
prompt ----------------;

prompt ***DIVISION***;
prompt non-null count ***DIVISION***;
select count(*) from EDGIS.SERVICEPOINT where DIVISION is not NULL;
prompt ----------------;
update EDGIS.SERVICEPOINT set DIVISION = NULL where DIVISION is not NULL;
update EDGIS.A71 set DIVISION = NULL where DIVISION is not NULL;
prompt ----------------;
select count(*) from EDGIS.SERVICEPOINT where DIVISION is not NULL;
prompt ----------------;

exec :n := (dbms_utility.get_time - :n)/100
exec dbms_output.put_line(:n)

spool off;






