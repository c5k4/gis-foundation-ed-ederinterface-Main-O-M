Prompt drop Function LOAD_TRF_BANK_PEAK_HIST;
DROP FUNCTION EDTLM.LOAD_TRF_BANK_PEAK_HIST
/

Prompt Function LOAD_TRF_BANK_PEAK_HIST;
--
-- LOAD_TRF_BANK_PEAK_HIST  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_TRF_BANK_PEAK_HIST" (
     p_schema VARCHAR2,
     batchDate DATE,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(4000);
     v_log_status VARCHAR2(10);
     v_totalRowsMigrated NUMBER := 0;
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');

     v_sourceTable VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_targetTable VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
-- Prepared by Jim Moore 8/15/2014 IBM for TLM2
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'TRF_PEAK_GEN_HIST';
          v_targetTable := 'TRF_BANK_PEAK_GEN_HIST';
          v_targetTableFkColumn := 'TRF_PEAK_GEN_HIST_ID';
     end if;
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');

     -- create migration log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('LOAD_' || v_targetTable || '_'|| to_char(batchDate,'mmddyyyy'));

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_GROUP_TYPE';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- Jim Moore IBM 20140905 add records to a Global Temp Table for faster processing
     v_stmt := '
           insert into CALC_TRF_GROUP_TYPE (trf_id, trf_bank_id,
                               TRF_TYP, TRF_GRP_TYPE)
                            ( Select unique
                               tb.trf_id, tb.id trf_bank_id,
                               tb.TRF_TYP, tg.TRF_GRP_TYPE
                          FROM
                               '||p_schema||'.TRANSFORMER_BANK tb,
                               '||p_schema||'.TRF_TYPE tt,
                               '||p_schema||'.TRF_GROUP_TYPE tg
                         WHERE
                              tb.trf_typ      = tt.type_trf_cd AND
                              tt.type_trf_cd = tg.trf_type
                          )' ;

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
         insert into '||p_schema||'.'||v_targetTable||'(
        	'|| v_targetTableFkColumn ||',
        	TRF_CAP,
        	TRF_BANK_ID,
        	NP_KVA)
          select  tph.id '|| v_targetTableFkColumn ||'
                , 0 trf_cap
                , tb.id TRF_BANK_ID
                , nvl(tb.np_kva,0) np_kva
            from
                 '||p_schema||'.transformer_bank tb,
                 '||p_schema||'.'||v_sourceTable||' tph,
                 '||p_schema||'.CALC_TRF_GROUP_TYPE tgt
           where tph.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                 tb.trf_id  = tph.TRF_ID
             and tgt.trf_BANK_ID = TB.ID
             AND TGT.trf_typ NOT in (21,32)
             AND tph.load_undetermined is  null
      UNION
          select  tph.id '|| v_targetTableFkColumn ||'
                , 0 trf_cap
                , tb.id TRF_BANK_ID
                , TD.LIGHTER NP_KVA
            from
                 '||p_schema||'.transformer_bank tb,
                 '||p_schema||'.'||v_sourceTable||' tph,
                 '||p_schema||'.CALC_TRF_GROUP_TYPE tgt,
                 '||p_schema||'.trf_duplex_lookup TD
           where tph.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                 tb.trf_id  = tph.TRF_ID
             and tgt.trf_BANK_ID = TB.ID
             AND TGT.trf_typ in (21,32)
             AND TD.TOTAL = nvl(tb.np_kva,0)
             AND tph.load_undetermined is  null
      UNION
          select  tph.id '|| v_targetTableFkColumn ||'
                , 0 trf_cap
                , tb.id TRF_BANK_ID
                , TD.POWER NP_KVA
            from
                 '||p_schema||'.transformer_bank tb,
                 '||p_schema||'.'||v_sourceTable||' tph,
                 '||p_schema||'.CALC_TRF_GROUP_TYPE tgt,
                 '||p_schema||'.trf_duplex_lookup TD
           where tph.batch_date = to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                 tb.trf_id  = tph.TRF_ID
             and tgt.trf_BANK_ID = TB.ID
             AND TGT.trf_typ in (21,32)
             AND TD.TOTAL = nvl(tb.np_kva,0)
             AND tph.load_undetermined is  null
             ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_totalRowsMigrated := v_totalRowsMigrated + SQL%ROWCOUNT;
     commit;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('LOAD_' || v_targetTable || '_'|| to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);


     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''TRF_BANK_PEAK_HIST'',
               estimate_percent => 50
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          DBMS_OUTPUT.PUT_LINE('ORA ERR - '||sqlerrm);
          v_log_status := LOG_MONTHLY_LOAD_ERROR('LOAD_' || v_targetTable || '_'|| to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=LOAD_' || v_targetTable || '_'|| to_char(batchDate,'mmddyyyy'));

END LOAD_TRF_BANK_PEAK_HIST;
/
