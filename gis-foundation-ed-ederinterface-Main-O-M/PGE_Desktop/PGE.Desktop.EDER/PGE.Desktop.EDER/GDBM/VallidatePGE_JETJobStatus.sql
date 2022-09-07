--
-- 	Validation cript to test the Status value in the Jet Job table.
--	This tests the JET Job Status value so it is set to retire (2) when all associated equipment has been used (Status = 2).
--
--	Avery (RDI) - 2/19/2016
--
--



select JOBNUMBER 
from            WEBR.JET_EQUIPMENT
where            STATUS = 1 
and 		 JOBNUMBER in 
		  (select JOBNUMBER from WEBR.JET_JOBS where STATUS = 2);


select JOBNUMBER 
from            WEBR.JET_JOBS
where            STATUS = 2 
and 		 JOBNUMBER in 
		  (select JOBNUMBER from WEBR.JET_EQUIPMENT where STATUS = 1);







