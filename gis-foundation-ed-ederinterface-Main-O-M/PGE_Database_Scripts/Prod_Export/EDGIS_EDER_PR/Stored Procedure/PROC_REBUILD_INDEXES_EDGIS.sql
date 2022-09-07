--------------------------------------------------------
--  DDL for Procedure PROC_REBUILD_INDEXES_EDGIS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."PROC_REBUILD_INDEXES_EDGIS" (SchemaName IN NVARCHAR2, FreshExecution in BOOLEAN)
AS
--CURSOR CUR_INDEXES IS SELECT index_name FROM all_indexes WHERE owner = PROC_REBUILD_INDEXS.SchemaName and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N';
 CURSOR CUR_INDEXES_CHANGEDETECTION IS SELECT index_name FROM all_indexes WHERE owner = PROC_REBUILD_INDEXES_EDGIS.SchemaName
 and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' AND table_name = 'PGE_CHANGEDETECTIONGRIDS' ORDER BY owner, index_name;
 
CURSOR CUR_INDEXES IS SELECT index_name FROM all_indexes WHERE owner = PROC_REBUILD_INDEXES_EDGIS.SchemaName and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' AND table_name <> 'PGE_CHANGEDETECTIONGRIDS' ORDER BY owner, index_name;

CURSOR CUR_INDEXES2 IS SELECT index_name FROM UC4ADMIN.FAILED_INDEXES WHERE OWNER = PROC_REBUILD_INDEXES_EDGIS.SchemaName;
lv_idx_name all_indexes.index_name%type;
lv_idx_name2 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
lv_idx_name3 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
AlterQueryStatement NVARCHAR2(2000);
cnt integer:=0;
cntfailed integer:=0;
v_errm varchar2(100):='';
BEGIN
---------------------------------------------------ATTEMPT 1 ---------------------------------------------------------------------------
IF FreshExecution = TRUE THEN
 DBMS_OUTPUT.PUT_LINE('Executing alter index rebuild parallel for PGE_CHANGEDETECTIONGRIDS...Time Started: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
 OPEN CUR_INDEXES_CHANGEDETECTION;
    LOOP
    FETCH CUR_INDEXES_CHANGEDETECTION INTO lv_idx_name;
        IF CUR_INDEXES_CHANGEDETECTION%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN   
            cnt:=cnt +1;
           
            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING';
            dbms_output.put_line('ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING' );
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN            
                dbms_output.put_line('Error:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING;');
                --v_code := SQLCODE;
                v_errm := SUBSTR(SQLERRM, 1 , 100);
                INSERT INTO UC4ADMIN.FAILED_INDEXES(OWNER,INDEX_NAME,ALTER_QUERY,Remarks) 
                VALUES(PROC_REBUILD_INDEXES_EDGIS.SchemaName,lv_idx_name,'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12) NOLOGGING;',v_errm);
                COMMIT;
                cntfailed :=cntfailed+1;
            END;
        END;
    END LOOP;
    dbms_output.put_line('Total parallel indexes executed for PGE_CHANGEDETECTIONGRIDS.FirstExecution:TRUE-'||cnt);
    dbms_output.put_line('Total parallel failed executed '|| cntfailed);
    DBMS_OUTPUT.PUT_LINE('Time Completed: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    CLOSE CUR_INDEXES_CHANGEDETECTION;
    ---Executing alter index noparallel...
    DBMS_OUTPUT.PUT_LINE('Executed alter index rebuild parallel for PGE_CHANGEDETECTIONGRIDS...Time Started nonparallel: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    DBMS_LOCK.Sleep(10);
    
    cnt:=0;
    
    DBMS_OUTPUT.PUT_LINE('Executing alter index rebuild noparallel for PGE_CHANGEDETECTIONGRIDS...Time Started: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
 OPEN CUR_INDEXES_CHANGEDETECTION;
    LOOP
    FETCH CUR_INDEXES_CHANGEDETECTION INTO lv_idx_name;
        IF CUR_INDEXES_CHANGEDETECTION%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN   
            cnt:=cnt +1;
           
            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' NOPARALLEL';
            dbms_output.put_line('ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' NOPARALLEL' );
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN            
                dbms_output.put_line('Error:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING;');
                --v_code := SQLCODE;
                v_errm := SUBSTR(SQLERRM, 1 , 100);
                INSERT INTO UC4ADMIN.FAILED_INDEXES(OWNER,INDEX_NAME,ALTER_QUERY,Remarks) 
                VALUES(PROC_REBUILD_INDEXES_EDGIS.SchemaName,lv_idx_name,'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12) NOLOGGING;',v_errm);
                COMMIT;
                cntfailed :=cntfailed+1;
            END;
        END;
    END LOOP;
    dbms_output.put_line('Total parallel indexes executed for PGE_CHANGEDETECTIONGRIDS.FirstExecution:TRUE-'||cnt);
    dbms_output.put_line('Total parallel failed executed '|| cntfailed);
    DBMS_OUTPUT.PUT_LINE('Time Completed: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    CLOSE CUR_INDEXES_CHANGEDETECTION;
    ---Executing alter index noparallel...
    DBMS_OUTPUT.PUT_LINE('Executed alter index rebuild noparallel for PGE_CHANGEDETECTIONGRIDS...Time Started nonparallel: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    DBMS_LOCK.Sleep(10);
    
    cnt:=0;
    ---Executing alter index noparallel...
    OPEN CUR_INDEXES;
    LOOP
    FETCH CUR_INDEXES INTO lv_idx_name;
        IF CUR_INDEXES%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN   
            cnt:=cnt +1;
           
            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING';
            dbms_output.put_line('ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING' );
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN            
                dbms_output.put_line('Error:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12 ) NOLOGGING;');
                --v_code := SQLCODE;
                v_errm := SUBSTR(SQLERRM, 1 , 100);
                INSERT INTO UC4ADMIN.FAILED_INDEXES(OWNER,INDEX_NAME,ALTER_QUERY,Remarks) 
                VALUES(PROC_REBUILD_INDEXES_EDGIS.SchemaName,lv_idx_name,'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 12) NOLOGGING;',v_errm);
                COMMIT;
                cntfailed :=cntfailed+1;
            END;
        END;
    END LOOP;
    dbms_output.put_line('Total parallel indexes executed.FirstExecution:TRUE-'||cnt);
    dbms_output.put_line('Total parallel failed executed '|| cntfailed);
    DBMS_OUTPUT.PUT_LINE('Time Completed: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    CLOSE CUR_INDEXES;
    ---Executing alter index noparallel...
    DBMS_OUTPUT.PUT_LINE('Time Started nonparallel: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    DBMS_LOCK.Sleep(10);
    cnt:=0;
    OPEN CUR_INDEXES;
    LOOP
    FETCH CUR_INDEXES INTO lv_idx_name;
        IF CUR_INDEXES%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN   
            cnt:=cnt +1;
           
            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' NOPARALLEL';
            dbms_output.put_line('ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name ||' NOPARALLEL');
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN       
             v_errm := SUBSTR(SQLERRM, 1 , 100);
                dbms_output.put_line('Error:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' NOPARALLEL;');
                INSERT INTO UC4ADMIN.FAILED_INDEXES(OWNER,INDEX_NAME,ALTER_QUERY,Remarks) VALUES(PROC_REBUILD_INDEXES_EDGIS.SchemaName,lv_idx_name,'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name || ' NOPARALLEL;',v_errm);
                COMMIT;
                cntfailed :=cntfailed+1;
            END;
        END;
    END LOOP;
    dbms_output.put_line('Total no parallel indexes executed.FirstExecution:TRUE-'||cnt);
    dbms_output.put_line('Total no parallel failed executed '|| cntfailed);
    DBMS_OUTPUT.PUT_LINE('Time Completed nonparallel: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    CLOSE CUR_INDEXES;
ELSE
    OPEN CUR_INDEXES2;
     DBMS_OUTPUT.PUT_LINE('Time Started Retry: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
    LOOP
    FETCH CUR_INDEXES2 INTO lv_idx_name2;
        IF CUR_INDEXES2%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN        

            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' REBUILD  parallel (degree 12) NOLOGGING';        
            dbms_output.put_line('RetriedSuccess: '||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' REBUILD  parallel (degree 12 ) NOLOGGING');
            COMMIT;  
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN
            v_errm := SUBSTR(SQLERRM, 1 , 100);
                dbms_output.put_line('RetriedError: '||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' REBUILD parallel (degree 12 ) NOLOGGING;');
                UPDATE UC4ADMIN.FAILED_INDEXES SET Remarks =v_errm  WHERE INDEX_NAME = lv_idx_name2 AND OWNER = PROC_REBUILD_INDEXES_EDGIS.SchemaName;
                COMMIT;
            END;
        END;
    END LOOP;
    CLOSE CUR_INDEXES2;
    --Execute No parallel
    OPEN CUR_INDEXES2;
    LOOP
    FETCH CUR_INDEXES2 INTO lv_idx_name2;
        IF CUR_INDEXES2%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN        

            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' NOPARALLEL';        
            DELETE FROM UC4ADMIN.FAILED_INDEXES WHERE INDEX_NAME = lv_idx_name2 AND OWNER = PROC_REBUILD_INDEXES_EDGIS.SchemaName;        
            dbms_output.put_line('RetriedSuccess: '||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' NOPARALLEL');
            COMMIT;  
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN
            v_errm := SUBSTR(SQLERRM, 1 , 100);
                dbms_output.put_line('RetriedError: '||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXES_EDGIS.SchemaName || '.' || lv_idx_name2 || ' NOPARALLEL');
                UPDATE UC4ADMIN.FAILED_INDEXES SET Remarks =v_errm  WHERE INDEX_NAME = lv_idx_name2 AND OWNER = PROC_REBUILD_INDEXES_EDGIS.SchemaName;
                COMMIT;
            END;
        END;
    END LOOP;
    CLOSE CUR_INDEXES2;
     DBMS_OUTPUT.PUT_LINE('Time Completed Retry: ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'));
END IF;
END;

-------------------------------------------------------------END----------------------------------------------------------------------------------------
