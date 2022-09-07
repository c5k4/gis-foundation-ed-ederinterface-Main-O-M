--
-- 	Script to validates the initialization of ROBC Established ROBC values which come from the from Desired ROBC values.
--
--	Avery (RDI) - 2/19/2016
--
--


select count(*) from edgis.robc where establishedrobc is null and desiredrobc is not null;
select count(*) from edgis.robc where ESTABLISHEDSUBBLOCK is null and DESIREDSUBBLOCK is not null and EstablishedROBC NOT IN (50, 60);