set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
set timing on
select sysdate from dual;
select 'updating feature classes that have circuitid and feedertype fields' from dual;

DECLARE
   cursor push_feeder_type_list is select owner||'.'||table_name object_name from sde.column_registry where column_name='FEEDERTYPE' and table_name not like '%CIRCUITSOURCE%' and table_name in (select table_name from sde.column_registry where column_name='CIRCUITID') order by table_name;
sql_stmt varchar2(2000);
feedertype_num number;
BEGIN
FOR tab_obj IN push_feeder_type_list LOOP
  sql_stmt:='update '||tab_obj.object_name||' set FEEDERTYPE=3 where circuitid in (select circuitid from edgis.temp_circuitsource where feedertype=3) and circuitid is not null and feedertype is null';
  dbms_output.put_line(sql_stmt);
  execute immediate sql_stmt;
  sql_stmt:='update '||tab_obj.object_name||' set FEEDERTYPE=2 where circuitid in (select circuitid from edgis.temp_circuitsource where feedertype=2) and circuitid is not null and feedertype is null';
  dbms_output.put_line(sql_stmt);
  execute immediate sql_stmt;
  sql_stmt:='update '||tab_obj.object_name||' set FEEDERTYPE=1 where circuitid in (select circuitid from edgis.temp_circuitsource where feedertype=1) and circuitid is not null and feedertype is null';
  dbms_output.put_line(sql_stmt);
  execute immediate sql_stmt;
  -- commit;
END LOOP;
END;
/

DECLARE
  cursor table_ids is select owner,table_name,registration_id from sde.table_registry 
  where table_name in (
     select table_name 
	 from sde.column_registry 
	 where column_name='CIRCUITCOLOR'
	 and table_name like '%PRI%');
sql_stmt varchar2(2000);
result_count number;
BEGIN
    for tabname in table_ids loop
      sql_stmt := 'update '||tabname.owner||'.'||tabname.table_name||' cs set cs.circuitcolor=(select cct.circuitcolor from edgis.temp_circuitsource cct where cct.circuitid=cs.circuitid group by cct.circuitcolor ) where exists (select 1 from edgis.temp_circuitsource cct1 where  cct1.circuitid=cs.circuitid )';
      dbms_output.put_line(sql_stmt);
      execute immediate sql_stmt;
                  sql_stmt :='select count(*) from '||tabname.owner||'.'||tabname.table_name||' where circuitcolor is null';
                  dbms_output.put_line(sql_stmt);
                  result_count :=0;
      execute immediate sql_stmt into result_count;
                  dbms_output.put_line(result_count);
      sql_stmt := 'update '||tabname.owner||'.A'||tabname.registration_id||' cs set cs.circuitcolor=(select cct.circuitcolor from edgis.temp_circuitsource cct where cct.circuitid=cs.circuitid group by cct.circuitcolor ) where exists (select 1 from edgis.temp_circuitsource cct1 where  cct1.circuitid=cs.circuitid )'; 
      dbms_output.put_line(sql_stmt);
                  execute immediate sql_stmt;
                  sql_stmt :='select count(*) from '||tabname.owner||'.A'||tabname.registration_id||' where circuitcolor is null';
                  dbms_output.put_line(sql_stmt);
                  result_count :=0;
      execute immediate sql_stmt into result_count;
                  dbms_output.put_line(result_count);
                end loop;
                commit;
end;
/

select sysdate from dual;