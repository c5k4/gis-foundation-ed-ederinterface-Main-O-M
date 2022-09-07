--
-- 	Script to initialize ROBC Established ROBC values from Desired ROBC values.
--	This scipt updates the ROBC value where it is null and it has a Desired ROBC value.
--	Note:  Both the base table and delta table (Add) will be processed.
--
--	Avery (RDI) - 2/19/2016
--
-- Subhankar 3/1/2016
-- This scipt updates the SUBBLOCK value also where it is null and it has a Desired ROBC value.

update edgis.robc set establishedrobc = desiredrobc where establishedrobc is null and desiredrobc is not null;
update edgis.a77 set establishedrobc = desiredrobc where establishedrobc is null and desiredrobc is not null;

update edgis.robc set ESTABLISHEDSUBBLOCK = DESIREDSUBBLOCK where ESTABLISHEDSUBBLOCK is null and DESIREDSUBBLOCK is not null and EstablishedROBC NOT IN (50, 60);
update edgis.a77 set ESTABLISHEDSUBBLOCK = DESIREDSUBBLOCK where ESTABLISHEDSUBBLOCK is null and DESIREDSUBBLOCK is not null and EstablishedROBC NOT IN (50, 60);