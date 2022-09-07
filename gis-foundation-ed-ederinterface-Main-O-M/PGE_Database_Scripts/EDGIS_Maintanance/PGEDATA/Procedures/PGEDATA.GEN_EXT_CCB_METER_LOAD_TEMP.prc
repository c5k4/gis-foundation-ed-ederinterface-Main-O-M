Prompt drop Procedure GEN_EXT_CCB_METER_LOAD_TEMP;
DROP PROCEDURE PGEDATA.GEN_EXT_CCB_METER_LOAD_TEMP
/

Prompt Procedure GEN_EXT_CCB_METER_LOAD_TEMP;
--
-- GEN_EXT_CCB_METER_LOAD_TEMP  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.GEN_EXT_CCB_METER_LOAD_TEMP
As
     V_Date date;
     DB_ERROR_CODE VARCHAR2(20);
     DB_ERROR_MSG  VARCHAR2(200);
     V_Action_name Varchar2(50);
     V_Status Varchar2(50);
     v_rowsProcessed Number;
begin
V_Action_name := 'Deleting Data from Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
-- Delete existing record for Service point and revanue month which present in staging tables.
    delete /*+ parallel(d,4) */ from PGE_EXT_CCB_METER_LOAD_ARCHIVE d
     Where exists
    ( select b.UNIQUESPID,b.ACCOUNTNUM, b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
        From PGE_CCBTOEDGIS_STG b
       where b.servicepointid = d.service_point_id
         and to_char(b.batchdate,'MMYY') = d.REV_MONTH  || d.rev_year
         and b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MMYY') = to_char(b.batchdate,'MMYY') ));

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
V_Action_name := 'Inserting Data in the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
    V_Status := log_process_status('I',V_action_name,'',0);
    v_rowsProcessed :=0 ;

    insert into PGE_EXT_CCB_METER_LOAD_ARCHIVE (SERVICE_POINT_ID,UNQSPID,ACCT_ID,REV_MONTH,REV_YEAR,REV_KWHR,REV_KW,PFACTOR,SM_SP_STATUS)
    select b.servicepointid,b.UNIQUESPID,b.ACCOUNTNUM,to_char(b.batchdate,'MM'),to_char(b.batchdate,'YYYY'), b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
      From PGE_CCBTOEDGIS_STG b
     where b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MM') = to_char(b.batchdate,'MM') )
       and not exists ( select * from PGE_EXT_CCB_METER_LOAD_ARCHIVE c
                         where c.SERVICE_POINT_ID = b.servicepointid
                           and c.REV_MONTH = to_char(b.batchdate,'MM')
                           and c.Rev_year  = to_char(b.batchdate,'YYYY')   );

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

   V_Action_name := 'Deleting Data from Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
-- Delete existing record for Service point and revanue month which present in staging tables.
   execute immediate 'truncate table PGE_EXT_CCB_METER_LOAD';

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

-- Inserting records for Service point and revanue month which is not present in staging tables.

    V_Action_name := 'Inserting Data in the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
    V_Status := log_process_status('I',V_action_name,'',0);
    v_rowsProcessed :=0 ;

    insert into PGE_EXT_CCB_METER_LOAD (SERVICE_POINT_ID,UNQSPID,ACCT_ID,REV_MONTH,REV_YEAR,REV_KWHR,REV_KW,PFACTOR,SM_SP_STATUS)
    select b.servicepointid,b.UNIQUESPID,b.ACCOUNTNUM,to_char(b.batchdate,'MM'),to_char(b.batchdate,'YYYY'), b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
      From PGE_CCBTOEDGIS_STG b
     where b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a where a.servicepointid = b.servicepointid
                                and to_char(a.batchdate,'MM') = to_char(b.batchdate,'MM') )
       and not exists ( select * from PGE_EXT_CCB_METER_LOAD c
                         where c.SERVICE_POINT_ID = b.servicepointid
                           and c.REV_MONTH = to_char(b.batchdate,'MM')
                           and c.Rev_year  = to_char(b.batchdate,'YYYY')   );

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

    update PGE_CCB_SP_ACTION set TLMPROCESSED = 'YES' ;

    V_Action_name := 'Gathering Stats on the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');

    V_Status := log_process_status('I',V_action_name,'',0);
    dbms_stats.gather_table_stats('PGEDATA','PGE_EXT_CCB_METER_LOAD');
    V_Status := log_process_status('U',V_action_name,'',0);

	 execute immediate 'DROP TABLE PGE_CCBTOEDGIS_STG PURGE';

EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);
     V_Status := log_process_status('I',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);
End;

/
