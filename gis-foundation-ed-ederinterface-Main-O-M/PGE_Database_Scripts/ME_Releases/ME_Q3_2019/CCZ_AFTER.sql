DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.ANCHOR E1 WHERE C1.TABLENAME = 'EDGIS.ANCHOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.CAPACITORBANK E1 WHERE C1.TABLENAME = 'EDGIS.CAPACITORBANK' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DCCONDUCTOR E1 WHERE C1.TABLENAME = 'EDGIS.DCCONDUCTOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DCRECTIFIER E1 WHERE C1.TABLENAME = 'EDGIS.DCRECTIFIER' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DCSERVICELOCATION E1 WHERE C1.TABLENAME = 'EDGIS.DCSERVICELOCATION' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DEACTIVATEDELECTRICLINESEGMENT E1 WHERE C1.TABLENAME = 'EDGIS.DEACTIVATEDELECTRICLINESEGMENT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DELIVERYPOINT E1 WHERE C1.TABLENAME = 'EDGIS.DELIVERYPOINT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DEVICEGROUP E1 WHERE C1.TABLENAME = 'EDGIS.DEVICEGROUP' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DISTBUSBAR E1 WHERE C1.TABLENAME = 'EDGIS.DISTBUSBAR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.DYNAMICPROTECTIVEDEVICE E1 WHERE C1.TABLENAME = 'EDGIS.DYNAMICPROTECTIVEDEVICE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.ELECTRICSTITCHPOINT E1 WHERE C1.TABLENAME = 'EDGIS.ELECTRICSTITCHPOINT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.FAULTINDICATOR E1 WHERE C1.TABLENAME = 'EDGIS.FAULTINDICATOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.FUSE E1 WHERE C1.TABLENAME = 'EDGIS.FUSE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.NETWORKPROTECTOR E1 WHERE C1.TABLENAME = 'EDGIS.NETWORKPROTECTOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.NEUTRALCONDUCTOR E1 WHERE C1.TABLENAME = 'EDGIS.NEUTRALCONDUCTOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.OPENPOINT E1 WHERE C1.TABLENAME = 'EDGIS.OPENPOINT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PADMOUNTSTRUCTURE E1 WHERE C1.TABLENAME = 'EDGIS.PADMOUNTSTRUCTURE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PHOTOVOLTAICCELL E1 WHERE C1.TABLENAME = 'EDGIS.PHOTOVOLTAICCELL' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PRIMARYMETER E1 WHERE C1.TABLENAME = 'EDGIS.PRIMARYMETER' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PRIMARYRISER E1 WHERE C1.TABLENAME = 'EDGIS.PRIMARYRISER' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PRIOHCONDUCTOR E1 WHERE C1.TABLENAME = 'EDGIS.PRIOHCONDUCTOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.PRIUGCONDUCTOR E1 WHERE C1.TABLENAME = 'EDGIS.PRIUGCONDUCTOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SECONDARYLOADPOINT E1 WHERE C1.TABLENAME = 'EDGIS.SECONDARYLOADPOINT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SECONDARYRISER E1 WHERE C1.TABLENAME = 'EDGIS.SECONDARYRISER' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SERVICELOCATION E1 WHERE C1.TABLENAME = 'EDGIS.SERVICELOCATION' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.STEPDOWN E1 WHERE C1.TABLENAME = 'EDGIS.STEPDOWN' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.STREETLIGHT E1 WHERE C1.TABLENAME = 'EDGIS.STREETLIGHT' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SUBSTATION E1 WHERE C1.TABLENAME = 'EDGIS.SUBSTATION' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SUBSURFACESTRUCTURE E1 WHERE C1.TABLENAME = 'EDGIS.SUBSURFACESTRUCTURE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SUPPORTSTRUCTURE E1 WHERE C1.TABLENAME = 'EDGIS.SUPPORTSTRUCTURE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.SWITCH E1 WHERE C1.TABLENAME = 'EDGIS.SWITCH' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.TIE E1 WHERE C1.TABLENAME = 'EDGIS.TIE' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.TRANSFORMER E1 WHERE C1.TABLENAME = 'EDGIS.TRANSFORMER' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.VAULTPOLY E1 WHERE C1.TABLENAME = 'EDGIS.VAULTPOLY' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.VOLTAGEREGULATOR E1 WHERE C1.TABLENAME = 'EDGIS.VOLTAGEREGULATOR' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A7366252 E1 WHERE C1.TABLENAME = 'EDGIS.A7366252' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A124 E1 WHERE C1.TABLENAME = 'EDGIS.A124' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A136 E1 WHERE C1.TABLENAME = 'EDGIS.A136' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A123 E1 WHERE C1.TABLENAME = 'EDGIS.A123' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A16727 E1 WHERE C1.TABLENAME = 'EDGIS.A16727' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A152 E1 WHERE C1.TABLENAME = 'EDGIS.A152' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A132 E1 WHERE C1.TABLENAME = 'EDGIS.A132' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A145 E1 WHERE C1.TABLENAME = 'EDGIS.A145' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A135 E1 WHERE C1.TABLENAME = 'EDGIS.A135' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A114 E1 WHERE C1.TABLENAME = 'EDGIS.A114' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A113 E1 WHERE C1.TABLENAME = 'EDGIS.A113' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A120 E1 WHERE C1.TABLENAME = 'EDGIS.A120' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A119 E1 WHERE C1.TABLENAME = 'EDGIS.A119' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A7727 E1 WHERE C1.TABLENAME = 'EDGIS.A7727' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A150 E1 WHERE C1.TABLENAME = 'EDGIS.A150' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A126 E1 WHERE C1.TABLENAME = 'EDGIS.A126' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A146 E1 WHERE C1.TABLENAME = 'EDGIS.A146' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A1727 E1 WHERE C1.TABLENAME = 'EDGIS.A1727' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A130 E1 WHERE C1.TABLENAME = 'EDGIS.A130' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A385 E1 WHERE C1.TABLENAME = 'EDGIS.A385' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A139 E1 WHERE C1.TABLENAME = 'EDGIS.A139' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A137 E1 WHERE C1.TABLENAME = 'EDGIS.A137' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A115 E1 WHERE C1.TABLENAME = 'EDGIS.A115' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A384 E1 WHERE C1.TABLENAME = 'EDGIS.A384' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A128 E1 WHERE C1.TABLENAME = 'EDGIS.A128' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A118 E1 WHERE C1.TABLENAME = 'EDGIS.A118' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A127 E1 WHERE C1.TABLENAME = 'EDGIS.A127' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A143 E1 WHERE C1.TABLENAME = 'EDGIS.A143' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A133 E1 WHERE C1.TABLENAME = 'EDGIS.A133' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A144 E1 WHERE C1.TABLENAME = 'EDGIS.A144' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A121 E1 WHERE C1.TABLENAME = 'EDGIS.A121' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A122 E1 WHERE C1.TABLENAME = 'EDGIS.A122' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A117 E1 WHERE C1.TABLENAME = 'EDGIS.A117' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A147 E1 WHERE C1.TABLENAME = 'EDGIS.A147' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
DELETE FROM RELEDITOR.CCZ_DIFF C1 WHERE EXISTS (SELECT 1 FROM EDGIS.A116 E1 WHERE C1.TABLENAME = 'EDGIS.A116' AND C1.GLOBALID = E1.GLOBALID AND NVL(C1.CITY,'0') = NVL(E1.CITY,'0') AND NVL(C1.COUNTY,0) = NVL(E1.COUNTY,0) AND NVL(C1.ZIP,'0') = NVL(E1.ZIP,'0'));
COMMIT;