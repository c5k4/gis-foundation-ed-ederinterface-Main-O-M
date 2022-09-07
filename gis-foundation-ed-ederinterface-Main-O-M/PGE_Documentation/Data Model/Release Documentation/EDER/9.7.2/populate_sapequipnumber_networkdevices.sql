/* ************************************* */
-- Switch subtype GroundSwitch 7
/* ************************************* */
select count(*) from (
select * 
from edgis.switch  
where subtypecd=7
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on sw.structureguid=dg.globalid 
where sw.SAPSortField  is null;

MERGE INTO 
edgis.switch tbl
USING ( 
select 
sw.rowid as rid,
sw.globalid as tabglobalid,
dgxfmr.globalid dgid,
dgxfmr.operatingnumber opnum
from (
select * 
from edgis.switch  
where switch.subtypecd=7
and switch.SAPSortField is null
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dgxfmr 
on sw.structureguid=dgxfmr.globalid 
) TU
on (tbl.globalid=TU.tabglobalid)
when matched then update set tbl.SAPSortField=TU.opnum;

select count(*) from (
select * 
from edgis.switch  
where subtypecd=7
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on sw.structureguid=dg.globalid 
where sw.SAPSortField  is null;

 /* *************************************************** */
 -- TransformerDevice subtype PrimaryConnectionChamber 1
 /* *************************************************** */
select count(*) from edgis.transformerdevice td 
inner join (
select globalid,operatingnumber from edgis.transformer where operatingnumber is not null
) xfmr 
on 
td.transformerguid=xfmr.globalid 
where td.SAPSortField  is null;


MERGE INTO 
edgis.transformerdevice tbl
USING ( 
select 
tab1.rowid as rid,
tab1.globalid as tabglobalid,
xfmr.globalid tab2gid,
xfmr.operatingnumber tab2opnum
from (
select * 
from edgis.transformerdevice  
where transformerdevice.SAPSortField is null
) tab1
inner join (
select globalid,operatingnumber from edgis.transformer where operatingnumber is not null
) xfmr 
on 
tab1.transformerguid=xfmr.globalid  
) TU
on (tbl.globalid=TU.tabglobalid)
when matched then update set tbl.SAPSortField=TU.tab2opnum;

select count(*) from edgis.transformerdevice td 
inner join (
select globalid,operatingnumber from edgis.transformer where operatingnumber is not null
) xfmr 
on 
td.transformerguid=xfmr.globalid 
where td.SAPSortField  is null;

/* *************************************************** */
--         NetworkProtector 
/* *************************************************** */
select count(*) from (
select * 
from edgis.networkprotector  
) np
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on np.structureguid=dg.globalid 
where np.SAPSortField  is null;


MERGE INTO 
edgis.networkprotector tbl
USING ( 
select 
np.rowid as rid,
np.globalid as tabglobalid,
dgxfmr.globalid dgid,
dgxfmr.operatingnumber opnum
from (
select * 
from edgis.networkprotector  
where SAPSortField is null
) np
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dgxfmr
on np.structureguid=dgxfmr.globalid  
) TU
on (tbl.globalid=TU.tabglobalid)
when matched then update set tbl.SAPSortField=TU.opnum;

select count(*) from (
select * 
from edgis.networkprotector  
) np
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on np.structureguid=dg.globalid 
where np.SAPSortField  is null;

/* *************************************************** */
--        OpenPoint subtype Dead Break Elbow
/* *************************************************** */
select count(*) from (
select * from edgis.openpoint  
where subtypecd=2
) opp
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2
  ) dg1  
  inner join
  (
  select * from edgis.transformer
  ) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on opp.structureguid=dg.globalid 
where opp.SAPSortField  is null;


MERGE INTO 
edgis.openpoint tbl
USING ( 
select 
opp.rowid as rid,
opp.globalid as tabglobalid,
dgxfmr.globalid dgid,
dgxfmr.operatingnumber opnum
from (
select * from edgis.openpoint  
where subtypecd=2
and SAPSortField  is null
) opp
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2
  ) dg1  
  inner join
  (
  select * from edgis.transformer
  ) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dgxfmr
on opp.structureguid=dgxfmr.globalid 
) TU
on (tbl.globalid=TU.tabglobalid)
when matched then update set tbl.SAPSortField=TU.opnum;

select count(*) from (
select * from edgis.openpoint  
where subtypecd=2
) opp
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2
  ) dg1  
  inner join
  (
  select * from edgis.transformer
  ) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on opp.structureguid=dg.globalid 
where opp.SAPSortField  is null;

/* *************************************************** */
--       Switch subtype Subsurface Switch
/* *************************************************** */
select count(*) from (
select * 
from edgis.switch  
where subtypecd=5
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on sw.structureguid=dg.globalid 
where sw.SAPSortField  is null;

MERGE INTO 
edgis.switch tbl
USING ( 
select 
sw.rowid as rid,
sw.globalid as tabglobalid,
dgxfmr.globalid dgid,
dgxfmr.operatingnumber opnum
from (
select * 
from edgis.switch  
where switch.subtypecd=5
and switch.SAPSortField is null
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dgxfmr 
on sw.structureguid=dgxfmr.globalid 
) TU
on (tbl.globalid=TU.tabglobalid)
when matched then update set tbl.SAPSortField=TU.opnum;

select count(*) from (
select * 
from edgis.switch  
where subtypecd=5
) sw
inner join (
select 
     dg1.globalid,
     xfmr.operatingnumber
from ( 
  select * from edgis.devicegroup 
  where devicegroup.subtypecd=2) dg1  
  inner join
  (select * from edgis.transformer) xfmr
  on dg1.globalid= xfmr.STRUCTUREGUID
) dg
on sw.structureguid=dg.globalid 
where sw.SAPSortField  is null;

commit;
