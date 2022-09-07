SPOOL C:\Temp\DeviceGroup_CountChecksAfterUpdate.txt;
-- first six count number of items being deleted
select count(*) from edgis.devicegroup where subtypecd = 2 and devicegrouptype = 11;
select count(*) from edgis.devicegroup where subtypecd = 2 and devicegrouptype = 14;
select count(*) from edgis.devicegroup where subtypecd = 2 and devicegrouptype = 18;
select count(*) from edgis.a145 where subtypecd = 2 and devicegrouptype = 11;
select count(*) from edgis.a145 where subtypecd = 2 and devicegrouptype = 14;
select count(*) from edgis.a145 where subtypecd = 2 and devicegrouptype = 18;

select count(*) from edgis.devicegroup where subtypecd = 3 and devicegrouptype = 25;
select count(*) from edgis.devicegroup where subtypecd = 3 and devicegrouptype = 29;
select count(*) from edgis.devicegroup where subtypecd = 3 and devicegrouptype = 31;
select count(*) from edgis.a145 where subtypecd = 3 and devicegrouptype = 25;
select count(*) from edgis.a145 where subtypecd = 3 and devicegrouptype = 29;
select count(*) from edgis.a145 where subtypecd = 3 and devicegrouptype = 31;
-- end of six being deleted


select 'total ' || count(*) from edgis.devicegroup;
select 'null jboxs ' || count(*) from edgis.devicegroup where ((subtypecd = 2 and devicegrouptype = 29) 
   or (subtypecd = 3 and devicegrouptype = 36)) 
   and (trim(devicegroupname) is null);

-- the following two lines will error out until the device group python script has created the admslabel field
select 'not null ' || count(*) from edgis.devicegroup where admslabel is not null;
select 'null label ' || count(*) from edgis.devicegroup where admslabel is null;

spool off;