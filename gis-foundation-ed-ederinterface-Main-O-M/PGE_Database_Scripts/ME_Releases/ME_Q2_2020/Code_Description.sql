
spool "D:\Temp\CODES_AND_DESCRIPTIONS.txt"
set define off;
set escape on;
set serverout on;

INSERT INTO EDGIS.PGE_CODES_AND_DESCRIPTIONS VALUES ('Conductor Code - UG','401','401 - 1100cu epr-conc-encap-lldpe 15kV');

INSERT INTO EDGIS.PGE_CODES_AND_DESCRIPTIONS VALUES ('Conductor Code - UG','402','402 - 1100cu epr-conc-encap-lszh 15kV');

INSERT INTO EDGIS.PGE_CODES_AND_DESCRIPTIONS VALUES ('Conductor Code - UG','403','403 - 1100cu epr-conc-encap-lldpe 25kV');


COMMIT;

set define on;
set escape off;
spool off;