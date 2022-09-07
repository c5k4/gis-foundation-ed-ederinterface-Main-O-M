--  Before running this script,  the geodatabase has to be in state 0
--====================================================================

set serveroutput on;
/
BEGIN
dbms_output.put_line('Performing updates to the GIS table');

--SQL that goes through the service point table and checks for empty values and updates them null
dbms_output.put_line('Updating Servicepoint multiversion view ..'); 
       
update EDGIS.SERVICEPOINT
set CONVERSIONWORKPACKAGE = nvl(trim(CONVERSIONWORKPACKAGE), null),
  SERVICEPOINTID = nvl(trim(SERVICEPOINTID), null),
  UNIQUESPID = nvl(trim(UNIQUESPID), null),
  STREETNAME1 = nvl(trim(STREETNAME1), null),
  STREETNAME2 = nvl(trim(STREETNAME2), null),
  STATE = nvl(trim(STATE), null),
  ZIP = nvl(trim(ZIP), null),
  GPSSOURCE = nvl(trim(GPSSOURCE), null),
  ELEVATION = nvl(trim(ELEVATION), null),
  METERNUMBER = nvl(trim(METERNUMBER), null),
  BILLINGCYCLE = nvl(trim(BILLINGCYCLE), null),
  METERROUTE = nvl(trim(METERROUTE), null),
  PREMISETYPE = nvl(trim(PREMISETYPE), null),
  TOWNSHIPTERRITORYCODE = nvl(trim(TOWNSHIPTERRITORYCODE), null),
  NETENERGYMETERING = nvl(trim(NETENERGYMETERING), null),
  APNNUM = nvl(trim(APNNUM), null),
  NOTRANSFORMERIDC = nvl(trim(NOTRANSFORMERIDC), null),
  ESSENTIALCUSTOMERIDC = nvl(trim(ESSENTIALCUSTOMERIDC), null),
  SENSITIVECUSTOMERIDC = nvl(trim(SENSITIVECUSTOMERIDC), null),
  LIFESUPPORTIDC = nvl(trim(LIFESUPPORTIDC), null),
  RATESCHEDULE = nvl(trim(RATESCHEDULE), null),
  SERVICEAGREEMENTID = nvl(trim(SERVICEAGREEMENTID), null),
  ACCOUNTNUM = nvl(trim(ACCOUNTNUM), null),
  DATASOURCE = nvl(trim(DATASOURCE), null),
  PHONENUM = nvl(trim(PHONENUM), null),
  ACCOUNTID = nvl(trim(ACCOUNTID), null),
  CUSTOMERCLASS = nvl(trim(CUSTOMERCLASS), null),
  CUSTOMERTYPE = nvl(trim(CUSTOMERTYPE), null),
  SPECIALCUSTOMERTYPE = nvl(trim(SPECIALCUSTOMERTYPE), null),
  REVENUEACCOUNTCODE = nvl(trim(REVENUEACCOUNTCODE), null),
  MAILNAME1 = nvl(trim(MAILNAME1), null),
  MAILNAME2 = nvl(trim(MAILNAME2), null),
  MAILSTREETNUM = nvl(trim(MAILSTREETNUM), null),
  MAILSTREETNAME1 = nvl(trim(MAILSTREETNAME1), null),
  MAILSTREETNAME2 = nvl(trim(MAILSTREETNAME2), null),
  MAILCITY = nvl(trim(MAILCITY), null),
  MAILSTATE = nvl(trim(MAILSTATE), null),
  MAILZIPCODE = nvl(trim(MAILZIPCODE), null),
  LOADSOURCEGUID = nvl(trim(LOADSOURCEGUID), null),
  SERVICELOCATIONGUID = nvl(trim(SERVICELOCATIONGUID), null),
  PRIMARYMETERGUID = nvl(trim(PRIMARYMETERGUID), null),
  GEMSOTHERMAPNUM = nvl(trim(GEMSOTHERMAPNUM), null),
  SECONDARYGENERATIONGUID = nvl(trim(SECONDARYGENERATIONGUID), null),
  PRIMARYGENERATIONGUID = nvl(trim(PRIMARYGENERATIONGUID), null),
  STREETNUMBER = nvl(trim(STREETNUMBER), null),
  TRANSFORMERGUID = nvl(trim(TRANSFORMERGUID), null),
  LOCALOFFICEID = nvl(trim(LOCALOFFICEID), null),
  CITY = nvl(trim(CITY), null),
  DCSERVICELOCATIONGUID = nvl(trim(DCSERVICELOCATIONGUID), null),
  DCRECTIFIERGUID = nvl(trim(DCRECTIFIERGUID), null);

dbms_output.put_line('Updating NULL values --- Done!!!');
COMMIT;

-- copy the service point id to the uniquespid field where the unique spid field is null.
dbms_output.put_line('Updating UNIQUESPID');
update EDGIS.SERVICEPOINT
set UNIQUESPID = (case when length(trim(UNIQUESPID)) is null then SERVICEPOINTID else UNIQUESPID end);
dbms_output.put_line('Updating UNIQUESPID -- Done!!!');
COMMIT;

-- then remove all service point ids that are duplicates, keeping one row.
dbms_output.put_line('Removing  rows  with duplicate SERVICEPOINTID');
DELETE FROM EDGIS.SERVICEPOINT WHERE rowid not in (SELECT MIN(rowid) FROM EDGIS.SERVICEPOINT GROUP BY SERVICEPOINTID);
dbms_output.put_line('Removing  rows  with duplicate SERVICEPOINTID --- Done!!!');
COMMIT;

-- then remove all service points where the uniquespid is duplicate.
dbms_output.put_line('Removing  rows  with duplicate UNIQUESPID');
DELETE FROM EDGIS.SERVICEPOINT WHERE rowid not in (SELECT MIN(rowid) FROM EDGIS.SERVICEPOINT GROUP BY UNIQUESPID);
dbms_output.put_line('Removing  rows  with duplicate UNIQUESPID -- Done!!!');
COMMIT;
 
dbms_output.put_line('Done!!!');

END;


