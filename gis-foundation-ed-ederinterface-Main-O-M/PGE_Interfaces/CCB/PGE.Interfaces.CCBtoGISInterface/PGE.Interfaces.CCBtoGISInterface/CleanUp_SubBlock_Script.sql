--
-- 	Script to clean established subblock values where established ROBC value is in 50 or 60.
--
--	Subhankar: 03/04/2016
--
--


update edgis.robc set establishedsubblock = null where establishedsubblock is not null and establishedrobc in (50, 60);
update edgis.A77 set establishedsubblock = null where establishedsubblock is not null and establishedrobc in (50, 60);
commit;