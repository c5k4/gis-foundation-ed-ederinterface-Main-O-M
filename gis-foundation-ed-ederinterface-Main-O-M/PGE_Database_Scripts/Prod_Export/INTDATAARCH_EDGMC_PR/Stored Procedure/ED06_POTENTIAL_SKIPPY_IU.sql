--------------------------------------------------------
--  DDL for Procedure ED06_POTENTIAL_SKIPPY_IU
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "INTDATAARCH"."ED06_POTENTIAL_SKIPPY_IU" (startdate IN INTEGER,enddate IN INTEGER) 
AS
begin

INSERT INTO INTDATAARCH.ED06_Potential_Skippy 
--(FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_OID,FEAT_SAPEQIPID_OLD,CAPTURE_DATE,BACKUPDATE,RUNDATE,REPROCESSED)
(FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_OID,FEAT_SAPEQIPID_OLD,CAPTURE_DATE,BACKUPDATE,RUNDATE)
(
Select 
--FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_OID,FEAT_SAPEQIPID_OLD,CAPTURE_DATE,BACKUPDATE,sysdate RUNDATE, '0' REPROCESSED
FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_OID,FEAT_SAPEQIPID_OLD,CAPTURE_DATE,BACKUPDATE,sysdate RUNDATE
from INTDATAARCH.PGE_GDBM_Ah_INFO where feat_classname in ('EDGIS.ELECTRICSTITCHPOINT','EDGIS.CIRCUITSOURCE','EDGIS.SUPPORTSTRUCTURE','EDGIS.SUPPORTSTRUCTUREPTT',
'EDGIS.SUBSURFACESTRUCTURE','EDGIS.PADMOUNTSTRUCTURE','EDGIS.DEVICEGROUP','EDGIS.VOLTAGEREGULATORUNIT','EDGIS.VOLTAGEREGULATOR','EDGIS.CAPACITORBANK','EDGIS.CONTROLLER',
'EDGIS.DYNAMICPROTECTIVEDEVICE','EDGIS.SWITCH','EDGIS.STEPDOWNUNIT','EDGIS.STEPDOWN','EDGIS.TRANSFORMERUNIT','EDGIS.TRANSFORMER','EDGIS.STREETLIGHT','EDGIS.FAULTINDICATOR',
'EDGIS.OPENPOINT','EDGIS.TRANSFORMERDEVICE','EDGIS.NETWORKPROTECTOR') 
AND BACKUPDATE >= sysdate - startdate
AND BACKUPDATE <= sysdate - enddate
and ("USAGE" is null or "USAGE" not like '%NOED06%')
AND ACTION IN ('U','I') 
AND STATUS='C' 
AND
( -- Feature Class Check
( FEAT_CLASSNAME = 'EDGIS.ELECTRICSTITCHPOINT' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID = FEAT_GLOBALID AND OBJECTID = FEAT_OID) in ('2')
AND (SELECT STATUS from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID = FEAT_GLOBALID) <> 'Y'
) --StitchPoint
OR ( FEAT_CLASSNAME = 'EDGIS.CIRCUITSOURCE' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID  = (select DEVICEGUID from EDGIS.ZZ_MV_CIRCUITSOURCE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('2')
AND (Select STATUS from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID  = (select DEVICEGUID from EDGIS.ZZ_MV_CIRCUITSOURCE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_ELECTRICSTITCHPOINT@EDER where GLOBALID  = (select DEVICEGUID from EDGIS.ZZ_MV_CIRCUITSOURCE@EDER where GLOBALID = FEAT_GLOBALID)) <> 'Y'
) --StitchPoint -- ASSETID will be from 'EDGIS.ELECTRICSTITCHPOINT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.SUPPORTSTRUCTURE' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_SUPPORTSTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) in ('1','2','3','4','5','6','7','8')
AND (SELECT STATUS from EDGIS.ZZ_MV_SUPPORTSTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)  <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_SUPPORTSTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)  <> 'Y'
) -- Pole
OR ( FEAT_CLASSNAME = 'EDGIS.SUPPORTSTRUCTUREPTT' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_SUPPORTSTRUCTUREPTT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) in ('1','2','4','5','6','7','8')
AND (SELECT STATUS from EDGIS.ZZ_MV_SUPPORTSTRUCTUREPTT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)  <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_SUPPORTSTRUCTUREPTT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Pole
OR ( FEAT_CLASSNAME = 'EDGIS.SUBSURFACESTRUCTURE' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) in ('1','2','5','6','7','8')
AND (SELECT STATUS from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --SubsurfaceStructure
OR ( FEAT_CLASSNAME = 'EDGIS.SUBSURFACESTRUCTURE' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) in ('3')
AND (SELECT STATUS from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_SUBSURFACESTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Vault
OR ( FEAT_CLASSNAME = 'EDGIS.PADMOUNTSTRUCTURE'
AND (SELECT STATUS from EDGIS.ZZ_MV_PADMOUNTSTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_PADMOUNTSTRUCTURE@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --PadmountStructure
OR ( FEAT_CLASSNAME = 'EDGIS.DEVICEGROUP' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID and DeviceGroupType != 33 ) in ('2','3')
AND (SELECT STATUS from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --DeviceGroup
OR ( FEAT_CLASSNAME = 'EDGIS.DEVICEGROUP' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID and DeviceGroupType = 33 ) in ('3')
AND (SELECT STATUS from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --DeviceGroup
OR ( FEAT_CLASSNAME = 'EDGIS.DEVICEGROUP' 
AND (SELECT SUBTYPECD from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID and DeviceGroupType = 33 ) in ('2')
AND (SELECT STATUS from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (SELECT CUSTOMEROWNED from EDGIS.ZZ_MV_DEVICEGROUP@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --NetworkDeviceGroup
OR ( FEAT_CLASSNAME = 'EDGIS.VOLTAGEREGULATORUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('1','2')
AND (Select STATUS from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --Regulator
OR ( FEAT_CLASSNAME = 'EDGIS.VOLTAGEREGULATORUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('3')
AND (Select STATUS from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = (select REGULATORGUID from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --Booster
OR ( FEAT_CLASSNAME = 'EDGIS.VOLTAGEREGULATOR' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1','2')) 
AND (select Count(*) from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where REGULATORGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Regulator -- ASSETID will be from 'EDGIS.VOLTAGEREGULATORUNIT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.VOLTAGEREGULATOR' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('3')) 
AND (select Count(*) from EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT@EDER where REGULATORGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_VOLTAGEREGULATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Booster -- ASSETID will be from 'EDGIS.VOLTAGEREGULATORUNIT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.CAPACITORBANK'
AND (Select STATUS from EDGIS.ZZ_MV_CAPACITORBANK@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CAPACITORBANK@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --CapacitorBank
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('2')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('3')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('4')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('5')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.CONTROLLER' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('6')
AND (Select STATUS from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_CONTROLLER@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --Controller
OR ( FEAT_CLASSNAME = 'EDGIS.DYNAMICPROTECTIVEDEVICE' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('2')
AND (Select STATUS from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Interrupter
OR ( FEAT_CLASSNAME = 'EDGIS.DYNAMICPROTECTIVEDEVICE' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('3')
AND (Select STATUS from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Recloser
OR ( FEAT_CLASSNAME = 'EDGIS.DYNAMICPROTECTIVEDEVICE' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('8')
AND (Select STATUS from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Sectionalizer
OR ( FEAT_CLASSNAME = 'EDGIS.SWITCH' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1','2','3','4','5','6')
AND (Select STATUS from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Switch
OR ( FEAT_CLASSNAME = 'EDGIS.SWITCH' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('7')
AND (Select STATUS from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --GroundSwitch
OR ( FEAT_CLASSNAME = 'EDGIS.SWITCH' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1','2','3','4','5','6')
AND (Select STATUS from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_SWITCH@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --NetSwitch
OR ( FEAT_CLASSNAME = 'EDGIS.STEPDOWNUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = (select STEPDOWNGUID from EDGIS.ZZ_MV_STEPDOWNUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('1')
AND (Select STATUS from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = (select STEPDOWNGUID from EDGIS.ZZ_MV_STEPDOWNUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = (select STEPDOWNGUID from EDGIS.ZZ_MV_STEPDOWNUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --StepDown
--OR ( FEAT_CLASSNAME = 'EDGIS.STEPDOWN' AND (Select SUBTYPECD from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = FEAT_GLOBALID) IN ('1')) --StepDown (Modified Below)
OR ( FEAT_CLASSNAME = 'EDGIS.STEPDOWN' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1')) 
AND (select Count(*) from EDGIS.ZZ_MV_STEPDOWNUNIT@EDER where STEPDOWNGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_STEPDOWN@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --StepDown
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMERUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('1','2','3','4','8')
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --Transformer 
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMERUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('7')
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --Transformer
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMERUNIT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) IN ('5')
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = (select TRANSFORMERGUID from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where GLOBALID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID)) <> 'Y'
) --NetworkTransformer
--OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID) IN ('1','2','3','4','8')) --Transformer --ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table  (Modified Below)
--OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID) IN ('7')) --Transformer ----ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table (Modified Below)
--OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID) IN ('5')) --NetworkTransformer ----ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table (Modified Below)
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1','2','3','4','8')) 
AND (select Count(*) from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where TRANSFORMERGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Transformer --ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('7')) 
AND (select Count(*) from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where TRANSFORMERGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Transformer ----ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMER' 
AND (((Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('5')) 
AND (select Count(*) from EDGIS.ZZ_MV_TRANSFORMERUNIT@EDER where TRANSFORMERGUID = FEAT_GLOBALID  AND OBJECTID = FEAT_OID ) > 0) 
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMER@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --NetworkTransformer ----ASSETID will be from 'EDGIS.TRANSFORMERUNIT in ED06 STAGING Table
OR ( FEAT_CLASSNAME = 'EDGIS.STREETLIGHT'
AND (Select STATUS from EDGIS.ZZ_MV_STREETLIGHT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_STREETLIGHT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --StreetLight
OR ( FEAT_CLASSNAME = 'EDGIS.FAULTINDICATOR'
AND (Select STATUS from EDGIS.ZZ_MV_FAULTINDICATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_FAULTINDICATOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --FaultIndicator
OR ( FEAT_CLASSNAME = 'EDGIS.OPENPOINT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1','2')
AND (Select STATUS from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --Elbow
OR ( FEAT_CLASSNAME = 'EDGIS.OPENPOINT' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('2')
AND (Select STATUS from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_OPENPOINT@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> 'Y'
) --NetJunc
OR ( FEAT_CLASSNAME = 'EDGIS.TRANSFORMERDEVICE' 
AND (Select SUBTYPECD from EDGIS.ZZ_MV_TRANSFORMERDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) IN ('1')
AND (Select STATUS from EDGIS.ZZ_MV_TRANSFORMERDEVICE@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_TRANSFORMERDEVICE@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --PrimaryConnectionChamber
OR ( FEAT_CLASSNAME = 'EDGIS.NETWORKPROTECTOR' 
AND (Select STATUS from EDGIS.ZZ_MV_NETWORKPROTECTOR@EDER where GLOBALID  = FEAT_GLOBALID  AND OBJECTID = FEAT_OID) <> '0'
--AND (Select CUSTOMEROWNED from EDGIS.ZZ_MV_NETWORKPROTECTOR@EDER where GLOBALID  = FEAT_GLOBALID) <> 'Y'
) --NetworkProtector
)
AND FEAT_GLOBALID not in 
(Select Assetid from INTDATAARCH.PGE_GISSAP_ASSETSYNCH where 
DATEPROCESSED >= sysdate - startdate)
--AND DATEPROCESSED <= sysdate-2)
 AND FEAT_CLASSNAME not in ('EDGIS.TRANSFORMER','EDGIS.CIRCUITSOURCE','EDGIS.VOLTAGEREGULATOR','EDGIS.STEPDOWN'));

 COMMIT;
 end;