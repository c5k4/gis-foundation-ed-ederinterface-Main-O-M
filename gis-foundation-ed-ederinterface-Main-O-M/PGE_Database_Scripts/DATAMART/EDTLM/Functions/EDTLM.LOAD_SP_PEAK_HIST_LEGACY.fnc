Prompt drop Function LOAD_SP_PEAK_HIST_LEGACY;
DROP FUNCTION EDTLM.LOAD_SP_PEAK_HIST_LEGACY
/

Prompt Function LOAD_SP_PEAK_HIST_LEGACY;
--
-- LOAD_SP_PEAK_HIST_LEGACY  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."LOAD_SP_PEAK_HIST_LEGACY" (
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
     v_s_pf CONSTANT NUMBER := 0.85;
     v_w_pf CONSTANT NUMBER := 0.95;
     v_season VARCHAR2(1);
     v_seasonal_pf NUMBER;

BEGIN
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     v_batchMonth := SUBSTR((to_char(batchDate,'MONTH')),1,3);
     if (upper(v_batchMonth) in ('NOV','DEC','JAN','FEB','MAR')) then
          v_season := 'W';
          v_seasonal_pf := v_w_pf;
     else
          v_season := 'S';
          v_seasonal_pf := v_s_pf;
     end if;

     -- create monthly load log record
     v_log_status := INSERT_MONTHLY_LOAD_LOG('LOAD_SP_PEAK_HIST_LGCY_'||to_char(batchDate,'mmddyyyy'));

     ----------------------------------------------------------------------------------------
     -- Scnenario 1 - Legacy, domestic/non-domestic with KVAR (for legacy, kw and pf are > 0)
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 1 - Legacy, domestic/non-domestic with KVAR (for legacy, kw and pf are > 0)');
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select
               a.service_point_id,
               (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               case when (NVL(TRIM(b.REV_KW),0) = 0 AND NVL(TRIM(b.PFACTOR),0) = 0) then 0
                    when NVL(TRIM(b.PFACTOR),0) = 0 then 0
                    else round(NVL(TRIM(b.REV_KW),0)/(NVL(TRIM(b.PFACTOR),0)/100),1) end,
               case when (NVL(TRIM(b.REV_KW),0) = 0 AND NVL(TRIM(b.PFACTOR),0) = 0) then 0
                    when NVL(TRIM(b.PFACTOR),0) = 0 then 0
                    else round(NVL(TRIM(b.REV_KW),0)/(NVL(TRIM(b.PFACTOR),0)/100),1) end,
               a.CUST_TYP,
               ''L'' as sm_flg,
               a.ID
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
               a.trf_id = c.id and
                NVL(TRIM(b.PFACTOR),0) > 0 and
                NVL(TRIM(b.REV_KW),0) > 0 and
               --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
               a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
               and
               b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)

               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     ----------------------------------------------------------------------------------------
     -- Scnenario 2 - Legacy, domestic withOUT KVAR (for legacy, pf = 0 or null)
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 2 - Legacy, domestic withOUT KVAR (for legacy, pf = 0 or null)');
     ----------------------------------------------------------------------------------------

     -- populate temporary tables used for calculation
     v_stmt := 'truncate table '||p_schema||'.calc_trf_dom_count';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt :=
     'insert into '||p_schema||'.calc_trf_dom_count
          select
               a.id as device_id,
               count(distinct b.ID) as dom_count
          from '
               ||p_schema||'.TRANSFORMER a,'
               ||p_schema||'.METER b
          where
               a.ID = b.TRF_ID and
               b.CUST_TYP = ''DOM''
               AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (b.REC_STATUS <> ''D'' OR b.REC_STATUS is null)
               and b.SERVICE_POINT_ID not in (select s.SERVICE_POINT_ID from '||p_schema||'.STG_SM_SP_LOAD s)
          group by a.id';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''calc_trf_dom_count'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- populate temporary tables used for calculation
     v_stmt := 'truncate table '||p_schema||'.calc_trf_ab_value';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     v_stmt :=
     'insert into '||p_schema||'.calc_trf_ab_value
          select
               T.ID as device_id,
               KCCS.A_value summer_a_value,
               KCCS.B_value summer_b_value,
               KCCW.A_value winter_a_value,
               KCCW.B_value winter_b_value
          from '
               ||p_schema||'.TRANSFORMER t,'
               ||p_schema||'.KVA_conversion_coefficients KCCS,'
               ||p_schema||'.KVA_conversion_coefficients KCCW,'
               ||p_schema||'.CALC_TRF_DOM_COUNT TDC
          where
               T.ID = TDC.DEVICE_ID AND
               KCCS.Climate_zone = T.CLIMATE_ZONE_CD AND
               KCCS.Season = ''S'' AND
               KCCS.customer_count_gt =
                  DECODE(TDC.DOM_COUNT, 1, 0, 2, 1, 3, 2, 4, 2, 5, 2,
                     DECODE(TRUNC(TDC.DOM_COUNT/13), 0, 5,
                        DECODE(TRUNC(TDC.DOM_COUNT/21), 0, 12,
                                             20))) AND
               KCCW.Climate_zone = T.CLIMATE_ZONE_CD AND
               KCCW.Season = ''W'' AND
               KCCW.customer_count_gt =
                  DECODE(TDC.DOM_COUNT, 1, 0, 2, 1, 3, 2, 4, 2, 5, 2,
                     DECODE(TRUNC(TDC.DOM_COUNT/13), 0, 5,
                        DECODE(TRUNC(TDC.DOM_COUNT/21), 0, 12, 20)))';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''calc_trf_ab_value'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     if (v_season = 'S') THEN
         v_stmt := '
              insert into '||p_schema||'.SP_PEAK_HIST (
                   SERVICE_POINT_ID,
                   TRF_PEAK_HIST_ID,
                   SP_PEAK_TIME,
                   sp_peak_kva,
                   sp_kva_trf_peak,
                   cust_typ,
                   sm_flg,
                   meter_id)
              select
                   a.service_point_id,
                   (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
                   to_date('''||v_batchDateStr||''',''dd-mon-yy''),
                   round(((NVL(TRIM(b.REV_KWHR),0) * d.summer_a_value) + (d.summer_b_value/TDC.DOM_COUNT))/'||v_s_pf||',1),
                   round(((NVL(TRIM(b.REV_KWHR),0) * d.summer_a_value) + (d.summer_b_value/TDC.DOM_COUNT))/'||v_s_pf||',1),
                   a.CUST_TYP,
                   ''L'' as sm_flg,
                   a.ID
              FROM '
                   ||p_schema||'.METER a, '
                   ||p_schema||'.STG_CCB_METER_LOAD b, '
                   ||p_schema||'.calc_trf_ab_value d, '
                   ||p_schema||'.TRANSFORMER c,'
                   ||p_schema||'.CALC_TRF_DOM_COUNT TDC
              WHERE
                   a.trf_id = c.id and
				   TDC.DEVICE_ID = c.ID and
                   a.cust_typ = ''DOM'' and
                   a.trf_id = d.device_id and not
                   (NVL(TRIM(b.PFACTOR),0) > 0 and
                    NVL(TRIM(b.REV_KW),0) > 0) and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
               AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and  b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';
     ELSE
         v_stmt := '
              insert into '||p_schema||'.SP_PEAK_HIST (
                   SERVICE_POINT_ID,
                   TRF_PEAK_HIST_ID,
                   SP_PEAK_TIME,
                   sp_peak_kva,
                   sp_kva_trf_peak,
                   cust_typ,
                   sm_flg,
                   meter_id)
              select
                   a.service_point_id,
                   (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
                   to_date('''||v_batchDateStr||''',''dd-mon-yy''),
                   round(((NVL(TRIM(b.REV_KWHR),0) * d.winter_a_value) + (d.winter_b_value/TDC.DOM_COUNT))/'||v_w_pf||',1),
                   round(((NVL(TRIM(b.REV_KWHR),0) * d.winter_a_value) + (d.winter_b_value/TDC.DOM_COUNT))/'||v_w_pf||',1),
                   a.CUST_TYP,
                   ''L'' as sm_flg,
                   a.ID
              FROM '
                   ||p_schema||'.METER a, '
                   ||p_schema||'.STG_CCB_METER_LOAD b, '
                   ||p_schema||'.calc_trf_ab_value d, '
                   ||p_schema||'.TRANSFORMER c,'
                   ||p_schema||'.CALC_TRF_DOM_COUNT TDC
              WHERE
                   a.trf_id = c.id and
				   TDC.DEVICE_ID = c.ID and
                   a.cust_typ = ''DOM'' and
                   a.trf_id = d.device_id and
                   -- NVL(TRIM(b.REV_KWHR),0) > 0 and
                   not (NVL(TRIM(b.PFACTOR),0) > 0 and NVL(TRIM(b.REV_KW),0) > 0) and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
                   AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and
                   b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';
     END IF;

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     ----------------------------------------------------------------------------------------
     -- Scnenario 3 - If the SP has a non-zero kW (demand) value and customer type is Commercial
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 3 - If the SP has a non-zero kW (demand) value and customer type is Commercial');
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select
               a.service_point_id,
               (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               round(NVL(TRIM(b.REV_KW),0)/'||v_seasonal_pf||',1),
               round(NVL(TRIM(b.REV_KW),0)/'||v_seasonal_pf||',1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               a.ID
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
                   a.trf_id = c.id and
                   a.cust_typ = ''COM'' and
                   NVL(TRIM(b.REV_KW),0) > 0 and
                   (NVL(TRIM(b.PFACTOR),0) is null or NVL(TRIM(b.PFACTOR),0) = 0) and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
                  AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and
                   b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     ----------------------------------------------------------------------------------------
     -- Scnenario 4 - If the SP has a non-zero kW (demand) value and customer type is Agriculture, Industrial or Other
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 4 - If the SP has a non-zero kW (demand) value and customer type is Agriculture, Industrial or Other');
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select
               a.service_point_id,
               (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               round(NVL(TRIM(b.REV_KW),0)/.85,1),
               round(NVL(TRIM(b.REV_KW),0)/.85,1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               a.ID
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
                   a.trf_id = c.id and
                   a.cust_typ in (''AGR'',''IND'',''OTH'') and
                   NVL(TRIM(b.REV_KW),0) > 0 and
                   (NVL(TRIM(b.PFACTOR),0) is null or NVL(TRIM(b.PFACTOR),0) = 0) and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
                   AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and
                   b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     ----------------------------------------------------------------------------------------
     -- Scnenario 5 - If the SP has a zero or null kW (demand) value and customer type is Commercial
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 5 - If the SP has a zero or null kW (demand) value and customer type is Commercial');
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select
               a.service_point_id,
               (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               round(NVL(TRIM(b.REV_KWHR),0)*(1.0 /(24.0 * '||v_seasonal_pf||' * .48)),1),
               round(NVL(TRIM(b.REV_KWHR),0)*(1.0 /(24.0 * '||v_seasonal_pf||' * .48)),1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               a.ID
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
                   a.trf_id = c.id and
                   a.cust_typ in (''COM'') and
                   (NVL(TRIM(b.REV_KW),0) is null or NVL(TRIM(b.REV_KW),0) = 0) and
                   -- NVL(TRIM(b.REV_KWHR),0) > 0 and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
                   AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and
                   b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     ----------------------------------------------------------------------------------------
     -- Scnenario 6 - If the SP has a zero or null kW (demand) value and customer type is Agricultural, Industrial or Other
     DBMS_OUTPUT.PUT_LINE('DEBUG - Scnenario 6 - If the SP has a zero or null kW (demand) value and customer type is Agricultural, Industrial or Other');
     ----------------------------------------------------------------------------------------
     v_stmt := '
          insert into '||p_schema||'.SP_PEAK_HIST (
               SERVICE_POINT_ID,
               TRF_PEAK_HIST_ID,
               SP_PEAK_TIME,
               sp_peak_kva,
               sp_kva_trf_peak,
               cust_typ,
               sm_flg,
               meter_id)
          select
               a.service_point_id,
               (select id from '||p_schema||'.trf_peak_hist x where x.trf_id=a.trf_id and x.batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy'')),
               to_date('''||v_batchDateStr||''',''dd-mon-yy''),
               round(NVL(TRIM(b.REV_KWHR),0)*(1.0 /(24.0 * .85 * .48)),1),
               round(NVL(TRIM(b.REV_KWHR),0)*(1.0 /(24.0 * .85 * .48)),1),
               a.CUST_TYP,
               ''L'' as sm_flg,
               a.ID
          FROM '
               ||p_schema||'.METER a, '
               ||p_schema||'.STG_CCB_METER_LOAD b, '
               ||p_schema||'.TRANSFORMER c
          WHERE
                   a.trf_id = c.id and
                   a.cust_typ in (''AGR'',''IND'',''OTH'') and
                   (NVL(TRIM(b.REV_KW),0) is null or NVL(TRIM(b.REV_KW),0) = 0) and
                   -- NVL(TRIM(b.REV_KWHR),0) > 0 and
                   --(b.sm_sp_status is null or b.sm_sp_status in (''10'',''20'')) and
                   a.SERVICE_POINT_ID = b.SERVICE_POINT_ID
                   AND (a.REC_STATUS <> ''D'' OR a.REC_STATUS is null)
                   AND (c.REC_STATUS <> ''D'' OR c.REC_STATUS is null)
                   and
                   b.SERVICE_POINT_ID NOT IN (SELECT z.SERVICE_POINT_ID FROM '||p_schema||'.STG_SM_SP_LOAD z)
                   ';

     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;
     v_rowsProcessed := SQL%ROWCOUNT;
     DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
     v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
     commit;

     DBMS_OUTPUT.PUT_LINE('Total rows migrated - '||v_totalRowsMigrated);

     -- clean up.
     -- convert negative calculated SP_PEAK_KVA to .1
     v_stmt := '
          update '||p_schema||'.SP_PEAK_HIST set SP_PEAK_KVA = .1
          where
               SP_PEAK_KVA < 0 and
               TRF_PEAK_HIST_ID in
               (select ID from '||p_schema||'.trf_peak_hist where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- convert negative calculated SP_KVA_TRF_PEAK to .1
     v_stmt := '
          update '||p_schema||'.SP_PEAK_HIST set SP_KVA_TRF_PEAK = .1
          where
               SP_KVA_TRF_PEAK < 0 and
               TRF_PEAK_HIST_ID in
               (select ID from '||p_schema||'.trf_peak_hist where batch_date=to_date('''||v_batchDateStr||''',''dd-mon-yy''))
               ';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     -- update migration log record
     v_log_status := LOG_MONTHLY_LOAD_SUCCESS('LOAD_SP_PEAK_HIST_LGCY_'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||p_schema||''',
               tabname => ''SP_PEAK_HIST'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;

     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MONTHLY_LOAD_ERROR('LOAD_SP_PEAK_HIST_LGCY_'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MONTHLY_LOAD_LOG for tablename=LOAD_SP_PEAK_HIST_LGCY_'||to_char(batchDate,'mmddyyyy'));

END LOAD_SP_PEAK_HIST_LEGACY;
/
