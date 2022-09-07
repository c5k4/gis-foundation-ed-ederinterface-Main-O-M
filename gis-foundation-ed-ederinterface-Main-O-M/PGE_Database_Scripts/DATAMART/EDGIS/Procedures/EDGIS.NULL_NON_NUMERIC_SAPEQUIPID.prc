Prompt drop Procedure NULL_NON_NUMERIC_SAPEQUIPID;
DROP PROCEDURE EDGIS.NULL_NON_NUMERIC_SAPEQUIPID
/

Prompt Procedure NULL_NON_NUMERIC_SAPEQUIPID;
--
-- NULL_NON_NUMERIC_SAPEQUIPID  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.NULL_NON_NUMERIC_SAPEQUIPID
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
/


Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO GISINTERFACE
/

Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO GIS_I
/

Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO MM_ADMIN
/

Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO SDE_EDITOR
/

Prompt Grants on PROCEDURE NULL_NON_NUMERIC_SAPEQUIPID TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.NULL_NON_NUMERIC_SAPEQUIPID TO SDE_VIEWER
/
