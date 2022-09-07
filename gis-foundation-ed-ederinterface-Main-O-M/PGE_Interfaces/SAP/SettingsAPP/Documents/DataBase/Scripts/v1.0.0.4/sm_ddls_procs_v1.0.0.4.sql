
------------------v1.0.0.4----------------------------------

spool sm_ddls_procs_v1.0.0.4.log


--------------------------------------------------------
--  File created - Tuesday-August-05-2014   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package SM_CHANGE_DETECTION_PKG
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "EDSETT"."SM_CHANGE_DETECTION_PKG" AS 

  GLOBALID VARCHAR2(40);
  err_num    NUMBER;
  err_msg    VARCHAR2(100);
  REC_FOUND   EXCEPTION;
  CONFIG_ERR EXCEPTION;
  

PROCEDURE  SP_SM_DEVICE_DETECTION( 
    I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code  IN VARCHAR2,
    I_Switch_type_code   IN VARCHAR2);
    
    
    FUNCTION GET_GLOBALID RETURN VARCHAR2;
    
    
    PROCEDURE SP_SECTIONALIZER_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2);
    
    
    PROCEDURE SP_CAPACITOR_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    PROCEDURE SP_CIRCUIT_BREAKER_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    PROCEDURE SP_INTERRUPTER_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    PROCEDURE SP_NETWORK_PROTECTOR_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    PROCEDURE SP_RECLOSER_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    PROCEDURE SP_REGULATOR_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    PROCEDURE SP_SWITCH_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2);
    
    
    
 
END SM_CHANGE_DETECTION_PKG;

/
--------------------------------------------------------
--  DDL for Package SM_EXPOSE_DATA_PKG
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "EDSETT"."SM_EXPOSE_DATA_PKG" AS 

DATEFROM               DATE ;
DATETO                 DATE ;

PROCEDURE SP_SM_SET_DATE_PARAMS(
    I_From     IN DATE,
    I_To           IN DATE);
    
FUNCTION GET_DATEFROM
  RETURN DATE;

FUNCTION GET_DATETO
  RETURN DATE;

END SM_EXPOSE_DATA_PKG;

/

  GRANT EXECUTE ON "EDSETT"."SM_EXPOSE_DATA_PKG" TO "GIS_I";
--------------------------------------------------------
--  DDL for Package Body SM_CHANGE_DETECTION_PKG
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "EDSETT"."SM_CHANGE_DETECTION_PKG" 
AS
PROCEDURE SP_SM_DEVICE_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE := I_reason_type ;
  err_num     :=0;
  GLOBALID    := I_Global_id_Current;
  --Remove the existing current global id entries from error table before new ones
  DELETE
  FROM sm_errors
  WHERE GLOBAL_ID=I_Global_id_Current;
  COMMIT;
  
  
  
  SELECT COUNT(*)
  INTO NUM1
  FROM SM_FC_LAYER_MAPPING
  WHERE FEATURE_CLASS_NAME = I_feature_class_name
  AND SUBTYPE              = I_feature_class_subtype;
  
  IF NUM1 = 0 THEN
  RAISE CONFIG_ERR;
  ELSE  
  SELECT SM_TABLE
  INTO DEVICE_TYPE
  FROM SM_FC_LAYER_MAPPING
  WHERE FEATURE_CLASS_NAME = I_feature_class_name
  AND SUBTYPE              = I_feature_class_subtype;
  END IF;
  
  CASE
  WHEN DEVICE_TYPE = 'SM_SECTIONALIZER' THEN
    BEGIN
      SP_SECTIONALIZER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_RECLOSER' THEN
    BEGIN
      SP_RECLOSER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_SWITCH' THEN
    BEGIN
      SP_SWITCH_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_REGULATOR' THEN
    BEGIN
      SP_REGULATOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_INTERRUPTER' THEN
    BEGIN
      SP_INTERRUPTER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_CAPACITOR' THEN
    BEGIN
      SP_CAPACITOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_CIRCUIT_BREAKER' THEN
    BEGIN
      SP_CIRCUIT_BREAKER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_NETWORK_PROTECTOR' THEN
    BEGIN
      SP_NETWORK_PROTECTOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  END CASE;
EXCEPTION
WHEN NO_DATA_FOUND THEN
  err_num :=3;
  err_msg := 'Data not found in Settings';
  INSERT INTO sm_errors VALUES
    (I_Global_id_Current, err_num,err_msg
    );
WHEN REC_FOUND THEN
  err_num := 2;
  err_msg := 'Data already exists in Settings';
  INSERT INTO sm_errors VALUES
    (I_Global_id_Current,err_num,err_msg
    );
    
WHEN CONFIG_ERR THEN
  err_num := 4;
  err_msg := 'Missing feature class mapping configuration in Settings';
  INSERT INTO sm_errors VALUES
    (I_Global_id_Current,err_num,err_msg
    );
  
WHEN OTHERS THEN
  err_num := 99;
  err_msg := SUBSTR(SQLERRM, 1, 100);
  INSERT INTO sm_errors VALUES
    (I_Global_id_Current,err_num,err_msg
    );
END SP_SM_DEVICE_DETECTION;
FUNCTION GET_GLOBALID
  RETURN VARCHAR2
IS
BEGIN
  RETURN GLOBALID;
