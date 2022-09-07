Prompt drop Trigger PGE_CD_CIRCU_UPDATE;
DROP TRIGGER PGEDATA.PGE_CD_CIRCU_UPDATE
/

Prompt Trigger PGE_CD_CIRCU_UPDATE;
--
-- PGE_CD_CIRCU_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_CIRCU_UPDATE AFTER UPDATE on PGEDATA.PGEDATA_SM_CIRCUIT_BREAKER_EAD REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_CIRCUIT_BREAKER_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	 
     IF(  UPDATING(  'ANNUAL_LF'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'ANNUAL_LF'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ANNUAL_LF"  ,  -1  )  <>  NVL(  :old."ANNUAL_LF"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.ANNUAL_LF  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BAUD_RATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'BAUD_RATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAUD_RATE"  ,  -1  )  <>  NVL(  :old."BAUD_RATE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.BAUD_RATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CC_RATING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'CC_RATING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CC_RATING"  ,  -1  )  <>  NVL(  :old."CC_RATING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.CC_RATING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CURRENT_FUTURE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'CURRENT_FUTURE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CURRENT_FUTURE"  ,  -1  )  <>  NVL(  :old."CURRENT_FUTURE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.CURRENT_FUTURE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DEVICE_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DEVICE_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DEVICE_ID"  ,  -1  )  <>  NVL(  :old."DEVICE_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.DEVICE_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIRECT_TRANSFER_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DIRECT_TRANSFER_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIRECT_TRANSFER_TRIP"  ,  -1  )  <>  NVL(  :old."DIRECT_TRANSFER_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.DIRECT_TRANSFER_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DISTRICT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DISTRICT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DISTRICT"  ,  -1  )  <>  NVL(  :old."DISTRICT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.DISTRICT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIVISION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DIVISION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIVISION"  ,  -1  )  <>  NVL(  :old."DIVISION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.DIVISION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DPA_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DPA_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DPA_CD"  ,  -1  )  <>  NVL(  :old."DPA_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.DPA_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FEATURE_CLASS_NAME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'FEATURE_CLASS_NAME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FEATURE_CLASS_NAME"  ,  -1  )  <>  NVL(  :old."FEATURE_CLASS_NAME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.FEATURE_CLASS_NAME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'FLISR'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR"  ,  -1  )  <>  NVL(  :old."FLISR"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.FLISR  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR_ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'FLISR_ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.FLISR_ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR_OPERATING_MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'FLISR_OPERATING_MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR_OPERATING_MODE"  ,  -1  )  <>  NVL(  :old."FLISR_OPERATING_MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.FLISR_OPERATING_MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GLOBAL_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GLOBAL_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GLOBAL_ID"  ,  -1  )  <>  NVL(  :old."GLOBAL_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GLOBAL_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."GRD_BK_CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."GRD_BK_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_INS_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_INS_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_INS_TRIP"  ,  -1  )  <>  NVL(  :old."GRD_BK_INS_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_INS_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_LEVER_SET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_LEVER_SET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_LEVER_SET"  ,  -1  )  <>  NVL(  :old."GRD_BK_LEVER_SET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_LEVER_SET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_MIN_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_MIN_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_MIN_TRIP"  ,  -1  )  <>  NVL(  :old."GRD_BK_MIN_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_MIN_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_RELAY_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_RELAY_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_RELAY_CD"  ,  -1  )  <>  NVL(  :old."GRD_BK_RELAY_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_RELAY_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."GRD_BK_RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_BK_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_BK_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_BK_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."GRD_BK_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_BK_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."GRD_PR_CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."GRD_PR_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_INS_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_INS_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_INS_TRIP"  ,  -1  )  <>  NVL(  :old."GRD_PR_INS_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_INS_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_LEVER_SET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_LEVER_SET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_LEVER_SET"  ,  -1  )  <>  NVL(  :old."GRD_PR_LEVER_SET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_LEVER_SET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_MIN_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_MIN_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_MIN_TRIP"  ,  -1  )  <>  NVL(  :old."GRD_PR_MIN_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_MIN_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_RELAY_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_RELAY_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_RELAY_CD"  ,  -1  )  <>  NVL(  :old."GRD_PR_RELAY_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_RELAY_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."GRD_PR_RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_PR_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'GRD_PR_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_PR_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."GRD_PR_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.GRD_PR_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ID"  ,  -1  )  <>  NVL(  :old."ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LIMITING_FACTOR'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'LIMITING_FACTOR'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LIMITING_FACTOR"  ,  -1  )  <>  NVL(  :old."LIMITING_FACTOR"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.LIMITING_FACTOR  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_STATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'MASTER_STATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_STATION"  ,  -1  )  <>  NVL(  :old."MASTER_STATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.MASTER_STATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MIN_NOR_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'MIN_NOR_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MIN_NOR_VOLT"  ,  -1  )  <>  NVL(  :old."MIN_NOR_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.MIN_NOR_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'NETWORK'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'NETWORK'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."NETWORK"  ,  -1  )  <>  NVL(  :old."NETWORK"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.NETWORK  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'NOTES'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'NOTES'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."NOTES"  ,  -1  )  <>  NVL(  :old."NOTES"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.NOTES  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OK_TO_BYPASS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'OK_TO_BYPASS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OK_TO_BYPASS"  ,  -1  )  <>  NVL(  :old."OK_TO_BYPASS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.OK_TO_BYPASS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'OPERATING_MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_MODE"  ,  -1  )  <>  NVL(  :old."OPERATING_MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.OPERATING_MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'OPERATING_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_NUM"  ,  -1  )  <>  NVL(  :old."OPERATING_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.OPERATING_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPS_TO_LOCKOUT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'OPS_TO_LOCKOUT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPS_TO_LOCKOUT"  ,  -1  )  <>  NVL(  :old."OPS_TO_LOCKOUT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.OPS_TO_LOCKOUT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PEER_REVIEW_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEER_REVIEW_BY"  ,  -1  )  <>  NVL(  :old."PEER_REVIEW_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PEER_REVIEW_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."PHA_BK_CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."PHA_BK_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_INS_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_INS_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_INS_TRIP"  ,  -1  )  <>  NVL(  :old."PHA_BK_INS_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_INS_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_LEVER_SET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_LEVER_SET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_LEVER_SET"  ,  -1  )  <>  NVL(  :old."PHA_BK_LEVER_SET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_LEVER_SET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_MIN_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_MIN_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_MIN_TRIP"  ,  -1  )  <>  NVL(  :old."PHA_BK_MIN_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_MIN_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_RELAY_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_RELAY_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_RELAY_CD"  ,  -1  )  <>  NVL(  :old."PHA_BK_RELAY_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_RELAY_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."PHA_BK_RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_BK_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_BK_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_BK_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."PHA_BK_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_BK_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."PHA_PR_CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."PHA_PR_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_INS_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_INS_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_INS_TRIP"  ,  -1  )  <>  NVL(  :old."PHA_PR_INS_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_INS_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_LEVER_SET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_LEVER_SET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_LEVER_SET"  ,  -1  )  <>  NVL(  :old."PHA_PR_LEVER_SET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_LEVER_SET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_MIN_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_MIN_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_MIN_TRIP"  ,  -1  )  <>  NVL(  :old."PHA_PR_MIN_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_MIN_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_RELAY_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_RELAY_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_RELAY_CD"  ,  -1  )  <>  NVL(  :old."PHA_PR_RELAY_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_RELAY_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."PHA_PR_RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_PR_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PHA_PR_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_PR_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."PHA_PR_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PHA_PR_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREPARED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PREPARED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREPARED_BY"  ,  -1  )  <>  NVL(  :old."PREPARED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PREPARED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PROCESSED_FLAG'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PROCESSED_FLAG'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PROCESSED_FLAG"  ,  -1  )  <>  NVL(  :old."PROCESSED_FLAG"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.PROCESSED_FLAG  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RADIO_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MANF_CD"  ,  -1  )  <>  NVL(  :old."RADIO_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RADIO_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RADIO_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RADIO_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RADIO_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RADIO_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RECLOSE_BLOCKING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RECLOSE_BLOCKING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RECLOSE_BLOCKING"  ,  -1  )  <>  NVL(  :old."RECLOSE_BLOCKING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RECLOSE_BLOCKING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELEASED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RELEASED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELEASED_BY"  ,  -1  )  <>  NVL(  :old."RELEASED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RELEASED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REPEATER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'REPEATER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REPEATER"  ,  -1  )  <>  NVL(  :old."REPEATER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.REPEATER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_ADDRESS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_ADDRESS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_ADDRESS"  ,  -1  )  <>  NVL(  :old."RTU_ADDRESS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_ADDRESS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_EXIST'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_EXIST'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_EXIST"  ,  -1  )  <>  NVL(  :old."RTU_EXIST"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_EXIST  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MANF_CD"  ,  -1  )  <>  NVL(  :old."RTU_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'RTU_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.RTU_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'SCADA'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA"  ,  -1  )  <>  NVL(  :old."SCADA"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.SCADA  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'SCADA_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA_TYPE"  ,  -1  )  <>  NVL(  :old."SCADA_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.SCADA_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SPECIAL_CONDITIONS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'SPECIAL_CONDITIONS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SPECIAL_CONDITIONS"  ,  -1  )  <>  NVL(  :old."SPECIAL_CONDITIONS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.SPECIAL_CONDITIONS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SUMMER_LOAD_LIMIT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'SUMMER_LOAD_LIMIT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SUMMER_LOAD_LIMIT"  ,  -1  )  <>  NVL(  :old."SUMMER_LOAD_LIMIT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.SUMMER_LOAD_LIMIT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_DISABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'TRANSMIT_DISABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_DISABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_DISABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.TRANSMIT_DISABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_ENABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'TRANSMIT_ENABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_ENABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_ENABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.TRANSMIT_ENABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'WINTER_LOAD_LIMIT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'WINTER_LOAD_LIMIT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."WINTER_LOAD_LIMIT"  ,  -1  )  <>  NVL(  :old."WINTER_LOAD_LIMIT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_CIRCUIT_BREAKER_EAD.WINTER_LOAD_LIMIT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATE_MODIFIED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'DATE_MODIFIED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."DATE_MODIFIED",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."DATE_MODIFIED",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CIRCUIT_BREAKER_EAD.DATE_MODIFIED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EFFECTIVE_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'EFFECTIVE_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."EFFECTIVE_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."EFFECTIVE_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CIRCUIT_BREAKER_EAD.EFFECTIVE_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'PEER_REVIEW_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CIRCUIT_BREAKER_EAD.PEER_REVIEW_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMESTAMP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  'TIMESTAMP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."TIMESTAMP",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( TO_CHAR( :old."TIMESTAMP",'YYYYMMDDHH24MISSFF') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_CIRCUIT_BREAKER_EAD.TIMESTAMP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_CIRCUIT_BREAKER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;  
END;
/
