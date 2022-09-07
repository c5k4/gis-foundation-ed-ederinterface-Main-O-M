set serveroutput on
DECLARE
CURSOR versioned_tables IS 
  SELECT tr.registration_id,tr.rowid_column,la.layer_id,tr.table_name,tr.owner FROM (select * from sde.table_registry 
  WHERE bitand(sde.table_registry.object_flags,8) = 8 and (owner||'.'||table_name) not in (
       select objectclassname from sde.mm_class_modelnames where modelname='CIRCUITSOURCE'
       ) ) tr
  join sde.layers la on la.table_name=tr.table_name and la.owner=tr.owner
  ORDER BY tr.table_name;
CURSOR nonversioned_tables IS 
  SELECT tr.registration_id,tr.rowid_column,la.layer_id,tr.table_name,tr.owner FROM (select * from sde.table_registry 
  WHERE bitand(sde.table_registry.object_flags,8) = 8 and (owner||'.'||table_name) not in (
       select objectclassname from sde.mm_class_modelnames where modelname='CIRCUITSOURCE'
       ) ) tr
  join sde.layers la on la.table_name=tr.table_name and la.owner=tr.owner
  ORDER BY tr.table_name;
sqlstm VARCHAR2(1024);
row_cnt NUMBER;
tb_name VARCHAR2(32);
seq_cnt number;
fid_cnt number;
fseq_cnt number;
BEGIN
dbms_output.put_line('If any Tables are found to have problems the will be will be listed here: ');
dbms_output.put_line('*****Clearing Versioned Tables****** ');
row_cnt := -1 ;
  FOR table_info IN versioned_tables LOOP
    dbms_output.put_line('TABLE: '||table_info.owner||'.'||table_info.table_name);
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='FEEDERID';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set FEEDERID=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        sqlstm := 'update '||table_info.owner||'.A'||table_info.registration_id||' set FEEDERID=null';
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='FEEDERID2';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set FEEDERID2=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        sqlstm := 'update '||table_info.owner||'.A'||table_info.registration_id||' set FEEDERID2=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='CIRCUITID';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID=null' ;
        dbms_output.put_line(sqlstm);
        row_cnt := -1 ;
        execute immediate sqlstm;
        sqlstm := 'update '||table_info.owner||'.A'||table_info.registration_id||' set CIRCUITID=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;	
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='CIRCUITID2';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID2=null' ;
        dbms_output.put_line(sqlstm);
        row_cnt := -1 ;
        execute immediate sqlstm;
        sqlstm := 'update '||table_info.owner||'.A'||table_info.registration_id||' set CIRCUITID=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;	
  END LOOP;
dbms_output.put_line('*****Clearing Non-Versioned Tables****** ');  
row_cnt := -1 ;
  FOR table_info IN nonversioned_tables LOOP
    dbms_output.put_line('TABLE: '||table_info.owner||'.'||table_info.table_name);
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='FEEDERID';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set FEEDERID=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='FEEDERID2';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set FEEDERID2=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='CIRCUITID';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;	
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and COLUMN_NAME='CIRCUITID2';
    IF row_cnt = 1 then
        sqlstm := 'update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID2=null' ;
        dbms_output.put_line(sqlstm);
        execute immediate sqlstm;
        row_cnt := -1 ;
    END IF;	
  END LOOP;
END;
/

