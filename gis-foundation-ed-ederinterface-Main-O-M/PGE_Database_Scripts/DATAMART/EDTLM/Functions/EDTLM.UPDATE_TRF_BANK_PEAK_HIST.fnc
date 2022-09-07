Prompt drop Function UPDATE_TRF_BANK_PEAK_HIST;
DROP FUNCTION EDTLM.UPDATE_TRF_BANK_PEAK_HIST
/

Prompt Function UPDATE_TRF_BANK_PEAK_HIST;
--
-- UPDATE_TRF_BANK_PEAK_HIST  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPDATE_TRF_BANK_PEAK_HIST" (
     p_schema VARCHAR2,
     batchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_recs_updated NUMBER := 0;
     v_totalRowsMigrated NUMBER := 0;
     v_sourceTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'TRF_PEAK_GEN_HIST';
          v_targetTable := 'TRF_BANK_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;

     -- create migration log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'));

     -- truncate work tables
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     ---
     --- create work table mapping trf_peak_ids with number of transformer units
     ---
      v_stmt :=
           '
           insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST
                select a.ID, count(*) from '||p_schema||'.'||v_sourceTable||' a, '||p_schema||'.'||v_targetTable||' b
                where
                     b.'||v_targetTableFkColumn||' = a.ID
                     and a.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')
                group by a.ID
            ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     ----------------------------------------------------------------------------------------------
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 1 - Exactly one transformer unit');
     ----------------------------------------------------------------------------------------------
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' a set
               a.TRF_PEAK_KVA = NVL((select b.TRF_PEAK_KVA from '||p_schema||'.'||v_sourceTable||' b where b.ID = a.'||v_targetTableFkColumn||'),0)
          where a.'||v_targetTableFkColumn||' in
                    (
                    select TRF_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST
                    where TRF_BANKS_CNT = 1
                    )
          ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     ----------------------------------------------------------------------------------------------
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 2 - Exactly two transformer units, use ODOD');
     ----------------------------------------------------------------------------------------------

     --
     -- Scnenario 2a - Lighter
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for lighter transformers (highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L
               select X.'||v_targetTableFkColumn||', max(X.ID) '||v_targetTableFkColumn||', null NP_KVA
               from
                    '||p_schema||'.'||v_targetTable||' X,
                    (
                    select a.'||v_targetTableFkColumn||', max(a.NP_KVA) NP_KVA
                    from '||p_schema||'.'||v_targetTable||' a, CALC_TRF_BANK_PEAK_HIST b
                    where
                         b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                         b.TRF_BANKS_CNT=2
                    group by
                         a.'||v_targetTableFkColumn||'
                    ) Y
              where
                   X.'||v_targetTableFkColumn||' = Y.'||v_targetTableFkColumn||' and
                   X.NP_KVA = Y.NP_KVA
              group by x.'||v_targetTableFkColumn||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('ODOD','L',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;

     --
     -- Scnenario 2b - Power
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for power transformers (NOT highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P
               select A.'||v_targetTableFkColumn||', A.ID, null
               from
                    '||p_schema||'.'||v_targetTable||' A, CALC_TRF_BANK_PEAK_HIST b
               where
                    b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                    b.TRF_BANKS_CNT=2 and
                    A.ID not in (select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_L)';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('ODOD','P',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;


     ----------------------------------------------------------------------------------------------
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 3 - Exactly three transformer units, secondary voltage NOT 120/240 3phase, use YY (or DY)');
     ----------------------------------------------------------------------------------------------

     --
     -- Scnenario 3a - Lighter
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for lighter transformers (highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L
               select X.'||v_targetTableFkColumn||', max(X.ID), null
               from
                    '||p_schema||'.'||v_targetTable||' X,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    (
                    select a.'||v_targetTableFkColumn||', max(a.NP_KVA) NP_KVA
                    from '||p_schema||'.'||v_targetTable||' a, CALC_TRF_BANK_PEAK_HIST b
                    where
                         b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                         b.TRF_BANKS_CNT=3
                    group by
                         a.'||v_targetTableFkColumn||'
                    ) Y
              where
                   X.'||v_targetTableFkColumn||' = Y.'||v_targetTableFkColumn||' and
                   X.'||v_targetTableFkColumn||' = Q.ID and
                   Q.TRF_ID = R.ID and
                   (R.LOWSIDE_VOLTAGE is null OR R.LOWSIDE_VOLTAGE != 23) and
                   X.NP_KVA = Y.NP_KVA
              group by x.'||v_targetTableFkColumn||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('YY','L',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;

     --
     -- Scnenario 3b - Power
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for power transformers (NOT highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P
               select A.'||v_targetTableFkColumn||', A.ID, null
               from
                    '||p_schema||'.'||v_targetTable||' A,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    CALC_TRF_BANK_PEAK_HIST b
               where
                    b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                    b.TRF_PEAK_HIST_ID = Q.ID and
                    Q.TRF_ID = R.ID and
                   (R.LOWSIDE_VOLTAGE is null OR R.LOWSIDE_VOLTAGE != 23) and
                    b.TRF_BANKS_CNT=3 and
                    A.ID not in (select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_L)';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('YY','P',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;


     ----------------------------------------------------------------------------------------------
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 4 - Exactly three transformer units, secondary voltage EQUALS 120/240 3phase, transformer primary EQUALS circuit primary, use DD');
     ----------------------------------------------------------------------------------------------

     --
     -- Scnenario 4a - Lighter
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for lighter transformers (highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L
               select X.'||v_targetTableFkColumn||', max(X.ID), decode(Y.NP_KVA, 0, .1, Y.NP_KVA) -- if data is dirty and this is zero, set to .1 so calculation will not fail
               from
                    '||p_schema||'.'||v_targetTable||' X,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    '||p_schema||'.OPERATING_VOLTAGE OV,
                    (
                    select a.'||v_targetTableFkColumn||', max(a.NP_KVA) NP_KVA
                    from '||p_schema||'.'||v_targetTable||' a, CALC_TRF_BANK_PEAK_HIST b
                    where
                         b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                         b.TRF_BANKS_CNT=3
                    group by
                         a.'||v_targetTableFkColumn||'
                    ) Y
              where
                   X.'||v_targetTableFkColumn||' = Y.'||v_targetTableFkColumn||' and
                   X.'||v_targetTableFkColumn||' = Q.ID and
                   Q.TRF_ID = R.ID and
                   R.OPERATING_VOLTAGE = OV.CD and
                   OV.VOLTAGE = (select CV.circuit_voltage from '||p_schema||'.CIRCUIT_VOLTAGE CV where CV.circuit_cd = substr(substr(R.CIRCUIT_ID, -4),1,2)) and
                   R.LOWSIDE_VOLTAGE = 23 and
                   X.NP_KVA = Y.NP_KVA
              group by x.'||v_targetTableFkColumn||', Y.NP_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- for this scenario, we need data from both CALC_TRF_BANK_PEAK_HIST_L and CALC_TRF_BANK_PEAK_HIST_P so
     -- populate CALC_TRF_BANK_PEAK_HIST_P now

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for power transformers (NOT highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P
               select A.'||v_targetTableFkColumn||', A.ID, null
               from
                    '||p_schema||'.'||v_targetTable||' A,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    '||p_schema||'.OPERATING_VOLTAGE OV,
                    CALC_TRF_BANK_PEAK_HIST b
               where
                    b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                    b.TRF_PEAK_HIST_ID = Q.ID and
                    Q.TRF_ID = R.ID and
                    R.OPERATING_VOLTAGE = OV.CD and
                    OV.VOLTAGE = (select CV.circuit_voltage from '||p_schema||'.CIRCUIT_VOLTAGE CV where CV.circuit_cd = substr(substr(R.CIRCUIT_ID, -4),1,2)) and
                    R.LOWSIDE_VOLTAGE = 23 and
                    b.TRF_BANKS_CNT=3 and
                    A.ID not in (select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_L)';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P_UNQ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST record for power transformers (NOT highest NP kva) with the lowest
     -- npKVA
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P_UNQ
               select X.'||v_targetTableFkColumn||', max(X.ID), decode(Y.NP_KVA, 0, .1, Y.NP_KVA) -- if data is dirty and this is zero, set to .1 so calculation will not fail
               from
                    '||p_schema||'.'||v_targetTable||' X,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    '||p_schema||'.OPERATING_VOLTAGE OV,
                    (
                    select a.'||v_targetTableFkColumn||', min(a.NP_KVA) NP_KVA  --- note that the only change is to change max to min
                    from '||p_schema||'.'||v_targetTable||' a, CALC_TRF_BANK_PEAK_HIST b
                    where
                         b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                         b.TRF_BANKS_CNT=3
                    group by
                         a.'||v_targetTableFkColumn||'
                    ) Y
              where
                   X.'||v_targetTableFkColumn||' = Y.'||v_targetTableFkColumn||' and
                   X.'||v_targetTableFkColumn||' = Q.ID and
                   Q.TRF_ID = R.ID and
                   R.OPERATING_VOLTAGE = OV.CD and
                   OV.VOLTAGE = (select CV.circuit_voltage from '||p_schema||'.CIRCUIT_VOLTAGE CV where CV.circuit_cd = substr(substr(R.CIRCUIT_ID, -4),1,2)) and
                   R.LOWSIDE_VOLTAGE = 23 and
                   X.NP_KVA = Y.NP_KVA
              group by x.'||v_targetTableFkColumn||', Y.NP_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     --
     -- Scnenario 4a - Lighter
     --

     -- calculate domestic and non-domestic load and sum them up
     -- cannot use the common function for this scenario since domestic calc is complex
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' X set TRF_PEAK_KVA =
               -- domestic assumed to be 1 phase
               (select round(Y.kva,1) from
                    (
                    select a.TRF_PEAK_KVA_DOM*((100/( 1 + (p.NP_KVA/(2*l.NP_KVA))))/100) kva, a.id from '||p_schema||'.'||v_sourceTable||' a, CALC_TRF_BANK_PEAK_HIST_L l, CALC_TRF_BANK_PEAK_HIST_P_UNQ p
                    where
                         a.ID = l.TRF_PEAK_HIST_ID and
                         a.ID = p.TRF_PEAK_HIST_ID
                    ) Y
               where
                    y.id = x.'||v_targetTableFkColumn||'
               )
               +
               -- non-domestic assumed to be 3 phase
              (select round(Y.kva,1) from
                   (
                   select (a.TRF_PEAK_KVA-a.TRF_PEAK_KVA_DOM)*(b.DISTRIBUTION_PCT/100) kva, a.id from '||p_schema||'.'||v_sourceTable||' a, '||p_schema||'.TRF_LOAD_DIST b
                   where
                         b.CONNECTION_TYP = ''DD'' and
                         b.TRF_TYP = ''L'' and
                         b.PHASE = ''3''
              ) Y
              where
                   y.id = x.'||v_targetTableFkColumn||'
              )
        where X.ID in
                (
                select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_L
                    where TRF_PEAK_HIST_ID = x.'||v_targetTableFkColumn||'
                )
                ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     --
     -- Scnenario 4b - Power
     --

     -- calculate domestic and non-domestic load and sum them up
     -- cannot use the common function for this scenario since domestic calc is complex
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' X set TRF_PEAK_KVA =
               -- domestic assumed to be 1 phase
               (select round(Y.kva,1) from
                    (
                    select a.TRF_PEAK_KVA_DOM*((100/( 1 + ((2*l.NP_KVA)/p.NP_KVA)))/100) kva, a.id from '||p_schema||'.'||v_sourceTable||' a, CALC_TRF_BANK_PEAK_HIST_L l, CALC_TRF_BANK_PEAK_HIST_P_UNQ p
                    where
                         a.ID = l.TRF_PEAK_HIST_ID and
                         a.ID = p.TRF_PEAK_HIST_ID
                    ) Y
               where
                    y.id = x.'||v_targetTableFkColumn||'
               )
               +
               -- non-domestic assumed to be 3 phase
              (select round(Y.kva,1) from
                   (
                   select (a.TRF_PEAK_KVA-a.TRF_PEAK_KVA_DOM)*(b.DISTRIBUTION_PCT/100) kva, a.id from '||p_schema||'.'||v_sourceTable||' a, '||p_schema||'.TRF_LOAD_DIST b
                   where
                         b.CONNECTION_TYP = ''DD'' and
                         b.TRF_TYP = ''P'' and
                         b.PHASE = ''3''
              ) Y
              where
                   y.id = x.'||v_targetTableFkColumn||'
              )
        where X.ID in
                (
                select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_P
                    where TRF_PEAK_HIST_ID = x.'||v_targetTableFkColumn||'
                )
                ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

     ----------------------------------------------------------------------------------------------
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 5 - Exactly three transformer units, secondary voltage EQUALS 120/240 3phase, transformer primary NOT equals circuit primary, use YD');
     ----------------------------------------------------------------------------------------------

     --
     -- Scnenario 5a - Lighter
     --
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for lighter transformers (highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_L
               select X.'||v_targetTableFkColumn||', max(X.ID), null
               from
                    '||p_schema||'.'||v_targetTable||' X,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    '||p_schema||'.OPERATING_VOLTAGE OV,
                    (
                    select a.'||v_targetTableFkColumn||', max(a.NP_KVA) NP_KVA
                    from '||p_schema||'.'||v_targetTable||' a, CALC_TRF_BANK_PEAK_HIST b
                    where
                         b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                         b.TRF_BANKS_CNT=3
                    group by
                         a.'||v_targetTableFkColumn||'
                    ) Y
              where
                   X.'||v_targetTableFkColumn||' = Y.'||v_targetTableFkColumn||' and
                   X.'||v_targetTableFkColumn||' = Q.ID and
                   Q.TRF_ID = R.ID and
                   R.OPERATING_VOLTAGE = OV.CD and
                   OV.VOLTAGE != (select CV.circuit_voltage from '||p_schema||'.CIRCUIT_VOLTAGE CV where CV.circuit_cd = substr(substr(R.CIRCUIT_ID, -4),1,2)) and
                   R.LOWSIDE_VOLTAGE = 23 and
                   X.NP_KVA = Y.NP_KVA
              group by x.'||v_targetTableFkColumn||'';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('YD','L',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;

     --
     -- Scnenario 5b - Power
     --

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- identify TRF_BANK_PEAK_HIST records for power transformers (NOT highest NP kva)
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_BANK_PEAK_HIST_P
               select A.'||v_targetTableFkColumn||', A.ID, null
               from
                    '||p_schema||'.'||v_targetTable||' A,
                    '||p_schema||'.'||v_sourceTable||' Q,
                    '||p_schema||'.TRANSFORMER R,
                    '||p_schema||'.OPERATING_VOLTAGE OV,
                    CALC_TRF_BANK_PEAK_HIST b
               where
                    b.TRF_PEAK_HIST_ID = a.'||v_targetTableFkColumn||' and
                    b.TRF_PEAK_HIST_ID = Q.ID and
                    Q.TRF_ID = R.ID and
                    R.OPERATING_VOLTAGE = OV.CD and
                    OV.VOLTAGE != (select CV.circuit_voltage from '||p_schema||'.CIRCUIT_VOLTAGE CV where CV.circuit_cd = substr(substr(R.CIRCUIT_ID, -4),1,2)) and
                    R.LOWSIDE_VOLTAGE = 23 and
                    b.TRF_BANKS_CNT=3 and
                    A.ID not in (select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_L)';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- calculate domestic and non-domestic load and sum them up
     v_recs_updated := UTIL_UPD_TRF_BANK_PEAK_HIST('YD','P',p_schema,p_generated);
     v_totalRowsMigrated := v_totalRowsMigrated + v_recs_updated;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=UPDATE_'||v_targetTable||'_'||to_char(batchDate,'mmddyyyy'));

END UPDATE_TRF_BANK_PEAK_HIST;
/
