WHENEVER SQLERROR EXIT SQL.SQLCODE
select count(*) from EDGIS.PGE_UNDERGROUNDNETWORK_TRACE_A;
select count(*) from EDGIS.PGE_UNDERGROUNDNETWORK_TRACE_B;
exit;