END;
PROCEDURE SP_SECTIONALIZER_DETECTION
  (
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2
  )
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  NUM1        NUMBER;
  VAR1        VARCHAR2(50);
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_SECTIONALIZER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_SECTIONALIZER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SECTIONALIZER',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_SECTIONALIZER_HIST
        (
          PEER_REVIEW_DT,
          EFFECTIVE_DT,
          TIMESTAMP,
          DATE_MODIFIED,
          PREPARED_BY,
          DEVICE_ID,
          OPERATING_NUM,
          GLOBAL_ID,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          ONE_SHOT_LOCKOUT_TIME,
          ONE_SHOT_LOCKOUT_NUM,
          LOCKOUT_NUM,
          VOLT_THRESHOLD,
          RESET,
          GRD_INRUSH_TIME,
          GRD_INRUSH_MULTIPLIER,
          GRD_INRUSH_DURATION,
          MIN_GRD_TO_CT,
          REQUIRED_FAULT_CURRENT,
          FIRST_RECLOSE_RESET_TIME,
          PHA_INRUSH_TIME,
          PHA_INRUSH_MULTIPLIER,
          PHA_INRUSH_DURATION,
          MIN_PC_TO_CT,
          SECT_TYPE,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          RELEASED_BY,
          PROCESSED_FLAG,
          CONTROL_SERIAL_NUM,
          CURRENT_FUTURE,
          NOTES,
          DISTRICT,
          DIVISION,
          PEER_REVIEW_BY,
          FEATURE_CLASS_NAME
        )
      SELECT PEER_REVIEW_DT,
        EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        GLOBAL_ID,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        ONE_SHOT_LOCKOUT_TIME,
        ONE_SHOT_LOCKOUT_NUM,
        LOCKOUT_NUM,
        VOLT_THRESHOLD,
        RESET,
        GRD_INRUSH_TIME,
        GRD_INRUSH_MULTIPLIER,
        GRD_INRUSH_DURATION,
        MIN_GRD_TO_CT,
        REQUIRED_FAULT_CURRENT,
        FIRST_RECLOSE_RESET_TIME,
        PHA_INRUSH_TIME,
        PHA_INRUSH_MULTIPLIER,
        PHA_INRUSH_DURATION,
        MIN_PC_TO_CT,
        SECT_TYPE,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        FEATURE_CLASS_NAME
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SECTIONALIZER_HIST',
          SM_SECTIONALIZER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_SECTIONALIZER
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District,
        CONTROL_TYPE    =I_Control_type_code
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_SECTIONALIZER_HIST
        (
          PEER_REVIEW_DT,
          EFFECTIVE_DT,
          TIMESTAMP,
          DATE_MODIFIED,
          PREPARED_BY,
          DEVICE_ID,
          OPERATING_NUM,
          GLOBAL_ID,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          ONE_SHOT_LOCKOUT_TIME,
          ONE_SHOT_LOCKOUT_NUM,
          LOCKOUT_NUM,
          VOLT_THRESHOLD,
          RESET,
          GRD_INRUSH_TIME,
          GRD_INRUSH_MULTIPLIER,
          GRD_INRUSH_DURATION,
          MIN_GRD_TO_CT,
          REQUIRED_FAULT_CURRENT,
          FIRST_RECLOSE_RESET_TIME,
          PHA_INRUSH_TIME,
          PHA_INRUSH_MULTIPLIER,
          PHA_INRUSH_DURATION,
          MIN_PC_TO_CT,
          SECT_TYPE,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          RELEASED_BY,
          PROCESSED_FLAG,
          CONTROL_SERIAL_NUM,
          CURRENT_FUTURE,
          NOTES,
          DISTRICT,
          DIVISION,
          PEER_REVIEW_BY,
          FEATURE_CLASS_NAME
        )
      SELECT PEER_REVIEW_DT,
        EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        GLOBAL_ID,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        ONE_SHOT_LOCKOUT_TIME,
        ONE_SHOT_LOCKOUT_NUM,
        LOCKOUT_NUM,
        VOLT_THRESHOLD,
        RESET,
        GRD_INRUSH_TIME,
        GRD_INRUSH_MULTIPLIER,
        GRD_INRUSH_DURATION,
        MIN_GRD_TO_CT,
        REQUIRED_FAULT_CURRENT,
        FIRST_RECLOSE_RESET_TIME,
        PHA_INRUSH_TIME,
        PHA_INRUSH_MULTIPLIER,
        PHA_INRUSH_DURATION,
        MIN_PC_TO_CT,
        SECT_TYPE,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        FEATURE_CLASS_NAME
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Current;
      DELETE FROM SM_SECTIONALIZER WHERE GLOBAL_ID = I_Global_id_Current;
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SECTIONALIZER_HIST',
          SM_SECTIONALIZER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_DATA_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_SECTIONALIZER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_SECTIONALIZER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SECTIONALIZER',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_SECTIONALIZER
      SET
        (
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          ONE_SHOT_LOCKOUT_TIME,
          ONE_SHOT_LOCKOUT_NUM,
          LOCKOUT_NUM,
          VOLT_THRESHOLD,
          RESET,
          GRD_INRUSH_TIME,
          GRD_INRUSH_MULTIPLIER,
          GRD_INRUSH_DURATION,
          MIN_GRD_TO_CT,
          REQUIRED_FAULT_CURRENT,
          FIRST_RECLOSE_RESET_TIME,
          PHA_INRUSH_TIME,
          PHA_INRUSH_MULTIPLIER,
          PHA_INRUSH_DURATION,
          MIN_PC_TO_CT,
          SECT_TYPE,
          CONTROL_TYPE,
          OK_TO_BYPASS
        )
        =
        (SELECT RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          ONE_SHOT_LOCKOUT_TIME,
          ONE_SHOT_LOCKOUT_NUM,
          LOCKOUT_NUM,
          VOLT_THRESHOLD,
          RESET,
          GRD_INRUSH_TIME,
          GRD_INRUSH_MULTIPLIER,
          GRD_INRUSH_DURATION,
          MIN_GRD_TO_CT,
          REQUIRED_FAULT_CURRENT,
          FIRST_RECLOSE_RESET_TIME,
          PHA_INRUSH_TIME,
          PHA_INRUSH_MULTIPLIER,
          PHA_INRUSH_DURATION,
          MIN_PC_TO_CT,
          SECT_TYPE,
          CONTROL_TYPE,
          OK_TO_BYPASS
        FROM SM_SECTIONALIZER
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_SECTIONALIZER_HIST
        (
          PEER_REVIEW_DT,
          EFFECTIVE_DT,
          TIMESTAMP,
          DATE_MODIFIED,
          PREPARED_BY,
          DEVICE_ID,
          OPERATING_NUM,
          GLOBAL_ID,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          ONE_SHOT_LOCKOUT_TIME,
          ONE_SHOT_LOCKOUT_NUM,
          LOCKOUT_NUM,
          VOLT_THRESHOLD,
          RESET,
          GRD_INRUSH_TIME,
          GRD_INRUSH_MULTIPLIER,
          GRD_INRUSH_DURATION,
          MIN_GRD_TO_CT,
          REQUIRED_FAULT_CURRENT,
          FIRST_RECLOSE_RESET_TIME,
          PHA_INRUSH_TIME,
          PHA_INRUSH_MULTIPLIER,
          PHA_INRUSH_DURATION,
          MIN_PC_TO_CT,
          SECT_TYPE,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          RELEASED_BY,
          PROCESSED_FLAG,
          CONTROL_SERIAL_NUM,
          CURRENT_FUTURE,
          NOTES,
          DISTRICT,
          DIVISION,
          PEER_REVIEW_BY,
          FEATURE_CLASS_NAME
        )
      SELECT PEER_REVIEW_DT,
        EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        GLOBAL_ID,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        ONE_SHOT_LOCKOUT_TIME,
        ONE_SHOT_LOCKOUT_NUM,
        LOCKOUT_NUM,
        VOLT_THRESHOLD,
        RESET,
        GRD_INRUSH_TIME,
        GRD_INRUSH_MULTIPLIER,
        GRD_INRUSH_DURATION,
        MIN_GRD_TO_CT,
        REQUIRED_FAULT_CURRENT,
        FIRST_RECLOSE_RESET_TIME,
        PHA_INRUSH_TIME,
        PHA_INRUSH_MULTIPLIER,
        PHA_INRUSH_DURATION,
        MIN_PC_TO_CT,
        SECT_TYPE,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        FEATURE_CLASS_NAME
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Previous;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SECTIONALIZER_HIST',
          SM_SECTIONALIZER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      -- Remove previous global id from te device table
      DELETE
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_SECTIONALIZER_DETECTION;
PROCEDURE SP_CAPACITOR_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  VAR1        VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_CAPACITOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_CAPACITOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CAPACITOR',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_CAPACITOR_HIST
        (
          AUTO_BVR_CALC,
          BAUD_RATE,
          CONTROLLER_UNIT_MODEL,
          CONTROL_SERIAL_NUM,
          CONTROL_TYPE,
          CURRENT_FUTURE,
          DATA_LOGGING_INTERVAL,
          DATE_MODIFIED,
          DAYLIGHT_SAVINGS_TIME,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_DOCUMENT,
          EST_BANK_VOLTAGE_RISE,
          EST_VOLTAGE_CHANGE,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          GLOBAL_ID,
          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
          LOW_VOLTAGE_OVERRIDE_SETPOINT,
          MASTER_STATION,
          MAXCYCLES,
          MIN_SW_VOLTAGE,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PREFERED_BANK_POSITION,
          PREPARED_BY,
          PROCESSED_FLAG,
          PULSE_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SCH1_CONTROL_STRATEGY,
          SCH1_END_DATE,
          SCH1_HIGH_VOLTAGE_SETPOINT,
          SCH1_HOLIDAYS,
          SCH1_KVAR_SETPOINT_OFF,
          SCH1_KVAR_SETPOINT_ON,
          SCH1_LOW_VOLTAGE_SETPOINT,
          SCH1_SATURDAY,
          SCH1_SCHEDULE,
          SCH1_START_DATE,
          SCH1_SUNDAY,
          SCH1_TEMP_SETPOINT_OFF,
          SCH1_TEMP_SETPOINT_ON,
          SCH1_TIME_OFF,
          SCH1_TIME_ON,
          SCH1_WEEKDAYS,
          SCH2_CONTROL_STRATEGY,
          SCH2_END_DATE,
          SCH2_HIGH_VOLTAGE_SETPOINT,
          SCH2_HOLIDAYS,
          SCH2_KVAR_SETPOINT_OFF,
          SCH2_KVAR_SETPOINT_ON,
          SCH2_LOW_VOLTAGE_SETPOINT,
          SCH2_SATURDAY,
          SCH2_SCHEDULE,
          SCH2_START_DATE,
          SCH2_SUNDAY,
          SCH2_TEMP_SETPOINT_OFF,
          SCH2_TEMP_SETPOINT_ON,
          SCH2_TIME_OFF,
          SCH2_TIME_ON,
          SCH2_WEEKDAYS,
          SCH3_CONTROL_STRATEGY,
          SCH3_END_DATE,
          SCH3_HIGH_VOLTAGE_SETPOINT,
          SCH3_HOLIDAYS,
          SCH3_KVAR_SETPOINT_OFF,
          SCH3_KVAR_SETPOINT_ON,
          SCH3_LOW_VOLTAGE_SETPOINT,
          SCH3_SATURDAY,
          SCH3_SCHEDULE,
          SCH3_START_DATE,
          SCH3_SUNDAY,
          SCH3_TEMP_SETPOINT_OFF,
          SCH3_TEMP_SETPOINT_ON,
          SCH3_TIME_OFF,
          SCH3_TIME_ON,
          SCH3_WEEKDAYS,
          SCH4_CONTROL_STRATEGY,
          SCH4_END_DATE,
          SCH4_HIGH_VOLTAGE_SETPOINT,
          SCH4_HOLIDAYS,
          SCH4_KVAR_SETPOINT_OFF,
          SCH4_KVAR_SETPOINT_ON,
          SCH4_LOW_VOLTAGE_SETPOINT,
          SCH4_SATURDAY,
          SCH4_SCHEDULE,
          SCH4_START_DATE,
          SCH4_SUNDAY,
          SCH4_TEMP_SETPOINT_OFF,
          SCH4_TEMP_SETPOINT_ON,
          SCH4_TIME_OFF,
          SCH4_TIME_ON,
          SCH4_WEEKDAYS,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          SWITCH_POSITION,
          TEMPERATURE_CHANGE_TIME,
          TEMPERATURE_OVERRIDE,
          TIMESTAMP,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          VOLTAGE_CHANGE_TIME,
          VOLTAGE_OVERRIDE_TIME,
          VOLT_VAR_TEAM_MEMBER
        )
      SELECT AUTO_BVR_CALC,
        BAUD_RATE,
        CONTROLLER_UNIT_MODEL,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CURRENT_FUTURE,
        DATA_LOGGING_INTERVAL,
        DATE_MODIFIED,
        DAYLIGHT_SAVINGS_TIME,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        EST_BANK_VOLTAGE_RISE,
        EST_VOLTAGE_CHANGE,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
        LOW_VOLTAGE_OVERRIDE_SETPOINT,
        MASTER_STATION,
        MAXCYCLES,
        MIN_SW_VOLTAGE,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PREFERED_BANK_POSITION,
        PREPARED_BY,
        PROCESSED_FLAG,
        PULSE_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SCH1_CONTROL_STRATEGY,
        SCH1_END_DATE,
        SCH1_HIGH_VOLTAGE_SETPOINT,
        SCH1_HOLIDAYS,
        SCH1_KVAR_SETPOINT_OFF,
        SCH1_KVAR_SETPOINT_ON,
        SCH1_LOW_VOLTAGE_SETPOINT,
        SCH1_SATURDAY,
        SCH1_SCHEDULE,
        SCH1_START_DATE,
        SCH1_SUNDAY,
        SCH1_TEMP_SETPOINT_OFF,
        SCH1_TEMP_SETPOINT_ON,
        SCH1_TIME_OFF,
        SCH1_TIME_ON,
        SCH1_WEEKDAYS,
        SCH2_CONTROL_STRATEGY,
        SCH2_END_DATE,
        SCH2_HIGH_VOLTAGE_SETPOINT,
        SCH2_HOLIDAYS,
        SCH2_KVAR_SETPOINT_OFF,
        SCH2_KVAR_SETPOINT_ON,
        SCH2_LOW_VOLTAGE_SETPOINT,
        SCH2_SATURDAY,
        SCH2_SCHEDULE,
        SCH2_START_DATE,
        SCH2_SUNDAY,
        SCH2_TEMP_SETPOINT_OFF,
        SCH2_TEMP_SETPOINT_ON,
        SCH2_TIME_OFF,
        SCH2_TIME_ON,
        SCH2_WEEKDAYS,
        SCH3_CONTROL_STRATEGY,
        SCH3_END_DATE,
        SCH3_HIGH_VOLTAGE_SETPOINT,
        SCH3_HOLIDAYS,
        SCH3_KVAR_SETPOINT_OFF,
        SCH3_KVAR_SETPOINT_ON,
        SCH3_LOW_VOLTAGE_SETPOINT,
        SCH3_SATURDAY,
        SCH3_SCHEDULE,
        SCH3_START_DATE,
        SCH3_SUNDAY,
        SCH3_TEMP_SETPOINT_OFF,
        SCH3_TEMP_SETPOINT_ON,
        SCH3_TIME_OFF,
        SCH3_TIME_ON,
        SCH3_WEEKDAYS,
        SCH4_CONTROL_STRATEGY,
        SCH4_END_DATE,
        SCH4_HIGH_VOLTAGE_SETPOINT,
        SCH4_HOLIDAYS,
        SCH4_KVAR_SETPOINT_OFF,
        SCH4_KVAR_SETPOINT_ON,
        SCH4_LOW_VOLTAGE_SETPOINT,
        SCH4_SATURDAY,
        SCH4_SCHEDULE,
        SCH4_START_DATE,
        SCH4_SUNDAY,
        SCH4_TEMP_SETPOINT_OFF,
        SCH4_TEMP_SETPOINT_ON,
        SCH4_TIME_OFF,
        SCH4_TIME_ON,
        SCH4_WEEKDAYS,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        SWITCH_POSITION,
        TEMPERATURE_CHANGE_TIME,
        TEMPERATURE_OVERRIDE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        VOLTAGE_CHANGE_TIME,
        VOLTAGE_OVERRIDE_TIME,
        VOLT_VAR_TEAM_MEMBER
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID= I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CAPACITOR_HIST',
          SM_CAPACITOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_CAPACITOR
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District,
        CONTROL_TYPE    =I_Control_type_code
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    SELECT GLOBAL_ID
    INTO VAR1
    FROM SM_CAPACITOR
    WHERE GLOBAL_ID   = I_Global_id_Current
    AND CURRENT_FUTURE='C';
    -- first copy the entire current record to history table
    INSERT
    INTO SM_CAPACITOR_HIST
      (
        AUTO_BVR_CALC,
        BAUD_RATE,
        CONTROLLER_UNIT_MODEL,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CURRENT_FUTURE,
        DATA_LOGGING_INTERVAL,
        DATE_MODIFIED,
        DAYLIGHT_SAVINGS_TIME,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        EST_BANK_VOLTAGE_RISE,
        EST_VOLTAGE_CHANGE,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
        LOW_VOLTAGE_OVERRIDE_SETPOINT,
        MASTER_STATION,
        MAXCYCLES,
        MIN_SW_VOLTAGE,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PREFERED_BANK_POSITION,
        PREPARED_BY,
        PROCESSED_FLAG,
        PULSE_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SCH1_CONTROL_STRATEGY,
        SCH1_END_DATE,
        SCH1_HIGH_VOLTAGE_SETPOINT,
        SCH1_HOLIDAYS,
        SCH1_KVAR_SETPOINT_OFF,
        SCH1_KVAR_SETPOINT_ON,
        SCH1_LOW_VOLTAGE_SETPOINT,
        SCH1_SATURDAY,
        SCH1_SCHEDULE,
        SCH1_START_DATE,
        SCH1_SUNDAY,
        SCH1_TEMP_SETPOINT_OFF,
        SCH1_TEMP_SETPOINT_ON,
        SCH1_TIME_OFF,
        SCH1_TIME_ON,
        SCH1_WEEKDAYS,
        SCH2_CONTROL_STRATEGY,
        SCH2_END_DATE,
        SCH2_HIGH_VOLTAGE_SETPOINT,
        SCH2_HOLIDAYS,
        SCH2_KVAR_SETPOINT_OFF,
        SCH2_KVAR_SETPOINT_ON,
        SCH2_LOW_VOLTAGE_SETPOINT,
        SCH2_SATURDAY,
        SCH2_SCHEDULE,
        SCH2_START_DATE,
        SCH2_SUNDAY,
        SCH2_TEMP_SETPOINT_OFF,
        SCH2_TEMP_SETPOINT_ON,
        SCH2_TIME_OFF,
        SCH2_TIME_ON,
        SCH2_WEEKDAYS,
        SCH3_CONTROL_STRATEGY,
        SCH3_END_DATE,
        SCH3_HIGH_VOLTAGE_SETPOINT,
        SCH3_HOLIDAYS,
        SCH3_KVAR_SETPOINT_OFF,
        SCH3_KVAR_SETPOINT_ON,
        SCH3_LOW_VOLTAGE_SETPOINT,
        SCH3_SATURDAY,
        SCH3_SCHEDULE,
        SCH3_START_DATE,
        SCH3_SUNDAY,
        SCH3_TEMP_SETPOINT_OFF,
        SCH3_TEMP_SETPOINT_ON,
        SCH3_TIME_OFF,
        SCH3_TIME_ON,
        SCH3_WEEKDAYS,
        SCH4_CONTROL_STRATEGY,
        SCH4_END_DATE,
        SCH4_HIGH_VOLTAGE_SETPOINT,
        SCH4_HOLIDAYS,
        SCH4_KVAR_SETPOINT_OFF,
        SCH4_KVAR_SETPOINT_ON,
        SCH4_LOW_VOLTAGE_SETPOINT,
        SCH4_SATURDAY,
        SCH4_SCHEDULE,
        SCH4_START_DATE,
        SCH4_SUNDAY,
        SCH4_TEMP_SETPOINT_OFF,
        SCH4_TEMP_SETPOINT_ON,
        SCH4_TIME_OFF,
        SCH4_TIME_ON,
        SCH4_WEEKDAYS,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        SWITCH_POSITION,
        TEMPERATURE_CHANGE_TIME,
        TEMPERATURE_OVERRIDE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        VOLTAGE_CHANGE_TIME,
        VOLTAGE_OVERRIDE_TIME,
        VOLT_VAR_TEAM_MEMBER
      )
    SELECT AUTO_BVR_CALC,
      BAUD_RATE,
      CONTROLLER_UNIT_MODEL,
      CONTROL_SERIAL_NUM,
      CONTROL_TYPE,
      CURRENT_FUTURE,
      DATA_LOGGING_INTERVAL,
      DATE_MODIFIED,
      DAYLIGHT_SAVINGS_TIME,
      DEVICE_ID,
      DISTRICT,
      DIVISION,
      EFFECTIVE_DT,
      ENGINEERING_DOCUMENT,
      EST_BANK_VOLTAGE_RISE,
      EST_VOLTAGE_CHANGE,
      FEATURE_CLASS_NAME,
      FIRMWARE_VERSION,
      GLOBAL_ID,
      HIGH_VOLTAGE_OVERRIDE_SETPOINT,
      LOW_VOLTAGE_OVERRIDE_SETPOINT,
      MASTER_STATION,
      MAXCYCLES,
      MIN_SW_VOLTAGE,
      NOTES,
      OK_TO_BYPASS,
      OPERATING_NUM,
      PEER_REVIEW_BY,
      PEER_REVIEW_DT,
      PREFERED_BANK_POSITION,
      PREPARED_BY,
      PROCESSED_FLAG,
      PULSE_TIME,
      RADIO_MANF_CD,
      RADIO_MODEL_NUM,
      RADIO_SERIAL_NUM,
      RELAY_TYPE,
      RELEASED_BY,
      REPEATER,
      RTU_ADDRESS,
      SCADA,
      SCADA_TYPE,
      SCH1_CONTROL_STRATEGY,
      SCH1_END_DATE,
      SCH1_HIGH_VOLTAGE_SETPOINT,
      SCH1_HOLIDAYS,
      SCH1_KVAR_SETPOINT_OFF,
      SCH1_KVAR_SETPOINT_ON,
      SCH1_LOW_VOLTAGE_SETPOINT,
      SCH1_SATURDAY,
      SCH1_SCHEDULE,
      SCH1_START_DATE,
      SCH1_SUNDAY,
      SCH1_TEMP_SETPOINT_OFF,
      SCH1_TEMP_SETPOINT_ON,
      SCH1_TIME_OFF,
      SCH1_TIME_ON,
      SCH1_WEEKDAYS,
      SCH2_CONTROL_STRATEGY,
      SCH2_END_DATE,
      SCH2_HIGH_VOLTAGE_SETPOINT,
      SCH2_HOLIDAYS,
      SCH2_KVAR_SETPOINT_OFF,
      SCH2_KVAR_SETPOINT_ON,
      SCH2_LOW_VOLTAGE_SETPOINT,
      SCH2_SATURDAY,
      SCH2_SCHEDULE,
      SCH2_START_DATE,
      SCH2_SUNDAY,
      SCH2_TEMP_SETPOINT_OFF,
      SCH2_TEMP_SETPOINT_ON,
      SCH2_TIME_OFF,
      SCH2_TIME_ON,
      SCH2_WEEKDAYS,
      SCH3_CONTROL_STRATEGY,
      SCH3_END_DATE,
      SCH3_HIGH_VOLTAGE_SETPOINT,
      SCH3_HOLIDAYS,
      SCH3_KVAR_SETPOINT_OFF,
      SCH3_KVAR_SETPOINT_ON,
      SCH3_LOW_VOLTAGE_SETPOINT,
      SCH3_SATURDAY,
      SCH3_SCHEDULE,
      SCH3_START_DATE,
      SCH3_SUNDAY,
      SCH3_TEMP_SETPOINT_OFF,
      SCH3_TEMP_SETPOINT_ON,
      SCH3_TIME_OFF,
      SCH3_TIME_ON,
      SCH3_WEEKDAYS,
      SCH4_CONTROL_STRATEGY,
      SCH4_END_DATE,
      SCH4_HIGH_VOLTAGE_SETPOINT,
      SCH4_HOLIDAYS,
      SCH4_KVAR_SETPOINT_OFF,
      SCH4_KVAR_SETPOINT_ON,
      SCH4_LOW_VOLTAGE_SETPOINT,
      SCH4_SATURDAY,
      SCH4_SCHEDULE,
      SCH4_START_DATE,
      SCH4_SUNDAY,
      SCH4_TEMP_SETPOINT_OFF,
      SCH4_TEMP_SETPOINT_ON,
      SCH4_TIME_OFF,
      SCH4_TIME_ON,
      SCH4_WEEKDAYS,
      SOFTWARE_VERSION,
      SPECIAL_CONDITIONS,
      SWITCH_POSITION,
      TEMPERATURE_CHANGE_TIME,
      TEMPERATURE_OVERRIDE,
      TIMESTAMP,
      TIME_DELAY,
      TRANSMIT_DISABLE_DELAY,
      TRANSMIT_ENABLE_DELAY,
      VOLTAGE_CHANGE_TIME,
      VOLTAGE_OVERRIDE_TIME,
      VOLT_VAR_TEAM_MEMBER
    FROM SM_CAPACITOR
    WHERE GLOBAL_ID= I_Global_id_Current;
    DELETE FROM SM_CAPACITOR WHERE GLOBAL_ID = I_Global_id_Current;
    -- Insert a record in comments table with notes set to INST
    -- Insert a record in comments table with notes set to INST
    INSERT
    INTO SM_COMMENT_HIST
      (
        DEVICE_HIST_TABLE_NAME,
        HIST_ID,
        WORK_DATE,
        WORK_TYPE,
        PERFORMED_BY,
        ENTRY_DATE,
        COMMENTS
      )
      VALUES
      (
        'SM_CAPACITOR_HIST',
        SM_CAPACITOR_HIST_SEQ.CURRVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
    COMMIT;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_CAPACITOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_CAPACITOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CAPACITOR',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_CAPACITOR
      SET
        (
          AUTO_BVR_CALC,
          BAUD_RATE,
          CONTROLLER_UNIT_MODEL,
          CONTROL_TYPE,
          DATA_LOGGING_INTERVAL,
          DAYLIGHT_SAVINGS_TIME,
          ENGINEERING_DOCUMENT,
          EST_BANK_VOLTAGE_RISE,
          EST_VOLTAGE_CHANGE,
          FIRMWARE_VERSION,
          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
          LOW_VOLTAGE_OVERRIDE_SETPOINT,
          MASTER_STATION,
          MAXCYCLES,
          MIN_SW_VOLTAGE,
          OK_TO_BYPASS,
          PREFERED_BANK_POSITION,
          PULSE_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SCH1_CONTROL_STRATEGY,
          SCH1_END_DATE,
          SCH1_HIGH_VOLTAGE_SETPOINT,
          SCH1_HOLIDAYS,
          SCH1_KVAR_SETPOINT_OFF,
          SCH1_KVAR_SETPOINT_ON,
          SCH1_LOW_VOLTAGE_SETPOINT,
          SCH1_SATURDAY,
          SCH1_SCHEDULE,
          SCH1_START_DATE,
          SCH1_SUNDAY,
          SCH1_TEMP_SETPOINT_OFF,
          SCH1_TEMP_SETPOINT_ON,
          SCH1_TIME_OFF,
          SCH1_TIME_ON,
          SCH1_WEEKDAYS,
          SCH2_CONTROL_STRATEGY,
          SCH2_END_DATE,
          SCH2_HIGH_VOLTAGE_SETPOINT,
          SCH2_HOLIDAYS,
          SCH2_KVAR_SETPOINT_OFF,
          SCH2_KVAR_SETPOINT_ON,
          SCH2_LOW_VOLTAGE_SETPOINT,
          SCH2_SATURDAY,
          SCH2_SCHEDULE,
          SCH2_START_DATE,
          SCH2_SUNDAY,
          SCH2_TEMP_SETPOINT_OFF,
          SCH2_TEMP_SETPOINT_ON,
          SCH2_TIME_OFF,
          SCH2_TIME_ON,
          SCH2_WEEKDAYS,
          SCH3_CONTROL_STRATEGY,
          SCH3_END_DATE,
          SCH3_HIGH_VOLTAGE_SETPOINT,
          SCH3_HOLIDAYS,
          SCH3_KVAR_SETPOINT_OFF,
          SCH3_KVAR_SETPOINT_ON,
          SCH3_LOW_VOLTAGE_SETPOINT,
          SCH3_SATURDAY,
          SCH3_SCHEDULE,
          SCH3_START_DATE,
          SCH3_SUNDAY,
          SCH3_TEMP_SETPOINT_OFF,
          SCH3_TEMP_SETPOINT_ON,
          SCH3_TIME_OFF,
          SCH3_TIME_ON,
          SCH3_WEEKDAYS,
          SCH4_CONTROL_STRATEGY,
          SCH4_END_DATE,
          SCH4_HIGH_VOLTAGE_SETPOINT,
          SCH4_HOLIDAYS,
          SCH4_KVAR_SETPOINT_OFF,
          SCH4_KVAR_SETPOINT_ON,
          SCH4_LOW_VOLTAGE_SETPOINT,
          SCH4_SATURDAY,
          SCH4_SCHEDULE,
          SCH4_START_DATE,
          SCH4_SUNDAY,
          SCH4_TEMP_SETPOINT_OFF,
          SCH4_TEMP_SETPOINT_ON,
          SCH4_TIME_OFF,
          SCH4_TIME_ON,
          SCH4_WEEKDAYS,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          SWITCH_POSITION,
          TEMPERATURE_CHANGE_TIME,
          TEMPERATURE_OVERRIDE,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          VOLTAGE_CHANGE_TIME,
          VOLTAGE_OVERRIDE_TIME,
          VOLT_VAR_TEAM_MEMBER
        )
        =
        (SELECT AUTO_BVR_CALC,
          BAUD_RATE,
          CONTROLLER_UNIT_MODEL,
          CONTROL_TYPE,
          DATA_LOGGING_INTERVAL,
          DAYLIGHT_SAVINGS_TIME,
          ENGINEERING_DOCUMENT,
          EST_BANK_VOLTAGE_RISE,
          EST_VOLTAGE_CHANGE,
          FIRMWARE_VERSION,
          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
          LOW_VOLTAGE_OVERRIDE_SETPOINT,
          MASTER_STATION,
          MAXCYCLES,
          MIN_SW_VOLTAGE,
          OK_TO_BYPASS,
          PREFERED_BANK_POSITION,
          PULSE_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SCH1_CONTROL_STRATEGY,
          SCH1_END_DATE,
          SCH1_HIGH_VOLTAGE_SETPOINT,
          SCH1_HOLIDAYS,
          SCH1_KVAR_SETPOINT_OFF,
          SCH1_KVAR_SETPOINT_ON,
          SCH1_LOW_VOLTAGE_SETPOINT,
          SCH1_SATURDAY,
          SCH1_SCHEDULE,
          SCH1_START_DATE,
          SCH1_SUNDAY,
          SCH1_TEMP_SETPOINT_OFF,
          SCH1_TEMP_SETPOINT_ON,
          SCH1_TIME_OFF,
          SCH1_TIME_ON,
          SCH1_WEEKDAYS,
          SCH2_CONTROL_STRATEGY,
          SCH2_END_DATE,
          SCH2_HIGH_VOLTAGE_SETPOINT,
          SCH2_HOLIDAYS,
          SCH2_KVAR_SETPOINT_OFF,
          SCH2_KVAR_SETPOINT_ON,
          SCH2_LOW_VOLTAGE_SETPOINT,
          SCH2_SATURDAY,
          SCH2_SCHEDULE,
          SCH2_START_DATE,
          SCH2_SUNDAY,
          SCH2_TEMP_SETPOINT_OFF,
          SCH2_TEMP_SETPOINT_ON,
          SCH2_TIME_OFF,
          SCH2_TIME_ON,
          SCH2_WEEKDAYS,
          SCH3_CONTROL_STRATEGY,
          SCH3_END_DATE,
          SCH3_HIGH_VOLTAGE_SETPOINT,
          SCH3_HOLIDAYS,
          SCH3_KVAR_SETPOINT_OFF,
          SCH3_KVAR_SETPOINT_ON,
          SCH3_LOW_VOLTAGE_SETPOINT,
          SCH3_SATURDAY,
          SCH3_SCHEDULE,
          SCH3_START_DATE,
          SCH3_SUNDAY,
          SCH3_TEMP_SETPOINT_OFF,
          SCH3_TEMP_SETPOINT_ON,
          SCH3_TIME_OFF,
          SCH3_TIME_ON,
          SCH3_WEEKDAYS,
          SCH4_CONTROL_STRATEGY,
          SCH4_END_DATE,
          SCH4_HIGH_VOLTAGE_SETPOINT,
          SCH4_HOLIDAYS,
          SCH4_KVAR_SETPOINT_OFF,
          SCH4_KVAR_SETPOINT_ON,
          SCH4_LOW_VOLTAGE_SETPOINT,
          SCH4_SATURDAY,
          SCH4_SCHEDULE,
          SCH4_START_DATE,
          SCH4_SUNDAY,
          SCH4_TEMP_SETPOINT_OFF,
          SCH4_TEMP_SETPOINT_ON,
          SCH4_TIME_OFF,
          SCH4_TIME_ON,
          SCH4_WEEKDAYS,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          SWITCH_POSITION,
          TEMPERATURE_CHANGE_TIME,
          TEMPERATURE_OVERRIDE,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          VOLTAGE_CHANGE_TIME,
          VOLTAGE_OVERRIDE_TIME,
          VOLT_VAR_TEAM_MEMBER
        FROM SM_CAPACITOR
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_CAPACITOR_HIST
        (
          AUTO_BVR_CALC,
          BAUD_RATE,
          CONTROLLER_UNIT_MODEL,
          CONTROL_SERIAL_NUM,
          CONTROL_TYPE,
          CURRENT_FUTURE,
          DATA_LOGGING_INTERVAL,
          DATE_MODIFIED,
          DAYLIGHT_SAVINGS_TIME,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_DOCUMENT,
          EST_BANK_VOLTAGE_RISE,
          EST_VOLTAGE_CHANGE,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          GLOBAL_ID,
          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
          LOW_VOLTAGE_OVERRIDE_SETPOINT,
          MASTER_STATION,
          MAXCYCLES,
          MIN_SW_VOLTAGE,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PREFERED_BANK_POSITION,
          PREPARED_BY,
          PROCESSED_FLAG,
          PULSE_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SCH1_CONTROL_STRATEGY,
          SCH1_END_DATE,
          SCH1_HIGH_VOLTAGE_SETPOINT,
          SCH1_HOLIDAYS,
          SCH1_KVAR_SETPOINT_OFF,
          SCH1_KVAR_SETPOINT_ON,
          SCH1_LOW_VOLTAGE_SETPOINT,
          SCH1_SATURDAY,
          SCH1_SCHEDULE,
          SCH1_START_DATE,
          SCH1_SUNDAY,
          SCH1_TEMP_SETPOINT_OFF,
          SCH1_TEMP_SETPOINT_ON,
          SCH1_TIME_OFF,
          SCH1_TIME_ON,
          SCH1_WEEKDAYS,
          SCH2_CONTROL_STRATEGY,
          SCH2_END_DATE,
          SCH2_HIGH_VOLTAGE_SETPOINT,
          SCH2_HOLIDAYS,
          SCH2_KVAR_SETPOINT_OFF,
          SCH2_KVAR_SETPOINT_ON,
          SCH2_LOW_VOLTAGE_SETPOINT,
          SCH2_SATURDAY,
          SCH2_SCHEDULE,
          SCH2_START_DATE,
          SCH2_SUNDAY,
          SCH2_TEMP_SETPOINT_OFF,
          SCH2_TEMP_SETPOINT_ON,
          SCH2_TIME_OFF,
          SCH2_TIME_ON,
          SCH2_WEEKDAYS,
          SCH3_CONTROL_STRATEGY,
          SCH3_END_DATE,
          SCH3_HIGH_VOLTAGE_SETPOINT,
          SCH3_HOLIDAYS,
          SCH3_KVAR_SETPOINT_OFF,
          SCH3_KVAR_SETPOINT_ON,
          SCH3_LOW_VOLTAGE_SETPOINT,
          SCH3_SATURDAY,
          SCH3_SCHEDULE,
          SCH3_START_DATE,
          SCH3_SUNDAY,
          SCH3_TEMP_SETPOINT_OFF,
          SCH3_TEMP_SETPOINT_ON,
          SCH3_TIME_OFF,
          SCH3_TIME_ON,
          SCH3_WEEKDAYS,
          SCH4_CONTROL_STRATEGY,
          SCH4_END_DATE,
          SCH4_HIGH_VOLTAGE_SETPOINT,
          SCH4_HOLIDAYS,
          SCH4_KVAR_SETPOINT_OFF,
          SCH4_KVAR_SETPOINT_ON,
          SCH4_LOW_VOLTAGE_SETPOINT,
          SCH4_SATURDAY,
          SCH4_SCHEDULE,
          SCH4_START_DATE,
          SCH4_SUNDAY,
          SCH4_TEMP_SETPOINT_OFF,
          SCH4_TEMP_SETPOINT_ON,
          SCH4_TIME_OFF,
          SCH4_TIME_ON,
          SCH4_WEEKDAYS,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          SWITCH_POSITION,
          TEMPERATURE_CHANGE_TIME,
          TEMPERATURE_OVERRIDE,
          TIMESTAMP,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          VOLTAGE_CHANGE_TIME,
          VOLTAGE_OVERRIDE_TIME,
          VOLT_VAR_TEAM_MEMBER
        )
      SELECT AUTO_BVR_CALC,
        BAUD_RATE,
        CONTROLLER_UNIT_MODEL,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CURRENT_FUTURE,
        DATA_LOGGING_INTERVAL,
        DATE_MODIFIED,
        DAYLIGHT_SAVINGS_TIME,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        EST_BANK_VOLTAGE_RISE,
        EST_VOLTAGE_CHANGE,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
        LOW_VOLTAGE_OVERRIDE_SETPOINT,
        MASTER_STATION,
        MAXCYCLES,
        MIN_SW_VOLTAGE,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PREFERED_BANK_POSITION,
        PREPARED_BY,
        PROCESSED_FLAG,
        PULSE_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SCH1_CONTROL_STRATEGY,
        SCH1_END_DATE,
        SCH1_HIGH_VOLTAGE_SETPOINT,
        SCH1_HOLIDAYS,
        SCH1_KVAR_SETPOINT_OFF,
        SCH1_KVAR_SETPOINT_ON,
        SCH1_LOW_VOLTAGE_SETPOINT,
        SCH1_SATURDAY,
        SCH1_SCHEDULE,
        SCH1_START_DATE,
        SCH1_SUNDAY,
        SCH1_TEMP_SETPOINT_OFF,
        SCH1_TEMP_SETPOINT_ON,
        SCH1_TIME_OFF,
        SCH1_TIME_ON,
        SCH1_WEEKDAYS,
        SCH2_CONTROL_STRATEGY,
        SCH2_END_DATE,
        SCH2_HIGH_VOLTAGE_SETPOINT,
        SCH2_HOLIDAYS,
        SCH2_KVAR_SETPOINT_OFF,
        SCH2_KVAR_SETPOINT_ON,
        SCH2_LOW_VOLTAGE_SETPOINT,
        SCH2_SATURDAY,
        SCH2_SCHEDULE,
        SCH2_START_DATE,
        SCH2_SUNDAY,
        SCH2_TEMP_SETPOINT_OFF,
        SCH2_TEMP_SETPOINT_ON,
        SCH2_TIME_OFF,
        SCH2_TIME_ON,
        SCH2_WEEKDAYS,
        SCH3_CONTROL_STRATEGY,
        SCH3_END_DATE,
        SCH3_HIGH_VOLTAGE_SETPOINT,
        SCH3_HOLIDAYS,
        SCH3_KVAR_SETPOINT_OFF,
        SCH3_KVAR_SETPOINT_ON,
        SCH3_LOW_VOLTAGE_SETPOINT,
        SCH3_SATURDAY,
        SCH3_SCHEDULE,
        SCH3_START_DATE,
        SCH3_SUNDAY,
        SCH3_TEMP_SETPOINT_OFF,
        SCH3_TEMP_SETPOINT_ON,
        SCH3_TIME_OFF,
        SCH3_TIME_ON,
        SCH3_WEEKDAYS,
        SCH4_CONTROL_STRATEGY,
        SCH4_END_DATE,
        SCH4_HIGH_VOLTAGE_SETPOINT,
        SCH4_HOLIDAYS,
        SCH4_KVAR_SETPOINT_OFF,
        SCH4_KVAR_SETPOINT_ON,
        SCH4_LOW_VOLTAGE_SETPOINT,
        SCH4_SATURDAY,
        SCH4_SCHEDULE,
        SCH4_START_DATE,
        SCH4_SUNDAY,
        SCH4_TEMP_SETPOINT_OFF,
        SCH4_TEMP_SETPOINT_ON,
        SCH4_TIME_OFF,
        SCH4_TIME_ON,
        SCH4_WEEKDAYS,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        SWITCH_POSITION,
        TEMPERATURE_CHANGE_TIME,
        TEMPERATURE_OVERRIDE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        VOLTAGE_CHANGE_TIME,
        VOLTAGE_OVERRIDE_TIME,
        VOLT_VAR_TEAM_MEMBER
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID= I_Global_id_Current;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CAPACITOR_HIST',
          SM_CAPACITOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_CAPACITOR WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_CAPACITOR_DETECTION;
PROCEDURE SP_CIRCUIT_BREAKER_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  VAR1        VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_CIRCUIT_BREAKER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_CIRCUIT_BREAKER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CIRCUIT_BREAKER',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_CIRCUIT_BREAKER_HIST
        (
          ANNUAL_LF,
          BAUD_RATE,
          CC_RATING,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          DPA_CD,
          EFFECTIVE_DT,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FLISR,
          FLISR_ENGINEERING_COMMENTS,
          FLISR_OPERATING_MODE,
          GLOBAL_ID,
          GRD_BK_INS_TRIP,
          GRD_BK_LEVER_SET,
          GRD_BK_MIN_TRIP,
          GRD_BK_RELAY_TYPE,
          GRD_PR_INS_TRIP,
          GRD_PR_LEVER_SET,
          GRD_PR_MIN_TRIP,
          GRD_PR_RELAY_TYPE,
          LIMITING_FACTOR,
          MASTER_STATION,
          MIN_NOR_VOLT,
          NETWORK,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_MODE,
          OPERATING_NUM,
          OPS_TO_LOCKOUT,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHA_BK_INS_TRIP,
          PHA_BK_LEVER_SET,
          PHA_BK_MIN_TRIP,
          PHA_BK_RELAY_TYPE,
          PHA_PR_INS_TRIP,
          PHA_PR_LEVER_SET,
          PHA_PR_MIN_TRIP,
          PHA_PR_RELAY_TYPE,
          PREPARED_BY,
          PROCESSED_FLAG,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SPECIAL_CONDITIONS,
          SUMMER_LOAD_LIMIT,
          TIMESTAMP,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          WINTER_LOAD_LIMIT
        )
      SELECT ANNUAL_LF,
        BAUD_RATE,
        CC_RATING,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        DPA_CD,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FLISR,
        FLISR_ENGINEERING_COMMENTS,
        FLISR_OPERATING_MODE,
        GLOBAL_ID,
        GRD_BK_INS_TRIP,
        GRD_BK_LEVER_SET,
        GRD_BK_MIN_TRIP,
        GRD_BK_RELAY_TYPE,
        GRD_PR_INS_TRIP,
        GRD_PR_LEVER_SET,
        GRD_PR_MIN_TRIP,
        GRD_PR_RELAY_TYPE,
        LIMITING_FACTOR,
        MASTER_STATION,
        MIN_NOR_VOLT,
        NETWORK,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_MODE,
        OPERATING_NUM,
        OPS_TO_LOCKOUT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_BK_INS_TRIP,
        PHA_BK_LEVER_SET,
        PHA_BK_MIN_TRIP,
        PHA_BK_RELAY_TYPE,
        PHA_PR_INS_TRIP,
        PHA_PR_LEVER_SET,
        PHA_PR_MIN_TRIP,
        PHA_PR_RELAY_TYPE,
        PREPARED_BY,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SPECIAL_CONDITIONS,
        SUMMER_LOAD_LIMIT,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        WINTER_LOAD_LIMIT
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID= I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CIRCUIT_BREAKER_HIST',
          SM_CIRCUIT_BREAKER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_CIRCUIT_BREAKER
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    SELECT GLOBAL_ID
    INTO VAR1
    FROM SM_CIRCUIT_BREAKER
    WHERE GLOBAL_ID   = I_Global_id_Current
    AND CURRENT_FUTURE='C';
    -- first copy the entire current record to history table
    INSERT
    INTO SM_CIRCUIT_BREAKER_HIST
      (
        ANNUAL_LF,
        BAUD_RATE,
        CC_RATING,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        DPA_CD,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FLISR,
        FLISR_ENGINEERING_COMMENTS,
        FLISR_OPERATING_MODE,
        GLOBAL_ID,
        GRD_BK_INS_TRIP,
        GRD_BK_LEVER_SET,
        GRD_BK_MIN_TRIP,
        GRD_BK_RELAY_TYPE,
        GRD_PR_INS_TRIP,
        GRD_PR_LEVER_SET,
        GRD_PR_MIN_TRIP,
        GRD_PR_RELAY_TYPE,
        LIMITING_FACTOR,
        MASTER_STATION,
        MIN_NOR_VOLT,
        NETWORK,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_MODE,
        OPERATING_NUM,
        OPS_TO_LOCKOUT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_BK_INS_TRIP,
        PHA_BK_LEVER_SET,
        PHA_BK_MIN_TRIP,
        PHA_BK_RELAY_TYPE,
        PHA_PR_INS_TRIP,
        PHA_PR_LEVER_SET,
        PHA_PR_MIN_TRIP,
        PHA_PR_RELAY_TYPE,
        PREPARED_BY,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SPECIAL_CONDITIONS,
        SUMMER_LOAD_LIMIT,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        WINTER_LOAD_LIMIT
      )
    SELECT ANNUAL_LF,
      BAUD_RATE,
      CC_RATING,
      CURRENT_FUTURE,
      DATE_MODIFIED,
      DEVICE_ID,
      DISTRICT,
      DIVISION,
      DPA_CD,
      EFFECTIVE_DT,
      ENGINEERING_COMMENTS,
      ENGINEERING_DOCUMENT,
      FEATURE_CLASS_NAME,
      FLISR,
      FLISR_ENGINEERING_COMMENTS,
      FLISR_OPERATING_MODE,
      GLOBAL_ID,
      GRD_BK_INS_TRIP,
      GRD_BK_LEVER_SET,
      GRD_BK_MIN_TRIP,
      GRD_BK_RELAY_TYPE,
      GRD_PR_INS_TRIP,
      GRD_PR_LEVER_SET,
      GRD_PR_MIN_TRIP,
      GRD_PR_RELAY_TYPE,
      LIMITING_FACTOR,
      MASTER_STATION,
      MIN_NOR_VOLT,
      NETWORK,
      NOTES,
      OK_TO_BYPASS,
      OPERATING_MODE,
      OPERATING_NUM,
      OPS_TO_LOCKOUT,
      PEER_REVIEW_BY,
      PEER_REVIEW_DT,
      PHA_BK_INS_TRIP,
      PHA_BK_LEVER_SET,
      PHA_BK_MIN_TRIP,
      PHA_BK_RELAY_TYPE,
      PHA_PR_INS_TRIP,
      PHA_PR_LEVER_SET,
      PHA_PR_MIN_TRIP,
      PHA_PR_RELAY_TYPE,
      PREPARED_BY,
      PROCESSED_FLAG,
      RADIO_MANF_CD,
      RADIO_MODEL_NUM,
      RADIO_SERIAL_NUM,
      RELAY_TYPE,
      RELEASED_BY,
      REPEATER,
      RTU_ADDRESS,
      SCADA,
      SCADA_TYPE,
      SPECIAL_CONDITIONS,
      SUMMER_LOAD_LIMIT,
      TIMESTAMP,
      TRANSMIT_DISABLE_DELAY,
      TRANSMIT_ENABLE_DELAY,
      WINTER_LOAD_LIMIT
    FROM SM_CIRCUIT_BREAKER
    WHERE GLOBAL_ID= I_Global_id_Current;
    DELETE FROM SM_CIRCUIT_BREAKER WHERE GLOBAL_ID = I_Global_id_Current;
    -- Insert a record in comments table with notes set to INST
    -- Insert a record in comments table with notes set to INST
    INSERT
    INTO SM_COMMENT_HIST
      (
        DEVICE_HIST_TABLE_NAME,
        HIST_ID,
        WORK_DATE,
        WORK_TYPE,
        PERFORMED_BY,
        ENTRY_DATE,
        COMMENTS
      )
      VALUES
      (
        'SM_CIRCUIT_BREAKER_HIST',
        SM_CIRCUIT_BREAKER_HIST_SEQ.CURRVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
    COMMIT;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_CIRCUIT_BREAKER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_CIRCUIT_BREAKER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CIRCUIT_BREAKER ',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_CIRCUIT_BREAKER
      SET
        (
          ANNUAL_LF,
          BAUD_RATE,
          CC_RATING,
          DPA_CD,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FLISR,
          FLISR_ENGINEERING_COMMENTS,
          FLISR_OPERATING_MODE,
          GRD_BK_INS_TRIP,
          GRD_BK_LEVER_SET,
          GRD_BK_MIN_TRIP,
          GRD_BK_RELAY_TYPE,
          GRD_PR_INS_TRIP,
          GRD_PR_LEVER_SET,
          GRD_PR_MIN_TRIP,
          GRD_PR_RELAY_TYPE,
          LIMITING_FACTOR,
          MASTER_STATION,
          MIN_NOR_VOLT,
          NETWORK,
          OK_TO_BYPASS,
          OPERATING_MODE,
          OPS_TO_LOCKOUT,
          PHA_BK_INS_TRIP,
          PHA_BK_LEVER_SET,
          PHA_BK_MIN_TRIP,
          PHA_BK_RELAY_TYPE,
          PHA_PR_INS_TRIP,
          PHA_PR_LEVER_SET,
          PHA_PR_MIN_TRIP,
          PHA_PR_RELAY_TYPE,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SPECIAL_CONDITIONS,
          SUMMER_LOAD_LIMIT,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          WINTER_LOAD_LIMIT
        )
        =
        (SELECT ANNUAL_LF,
          BAUD_RATE,
          CC_RATING,
          DPA_CD,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FLISR,
          FLISR_ENGINEERING_COMMENTS,
          FLISR_OPERATING_MODE,
          GRD_BK_INS_TRIP,
          GRD_BK_LEVER_SET,
          GRD_BK_MIN_TRIP,
          GRD_BK_RELAY_TYPE,
          GRD_PR_INS_TRIP,
          GRD_PR_LEVER_SET,
          GRD_PR_MIN_TRIP,
          GRD_PR_RELAY_TYPE,
          LIMITING_FACTOR,
          MASTER_STATION,
          MIN_NOR_VOLT,
          NETWORK,
          OK_TO_BYPASS,
          OPERATING_MODE,
          OPS_TO_LOCKOUT,
          PHA_BK_INS_TRIP,
          PHA_BK_LEVER_SET,
          PHA_BK_MIN_TRIP,
          PHA_BK_RELAY_TYPE,
          PHA_PR_INS_TRIP,
          PHA_PR_LEVER_SET,
          PHA_PR_MIN_TRIP,
          PHA_PR_RELAY_TYPE,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SPECIAL_CONDITIONS,
          SUMMER_LOAD_LIMIT,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          WINTER_LOAD_LIMIT
        FROM SM_CIRCUIT_BREAKER
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_CIRCUIT_BREAKER_HIST
        (
          ANNUAL_LF,
          BAUD_RATE,
          CC_RATING,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          DPA_CD,
          EFFECTIVE_DT,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FLISR,
          FLISR_ENGINEERING_COMMENTS,
          FLISR_OPERATING_MODE,
          GLOBAL_ID,
          GRD_BK_INS_TRIP,
          GRD_BK_LEVER_SET,
          GRD_BK_MIN_TRIP,
          GRD_BK_RELAY_TYPE,
          GRD_PR_INS_TRIP,
          GRD_PR_LEVER_SET,
          GRD_PR_MIN_TRIP,
          GRD_PR_RELAY_TYPE,
          LIMITING_FACTOR,
          MASTER_STATION,
          MIN_NOR_VOLT,
          NETWORK,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_MODE,
          OPERATING_NUM,
          OPS_TO_LOCKOUT,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHA_BK_INS_TRIP,
          PHA_BK_LEVER_SET,
          PHA_BK_MIN_TRIP,
          PHA_BK_RELAY_TYPE,
          PHA_PR_INS_TRIP,
          PHA_PR_LEVER_SET,
          PHA_PR_MIN_TRIP,
          PHA_PR_RELAY_TYPE,
          PREPARED_BY,
          PROCESSED_FLAG,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SPECIAL_CONDITIONS,
          SUMMER_LOAD_LIMIT,
          TIMESTAMP,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          WINTER_LOAD_LIMIT
        )
      SELECT ANNUAL_LF,
        BAUD_RATE,
        CC_RATING,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        DPA_CD,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FLISR,
        FLISR_ENGINEERING_COMMENTS,
        FLISR_OPERATING_MODE,
        GLOBAL_ID,
        GRD_BK_INS_TRIP,
        GRD_BK_LEVER_SET,
        GRD_BK_MIN_TRIP,
        GRD_BK_RELAY_TYPE,
        GRD_PR_INS_TRIP,
        GRD_PR_LEVER_SET,
        GRD_PR_MIN_TRIP,
        GRD_PR_RELAY_TYPE,
        LIMITING_FACTOR,
        MASTER_STATION,
        MIN_NOR_VOLT,
        NETWORK,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_MODE,
        OPERATING_NUM,
        OPS_TO_LOCKOUT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_BK_INS_TRIP,
        PHA_BK_LEVER_SET,
        PHA_BK_MIN_TRIP,
        PHA_BK_RELAY_TYPE,
        PHA_PR_INS_TRIP,
        PHA_PR_LEVER_SET,
        PHA_PR_MIN_TRIP,
        PHA_PR_RELAY_TYPE,
        PREPARED_BY,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SPECIAL_CONDITIONS,
        SUMMER_LOAD_LIMIT,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        WINTER_LOAD_LIMIT
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID= I_Global_id_Current;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_CIRCUIT_BREAKER _HIST',
          SM_CIRCUIT_BREAKER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_CIRCUIT_BREAKER WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_CIRCUIT_BREAKER_DETECTION;
PROCEDURE SP_INTERRUPTER_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  VAR1        VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_INTERRUPTER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_INTERRUPTER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_INTERRUPTER',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_INTERRUPTER_HIST
        (
          BAUD_RATE,
          CONTROL_SERIAL_NUM,
          CONTROL_TYPE,
          CT_RATIO,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          GLOBAL_ID,
          GRD_CUR_TRIP,
          GRD_INST_PICKUP_SETTING,
          GRD_PICKUP_SETTING,
          GRD_TD_LEVER_SETTING,
          GRD_TRIP_CD,
          MANF_CD,
          MASTER_STATION,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          OPERATIONAL_MODE_SWITCH,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHA_CUR_TRIP,
          PHA_INST_PICKUP_SETTING,
          PHA_PICKUP_SETTING,
          PHA_TD_LEVER_SETTING,
          PHA_TRIP_CD,
          PREPARED_BY,
          PRIMARY_VOLTAGE,
          PROCESSED_FLAG,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          TIMESTAMP,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TYP_CRV_GRD,
          TYP_CRV_PHA
        )
      SELECT BAUD_RATE,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        GRD_CUR_TRIP,
        GRD_INST_PICKUP_SETTING,
        GRD_PICKUP_SETTING,
        GRD_TD_LEVER_SETTING,
        GRD_TRIP_CD,
        MANF_CD,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OPERATIONAL_MODE_SWITCH,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_CUR_TRIP,
        PHA_INST_PICKUP_SETTING,
        PHA_PICKUP_SETTING,
        PHA_TD_LEVER_SETTING,
        PHA_TRIP_CD,
        PREPARED_BY,
        PRIMARY_VOLTAGE,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TYP_CRV_GRD,
        TYP_CRV_PHA
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID=I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_INTERRUPTER_HIST',
          SM_INTERRUPTER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_INTERRUPTER
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District,
        CONTROL_TYPE    =I_Control_type_code
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    SELECT GLOBAL_ID
    INTO VAR1
    FROM SM_INTERRUPTER
    WHERE GLOBAL_ID   = I_Global_id_Current
    AND CURRENT_FUTURE='C';
    -- first copy the entire current record to history table
    INSERT
    INTO SM_INTERRUPTER_HIST
      (
        BAUD_RATE,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        GRD_CUR_TRIP,
        GRD_INST_PICKUP_SETTING,
        GRD_PICKUP_SETTING,
        GRD_TD_LEVER_SETTING,
        GRD_TRIP_CD,
        MANF_CD,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OPERATIONAL_MODE_SWITCH,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_CUR_TRIP,
        PHA_INST_PICKUP_SETTING,
        PHA_PICKUP_SETTING,
        PHA_TD_LEVER_SETTING,
        PHA_TRIP_CD,
        PREPARED_BY,
        PRIMARY_VOLTAGE,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TYP_CRV_GRD,
        TYP_CRV_PHA
      )
    SELECT BAUD_RATE,
      CONTROL_SERIAL_NUM,
      CONTROL_TYPE,
      CT_RATIO,
      CURRENT_FUTURE,
      DATE_MODIFIED,
      DEVICE_ID,
      DISTRICT,
      DIVISION,
      EFFECTIVE_DT,
      ENGINEERING_COMMENTS,
      ENGINEERING_DOCUMENT,
      FEATURE_CLASS_NAME,
      FIRMWARE_VERSION,
      GLOBAL_ID,
      GRD_CUR_TRIP,
      GRD_INST_PICKUP_SETTING,
      GRD_PICKUP_SETTING,
      GRD_TD_LEVER_SETTING,
      GRD_TRIP_CD,
      MANF_CD,
      MASTER_STATION,
      NOTES,
      OK_TO_BYPASS,
      OPERATING_NUM,
      OPERATIONAL_MODE_SWITCH,
      PEER_REVIEW_BY,
      PEER_REVIEW_DT,
      PHA_CUR_TRIP,
      PHA_INST_PICKUP_SETTING,
      PHA_PICKUP_SETTING,
      PHA_TD_LEVER_SETTING,
      PHA_TRIP_CD,
      PREPARED_BY,
      PRIMARY_VOLTAGE,
      PROCESSED_FLAG,
      RADIO_MANF_CD,
      RADIO_MODEL_NUM,
      RADIO_SERIAL_NUM,
      RELAY_TYPE,
      RELEASED_BY,
      REPEATER,
      RTU_ADDRESS,
      SCADA,
      SCADA_TYPE,
      SOFTWARE_VERSION,
      SPECIAL_CONDITIONS,
      TIMESTAMP,
      TRANSMIT_DISABLE_DELAY,
      TRANSMIT_ENABLE_DELAY,
      TYP_CRV_GRD,
      TYP_CRV_PHA
    FROM SM_INTERRUPTER
    WHERE GLOBAL_ID=I_Global_id_Current;
    DELETE FROM SM_INTERRUPTER WHERE GLOBAL_ID = I_Global_id_Current;
    -- Insert a record in comments table with notes set to INST
    -- Insert a record in comments table with notes set to INST
    INSERT
    INTO SM_COMMENT_HIST
      (
        DEVICE_HIST_TABLE_NAME,
        HIST_ID,
        WORK_DATE,
        WORK_TYPE,
        PERFORMED_BY,
        ENTRY_DATE,
        COMMENTS
      )
      VALUES
      (
        'SM_INTERRUPTER_HIST',
        SM_INTERRUPTER_HIST_SEQ.CURRVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
    COMMIT;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_INTERRUPTER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_INTERRUPTER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_INTERRUPTER',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_INTERRUPTER
      SET
        (
          BAUD_RATE,
          CONTROL_TYPE,
          CT_RATIO,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FIRMWARE_VERSION,
          GRD_CUR_TRIP,
          GRD_INST_PICKUP_SETTING,
          GRD_PICKUP_SETTING,
          GRD_TD_LEVER_SETTING,
          GRD_TRIP_CD,
          MANF_CD,
          MASTER_STATION,
          OK_TO_BYPASS,
          OPERATIONAL_MODE_SWITCH,
          PHA_CUR_TRIP,
          PHA_INST_PICKUP_SETTING,
          PHA_PICKUP_SETTING,
          PHA_TD_LEVER_SETTING,
          PHA_TRIP_CD,
          PRIMARY_VOLTAGE,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TYP_CRV_GRD,
          TYP_CRV_PHA
        )
        =
        (SELECT BAUD_RATE,
          CONTROL_TYPE,
          CT_RATIO,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FIRMWARE_VERSION,
          GRD_CUR_TRIP,
          GRD_INST_PICKUP_SETTING,
          GRD_PICKUP_SETTING,
          GRD_TD_LEVER_SETTING,
          GRD_TRIP_CD,
          MANF_CD,
          MASTER_STATION,
          OK_TO_BYPASS,
          OPERATIONAL_MODE_SWITCH,
          PHA_CUR_TRIP,
          PHA_INST_PICKUP_SETTING,
          PHA_PICKUP_SETTING,
          PHA_TD_LEVER_SETTING,
          PHA_TRIP_CD,
          PRIMARY_VOLTAGE,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TYP_CRV_GRD,
          TYP_CRV_PHA
        FROM SM_INTERRUPTER
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_INTERRUPTER_HIST
        (
          BAUD_RATE,
          CONTROL_SERIAL_NUM,
          CONTROL_TYPE,
          CT_RATIO,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_COMMENTS,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          GLOBAL_ID,
          GRD_CUR_TRIP,
          GRD_INST_PICKUP_SETTING,
          GRD_PICKUP_SETTING,
          GRD_TD_LEVER_SETTING,
          GRD_TRIP_CD,
          MANF_CD,
          MASTER_STATION,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          OPERATIONAL_MODE_SWITCH,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHA_CUR_TRIP,
          PHA_INST_PICKUP_SETTING,
          PHA_PICKUP_SETTING,
          PHA_TD_LEVER_SETTING,
          PHA_TRIP_CD,
          PREPARED_BY,
          PRIMARY_VOLTAGE,
          PROCESSED_FLAG,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          TIMESTAMP,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TYP_CRV_GRD,
          TYP_CRV_PHA
        )
      SELECT BAUD_RATE,
        CONTROL_SERIAL_NUM,
        CONTROL_TYPE,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_COMMENTS,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        GLOBAL_ID,
        GRD_CUR_TRIP,
        GRD_INST_PICKUP_SETTING,
        GRD_PICKUP_SETTING,
        GRD_TD_LEVER_SETTING,
        GRD_TRIP_CD,
        MANF_CD,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OPERATIONAL_MODE_SWITCH,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHA_CUR_TRIP,
        PHA_INST_PICKUP_SETTING,
        PHA_PICKUP_SETTING,
        PHA_TD_LEVER_SETTING,
        PHA_TRIP_CD,
        PREPARED_BY,
        PRIMARY_VOLTAGE,
        PROCESSED_FLAG,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        TIMESTAMP,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TYP_CRV_GRD,
        TYP_CRV_PHA
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID=I_Global_id_Current;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_INTERRUPTER_HIST',
          SM_INTERRUPTER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_INTERRUPTER WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_INTERRUPTER_DETECTION;
PROCEDURE SP_NETWORK_PROTECTOR_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  VAR1        VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_NETWORK_PROTECTOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_NETWORK_PROTECTOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_NETWORK_PROTECTOR',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_NETWORK_PROTECTOR_HIST
        (
          ANTI_PUMPING_PROTECTION,
          BAUD_RATE,
          BREAKER_CYCLES_ANTI_PUMPING,
          CLOSING_MODE,
          CONTROL_SERIAL_NUM,
          CT_RATIO,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          FUSE_STYLE_NUM,
          GLOBAL_ID,
          LINE_TO_LINE_SEC_VOLT,
          MASTER_ANGLE,
          MASTER_OFFSET_VOLT,
          MASTER_STATION,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          OVERCUR_INSTANT_TRIP,
          OVERVOLT_PHASING_VOLT,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHASING_ANGLE,
          PHASING_LINE_ADJUSTMENT,
          PHASING_OFFSET_VOLT,
          PREPARED_BY,
          PROCESSED_FLAG,
          PUMP_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RESET_TIME_ANTI_PUMPING,
          REVERSE_TRIP_SETTING,
          ROTATION,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          STRAIGHT_LINE_MASTER_LINE,
          TIMESTAMP,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TRIP_MODE,
          WATT_VAR_TRIP_CHAR
        )
      SELECT ANTI_PUMPING_PROTECTION,
        BAUD_RATE,
        BREAKER_CYCLES_ANTI_PUMPING,
        CLOSING_MODE,
        CONTROL_SERIAL_NUM,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        FUSE_STYLE_NUM,
        GLOBAL_ID,
        LINE_TO_LINE_SEC_VOLT,
        MASTER_ANGLE,
        MASTER_OFFSET_VOLT,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OVERCUR_INSTANT_TRIP,
        OVERVOLT_PHASING_VOLT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHASING_ANGLE,
        PHASING_LINE_ADJUSTMENT,
        PHASING_OFFSET_VOLT,
        PREPARED_BY,
        PROCESSED_FLAG,
        PUMP_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RESET_TIME_ANTI_PUMPING,
        REVERSE_TRIP_SETTING,
        ROTATION,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        STRAIGHT_LINE_MASTER_LINE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TRIP_MODE,
        WATT_VAR_TRIP_CHAR
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID=I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_NETWORK_PROTECTOR_HIST',
          SM_NETWORK_PROTECTOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_NETWORK_PROTECTOR
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    SELECT GLOBAL_ID
    INTO VAR1
    FROM SM_NETWORK_PROTECTOR
    WHERE GLOBAL_ID   = I_Global_id_Current
    AND CURRENT_FUTURE='C';
    -- first copy the entire current record to history table
    INSERT
    INTO SM_NETWORK_PROTECTOR_HIST
      (
        ANTI_PUMPING_PROTECTION,
        BAUD_RATE,
        BREAKER_CYCLES_ANTI_PUMPING,
        CLOSING_MODE,
        CONTROL_SERIAL_NUM,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        FUSE_STYLE_NUM,
        GLOBAL_ID,
        LINE_TO_LINE_SEC_VOLT,
        MASTER_ANGLE,
        MASTER_OFFSET_VOLT,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OVERCUR_INSTANT_TRIP,
        OVERVOLT_PHASING_VOLT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHASING_ANGLE,
        PHASING_LINE_ADJUSTMENT,
        PHASING_OFFSET_VOLT,
        PREPARED_BY,
        PROCESSED_FLAG,
        PUMP_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RESET_TIME_ANTI_PUMPING,
        REVERSE_TRIP_SETTING,
        ROTATION,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        STRAIGHT_LINE_MASTER_LINE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TRIP_MODE,
        WATT_VAR_TRIP_CHAR
      )
    SELECT ANTI_PUMPING_PROTECTION,
      BAUD_RATE,
      BREAKER_CYCLES_ANTI_PUMPING,
      CLOSING_MODE,
      CONTROL_SERIAL_NUM,
      CT_RATIO,
      CURRENT_FUTURE,
      DATE_MODIFIED,
      DEVICE_ID,
      DISTRICT,
      DIVISION,
      EFFECTIVE_DT,
      ENGINEERING_DOCUMENT,
      FEATURE_CLASS_NAME,
      FIRMWARE_VERSION,
      FUSE_STYLE_NUM,
      GLOBAL_ID,
      LINE_TO_LINE_SEC_VOLT,
      MASTER_ANGLE,
      MASTER_OFFSET_VOLT,
      MASTER_STATION,
      NOTES,
      OK_TO_BYPASS,
      OPERATING_NUM,
      OVERCUR_INSTANT_TRIP,
      OVERVOLT_PHASING_VOLT,
      PEER_REVIEW_BY,
      PEER_REVIEW_DT,
      PHASING_ANGLE,
      PHASING_LINE_ADJUSTMENT,
      PHASING_OFFSET_VOLT,
      PREPARED_BY,
      PROCESSED_FLAG,
      PUMP_TIME,
      RADIO_MANF_CD,
      RADIO_MODEL_NUM,
      RADIO_SERIAL_NUM,
      RELAY_TYPE,
      RELEASED_BY,
      REPEATER,
      RESET_TIME_ANTI_PUMPING,
      REVERSE_TRIP_SETTING,
      ROTATION,
      RTU_ADDRESS,
      SCADA,
      SCADA_TYPE,
      SOFTWARE_VERSION,
      SPECIAL_CONDITIONS,
      STRAIGHT_LINE_MASTER_LINE,
      TIMESTAMP,
      TIME_DELAY,
      TRANSMIT_DISABLE_DELAY,
      TRANSMIT_ENABLE_DELAY,
      TRIP_MODE,
      WATT_VAR_TRIP_CHAR
    FROM SM_NETWORK_PROTECTOR
    WHERE GLOBAL_ID=I_Global_id_Current;
    DELETE FROM SM_NETWORK_PROTECTOR WHERE GLOBAL_ID = I_Global_id_Current;
    -- Insert a record in comments table with notes set to INST
    -- Insert a record in comments table with notes set to INST
    INSERT
    INTO SM_COMMENT_HIST
      (
        DEVICE_HIST_TABLE_NAME,
        HIST_ID,
        WORK_DATE,
        WORK_TYPE,
        PERFORMED_BY,
        ENTRY_DATE,
        COMMENTS
      )
      VALUES
      (
        'SM_NETWORK_PROTECTOR_HIST',
        SM_NETWORK_PROTECTOR_HIST_SEQ.CURRVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
    COMMIT;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_NETWORK_PROTECTOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_NETWORK_PROTECTOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_NETWORK_PROTECTOR',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_NETWORK_PROTECTOR
      SET
        (
          ANTI_PUMPING_PROTECTION,
          BAUD_RATE,
          BREAKER_CYCLES_ANTI_PUMPING,
          CLOSING_MODE,
          CT_RATIO,
          ENGINEERING_DOCUMENT,
          FIRMWARE_VERSION,
          FUSE_STYLE_NUM,
          LINE_TO_LINE_SEC_VOLT,
          MASTER_ANGLE,
          MASTER_OFFSET_VOLT,
          MASTER_STATION,
          OK_TO_BYPASS,
          OVERCUR_INSTANT_TRIP,
          OVERVOLT_PHASING_VOLT,
          PHASING_ANGLE,
          PHASING_LINE_ADJUSTMENT,
          PHASING_OFFSET_VOLT,
          PUMP_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RESET_TIME_ANTI_PUMPING,
          REVERSE_TRIP_SETTING,
          ROTATION,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          STRAIGHT_LINE_MASTER_LINE,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TRIP_MODE,
          WATT_VAR_TRIP_CHAR
        )
        =
        (SELECT ANTI_PUMPING_PROTECTION,
          BAUD_RATE,
          BREAKER_CYCLES_ANTI_PUMPING,
          CLOSING_MODE,
          CT_RATIO,
          ENGINEERING_DOCUMENT,
          FIRMWARE_VERSION,
          FUSE_STYLE_NUM,
          LINE_TO_LINE_SEC_VOLT,
          MASTER_ANGLE,
          MASTER_OFFSET_VOLT,
          MASTER_STATION,
          OK_TO_BYPASS,
          OVERCUR_INSTANT_TRIP,
          OVERVOLT_PHASING_VOLT,
          PHASING_ANGLE,
          PHASING_LINE_ADJUSTMENT,
          PHASING_OFFSET_VOLT,
          PUMP_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          REPEATER,
          RESET_TIME_ANTI_PUMPING,
          REVERSE_TRIP_SETTING,
          ROTATION,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          STRAIGHT_LINE_MASTER_LINE,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TRIP_MODE,
          WATT_VAR_TRIP_CHAR
        FROM SM_NETWORK_PROTECTOR
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_NETWORK_PROTECTOR_HIST
        (
          ANTI_PUMPING_PROTECTION,
          BAUD_RATE,
          BREAKER_CYCLES_ANTI_PUMPING,
          CLOSING_MODE,
          CONTROL_SERIAL_NUM,
          CT_RATIO,
          CURRENT_FUTURE,
          DATE_MODIFIED,
          DEVICE_ID,
          DISTRICT,
          DIVISION,
          EFFECTIVE_DT,
          ENGINEERING_DOCUMENT,
          FEATURE_CLASS_NAME,
          FIRMWARE_VERSION,
          FUSE_STYLE_NUM,
          GLOBAL_ID,
          LINE_TO_LINE_SEC_VOLT,
          MASTER_ANGLE,
          MASTER_OFFSET_VOLT,
          MASTER_STATION,
          NOTES,
          OK_TO_BYPASS,
          OPERATING_NUM,
          OVERCUR_INSTANT_TRIP,
          OVERVOLT_PHASING_VOLT,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          PHASING_ANGLE,
          PHASING_LINE_ADJUSTMENT,
          PHASING_OFFSET_VOLT,
          PREPARED_BY,
          PROCESSED_FLAG,
          PUMP_TIME,
          RADIO_MANF_CD,
          RADIO_MODEL_NUM,
          RADIO_SERIAL_NUM,
          RELAY_TYPE,
          RELEASED_BY,
          REPEATER,
          RESET_TIME_ANTI_PUMPING,
          REVERSE_TRIP_SETTING,
          ROTATION,
          RTU_ADDRESS,
          SCADA,
          SCADA_TYPE,
          SOFTWARE_VERSION,
          SPECIAL_CONDITIONS,
          STRAIGHT_LINE_MASTER_LINE,
          TIMESTAMP,
          TIME_DELAY,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          TRIP_MODE,
          WATT_VAR_TRIP_CHAR
        )
      SELECT ANTI_PUMPING_PROTECTION,
        BAUD_RATE,
        BREAKER_CYCLES_ANTI_PUMPING,
        CLOSING_MODE,
        CONTROL_SERIAL_NUM,
        CT_RATIO,
        CURRENT_FUTURE,
        DATE_MODIFIED,
        DEVICE_ID,
        DISTRICT,
        DIVISION,
        EFFECTIVE_DT,
        ENGINEERING_DOCUMENT,
        FEATURE_CLASS_NAME,
        FIRMWARE_VERSION,
        FUSE_STYLE_NUM,
        GLOBAL_ID,
        LINE_TO_LINE_SEC_VOLT,
        MASTER_ANGLE,
        MASTER_OFFSET_VOLT,
        MASTER_STATION,
        NOTES,
        OK_TO_BYPASS,
        OPERATING_NUM,
        OVERCUR_INSTANT_TRIP,
        OVERVOLT_PHASING_VOLT,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        PHASING_ANGLE,
        PHASING_LINE_ADJUSTMENT,
        PHASING_OFFSET_VOLT,
        PREPARED_BY,
        PROCESSED_FLAG,
        PUMP_TIME,
        RADIO_MANF_CD,
        RADIO_MODEL_NUM,
        RADIO_SERIAL_NUM,
        RELAY_TYPE,
        RELEASED_BY,
        REPEATER,
        RESET_TIME_ANTI_PUMPING,
        REVERSE_TRIP_SETTING,
        ROTATION,
        RTU_ADDRESS,
        SCADA,
        SCADA_TYPE,
        SOFTWARE_VERSION,
        SPECIAL_CONDITIONS,
        STRAIGHT_LINE_MASTER_LINE,
        TIMESTAMP,
        TIME_DELAY,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        TRIP_MODE,
        WATT_VAR_TRIP_CHAR
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID=I_Global_id_Current;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_NETWORK_PROTECTOR_HIST',
          SM_NETWORK_PROTECTOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_NETWORK_PROTECTOR WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_NETWORK_PROTECTOR_DETECTION;
PROCEDURE SP_RECLOSER_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2)
AS
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  VAR1        VARCHAR2(50);
  NUM1        NUMBER;
BEGIN
  REASON_TYPE   := I_reason_type ;
  IF REASON_TYPE = 'I' THEN
    BEGIN
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID = I_Global_id_Current;
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_RECLOSER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_RECLOSER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_RECLOSER',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      -- first copy the entire current record to history table
      INSERT
      INTO SM_RECLOSER_HIST
        (
          EFFECTIVE_DT,
          TIMESTAMP,
          DATE_MODIFIED,
          RELAY_TYPE,
          PREPARED_BY,
          DEVICE_ID,
          OPERATING_NUM,
          FEATURE_CLASS_NAME,
          GLOBAL_ID,
          ALT2_PHA_INST_TRIP_CD,
          ALT2_GRD_INST_TRIP_CD,
          ALT2_PHA_MIN_TRIP,
          ALT2_GRD_MIN_TRIP,
          ALT2_LS_RESET_TIME,
          ALT2_LS_LOCKOUT_OPS,
          ALT2_INRUSH_DURATION,
          ALT2_PHA_INRUSH_THRESHOLD,
          ALT2_GRD_INRUSH_THRESHOLD,
          ALT2_PHA_ARMING_THRESHOLD,
          ALT2_GRD_ARMING_THRESHOLD,
          ALT2_PERMIT_LS_ENABLING,
          ALT_COLD_LOAD_PLI_CURVE_GRD,
          ALT_COLD_LOAD_PLI_CURVE_PHA,
          ALT_COLD_LOAD_PLI_GRD,
          ALT_COLD_LOAD_PLI_PHA,
          ALT_COLD_LOAD_PLI_USED,
          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT_HIGH_CURRENT_LOCKOUT_PHA,
          ALT_HIGH_CURRENT_LOCKOUT_USED,
          ALT_SGF_TIME_DELAY,
          ALT_SGF_MIN_TRIP_PERCENT,
          ALT_SGF_CD,
          ALT_RECLOSE_RETRY_ENABLED,
          ALT_RESET,
          ALT_RECLOSE3_TIME,
          ALT_RECLOSE2_TIME,
          ALT_RECLOSE1_TIME,
          ALT_TOT_LOCKOUT_OPS,
          ALT_PHA_TADD_SLOW,
          ALT_GRD_TADD_SLOW,
          ALT_PHA_TADD_FAST,
          ALT_GRD_TADD_FAST,
          ALT_PHA_VMUL_SLOW,
          ALT_GRD_VMUL_SLOW,
          ALT_PHA_VMUL_FAST,
          ALT_GRD_SLOW_CRV_OPS,
          ALT_PHA_SLOW_CRV_OPS,
          ALT_GRD_VMUL_FAST,
          ALT_PHA_SLOW_CRV,
          ALT_GRD_SLOW_CRV,
          ALT_PHA_FAST_CRV,
          ALT_GRD_FAST_CRV,
          ALT_PHA_RESP_TIME,
          ALT_GRD_RESP_TIME,
          ALT_TCC2_SLOW_CURVES_USED,
          ALT_TCC1_FAST_CURVES_USED,
          ALT_PHA_OP_F_CRV,
          ALT_GRD_OP_F_CRV,
          ALT_PHA_INST_TRIP_CD,
          ALT_GRD_INST_TRIP_CD,
          ALT_PHA_MIN_TRIP,
          ALT_GRD_MIN_TRIP,
          COLD_LOAD_PLI_CURVE_GRD,
          COLD_LOAD_PLI_CURVE_PHA,
          COLD_LOAD_PLI_GRD,
          COLD_LOAD_PLI_PHA,
          COLD_LOAD_PLI_USED,
          HIGH_CURRENT_LOCKUOUT_GRD,
          HIGH_CURRENT_LOCKOUT_PHA,
          HIGH_CURRENT_LOCKOUT_USED,
          SGF_TIME_DELAY,
          SGF_MIN_TRIP_PERCENT,
          SGF_CD,
          RECLOSE_RETRY_ENABLED,
          RESET,
          RECLOSE3_TIME,
          RECLOSE2_TIME,
          RECLOSE1_TIME,
          TOT_LOCKOUT_OPS,
          PHA_TADD_SLOW,
          GRD_TADD_SLOW,
          PHA_TADD_FAST,
          GRD_TADD_FAST,
          PHA_VMUL_SLOW,
          GRD_VMUL_SLOW,
          PHA_VMUL_FAST,
          GRD_VMUL_FAST,
          GRD_SLOW_CRV_OPS,
          PHA_SLOW_CRV_OPS,
          PHA_SLOW_CRV,
          GRD_SLOW_CRV,
          PHA_FAST_CRV,
          GRD_FAST_CRV,
          PHA_RESP_TIME,
          GRD_RESP_TIME,
          PHA_OP_F_CRV,
          GRD_OP_F_CRV,
          TCC2_SLOW_CURVES_USED,
          TCC1_FAST_CURVES_USED,
          PHA_INST_TRIP_CD,
          GRD_INST_TRIP_CD,
          PHA_MIN_TRIP,
          GRD_MIN_TRIP,
          OPERATING_AS_CD,
          BYPASS_PLANS,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          RELEASED_BY,
          PROCESSED_FLAG,
          CONTROL_SERIAL_NUM,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          CURRENT_FUTURE,
          NOTES,
          DISTRICT,
          DIVISION,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          FLISR_ENGINEERING_COMMENTS,
          "Limiting_Factor",
          "Winter_Load_Limit",
          "Summer_Load_Limit",
          FLISR,
          ENGINEERING_COMMENTS,
          ACTIVE_PROFILE,
          ALT3_COLD_LOAD_PLI_CURVE_GRD,
          ALT3_COLD_LOAD_PLI_CURVE_PHA,
          ALT3_COLD_LOAD_PLI_GRD,
          ALT3_COLD_LOAD_PLI_PHA,
          ALT3_COLD_LOAD_PLI_USED,
          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
          ALT3_HIGH_CURRENT_LOCKOUT_USED,
          ALT3_SGF_TIME_DELAY,
          ALT3_SGF_MIN_TRIP_PERCENT,
          ALT3_SGF_CD,
          ALT3_RECLOSE_RETRY_ENABLED,
          ALT3_RESET,
          ALT3_RECLOSE3_TIME,
          ALT3_RECLOSE2_TIME,
          ALT3_RECLOSE1_TIME,
          ALT3_TOT_LOCKOUT_OPS,
          ALT3_GRD_DELAY,
          ALT3_PHA_DELAY,
          ALT3_PHA_TADD_SLOW,
          ALT3_GRD_TADD_SLOW,
          ALT3_PHA_TADD_FAST,
          ALT3_GRD_TADD_FAST,
          ALT3_PHA_VMUL_SLOW,
          ALT3_GRD_VMUL_SLOW,
          ALT3_PHA_VMUL_FAST,
          ALT3_GRD_SLOW_CRV_OPS,
          ALT3_PHA_SLOW_CRV_OPS,
          ALT3_GRD_VMUL_FAST,
          ALT3_PHA_SLOW_CRV,
          ALT3_GRD_SLOW_CRV,
          ALT3_PHA_FAST_CRV,
          ALT3_GRD_FAST_CRV,
          ALT3_TCC2_SLOW_CURVES_USED,
          ALT3_TCC1_FAST_CURVES_USED,
          ALT3_PHA_OP_F_CRV,
          ALT3_GRD_OP_F_CRV,
          ALT3_PHA_INST_TRIP_CD,
          ALT3_GRD_INST_TRIP_CD,
          ALT3_PHA_MIN_TRIP,
          ALT3_GRD_MIN_TRIP,
          ALT2_PHA_TADD_FAST,
          ALT2_GRD_TADD_FAST,
          ALT2_GRD_VMUL_SLOW,
          ALT2_PHA_VMUL_FAST,
          ALT2_PHA_FAST_CRV,
          ALT2_GRD_FAST_CRV
        )
      SELECT EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        RELAY_TYPE,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        FEATURE_CLASS_NAME,
        GLOBAL_ID,
        ALT2_PHA_INST_TRIP_CD,
        ALT2_GRD_INST_TRIP_CD,
        ALT2_PHA_MIN_TRIP,
        ALT2_GRD_MIN_TRIP,
        ALT2_LS_RESET_TIME,
        ALT2_LS_LOCKOUT_OPS,
        ALT2_INRUSH_DURATION,
        ALT2_PHA_INRUSH_THRESHOLD,
        ALT2_GRD_INRUSH_THRESHOLD,
        ALT2_PHA_ARMING_THRESHOLD,
        ALT2_GRD_ARMING_THRESHOLD,
        ALT2_PERMIT_LS_ENABLING,
        ALT_COLD_LOAD_PLI_CURVE_GRD,
        ALT_COLD_LOAD_PLI_CURVE_PHA,
        ALT_COLD_LOAD_PLI_GRD,
        ALT_COLD_LOAD_PLI_PHA,
        ALT_COLD_LOAD_PLI_USED,
        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT_HIGH_CURRENT_LOCKOUT_PHA,
        ALT_HIGH_CURRENT_LOCKOUT_USED,
        ALT_SGF_TIME_DELAY,
        ALT_SGF_MIN_TRIP_PERCENT,
        ALT_SGF_CD,
        ALT_RECLOSE_RETRY_ENABLED,
        ALT_RESET,
        ALT_RECLOSE3_TIME,
        ALT_RECLOSE2_TIME,
        ALT_RECLOSE1_TIME,
        ALT_TOT_LOCKOUT_OPS,
        ALT_PHA_TADD_SLOW,
        ALT_GRD_TADD_SLOW,
        ALT_PHA_TADD_FAST,
        ALT_GRD_TADD_FAST,
        ALT_PHA_VMUL_SLOW,
        ALT_GRD_VMUL_SLOW,
        ALT_PHA_VMUL_FAST,
        ALT_GRD_SLOW_CRV_OPS,
        ALT_PHA_SLOW_CRV_OPS,
        ALT_GRD_VMUL_FAST,
        ALT_PHA_SLOW_CRV,
        ALT_GRD_SLOW_CRV,
        ALT_PHA_FAST_CRV,
        ALT_GRD_FAST_CRV,
        ALT_PHA_RESP_TIME,
        ALT_GRD_RESP_TIME,
        ALT_TCC2_SLOW_CURVES_USED,
        ALT_TCC1_FAST_CURVES_USED,
        ALT_PHA_OP_F_CRV,
        ALT_GRD_OP_F_CRV,
        ALT_PHA_INST_TRIP_CD,
        ALT_GRD_INST_TRIP_CD,
        ALT_PHA_MIN_TRIP,
        ALT_GRD_MIN_TRIP,
        COLD_LOAD_PLI_CURVE_GRD,
        COLD_LOAD_PLI_CURVE_PHA,
        COLD_LOAD_PLI_GRD,
        COLD_LOAD_PLI_PHA,
        COLD_LOAD_PLI_USED,
        HIGH_CURRENT_LOCKUOUT_GRD,
        HIGH_CURRENT_LOCKOUT_PHA,
        HIGH_CURRENT_LOCKOUT_USED,
        SGF_TIME_DELAY,
        SGF_MIN_TRIP_PERCENT,
        SGF_CD,
        RECLOSE_RETRY_ENABLED,
        RESET,
        RECLOSE3_TIME,
        RECLOSE2_TIME,
        RECLOSE1_TIME,
        TOT_LOCKOUT_OPS,
        PHA_TADD_SLOW,
        GRD_TADD_SLOW,
        PHA_TADD_FAST,
        GRD_TADD_FAST,
        PHA_VMUL_SLOW,
        GRD_VMUL_SLOW,
        PHA_VMUL_FAST,
        GRD_VMUL_FAST,
        GRD_SLOW_CRV_OPS,
        PHA_SLOW_CRV_OPS,
        PHA_SLOW_CRV,
        GRD_SLOW_CRV,
        PHA_FAST_CRV,
        GRD_FAST_CRV,
        PHA_RESP_TIME,
        GRD_RESP_TIME,
        PHA_OP_F_CRV,
        GRD_OP_F_CRV,
        TCC2_SLOW_CURVES_USED,
        TCC1_FAST_CURVES_USED,
        PHA_INST_TRIP_CD,
        GRD_INST_TRIP_CD,
        PHA_MIN_TRIP,
        GRD_MIN_TRIP,
        OPERATING_AS_CD,
        BYPASS_PLANS,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
       FLISR_ENGINEERING_COMMENTS,
        "Limiting_Factor",
        "Winter_Load_Limit",
        "Summer_Load_Limit",
        FLISR,
        ENGINEERING_COMMENTS,
        ACTIVE_PROFILE,
        ALT3_COLD_LOAD_PLI_CURVE_GRD,
        ALT3_COLD_LOAD_PLI_CURVE_PHA,
        ALT3_COLD_LOAD_PLI_GRD,
        ALT3_COLD_LOAD_PLI_PHA,
        ALT3_COLD_LOAD_PLI_USED,
        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
        ALT3_HIGH_CURRENT_LOCKOUT_USED,
        ALT3_SGF_TIME_DELAY,
        ALT3_SGF_MIN_TRIP_PERCENT,
        ALT3_SGF_CD,
        ALT3_RECLOSE_RETRY_ENABLED,
        ALT3_RESET,
        ALT3_RECLOSE3_TIME,
        ALT3_RECLOSE2_TIME,
        ALT3_RECLOSE1_TIME,
        ALT3_TOT_LOCKOUT_OPS,
        ALT3_GRD_DELAY,
        ALT3_PHA_DELAY,
        ALT3_PHA_TADD_SLOW,
        ALT3_GRD_TADD_SLOW,
        ALT3_PHA_TADD_FAST,
        ALT3_GRD_TADD_FAST,
        ALT3_PHA_VMUL_SLOW,
        ALT3_GRD_VMUL_SLOW,
        ALT3_PHA_VMUL_FAST,
        ALT3_GRD_SLOW_CRV_OPS,
        ALT3_PHA_SLOW_CRV_OPS,
        ALT3_GRD_VMUL_FAST,
        ALT3_PHA_SLOW_CRV,
        ALT3_GRD_SLOW_CRV,
        ALT3_PHA_FAST_CRV,
        ALT3_GRD_FAST_CRV,
        ALT3_TCC2_SLOW_CURVES_USED,
        ALT3_TCC1_FAST_CURVES_USED,
        ALT3_PHA_OP_F_CRV,
        ALT3_GRD_OP_F_CRV,
        ALT3_PHA_INST_TRIP_CD,
        ALT3_GRD_INST_TRIP_CD,
        ALT3_PHA_MIN_TRIP,
        ALT3_GRD_MIN_TRIP,
        ALT2_PHA_TADD_FAST,
        ALT2_GRD_TADD_FAST,
        ALT2_GRD_VMUL_SLOW,
        ALT2_PHA_VMUL_FAST,
        ALT2_PHA_FAST_CRV,
        ALT2_GRD_FAST_CRV
      FROM SM_RECLOSER
      WHERE GLOBAL_ID=I_Global_id_Current;
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_RECLOSER_HIST',
          SM_RECLOSER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      UPDATE SM_RECLOSER
      SET OPERATING_NUM = I_operating_num,
        DIVISION        =I_Division,
        DISTRICT        = I_District,
        CONTROL_TYPE    =I_Control_type_code
      WHERE GLOBAL_ID   = I_Global_id_Current;
      COMMIT;
    END;
  END IF;
  IF REASON_TYPE = 'D' THEN
    SELECT GLOBAL_ID
    INTO VAR1
    FROM SM_RECLOSER
    WHERE GLOBAL_ID   = I_Global_id_Current
    AND CURRENT_FUTURE='C';
    -- first copy the entire current record to history table
    INSERT
    INTO SM_RECLOSER_HIST
      (
        EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        RELAY_TYPE,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        FEATURE_CLASS_NAME,
        GLOBAL_ID,
        ALT2_PHA_INST_TRIP_CD,
        ALT2_GRD_INST_TRIP_CD,
        ALT2_PHA_MIN_TRIP,
        ALT2_GRD_MIN_TRIP,
        ALT2_LS_RESET_TIME,
        ALT2_LS_LOCKOUT_OPS,
        ALT2_INRUSH_DURATION,
        ALT2_PHA_INRUSH_THRESHOLD,
        ALT2_GRD_INRUSH_THRESHOLD,
        ALT2_PHA_ARMING_THRESHOLD,
        ALT2_GRD_ARMING_THRESHOLD,
        ALT2_PERMIT_LS_ENABLING,
        ALT_COLD_LOAD_PLI_CURVE_GRD,
        ALT_COLD_LOAD_PLI_CURVE_PHA,
        ALT_COLD_LOAD_PLI_GRD,
        ALT_COLD_LOAD_PLI_PHA,
        ALT_COLD_LOAD_PLI_USED,
        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT_HIGH_CURRENT_LOCKOUT_PHA,
        ALT_HIGH_CURRENT_LOCKOUT_USED,
        ALT_SGF_TIME_DELAY,
        ALT_SGF_MIN_TRIP_PERCENT,
        ALT_SGF_CD,
        ALT_RECLOSE_RETRY_ENABLED,
        ALT_RESET,
        ALT_RECLOSE3_TIME,
        ALT_RECLOSE2_TIME,
        ALT_RECLOSE1_TIME,
        ALT_TOT_LOCKOUT_OPS,
        ALT_PHA_TADD_SLOW,
        ALT_GRD_TADD_SLOW,
        ALT_PHA_TADD_FAST,
        ALT_GRD_TADD_FAST,
        ALT_PHA_VMUL_SLOW,
        ALT_GRD_VMUL_SLOW,
        ALT_PHA_VMUL_FAST,
        ALT_GRD_SLOW_CRV_OPS,
        ALT_PHA_SLOW_CRV_OPS,
        ALT_GRD_VMUL_FAST,
        ALT_PHA_SLOW_CRV,
        ALT_GRD_SLOW_CRV,
        ALT_PHA_FAST_CRV,
        ALT_GRD_FAST_CRV,
        ALT_PHA_RESP_TIME,
        ALT_GRD_RESP_TIME,
        ALT_TCC2_SLOW_CURVES_USED,
        ALT_TCC1_FAST_CURVES_USED,
        ALT_PHA_OP_F_CRV,
        ALT_GRD_OP_F_CRV,
        ALT_PHA_INST_TRIP_CD,
        ALT_GRD_INST_TRIP_CD,
        ALT_PHA_MIN_TRIP,
        ALT_GRD_MIN_TRIP,
        COLD_LOAD_PLI_CURVE_GRD,
        COLD_LOAD_PLI_CURVE_PHA,
        COLD_LOAD_PLI_GRD,
        COLD_LOAD_PLI_PHA,
        COLD_LOAD_PLI_USED,
        HIGH_CURRENT_LOCKUOUT_GRD,
        HIGH_CURRENT_LOCKOUT_PHA,
        HIGH_CURRENT_LOCKOUT_USED,
        SGF_TIME_DELAY,
        SGF_MIN_TRIP_PERCENT,
        SGF_CD,
        RECLOSE_RETRY_ENABLED,
        RESET,
        RECLOSE3_TIME,
        RECLOSE2_TIME,
        RECLOSE1_TIME,
        TOT_LOCKOUT_OPS,
        PHA_TADD_SLOW,
        GRD_TADD_SLOW,
        PHA_TADD_FAST,
        GRD_TADD_FAST,
        PHA_VMUL_SLOW,
        GRD_VMUL_SLOW,
        PHA_VMUL_FAST,
        GRD_VMUL_FAST,
        GRD_SLOW_CRV_OPS,
        PHA_SLOW_CRV_OPS,
        PHA_SLOW_CRV,
        GRD_SLOW_CRV,
        PHA_FAST_CRV,
        GRD_FAST_CRV,
        PHA_RESP_TIME,
        GRD_RESP_TIME,
        PHA_OP_F_CRV,
        GRD_OP_F_CRV,
        TCC2_SLOW_CURVES_USED,
        TCC1_FAST_CURVES_USED,
        PHA_INST_TRIP_CD,
        GRD_INST_TRIP_CD,
        PHA_MIN_TRIP,
        GRD_MIN_TRIP,
        OPERATING_AS_CD,
        BYPASS_PLANS,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
        FLISR_ENGINEERING_COMMENTS,
        "Limiting_Factor",
        "Winter_Load_Limit",
        "Summer_Load_Limit",
        FLISR,
        ENGINEERING_COMMENTS,
        ACTIVE_PROFILE,
        ALT3_COLD_LOAD_PLI_CURVE_GRD,
        ALT3_COLD_LOAD_PLI_CURVE_PHA,
        ALT3_COLD_LOAD_PLI_GRD,
        ALT3_COLD_LOAD_PLI_PHA,
        ALT3_COLD_LOAD_PLI_USED,
        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
        ALT3_HIGH_CURRENT_LOCKOUT_USED,
        ALT3_SGF_TIME_DELAY,
        ALT3_SGF_MIN_TRIP_PERCENT,
        ALT3_SGF_CD,
        ALT3_RECLOSE_RETRY_ENABLED,
        ALT3_RESET,
        ALT3_RECLOSE3_TIME,
        ALT3_RECLOSE2_TIME,
        ALT3_RECLOSE1_TIME,
        ALT3_TOT_LOCKOUT_OPS,
        ALT3_GRD_DELAY,
        ALT3_PHA_DELAY,
        ALT3_PHA_TADD_SLOW,
        ALT3_GRD_TADD_SLOW,
        ALT3_PHA_TADD_FAST,
        ALT3_GRD_TADD_FAST,
        ALT3_PHA_VMUL_SLOW,
        ALT3_GRD_VMUL_SLOW,
        ALT3_PHA_VMUL_FAST,
        ALT3_GRD_SLOW_CRV_OPS,
        ALT3_PHA_SLOW_CRV_OPS,
        ALT3_GRD_VMUL_FAST,
        ALT3_PHA_SLOW_CRV,
        ALT3_GRD_SLOW_CRV,
        ALT3_PHA_FAST_CRV,
        ALT3_GRD_FAST_CRV,
        ALT3_TCC2_SLOW_CURVES_USED,
        ALT3_TCC1_FAST_CURVES_USED,
        ALT3_PHA_OP_F_CRV,
        ALT3_GRD_OP_F_CRV,
        ALT3_PHA_INST_TRIP_CD,
        ALT3_GRD_INST_TRIP_CD,
        ALT3_PHA_MIN_TRIP,
        ALT3_GRD_MIN_TRIP,
        ALT2_PHA_TADD_FAST,
        ALT2_GRD_TADD_FAST,
        ALT2_GRD_VMUL_SLOW,
        ALT2_PHA_VMUL_FAST,
        ALT2_PHA_FAST_CRV,
        ALT2_GRD_FAST_CRV
      )
    SELECT EFFECTIVE_DT,
      TIMESTAMP,
      DATE_MODIFIED,
      RELAY_TYPE,
      PREPARED_BY,
      DEVICE_ID,
      OPERATING_NUM,
      FEATURE_CLASS_NAME,
      GLOBAL_ID,
      ALT2_PHA_INST_TRIP_CD,
      ALT2_GRD_INST_TRIP_CD,
      ALT2_PHA_MIN_TRIP,
      ALT2_GRD_MIN_TRIP,
      ALT2_LS_RESET_TIME,
      ALT2_LS_LOCKOUT_OPS,
      ALT2_INRUSH_DURATION,
      ALT2_PHA_INRUSH_THRESHOLD,
      ALT2_GRD_INRUSH_THRESHOLD,
      ALT2_PHA_ARMING_THRESHOLD,
      ALT2_GRD_ARMING_THRESHOLD,
      ALT2_PERMIT_LS_ENABLING,
      ALT_COLD_LOAD_PLI_CURVE_GRD,
      ALT_COLD_LOAD_PLI_CURVE_PHA,
      ALT_COLD_LOAD_PLI_GRD,
      ALT_COLD_LOAD_PLI_PHA,
      ALT_COLD_LOAD_PLI_USED,
      ALT_HIGH_CURRENT_LOCKUOUT_GRD,
      ALT_HIGH_CURRENT_LOCKOUT_PHA,
      ALT_HIGH_CURRENT_LOCKOUT_USED,
      ALT_SGF_TIME_DELAY,
      ALT_SGF_MIN_TRIP_PERCENT,
      ALT_SGF_CD,
      ALT_RECLOSE_RETRY_ENABLED,
      ALT_RESET,
      ALT_RECLOSE3_TIME,
      ALT_RECLOSE2_TIME,
      ALT_RECLOSE1_TIME,
      ALT_TOT_LOCKOUT_OPS,
      ALT_PHA_TADD_SLOW,
      ALT_GRD_TADD_SLOW,
      ALT_PHA_TADD_FAST,
      ALT_GRD_TADD_FAST,
      ALT_PHA_VMUL_SLOW,
      ALT_GRD_VMUL_SLOW,
      ALT_PHA_VMUL_FAST,
      ALT_GRD_SLOW_CRV_OPS,
      ALT_PHA_SLOW_CRV_OPS,
      ALT_GRD_VMUL_FAST,
      ALT_PHA_SLOW_CRV,
      ALT_GRD_SLOW_CRV,
      ALT_PHA_FAST_CRV,
      ALT_GRD_FAST_CRV,
      ALT_PHA_RESP_TIME,
      ALT_GRD_RESP_TIME,
      ALT_TCC2_SLOW_CURVES_USED,
      ALT_TCC1_FAST_CURVES_USED,
      ALT_PHA_OP_F_CRV,
      ALT_GRD_OP_F_CRV,
      ALT_PHA_INST_TRIP_CD,
      ALT_GRD_INST_TRIP_CD,
      ALT_PHA_MIN_TRIP,
      ALT_GRD_MIN_TRIP,
      COLD_LOAD_PLI_CURVE_GRD,
      COLD_LOAD_PLI_CURVE_PHA,
      COLD_LOAD_PLI_GRD,
      COLD_LOAD_PLI_PHA,
      COLD_LOAD_PLI_USED,
      HIGH_CURRENT_LOCKUOUT_GRD,
      HIGH_CURRENT_LOCKOUT_PHA,
      HIGH_CURRENT_LOCKOUT_USED,
      SGF_TIME_DELAY,
      SGF_MIN_TRIP_PERCENT,
      SGF_CD,
      RECLOSE_RETRY_ENABLED,
      RESET,
      RECLOSE3_TIME,
      RECLOSE2_TIME,
      RECLOSE1_TIME,
      TOT_LOCKOUT_OPS,
      PHA_TADD_SLOW,
      GRD_TADD_SLOW,
      PHA_TADD_FAST,
      GRD_TADD_FAST,
      PHA_VMUL_SLOW,
      GRD_VMUL_SLOW,
      PHA_VMUL_FAST,
      GRD_VMUL_FAST,
      GRD_SLOW_CRV_OPS,
      PHA_SLOW_CRV_OPS,
      PHA_SLOW_CRV,
      GRD_SLOW_CRV,
      PHA_FAST_CRV,
      GRD_FAST_CRV,
      PHA_RESP_TIME,
      GRD_RESP_TIME,
      PHA_OP_F_CRV,
      GRD_OP_F_CRV,
      TCC2_SLOW_CURVES_USED,
      TCC1_FAST_CURVES_USED,
      PHA_INST_TRIP_CD,
      GRD_INST_TRIP_CD,
      PHA_MIN_TRIP,
      GRD_MIN_TRIP,
      OPERATING_AS_CD,
      BYPASS_PLANS,
      CONTROL_TYPE,
      OK_TO_BYPASS,
      RELEASED_BY,
      PROCESSED_FLAG,
      CONTROL_SERIAL_NUM,
      SOFTWARE_VERSION,
      FIRMWARE_VERSION,
      CURRENT_FUTURE,
      NOTES,
      DISTRICT,
      DIVISION,
      PEER_REVIEW_BY,
      PEER_REVIEW_DT,
      RADIO_SERIAL_NUM,
      RADIO_MODEL_NUM,
      RADIO_MANF_CD,
      SPECIAL_CONDITIONS,
      REPEATER,
      RTU_ADDRESS,
      TRANSMIT_DISABLE_DELAY,
      TRANSMIT_ENABLE_DELAY,
      BAUD_RATE,
      MASTER_STATION,
      SCADA_TYPE,
      SCADA,
      ENGINEERING_DOCUMENT,
      FLISR_ENGINEERING_COMMENTS,
      "Limiting_Factor",
      "Winter_Load_Limit",
      "Summer_Load_Limit",
      FLISR,
      ENGINEERING_COMMENTS,
      ACTIVE_PROFILE,
      ALT3_COLD_LOAD_PLI_CURVE_GRD,
      ALT3_COLD_LOAD_PLI_CURVE_PHA,
      ALT3_COLD_LOAD_PLI_GRD,
      ALT3_COLD_LOAD_PLI_PHA,
      ALT3_COLD_LOAD_PLI_USED,
      ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
      ALT3_HIGH_CURRENT_LOCKOUT_PHA,
      ALT3_HIGH_CURRENT_LOCKOUT_USED,
      ALT3_SGF_TIME_DELAY,
      ALT3_SGF_MIN_TRIP_PERCENT,
      ALT3_SGF_CD,
      ALT3_RECLOSE_RETRY_ENABLED,
      ALT3_RESET,
      ALT3_RECLOSE3_TIME,
      ALT3_RECLOSE2_TIME,
      ALT3_RECLOSE1_TIME,
      ALT3_TOT_LOCKOUT_OPS,
      ALT3_GRD_DELAY,
      ALT3_PHA_DELAY,
      ALT3_PHA_TADD_SLOW,
      ALT3_GRD_TADD_SLOW,
      ALT3_PHA_TADD_FAST,
      ALT3_GRD_TADD_FAST,
      ALT3_PHA_VMUL_SLOW,
      ALT3_GRD_VMUL_SLOW,
      ALT3_PHA_VMUL_FAST,
      ALT3_GRD_SLOW_CRV_OPS,
      ALT3_PHA_SLOW_CRV_OPS,
      ALT3_GRD_VMUL_FAST,
      ALT3_PHA_SLOW_CRV,
      ALT3_GRD_SLOW_CRV,
      ALT3_PHA_FAST_CRV,
      ALT3_GRD_FAST_CRV,
      ALT3_TCC2_SLOW_CURVES_USED,
      ALT3_TCC1_FAST_CURVES_USED,
      ALT3_PHA_OP_F_CRV,
      ALT3_GRD_OP_F_CRV,
      ALT3_PHA_INST_TRIP_CD,
      ALT3_GRD_INST_TRIP_CD,
      ALT3_PHA_MIN_TRIP,
      ALT3_GRD_MIN_TRIP,
      ALT2_PHA_TADD_FAST,
      ALT2_GRD_TADD_FAST,
      ALT2_GRD_VMUL_SLOW,
      ALT2_PHA_VMUL_FAST,
      ALT2_PHA_FAST_CRV,
      ALT2_GRD_FAST_CRV
    FROM SM_RECLOSER
    WHERE GLOBAL_ID=I_Global_id_Current;
    DELETE FROM SM_RECLOSER WHERE GLOBAL_ID = I_Global_id_Current;
    -- Insert a record in comments table with notes set to INST
    -- Insert a record in comments table with notes set to INST
    INSERT
    INTO SM_COMMENT_HIST
      (
        DEVICE_HIST_TABLE_NAME,
        HIST_ID,
        WORK_DATE,
        WORK_TYPE,
        PERFORMED_BY,
        ENTRY_DATE,
        COMMENTS
      )
      VALUES
      (
        'SM_RECLOSER_HIST',
        SM_RECLOSER_HIST_SEQ.CURRVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
    COMMIT;
  END IF;
  IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Raise REC_FOUND exception if the current global id already exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_RECLOSER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_RECLOSER
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to OTHR
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_RECLOSER',
          I_Global_id_Current,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      UPDATE SM_RECLOSER
      SET
        (
          RELAY_TYPE,
          ALT2_PHA_INST_TRIP_CD,
          ALT2_GRD_INST_TRIP_CD,
          ALT2_PHA_MIN_TRIP,
          ALT2_GRD_MIN_TRIP,
          ALT2_LS_RESET_TIME,
          ALT2_LS_LOCKOUT_OPS,
          ALT2_INRUSH_DURATION,
          ALT2_PHA_INRUSH_THRESHOLD,
          ALT2_GRD_INRUSH_THRESHOLD,
          ALT2_PHA_ARMING_THRESHOLD,
          ALT2_GRD_ARMING_THRESHOLD,
          ALT2_PERMIT_LS_ENABLING,
          ALT_COLD_LOAD_PLI_CURVE_GRD,
          ALT_COLD_LOAD_PLI_CURVE_PHA,
          ALT_COLD_LOAD_PLI_GRD,
          ALT_COLD_LOAD_PLI_PHA,
          ALT_COLD_LOAD_PLI_USED,
          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT_HIGH_CURRENT_LOCKOUT_PHA,
          ALT_HIGH_CURRENT_LOCKOUT_USED,
          ALT_SGF_TIME_DELAY,
          ALT_SGF_MIN_TRIP_PERCENT,
          ALT_SGF_CD,
          ALT_RECLOSE_RETRY_ENABLED,
          ALT_RESET,
          ALT_RECLOSE3_TIME,
          ALT_RECLOSE2_TIME,
          ALT_RECLOSE1_TIME,
          ALT_TOT_LOCKOUT_OPS,
          ALT_PHA_TADD_SLOW,
          ALT_GRD_TADD_SLOW,
          ALT_PHA_TADD_FAST,
          ALT_GRD_TADD_FAST,
          ALT_PHA_VMUL_SLOW,
          ALT_GRD_VMUL_SLOW,
          ALT_PHA_VMUL_FAST,
          ALT_GRD_SLOW_CRV_OPS,
          ALT_PHA_SLOW_CRV_OPS,
          ALT_GRD_VMUL_FAST,
          ALT_PHA_SLOW_CRV,
          ALT_GRD_SLOW_CRV,
          ALT_PHA_FAST_CRV,
          ALT_GRD_FAST_CRV,
          ALT_PHA_RESP_TIME,
          ALT_GRD_RESP_TIME,
          ALT_TCC2_SLOW_CURVES_USED,
          ALT_TCC1_FAST_CURVES_USED,
          ALT_PHA_OP_F_CRV,
          ALT_GRD_OP_F_CRV,
          ALT_PHA_INST_TRIP_CD,
          ALT_GRD_INST_TRIP_CD,
          ALT_PHA_MIN_TRIP,
          ALT_GRD_MIN_TRIP,
          COLD_LOAD_PLI_CURVE_GRD,
          COLD_LOAD_PLI_CURVE_PHA,
          COLD_LOAD_PLI_GRD,
          COLD_LOAD_PLI_PHA,
          COLD_LOAD_PLI_USED,
          HIGH_CURRENT_LOCKUOUT_GRD,
          HIGH_CURRENT_LOCKOUT_PHA,
          HIGH_CURRENT_LOCKOUT_USED,
          SGF_TIME_DELAY,
          SGF_MIN_TRIP_PERCENT,
          SGF_CD,
          RECLOSE_RETRY_ENABLED,
          RESET,
          RECLOSE3_TIME,
          RECLOSE2_TIME,
          RECLOSE1_TIME,
          TOT_LOCKOUT_OPS,
          PHA_TADD_SLOW,
          GRD_TADD_SLOW,
          PHA_TADD_FAST,
          GRD_TADD_FAST,
          PHA_VMUL_SLOW,
          GRD_VMUL_SLOW,
          PHA_VMUL_FAST,
          GRD_VMUL_FAST,
          GRD_SLOW_CRV_OPS,
          PHA_SLOW_CRV_OPS,
          PHA_SLOW_CRV,
          GRD_SLOW_CRV,
          PHA_FAST_CRV,
          GRD_FAST_CRV,
          PHA_RESP_TIME,
          GRD_RESP_TIME,
          PHA_OP_F_CRV,
          GRD_OP_F_CRV,
          TCC2_SLOW_CURVES_USED,
          TCC1_FAST_CURVES_USED,
          PHA_INST_TRIP_CD,
          GRD_INST_TRIP_CD,
          PHA_MIN_TRIP,
          GRD_MIN_TRIP,
          OPERATING_AS_CD,
          BYPASS_PLANS,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          FLISR_ENGINEERING_COMMENTS,
          "Limiting_Factor",
          "Winter_Load_Limit",
          "Summer_Load_Limit",
          FLISR,
          ENGINEERING_COMMENTS,
          ACTIVE_PROFILE,
          ALT3_COLD_LOAD_PLI_CURVE_GRD,
          ALT3_COLD_LOAD_PLI_CURVE_PHA,
          ALT3_COLD_LOAD_PLI_GRD,
          ALT3_COLD_LOAD_PLI_PHA,
          ALT3_COLD_LOAD_PLI_USED,
          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
          ALT3_HIGH_CURRENT_LOCKOUT_USED,
          ALT3_SGF_TIME_DELAY,
          ALT3_SGF_MIN_TRIP_PERCENT,
          ALT3_SGF_CD,
          ALT3_RECLOSE_RETRY_ENABLED,
          ALT3_RESET,
          ALT3_RECLOSE3_TIME,
          ALT3_RECLOSE2_TIME,
          ALT3_RECLOSE1_TIME,
          ALT3_TOT_LOCKOUT_OPS,
          ALT3_GRD_DELAY,
          ALT3_PHA_DELAY,
          ALT3_PHA_TADD_SLOW,
          ALT3_GRD_TADD_SLOW,
          ALT3_PHA_TADD_FAST,
          ALT3_GRD_TADD_FAST,
          ALT3_PHA_VMUL_SLOW,
          ALT3_GRD_VMUL_SLOW,
          ALT3_PHA_VMUL_FAST,
          ALT3_GRD_SLOW_CRV_OPS,
          ALT3_PHA_SLOW_CRV_OPS,
          ALT3_GRD_VMUL_FAST,
          ALT3_PHA_SLOW_CRV,
          ALT3_GRD_SLOW_CRV,
          ALT3_PHA_FAST_CRV,
          ALT3_GRD_FAST_CRV,
          ALT3_TCC2_SLOW_CURVES_USED,
          ALT3_TCC1_FAST_CURVES_USED,
          ALT3_PHA_OP_F_CRV,
          ALT3_GRD_OP_F_CRV,
          ALT3_PHA_INST_TRIP_CD,
          ALT3_GRD_INST_TRIP_CD,
          ALT3_PHA_MIN_TRIP,
          ALT3_GRD_MIN_TRIP,
          ALT2_PHA_TADD_FAST,
          ALT2_GRD_TADD_FAST,
          ALT2_GRD_VMUL_SLOW,
          ALT2_PHA_VMUL_FAST,
          ALT2_PHA_FAST_CRV,
          ALT2_GRD_FAST_CRV
        )
        =
        (SELECT RELAY_TYPE,
          ALT2_PHA_INST_TRIP_CD,
          ALT2_GRD_INST_TRIP_CD,
          ALT2_PHA_MIN_TRIP,
          ALT2_GRD_MIN_TRIP,
          ALT2_LS_RESET_TIME,
          ALT2_LS_LOCKOUT_OPS,
          ALT2_INRUSH_DURATION,
          ALT2_PHA_INRUSH_THRESHOLD,
          ALT2_GRD_INRUSH_THRESHOLD,
          ALT2_PHA_ARMING_THRESHOLD,
          ALT2_GRD_ARMING_THRESHOLD,
          ALT2_PERMIT_LS_ENABLING,
          ALT_COLD_LOAD_PLI_CURVE_GRD,
          ALT_COLD_LOAD_PLI_CURVE_PHA,
          ALT_COLD_LOAD_PLI_GRD,
          ALT_COLD_LOAD_PLI_PHA,
          ALT_COLD_LOAD_PLI_USED,
          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT_HIGH_CURRENT_LOCKOUT_PHA,
          ALT_HIGH_CURRENT_LOCKOUT_USED,
          ALT_SGF_TIME_DELAY,
          ALT_SGF_MIN_TRIP_PERCENT,
          ALT_SGF_CD,
          ALT_RECLOSE_RETRY_ENABLED,
          ALT_RESET,
          ALT_RECLOSE3_TIME,
          ALT_RECLOSE2_TIME,
          ALT_RECLOSE1_TIME,
          ALT_TOT_LOCKOUT_OPS,
          ALT_PHA_TADD_SLOW,
          ALT_GRD_TADD_SLOW,
          ALT_PHA_TADD_FAST,
          ALT_GRD_TADD_FAST,
          ALT_PHA_VMUL_SLOW,
          ALT_GRD_VMUL_SLOW,
          ALT_PHA_VMUL_FAST,
          ALT_GRD_SLOW_CRV_OPS,
          ALT_PHA_SLOW_CRV_OPS,
          ALT_GRD_VMUL_FAST,
          ALT_PHA_SLOW_CRV,
          ALT_GRD_SLOW_CRV,
          ALT_PHA_FAST_CRV,
          ALT_GRD_FAST_CRV,
          ALT_PHA_RESP_TIME,
          ALT_GRD_RESP_TIME,
          ALT_TCC2_SLOW_CURVES_USED,
          ALT_TCC1_FAST_CURVES_USED,
          ALT_PHA_OP_F_CRV,
          ALT_GRD_OP_F_CRV,
          ALT_PHA_INST_TRIP_CD,
          ALT_GRD_INST_TRIP_CD,
          ALT_PHA_MIN_TRIP,
          ALT_GRD_MIN_TRIP,
          COLD_LOAD_PLI_CURVE_GRD,
          COLD_LOAD_PLI_CURVE_PHA,
          COLD_LOAD_PLI_GRD,
          COLD_LOAD_PLI_PHA,
          COLD_LOAD_PLI_USED,
          HIGH_CURRENT_LOCKUOUT_GRD,
          HIGH_CURRENT_LOCKOUT_PHA,
          HIGH_CURRENT_LOCKOUT_USED,
          SGF_TIME_DELAY,
          SGF_MIN_TRIP_PERCENT,
          SGF_CD,
          RECLOSE_RETRY_ENABLED,
          RESET,
          RECLOSE3_TIME,
          RECLOSE2_TIME,
          RECLOSE1_TIME,
          TOT_LOCKOUT_OPS,
          PHA_TADD_SLOW,
          GRD_TADD_SLOW,
          PHA_TADD_FAST,
          GRD_TADD_FAST,
          PHA_VMUL_SLOW,
          GRD_VMUL_SLOW,
          PHA_VMUL_FAST,
          GRD_VMUL_FAST,
          GRD_SLOW_CRV_OPS,
          PHA_SLOW_CRV_OPS,
          PHA_SLOW_CRV,
          GRD_SLOW_CRV,
          PHA_FAST_CRV,
          GRD_FAST_CRV,
          PHA_RESP_TIME,
          GRD_RESP_TIME,
          PHA_OP_F_CRV,
          GRD_OP_F_CRV,
          TCC2_SLOW_CURVES_USED,
          TCC1_FAST_CURVES_USED,
          PHA_INST_TRIP_CD,
          GRD_INST_TRIP_CD,
          PHA_MIN_TRIP,
          GRD_MIN_TRIP,
          OPERATING_AS_CD,
          BYPASS_PLANS,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          FLISR_ENGINEERING_COMMENTS,
          "Limiting_Factor",
          "Winter_Load_Limit",
          "Summer_Load_Limit",
          FLISR,
          ENGINEERING_COMMENTS,
          ACTIVE_PROFILE,
          ALT3_COLD_LOAD_PLI_CURVE_GRD,
          ALT3_COLD_LOAD_PLI_CURVE_PHA,
          ALT3_COLD_LOAD_PLI_GRD,
          ALT3_COLD_LOAD_PLI_PHA,
          ALT3_COLD_LOAD_PLI_USED,
          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
          ALT3_HIGH_CURRENT_LOCKOUT_USED,
          ALT3_SGF_TIME_DELAY,
          ALT3_SGF_MIN_TRIP_PERCENT,
          ALT3_SGF_CD,
          ALT3_RECLOSE_RETRY_ENABLED,
          ALT3_RESET,
          ALT3_RECLOSE3_TIME,
          ALT3_RECLOSE2_TIME,
          ALT3_RECLOSE1_TIME,
          ALT3_TOT_LOCKOUT_OPS,
          ALT3_GRD_DELAY,
          ALT3_PHA_DELAY,
          ALT3_PHA_TADD_SLOW,
          ALT3_GRD_TADD_SLOW,
          ALT3_PHA_TADD_FAST,
          ALT3_GRD_TADD_FAST,
          ALT3_PHA_VMUL_SLOW,
          ALT3_GRD_VMUL_SLOW,
          ALT3_PHA_VMUL_FAST,
          ALT3_GRD_SLOW_CRV_OPS,
          ALT3_PHA_SLOW_CRV_OPS,
          ALT3_GRD_VMUL_FAST,
          ALT3_PHA_SLOW_CRV,
          ALT3_GRD_SLOW_CRV,
          ALT3_PHA_FAST_CRV,
          ALT3_GRD_FAST_CRV,
          ALT3_TCC2_SLOW_CURVES_USED,
          ALT3_TCC1_FAST_CURVES_USED,
          ALT3_PHA_OP_F_CRV,
          ALT3_GRD_OP_F_CRV,
          ALT3_PHA_INST_TRIP_CD,
          ALT3_GRD_INST_TRIP_CD,
          ALT3_PHA_MIN_TRIP,
          ALT3_GRD_MIN_TRIP,
          ALT2_PHA_TADD_FAST,
          ALT2_GRD_TADD_FAST,
          ALT2_GRD_VMUL_SLOW,
          ALT2_PHA_VMUL_FAST,
          ALT2_PHA_FAST_CRV,
          ALT2_GRD_FAST_CRV
        FROM SM_RECLOSER
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_RECLOSER_HIST
        (
          EFFECTIVE_DT,
          TIMESTAMP,
          DATE_MODIFIED,
          RELAY_TYPE,
          PREPARED_BY,
          DEVICE_ID,
          OPERATING_NUM,
          FEATURE_CLASS_NAME,
          GLOBAL_ID,
          ALT2_PHA_INST_TRIP_CD,
          ALT2_GRD_INST_TRIP_CD,
          ALT2_PHA_MIN_TRIP,
          ALT2_GRD_MIN_TRIP,
          ALT2_LS_RESET_TIME,
          ALT2_LS_LOCKOUT_OPS,
          ALT2_INRUSH_DURATION,
          ALT2_PHA_INRUSH_THRESHOLD,
          ALT2_GRD_INRUSH_THRESHOLD,
          ALT2_PHA_ARMING_THRESHOLD,
          ALT2_GRD_ARMING_THRESHOLD,
          ALT2_PERMIT_LS_ENABLING,
          ALT_COLD_LOAD_PLI_CURVE_GRD,
          ALT_COLD_LOAD_PLI_CURVE_PHA,
          ALT_COLD_LOAD_PLI_GRD,
          ALT_COLD_LOAD_PLI_PHA,
          ALT_COLD_LOAD_PLI_USED,
          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT_HIGH_CURRENT_LOCKOUT_PHA,
          ALT_HIGH_CURRENT_LOCKOUT_USED,
          ALT_SGF_TIME_DELAY,
          ALT_SGF_MIN_TRIP_PERCENT,
          ALT_SGF_CD,
          ALT_RECLOSE_RETRY_ENABLED,
          ALT_RESET,
          ALT_RECLOSE3_TIME,
          ALT_RECLOSE2_TIME,
          ALT_RECLOSE1_TIME,
          ALT_TOT_LOCKOUT_OPS,
          ALT_PHA_TADD_SLOW,
          ALT_GRD_TADD_SLOW,
          ALT_PHA_TADD_FAST,
          ALT_GRD_TADD_FAST,
          ALT_PHA_VMUL_SLOW,
          ALT_GRD_VMUL_SLOW,
          ALT_PHA_VMUL_FAST,
          ALT_GRD_SLOW_CRV_OPS,
          ALT_PHA_SLOW_CRV_OPS,
          ALT_GRD_VMUL_FAST,
          ALT_PHA_SLOW_CRV,
          ALT_GRD_SLOW_CRV,
          ALT_PHA_FAST_CRV,
          ALT_GRD_FAST_CRV,
          ALT_PHA_RESP_TIME,
          ALT_GRD_RESP_TIME,
          ALT_TCC2_SLOW_CURVES_USED,
          ALT_TCC1_FAST_CURVES_USED,
          ALT_PHA_OP_F_CRV,
          ALT_GRD_OP_F_CRV,
          ALT_PHA_INST_TRIP_CD,
          ALT_GRD_INST_TRIP_CD,
          ALT_PHA_MIN_TRIP,
          ALT_GRD_MIN_TRIP,
          COLD_LOAD_PLI_CURVE_GRD,
          COLD_LOAD_PLI_CURVE_PHA,
          COLD_LOAD_PLI_GRD,
          COLD_LOAD_PLI_PHA,
          COLD_LOAD_PLI_USED,
          HIGH_CURRENT_LOCKUOUT_GRD,
          HIGH_CURRENT_LOCKOUT_PHA,
          HIGH_CURRENT_LOCKOUT_USED,
          SGF_TIME_DELAY,
          SGF_MIN_TRIP_PERCENT,
          SGF_CD,
          RECLOSE_RETRY_ENABLED,
          RESET,
          RECLOSE3_TIME,
          RECLOSE2_TIME,
          RECLOSE1_TIME,
          TOT_LOCKOUT_OPS,
          PHA_TADD_SLOW,
          GRD_TADD_SLOW,
          PHA_TADD_FAST,
          GRD_TADD_FAST,
          PHA_VMUL_SLOW,
          GRD_VMUL_SLOW,
          PHA_VMUL_FAST,
          GRD_VMUL_FAST,
          GRD_SLOW_CRV_OPS,
          PHA_SLOW_CRV_OPS,
          PHA_SLOW_CRV,
          GRD_SLOW_CRV,
          PHA_FAST_CRV,
          GRD_FAST_CRV,
          PHA_RESP_TIME,
          GRD_RESP_TIME,
          PHA_OP_F_CRV,
          GRD_OP_F_CRV,
          TCC2_SLOW_CURVES_USED,
          TCC1_FAST_CURVES_USED,
          PHA_INST_TRIP_CD,
          GRD_INST_TRIP_CD,
          PHA_MIN_TRIP,
          GRD_MIN_TRIP,
          OPERATING_AS_CD,
          BYPASS_PLANS,
          CONTROL_TYPE,
          OK_TO_BYPASS,
          RELEASED_BY,
          PROCESSED_FLAG,
          CONTROL_SERIAL_NUM,
          SOFTWARE_VERSION,
          FIRMWARE_VERSION,
          CURRENT_FUTURE,
          NOTES,
          DISTRICT,
          DIVISION,
          PEER_REVIEW_BY,
          PEER_REVIEW_DT,
          RADIO_SERIAL_NUM,
          RADIO_MODEL_NUM,
          RADIO_MANF_CD,
          SPECIAL_CONDITIONS,
          REPEATER,
          RTU_ADDRESS,
          TRANSMIT_DISABLE_DELAY,
          TRANSMIT_ENABLE_DELAY,
          BAUD_RATE,
          MASTER_STATION,
          SCADA_TYPE,
          SCADA,
          ENGINEERING_DOCUMENT,
          FLISR_ENGINEERING_COMMENTS,
          "Limiting_Factor",
          "Winter_Load_Limit",
          "Summer_Load_Limit",
          FLISR,
          ENGINEERING_COMMENTS,
          ACTIVE_PROFILE,
          ALT3_COLD_LOAD_PLI_CURVE_GRD,
          ALT3_COLD_LOAD_PLI_CURVE_PHA,
          ALT3_COLD_LOAD_PLI_GRD,
          ALT3_COLD_LOAD_PLI_PHA,
          ALT3_COLD_LOAD_PLI_USED,
          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
          ALT3_HIGH_CURRENT_LOCKOUT_USED,
          ALT3_SGF_TIME_DELAY,
          ALT3_SGF_MIN_TRIP_PERCENT,
          ALT3_SGF_CD,
          ALT3_RECLOSE_RETRY_ENABLED,
          ALT3_RESET,
          ALT3_RECLOSE3_TIME,
          ALT3_RECLOSE2_TIME,
          ALT3_RECLOSE1_TIME,
          ALT3_TOT_LOCKOUT_OPS,
          ALT3_GRD_DELAY,
          ALT3_PHA_DELAY,
          ALT3_PHA_TADD_SLOW,
          ALT3_GRD_TADD_SLOW,
          ALT3_PHA_TADD_FAST,
          ALT3_GRD_TADD_FAST,
          ALT3_PHA_VMUL_SLOW,
          ALT3_GRD_VMUL_SLOW,
          ALT3_PHA_VMUL_FAST,
          ALT3_GRD_SLOW_CRV_OPS,
          ALT3_PHA_SLOW_CRV_OPS,
          ALT3_GRD_VMUL_FAST,
          ALT3_PHA_SLOW_CRV,
          ALT3_GRD_SLOW_CRV,
          ALT3_PHA_FAST_CRV,
          ALT3_GRD_FAST_CRV,
          ALT3_TCC2_SLOW_CURVES_USED,
          ALT3_TCC1_FAST_CURVES_USED,
          ALT3_PHA_OP_F_CRV,
          ALT3_GRD_OP_F_CRV,
          ALT3_PHA_INST_TRIP_CD,
          ALT3_GRD_INST_TRIP_CD,
          ALT3_PHA_MIN_TRIP,
          ALT3_GRD_MIN_TRIP,
          ALT2_PHA_TADD_FAST,
          ALT2_GRD_TADD_FAST,
          ALT2_GRD_VMUL_SLOW,
          ALT2_PHA_VMUL_FAST,
          ALT2_PHA_FAST_CRV,
          ALT2_GRD_FAST_CRV
        )
      SELECT EFFECTIVE_DT,
        TIMESTAMP,
        DATE_MODIFIED,
        RELAY_TYPE,
        PREPARED_BY,
        DEVICE_ID,
        OPERATING_NUM,
        FEATURE_CLASS_NAME,
        GLOBAL_ID,
        ALT2_PHA_INST_TRIP_CD,
        ALT2_GRD_INST_TRIP_CD,
        ALT2_PHA_MIN_TRIP,
        ALT2_GRD_MIN_TRIP,
        ALT2_LS_RESET_TIME,
        ALT2_LS_LOCKOUT_OPS,
        ALT2_INRUSH_DURATION,
        ALT2_PHA_INRUSH_THRESHOLD,
        ALT2_GRD_INRUSH_THRESHOLD,
        ALT2_PHA_ARMING_THRESHOLD,
        ALT2_GRD_ARMING_THRESHOLD,
        ALT2_PERMIT_LS_ENABLING,
        ALT_COLD_LOAD_PLI_CURVE_GRD,
        ALT_COLD_LOAD_PLI_CURVE_PHA,
        ALT_COLD_LOAD_PLI_GRD,
        ALT_COLD_LOAD_PLI_PHA,
        ALT_COLD_LOAD_PLI_USED,
        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT_HIGH_CURRENT_LOCKOUT_PHA,
        ALT_HIGH_CURRENT_LOCKOUT_USED,
        ALT_SGF_TIME_DELAY,
        ALT_SGF_MIN_TRIP_PERCENT,
        ALT_SGF_CD,
        ALT_RECLOSE_RETRY_ENABLED,
        ALT_RESET,
        ALT_RECLOSE3_TIME,
        ALT_RECLOSE2_TIME,
        ALT_RECLOSE1_TIME,
        ALT_TOT_LOCKOUT_OPS,
        ALT_PHA_TADD_SLOW,
        ALT_GRD_TADD_SLOW,
        ALT_PHA_TADD_FAST,
        ALT_GRD_TADD_FAST,
        ALT_PHA_VMUL_SLOW,
        ALT_GRD_VMUL_SLOW,
        ALT_PHA_VMUL_FAST,
        ALT_GRD_SLOW_CRV_OPS,
        ALT_PHA_SLOW_CRV_OPS,
        ALT_GRD_VMUL_FAST,
        ALT_PHA_SLOW_CRV,
        ALT_GRD_SLOW_CRV,
        ALT_PHA_FAST_CRV,
        ALT_GRD_FAST_CRV,
        ALT_PHA_RESP_TIME,
        ALT_GRD_RESP_TIME,
        ALT_TCC2_SLOW_CURVES_USED,
        ALT_TCC1_FAST_CURVES_USED,
        ALT_PHA_OP_F_CRV,
        ALT_GRD_OP_F_CRV,
        ALT_PHA_INST_TRIP_CD,
        ALT_GRD_INST_TRIP_CD,
        ALT_PHA_MIN_TRIP,
        ALT_GRD_MIN_TRIP,
        COLD_LOAD_PLI_CURVE_GRD,
        COLD_LOAD_PLI_CURVE_PHA,
        COLD_LOAD_PLI_GRD,
        COLD_LOAD_PLI_PHA,
        COLD_LOAD_PLI_USED,
        HIGH_CURRENT_LOCKUOUT_GRD,
        HIGH_CURRENT_LOCKOUT_PHA,
        HIGH_CURRENT_LOCKOUT_USED,
        SGF_TIME_DELAY,
        SGF_MIN_TRIP_PERCENT,
        SGF_CD,
        RECLOSE_RETRY_ENABLED,
        RESET,
        RECLOSE3_TIME,
        RECLOSE2_TIME,
        RECLOSE1_TIME,
        TOT_LOCKOUT_OPS,
        PHA_TADD_SLOW,
        GRD_TADD_SLOW,
        PHA_TADD_FAST,
        GRD_TADD_FAST,
        PHA_VMUL_SLOW,
        GRD_VMUL_SLOW,
        PHA_VMUL_FAST,
        GRD_VMUL_FAST,
        GRD_SLOW_CRV_OPS,
        PHA_SLOW_CRV_OPS,
        PHA_SLOW_CRV,
        GRD_SLOW_CRV,
        PHA_FAST_CRV,
        GRD_FAST_CRV,
        PHA_RESP_TIME,
        GRD_RESP_TIME,
        PHA_OP_F_CRV,
        GRD_OP_F_CRV,
        TCC2_SLOW_CURVES_USED,
        TCC1_FAST_CURVES_USED,
        PHA_INST_TRIP_CD,
        GRD_INST_TRIP_CD,
        PHA_MIN_TRIP,
        GRD_MIN_TRIP,
        OPERATING_AS_CD,
        BYPASS_PLANS,
        CONTROL_TYPE,
        OK_TO_BYPASS,
        RELEASED_BY,
        PROCESSED_FLAG,
        CONTROL_SERIAL_NUM,
        SOFTWARE_VERSION,
        FIRMWARE_VERSION,
        CURRENT_FUTURE,
        NOTES,
        DISTRICT,
        DIVISION,
        PEER_REVIEW_BY,
        PEER_REVIEW_DT,
        RADIO_SERIAL_NUM,
        RADIO_MODEL_NUM,
        RADIO_MANF_CD,
        SPECIAL_CONDITIONS,
        REPEATER,
        RTU_ADDRESS,
        TRANSMIT_DISABLE_DELAY,
        TRANSMIT_ENABLE_DELAY,
        BAUD_RATE,
        MASTER_STATION,
        SCADA_TYPE,
        SCADA,
        ENGINEERING_DOCUMENT,
        FLISR_ENGINEERING_COMMENTS,
        "Limiting_Factor",
        "Winter_Load_Limit",
        "Summer_Load_Limit",
        FLISR,
        ENGINEERING_COMMENTS,
        ACTIVE_PROFILE,
        ALT3_COLD_LOAD_PLI_CURVE_GRD,
        ALT3_COLD_LOAD_PLI_CURVE_PHA,
        ALT3_COLD_LOAD_PLI_GRD,
        ALT3_COLD_LOAD_PLI_PHA,
        ALT3_COLD_LOAD_PLI_USED,
        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
        ALT3_HIGH_CURRENT_LOCKOUT_USED,
        ALT3_SGF_TIME_DELAY,
        ALT3_SGF_MIN_TRIP_PERCENT,
        ALT3_SGF_CD,
        ALT3_RECLOSE_RETRY_ENABLED,
        ALT3_RESET,
        ALT3_RECLOSE3_TIME,
        ALT3_RECLOSE2_TIME,
        ALT3_RECLOSE1_TIME,
        ALT3_TOT_LOCKOUT_OPS,
        ALT3_GRD_DELAY,
        ALT3_PHA_DELAY,
        ALT3_PHA_TADD_SLOW,
        ALT3_GRD_TADD_SLOW,
        ALT3_PHA_TADD_FAST,
        ALT3_GRD_TADD_FAST,
        ALT3_PHA_VMUL_SLOW,
        ALT3_GRD_VMUL_SLOW,
        ALT3_PHA_VMUL_FAST,
        ALT3_GRD_SLOW_CRV_OPS,
        ALT3_PHA_SLOW_CRV_OPS,
        ALT3_GRD_VMUL_FAST,
        ALT3_PHA_SLOW_CRV,
        ALT3_GRD_SLOW_CRV,
        ALT3_PHA_FAST_CRV,
        ALT3_GRD_FAST_CRV,
        ALT3_TCC2_SLOW_CURVES_USED,
        ALT3_TCC1_FAST_CURVES_USED,
        ALT3_PHA_OP_F_CRV,
        ALT3_GRD_OP_F_CRV,
        ALT3_PHA_INST_TRIP_CD,
        ALT3_GRD_INST_TRIP_CD,
        ALT3_PHA_MIN_TRIP,
        ALT3_GRD_MIN_TRIP,
        ALT2_PHA_TADD_FAST,
        ALT2_GRD_TADD_FAST,
        ALT2_GRD_VMUL_SLOW,
        ALT2_PHA_VMUL_FAST,
        ALT2_PHA_FAST_CRV,
        ALT2_GRD_FAST_CRV
      FROM SM_RECLOSER
      WHERE GLOBAL_ID=I_Global_id_Current;
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_RECLOSER_HIST',
          SM_RECLOSER_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_RECLOSER WHERE GLOBAL_ID = I_Global_id_Previous;
      COMMIT;
    END;
  END IF;
END SP_RECLOSER_DETECTION;

PROCEDURE SP_REGULATOR_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2)
AS
  REASON_TYPE  CHAR;
  DEVICE_TYPE  VARCHAR2(50);
  VAR1         VARCHAR2(50);
  NUM1         NUMBER;
 
BEGIN
  REASON_TYPE := I_reason_type ;
    
  IF REASON_TYPE = 'I' THEN
    BEGIN
      
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Current;
      
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_REGULATOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'C'
        );
               
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_REGULATOR
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          CONTROL_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Control_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_REGULATOR',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
           
    END;
  END IF;
  
  
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Current AND CURRENT_FUTURE='C';
     
      -- first copy the entire current record to history table

INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET, REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER
FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;




      
      
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          
          'SM_REGULATOR_HIST',
           SM_REGULATOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
     
      UPDATE SM_REGULATOR
      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,CONTROL_TYPE=I_Control_type_code
      WHERE GLOBAL_ID    = I_Global_id_Current;
      
      COMMIT;
      
    END;
 
  END IF;
  
  IF REASON_TYPE = 'D' THEN
    
   SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Current AND CURRENT_FUTURE='C';

      -- first copy the entire current record to history table


INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER
FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;







     DELETE FROM SM_REGULATOR WHERE GLOBAL_ID = I_Global_id_Current;

      -- Insert a record in comments table with notes set to INST

      -- Insert a record in comments table with notes set to INST


  INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_REGULATOR_HIST',
          SM_REGULATOR_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
    
    COMMIT;
    
  END IF;
  
  
  
 
 IF REASON_TYPE = 'R' THEN
    
  BEGIN
      -- Raise NO_REC_FOUND exception if the previous global id does not exist in device table

SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Previous AND CURRENT_FUTURE='C';


-- Raise REC_FOUND exception if the current global id already exist in device table 

SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Current AND CURRENT_FUTURE='C';

      
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      
      -- First insert the record in the device table with current_future set to 'C'
      
	INSERT INTO SM_REGULATOR ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CONTROL_TYPE, CURRENT_FUTURE ) 
VALUES ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, I_Control_type_code, 'C' );
               
      -- Insert the record in the device table with current_future set to 'F'

     
INSERT INTO SM_REGULATOR ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CONTROL_TYPE, CURRENT_FUTURE ) 
VALUES ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, I_Control_type_code, 'F' );


      -- Insert a record in comments table with notes set to OTHR

      INSERT INTO SM_COMMENT_HIST ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS ) 
VALUES ( 'SM_REGULATOR', I_Global_id_Current, sysdate, 'OTHR', 'SYSTEM', sysdate, 'New record from GIS system' );


UPDATE  SM_REGULATOR SET (BAND_WIDTH, BAUD_RATE, BLOCKED_PCT,  CONTROL_TYPE, 
ENGINEERING_DOCUMENT, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET,  HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE",  OK_TO_BYPASS, PEAK_LOAD,  POWER_FACTOR,  PRIMARY_CT_RATING, PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE,  REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)=
(SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT,  CONTROL_TYPE, 
ENGINEERING_DOCUMENT, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET,  HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE",  OK_TO_BYPASS, PEAK_LOAD,  POWER_FACTOR,  PRIMARY_CT_RATING, PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE,  REPEATER, REV_A_RESET, REV_A_VOLT,
REV_A_XSET, REV_B_RESET, REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER 
FROM  SM_REGULATOR WHERE GLOBAL_ID     = I_Global_id_Previous AND CURRENT_FUTURE='C')
WHERE GLOBAL_ID     = I_Global_id_Current;


INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, ID, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION, 
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG, 
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER
FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;



INSERT INTO SM_COMMENT_HIST ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS ) VALUES (
'SM_REGULATOR_HIST', SM_REGULATOR_HIST_SEQ.CURRVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );


DELETE FROM SM_REGULATOR WHERE GLOBAL_ID = I_Global_id_Previous;

COMMIT;
  
END;
END IF;
 
  
  
END SP_REGULATOR_DETECTION;

PROCEDURE SP_SWITCH_DETECTION(
   I_Global_id_Current  IN VARCHAR2,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN VARCHAR2,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2)
AS
  REASON_TYPE  CHAR;
  DEVICE_TYPE  VARCHAR2(50);
  VAR1         VARCHAR2(50);
  NUM1         NUMBER;
  REC_FOUND EXCEPTION ;
 
BEGIN
  REASON_TYPE := I_reason_type ;
    
  IF REASON_TYPE = 'I' THEN
    BEGIN
      
      -- check to see if globalid exist in table. If it does then throw exception
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID = I_Global_id_Current;
      
      IF NUM1        >= 1 THEN
        RAISE REC_FOUND;
      END IF;
      
      -- First insert the record in the device table with current_future set to 'C'
      INSERT
      INTO SM_SWITCH
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          SWITCH_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Switch_type_code,
          'C'
        );
               
      -- Insert the record in the device table with current_future set to 'F'
      INSERT
      INTO SM_SWITCH
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          SWITCH_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Switch_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SWITCH',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );
      COMMIT;
           
    END;
  END IF;
  
  
  IF REASON_TYPE = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SWITCH
      WHERE GLOBAL_ID = I_Global_id_Current AND CURRENT_FUTURE='C';
     
      -- first copy the entire current record to history table

 INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION)
SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION
FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Current;




      
      
      -- Insert a record in comments table with notes set to INST
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          
          'SM_SWITCH_HIST',
           SM_SWITCH_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
     
      UPDATE SM_SWITCH
      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,SWITCH_TYPE=I_Switch_type_code
      WHERE GLOBAL_ID    = I_Global_id_Current;
      
      COMMIT;
      
    END;
 
  END IF;
  
  IF REASON_TYPE = 'D' THEN
    
   SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SWITCH
      WHERE GLOBAL_ID = I_Global_id_Current AND CURRENT_FUTURE='C';

      -- first copy the entire current record to history table


 INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION)
SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION
FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Current;





     DELETE FROM SM_SWITCH WHERE GLOBAL_ID = I_Global_id_Current;

      -- Insert a record in comments table with notes set to INST

      -- Insert a record in comments table with notes set to INST


  INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SWITCH_HIST',
          SM_SWITCH_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
    
    COMMIT;
    
  END IF;
  
  
  
    
    IF REASON_TYPE = 'R' THEN
    BEGIN
      -- Raise NO_DATA_FOUND exception if the previous global id does not exist in device table

      SELECT GLOBAL_ID
      INTO VAR1
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';

      -- Raise REC_FOUND exception if the current global id already exist in device table


      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      IF NUM1          >= 1 THEN
        RAISE REC_FOUND;
      END IF;
     

 -- First insert the record in the device table with current_future set to 'C'
     

INSERT
      INTO SM_SWITCH
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          SWITCH_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Switch_type_code,
          'C'
        );
               
      -- Insert the record in the device table with current_future set to 'F'

      INSERT
      INTO SM_SWITCH
        (
          GLOBAL_ID,
          FEATURE_CLASS_NAME,
          OPERATING_NUM,
          DIVISION,
          DISTRICT,
          SWITCH_TYPE,
          CURRENT_FUTURE
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          I_Switch_type_code,
          'F'
        );
      -- Insert a record in comments table with notes set to INST
      
      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_TABLE_NAME,
          GLOBAL_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SWITCH',
          I_Global_id_Current,
          sysdate,
          'INST',
          'SYSTEM',
          sysdate,
          'New record from GIS system'
        );


