
-- 	Script to validate the cleanup Service Point Attribute Values script.
--	This scipt will ensure the values are NULL for the attributes not required by the GIS
--	or are not maintained by the CCB Interface GIS update process.
--	Note:  Only the base table will be processed.
--
--	Avery (RDI) - 2/10/2016
--
--

set serveroutput on
variable n number
exec :n := dbms_utility.get_time

set timing on;

spool c:\temp\SVC_PT_Cleanup_Validation_Log.txt;

prompt Validate Service Point Attribute Values

prompt ----------------;
prompt  LOG = c:\temp\SVC_PT_Cleanup_Validation_Log.txt;
prompt ----------------;


prompt ***SENSITIVECUSTOMERIDC***;
prompt non-null count ***SENSITIVECUSTOMERIDC***;
select count(*) from EDGIS.SERVICEPOINT where SENSITIVECUSTOMERIDC is not NULL;
prompt ----------------;


prompt ***LIFESUPPORTIDC***;
prompt non-null count ***LIFESUPPORTIDC***;
select count(*) from EDGIS.SERVICEPOINT where LIFESUPPORTIDC is not NULL;
prompt ----------------;


prompt ***AREACODE***;
prompt non-null count ***AREACODE***;
select count(*) from EDGIS.SERVICEPOINT where AREACODE is not NULL;
prompt ----------------;


prompt ***PHONENUM***;
prompt non-null count ***PHONENUM***;
select count(*) from EDGIS.SERVICEPOINT where PHONENUM is not NULL;
prompt ----------------;


prompt ***MAILNAME1***;
prompt non-null count ***MAILNAME1***;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME1 is not NULL;
prompt ----------------;


prompt ***MAILNAME2***;
prompt non-null count ***MAILNAME2***;
select count(*) from EDGIS.SERVICEPOINT where MAILNAME2 is not NULL;
prompt ----------------;


prompt ***MAILSTREETNUM***;
prompt non-null count ***MAILSTREETNUM***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNUM is not NULL;
prompt ----------------;


prompt ***MAILSTREETNAME1***;
prompt non-null count ***MAILSTREETNAME1***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME1 is not NULL;
prompt ----------------;


prompt ***MAILSTREETNAME2***;
prompt non-null count ***MAILSTREETNAME2***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTREETNAME2 is not NULL;
prompt ----------------;


prompt ***MAILCITY***;
prompt non-null count ***MAILCITY***;
select count(*) from EDGIS.SERVICEPOINT where MAILCITY is not NULL;
prompt ----------------;


prompt ***MAILSTATE***;
prompt non-null count ***MAILSTATE***;
select count(*) from EDGIS.SERVICEPOINT where MAILSTATE is not NULL;
prompt ----------------;


prompt ***MAILZIPCODE***;
prompt non-null count ***MAILZIPCODE***;
select count(*) from EDGIS.SERVICEPOINT where MAILZIPCODE is not NULL;
prompt ----------------;


prompt ***DISTRICT***;
prompt non-null count ***DISTRICT***;
select count(*) from EDGIS.SERVICEPOINT where DISTRICT is not NULL;
prompt ----------------;


prompt ***DIVISION***;
prompt non-null count ***DIVISION***;
select count(*) from EDGIS.SERVICEPOINT where DIVISION is not NULL;
prompt ----------------;


exec :n := (dbms_utility.get_time - :n)/100
exec dbms_output.put_line(:n)

spool off;