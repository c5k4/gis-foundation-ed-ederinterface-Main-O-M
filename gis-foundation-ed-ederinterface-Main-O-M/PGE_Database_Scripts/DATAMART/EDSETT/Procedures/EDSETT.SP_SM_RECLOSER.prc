Prompt drop Procedure SP_SM_RECLOSER;
DROP PROCEDURE EDSETT.SP_SM_RECLOSER
/

Prompt Procedure SP_SM_RECLOSER;
--
-- SP_SM_RECLOSER  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_RECLOSER"
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN

INSERT INTO SM_RECLOSER ( GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID,CONTROL_SERIAL_NUM,CONTROL_TYPE,BYPASS_PLANS,OPERATING_AS_CD,
CURRENT_FUTURE,GRD_MIN_TRIP,PHA_MIN_TRIP,GRD_INST_TRIP_CD,PHA_INST_TRIP_CD,GRD_OP_F_CRV,PHA_OP_F_CRV,GRD_RESP_TIME,PHA_RESP_TIME,GRD_FAST_CRV,PHA_FAST_CRV,
GRD_SLOW_CRV,PHA_SLOW_CRV,GRD_VMUL_FAST,PHA_VMUL_FAST,GRD_VMUL_SLOW,PHA_VMUL_SLOW,GRD_TADD_FAST,PHA_TADD_FAST,GRD_TADD_SLOW,PHA_TADD_SLOW,TOT_LOCKOUT_OPS,
RECLOSE1_TIME,RECLOSE2_TIME,RECLOSE3_TIME,RESET,SGF_CD,SGF_MIN_TRIP_PERCENT,SGF_TIME_DELAY,ALT_GRD_MIN_TRIP,ALT_PHA_MIN_TRIP,ALT_GRD_INST_TRIP_CD,ALT_PHA_INST_TRIP_CD,
ALT_GRD_OP_F_CRV,ALT_PHA_OP_F_CRV,ALT_GRD_RESP_TIME,ALT_PHA_RESP_TIME,ALT_GRD_FAST_CRV,ALT_PHA_FAST_CRV,ALT_GRD_SLOW_CRV,ALT_PHA_SLOW_CRV,ALT_GRD_VMUL_FAST,
ALT_PHA_VMUL_FAST,ALT_GRD_VMUL_SLOW,ALT_PHA_VMUL_SLOW,ALT_GRD_TADD_FAST,ALT_PHA_TADD_FAST,ALT_GRD_TADD_SLOW,ALT_PHA_TADD_SLOW,ALT_TOT_LOCKOUT_OPS,
ALT_RECLOSE1_TIME,ALT_RECLOSE2_TIME,ALT_RECLOSE3_TIME,ALT_RESET,ALT_SGF_CD,ALT_SGF_MIN_TRIP_PERCENT,ALT_SGF_TIME_DELAY,ALT2_PERMIT_LS_ENABLING,ALT2_GRD_ARMING_THRESHOLD,
ALT2_PHA_ARMING_THRESHOLD,ALT2_GRD_INRUSH_THRESHOLD,ALT2_PHA_INRUSH_THRESHOLD,ALT2_INRUSH_DURATION,ALT2_LS_LOCKOUT_OPS,ALT2_LS_RESET_TIME,ACTIVE_PROFILE,PERMIT_RB_CUTIN,DIRECT_TRANSFER_TRIP,BOC_VOLTAGE,RB_CUTOUT_TIME,DATE_MODIFIED)