UPDATE SM_SWITCH SET (RADIO_SERIAL_NUM, RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION,
SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE, ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, 
TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE, UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT,
LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION, SELECT_RETURN, 
UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, 
SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP, FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, 
GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE, ATS_CAPABLE, SECTIONALIZING_FEATURE, 
OK_TO_BYPASS, CONTROL_UNIT_TYPE, SOFTWARE_VERSION, FIRMWARE_VERSION, SWITCH_TYPE)
 =(SELECT RADIO_SERIAL_NUM, RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION,
SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE, ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, 
TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE, UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT,
LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION, SELECT_RETURN, 
UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, 
SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP, FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, 
GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE, ATS_CAPABLE, SECTIONALIZING_FEATURE, 
OK_TO_BYPASS, CONTROL_UNIT_TYPE, SOFTWARE_VERSION, FIRMWARE_VERSION, SWITCH_TYPE  
FROM SM_SWITCH  WHERE GLOBAL_ID   = I_Global_id_Previous   AND CURRENT_FUTURE='C' ) WHERE GLOBAL_ID = I_Global_id_Current;
     



-- first copy the entire previous record to history table

 INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION)
SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA, ENGINEERING_DOCUMENT, OPERATING_MODE,
ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, TRANSITION_DWELL, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
UNBALANCE_DETECT_VOLT, OVERVOLT_DETECT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE, NOTES, DISTRICT, DIVISION
FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Previous;



      INSERT
      INTO SM_COMMENT_HIST
        (
          DEVICE_HIST_TABLE_NAME,
          HIST_ID,
          WORK_DATE,
          WORK_TYPE,
          PERFORMED_BY,
          ENTRY_DATE,
          COMMENTS
        )
        VALUES
        (
          'SM_SWITCH_HIST',
          SM_SWITCH_HIST_SEQ.CURRVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );


      -- Remove previous global id from te device table

      DELETE
      FROM SM_SWITCH
      WHERE GLOBAL_ID = I_Global_id_Previous;


      COMMIT;

      
    END;
  END IF;
    
