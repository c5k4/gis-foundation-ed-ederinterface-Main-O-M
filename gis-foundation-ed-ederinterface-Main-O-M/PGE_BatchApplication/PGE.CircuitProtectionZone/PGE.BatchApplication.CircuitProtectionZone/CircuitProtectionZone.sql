DROP TABLE EDGIS.PGE_CircuitProtZones_Process CASCADE CONSTRAINTS;

CREATE TABLE EDGIS.PGE_CircuitProtZones_Process
(
  PROCESSID INTEGER,
  CIRCUITID VARCHAR(40),
  CIRCUITSTATUS VARCHAR(40)
);

create index CPZ_CIRCUITID_IDX ON EDGIS.PGE_CircuitProtZones_Process (CIRCUITID);
create index CPZ_CIRCUITSUCCESS_IDX ON EDGIS.PGE_CircuitProtZones_Process (CIRCUITSTATUS);

grant all on EDGIS.PGE_CircuitProtZones_Process to gis_i;
grant all on EDGIS.PGE_CircuitProtZones_Process to gis_i_write;
