Prompt drop Procedure SP_SM_REGULATOR;
DROP PROCEDURE EDSETT.SP_SM_REGULATOR
/

Prompt Procedure SP_SM_REGULATOR;
--
-- SP_SM_REGULATOR  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_REGULATOR"
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN

INSERT INTO SM_REGULATOR (GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,
DEVICE_ID,PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,CURRENT_FUTURE,PRIMARY_CT_RATING,BAND_WIDTH,PT_RATIO,RANGE_UNBLOCKED,
BLOCKED_PCT,STEPS,PEAK_LOAD,MIN_LOAD,PVD_MAX,PVD_MIN,SVD_MIN,POWER_FACTOR,LOAD_CYCLE,RISE_RATING,TIMER,
CONTROL_TYPE,FIRMWARE_VERSION,SOFTWARE_VERSION,BANK_CD)
SELECT GD.GLOBAL_ID,'EDGIS.VoltageRegulatorUnit',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
RE.DEVICE_ID,RE.PREPARED_BY,RE.LAST_MODIFIED,RE.EFFECTIVE_DATE,RE.CURRENT_FUTURE,RE.PRIMARY_CT_RATING,RE.BAND_WIDTH,
RE.PT_RATIO,RE.RANGE_UNBLOCKED,RE.BLOCKED_PCT,RE.STEPS,RE.PEAK_LOAD,RE.MIN_LOAD,RE.PVD_MAX,RE.PVD_MIN,RE.SVD_MIN,RE.POWER_FACTOR,
RE.LOAD_CYCLE,RE.RISE_RATING,RE.TIMER,
RB.CONTROL_TYPE,RB.FIRMWARE_VERSION,RB.SOFTWARE_VERSION,RB.BANK_CD
FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD
WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.DEVICE_ID=GD.DEVICE_ID
AND GD.BANKCODE = RB.BANK_CD
AND GD.FEATURE_CLASS_NAME ='Regulator';


COMMIT;

UPDATE  SM_REGULATOR
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_REGULATOR
SET SCADA='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC);

BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_REGULATOR RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP
UPDATE SM_REGULATOR  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#
WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;

INSERT INTO SM_COMMENT_HIST ( GLOBAL_ID,WORK_DATE,WORK_TYPE,PERFORMED_BY,ENTRY_DATE,COMMENTS)
SELECT  GD.GLOBAL_ID,RH.WORK_DATE,RH.WORK_TYPE,RH.PERFORMED_BY,RH.ENTRY_DATE,RH.COMMENTS
FROM GIS_CEDSADEVICEID  GD,CEDSA_REGULATOR_HIST RH  WHERE RH.DEVICE_ID=GD.DEVICE_ID;


COMMIT;

SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Regulator';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_REGULATOR_SETTINGS;
SELECT COUNT(*) INTO SM_SEC   FROM SM_REGULATOR;

DBMS_OUTPUT.PUT_LINE('Count of REGULATOR from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of REGULATOR from CEDSA_REGULATOR_SETTINGS : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of REGULATOR from SM_REGULATOR : '|| SM_SEC);


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME,GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SUBTransformerBank'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_REGULATOR WHERE SM_REGULATOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME,GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.VoltageRegulatorUnit'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_REGULATOR WHERE SM_REGULATOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;

BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME,GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubVoltageRegulatorUnit'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_REGULATOR WHERE SM_REGULATOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_REGULATOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;

-- This code is to remove the Bank Code suffix from the operating numbers
DELETE FROM GIS_CEDSADEVICEID_VR;

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code 1')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code 1%'  and current_future='F'
);

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code 2')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code 2%'  and current_future='F'
);

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code 3')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code 3%'  and current_future='F'
);

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code ')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code '  and current_future='F'
);


-- This code is to remove the Bank Code suffix from the operating numbers  where Bank Code (two spaces) values
INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code  1')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code  1%'  and current_future='F'
);

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code  2')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code  2%'  and current_future='F'
);

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,OPERATING_NUM)
(
   SELECT GLOBAL_ID,FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,replace(operating_num, '- Bank Code  3')
   FROM SM_REGULATOR
   WHERE  operating_num like '%- Bank Code  3%'  and current_future='F'
);






UPDATE    (
SELECT SR.operating_num old_opr_num, VR.operating_num new_opr_num
FROM SM_REGULATOR  SR, GIS_CEDSADEVICEID_VR VR
WHERE SR.global_id=VR.global_id )
SET old_opr_num=new_opr_num;

COMMIT;



SELECT COUNT(*) INTO SM_SEC   FROM SM_REGULATOR ;
DBMS_OUTPUT.PUT_LINE('Count of REGULATOR from SM_REGULATOR after inserting default C/F records: '|| SM_SEC);





END SP_SM_REGULATOR ;

/