END SP_SWITCH_DETECTION;








END SM_CHANGE_DETECTION_PKG;

/
--------------------------------------------------------
--  DDL for Package Body SM_EXPOSE_DATA_PKG
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "EDSETT"."SM_EXPOSE_DATA_PKG" 
AS



PROCEDURE SP_SM_SET_DATE_PARAMS(
    I_From     IN DATE,
    I_To           IN DATE)
AS
BEGIN

  DATEFROM := I_From;
  DATETO :=I_To;
  
END SP_SM_SET_DATE_PARAMS;


FUNCTION GET_DATEFROM
  RETURN DATE
IS
BEGIN
  RETURN DATEFROM;
END;

FUNCTION GET_DATETO
  RETURN DATE
IS
BEGIN
  RETURN DATETO;
END;

END SM_EXPOSE_DATA_PKG;

/

  GRANT EXECUTE ON "EDSETT"."SM_EXPOSE_DATA_PKG" TO "GIS_I";
--------------------------------------------------------
--  DDL for Procedure DISPLAY_COUNT_DEVICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."DISPLAY_COUNT_DEVICES" 
IS

GIS_TOTAL  NUMBER;
GIS_NOTIN  NUMBER;

CEDSA_TOTAL  NUMBER;
CEDSA_NONIN  NUMBER;

