WHENEVER SQLERROR EXIT SQL.SQLCODE
select count(*) from EDGIS.PGE_ELECDISTNETWORK_TRACE_A11;
select count(*) from EDGIS.PGE_ELECDISTNETWORK_TRACE_B;
exit;