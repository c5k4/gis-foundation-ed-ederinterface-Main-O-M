Prompt drop Trigger PGE_CD_SECTI_UPDATE;
DROP TRIGGER PGEDATA.PGE_CD_SECTI_UPDATE
/

Prompt Trigger PGE_CD_SECTI_UPDATE;
--
-- PGE_CD_SECTI_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_SECTI_UPDATE AFTER UPDATE on PGEDATA.PGEDATA_SM_SECTIONALIZER_EAD REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_SECTIONALIZER_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	 
     IF(  UPDATING(  'BAUD_RATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'BAUD_RATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAUD_RATE"  ,  -1  )  <>  NVL(  :old."BAUD_RATE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.BAUD_RATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BYPASS_PLANS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'BYPASS_PLANS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BYPASS_PLANS"  ,  -1  )  <>  NVL(  :old."BYPASS_PLANS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.BYPASS_PLANS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'CONTROL_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_TYPE"  ,  -1  )  <>  NVL(  :old."CONTROL_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.CONTROL_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CURRENT_FUTURE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'CURRENT_FUTURE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CURRENT_FUTURE"  ,  -1  )  <>  NVL(  :old."CURRENT_FUTURE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.CURRENT_FUTURE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DEVICE_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'DEVICE_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DEVICE_ID"  ,  -1  )  <>  NVL(  :old."DEVICE_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.DEVICE_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DISTRICT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'DISTRICT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DISTRICT"  ,  -1  )  <>  NVL(  :old."DISTRICT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.DISTRICT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIVISION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'DIVISION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIVISION"  ,  -1  )  <>  NVL(  :old."DIVISION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.DIVISION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FEATURE_CLASS_NAME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'FEATURE_CLASS_NAME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FEATURE_CLASS_NAME"  ,  -1  )  <>  NVL(  :old."FEATURE_CLASS_NAME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.FEATURE_CLASS_NAME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FIRST_RECLOSE_RESET_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'FIRST_RECLOSE_RESET_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FIRST_RECLOSE_RESET_TIME"  ,  -1  )  <>  NVL(  :old."FIRST_RECLOSE_RESET_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.FIRST_RECLOSE_RESET_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'FLISR'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR"  ,  -1  )  <>  NVL(  :old."FLISR"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.FLISR  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FLISR_ENGINEERING_COMMENTS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'FLISR_ENGINEERING_COMMENTS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  <>  NVL(  :old."FLISR_ENGINEERING_COMMENTS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.FLISR_ENGINEERING_COMMENTS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GLOBAL_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'GLOBAL_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GLOBAL_ID"  ,  -1  )  <>  NVL(  :old."GLOBAL_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.GLOBAL_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_INRUSH_DURATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'GRD_INRUSH_DURATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_INRUSH_DURATION"  ,  -1  )  <>  NVL(  :old."GRD_INRUSH_DURATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.GRD_INRUSH_DURATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_INRUSH_MULTIPLIER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'GRD_INRUSH_MULTIPLIER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_INRUSH_MULTIPLIER"  ,  -1  )  <>  NVL(  :old."GRD_INRUSH_MULTIPLIER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.GRD_INRUSH_MULTIPLIER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GRD_INRUSH_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'GRD_INRUSH_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GRD_INRUSH_TIME"  ,  -1  )  <>  NVL(  :old."GRD_INRUSH_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.GRD_INRUSH_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ID"  ,  -1  )  <>  NVL(  :old."ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LOCKOUT_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'LOCKOUT_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LOCKOUT_NUM"  ,  -1  )  <>  NVL(  :old."LOCKOUT_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.LOCKOUT_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_STATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'MASTER_STATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_STATION"  ,  -1  )  <>  NVL(  :old."MASTER_STATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.MASTER_STATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MIN_GRD_TO_CT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'MIN_GRD_TO_CT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MIN_GRD_TO_CT"  ,  -1  )  <>  NVL(  :old."MIN_GRD_TO_CT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.MIN_GRD_TO_CT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MIN_PC_TO_CT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'MIN_PC_TO_CT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MIN_PC_TO_CT"  ,  -1  )  <>  NVL(  :old."MIN_PC_TO_CT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.MIN_PC_TO_CT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OK_TO_BYPASS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'OK_TO_BYPASS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OK_TO_BYPASS"  ,  -1  )  <>  NVL(  :old."OK_TO_BYPASS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.OK_TO_BYPASS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ONE_SHOT_LOCKOUT_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'ONE_SHOT_LOCKOUT_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ONE_SHOT_LOCKOUT_NUM"  ,  -1  )  <>  NVL(  :old."ONE_SHOT_LOCKOUT_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.ONE_SHOT_LOCKOUT_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ONE_SHOT_LOCKOUT_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'ONE_SHOT_LOCKOUT_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ONE_SHOT_LOCKOUT_TIME"  ,  -1  )  <>  NVL(  :old."ONE_SHOT_LOCKOUT_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.ONE_SHOT_LOCKOUT_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'OPERATING_MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_MODE"  ,  -1  )  <>  NVL(  :old."OPERATING_MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.OPERATING_MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'OPERATING_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_NUM"  ,  -1  )  <>  NVL(  :old."OPERATING_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.OPERATING_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PEER_REVIEW_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEER_REVIEW_BY"  ,  -1  )  <>  NVL(  :old."PEER_REVIEW_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PEER_REVIEW_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_INRUSH_DURATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PHA_INRUSH_DURATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_INRUSH_DURATION"  ,  -1  )  <>  NVL(  :old."PHA_INRUSH_DURATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PHA_INRUSH_DURATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_INRUSH_MULTIPLIER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PHA_INRUSH_MULTIPLIER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_INRUSH_MULTIPLIER"  ,  -1  )  <>  NVL(  :old."PHA_INRUSH_MULTIPLIER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PHA_INRUSH_MULTIPLIER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHA_INRUSH_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PHA_INRUSH_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHA_INRUSH_TIME"  ,  -1  )  <>  NVL(  :old."PHA_INRUSH_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PHA_INRUSH_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREPARED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PREPARED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREPARED_BY"  ,  -1  )  <>  NVL(  :old."PREPARED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PREPARED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PROCESSED_FLAG'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PROCESSED_FLAG'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PROCESSED_FLAG"  ,  -1  )  <>  NVL(  :old."PROCESSED_FLAG"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.PROCESSED_FLAG  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RADIO_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MANF_CD"  ,  -1  )  <>  NVL(  :old."RADIO_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RADIO_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RADIO_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RADIO_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RADIO_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RADIO_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELEASED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RELEASED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELEASED_BY"  ,  -1  )  <>  NVL(  :old."RELEASED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RELEASED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REPEATER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'REPEATER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REPEATER"  ,  -1  )  <>  NVL(  :old."REPEATER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.REPEATER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REQUIRED_FAULT_CURRENT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'REQUIRED_FAULT_CURRENT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REQUIRED_FAULT_CURRENT"  ,  -1  )  <>  NVL(  :old."REQUIRED_FAULT_CURRENT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.REQUIRED_FAULT_CURRENT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RESET'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RESET'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RESET"  ,  -1  )  <>  NVL(  :old."RESET"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RESET  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_ADDRESS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_ADDRESS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_ADDRESS"  ,  -1  )  <>  NVL(  :old."RTU_ADDRESS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_ADDRESS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_EXIST'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_EXIST'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_EXIST"  ,  -1  )  <>  NVL(  :old."RTU_EXIST"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_EXIST  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MANF_CD"  ,  -1  )  <>  NVL(  :old."RTU_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RTU_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'RTU_SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."RTU_SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.RTU_SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'SCADA'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA"  ,  -1  )  <>  NVL(  :old."SCADA"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.SCADA  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'SCADA_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA_TYPE"  ,  -1  )  <>  NVL(  :old."SCADA_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.SCADA_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SECT_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'SECT_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SECT_TYPE"  ,  -1  )  <>  NVL(  :old."SECT_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.SECT_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SPECIAL_CONDITIONS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'SPECIAL_CONDITIONS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SPECIAL_CONDITIONS"  ,  -1  )  <>  NVL(  :old."SPECIAL_CONDITIONS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.SPECIAL_CONDITIONS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_DISABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'TRANSMIT_DISABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_DISABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_DISABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.TRANSMIT_DISABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_ENABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'TRANSMIT_ENABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_ENABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_ENABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.TRANSMIT_ENABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'VOLT_THRESHOLD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'VOLT_THRESHOLD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."VOLT_THRESHOLD"  ,  -1  )  <>  NVL(  :old."VOLT_THRESHOLD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_SECTIONALIZER_EAD.VOLT_THRESHOLD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATE_MODIFIED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'DATE_MODIFIED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."DATE_MODIFIED",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."DATE_MODIFIED",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_SECTIONALIZER_EAD.DATE_MODIFIED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EFFECTIVE_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'EFFECTIVE_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."EFFECTIVE_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."EFFECTIVE_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_SECTIONALIZER_EAD.EFFECTIVE_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'PEER_REVIEW_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_SECTIONALIZER_EAD.PEER_REVIEW_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMESTAMP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  'TIMESTAMP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."TIMESTAMP",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( TO_CHAR( :old."TIMESTAMP",'YYYYMMDDHH24MISSFF') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_SECTIONALIZER_EAD.TIMESTAMP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_SECTIONALIZER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;  
END;
/
