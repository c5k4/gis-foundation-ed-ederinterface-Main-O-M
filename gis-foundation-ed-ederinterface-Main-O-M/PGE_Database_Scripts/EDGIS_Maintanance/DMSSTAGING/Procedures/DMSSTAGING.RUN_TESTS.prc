Prompt drop Procedure RUN_TESTS;
DROP PROCEDURE DMSSTAGING.RUN_TESTS
/

Prompt Procedure RUN_TESTS;
--
-- RUN_TESTS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE DMSSTAGING."RUN_TESTS" (i_debug NUMBER)
IS
  v_sql VARCHAR2(4000);
  v_sql_insert VARCHAR2(4000);
  v_cnt NUMBER(10);
  CURSOR cases IS
      SELECT *
      FROM   test_case
      ORDER BY 4  ;
BEGIN 
    delete from test_result;
    FOR case_rec IN cases
    LOOP
        IF case_rec.premise = 'UNIQUE' THEN
            v_sql := 'select count(*) from '||case_rec.table_name||' where '|| case_rec.column_name ||' in (select '|| case_rec.column_name ||' from '||case_rec.table_name||' group  by '|| case_rec.column_name ||' having count(*) > 1 )';
        ELSIF case_rec.premise in ('NOT_NULL') THEN            
            v_sql := 'select count(*) from '|| case_rec.table_name 
                       ||' where '|| case_rec.column_name ||' is null '
                       || case_rec.where_clause ;
        ELSIF case_rec.premise IN ('INTEGRITY','DATA_CHECK') THEN            
            v_sql := 'select count(*) from ('||case_rec.where_clause  ||')';
            --dbms_output.put_line(v_sql);
        END IF;
        EXECUTE IMMEDIATE v_sql INTO v_cnt;
        IF v_cnt > 0 THEN 
           dbms_output.put_line('TEST CASE FAILED('||rpad(case_rec.table_name,10)||':'|| rpad(case_rec.column_name,15) ||':'|| rpad(v_cnt,4) ||') - '|| case_rec.description  ) ;           
           IF i_debug = 1 THEN            
              dbms_output.put_line('/************ *************\n\n');        
              dbms_output.put_line(replace(v_sql,'select count(*) from','select * from')); 
              dbms_output.put_line('TEST CASE FAILED('||rpad(case_rec.table_name,10)||':'|| rpad(case_rec.column_name,15) ||':'|| rpad(v_cnt,4) ||') - '|| case_rec.description  ) ;
           ELSIF i_debug = 2 THEN         
              dbms_output.put_line(replace(v_sql,'select count(*) from','select FEATURE_CLASS,GUID,'''||case_rec.table_name||'.'||case_rec.column_name||' test type '||case_rec.premise||''' test_info from'));              
           ELSE              
              IF case_rec.table_name <> 'SITE' THEN    
                v_sql_insert := replace(v_sql,'select count(*) from','insert into test_result(FEATURE_CLASS,GUID,TEST_TYPE_THAT_FAILED,DESCRIPTION,TIME_FAILED) select FEATURE_CLASS,GUID,'''||case_rec.table_name||'.'||case_rec.column_name||' test type '||case_rec.premise||''' test_info ,'''||case_rec.description||''', sysdate from') ;
                -- dbms_output.put_line(v_sql_insert);
                IF i_debug >= 4 THEN 
                   dbms_output.put_line(v_sql_insert);
                END IF;                
                execute immediate v_sql_insert ;    
                commit;
              ELSE 
                v_sql_insert := replace(v_sql,'select count(*) from','insert into test_result(FEATURE_CLASS,GUID,TEST_TYPE_THAT_FAILED,DESCRIPTION,TIME_FAILED) select ''Device Group or Substation Boundary'',SIID,'''||case_rec.table_name||'.'||case_rec.column_name||' test type '||case_rec.premise||''' test_info ,'''||case_rec.description||''', sysdate from') ;
                IF i_debug >= 4 THEN 
                   dbms_output.put_line(v_sql_insert);
                END IF;                
                execute immediate v_sql_insert ;    
                commit;
              END IF;              
           END IF;
         END IF;       
    END LOOP;    
END run_tests;
/


Prompt Grants on PROCEDURE RUN_TESTS TO GIS_I to GIS_I;
GRANT EXECUTE ON DMSSTAGING.RUN_TESTS TO GIS_I
/
