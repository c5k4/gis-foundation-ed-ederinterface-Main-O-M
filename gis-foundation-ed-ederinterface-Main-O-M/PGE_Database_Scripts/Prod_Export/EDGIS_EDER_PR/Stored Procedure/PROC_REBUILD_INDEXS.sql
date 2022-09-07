--------------------------------------------------------
--  DDL for Procedure PROC_REBUILD_INDEXS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."PROC_REBUILD_INDEXS" (SchemaName IN NVARCHAR2, FreshExecution in BOOLEAN)
AS
CURSOR CUR_INDEXES IS SELECT index_name FROM all_indexes WHERE owner = PROC_REBUILD_INDEXS.SchemaName and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N';
CURSOR CUR_INDEXES2 IS SELECT index_name FROM UC4ADMIN.FAILED_INDEXES WHERE OWNER = PROC_REBUILD_INDEXS.SchemaName;
lv_idx_name all_indexes.index_name%type;
lv_idx_name2 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
lv_idx_name3 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
AlterQueryStatement NVARCHAR2(2000);
cnt integer:=0;
cntfailed integer:=0;
BEGIN
---------------------------------------------------ATTEMPT 1 ---------------------------------------------------------------------------
IF FreshExecution = TRUE THEN
    OPEN CUR_INDEXES;
    LOOP
    FETCH CUR_INDEXES INTO lv_idx_name;
        IF CUR_INDEXES%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN   
            cnt:=cnt +1;
            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name || ' REBUILD ONLINE parallel (degree 8) NOLOGGING';
            dbms_output.put_line('Success:'||'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name );
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN            
                dbms_output.put_line('Error:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name || ' REBUILD parallel (degree 8) NOLOGGING');
                INSERT INTO UC4ADMIN.FAILED_INDEXES(OWNER,INDEX_NAME,ALTER_QUERY) VALUES(PROC_REBUILD_INDEXS.SchemaName,lv_idx_name,'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name || ' REBUILD ONLINE parallel (degree 8) NOLOGGING');
                COMMIT;
                cntfailed :=cntfailed+1;
            END;
        END;
    END LOOP;
    dbms_output.put_line('Total indexes executed.FirstExecution:TRUE-'||cnt);
    dbms_output.put_line('Total failed executed '|| cntfailed);
    CLOSE CUR_INDEXES;
ELSE
    OPEN CUR_INDEXES2;
    LOOP
    FETCH CUR_INDEXES2 INTO lv_idx_name2;
        IF CUR_INDEXES2%NOTFOUND
        THEN
        EXIT;
        END IF;
        BEGIN        

            EXECUTE IMMEDIATE 'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name2 || ' REBUILD parallel (degree 8) NOLOGGING';        
            DELETE FROM UC4ADMIN.FAILED_INDEXES WHERE INDEX_NAME = lv_idx_name2 AND OWNER = PROC_REBUILD_INDEXS.SchemaName;        
            dbms_output.put_line('Success:'||'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name2 || ' REBUILD parallel (degree 8) NOLOGGING');
            COMMIT;  
            EXCEPTION
            WHEN OTHERS THEN           
            BEGIN
                dbms_output.put_line('Error1:'||SQLERRM ||'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || lv_idx_name2 || ' REBUILD parallel (degree 8) NOLOGGING');
                UPDATE UC4ADMIN.FAILED_INDEXES SET INDEX_NAME = lv_idx_name2 WHERE INDEX_NAME = lv_idx_name2 AND OWNER = PROC_REBUILD_INDEXS.SchemaName;
                COMMIT;
            END;
        END;
    END LOOP;
    CLOSE CUR_INDEXES2;
END IF;
END;

-------------------------------------------------------------END----------------------------------------------------------------------------------------
