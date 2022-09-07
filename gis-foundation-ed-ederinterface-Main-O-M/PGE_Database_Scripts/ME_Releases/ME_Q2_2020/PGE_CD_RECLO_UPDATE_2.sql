CREATE OR REPLACE TRIGGER "PGEDATA"."PGE_CD_RECLO_UPDATE_2" AFTER UPDATE on PGEDATA.PGEDATA_SM_RECLOSER_EAD 
REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW
  DECLARE
    cursor watch_type_list is
	         SELECT distinct watch_type from CD_MAP_SETTINGS where 
WATCH_TABLE='PGEDATA_SM_RECLOSER_EAD';
	sql_stmt varchar2(4000);
	row_cnt number;
	is_tracked boolean;
	BEGIN
  IF(  UPDATING( 'DEF_TIME_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'DEF_TIME_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL( TO_CHAR( :new."DEF_TIME_PHA",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( 
    TO_CHAR( :old."DEF_TIME_PHA",'YYYYMMDDHH24MISSFF') , -1  )  then
                             dbms_output.put_line( 'Change Detected  so updating for field 
    PGEDATA_SM_RECLOSER_EAD.DEF_TIME_PHA  '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
    IF(  UPDATING( 'DEF_TIME_GROUND'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'DEF_TIME_GROUND'  
,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL( TO_CHAR( :new."DEF_TIME_GROUND",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  NVL( 
    TO_CHAR( :old."DEF_TIME_GROUND",'YYYYMMDDHH24MISSFF') , -1  )  then
                             dbms_output.put_line( 'Change Detected  so updating for field 
    PGEDATA_SM_RECLOSER_EAD.DEF_TIME_GROUND_PHA  '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
    IF(  UPDATING( 'MIN_RESP_TADDR_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'MIN_RESP_TADDR_PHA 
'  , 
     cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL( TO_CHAR( :new."MIN_RESP_TADDR_PHA",'YYYYMMDDHH24MISSFF')  ,  -1  )  <>  
NVL( 
    TO_CHAR( :old."MIN_RESP_TADDR_PHA",'YYYYMMDDHH24MISSFF') , -1  )  then
                             dbms_output.put_line( 'Change Detected  so updating for field 
    PGEDATA_SM_RECLOSER_EAD.MIN_RESP_TADDR_PHA  '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
    IF(  UPDATING( 'MIN_RESP_TADDR_GRO'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'MIN_RESP_TADDR_GRO' 
 ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."MIN_RESP_TADDR_GRO"  ,  -1  )  <>  NVL(  :old."MIN_RESP_TADDR_GRO" 
 ,  
    -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.MIN_RESP_TADDR_GRO '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
IF(  UPDATING( 'HLT_ENAB'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'HLT_ENAB'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                     IF  NVL(  :new."HLT_ENAB"  ,  -1  )  <>  NVL(  :old."HLT_ENAB"  ,  -1  )  then
                         dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.HLT_ENAB '  )  ;
                         CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line( 'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;
IF(  UPDATING( 'PHASE'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'PHASE'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                     IF  NVL(  :new."PHASE"  ,  -1  )  <>  NVL(  :old."PHASE"  ,  -1  )  then
                         dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.PHASE '  )  ;
                         CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line( 'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;
IF(  UPDATING( 'GROUND'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'GROUND'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                     IF  NVL(  :new."GROUND"  ,  -1  )  <>  NVL(  :old."GROUND"  ,  -1  )  then
                         dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.GROUND '  )  ;
                         CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line( 'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;
  
         IF(  UPDATING( 'OPT_LOCK_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'OPT_LOCK_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."OPT_LOCK_PHA"  ,  -1  )  <>  NVL(  :old."OPT_LOCK_PHA"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.OPT_LOCK_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'OPT_LOCK_GRO'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'OPT_LOCK_GRO'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."OPT_LOCK_GRO"  ,  -1  )  <>  NVL(  :old."OPT_LOCK_GRO"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.OPT_LOCK_GRO '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'FIRST_RECLO_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'FIRST_RECLO_PHA'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."FIRST_RECLO_PHA"  ,  -1  )  <>  NVL(  :old."FIRST_RECLO_PHA"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.FIRST_RECLO_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'FIRST_RECLO_GRO'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'FIRST_RECLO_GRO'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."FIRST_RECLO_GRO"  ,  -1  )  <>  NVL(  :old."FIRST_RECLO_GRO"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.FIRST_RECLO_GRO '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'SEC_RECLO_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'SEC_RECLO_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."SEC_RECLO_PHA"  ,  -1  )  <>  NVL(  :old."SEC_RECLO_PHA"  ,  -1  ) 
 
    then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.SEC_RECLO_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
     IF(  UPDATING( 'SEC_RECLO_GRO'  )  )  then
           FOR  cdtype  in  watch_type_list  LOOP
                is_tracked  :=  FALSE;
                is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'SEC_RECLO_GRO'  ,  cdtype.watch_type  )  ;
                IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                     IF  NVL(  :new."SEC_RECLO_GRO"  ,  -1  )  <>  NVL(  :old."SEC_RECLO_GRO"  ,  -1  )  then
                         dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.SEC_RECLO_GRO '  )  ;
                         CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                     ELSE  dbms_output.put_line( 'values  matched'  )  ;
                     END IF;
                ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                END IF;
           END LOOP;
     END IF;
     IF(  UPDATING( 'THIRD_RECLO_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'THIRD_RECLO_PHA'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."THIRD_RECLO_PHA"  ,  -1  )  <>  NVL(  :old."THIRD_RECLO_PHA"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.THIRD_RECLO_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'THIRD_RECLO_GRO'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'THIRD_RECLO_GRO'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."THIRD_RECLO_GRO"  ,  -1  )  <>  NVL(  :old."THIRD_RECLO_GRO"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.THIRD_RECLO_GRO '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'RESET_TIME'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'RESET_TIME'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."RESET_TIME"  ,  -1  )  <>  NVL(  :old."RESET_TIME"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.RESET_TIME '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'RESET_TIME_LOCK'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'RESET_TIME_LOCK'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."RESET_TIME_LOCK"  ,  -1  )  <>  NVL(  :old."RESET_TIME_LOCK"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.RESET_TIME_LOCK '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'MTT_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'MTT_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."MTT_PHA"  ,  -1  )  <>  NVL(  :old."MTT_PHA"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.MTT_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'MTT_GROU'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'MTT_GROU'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."MTT_GROU"  ,  -1  )  <>  NVL(  :old."MTT_GROU"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.MTT_GROU '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'TIME_DEL_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'TIME_DEL_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."TIME_DEL_PHA"  ,  -1  )  <>  NVL(  :old."TIME_DEL_PHA"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.TIME_DEL_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'TIME_DEL_GROU'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'TIME_DEL_GROU'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."TIME_DEL_GROU"  ,  -1  )  <>  NVL(  :old."TIME_DEL_GROU"  ,  -1  ) 
 
    then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.TIME_DEL_GROU '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'PU_MTT_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'PU_MTT_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."PU_MTT_PHA"  ,  -1  )  <>  NVL(  :old."PU_MTT_PHA"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.PU_MTT_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'PU_MTT_GROU'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'PU_MTT_GROU'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."PU_MTT_GROU"  ,  -1  )  <>  NVL(  :old."PU_MTT_GROU"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.PU_MTT_GROU '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'PU_CURVE_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'PU_CURVE_PHA'  ,  cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                         IF  NVL(  :new."PU_CURVE_PHA"  ,  -1  )  <>  NVL(  :old."PU_CURVE_PHA"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.PU_CURVE_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'PU_CURVE_GROU'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'PU_CURVE_GROU'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."PU_CURVE_GROU"  ,  -1  )  <>  NVL(  :old."PU_CURVE_GROU"  ,  -1  ) 
 
    then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.PU_CURVE_GROU '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'TADDR_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'TADDR_PHA'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."TADDR_PHA"  ,  -1  )  <>  NVL(  :old."TADDR_PHA"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.TADDR_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'TADDR_GROU'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'TADDR_GROU'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."TADDR_GROU"  ,  -1  )  <>  NVL(  :old."TADDR_GROU"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.TADDR_GROU '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'MIN_RESP_TADDR_T_D_PHA'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  
'MIN_RESP_TADDR_T_D_PHA' 
     ,  cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."MIN_RESP_TADDR_T_D_PHA"  ,  -1  )  <>  NVL(  
    :old."MIN_RESP_TADDR_T_D_PHA"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.MIN_RESP_TADDR_T_D_PHA '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'MIN_RESP_TADDR_T_D_GR'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  
'MIN_RESP_TADDR_T_D_GR'  
    ,  cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."MIN_RESP_TADDR_T_D_GR"  ,  -1  )  <>  NVL(  
    :old."MIN_RESP_TADDR_T_D_GR"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.MIN_RESP_TADDR_T_D_GR '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                            ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'TIME_T_ACTI'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'TIME_T_ACTI'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."TIME_T_ACTI"  ,  -1  )  <>  NVL(  :old."TIME_T_ACTI"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.TIME_T_ACTI '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'FAULT_CURR_ONLY'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'FAULT_CURR_ONLY'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."FAULT_CURR_ONLY"  ,  -1  )  <>  NVL(  :old."FAULT_CURR_ONLY"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.FAULT_CURR_ONLY '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'VOLT_LOSS_ONLY'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'VOLT_LOSS_ONLY'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."VOLT_LOSS_ONLY"  ,  -1  )  <>  NVL(  :old."VOLT_LOSS_ONLY"  ,  -1  
)  
    then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.VOLT_LOSS_ONLY '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'FAULT_CURR_W_VOL_LOSS'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  
'FAULT_CURR_W_VOL_LOSS'  
    ,  cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."FAULT_CURR_W_VOL_LOSS"  ,  -1  )  <>  NVL(  
    :old."FAULT_CURR_W_VOL_LOSS"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.FAULT_CURR_W_VOL_LOSS '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'VOLT_LOSS_DISP'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'VOLT_LOSS_DISP'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."VOLT_LOSS_DISP"  ,  -1  )  <>  NVL(  :old."VOLT_LOSS_DISP"  ,  -1  
)  
    then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.VOLT_LOSS_DISP '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'ENA_SWIT_MOD'  )  )  then
                   FOR  cdtype  in  watch_type_list  LOOP
                        is_tracked  :=  FALSE;
                        is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'ENA_SWIT_MOD'  ,  cdtype.watch_type  )  ;
                        IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  comparison: '  );
                             IF  NVL(  :new."ENA_SWIT_MOD"  ,  -1  )  <>  NVL(  :old."ENA_SWIT_MOD"  ,  -1  )  then
                                 dbms_output.put_line( 'Change  Detected  so  updating  for  field  PGEDATA_SM_RECLOSER_EAD.ENA_SWIT_MOD '  )  ;
                                 CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  cdtype.watch_type  )  ;
                             ELSE  dbms_output.put_line( 'values  matched'  )  ;
                             END IF;
                        ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                        END IF;
                   END LOOP;
             END IF;
         IF(  UPDATING( 'COUT_T_TRIP_VOLT_LOSS'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  
'COUT_T_TRIP_VOLT_LOSS'  
    ,  cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."COUT_T_TRIP_VOLT_LOSS"  ,  -1  )  <>  NVL(  
    :old."COUT_T_TRIP_VOLT_LOSS"  ,  -1  )  then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.COUT_T_TRIP_VOLT_LOSS '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'RESET_TIMERR'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'RESET_TIMERR'  ,  
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."RESET_TIMERR"  ,  -1  )  <>  NVL(  :old."RESET_TIMERR"  ,  -1  )  
then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.RESET_TIMERR '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'SEQ_COORDI_MODE'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'SEQ_COORDI_MODE'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."SEQ_COORDI_MODE"  ,  -1  )  <>  NVL(  :old."SEQ_COORDI_MODE"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.SEQ_COORDI_MODE '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'COLD_PHA_TMUL_FAST'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'COLD_PHA_TMUL_FAST'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."COLD_PHA_TMUL_FAST"  ,  -1  )  <>  NVL(  :old."COLD_PHA_TMUL_FAST"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.COLD_PHA_TMUL_FAST '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;
         IF(  UPDATING( 'COLD_GRD_TMUL_FAST'  )  )  then
               FOR  cdtype  in  watch_type_list  LOOP
                    is_tracked  :=  FALSE;
                    is_tracked  :=  CD_GIS.IS_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  , 'COLD_GRD_TMUL_FAST'  , 
 
    cdtype.watch_type  )  ;
                    IF  is_tracked  then  dbms_output.put_line( '  returned  TRUE  about  to  execute  
    comparison: '  );
                         IF  NVL(  :new."COLD_GRD_TMUL_FAST"  ,  -1  )  <>  NVL(  :old."COLD_GRD_TMUL_FAST"  ,  
-1  ) 
     then
                             dbms_output.put_line( 'Change  Detected  so  updating  for  field  
    PGEDATA_SM_RECLOSER_EAD.COLD_GRD_TMUL_FAST '  )  ;
                             CD_GIS.SET_FIELD_CD( 'PGEDATA_SM_RECLOSER_EAD'  ,  :new.GLOBAL_ID,  
    cdtype.watch_type  )  ;
                         ELSE  dbms_output.put_line( 'values  matched'  )  ;
                         END IF;
                    ELSE  dbms_output.put_line( 'returned  FALSE'  )  ;
                    END IF;
               END LOOP;
         END IF;

END;