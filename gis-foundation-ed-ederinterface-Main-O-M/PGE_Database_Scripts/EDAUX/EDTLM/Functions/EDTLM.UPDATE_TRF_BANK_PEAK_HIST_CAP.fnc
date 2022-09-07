Prompt drop Function UPDATE_TRF_BANK_PEAK_HIST_CAP;
DROP FUNCTION EDTLM.UPDATE_TRF_BANK_PEAK_HIST_CAP
/

Prompt Function UPDATE_TRF_BANK_PEAK_HIST_CAP;
--
-- UPDATE_TRF_BANK_PEAK_HIST_CAP  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."UPDATE_TRF_BANK_PEAK_HIST_CAP" (
     p_schema VARCHAR2,
     batchDate Date,
     p_generated VARCHAR2)
RETURN VARCHAR2
IS
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(4000);
     v_stmt1 VARCHAR2(4000);
     v_log_status VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_season VARCHAR2(1);
     v_batchDateStr VARCHAR2(10) := to_char(batchDate, 'dd-mon-yy');
     v_sourceTable VARCHAR2(50) := 'SP_PEAK_HIST';
     v_sourceTable_2 VARCHAR2(50) := 'TRF_PEAK_HIST';
     v_sourceTable_3 VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';

     v_targetTable VARCHAR2(50) := 'TRF_BANK_PEAK_HIST';
     v_targetTableFkColumn VARCHAR2(50) := 'TRF_PEAK_HIST_ID';
BEGIN
     if p_generated = 'GEN' then
          v_sourceTable := 'SP_PEAK_GEN_HIST';
          v_sourceTable_2 := 'TRF_PEAK_GEN_HIST';
          v_sourceTable_3 := 'TRF_BANK_PEAK_GEN_HIST';

          v_targetTable := 'TRF_BANK_PEAK_GEN_HIST';
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
     v_log_status := INSERT_MONTHLY_LOAD_LOG('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'));

     -- truncate work tables
     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_HIST_CAP';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          truncate table '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_GROUP_TYPE';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          truncate table '||p_schema||'.CALC_TRF_BANK_PEAK_LIST';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- insert into work tables
     -- Jim Moore IBM 20140911 added the LOAD_UNDERTERMINED is null to the query below
     -- insert into work tables
     v_stmt := '
          insert into '||p_schema||'.CALC_TRF_HIST_CAP (TRF_PEAK_HIST_ID)
               select a.ID
               from
                    ' ||p_schema||'.' || v_sourceTable_2 || ' a
               where
                    a.BATCH_DATE = to_date('''||v_batchDateStr||''',''dd-mon-yy'')'
                    ||' and a.LOAD_UNDETERMINED is null';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt := '
          insert into '||p_schema||'.CALC_CUST_TYP_TOTAL_KVA
               select
                    a.' || v_targetTableFkColumn || ',
                    ''DOMESTIC'' cust_typ,
                    sum(a.SP_KVA_TRF_PEAK) total_kva
               from
                    '||p_schema||'.'||v_sourceTable||' a,
                    '||p_schema||'.' || v_sourceTable_2 || ' b
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
                    '||p_schema||'.TRF_PEAK_HIST b
               where
                    a.'||v_targetTableFkColumn||' = b.ID and
                    b.BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'') and
                    a.CUST_TYP <> ''DOM''
               group by a.' || v_targetTableFkColumn;
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

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
     execute immediate v_stmt;

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
     execute immediate v_stmt;

     --if residential has more load overall than non-residential, set CUST_TYP_WITH_GREATER_LOAD to 'DOMESTIC'
     v_stmt := '
          update '||p_schema||'.CALC_TRF_HIST_CAP a set
               a.CUST_TYP_WITH_GREATER_LOAD = ''DOMESTIC''
          where
               a.DOM_TOTAL_KVA > a.NON_DOM_TOTAL_KVA';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

/*  no longer needed - Jim 20140911
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
     execute immediate v_stmt;
*/

     DBMS_OUTPUT.PUT_LINE('DEBUG - starting new code');
     -- Jim Moore IBM 20140905 add records to a Calc Table for faster processing
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


     -- Jim Moore IBM 20140905 add records to a Calc Table for faster processing
     v_stmt := '
           INSERT INTO CALC_TRF_BANK_PEAK_LIST (ID)
                       select bp.id
                           from  '||p_schema||'.' || v_sourceTable_3 || ' bp,
                                 '||p_schema||'.' || v_sourceTable_2 || ' p
                            where p.id = bp.' || v_targetTableFkColumn || '
                              and BATCH_DATE =  to_date('''||v_batchDateStr||''',''dd-mon-yy'')';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     -- finally, update TRF_PEAK_HIST.TRF_CAP
