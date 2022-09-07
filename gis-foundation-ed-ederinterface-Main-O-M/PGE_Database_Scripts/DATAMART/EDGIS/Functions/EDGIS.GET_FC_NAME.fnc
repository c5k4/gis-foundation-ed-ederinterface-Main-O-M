Prompt drop Function GET_FC_NAME;
DROP FUNCTION EDGIS.GET_FC_NAME
/

Prompt Function GET_FC_NAME;
--
-- GET_FC_NAME  (Function) 
--
CREATE OR REPLACE FUNCTION EDGIS.GET_FC_NAME (guid IN VARCHAR2)
RETURN nvarchar2
is
BEGIN
  DECLARE
    CURSOR ALL_TABLES_WE_CARE_ABOUT
    IS
      SELECT a.owner,
        a.table_name
      FROM sde.column_registry a
      WHERE a.column_name='GLOBALID'
      AND a.table_name  IN
        (SELECT b.table_name
        FROM sde.column_registry b
        WHERE b.column_name='SAPEQUIPID'
        )and a.table_name NOT LIKE 'ZPGEVW%' and a.table_name NOT LIKE 'ZZ_MV%';

    sqlstmt    VARCHAR2(2000);
    reg_num   NUMBER;
    count_num NUMBER;
    rowcnt number;
    rowcnt2 number;
    equipid varchar2(20);
  BEGIN
    dbms_output.put_line('GUID [ '||guid||' ]');
    FOR tables IN ALL_TABLES_WE_CARE_ABOUT
    LOOP
      dbms_output.put_line('TABLE [ '||tables.table_name||' ]');
      sqlstmt := 'select count(*) from (select view_name from all_views where view_name = ''ZZ_MV_'||tables.table_name||''')';
      execute immediate sqlstmt into rowcnt;
      if rowcnt=0 then
        sqlstmt := 'select count(*) from '||tables.table_name||' where globalid='''||guid||'''  ';
      ELSE
        sqlstmt := 'select count(*) from zz_mv_'||tables.table_name||' where globalid='''||guid||'''  ';
      end if;
      dbms_output.put_line('SQLSTMT [ '||SQLSTMT||' ]');
      execute immediate sqlstmt into rowcnt;
      if rowcnt>0 then
       dbms_output.put_line('BINGO!!! [ '||tables.table_name||' ]');
        sqlstmt := 'SELECT COUNT(*) FROM all_views WHERE LOWER(view_name) = ''zz_mv_'||lower(tables.table_name)||''' ';
        execute immediate sqlstmt into rowcnt;
        if rowcnt>0 then
          sqlstmt := 'select sapequipid from zz_mv_'||tables.table_name||' where globalid='''||guid||'''  ';
          dbms_output.put_line(sqlstmt);
          execute immediate sqlstmt into equipid;
          dbms_output.put_line('SAPEQUIPID [ '||equipid||' ]');
          return 'EDGIS.'||tables.table_name;
        end if;
      END IF;

    END LOOP;
    return null;
  END;
END GET_FC_NAME;

/


Prompt Grants on FUNCTION GET_FC_NAME TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO GISINTERFACE
/

Prompt Grants on FUNCTION GET_FC_NAME TO GIS_I to GIS_I;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO GIS_I
/

Prompt Grants on FUNCTION GET_FC_NAME TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO GIS_I_WRITE
/

Prompt Grants on FUNCTION GET_FC_NAME TO MM_ADMIN to MM_ADMIN;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO MM_ADMIN
/

Prompt Grants on FUNCTION GET_FC_NAME TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO SDE_EDITOR
/

Prompt Grants on FUNCTION GET_FC_NAME TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE ON EDGIS.GET_FC_NAME TO SDE_VIEWER
/
