Prompt drop Trigger PGE_CD_CAPAC_UPDATE;
DROP TRIGGER PGEDATA.PGE_CD_CAPAC_UPDATE
/

Prompt Trigger PGE_CD_CAPAC_UPDATE;
--
-- PGE_CD_CAPAC_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_CAPAC_UPDATE AFTER UPDATE on PGEDATA.PGEDATA_SM_CAPACITOR_EAD REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_CAPACITOR_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	 
     IF(  UPDATING(  'AUTO_BVR_CALC'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'AUTO_BVR_CALC'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."AUTO_BVR_CALC"  ,  -1  )  <>  NVL(  :old."AUTO_BVR_CALC"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.AUTO_BVR_CALC  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BAUD_RATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'BAUD_RATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAUD_RATE"  ,  -1  )  <>  NVL(  :old."BAUD_RATE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.BAUD_RATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROLLER_UNIT_MODEL'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'CONTROLLER_UNIT_MODEL'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROLLER_UNIT_MODEL"  ,  -1  )  <>  NVL(  :old."CONTROLLER_UNIT_MODEL"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.CONTROLLER_UNIT_MODEL  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'CONTROL_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_TYPE"  ,  -1  )  <>  NVL(  :old."CONTROL_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.CONTROL_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CURRENT_FUTURE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'CURRENT_FUTURE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CURRENT_FUTURE"  ,  -1  )  <>  NVL(  :old."CURRENT_FUTURE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.CURRENT_FUTURE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATA_LOGGING_INTERVAL'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DATA_LOGGING_INTERVAL'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DATA_LOGGING_INTERVAL"  ,  -1  )  <>  NVL(  :old."DATA_LOGGING_INTERVAL"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.DATA_LOGGING_INTERVAL  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DAYLIGHT_SAVINGS_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DAYLIGHT_SAVINGS_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DAYLIGHT_SAVINGS_TIME"  ,  -1  )  <>  NVL(  :old."DAYLIGHT_SAVINGS_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.DAYLIGHT_SAVINGS_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DEVICE_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DEVICE_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DEVICE_ID"  ,  -1  )  <>  NVL(  :old."DEVICE_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.DEVICE_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DISTRICT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DISTRICT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DISTRICT"  ,  -1  )  <>  NVL(  :old."DISTRICT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.DISTRICT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIVISION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DIVISION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIVISION"  ,  -1  )  <>  NVL(  :old."DIVISION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.DIVISION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EST_BANK_VOLTAGE_RISE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'EST_BANK_VOLTAGE_RISE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."EST_BANK_VOLTAGE_RISE"  ,  -1  )  <>  NVL(  :old."EST_BANK_VOLTAGE_RISE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.EST_BANK_VOLTAGE_RISE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EST_VOLTAGE_CHANGE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'EST_VOLTAGE_CHANGE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."EST_VOLTAGE_CHANGE"  ,  -1  )  <>  NVL(  :old."EST_VOLTAGE_CHANGE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.EST_VOLTAGE_CHANGE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FEATURE_CLASS_NAME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'FEATURE_CLASS_NAME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FEATURE_CLASS_NAME"  ,  -1  )  <>  NVL(  :old."FEATURE_CLASS_NAME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.FEATURE_CLASS_NAME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR_ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'FLISR_ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.FLISR_ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GLOBAL_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'GLOBAL_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GLOBAL_ID"  ,  -1  )  <>  NVL(  :old."GLOBAL_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.GLOBAL_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'HIGH_VOLTAGE_OVERRIDE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'HIGH_VOLTAGE_OVERRIDE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."HIGH_VOLTAGE_OVERRIDE_SETPOINT"  ,  -1  )  <>  NVL(  :old."HIGH_VOLTAGE_OVERRIDE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.HIGH_VOLTAGE_OVERRIDE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ID"  ,  -1  )  <>  NVL(  :old."ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LOW_VOLTAGE_OVERRIDE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'LOW_VOLTAGE_OVERRIDE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LOW_VOLTAGE_OVERRIDE_SETPOINT"  ,  -1  )  <>  NVL(  :old."LOW_VOLTAGE_OVERRIDE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.LOW_VOLTAGE_OVERRIDE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_STATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'MASTER_STATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_STATION"  ,  -1  )  <>  NVL(  :old."MASTER_STATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.MASTER_STATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MAXCYCLES'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'MAXCYCLES'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MAXCYCLES"  ,  -1  )  <>  NVL(  :old."MAXCYCLES"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.MAXCYCLES  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MIN_SW_VOLTAGE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'MIN_SW_VOLTAGE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MIN_SW_VOLTAGE"  ,  -1  )  <>  NVL(  :old."MIN_SW_VOLTAGE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.MIN_SW_VOLTAGE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'OPERATING_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_NUM"  ,  -1  )  <>  NVL(  :old."OPERATING_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.OPERATING_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PEER_REVIEW_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEER_REVIEW_BY"  ,  -1  )  <>  NVL(  :old."PEER_REVIEW_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.PEER_REVIEW_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREFERED_BANK_POSITION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PREFERED_BANK_POSITION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREFERED_BANK_POSITION"  ,  -1  )  <>  NVL(  :old."PREFERED_BANK_POSITION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.PREFERED_BANK_POSITION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREPARED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PREPARED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREPARED_BY"  ,  -1  )  <>  NVL(  :old."PREPARED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.PREPARED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PROCESSED_FLAG'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PROCESSED_FLAG'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PROCESSED_FLAG"  ,  -1  )  <>  NVL(  :old."PROCESSED_FLAG"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.PROCESSED_FLAG  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PULSE_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PULSE_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PULSE_TIME"  ,  -1  )  <>  NVL(  :old."PULSE_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.PULSE_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RADIO_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MANF_CD"  ,  -1  )  <>  NVL(  :old."RADIO_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RADIO_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RADIO_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RADIO_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RADIO_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RADIO_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELEASED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RELEASED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELEASED_BY"  ,  -1  )  <>  NVL(  :old."RELEASED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RELEASED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REPEATER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'REPEATER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REPEATER"  ,  -1  )  <>  NVL(  :old."REPEATER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.REPEATER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_ADDRESS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_ADDRESS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_ADDRESS"  ,  -1  )  <>  NVL(  :old."RTU_ADDRESS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_ADDRESS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_EXIST'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_EXIST'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_EXIST"  ,  -1  )  <>  NVL(  :old."RTU_EXIST"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_EXIST  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MANF_CD"  ,  -1  )  <>  NVL(  :old."RTU_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'RTU_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.RTU_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCADA'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA"  ,  -1  )  <>  NVL(  :old."SCADA"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCADA  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCADA_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA_TYPE"  ,  -1  )  <>  NVL(  :old."SCADA_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCADA_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_CONTROL_STRATEGY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_CONTROL_STRATEGY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_CONTROL_STRATEGY"  ,  -1  )  <>  NVL(  :old."SCH1_CONTROL_STRATEGY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_CONTROL_STRATEGY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_HIGH_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_HIGH_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH1_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_HIGH_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_HOLIDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_HOLIDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_HOLIDAYS"  ,  -1  )  <>  NVL(  :old."SCH1_HOLIDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_HOLIDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_KVAR_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_KVAR_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_KVAR_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH1_KVAR_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_KVAR_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_KVAR_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_KVAR_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_KVAR_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH1_KVAR_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_KVAR_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_LOW_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_LOW_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_LOW_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH1_LOW_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_LOW_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_SATURDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_SATURDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_SATURDAY"  ,  -1  )  <>  NVL(  :old."SCH1_SATURDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_SATURDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_SCHEDULE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_SCHEDULE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_SCHEDULE"  ,  -1  )  <>  NVL(  :old."SCH1_SCHEDULE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_SCHEDULE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_SUNDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_SUNDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_SUNDAY"  ,  -1  )  <>  NVL(  :old."SCH1_SUNDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_SUNDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_TEMP_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_TEMP_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_TEMP_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH1_TEMP_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_TEMP_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_TEMP_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_TEMP_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_TEMP_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH1_TEMP_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_TEMP_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_TIME_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_TIME_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_TIME_OFF"  ,  -1  )  <>  NVL(  :old."SCH1_TIME_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_TIME_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_TIME_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_TIME_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_TIME_ON"  ,  -1  )  <>  NVL(  :old."SCH1_TIME_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_TIME_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_WEEKDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_WEEKDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH1_WEEKDAYS"  ,  -1  )  <>  NVL(  :old."SCH1_WEEKDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH1_WEEKDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_CONTROL_STRATEGY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_CONTROL_STRATEGY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_CONTROL_STRATEGY"  ,  -1  )  <>  NVL(  :old."SCH2_CONTROL_STRATEGY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_CONTROL_STRATEGY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_HIGH_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_HIGH_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH2_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_HIGH_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_HOLIDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_HOLIDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_HOLIDAYS"  ,  -1  )  <>  NVL(  :old."SCH2_HOLIDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_HOLIDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_KVAR_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_KVAR_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_KVAR_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH2_KVAR_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_KVAR_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_KVAR_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_KVAR_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_KVAR_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH2_KVAR_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_KVAR_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_LOW_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_LOW_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_LOW_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH2_LOW_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_LOW_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_SATURDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_SATURDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_SATURDAY"  ,  -1  )  <>  NVL(  :old."SCH2_SATURDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_SATURDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_SCHEDULE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_SCHEDULE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_SCHEDULE"  ,  -1  )  <>  NVL(  :old."SCH2_SCHEDULE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_SCHEDULE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_SUNDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_SUNDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_SUNDAY"  ,  -1  )  <>  NVL(  :old."SCH2_SUNDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_SUNDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_TEMP_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_TEMP_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_TEMP_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH2_TEMP_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_TEMP_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_TEMP_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_TEMP_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_TEMP_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH2_TEMP_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_TEMP_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_TIME_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_TIME_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_TIME_OFF"  ,  -1  )  <>  NVL(  :old."SCH2_TIME_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_TIME_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_TIME_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_TIME_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_TIME_ON"  ,  -1  )  <>  NVL(  :old."SCH2_TIME_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_TIME_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_WEEKDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_WEEKDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH2_WEEKDAYS"  ,  -1  )  <>  NVL(  :old."SCH2_WEEKDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH2_WEEKDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_CONTROL_STRATEGY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_CONTROL_STRATEGY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_CONTROL_STRATEGY"  ,  -1  )  <>  NVL(  :old."SCH3_CONTROL_STRATEGY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_CONTROL_STRATEGY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_HIGH_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_HIGH_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH3_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_HIGH_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_HOLIDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_HOLIDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_HOLIDAYS"  ,  -1  )  <>  NVL(  :old."SCH3_HOLIDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_HOLIDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_KVAR_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_KVAR_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_KVAR_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH3_KVAR_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_KVAR_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_KVAR_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_KVAR_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_KVAR_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH3_KVAR_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_KVAR_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_LOW_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_LOW_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_LOW_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH3_LOW_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_LOW_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_SATURDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_SATURDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_SATURDAY"  ,  -1  )  <>  NVL(  :old."SCH3_SATURDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_SATURDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_SCHEDULE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_SCHEDULE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_SCHEDULE"  ,  -1  )  <>  NVL(  :old."SCH3_SCHEDULE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_SCHEDULE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_SUNDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_SUNDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_SUNDAY"  ,  -1  )  <>  NVL(  :old."SCH3_SUNDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_SUNDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_TEMP_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_TEMP_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_TEMP_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH3_TEMP_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_TEMP_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_TEMP_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_TEMP_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_TEMP_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH3_TEMP_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_TEMP_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_TIME_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_TIME_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_TIME_OFF"  ,  -1  )  <>  NVL(  :old."SCH3_TIME_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_TIME_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_TIME_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_TIME_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_TIME_ON"  ,  -1  )  <>  NVL(  :old."SCH3_TIME_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_TIME_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_WEEKDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_WEEKDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH3_WEEKDAYS"  ,  -1  )  <>  NVL(  :old."SCH3_WEEKDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH3_WEEKDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_CONTROL_STRATEGY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_CONTROL_STRATEGY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_CONTROL_STRATEGY"  ,  -1  )  <>  NVL(  :old."SCH4_CONTROL_STRATEGY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_CONTROL_STRATEGY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_HIGH_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_HIGH_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH4_HIGH_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_HIGH_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_HOLIDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_HOLIDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_HOLIDAYS"  ,  -1  )  <>  NVL(  :old."SCH4_HOLIDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_HOLIDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_KVAR_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_KVAR_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_KVAR_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH4_KVAR_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_KVAR_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_KVAR_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_KVAR_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_KVAR_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH4_KVAR_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_KVAR_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_LOW_VOLTAGE_SETPOINT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_LOW_VOLTAGE_SETPOINT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_LOW_VOLTAGE_SETPOINT"  ,  -1  )  <>  NVL(  :old."SCH4_LOW_VOLTAGE_SETPOINT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_LOW_VOLTAGE_SETPOINT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_SATURDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_SATURDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_SATURDAY"  ,  -1  )  <>  NVL(  :old."SCH4_SATURDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_SATURDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_SCHEDULE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_SCHEDULE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_SCHEDULE"  ,  -1  )  <>  NVL(  :old."SCH4_SCHEDULE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_SCHEDULE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_SUNDAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_SUNDAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_SUNDAY"  ,  -1  )  <>  NVL(  :old."SCH4_SUNDAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_SUNDAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_TEMP_SETPOINT_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_TEMP_SETPOINT_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_TEMP_SETPOINT_OFF"  ,  -1  )  <>  NVL(  :old."SCH4_TEMP_SETPOINT_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_TEMP_SETPOINT_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_TEMP_SETPOINT_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_TEMP_SETPOINT_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_TEMP_SETPOINT_ON"  ,  -1  )  <>  NVL(  :old."SCH4_TEMP_SETPOINT_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_TEMP_SETPOINT_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_TIME_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_TIME_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_TIME_OFF"  ,  -1  )  <>  NVL(  :old."SCH4_TIME_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_TIME_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_TIME_ON'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_TIME_ON'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_TIME_ON"  ,  -1  )  <>  NVL(  :old."SCH4_TIME_ON"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_TIME_ON  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_WEEKDAYS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_WEEKDAYS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCH4_WEEKDAYS"  ,  -1  )  <>  NVL(  :old."SCH4_WEEKDAYS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SCH4_WEEKDAYS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SPECIAL_CONDITIONS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SPECIAL_CONDITIONS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SPECIAL_CONDITIONS"  ,  -1  )  <>  NVL(  :old."SPECIAL_CONDITIONS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SPECIAL_CONDITIONS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SWITCH_POSITION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SWITCH_POSITION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SWITCH_POSITION"  ,  -1  )  <>  NVL(  :old."SWITCH_POSITION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.SWITCH_POSITION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TEMPERATURE_CHANGE_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TEMPERATURE_CHANGE_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TEMPERATURE_CHANGE_TIME"  ,  -1  )  <>  NVL(  :old."TEMPERATURE_CHANGE_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.TEMPERATURE_CHANGE_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TEMPERATURE_OVERRIDE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TEMPERATURE_OVERRIDE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TEMPERATURE_OVERRIDE"  ,  -1  )  <>  NVL(  :old."TEMPERATURE_OVERRIDE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.TEMPERATURE_OVERRIDE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIME_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TIME_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TIME_DELAY"  ,  -1  )  <>  NVL(  :old."TIME_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.TIME_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_DISABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TRANSMIT_DISABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_DISABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_DISABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.TRANSMIT_DISABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_ENABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TRANSMIT_ENABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_ENABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_ENABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.TRANSMIT_ENABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'VOLTAGE_CHANGE_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'VOLTAGE_CHANGE_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."VOLTAGE_CHANGE_TIME"  ,  -1  )  <>  NVL(  :old."VOLTAGE_CHANGE_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.VOLTAGE_CHANGE_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'VOLTAGE_OVERRIDE_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'VOLTAGE_OVERRIDE_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."VOLTAGE_OVERRIDE_TIME"  ,  -1  )  <>  NVL(  :old."VOLTAGE_OVERRIDE_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.VOLTAGE_OVERRIDE_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'VOLT_VAR_TEAM_MEMBER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'VOLT_VAR_TEAM_MEMBER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."VOLT_VAR_TEAM_MEMBER"  ,  -1  )  <>  NVL(  :old."VOLT_VAR_TEAM_MEMBER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CAPACITOR_EAD.VOLT_VAR_TEAM_MEMBER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATE_MODIFIED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'DATE_MODIFIED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."DATE_MODIFIED",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."DATE_MODIFIED",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.DATE_MODIFIED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EFFECTIVE_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'EFFECTIVE_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."EFFECTIVE_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."EFFECTIVE_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.EFFECTIVE_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'PEER_REVIEW_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.PEER_REVIEW_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_END_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_END_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH1_END_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH1_END_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH1_END_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH1_START_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH1_START_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH1_START_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH1_START_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH1_START_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_END_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_END_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH2_END_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH2_END_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH2_END_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH2_START_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH2_START_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH2_START_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH2_START_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH2_START_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_END_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_END_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH3_END_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH3_END_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH3_END_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH3_START_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH3_START_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH3_START_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH3_START_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH3_START_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_END_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_END_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH4_END_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH4_END_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH4_END_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCH4_START_DATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'SCH4_START_DATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."SCH4_START_DATE",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."SCH4_START_DATE",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.SCH4_START_DATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMESTAMP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  'TIMESTAMP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."TIMESTAMP",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( TO_CHAR( :old."TIMESTAMP",'YYYYMMDDHH24MISSFF') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CAPACITOR_EAD.TIMESTAMP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CAPACITOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;  
END;
/
