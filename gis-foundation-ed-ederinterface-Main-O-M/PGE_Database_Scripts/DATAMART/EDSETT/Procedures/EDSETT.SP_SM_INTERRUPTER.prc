Prompt drop Procedure SP_SM_INTERRUPTER;
DROP PROCEDURE EDSETT.SP_SM_INTERRUPTER
/

Prompt Procedure SP_SM_INTERRUPTER;
--
-- SP_SM_INTERRUPTER  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_INTERRUPTER"
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN

INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID ,OK_TO_BYPASS ,CONTROL_SERIAL_NUM ,
MANF_CD ,CONTROL_TYPE ,FIRMWARE_VERSION ,SOFTWARE_VERSION,CURRENT_FUTURE,GRD_CUR_TRIP,GRD_TRIP_CD,TYP_CRV_GRD,PHA_CUR_TRIP,PHA_TRIP_CD,TYP_CRV_PHA,PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT)
SELECT GD.GLOBAL_ID,'EDGIS.DynamicProtectiveDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,CI.DEVICE_ID ,CI.OK_TO_BYPASS, CI.CONTROL_SERIAL_# ,
CI.MANF_CD ,CI.CONTROL_TYPE ,CI.FIRMWARE_VERSION ,CI.SOFTWARE_VERSION ,CS.CURRENT_FUTURE,CS.GRD_CUR_TRIP,CS.GRD_TRIP_CD,CS.TYP_CRV_GRD,
CS.PHA_CUR_TRIP, CS.PHA_TRIP_CD, CS.TYP_CRV_PHA,CS.PREPARED_BY,CS.LAST_MODIFIED,CS.EFFECTIVE_DATE
FROM  CEDSA_INTERRUPTER CI,CEDSA_INTERRUPTER_SETTINGS CS,GIS_CEDSADEVICEID GD
WHERE CI.DEVICE_ID=CS.DEVICE_ID AND CI.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter';


COMMIT;

UPDATE  SM_INTERRUPTER
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_INTERRUPTER
SET SCADA='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC);

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_INTERRUPTER RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP
UPDATE SM_INTERRUPTER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;

INSERT INTO SM_COMMENT_HIST ( GLOBAL_ID,WORK_DATE,WORK_TYPE,PERFORMED_BY,ENTRY_DATE,COMMENTS)
SELECT  GD.GLOBAL_ID,IH.WORK_DATE,IH.WORK_TYPE,IH.PERFORMED_BY,IH.ENTRY_DATE,IH.COMMENTS
FROM GIS_CEDSADEVICEID  GD,CEDSA_INTERRUPTER_HIST IH  WHERE IH.DEVICE_ID=GD.DEVICE_ID;

COMMIT;

SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Interrupter';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_INTERRUPTER_SETTINGS;
SELECT COUNT(*) INTO SM_SEC   FROM SM_INTERRUPTER;

DBMS_OUTPUT.PUT_LINE('Count of Interrupter from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of Interrupter from CEDSA_SECTIONALIZER_SETTINGS : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of Interrupter from SM_INTERRUPTER : '|| SM_SEC);


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME,GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubInterruptingDevice' and GD.FEATURE_CLASS_NAME='Interrupter'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_INTERRUPTER WHERE SM_INTERRUPTER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME,GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.DynamicProtectiveDevice' and GD.FEATURE_CLASS_NAME='Interrupter'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_INTERRUPTER WHERE SM_INTERRUPTER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;







SELECT COUNT(*) INTO SM_SEC   FROM SM_INTERRUPTER ;
DBMS_OUTPUT.PUT_LINE('Count of Interrupter from SM_INTERRUPTER after inserting default C/F records: '|| SM_SEC);


END SP_SM_INTERRUPTER ;

/
