InstallCode

/*
select InstallCode,count(*) from edgis.TransformerDevice group by InstallCode order by InstallCode;

select InstallCode,count(*) from (
select 
DECODE(InstallCode,
'Overhead','OH',
'Pad Mounted','PAD',
'Subsurface','SUB',
'Underground','UG',
'New Structure','NEW',
InstallCode) InstallCode 
from edgis.TransformerDevice) group by InstallCode order by InstallCode;
*/

select InstallCode,count(*) from edgis.TransformerDevice group by InstallCode order by InstallCode;

update edgis.TransformerDevice np
set InstallCode = DECODE(InstallCode,
'Overhead','OH',
'Pad Mounted','PAD',
'Subsurface','SUB',
'Underground','UG',
'New Structure','NEW',
InstallCode) ;
commit;

select InstallCode,count(*) from edgis.TransformerDevice group by InstallCode order by InstallCode;