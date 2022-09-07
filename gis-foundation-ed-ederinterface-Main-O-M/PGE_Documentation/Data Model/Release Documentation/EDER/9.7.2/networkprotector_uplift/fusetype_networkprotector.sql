/*
select FuseType,count(*) from edgis.networkprotector group by FuseType order by FuseType;

select FuseType,count(*) from (
select 
DECODE(FuseType,
'External Silver/Sand Fuse','EXTSILSAND',
'Internal Copper Link/Metal All','INTCOPLNK',
FuseType) FuseType 
from edgis.networkprotector) group by FuseType order by FuseType;
*/

select FuseType,count(*) from edgis.networkprotector group by FuseType order by FuseType;

update edgis.networkprotector np
set FuseType = DECODE(FuseType,
'External Silver/Sand Fuse','EXTSILSAND',
'Internal Copper Link/Metal All','INTCOPLNK',
FuseType) ;
commit;

select FuseType,count(*) from edgis.networkprotector group by FuseType order by FuseType;

