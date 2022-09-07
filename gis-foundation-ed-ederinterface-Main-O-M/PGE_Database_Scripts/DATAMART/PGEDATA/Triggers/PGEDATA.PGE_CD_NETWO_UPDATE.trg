Prompt drop Trigger PGE_CD_NETWO_UPDATE;
DROP TRIGGER PGEDATA.PGE_CD_NETWO_UPDATE
/

Prompt Trigger PGE_CD_NETWO_UPDATE;
--
-- PGE_CD_NETWO_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER PGEDATA.PGE_CD_NETWO_UPDATE AFTER UPDATE on PGEDATA.PGEDATA_SM_NETWORK_PROT_EAD REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where WATCH_TABLE='PGEDATA_SM_NETWORK_PROT_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
	 
     IF(  UPDATING(  'ANTI_PUMPING_PROTECTION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'ANTI_PUMPING_PROTECTION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ANTI_PUMPING_PROTECTION"  ,  -1  )  <>  NVL(  :old."ANTI_PUMPING_PROTECTION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.ANTI_PUMPING_PROTECTION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BAUD_RATE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'BAUD_RATE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BAUD_RATE"  ,  -1  )  <>  NVL(  :old."BAUD_RATE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.BAUD_RATE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'BREAKER_CYCLES_ANTI_PUMPING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'BREAKER_CYCLES_ANTI_PUMPING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."BREAKER_CYCLES_ANTI_PUMPING"  ,  -1  )  <>  NVL(  :old."BREAKER_CYCLES_ANTI_PUMPING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.BREAKER_CYCLES_ANTI_PUMPING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CLOSING_MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'CLOSING_MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CLOSING_MODE"  ,  -1  )  <>  NVL(  :old."CLOSING_MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.CLOSING_MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CONTROL_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'CONTROL_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CONTROL_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."CONTROL_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.CONTROL_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CT_RATIO'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'CT_RATIO'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CT_RATIO"  ,  -1  )  <>  NVL(  :old."CT_RATIO"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.CT_RATIO  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'CURRENT_FUTURE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'CURRENT_FUTURE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."CURRENT_FUTURE"  ,  -1  )  <>  NVL(  :old."CURRENT_FUTURE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.CURRENT_FUTURE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DEVICE_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'DEVICE_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DEVICE_ID"  ,  -1  )  <>  NVL(  :old."DEVICE_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.DEVICE_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DISTRICT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'DISTRICT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DISTRICT"  ,  -1  )  <>  NVL(  :old."DISTRICT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.DISTRICT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DIVISION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'DIVISION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."DIVISION"  ,  -1  )  <>  NVL(  :old."DIVISION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.DIVISION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ENGINEERING_DOCUMENT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'ENGINEERING_DOCUMENT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ENGINEERING_DOCUMENT"  ,  -1  )  <>  NVL(  :old."ENGINEERING_DOCUMENT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.ENGINEERING_DOCUMENT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FEATURE_CLASS_NAME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'FEATURE_CLASS_NAME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FEATURE_CLASS_NAME"  ,  -1  )  <>  NVL(  :old."FEATURE_CLASS_NAME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.FEATURE_CLASS_NAME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FIRMWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'FIRMWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FIRMWARE_VERSION"  ,  -1  )  <>  NVL(  :old."FIRMWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.FIRMWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'FUSE_STYLE_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'FUSE_STYLE_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."FUSE_STYLE_NUM"  ,  -1  )  <>  NVL(  :old."FUSE_STYLE_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.FUSE_STYLE_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'GLOBAL_ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'GLOBAL_ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."GLOBAL_ID"  ,  -1  )  <>  NVL(  :old."GLOBAL_ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.GLOBAL_ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ID'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'ID'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ID"  ,  -1  )  <>  NVL(  :old."ID"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.ID  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'LINE_TO_LINE_SEC_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'LINE_TO_LINE_SEC_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."LINE_TO_LINE_SEC_VOLT"  ,  -1  )  <>  NVL(  :old."LINE_TO_LINE_SEC_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.LINE_TO_LINE_SEC_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_ANGLE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'MASTER_ANGLE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_ANGLE"  ,  -1  )  <>  NVL(  :old."MASTER_ANGLE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.MASTER_ANGLE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_OFFSET_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'MASTER_OFFSET_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_OFFSET_VOLT"  ,  -1  )  <>  NVL(  :old."MASTER_OFFSET_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.MASTER_OFFSET_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'MASTER_STATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'MASTER_STATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."MASTER_STATION"  ,  -1  )  <>  NVL(  :old."MASTER_STATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.MASTER_STATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'NOTES'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'NOTES'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."NOTES"  ,  -1  )  <>  NVL(  :old."NOTES"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.NOTES  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OK_TO_BYPASS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'OK_TO_BYPASS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OK_TO_BYPASS"  ,  -1  )  <>  NVL(  :old."OK_TO_BYPASS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.OK_TO_BYPASS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OPERATING_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'OPERATING_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OPERATING_NUM"  ,  -1  )  <>  NVL(  :old."OPERATING_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.OPERATING_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OVERCUR_INSTANT_TRIP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'OVERCUR_INSTANT_TRIP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OVERCUR_INSTANT_TRIP"  ,  -1  )  <>  NVL(  :old."OVERCUR_INSTANT_TRIP"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.OVERCUR_INSTANT_TRIP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'OVERVOLT_PHASING_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'OVERVOLT_PHASING_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."OVERVOLT_PHASING_VOLT"  ,  -1  )  <>  NVL(  :old."OVERVOLT_PHASING_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.OVERVOLT_PHASING_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PEER_REVIEW_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PEER_REVIEW_BY"  ,  -1  )  <>  NVL(  :old."PEER_REVIEW_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PEER_REVIEW_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHASING_ANGLE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PHASING_ANGLE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHASING_ANGLE"  ,  -1  )  <>  NVL(  :old."PHASING_ANGLE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PHASING_ANGLE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHASING_LINE_ADJUSTMENT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PHASING_LINE_ADJUSTMENT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHASING_LINE_ADJUSTMENT"  ,  -1  )  <>  NVL(  :old."PHASING_LINE_ADJUSTMENT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PHASING_LINE_ADJUSTMENT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PHASING_OFFSET_VOLT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PHASING_OFFSET_VOLT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PHASING_OFFSET_VOLT"  ,  -1  )  <>  NVL(  :old."PHASING_OFFSET_VOLT"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PHASING_OFFSET_VOLT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PREPARED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PREPARED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PREPARED_BY"  ,  -1  )  <>  NVL(  :old."PREPARED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PREPARED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PROCESSED_FLAG'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PROCESSED_FLAG'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PROCESSED_FLAG"  ,  -1  )  <>  NVL(  :old."PROCESSED_FLAG"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PROCESSED_FLAG  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PUMP_TIME'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PUMP_TIME'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."PUMP_TIME"  ,  -1  )  <>  NVL(  :old."PUMP_TIME"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.PUMP_TIME  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MANF_CD'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RADIO_MANF_CD'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MANF_CD"  ,  -1  )  <>  NVL(  :old."RADIO_MANF_CD"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RADIO_MANF_CD  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_MODEL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RADIO_MODEL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_MODEL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_MODEL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RADIO_MODEL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RADIO_SERIAL_NUM'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RADIO_SERIAL_NUM'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RADIO_SERIAL_NUM"  ,  -1  )  <>  NVL(  :old."RADIO_SERIAL_NUM"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RADIO_SERIAL_NUM  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELAY_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RELAY_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELAY_TYPE"  ,  -1  )  <>  NVL(  :old."RELAY_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RELAY_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RELEASED_BY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RELEASED_BY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RELEASED_BY"  ,  -1  )  <>  NVL(  :old."RELEASED_BY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RELEASED_BY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REPEATER'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'REPEATER'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REPEATER"  ,  -1  )  <>  NVL(  :old."REPEATER"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.REPEATER  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RESET_TIME_ANTI_PUMPING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RESET_TIME_ANTI_PUMPING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RESET_TIME_ANTI_PUMPING"  ,  -1  )  <>  NVL(  :old."RESET_TIME_ANTI_PUMPING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RESET_TIME_ANTI_PUMPING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'REVERSE_TRIP_SETTING'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'REVERSE_TRIP_SETTING'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."REVERSE_TRIP_SETTING"  ,  -1  )  <>  NVL(  :old."REVERSE_TRIP_SETTING"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.REVERSE_TRIP_SETTING  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'ROTATION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'ROTATION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."ROTATION"  ,  -1  )  <>  NVL(  :old."ROTATION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.ROTATION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'RTU_ADDRESS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'RTU_ADDRESS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."RTU_ADDRESS"  ,  -1  )  <>  NVL(  :old."RTU_ADDRESS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.RTU_ADDRESS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'SCADA'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA"  ,  -1  )  <>  NVL(  :old."SCADA"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.SCADA  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SCADA_TYPE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'SCADA_TYPE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SCADA_TYPE"  ,  -1  )  <>  NVL(  :old."SCADA_TYPE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.SCADA_TYPE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SOFTWARE_VERSION'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'SOFTWARE_VERSION'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SOFTWARE_VERSION"  ,  -1  )  <>  NVL(  :old."SOFTWARE_VERSION"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.SOFTWARE_VERSION  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'SPECIAL_CONDITIONS'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'SPECIAL_CONDITIONS'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."SPECIAL_CONDITIONS"  ,  -1  )  <>  NVL(  :old."SPECIAL_CONDITIONS"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.SPECIAL_CONDITIONS  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'STRAIGHT_LINE_MASTER_LINE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'STRAIGHT_LINE_MASTER_LINE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."STRAIGHT_LINE_MASTER_LINE"  ,  -1  )  <>  NVL(  :old."STRAIGHT_LINE_MASTER_LINE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.STRAIGHT_LINE_MASTER_LINE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIME_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'TIME_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TIME_DELAY"  ,  -1  )  <>  NVL(  :old."TIME_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.TIME_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_DISABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'TRANSMIT_DISABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_DISABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_DISABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.TRANSMIT_DISABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRANSMIT_ENABLE_DELAY'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'TRANSMIT_ENABLE_DELAY'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRANSMIT_ENABLE_DELAY"  ,  -1  )  <>  NVL(  :old."TRANSMIT_ENABLE_DELAY"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.TRANSMIT_ENABLE_DELAY  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TRIP_MODE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'TRIP_MODE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."TRIP_MODE"  ,  -1  )  <>  NVL(  :old."TRIP_MODE"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.TRIP_MODE  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'WATT_VAR_TRIP_CHAR'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'WATT_VAR_TRIP_CHAR'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL(  :new."WATT_VAR_TRIP_CHAR"  ,  -1  )  <>  NVL(  :old."WATT_VAR_TRIP_CHAR"  ,  -1  )  then
                         dbms_output.put_line(  'Change  Detected  so  updating  for  field  PGEDATA_SM_NETWORK_PROT_EAD.WATT_VAR_TRIP_CHAR  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'DATE_MODIFIED'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'DATE_MODIFIED'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."DATE_MODIFIED",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."DATE_MODIFIED",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_NETWORK_PROT_EAD.DATE_MODIFIED  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'EFFECTIVE_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'EFFECTIVE_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."EFFECTIVE_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."EFFECTIVE_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_NETWORK_PROT_EAD.EFFECTIVE_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'PEER_REVIEW_DT'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'PEER_REVIEW_DT'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') ,  -1  )  <>  NVL( TO_CHAR( :old."PEER_REVIEW_DT",'YYYYMMDDHH24MISS') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_NETWORK_PROT_EAD.PEER_REVIEW_DT  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;   
     IF(  UPDATING(  'TIMESTAMP'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  'TIMESTAMP'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line(  '  returned  TRUE  about  to  execute  comparison:  '  );
                     IF  NVL( TO_CHAR( :new."TIMESTAMP",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( TO_CHAR( :old."TIMESTAMP",'YYYYMMDDHH24MISSFF') , -1  )  then
                         dbms_output.put_line(  'Change Detected  so updating for field PGEDATA_SM_NETWORK_PROT_EAD.TIMESTAMP  '  )  ;
                         CD_GIS.SET_FIELD_CD(  'PGEDATA_SM_NETWORK_PROT_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line(  'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line(  'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;  
END;
/
