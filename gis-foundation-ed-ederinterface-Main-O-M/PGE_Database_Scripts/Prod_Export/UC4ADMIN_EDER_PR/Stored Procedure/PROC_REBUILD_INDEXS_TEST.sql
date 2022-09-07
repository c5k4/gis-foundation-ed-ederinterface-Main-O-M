--------------------------------------------------------
--  DDL for Procedure PROC_REBUILD_INDEXS_TEST
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "UC4ADMIN"."PROC_REBUILD_INDEXS_TEST" (SchemaName IN NVARCHAR2, FreshExecution in BOOLEAN)
AS
--CURSOR CUR_INDEXES IS SELECT index_name FROM all_indexes WHERE upper(owner) = upper(PROC_REBUILD_INDEXS.SchemaName) and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' ;
--CURSOR CUR_INDEXES2 IS SELECT index_name FROM UC4ADMIN.FAILED_INDEXES WHERE upper(OWNER) = upper(PROC_REBUILD_INDEXS.SchemaName)  ;
--lv_idx_name all_indexes.index_name%type;
--lv_idx_name2 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
--lv_idx_name3 UC4ADMIN.FAILED_INDEXES.INDEX_NAME%type;
AlterQueryStatement NVARCHAR2(2000);
verrordesc VARCHAR2(2000);
cnt integer:=0;
BEGIN
---------------------------------------------------ATTEMPT 1 ---------------------------------------------------------------------------
IF FreshExecution = TRUE THEN

BEGIN
--OPEN CUR_INDEXES;
--FOR CUR_tx IN (SELECT index_name FROM all_indexes WHERE upper(owner) = 'EDGIS' and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N' ) 
  FOR CUR_tx IN (SELECT index_name FROM all_indexes WHERE upper(owner) = upper(SchemaName) and INDEX_TYPE = 'NORMAL' AND TEMPORARY='N'  ) 
      LOOP
      Begin 
        cnt:=cnt +1;
        --dbms_output.put_line('Success:'||'ALTER INDEX ' ||  PROC_REBUILD_INDEXS.SchemaName || '.' || rows.index_name || ' REBUILD ONLINE parallel (degree 8)');
      end;
      END LOOP;
      dbms_output.put_line( cnt);
      --DBMS_OUTPUT.PUT_LINE(?** Exception Reindexing Table ? || IDX.index_name || ?; Error:? || SQLERRM);
END;
END if;
end PROC_REBUILD_INDEXS_TEST;
