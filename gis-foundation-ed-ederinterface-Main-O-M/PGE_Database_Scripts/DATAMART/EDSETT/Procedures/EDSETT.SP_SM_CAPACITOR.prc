Prompt drop Procedure SP_SM_CAPACITOR;
DROP PROCEDURE EDSETT.SP_SM_CAPACITOR
/

Prompt Procedure SP_SM_CAPACITOR;
--
-- SP_SM_CAPACITOR  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDSETT."SP_SM_CAPACITOR"
AS
  GD_COUNT NUMBER;
  SEC_SETT NUMBER;
  SM_SEC   NUMBER;
BEGIN
  INSERT
  INTO SM_CAPACITOR
    (
      GLOBAL_ID,
      FEATURE_CLASS_NAME,
      OPERATING_NUM,
      DIVISION,
      DISTRICT,
      CONTROLLER_UNIT_MODEL,
      CONTROL_TYPE,
      CONTROL_SERIAL_NUM,
      DEVICE_ID,
      PREPARED_BY,
      DATE_MODIFIED,
      EFFECTIVE_DT,
      CURRENT_FUTURE,
      SWITCH_POSITION,
      PREFERED_BANK_POSITION,
      MAXCYCLES,
      DAYLIGHT_SAVINGS_TIME,
      EST_VOLTAGE_CHANGE,
      VOLTAGE_OVERRIDE_TIME,
      HIGH_VOLTAGE_OVERRIDE_SETPOINT,
      LOW_VOLTAGE_OVERRIDE_SETPOINT,
      VOLTAGE_CHANGE_TIME,
      TEMPERATURE_OVERRIDE,
      TEMPERATURE_CHANGE_TIME,
      EST_BANK_VOLTAGE_RISE,
      AUTO_BVR_CALC,
      DATA_LOGGING_INTERVAL,
      PULSE_TIME,
      MIN_SW_VOLTAGE,
      TIME_DELAY,
      SCH1_SCHEDULE,
      SCH1_CONTROL_STRATEGY,
      SCH1_TIME_ON,
      SCH1_TIME_OFF,
      SCH1_LOW_VOLTAGE_SETPOINT,
      SCH1_HIGH_VOLTAGE_SETPOINT,
      SCH1_TEMP_SETPOINT_ON,
      SCH1_TEMP_SETPOINT_OFF,
      SCH1_START_DATE,
      SCH1_END_DATE,
      SCH1_WEEKDAYS,
      SCH1_SATURDAY,
      SCH1_SUNDAY,
      SCH1_HOLIDAYS
    )
  SELECT GD.GLOBAL_ID,
   'EDGIS.CapacitorBank',
    GD.OPERATING_NUM,
    GD.DIVISION,
    GD.DISTRICT,
    CC.CONTROL_TYPE,
    CC.CONTROL_TYPE,
    CC.CONTROL_SERIAL_#,
    CS.DEVICE_ID,
    CS.PREPARED_BY,
    CS.LAST_MODIFIED,
    CS.EFFECTIVE_DATE,
    CS.CURRENT_FUTURE,
    CS.SWITCH_POSITION,
    CS.PREFERED_BANK_POSITION,
    CS.MAXCYCLES,
    CS.DAYLIGHT_SAVINGS_TIME,
    CS.EST_VOLTAGE_CHANGE,
    CS.VOLTAGE_OVERRIDE_TIME,
    CS.HIGH_VOLTAGE_OVERRIDE_SETPOINT,
    CS.LOW_VOLTAGE_OVERRIDE_SETPOINT,
    CS.VOLTAGE_CHANGE_TIME,
    CS.TEMPERATURE_OVERRIDE,
    CS.TEMPERATURE_CHANGE_TIME,
    CS.EST_BANK_VOLTAGE_RISE,
    CS.AUTO_BVR_CALC,
    CS.DATA_LOGGING_INTERVAL,
    CS.PULSE_TIME,
    CS.MIN_SW_VOLTAGE,
    CS.TIME_DELAY,
    CSH.SCHEDULE,
    CSH.CONTROL_STRATEGY,
    CSH.TIME_ON,
    CSH.TIME_OFF,
    CSH.LOW_VOLTAGE_SETPOINT,
    CSH.HIGH_VOLTAGE_SETPOINT,
    CSH.TEMP_SETPOINT_ON,
    CSH.TEMP_SETPOINT_OFF,
    CSH.START_DATE,
    CSH.END_DATE,
    CSH.WEEKDAYS,
    CSH.SATURDAY,
    CSH.SUNDAY,
    CSH.HOLIDAYS
  FROM CEDSA_CAPACITOR CC ,
    CEDSA_CAPACITOR_SETTINGS CS ,
    CEDSA_CAPACITOR_SCHEDULES CSH,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =CSH.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND CSH.SCHEDULE          =1;




