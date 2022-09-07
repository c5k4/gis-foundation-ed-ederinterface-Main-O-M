Prompt drop Procedure TEST_SPECIAL_CHARS;
DROP PROCEDURE EDGIS.TEST_SPECIAL_CHARS
/

Prompt Procedure TEST_SPECIAL_CHARS;
--
-- TEST_SPECIAL_CHARS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.TEST_SPECIAL_CHARS
IS
  CURSOR ss_cursor is SELECT b.table_name, b.column_name
FROM sde.column_registry b
WHERE b.sde_type in (5,13,14) and b.column_size >=20 and b.column_name != 'VERSIONNAME'
 and b.table_name in (SELECT a.table_name
FROM sde.column_registry a
WHERE a.column_name='SAPEQUIPID'
and A.table_name NOT LIKE 'ZPGEVW%' and A.table_name NOT LIKE 'ZZ_MV%' and A.table_name not like 'A%' and a.table_name not like '%TT%'
);

  TYPE EmpCurTyp  IS REF CURSOR;
  v_emp_cursor    EmpCurTyp;
  fc VARCHAR2(200);
   guid varchar2(40);
   assetid varchar2(40);
    rowcnt number;
    objectid number;
    table_name varchar2(200);
    column_name varchar2(200);
   sqlstmt VARCHAR2(2000);
BEGIN
	FOR ss_row in ss_cursor LOOP

    select ss_row.table_name, ss_row.column_name into table_name, column_name from dual;
    sqlstmt := 'select globalid,'',EDGIS.'||table_name||''' from '||table_name||' where '||column_name||' like ''%,%'' or '||column_name||' like ''%''''%'' or '||column_name||' like ''%"%''';
    dbms_output.put_line(sqlstmt||' UNION ');

   END LOOP;
   dbms_output.put_line('done');
END;
/


Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO GISINTERFACE
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO GIS_I
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO IGPEDITOR
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO MM_ADMIN
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO SDE_EDITOR
/

Prompt Grants on PROCEDURE TEST_SPECIAL_CHARS TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.TEST_SPECIAL_CHARS TO SDE_VIEWER
/
