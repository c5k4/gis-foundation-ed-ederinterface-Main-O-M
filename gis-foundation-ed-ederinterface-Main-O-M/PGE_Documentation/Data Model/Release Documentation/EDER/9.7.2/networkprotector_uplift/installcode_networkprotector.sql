column instcode format A35
column installcode format A35
/*
select distinct installcode from edgis.networkprotector order by installcode;

select instcode,count(*) from (
select 
DECODE(installcode,
'Overhead','OH',
'Pad Mounted','PAD',
'Subsurface','SUB',
'Underground','UG',
'New Structure','NEW',
installcode) instcode 
from edgis.networkprotector) group by instcode order by instcode;
*/
select installcode,count(*) from edgis.networkprotector group by installcode order by installcode;

update edgis.networkprotector np
set installcode = DECODE(installcode,
'Overhead','OH',
'Pad Mounted','PAD',
'Subsurface','SUB',
'Underground','UG',
'New Structure','NEW',
installcode) ;
commit;

select installcode,count(*) from edgis.networkprotector group by installcode order by installcode;
