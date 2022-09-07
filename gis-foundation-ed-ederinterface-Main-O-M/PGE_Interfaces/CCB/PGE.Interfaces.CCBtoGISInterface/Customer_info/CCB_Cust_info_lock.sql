select count(*) 
from pgedata.pge_ccb_sp_io_monitor
where interfacetype = 'Inbound' and status <> 'Insert-Completed';

exit;