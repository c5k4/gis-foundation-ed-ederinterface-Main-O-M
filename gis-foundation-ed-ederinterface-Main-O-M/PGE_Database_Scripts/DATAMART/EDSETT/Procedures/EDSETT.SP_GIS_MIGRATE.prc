Prompt drop Procedure SP_GIS_MIGRATE;
DROP PROCEDURE EDSETT.SP_GIS_MIGRATE
/

Prompt Procedure SP_GIS_MIGRATE;
--
-- SP_GIS_MIGRATE  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_GIS_MIGRATE"
AS
NUM  NUMBER;

BEGIN



INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select GLOBALID,'Switch','EDGIS.Switch',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.SWITCH  where nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);


INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select SW.GLOBALID,'Switch','EDGIS.SubSwitch',SW.OPERATINGNUMBER,'',SB.DIVISION,SB.DISTRICT
from EDSETTGIS.SUBSWITCH  SW LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB  ON SB.NAME=SW.SUBSTATIONNAME
where nvl(SW.division, 999) in ( select nvl(div_#,999) from gis_mig_div);






INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select  GLOBALID,'Sectionalizer','EDGIS.DynamicProtectiveDevice',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.DYNAMICPROTECTIVEDEVICE WHERE SUBTYPECD = 8  and  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);


INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select GLOBALID,'Interrupter','EDGIS.DynamicProtectiveDevice',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.DYNAMICPROTECTIVEDEVICE WHERE SUBTYPECD = 2
and  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);



INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select GLOBALID,'Interrupter','EDGIS.SubInterruptingDevice',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.SUBINTERRUPTINGDEVICE WHERE SUBTYPECD = 4
and  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);


INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select
SI.GLOBALID,'CircuitBreaker','EDGIS.SubInterruptingDevice',SI.SUBSTATIONNAME||'-'||SI.OPERATINGNUMBER,SI.CEDSADEVICEID,SB.DIVISION,SB.DISTRICT
from EDSETTGIS.SUBINTERRUPTINGDEVICE  SI
LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB
ON SB.NAME=SI.SUBSTATIONNAME WHERE (SI.SUBTYPECD = 1 or SI.SUBTYPECD = 2) and  nvl(SB.division, 999) in ( select nvl
(div_#,999) from gis_mig_div);


INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
SELECT SC.GLOBALID,'Capacitor','EDGIS.SubCapacitorBank',SC.OPERATINGNUMBER,'',SB.DIVISION,SB.DISTRICT  from
EDSETTGIS.SUBCAPACITORBANK  SC
LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB  ON SB.NAME=SC.SUBSTATIONNAME
WHERE  nvl(SC.division, 999) in ( select nvl(div_#,999) from gis_mig_div);


INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select GLOBALID,'Capacitor','EDGIS.CapacitorBank',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.CAPACITORBANK
WHERE  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div)
and EDSETTGIS.CAPACITORBANK.SUBTYPECD <> 1;


INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
SELECT
SC.GLOBALID,'Recloser','EDGIS.SubInterruptingDevice',SC.OPERATINGNUMBER,SC.CEDSADEVICEID,SB.DIVISION,SB.DISTRICT  from
EDSETTGIS.SUBINTERRUPTINGDEVICE SC
LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB  ON SB.NAME=SC.SUBSTATIONNAME
WHERE  SC.SUBTYPECD = 3
AND  nvl(SC.division, 999) in ( select nvl(div_#,999) from gis_mig_div);



INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
select GLOBALID,'Recloser','EDGIS.DynamicProtectiveDevice',OPERATINGNUMBER,CEDSADEVICEID,DIVISION,DISTRICT from EDSETTGIS.DYNAMICPROTECTIVEDEVICE WHERE SUBTYPECD = 3
AND  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);



INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
SELECT SC.GLOBALID,'Regulator','EDGIS.SUBTransformerBank',SC.OPERATINGNUMBER,'',SB.DIVISION,SB.DISTRICT  from
EDSETTGIS.SUBTRANSFORMERBANK SC
LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB  ON SB.NAME=SC.SUBSTATIONNAME
WHERE  nvl(SC.division, 999) in ( select nvl(div_#,999) from gis_mig_div);




INSERT INTO GIS_CEDSADEVICEID
(GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT)
SELECT VRU.GLOBALID,'Regulator','EDGIS.SubVoltageRegulatorUnit',VR.OPERATINGNUMBER,'',SB.DIVISION,SB.DISTRICT  from
EDSETTGIS.SUBVOLTAGEREGULATORUNIT VRU, EDSETTGIS.SUBVOLTAGEREGULATOR  VR
LEFT OUTER JOIN EDSETTGIS.SUBSTATION SB  ON SB.NAME=VR.SUBSTATIONNAME
WHERE VR.GLOBALID = VRU.VOLTAGEREGULATORGUID AND   nvl(VR.division, 999) in ( select nvl(div_#,999) from gis_mig_div);



INSERT INTO GIS_CEDSADEVICEID (GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,DIVISION,DISTRICT,BANKCODE)
select VRU.GLOBALID,'Regulator','EDGIS.VoltageRegulatorUnit',OPERATINGNUMBER||' - Bank Code ' ||VRU.BANKCODE ,VR.CEDSADEVICEID,DIVISION,DISTRICT,VRU.BANKCODE
from EDSETTGIS.VOLTAGEREGULATOR  VR, EDSETTGIS.VOLTAGEREGULATORUNIT VRU
WHERE VR.GLOBALID = VRU.REGULATORGUID AND  nvl(division, 999) in ( select nvl(div_#,999) from gis_mig_div);


--- Replace numeric values with Alphanumuric values in DISTRICT,DIVISION Fields

UPDATE GIS_CEDSADEVICEID GD SET DISTRICT = ( SELECT DIST_NAME FROM GIS_DISTRICTS WHERE GD.DISTRICT=DIST_# );
UPDATE GIS_CEDSADEVICEID GD SET DIVISION = (SELECT DIV_NAME FROM GIS_DIVISIONS WHERE GD.DIVISION=DIV_#);


---Replace NULL values for DEVICE & DISTRICT fields in 	GIS_CEDSADEVICEID with appropriate values.

UPDATE  GIS_CEDSADEVICEID G   SET G.DIVISION = (SELECT DV.DIVISION FROM GIS_DIVDIST DV  WHERE DV.GLOBAL_ID = G.GLOBAL_ID)
WHERE   G.DIVISION IS NULL;

UPDATE  GIS_CEDSADEVICEID G   SET G.DISTRICT = (SELECT DV.DISTRICT FROM GIS_DIVDIST DV  WHERE DV.GLOBAL_ID = G.GLOBAL_ID)
WHERE   G.DISTRICT IS NULL;


COMMIT;




SELECT COUNT(*) INTO NUM FROM GIS_CEDSADEVICEID;

DBMS_OUTPUT.PUT_LINE('Number of rows inserted in GIS_CEDSADEVICEID : '|| NUM);


END SP_GIS_MIGRATE ;

/