TARGET	NUMBER;
DIVNULL NUMBER ;
DISTNULL NUMBER ;
DIVDISTNULL NUMBER;


BEGIN 
      	

SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Sectionalizer';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_SECTIONALIZER SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Sectionalizer') AND GD.FEATURE_CLASS_NAME ='Sectionalizer';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_SECTIONALIZER;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_SECTIONALIZER  CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_SECTIONALIZER CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Sectionalizer') ;

SELECT COUNT(*) INTO TARGET   FROM SM_SECTIONALIZER ;


DBMS_OUTPUT.PUT_LINE('Sectionalizer Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_SECTIONALIZER ( Future + Current) : '|| TARGET);

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');


SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Recloser';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_RECLOSER  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser') AND GD.FEATURE_CLASS_NAME ='Recloser';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_RECLOSER ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_RECLOSER   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_RECLOSER  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser') ;

SELECT COUNT(*) INTO TARGET   FROM SM_RECLOSER  ;


DBMS_OUTPUT.PUT_LINE('Recloser Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_RECLOSER  ( Future + Current) : '|| TARGET);

SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Interrupter';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_INTERRUPTER  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter') AND GD.FEATURE_CLASS_NAME ='Interrupter';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_INTERRUPTER ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_INTERRUPTER   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_INTERRUPTER  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter') ;

SELECT COUNT(*) INTO TARGET   FROM SM_INTERRUPTER  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Interrupter Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_INTERRUPTER  ( Future + Current) : '|| TARGET);

SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Regulator';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_REGULATOR  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator') AND GD.FEATURE_CLASS_NAME ='Regulator';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_REGULATOR ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_REGULATOR   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_REGULATOR  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator') ;

SELECT COUNT(*) INTO TARGET   FROM SM_REGULATOR  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Regulator Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_REGULATOR  ( Future + Current) : '|| TARGET);

SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Switch';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_SWITCH  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch') AND GD.FEATURE_CLASS_NAME ='Switch';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_SWITCH ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_SWITCH   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_SWITCH  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch') ;

SELECT COUNT(*) INTO TARGET   FROM SM_SWITCH  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Switch Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_SWITCH  ( Future + Current) : '|| TARGET);


SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Capacitor';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_CAPACITOR  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Capacitor') AND GD.FEATURE_CLASS_NAME ='Capacitor';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_CAPACITOR ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_CAPACITOR   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_CAPACITOR  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Capacitor') ;

SELECT COUNT(*) INTO TARGET   FROM SM_CAPACITOR  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Capacitor Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_CAPACITOR  ( Future + Current) : '|| TARGET);


SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Circuit_Breaker';
SELECT COUNT(*) INTO GIS_NOTIN FROM GIS_CEDSADEVICEID  GD where DEVICE_ID NOT IN (select SE.DEVICE_ID from SM_CIRCUIT_BREAKER  SE,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Circuit_Breaker') AND GD.FEATURE_CLASS_NAME ='Circuit_Breaker';

SELECT COUNT(*) INTO CEDSA_TOTAL  FROM CEDSA_CIRCUIT_BREAKER ;
SELECT COUNT(*) INTO CEDSA_NONIN  from CEDSA_CIRCUIT_BREAKER   CE where DEVICE_ID NOT IN (select CE.DEVICE_ID from CEDSA_CIRCUIT_BREAKER  CE,GIS_CEDSADEVICEID  GD
WHERE CE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Circuit_Breaker') ;

SELECT COUNT(*) INTO TARGET   FROM SM_CIRCUIT_BREAKER  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Circuit_Breaker Device Information : ' );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE('GIS Not In : '|| GIS_NOTIN);

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('CEDSA Not In : '|| CEDSA_NONIN);
DBMS_OUTPUT.PUT_LINE('Total Count from SM_CIRCUIT_BREAKER  ( Future + Current) : '|| TARGET);





SELECT count(*) INTO DIVNULL  from GIS_CEDSADEVICEID G  where G.DIVISION IS NULL ;

SELECT count(*) INTO DISTNULL from GIS_CEDSADEVICEID G  where G.DISTRICT IS NULL ;

SELECT count(*) INTO DIVDISTNULL from GIS_CEDSADEVICEID G  where G.DIVISION IS NULL OR G.DISTRICT IS NULL;


DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('GIS_CEDESADEVICE - DEVICE & DISTRICT NULL Value Details : ' );
DBMS_OUTPUT.PUT_LINE(' Number of rows having Division NULL : '|| DISTNULL );
DBMS_OUTPUT.PUT_LINE('Number of rows having District NULL : '|| DIVDISTNULL );

DBMS_OUTPUT.PUT_LINE(' Total Number of rows - Division + District having NULL values : '|| DIVDISTNULL  );


END DISPLAY_COUNT_DEVICES ;

/
--------------------------------------------------------
--  DDL for Procedure DISPLAY_COUNT_DEVICES_NEW
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."DISPLAY_COUNT_DEVICES_NEW" 
IS

GIS_TOTAL  NUMBER;
MISS_IN_GIS  NUMBER;

SUBGIS_TOTAL NUMBER;
SUBINGIS_NOTINCEDSA NUMBER;


CEDSA_TOTAL  NUMBER;
MISS_IN_CEDSA  NUMBER;
MISS_SCH_CEDSA NUMBER;
INCEDSA_NOTINGIS NUMBER;
SUBCEDSA_TOTAL NUMBER;
INGIS_NOTINCEDSA NUMBER;
BINGIS_NOTINCEDSA NUMBER;
TARGET	NUMBER;
DIVNULL  NUMBER;
DISTNULL NUMBER ;
DIVDISTNULL NUMBER;


BEGIN 
      	


-- Capacitor Bank
select count(*) INTO CEDSA_TOTAL from CEDSA_CAPACITOR CC;

-- Missing schedule in CEDSA

select COUNT(*)  INTO MISS_SCH_CEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC ,
    CEDSA_CAPACITOR_SETTINGS CS ,
    CEDSA_CAPACITOR_SCHEDULES CSH,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =CSH.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank'
  AND CSH.SCHEDULE          =1)
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank'
  AND GLOBAL_ID NOT IN
  (select GLOBAL_ID from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC,
    CEDSA_CAPACITOR_SETTINGS CS,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CS.CURRENT_FUTURE = 'C'
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank')
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank');
  
-- Missing in CEDSA


INSERT INTO INGIS_MISSINCEDSA (select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC,
    CEDSA_CAPACITOR_SETTINGS CS,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CS.CURRENT_FUTURE = 'C'
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank')
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank');

select COUNT(*) INTO INGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC,
    CEDSA_CAPACITOR_SETTINGS CS,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CS.CURRENT_FUTURE = 'C'
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank')
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.CapacitorBank';

-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL  FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Capacitor' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.CapacitorBank';

--In CEDSA not in GIS

select COUNT(*) INTO INCEDSA_NOTINGIS  from CEDSA_CAPACITOR CC
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = CC.device_id);



-- Sub Capacitor Bank



INSERT INTO INGIS_MISSINCEDSA (select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC ,
    CEDSA_CAPACITOR_SETTINGS CS ,
    CEDSA_CAPACITOR_SCHEDULES CSH,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =CSH.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.SubCapacitorBank'
  AND CSH.SCHEDULE          =1)
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubCapacitorBank');
  
select COUNT(*) INTO SUBINGIS_NOTINCEDSA  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
  FROM CEDSA_CAPACITOR CC ,
    CEDSA_CAPACITOR_SETTINGS CS ,
    CEDSA_CAPACITOR_SCHEDULES CSH,
    GIS_CEDSADEVICEID GD
  WHERE CS.DEVICE_ID        =CC.DEVICE_ID
  AND CS.DEVICE_ID          =CSH.DEVICE_ID
  AND CS.DEVICE_ID          =GD.DEVICE_ID
  AND CSH.CURRENT_FUTURE    =CS.CURRENT_FUTURE
  AND GD.FEATURE_CLASS_NAME ='Capacitor'
  AND GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.SubCapacitorBank'
  AND CSH.SCHEDULE          =1)
  AND FEATURE_CLASS_NAME = 'Capacitor' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubCapacitorBank';
  
-- GIS Count
SELECT COUNT(*) INTO SUBGIS_TOTAL   FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Capacitor' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.SubCapacitorBank';



SELECT COUNT(*) INTO TARGET   FROM SM_CAPACITOR  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Capacitor Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('Missing Schedule in CEDSA : ' || MISS_SCH_CEDSA );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sub Capacitor Device Information : ' );
DBMS_OUTPUT.PUT_LINE('Sub Capacitor Bank in GIS, Missing in CEDSA  : '||SUBINGIS_NOTINCEDSA );
DBMS_OUTPUT.PUT_LINE('Sub Capacitor  GIS Total : '|| SUBGIS_TOTAL );
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_CAPACITOR  ( Future + Current) : '|| TARGET);




-- Recloser
select count(*) INTO CEDSA_TOTAL from CEDSA_RECLOSER;


INSERT INTO INGIS_MISSINCEDSA (select * from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_RECLOSER  RE ,CEDSA_RECLOSER_SETTINGS  RS,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RS.DEVICE_ID AND RE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Recloser' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice');

select count(*) INTO INGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_RECLOSER  RE ,CEDSA_RECLOSER_SETTINGS  RS,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RS.DEVICE_ID AND RE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Recloser' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice';
  
-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL   FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Recloser' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.DynamicProtectiveDevice';

-- In CEDSA not in GIS

select count(*) INTO INCEDSA_NOTINGIS from CEDSA_RECLOSER REC
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = REC.device_id);



-- Sub Recloser

INSERT INTO INGIS_MISSINCEDSA(
select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_RECLOSER  RE ,CEDSA_RECLOSER_SETTINGS  RS,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RS.DEVICE_ID AND RE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'Recloser' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice');

select count(*) INTO SUBINGIS_NOTINCEDSA  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_RECLOSER  RE ,CEDSA_RECLOSER_SETTINGS  RS,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RS.DEVICE_ID AND RE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Recloser'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'Recloser' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice';

-- GIS Count  
SELECT COUNT(*)  INTO SUBGIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Recloser' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.SubInterruptingDevice';

SELECT COUNT(*) INTO TARGET   FROM SM_RECLOSER  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Recloser Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sub Recloser Device Information : ' );
DBMS_OUTPUT.PUT_LINE('Sub Recloser in GIS, Missing in CEDSA  : '||SUBINGIS_NOTINCEDSA  );
DBMS_OUTPUT.PUT_LINE('Sub Recloser  GIS Total : '|| SUBGIS_TOTAL );
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_RECLOSER  ( Future + Current) : '|| TARGET);




-- Interrupter
select count(*) INTO CEDSA_TOTAL from CEDSA_INTERRUPTER;

INSERT INTO INGIS_MISSINCEDSA(
select * from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM  CEDSA_INTERRUPTER CI,CEDSA_INTERRUPTER_SETTINGS CS,GIS_CEDSADEVICEID GD
	WHERE CI.DEVICE_ID=CS.DEVICE_ID AND CI.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Interrupter' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice');
  
 
select count(*) INTO  INGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM  CEDSA_INTERRUPTER CI,CEDSA_INTERRUPTER_SETTINGS CS,GIS_CEDSADEVICEID GD
	WHERE CI.DEVICE_ID=CS.DEVICE_ID AND CI.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Interrupter' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice';
  
  SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Interrupter' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.DynamicProtectiveDevice';

-- In CEDSA not in GIS
select count(*) INTO INCEDSA_NOTINGIS from CEDSA_INTERRUPTER I
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = I.device_id);



-- Sub Interrupter

INSERT INTO INGIS_MISSINCEDSA(
select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM  CEDSA_INTERRUPTER CI,CEDSA_INTERRUPTER_SETTINGS CS,GIS_CEDSADEVICEID GD
	WHERE CI.DEVICE_ID=CS.DEVICE_ID AND CI.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'Interrupter' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice');
  
select count(*) INTO SUBINGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM  CEDSA_INTERRUPTER CI,CEDSA_INTERRUPTER_SETTINGS CS,GIS_CEDSADEVICEID GD
	WHERE CI.DEVICE_ID=CS.DEVICE_ID AND CI.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Interrupter'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'Interrupter' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice';
 
-- GIS Count 
SELECT COUNT(*) INTO SUBGIS_TOTAL  FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Interrupter' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.SubInterruptingDevice';


SELECT COUNT(*) INTO TARGET   FROM SM_INTERRUPTER  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Interrupter Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('SubInterrupter Device Information : ' );
DBMS_OUTPUT.PUT_LINE('Sub Interrupter in GIS, Missing in CEDSA : '|| SUBINGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('Sub Interrupter GIS Total : '||SUBCEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_INTERRUPTER  ( Future + Current) : '|| TARGET);




-- Sectionalizer
select count(*) INTO CEDSA_TOTAL from CEDSA_SECTIONALIZER;


INSERT INTO INGIS_MISSINCEDSA(
select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_SECTIONALIZER SE, CEDSA_SECTIONALIZER_SETTINGS SS,GIS_CEDSADEVICEID  GD
	WHERE SE.DEVICE_ID=SS.DEVICE_ID AND SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Sectionalizer'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Sectionalizer' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice');

select count(*) INTO INGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
   FROM CEDSA_SECTIONALIZER SE, CEDSA_SECTIONALIZER_SETTINGS SS,GIS_CEDSADEVICEID  GD
	WHERE SE.DEVICE_ID=SS.DEVICE_ID AND SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Sectionalizer'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice'
	)
  AND FEATURE_CLASS_NAME = 'Sectionalizer' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.DynamicProtectiveDevice';
  
-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Sectionalizer' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.DynamicProtectiveDevice';

-- In CEDSA not in GIS
select count(*) INTO INCEDSA_NOTINGIS from CEDSA_SECTIONALIZER SEC
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = SEC.device_id);

SELECT COUNT(*) INTO TARGET   FROM SM_SECTIONALIZER ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sectionalizer Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Total Count from SM_SECTIONALIZER ( Future + Current) : '|| TARGET);



-- Switch
select count(*) INTO CEDSA_TOTAL  from CEDSA_SWITCH;

INSERT INTO INGIS_MISSINCEDSA(
select GD.*  from  GIS_CEDSADEVICEID GD, EDSETTGIS.SWITCH SW
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
	WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.Switch'
	)
  AND FEATURE_CLASS_NAME = 'Switch' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.Switch'
  AND GD.GLOBAL_ID = SW.GLOBALID
  AND (SW.SWITCHTYPE = 38 -- Automatic Switch
    OR SW.SWITCHTYPE = 18 -- SCADA Switch
        OR SW.SUBTYPECD = 6)); -- SCADA Mate


select count(*) INTO INGIS_NOTINCEDSA from  GIS_CEDSADEVICEID GD, EDSETTGIS.SWITCH SW
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
	WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.Switch'
	)
  AND FEATURE_CLASS_NAME = 'Switch' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.Switch'
  AND GD.GLOBAL_ID = SW.GLOBALID
  AND (SW.SWITCHTYPE = 38 -- Automatic Switch
    OR SW.SWITCHTYPE = 18 -- SCADA Switch
        OR SW.SUBTYPECD = 6); -- SCADA Mate
  
-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL  FROM GIS_CEDSADEVICEID  GD, EDSETTGIS.SWITCH SW
WHERE GD.FEATURE_CLASS_NAME ='Switch' 
  AND GD.GIS_FEATURE_CLASS_NAME='EDGIS.Switch'
  AND GD.GLOBAL_ID = SW.GLOBALID
  AND (SW.SWITCHTYPE = 38 -- Automatic Switch
    OR SW.SWITCHTYPE = 18 -- SCADA Switch
        OR SW.SUBTYPECD = 6); -- SCADA Mate

-- In CEDSA not in GIS
select count(*) INTO INCEDSA_NOTINGIS  from CEDSA_SWITCH SW
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = SW.device_id);



-- Sub Switch

INSERT INTO INGIS_MISSINCEDSA(
select *  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
	WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubSwitch'
	)
  AND FEATURE_CLASS_NAME = 'Switch' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubSwitch');
  
select count(*) INTO SUBINGIS_NOTINCEDSA  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
	WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubSwitch'
	)
  AND FEATURE_CLASS_NAME = 'Switch' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubSwitch';
  
-- GIS Count
SELECT COUNT(*) INTO SUBGIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Switch' and 
GD.GIS_FEATURE_CLASS_NAME='EDGIS.SubSwitch';

SELECT COUNT(*) INTO TARGET   FROM SM_SWITCH  ;

DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Switch Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sub Switch Device Information : ' );
DBMS_OUTPUT.PUT_LINE('Sub Switch in GIS, Missing in CEDSA : '|| SUBINGIS_NOTINCEDSA );
DBMS_OUTPUT.PUT_LINE('Sub Switch  GIS Total : '|| SUBGIS_TOTAL );
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_SWITCH  ( Future + Current) : '|| TARGET);






-- Circuit Breaker
select count(*) INTO CEDSA_TOTAL from CEDSA_CIRCUIT_BREAKER;

INSERT INTO INGIS_MISSINCEDSA (select GD.*  from  GIS_CEDSADEVICEID GD, EDSETTGIS.SUBINTERRUPTINGDEVICE SI
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM  CEDSA_CIRCUIT C ,CEDSA_CIRCUIT_RELAY CR,GIS_CEDSADEVICEID  GD   
	WHERE CR.CIRCUIT_BREAKER=C.DEVICE_ID and CR.CIRCUIT_BREAKER=GD.DEVICE_ID 
	and GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND CR.RELAY_CD='PHA'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'CircuitBreaker' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
  AND GD.GLOBAL_ID = SI.GLOBALID
  AND SI.SUBTYPECD = 1
  AND SI.CIRCUITID is not null
  AND GD.OPERATING_NUM not like '%CAP%');

select count(*) INTO INGIS_NOTINCEDSA  from  GIS_CEDSADEVICEID GD, EDSETTGIS.SUBINTERRUPTINGDEVICE SI
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM  CEDSA_CIRCUIT C ,CEDSA_CIRCUIT_RELAY CR,GIS_CEDSADEVICEID  GD   
	WHERE CR.CIRCUIT_BREAKER=C.DEVICE_ID and CR.CIRCUIT_BREAKER=GD.DEVICE_ID 
	and GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND CR.RELAY_CD='PHA'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
	)
  AND FEATURE_CLASS_NAME = 'CircuitBreaker' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
  AND GD.GLOBAL_ID = SI.GLOBALID
  AND SI.SUBTYPECD = 1
  AND SI.CIRCUITID is not null
  AND GD.OPERATING_NUM not like '%CAP%';

-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL   FROM GIS_CEDSADEVICEID GD, EDSETTGIS.SUBINTERRUPTINGDEVICE SI
WHERE GD.FEATURE_CLASS_NAME ='CircuitBreaker' 
  AND GD.GIS_FEATURE_CLASS_NAME='EDGIS.SubInterruptingDevice'
  AND GD.GLOBAL_ID = SI.GLOBALID
  AND SI.SUBTYPECD = 1
  AND SI.CIRCUITID is not null
  AND GD.OPERATING_NUM not like '%CAP%';

SELECT COUNT(*) INTO TARGET   FROM SM_CIRCUIT_BREAKER  ;



DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Circuit_Breaker - SubInterruptingDevice Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_CIRCUIT_BREAKER  ( Future + Current) : '|| TARGET);


DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');


-- Sub Transformer
select count(*) INTO CEDSA_TOTAL from CEDSA_REGULATOR;

select count(*) INTO INGIS_NOTINCEDSA  from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_REGULATOR CR ,GIS_CEDSADEVICEID  GD   
	WHERE CR.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubTransformerBank'
	)
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubTransformerBank';
  
-- GIS Count
SELECT COUNT(*) INTO GIS_TOTAL  FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Regulator' and 
GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubTransformerBank';


SELECT COUNT(*) INTO TARGET   FROM SM_REGULATOR  ;



DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sub Transformer Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_REGULATOR  ( Future + Current) : '|| TARGET);





-- Voltage Regulator
select count(*) INTO CEDSA_TOTAL from CEDSA_REGULATOR;

INSERT INTO INGIS_MISSINCEDSA (select * from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
                FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD   
                WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
  and GD.BANKCODE = RB.BANK_CD
                AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit'
                )
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit');


--- THis is for booster regulator
select  count(*) INTO BINGIS_NOTINCEDSA
  from GIS_CEDSADEVICEID GC, EDSETTGIS.VOLTAGEREGULATOR VR, EDSETTGIS.VOLTAGEREGULATORUNIT VRU
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
                FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD   
                WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
  and GD.BANKCODE = RB.BANK_CD
                AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit'
                )
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit'
    AND GC.GLOBAL_ID = VRU.GLOBALID
  and vr.globalid = vru.regulatorguid
  AND VR.SUBTYPECD = 3;

  
  
  select  count(*) INTO INGIS_NOTINCEDSA 
  from GIS_CEDSADEVICEID GC, EDSETTGIS.VOLTAGEREGULATOR VR, EDSETTGIS.VOLTAGEREGULATORUNIT VRU
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
                FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD   
                WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
  and GD.BANKCODE = RB.BANK_CD
                AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit'
                )
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.VoltageRegulatorUnit'
    AND GC.GLOBAL_ID = VRU.GLOBALID
  and vr.globalid = vru.regulatorguid
  AND VR.SUBTYPECD <> 3;




  
-- GIS Count
SELECT COUNT(*)  INTO GIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Regulator' and 
GD.GIS_FEATURE_CLASS_NAME ='EDGIS.VoltageRegulatorUnit';

-- In CEDSA not in GIS
select count(*) INTO INCEDSA_NOTINGIS from CEDSA_REGULATOR REG
where not exists 
(select GD.device_id from GIS_CEDSADEVICEID GD
where GD.device_id = REG.device_id);



-- Sub Voltage Regulator

INSERT INTO INGIS_MISSINCEDSA (select * from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.REGULATOR_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubVoltageRegulatorUnit'
	)
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubVoltageRegulatorUnit' );
  
  
select count(*) INTO SUBINGIS_NOTINCEDSA from  GIS_CEDSADEVICEID
WHERE GLOBAL_ID NOT IN
(SELECT GD.GLOBAL_ID
	FROM CEDSA_REGULATOR_SETTINGS RE, CEDSA_REGULATOR_BANK RB ,GIS_CEDSADEVICEID  GD   
	WHERE RE.DEVICE_ID=RB.DEVICE_ID AND RB.REGULATOR_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Regulator'
	AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubVoltageRegulatorUnit'
	)
  AND FEATURE_CLASS_NAME = 'Regulator' 
  AND GIS_FEATURE_CLASS_NAME = 'EDGIS.SubVoltageRegulatorUnit';
  
-- GIS Count
SELECT COUNT(*)  INTO SUBGIS_TOTAL FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Regulator' and 
GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubVoltageRegulatorUnit';


SELECT COUNT(*) INTO TARGET   FROM SM_REGULATOR  ;


DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Regulator Device Information : ' );

