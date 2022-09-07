Prompt drop Function MIGRATE_CIRCUIT_LOAD_HIST_NEW;
DROP FUNCTION EDTLM.MIGRATE_CIRCUIT_LOAD_HIST_NEW
/

Prompt Function MIGRATE_CIRCUIT_LOAD_HIST_NEW;
--
-- MIGRATE_CIRCUIT_LOAD_HIST_NEW  (Function) 
--
CREATE OR REPLACE FUNCTION EDTLM."MIGRATE_CIRCUIT_LOAD_HIST_NEW" (
     wave         VARCHAR2,
     sourceSchema VARCHAR2,
     targetSchema VARCHAR2,
     batchDate DATE)
RETURN VARCHAR2
IS
----------------------------------------------------------------------------
-- Initial VERSION 1.0
----------------------------------------------------------------------------
     v_returVal VARCHAR2(10);
     v_stmt VARCHAR2(2000);
     v_stmt1 VARCHAR2(2000);
     v_log_status VARCHAR2(10);
     v_batchDateStr VARCHAR2(10);
     v_batchMonth VARCHAR2(10);
     v_rowsProcessed NUMBER := 0;
     v_totalRowsMigrated NUMBER := 0;
     v_cnt NUMBER :=0;
     v_date DATE := batchDate;

     CURSOR  CKT_LIST_SUMMER IS
            SELECT (SELECT ID FROM  EDSETTCC.SM_CIRCUIT_LOAD x where x.REF_DEVICE_ID= SM.ID) SM_CIRCUIT_LOAD_ID , CSL.TOTAL_KW ,to_timestamp(trunc(v_date) || ' ' || substr('0000' || CSL.PEAK_TIME,-4),'DD-Mon-YY HH24MI' ) PEAK_TIME
              FROM
              CEDSA_CIRCUIT_LOAD_SUMMARY CSL ,
              CEDSA_CIRCUIT  C,
              EDSETTCC.SM_CIRCUIT_BREAKER SM
              WHERE CSL.CIRCUIT_ID=C.CIRCUIT_ID and C.DEVICE_ID=SM.DEVICE_ID and SM.CURRENT_FUTURE='C'  and ( mod(csl.PEAK_TIME,100) <= 59 and mod(PEAK_TIME,100) >= 0  and  peak_time <= 2359) and CSL.SEASON='S';

    CURSOR  CKT_LIST_WINTER IS
            SELECT (SELECT ID FROM  edsettcc.SM_CIRCUIT_LOAD x where x.REF_DEVICE_ID= SM.ID) SM_CIRCUIT_LOAD_ID , CSL.TOTAL_KW, to_timestamp(trunc(v_date) || ' ' || substr('0000' || CSL.PEAK_TIME,-4),'DD-Mon-YY HH24MI' ) PEAK_TIME
              FROM
              CEDSA_CIRCUIT_LOAD_SUMMARY CSL ,
              CEDSA_CIRCUIT  C,
              EDSETTCC.SM_CIRCUIT_BREAKER SM
              WHERE CSL.CIRCUIT_ID=C.CIRCUIT_ID and C.DEVICE_ID=SM.DEVICE_ID and SM.CURRENT_FUTURE='C' and ( mod(csl.PEAK_TIME,100) <= 59 and mod(PEAK_TIME,100) >= 0 and  peak_time <= 2359 ) and CSL.SEASON='W';


    TYPE CKT_REC IS REF CURSOR;
    CKT_REC_SUMM  CKT_REC;
    CKT_REC_WINT  CKT_REC;



BEGIN
     v_batchDateStr := to_char(batchDate, 'dd-mon-yy');
     v_batchMonth := UPPER(SUBSTR((to_char(batchDate,'MONTH')),1,3));

     -- create migration log record
     v_log_status := INSERT_MIGRATION_LOG(wave, 'MIGRATE_CIRCUIT_LOAD_HIST'||to_char(batchDate,'mmddyyyy'));

     ----------------------------------------------------------------------------------------
     -- Populate data in SM_CIRCUIT_LOAD_HIST for both Winter and Summber months
     ----------------------------------------------------------------------------------------

      if (to_char(batchDate, 'mm') in ('11','12','01','02','03')) then   -- Migrate Winter KW values

        for CKT_REC_SUMM  in  CKT_LIST_WINTER
        loop

            -- DBMS_OUTPUT.PUT_LINE('DEBUG - '||CKT_REC_SUMM.SM_CIRCUIT_LOAD_ID||CKT_REC_SUMM.TOTAL_KW) ;
            insert into edsettCC.SM_CIRCUIT_LOAD_HIST(SM_CIRCUIT_LOAD_ID,PEAK_DTM,TOTAL_KW_LOAD)
            values ( CKT_REC_SUMM.SM_CIRCUIT_LOAD_ID,CKT_REC_SUMM.PEAK_TIME,CKT_REC_SUMM.TOTAL_KW );

            v_rowsProcessed := SQL%ROWCOUNT;
            DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
            v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;

        end loop;

    else -- Migrate Summer KW values for months - 04,05,06,07,08,10

       /* Process data Summer  months from Apr to Oct */
        for CKT_REC_SUMM  in  CKT_LIST_SUMMER
        loop
            -- DBMS_OUTPUT.PUT_LINE('DEBUG - '||CKT_REC_SUMM.SM_CIRCUIT_LOAD_ID||CKT_REC_SUMM.TOTAL_KW) ;
            insert into edsettCC.SM_CIRCUIT_LOAD_HIST(SM_CIRCUIT_LOAD_ID,PEAK_DTM,TOTAL_KW_LOAD)
            values ( CKT_REC_SUMM.SM_CIRCUIT_LOAD_ID,CKT_REC_SUMM.PEAK_TIME,CKT_REC_SUMM.TOTAL_KW );

            v_rowsProcessed := SQL%ROWCOUNT;
            DBMS_OUTPUT.PUT_LINE('Records processed - '||v_rowsProcessed);
            v_totalRowsMigrated := v_totalRowsMigrated + v_rowsProcessed;
        end loop;

    end if;

     -- update migration log record
     v_log_status := LOG_MIGRATION_SUCCESS(wave, 'MIGRATE_CIRCUIT_LOAD_HIST'||to_char(batchDate,'mmddyyyy'),v_totalRowsMigrated);

     -- gather stats
     v_stmt := '
          call DBMS_STATS.GATHER_TABLE_STATS (
  	  	       ownname => '''||targetSchema||''',
               tabname => ''SM_CIRCUIT_LOAD_HIST'',
               estimate_percent => 10
          )';
     DBMS_OUTPUT.PUT_LINE('DEBUG - '||v_stmt);
     execute immediate v_stmt;


     RETURN ('TRUE');
EXCEPTION
     WHEN OTHERS THEN
          v_log_status := LOG_MIGRATION_ERROR(wave, 'MIGRATE_CIRCUIT_LOAD_HIST'||to_char(batchDate,'mmddyyyy'), sqlerrm);
          raise_application_error (-20001, 'migration failed.  Please check entry in MIGRATION_LOG for wave='||wave||' and tablename=MIGRATE_CIRCUIT_LOAD_HIST'||to_char(batchDate,'mmddyyyy'));

END MIGRATE_CIRCUIT_LOAD_HIST_NEW;
/
