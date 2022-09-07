Prompt drop Trigger PGE_CD_REGUL_UPDATE;
DROP TRIGGER PGEDATA.PGE_CD_REGUL_UPDATE
/

Prompt Trigger PGE_CD_REGUL_UPDATE;
--
-- PGE_CD_REGUL_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_REGUL_UPDATE AFTER UPDATE on PGEDATA.PGEDATA_SM_REGULATOR_EAD REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_REGULATOR_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	 
     IF(  UPDATING(  'BAND_WIDTH'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'BAND_WIDTH'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAND_WIDTH"  ,  -1  )  <>  NVL(  :old."BAND_WIDTH"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.BAND_WIDTH  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BANK_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'BANK_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BANK_CD"  ,  -1  )  <>  NVL(  :old."BANK_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.BANK_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BAUD_RATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'BAUD_RATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAUD_RATE"  ,  -1  )  <>  NVL(  :old."BAUD_RATE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.BAUD_RATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BLOCKED_PCT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'BLOCKED_PCT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BLOCKED_PCT"  ,  -1  )  <>  NVL(  :old."BLOCKED_PCT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.BLOCKED_PCT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'CONTROL_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_TYPE"  ,  -1  )  <>  NVL(  :old."CONTROL_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.CONTROL_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CURRENT_FUTURE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'CURRENT_FUTURE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CURRENT_FUTURE"  ,  -1  )  <>  NVL(  :old."CURRENT_FUTURE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.CURRENT_FUTURE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DEVICE_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'DEVICE_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DEVICE_ID"  ,  -1  )  <>  NVL(  :old."DEVICE_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.DEVICE_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DISTRICT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'DISTRICT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DISTRICT"  ,  -1  )  <>  NVL(  :old."DISTRICT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.DISTRICT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIVISION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'DIVISION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIVISION"  ,  -1  )  <>  NVL(  :old."DIVISION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.DIVISION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EMERGENCY_ONLY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'EMERGENCY_ONLY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."EMERGENCY_ONLY"  ,  -1  )  <>  NVL(  :old."EMERGENCY_ONLY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.EMERGENCY_ONLY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FEATURE_CLASS_NAME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FEATURE_CLASS_NAME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FEATURE_CLASS_NAME"  ,  -1  )  <>  NVL(  :old."FEATURE_CLASS_NAME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FEATURE_CLASS_NAME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR_ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FLISR_ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FLISR_ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_A_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_A_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_A_RESET"  ,  -1  )  <>  NVL(  :old."FWD_A_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_A_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_A_STATUS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_A_STATUS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_A_STATUS"  ,  -1  )  <>  NVL(  :old."FWD_A_STATUS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_A_STATUS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_A_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_A_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_A_VOLT"  ,  -1  )  <>  NVL(  :old."FWD_A_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_A_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_A_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_A_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_A_XSET"  ,  -1  )  <>  NVL(  :old."FWD_A_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_A_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_B_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_B_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_B_RESET"  ,  -1  )  <>  NVL(  :old."FWD_B_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_B_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_B_STATUS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_B_STATUS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_B_STATUS"  ,  -1  )  <>  NVL(  :old."FWD_B_STATUS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_B_STATUS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_B_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_B_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_B_VOLT"  ,  -1  )  <>  NVL(  :old."FWD_B_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_B_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_B_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_B_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_B_XSET"  ,  -1  )  <>  NVL(  :old."FWD_B_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_B_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_C_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_C_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_C_RESET"  ,  -1  )  <>  NVL(  :old."FWD_C_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_C_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_C_STATUS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_C_STATUS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_C_STATUS"  ,  -1  )  <>  NVL(  :old."FWD_C_STATUS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_C_STATUS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_C_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_C_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_C_VOLT"  ,  -1  )  <>  NVL(  :old."FWD_C_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_C_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FWD_C_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'FWD_C_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FWD_C_XSET"  ,  -1  )  <>  NVL(  :old."FWD_C_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.FWD_C_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GLOBAL_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'GLOBAL_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GLOBAL_ID"  ,  -1  )  <>  NVL(  :old."GLOBAL_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.GLOBAL_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'HIGH_VOLTAGE_LIMIT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'HIGH_VOLTAGE_LIMIT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."HIGH_VOLTAGE_LIMIT"  ,  -1  )  <>  NVL(  :old."HIGH_VOLTAGE_LIMIT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.HIGH_VOLTAGE_LIMIT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ID"  ,  -1  )  <>  NVL(  :old."ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LOAD_CYCLE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'LOAD_CYCLE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LOAD_CYCLE"  ,  -1  )  <>  NVL(  :old."LOAD_CYCLE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.LOAD_CYCLE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LOW_VOLTAGE_LIMIT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'LOW_VOLTAGE_LIMIT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LOW_VOLTAGE_LIMIT"  ,  -1  )  <>  NVL(  :old."LOW_VOLTAGE_LIMIT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.LOW_VOLTAGE_LIMIT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_STATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'MASTER_STATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_STATION"  ,  -1  )  <>  NVL(  :old."MASTER_STATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.MASTER_STATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MIN_LOAD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'MIN_LOAD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MIN_LOAD"  ,  -1  )  <>  NVL(  :old."MIN_LOAD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.MIN_LOAD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MODE"  ,  -1  )  <>  NVL(  :old."MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OK_TO_BYPASS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'OK_TO_BYPASS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OK_TO_BYPASS"  ,  -1  )  <>  NVL(  :old."OK_TO_BYPASS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.OK_TO_BYPASS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'OPERATING_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_NUM"  ,  -1  )  <>  NVL(  :old."OPERATING_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.OPERATING_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEAK_LOAD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PEAK_LOAD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEAK_LOAD"  ,  -1  )  <>  NVL(  :old."PEAK_LOAD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PEAK_LOAD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PEER_REVIEW_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEER_REVIEW_BY"  ,  -1  )  <>  NVL(  :old."PEER_REVIEW_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PEER_REVIEW_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'POWER_FACTOR'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'POWER_FACTOR'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."POWER_FACTOR"  ,  -1  )  <>  NVL(  :old."POWER_FACTOR"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.POWER_FACTOR  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREPARED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PREPARED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREPARED_BY"  ,  -1  )  <>  NVL(  :old."PREPARED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PREPARED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PRIMARY_CT_RATING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PRIMARY_CT_RATING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PRIMARY_CT_RATING"  ,  -1  )  <>  NVL(  :old."PRIMARY_CT_RATING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PRIMARY_CT_RATING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PROCESSED_FLAG'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PROCESSED_FLAG'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PROCESSED_FLAG"  ,  -1  )  <>  NVL(  :old."PROCESSED_FLAG"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PROCESSED_FLAG  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PT_RATIO'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PT_RATIO'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PT_RATIO"  ,  -1  )  <>  NVL(  :old."PT_RATIO"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PT_RATIO  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PVD_MAX'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PVD_MAX'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PVD_MAX"  ,  -1  )  <>  NVL(  :old."PVD_MAX"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PVD_MAX  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PVD_MIN'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PVD_MIN'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PVD_MIN"  ,  -1  )  <>  NVL(  :old."PVD_MIN"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.PVD_MIN  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RADIO_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MANF_CD"  ,  -1  )  <>  NVL(  :old."RADIO_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RADIO_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RADIO_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RADIO_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RADIO_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RADIO_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RANGE_UNBLOCKED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RANGE_UNBLOCKED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RANGE_UNBLOCKED"  ,  -1  )  <>  NVL(  :old."RANGE_UNBLOCKED"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RANGE_UNBLOCKED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELEASED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RELEASED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELEASED_BY"  ,  -1  )  <>  NVL(  :old."RELEASED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RELEASED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REPEATER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REPEATER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REPEATER"  ,  -1  )  <>  NVL(  :old."REPEATER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REPEATER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REVERSIBLE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REVERSIBLE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REVERSIBLE"  ,  -1  )  <>  NVL(  :old."REVERSIBLE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REVERSIBLE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_A_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_A_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_A_RESET"  ,  -1  )  <>  NVL(  :old."REV_A_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_A_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_A_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_A_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_A_VOLT"  ,  -1  )  <>  NVL(  :old."REV_A_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_A_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_A_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_A_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_A_XSET"  ,  -1  )  <>  NVL(  :old."REV_A_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_A_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_B_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_B_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_B_RESET"  ,  -1  )  <>  NVL(  :old."REV_B_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_B_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_B_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_B_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_B_VOLT"  ,  -1  )  <>  NVL(  :old."REV_B_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_B_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_B_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_B_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_B_XSET"  ,  -1  )  <>  NVL(  :old."REV_B_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_B_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_C_RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_C_RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_C_RESET"  ,  -1  )  <>  NVL(  :old."REV_C_RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_C_RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_C_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_C_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_C_VOLT"  ,  -1  )  <>  NVL(  :old."REV_C_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_C_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_C_XSET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_C_XSET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_C_XSET"  ,  -1  )  <>  NVL(  :old."REV_C_XSET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_C_XSET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REV_THRESHOLD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'REV_THRESHOLD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REV_THRESHOLD"  ,  -1  )  <>  NVL(  :old."REV_THRESHOLD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.REV_THRESHOLD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RISE_RATING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RISE_RATING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RISE_RATING"  ,  -1  )  <>  NVL(  :old."RISE_RATING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RISE_RATING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_ADDRESS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_ADDRESS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_ADDRESS"  ,  -1  )  <>  NVL(  :old."RTU_ADDRESS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_ADDRESS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_EXIST'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_EXIST'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_EXIST"  ,  -1  )  <>  NVL(  :old."RTU_EXIST"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_EXIST  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MANF_CD"  ,  -1  )  <>  NVL(  :old."RTU_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'RTU_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.RTU_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SCADA'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA"  ,  -1  )  <>  NVL(  :old."SCADA"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SCADA  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SCADA_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA_TYPE"  ,  -1  )  <>  NVL(  :old."SCADA_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SCADA_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SEASON_OFF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SEASON_OFF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SEASON_OFF"  ,  -1  )  <>  NVL(  :old."SEASON_OFF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SEASON_OFF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SPECIAL_CONDITIONS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SPECIAL_CONDITIONS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SPECIAL_CONDITIONS"  ,  -1  )  <>  NVL(  :old."SPECIAL_CONDITIONS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SPECIAL_CONDITIONS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'STEPS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'STEPS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."STEPS"  ,  -1  )  <>  NVL(  :old."STEPS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.STEPS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SVD_MIN'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SVD_MIN'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SVD_MIN"  ,  -1  )  <>  NVL(  :old."SVD_MIN"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SVD_MIN  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SWITCH_POSITION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'SWITCH_POSITION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SWITCH_POSITION"  ,  -1  )  <>  NVL(  :old."SWITCH_POSITION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.SWITCH_POSITION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'TIMER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TIMER"  ,  -1  )  <>  NVL(  :old."TIMER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.TIMER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_DISABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'TRANSMIT_DISABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_DISABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_DISABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.TRANSMIT_DISABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_ENABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'TRANSMIT_ENABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_ENABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_ENABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.TRANSMIT_ENABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'USE_RX'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'USE_RX'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."USE_RX"  ,  -1  )  <>  NVL(  :old."USE_RX"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.USE_RX  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'VOLT_VAR_TEAM_MEMBER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'VOLT_VAR_TEAM_MEMBER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."VOLT_VAR_TEAM_MEMBER"  ,  -1  )  <>  NVL(  :old."VOLT_VAR_TEAM_MEMBER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_REGULATOR_EAD.VOLT_VAR_TEAM_MEMBER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATE_MODIFIED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'DATE_MODIFIED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."DATE_MODIFIED",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."DATE_MODIFIED",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_REGULATOR_EAD.DATE_MODIFIED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EFFECTIVE_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'EFFECTIVE_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."EFFECTIVE_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."EFFECTIVE_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_REGULATOR_EAD.EFFECTIVE_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'PEER_REVIEW_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_REGULATOR_EAD.PEER_REVIEW_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMESTAMP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  'TIMESTAMP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."TIMESTAMP",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( TO_CHAR( :old."TIMESTAMP",'YYYYMMDDHH24MISSFF') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_REGULATOR_EAD.TIMESTAMP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_REGULATOR_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;  
END;
/
