--
-- 	Script to corect the Status value in the Jet Job table.
--	This corrects the JET Job Status value so it is set to retire (2) when all associated equipment has been used (Status = 2).
--
--	Avery (RDI) - 2/19/2016
--
--


update  WEBR.JET_JOBS set STATUS = 2
where	STATUS = 1
and	JOBNUMBER in
	  (select JOBNUMBER from WEBR.JET_EQUIPMENT 
	     where STATUS = 2);
	     
update  WEBR.JET_JOBS set STATUS = 1
where	STATUS = 2
and	JOBNUMBER in
	  (select JOBNUMBER from WEBR.JET_EQUIPMENT 
	     where STATUS = 1);