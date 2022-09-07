--------------------------------------------------------
--  DDL for Procedure GENERATE_EXT_CCB_METER_LOAD_1
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."GENERATE_EXT_CCB_METER_LOAD_1" 
As
     V_Date date;
     DB_ERROR_CODE VARCHAR2(20);
     DB_ERROR_MSG  VARCHAR2(200);
     V_Action_name Varchar2(50);
     V_Status Varchar2(50);
     v_rowsProcessed Number;
begin
-- Delete existing record for Service point and revanue month which present in staging tables.

   V_Action_name := 'Deleting Data from Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;

    delete /*+ parallel(d,4) */ from PGE_EXT_CCB_METER_LOAD_1 d
     Where exists
    ( select b.UNIQUESPID,b.ACCOUNTNUM, b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
        From PGE_CCBTOEDGIS_STG_04112017_1 b
       where b.servicepointid = d.service_point_id
         and to_char(b.batchdate,'MMYYYY') = d.REV_MONTH  || D.REV_YEAR
         and b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG_04112017_1 a where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MMYYYY') = to_char(b.batchdate,'MMYYYY') ));

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

-- Inserting records for Service point and revanue month which is not present in staging tables.

    V_Action_name := 'Inserting Data in the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
    V_Status := log_process_status('I',V_action_name,'',0);
    v_rowsProcessed :=0 ;

    insert into PGE_EXT_CCB_METER_LOAD_1 (SERVICE_POINT_ID,UNQSPID,ACCT_ID,REV_MONTH,REV_YEAR,REV_KWHR,REV_KW,PFACTOR,SM_SP_STATUS)
    select b.servicepointid,b.UNIQUESPID,b.ACCOUNTNUM,to_char(b.batchdate,'MM'),to_char(b.batchdate,'YYYY'), b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
      From PGE_CCBTOEDGIS_STG_04112017_1 b
     where b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG_04112017_1 a where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MM') = to_char(b.batchdate,'MM') )
       and not exists ( select * from PGE_EXT_CCB_METER_LOAD_1 c
                         where c.SERVICE_POINT_ID = b.servicepointid
                           and c.REV_MONTH = to_char(b.batchdate,'MM')
                           and c.Rev_year  = to_char(b.batchdate,'YYYY')   );

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

    update PGE_CCB_SP_ACTION set TLMPROCESSED = 'YES' ;

    V_Action_name := 'Gathering Stats on the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');

    V_Status := log_process_status('I',V_action_name,'',0);
    dbms_stats.gather_table_stats('PGEDATA','PGE_EXT_CCB_METER_LOAD_1');
    V_Status := log_process_status('U',V_action_name,'',0);


    Delete From PGE_CCBTOEDGIS_STG_04112017_1 VSP
     where ( VSP.servicepointid , TO_CHAR(VSP.DATECREATED, 'YYYYMMDDHH24MISS'))
           in ( Select stg1.servicepointid,TO_CHAR(stg1.DATECREATED, 'YYYYMMDDHH24MISS')
                  From PGEDATA.PGE_CCBTOEDGIS_STG_04112017_1 stg1 where stg1.DATECREATED is not null
                minus
                Select stg2.servicepointid,max(TO_CHAR(stg2.DATECREATED, 'YYYYMMDDHH24MISS'))
                  from PGEDATA.PGE_CCBTOEDGIS_STG_04112017_1 stg2  where stg2.DATECREATED is not null
                 group by stg2.servicepointid
               );

EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);
     V_Status := log_process_status('I',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);
End;