SELECT  GD.GLOBAL_ID,'EDGIS.DynamicProtectiveDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,RE.DEVICE_ID,RE.CONTROL_SERIAL_#,
RE.CONTROL_TYPE,RE.BYPASS_PLANS,RE.OPERATING_AS_CD,RS.CURRENT_FUTURE,RS.GRD_MIN_TRIP,RS.PHA_MIN_TRIP,RS.GRD_INST_TRIP_CD,RS.PHA_INST_TRIP_CD,
RS.GRD_OP_F_CRV,RS.PHA_OP_F_CRV,DECODE(RS.GRD_RESP_TIME,'X',NULL, TO_NUMBER(RS.GRD_RESP_TIME)),DECODE(RS.PHA_RESP_TIME,'X',NULL,TO_NUMBER(RS.PHA_RESP_TIME)),RS.GRD_FAST_CRV,RS.PHA_FAST_CRV,RS.GRD_SLOW_CRV,RS.PHA_SLOW_CRV,RS.GRD_VMUL_FAST,RS.PHA_VMUL_FAST,
RS.GRD_VMUL_SLOW,RS.PHA_VMUL_SLOW,RS.GRD_TADD_FAST,RS.PHA_TADD_FAST,RS.GRD_TADD_SLOW,RS.PHA_TADD_SLOW,RS.TOT_LOCKOUT_OPS,RS.RECLOSE1_TIME,RS.RECLOSE2_TIME,
RS.RECLOSE3_TIME,RS.RESET,RS.SGF_CD,RS.SGF_MIN_TRIP_PERCENT,RS.SGF_TIME_DELAY,RS.ALT_GRD_MIN_TRIP,RS.ALT_PHA_MIN_TRIP,RS.ALT_GRD_INST_TRIP_CD,RS.ALT_PHA_INST_TRIP_CD,
RS.ALT_GRD_OP_F_CRV,RS.ALT_PHA_OP_F_CRV,DECODE(RS.ALT_GRD_RESP_TIME,'X',NULL,TO_NUMBER(RS.ALT_GRD_RESP_TIME)),DECODE(RS.ALT_PHA_RESP_TIME,'X',NULL,TO_NUMBER(RS.ALT_PHA_RESP_TIME)),RS.ALT_GRD_FAST_CRV,RS.ALT_PHA_FAST_CRV,RS.ALT_GRD_SLOW_CRV,RS.ALT_PHA_SLOW_CRV,
RS.ALT_GRD_VMUL_FAST,RS.ALT_PHA_VMUL_FAST,RS.ALT_GRD_VMUL_SLOW,RS.ALT_PHA_VMUL_SLOW,RS.ALT_GRD_TADD_FAST,RS.ALT_PHA_TADD_FAST,RS.ALT_GRD_TADD_SLOW,RS.ALT_PHA_TADD_SLOW,
RS.ALT_TOT_LOCKOUT_OPS,RS.ALT_RECLOSE1_TIME,RS.ALT_RECLOSE2_TIME,RS.ALT_RECLOSE3_TIME,RS.ALT_RESET,RS.ALT_SGF_CD,RS.ALT_SGF_MIN_TRIP_PERCENT,RS.ALT_SGF_TIME_DELAY,RS.PERMIT_LS_ENABLING,
RS.GRD_ARMING_THRESHOLD,RS.PHA_ARMING_THRESHOLD,RS.GRD_INRUSH_THRESHOLD,RS.PHA_INRUSH_THRESHOLD,RS.INRUSH_DURATION,RS.LS_LOCKOUT_OPS,RS.LS_RESET_TIME,RS.ACTIVE_PROFILE,
RS.PERMIT_RB_CUTIN,RS.DIRECT_TRANSFER_TRIP,RS.BOC_VOLTAGE,RS.RB_CUTOUT_TIME,RS.LAST_MODIFIED
FROM CEDSA_RECLOSER  RE ,CEDSA_RECLOSER_SETTINGS  RS,GIS_CEDSADEVICEID  GD   WHERE RE.DEVICE_ID=RS.DEVICE_ID AND RE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser';

COMMIT;

UPDATE  SM_RECLOSER
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_RECLOSER
SET SCADA='N'
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC);


BEGIN
FOR I IN (SELECT CD.DEVICE_ID,CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_RECLOSER RE  WHERE CD.DEVICE_ID = RE.DEVICE_ID )
LOOP
UPDATE SM_RECLOSER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#
 WHERE DEVICE_ID =I.DEVICE_ID;
END LOOP;
END;

INSERT INTO SM_COMMENT_HIST ( GLOBAL_ID,WORK_DATE,WORK_TYPE,PERFORMED_BY,ENTRY_DATE,COMMENTS)
SELECT  GD.GLOBAL_ID,RH.WORK_DATE,RH.WORK_TYPE,RH.PERFORMED_BY,RH.ENTRY_DATE,RH.COMMENTS
FROM GIS_CEDSADEVICEID  GD,CEDSA_RECLOSER_HIST RH  WHERE RH.DEVICE_ID=GD.DEVICE_ID;

COMMIT;

SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Recloser';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_RECLOSER_SETTINGS;
SELECT COUNT(*) INTO SM_SEC   FROM SM_RECLOSER;

DBMS_OUTPUT.PUT_LINE('Count of Reclosure from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of Reclosure from CEDSA_RECLOSER_SETTINGS : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of Reclosure from SM_RECLOSER : '|| SM_SEC);



BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME,GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.FEATURE_CLASS_NAME ='Recloser' AND GD.GIS_FEATURE_CLASS_NAME ='EDGIS.DynamicProtectiveDevice'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_RECLOSER WHERE SM_RECLOSER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_RECLOSER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_RECLOSER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

END LOOP;
END;


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME,GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.FEATURE_CLASS_NAME ='Recloser' AND GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubInterruptingDevice'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_RECLOSER WHERE SM_RECLOSER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_RECLOSER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_RECLOSER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

END LOOP;
END;


UPDATE SM_RECLOSER SET CONTROL_TYPE='3A' WHERE CONTROL_TYPE IS NULL;
COMMIT;

SELECT COUNT(*) INTO SM_SEC   FROM SM_RECLOSER ;
DBMS_OUTPUT.PUT_LINE('Count of Recloser from SM_RECLOSER after inserting default C/F records: '|| SM_SEC);


END SP_SM_RECLOSER ;

/
