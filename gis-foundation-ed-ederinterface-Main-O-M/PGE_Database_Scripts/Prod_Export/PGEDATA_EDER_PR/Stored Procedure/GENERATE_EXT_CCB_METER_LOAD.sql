--------------------------------------------------------
--  DDL for Procedure GENERATE_EXT_CCB_METER_LOAD
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."GENERATE_EXT_CCB_METER_LOAD" 
As
     V_Date date;
     DB_ERROR_CODE VARCHAR2(20);
     DB_ERROR_MSG  VARCHAR2(200);
     V_Action_name Varchar2(50);
     V_Status Varchar2(50);
     v_rowsProcessed Number;
begin
-- 5/13/19 - tjj4 - modified the procedure to only consider records from the full synch run from CCB
--                  any record with an Action of null (this is the full synch run) will replace
--                  whatever is already in the meter_load table for that service point and revenue
--                  month.
--                  all Delta records, those with Action not null, will be ignored

   V_Action_name := 'Deleting Data from Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;

-- Note: under normal operating circumstances this statement should do nothing,
--       it's here to handle the cases where we have to rerun the interface due
--       to a processing error or for the incredibly rare case of EDGIS getting
--       two mass loads for the same revenue date

    delete /*+ parallel(d,4) */ from PGE_EXT_CCB_METER_LOAD d
     Where exists
    ( select 'me'
        From PGE_CCBTOEDGIS_STG b
       where  b.servicepointid = d.service_point_id
        and b.error_description is null
         and to_char(b.batchdate,'MMYYYY') = d.REV_MONTH  || D.REV_YEAR
         and action is null);  -- no need to consider date created here

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

-- Inserting in meter_load Service point and revenue date records for full synch run

    V_Action_name := 'Inserting Data in the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
    V_Status := log_process_status('I',V_action_name,'',0);
    v_rowsProcessed :=0 ;

    insert into PGE_EXT_CCB_METER_LOAD (SERVICE_POINT_ID,UNQSPID,ACCT_ID,REV_MONTH,REV_YEAR,REV_KWHR,REV_KW,PFACTOR,SM_SP_STATUS)
    select b.servicepointid,b.UNIQUESPID,b.ACCOUNTNUM,to_char(b.batchdate,'MM'),to_char(b.batchdate,'YYYY'), b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
      From PGE_CCBTOEDGIS_STG b
     where action is null and b.error_description is null
       and b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a
                              where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MM') = to_char(b.batchdate,'MM')
                                and a.action is null );
        -- no need to consider the record might already exist because we deleted it above

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

    update PGE_CCB_SP_ACTION set TLMPROCESSED = 'YES' ;

    V_Action_name := 'Gathering Stats on the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');

    V_Status := log_process_status('I',V_action_name,'',0);
    dbms_stats.gather_table_stats('PGEDATA','PGE_EXT_CCB_METER_LOAD');
    V_Status := log_process_status('U',V_action_name,'',0);

-- m4jf edgisrearch -Record will not be deleted , will be deleted using data retention job
    --Delete From pge_ccbtoedgis_stg VSP
     --where ( VSP.servicepointid , TO_CHAR(VSP.DATECREATED, 'YYYYMMDDHH24MISS'))
           --in ( Select stg1.servicepointid,TO_CHAR(stg1.DATECREATED, 'YYYYMMDDHH24MISS')
                --  From PGEDATA.pge_ccbtoedgis_stg stg1 where stg1.error_description is null and stg1.DATECREATED is not null
              --  minus
               -- Select stg2.servicepointid,max(TO_CHAR(stg2.DATECREATED, 'YYYYMMDDHH24MISS'))
                --  from PGEDATA.pge_ccbtoedgis_stg stg2  where stg2.error_description is null and stg2.DATECREATED is not null --and stg2.action is null -- a3di: changed to prevent daily delta deltion from STG table
               --  group by stg2.servicepointid
              -- );


EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);
     V_Status := log_process_status('I',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);
End;
