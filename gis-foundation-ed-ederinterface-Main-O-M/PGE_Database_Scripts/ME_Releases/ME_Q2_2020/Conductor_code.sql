spool "D:\Temp\Conductor_code.txt"
set define off;
set escape on;
set serverout on;

VARIABLE MAXOBJECTID NUMBER;
BEGIN

	SELECT MAX(OBJECTID) INTO :MAXOBJECTID FROM EDGIS.PGE_ConductorCodeMap;
END;
/
insert into EDGIS.PGE_ConductorCodeMap values(:MAXOBJECTID+1,'2','401','C','1100','EPR','2',null);
insert into EDGIS.PGE_ConductorCodeMap values(:MAXOBJECTID+2,'2','402','C','1100','EPR','2',null);
insert into EDGIS.PGE_ConductorCodeMap values(:MAXOBJECTID+3,'2','403','C','1100','EPR','4',null);

commit;

set define on;
set escape off;
spool off;