DBMS_OUTPUT.PUT_LINE('CEDSA Total : '|| CEDSA_TOTAL );
DBMS_OUTPUT.PUT_LINE('Booster Regulator - In GIS, Missing in CEDSA : '|| BINGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In GIS, Missing in CEDSA : '|| INGIS_NOTINCEDSA);
DBMS_OUTPUT.PUT_LINE('In CEDSA, Missing in GIS : '||INCEDSA_NOTINGIS );
DBMS_OUTPUT.PUT_LINE('GIS Total : '|| GIS_TOTAL);
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('Sub Voltage Regulator Device Information : ' );
DBMS_OUTPUT.PUT_LINE('Sub Voltage Regulator Bank in GIS, Missing in CEDSA  : '||SUBINGIS_NOTINCEDSA );
DBMS_OUTPUT.PUT_LINE('Sub Voltage Regulator  GIS Total : '|| SUBGIS_TOTAL );
DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE('Total Count from SM_REGULATOR  ( Future + Current) : '|| TARGET);



SELECT count(*) INTO DIVNULL  from GIS_CEDSADEVICEID G  where G.DIVISION IS NULL ;

SELECT count(*) INTO DISTNULL from GIS_CEDSADEVICEID G  where G.DISTRICT IS NULL ;

SELECT count(*) INTO DIVDISTNULL from GIS_CEDSADEVICEID G  where G.DIVISION IS NULL OR G.DISTRICT IS NULL;


DBMS_OUTPUT.PUT_LINE(' ');
DBMS_OUTPUT.PUT_LINE(' ');

DBMS_OUTPUT.PUT_LINE('GIS_CEDESADEVICE - DEVICE and DISTRICT NULL Value Details : ' );
DBMS_OUTPUT.PUT_LINE(' Number of rows having Division NULL : '|| DIVNULL );
DBMS_OUTPUT.PUT_LINE('Number of rows having District NULL : '|| DISTNULL );

DBMS_OUTPUT.PUT_LINE(' Total Number of rows - Division + District having NULL values : '|| DIVDISTNULL  );


COMMIT;


END DISPLAY_COUNT_DEVICES_NEW ;

/
--------------------------------------------------------
--  DDL for Procedure GET_SECTIONALIZER_HISTORY
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."GET_SECTIONALIZER_HISTORY" (
  resultSet OUT SYS_REFCURSOR, workType IN VARCHAR2)
IS
BEGIN

  OPEN resultSet FOR
  SELECT HISTORY_ID, WORK_TYPE, DEVICE_ID, LAST_MODIFIED, COMMENTS 
  FROM CEDSA_SECTIONALIZER_HIST 
  WHERE WORK_TYPE = workType;
  
END;

/
--------------------------------------------------------
--  DDL for Procedure SP_CEDSA_MIGRATE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_CEDSA_MIGRATE" 
AS
NUM  NUMBER;

BEGIN



INSERT INTO CEDSA_CAPACITOR SELECT * FROM EDSETTCEDSA.CAPACITOR;
 
INSERT INTO CEDSA_CAPACITOR_HIST SELECT * FROM EDSETTCEDSA.CAPACITOR_HIST;
 
INSERT INTO CEDSA_CAPACITOR_SCHEDULES SELECT * FROM EDSETTCEDSA.CAPACITOR_SCHEDULES;
 
INSERT INTO CEDSA_CAPACITOR_SETTINGS SELECT * FROM EDSETTCEDSA.CAPACITOR_SETTINGS;
 
INSERT INTO CEDSA_CIRCUIT SELECT * FROM EDSETTCEDSA.CIRCUIT;
 
INSERT INTO CEDSA_CIRCUIT_BREAKER SELECT * FROM EDSETTCEDSA.CIRCUIT_BREAKER;
 
INSERT INTO CEDSA_CIRCUIT_RELAY SELECT * FROM EDSETTCEDSA.CIRCUIT_RELAY;
 
INSERT INTO CEDSA_INTERRUPTER SELECT * FROM  EDSETTCEDSA.INTERRUPTER;
 
INSERT INTO CEDSA_INTERRUPTER_HIST SELECT * FROM  EDSETTCEDSA.INTERRUPTER_HIST;
 
INSERT INTO CEDSA_INTERRUPTER_SETTINGS SELECT * FROM  EDSETTCEDSA.INTERRUPTER_SETTINGS;
 
INSERT INTO CEDSA_RECLOSER SELECT * FROM  EDSETTCEDSA.RECLOSER;
 
INSERT INTO CEDSA_RECLOSER_HIST SELECT * FROM  EDSETTCEDSA.RECLOSER_HIST;

INSERT INTO CEDSA_RECLOSER_SETTINGS SELECT * FROM  EDSETTCEDSA.RECLOSER_SETTINGS;
 
INSERT INTO CEDSA_REGULATOR SELECT * FROM  EDSETTCEDSA.REGULATOR;
 
INSERT INTO CEDSA_REGULATOR_BANK SELECT * FROM  EDSETTCEDSA.REGULATOR_BANK;
 
INSERT INTO CEDSA_REGULATOR_BANK_HIST SELECT * FROM  EDSETTCEDSA.REGULATOR_BANK_HIST;
 
INSERT INTO CEDSA_REGULATOR_HIST SELECT * FROM  EDSETTCEDSA.REGULATOR_HIST;
 
INSERT INTO CEDSA_REGULATOR_SETTINGS  SELECT * FROM  EDSETTCEDSA.REGULATOR_SETTINGS;
 
INSERT INTO CEDSA_SCADA  SELECT * FROM  EDSETTCEDSA.SCADA;
 
INSERT INTO CEDSA_SECTIONALIZER  SELECT * FROM  EDSETTCEDSA.SECTIONALIZER;
 
INSERT INTO CEDSA_SECTIONALIZER_HIST  SELECT * FROM  EDSETTCEDSA.SECTIONALIZER_HIST;
 
INSERT INTO CEDSA_SECTIONALIZER_SETTINGS  SELECT * FROM  EDSETTCEDSA.SECTIONALIZER_SETTINGS;
 
INSERT INTO CEDSA_SWITCH  SELECT * FROM  EDSETTCEDSA.SWITCH;
 
INSERT INTO CEDSA_SWITCH_HIST  SELECT * FROM  EDSETTCEDSA.SWITCH_HIST;


COMMIT;




SELECT COUNT(*) INTO NUM FROM CEDSA_CAPACITOR;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CAPACITOR : '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_CAPACITOR_HIST;
 
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CAPACITOR_HIST : '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_CAPACITOR_SCHEDULES;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CAPACITOR_SCHEDULES: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_CAPACITOR_SETTINGS;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CAPACITOR_SETTINGS: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_CIRCUIT;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CIRCUIT: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_CIRCUIT_BREAKER;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CIRCUIT_BREAKER: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_CIRCUIT_RELAY;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_CIRCUIT_RELAY: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_INTERRUPTER;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_INTERRUPTER: '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_INTERRUPTER_HIST;
 
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_INTERRUPTER_HIST: '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_INTERRUPTER_SETTINGS;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_INTERRUPTER_SETTINGS: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_RECLOSER;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_RECLOSER: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_RECLOSER_HIST;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_RECLOSER_HIST: '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_RECLOSER_SETTINGS;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_RECLOSER_SETTINGS: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_REGULATOR;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_REGULATOR: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_REGULATOR_BANK;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_REGULATOR_BANK: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_REGULATOR_BANK_HIST;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_REGULATOR_BANK_HIST: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_REGULATOR_HIST;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_REGULATOR_HIST: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_REGULATOR_SETTINGS;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_REGULATOR_SETTINGS: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_SCADA;
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SCADA: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_SECTIONALIZER;
 
DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SECTIONALIZER: '|| NUM);

SELECT COUNT(*) INTO NUM FROM CEDSA_SECTIONALIZER_HIST;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SECTIONALIZER_HIST: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_SECTIONALIZER_SETTINGS;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SECTIONALIZER_SETTINGS: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_SWITCH;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SWITCH: '|| NUM);
 
SELECT COUNT(*) INTO NUM FROM CEDSA_SWITCH_HIST;

DBMS_OUTPUT.PUT_LINE('Number of rows in CEDSA_SWITCH_HIST: '|| NUM);




END SP_CEDSA_MIGRATE ;

/
--------------------------------------------------------
--  DDL for Procedure SP_GIS_MIGRATE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_GIS_MIGRATE" 
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
--------------------------------------------------------
--  DDL for Procedure SP_GIS_MIGRATE_UPD
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_GIS_MIGRATE_UPD" 
AS
NUM  NUMBER;

BEGIN


---- Insert DEVICE_ID, OPER_NUM into GIS tables for Devices where (DEVICE_ID is NULL or 9999) from CEDSA

DELETE from CEDSA_DEVICE_TMP;
DELETE from GIS_CEDSADEVICEID_CB;
DELETE from GIS_CEDSADEVICEID_VR;

INSERT into GIS_CEDSADEVICEID_CB (GLOBAL_ID,OPERATING_NUM,DEVICE_ID)
select G.global_id, operating_num, CC.DEVICE_ID from gis_cedsadeviceid G, CEDSA_CIRCUIT CC,
  (select * from edsettgis.subinterruptingdevice 
    where  SUBSTATIONID is not null 
    AND OPERATINGNUMBER is not null) SI
where feature_class_name = 'CircuitBreaker'
and (G.device_id is null or G.device_id = '9999')
and G.global_id = SI.globalid
and CC.fdr_# is not null
and TO_CHAR(CC.FDR_#) = TO_CHAR(SI.SUBSTATIONID) || TO_CHAR(SUBSTR(SI.OPERATINGNUMBER,0,4))
order by operating_num;

INSERT into GIS_CEDSADEVICEID_VR ( GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,OPERATING_NUM)
   select GLOBAL_ID,FEATURE_CLASS_NAME,GIS_FEATURE_CLASS_NAME,DEVICE_ID,DIVISION,DISTRICT,BANKCODE,
   replace (replace (replace (replace (operating_num, ' - Bank Code '), ' - Bank Code 1'), ' - Bank Code 2'), ' - Bank Code 3') from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999) 
and feature_class_name = 'Regulator' ;
   

INSERT into CEDSA_DEVICE_TMP (device_id,oper_#)
 (select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
  (select oper_# from (select  cd.device_id,cd.oper_#   from  CEDSA_DEVICE CD,(select * from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999) and  feature_class_name = 'Regulator') GD 
  where  CD.OPER_#= replace (replace (replace (replace (GD.operating_num, ' - Bank Code '), ' - Bank Code 1'), ' - Bank Code 2'), ' - Bank Code 3') )  GDO
  group by GDO.oper_# having count(*)  =1 ) );

 
INSERT into CEDSA_DEVICE_TMP (device_id,oper_#)
  (select device_id,oper_# from  CEDSA_DEVICE CD1 where CD1.oper_# IN
    (select oper_# from (select cd.device_id,cd.oper_#  from  CEDSA_DEVICE CD,(select * from GIS_CEDSADEVICEID where (DEVICE_ID is null OR  DEVICE_ID=9999) and feature_class_name <> 'Regulator') GD 
     where  CD.OPER_#= GD.operating_num ) GDO
  group by GDO.oper_# having count(*)  =1 ) ) ;

COMMIT;

---- Update DEVICE_ID for Devices where DEVICE_ID is NULL or 9999

UPDATE   (
select GD.device_id old_device_id, UGD.device_id new_device_id
from GIS_CEDSADEVICEID GD, CEDSA_DEVICE_TMP UGD
where GD.operating_num=UGD.oper_# )
set old_device_id=new_device_id;


UPDATE    (
select GD.device_id old_device_id, UGD.device_id new_device_id
from GIS_CEDSADEVICEID_VR GD, CEDSA_DEVICE_TMP UGD
where GD.operating_num=UGD.oper_# )
set old_device_id=new_device_id;


UPDATE    (
select GD.device_id old_device_id, GDR.device_id new_device_id
from GIS_CEDSADEVICEID_VR GDR,  GIS_CEDSADEVICEID GD
where GD.GLOBAL_ID=GDR.GLOBAL_ID)
set old_device_id=new_device_id;


---- Updating DEVICE_ID for Ciruit Breaker where DEVICE_ID is NULL or 9999


UPDATE   (
select GD.device_id old_device_id, GDC.device_id new_device_id
from GIS_CEDSADEVICEID_CB GDC,  GIS_CEDSADEVICEID GD
where GD.GLOBAL_ID=GDC.GLOBAL_ID)
set old_device_id=new_device_id;


COMMIT;

END SP_GIS_MIGRATE_UPD ;

/
--------------------------------------------------------
--  DDL for Procedure SP_SM_CAPACITOR
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_CAPACITOR" 
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
      NOTES,
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
    CS.USER_AUDIT,
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
      NOTES,
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
    CS.USER_AUDIT,
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
--------------------------------------------------------
--  DDL for Procedure SP_SM_CIRCUIT_BREAKER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_CIRCUIT_BREAKER" 
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN 

INSERT INTO SM_CIRCUIT_BREAKER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,
DEVICE_ID,DATE_MODIFIED,NOTES,CURRENT_FUTURE,PHA_PR_RELAY_TYPE,PHA_PR_MIN_TRIP,PHA_PR_INS_TRIP,PHA_PR_LEVER_SET,
MIN_NOR_VOLT,ANNUAL_LF,NETWORK,DPA_CD,CC_RATING,FLISR,SCADA,DIRECT_TRANSFER_TRIP,RECLOSE_BLOCKING)
SELECT GD.GLOBAL_ID,'EDGIS.SubInterruptingDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
CR.CIRCUIT_BREAKER,CR.LAST_MODIFIED,CR.USER_AUDIT,'C',CR.RELAY_TYPE,CR.MIN_TRIP,CR.INS_TRIP,CR.LEVER_SET,
C.MIN_NOR_VOLT,C.ANNUAL_LF,C.NETWORK,C.DPA_CD,C.CC_RATING,C.FLISR,C.SCADA,C.DIRECT_TRANSFER_TRIP,C.RECLOSE_BLOCKING
FROM  CEDSA_CIRCUIT C ,CEDSA_CIRCUIT_RELAY CR,GIS_CEDSADEVICEID  GD   
WHERE CR.CIRCUIT_BREAKER=C.DEVICE_ID and CR.CIRCUIT_BREAKER=GD.DEVICE_ID and GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND CR.RELAY_CD='PHA';


INSERT INTO SM_CIRCUIT_BREAKER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,
DEVICE_ID,DATE_MODIFIED,NOTES,CURRENT_FUTURE,PHA_PR_RELAY_TYPE,PHA_PR_MIN_TRIP,PHA_PR_INS_TRIP,PHA_PR_LEVER_SET,
MIN_NOR_VOLT,ANNUAL_LF,NETWORK,DPA_CD,CC_RATING,FLISR,SCADA,DIRECT_TRANSFER_TRIP,RECLOSE_BLOCKING)
SELECT GD.GLOBAL_ID,'EDGIS.SubInterruptingDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
CR.CIRCUIT_BREAKER,CR.LAST_MODIFIED,CR.USER_AUDIT,'F',CR.RELAY_TYPE,CR.MIN_TRIP,CR.INS_TRIP,CR.LEVER_SET,
C.MIN_NOR_VOLT,C.ANNUAL_LF,C.NETWORK,C.DPA_CD,C.CC_RATING,C.FLISR,C.SCADA,C.DIRECT_TRANSFER_TRIP,C.RECLOSE_BLOCKING
FROM  CEDSA_CIRCUIT C ,CEDSA_CIRCUIT_RELAY CR,GIS_CEDSADEVICEID  GD   
WHERE CR.CIRCUIT_BREAKER=C.DEVICE_ID and CR.CIRCUIT_BREAKER=GD.DEVICE_ID and GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND CR.RELAY_CD='PHA';


COMMIT;

BEGIN

FOR I IN (SELECT CR.CIRCUIT_BREAKER,GD.GLOBAL_ID,GD.FEATURE_CLASS_NAME,GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,CR.LAST_MODIFIED,CR.USER_AUDIT,CR.RELAY_TYPE,CR.MIN_TRIP,CR.INS_TRIP,CR.LEVER_SET,
C.MIN_NOR_VOLT,C.ANNUAL_LF,C.NETWORK ,C.DPA_CD,C.CC_RATING,C.FLISR,C.SCADA
FROM  CEDSA_CIRCUIT C ,CEDSA_CIRCUIT_RELAY CR,GIS_CEDSADEVICEID  GD   
WHERE CR.CIRCUIT_BREAKER=C.DEVICE_ID and CR.CIRCUIT_BREAKER=GD.DEVICE_ID and GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND CR.RELAY_CD='GRD')

LOOP

	UPDATE SM_CIRCUIT_BREAKER  SET
GRD_PR_RELAY_TYPE=I.RELAY_TYPE,GRD_PR_MIN_TRIP=I.MIN_TRIP,GRD_PR_INS_TRIP=I.INS_TRIP,GRD_PR_LEVER_SET=I.LEVER_SET
WHERE DEVICE_ID=I.CIRCUIT_BREAKER;

END LOOP;
END;


COMMIT;

/*  Commented as per change request on 062714

UPDATE  SM_CIRCUIT_BREAKER
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_CIRCUIT_BREAKER
SET SCADA='N' 
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC);  */


BEGIN
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_CIRCUIT_BREAKER RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID ) 
LOOP
UPDATE SM_CIRCUIT_BREAKER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
END LOOP;
END;

COMMIT;

SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='CircuitBreaker';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_CIRCUIT_RELAY;
SELECT COUNT(*) INTO SM_SEC   FROM SM_CIRCUIT_BREAKER;

DBMS_OUTPUT.PUT_LINE('Count of CIRCUIT_BREAKER from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of CIRCUIT_BREAKER from CEDSA_CIRCUIT_RELAY : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of CIRCUIT_BREAKER from SM_CIRCUIT_BREAKER : '|| SM_SEC);


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.FEATURE_CLASS_NAME ='CircuitBreaker' AND  GD.GIS_FEATURE_CLASS_NAME = 'EDGIS.SubInterruptingDevice'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_CIRCUIT_BREAKER WHERE SM_CIRCUIT_BREAKER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_CIRCUIT_BREAKER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_CIRCUIT_BREAKER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;


SELECT COUNT(*) INTO SM_SEC   FROM SM_CIRCUIT_BREAKER ;
DBMS_OUTPUT.PUT_LINE('Count of CircuitBreaker from SM_CIRCUIT_BREAKER after inserting default C/F records: '|| SM_SEC);


END SP_SM_CIRCUIT_BREAKER ;

/
--------------------------------------------------------
--  DDL for Procedure SP_SM_INTERRUPTER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_INTERRUPTER" 
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN 

INSERT INTO SM_INTERRUPTER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID ,OK_TO_BYPASS ,CONTROL_SERIAL_NUM ,
MANF_CD ,CONTROL_TYPE ,FIRMWARE_VERSION ,SOFTWARE_VERSION,CURRENT_FUTURE,GRD_CUR_TRIP,GRD_TRIP_CD,TYP_CRV_GRD,PHA_CUR_TRIP,PHA_TRIP_CD,TYP_CRV_PHA,PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,NOTES)
SELECT GD.GLOBAL_ID,'EDGIS.DynamicProtectiveDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,CI.DEVICE_ID ,CI.OK_TO_BYPASS, CI.CONTROL_SERIAL_# ,
CI.MANF_CD ,CI.CONTROL_TYPE ,CI.FIRMWARE_VERSION ,CI.SOFTWARE_VERSION ,CS.CURRENT_FUTURE,CS.GRD_CUR_TRIP,CS.GRD_TRIP_CD,CS.TYP_CRV_GRD,
CS.PHA_CUR_TRIP, CS.PHA_TRIP_CD, CS.TYP_CRV_PHA,CS.PREPARED_BY,CS.LAST_MODIFIED,CS.EFFECTIVE_DATE,CS.USER_AUDIT
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
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_INTERRUPTER RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID ) 
LOOP
UPDATE SM_INTERRUPTER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
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
--------------------------------------------------------
--  DDL for Procedure SP_SM_NETWORK_PROTECTOR
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_NETWORK_PROTECTOR" 
AS
  GD_COUNT NUMBER;
  SEC_SETT NUMBER;
  SM_SEC   NUMBER;

BEGIN


FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.FEATURE_CLASS_NAME ='NetworkProtector'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_NETWORK_PROTECTOR WHERE SM_NETWORK_PROTECTOR.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_NETWORK_PROTECTOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.NetworkProtector', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_NETWORK_PROTECTOR(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, 'EDGIS.NetworkProtector', I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;


SELECT COUNT(*) INTO SM_SEC   FROM SM_NETWORK_PROTECTOR ;

DBMS_OUTPUT.PUT_LINE('Count of CircuitBreaker from SM_NETWORK_PROTECTOR after inserting default C/F records: '|| SM_SEC);



END SP_SM_NETWORK_PROTECTOR ;

/
--------------------------------------------------------
--  DDL for Procedure SP_SM_RECLOSER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_RECLOSER" 
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN 

INSERT INTO SM_RECLOSER ( GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID,CONTROL_SERIAL_NUM,CONTROL_TYPE,BYPASS_PLANS,OPERATING_AS_CD,
NOTES,CURRENT_FUTURE,GRD_MIN_TRIP,PHA_MIN_TRIP,GRD_INST_TRIP_CD,PHA_INST_TRIP_CD,GRD_OP_F_CRV,PHA_OP_F_CRV,GRD_RESP_TIME,PHA_RESP_TIME,GRD_FAST_CRV,PHA_FAST_CRV,
GRD_SLOW_CRV,PHA_SLOW_CRV,GRD_VMUL_FAST,PHA_VMUL_FAST,GRD_VMUL_SLOW,PHA_VMUL_SLOW,GRD_TADD_FAST,PHA_TADD_FAST,GRD_TADD_SLOW,PHA_TADD_SLOW,TOT_LOCKOUT_OPS,
RECLOSE1_TIME,RECLOSE2_TIME,RECLOSE3_TIME,RESET,SGF_CD,SGF_MIN_TRIP_PERCENT,SGF_TIME_DELAY,ALT_GRD_MIN_TRIP,ALT_PHA_MIN_TRIP,ALT_GRD_INST_TRIP_CD,ALT_PHA_INST_TRIP_CD,
ALT_GRD_OP_F_CRV,ALT_PHA_OP_F_CRV,ALT_GRD_RESP_TIME,ALT_PHA_RESP_TIME,ALT_GRD_FAST_CRV,ALT_PHA_FAST_CRV,ALT_GRD_SLOW_CRV,ALT_PHA_SLOW_CRV,ALT_GRD_VMUL_FAST,
ALT_PHA_VMUL_FAST,ALT_GRD_VMUL_SLOW,ALT_PHA_VMUL_SLOW,ALT_GRD_TADD_FAST,ALT_PHA_TADD_FAST,ALT_GRD_TADD_SLOW,ALT_PHA_TADD_SLOW,ALT_TOT_LOCKOUT_OPS,
ALT_RECLOSE1_TIME,ALT_RECLOSE2_TIME,ALT_RECLOSE3_TIME,ALT_RESET,ALT_SGF_CD,ALT_SGF_MIN_TRIP_PERCENT,ALT_SGF_TIME_DELAY,ALT2_PERMIT_LS_ENABLING,ALT2_GRD_ARMING_THRESHOLD,
ALT2_PHA_ARMING_THRESHOLD,ALT2_GRD_INRUSH_THRESHOLD,ALT2_PHA_INRUSH_THRESHOLD,ALT2_INRUSH_DURATION,ALT2_LS_LOCKOUT_OPS,ALT2_LS_RESET_TIME,ACTIVE_PROFILE,PERMIT_RB_CUTIN,DIRECT_TRANSFER_TRIP,BOC_VOLTAGE,RB_CUTOUT_TIME,DATE_MODIFIED)

SELECT  GD.GLOBAL_ID,'EDGIS.DynamicProtectiveDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,RE.DEVICE_ID,RE.CONTROL_SERIAL_#,
RE.CONTROL_TYPE,RE.BYPASS_PLANS,RE.OPERATING_AS_CD,RS.USER_AUDIT,RS.CURRENT_FUTURE,RS.GRD_MIN_TRIP,RS.PHA_MIN_TRIP,RS.GRD_INST_TRIP_CD,RS.PHA_INST_TRIP_CD,
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
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_RECLOSER RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID ) 
LOOP
UPDATE SM_RECLOSER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
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
--------------------------------------------------------
--  DDL for Procedure SP_SM_REGULATOR
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_REGULATOR" 
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN

INSERT INTO SM_REGULATOR (GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,
DEVICE_ID,PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,NOTES,CURRENT_FUTURE,PRIMARY_CT_RATING,BAND_WIDTH,PT_RATIO,RANGE_UNBLOCKED,
BLOCKED_PCT,STEPS,PEAK_LOAD,MIN_LOAD,PVD_MAX,PVD_MIN,SVD_MIN,POWER_FACTOR,LOAD_CYCLE,RISE_RATING,TIMER,
CONTROL_TYPE,FIRMWARE_VERSION,SOFTWARE_VERSION)
SELECT GD.GLOBAL_ID,'EDGIS.VoltageRegulatorUnit',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
RE.DEVICE_ID,RE.PREPARED_BY,RE.LAST_MODIFIED,RE.EFFECTIVE_DATE,RE.USER_AUDIT,RE.CURRENT_FUTURE,RE.PRIMARY_CT_RATING,RE.BAND_WIDTH,
RE.PT_RATIO,RE.RANGE_UNBLOCKED,RE.BLOCKED_PCT,RE.STEPS,RE.PEAK_LOAD,RE.MIN_LOAD,RE.PVD_MAX,RE.PVD_MIN,RE.SVD_MIN,RE.POWER_FACTOR,
RE.LOAD_CYCLE,RE.RISE_RATING,RE.TIMER,
RB.CONTROL_TYPE,RB.FIRMWARE_VERSION,RB.SOFTWARE_VERSION
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
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_REGULATOR RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID )
LOOP
UPDATE SM_REGULATOR  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
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

SELECT COUNT(*) INTO SM_SEC   FROM SM_REGULATOR ;
DBMS_OUTPUT.PUT_LINE('Count of REGULATOR from SM_REGULATOR after inserting default C/F records: '|| SM_SEC);





END SP_SM_REGULATOR ;

/
--------------------------------------------------------
--  DDL for Procedure SP_SM_SECTIONALIZER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_SECTIONALIZER" 
AS

GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN 
      	
INSERT INTO SM_SECTIONALIZER ( GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID,PREPARED_BY,CONTROL_SERIAL_NUM,DATE_MODIFIED,EFFECTIVE_DT,NOTES,CURRENT_FUTURE,CONTROL_TYPE,SECT_TYPE,MIN_PC_TO_CT,
PHA_INRUSH_DURATION,PHA_INRUSH_MULTIPLIER,PHA_INRUSH_TIME,FIRST_RECLOSE_RESET_TIME,REQUIRED_FAULT_CURRENT,MIN_GRD_TO_CT,GRD_INRUSH_DURATION,GRD_INRUSH_MULTIPLIER,GRD_INRUSH_TIME,
RESET,VOLT_THRESHOLD,LOCKOUT_NUM,ONE_SHOT_LOCKOUT_NUM,FIRMWARE_VERSION,SOFTWARE_VERSION)
SELECT GD.GLOBAL_ID,'EDGIS.DynamicProtectiveDevice',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,SE.DEVICE_ID,SS.PREPARED_BY,SE.CONTROL_SERIAL_#,SS.LAST_MODIFIED,SS.EFFECTIVE_DATE,SS.USER_AUDIT,SS.CURRENT_FUTURE,SE.CONTROL_TYPE,SE.SECT_TYPE,
SS.MIN_PC_TO_CT,SS.PHA_INRUSH_DURATION,SS.PHA_INRUSH_MULTIPLIER,SS.PHA_INRUSH_TIME,SS.FIRST_RECLOSE_RESET_TIME,SS.REQUIRED_FAULT_CURRENT,SS.MIN_GRD_TO_CT,
SS.GRD_INRUSH_DURATION,SS.GRD_INRUSH_MULTIPLIER,SS.GRD_INRUSH_TIME,SS.RESET,SS.VOLT_THRESHOLD,SS.LOCKOUT_#,SS.ONE_SHOT_LOCKOUT_#,
SE.FIRMWARE_VERSION,SE.SOFTWARE_VERSION
FROM CEDSA_SECTIONALIZER SE, CEDSA_SECTIONALIZER_SETTINGS SS,GIS_CEDSADEVICEID  GD
WHERE SE.DEVICE_ID=SS.DEVICE_ID AND SE.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Sectionalizer';

UPDATE  SM_SECTIONALIZER 
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_SECTIONALIZER 
SET SCADA='N' 
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC) ;

COMMIT;

BEGIN
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_SECTIONALIZER SE   WHERE CD.DEVICE_ID =SE.DEVICE_ID ) 
LOOP
UPDATE SM_SECTIONALIZER  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
END LOOP;
END;

INSERT INTO SM_COMMENT_HIST ( GLOBAL_ID,WORK_DATE,WORK_TYPE,PERFORMED_BY,ENTRY_DATE,COMMENTS) 
SELECT  GD.GLOBAL_ID,SH.WORK_DATE,SH.WORK_TYPE,SH.PERFORMED_BY,SH.ENTRY_DATE,SH.COMMENTS
FROM GIS_CEDSADEVICEID  GD,CEDSA_SECTIONALIZER_HIST SH  WHERE SH.DEVICE_ID=GD.DEVICE_ID; 


SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Sectionalizer';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_SECTIONALIZER_SETTINGS;
SELECT COUNT(*) INTO SM_SEC   FROM SM_SECTIONALIZER ;

DBMS_OUTPUT.PUT_LINE('Count of Sectionalizer from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of Sectionalizer from CEDSA_SECTIONALIZER_SETTINGS : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of Sectionalizer from SM_SECTIONALIZER : '|| SM_SEC);

COMMIT;

BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME,GD.GIS_FEATURE_CLASS_NAME, GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.FEATURE_CLASS_NAME ='Sectionalizer' AND GD.GIS_FEATURE_CLASS_NAME='EDGIS.DynamicProtectiveDevice'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_SECTIONALIZER WHERE SM_SECTIONALIZER.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_SECTIONALIZER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C');

INSERT INTO SM_SECTIONALIZER(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F');

COMMIT;
END LOOP;
END;

SELECT COUNT(*) INTO SM_SEC   FROM SM_SECTIONALIZER ;
DBMS_OUTPUT.PUT_LINE('Count of Sectionalizer from SM_SECTIONALIZER after inserting default C/F records: '|| SM_SEC);


END SP_SM_SECTIONALIZER ;

/
--------------------------------------------------------
--  DDL for Procedure SP_SM_SWITCH
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDSETT"."SP_SM_SWITCH" 
AS
GD_COUNT  NUMBER;
SEC_SETT  NUMBER;
SM_SEC	  NUMBER;

BEGIN 

INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,CURRENT_FUTURE,
DEVICE_ID,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,SWITCH_TYPE ,SECTIONALIZING_FEATURE,FIRMWARE_VERSION,SOFTWARE_VERSION,DATE_MODIFIED)
SELECT GD.GLOBAL_ID,'EDGIS.Switch',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
'C',CS.DEVICE_ID,CS.CONTROL_SERIAL_#,CS.CONTROL_TYPE,CS.SWITCH_TYPE,CS.SECTIONALIZING_FEATURE,CS.FIRMWARE_VERSION,CS.SOFTWARE_VERSION,SYSDATE
FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch';

INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,CURRENT_FUTURE,
DEVICE_ID,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,SWITCH_TYPE ,SECTIONALIZING_FEATURE,FIRMWARE_VERSION,SOFTWARE_VERSION,DATE_MODIFIED)
SELECT GD.GLOBAL_ID,'EDGIS.Switch',GD.OPERATING_NUM,GD.DIVISION,GD.DISTRICT,
'F',CS.DEVICE_ID,CS.CONTROL_SERIAL_#,CS.CONTROL_TYPE,CS.SWITCH_TYPE,CS.SECTIONALIZING_FEATURE,CS.FIRMWARE_VERSION,CS.SOFTWARE_VERSION,SYSDATE
FROM CEDSA_SWITCH CS ,GIS_CEDSADEVICEID  GD   
WHERE CS.DEVICE_ID=GD.DEVICE_ID AND GD.FEATURE_CLASS_NAME ='Switch';



COMMIT;


UPDATE  SM_SWITCH
SET SCADA='Y'
WHERE DEVICE_ID IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC ) ;

UPDATE  SM_SWITCH
SET SCADA='N' 
WHERE DEVICE_ID NOT IN ( SELECT DEVICE_ID FROM CEDSA_SCADA SC);


BEGIN
FOR I IN (SELECT CD.SCADA_TYPE,CD.RADIO_MANF_CD,CD.RADIO_MODEL_#,CD.RADIO_SERIAL_#  FROM CEDSA_SCADA CD ,SM_SWITCH RE   WHERE CD.DEVICE_ID =RE.DEVICE_ID ) 
LOOP
UPDATE SM_SWITCH  SET  SCADA_TYPE=I.SCADA_TYPE,RADIO_MANF_CD=I.RADIO_MANF_CD,RADIO_MODEL_NUM=I.RADIO_MODEL_#,RADIO_SERIAL_NUM=I.RADIO_SERIAL_#;
END LOOP;
END;

INSERT INTO SM_COMMENT_HIST ( GLOBAL_ID,WORK_DATE,WORK_TYPE,PERFORMED_BY,ENTRY_DATE,COMMENTS) 
SELECT  GD.GLOBAL_ID,SH.WORK_DATE,SH.WORK_TYPE,SH.PERFORMED_BY,SH.ENTRY_DATE,SH.COMMENTS
FROM GIS_CEDSADEVICEID  GD,CEDSA_SWITCH_HIST SH  WHERE SH.DEVICE_ID=GD.DEVICE_ID; 

COMMIT;

SELECT COUNT(*) INTO GD_COUNT FROM GIS_CEDSADEVICEID  GD WHERE GD.FEATURE_CLASS_NAME ='Switch';
SELECT COUNT(*) INTO SEC_SETT FROM CEDSA_SWITCH;
SELECT COUNT(*) INTO SM_SEC   FROM SM_SWITCH;

DBMS_OUTPUT.PUT_LINE('Count of SWITCH from GIS_CEDSADEVICEID : '|| GD_COUNT );
DBMS_OUTPUT.PUT_LINE('Count of SWITCH from CEDSA_SWITCH : '|| SEC_SETT);
DBMS_OUTPUT.PUT_LINE('Count of SWITCH from SM_SWITCH : '|| SM_SEC);


BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME,GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.Switch'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_SWITCH WHERE SM_SWITCH.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE,DATE_MODIFIED)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C',SYSDATE);

INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE,DATE_MODIFIED)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F',SYSDATE);

END LOOP;
END;




BEGIN
FOR I IN (SELECT GD.GLOBAL_ID, GD.FEATURE_CLASS_NAME, GD.GIS_FEATURE_CLASS_NAME,GD.OPERATING_NUM, GD.DIVISION, GD.DISTRICT, GD.DEVICE_ID
FROM  GIS_CEDSADEVICEID GD WHERE  GD.GIS_FEATURE_CLASS_NAME ='EDGIS.SubSwitch'
AND   NOT EXISTS (SELECT DEVICE_ID FROM SM_SWITCH WHERE SM_SWITCH.DEVICE_ID = GD.DEVICE_ID))

LOOP
INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE,DATE_MODIFIED)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'C',SYSDATE);

INSERT INTO SM_SWITCH(GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,DEVICE_ID, CURRENT_FUTURE,DATE_MODIFIED)
VALUES( I.GLOBAL_ID, I.GIS_FEATURE_CLASS_NAME, I.OPERATING_NUM, I.DIVISION, I.DISTRICT, I.DEVICE_ID,'F',SYSDATE);

END LOOP;
END;


UPDATE SM_SWITCH SET SWITCH_TYPE='1'   WHERE SWITCH_TYPE IS NULL;
UPDATE SM_SWITCH SET  ATS_CAPABLE='N'  WHERE  ATS_CAPABLE IS NULL;
COMMIT;

SELECT COUNT(*) INTO SM_SEC   FROM SM_SWITCH ;
DBMS_OUTPUT.PUT_LINE('Count of Switch from SM_SWITCH after inserting default C/F records: '|| SM_SEC);



END SP_SM_SWITCH ;

/


spool off
