spool D:\Temp\PB\StoredProcedures_Settings.txt

  CREATE OR REPLACE PACKAGE "EDSETT"."SM_CHANGE_DETECTION_PKG" AS
  GLOBALID VARCHAR2(40);
  err_num    NUMBER;
  err_msg    VARCHAR2(100);
  REC_FOUND   EXCEPTION;
  CONFIG_ERR EXCEPTION;
  UPD_CODE_FIVE EXCEPTION;
  INS_CODE_SIX EXCEPTION;
   --Changes for ENOS to SAP migration - change detection Start..
  DUPLICATE_RCORD_ERROR EXCEPTION;
  --Changes for ENOS to SAP migration - change detection End..

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
    I_Switch_type_code   IN VARCHAR2,
    I_Bank_code          IN NUMBER,
 --Changes for ENOS to SAP migration - change detection Start.
    I_Service_Point_GUID    IN VARCHAR2,
    I_SAP_EGI_Notification  IN VARCHAR2,
    I_Project_Name          IN VARCHAR2,
    I_Gen_Type              IN VARCHAR2,
    I_Program_Type          IN VARCHAR2,
    I_Eff_Rating_Mach_KW    IN NUMBER,
    I_Eff_Rating_Inv_KW     IN NUMBER,
    I_Eff_Rating_Mach_KVA   IN NUMBER,
    I_Eff_Rating_Inv_KVA    IN NUMBER,
    I_Backup_Gen            IN VARCHAR2,
    I_Max_Storage_Capacity  IN NUMBER,
    I_Charge_Demand_KW      IN NUMBER,
    I_Power_Source          IN VARCHAR2);
    --Changes for ENOS to SAP migration - change detection End.


    FUNCTION GET_GLOBALID RETURN VARCHAR2;

--    Change for project GIS powerbase integration - Start
--    PROCEDURE SP_SECTIONALIZER_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--
--
--    PROCEDURE SP_CAPACITOR_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--
--    PROCEDURE SP_CIRCUIT_BREAKER_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--
--
--    PROCEDURE SP_INTERRUPTER_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--    Change for project GIS powerbase integration - End

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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);

--    Change for project GIS powerbase integration - Start
--    PROCEDURE SP_RECLOSER_TS_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER
--  );
--  PROCEDURE SP_RECLOSER_FS_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER
--  );
--
--    PROCEDURE SP_RECLOSER_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--
--
--    PROCEDURE SP_REGULATOR_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--
--
--    PROCEDURE SP_SWITCH_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--    Change for project GIS powerbase integration - End

    PROCEDURE SP_BANKCODE_UPDATE;

    PROCEDURE SP_SPECIAL_LOAD_DETECTION(
    I_Global_id_Current  IN CHAR,
    I_reason_type        IN VARCHAR2,
    I_feature_class_name IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num      IN VARCHAR2,
    I_Global_id_Previous IN CHAR,
    I_Division           IN VARCHAR2,
    I_District           IN VARCHAR2,
    I_Control_type_code    IN VARCHAR2,
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);

--    Change for project GIS powerbase integration - Start
--    PROCEDURE SP_PRIMARY_METER_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER);
--    Change for project GIS powerbase integration - End

    PROCEDURE SP_PRIMARY_GEN_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER,
 --Changes for ENOS to SAP migration - change detection Start.
    I_Service_Point_GUID    IN VARCHAR2,
    I_SAP_EGI_Notification  IN VARCHAR2,
    I_Project_Name          IN VARCHAR2,
    I_Gen_Type              IN VARCHAR2,
    I_Program_Type          IN VARCHAR2,
    I_Eff_Rating_Mach_KW    IN NUMBER,
    I_Eff_Rating_Inv_KW     IN NUMBER,
    I_Eff_Rating_Mach_KVA   IN NUMBER,
    I_Eff_Rating_Inv_KVA    IN NUMBER,
    I_Backup_Gen            IN VARCHAR2,
    I_Max_Storage_Capacity  IN NUMBER,
    I_Charge_Demand_KW      IN NUMBER,
    I_Power_Source          IN VARCHAR2);
--Changes for ENOS to SAP migration - change detection End.

--    Change for project GIS powerbase integration - Start
-- PROCEDURE SP_SWITCH_MSO_DETECTION(
--    I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER);
--    Change for project GIS powerbase integration - End

END SM_CHANGE_DETECTION_PKG;

/


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
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER,
    --Changes for ENOS to SAP migration - change detection Start.
    I_Service_Point_GUID    IN VARCHAR2,
    I_SAP_EGI_Notification  IN VARCHAR2,
    I_Project_Name          IN VARCHAR2,
    I_Gen_Type              IN VARCHAR2,
    I_Program_Type          IN VARCHAR2,
    I_Eff_Rating_Mach_KW    IN NUMBER,
    I_Eff_Rating_Inv_KW     IN NUMBER,
    I_Eff_Rating_Mach_KVA   IN NUMBER,
    I_Eff_Rating_Inv_KVA    IN NUMBER,
    I_Backup_Gen            IN VARCHAR2,
    I_Max_Storage_Capacity  IN NUMBER,
    I_Charge_Demand_KW      IN NUMBER,
    I_Power_Source          IN VARCHAR2)
    --Changes for ENOS to SAP migration - change detection End..
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
  --    Change for project GIS powerbase integration - Start
--  WHEN DEVICE_TYPE = 'SM_SECTIONALIZER' THEN
--    BEGIN
--      SP_SECTIONALIZER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END;
--  WHEN DEVICE_TYPE = 'SM_RECLOSER' THEN
--    BEGIN
--        --Changes done by TCS on 26Oct 2016
--        IF(I_Control_type_code = 'TS') THEN
--          SP_RECLOSER_TS_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Switch_type_code,I_Bank_code );
--          err_num:=0;
--          INSERT INTO sm_errors VALUES
--            (I_Global_id_Current,err_num,NULL
--            );
--
--
--         --Changes done by TCS on July 2018
--        ELSE IF(I_Control_type_code = 'FS') THEN
--          SP_RECLOSER_FS_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Switch_type_code,I_Bank_code );
--          err_num:=0;
--          INSERT INTO sm_errors VALUES
--            (I_Global_id_Current,err_num,NULL
--            );
--
--
--        ELSE
--          SP_RECLOSER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--          err_num:=0;
--          INSERT INTO sm_errors VALUES
--            (I_Global_id_Current,err_num,NULL
--            );
--        END IF;
-- 	END IF;
--        END;
--  WHEN DEVICE_TYPE = 'SM_SWITCH' THEN
--    BEGIN
--	--Changes done by TCS on December 2019
--     IF(I_Control_type_code = 'MSO') THEN
--       SP_SWITCH_MSO_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--
--     ELSE
--      SP_SWITCH_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END IF;
--    END;
--  WHEN DEVICE_TYPE = 'SM_REGULATOR' THEN
--    BEGIN
--      SP_REGULATOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END;
--  WHEN DEVICE_TYPE = 'SM_INTERRUPTER' THEN
--    BEGIN
--      SP_INTERRUPTER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END;
--  WHEN DEVICE_TYPE = 'SM_CAPACITOR' THEN
--    BEGIN
--      SP_CAPACITOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END;
--  WHEN DEVICE_TYPE = 'SM_CIRCUIT_BREAKER' THEN
--    BEGIN
--      SP_CIRCUIT_BREAKER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--      err_num:=0;
--      INSERT INTO sm_errors VALUES
--        (I_Global_id_Current,err_num,NULL
--        );
--    END;
--    Change for project GIS powerbase integration - End
  WHEN DEVICE_TYPE = 'SM_NETWORK_PROTECTOR' THEN
    BEGIN
      SP_NETWORK_PROTECTOR_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
    END;
  WHEN DEVICE_TYPE = 'SM_CONDUCTOR' THEN
     BEGIN
      SP_SPECIAL_LOAD_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
      err_num:=0;
      INSERT INTO sm_errors VALUES
        (I_Global_id_Current,err_num,NULL
        );
     END;
--    Change for project GIS powerbase integration - Start
--  WHEN DEVICE_TYPE = 'SM_PRIMARY_METER' THEN
--     BEGIN
--          dbms_output.put_line(I_Global_id_Current);
----        SP_SPECIAL_LOAD_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--        SP_PRIMARY_METER_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
--        err_num:=0;
--        INSERT INTO sm_errors VALUES ( I_Global_id_Current,err_num,NULL );
--     END;
--      --Changes for ENOS to SAP migration - change detection Start..
-- --WHEN DEVICE_TYPE = 'SM_PRIMARY_GEN' THEN
--    Change for project GIS powerbase integration - End 
  WHEN DEVICE_TYPE = 'SM_GENERATION' THEN
     BEGIN
        --SP_PRIMARY_GEN_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code );
        SP_PRIMARY_GEN_DETECTION(I_Global_id_Current, I_reason_type,I_feature_class_name,I_feature_class_subtype,I_operating_num,I_Global_id_Previous,I_Division,I_District,I_Control_type_code,I_Switch_type_code,I_Bank_code,I_Service_Point_GUID,I_SAP_EGI_Notification,I_Project_Name,I_Gen_Type,I_Program_Type,I_Eff_Rating_Mach_KW,I_Eff_Rating_Inv_KW,I_Eff_Rating_Mach_KVA,I_Eff_Rating_Inv_KVA,I_Backup_Gen,I_Max_Storage_Capacity,I_Charge_Demand_KW,I_Power_Source);

        --Changes for ENOS to SAP migration - change detection End..
        err_num:=0;
        INSERT INTO sm_errors VALUES ( I_Global_id_Current,err_num,NULL );
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
    --- This code updates the BANKCODE VALUEs if any value is set to NULL
    IF DEVICE_TYPE = 'SM_REGULATOR' THEN
    BEGIN
      SP_BANKCODE_UPDATE;
    END;
   END IF;
END SP_SM_DEVICE_DETECTION;





PROCEDURE SP_BANKCODE_UPDATE
AS
CURSOR BANKCODE_LIST IS
  select OPERATING_NUM,global_id from edsett.gis_cedsadeviceid where bankcode is null and FEATURE_CLASS_NAME='Regulator' and GIS_FEATURE_CLASS_NAME='EDGIS.VoltageRegulatorUnit';
 sqlstr VARCHAR2(10000);
 sqlstr2 VARCHAR2(10000);
 row_cnt varchar(2000);
 bankcode number;
 TYPE bankCodes IS REF CURSOR;
 bankCodeCursor bankCodes;
