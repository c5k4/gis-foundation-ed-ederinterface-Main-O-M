--------------------------------------------------------
--  File created - Tuesday-September-16-2014   
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
  UPD_CODE_FIVE EXCEPTION;
  INS_CODE_SIX EXCEPTION;


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
  

  SELECT COUNT(*)
  INTO NUM1
  FROM SM_FC_LAYER_MAPPING
  WHERE FEATURE_CLASS_NAME = I_feature_class_name
  AND SUBTYPE              = I_feature_class_subtype;



  IF NUM1 = 0 THEN
  RAISE CONFIG_ERR;
  ELSE
  SELECT DISTINCT(SM_TABLE)
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
WHEN UPD_CODE_FIVE THEN
  err_num :=5;
  err_msg := 'Data already exists in Settings,Updated the record with passed values ';
  INSERT INTO sm_errors VALUES
    (I_Global_id_Current, err_num,err_msg
    );
WHEN INS_CODE_SIX THEN
  err_num := 6;
  err_msg := 'Data not found in the Settings, Inserted a new record using passed values';
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
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
-- Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
-- Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
-- Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SECTIONALIZER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
  
  
  -- Assign the ACTION Value to REASON_TYPE to process the txn
  
  
  
  IF ACTION = 'I' THEN
    BEGIN       
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
       
     --  If the Valid num was assinged to 5 in earlier validations, then riase Update code   
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;      
      
    END;
  END IF;
  
  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
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
          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
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
      
      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
       
    END;
  END IF;
  
  IF ACTION = 'D' THEN
    BEGIN
      
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
          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
       
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
       
    END;
  END IF;
  
  IF ACTION = 'R' THEN
    BEGIN
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
          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
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
      
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
             
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
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;
  
  --- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
-- Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID = I_Global_id_Current;
          
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CAPACITOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
   
  
  IF ACTION = 'I' THEN
    BEGIN     
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
      
        IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
        END IF;
      
    END;
  END IF;
  IF ACTION = 'U' THEN
    BEGIN
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
          SM_CAPACITOR_HIST_SEQ.NEXTVAL,
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
     
      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;  
    END;
  END IF;
  IF ACTION = 'D' THEN  
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
        SM_CAPACITOR_HIST_SEQ.NEXTVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
     
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
  END IF;
  IF ACTION = 'R' THEN
    BEGIN
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
          SM_CAPACITOR_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_CAPACITOR WHERE GLOBAL_ID = I_Global_id_Previous;
       
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      
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
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID = I_Global_id_Current;
      
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_CIRCUIT_BREAKER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
   
  IF ACTION = 'I' THEN
    BEGIN     
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
      
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;  
      
    END;
  END IF;
  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
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
          SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
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
      
        -- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      
    END;
  END IF;
  IF ACTION = 'D' THEN
  BEGIN
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
        SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
      
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
  END;
  END IF;
  IF ACTION = 'R' THEN
    BEGIN
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
          SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_CIRCUIT_BREAKER WHERE GLOBAL_ID = I_Global_id_Previous;
    
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      
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
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    

      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_INTERRUPTER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
  

  IF ACTION = 'I' THEN
    BEGIN            
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
       
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
    END;
  END IF;
  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN  
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
          SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
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
    
      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      
    END;
  END IF;
  IF ACTION = 'D' THEN
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
        SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
     
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
  END IF;
  IF ACTION = 'R' THEN
    BEGIN
     
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
          SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_INTERRUPTER WHERE GLOBAL_ID = I_Global_id_Previous;
      
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
       
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
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    

      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_NETWORK_PROTECTOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
  

  IF ACTION = 'I' THEN
    BEGIN
      
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
      
      
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;   
      
    END;
  END IF;
  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
     
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
          SM_NETWORK_PROTECTOR_HIST_SEQ.NEXTVAL,
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
       
   --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
       
    END;
  END IF;
  IF ACTION= 'D' THEN

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
        SM_NETWORK_PROTECTOR_HIST_SEQ.NEXTVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
  
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
  END IF;
  IF ACTION = 'R' THEN
    BEGIN
      
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
          SM_NETWORK_PROTECTOR_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_NETWORK_PROTECTOR WHERE GLOBAL_ID = I_Global_id_Previous;
              
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 
      
    END;
  END IF;
