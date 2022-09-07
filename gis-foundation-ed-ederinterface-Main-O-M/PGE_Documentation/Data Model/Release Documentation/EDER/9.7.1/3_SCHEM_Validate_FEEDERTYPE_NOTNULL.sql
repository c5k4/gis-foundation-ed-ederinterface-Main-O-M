set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
set timing on
set linesize 160
set pagesize 50000
select sysdate from dual;
DECLARE
CURSOR versioned_tables IS 
  SELECT tr.registration_id,tr.rowid_column,la.layer_id,tr.table_name,tr.owner FROM (select * from sde.table_registry 
  WHERE bitand(sde.table_registry.object_flags,8) = 8) tr
  join sde.layers la on la.table_name=tr.table_name and la.owner=tr.owner
  where tr.owner='EDGIS'
  ORDER BY tr.table_name;
CURSOR non_versioned_tables IS 
  SELECT tr.registration_id, tr.rowid_column,la.layer_id,tr.table_name,tr.owner FROM (select * from sde.table_registry 
  WHERE bitand(sde.table_registry.object_flags,8) = 0) tr
  join sde.layers la on la.table_name=tr.table_name and la.owner=tr.owner
  where tr.owner='EDGIS'
  ORDER BY tr.table_name;
sqlstm VARCHAR2(1024);
row_cnt NUMBER;
tb_name VARCHAR2(32);
seq_cnt number;
fid_cnt number;
fseq_cnt number;
BEGIN
dbms_output.put_line('If any Tables are found to have problems the will be will be listed here: ');
dbms_output.put_line('*****Checking Versioned Tables****** ');
  FOR table_info IN versioned_tables LOOP
    -- dbms_output.put_line('TABLE: '||table_info.owner||'.'||table_info.table_name);
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID';
    IF row_cnt = 1 then
	  SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='FEEDERTYPE';
      IF row_cnt = 1 then
        sqlstm := 'select sum(total) from ( select count(*) as total from '||table_info.owner||'.'||table_info.table_name||' where FEEDERTYPE is null and CIRCUITID is not null union select count(*) as total from '||table_info.owner||'.A'||table_info.registration_id||' where FEEDERTYPE is null and CIRCUITID is not null ) ';
        -- dbms_output.put_line(sqlstm);
        row_cnt := -1 ;
        execute immediate sqlstm into row_cnt ;               
        IF row_cnt>0 then 
            dbms_output.put_line('! ! ! ! ! FEEDERTYPE is null, but has CircuitID count of '||row_cnt||' for '||table_info.owner||'.'||table_info.table_name);
        END IF;
      END IF;
	END IF;
  END LOOP;
dbms_output.put_line('*****Checking Non-Versioned Tables****** ');  
  FOR table_info IN non_versioned_tables LOOP
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID';
    IF row_cnt = 1 then
      SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='FEEDERTYPE';
      IF row_cnt = 1 then
        sqlstm := 'select count(*)  from '||table_info.owner||'.'||table_info.table_name||'  where FEEDERTYPE is null and CIRCUITID is not null  ';
        -- dbms_output.put_line(sqlstm);
        row_cnt := -1 ;
        execute immediate sqlstm into row_cnt ;               
        IF row_cnt>0 then 
            dbms_output.put_line('! ! ! ! ! FEEDERTYPE is null, but has CircuitID count of '||row_cnt||' for '||table_info.owner||'.'||table_info.table_name);
        END IF;
      END IF;
	END IF;
  END LOOP;
END;
/
select sysdate from dual;
