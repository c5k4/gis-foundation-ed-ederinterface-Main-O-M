Prompt drop Function LOAD_TRF_CCB_METER;
DROP FUNCTION EDTLM.LOAD_TRF_CCB_METER
/

Prompt Function LOAD_TRF_CCB_METER;
--
-- LOAD_TRF_CCB_METER  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_TRF_CCB_METER" (
     p_schema VARCHAR2,
     batchDate DATE)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
BEGIN
     -- create migration log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('TRF_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'));

     v_stmt := 'insert into '||p_schema||'.TRF_CCB_METER_LOAD (
                     trf_id,
                     batch_date
                     )
                select distinct
                     a.TRF_ID,
                     to_date('''||v_batchDateStr||''',''dd-mon-yy'')
                FROM '
                     ||p_schema||'.METER a, '
                     ||p_schema||'.TRANSFORMER b
                WHERE
                     a.TRF_ID      = b.ID ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('TRF_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''TRF_CCB_METER_LOAD'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('TRF_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'Load failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=TRF_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'));
END LOAD_TRF_CCB_METER;
/