INSERT
  INTO SM_CAPACITOR
    (
      GLOBAL_ID,
      FEATURE_CLASS_NAME,
      OPERATING_NUM,
      DIVISION,
      DISTRICT,
      CONTROLLER_UNIT_MODEL,
      CONTROL_TYPE,
      CONTROL_SERIAL_NUM,
      DEVICE_ID,
      PREPARED_BY,
      DATE_MODIFIED,
      EFFECTIVE_DT,
      CURRENT_FUTURE,
      SWITCH_POSITION,
      PREFERED_BANK_POSITION,
      MAXCYCLES,
      DAYLIGHT_SAVINGS_TIME,
      EST_VOLTAGE_CHANGE,
      VOLTAGE_OVERRIDE_TIME,
      HIGH_VOLTAGE_OVERRIDE_SETPOINT,
      LOW_VOLTAGE_OVERRIDE_SETPOINT,
      VOLTAGE_CHANGE_TIME,
      TEMPERATURE_OVERRIDE,
      TEMPERATURE_CHANGE_TIME,
      EST_BANK_VOLTAGE_RISE,
      AUTO_BVR_CALC,
      DATA_LOGGING_INTERVAL,
      PULSE_TIME,
      MIN_SW_VOLTAGE,
      TIME_DELAY
    )
