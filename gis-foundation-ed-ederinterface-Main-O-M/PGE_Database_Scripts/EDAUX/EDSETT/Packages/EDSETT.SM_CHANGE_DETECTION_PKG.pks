Prompt drop Package SM_CHANGE_DETECTION_PKG;
DROP PACKAGE EDSETT.SM_CHANGE_DETECTION_PKG
/

Prompt Package SM_CHANGE_DETECTION_PKG;
--
-- SM_CHANGE_DETECTION_PKG  (Package) 
--
CREATE OR REPLACE PACKAGE EDSETT."SM_CHANGE_DETECTION_PKG" AS

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
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code          IN NUMBER);


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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);

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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);


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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);


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

    PROCEDURE SP_RECLOSER_TS_DETECTION
  (
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER
  );
  PROCEDURE SP_RECLOSER_FS_DETECTION
  (
    I_Global_id_Current     IN VARCHAR2,
    I_reason_type           IN VARCHAR2,
    I_feature_class_name    IN VARCHAR2,
    I_feature_class_subtype IN NUMBER,
    I_operating_num         IN VARCHAR2,
    I_Global_id_Previous    IN VARCHAR2,
    I_Division              IN VARCHAR2,
    I_District              IN VARCHAR2,
    I_Switch_type_code      IN VARCHAR2,
    I_Bank_code             IN NUMBER
  );

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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);


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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);


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
    I_Switch_type_code IN VARCHAR2,
    I_Bank_code          IN NUMBER);

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


    PROCEDURE SP_PRIMARY_METER_DETECTION(
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
    I_Bank_code             IN NUMBER);

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

END SM_CHANGE_DETECTION_PKG;
/
