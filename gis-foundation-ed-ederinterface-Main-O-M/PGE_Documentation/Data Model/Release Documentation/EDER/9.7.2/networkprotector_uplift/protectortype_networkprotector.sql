/*
select ProtectorType,count(*) from edgis.networkprotector group by ProtectorType order by ProtectorType;

select ProtectorType,count(*) from (
select 
DECODE(ProtectorType,
'CM-22','CM22',
'CM-52','CM52',
'CMD','CMD',
'MG-8','MG8',
'NP113','NP113',
'NP313','NP313',
'NP315','NP315',
ProtectorType) ProtectorType 
from edgis.networkprotector) group by ProtectorType order by ProtectorType;
*/

select ProtectorType,count(*) from edgis.networkprotector group by ProtectorType order by ProtectorType;

update edgis.networkprotector np
set ProtectorType = DECODE(ProtectorType,
'CM-22','CM22',
'CM-52','CM52',
'CMD','CMD',
'MG-8','MG8',
'NP113','NP113',
'NP313','NP313',
'NP315','NP315',
ProtectorType) ;
commit;

select ProtectorType,count(*) from edgis.networkprotector group by ProtectorType order by ProtectorType;
