Prompt drop Function LOAD_SP_CCB_METER;
DROP FUNCTION EDTLM.LOAD_SP_CCB_METER
/

Prompt Function LOAD_SP_CCB_METER;
--
-- LOAD_SP_CCB_METER  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_SP_CCB_METER" (
     p_schema VARCHAR2,
     batchDate DATE)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_rowsProcessed NUMBER := 0;
     v_totalRowsMigrated NUMBER := 0;
     v_season VARCHAR2(1);
     v_seasonal_pf NUMBER;

BEGIN
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     v_batchMonth := SUBSTR((to_char(batchDate,'MONTH')),1,3);

     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('SP_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'));

     v_stmt := '
          insert into '||p_schema||'.SP_CCB_METER_LOAD (
               SERVICE_POINT_ID,
               METER_ID,
               CUST_TYP,
               REV_KW,
               REV_KWHR,
               PFACTOR,
               TRF_CCB_METER_LOAD_ID
               )
          select
               a.SERVICE_POINT_ID,
               a.ID,
               a.CUST_TYP,
               b.REV_KW,
               --b.REV_KWHR/10, -- Scaling down by 10 is being removed as this is already happening in DATA_LOAD_VALIDATION
			   b.REV_KWHR,
               b.PFACTOR,
               c.id
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b,'
               ||p_schema||'.TRF_CCB_METER_LOAD c
          WHERE
                c.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                a.trf_id = c.trf_id and
                a.SERVICE_POINT_ID = b.SERVICE_POINT_ID ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('SP_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);


     --v_stmt := 'delete from ' || p_schema || '.SP_CCB_METER_LOAD where rev_kw is null and REV_KWHR is null'; -- Updated as per request from J2T4 and TJJ4
	 v_stmt := 'delete from ' || p_schema || '.SP_CCB_METER_LOAD where rev_kw is null and REV_KWHR is null';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''SP_CCB_METER_LOAD'',
               estimate_percent => 10     )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');
EXCEPTION
    WHEN OTHERS THEN
         v_log_status := LOG_MONTHLY_LOAD_ERROR('SP_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
         raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=SP_CCB_METER_LOAD_'||to_char(batchDate,'mmddyyyy'));
END LOAD_SP_CCB_METER;
/