BEGIN
  FOR regulator_info IN BANKCODE_LIST LOOP
   bankcode := 0;
   LOOP
             bankcode := bankcode+1;
             sqlstr := 'select bank_cd from edsett.sm_regulator where bank_cd='||bankcode||' ';
             dbms_output.put_line('sqlstr is :'||sqlstr);
             OPEN bankCodeCursor for sqlstr;
             FETCH bankCodeCursor INTO row_cnt;
             EXIT WHEN bankCodeCursor%NOTFOUND;
   END LOOP;
   sqlstr2 := 'update edsett.sm_regulator set bank_cd='||bankcode||' where GLOBAL_ID='''||regulator_info.global_id||''' ';
   dbms_output.put_line('sqlstmt2 is :'||sqlstr2);
   EXECUTE IMMEDIATE sqlstr2;
  END LOOP;
END SP_BANKCODE_UPDATE;



FUNCTION GET_GLOBALID
  RETURN VARCHAR2
IS
BEGIN
  RETURN GLOBALID;
END;

--    Change for project GIS powerbase integration - Start
--PROCEDURE SP_SECTIONALIZER_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN  NUMBER
--  )
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
---- Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
---- Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
---- Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--  -- Assign the ACTION Value to REASON_TYPE to process the txn
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_SECTIONALIZER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_SECTIONALIZER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SECTIONALIZER',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--     --  If the Valid num was assinged to 5 in earlier validations, then riase Update code
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--    END;
--  END IF;
--
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--           -- first copy the entire current record to history table
--      INSERT
--      INTO SM_SECTIONALIZER_HIST
--        (
--          PEER_REVIEW_DT,
--          EFFECTIVE_DT,
--          TIMESTAMP,
--          DATE_MODIFIED,
--          PREPARED_BY,
--          DEVICE_ID,
--          OPERATING_NUM,
--          GLOBAL_ID,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          ONE_SHOT_LOCKOUT_TIME,
--          ONE_SHOT_LOCKOUT_NUM,
--          LOCKOUT_NUM,
--          VOLT_THRESHOLD,
--          RESET,
--          GRD_INRUSH_TIME,
--          GRD_INRUSH_MULTIPLIER,
--          GRD_INRUSH_DURATION,
--          MIN_GRD_TO_CT,
--          REQUIRED_FAULT_CURRENT,
--          FIRST_RECLOSE_RESET_TIME,
--          PHA_INRUSH_TIME,
--          PHA_INRUSH_MULTIPLIER,
--          PHA_INRUSH_DURATION,
--          MIN_PC_TO_CT,
--          SECT_TYPE,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          RELEASED_BY,
--          PROCESSED_FLAG,
--          CONTROL_SERIAL_NUM,
--          CURRENT_FUTURE,
--          DISTRICT,
--          DIVISION,
--          PEER_REVIEW_BY,
--          FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--
--        )
--      SELECT PEER_REVIEW_DT,
--        EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        GLOBAL_ID,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        ONE_SHOT_LOCKOUT_TIME,
--        ONE_SHOT_LOCKOUT_NUM,
--        LOCKOUT_NUM,
--        VOLT_THRESHOLD,
--        RESET,
--        GRD_INRUSH_TIME,
--        GRD_INRUSH_MULTIPLIER,
--        GRD_INRUSH_DURATION,
--        MIN_GRD_TO_CT,
--        REQUIRED_FAULT_CURRENT,
--        FIRST_RECLOSE_RESET_TIME,
--        PHA_INRUSH_TIME,
--        PHA_INRUSH_MULTIPLIER,
--        PHA_INRUSH_DURATION,
--        MIN_PC_TO_CT,
--        SECT_TYPE,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        CURRENT_FUTURE,
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SECTIONALIZER_HIST',
--          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_SECTIONALIZER
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District,
--        CONTROL_TYPE    =I_Control_type_code
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--
--  IF ACTION = 'D' THEN
--    BEGIN
--
--      INSERT
--      INTO SM_SECTIONALIZER_HIST
--        (
--          PEER_REVIEW_DT,
--          EFFECTIVE_DT,
--          TIMESTAMP,
--          DATE_MODIFIED,
--          PREPARED_BY,
--          DEVICE_ID,
--          OPERATING_NUM,
--          GLOBAL_ID,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          ONE_SHOT_LOCKOUT_TIME,
--          ONE_SHOT_LOCKOUT_NUM,
--          LOCKOUT_NUM,
--          VOLT_THRESHOLD,
--          RESET,
--          GRD_INRUSH_TIME,
--          GRD_INRUSH_MULTIPLIER,
--          GRD_INRUSH_DURATION,
--          MIN_GRD_TO_CT,
--          REQUIRED_FAULT_CURRENT,
--          FIRST_RECLOSE_RESET_TIME,
--          PHA_INRUSH_TIME,
--          PHA_INRUSH_MULTIPLIER,
--          PHA_INRUSH_DURATION,
--          MIN_PC_TO_CT,
--          SECT_TYPE,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          RELEASED_BY,
--          PROCESSED_FLAG,
--          CONTROL_SERIAL_NUM,
--          CURRENT_FUTURE,
--
--          DISTRICT,
--          DIVISION,
--          PEER_REVIEW_BY,
--          FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--        )
--      SELECT PEER_REVIEW_DT,
--        EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        GLOBAL_ID,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        ONE_SHOT_LOCKOUT_TIME,
--        ONE_SHOT_LOCKOUT_NUM,
--        LOCKOUT_NUM,
--        VOLT_THRESHOLD,
--        RESET,
--        GRD_INRUSH_TIME,
--        GRD_INRUSH_MULTIPLIER,
--        GRD_INRUSH_DURATION,
--        MIN_GRD_TO_CT,
--        REQUIRED_FAULT_CURRENT,
--        FIRST_RECLOSE_RESET_TIME,
--        PHA_INRUSH_TIME,
--        PHA_INRUSH_MULTIPLIER,
--        PHA_INRUSH_DURATION,
--        MIN_PC_TO_CT,
--        SECT_TYPE,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        CURRENT_FUTURE,
--
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      DELETE FROM SM_SECTIONALIZER WHERE GLOBAL_ID = I_Global_id_Current;
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SECTIONALIZER_HIST',
--          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--    END;
--  END IF;
--
--  IF ACTION = 'R' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_SECTIONALIZER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_SECTIONALIZER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SECTIONALIZER',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' --' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_SECTIONALIZER_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_SECTIONALIZER
--      SET
--        (
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          ONE_SHOT_LOCKOUT_TIME,
--          ONE_SHOT_LOCKOUT_NUM,
--          LOCKOUT_NUM,
--          VOLT_THRESHOLD,
--          RESET,
--          GRD_INRUSH_TIME,
--          GRD_INRUSH_MULTIPLIER,
--          GRD_INRUSH_DURATION,
--          MIN_GRD_TO_CT,
--          REQUIRED_FAULT_CURRENT,
--          FIRST_RECLOSE_RESET_TIME,
--          PHA_INRUSH_TIME,
--          PHA_INRUSH_MULTIPLIER,
--          PHA_INRUSH_DURATION,
--          MIN_PC_TO_CT,
--          SECT_TYPE,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        )
--        =
--        (SELECT RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          ONE_SHOT_LOCKOUT_TIME,
--          ONE_SHOT_LOCKOUT_NUM,
--          LOCKOUT_NUM,
--          VOLT_THRESHOLD,
--          RESET,
--          GRD_INRUSH_TIME,
--          GRD_INRUSH_MULTIPLIER,
--          GRD_INRUSH_DURATION,
--          MIN_GRD_TO_CT,
--          REQUIRED_FAULT_CURRENT,
--          FIRST_RECLOSE_RESET_TIME,
--          PHA_INRUSH_TIME,
--          PHA_INRUSH_MULTIPLIER,
--          PHA_INRUSH_DURATION,
--          MIN_PC_TO_CT,
--          SECT_TYPE,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        FROM SM_SECTIONALIZER
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT
--      INTO SM_SECTIONALIZER_HIST
--        (
--          PEER_REVIEW_DT,
--          EFFECTIVE_DT,
--          TIMESTAMP,
--          DATE_MODIFIED,
--          PREPARED_BY,
--          DEVICE_ID,
--          OPERATING_NUM,
--          GLOBAL_ID,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          ONE_SHOT_LOCKOUT_TIME,
--          ONE_SHOT_LOCKOUT_NUM,
--          LOCKOUT_NUM,
--          VOLT_THRESHOLD,
--          RESET,
--          GRD_INRUSH_TIME,
--          GRD_INRUSH_MULTIPLIER,
--          GRD_INRUSH_DURATION,
--          MIN_GRD_TO_CT,
--          REQUIRED_FAULT_CURRENT,
--          FIRST_RECLOSE_RESET_TIME,
--          PHA_INRUSH_TIME,
--          PHA_INRUSH_MULTIPLIER,
--          PHA_INRUSH_DURATION,
--          MIN_PC_TO_CT,
--          SECT_TYPE,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          RELEASED_BY,
--          PROCESSED_FLAG,
--          CONTROL_SERIAL_NUM,
--          CURRENT_FUTURE,
--
--          DISTRICT,
--          DIVISION,
--          PEER_REVIEW_BY,
--          FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--        )
--      SELECT PEER_REVIEW_DT,
--        EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        GLOBAL_ID,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        ONE_SHOT_LOCKOUT_TIME,
--        ONE_SHOT_LOCKOUT_NUM,
--        LOCKOUT_NUM,
--        VOLT_THRESHOLD,
--        RESET,
--        GRD_INRUSH_TIME,
--        GRD_INRUSH_MULTIPLIER,
--        GRD_INRUSH_DURATION,
--        MIN_GRD_TO_CT,
--        REQUIRED_FAULT_CURRENT,
--        FIRST_RECLOSE_RESET_TIME,
--        PHA_INRUSH_TIME,
--        PHA_INRUSH_MULTIPLIER,
--        PHA_INRUSH_DURATION,
--        MIN_PC_TO_CT,
--        SECT_TYPE,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        CURRENT_FUTURE,
--
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        FEATURE_CLASS_NAME,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,BYPASS_PLANS,OPERATING_MODE,FLISR
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID = I_Global_id_Previous;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SECTIONALIZER_HIST',
--          SM_SECTIONALIZER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      -- Remove previous global id from te device table
--      DELETE
--      FROM SM_SECTIONALIZER
--      WHERE GLOBAL_ID = I_Global_id_Previous;
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--END SP_SECTIONALIZER_DETECTION;
--
--PROCEDURE SP_CAPACITOR_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER)
--AS
--   REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
--  --- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
---- Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_CAPACITOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_CAPACITOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CAPACITOR',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--        IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--        END IF;
--
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    BEGIN
--      -- first copy the entire current record to history table
--      INSERT
--      INTO SM_CAPACITOR_HIST
--        (
--          AUTO_BVR_CALC,
--          BAUD_RATE,
--          CONTROLLER_UNIT_MODEL,
--          CONTROL_SERIAL_NUM,
--          CONTROL_TYPE,
--          CURRENT_FUTURE,
--          DATA_LOGGING_INTERVAL,
--          DATE_MODIFIED,
--          DAYLIGHT_SAVINGS_TIME,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          EFFECTIVE_DT,
--          EST_BANK_VOLTAGE_RISE,
--          EST_VOLTAGE_CHANGE,
--          FEATURE_CLASS_NAME,
--          FIRMWARE_VERSION,
--          GLOBAL_ID,
--          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--          LOW_VOLTAGE_OVERRIDE_SETPOINT,
--          MASTER_STATION,
--          MAXCYCLES,
--          MIN_SW_VOLTAGE,
--          OPERATING_NUM,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PREPARED_BY,
--          PROCESSED_FLAG,
--          PULSE_TIME,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SCH1_CONTROL_STRATEGY,
--          SCH1_END_DATE,
--          SCH1_HIGH_VOLTAGE_SETPOINT,
--          SCH1_HOLIDAYS,
--          SCH1_KVAR_SETPOINT_OFF,
--          SCH1_KVAR_SETPOINT_ON,
--          SCH1_LOW_VOLTAGE_SETPOINT,
--          SCH1_SATURDAY,
--          SCH1_SCHEDULE,
--          SCH1_START_DATE,
--          SCH1_SUNDAY,
--          SCH1_TEMP_SETPOINT_OFF,
--          SCH1_TEMP_SETPOINT_ON,
--          SCH1_TIME_OFF,
--          SCH1_TIME_ON,
--          SCH1_WEEKDAYS,
--          SCH2_CONTROL_STRATEGY,
--          SCH2_END_DATE,
--          SCH2_HIGH_VOLTAGE_SETPOINT,
--          SCH2_HOLIDAYS,
--          SCH2_KVAR_SETPOINT_OFF,
--          SCH2_KVAR_SETPOINT_ON,
--          SCH2_LOW_VOLTAGE_SETPOINT,
--          SCH2_SATURDAY,
--          SCH2_SCHEDULE,
--          SCH2_START_DATE,
--          SCH2_SUNDAY,
--          SCH2_TEMP_SETPOINT_OFF,
--          SCH2_TEMP_SETPOINT_ON,
--          SCH2_TIME_OFF,
--          SCH2_TIME_ON,
--          SCH2_WEEKDAYS,
--          SCH3_CONTROL_STRATEGY,
--          SCH3_END_DATE,
--          SCH3_HIGH_VOLTAGE_SETPOINT,
--          SCH3_HOLIDAYS,
--          SCH3_KVAR_SETPOINT_OFF,
--          SCH3_KVAR_SETPOINT_ON,
--          SCH3_LOW_VOLTAGE_SETPOINT,
--          SCH3_SATURDAY,
--          SCH3_SCHEDULE,
--          SCH3_START_DATE,
--          SCH3_SUNDAY,
--          SCH3_TEMP_SETPOINT_OFF,
--          SCH3_TEMP_SETPOINT_ON,
--          SCH3_TIME_OFF,
--          SCH3_TIME_ON,
--          SCH3_WEEKDAYS,
--          SCH4_CONTROL_STRATEGY,
--          SCH4_END_DATE,
--          SCH4_HIGH_VOLTAGE_SETPOINT,
--          SCH4_HOLIDAYS,
--          SCH4_KVAR_SETPOINT_OFF,
--          SCH4_KVAR_SETPOINT_ON,
--          SCH4_LOW_VOLTAGE_SETPOINT,
--          SCH4_SATURDAY,
--          SCH4_SCHEDULE,
--          SCH4_START_DATE,
--          SCH4_SUNDAY,
--          SCH4_TEMP_SETPOINT_OFF,
--          SCH4_TEMP_SETPOINT_ON,
--          SCH4_TIME_OFF,
--          SCH4_TIME_ON,
--          SCH4_WEEKDAYS,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          SWITCH_POSITION,
--          TEMPERATURE_CHANGE_TIME,
--          TEMPERATURE_OVERRIDE,
--          TIMESTAMP,
--          TIME_DELAY,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          VOLTAGE_CHANGE_TIME,
--          VOLTAGE_OVERRIDE_TIME,
--          VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--          SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--        )
--      SELECT AUTO_BVR_CALC,
--        BAUD_RATE,
--        CONTROLLER_UNIT_MODEL,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CURRENT_FUTURE,
--        DATA_LOGGING_INTERVAL,
--        DATE_MODIFIED,
--        DAYLIGHT_SAVINGS_TIME,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--        EST_BANK_VOLTAGE_RISE,
--        EST_VOLTAGE_CHANGE,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--        LOW_VOLTAGE_OVERRIDE_SETPOINT,
--        MASTER_STATION,
--        MAXCYCLES,
--        MIN_SW_VOLTAGE,
--        OPERATING_NUM,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        PULSE_TIME,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SCH1_CONTROL_STRATEGY,
--        SCH1_END_DATE,
--        SCH1_HIGH_VOLTAGE_SETPOINT,
--        SCH1_HOLIDAYS,
--        SCH1_KVAR_SETPOINT_OFF,
--        SCH1_KVAR_SETPOINT_ON,
--        SCH1_LOW_VOLTAGE_SETPOINT,
--        SCH1_SATURDAY,
--        SCH1_SCHEDULE,
--        SCH1_START_DATE,
--        SCH1_SUNDAY,
--        SCH1_TEMP_SETPOINT_OFF,
--        SCH1_TEMP_SETPOINT_ON,
--        SCH1_TIME_OFF,
--        SCH1_TIME_ON,
--        SCH1_WEEKDAYS,
--        SCH2_CONTROL_STRATEGY,
--        SCH2_END_DATE,
--        SCH2_HIGH_VOLTAGE_SETPOINT,
--        SCH2_HOLIDAYS,
--        SCH2_KVAR_SETPOINT_OFF,
--        SCH2_KVAR_SETPOINT_ON,
--        SCH2_LOW_VOLTAGE_SETPOINT,
--        SCH2_SATURDAY,
--        SCH2_SCHEDULE,
--        SCH2_START_DATE,
--        SCH2_SUNDAY,
--        SCH2_TEMP_SETPOINT_OFF,
--        SCH2_TEMP_SETPOINT_ON,
--        SCH2_TIME_OFF,
--        SCH2_TIME_ON,
--        SCH2_WEEKDAYS,
--        SCH3_CONTROL_STRATEGY,
--        SCH3_END_DATE,
--        SCH3_HIGH_VOLTAGE_SETPOINT,
--        SCH3_HOLIDAYS,
--        SCH3_KVAR_SETPOINT_OFF,
--        SCH3_KVAR_SETPOINT_ON,
--        SCH3_LOW_VOLTAGE_SETPOINT,
--        SCH3_SATURDAY,
--        SCH3_SCHEDULE,
--        SCH3_START_DATE,
--        SCH3_SUNDAY,
--        SCH3_TEMP_SETPOINT_OFF,
--        SCH3_TEMP_SETPOINT_ON,
--        SCH3_TIME_OFF,
--        SCH3_TIME_ON,
--        SCH3_WEEKDAYS,
--        SCH4_CONTROL_STRATEGY,
--        SCH4_END_DATE,
--        SCH4_HIGH_VOLTAGE_SETPOINT,
--        SCH4_HOLIDAYS,
--        SCH4_KVAR_SETPOINT_OFF,
--        SCH4_KVAR_SETPOINT_ON,
--        SCH4_LOW_VOLTAGE_SETPOINT,
--        SCH4_SATURDAY,
--        SCH4_SCHEDULE,
--        SCH4_START_DATE,
--        SCH4_SUNDAY,
--        SCH4_TEMP_SETPOINT_OFF,
--        SCH4_TEMP_SETPOINT_ON,
--        SCH4_TIME_OFF,
--        SCH4_TIME_ON,
--        SCH4_WEEKDAYS,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        SWITCH_POSITION,
--        TEMPERATURE_CHANGE_TIME,
--        TEMPERATURE_OVERRIDE,
--        TIMESTAMP,
--        TIME_DELAY,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        VOLTAGE_CHANGE_TIME,
--        VOLTAGE_OVERRIDE_TIME,
--        VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--        SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID= I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CAPACITOR_HIST',
--          SM_CAPACITOR_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_CAPACITOR
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District,
--        CONTROL_TYPE    =I_Control_type_code
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--  IF ACTION = 'D' THEN
--    -- first copy the entire current record to history table
--    INSERT
--    INTO SM_CAPACITOR_HIST
--      (
--        AUTO_BVR_CALC,
--        BAUD_RATE,
--        CONTROLLER_UNIT_MODEL,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CURRENT_FUTURE,
--        DATA_LOGGING_INTERVAL,
--        DATE_MODIFIED,
--        DAYLIGHT_SAVINGS_TIME,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--
--        EST_BANK_VOLTAGE_RISE,
--        EST_VOLTAGE_CHANGE,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--        LOW_VOLTAGE_OVERRIDE_SETPOINT,
--        MASTER_STATION,
--        MAXCYCLES,
--        MIN_SW_VOLTAGE,
--
--        OPERATING_NUM,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        PULSE_TIME,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SCH1_CONTROL_STRATEGY,
--        SCH1_END_DATE,
--        SCH1_HIGH_VOLTAGE_SETPOINT,
--        SCH1_HOLIDAYS,
--        SCH1_KVAR_SETPOINT_OFF,
--        SCH1_KVAR_SETPOINT_ON,
--        SCH1_LOW_VOLTAGE_SETPOINT,
--        SCH1_SATURDAY,
--        SCH1_SCHEDULE,
--        SCH1_START_DATE,
--        SCH1_SUNDAY,
--        SCH1_TEMP_SETPOINT_OFF,
--        SCH1_TEMP_SETPOINT_ON,
--        SCH1_TIME_OFF,
--        SCH1_TIME_ON,
--        SCH1_WEEKDAYS,
--        SCH2_CONTROL_STRATEGY,
--        SCH2_END_DATE,
--        SCH2_HIGH_VOLTAGE_SETPOINT,
--        SCH2_HOLIDAYS,
--        SCH2_KVAR_SETPOINT_OFF,
--        SCH2_KVAR_SETPOINT_ON,
--        SCH2_LOW_VOLTAGE_SETPOINT,
--        SCH2_SATURDAY,
--        SCH2_SCHEDULE,
--        SCH2_START_DATE,
--        SCH2_SUNDAY,
--        SCH2_TEMP_SETPOINT_OFF,
--        SCH2_TEMP_SETPOINT_ON,
--        SCH2_TIME_OFF,
--        SCH2_TIME_ON,
--        SCH2_WEEKDAYS,
--        SCH3_CONTROL_STRATEGY,
--        SCH3_END_DATE,
--        SCH3_HIGH_VOLTAGE_SETPOINT,
--        SCH3_HOLIDAYS,
--        SCH3_KVAR_SETPOINT_OFF,
--        SCH3_KVAR_SETPOINT_ON,
--        SCH3_LOW_VOLTAGE_SETPOINT,
--        SCH3_SATURDAY,
--        SCH3_SCHEDULE,
--        SCH3_START_DATE,
--        SCH3_SUNDAY,
--        SCH3_TEMP_SETPOINT_OFF,
--        SCH3_TEMP_SETPOINT_ON,
--        SCH3_TIME_OFF,
--        SCH3_TIME_ON,
--        SCH3_WEEKDAYS,
--        SCH4_CONTROL_STRATEGY,
--        SCH4_END_DATE,
--        SCH4_HIGH_VOLTAGE_SETPOINT,
--        SCH4_HOLIDAYS,
--        SCH4_KVAR_SETPOINT_OFF,
--        SCH4_KVAR_SETPOINT_ON,
--        SCH4_LOW_VOLTAGE_SETPOINT,
--        SCH4_SATURDAY,
--        SCH4_SCHEDULE,
--        SCH4_START_DATE,
--        SCH4_SUNDAY,
--        SCH4_TEMP_SETPOINT_OFF,
--        SCH4_TEMP_SETPOINT_ON,
--        SCH4_TIME_OFF,
--        SCH4_TIME_ON,
--        SCH4_WEEKDAYS,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        SWITCH_POSITION,
--        TEMPERATURE_CHANGE_TIME,
--        TEMPERATURE_OVERRIDE,
--        TIMESTAMP,
--        TIME_DELAY,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        VOLTAGE_CHANGE_TIME,
--        VOLTAGE_OVERRIDE_TIME,
--        VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--        SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--      )
--    SELECT AUTO_BVR_CALC,
--      BAUD_RATE,
--      CONTROLLER_UNIT_MODEL,
--      CONTROL_SERIAL_NUM,
--      CONTROL_TYPE,
--      CURRENT_FUTURE,
--      DATA_LOGGING_INTERVAL,
--      DATE_MODIFIED,
--      DAYLIGHT_SAVINGS_TIME,
--      DEVICE_ID,
--      DISTRICT,
--      DIVISION,
--      EFFECTIVE_DT,
--
--      EST_BANK_VOLTAGE_RISE,
--      EST_VOLTAGE_CHANGE,
--      FEATURE_CLASS_NAME,
--      FIRMWARE_VERSION,
--      GLOBAL_ID,
--      HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--      LOW_VOLTAGE_OVERRIDE_SETPOINT,
--      MASTER_STATION,
--      MAXCYCLES,
--      MIN_SW_VOLTAGE,
--
--      OPERATING_NUM,
--      PEER_REVIEW_BY,
--      PEER_REVIEW_DT,
--      PREPARED_BY,
--      PROCESSED_FLAG,
--      PULSE_TIME,
--      RADIO_MANF_CD,
--      RADIO_MODEL_NUM,
--      RADIO_SERIAL_NUM,
--      RELAY_TYPE,
--      RELEASED_BY,
--      REPEATER,
--      RTU_ADDRESS,
--      SCADA,
--      SCADA_TYPE,
--      SCH1_CONTROL_STRATEGY,
--      SCH1_END_DATE,
--      SCH1_HIGH_VOLTAGE_SETPOINT,
--      SCH1_HOLIDAYS,
--      SCH1_KVAR_SETPOINT_OFF,
--      SCH1_KVAR_SETPOINT_ON,
--      SCH1_LOW_VOLTAGE_SETPOINT,
--      SCH1_SATURDAY,
--      SCH1_SCHEDULE,
--      SCH1_START_DATE,
--      SCH1_SUNDAY,
--      SCH1_TEMP_SETPOINT_OFF,
--      SCH1_TEMP_SETPOINT_ON,
--      SCH1_TIME_OFF,
--      SCH1_TIME_ON,
--      SCH1_WEEKDAYS,
--      SCH2_CONTROL_STRATEGY,
--      SCH2_END_DATE,
--      SCH2_HIGH_VOLTAGE_SETPOINT,
--      SCH2_HOLIDAYS,
--      SCH2_KVAR_SETPOINT_OFF,
--      SCH2_KVAR_SETPOINT_ON,
--      SCH2_LOW_VOLTAGE_SETPOINT,
--      SCH2_SATURDAY,
--      SCH2_SCHEDULE,
--      SCH2_START_DATE,
--      SCH2_SUNDAY,
--      SCH2_TEMP_SETPOINT_OFF,
--      SCH2_TEMP_SETPOINT_ON,
--      SCH2_TIME_OFF,
--      SCH2_TIME_ON,
--      SCH2_WEEKDAYS,
--      SCH3_CONTROL_STRATEGY,
--      SCH3_END_DATE,
--      SCH3_HIGH_VOLTAGE_SETPOINT,
--      SCH3_HOLIDAYS,
--      SCH3_KVAR_SETPOINT_OFF,
--      SCH3_KVAR_SETPOINT_ON,
--      SCH3_LOW_VOLTAGE_SETPOINT,
--      SCH3_SATURDAY,
--      SCH3_SCHEDULE,
--      SCH3_START_DATE,
--      SCH3_SUNDAY,
--      SCH3_TEMP_SETPOINT_OFF,
--      SCH3_TEMP_SETPOINT_ON,
--      SCH3_TIME_OFF,
--      SCH3_TIME_ON,
--      SCH3_WEEKDAYS,
--      SCH4_CONTROL_STRATEGY,
--      SCH4_END_DATE,
--      SCH4_HIGH_VOLTAGE_SETPOINT,
--      SCH4_HOLIDAYS,
--      SCH4_KVAR_SETPOINT_OFF,
--      SCH4_KVAR_SETPOINT_ON,
--      SCH4_LOW_VOLTAGE_SETPOINT,
--      SCH4_SATURDAY,
--      SCH4_SCHEDULE,
--      SCH4_START_DATE,
--      SCH4_SUNDAY,
--      SCH4_TEMP_SETPOINT_OFF,
--      SCH4_TEMP_SETPOINT_ON,
--      SCH4_TIME_OFF,
--      SCH4_TIME_ON,
--      SCH4_WEEKDAYS,
--      SOFTWARE_VERSION,
--      SPECIAL_CONDITIONS,
--      SWITCH_POSITION,
--      TEMPERATURE_CHANGE_TIME,
--      TEMPERATURE_OVERRIDE,
--      TIMESTAMP,
--      TIME_DELAY,
--      TRANSMIT_DISABLE_DELAY,
--      TRANSMIT_ENABLE_DELAY,
--      VOLTAGE_CHANGE_TIME,
--      VOLTAGE_OVERRIDE_TIME,
--      VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--      RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--      SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--    FROM SM_CAPACITOR
--    WHERE GLOBAL_ID= I_Global_id_Current;
--    DELETE FROM SM_CAPACITOR WHERE GLOBAL_ID = I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_CAPACITOR_HIST',
--        SM_CAPACITOR_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--       -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_CAPACITOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_CAPACITOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CAPACITOR',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_CAPACITOR_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_CAPACITOR
--      SET
--        (
--          AUTO_BVR_CALC,
--          BAUD_RATE,
--          CONTROLLER_UNIT_MODEL,
--          CONTROL_TYPE,
--          DATA_LOGGING_INTERVAL,
--          DAYLIGHT_SAVINGS_TIME,
--
--          EST_BANK_VOLTAGE_RISE,
--          EST_VOLTAGE_CHANGE,
--          FIRMWARE_VERSION,
--          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--          LOW_VOLTAGE_OVERRIDE_SETPOINT,
--          MASTER_STATION,
--          MAXCYCLES,
--          MIN_SW_VOLTAGE,
--          PULSE_TIME,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SCH1_CONTROL_STRATEGY,
--          SCH1_END_DATE,
--          SCH1_HIGH_VOLTAGE_SETPOINT,
--          SCH1_HOLIDAYS,
--          SCH1_KVAR_SETPOINT_OFF,
--          SCH1_KVAR_SETPOINT_ON,
--          SCH1_LOW_VOLTAGE_SETPOINT,
--          SCH1_SATURDAY,
--          SCH1_SCHEDULE,
--          SCH1_START_DATE,
--          SCH1_SUNDAY,
--          SCH1_TEMP_SETPOINT_OFF,
--          SCH1_TEMP_SETPOINT_ON,
--          SCH1_TIME_OFF,
--          SCH1_TIME_ON,
--          SCH1_WEEKDAYS,
--          SCH2_CONTROL_STRATEGY,
--          SCH2_END_DATE,
--          SCH2_HIGH_VOLTAGE_SETPOINT,
--          SCH2_HOLIDAYS,
--          SCH2_KVAR_SETPOINT_OFF,
--          SCH2_KVAR_SETPOINT_ON,
--          SCH2_LOW_VOLTAGE_SETPOINT,
--          SCH2_SATURDAY,
--          SCH2_SCHEDULE,
--          SCH2_START_DATE,
--          SCH2_SUNDAY,
--          SCH2_TEMP_SETPOINT_OFF,
--          SCH2_TEMP_SETPOINT_ON,
--          SCH2_TIME_OFF,
--          SCH2_TIME_ON,
--          SCH2_WEEKDAYS,
--          SCH3_CONTROL_STRATEGY,
--          SCH3_END_DATE,
--          SCH3_HIGH_VOLTAGE_SETPOINT,
--          SCH3_HOLIDAYS,
--          SCH3_KVAR_SETPOINT_OFF,
--          SCH3_KVAR_SETPOINT_ON,
--          SCH3_LOW_VOLTAGE_SETPOINT,
--          SCH3_SATURDAY,
--          SCH3_SCHEDULE,
--          SCH3_START_DATE,
--          SCH3_SUNDAY,
--          SCH3_TEMP_SETPOINT_OFF,
--          SCH3_TEMP_SETPOINT_ON,
--          SCH3_TIME_OFF,
--          SCH3_TIME_ON,
--          SCH3_WEEKDAYS,
--          SCH4_CONTROL_STRATEGY,
--          SCH4_END_DATE,
--          SCH4_HIGH_VOLTAGE_SETPOINT,
--          SCH4_HOLIDAYS,
--          SCH4_KVAR_SETPOINT_OFF,
--          SCH4_KVAR_SETPOINT_ON,
--          SCH4_LOW_VOLTAGE_SETPOINT,
--          SCH4_SATURDAY,
--          SCH4_SCHEDULE,
--          SCH4_START_DATE,
--          SCH4_SUNDAY,
--          SCH4_TEMP_SETPOINT_OFF,
--          SCH4_TEMP_SETPOINT_ON,
--          SCH4_TIME_OFF,
--          SCH4_TIME_ON,
--          SCH4_WEEKDAYS,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          SWITCH_POSITION,
--          TEMPERATURE_CHANGE_TIME,
--          TEMPERATURE_OVERRIDE,
--          TIME_DELAY,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          VOLTAGE_CHANGE_TIME,
--          VOLTAGE_OVERRIDE_TIME,
--          VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION,
--          RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--          SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        )
--        =
--        (SELECT AUTO_BVR_CALC,
--          BAUD_RATE,
--          CONTROLLER_UNIT_MODEL,
--          CONTROL_TYPE,
--          DATA_LOGGING_INTERVAL,
--          DAYLIGHT_SAVINGS_TIME,
--          EST_BANK_VOLTAGE_RISE,
--          EST_VOLTAGE_CHANGE,
--          FIRMWARE_VERSION,
--          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--          LOW_VOLTAGE_OVERRIDE_SETPOINT,
--          MASTER_STATION,
--          MAXCYCLES,
--          MIN_SW_VOLTAGE,
--          PULSE_TIME,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SCH1_CONTROL_STRATEGY,
--          SCH1_END_DATE,
--          SCH1_HIGH_VOLTAGE_SETPOINT,
--          SCH1_HOLIDAYS,
--          SCH1_KVAR_SETPOINT_OFF,
--          SCH1_KVAR_SETPOINT_ON,
--          SCH1_LOW_VOLTAGE_SETPOINT,
--          SCH1_SATURDAY,
--          SCH1_SCHEDULE,
--          SCH1_START_DATE,
--          SCH1_SUNDAY,
--          SCH1_TEMP_SETPOINT_OFF,
--          SCH1_TEMP_SETPOINT_ON,
--          SCH1_TIME_OFF,
--          SCH1_TIME_ON,
--          SCH1_WEEKDAYS,
--          SCH2_CONTROL_STRATEGY,
--          SCH2_END_DATE,
--          SCH2_HIGH_VOLTAGE_SETPOINT,
--          SCH2_HOLIDAYS,
--          SCH2_KVAR_SETPOINT_OFF,
--          SCH2_KVAR_SETPOINT_ON,
--          SCH2_LOW_VOLTAGE_SETPOINT,
--          SCH2_SATURDAY,
--          SCH2_SCHEDULE,
--          SCH2_START_DATE,
--          SCH2_SUNDAY,
--          SCH2_TEMP_SETPOINT_OFF,
--          SCH2_TEMP_SETPOINT_ON,
--          SCH2_TIME_OFF,
--          SCH2_TIME_ON,
--          SCH2_WEEKDAYS,
--          SCH3_CONTROL_STRATEGY,
--          SCH3_END_DATE,
--          SCH3_HIGH_VOLTAGE_SETPOINT,
--          SCH3_HOLIDAYS,
--          SCH3_KVAR_SETPOINT_OFF,
--          SCH3_KVAR_SETPOINT_ON,
--          SCH3_LOW_VOLTAGE_SETPOINT,
--          SCH3_SATURDAY,
--          SCH3_SCHEDULE,
--          SCH3_START_DATE,
--          SCH3_SUNDAY,
--          SCH3_TEMP_SETPOINT_OFF,
--          SCH3_TEMP_SETPOINT_ON,
--          SCH3_TIME_OFF,
--          SCH3_TIME_ON,
--          SCH3_WEEKDAYS,
--          SCH4_CONTROL_STRATEGY,
--          SCH4_END_DATE,
--          SCH4_HIGH_VOLTAGE_SETPOINT,
--          SCH4_HOLIDAYS,
--          SCH4_KVAR_SETPOINT_OFF,
--          SCH4_KVAR_SETPOINT_ON,
--          SCH4_LOW_VOLTAGE_SETPOINT,
--          SCH4_SATURDAY,
--          SCH4_SCHEDULE,
--          SCH4_START_DATE,
--          SCH4_SUNDAY,
--          SCH4_TEMP_SETPOINT_OFF,
--          SCH4_TEMP_SETPOINT_ON,
--          SCH4_TIME_OFF,
--          SCH4_TIME_ON,
--          SCH4_WEEKDAYS,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          SWITCH_POSITION,
--          TEMPERATURE_CHANGE_TIME,
--          TEMPERATURE_OVERRIDE,
--          TIME_DELAY,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          VOLTAGE_CHANGE_TIME,
--          VOLTAGE_OVERRIDE_TIME,
--          VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--          SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        FROM SM_CAPACITOR
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT
--      INTO SM_CAPACITOR_HIST
--        (
--          AUTO_BVR_CALC,
--          BAUD_RATE,
--          CONTROLLER_UNIT_MODEL,
--          CONTROL_SERIAL_NUM,
--          CONTROL_TYPE,
--          CURRENT_FUTURE,
--          DATA_LOGGING_INTERVAL,
--          DATE_MODIFIED,
--          DAYLIGHT_SAVINGS_TIME,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          EFFECTIVE_DT,
--          EST_BANK_VOLTAGE_RISE,
--          EST_VOLTAGE_CHANGE,
--          FEATURE_CLASS_NAME,
--          FIRMWARE_VERSION,
--          GLOBAL_ID,
--          HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--          LOW_VOLTAGE_OVERRIDE_SETPOINT,
--          MASTER_STATION,
--          MAXCYCLES,
--          MIN_SW_VOLTAGE,
--          OPERATING_NUM,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PREPARED_BY,
--          PROCESSED_FLAG,
--          PULSE_TIME,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SCH1_CONTROL_STRATEGY,
--          SCH1_END_DATE,
--          SCH1_HIGH_VOLTAGE_SETPOINT,
--          SCH1_HOLIDAYS,
--          SCH1_KVAR_SETPOINT_OFF,
--          SCH1_KVAR_SETPOINT_ON,
--          SCH1_LOW_VOLTAGE_SETPOINT,
--          SCH1_SATURDAY,
--          SCH1_SCHEDULE,
--          SCH1_START_DATE,
--          SCH1_SUNDAY,
--          SCH1_TEMP_SETPOINT_OFF,
--          SCH1_TEMP_SETPOINT_ON,
--          SCH1_TIME_OFF,
--          SCH1_TIME_ON,
--          SCH1_WEEKDAYS,
--          SCH2_CONTROL_STRATEGY,
--          SCH2_END_DATE,
--          SCH2_HIGH_VOLTAGE_SETPOINT,
--          SCH2_HOLIDAYS,
--          SCH2_KVAR_SETPOINT_OFF,
--          SCH2_KVAR_SETPOINT_ON,
--          SCH2_LOW_VOLTAGE_SETPOINT,
--          SCH2_SATURDAY,
--          SCH2_SCHEDULE,
--          SCH2_START_DATE,
--          SCH2_SUNDAY,
--          SCH2_TEMP_SETPOINT_OFF,
--          SCH2_TEMP_SETPOINT_ON,
--          SCH2_TIME_OFF,
--          SCH2_TIME_ON,
--          SCH2_WEEKDAYS,
--          SCH3_CONTROL_STRATEGY,
--          SCH3_END_DATE,
--          SCH3_HIGH_VOLTAGE_SETPOINT,
--          SCH3_HOLIDAYS,
--          SCH3_KVAR_SETPOINT_OFF,
--          SCH3_KVAR_SETPOINT_ON,
--          SCH3_LOW_VOLTAGE_SETPOINT,
--          SCH3_SATURDAY,
--          SCH3_SCHEDULE,
--          SCH3_START_DATE,
--          SCH3_SUNDAY,
--          SCH3_TEMP_SETPOINT_OFF,
--          SCH3_TEMP_SETPOINT_ON,
--          SCH3_TIME_OFF,
--          SCH3_TIME_ON,
--          SCH3_WEEKDAYS,
--          SCH4_CONTROL_STRATEGY,
--          SCH4_END_DATE,
--          SCH4_HIGH_VOLTAGE_SETPOINT,
--          SCH4_HOLIDAYS,
--          SCH4_KVAR_SETPOINT_OFF,
--          SCH4_KVAR_SETPOINT_ON,
--          SCH4_LOW_VOLTAGE_SETPOINT,
--          SCH4_SATURDAY,
--          SCH4_SCHEDULE,
--          SCH4_START_DATE,
--          SCH4_SUNDAY,
--          SCH4_TEMP_SETPOINT_OFF,
--          SCH4_TEMP_SETPOINT_ON,
--          SCH4_TIME_OFF,
--          SCH4_TIME_ON,
--          SCH4_WEEKDAYS,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          SWITCH_POSITION,
--          TEMPERATURE_CHANGE_TIME,
--          TEMPERATURE_OVERRIDE,
--          TIMESTAMP,
--          TIME_DELAY,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          VOLTAGE_CHANGE_TIME,
--          VOLTAGE_OVERRIDE_TIME,
--          VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--          SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--        )
--      SELECT AUTO_BVR_CALC,
--        BAUD_RATE,
--        CONTROLLER_UNIT_MODEL,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CURRENT_FUTURE,
--        DATA_LOGGING_INTERVAL,
--        DATE_MODIFIED,
--        DAYLIGHT_SAVINGS_TIME,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--
--        EST_BANK_VOLTAGE_RISE,
--        EST_VOLTAGE_CHANGE,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        HIGH_VOLTAGE_OVERRIDE_SETPOINT,
--        LOW_VOLTAGE_OVERRIDE_SETPOINT,
--        MASTER_STATION,
--        MAXCYCLES,
--        MIN_SW_VOLTAGE,
--
--        OPERATING_NUM,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        PULSE_TIME,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SCH1_CONTROL_STRATEGY,
--        SCH1_END_DATE,
--        SCH1_HIGH_VOLTAGE_SETPOINT,
--        SCH1_HOLIDAYS,
--        SCH1_KVAR_SETPOINT_OFF,
--        SCH1_KVAR_SETPOINT_ON,
--        SCH1_LOW_VOLTAGE_SETPOINT,
--        SCH1_SATURDAY,
--        SCH1_SCHEDULE,
--        SCH1_START_DATE,
--        SCH1_SUNDAY,
--        SCH1_TEMP_SETPOINT_OFF,
--        SCH1_TEMP_SETPOINT_ON,
--        SCH1_TIME_OFF,
--        SCH1_TIME_ON,
--        SCH1_WEEKDAYS,
--        SCH2_CONTROL_STRATEGY,
--        SCH2_END_DATE,
--        SCH2_HIGH_VOLTAGE_SETPOINT,
--        SCH2_HOLIDAYS,
--        SCH2_KVAR_SETPOINT_OFF,
--        SCH2_KVAR_SETPOINT_ON,
--        SCH2_LOW_VOLTAGE_SETPOINT,
--        SCH2_SATURDAY,
--        SCH2_SCHEDULE,
--        SCH2_START_DATE,
--        SCH2_SUNDAY,
--        SCH2_TEMP_SETPOINT_OFF,
--        SCH2_TEMP_SETPOINT_ON,
--        SCH2_TIME_OFF,
--        SCH2_TIME_ON,
--        SCH2_WEEKDAYS,
--        SCH3_CONTROL_STRATEGY,
--        SCH3_END_DATE,
--        SCH3_HIGH_VOLTAGE_SETPOINT,
--        SCH3_HOLIDAYS,
--        SCH3_KVAR_SETPOINT_OFF,
--        SCH3_KVAR_SETPOINT_ON,
--        SCH3_LOW_VOLTAGE_SETPOINT,
--        SCH3_SATURDAY,
--        SCH3_SCHEDULE,
--        SCH3_START_DATE,
--        SCH3_SUNDAY,
--        SCH3_TEMP_SETPOINT_OFF,
--        SCH3_TEMP_SETPOINT_ON,
--        SCH3_TIME_OFF,
--        SCH3_TIME_ON,
--        SCH3_WEEKDAYS,
--        SCH4_CONTROL_STRATEGY,
--        SCH4_END_DATE,
--        SCH4_HIGH_VOLTAGE_SETPOINT,
--        SCH4_HOLIDAYS,
--        SCH4_KVAR_SETPOINT_OFF,
--        SCH4_KVAR_SETPOINT_ON,
--        SCH4_LOW_VOLTAGE_SETPOINT,
--        SCH4_SATURDAY,
--        SCH4_SCHEDULE,
--        SCH4_START_DATE,
--        SCH4_SUNDAY,
--        SCH4_TEMP_SETPOINT_OFF,
--        SCH4_TEMP_SETPOINT_ON,
--        SCH4_TIME_OFF,
--        SCH4_TIME_ON,
--        SCH4_WEEKDAYS,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        SWITCH_POSITION,
--        TEMPERATURE_CHANGE_TIME,
--        TEMPERATURE_OVERRIDE,
--        TIMESTAMP,
--        TIME_DELAY,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        VOLTAGE_CHANGE_TIME,
--        VOLTAGE_OVERRIDE_TIME,
--        VOLT_VAR_TEAM_MEMBER,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,
--        SCH1_BANK_POSITION,
--SCH2_BANK_POSITION,
--SCH3_BANK_POSITION,
--SCH4_BANK_POSITION,
--SCH1_TIME_ON2,
--SCH1_TIME_OFF2,
--SCH1_WEEKDAYS2,
--SCH1_SATURDAY2,
--SCH1_SUNDAY2,
--SCH1_HOLIDAYS2,
--SCH2_TIME_ON2,
--SCH2_TIME_OFF2,
--SCH2_WEEKDAYS2,
--SCH2_SATURDAY2,
--SCH2_SUNDAY2,
--SCH2_HOLIDAYS2,
--SCH3_TIME_ON2,
--SCH3_TIME_OFF2,
--SCH3_WEEKDAYS2,
--SCH3_SATURDAY2,
--SCH3_SUNDAY2,
--SCH3_HOLIDAYS2,
--SCH4_TIME_ON2,
--SCH4_TIME_OFF2,
--SCH4_WEEKDAYS2,
--SCH4_SATURDAY2,
--SCH4_SUNDAY2,
--SCH4_HOLIDAYS2,
--SEASON_OFF,
--SCH1_MONTHUR,
--SCH1_FRISUN,
--SCH1_MONTHUR2,
--SCH1_FRISUN2,
--SCH2_MONTHUR,
--SCH2_FRISUN,
--SCH2_MONTHUR2,
--SCH2_FRISUN2,
--SCH3_MONTHUR,
--SCH3_FRISUN,
--SCH3_MONTHUR2,
--SCH3_FRISUN2,
--SCH4_MONTHUR,
--SCH4_FRISUN,
--SCH4_MONTHUR2,
--SCH4_FRISUN2
--      FROM SM_CAPACITOR
--      WHERE GLOBAL_ID= I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CAPACITOR_HIST',
--          SM_CAPACITOR_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_CAPACITOR WHERE GLOBAL_ID = I_Global_id_Previous;
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--END SP_CAPACITOR_DETECTION;
--
--PROCEDURE SP_CIRCUIT_BREAKER_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER)
--
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_CIRCUIT_BREAKER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_CIRCUIT_BREAKER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CIRCUIT_BREAKER',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--      -- first copy the entire current record to history table
--      INSERT
--      INTO SM_CIRCUIT_BREAKER_HIST
--        (
--          ANNUAL_LF,
--          BAUD_RATE,
--          CC_RATING,
--          CURRENT_FUTURE,
--          DATE_MODIFIED,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          DPA_CD,
--          EFFECTIVE_DT,
--          ENGINEERING_COMMENTS,
--          FEATURE_CLASS_NAME,
--          FLISR,
--          FLISR_ENGINEERING_COMMENTS,
--          FLISR_OPERATING_MODE,
--          GLOBAL_ID,
--          GRD_BK_INS_TRIP,
--          GRD_BK_LEVER_SET,
--          GRD_BK_MIN_TRIP,
--          GRD_BK_RELAY_TYPE,
--          GRD_PR_INS_TRIP,
--          GRD_PR_LEVER_SET,
--          GRD_PR_MIN_TRIP,
--          GRD_PR_RELAY_TYPE,
--          LIMITING_FACTOR,
--          MASTER_STATION,
--          MIN_NOR_VOLT,
--          NETWORK,
--          OK_TO_BYPASS,
--          OPERATING_MODE,
--          OPERATING_NUM,
--          OPS_TO_LOCKOUT,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PHA_BK_INS_TRIP,
--          PHA_BK_LEVER_SET,
--          PHA_BK_MIN_TRIP,
--          PHA_BK_RELAY_TYPE,
--          PHA_PR_INS_TRIP,
--          PHA_PR_LEVER_SET,
--          PHA_PR_MIN_TRIP,
--          PHA_PR_RELAY_TYPE,
--          PREPARED_BY,
--          PROCESSED_FLAG,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SPECIAL_CONDITIONS,
--          SUMMER_LOAD_LIMIT,
--          TIMESTAMP,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--          ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--        )
--      SELECT ANNUAL_LF,
--        BAUD_RATE,
--        CC_RATING,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        DPA_CD,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FLISR,
--        FLISR_ENGINEERING_COMMENTS,
--        FLISR_OPERATING_MODE,
--        GLOBAL_ID,
--        GRD_BK_INS_TRIP,
--        GRD_BK_LEVER_SET,
--        GRD_BK_MIN_TRIP,
--        GRD_BK_RELAY_TYPE,
--        GRD_PR_INS_TRIP,
--        GRD_PR_LEVER_SET,
--        GRD_PR_MIN_TRIP,
--        GRD_PR_RELAY_TYPE,
--        LIMITING_FACTOR,
--        MASTER_STATION,
--        MIN_NOR_VOLT,
--        NETWORK,
--        OK_TO_BYPASS,
--        OPERATING_MODE,
--        OPERATING_NUM,
--        OPS_TO_LOCKOUT,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_BK_INS_TRIP,
--        PHA_BK_LEVER_SET,
--        PHA_BK_MIN_TRIP,
--        PHA_BK_RELAY_TYPE,
--        PHA_PR_INS_TRIP,
--        PHA_PR_LEVER_SET,
--        PHA_PR_MIN_TRIP,
--        PHA_PR_RELAY_TYPE,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SPECIAL_CONDITIONS,
--        SUMMER_LOAD_LIMIT,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--        ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID= I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CIRCUIT_BREAKER_HIST',
--          SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_CIRCUIT_BREAKER
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--        -- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--  IF ACTION = 'D' THEN
--  BEGIN
--    -- first copy the entire current record to history table
--    INSERT
--    INTO SM_CIRCUIT_BREAKER_HIST
--      (
--        ANNUAL_LF,
--        BAUD_RATE,
--        CC_RATING,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        DPA_CD,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FLISR,
--        FLISR_ENGINEERING_COMMENTS,
--        FLISR_OPERATING_MODE,
--        GLOBAL_ID,
--        GRD_BK_INS_TRIP,
--        GRD_BK_LEVER_SET,
--        GRD_BK_MIN_TRIP,
--        GRD_BK_RELAY_TYPE,
--        GRD_PR_INS_TRIP,
--        GRD_PR_LEVER_SET,
--        GRD_PR_MIN_TRIP,
--        GRD_PR_RELAY_TYPE,
--        LIMITING_FACTOR,
--        MASTER_STATION,
--        MIN_NOR_VOLT,
--        NETWORK,
--        OK_TO_BYPASS,
--        OPERATING_MODE,
--        OPERATING_NUM,
--        OPS_TO_LOCKOUT,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_BK_INS_TRIP,
--        PHA_BK_LEVER_SET,
--        PHA_BK_MIN_TRIP,
--        PHA_BK_RELAY_TYPE,
--        PHA_PR_INS_TRIP,
--        PHA_PR_LEVER_SET,
--        PHA_PR_MIN_TRIP,
--        PHA_PR_RELAY_TYPE,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SPECIAL_CONDITIONS,
--        SUMMER_LOAD_LIMIT,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--        ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--      )
--    SELECT ANNUAL_LF,
--      BAUD_RATE,
--      CC_RATING,
--      CURRENT_FUTURE,
--      DATE_MODIFIED,
--      DEVICE_ID,
--      DISTRICT,
--      DIVISION,
--      DPA_CD,
--      EFFECTIVE_DT,
--      ENGINEERING_COMMENTS,
--      FEATURE_CLASS_NAME,
--      FLISR,
--      FLISR_ENGINEERING_COMMENTS,
--      FLISR_OPERATING_MODE,
--      GLOBAL_ID,
--      GRD_BK_INS_TRIP,
--      GRD_BK_LEVER_SET,
--      GRD_BK_MIN_TRIP,
--      GRD_BK_RELAY_TYPE,
--      GRD_PR_INS_TRIP,
--      GRD_PR_LEVER_SET,
--      GRD_PR_MIN_TRIP,
--      GRD_PR_RELAY_TYPE,
--      LIMITING_FACTOR,
--      MASTER_STATION,
--      MIN_NOR_VOLT,
--      NETWORK,
--      OK_TO_BYPASS,
--      OPERATING_MODE,
--      OPERATING_NUM,
--      OPS_TO_LOCKOUT,
--      PEER_REVIEW_BY,
--      PEER_REVIEW_DT,
--      PHA_BK_INS_TRIP,
--      PHA_BK_LEVER_SET,
--      PHA_BK_MIN_TRIP,
--      PHA_BK_RELAY_TYPE,
--      PHA_PR_INS_TRIP,
--      PHA_PR_LEVER_SET,
--      PHA_PR_MIN_TRIP,
--      PHA_PR_RELAY_TYPE,
--      PREPARED_BY,
--      PROCESSED_FLAG,
--      RADIO_MANF_CD,
--      RADIO_MODEL_NUM,
--      RADIO_SERIAL_NUM,
--      RELAY_TYPE,
--      RELEASED_BY,
--      REPEATER,
--      RTU_ADDRESS,
--      SCADA,
--      SCADA_TYPE,
--      SPECIAL_CONDITIONS,
--      SUMMER_LOAD_LIMIT,
--      TIMESTAMP,
--      TRANSMIT_DISABLE_DELAY,
--      TRANSMIT_ENABLE_DELAY,
--      WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--      ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--    FROM SM_CIRCUIT_BREAKER
--    WHERE GLOBAL_ID= I_Global_id_Current;
--    DELETE FROM SM_CIRCUIT_BREAKER WHERE GLOBAL_ID = I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_CIRCUIT_BREAKER_HIST',
--        SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END;
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_CIRCUIT_BREAKER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_CIRCUIT_BREAKER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CIRCUIT_BREAKER ',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_CIRCUIT_BREAKER_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_CIRCUIT_BREAKER
--      SET
--        (
--          ANNUAL_LF,
--          BAUD_RATE,
--          CC_RATING,
--          DPA_CD,
--          ENGINEERING_COMMENTS,
--          FLISR,
--          FLISR_ENGINEERING_COMMENTS,
--          FLISR_OPERATING_MODE,
--          GRD_BK_INS_TRIP,
--          GRD_BK_LEVER_SET,
--          GRD_BK_MIN_TRIP,
--          GRD_BK_RELAY_TYPE,
--          GRD_PR_INS_TRIP,
--          GRD_PR_LEVER_SET,
--          GRD_PR_MIN_TRIP,
--          GRD_PR_RELAY_TYPE,
--          LIMITING_FACTOR,
--          MASTER_STATION,
--          MIN_NOR_VOLT,
--          NETWORK,
--          OK_TO_BYPASS,
--          OPERATING_MODE,
--          OPS_TO_LOCKOUT,
--          PHA_BK_INS_TRIP,
--          PHA_BK_LEVER_SET,
--          PHA_BK_MIN_TRIP,
--          PHA_BK_RELAY_TYPE,
--          PHA_PR_INS_TRIP,
--          PHA_PR_LEVER_SET,
--          PHA_PR_MIN_TRIP,
--          PHA_PR_RELAY_TYPE,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SPECIAL_CONDITIONS,
--          SUMMER_LOAD_LIMIT,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--          ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        )
--        =
--        (SELECT ANNUAL_LF,
--          BAUD_RATE,
--          CC_RATING,
--          DPA_CD,
--          ENGINEERING_COMMENTS,
--          FLISR,
--          FLISR_ENGINEERING_COMMENTS,
--          FLISR_OPERATING_MODE,
--          GRD_BK_INS_TRIP,
--          GRD_BK_LEVER_SET,
--          GRD_BK_MIN_TRIP,
--          GRD_BK_RELAY_TYPE,
--          GRD_PR_INS_TRIP,
--          GRD_PR_LEVER_SET,
--          GRD_PR_MIN_TRIP,
--          GRD_PR_RELAY_TYPE,
--          LIMITING_FACTOR,
--          MASTER_STATION,
--          MIN_NOR_VOLT,
--          NETWORK,
--          OK_TO_BYPASS,
--          OPERATING_MODE,
--          OPS_TO_LOCKOUT,
--          PHA_BK_INS_TRIP,
--          PHA_BK_LEVER_SET,
--          PHA_BK_MIN_TRIP,
--          PHA_BK_RELAY_TYPE,
--          PHA_PR_INS_TRIP,
--          PHA_PR_LEVER_SET,
--          PHA_PR_MIN_TRIP,
--          PHA_PR_RELAY_TYPE,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SPECIAL_CONDITIONS,
--          SUMMER_LOAD_LIMIT,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--          ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        FROM SM_CIRCUIT_BREAKER
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT
--      INTO SM_CIRCUIT_BREAKER_HIST
--        (
--          ANNUAL_LF,
--          BAUD_RATE,
--          CC_RATING,
--          CURRENT_FUTURE,
--          DATE_MODIFIED,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          DPA_CD,
--          EFFECTIVE_DT,
--          ENGINEERING_COMMENTS,
--          FEATURE_CLASS_NAME,
--          FLISR,
--          FLISR_ENGINEERING_COMMENTS,
--          FLISR_OPERATING_MODE,
--          GLOBAL_ID,
--          GRD_BK_INS_TRIP,
--          GRD_BK_LEVER_SET,
--          GRD_BK_MIN_TRIP,
--          GRD_BK_RELAY_TYPE,
--          GRD_PR_INS_TRIP,
--          GRD_PR_LEVER_SET,
--          GRD_PR_MIN_TRIP,
--          GRD_PR_RELAY_TYPE,
--          LIMITING_FACTOR,
--          MASTER_STATION,
--          MIN_NOR_VOLT,
--          NETWORK,
--          OK_TO_BYPASS,
--          OPERATING_MODE,
--          OPERATING_NUM,
--          OPS_TO_LOCKOUT,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PHA_BK_INS_TRIP,
--          PHA_BK_LEVER_SET,
--          PHA_BK_MIN_TRIP,
--          PHA_BK_RELAY_TYPE,
--          PHA_PR_INS_TRIP,
--          PHA_PR_LEVER_SET,
--          PHA_PR_MIN_TRIP,
--          PHA_PR_RELAY_TYPE,
--          PREPARED_BY,
--          PROCESSED_FLAG,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SPECIAL_CONDITIONS,
--          SUMMER_LOAD_LIMIT,
--          TIMESTAMP,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--          ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--        )
--      SELECT ANNUAL_LF,
--        BAUD_RATE,
--        CC_RATING,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        DPA_CD,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FLISR,
--        FLISR_ENGINEERING_COMMENTS,
--        FLISR_OPERATING_MODE,
--        GLOBAL_ID,
--        GRD_BK_INS_TRIP,
--        GRD_BK_LEVER_SET,
--        GRD_BK_MIN_TRIP,
--        GRD_BK_RELAY_TYPE,
--        GRD_PR_INS_TRIP,
--        GRD_PR_LEVER_SET,
--        GRD_PR_MIN_TRIP,
--        GRD_PR_RELAY_TYPE,
--        LIMITING_FACTOR,
--        MASTER_STATION,
--        MIN_NOR_VOLT,
--        NETWORK,
--        OK_TO_BYPASS,
--        OPERATING_MODE,
--        OPERATING_NUM,
--        OPS_TO_LOCKOUT,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_BK_INS_TRIP,
--        PHA_BK_LEVER_SET,
--        PHA_BK_MIN_TRIP,
--        PHA_BK_RELAY_TYPE,
--        PHA_PR_INS_TRIP,
--        PHA_PR_LEVER_SET,
--        PHA_PR_MIN_TRIP,
--        PHA_PR_RELAY_TYPE,
--        PREPARED_BY,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SPECIAL_CONDITIONS,
--        SUMMER_LOAD_LIMIT,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES
--        ,GRD_PR_INST_DELAY,PHA_PR_INST_DELAY,PHA_BK_INST_DELAY,GRD_BK_INST_DELAY,GRD_PR_OPS_TO_LOCKOUT,PHA_PR_OPS_TO_LOCKOUT,
--          PHA_BK_OPS_TO_LOCKOUT,GRD_BK_OPS_TO_LOCKOUT
--      FROM SM_CIRCUIT_BREAKER
--      WHERE GLOBAL_ID= I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_CIRCUIT_BREAKER _HIST',
--          SM_CIRCUIT_BREAKER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_CIRCUIT_BREAKER WHERE GLOBAL_ID = I_Global_id_Previous;
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--END SP_CIRCUIT_BREAKER_DETECTION;
--
--PROCEDURE SP_INTERRUPTER_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER)
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_INTERRUPTER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_INTERRUPTER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_INTERRUPTER',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--      -- first copy the entire current record to history table
--      INSERT
--      INTO SM_INTERRUPTER_HIST
--        (
--          BAUD_RATE,
--          CONTROL_SERIAL_NUM,
--          CONTROL_TYPE,
--          CT_RATIO,
--          CURRENT_FUTURE,
--          DATE_MODIFIED,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          EFFECTIVE_DT,
--          ENGINEERING_COMMENTS,
--          FEATURE_CLASS_NAME,
--          FIRMWARE_VERSION,
--          GLOBAL_ID,
--          GRD_CUR_TRIP,
--          GRD_INST_PICKUP_SETTING,
--          GRD_PICKUP_SETTING,
--          GRD_TD_LEVER_SETTING,
--          GRD_TRIP_CD,
--          MANF_CD,
--          MASTER_STATION,
--          OK_TO_BYPASS,
--          OPERATING_NUM,
--          OPERATIONAL_MODE_SWITCH,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PHA_CUR_TRIP,
--          PHA_INST_PICKUP_SETTING,
--          PHA_PICKUP_SETTING,
--          PHA_TD_LEVER_SETTING,
--          PHA_TRIP_CD,
--          PREPARED_BY,
--          PRIMARY_VOLTAGE,
--          PROCESSED_FLAG,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          TIMESTAMP,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          TYP_CRV_GRD,
--          TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--        )
--      SELECT BAUD_RATE,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CT_RATIO,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        GRD_CUR_TRIP,
--        GRD_INST_PICKUP_SETTING,
--        GRD_PICKUP_SETTING,
--        GRD_TD_LEVER_SETTING,
--        GRD_TRIP_CD,
--        MANF_CD,
--        MASTER_STATION,
--        OK_TO_BYPASS,
--        OPERATING_NUM,
--        OPERATIONAL_MODE_SWITCH,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_CUR_TRIP,
--        PHA_INST_PICKUP_SETTING,
--        PHA_PICKUP_SETTING,
--        PHA_TD_LEVER_SETTING,
--        PHA_TRIP_CD,
--        PREPARED_BY,
--        PRIMARY_VOLTAGE,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        TYP_CRV_GRD,
--        TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_INTERRUPTER_HIST',
--          SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_INTERRUPTER
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District,
--        CONTROL_TYPE    =I_Control_type_code
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--      --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--  IF ACTION = 'D' THEN
--    -- first copy the entire current record to history table
--    INSERT
--    INTO SM_INTERRUPTER_HIST
--      (
--        BAUD_RATE,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CT_RATIO,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        GRD_CUR_TRIP,
--        GRD_INST_PICKUP_SETTING,
--        GRD_PICKUP_SETTING,
--        GRD_TD_LEVER_SETTING,
--        GRD_TRIP_CD,
--        MANF_CD,
--        MASTER_STATION,
--        OK_TO_BYPASS,
--        OPERATING_NUM,
--        OPERATIONAL_MODE_SWITCH,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_CUR_TRIP,
--        PHA_INST_PICKUP_SETTING,
--        PHA_PICKUP_SETTING,
--        PHA_TD_LEVER_SETTING,
--        PHA_TRIP_CD,
--        PREPARED_BY,
--        PRIMARY_VOLTAGE,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        TYP_CRV_GRD,
--        TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--      )
--    SELECT BAUD_RATE,
--      CONTROL_SERIAL_NUM,
--      CONTROL_TYPE,
--      CT_RATIO,
--      CURRENT_FUTURE,
--      DATE_MODIFIED,
--      DEVICE_ID,
--      DISTRICT,
--      DIVISION,
--      EFFECTIVE_DT,
--      ENGINEERING_COMMENTS,
--      FEATURE_CLASS_NAME,
--      FIRMWARE_VERSION,
--      GLOBAL_ID,
--      GRD_CUR_TRIP,
--      GRD_INST_PICKUP_SETTING,
--      GRD_PICKUP_SETTING,
--      GRD_TD_LEVER_SETTING,
--      GRD_TRIP_CD,
--      MANF_CD,
--      MASTER_STATION,
--      OK_TO_BYPASS,
--      OPERATING_NUM,
--      OPERATIONAL_MODE_SWITCH,
--      PEER_REVIEW_BY,
--      PEER_REVIEW_DT,
--      PHA_CUR_TRIP,
--      PHA_INST_PICKUP_SETTING,
--      PHA_PICKUP_SETTING,
--      PHA_TD_LEVER_SETTING,
--      PHA_TRIP_CD,
--      PREPARED_BY,
--      PRIMARY_VOLTAGE,
--      PROCESSED_FLAG,
--      RADIO_MANF_CD,
--      RADIO_MODEL_NUM,
--      RADIO_SERIAL_NUM,
--      RELAY_TYPE,
--      RELEASED_BY,
--      REPEATER,
--      RTU_ADDRESS,
--      SCADA,
--      SCADA_TYPE,
--      SOFTWARE_VERSION,
--      SPECIAL_CONDITIONS,
--      TIMESTAMP,
--      TRANSMIT_DISABLE_DELAY,
--      TRANSMIT_ENABLE_DELAY,
--      TYP_CRV_GRD,
--      TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--    FROM SM_INTERRUPTER
--    WHERE GLOBAL_ID=I_Global_id_Current;
--    DELETE FROM SM_INTERRUPTER WHERE GLOBAL_ID = I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_INTERRUPTER_HIST',
--        SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_INTERRUPTER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_INTERRUPTER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_INTERRUPTER',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_INTERRUPTER_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_INTERRUPTER
--      SET
--        (
--          BAUD_RATE,
--          CONTROL_TYPE,
--          CT_RATIO,
--          ENGINEERING_COMMENTS,
--          FIRMWARE_VERSION,
--          GRD_CUR_TRIP,
--          GRD_INST_PICKUP_SETTING,
--          GRD_PICKUP_SETTING,
--          GRD_TD_LEVER_SETTING,
--          GRD_TRIP_CD,
--          MANF_CD,
--          MASTER_STATION,
--          OK_TO_BYPASS,
--          OPERATIONAL_MODE_SWITCH,
--          PHA_CUR_TRIP,
--          PHA_INST_PICKUP_SETTING,
--          PHA_PICKUP_SETTING,
--          PHA_TD_LEVER_SETTING,
--          PHA_TRIP_CD,
--          PRIMARY_VOLTAGE,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          TYP_CRV_GRD,
--          TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        )
--        =
--        (SELECT BAUD_RATE,
--          CONTROL_TYPE,
--          CT_RATIO,
--          ENGINEERING_COMMENTS,
--
--          FIRMWARE_VERSION,
--          GRD_CUR_TRIP,
--          GRD_INST_PICKUP_SETTING,
--          GRD_PICKUP_SETTING,
--          GRD_TD_LEVER_SETTING,
--          GRD_TRIP_CD,
--          MANF_CD,
--          MASTER_STATION,
--          OK_TO_BYPASS,
--          OPERATIONAL_MODE_SWITCH,
--          PHA_CUR_TRIP,
--          PHA_INST_PICKUP_SETTING,
--          PHA_PICKUP_SETTING,
--          PHA_TD_LEVER_SETTING,
--          PHA_TRIP_CD,
--          PRIMARY_VOLTAGE,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          TYP_CRV_GRD,
--          TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS,
--		  DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        FROM SM_INTERRUPTER
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT
--      INTO SM_INTERRUPTER_HIST
--        (
--          BAUD_RATE,
--          CONTROL_SERIAL_NUM,
--          CONTROL_TYPE,
--          CT_RATIO,
--          CURRENT_FUTURE,
--          DATE_MODIFIED,
--          DEVICE_ID,
--          DISTRICT,
--          DIVISION,
--          EFFECTIVE_DT,
--          ENGINEERING_COMMENTS,
--          FEATURE_CLASS_NAME,
--          FIRMWARE_VERSION,
--          GLOBAL_ID,
--          GRD_CUR_TRIP,
--          GRD_INST_PICKUP_SETTING,
--          GRD_PICKUP_SETTING,
--          GRD_TD_LEVER_SETTING,
--          GRD_TRIP_CD,
--          MANF_CD,
--          MASTER_STATION,
--          OK_TO_BYPASS,
--          OPERATING_NUM,
--          OPERATIONAL_MODE_SWITCH,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          PHA_CUR_TRIP,
--          PHA_INST_PICKUP_SETTING,
--          PHA_PICKUP_SETTING,
--          PHA_TD_LEVER_SETTING,
--          PHA_TRIP_CD,
--          PREPARED_BY,
--          PRIMARY_VOLTAGE,
--          PROCESSED_FLAG,
--          RADIO_MANF_CD,
--          RADIO_MODEL_NUM,
--          RADIO_SERIAL_NUM,
--          RELAY_TYPE,
--          RELEASED_BY,
--          REPEATER,
--          RTU_ADDRESS,
--          SCADA,
--          SCADA_TYPE,
--          SOFTWARE_VERSION,
--          SPECIAL_CONDITIONS,
--          TIMESTAMP,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          TYP_CRV_GRD,
--          TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--        )
--      SELECT BAUD_RATE,
--        CONTROL_SERIAL_NUM,
--        CONTROL_TYPE,
--        CT_RATIO,
--        CURRENT_FUTURE,
--        DATE_MODIFIED,
--        DEVICE_ID,
--        DISTRICT,
--        DIVISION,
--        EFFECTIVE_DT,
--        ENGINEERING_COMMENTS,
--        FEATURE_CLASS_NAME,
--        FIRMWARE_VERSION,
--        GLOBAL_ID,
--        GRD_CUR_TRIP,
--        GRD_INST_PICKUP_SETTING,
--        GRD_PICKUP_SETTING,
--        GRD_TD_LEVER_SETTING,
--        GRD_TRIP_CD,
--        MANF_CD,
--        MASTER_STATION,
--        OK_TO_BYPASS,
--        OPERATING_NUM,
--        OPERATIONAL_MODE_SWITCH,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        PHA_CUR_TRIP,
--        PHA_INST_PICKUP_SETTING,
--        PHA_PICKUP_SETTING,
--        PHA_TD_LEVER_SETTING,
--        PHA_TRIP_CD,
--        PREPARED_BY,
--        PRIMARY_VOLTAGE,
--        PROCESSED_FLAG,
--        RADIO_MANF_CD,
--        RADIO_MODEL_NUM,
--        RADIO_SERIAL_NUM,
--        RELAY_TYPE,
--        RELEASED_BY,
--        REPEATER,
--        RTU_ADDRESS,
--        SCADA,
--        SCADA_TYPE,
--        SOFTWARE_VERSION,
--        SPECIAL_CONDITIONS,
--        TIMESTAMP,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        TYP_CRV_GRD,
--        TYP_CRV_PHA,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--      FROM SM_INTERRUPTER
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_INTERRUPTER_HIST',
--          SM_INTERRUPTER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_INTERRUPTER WHERE GLOBAL_ID = I_Global_id_Previous;
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--    END;
--  END IF;
--END SP_INTERRUPTER_DETECTION;
--    Change for project GIS powerbase integration - End

--Changes done by TCS on 6th June 17
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
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER
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
          CURRENT_FUTURE,
          REVERSETRIP,
          TIMEDELAY,
          OVERCURRENT,
          PHASINGLINEVOLTAGEOFFSET,
          PHASINGLINEANGLE,
          MASTERLINEVOLTAGEOFFSET,
          MASTERLINEANGLE,
          WATTTRIPANGLE,
          GULLWINGANGLE,
          VARTRIPANGLE,
          CTPRIMARYRATING
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'C',
          .25,
          0,
          0,
          0.0,
          -5.0,
          1.0,
          90,
          5.0,
          -5.0,
          -5.0,
          1600
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
          CURRENT_FUTURE,
          REVERSETRIP,
          TIMEDELAY,
          OVERCURRENT,
          PHASINGLINEVOLTAGEOFFSET,
          PHASINGLINEANGLE,
          MASTERLINEVOLTAGEOFFSET,
          MASTERLINEANGLE,
          WATTTRIPANGLE,
          GULLWINGANGLE,
          VARTRIPANGLE,
          CTPRIMARYRATING
        )
        VALUES
        (
          I_Global_id_Current,
          I_feature_class_name,
          I_operating_num ,
          I_Division ,
          I_District,
          'F',
          .25,
          0,
           0,
          0.0,
          -5.0,
          1.0,
          90,
          5.0,
          -5.0,
          -5.0,
          1600
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
        GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
             )
      SELECT GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
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
        GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
             )
      SELECT GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
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
          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
        );
	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
	  UPDATE SM_NETWORK_PROTECTOR_HIST  SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
      UPDATE SM_NETWORK_PROTECTOR
      SET
        (
          FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,

        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO)
        =
        (SELECT FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,

        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO  FROM SM_NETWORK_PROTECTOR
        WHERE GLOBAL_ID   = I_Global_id_Previous
        AND CURRENT_FUTURE='C'
        )
      WHERE GLOBAL_ID = I_Global_id_Current;
      INSERT
      INTO SM_NETWORK_PROTECTOR_HIST
        (
           GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
        )
      SELECT GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM,DEVICE_ID,
        PREPARED_BY,DATE_MODIFIED,EFFECTIVE_DT,
        PEER_REVIEW_DT,PEER_REVIEW_BY
        ,DIVISION,
        DISTRICT,
        NOTES,
        CURRENT_FUTURE,
        SPECIAL_CONDITIONS,
        TRIPMODE,
        REVERSETRIP,TIMEDELAY,OVERCURRENT,
        CLOSEMODE,PHASINGLINEVOLTAGEOFFSET,
        PHASINGLINEANGLE, MASTERLINEVOLTAGEOFFSET,
        MASTERLINEANGLE,WATTVARTRIP,WATTTRIPANGLE,
        GULLWINGANGLE,VARTRIPANGLE,CTPRIMARYRATING,SERIAL_NUM,CTRATIO
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

--    Change for project GIS powerbase integration - Start
--Changes done by TCS on 26Oct 2016
--PROCEDURE SP_RECLOSER_TS_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER
--  )
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current) NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current) NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new (current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new (current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER_TS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER_TS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_TS',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--      -- first copy the entire current record to history table
--      INSERT INTO SM_RECLOSER_TS_HIST
--        (
--GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--        )
--      SELECT
--GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_TS_HIST',
--          SM_RECLOSER_TS_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_RECLOSER_TS
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--       --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--  IF ACTION= 'D' THEN
--    -- first copy the entire current record to history table
--    INSERT INTO SM_RECLOSER_TS_HIST
--      (
--          GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--      )
--    SELECT
--          GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--    FROM SM_RECLOSER_TS
--    WHERE GLOBAL_ID=I_Global_id_Current;
--    DELETE FROM SM_RECLOSER_TS WHERE GLOBAL_ID =
--I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_RECLOSER_TS_HIST',
--        SM_RECLOSER_TS_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER_TS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER_TS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_TS',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET  GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_RECLOSER_TS_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_RECLOSER_TS
--      SET
--        (
--                --GLOBAL_ID,-- S2NN 12/31/2016 Error in Change Detection
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
----CURRENT_FUTURE,-- S2NN 12/31/2016 Error in Change Detection
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--        )
--        =
--        (SELECT
--          --GLOBAL_ID,-- S2NN 12/31/2016 Error in Change Detection
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
----CURRENT_FUTURE,-- S2NN 12/31/2016 Error in Change Detection
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--        FROM SM_RECLOSER_TS
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT INTO SM_RECLOSER_TS_HIST
--        (
--                   GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--        )
--      SELECT
--          GLOBAL_ID,
--FEATURE_CLASS_NAME,
--OPERATING_NUM,
--DEVICE_ID,
--PREPARED_BY,
--DATE_MODIFIED,
--TIMESTAMP,
--EFFECTIVE_DT,
--PEER_REVIEW_DT,
--PEER_REVIEW_BY,
--DIVISION,
--DISTRICT,
--CURRENT_FUTURE,
--INI_EMULATED_DEVICE,
--INI_INVERSE_SEGMENT,
--INI_SPEED,
--INI_AMPERE_RATING,
--INI_MIN_TRIP_A,
--INI_TIME_MULTIPLIER,
--INI_RESET_TYPE,
--INI_RESET_TIME,
--INI_LOWCUTOFF,
--INI_LOWCUTOFF_CURR_A,
--INI_DEFINITETIME1,
--INI_DEFINITETIME1_CURR_A,
--INI_DEFINITETIME1_TIME,
--INI_DEFINITETIME2,
--INI_DEFINITETIME2_CURR_A,
--INI_DEFINITETIME2_TIME,
--INI_OPEN_INTERVAL_TIME,
--INI_COIL_RATING,
--TEST1_EMULATED_DEVICE,
--TEST1_INVERSE_SEGMENT,
--TEST1_SPEED,
--TEST1_AMPERE_RATING,
--TEST1_MIN_TRIP_A,
--TEST1_TIME_MULTIPLIER,
--TEST1_RESET_TYPE,
--TEST1_RESET_TIME,
--TEST1_LOWCUTOFF,
--TEST1_LOWCUTOFF_CURR_A,
--TEST1_DEFINITETIME1,
--TEST1_DEFINITETIME1_CUR_A,
--TEST1_DEFINITETIME1_TIME,
--TEST1_DEFINITETIME2,
--TEST1_DEFINITETIME2_CURR_A,
--TEST1_DEFINITETIME2_TIME,
--TEST1_OPEN_INTERVAL_TIME,
--TEST1_COIL_RATING,
--TEST2_EMULATED_DEVICE,
--TEST2_INVERSE_SEGMENT,
--TEST2_SPEED,
--TEST2_AMPERE_RATING,
--TEST2_MIN_TRIP_A,
--TEST2_TIME_MULTIPLIER,
--TEST2_RESET_TYPE,
--TEST2_RESET_TIME,
--TEST2_LOWCUTOFF,
--TEST2_LOWCUTOFF_CURR_A,
--TEST2_DEFINITETIME1,
--TEST2_DEFINITETIME1_CURR_A,
--TEST2_DEFINITETIME1_TIME,
--TEST2_DEFINITETIME2,
--TEST2_DEFINITETIME2_CURR_A,
--TEST2_DEFINITETIME2_TIME,
--TEST2_OPEN_INTERVAL_TIME,
--TEST2_COIL_RATING,
--TEST3_EMULATED_DEVICE,
--TEST3_INVERSE_SEGMENT,
--TEST3_SPEED,
--TEST3_AMPERE_RATING,
--TEST3_MIN_TRIP_A,
--TEST3_TIME_MULTIPLIER,
--TEST3_RESET_TYPE,
--TEST3_RESET_TIME,
--TEST3_LOWCUTOFF,
--TEST3_LOWCUTOFF_CURR_A,
--TEST3_DEFINITETIME1,
--TEST3_DEFINITETIME1_CURR_A,
--TEST3_DEFINITETIME1_TIME,
--TEST3_DEFINITETIME2,
--TEST3_DEFINITETIME2_CURR_A,
--TEST3_DEFINITETIME2_TIME,
--TEST3_COIL_RATING,
--HIGH_CURRENT_CUTOFF,
--HIGH_CURRENT_CUTOFF_A,
--SEC_MODE_COUNTS,
--SEC_MODE,
--SEC_MODE_RESET_TIME,
--SEC_MODE_STARTING_CURRENT,
--SEQUENCE_RESET_TIME,OK_TO_BYPASS,
--BYPASS_PLANS
--      FROM SM_RECLOSER_TS
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_TS_HIST',
--          SM_RECLOSER_TS_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_RECLOSER_TS WHERE GLOBAL_ID =
--I_Global_id_Previous;
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--
--    END;
--  END IF;
--END SP_RECLOSER_TS_DETECTION;
---- FuseSaver-2018---
----Changes done by TCS on 2ndJuly,2018
--PROCEDURE SP_RECLOSER_FS_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER
--  )
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current) NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current) NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new (current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new (current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER_FS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER_FS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_FS',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--      -- first copy the entire current record to history table
--      INSERT INTO SM_RECLOSER_FS_HIST
--        (
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--        )
--      SELECT
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_FS_HIST',
--          SM_RECLOSER_FS_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_RECLOSER_FS
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--       --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--  IF ACTION= 'D' THEN
--    -- first copy the entire current record to history table
--    INSERT INTO SM_RECLOSER_FS_HIST
--      (
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--      )
--    SELECT
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--    FROM SM_RECLOSER_FS
--    WHERE GLOBAL_ID=I_Global_id_Current;
--    DELETE FROM SM_RECLOSER_FS WHERE GLOBAL_ID =
--I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_RECLOSER_FS_HIST',
--        SM_RECLOSER_FS_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER_FS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER_FS
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_FS',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET  GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_RECLOSER_FS_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_RECLOSER_FS
--      SET
--        (
--                --GLOBAL_ID,-- S2NN 12/31/2016 Error in Change Detection
--FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  --CURRENT_FUTURE,--K5KJ 03/26/2021 Error in Change Detection
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--        )
--        =
--        (SELECT
--FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  --CURRENT_FUTURE,--K5KJ 03/26/2021 Error in Change Detection
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--        FROM SM_RECLOSER_FS
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT INTO SM_RECLOSER_FS_HIST
--        (
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--        )
--      SELECT
--
--  GLOBAL_ID,
--  FEATURE_CLASS_NAME,
--  OPERATING_NUM,
--  DEVICE_ID,
--  PREPARED_BY,
--  DATE_MODIFIED,
--  TIMESTAMP,
--  EFFECTIVE_DT,
--  PEER_REVIEW_DT,
--  PEER_REVIEW_BY,
--  DIVISION,
--  DISTRICT,
--  CURRENT_FUTURE,
--  POLICY_TYPE,
--  FIRMWARE_VERSION,
--  SOFTWARE_VERSION,
--  CONTROL_SERIAL_NUM,
--  RELEASED_BY,
--  OK_TO_BYPASS,
--  SPECIAL_CONDITIONS,
--  PERMIT_RB_CUTIN,
--  BYPASS_PLANS,
--  ENGINEERING_COMMENTS,
--  SCADA,
--  SCADA_TYPE,
--  MASTER_STATION,
--  BAUD_RATE,
--  TRANSMIT_ENABLE_DELAY,
--  TRANSMIT_DISABLE_DELAY,
--  RTU_ADDRESS,
--  REPEATER,
--  RADIO_MANF_CD,
--  RADIO_MODEL_NUM,
--  RADIO_SERIAL_NUM,
--  RTU_EXIST,
--  RTU_MODEL_NUM,
--  RTU_SERIAL_NUM,
--  RTU_FIRMWARE_VERSION,
--  RTU_SOFTWARE_VERSION,
--  RTU_MANF_CD,
--  NORMAL_MIN_TRIP_CM,
--  NORMAL_MAX_FT,
--  NORMAL_INST_TS,
--  NORMAL_MIN_TT,
--  FASTMODE_MIN_TCM,
--  FASTMODE_MAX_FT,
--  FASTMODE_INST_TS,
--  FASTMODE_MIN_TT,
--  OS_DEAD_TS,
--  OS_ENABLE_EXT_LEV,
--  OS_MANUAL_GAN_OP,
--  OS_DE_ENER_LT,
--  OS_RECLAIM_TIME,
--  OS_INRUSH_RM,
--  OS_INRUSH_RT,
--  OS_CAP_CHARGE,
--  LEVER_UP_PM,
--  LEVER_UP_DE_EL_PM,
--  LEVER_UP_PSEDO_3PH,
--  LEVER_UP_3PH_LOCK,
--  LEVER_DOWN_PM,
--  LEVER_DOWN_DE_EL_PM,
--  LEVER_DOWN_PSEDO_3PH,
--  LEVER_DOWN_3PH_LOCK,
--  LEVER_DOWN_MI,
--  OTH_LOAD_PP,
--  OTH_ENDLIFE_POLICY,
--  OTH_LINE_FREQ,
--  OTH_PH_A_LABEL,
--  OTH_PH_B_LABEL,
--  OTH_PH_C_LABEL,
--  OTH_TIMEZONE_DISPLAY,
--  OTH_FAULT_PI,
--  FUSE_TYPE,
--  FUSE_RATING
--      FROM SM_RECLOSER_FS
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_FS_HIST',
--          SM_RECLOSER_FS_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_RECLOSER_FS WHERE GLOBAL_ID =
--I_Global_id_Previous;
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--
--    END;
--  END IF;
--END SP_RECLOSER_FS_DETECTION;
--
--
-----End Fuse Saver
-- PROCEDURE SP_RECLOSER_DETECTION
--  (
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER
--  )
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--    END;
--  END IF;
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--      -- first copy the entire current record to history table
--      INSERT
--      INTO SM_RECLOSER_HIST
--        (
--          EFFECTIVE_DT,
--          TIMESTAMP,
--          DATE_MODIFIED,
--          RELAY_TYPE,
--          PREPARED_BY,
--          DEVICE_ID,
--          OPERATING_NUM,
--          FEATURE_CLASS_NAME,
--          GLOBAL_ID,
--          ALT2_PHA_INST_TRIP_CD,
--          ALT2_GRD_INST_TRIP_CD,
--          ALT2_PHA_MIN_TRIP,
--          ALT2_GRD_MIN_TRIP,
--          ALT2_LS_RESET_TIME,
--          ALT2_LS_LOCKOUT_OPS,
--          ALT2_INRUSH_DURATION,
--          ALT2_PHA_INRUSH_THRESHOLD,
--          ALT2_GRD_INRUSH_THRESHOLD,
--          ALT2_PHA_ARMING_THRESHOLD,
--          ALT2_GRD_ARMING_THRESHOLD,
--          ALT2_PERMIT_LS_ENABLING,
--          ALT_COLD_LOAD_PLI_CURVE_GRD,
--          ALT_COLD_LOAD_PLI_CURVE_PHA,
--          ALT_COLD_LOAD_PLI_GRD,
--          ALT_COLD_LOAD_PLI_PHA,
--          ALT_COLD_LOAD_PLI_USED,
--          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT_HIGH_CURRENT_LOCKOUT_USED,
--          ALT_SGF_TIME_DELAY,
--          ALT_SGF_MIN_TRIP_PERCENT,
--          ALT_SGF_CD,
--          ALT_RECLOSE_RETRY_ENABLED,
--          ALT_RESET,
--          ALT_RECLOSE3_TIME,
--          ALT_RECLOSE2_TIME,
--          ALT_RECLOSE1_TIME,
--          ALT_TOT_LOCKOUT_OPS,
--          ALT_PHA_TADD_SLOW,
--          ALT_GRD_TADD_SLOW,
--          ALT_PHA_TADD_FAST,
--          ALT_GRD_TADD_FAST,
--          ALT_PHA_VMUL_SLOW,
--          ALT_GRD_VMUL_SLOW,
--          ALT_PHA_VMUL_FAST,
--          ALT_GRD_SLOW_CRV_OPS,
--          ALT_PHA_SLOW_CRV_OPS,
--          ALT_GRD_VMUL_FAST,
--          ALT_PHA_SLOW_CRV,
--          ALT_GRD_SLOW_CRV,
--          ALT_PHA_FAST_CRV,
--          ALT_GRD_FAST_CRV,
--          ALT_PHA_RESP_TIME,
--          ALT_GRD_RESP_TIME,
--          ALT_TCC2_SLOW_CURVES_USED,
--          ALT_TCC1_FAST_CURVES_USED,
--          ALT_PHA_OP_F_CRV,
--          ALT_GRD_OP_F_CRV,
--          ALT_PHA_INST_TRIP_CD,
--          ALT_GRD_INST_TRIP_CD,
--          ALT_PHA_MIN_TRIP,
--          ALT_GRD_MIN_TRIP,
--          COLD_LOAD_PLI_CURVE_GRD,
--          COLD_LOAD_PLI_CURVE_PHA,
--          COLD_LOAD_PLI_GRD,
--          COLD_LOAD_PLI_PHA,
--          COLD_LOAD_PLI_USED,
--          HIGH_CURRENT_LOCKUOUT_GRD,
--          HIGH_CURRENT_LOCKOUT_PHA,
--          HIGH_CURRENT_LOCKOUT_USED,
--          SGF_TIME_DELAY,
--          SGF_MIN_TRIP_PERCENT,
--          SGF_CD,
--          RECLOSE_RETRY_ENABLED,
--          RESET,
--          RECLOSE3_TIME,
--          RECLOSE2_TIME,
--          RECLOSE1_TIME,
--          TOT_LOCKOUT_OPS,
--          PHA_TADD_SLOW,
--          GRD_TADD_SLOW,
--          PHA_TADD_FAST,
--          GRD_TADD_FAST,
--          COLD_PHA_TMUL_FAST,
--          COLD_GRD_TMUL_FAST,
--          PHA_TMUL_SLOW,
--          GRD_TMUL_SLOW,
--          PHA_TMUL_FAST,
--          GRD_TMUL_FAST,
--          GRD_SLOW_CRV_OPS,
--          PHA_SLOW_CRV_OPS,
--          PHA_SLOW_CRV,
--          GRD_SLOW_CRV,
--          PHA_FAST_CRV,
--          GRD_FAST_CRV,
--          PHA_RESP_TIME,
--          GRD_RESP_TIME,
--          PHA_OP_F_CRV,
--          GRD_OP_F_CRV,
--          TCC2_SLOW_CURVES_USED,
--          TCC1_FAST_CURVES_USED,
--          PHA_INST_TRIP_CD,
--          GRD_INST_TRIP_CD,
--          PHA_MIN_TRIP,
--          GRD_MIN_TRIP,
--          OPERATING_AS_CD,
--          BYPASS_PLANS,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          RELEASED_BY,
--          PROCESSED_FLAG,
--          CONTROL_SERIAL_NUM,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          CURRENT_FUTURE,
--          DISTRICT,
--          DIVISION,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          FLISR_ENGINEERING_COMMENTS,
--          "Limiting_Factor",
--          "Winter_Load_Limit",
--          "Summer_Load_Limit",
--          FLISR,
--          ENGINEERING_COMMENTS,
--          ACTIVE_PROFILE,
--          ALT3_COLD_LOAD_PLI_CURVE_GRD,
--          ALT3_COLD_LOAD_PLI_CURVE_PHA,
--          ALT3_COLD_LOAD_PLI_GRD,
--          ALT3_COLD_LOAD_PLI_PHA,
--          ALT3_COLD_LOAD_PLI_USED,
--          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT3_HIGH_CURRENT_LOCKOUT_USED,
--          ALT3_SGF_TIME_DELAY,
--          ALT3_SGF_MIN_TRIP_PERCENT,
--          ALT3_SGF_CD,
--          ALT3_RECLOSE_RETRY_ENABLED,
--          ALT3_RESET,
--          ALT3_RECLOSE3_TIME,
--          ALT3_RECLOSE2_TIME,
--          ALT3_RECLOSE1_TIME,
--          ALT3_TOT_LOCKOUT_OPS,
--          ALT3_GRD_DELAY,
--          ALT3_PHA_DELAY,
--          ALT3_PHA_TADD_SLOW,
--          ALT3_GRD_TADD_SLOW,
--          ALT3_PHA_TADD_FAST,
--          ALT3_GRD_TADD_FAST,
--          ALT3_PHA_VMUL_SLOW,
--          ALT3_GRD_VMUL_SLOW,
--          ALT3_PHA_VMUL_FAST,
--          ALT3_GRD_SLOW_CRV_OPS,
--          ALT3_PHA_SLOW_CRV_OPS,
--          ALT3_GRD_VMUL_FAST,
--          ALT3_PHA_SLOW_CRV,
--          ALT3_GRD_SLOW_CRV,
--          ALT3_PHA_FAST_CRV,
--          ALT3_GRD_FAST_CRV,
--          ALT3_TCC2_SLOW_CURVES_USED,
--          ALT3_TCC1_FAST_CURVES_USED,
--          ALT3_PHA_OP_F_CRV,
--          ALT3_GRD_OP_F_CRV,
--          ALT3_PHA_INST_TRIP_CD,
--          ALT3_GRD_INST_TRIP_CD,
--          ALT3_PHA_MIN_TRIP,
--          ALT3_GRD_MIN_TRIP,
--          ALT2_PHA_TADD_FAST,
--          ALT2_GRD_TADD_FAST,
--          ALT2_GRD_VMUL_SLOW,
--          ALT2_PHA_VMUL_FAST,
--          ALT2_PHA_FAST_CRV,
--          ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--        )
--      SELECT EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        RELAY_TYPE,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        FEATURE_CLASS_NAME,
--        GLOBAL_ID,
--        ALT2_PHA_INST_TRIP_CD,
--        ALT2_GRD_INST_TRIP_CD,
--        ALT2_PHA_MIN_TRIP,
--        ALT2_GRD_MIN_TRIP,
--        ALT2_LS_RESET_TIME,
--        ALT2_LS_LOCKOUT_OPS,
--        ALT2_INRUSH_DURATION,
--        ALT2_PHA_INRUSH_THRESHOLD,
--        ALT2_GRD_INRUSH_THRESHOLD,
--        ALT2_PHA_ARMING_THRESHOLD,
--        ALT2_GRD_ARMING_THRESHOLD,
--        ALT2_PERMIT_LS_ENABLING,
--        ALT_COLD_LOAD_PLI_CURVE_GRD,
--        ALT_COLD_LOAD_PLI_CURVE_PHA,
--        ALT_COLD_LOAD_PLI_GRD,
--        ALT_COLD_LOAD_PLI_PHA,
--        ALT_COLD_LOAD_PLI_USED,
--        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT_HIGH_CURRENT_LOCKOUT_USED,
--        ALT_SGF_TIME_DELAY,
--        ALT_SGF_MIN_TRIP_PERCENT,
--        ALT_SGF_CD,
--        ALT_RECLOSE_RETRY_ENABLED,
--        ALT_RESET,
--        ALT_RECLOSE3_TIME,
--        ALT_RECLOSE2_TIME,
--        ALT_RECLOSE1_TIME,
--        ALT_TOT_LOCKOUT_OPS,
--        ALT_PHA_TADD_SLOW,
--        ALT_GRD_TADD_SLOW,
--        ALT_PHA_TADD_FAST,
--        ALT_GRD_TADD_FAST,
--        ALT_PHA_VMUL_SLOW,
--        ALT_GRD_VMUL_SLOW,
--        ALT_PHA_VMUL_FAST,
--        ALT_GRD_SLOW_CRV_OPS,
--        ALT_PHA_SLOW_CRV_OPS,
--        ALT_GRD_VMUL_FAST,
--        ALT_PHA_SLOW_CRV,
--        ALT_GRD_SLOW_CRV,
--        ALT_PHA_FAST_CRV,
--        ALT_GRD_FAST_CRV,
--        ALT_PHA_RESP_TIME,
--        ALT_GRD_RESP_TIME,
--        ALT_TCC2_SLOW_CURVES_USED,
--        ALT_TCC1_FAST_CURVES_USED,
--        ALT_PHA_OP_F_CRV,
--        ALT_GRD_OP_F_CRV,
--        ALT_PHA_INST_TRIP_CD,
--        ALT_GRD_INST_TRIP_CD,
--        ALT_PHA_MIN_TRIP,
--        ALT_GRD_MIN_TRIP,
--        COLD_LOAD_PLI_CURVE_GRD,
--        COLD_LOAD_PLI_CURVE_PHA,
--        COLD_LOAD_PLI_GRD,
--        COLD_LOAD_PLI_PHA,
--        COLD_LOAD_PLI_USED,
--        HIGH_CURRENT_LOCKUOUT_GRD,
--        HIGH_CURRENT_LOCKOUT_PHA,
--        HIGH_CURRENT_LOCKOUT_USED,
--        SGF_TIME_DELAY,
--        SGF_MIN_TRIP_PERCENT,
--        SGF_CD,
--        RECLOSE_RETRY_ENABLED,
--        RESET,
--        RECLOSE3_TIME,
--        RECLOSE2_TIME,
--        RECLOSE1_TIME,
--        TOT_LOCKOUT_OPS,
--        PHA_TADD_SLOW,
--        GRD_TADD_SLOW,
--        PHA_TADD_FAST,
--        GRD_TADD_FAST,
--        COLD_PHA_TMUL_FAST,
--        COLD_GRD_TMUL_FAST,
--        PHA_TMUL_SLOW,
--        GRD_TMUL_SLOW,
--        PHA_TMUL_FAST,
--        GRD_TMUL_FAST,
--        GRD_SLOW_CRV_OPS,
--        PHA_SLOW_CRV_OPS,
--        PHA_SLOW_CRV,
--        GRD_SLOW_CRV,
--        PHA_FAST_CRV,
--        GRD_FAST_CRV,
--        PHA_RESP_TIME,
--        GRD_RESP_TIME,
--        PHA_OP_F_CRV,
--        GRD_OP_F_CRV,
--        TCC2_SLOW_CURVES_USED,
--        TCC1_FAST_CURVES_USED,
--        PHA_INST_TRIP_CD,
--        GRD_INST_TRIP_CD,
--        PHA_MIN_TRIP,
--        GRD_MIN_TRIP,
--        OPERATING_AS_CD,
--        BYPASS_PLANS,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        CURRENT_FUTURE,
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--       FLISR_ENGINEERING_COMMENTS,
--        "Limiting_Factor",
--        "Winter_Load_Limit",
--        "Summer_Load_Limit",
--        FLISR,
--        ENGINEERING_COMMENTS,
--        ACTIVE_PROFILE,
--        ALT3_COLD_LOAD_PLI_CURVE_GRD,
--        ALT3_COLD_LOAD_PLI_CURVE_PHA,
--        ALT3_COLD_LOAD_PLI_GRD,
--        ALT3_COLD_LOAD_PLI_PHA,
--        ALT3_COLD_LOAD_PLI_USED,
--        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT3_HIGH_CURRENT_LOCKOUT_USED,
--        ALT3_SGF_TIME_DELAY,
--        ALT3_SGF_MIN_TRIP_PERCENT,
--        ALT3_SGF_CD,
--        ALT3_RECLOSE_RETRY_ENABLED,
--        ALT3_RESET,
--        ALT3_RECLOSE3_TIME,
--        ALT3_RECLOSE2_TIME,
--        ALT3_RECLOSE1_TIME,
--        ALT3_TOT_LOCKOUT_OPS,
--        ALT3_GRD_DELAY,
--        ALT3_PHA_DELAY,
--        ALT3_PHA_TADD_SLOW,
--        ALT3_GRD_TADD_SLOW,
--        ALT3_PHA_TADD_FAST,
--        ALT3_GRD_TADD_FAST,
--        ALT3_PHA_VMUL_SLOW,
--        ALT3_GRD_VMUL_SLOW,
--        ALT3_PHA_VMUL_FAST,
--        ALT3_GRD_SLOW_CRV_OPS,
--        ALT3_PHA_SLOW_CRV_OPS,
--        ALT3_GRD_VMUL_FAST,
--        ALT3_PHA_SLOW_CRV,
--        ALT3_GRD_SLOW_CRV,
--        ALT3_PHA_FAST_CRV,
--        ALT3_GRD_FAST_CRV,
--        ALT3_TCC2_SLOW_CURVES_USED,
--        ALT3_TCC1_FAST_CURVES_USED,
--        ALT3_PHA_OP_F_CRV,
--        ALT3_GRD_OP_F_CRV,
--        ALT3_PHA_INST_TRIP_CD,
--        ALT3_GRD_INST_TRIP_CD,
--        ALT3_PHA_MIN_TRIP,
--        ALT3_GRD_MIN_TRIP,
--        ALT2_PHA_TADD_FAST,
--        ALT2_GRD_TADD_FAST,
--        ALT2_GRD_VMUL_SLOW,
--        ALT2_PHA_VMUL_FAST,
--        ALT2_PHA_FAST_CRV,
--        ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_HIST',
--          SM_RECLOSER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      UPDATE SM_RECLOSER
--      SET OPERATING_NUM = I_operating_num,
--        DIVISION        =I_Division,
--        DISTRICT        = I_District,
--        CONTROL_TYPE    =I_Control_type_code
--      WHERE GLOBAL_ID   = I_Global_id_Current;
--
--       --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--  IF ACTION= 'D' THEN
--    -- first copy the entire current record to history table
--    INSERT
--    INTO SM_RECLOSER_HIST
--      (
--        EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        RELAY_TYPE,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        FEATURE_CLASS_NAME,
--        GLOBAL_ID,
--        ALT2_PHA_INST_TRIP_CD,
--        ALT2_GRD_INST_TRIP_CD,
--        ALT2_PHA_MIN_TRIP,
--        ALT2_GRD_MIN_TRIP,
--        ALT2_LS_RESET_TIME,
--        ALT2_LS_LOCKOUT_OPS,
--        ALT2_INRUSH_DURATION,
--        ALT2_PHA_INRUSH_THRESHOLD,
--        ALT2_GRD_INRUSH_THRESHOLD,
--        ALT2_PHA_ARMING_THRESHOLD,
--        ALT2_GRD_ARMING_THRESHOLD,
--        ALT2_PERMIT_LS_ENABLING,
--        ALT_COLD_LOAD_PLI_CURVE_GRD,
--        ALT_COLD_LOAD_PLI_CURVE_PHA,
--        ALT_COLD_LOAD_PLI_GRD,
--        ALT_COLD_LOAD_PLI_PHA,
--        ALT_COLD_LOAD_PLI_USED,
--        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT_HIGH_CURRENT_LOCKOUT_USED,
--        ALT_SGF_TIME_DELAY,
--        ALT_SGF_MIN_TRIP_PERCENT,
--        ALT_SGF_CD,
--        ALT_RECLOSE_RETRY_ENABLED,
--        ALT_RESET,
--        ALT_RECLOSE3_TIME,
--        ALT_RECLOSE2_TIME,
--        ALT_RECLOSE1_TIME,
--        ALT_TOT_LOCKOUT_OPS,
--        ALT_PHA_TADD_SLOW,
--        ALT_GRD_TADD_SLOW,
--        ALT_PHA_TADD_FAST,
--        ALT_GRD_TADD_FAST,
--        ALT_PHA_VMUL_SLOW,
--        ALT_GRD_VMUL_SLOW,
--        ALT_PHA_VMUL_FAST,
--        ALT_GRD_SLOW_CRV_OPS,
--        ALT_PHA_SLOW_CRV_OPS,
--        ALT_GRD_VMUL_FAST,
--        ALT_PHA_SLOW_CRV,
--        ALT_GRD_SLOW_CRV,
--        ALT_PHA_FAST_CRV,
--        ALT_GRD_FAST_CRV,
--        ALT_PHA_RESP_TIME,
--        ALT_GRD_RESP_TIME,
--        ALT_TCC2_SLOW_CURVES_USED,
--        ALT_TCC1_FAST_CURVES_USED,
--        ALT_PHA_OP_F_CRV,
--        ALT_GRD_OP_F_CRV,
--        ALT_PHA_INST_TRIP_CD,
--        ALT_GRD_INST_TRIP_CD,
--        ALT_PHA_MIN_TRIP,
--        ALT_GRD_MIN_TRIP,
--        COLD_LOAD_PLI_CURVE_GRD,
--        COLD_LOAD_PLI_CURVE_PHA,
--        COLD_LOAD_PLI_GRD,
--        COLD_LOAD_PLI_PHA,
--        COLD_LOAD_PLI_USED,
--        HIGH_CURRENT_LOCKUOUT_GRD,
--        HIGH_CURRENT_LOCKOUT_PHA,
--        HIGH_CURRENT_LOCKOUT_USED,
--        SGF_TIME_DELAY,
--        SGF_MIN_TRIP_PERCENT,
--        SGF_CD,
--        RECLOSE_RETRY_ENABLED,
--        RESET,
--        RECLOSE3_TIME,
--        RECLOSE2_TIME,
--        RECLOSE1_TIME,
--        TOT_LOCKOUT_OPS,
--        PHA_TADD_SLOW,
--        GRD_TADD_SLOW,
--        PHA_TADD_FAST,
--        GRD_TADD_FAST,
--        COLD_PHA_TMUL_FAST,
--        COLD_GRD_TMUL_FAST,
--        PHA_TMUL_SLOW,
--        GRD_TMUL_SLOW,
--        PHA_TMUL_FAST,
--        GRD_TMUL_FAST,
--        GRD_SLOW_CRV_OPS,
--        PHA_SLOW_CRV_OPS,
--        PHA_SLOW_CRV,
--        GRD_SLOW_CRV,
--        PHA_FAST_CRV,
--        GRD_FAST_CRV,
--        PHA_RESP_TIME,
--        GRD_RESP_TIME,
--        PHA_OP_F_CRV,
--        GRD_OP_F_CRV,
--        TCC2_SLOW_CURVES_USED,
--        TCC1_FAST_CURVES_USED,
--        PHA_INST_TRIP_CD,
--        GRD_INST_TRIP_CD,
--        PHA_MIN_TRIP,
--        GRD_MIN_TRIP,
--        OPERATING_AS_CD,
--        BYPASS_PLANS,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        CURRENT_FUTURE,
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--        FLISR_ENGINEERING_COMMENTS,
--        "Limiting_Factor",
--        "Winter_Load_Limit",
--        "Summer_Load_Limit",
--        FLISR,
--        ENGINEERING_COMMENTS,
--        ACTIVE_PROFILE,
--        ALT3_COLD_LOAD_PLI_CURVE_GRD,
--        ALT3_COLD_LOAD_PLI_CURVE_PHA,
--        ALT3_COLD_LOAD_PLI_GRD,
--        ALT3_COLD_LOAD_PLI_PHA,
--        ALT3_COLD_LOAD_PLI_USED,
--        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT3_HIGH_CURRENT_LOCKOUT_USED,
--        ALT3_SGF_TIME_DELAY,
--        ALT3_SGF_MIN_TRIP_PERCENT,
--        ALT3_SGF_CD,
--        ALT3_RECLOSE_RETRY_ENABLED,
--        ALT3_RESET,
--        ALT3_RECLOSE3_TIME,
--        ALT3_RECLOSE2_TIME,
--        ALT3_RECLOSE1_TIME,
--        ALT3_TOT_LOCKOUT_OPS,
--        ALT3_GRD_DELAY,
--        ALT3_PHA_DELAY,
--        ALT3_PHA_TADD_SLOW,
--        ALT3_GRD_TADD_SLOW,
--        ALT3_PHA_TADD_FAST,
--        ALT3_GRD_TADD_FAST,
--        ALT3_PHA_VMUL_SLOW,
--        ALT3_GRD_VMUL_SLOW,
--        ALT3_PHA_VMUL_FAST,
--        ALT3_GRD_SLOW_CRV_OPS,
--        ALT3_PHA_SLOW_CRV_OPS,
--        ALT3_GRD_VMUL_FAST,
--        ALT3_PHA_SLOW_CRV,
--        ALT3_GRD_SLOW_CRV,
--        ALT3_PHA_FAST_CRV,
--        ALT3_GRD_FAST_CRV,
--        ALT3_TCC2_SLOW_CURVES_USED,
--        ALT3_TCC1_FAST_CURVES_USED,
--        ALT3_PHA_OP_F_CRV,
--        ALT3_GRD_OP_F_CRV,
--        ALT3_PHA_INST_TRIP_CD,
--        ALT3_GRD_INST_TRIP_CD,
--        ALT3_PHA_MIN_TRIP,
--        ALT3_GRD_MIN_TRIP,
--        ALT2_PHA_TADD_FAST,
--        ALT2_GRD_TADD_FAST,
--        ALT2_GRD_VMUL_SLOW,
--        ALT2_PHA_VMUL_FAST,
--        ALT2_PHA_FAST_CRV,
--        ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--      )
--    SELECT EFFECTIVE_DT,
--      TIMESTAMP,
--      DATE_MODIFIED,
--      RELAY_TYPE,
--      PREPARED_BY,
--      DEVICE_ID,
--      OPERATING_NUM,
--      FEATURE_CLASS_NAME,
--      GLOBAL_ID,
--      ALT2_PHA_INST_TRIP_CD,
--      ALT2_GRD_INST_TRIP_CD,
--      ALT2_PHA_MIN_TRIP,
--      ALT2_GRD_MIN_TRIP,
--      ALT2_LS_RESET_TIME,
--      ALT2_LS_LOCKOUT_OPS,
--      ALT2_INRUSH_DURATION,
--      ALT2_PHA_INRUSH_THRESHOLD,
--      ALT2_GRD_INRUSH_THRESHOLD,
--      ALT2_PHA_ARMING_THRESHOLD,
--      ALT2_GRD_ARMING_THRESHOLD,
--      ALT2_PERMIT_LS_ENABLING,
--      ALT_COLD_LOAD_PLI_CURVE_GRD,
--      ALT_COLD_LOAD_PLI_CURVE_PHA,
--      ALT_COLD_LOAD_PLI_GRD,
--      ALT_COLD_LOAD_PLI_PHA,
--      ALT_COLD_LOAD_PLI_USED,
--      ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--      ALT_HIGH_CURRENT_LOCKOUT_PHA,
--      ALT_HIGH_CURRENT_LOCKOUT_USED,
--      ALT_SGF_TIME_DELAY,
--      ALT_SGF_MIN_TRIP_PERCENT,
--      ALT_SGF_CD,
--      ALT_RECLOSE_RETRY_ENABLED,
--      ALT_RESET,
--      ALT_RECLOSE3_TIME,
--      ALT_RECLOSE2_TIME,
--      ALT_RECLOSE1_TIME,
--      ALT_TOT_LOCKOUT_OPS,
--      ALT_PHA_TADD_SLOW,
--      ALT_GRD_TADD_SLOW,
--      ALT_PHA_TADD_FAST,
--      ALT_GRD_TADD_FAST,
--      ALT_PHA_VMUL_SLOW,
--      ALT_GRD_VMUL_SLOW,
--      ALT_PHA_VMUL_FAST,
--      ALT_GRD_SLOW_CRV_OPS,
--      ALT_PHA_SLOW_CRV_OPS,
--      ALT_GRD_VMUL_FAST,
--      ALT_PHA_SLOW_CRV,
--      ALT_GRD_SLOW_CRV,
--      ALT_PHA_FAST_CRV,
--      ALT_GRD_FAST_CRV,
--      ALT_PHA_RESP_TIME,
--      ALT_GRD_RESP_TIME,
--      ALT_TCC2_SLOW_CURVES_USED,
--      ALT_TCC1_FAST_CURVES_USED,
--      ALT_PHA_OP_F_CRV,
--      ALT_GRD_OP_F_CRV,
--      ALT_PHA_INST_TRIP_CD,
--      ALT_GRD_INST_TRIP_CD,
--      ALT_PHA_MIN_TRIP,
--      ALT_GRD_MIN_TRIP,
--      COLD_LOAD_PLI_CURVE_GRD,
--      COLD_LOAD_PLI_CURVE_PHA,
--      COLD_LOAD_PLI_GRD,
--      COLD_LOAD_PLI_PHA,
--      COLD_LOAD_PLI_USED,
--      HIGH_CURRENT_LOCKUOUT_GRD,
--      HIGH_CURRENT_LOCKOUT_PHA,
--      HIGH_CURRENT_LOCKOUT_USED,
--      SGF_TIME_DELAY,
--      SGF_MIN_TRIP_PERCENT,
--      SGF_CD,
--      RECLOSE_RETRY_ENABLED,
--      RESET,
--      RECLOSE3_TIME,
--      RECLOSE2_TIME,
--      RECLOSE1_TIME,
--      TOT_LOCKOUT_OPS,
--      PHA_TADD_SLOW,
--      GRD_TADD_SLOW,
--      PHA_TADD_FAST,
--      GRD_TADD_FAST,
--      COLD_PHA_TMUL_FAST,
--      COLD_GRD_TMUL_FAST,
--      PHA_TMUL_SLOW,
--      GRD_TMUL_SLOW,
--      PHA_TMUL_FAST,
--      GRD_TMUL_FAST,
--      GRD_SLOW_CRV_OPS,
--      PHA_SLOW_CRV_OPS,
--      PHA_SLOW_CRV,
--      GRD_SLOW_CRV,
--      PHA_FAST_CRV,
--      GRD_FAST_CRV,
--      PHA_RESP_TIME,
--      GRD_RESP_TIME,
--      PHA_OP_F_CRV,
--      GRD_OP_F_CRV,
--      TCC2_SLOW_CURVES_USED,
--      TCC1_FAST_CURVES_USED,
--      PHA_INST_TRIP_CD,
--      GRD_INST_TRIP_CD,
--      PHA_MIN_TRIP,
--      GRD_MIN_TRIP,
--      OPERATING_AS_CD,
--      BYPASS_PLANS,
--      CONTROL_TYPE,
--      OK_TO_BYPASS,
--      RELEASED_BY,
--      PROCESSED_FLAG,
--      CONTROL_SERIAL_NUM,
--      SOFTWARE_VERSION,
--      FIRMWARE_VERSION,
--      CURRENT_FUTURE,
--      DISTRICT,
--      DIVISION,
--      PEER_REVIEW_BY,
--      PEER_REVIEW_DT,
--      RADIO_SERIAL_NUM,
--      RADIO_MODEL_NUM,
--      RADIO_MANF_CD,
--      SPECIAL_CONDITIONS,
--      REPEATER,
--      RTU_ADDRESS,
--      TRANSMIT_DISABLE_DELAY,
--      TRANSMIT_ENABLE_DELAY,
--      BAUD_RATE,
--      MASTER_STATION,
--      SCADA_TYPE,
--      SCADA,
--      FLISR_ENGINEERING_COMMENTS,
--      "Limiting_Factor",
--      "Winter_Load_Limit",
--      "Summer_Load_Limit",
--      FLISR,
--      ENGINEERING_COMMENTS,
--      ACTIVE_PROFILE,
--      ALT3_COLD_LOAD_PLI_CURVE_GRD,
--      ALT3_COLD_LOAD_PLI_CURVE_PHA,
--      ALT3_COLD_LOAD_PLI_GRD,
--      ALT3_COLD_LOAD_PLI_PHA,
--      ALT3_COLD_LOAD_PLI_USED,
--      ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--      ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--      ALT3_HIGH_CURRENT_LOCKOUT_USED,
--      ALT3_SGF_TIME_DELAY,
--      ALT3_SGF_MIN_TRIP_PERCENT,
--      ALT3_SGF_CD,
--      ALT3_RECLOSE_RETRY_ENABLED,
--      ALT3_RESET,
--      ALT3_RECLOSE3_TIME,
--      ALT3_RECLOSE2_TIME,
--      ALT3_RECLOSE1_TIME,
--      ALT3_TOT_LOCKOUT_OPS,
--      ALT3_GRD_DELAY,
--      ALT3_PHA_DELAY,
--      ALT3_PHA_TADD_SLOW,
--      ALT3_GRD_TADD_SLOW,
--      ALT3_PHA_TADD_FAST,
--      ALT3_GRD_TADD_FAST,
--      ALT3_PHA_VMUL_SLOW,
--      ALT3_GRD_VMUL_SLOW,
--      ALT3_PHA_VMUL_FAST,
--      ALT3_GRD_SLOW_CRV_OPS,
--      ALT3_PHA_SLOW_CRV_OPS,
--      ALT3_GRD_VMUL_FAST,
--      ALT3_PHA_SLOW_CRV,
--      ALT3_GRD_SLOW_CRV,
--      ALT3_PHA_FAST_CRV,
--      ALT3_GRD_FAST_CRV,
--      ALT3_TCC2_SLOW_CURVES_USED,
--      ALT3_TCC1_FAST_CURVES_USED,
--      ALT3_PHA_OP_F_CRV,
--      ALT3_GRD_OP_F_CRV,
--      ALT3_PHA_INST_TRIP_CD,
--      ALT3_GRD_INST_TRIP_CD,
--      ALT3_PHA_MIN_TRIP,
--      ALT3_GRD_MIN_TRIP,
--      ALT2_PHA_TADD_FAST,
--      ALT2_GRD_TADD_FAST,
--      ALT2_GRD_VMUL_SLOW,
--      ALT2_PHA_VMUL_FAST,
--      ALT2_PHA_FAST_CRV,
--      ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--      RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--    FROM SM_RECLOSER
--    WHERE GLOBAL_ID=I_Global_id_Current;
--    DELETE FROM SM_RECLOSER WHERE GLOBAL_ID = I_Global_id_Current;
--    -- Insert a record in comments table with notes set to INST
--    -- Insert a record in comments table with notes set to INST
--    INSERT
--    INTO SM_COMMENT_HIST
--      (
--        DEVICE_HIST_TABLE_NAME,
--        HIST_ID,
--        WORK_DATE,
--        WORK_TYPE,
--        PERFORMED_BY,
--        ENTRY_DATE,
--        COMMENTS
--      )
--      VALUES
--      (
--        'SM_RECLOSER_HIST',
--        SM_RECLOSER_HIST_SEQ.NEXTVAL,
--        sysdate,
--        'OTHR',
--        'SYSTEM',
--        sysdate,
--        'Record updated in GIS system'
--      );
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--  IF ACTION = 'R' THEN
--    BEGIN
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_RECLOSER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'C'
--        );
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_RECLOSER
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to OTHR
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER',
--          I_Global_id_Current,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--         'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_RECLOSER_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--      UPDATE SM_RECLOSER
--      SET
--        (
--          RELAY_TYPE,
--          ALT2_PHA_INST_TRIP_CD,
--          ALT2_GRD_INST_TRIP_CD,
--          ALT2_PHA_MIN_TRIP,
--          ALT2_GRD_MIN_TRIP,
--          ALT2_LS_RESET_TIME,
--          ALT2_LS_LOCKOUT_OPS,
--          ALT2_INRUSH_DURATION,
--          ALT2_PHA_INRUSH_THRESHOLD,
--          ALT2_GRD_INRUSH_THRESHOLD,
--          ALT2_PHA_ARMING_THRESHOLD,
--          ALT2_GRD_ARMING_THRESHOLD,
--          ALT2_PERMIT_LS_ENABLING,
--          ALT_COLD_LOAD_PLI_CURVE_GRD,
--          ALT_COLD_LOAD_PLI_CURVE_PHA,
--          ALT_COLD_LOAD_PLI_GRD,
--          ALT_COLD_LOAD_PLI_PHA,
--          ALT_COLD_LOAD_PLI_USED,
--          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT_HIGH_CURRENT_LOCKOUT_USED,
--          ALT_SGF_TIME_DELAY,
--          ALT_SGF_MIN_TRIP_PERCENT,
--          ALT_SGF_CD,
--          ALT_RECLOSE_RETRY_ENABLED,
--          ALT_RESET,
--          ALT_RECLOSE3_TIME,
--          ALT_RECLOSE2_TIME,
--          ALT_RECLOSE1_TIME,
--          ALT_TOT_LOCKOUT_OPS,
--          ALT_PHA_TADD_SLOW,
--          ALT_GRD_TADD_SLOW,
--          ALT_PHA_TADD_FAST,
--          ALT_GRD_TADD_FAST,
--          ALT_PHA_VMUL_SLOW,
--          ALT_GRD_VMUL_SLOW,
--          ALT_PHA_VMUL_FAST,
--          ALT_GRD_SLOW_CRV_OPS,
--          ALT_PHA_SLOW_CRV_OPS,
--          ALT_GRD_VMUL_FAST,
--          ALT_PHA_SLOW_CRV,
--          ALT_GRD_SLOW_CRV,
--          ALT_PHA_FAST_CRV,
--          ALT_GRD_FAST_CRV,
--          ALT_PHA_RESP_TIME,
--          ALT_GRD_RESP_TIME,
--          ALT_TCC2_SLOW_CURVES_USED,
--          ALT_TCC1_FAST_CURVES_USED,
--          ALT_PHA_OP_F_CRV,
--          ALT_GRD_OP_F_CRV,
--          ALT_PHA_INST_TRIP_CD,
--          ALT_GRD_INST_TRIP_CD,
--          ALT_PHA_MIN_TRIP,
--          ALT_GRD_MIN_TRIP,
--          COLD_LOAD_PLI_CURVE_GRD,
--          COLD_LOAD_PLI_CURVE_PHA,
--          COLD_LOAD_PLI_GRD,
--          COLD_LOAD_PLI_PHA,
--          COLD_LOAD_PLI_USED,
--          HIGH_CURRENT_LOCKUOUT_GRD,
--          HIGH_CURRENT_LOCKOUT_PHA,
--          HIGH_CURRENT_LOCKOUT_USED,
--          SGF_TIME_DELAY,
--          SGF_MIN_TRIP_PERCENT,
--          SGF_CD,
--          RECLOSE_RETRY_ENABLED,
--          RESET,
--          RECLOSE3_TIME,
--          RECLOSE2_TIME,
--          RECLOSE1_TIME,
--          TOT_LOCKOUT_OPS,
--          PHA_TADD_SLOW,
--          GRD_TADD_SLOW,
--          PHA_TADD_FAST,
--          GRD_TADD_FAST,
--          COLD_PHA_TMUL_FAST,
--          COLD_GRD_TMUL_FAST,
--          PHA_TMUL_SLOW,
--          GRD_TMUL_SLOW,
--          PHA_TMUL_FAST,
--          GRD_TMUL_FAST,
--          GRD_SLOW_CRV_OPS,
--          PHA_SLOW_CRV_OPS,
--          PHA_SLOW_CRV,
--          GRD_SLOW_CRV,
--          PHA_FAST_CRV,
--          GRD_FAST_CRV,
--          PHA_RESP_TIME,
--          GRD_RESP_TIME,
--          PHA_OP_F_CRV,
--          GRD_OP_F_CRV,
--          TCC2_SLOW_CURVES_USED,
--          TCC1_FAST_CURVES_USED,
--          PHA_INST_TRIP_CD,
--          GRD_INST_TRIP_CD,
--          PHA_MIN_TRIP,
--          GRD_MIN_TRIP,
--          OPERATING_AS_CD,
--          BYPASS_PLANS,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          FLISR_ENGINEERING_COMMENTS,
--          "Limiting_Factor",
--          "Winter_Load_Limit",
--          "Summer_Load_Limit",
--          FLISR,
--          ENGINEERING_COMMENTS,
--          ACTIVE_PROFILE,
--          ALT3_COLD_LOAD_PLI_CURVE_GRD,
--          ALT3_COLD_LOAD_PLI_CURVE_PHA,
--          ALT3_COLD_LOAD_PLI_GRD,
--          ALT3_COLD_LOAD_PLI_PHA,
--          ALT3_COLD_LOAD_PLI_USED,
--          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT3_HIGH_CURRENT_LOCKOUT_USED,
--          ALT3_SGF_TIME_DELAY,
--          ALT3_SGF_MIN_TRIP_PERCENT,
--          ALT3_SGF_CD,
--          ALT3_RECLOSE_RETRY_ENABLED,
--          ALT3_RESET,
--          ALT3_RECLOSE3_TIME,
--          ALT3_RECLOSE2_TIME,
--          ALT3_RECLOSE1_TIME,
--          ALT3_TOT_LOCKOUT_OPS,
--          ALT3_GRD_DELAY,
--          ALT3_PHA_DELAY,
--          ALT3_PHA_TADD_SLOW,
--          ALT3_GRD_TADD_SLOW,
--          ALT3_PHA_TADD_FAST,
--          ALT3_GRD_TADD_FAST,
--          ALT3_PHA_VMUL_SLOW,
--          ALT3_GRD_VMUL_SLOW,
--          ALT3_PHA_VMUL_FAST,
--          ALT3_GRD_SLOW_CRV_OPS,
--          ALT3_PHA_SLOW_CRV_OPS,
--          ALT3_GRD_VMUL_FAST,
--          ALT3_PHA_SLOW_CRV,
--          ALT3_GRD_SLOW_CRV,
--          ALT3_PHA_FAST_CRV,
--          ALT3_GRD_FAST_CRV,
--          ALT3_TCC2_SLOW_CURVES_USED,
--          ALT3_TCC1_FAST_CURVES_USED,
--          ALT3_PHA_OP_F_CRV,
--          ALT3_GRD_OP_F_CRV,
--          ALT3_PHA_INST_TRIP_CD,
--          ALT3_GRD_INST_TRIP_CD,
--          ALT3_PHA_MIN_TRIP,
--          ALT3_GRD_MIN_TRIP,
--          ALT2_PHA_TADD_FAST,
--          ALT2_GRD_TADD_FAST,
--          ALT2_GRD_VMUL_SLOW,
--          ALT2_PHA_VMUL_FAST,
--          ALT2_PHA_FAST_CRV,
--          ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        )
--        =
--        (SELECT RELAY_TYPE,
--          ALT2_PHA_INST_TRIP_CD,
--          ALT2_GRD_INST_TRIP_CD,
--          ALT2_PHA_MIN_TRIP,
--          ALT2_GRD_MIN_TRIP,
--          ALT2_LS_RESET_TIME,
--          ALT2_LS_LOCKOUT_OPS,
--          ALT2_INRUSH_DURATION,
--          ALT2_PHA_INRUSH_THRESHOLD,
--          ALT2_GRD_INRUSH_THRESHOLD,
--          ALT2_PHA_ARMING_THRESHOLD,
--          ALT2_GRD_ARMING_THRESHOLD,
--          ALT2_PERMIT_LS_ENABLING,
--          ALT_COLD_LOAD_PLI_CURVE_GRD,
--          ALT_COLD_LOAD_PLI_CURVE_PHA,
--          ALT_COLD_LOAD_PLI_GRD,
--          ALT_COLD_LOAD_PLI_PHA,
--          ALT_COLD_LOAD_PLI_USED,
--          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT_HIGH_CURRENT_LOCKOUT_USED,
--          ALT_SGF_TIME_DELAY,
--          ALT_SGF_MIN_TRIP_PERCENT,
--          ALT_SGF_CD,
--          ALT_RECLOSE_RETRY_ENABLED,
--          ALT_RESET,
--          ALT_RECLOSE3_TIME,
--          ALT_RECLOSE2_TIME,
--          ALT_RECLOSE1_TIME,
--          ALT_TOT_LOCKOUT_OPS,
--          ALT_PHA_TADD_SLOW,
--          ALT_GRD_TADD_SLOW,
--          ALT_PHA_TADD_FAST,
--          ALT_GRD_TADD_FAST,
--          ALT_PHA_VMUL_SLOW,
--          ALT_GRD_VMUL_SLOW,
--          ALT_PHA_VMUL_FAST,
--          ALT_GRD_SLOW_CRV_OPS,
--          ALT_PHA_SLOW_CRV_OPS,
--          ALT_GRD_VMUL_FAST,
--          ALT_PHA_SLOW_CRV,
--          ALT_GRD_SLOW_CRV,
--          ALT_PHA_FAST_CRV,
--          ALT_GRD_FAST_CRV,
--          ALT_PHA_RESP_TIME,
--          ALT_GRD_RESP_TIME,
--          ALT_TCC2_SLOW_CURVES_USED,
--          ALT_TCC1_FAST_CURVES_USED,
--          ALT_PHA_OP_F_CRV,
--          ALT_GRD_OP_F_CRV,
--          ALT_PHA_INST_TRIP_CD,
--          ALT_GRD_INST_TRIP_CD,
--          ALT_PHA_MIN_TRIP,
--          ALT_GRD_MIN_TRIP,
--          COLD_LOAD_PLI_CURVE_GRD,
--          COLD_LOAD_PLI_CURVE_PHA,
--          COLD_LOAD_PLI_GRD,
--          COLD_LOAD_PLI_PHA,
--          COLD_LOAD_PLI_USED,
--          HIGH_CURRENT_LOCKUOUT_GRD,
--          HIGH_CURRENT_LOCKOUT_PHA,
--          HIGH_CURRENT_LOCKOUT_USED,
--          SGF_TIME_DELAY,
--          SGF_MIN_TRIP_PERCENT,
--          SGF_CD,
--          RECLOSE_RETRY_ENABLED,
--          RESET,
--          RECLOSE3_TIME,
--          RECLOSE2_TIME,
--          RECLOSE1_TIME,
--          TOT_LOCKOUT_OPS,
--          PHA_TADD_SLOW,
--          GRD_TADD_SLOW,
--          PHA_TADD_FAST,
--          GRD_TADD_FAST,
--          COLD_PHA_TMUL_FAST,
--          COLD_GRD_TMUL_FAST,
--          PHA_TMUL_SLOW,
--          GRD_TMUL_SLOW,
--          PHA_TMUL_FAST,
--          GRD_TMUL_FAST,
--          GRD_SLOW_CRV_OPS,
--          PHA_SLOW_CRV_OPS,
--          PHA_SLOW_CRV,
--          GRD_SLOW_CRV,
--          PHA_FAST_CRV,
--          GRD_FAST_CRV,
--          PHA_RESP_TIME,
--          GRD_RESP_TIME,
--          PHA_OP_F_CRV,
--          GRD_OP_F_CRV,
--          TCC2_SLOW_CURVES_USED,
--          TCC1_FAST_CURVES_USED,
--          PHA_INST_TRIP_CD,
--          GRD_INST_TRIP_CD,
--          PHA_MIN_TRIP,
--          GRD_MIN_TRIP,
--          OPERATING_AS_CD,
--          BYPASS_PLANS,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          FLISR_ENGINEERING_COMMENTS,
--          "Limiting_Factor",
--          "Winter_Load_Limit",
--          "Summer_Load_Limit",
--          FLISR,
--          ENGINEERING_COMMENTS,
--          ACTIVE_PROFILE,
--          ALT3_COLD_LOAD_PLI_CURVE_GRD,
--          ALT3_COLD_LOAD_PLI_CURVE_PHA,
--          ALT3_COLD_LOAD_PLI_GRD,
--          ALT3_COLD_LOAD_PLI_PHA,
--          ALT3_COLD_LOAD_PLI_USED,
--          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT3_HIGH_CURRENT_LOCKOUT_USED,
--          ALT3_SGF_TIME_DELAY,
--          ALT3_SGF_MIN_TRIP_PERCENT,
--          ALT3_SGF_CD,
--          ALT3_RECLOSE_RETRY_ENABLED,
--          ALT3_RESET,
--          ALT3_RECLOSE3_TIME,
--          ALT3_RECLOSE2_TIME,
--          ALT3_RECLOSE1_TIME,
--          ALT3_TOT_LOCKOUT_OPS,
--          ALT3_GRD_DELAY,
--          ALT3_PHA_DELAY,
--          ALT3_PHA_TADD_SLOW,
--          ALT3_GRD_TADD_SLOW,
--          ALT3_PHA_TADD_FAST,
--          ALT3_GRD_TADD_FAST,
--          ALT3_PHA_VMUL_SLOW,
--          ALT3_GRD_VMUL_SLOW,
--          ALT3_PHA_VMUL_FAST,
--          ALT3_GRD_SLOW_CRV_OPS,
--          ALT3_PHA_SLOW_CRV_OPS,
--          ALT3_GRD_VMUL_FAST,
--          ALT3_PHA_SLOW_CRV,
--          ALT3_GRD_SLOW_CRV,
--          ALT3_PHA_FAST_CRV,
--          ALT3_GRD_FAST_CRV,
--          ALT3_TCC2_SLOW_CURVES_USED,
--          ALT3_TCC1_FAST_CURVES_USED,
--          ALT3_PHA_OP_F_CRV,
--          ALT3_GRD_OP_F_CRV,
--          ALT3_PHA_INST_TRIP_CD,
--          ALT3_GRD_INST_TRIP_CD,
--          ALT3_PHA_MIN_TRIP,
--          ALT3_GRD_MIN_TRIP,
--          ALT2_PHA_TADD_FAST,
--          ALT2_GRD_TADD_FAST,
--          ALT2_GRD_VMUL_SLOW,
--          ALT2_PHA_VMUL_FAST,
--          ALT2_PHA_FAST_CRV,
--          ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--        FROM SM_RECLOSER
--        WHERE GLOBAL_ID   = I_Global_id_Previous
--        AND CURRENT_FUTURE='C'
--        )
--      WHERE GLOBAL_ID = I_Global_id_Current;
--      INSERT
--      INTO SM_RECLOSER_HIST
--        (
--          EFFECTIVE_DT,
--          TIMESTAMP,
--          DATE_MODIFIED,
--          RELAY_TYPE,
--          PREPARED_BY,
--          DEVICE_ID,
--          OPERATING_NUM,
--          FEATURE_CLASS_NAME,
--          GLOBAL_ID,
--          ALT2_PHA_INST_TRIP_CD,
--          ALT2_GRD_INST_TRIP_CD,
--          ALT2_PHA_MIN_TRIP,
--          ALT2_GRD_MIN_TRIP,
--          ALT2_LS_RESET_TIME,
--          ALT2_LS_LOCKOUT_OPS,
--          ALT2_INRUSH_DURATION,
--          ALT2_PHA_INRUSH_THRESHOLD,
--          ALT2_GRD_INRUSH_THRESHOLD,
--          ALT2_PHA_ARMING_THRESHOLD,
--          ALT2_GRD_ARMING_THRESHOLD,
--          ALT2_PERMIT_LS_ENABLING,
--          ALT_COLD_LOAD_PLI_CURVE_GRD,
--          ALT_COLD_LOAD_PLI_CURVE_PHA,
--          ALT_COLD_LOAD_PLI_GRD,
--          ALT_COLD_LOAD_PLI_PHA,
--          ALT_COLD_LOAD_PLI_USED,
--          ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT_HIGH_CURRENT_LOCKOUT_USED,
--          ALT_SGF_TIME_DELAY,
--          ALT_SGF_MIN_TRIP_PERCENT,
--          ALT_SGF_CD,
--          ALT_RECLOSE_RETRY_ENABLED,
--          ALT_RESET,
--          ALT_RECLOSE3_TIME,
--          ALT_RECLOSE2_TIME,
--          ALT_RECLOSE1_TIME,
--          ALT_TOT_LOCKOUT_OPS,
--          ALT_PHA_TADD_SLOW,
--          ALT_GRD_TADD_SLOW,
--          ALT_PHA_TADD_FAST,
--          ALT_GRD_TADD_FAST,
--          ALT_PHA_VMUL_SLOW,
--          ALT_GRD_VMUL_SLOW,
--          ALT_PHA_VMUL_FAST,
--          ALT_GRD_SLOW_CRV_OPS,
--          ALT_PHA_SLOW_CRV_OPS,
--          ALT_GRD_VMUL_FAST,
--          ALT_PHA_SLOW_CRV,
--          ALT_GRD_SLOW_CRV,
--          ALT_PHA_FAST_CRV,
--          ALT_GRD_FAST_CRV,
--          ALT_PHA_RESP_TIME,
--          ALT_GRD_RESP_TIME,
--          ALT_TCC2_SLOW_CURVES_USED,
--          ALT_TCC1_FAST_CURVES_USED,
--          ALT_PHA_OP_F_CRV,
--          ALT_GRD_OP_F_CRV,
--          ALT_PHA_INST_TRIP_CD,
--          ALT_GRD_INST_TRIP_CD,
--          ALT_PHA_MIN_TRIP,
--          ALT_GRD_MIN_TRIP,
--          COLD_LOAD_PLI_CURVE_GRD,
--          COLD_LOAD_PLI_CURVE_PHA,
--          COLD_LOAD_PLI_GRD,
--          COLD_LOAD_PLI_PHA,
--          COLD_LOAD_PLI_USED,
--          HIGH_CURRENT_LOCKUOUT_GRD,
--          HIGH_CURRENT_LOCKOUT_PHA,
--          HIGH_CURRENT_LOCKOUT_USED,
--          SGF_TIME_DELAY,
--          SGF_MIN_TRIP_PERCENT,
--          SGF_CD,
--          RECLOSE_RETRY_ENABLED,
--          RESET,
--          RECLOSE3_TIME,
--          RECLOSE2_TIME,
--          RECLOSE1_TIME,
--          TOT_LOCKOUT_OPS,
--          PHA_TADD_SLOW,
--          GRD_TADD_SLOW,
--          PHA_TADD_FAST,
--          GRD_TADD_FAST,
--          COLD_PHA_TMUL_FAST,
--          COLD_GRD_TMUL_FAST,
--          PHA_TMUL_SLOW,
--          GRD_TMUL_SLOW,
--          PHA_TMUL_FAST,
--          GRD_TMUL_FAST,
--          GRD_SLOW_CRV_OPS,
--          PHA_SLOW_CRV_OPS,
--          PHA_SLOW_CRV,
--          GRD_SLOW_CRV,
--          PHA_FAST_CRV,
--          GRD_FAST_CRV,
--          PHA_RESP_TIME,
--          GRD_RESP_TIME,
--          PHA_OP_F_CRV,
--          GRD_OP_F_CRV,
--          TCC2_SLOW_CURVES_USED,
--          TCC1_FAST_CURVES_USED,
--          PHA_INST_TRIP_CD,
--          GRD_INST_TRIP_CD,
--          PHA_MIN_TRIP,
--          GRD_MIN_TRIP,
--          OPERATING_AS_CD,
--          BYPASS_PLANS,
--          CONTROL_TYPE,
--          OK_TO_BYPASS,
--          RELEASED_BY,
--          PROCESSED_FLAG,
--          CONTROL_SERIAL_NUM,
--          SOFTWARE_VERSION,
--          FIRMWARE_VERSION,
--          CURRENT_FUTURE,
--          DISTRICT,
--          DIVISION,
--          PEER_REVIEW_BY,
--          PEER_REVIEW_DT,
--          RADIO_SERIAL_NUM,
--          RADIO_MODEL_NUM,
--          RADIO_MANF_CD,
--          SPECIAL_CONDITIONS,
--          REPEATER,
--          RTU_ADDRESS,
--          TRANSMIT_DISABLE_DELAY,
--          TRANSMIT_ENABLE_DELAY,
--          BAUD_RATE,
--          MASTER_STATION,
--          SCADA_TYPE,
--          SCADA,
--          FLISR_ENGINEERING_COMMENTS,
--          "Limiting_Factor",
--          "Winter_Load_Limit",
--          "Summer_Load_Limit",
--          FLISR,
--          ENGINEERING_COMMENTS,
--          ACTIVE_PROFILE,
--          ALT3_COLD_LOAD_PLI_CURVE_GRD,
--          ALT3_COLD_LOAD_PLI_CURVE_PHA,
--          ALT3_COLD_LOAD_PLI_GRD,
--          ALT3_COLD_LOAD_PLI_PHA,
--          ALT3_COLD_LOAD_PLI_USED,
--          ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--          ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--          ALT3_HIGH_CURRENT_LOCKOUT_USED,
--          ALT3_SGF_TIME_DELAY,
--          ALT3_SGF_MIN_TRIP_PERCENT,
--          ALT3_SGF_CD,
--          ALT3_RECLOSE_RETRY_ENABLED,
--          ALT3_RESET,
--          ALT3_RECLOSE3_TIME,
--          ALT3_RECLOSE2_TIME,
--          ALT3_RECLOSE1_TIME,
--          ALT3_TOT_LOCKOUT_OPS,
--          ALT3_GRD_DELAY,
--          ALT3_PHA_DELAY,
--          ALT3_PHA_TADD_SLOW,
--          ALT3_GRD_TADD_SLOW,
--          ALT3_PHA_TADD_FAST,
--          ALT3_GRD_TADD_FAST,
--          ALT3_PHA_VMUL_SLOW,
--          ALT3_GRD_VMUL_SLOW,
--          ALT3_PHA_VMUL_FAST,
--          ALT3_GRD_SLOW_CRV_OPS,
--          ALT3_PHA_SLOW_CRV_OPS,
--          ALT3_GRD_VMUL_FAST,
--          ALT3_PHA_SLOW_CRV,
--          ALT3_GRD_SLOW_CRV,
--          ALT3_PHA_FAST_CRV,
--          ALT3_GRD_FAST_CRV,
--          ALT3_TCC2_SLOW_CURVES_USED,
--          ALT3_TCC1_FAST_CURVES_USED,
--          ALT3_PHA_OP_F_CRV,
--          ALT3_GRD_OP_F_CRV,
--          ALT3_PHA_INST_TRIP_CD,
--          ALT3_GRD_INST_TRIP_CD,
--          ALT3_PHA_MIN_TRIP,
--          ALT3_GRD_MIN_TRIP,
--          ALT2_PHA_TADD_FAST,
--          ALT2_GRD_TADD_FAST,
--          ALT2_GRD_VMUL_SLOW,
--          ALT2_PHA_VMUL_FAST,
--          ALT2_PHA_FAST_CRV,
--          ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--          RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--        )
--      SELECT EFFECTIVE_DT,
--        TIMESTAMP,
--        DATE_MODIFIED,
--        RELAY_TYPE,
--        PREPARED_BY,
--        DEVICE_ID,
--        OPERATING_NUM,
--        FEATURE_CLASS_NAME,
--        GLOBAL_ID,
--        ALT2_PHA_INST_TRIP_CD,
--        ALT2_GRD_INST_TRIP_CD,
--        ALT2_PHA_MIN_TRIP,
--        ALT2_GRD_MIN_TRIP,
--        ALT2_LS_RESET_TIME,
--        ALT2_LS_LOCKOUT_OPS,
--        ALT2_INRUSH_DURATION,
--        ALT2_PHA_INRUSH_THRESHOLD,
--        ALT2_GRD_INRUSH_THRESHOLD,
--        ALT2_PHA_ARMING_THRESHOLD,
--        ALT2_GRD_ARMING_THRESHOLD,
--        ALT2_PERMIT_LS_ENABLING,
--        ALT_COLD_LOAD_PLI_CURVE_GRD,
--        ALT_COLD_LOAD_PLI_CURVE_PHA,
--        ALT_COLD_LOAD_PLI_GRD,
--        ALT_COLD_LOAD_PLI_PHA,
--        ALT_COLD_LOAD_PLI_USED,
--        ALT_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT_HIGH_CURRENT_LOCKOUT_USED,
--        ALT_SGF_TIME_DELAY,
--        ALT_SGF_MIN_TRIP_PERCENT,
--        ALT_SGF_CD,
--        ALT_RECLOSE_RETRY_ENABLED,
--        ALT_RESET,
--        ALT_RECLOSE3_TIME,
--        ALT_RECLOSE2_TIME,
--        ALT_RECLOSE1_TIME,
--        ALT_TOT_LOCKOUT_OPS,
--        ALT_PHA_TADD_SLOW,
--        ALT_GRD_TADD_SLOW,
--        ALT_PHA_TADD_FAST,
--        ALT_GRD_TADD_FAST,
--        ALT_PHA_VMUL_SLOW,
--        ALT_GRD_VMUL_SLOW,
--        ALT_PHA_VMUL_FAST,
--        ALT_GRD_SLOW_CRV_OPS,
--        ALT_PHA_SLOW_CRV_OPS,
--        ALT_GRD_VMUL_FAST,
--        ALT_PHA_SLOW_CRV,
--        ALT_GRD_SLOW_CRV,
--        ALT_PHA_FAST_CRV,
--        ALT_GRD_FAST_CRV,
--        ALT_PHA_RESP_TIME,
--        ALT_GRD_RESP_TIME,
--        ALT_TCC2_SLOW_CURVES_USED,
--        ALT_TCC1_FAST_CURVES_USED,
--        ALT_PHA_OP_F_CRV,
--        ALT_GRD_OP_F_CRV,
--        ALT_PHA_INST_TRIP_CD,
--        ALT_GRD_INST_TRIP_CD,
--        ALT_PHA_MIN_TRIP,
--        ALT_GRD_MIN_TRIP,
--        COLD_LOAD_PLI_CURVE_GRD,
--        COLD_LOAD_PLI_CURVE_PHA,
--        COLD_LOAD_PLI_GRD,
--        COLD_LOAD_PLI_PHA,
--        COLD_LOAD_PLI_USED,
--        HIGH_CURRENT_LOCKUOUT_GRD,
--        HIGH_CURRENT_LOCKOUT_PHA,
--        HIGH_CURRENT_LOCKOUT_USED,
--        SGF_TIME_DELAY,
--        SGF_MIN_TRIP_PERCENT,
--        SGF_CD,
--        RECLOSE_RETRY_ENABLED,
--        RESET,
--        RECLOSE3_TIME,
--        RECLOSE2_TIME,
--        RECLOSE1_TIME,
--        TOT_LOCKOUT_OPS,
--        PHA_TADD_SLOW,
--        GRD_TADD_SLOW,
--        PHA_TADD_FAST,
--        GRD_TADD_FAST,
--        COLD_PHA_TMUL_FAST,
--        COLD_GRD_TMUL_FAST,
--        PHA_TMUL_SLOW,
--        GRD_TMUL_SLOW,
--        PHA_TMUL_FAST,
--        GRD_TMUL_FAST,
--        GRD_SLOW_CRV_OPS,
--        PHA_SLOW_CRV_OPS,
--        PHA_SLOW_CRV,
--        GRD_SLOW_CRV,
--        PHA_FAST_CRV,
--        GRD_FAST_CRV,
--        PHA_RESP_TIME,
--        GRD_RESP_TIME,
--        PHA_OP_F_CRV,
--        GRD_OP_F_CRV,
--        TCC2_SLOW_CURVES_USED,
--        TCC1_FAST_CURVES_USED,
--        PHA_INST_TRIP_CD,
--        GRD_INST_TRIP_CD,
--        PHA_MIN_TRIP,
--        GRD_MIN_TRIP,
--        OPERATING_AS_CD,
--        BYPASS_PLANS,
--        CONTROL_TYPE,
--        OK_TO_BYPASS,
--        RELEASED_BY,
--        PROCESSED_FLAG,
--        CONTROL_SERIAL_NUM,
--        SOFTWARE_VERSION,
--        FIRMWARE_VERSION,
--        CURRENT_FUTURE,
--        DISTRICT,
--        DIVISION,
--        PEER_REVIEW_BY,
--        PEER_REVIEW_DT,
--        RADIO_SERIAL_NUM,
--        RADIO_MODEL_NUM,
--        RADIO_MANF_CD,
--        SPECIAL_CONDITIONS,
--        REPEATER,
--        RTU_ADDRESS,
--        TRANSMIT_DISABLE_DELAY,
--        TRANSMIT_ENABLE_DELAY,
--        BAUD_RATE,
--        MASTER_STATION,
--        SCADA_TYPE,
--        SCADA,
--        FLISR_ENGINEERING_COMMENTS,
--        "Limiting_Factor",
--        "Winter_Load_Limit",
--        "Summer_Load_Limit",
--        FLISR,
--        ENGINEERING_COMMENTS,
--        ACTIVE_PROFILE,
--        ALT3_COLD_LOAD_PLI_CURVE_GRD,
--        ALT3_COLD_LOAD_PLI_CURVE_PHA,
--        ALT3_COLD_LOAD_PLI_GRD,
--        ALT3_COLD_LOAD_PLI_PHA,
--        ALT3_COLD_LOAD_PLI_USED,
--        ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
--        ALT3_HIGH_CURRENT_LOCKOUT_PHA,
--        ALT3_HIGH_CURRENT_LOCKOUT_USED,
--        ALT3_SGF_TIME_DELAY,
--        ALT3_SGF_MIN_TRIP_PERCENT,
--        ALT3_SGF_CD,
--        ALT3_RECLOSE_RETRY_ENABLED,
--        ALT3_RESET,
--        ALT3_RECLOSE3_TIME,
--        ALT3_RECLOSE2_TIME,
--        ALT3_RECLOSE1_TIME,
--        ALT3_TOT_LOCKOUT_OPS,
--        ALT3_GRD_DELAY,
--        ALT3_PHA_DELAY,
--        ALT3_PHA_TADD_SLOW,
--        ALT3_GRD_TADD_SLOW,
--        ALT3_PHA_TADD_FAST,
--        ALT3_GRD_TADD_FAST,
--        ALT3_PHA_VMUL_SLOW,
--        ALT3_GRD_VMUL_SLOW,
--        ALT3_PHA_VMUL_FAST,
--        ALT3_GRD_SLOW_CRV_OPS,
--        ALT3_PHA_SLOW_CRV_OPS,
--        ALT3_GRD_VMUL_FAST,
--        ALT3_PHA_SLOW_CRV,
--        ALT3_GRD_SLOW_CRV,
--        ALT3_PHA_FAST_CRV,
--        ALT3_GRD_FAST_CRV,
--        ALT3_TCC2_SLOW_CURVES_USED,
--        ALT3_TCC1_FAST_CURVES_USED,
--        ALT3_PHA_OP_F_CRV,
--        ALT3_GRD_OP_F_CRV,
--        ALT3_PHA_INST_TRIP_CD,
--        ALT3_GRD_INST_TRIP_CD,
--        ALT3_PHA_MIN_TRIP,
--        ALT3_GRD_MIN_TRIP,
--        ALT2_PHA_TADD_FAST,
--        ALT2_GRD_TADD_FAST,
--        ALT2_GRD_VMUL_SLOW,
--        ALT2_PHA_VMUL_FAST,
--        ALT2_PHA_FAST_CRV,
--        ALT2_GRD_FAST_CRV,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION,
--        RTU_SOFTWARE_VERSION, RTU_MANF_CD,MULTI_FUNCTIONAL,ALT_GRD_LOCKOUT_OPS,
--ALT_PHA_LOCKOUT_OPS
--      FROM SM_RECLOSER
--      WHERE GLOBAL_ID=I_Global_id_Current;
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_RECLOSER_HIST',
--          SM_RECLOSER_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--      DELETE FROM SM_RECLOSER WHERE GLOBAL_ID = I_Global_id_Previous;
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--
--    END;
--  END IF;
--END SP_RECLOSER_DETECTION;
--
--PROCEDURE SP_REGULATOR_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER)
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_REGULATOR
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_REGULATOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_REGULATOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_REGULATOR
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_REGULATOR
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--
--
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_REGULATOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          BANK_CD,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          I_Bank_code,
--          'C'
--        );
--
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_REGULATOR
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          CONTROL_TYPE,
--          BANK_CD,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Control_type_code,
--          I_Bank_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_REGULATOR',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--
--       IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--    END;
--  END IF;
--
--
--  IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--
--      -- first copy the entire current record to history table
--
--INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT,  LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY)
--SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET, REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY
--FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;
--
--
--
--
--
--
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--
--          'SM_REGULATOR_HIST',
--           SM_REGULATOR_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--      -- Not Updating BANK_CODE, because we have to save the previous value
--      UPDATE SM_REGULATOR
--      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,CONTROL_TYPE=I_Control_type_code
--      WHERE GLOBAL_ID    = I_Global_id_Current;
--
--   --- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--       IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--
--    END;
--
--  END IF;
--
--  IF ACTION = 'D' THEN
--
--
--      -- first copy the entire current record to history table
--
--
--INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY)
--SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY
--FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;
--
--
--
--
--
--
--
--     DELETE FROM SM_REGULATOR WHERE GLOBAL_ID = I_Global_id_Current;
--
--      -- Insert a record in comments table with notes set to INST
--
--      -- Insert a record in comments table with notes set to INST
--
--
--  INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_REGULATOR_HIST',
--          SM_REGULATOR_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--     IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--
--
--
--
-- IF ACTION = 'R' THEN
--
--  BEGIN
--
--
--      -- First insert the record in the device table with current_future set to 'C'
--
--	INSERT INTO SM_REGULATOR ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CONTROL_TYPE, BANK_CD,CURRENT_FUTURE )
--VALUES ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, I_Control_type_code,I_Bank_code, 'C' );
--
--      -- Insert the record in the device table with current_future set to 'F'
--
--
--INSERT INTO SM_REGULATOR ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CONTROL_TYPE, BANK_CD,CURRENT_FUTURE )
--VALUES ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, I_Control_type_code,I_Bank_code, 'F' );
--
--
--      -- Insert a record in comments table with notes set to OTHR
--
--      INSERT INTO SM_COMMENT_HIST ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--VALUES ( 'SM_REGULATOR', I_Global_id_Current, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_REGULATOR_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--
--UPDATE  SM_REGULATOR SET (BAND_WIDTH, BAUD_RATE, BLOCKED_PCT,  CONTROL_TYPE,
-- FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET,  HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, PEAK_LOAD,  POWER_FACTOR,  PRIMARY_CT_RATING, PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE,  REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY)=
--(SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT,  CONTROL_TYPE,
-- FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET,  HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, PEAK_LOAD,  POWER_FACTOR,  PRIMARY_CT_RATING, PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE,  REPEATER, REV_A_RESET, REV_A_VOLT,
--REV_A_XSET, REV_B_RESET, REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--FROM  SM_REGULATOR WHERE GLOBAL_ID     = I_Global_id_Previous AND CURRENT_FUTURE='C')
--WHERE GLOBAL_ID     = I_Global_id_Current;
--
--
--INSERT INTO SM_REGULATOR_HIST(BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET,  REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY)
--SELECT BAND_WIDTH, BAUD_RATE, BLOCKED_PCT, CONTROL_SERIAL_NUM, CONTROL_TYPE, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION,
--EFFECTIVE_DT,  FEATURE_CLASS_NAME, FIRMWARE_VERSION, FWD_A_RESET, FWD_A_STATUS, FWD_A_VOLT, FWD_A_XSET, FWD_B_RESET, FWD_B_STATUS,
--FWD_B_VOLT, FWD_B_XSET, FWD_C_RESET, FWD_C_STATUS, FWD_C_VOLT, FWD_C_XSET, GLOBAL_ID, HIGH_VOLTAGE_LIMIT, LOAD_CYCLE, LOW_VOLTAGE_LIMIT, MASTER_STATION,
--MIN_LOAD, "MODE",  OK_TO_BYPASS, OPERATING_NUM, PEAK_LOAD, PEER_REVIEW_BY, PEER_REVIEW_DT, POWER_FACTOR, PREPARED_BY, PRIMARY_CT_RATING, PROCESSED_FLAG,
--PT_RATIO, PVD_MAX, PVD_MIN, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RANGE_UNBLOCKED, RELAY_TYPE, RELEASED_BY, REPEATER, REV_A_RESET,  REV_A_VOLT,
--REV_A_XSET, REV_B_RESET,  REV_B_VOLT, REV_B_XSET, REV_C_RESET, REV_C_VOLT, REV_C_XSET, REV_THRESHOLD, RISE_RATING, RTU_ADDRESS, SCADA, SCADA_TYPE,
--SOFTWARE_VERSION, SPECIAL_CONDITIONS, STEPS, SVD_MIN, TIMER, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, USE_RX, VOLT_VAR_TEAM_MEMBER,BANK_CD,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,ENGINEERING_COMMENTS,FLISR_ENGINEERING_COMMENTS,SEASON_OFF,SWITCH_POSITION ,EMERGENCY_ONLY
--FROM SM_REGULATOR WHERE GLOBAL_ID=I_Global_id_Current;
--
--
--
--INSERT INTO SM_COMMENT_HIST ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS ) VALUES (
--'SM_REGULATOR_HIST', SM_REGULATOR_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );
--
--
--DELETE FROM SM_REGULATOR WHERE GLOBAL_ID = I_Global_id_Previous;
--
--      IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--
--
--END;
--END IF;
--
--
--
--END SP_REGULATOR_DETECTION;
--
--PROCEDURE SP_SWITCH_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER)
--
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_SWITCH
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'C'
--        );
--
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_SWITCH
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--    IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--    END;
--  END IF;
--
--
--  IF ACTION= 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--
--      -- first copy the entire current record to history table
--
-- INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS)
--SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Current;
--
--
--
--
--
--
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--
--          'SM_SWITCH_HIST',
--           SM_SWITCH_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--      UPDATE SM_SWITCH
--      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,SWITCH_TYPE=I_Switch_type_code
--      WHERE GLOBAL_ID    = I_Global_id_Current;
--
--        IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--        END IF;
--
--    END;
--
--  END IF;
--
--  IF ACTION = 'D' THEN
--
--
-- INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS)
--SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,
--RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Current;
--
--     DELETE FROM SM_SWITCH WHERE GLOBAL_ID = I_Global_id_Current;
--
--      -- Insert a record in comments table with notes set to INST
--
--      -- Insert a record in comments table with notes set to INst
--
--  INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_HIST',
--          SM_SWITCH_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--     IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--
--
--
--
--    IF ACTION = 'R' THEN
--    BEGIN
--
-- -- First insert the record in the device table with current_future set to 'C'
--
--
--INSERT
--      INTO SM_SWITCH
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'C'
--        );
--
--      -- Insert the record in the device table with current_future set to 'F'
--
--      INSERT
--      INTO SM_SWITCH
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_SWITCH_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--
--
--UPDATE SM_SWITCH SET (RADIO_SERIAL_NUM, RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION,
--SCADA_TYPE, SCADA,  OPERATING_MODE, ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN,
--OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE, UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT,
--LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION, SELECT_RETURN,
--UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER,
--SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP, FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT,
--GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE, ATS_CAPABLE, SECTIONALIZING_FEATURE,
--OK_TO_BYPASS, CONTROL_UNIT_TYPE, SOFTWARE_VERSION, FIRMWARE_VERSION, SWITCH_TYPE,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY)
-- =(SELECT RADIO_SERIAL_NUM, RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION,
--SCADA_TYPE, SCADA,  OPERATING_MODE, ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN,
--OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE, UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT,
--LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION, SELECT_RETURN,
--UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER,
--SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP, FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT,
--GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE, ATS_CAPABLE, SECTIONALIZING_FEATURE,
--OK_TO_BYPASS, CONTROL_UNIT_TYPE, SOFTWARE_VERSION, FIRMWARE_VERSION, SWITCH_TYPE,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS,
--DATE_MODIFIED, PREPARED_BY, EFFECTIVE_DT, PEER_REVIEW_DT, PEER_REVIEW_BY
--FROM SM_SWITCH  WHERE GLOBAL_ID   = I_Global_id_Previous   AND CURRENT_FUTURE='C' ) WHERE GLOBAL_ID = I_Global_id_Current;
--
---- first copy the entire previous record to history table
--
-- INSERT INTO SM_SWITCH_HIST ( PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS)
--SELECT      PEER_REVIEW_BY, PEER_REVIEW_DT, EFFECTIVE_DT, TIMESTAMP, DATE_MODIFIED, SWITCH_TYPE, PREPARED_BY, DEVICE_ID, OPERATING_NUM, FEATURE_CLASS_NAME, GLOBAL_ID, RADIO_SERIAL_NUM,
--RADIO_MODEL_NUM, RADIO_MANF_CD, SPECIAL_CONDITIONS, REPEATER, RTU_ADDRESS, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, BAUD_RATE, MASTER_STATION, SCADA_TYPE, SCADA,  OPERATING_MODE,
--ENGINEERING_COMMENTS, LIMITING_FACTOR, WINTER_LOAD_LIMIT, SUMMER_LOAD_LIMIT, FLISR, WINDOW_LENGTH, WINDOW_BEGIN, OC_LOCKOUT_PICKUP, LOCKOUT_RESET, RETURN_TO_SOURCE_TIME, LOSS_OF_RIGHT_SOURCE, LOSS_OF_LEFT_SOURCE,
--UNBALANCE_DETECT_VOLT, RETURN_TO_SOURCE_VOLT, LOSS_OF_SOURCE, LOCKOUT_LEVEL, COMM_O_BIT_RATE, ACCESS_CODE, SET_BASE_RIGHT, SET_BASE_LEFT, NORMALIZE_RIGHT, NORMALIZE_LEFT, DWELL_TIMER, SELECT_TRANSACTION,
--SELECT_RETURN, UNBALANCE_DETECT, SELECT_PREFERRED, ATS_ALTERNATE_FEED, ATS_PREFERRED_FEED, AUTO_RECLOSE_TIME, CURR_THRESH, TIME_THRESH, VOLT_LOSS_THRESH, OVERC_SHOTS_TO_LO_OPER, SHOTS_TO_LOCKOUT_TIME, SHOTS_REQ_LOCKOUT, RECL_COUNT_TO_TRIP,
--FAULT_CUR_LOSS, OC_TO_VOLT_TIME, RECLOSE_RESET_TIME, SECT_RESET_TIME, GRD_INRUSH_MULT, PHA_INRUSH_MULT, GRD_INRUSH_TIME, PHA_INRUSH_TIME, GRD_FAULT_DURATION, PHA_FAULT_DURATION, GRD_FAULT_CUR_LEVEL, PHA_FAULT_CUR_LEVEL, ATS_FEATURE,
--ATS_CAPABLE, SECTIONALIZING_FEATURE, OK_TO_BYPASS, RELEASED_BY, PROCESSED_FLAG, CONTROL_UNIT_TYPE, CONTROL_SERIAL_NUM, SOFTWARE_VERSION, FIRMWARE_VERSION, CURRENT_FUTURE,  DISTRICT, DIVISION,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,FLISR_ENGINEERING_COMMENTS
--FROM SM_SWITCH WHERE GLOBAL_ID=I_Global_id_Previous;
--
--
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_HIST',
--          SM_SWITCH_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--
--      -- Remove previous global id from te device table
--
--      DELETE
--      FROM SM_SWITCH
--      WHERE GLOBAL_ID = I_Global_id_Previous;
--
--    IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--
--END SP_SWITCH_DETECTION;
--    Change for project GIS powerbase integration - End

PROCEDURE SP_SPECIAL_LOAD_DETECTION(
    I_Global_id_Current     IN CHAR,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN CHAR,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER)
AS
    REASON_TYPE CHAR;
    DEVICE_TYPE VARCHAR2(50);
    NUM1        NUMBER;
    NUM2        NUMBER;
    VALID_NUM   NUMBER;
    VAR1        VARCHAR2(50);
    ACTION      CHAR;
BEGIN
--   dbms_output.put_line(ACTION || ' ' || I_reason_type );
   REASON_TYPE   := I_reason_type ;
   VALID_NUM:=0;
--- Validatations for I/U/D/R conditions
   CASE
   WHEN REASON_TYPE = 'I' THEN
        ACTION := 'I';
        VALID_NUM:=6;
   WHEN REASON_TYPE = 'U' THEN
         ACTION := 'U';
         VALID_NUM:=5;
   WHEN REASON_TYPE = 'D' THEN
         ACTION := 'D';
         VALID_NUM:=6;
   WHEN REASON_TYPE = 'R' THEN
      BEGIN
--      Check if the previous global id exist in device table
        SELECT COUNT(*) INTO NUM2
          FROM SM_SPECIAL_LOAD
         WHERE REF_GLOBAL_ID   = I_Global_id_Previous;
--      Check if the current global id does not exist in device table
        SELECT COUNT(*) INTO NUM1
          FROM SM_SPECIAL_LOAD
         WHERE REF_GLOBAL_ID   = I_Global_id_Current;

--     If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
        IF (NUM2 >=1  AND NUM1 < 1 ) THEN
           ACTION := 'R';
           VALID_NUM:=0;
        END IF;
--     If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
        IF (NUM2 >=1  AND NUM1 >=1 ) THEN
           ACTION := 'U';
           VALID_NUM:=5;
        END IF;
--     If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
        IF (NUM2 <1  AND NUM1 >=1 ) THEN
           ACTION := 'U';
           VALID_NUM:=5;
        END IF;
--    If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
        IF (NUM2 <1  AND NUM1 <1 ) THEN
           ACTION := 'I';
           VALID_NUM:=6;
        END IF;
     END;
   END CASE;
   IF ACTION = 'I' THEN
      BEGIN
         IF VALID_NUM=6 THEN
            RAISE  INS_CODE_SIX;
         END IF;
      END;
   END IF;

   IF ACTION = 'U' THEN
     -- check to see if globalid exist in table. If it does not then throw exception
     BEGIN
       IF VALID_NUM=5 THEN
          RAISE  UPD_CODE_FIVE;
       END IF;
     END;
  END IF;

  IF ACTION= 'D' THEN
     IF VALID_NUM=6 THEN
        RAISE INS_CODE_SIX;
     END IF;
  END IF;

  IF ACTION = 'R' THEN
     BEGIN
        -- Replacing New Global Id with OLD in the Special Load table First insert the record in the device table
        Update SM_SPECIAL_LOAD set REF_GLOBAL_ID = I_Global_id_Current
         WHERE REF_GLOBAL_ID   = I_Global_id_Previous;

--      SYS.DBMS_OUTPUT.PUT_LINE('DELETE FROM SM_SPECIAL_LOAD WHERE REF_GLOBAL_ID = ' || I_Global_id_Previous );
        IF VALID_NUM=5 THEN
           RAISE  UPD_CODE_FIVE;
        END IF;
        IF VALID_NUM=6 THEN
           RAISE  INS_CODE_SIX;
        END IF;
     exception
        WHEN OTHERS THEN
          dbms_output.put_line(SQLCODE || ' ' || sqlerrm );
     END;
  END IF;
END SP_SPECIAL_LOAD_DETECTION;

--    Change for project GIS powerbase integration - Start
--PROCEDURE SP_PRIMARY_METER_DETECTION(
--    I_Global_id_Current     IN VARCHAR2,
--    I_reason_type           IN VARCHAR2,
--    I_feature_class_name    IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num         IN VARCHAR2,
--    I_Global_id_Previous    IN VARCHAR2,
--    I_Division              IN VARCHAR2,
--    I_District              IN VARCHAR2,
--    I_Control_type_code     IN VARCHAR2,
--    I_Switch_type_code      IN VARCHAR2,
--    I_Bank_code             IN NUMBER)
--
--AS
--   REASON_TYPE CHAR;
--   DEVICE_TYPE VARCHAR2(50);
--   NUM1        NUMBER;
--   NUM2        NUMBER;
--   VALID_NUM   NUMBER;
--   VAR1        VARCHAR2(50);
--   ACTION      CHAR;
--    --Changes for ENOS to SAP migration - change detection Start.
--   Var_ID NUMBER;
--   Var_ID_2 NUMBER;
--   --Changes for ENOS to SAP migration - change detection End.
--BEGIN
--
--   REASON_TYPE   := I_reason_type ;
--   VALID_NUM:=0;
--dbms_output.put_line(reason_type);
--   --- Validatations for I/U/D/R conditions
--   CASE
--   WHEN REASON_TYPE = 'I' THEN
--   BEGIN
--   --  Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_METER
--       WHERE GLOBAL_ID = I_Global_id_Current;
--
--      IF NUM1  >= 1 THEN
--         ACTION  :='U';
--         VALID_NUM:=5;
--      ELSE
--         ACTION:='I';
--      END IF;
--   END;
--
--
--   WHEN REASON_TYPE = 'U' THEN
--   BEGIN
----    Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_METER
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--         ACTION := 'I';
--         VALID_NUM:=6;
--      ELSE
--         ACTION:='U';
--      END IF;
--   END;
--
--   WHEN REASON_TYPE = 'D' THEN
--   BEGIN
----    Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_METER
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--         VALID_NUM:=6;
--         ACTION :='D';
--      ELSE
--         ACTION := 'D';
--      END IF;
--   END;
--
--   WHEN REASON_TYPE = 'R' THEN
--   BEGIN
----    Check if the previous global id exist in device table
--      SELECT COUNT(*) INTO NUM2
--        FROM SM_PRIMARY_METER
--       WHERE GLOBAL_ID   = I_Global_id_Previous
--         AND CURRENT_FUTURE='C';
--
----    Check if the current global id does not exist in device table
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_METER
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';
--
----    If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--         ACTION := 'R';
--         VALID_NUM:=0;
--      END IF;
--
----    If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--         ACTION := 'U';
--         VALID_NUM:=5;
--      END IF;
--
----    If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--         ACTION := 'U';
--         VALID_NUM:=5;
--      END IF;
--
----    If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--         ACTION := 'I';
--         VALID_NUM:=6;
--      END IF;
--   END;
--   END CASE;
--
--   dbms_output.put_line(action ||  ' ' || VALID_NUM );
--
--   IF ACTION = 'I' THEN
--      BEGIN
--
----    First insert the record in the device table with current_future set to 'C'
--         INSERT INTO SM_PRIMARY_METER
--         ( GLOBAL_ID, FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,CURRENT_FUTURE)
--         VALUES
--         ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'C');
--
----Changes for ENOS to SAP migration - change detection Start. Not inserting the Future settings for primary meter
----    Insert the record in the device table with current_future set to 'F'
--      dbms_output.put_line(action);
----         INSERT INTO SM_PRIMARY_METER
----         ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
----         VALUES
----         ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'F' );
----    Insert a record in comments table with notes set to INST
--
--          SELECT NVL(MAX(ID),0) INTO Var_ID FROM SM_PRIMARY_METER;
--        --SELECT (NVL(MAX(ID),0)+1) INTO Var_ID_2 FROM SM_PROTECTION;
--
--    -- INSERT New record in protection table
--         INSERT INTO SM_PROTECTION
--         (parent_type, parent_id, protection_type,CURRENT_FUTURE)
--         VALUES
--         ('PRIMARYMETER',Var_ID,'UNSP','C');
--
--          --Changes for ENOS to SAP migration - change detection End..
--
----    Insert a record in comments table with notes set to INST
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE,PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_METER', I_Global_id_Current, sysdate, 'INST', 'SYSTEM', sysdate, 'New record from GIS system' );
--
--         IF VALID_NUM=6 THEN
--            RAISE  INS_CODE_SIX;
--         END IF;
--      END;
--   END IF;
--   IF ACTION = 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--      BEGIN
--      --Changes for ENOS to SAP migration - change detection Start.
--
----         INSERT INTO SM_PRIMARY_METER_HIST
----            ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
----              ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP,
----              GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE,
----              LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY,
----              PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET,
----              PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE,
----              RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY,
----              TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION,
----              RTU_MANF_CD,NOTES,DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION,
----              GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
----              PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING )
----         SELECT
----              ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
----              ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP,
----              GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE,
----              LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY,
----              PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET,
----              PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE,
----              RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY,
----              TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION,
----              RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION, GRD_BK_SOFTWARE_VERSION,
----              GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
----              PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----           FROM SM_PRIMARY_METER
----          WHERE GLOBAL_ID= I_Global_id_Current;
--
--        INSERT INTO SM_PRIMARY_METER_HIST
--          (GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED,DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,DIRECT_TRANSFER_TRIP, CURRENT_FUTURE)
--        SELECT
--          GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED,DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,DIRECT_TRANSFER_TRIP,CURRENT_FUTURE
--        FROM SM_PRIMARY_METER
--        WHERE GLOBAL_ID= I_Global_id_Current;
--
--   --Changes for ENOS to SAP migration - change detection End..
--
--      -- Insert a record in comments table with notes set to INST
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS  )
--          VALUES
--         ( 'SM_PRIMARY_METER', SM_PRIMARY_METER_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate,
--           'Record updated in GIS system');
--
--         UPDATE SM_PRIMARY_METER
--            SET OPERATING_NUM = I_operating_num,
--                DIVISION        =I_Division,
--                DISTRICT        = I_District
--          WHERE GLOBAL_ID   = I_Global_id_Current;
--
--        -- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
--         IF VALID_NUM=5 THEN
--            RAISE  UPD_CODE_FIVE;
--         END IF;
--      END;
--   END IF;
--   IF ACTION = 'D' THEN
--      BEGIN
--      -- first copy the entire current record to history table
--       --Changes for ENOS to SAP migration - change detection Start.
--
----         INSERT INTO SM_PRIMARY_METER_HIST
----             ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
----               ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID,
----               GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
----               GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM,
----               OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE,
----               PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD,
----               RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
----               SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM,
----               RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES,DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM,
----               GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM,
----               PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM,
----               PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----             )
----         SELECT ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
----                ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID,
----                GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
----                GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM,
----                OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE,
----                PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD,
----                RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
----                SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM,
----                RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM,
----                GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM,
----                PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM,
----                PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----           FROM SM_PRIMARY_METER
----          WHERE GLOBAL_ID= I_Global_id_Current;
--
--        INSERT INTO SM_PRIMARY_METER_HIST
--          (GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED, DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,DIRECT_TRANSFER_TRIP,CURRENT_FUTURE)
--        SELECT
--          GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED, DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,DIRECT_TRANSFER_TRIP,CURRENT_FUTURE
--        FROM SM_PRIMARY_METER
--        WHERE GLOBAL_ID= I_Global_id_Current;
--
--        -- Delete record from SM protection also
--        DELETE FROM  SM_PROTECTION WHERE PARENT_TYPE='PRIMARYMETER' AND
--        PARENT_ID = (SELECT ID FROM SM_PRIMARY_METER WHERE GLOBAL_ID = I_Global_id_Current);
--
-- --Changes for ENOS to SAP migration - change detection End..
--
--         DELETE FROM SM_PRIMARY_METER WHERE GLOBAL_ID = I_Global_id_Current;
--
--         DELETE FROM SM_SPECIAL_LOAD WHERE REF_GLOBAL_ID = I_Global_id_Current;
--     --  Insert a record in comments table with notes set to INST
--     --  Insert a record in comments table with notes set to INST
--
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_METER_HIST', SM_PRIMARY_METER_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );
--
--         IF VALID_NUM=6 THEN
--            RAISE  INS_CODE_SIX;
--         END IF;
--      END;
--   END IF;
--   IF ACTION = 'R' THEN
--      BEGIN
--         dbms_output.put_line(action);
--      -- First insert the record in the device table with current_future set to 'C'
--         INSERT
--         INTO SM_PRIMARY_METER
--            ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
--         VALUES
--            ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, 'C' );
--
--     --Changes for ENOS to SAP migration -Start.. change detection Start. Not inserting future settings
--
----      -- Insert the record in the device table with current_future set to 'F'
----         INSERT INTO SM_PRIMARY_METER
----            ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
----         VALUES
----            ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'F' );
--
-- SELECT NVL(MAX(ID),0) INTO Var_ID FROM SM_PRIMARY_METER;
--        --SELECT (NVL(MAX(ID),0)+1) INTO Var_ID_2 FROM SM_PROTECTION;
--
--    -- INSERT New record in protection table
--         INSERT INTO SM_PROTECTION
--         (parent_type, parent_id, protection_type,CURRENT_FUTURE)
--         VALUES
--         ('PRIMARYMETER',Var_ID,'UNSP','C');
--
--   --Changes for ENOS to SAP migration - change detection End.
--
--      -- Insert a record in comments table with notes set to OTHR
--         INSERT INTO SM_COMMENT_HIST
--            ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--            ( 'SM_PRIMARY_METER', I_Global_id_Current, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_PRIMARY_METER_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--
--         dbms_output.put_line('X');
--
--        --Changes for ENOS to SAP migration - change detection Start.
--
----         UPDATE SM_PRIMARY_METER A SET
----           ( ANNUAL_LF, BAUD_RATE, CC_RATING, DPA_CD, ENGINEERING_COMMENTS, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE,
----             GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
----             GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPS_TO_LOCKOUT,
----             PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP,
----             PHA_PR_RELAY_TYPE, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE,
----             SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT, RTU_EXIST,
----             RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP,
----             GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION,
----             PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION,
----             PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----           )
----         = ( SELECT ANNUAL_LF, BAUD_RATE, CC_RATING, DPA_CD, ENGINEERING_COMMENTS, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE,
----                    GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
----                    GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPS_TO_LOCKOUT,
----                    PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP,
----                    PHA_PR_RELAY_TYPE, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE,
----                    SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT, RTU_EXIST,
----                    RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP,
----                    GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION,
----                    PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION,
----                    PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----               FROM SM_PRIMARY_METER B
----              WHERE GLOBAL_ID   = I_Global_id_Previous
----                AND A.CURRENT_FUTURE=B.CURRENT_FUTURE
----           )
----          WHERE GLOBAL_ID = I_Global_id_Current;
----
----         dbms_output.put_line('X');
----
----         INSERT INTO SM_PRIMARY_METER_HIST
----         ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT, ENGINEERING_COMMENTS,
----           FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP,
----           GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT,
----           NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET,
----           PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG,
----           RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
----           SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM,
----           RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD, NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,
----           GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
----           PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----         )
----         SELECT
----           ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT, ENGINEERING_COMMENTS,
----           FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP,
----           GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT,
----           NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET,
----           PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG,
----           RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
----           SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM,
----           RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES , DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,
----           GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
----           PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
----          FROM SM_PRIMARY_METER
----         WHERE GLOBAL_ID= I_Global_id_Current;
--
--
--
-- UPDATE SM_PRIMARY_METER A SET
--           ( GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED,DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,ID,DIRECT_TRANSFER_TRIP
--           )
--         =( SELECT GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DATE_MODIFIED,DISTRICT, DIVISION,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,ID,DIRECT_TRANSFER_TRIP
--               FROM SM_PRIMARY_METER B
--              WHERE GLOBAL_ID   = I_Global_id_Previous
--              AND A.CURRENT_FUTURE=B.CURRENT_FUTURE
--           )
--          WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--         INSERT INTO SM_PRIMARY_METER_HIST
--         ( GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DISTRICT, DIVISION,DATE_MODIFIED,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,ID,DIRECT_TRANSFER_TRIP,CURRENT_FUTURE
--         )
--         SELECT
--         GLOBAL_ID,FEATURE_CLASS_NAME,OPERATING_NUM, PREPARED_BY,DISTRICT, DIVISION,DATE_MODIFIED,PEER_REVIEW_DT,PEER_REVIEW_BY,NOTES, PROCESSED_FLAG,ID,DIRECT_TRANSFER_TRIP,CURRENT_FUTURE
--          FROM SM_PRIMARY_METER
--         WHERE GLOBAL_ID= I_Global_id_Current;
--
--  --Changes for ENOS to SAP migration - change detection End.
--
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_METER_HIST', SM_PRIMARY_METER_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );
--
--         DELETE FROM SM_PRIMARY_METER WHERE GLOBAL_ID = I_Global_id_Previous;
--
--         IF VALID_NUM=6 THEN
--            RAISE  INS_CODE_SIX;
--         END IF;
--
--         IF VALID_NUM=5 THEN
--            RAISE  UPD_CODE_FIVE;
--         END IF;
--      END;
--   END IF;
--END SP_PRIMARY_METER_DETECTION;
--    Change for project GIS powerbase integration - End

PROCEDURE SP_PRIMARY_GEN_DETECTION(
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Control_type_code     IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER,
     --Changes for ENOS to SAP migration - change detection Start.
    I_Service_Point_GUID    IN VARCHAR2,
    I_SAP_EGI_Notification  IN VARCHAR2,
    I_Project_Name          IN VARCHAR2,
    I_Gen_Type              IN VARCHAR2,
    I_Program_Type          IN VARCHAR2,
    I_Eff_Rating_Mach_KW    IN NUMBER,
    I_Eff_Rating_Inv_KW     IN NUMBER,
    I_Eff_Rating_Mach_KVA   IN NUMBER,
    I_Eff_Rating_Inv_KVA    IN NUMBER,
    I_Backup_Gen            IN VARCHAR2,
    I_Max_Storage_Capacity  IN NUMBER,
    I_Charge_Demand_KW      IN NUMBER,
    I_Power_Source          IN VARCHAR2 )
    --Changes for ENOS to SAP migration - change detection End..

AS
   REASON_TYPE CHAR;
   DEVICE_TYPE VARCHAR2(50);
   NUM1        NUMBER;
   NUM2        NUMBER;
   VALID_NUM   NUMBER;
   VAR1        VARCHAR2(50);
   ACTION      CHAR;
    --Changes for ENOS to SAP migration - change detection Start.
   Var_ID NUMBER;
   Var_ID_2 NUMBER;
   --Changes for ENOS to SAP migration - change detection End.
BEGIN

   REASON_TYPE   := I_reason_type ;
   VALID_NUM:=0;

   --- Validatations for I/U/D/R conditions
   CASE
   WHEN REASON_TYPE = 'I' THEN
   BEGIN
   --  Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
      --Changes for ENOS to SAP migration - change detection Start.
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_GEN
--       WHERE GLOBAL_ID = I_Global_id_Current;

SELECT COUNT(*) INTO NUM1
        FROM SM_GENERATION
        WHERE GLOBAL_ID = I_Global_id_Current;

--Changes for ENOS to SAP migration - change detection End.

      IF NUM1  >= 1 THEN
         ACTION  :='U';
         VALID_NUM:=5;
      ELSE
         ACTION:='I';
      END IF;
   END;


   WHEN REASON_TYPE = 'U' THEN
   BEGIN
--    Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
      --Changes for ENOS to SAP migration - change detection Start.

--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_GEN
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';

        SELECT COUNT(*) INTO NUM1
        FROM SM_GENERATION
       WHERE GLOBAL_ID   = I_Global_id_Current
        AND CURRENT_FUTURE='C';

--Changes for ENOS to SAP migration - change detection End.

      IF NUM1 < 1  THEN
         ACTION := 'I';
         VALID_NUM:=6;
      ELSE
         ACTION:='U';
      END IF;
   END;

   WHEN REASON_TYPE = 'D' THEN
   BEGIN
--    Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
       --Changes for ENOS to SAP migration - change detection Start.

--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_GEN
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';


       SELECT COUNT(*) INTO NUM1
        FROM SM_GENERATION
       WHERE GLOBAL_ID   = I_Global_id_Current
        AND CURRENT_FUTURE='C';

  --Changes for ENOS to SAP migration - change detection End.

      IF NUM1 < 1  THEN
         VALID_NUM:=6;
         ACTION :='D';
      ELSE
         ACTION := 'D';
      END IF;
   END;

   WHEN REASON_TYPE = 'R' THEN
   BEGIN
--    Check if the previous global id exist in device table
       --Changes for ENOS to SAP migration - change detection Start.

----    Check if the previous global id exist in device table
--      SELECT COUNT(*) INTO NUM2
--        FROM SM_PRIMARY_GEN
--       WHERE GLOBAL_ID   = I_Global_id_Previous
--         AND CURRENT_FUTURE='C';
--
----    Check if the current global id does not exist in device table
--      SELECT COUNT(*) INTO NUM1
--        FROM SM_PRIMARY_GEN
--       WHERE GLOBAL_ID   = I_Global_id_Current
--         AND CURRENT_FUTURE='C';

         --    Check if the previous global id exist in device table
      SELECT COUNT(*) INTO NUM2
        FROM SM_GENERATION
       WHERE GLOBAL_ID   = I_Global_id_Previous
         AND CURRENT_FUTURE='C';

--    Check if the current global id does not exist in device table
      SELECT COUNT(*) INTO NUM1
        FROM SM_GENERATION
       WHERE GLOBAL_ID   = I_Global_id_Current
         AND CURRENT_FUTURE='C';

   --Changes for ENOS to SAP migration - change detection End.

--    If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
         ACTION := 'R';
         VALID_NUM:=0;
      END IF;

--    If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
         ACTION := 'U';
         VALID_NUM:=5;
      END IF;

--    If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
      IF (NUM2 <1  AND NUM1 >=1 ) THEN
         ACTION := 'U';
         VALID_NUM:=5;
      END IF;

--    If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
      IF (NUM2 <1  AND NUM1 <1 ) THEN
         ACTION := 'I';
         VALID_NUM:=6;
      END IF;
   END;
   END CASE;

   IF ACTION = 'I' THEN
      BEGIN

--Changes for ENOS to SAP migration - change detection Start.

----    First insert the record in the device table with current_future set to 'C'
--         INSERT INTO SM_PRIMARY_GEN
--         ( GLOBAL_ID, FEATURE_CLASS_NAME,OPERATING_NUM,DIVISION,DISTRICT,CURRENT_FUTURE)
--         VALUES
--         ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'C');
----    Insert the record in the device table with current_future set to 'F'
--         INSERT INTO SM_PRIMARY_GEN
--         ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
--         VALUES
--         ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'F' );

--         INSERT INTO SM_PRIMARY_GEN_DTL
--        ( PRIMARY_GEN_ID,GENERATOR_TYPE,RATED_POWER_KVA,RATED_VOLT_KVLL,POWER_FACTOR_PERC, ACTIVE_GENERATION_KW )
--        ( select a.ID, ' ', 0,0,0,0
--            from SM_PRIMARY_GEN a
--           where a.current_future = 'C'
--             and a.global_id = I_Global_id_Current );

----    Insert a record in comments table with notes set to INST
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE,PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_GEN', I_Global_id_Current, sysdate, 'INST', 'SYSTEM', sysdate, 'New record from GIS system' );

-- Inserting new record in SM_GENERATION

         INSERT INTO SM_GENERATION
         (GLOBAL_ID, sap_egi_notification, project_name, gen_type, program_type, eff_rating_mach_kw, eff_rating_inv_kw, eff_rating_mach_kva, eff_rating_inv_kva, backup_generation, max_storage_capacity, charge_demand_kw, power_source,CURRENT_FUTURE,CREATEDBY,DATECREATED )
         VALUES
         (I_Global_id_Current,I_SAP_EGI_Notification,I_Project_Name,I_Gen_Type,I_Program_Type,I_Eff_Rating_Mach_KW,I_Eff_Rating_Inv_KW,I_Eff_Rating_Mach_KVA,I_Eff_Rating_Inv_KVA,I_Backup_Gen,I_Max_Storage_Capacity,I_Charge_Demand_KW,I_Power_Source,'C','EDSETT',sysdate);

        SELECT NVL(MAX(ID),0) INTO Var_ID FROM SM_GENERATION;
        --SELECT (NVL(MAX(ID),0)+1) INTO Var_ID_2 FROM SM_PROTECTION;

          -- INSERT New record in protection table
         INSERT INTO SM_PROTECTION
         (parent_type, parent_id, protection_type,CURRENT_FUTURE)
         VALUES
         ('GENERATION',Var_ID,'UNSP','C');

--    Insert a record in comments table with notes set to INST
         INSERT INTO SM_COMMENT_HIST
         ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE,PERFORMED_BY, ENTRY_DATE, COMMENTS )
         VALUES
         ( 'SM_GENERATION', I_Global_id_Current, sysdate, 'INST', 'SYSTEM', sysdate, 'New record from GIS system' );

  --Changes for ENOS to SAP migration - change detection End.
         IF VALID_NUM=6 THEN
            RAISE  INS_CODE_SIX;
         END IF;
      END;
   END IF;
   IF ACTION = 'U' THEN
      -- check to see if globalid exist in table. If it does not then throw exception
      BEGIN
      --Changes for ENOS to SAP migration - change detection Start.
      -- first copy the entire current record to history table
--         INSERT INTO SM_PRIMARY_GEN_HIST
--            ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
--              ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP,
--              GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE,
--              LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY,
--              PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET,
--              PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE,
--              RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY,
--              TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION,
--              RTU_MANF_CD,NOTES,DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION,
--              GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
--              PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING )
--         SELECT
--              ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
--              ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP,
--              GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE,
--              LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY,
--              PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET,
--              PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE,
--              RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY,
--              TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION,
--              RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION, GRD_BK_SOFTWARE_VERSION,
--              GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
--              PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--           FROM SM_PRIMARY_GEN
--          WHERE GLOBAL_ID= I_Global_id_Current;

--      -- Insert a record in comments table with notes set to INST
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--          VALUES
--         ( 'SM_PRIMARY_GEN', SM_PRIMARY_GEN_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate,'Record updated in GIS system');

--         UPDATE SM_PRIMARY_GEN
--            SET OPERATING_NUM = I_operating_num,
--                DIVISION        =I_Division,
--                DISTRICT        = I_District
--          WHERE GLOBAL_ID   = I_Global_id_Current;

---- Take only relevent fields // New Code - Uncomment it
        INSERT INTO sm_generation_hist
            ( BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,SM_GENERATION_ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE )
         SELECT
              BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE
          FROM sm_generation
          WHERE GLOBAL_ID= I_Global_id_Current;

-- Insert a record in comments table with notes set to INST
         INSERT INTO SM_COMMENT_HIST
         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
          VALUES
         ( 'SM_GENERATION', SM_PRIMARY_GEN_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate,'Record updated in GIS system');

 UPDATE SM_GENERATION
            SET sap_egi_notification = I_SAP_EGI_Notification,
                project_name        =I_Project_Name,
                gen_type        = I_Gen_Type,
                program_type=I_Program_Type,
                eff_rating_mach_kw = I_Eff_Rating_Mach_KW,
                eff_rating_inv_kw = I_Eff_Rating_Inv_KW,
                eff_rating_mach_kva = I_Eff_Rating_Mach_KVA,
                eff_rating_inv_kva = I_Eff_Rating_Inv_KVA,
                backup_generation = I_Backup_Gen,
                max_storage_capacity = I_Max_Storage_Capacity,
                charge_demand_kw =   I_Charge_Demand_KW,
                power_source = I_Power_Source
          WHERE GLOBAL_ID   = I_Global_id_Current;

 --Changes for ENOS to SAP migration - change detection End.

        -- Incase VALID NUM is assinged to 5 in validation section, then raise update exception
         IF VALID_NUM=5 THEN
            RAISE  UPD_CODE_FIVE;
         END IF;
      END;
   END IF;
   IF ACTION = 'D' THEN
      BEGIN
      -- first copy the entire current record to history table
       --Changes for ENOS to SAP migration - change detection Start.

      -- first copy the entire current record to history table
--         INSERT INTO SM_PRIMARY_GEN_HIST
--             ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
--               ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID,
--               GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
--               GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM,
--               OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE,
--               PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD,
--               RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
--               SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM,
--               RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES,DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM,
--               GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM,
--               PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM,
--               PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--             )
--         SELECT ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT,
--                ENGINEERING_COMMENTS, FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID,
--                GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
--                GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM,
--                OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE,
--                PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG, RADIO_MANF_CD,
--                RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
--                SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM,
--                RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM,
--                GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM,
--                PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM,
--                PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--           FROM SM_PRIMARY_GEN
--          WHERE GLOBAL_ID= I_Global_id_Current;
--
--         DELETE FROM SM_PRIMARY_GEN_DTL WHERE PRIMARY_GEN_ID = ( select  ID from SM_PRIMARY_GEN
--                                                                  where GLOBAL_ID = I_Global_id_current
--                                                                    AND CURRENT_FUTURE= 'C' ) ;
--
--         DELETE FROM SM_PRIMARY_GEN WHERE GLOBAL_ID = I_Global_id_Current;

     --  Insert a record in comments table with notes set to INST

--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_GEN_HIST', SM_PRIMARY_GEN_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );


  INSERT INTO sm_generation_hist
            ( BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,SM_GENERATION_ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE )
         SELECT
              BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE
          FROM sm_generation
          WHERE GLOBAL_ID= I_Global_id_Current;

          -- start code to delete all related equipment

        -- ENOS system testing issue - after changing relationship bw sm_generator and sm_gen_equipment
		 delete from EDSETT.SM_GEN_EQUIPMENT  where GENERATOR_ID in (
			select A.ID from EDSETT.SM_GENERATOR A join EDSETT.SM_PROTECTION B on A.protection_id = B.ID
										join EDSETT.SM_GENERATION C on B.parent_id = C.ID
					where  A.GEN_TECH_CD  in ('INVE','INVI') and B.PARENT_TYPE='GENERATION' and  C.GLOBAL_ID = I_Global_id_Current
		  ) ;


		  delete from EDSETT.SM_GENERATOR where PROTECTION_ID in (
				select B.ID from  EDSETT.SM_PROTECTION B join EDSETT.SM_GENERATION C  on B.parent_id = C.ID
					where B.PARENT_TYPE='GENERATION' and C.GLOBAL_ID = I_Global_id_Current
		  );

		  delete from EDSETT.SM_PROTECTION where PARENT_TYPE='GENERATION' and parent_id in
        (select ID from EDSETT.SM_GENERATION where   GLOBAL_ID =I_Global_id_Current  ) ;
   -- end code to delete all related equipment

   DELETE FROM SM_GENERATION WHERE GLOBAL_ID = I_Global_id_Current;

   INSERT INTO SM_COMMENT_HIST
         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
         VALUES
         ( 'SM_GENERATION', SM_GENERATION_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record deleted in GIS system' );


 --Changes for ENOS to SAP migration - change detection End.

         IF VALID_NUM=6 THEN
            RAISE  INS_CODE_SIX;
         END IF;
      END;
   END IF;
   IF ACTION = 'R' THEN
      BEGIN
      --Changes for ENOS to SAP migration - change detection Start.

--      -- First insert the record in the device table with current_future set to 'C'
--         INSERT
--         INTO SM_PRIMARY_GEN
--            ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
--         VALUES
--            ( I_Global_id_Current, I_feature_class_name, I_operating_num , I_Division , I_District, 'C' );
--
--      -- Insert the record in the device table with current_future set to 'F'
--         INSERT INTO SM_PRIMARY_GEN
--            ( GLOBAL_ID, FEATURE_CLASS_NAME, OPERATING_NUM, DIVISION, DISTRICT, CURRENT_FUTURE )
--         VALUES
--            ( I_Global_id_Current, I_feature_class_name, I_operating_num, I_Division, I_District, 'F' );
--      -- Insert a record in comments table with notes set to OTHR
--         INSERT INTO SM_COMMENT_HIST
--            ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--            ( 'SM_PRIMARY_GEN', I_Global_id_Current, sysdate, 'OTHR', 'SYSTEM', sysdate, 'New record from GIS system' );
--
--         UPDATE SM_PRIMARY_GEN A SET
--           ( ANNUAL_LF, BAUD_RATE, CC_RATING, DPA_CD, ENGINEERING_COMMENTS, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE,
--             GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
--             GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPS_TO_LOCKOUT,
--             PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP,
--             PHA_PR_RELAY_TYPE, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE,
--             SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT, RTU_EXIST,
--             RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP,
--             GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION,
--             PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION,
--             PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--           )
--         = ( SELECT ANNUAL_LF, BAUD_RATE, CC_RATING, DPA_CD, ENGINEERING_COMMENTS, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE,
--                    GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP, GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP,
--                    GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT, NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPS_TO_LOCKOUT,
--                    PHA_BK_INS_TRIP, PHA_BK_LEVER_SET, PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP,
--                    PHA_PR_RELAY_TYPE, RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE,
--                    SPECIAL_CONDITIONS, SUMMER_LOAD_LIMIT, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT, RTU_EXIST,
--                    RTU_MODEL_NUM, RTU_SERIAL_NUM, RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES, DIRECT_TRANSFER_TRIP,
--                    GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION,
--                    PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION, PHA_BK_SOFTWARE_VERSION,
--                    PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--               FROM SM_PRIMARY_GEN B
--              WHERE GLOBAL_ID   = I_Global_id_Previous
--                AND b.CURRENT_FUTURE=a.CURRENT_FUTURE
--           )
--          WHERE GLOBAL_ID = I_Global_id_Current;
--
--         INSERT INTO SM_PRIMARY_GEN_HIST
--         ( ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT, ENGINEERING_COMMENTS,
--           FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP,
--           GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT,
--           NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET,
--           PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG,
--           RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
--           SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM,
--           RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD, NOTES, DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,
--           GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
--           PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--         )
--         SELECT
--           ANNUAL_LF, BAUD_RATE, CC_RATING, CURRENT_FUTURE, DATE_MODIFIED, DEVICE_ID, DISTRICT, DIVISION, DPA_CD, EFFECTIVE_DT, ENGINEERING_COMMENTS,
--           FEATURE_CLASS_NAME, FLISR, FLISR_ENGINEERING_COMMENTS, FLISR_OPERATING_MODE, GLOBAL_ID, GRD_BK_INS_TRIP, GRD_BK_LEVER_SET, GRD_BK_MIN_TRIP,
--           GRD_BK_RELAY_TYPE, GRD_PR_INS_TRIP, GRD_PR_LEVER_SET, GRD_PR_MIN_TRIP, GRD_PR_RELAY_TYPE, LIMITING_FACTOR, MASTER_STATION, MIN_NOR_VOLT,
--           NETWORK, OK_TO_BYPASS, OPERATING_MODE, OPERATING_NUM, OPS_TO_LOCKOUT, PEER_REVIEW_BY, PEER_REVIEW_DT, PHA_BK_INS_TRIP, PHA_BK_LEVER_SET,
--           PHA_BK_MIN_TRIP, PHA_BK_RELAY_TYPE, PHA_PR_INS_TRIP, PHA_PR_LEVER_SET, PHA_PR_MIN_TRIP, PHA_PR_RELAY_TYPE, PREPARED_BY, PROCESSED_FLAG,
--           RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, RELAY_TYPE, RELEASED_BY, REPEATER, RTU_ADDRESS, SCADA, SCADA_TYPE, SPECIAL_CONDITIONS,
--           SUMMER_LOAD_LIMIT, TIMESTAMP, TRANSMIT_DISABLE_DELAY, TRANSMIT_ENABLE_DELAY, WINTER_LOAD_LIMIT,RTU_EXIST, RTU_MODEL_NUM, RTU_SERIAL_NUM,
--           RTU_FIRMWARE_VERSION, RTU_SOFTWARE_VERSION, RTU_MANF_CD,NOTES , DIRECT_TRANSFER_TRIP, GRD_BK_CONTROL_SERIAL_NUM, GRD_BK_FIRMWARE_VERSION,
--           GRD_BK_SOFTWARE_VERSION, GRD_PR_CONTROL_SERIAL_NUM, GRD_PR_FIRMWARE_VERSION, PHA_BK_CONTROL_SERIAL_NUM, PHA_BK_FIRMWARE_VERSION,
--           PHA_BK_SOFTWARE_VERSION, PHA_PR_CONTROL_SERIAL_NUM, PHA_PR_FIRMWARE_VERSION, PHA_PR_SOFTWARE_VERSION, RECLOSE_BLOCKING
--          FROM SM_PRIMARY_GEN
--         WHERE GLOBAL_ID= I_Global_id_Current;
--
--         INSERT INTO SM_COMMENT_HIST
--         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
--         VALUES
--         ( 'SM_PRIMARY_GEN_HIST', SM_PRIMARY_GEN_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );
--
--
--         Update SM_PRIMARY_GEN_DTL set (PRIMARY_GEN_ID) = ( select ID from SM_PRIMARY_GEN
--                                                             where GLOBAL_ID = I_Global_id_current
--                                                               AND CURRENT_FUTURE= 'C' )
--          WHERE ID = ( select ID from SM_PRIMARY_GEN
--                        where GLOBAL_ID = I_Global_id_Previous
--                          And CURRENT_FUTURE= 'C' );
--
--         DELETE FROM SM_PRIMARY_GEN WHERE GLOBAL_ID = I_Global_id_Previous;

  -- Inserting new record in SM_GENERATION
         INSERT INTO SM_GENERATION
         (GLOBAL_ID, sap_egi_notification, project_name, gen_type, program_type, eff_rating_mach_kw, eff_rating_inv_kw, eff_rating_mach_kva, eff_rating_inv_kva, backup_generation, max_storage_capacity, charge_demand_kw, power_source,CURRENT_FUTURE,CREATEDBY,DATECREATED )
         VALUES
         (I_Global_id_Current,I_SAP_EGI_Notification,I_Project_Name,I_Gen_Type,I_Program_Type,I_Eff_Rating_Mach_KW,I_Eff_Rating_Inv_KW,I_Eff_Rating_Mach_KVA,I_Eff_Rating_Inv_KVA,I_Backup_Gen,I_Max_Storage_Capacity,I_Charge_Demand_KW,I_Power_Source,'C','EDSETT',sysdate);


         -- Insert a record in comments table with notes set to OTHR
         INSERT INTO SM_COMMENT_HIST
            ( DEVICE_TABLE_NAME, GLOBAL_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
         VALUES
            ( 'SM_GENERATION', I_Global_id_Current, sysdate, 'OTHR', 'SYSTEM', sysdate, 'New record from GIS system' );


      UPDATE SM_GENERATION A SET
           ( sap_egi_notification, project_name, gen_type, program_type, eff_rating_mach_kw, eff_rating_inv_kw, eff_rating_mach_kva, eff_rating_inv_kva, backup_generation, max_storage_capacity, charge_demand_kw, power_source,CURRENT_FUTURE
           )
         = ( SELECT sap_egi_notification, project_name, gen_type, program_type, eff_rating_mach_kw, eff_rating_inv_kw, eff_rating_mach_kva, eff_rating_inv_kva, backup_generation, max_storage_capacity, charge_demand_kw, power_source,CURRENT_FUTURE
               FROM SM_GENERATION B
              WHERE GLOBAL_ID   = I_Global_id_Previous
           )
          WHERE GLOBAL_ID = I_Global_id_Current;

        INSERT INTO sm_generation_hist
            ( BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,SM_GENERATION_ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE )
         SELECT
              BACKUP_GENERATION,CHARGE_DEMAND_KW,CREATEDBY,DATECREATED,DATE_MODIFIED,DIRECT_TRANSFER_TRIP,EFF_RATING_INV_KVA,
              EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_MACH_KW,EXPORT_KW,GEN_TYPE,GRD_FAULT_DETECTION_CD,GLOBAL_ID,ID,
              MAX_STORAGE_CAPACITY,MODIFIEDBY,NOTES,POWER_SOURCE,PROGRAM_TYPE,PROJECT_NAME,SAP_EGI_NOTIFICATION,CURRENT_FUTURE
          FROM sm_generation
          WHERE GLOBAL_ID= I_Global_id_Current;


           INSERT INTO SM_COMMENT_HIST
         ( DEVICE_HIST_TABLE_NAME, HIST_ID, WORK_DATE, WORK_TYPE, PERFORMED_BY, ENTRY_DATE, COMMENTS )
         VALUES
         ( 'SM_GENERATION_HIST', SM_GENERATION_HIST_SEQ.NEXTVAL, sysdate, 'OTHR', 'SYSTEM', sysdate, 'Record updated in GIS system' );



-- start coded written to update related equipment
		update SM_PROTECTION set parent_id = (select ID from SM_GENERATION WHERE GLOBAL_ID = I_Global_id_Current )
			where parent_id = (select ID from SM_GENERATION WHERE GLOBAL_ID = I_Global_id_Previous ) ;
    -- end coded written to update related equipment

         DELETE FROM SM_GENERATION WHERE GLOBAL_ID = I_Global_id_Previous;

--Changes for ENOS to SAP migration - change detection End.

         IF VALID_NUM=6 THEN
            RAISE  INS_CODE_SIX;
         END IF;

         IF VALID_NUM=5 THEN
            RAISE  UPD_CODE_FIVE;
         END IF;
      END;
   END IF;
END SP_PRIMARY_GEN_DETECTION;

--    Change for project GIS powerbase integration - Start
--PROCEDURE SP_SWITCH_MSO_DETECTION(
--   I_Global_id_Current  IN VARCHAR2,
--    I_reason_type        IN VARCHAR2,
--    I_feature_class_name IN VARCHAR2,
--    I_feature_class_subtype IN NUMBER,
--    I_operating_num      IN VARCHAR2,
--    I_Global_id_Previous IN VARCHAR2,
--    I_Division           IN VARCHAR2,
--    I_District           IN VARCHAR2,
--    I_Control_type_code    IN VARCHAR2,
--    I_Switch_type_code IN VARCHAR2,
--    I_Bank_code          IN NUMBER)
--
--AS
--  REASON_TYPE CHAR;
--  DEVICE_TYPE VARCHAR2(50);
--  NUM1        NUMBER;
--  NUM2        NUMBER;
--  VALID_NUM   NUMBER;
--  VAR1        VARCHAR2(50);
--  ACTION      CHAR;
--BEGIN
--
--  REASON_TYPE   := I_reason_type ;
--  VALID_NUM:=0;
--
----- Validatations for I/U/D/R conditions
--CASE
--WHEN REASON_TYPE = 'I' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 5, else Insert the record and  return error code 0
--
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID = I_Global_id_Current;
--
--
--      IF NUM1  >= 1 THEN
--        ACTION  :='U';
--        VALID_NUM:=5;
--      ELSE
--       ACTION:='I';
--     END IF;
--END;
--
--WHEN REASON_TYPE = 'U' THEN
--BEGIN
----Check if a row already exists then update it with the parameter data,  return an error code of 0, else Insert the record and  return error code 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--       IF NUM1 < 1  THEN
--          ACTION := 'I';
--          VALID_NUM:=6;
--        ELSE
--          ACTION:='U';
--       END IF;
--END;
--
--WHEN REASON_TYPE = 'D' THEN
--BEGIN
----Check if a row already exists then delete it,  return an error code of 0,ELSE return an error code of 6
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
--
--      IF NUM1 < 1  THEN
--        VALID_NUM:=6;
--        ACTION :='D';
--      ELSE
--      ACTION := 'D';
--      END IF;
--END;
--
--WHEN REASON_TYPE = 'R' THEN
--BEGIN
---- Check if the previous global id exist in device table
--      SELECT COUNT(*)
--      INTO NUM2
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID   = I_Global_id_Previous
--      AND CURRENT_FUTURE='C';
--      -- Check if the current global id does not exist in device table
--      SELECT COUNT(*)
--      INTO NUM1
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID   = I_Global_id_Current
--      AND CURRENT_FUTURE='C';
-- -- If original(previous) NUM2= exists and new(current)NUM1 = does not exist, then replace with current data
--
--      IF (NUM2 >=1  AND NUM1 < 1 ) THEN
--        ACTION := 'R';
--        VALID_NUM:=0;
--      END IF;
---- If original(previous) NUM2= exists and new(current)NUM1 = exist, then update current record with parameters data
--      IF (NUM2 >=1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2 = does not exists and new(current)NUM1 = exist, then update the current record with parameters data
--      IF (NUM2 <1  AND NUM1 >=1 ) THEN
--        ACTION := 'U';
--        VALID_NUM:=5;
--      END IF;
---- If original(previous) NUM2= does not exists and new(current)NUM1 = does not exist, then Insert record  with parameters data
--      IF (NUM2 <1  AND NUM1 <1 ) THEN
--        ACTION := 'I';
--        VALID_NUM:=6;
--      END IF;
--END;
--END CASE;
--
--
--
--
--  IF ACTION = 'I' THEN
--    BEGIN
--      -- First insert the record in the device table with current_future set to 'C'
--      INSERT
--      INTO SM_SWITCH_MSO
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'C'
--        );
--
--      -- Insert the record in the device table with current_future set to 'F'
--      INSERT
--      INTO SM_SWITCH_MSO
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_MSO',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'New record from GIS system'
--        );
--
--    IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--    END;
--  END IF;
--
--
--  IF ACTION= 'U' THEN
--    -- check to see if globalid exist in table. If it does not then throw exception
--    BEGIN
--
--
--      -- first copy the entire current record to history table
--
-- INSERT INTO SM_SWITCH_MSO_HIST (ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DEVICE_ID,DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,
--GROUND_SCADA_IDENTI_A,GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,PEER_REVIEW_BY,
--PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,
--PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,RESIDUAL_GR_INST_50G,
--RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,
--SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,
--TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR)
--SELECT  ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DEVICE_ID,DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,
--GROUND_SCADA_IDENTI_A,GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,PEER_REVIEW_BY,
--PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,
--PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,RESIDUAL_GR_INST_50G,
--RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,
--SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,
--TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR FROM SM_SWITCH_MSO WHERE GLOBAL_ID=I_Global_id_Current;
--
--
--
--
--      -- Insert a record in comments table with notes set to INST
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--
--          'SM_SWITCH_MSO_HIST',
--           SM_SWITCH_MSO_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--      UPDATE SM_SWITCH_MSO
--      SET OPERATING_NUM  = I_operating_num, DIVISION=I_Division,DISTRICT= I_District,SWITCH_TYPE=I_Switch_type_code
--      WHERE GLOBAL_ID    = I_Global_id_Current;
--
--        IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--        END IF;
--
--    END;
--
--  END IF;
--
--  IF ACTION = 'D' THEN
--
--
-- INSERT INTO SM_SWITCH_MSO_HIST (ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DEVICE_ID,DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,
--GROUND_SCADA_IDENTI_A,GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,
--PEER_REVIEW_BY,PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,
--PHASE_TIME_DELAY,PREPARED_BY,PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,
--RESIDUAL_GR_INST_50G,RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,
--SCADA,SCADA_TYPE,SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,
--TIMESTAMP,TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR)
--SELECT      ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DEVICE_ID,DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,
--GROUND_SCADA_IDENTI_A,GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,
--PEER_REVIEW_BY,PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,
--PHASE_TIME_DELAY,PREPARED_BY,PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,
--RESIDUAL_GR_INST_50G,RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,
--SCADA,SCADA_TYPE,SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,
--TIMESTAMP,TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR  FROM SM_SWITCH_MSO WHERE GLOBAL_ID=I_Global_id_Current;
--
--     DELETE FROM SM_SWITCH_MSO WHERE GLOBAL_ID = I_Global_id_Current;
--
--      -- Insert a record in comments table with notes set to INST
--
--      -- Insert a record in comments table with notes set to INst
--
--  INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_MSO_HIST',
--          SM_SWITCH_MSO_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--     IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--  END IF;
--
--
--
--
--    IF ACTION = 'R' THEN
--    BEGIN
--
-- -- First insert the record in the device table with current_future set to 'C'
--
--
--INSERT
--      INTO SM_SWITCH_MSO
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'C'
--        );
--
--      -- Insert the record in the device table with current_future set to 'F'
--
--      INSERT
--      INTO SM_SWITCH_MSO
--        (
--          GLOBAL_ID,
--          FEATURE_CLASS_NAME,
--          OPERATING_NUM,
--          DIVISION,
--          DISTRICT,
--          SWITCH_TYPE,
--          CURRENT_FUTURE
--        )
--        VALUES
--        (
--          I_Global_id_Current,
--          I_feature_class_name,
--          I_operating_num ,
--          I_Division ,
--          I_District,
--          I_Switch_type_code,
--          'F'
--        );
--      -- Insert a record in comments table with notes set to INST
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_TABLE_NAME,
--          GLOBAL_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_MSO',
--          I_Global_id_Current,
--          sysdate,
--          'INST',
--          'SYSTEM',
--          sysdate,
--          'Record replaced in GIS system' -- Old GlobalID: '||I_Global_id_Previous
--        );
--	  UPDATE SM_COMMENT_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--	  UPDATE SM_SWITCH_MSO_HIST SET GLOBAL_ID = I_Global_id_Current WHERE GLOBAL_ID = I_Global_id_Previous;
--
--
--UPDATE SM_SWITCH_MSO SET (ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GROUND_SCADA_IDENTI_A,
--GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,PEER_REVIEW_BY,
--PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,
--PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,RESIDUAL_GR_INST_50G,
--RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,
--SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,
--TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR)
-- =(SELECT ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,
--DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GROUND_SCADA_IDENTI_A,
--GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,OPERATING_NUM,PEER_REVIEW_BY,
--PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,
--PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,RESET_TIME,RESIDUAL_GR_INST_50G,
--RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,
--SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,
--TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR
--FROM SM_SWITCH_MSO  WHERE GLOBAL_ID   = I_Global_id_Previous   AND CURRENT_FUTURE='C' ) WHERE GLOBAL_ID = I_Global_id_Current;
--
---- first copy the entire previous record to history table
--
-- INSERT INTO SM_SWITCH_MSO_HIST (ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,DEVICE_ID,
--DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,GROUND_SCADA_IDENTI_A,
--GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,
--OPERATING_NUM,PEER_REVIEW_BY,PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,
--PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,
--RESET_TIME,RESIDUAL_GR_INST_50G,RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,
--RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,
--SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR)
--SELECT  ATS_CAPABLE,ATS_FEATURE,BAUD_RATE,CONTROL_SERIAL_NUM,CONTROL_UNIT_TYPE,CURRENT_FUTURE,CURRENT_RATIO_CTR,DATE_MODIFIED,DEVICE_ID,
--DISTRICT,DIVISION,EFFECTIVE_DT,FEATURE_CLASS_NAME,FIRMWARE_VERSION,FLISR,FLISR_ENGINEERING_COMMENTS,GLOBAL_ID,GROUND_SCADA_IDENTI_A,
--GROUND_TIME_DELAY,LIMITING_FACTOR,MASTER_STATION,OK_TO_BYPASS,OPERATING_MODE,
--OPERATING_NUM,PEER_REVIEW_BY,PEER_REVIEW_DT,PHASE_A_SCADA_IDENTI_A,PHASE_B_SCADA_IDENTI_A,PHASE_C_SCADA_IDENTI_A,
--PHASE_INST_50P,PHASE_TIME_DELAY,PREPARED_BY,PROCESSED_FLAG,RADIO_MANF_CD,RADIO_MODEL_NUM,RADIO_SERIAL_NUM,RELEASED_BY,REPEATER,
--RESET_TIME,RESIDUAL_GR_INST_50G,RTU_ADDRESS,RTU_EXIST,RTU_FIRMWARE_VERSION,RTU_MANF_CD,RTU_MODEL_NUM,RTU_SERIAL_NUM,
--RTU_SOFTWARE_VERSION,SCADA,SCADA_TYPE,SECTIONALIZING_FEATURE,SHOT_TO_LOCKOUT_SECT,SOFTWARE_VERSION,SPECIAL_CONDITIONS,
--SUMMER_LOAD_LIMIT,SWITCH_TYPE,TIMESTAMP,TRANSMIT_DISABLE_DELAY,TRANSMIT_ENABLE_DELAY,VOLTAGE_RATIO_VTR
--FROM SM_SWITCH_MSO WHERE GLOBAL_ID=I_Global_id_Previous;
--
--
--
--      INSERT
--      INTO SM_COMMENT_HIST
--        (
--          DEVICE_HIST_TABLE_NAME,
--          HIST_ID,
--          WORK_DATE,
--          WORK_TYPE,
--          PERFORMED_BY,
--          ENTRY_DATE,
--          COMMENTS
--        )
--        VALUES
--        (
--          'SM_SWITCH_MSO_HIST',
--          SM_SWITCH_MSO_HIST_SEQ.NEXTVAL,
--          sysdate,
--          'OTHR',
--          'SYSTEM',
--          sysdate,
--          'Record updated in GIS system'
--        );
--
--
--      -- Remove previous global id from te device table
--
--      DELETE
--      FROM SM_SWITCH_MSO
--      WHERE GLOBAL_ID = I_Global_id_Previous;
--
--    IF VALID_NUM=6 THEN
--        RAISE  INS_CODE_SIX;
--      END IF;
--
--      IF VALID_NUM=5 THEN
--        RAISE  UPD_CODE_FIVE;
--      END IF;
--    END;
--  END IF;
--
--END SP_SWITCH_MSO_DETECTION;
--    Change for project GIS powerbase integration - End

END SM_CHANGE_DETECTION_PKG;

/

spool off
