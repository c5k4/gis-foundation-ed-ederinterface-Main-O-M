set serveroutput on
set timing on
DECLARE
cursor unregistered_tables_to_set is 
   select owner,table_name from all_tables where owner='EDGIS' and table_name like 'PGE%' and (owner,table_name) not in (select owner,table_name from sde.table_registry);
sql_stmt varchar2(2000);
sql_editor varchar2(2000);
sql_viewer varchar2(2000);
object_tmp varchar2(65);
BEGIN
 for tab_value in unregistered_tables_to_set loop
     object_tmp := tab_value.owner||'.'||tab_value.table_name ;
     sql_editor := 'grant all on '||object_tmp||' to ';
	 sql_viewer := 'grant SELECT on '||object_tmp||' to ';
	 sql_stmt := sql_editor||' GISINTERFACE';
	 dbms_output.put_line(sql_stmt);
	 execute immediate sql_stmt ;
	 sql_stmt := sql_editor||' SDE_EDITOR';
	 dbms_output.put_line(sql_stmt);
	 execute immediate sql_stmt ;
	 sql_stmt := sql_viewer||' SDE_VIEWER';
	 dbms_output.put_line(sql_stmt);
	 execute immediate sql_stmt ;
	 sql_stmt := sql_viewer||' EDGISBO';
	 dbms_output.put_line(sql_stmt);
	 execute immediate sql_stmt ;
 END LOOP;
END;
/
