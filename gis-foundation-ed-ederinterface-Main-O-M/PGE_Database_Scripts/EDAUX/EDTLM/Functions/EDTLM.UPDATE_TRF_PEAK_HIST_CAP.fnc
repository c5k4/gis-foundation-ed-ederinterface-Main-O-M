Prompt drop Function UPDATE_TRF_PEAK_HIST_CAP;
DROP FUNCTION EDTLM.UPDATE_TRF_PEAK_HIST_CAP
/

Prompt Function UPDATE_TRF_PEAK_HIST_CAP;
--
-- UPDATE_TRF_PEAK_HIST_CAP  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPDATE_TRF_PEAK_HIST_CAP" (
     p_schema VARCHAR2,
     batchDate Date,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_season VARCHAR2(1);
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
     v_sourceTable VARCHAR2(50) := 'SP_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'SP_PEAK_GEN_HIST';
          v_targetTable := 'TRF_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;

     --determine season
     v_batchMonth := SUBSTR((to_char(batchDate,'MONTH')),1,3);
     if (upper(v_batchMonth) in ('NOV','DEC','JAN','FEB','MAR')) then
          v_season := 'W';
     else
          v_season := 'S';
     end if;
     DBMS_OUTPUT.PUT_LINE('SEASON - '||v_season);

     -- create monthly load log record
     --------v_log_status := INSERT_MONTHLY_LOAD_LOG('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'));

     -- truncate work tables
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_HIST_CAP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     v_stmt := '
          truncate table '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- insert into work tables
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_HIST_CAP (TRF_PEAK_HIST_ID)
               select a.ID
               from
                    '||p_schema||'.'||v_targetTable||' a
               where
                    a.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     v_stmt := '
          insert into '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA
               select
                    a.'||v_targetTableFkColumn||',
                    ''DOMESTIC'' cust_typ,
                    sum(a.SP_KVA_TRF_PEAK) total_kva
               from
                    '||p_schema||'.'||v_sourceTable||' a,
                    '||p_schema||'.'||v_targetTable||' b
               where
                    a.'||v_targetTableFkColumn||' = b.ID and
                    b.BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                    a.CUST_TYP = ''DOM''
               group by a.'||v_targetTableFkColumn||'
               union all
               select
                    a.'||v_targetTableFkColumn||',
                    ''NON_DOMESTIC'' cust_typ,
                    sum(a.SP_KVA_TRF_PEAK) total_kva
               from
                    '||p_schema||'.'||v_sourceTable||' a,
                    '||p_schema||'.'||v_targetTable||' b
               where
                    a.'||v_targetTableFkColumn||' = b.ID and
                    b.BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                    a.CUST_TYP <> ''DOM''
               group by a.'||v_targetTableFkColumn||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- update TRF_PEAK_HIST
     -- set residential total KVA
     v_stmt := '
          update '||p_schema||'.CALC_TRF_HIST_CAP a set
               DOM_TOTAL_KVA =
                    NVL
                    (
                          (
                          select
                               b.total_kva
                          from
                               '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA b
                          where
                               a.TRF_PEAK_HIST_ID = b.TRF_PEAK_HIST_ID and
                               b.CUST_TYP=''DOMESTIC''
                          ),
                          0
                     )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- set non-residential total KVA
     v_stmt := '
          update '||p_schema||'.CALC_TRF_HIST_CAP a set
               NON_DOM_TOTAL_KVA =
                    NVL
                    (
                          (
                          select
                               b.total_kva
                          from
                               '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA b
                          where
                               a.TRF_PEAK_HIST_ID = b.TRF_PEAK_HIST_ID and
                               b.CUST_TYP=''NON_DOMESTIC''
                          ),
                          0
                     )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     --if residential has more load overall than non-residential, set CUST_TYP_WITH_GREATER_LOAD to 'DOMESTIC'
     v_stmt := '
          update '||p_schema||'.CALC_TRF_HIST_CAP a set
               a.CUST_TYP_WITH_GREATER_LOAD = ''DOMESTIC''
          where
               a.DOM_TOTAL_KVA > a.NON_DOM_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- calculate name plate total kva
     v_stmt := '
          update '||p_schema||'.CALC_TRF_HIST_CAP a set
          a.TRF_NP_TOTAL_KVA =
          NVL
          (
               (
               select b.TOTAL_NP_CAP
               from (
                    select
                         a.ID '||v_targetTableFkColumn||',
                         sum(c.NP_KVA) TOTAL_NP_CAP
                    from
                         '||p_schema||'.'||v_targetTable||' a,
                         '||p_schema||'.TRANSFORMER b,
                         '||p_schema||'.TRANSFORMER_BANK c
                    where
                         a.TRF_ID = b.ID and
                         c.TRF_ID = b.ID
                    group by a.ID
                    ) b
               where
                    a.TRF_PEAK_HIST_ID = b.'||v_targetTableFkColumn||'
               ),
               0
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- finally, update TRF_PEAK_HIST.TRF_CAP
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' x set
          TRF_CAP =
          NVL
          (
               (
               select y.trf_cap
               from (
                    select
                         b.ID,
                         a.TRF_NP_TOTAL_KVA*decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'', d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from
                         '||p_schema||'.CALC_TRF_HIST_CAP a,
                         '||p_schema||'.'||v_targetTable||' b,
                         '||p_schema||'.TRANSFORMER c,
                         '||p_schema||'.TRF_NP_CAP_MULT d
                    where
                         a.TRF_PEAK_HIST_ID = b.id and
                         b.TRF_ID = c.ID and
                         d.SEASON = '''||v_season||''' and
                         a.TRF_NP_TOTAL_KVA >= d.KVA_LOW and
                         a.TRF_NP_TOTAL_KVA <= d.KVA_HIGH and
                         d.PHASE_CD = c.PHASE_CD and
                         d.INSTALLATION_TYP = c.INSTALLATION_TYP and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG
                    ) Y
               where
                    x.id = y.id
               ),
               0
          )
          where x.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     --------execute immediate v_stmt;

     -- update migration log record
     --------v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'),SQL%ROWCOUNT);

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for tablename=UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate, 'mmddyyyy'));

END UPDATE_TRF_PEAK_HIST_CAP;
/