-- Jim Moore IBM 20140905 added this to update TRF_BANK_PEAK_HIST rather than TRF_PEAK_HIST
--Changed 20140911 to handle split records for Duplex transformer.  See three updates below
-- first section - tally every thing that is not trf type 21 and 32
-- second section - tally lighter for those duplex - trf type is either 21 or 32
-- third section - tally POWER for those duplex - trf type is either 21 or 32
   DBMS_OUTPUT.PUT_LINE('DEBUG - Preparing final update statement');
     v_stmt := '
          update '||p_schema||'.'||v_targetTable||' x set
          TRF_CAP =
          NVL
          (
               (
               select y.trf_cap
               from (
                   select
                         f.ID,
                         f.NP_KVA*decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'',
                                                   d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from
                      '||p_schema||'.CALC_TRF_HIST_CAP a,
                      '||p_schema||'.' || v_sourceTable_2 || ' b,
                      '||p_schema||'.TRANSFORMER c,
                      '||p_schema||'.TRF_NP_CAP_MULT d,
                      '||p_schema||'.TRF_GROUP_TYPE e,
                      '||p_schema||'.' || v_sourceTable_3 || ' f,
                      '||p_schema||'.TRANSFORMER_BANK g
                    where
                         a.TRF_PEAK_HIST_ID = b.id and
                         b.TRF_ID = c.ID and
                         f.' || v_targetTableFkColumn || ' = b.id and
                         f.TRF_BANK_ID = g.id and
                         g.TRF_TYP = e.TRF_TYPE and
                         d.SEASON = '''||v_season||''' and
                         f.NP_KVA >= d.KVA_LOW and
                         f.NP_KVA <= d.KVA_HIGH and
                         d.TRF_GRP_TYPE = e.TRF_GRP_TYPE and
                         --d.INSTALLATION_TYP = c.INSTALLATION_TYP and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG and
                         g.trf_typ not in (21,32) and
                         --and not in UG or SS
                         g.trf_typ not in (select trf_type from '||p_schema||'.TRF_GROUP_TYPE where trf_grp_type in (''SS-S'',''SS-T'',''UG-S'',''UG-T'')) and
                         d.vault_ind = 0
                 union
                    select
                         f.ID,
                         f.NP_KVA*decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'',
                                                   d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from
                      '||p_schema||'.CALC_TRF_HIST_CAP a,
                      '||p_schema||'.' || v_sourceTable_2 || ' b,
                      '||p_schema||'.TRANSFORMER c,
                      '||p_schema||'.TRF_NP_CAP_MULT d,
                      '||p_schema||'.TRF_GROUP_TYPE e,
                      '||p_schema||'.' || v_sourceTable_3 || ' f,
                      '||p_schema||'.TRANSFORMER_BANK g
                    where
                         a.TRF_PEAK_HIST_ID = b.id and
                         b.TRF_ID = c.ID and
                         f.' || v_targetTableFkColumn || ' = b.id and
                         f.TRF_BANK_ID = g.id and
                         g.TRF_TYP = e.TRF_TYPE and
                         d.SEASON = '''||v_season||''' and
                         f.NP_KVA >= d.KVA_LOW and
                         f.NP_KVA <= d.KVA_HIGH and
                         d.TRF_GRP_TYPE = e.TRF_GRP_TYPE and
                         --d.INSTALLATION_TYP = c.INSTALLATION_TYP and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG and
                         --g.trf_typ not in (21,32) and
                         -- and in UG and SS
                         g.trf_typ in (select trf_type from '||p_schema||'.TRF_GROUP_TYPE where trf_grp_type in (''SS-S'',''SS-T'',''UG-S'',''UG-T'')) and
                         d.vault_ind = DECODE(c.VAULT, null, 0, 1)
                union
                       ';

     v_stmt1 := '
                   select
                         b.ID,
                         TD.LIGHTER*decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'',
                                                   d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from
                      '||p_schema||'.CALC_TRF_HIST_CAP a,
                      '||p_schema||'.' || v_sourceTable_3 || ' b,
                      '||p_schema||'.TRANSFORMER c,
                      '||p_schema||'.transformer_bank tb,
                      '||p_schema||'.CALC_TRF_GROUP_TYPE TG,
                      '||p_schema||'.TRF_NP_CAP_MULT d ,
                      '||p_schema||'.trf_duplex_lookup td
                    where
                         c.id = tb.trf_id and
                         a.TRF_PEAK_HIST_ID = b.' || v_targetTableFkColumn || ' and
                         b.trf_bank_id = tg.trf_bank_id  and
                         c.id = tg.trf_id and
                         d.TRF_GRP_TYPE = tg.TRF_GRP_TYPE and
                         d.SEASON = '''||v_season||''' and
                         tb.NP_KVA = d.KVA_LOW and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG and
                         tg.trf_typ in (21,32) and
                         td.lighter = b.NP_KVA
                union
                   select
                         b.ID,
                         TD.POWER *decode(a.CUST_TYP_WITH_GREATER_LOAD,''DOMESTIC'',
                                                   d.CAP_MULTIPLIER_A, d.CAP_MULTIPLIER_B) trf_cap
                    from
                      '||p_schema||'.CALC_TRF_HIST_CAP a,
                      '||p_schema||'.' || v_sourceTable_3 || ' b,
                      '||p_schema||'.TRANSFORMER c,
                      '||p_schema||'.transformer_bank tb,
                      '||p_schema||'.CALC_TRF_GROUP_TYPE TG,
                      '||p_schema||'.TRF_NP_CAP_MULT d ,
                      '||p_schema||'.trf_duplex_lookup td
                    where
                         c.id = tb.trf_id and
                         a.TRF_PEAK_HIST_ID = b.' || v_targetTableFkColumn || ' and
                         b.trf_bank_id = tg.trf_bank_id  and
                         c.id = tg.trf_id and
                         d.TRF_GRP_TYPE = tg.TRF_GRP_TYPE and
                         d.SEASON = '''||v_season||''' and
                         tb.NP_KVA = d.KVA_LOW and
                         d.COAST_INTERIOR_FLG = c.COAST_INTERIOR_FLG and
                         tg.trf_typ in (21,32) and
                         td.power = b.NP_KVA
                    ) Y
               where
                    x.id = y.id
               ),
               0
          )
        where x.id in ( SELECT * FROM '||p_schema||'.CALC_TRF_BANK_PEAK_LIST)';
     DBMS_OUTPUT.PUT_LINE('DEBUG - Final update statement prepared');
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt || v_stmt1);
     execute immediate v_stmt || v_stmt1;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'),SQL%ROWCOUNT);

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for tablename=UPDATE_'||v_targetTable||'_CAP_'||to_char(batchDate, 'mmddyyyy'));

END UPDATE_TRF_BANK_PEAK_HIST_CAP;
/
