--------------------------------------------------------
--  DDL for Procedure NULL_NON_NUMERIC_SAPEQUIPID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."NULL_NON_NUMERIC_SAPEQUIPID" 
IS
  SQLSTMT    VARCHAR2(32000);
  v_rc sys_refcursor;
  globalId varchar2(40);
  CURSOR table_name_cursor
  IS
    SELECT b.table_name
    FROM sde.column_registry b
    WHERE b.column_name='SAPEQUIPID'
    AND b.table_name NOT LIKE 'ZPGEVW%'
    AND b.table_name NOT LIKE 'ZZ_MV%'
    AND b.table_name NOT LIKE 'A%'
    AND b.table_name not like '%PTT%';
BEGIN
  FOR rec IN table_name_cursor
  LOOP
    BEGIN
      SQLSTMT := 'SELECT globalId  FROM zz_mv_'||rec.table_name||' WHERE REGEXP_LIKE(sapequipid,''[^0-9]+'') ';
--      dbms_output.put_line(sqlstmt);
      open v_rc for sqlstmt;
      loop
        fetch v_rc into globalid;
        exit when v_rc%NOTFOUND;  -- Exit the loop when we've run out of data
        dbms_output.put_line(globalId||',EDGIS.'||rec.table_name);
      end loop;
      close v_rc;
    EXCEPTION
    WHEN NO_DATA_FOUND THEN
      continue;
    END;
  END LOOP;
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line(' query error'||sqlerrm);
END NULL_NON_NUMERIC_SAPEQUIPID;
