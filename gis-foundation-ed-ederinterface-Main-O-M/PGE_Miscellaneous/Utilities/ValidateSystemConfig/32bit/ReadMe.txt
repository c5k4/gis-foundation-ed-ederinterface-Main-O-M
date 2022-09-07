1. Configure log file path . On this path ,validation report will be generated. 
<add key="logfilelocation" value="c:/temp" />
2. Add QA Geomart TNS entry in tns_names.ora file if not added already. 
EDWIPGMC_QA, EDWIPGMC_QA.world=
  (DESCRIPTION =
    (ADDRESS=(PROTOCOL=TCP)(HOST=EDGMGQA1-SCAN)(PORT=1521))  
 (CONNECT_DATA=
      (SERVER=DEDICATED)
      (SERVICE_NAME= EDWIPGMC_QA)
    )
  )

EDPUBA_PRD_DC2, EDPUBA_PRD_DC2.WORLD =
  (DESCRIPTION =
    (FAILOVER=ON)
    (ADDRESS = (PROTOCOL = TCP)(HOST =  PREDGMDBOLG002.comp.pge.com)(PORT = 1521))
        (CONNECT_DATA =
      (SERVER = DEDICATED)
      (SERVICE_NAME = EDGEP2AP )
    )
  )