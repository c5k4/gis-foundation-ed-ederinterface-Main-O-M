-- these are the six being removed, so we are moving them to a different type
Update edgis.devicegroup set devicegrouptype = 10 where subtypecd = 2 and devicegrouptype = 11;
Update edgis.devicegroup set devicegrouptype = 13 where subtypecd = 2 and devicegrouptype = 14;
Update edgis.devicegroup set devicegrouptype = 17 where subtypecd = 2 and devicegrouptype = 18;

Update edgis.devicegroup set devicegrouptype = 24 where subtypecd = 3 and devicegrouptype = 25;
Update edgis.devicegroup set devicegrouptype = 28 where subtypecd = 3 and devicegrouptype = 29;
Update edgis.devicegroup set devicegrouptype = 30 where subtypecd = 3 and devicegrouptype = 31;

Update edgis.A145 set devicegrouptype = 10 where subtypecd = 2 and devicegrouptype = 11;
Update edgis.A145 set devicegrouptype = 13 where subtypecd = 2 and devicegrouptype = 14;
Update edgis.A145 set devicegrouptype = 17 where subtypecd = 2 and devicegrouptype = 18;

Update edgis.A145 set devicegrouptype = 24 where subtypecd = 3 and devicegrouptype = 25;
Update edgis.A145 set devicegrouptype = 28 where subtypecd = 3 and devicegrouptype = 29;
Update edgis.A145 set devicegrouptype = 30 where subtypecd = 3 and devicegrouptype = 31;

commit;