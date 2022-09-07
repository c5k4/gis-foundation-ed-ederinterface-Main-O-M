Prompt drop Function POPULATE_TRF_PEAK_TABLE;
DROP FUNCTION EDTLM.POPULATE_TRF_PEAK_TABLE
/

Prompt Function POPULATE_TRF_PEAK_TABLE;
--
-- POPULATE_TRF_PEAK_TABLE  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."POPULATE_TRF_PEAK_TABLE" (
     p_schema VARCHAR2,
     endBatchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_dateStr VARCHAR(10) := to_char(endBatchDate, 'dd-mon-yy');
     v_sourceTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_PEAK';
     v_targetTable2 VARCHAR2(50) := 'TRF_PEAK_BY_CUST_TYP';
     v_targetTable3 VARCHAR2(50) := 'TRF_BANK_PEAK'; --Subhankar
     v_targetTable2_constraint VARCHAR2(50) := 'TRF_PEAK_BY_CUST_TYP_FK1';
     v_targetTable3_constraint VARCHAR2(50) := 'TRF_BANK_PEAK_FK1';
     v_totalRowsMigrated NUMBER := 0;
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'TRF_PEAK_GEN_HIST';
          v_targetTable := 'TRF_PEAK_GEN';
          v_targetTable2 := 'TRF_PEAK_GEN_BY_CUST_TYP';
          v_targetTable3  := 'TRF_BANK_PEAK_GEN'; --Subhankar
          v_targetTable2_constraint := 'TRF_PEAK_GEN_BY_CUST_TYP_FK1';
          v_targetTable3_constraint := 'TRF_BANK_PEAK_GEN_FK1';
     end if;
     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

     -- empty temp tables
     v_stmt := '
          truncate table '||p_schema||'.'||v_targetTable2||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     --Subhankar
      v_stmt := '
          truncate table '||p_schema||'.'||v_targetTable3||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- has to disable constraints to truncate main target table
     v_stmt := '
          alter table '||p_schema||'.'||v_targetTable2||' disable constraint '||v_targetTable2_constraint;
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          alter table '||p_schema||'.'||v_targetTable3||' disable constraint '||v_targetTable3_constraint;
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          truncate table '||p_schema||'.'||v_targetTable||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          alter table '||p_schema||'.'||v_targetTable2||' enable constraint '||v_targetTable2_constraint;
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_stmt := '
          alter table '||p_schema||'.'||v_targetTable3||' enable constraint '||v_targetTable3_constraint;
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- create shell TRF_PEAK records
     v_stmt := '
          insert into
               '||p_schema||'.'||v_targetTable||' (TRF_ID)
          select ID
          from '||p_schema||'.TRANSFORMER a';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     -- Determine the summer peaks.  Note that this could produce duplicate records
     -- if the peak loads are the same for two or more months
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_SMR';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


      v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_SMR
          select
               a.TRF_ID, a.BATCH_DATE smr_peak_date, a.TRF_PEAK_KVA smr_kva, 0, 0,
               a.SM_CUST_TOTAL smr_peak_sm_cust_cnt, a.CCB_CUST_TOTAL smr_peak_total_cust_cnt,
               a.load_undetermined LOAD_UNDETERMINED_SMR,
               (case
                  when a.trf_peak_kva = 0 then 0
                  when a.trf_peak_kw = 0 then 0
                  when round((a.trf_ave_kw/a.trf_peak_kw)*100, 0) > 999 then 999
                  else round((a.trf_ave_kw/a.trf_peak_kw)*100, 0)
                end) smr_lf
          from
               '||p_schema||'.'||v_sourceTable||' a,
               (
               select
                    b.trf_id, max(b.TRF_PEAK_KVA) smr_kva from '||p_schema||'.'||v_sourceTable||' b
               where
                    b.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
                    SUBSTR((to_char(b.BATCH_DATE,''MONTH'')),1,3) in (''APR'',''MAY'',''JUN'',''JUL'',''AUG'',''SEP'',''OCT'')
               group by b.TRF_ID
               ) Y
          where
               a.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
               a.TRF_ID = y.trf_id and
               a.TRF_PEAK_KVA=y.smr_kva and
               SUBSTR((to_char(a.BATCH_DATE,''MONTH'')),1,3) in (''APR'',''MAY'',''JUN'',''JUL'',''AUG'',''SEP'',''OCT'')
     ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- If there are duplicate records, find the record with latest batch date
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_SMR_2';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

    v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_SMR_2
               select
                    a.TRF_ID,0,a.SMR_KVA,0,a.SMR_PEAK_DATE,a.SMR_PEAK_SM_CUST_CNT,a.SMR_PEAK_TOTAL_CUST_CNT, a.LOAD_UNDETERMINED_SMR, a.SMR_LF
               from
                    '||p_schema||'.CALC_TRF_PEAK_SMR a
               join
                    (
                    select
                         c.TRF_ID,
                         max(c.smr_peak_date) smr_peak_date
                    from '||p_schema||'.CALC_TRF_PEAK_SMR c
                    group by c.trf_id
                    ) b
                on
                    a.TRF_ID=b.TRF_ID and
                    a.SMR_PEAK_DATE=b.smr_peak_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update TRF_PEAK summer peak values
      v_stmt := '
          update
          (
               select
                   c.smr_peak_date old_smr_peak_date, c.smr_kva old_smr_kva, c.smr_peak_sm_cust_cnt old_smr_peak_sm_cust_cnt, c.smr_peak_total_cust_cnt old_smr_peak_total_cust_cnt,c.SMR_LOAD_UNDETERMINED OLD_SMR_LOAD_UNDETERMINED, c.SMR_LF OLD_SMR_LF,
                   b.smr_peak_date new_smr_peak_date, b.smr_kva new_smr_kva, b.smr_peak_sm_cust_cnt new_smr_peak_sm_cust_cnt, b.smr_peak_total_cust_cnt new_smr_peak_total_cust_cnt, b.LOAD_UNDETERMINED_SMR NEW_SMR_LOAD_UNDETERMINED, b.SMR_LF NEW_SMR_LF
               from
                    '||p_schema||'.'||v_targetTable||' C,
                    '||p_schema||'.CALC_TRF_PEAK_SMR_2 B
               where
                    C.TRF_ID = B.TRF_ID
          )
          set
               old_smr_peak_date=new_smr_peak_date,
               old_smr_kva=new_smr_kva,
               old_smr_peak_sm_cust_cnt=new_smr_peak_sm_cust_cnt,
               old_smr_peak_total_cust_cnt=new_smr_peak_total_cust_cnt,
               OLD_SMR_LOAD_UNDETERMINED = NEW_SMR_LOAD_UNDETERMINED,
               OLD_SMR_LF = NEW_SMR_LF';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     -- Determine the winter peaks.  Note that this could produce duplicate records
     -- if the peak loads are the same for two or more months
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_WNTR';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

      v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_WNTR
          select
                a.TRF_ID, a.BATCH_DATE wntr_peak_date, a.TRF_PEAK_KVA wntr_kva, 0, 0,
                a.SM_CUST_TOTAL wntr_peak_sm_cust_cnt, a.CCB_CUST_TOTAL wntr_peak_total_cust_cnt,
                a.load_undetermined LOAD_UNDETERMINED_WNTR,
                (case
                  when a.trf_peak_kva = 0 then 0
                  when a.trf_peak_kw = 0 then 0
                  when round((a.trf_ave_kw/a.trf_peak_kw)*100, 0) > 999 then 999
                  else round((a.trf_ave_kw/a.trf_peak_kw)*100, 0)
                end) wntr_lf
          from
               '||p_schema||'.'||v_sourceTable||' a,
               (
               select
                    b.trf_id, max(b.TRF_PEAK_KVA) wntr_kva from '||p_schema||'.'||v_sourceTable||' b
               where
                    b.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
                    SUBSTR((to_char(b.BATCH_DATE,''MONTH'')),1,3) in (''NOV'',''DEC'',''JAN'',''FEB'',''MAR'')
               group by b.TRF_ID
               ) Y
          where
               a.BATCH_DATE > add_months(to_date('''||v_dateStr||''',''dd-mon-yy''),-12) and
               a.TRF_ID = y.trf_id and
               a.TRF_PEAK_KVA=y.wntr_kva and
               SUBSTR((to_char(a.BATCH_DATE,''MONTH'')),1,3) in (''NOV'',''DEC'',''JAN'',''FEB'',''MAR'')
     ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- If there are duplicate records, find the record with latest batch date
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_PEAK_WNTR_2';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

      v_stmt := '
          insert into '||p_schema||'.CALC_TRF_PEAK_WNTR_2
               select
                   a.TRF_ID,0,a.WNTR_KVA,0,a.WNTR_PEAK_DATE,a.WNTR_PEAK_SM_CUST_CNT,a.WNTR_PEAK_TOTAL_CUST_CNT, a.LOAD_UNDETERMINED_WNTR, a.WNTR_LF
               from
                    '||p_schema||'.CALC_TRF_PEAK_WNTR a
               join
                    (
                    select
                         c.TRF_ID,
                         max(c.wntr_peak_date) wntr_peak_date
                    from '||p_schema||'.CALC_TRF_PEAK_WNTR c
                    group by c.trf_id
                    ) b
                on
                    a.TRF_ID=b.TRF_ID and
                    a.WNTR_PEAK_DATE=b.wntr_peak_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update TRF_PEAK winter peak values
      v_stmt := '
          update
          (
               select
                    c.wntr_peak_date old_wntr_peak_date, c.wntr_kva old_wntr_kva, c.wntr_peak_sm_cust_cnt old_wntr_peak_sm_cust_cnt, c.wntr_peak_total_cust_cnt old_wntr_peak_total_cust_cnt,c.WNTR_LOAD_UNDETERMINED OLD_WNTR_LOAD_UNDETERMINED, c.WNTR_LF OLD_WNTR_LF,
                    b.wntr_peak_date new_wntr_peak_date, b.wntr_kva new_wntr_kva, b.wntr_peak_sm_cust_cnt new_wntr_peak_sm_cust_cnt, b.wntr_peak_total_cust_cnt new_wntr_peak_total_cust_cnt, b.LOAD_UNDETERMINED_WNTR NEW_WNTR_LOAD_UNDETERMINED, b.WNTR_LF NEW_WNTR_LF
               from
                    '||p_schema||'.'||v_targetTable||' C,
                    '||p_schema||'.CALC_TRF_PEAK_WNTR_2 B
               where
                    C.TRF_ID = B.TRF_ID
          )
          set
               old_wntr_peak_date=new_wntr_peak_date,
               old_wntr_kva=new_wntr_kva,
               old_wntr_peak_sm_cust_cnt=new_wntr_peak_sm_cust_cnt,
               old_wntr_peak_total_cust_cnt=new_wntr_peak_total_cust_cnt,
               OLD_WNTR_LOAD_UNDETERMINED = NEW_WNTR_LOAD_UNDETERMINED,
               OLD_WNTR_LF = NEW_WNTR_LF';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update Data Start Date
     v_stmt := '
          truncate table '||p_schema||'.calc_trf_data_start_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||p_schema||'.calc_trf_data_start_date
          select
               trf_id,
               min(BATCH_DATE) data_start_date
          from '||p_schema||'.'||v_sourceTable||'
          group by trf_id';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          update
          (
               select a.data_start_date old_data_start_date, b.data_start_date new_data_start_date
               from
                    '||p_schema||'.'||v_targetTable||' a,
                    '||p_schema||'.CALC_TRF_DATA_START_DATE b
               where
                    a.trf_id = b.trf_id
          )
          set
               old_data_start_date = new_data_start_date';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- delete TRFs with null winter and summer timestamp
     v_stmt := '
          delete from
               '||p_schema||'.'||v_targetTable||'
          where
               SMR_PEAK_DATE is null and WNTR_PEAK_DATE is null';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     -- deduct rows deleted from total count
     v_totalRowsMigrated := v_totalRowsMigrated - SQL%ROWCOUNT;

     -- update monthly load log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
                   ownname => '''||p_schema||''',
               tabname => '''||v_targetTable||''',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


    -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
                   ownname => '''||p_schema||''',
               tabname => ''CALC_TRF_PEAK_SMR_2'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
                   ownname => '''||p_schema||''',
               tabname => ''CALC_TRF_PEAK_WNTR_2'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');

EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=POPULATE_'||v_targetTable||'_'||to_char(endBatchDate,'mmddyyyy'));

END POPULATE_TRF_PEAK_TABLE;
/
