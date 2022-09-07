Prompt drop Package Body CD_GIS;
DROP PACKAGE BODY PGEDATA.CD_GIS
/

Prompt Package Body CD_GIS;
--
-- CD_GIS  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY PGEDATA.CD_GIS AS
FUNCTION IS_FIELD_CD
(
 t_name IN NVARCHAR2,
 field_name IN NVARCHAR2,
 cd_type NVARCHAR2)
RETURN BOOLEAN
    IS
	  sql_stmt VARCHAR2(2000);
      if_exist NUMBER;
    BEGIN
      if_exist:=0;
	  dbms_output.put_line('IS_FIELD_CD testing started with :' ||if_exist);
	  sql_stmt := 'select count(*) from pgedata.CD_MAP_SETTINGS where WATCH_TABLE='''||t_name||''' and WATCH_FIELD='''||field_name||''' and WATCH_TYPE='''||cd_type||''' ';
	  dbms_output.put_line('Running: ' ||sql_stmt);
	  dbms_output.put_line('IS_FIELD_CD testing returned :' ||if_exist);
	  execute immediate sql_stmt into if_exist;
      IF if_exist>0 THEN
	    dbms_output.put_line('IS_FIELD_CD about to return: TRUE');
        RETURN TRUE;
      ELSE
	    dbms_output.put_line('IS_FIELD_CD about to return: FALSE');
        RETURN FALSE;
      END IF;
END IS_FIELD_CD;
PROCEDURE SET_FIELD_CD
(
 t_name IN NVARCHAR2,
 globalid IN NVARCHAR2,
 cd_type NVARCHAR2)
    IS
	sql_stmt VARCHAR2(2000);
	if_exist NUMBER;
    BEGIN
	  if_exist :=0;
	  dbms_output.put_line('SET_FIELD_CD testing started with :' ||if_exist);
	  sql_stmt := 'select count(*) from CD_LIST cd where cd.WATCH_TABLE='''||t_name||''' and cd.GUID='''||globalid||''' ' ;
	  dbms_output.put_line('SET_FIELD_CD Running: ' ||sql_stmt);
	  execute immediate sql_stmt into if_exist;
	  dbms_output.put_line('SET_FIELD_CD testing returned :' ||if_exist);
	  IF if_exist>0 then
		sql_stmt := 'update CD_LIST set '||cd_type||'=1 where GUID='''||globalid||''' and WATCH_TABLE='''||t_name||''' ';
		dbms_output.put_line('SET_FIELD_CD Running: ' ||sql_stmt);
		execute immediate sql_stmt;
	  ELSE
	   sql_stmt := 'insert into CD_LIST (WATCH_TABLE,GUID,'||cd_type||') values ('''||t_name||''', '''||globalid||''',1)';
	   dbms_output.put_line('SET_FIELD_CD Running: ' ||sql_stmt);
	   execute immediate sql_stmt;
	 END IF;
END SET_FIELD_CD;
PROCEDURE UNSET_FIELD_CD
(
 t_name IN NVARCHAR2,
 globalid IN NVARCHAR2,
 cd_type NVARCHAR2)
    IS
	sql_stmt VARCHAR2(2000);
	if_exist NUMBER;
    BEGIN
	  if_exist :=0;
	  dbms_output.put_line('UNSET_FIELD_CD testing started with :' ||if_exist);
	  sql_stmt := 'select count(*) from CD_LIST cd where cd.WATCH_TABLE='''||t_name||''' and cd.GUID='''||globalid||''' ' ;
	  dbms_output.put_line('UNSET_FIELD_CD Running: ' ||sql_stmt);
	  execute immediate sql_stmt into if_exist;
	  dbms_output.put_line('UNSET_FIELD_CD testing returned :' ||if_exist);
	  IF if_exist>0 then
		sql_stmt := 'update CD_LIST set '||cd_type||'=null where GUID='''||globalid||''' and WATCH_TABLE='''||t_name||''' ';
		dbms_output.put_line('UNSET_FIELD_CD Running: ' ||sql_stmt);
		execute immediate sql_stmt;
	  ELSE
	   dbms_output.put_line('UNSET_FIELD_CD , Row to un-set not detected, skipping');
	 END IF;
END UNSET_FIELD_CD;
END CD_GIS;
/
