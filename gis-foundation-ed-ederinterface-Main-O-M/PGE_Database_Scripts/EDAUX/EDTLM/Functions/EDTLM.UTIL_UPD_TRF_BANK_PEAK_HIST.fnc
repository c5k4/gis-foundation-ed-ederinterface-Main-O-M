Prompt drop Function UTIL_UPD_TRF_BANK_PEAK_HIST;
DROP FUNCTION EDTLM.UTIL_UPD_TRF_BANK_PEAK_HIST
/

Prompt Function UTIL_UPD_TRF_BANK_PEAK_HIST;
--
-- UTIL_UPD_TRF_BANK_PEAK_HIST  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UTIL_UPD_TRF_BANK_PEAK_HIST" (
    conn_type             VARCHAR2,
    lighterOrPower        VARCHAR2,
    p_schema VARCHAR2,
    p_generated VARCHAR2)
RETURN NUMBER
IS
     v_stmt VARCHAR2(2000);
     v_totalRowsMigrated NUMBER := 0;
     v_sourceTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'TRF_PEAK_GEN_HIST';
          v_targetTable := 'TRF_BANK_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;

     -- calculate domestic and non-domestic load and sum them up
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' X set TRF_PEAK_KVA =
               -- domestic assumed to be 1 phase
               (select round(Y.kva,1) from
                    (
                    select a.TRF_PEAK_KVA_DOM*(b.DISTRIBUTION_PCT/100) kva, a.id from '||p_schema||'.'||v_sourceTable||' a, '||p_schema||'.TRF_LOAD_DIST b
                    where
                         b.CONNECTION_TYP = '''||conn_type||''' and
                         b.TRF_TYP = '''||lighterOrPower||''' and
                         b.PHASE = ''1''
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
                         b.CONNECTION_TYP = '''||conn_type||''' and
                         b.TRF_TYP = '''||lighterOrPower||''' and
                         b.PHASE = ''3''
              ) Y
              where
                   y.id = x.'||v_targetTableFkColumn||'
              )
        where X.ID in
                (
                select TRF_BANK_PEAK_HIST_ID from CALC_TRF_BANK_PEAK_HIST_'||lighterOrPower||'
                    where TRF_PEAK_HIST_ID = x.'||v_targetTableFkColumn||'
                )
                ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;

  RETURN v_totalRowsMigrated;
END UTIL_UPD_TRF_BANK_PEAK_HIST;
/
