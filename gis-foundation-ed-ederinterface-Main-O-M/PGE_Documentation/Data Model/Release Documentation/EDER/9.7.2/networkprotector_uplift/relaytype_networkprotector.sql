/*
select RelayType,count(*) from edgis.networkprotector group by RelayType order by RelayType;

select RelayType,count(*) from (
select 
DECODE(RelayType,
'Electromechanical','EM',
'Microprocessor','MP',
'Solid State','SS',
'Unknown','UNK',
RelayType) RelayType 
from edgis.networkprotector) group by RelayType order by RelayType;
*/

select RelayType,count(*) from edgis.networkprotector group by RelayType order by RelayType;

update edgis.networkprotector np
set RelayType = DECODE(RelayType,
'Electromechanical','EM',
'Microprocessor','MP',
'Solid State','SS',
'Unknown','UNK',
RelayType) ;
commit;

select RelayType,count(*) from edgis.networkprotector group by RelayType order by RelayType;