SELECT GD.GLOBAL_ID,
   'EDGIS.CapacitorBank',
    GD.OPERATING_NUM,
    GD.DIVISION,
    GD.DISTRICT,
    CC.CONTROL_TYPE,
    CC.CONTROL_TYPE,
    CC.CONTROL_SERIAL_#,
    CS.DEVICE_ID,
    CS.PREPARED_BY,
    CS.LAST_MODIFIED,
    CS.EFFECTIVE_DATE,
    CS.CURRENT_FUTURE,
    CS.SWITCH_POSITION,
    CS.PREFERED_BANK_POSITION,
    CS.MAXCYCLES,
    CS.DAYLIGHT_SAVINGS_TIME,
    CS.EST_VOLTAGE_CHANGE,
    CS.VOLTAGE_OVERRIDE_TIME,
    CS.HIGH_VOLTAGE_OVERRIDE_SETPOINT,
    CS.LOW_VOLTAGE_OVERRIDE_SETPOINT,
    CS.VOLTAGE_CHANGE_TIME,
    CS.TEMPERATURE_OVERRIDE,
    CS.TEMPERATURE_CHANGE_TIME,
    CS.EST_BANK_VOLTAGE_RISE,
    CS.AUTO_BVR_CALC,
    CS.DATA_LOGGING_INTERVAL,
    CS.PULSE_TIME,
    CS.MIN_SW_VOLTAGE,
    CS.TIME_DELAY
  FROM CEDSA_CAPACITOR CC ,
    CEDSA_CAPACITOR_SETTINGS CS ,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GLOBAL_ID NOT IN ( SELECT GLOBAL_ID  FROM SM_CAPACITOR);



  BEGIN
    FOR I IN
    (SELECT CS.DEVICE_ID,CS.CURRENT_FUTURE,
      CSH.SCHEDULE,
      CSH.CONTROL_STRATEGY,
      CSH.TIME_ON,
      CSH.TIME_OFF,
      CSH.LOW_VOLTAGE_SETPOINT,
      CSH.HIGH_VOLTAGE_SETPOINT,
      CSH.TEMP_SETPOINT_ON,
      CSH.TEMP_SETPOINT_OFF,
      CSH.START_DATE,
      CSH.END_DATE,
      CSH.WEEKDAYS,
      CSH.SATURDAY,
      CSH.SUNDAY,
      CSH.HOLIDAYS
    FROM CEDSA_CAPACITOR CC ,
      CEDSA_CAPACITOR_SETTINGS CS ,
      CEDSA_CAPACITOR_SCHEDULES CSH,
      GIS_CEDSADEVICEID GD
    WHERE CS.DEVICE_ID        =CC.DEVICE_ID
    AND CS.DEVICE_ID          =CSH.DEVICE_ID
    AND CS.DEVICE_ID          =GD.DEVICE_ID
    AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
    AND GD.FEATURE_CLASS_NAME ='Capacitor'
    AND CSH.SCHEDULE          =2
    )
    LOOP
      UPDATE SM_CAPACITOR
      SET SCH2_SCHEDULE           = I.SCHEDULE,
        SCH2_CONTROL_STRATEGY     =I.CONTROL_STRATEGY,
        SCH2_TIME_ON              =I.TIME_ON,
        SCH2_TIME_OFF             =I.TIME_OFF,
        SCH2_LOW_VOLTAGE_SETPOINT =I.LOW_VOLTAGE_SETPOINT,
        SCH2_HIGH_VOLTAGE_SETPOINT=I.HIGH_VOLTAGE_SETPOINT,
        SCH2_TEMP_SETPOINT_ON     =I.TEMP_SETPOINT_ON,
        SCH2_TEMP_SETPOINT_OFF    =I.TEMP_SETPOINT_OFF,
        SCH2_START_DATE           =I.START_DATE,
        SCH2_END_DATE             =I.END_DATE,
        SCH2_WEEKDAYS             =I.WEEKDAYS,
        SCH2_SATURDAY             =I.SATURDAY,
        SCH2_SUNDAY               =I.SUNDAY,
        SCH2_HOLIDAYS             =I.HOLIDAYS
      WHERE DEVICE_ID             =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;
    END LOOP;
  END;
  BEGIN
    FOR I IN
    (SELECT CS.DEVICE_ID,CS.CURRENT_FUTURE,
      CSH.SCHEDULE,
      CSH.CONTROL_STRATEGY,
      CSH.TIME_ON,
      CSH.TIME_OFF,
      CSH.LOW_VOLTAGE_SETPOINT,
      CSH.HIGH_VOLTAGE_SETPOINT,
      CSH.TEMP_SETPOINT_ON,
      CSH.TEMP_SETPOINT_OFF,
      CSH.START_DATE,
      CSH.END_DATE,
      CSH.WEEKDAYS,
      CSH.SATURDAY,
      CSH.SUNDAY,
      CSH.HOLIDAYS
    FROM CEDSA_CAPACITOR CC ,
      CEDSA_CAPACITOR_SETTINGS CS ,
      CEDSA_CAPACITOR_SCHEDULES CSH,
      GIS_CEDSADEVICEID GD
    WHERE CS.DEVICE_ID        =CC.DEVICE_ID
    AND CS.DEVICE_ID          =CSH.DEVICE_ID
    AND CS.DEVICE_ID          =GD.DEVICE_ID
    AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
    AND GD.FEATURE_CLASS_NAME ='Capacitor'
    AND CSH.SCHEDULE          =3
    )
    LOOP
      UPDATE SM_CAPACITOR
      SET SCH3_SCHEDULE           = I.SCHEDULE,
        SCH3_CONTROL_STRATEGY     =I.CONTROL_STRATEGY,
        SCH3_TIME_ON              =I.TIME_ON,
        SCH3_TIME_OFF             =I.TIME_OFF,
        SCH3_LOW_VOLTAGE_SETPOINT =I.LOW_VOLTAGE_SETPOINT,
        SCH3_HIGH_VOLTAGE_SETPOINT=I.HIGH_VOLTAGE_SETPOINT,
        SCH3_TEMP_SETPOINT_ON     =I.TEMP_SETPOINT_ON,
        SCH3_TEMP_SETPOINT_OFF    =I.TEMP_SETPOINT_OFF,
        SCH3_START_DATE           =I.START_DATE,
        SCH3_END_DATE             =I.END_DATE,
        SCH3_WEEKDAYS             =I.WEEKDAYS,
        SCH3_SATURDAY             =I.SATURDAY,
        SCH3_SUNDAY               =I.SUNDAY,
        SCH3_HOLIDAYS             =I.HOLIDAYS
      WHERE DEVICE_ID             =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;
    END LOOP;
  END;
  BEGIN
    FOR I IN
    (SELECT CS.DEVICE_ID,CS.CURRENT_FUTURE,
      CSH.SCHEDULE,
      CSH.CONTROL_STRATEGY,
      CSH.TIME_ON,
      CSH.TIME_OFF,
      CSH.LOW_VOLTAGE_SETPOINT,
      CSH.HIGH_VOLTAGE_SETPOINT,
      CSH.TEMP_SETPOINT_ON,
      CSH.TEMP_SETPOINT_OFF,
      CSH.START_DATE,
      CSH.END_DATE,
      CSH.WEEKDAYS,
      CSH.SATURDAY,
      CSH.SUNDAY,
      CSH.HOLIDAYS
    FROM CEDSA_CAPACITOR CC ,
      CEDSA_CAPACITOR_SETTINGS CS ,
      CEDSA_CAPACITOR_SCHEDULES CSH,
      GIS_CEDSADEVICEID GD
    WHERE CS.DEVICE_ID        =CC.DEVICE_ID
    AND CS.DEVICE_ID          =CSH.DEVICE_ID
    AND CS.DEVICE_ID          =GD.DEVICE_ID
    AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
    AND GD.FEATURE_CLASS_NAME ='Capacitor'
    AND CSH.SCHEDULE          =4
    )
    LOOP
      UPDATE SM_CAPACITOR
      SET SCH4_SCHEDULE           = I.SCHEDULE,
        SCH4_CONTROL_STRATEGY     =I.CONTROL_STRATEGY,
        SCH4_TIME_ON              =I.TIME_ON,
        SCH4_TIME_OFF             =I.TIME_OFF,
        SCH4_LOW_VOLTAGE_SETPOINT =I.LOW_VOLTAGE_SETPOINT,
        SCH4_HIGH_VOLTAGE_SETPOINT=I.HIGH_VOLTAGE_SETPOINT,
        SCH4_TEMP_SETPOINT_ON     =I.TEMP_SETPOINT_ON,
        SCH4_TEMP_SETPOINT_OFF    =I.TEMP_SETPOINT_OFF,
        SCH4_START_DATE           =I.START_DATE,
        SCH4_END_DATE             =I.END_DATE,
        SCH4_WEEKDAYS             =I.WEEKDAYS,
        SCH4_SATURDAY             =I.SATURDAY,
        SCH4_SUNDAY               =I.SUNDAY,
        SCH4_HOLIDAYS             =I.HOLIDAYS
      WHERE DEVICE_ID             =I.DEVICE_ID AND CURRENT_FUTURE=I.CURRENT_FUTURE;
    END LOOP;
  END;
  COMMIT;
  UPDATE SM_CAPACITOR
  SET SCADA        ='Y'
  WHERE DEVICE_ID IN
    ( SELECT DEVICE_ID FROM CEDSA_SCADA SC
    ) ;
  UPDATE SM_CAPACITOR
  SET SCADA            ='N'
  WHERE DEVICE_ID NOT IN
    ( SELECT DEVICE_ID FROM CEDSA_SCADA SC
    );

  UPDATE SM_CAPACITOR
  SET SCADA        ='Y'
  WHERE DEVICE_ID IN
    ( SELECT DEVICE_ID FROM CEDSA_SCADA SC
    ) ;
  UPDATE SM_CAPACITOR
  SET SCADA            ='N'
  WHERE DEVICE_ID NOT IN
    ( SELECT DEVICE_ID FROM CEDSA_SCADA SC
    );
  BEGIN
    FOR I IN
    (SELECT CD.DEVICE_ID,
      CD.SCADA_TYPE,
      CD.RADIO_MANF_CD,
      CD.RADIO_MODEL_#,
      CD.RADIO_SERIAL_#
    FROM CEDSA_SCADA CD ,
      SM_CAPACITOR RE
    WHERE CD.DEVICE_ID = RE.DEVICE_ID
    )
    LOOP
      UPDATE SM_CAPACITOR
      SET SCADA_TYPE    =I.SCADA_TYPE,
        RADIO_MANF_CD   =I.RADIO_MANF_CD,
        RADIO_MODEL_NUM =I.RADIO_MODEL_#,
        RADIO_SERIAL_NUM=I.RADIO_SERIAL_#
      WHERE DEVICE_ID   =I.DEVICE_ID;
    END LOOP;
  END;
  INSERT
  INTO SM_COMMENT_HIST
    (
      GLOBAL_ID,
      WORK_DATE,
      WORK_TYPE,
      PERFORMED_BY,
      ENTRY_DATE,
      COMMENTS
    )
  SELECT GD.GLOBAL_ID,
    CH.WORK_DATE,
    CH.WORK_TYPE,
    CH.PERFORMED_BY,
    CH.ENTRY_DATE,
    CH.COMMENTS
  FROM GIS_CEDSADEVICEID GD,
    CEDSA_CAPACITOR_HIST CH
  WHERE CH.DEVICE_ID=GD.DEVICE_ID;
  COMMIT;
  SELECT COUNT(*)
  INTO GD_COUNT
  FROM GIS_CEDSADEVICEID GD
  WHERE GD.FEATURE_CLASS_NAME ='Capacitor';
  SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_CAPACITOR_SETTINGS;
  SELECT COUNT(*) INTO SM_SEC FROM SM_CAPACITOR;
  DBMS_OUTPUT.PUT_LINE('Count of Capacitor from GIS_CEDSADEVICEID : '|| GD_COUNT );
  DBMS_OUTPUT.PUT_LINE('Count of Capacitor from CEDSA_SECTIONALIZER_SETTINGS : '|| SEC_SETT);
  DBMS_OUTPUT.PUT_LINE('Count of Capacitor from SM_CAPACITOR : '|| SM_SEC);


  BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubCapacitorBank'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_CAPACITOR WHERE SM_CAPACITOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_CAPACITOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.SubCapacitorBank', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_CAPACITOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.SubCapacitorBank', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

END LOOP;
END;

BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.CapacitorBank'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_CAPACITOR WHERE SM_CAPACITOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_CAPACITOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.CapacitorBank', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_CAPACITOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.CapacitorBank', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

END LOOP;
END;

UPDATE SM_CAPACITOR SET CONTROL_TYPE='UNSP' WHERE CONTROL_TYPE IS NULL;

COMMIT;



SELECT COUNT(*) INTO SM_SEC   FROM SM_CAPACITOR ;
DBMS_OUTPUT.PUT_LINE('Count of CAPACITOR from SM_CAPACITOR after inserting default C/F records: '|| SM_SEC);


END SP_SM_CAPACITOR ;

/