END SP_NETWORK_PROTECTOR_DETECTION;


 PROCEDURE SP_RECLOSER_DETECTION
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
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    

      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_RECLOSER
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
  
  
 
 
 
  IF ACTION = 'I' THEN
    BEGIN
    
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
      
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;  
    END;
  END IF;
  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
     
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
          SM_RECLOSER_HIST_SEQ.NEXTVAL,
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
       
       --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
    END;
  END IF;
  IF ACTION= 'D' THEN   
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
        SM_RECLOSER_HIST_SEQ.NEXTVAL,
        sysdate,
        'OTHR',
        'SYSTEM',
        sysdate,
        'Record updated in GIS system'
      );
     
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
  END IF;
  IF ACTION = 'R' THEN
    BEGIN
      
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
          SM_RECLOSER_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );
      DELETE FROM SM_RECLOSER WHERE GLOBAL_ID = I_Global_id_Previous;
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
      
      
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
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    

      SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_REGULATOR
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_REGULATOR
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;
  
  

  IF ACTION = 'I' THEN
    BEGIN

  
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
       
    
       IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;      
      
    END;
  END IF;


  IF ACTION = 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
     

      -- first copy the entire current record to history table

INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT,  LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
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
           SM_REGULATOR_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );

      UPDATE SM_REGULATOR
      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,CONTROL_TYPE=I_Control_type_code
      WHERE GLOBAL_ID    = I_Global_id_Current;

   --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
       IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
       

    END;

  END IF;

  IF ACTION = 'D' THEN

  
      -- first copy the entire current record to history table


INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
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
          SM_REGULATOR_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );

     IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF; 

  END IF;




 IF ACTION = 'R' THEN

  BEGIN
 

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
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER)
SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
EFFECTIVE_DT, ENGINEERING_DOCUMENT, FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
MIN_LOAD, "MODE", NOTES, OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER
FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;



INSERT INTO SM_COMMENT_HIST ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS ) VALUES (
'SM_REGULATOR_HIST', SM_REGULATOR_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );


DELETE FROM SM_REGULATOR WHERE GLOBAL_ID = I_Global_id_Previous;

      IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
              

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
  REASON_TYPE CHAR;
  DEVICE_TYPE VARCHAR2(50);
  NUM1        NUMBER;
  NUM2        NUMBER;
  VALID_NUM   NUMBER;
  VAR1        VARCHAR2(50);
  ACTION      CHAR;
BEGIN
  
  REASON_TYPE   := I_reason_type ;
  VALID_NUM:=0;

--- Validatations for I/U/D/R conditions
CASE 
WHEN REASON_TYPE = 'I' THEN
BEGIN   
--Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0    

      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID = I_Global_id_Current;
      
     
      IF NUM1  >= 1 THEN          
        ACTION  :='U';
        VALID_NUM:=5;
      ELSE
       ACTION:='I';
     END IF;
END;

WHEN REASON_TYPE = 'U' THEN
BEGIN
--Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
       IF NUM1 < 1  THEN    	
          ACTION := 'I';      	   
          VALID_NUM:=6;
        ELSE
          ACTION:='U';
       END IF;
END;

WHEN REASON_TYPE = 'D' THEN
BEGIN
--Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
      
      IF NUM1 < 1  THEN
        VALID_NUM:=6;
        ACTION :='D';
      ELSE 
      ACTION := 'D';
      END IF;
END;

WHEN REASON_TYPE = 'R' THEN
BEGIN
-- Check if the previous global id exist in device table
      SELECT COUNT(*)
      INTO NUM2
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Previous
      AND CURRENT_FUTURE='C';
      -- Check if the current global id does not exist in device table
      SELECT COUNT(*)
      INTO NUM1
      FROM SM_SWITCH
      WHERE GLOBAL_ID   = I_Global_id_Current
      AND CURRENT_FUTURE='C';
 -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data

      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
        ACTION := 'R';
        VALID_NUM:=0;   
      END IF;
-- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN     				
        ACTION := 'U'; 
        VALID_NUM:=5;
      END IF;
-- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
        ACTION := 'I'; 
        VALID_NUM:=6;
      END IF;
END;
END CASE;


 

  IF ACTION = 'I' THEN
    BEGIN
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
       
    IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;   
    END;
  END IF;


  IF ACTION= 'U' THEN
    -- check to see if globalid exist in table. If it does not then throw exception
    BEGIN
      

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
           SM_SWITCH_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );

      UPDATE SM_SWITCH
      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,SWITCH_TYPE=I_Switch_type_code
      WHERE GLOBAL_ID    = I_Global_id_Current;

        IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
        END IF;

    END;

  END IF;

  IF ACTION = 'D' THEN
  

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

      -- Insert a record in comments table with notes set to INst

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
          SM_SWITCH_HIST_SEQ.NEXTVAL,
          sysdate,
          'OTHR',
          'SYSTEM',
          sysdate,
          'Record updated in GIS system'
        );

     IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;

  END IF;




    IF ACTION = 'R' THEN
    BEGIN
 
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
          SM_SWITCH_HIST_SEQ.NEXTVAL,
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

    IF VALID_NUM=6 THEN
        RAISE  INS_CODE_SIX;
      END IF;
      
      IF VALID_NUM=5 THEN
        RAISE  UPD_CODE_FIVE;
      END IF;
       


    END;
  END IF;

END SP_SWITCH_DETECTION;








END SM_CHANGE_DETECTION_PKG;

/
