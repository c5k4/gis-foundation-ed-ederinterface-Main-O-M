DROP TABLE RELEDITOR.CCZ_DIFF;
CREATE TABLE RELEDITOR.CCZ_DIFF
(
  TABLENAME  NVARCHAR2(40),
  GLOBALID   NVARCHAR2(40),
  CITY       NVARCHAR2(40),
  COUNTY     NUMBER(5),
  ZIP        NVARCHAR2(10)
)
/
CREATE INDEX RELEDITOR.ccz_Idx1 ON RELEDITOR.CCZ_DIFF
(GLOBALID, TABLENAME)
/
CREATE INDEX RELEDITOR.ccz_Idx2 ON RELEDITOR.CCZ_DIFF
(CITY, COUNTY, ZIP)
/
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.ANCHOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.ANCHOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.CAPACITORBANK', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.CAPACITORBANK;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DCCONDUCTOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DCCONDUCTOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DCRECTIFIER', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DCRECTIFIER;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DCSERVICELOCATION', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DCSERVICELOCATION;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DEACTIVATEDELECTRICLINESEGMENT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DEACTIVATEDELECTRICLINESEGMENT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DELIVERYPOINT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DELIVERYPOINT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DEVICEGROUP', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DEVICEGROUP;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DISTBUSBAR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DISTBUSBAR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.DYNAMICPROTECTIVEDEVICE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.DYNAMICPROTECTIVEDEVICE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.ELECTRICSTITCHPOINT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.ELECTRICSTITCHPOINT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.FAULTINDICATOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.FAULTINDICATOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.FUSE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.FUSE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.NETWORKPROTECTOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0), '0' FROM EDGIS.NETWORKPROTECTOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.NEUTRALCONDUCTOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.NEUTRALCONDUCTOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.OPENPOINT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.OPENPOINT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PADMOUNTSTRUCTURE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PADMOUNTSTRUCTURE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PHOTOVOLTAICCELL', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PHOTOVOLTAICCELL;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PRIMARYMETER', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PRIMARYMETER;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PRIMARYRISER', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PRIMARYRISER;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PRIOHCONDUCTOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PRIOHCONDUCTOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.PRIUGCONDUCTOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.PRIUGCONDUCTOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SECONDARYLOADPOINT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SECONDARYLOADPOINT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SECONDARYRISER', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SECONDARYRISER;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SERVICELOCATION', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SERVICELOCATION;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.STEPDOWN', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.STEPDOWN;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.STREETLIGHT', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.STREETLIGHT;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SUBSTATION', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SUBSTATION;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SUBSURFACESTRUCTURE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SUBSURFACESTRUCTURE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SUPPORTSTRUCTURE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SUPPORTSTRUCTURE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.SWITCH', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.SWITCH;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.TIE', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.TIE;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.TRANSFORMER', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.TRANSFORMER;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.VAULTPOLY', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.VAULTPOLY;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.VOLTAGEREGULATOR', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.VOLTAGEREGULATOR;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A7366252', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A7366252;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A124', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A124;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A136', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A136;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A123', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A123;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A16727', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A16727;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A152', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A152;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A132', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A132;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A145', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A145;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A135', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A135;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A114', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A114;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A113', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A113;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A120', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A120;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A119', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A119;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A7727', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),'0' FROM EDGIS.A7727;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A150', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A150;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A126', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A126;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A146', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A146;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A1727', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A1727;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A130', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A130;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A385', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A385;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A139', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A139;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A137', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A137;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A115', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A115;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A384', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A384;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A128', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A128;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A118', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A118;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A127', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A127;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A143', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A143;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A133', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A133;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A144', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A144;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A121', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A121;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A122', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A122;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A117', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A117;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A147', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A147;
INSERT INTO RELEDITOR.CCZ_DIFF SELECT 'EDGIS.A116', GLOBALID, NVL(CITY,'0'),NVL(COUNTY,0),NVL(ZIP,'0') FROM EDGIS.A116;
COMMIT;
