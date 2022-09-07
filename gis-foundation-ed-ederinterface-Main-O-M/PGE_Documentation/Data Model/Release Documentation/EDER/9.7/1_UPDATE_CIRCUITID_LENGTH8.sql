set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
set timing on
select sysdate from dual;
DECLARE
CURSOR versioned_tables IS 
  SELECT NVL(tr.registration_id,-1) registration_id,NVL(tr.rowid_column,'NONE') rowid_column,NVL(la.layer_id,-1) layer_id,tr.table_name,tr.owner,NVL(sgc3.column_name,'NOTST') column_name FROM (
  select * from sde.table_registry 
  WHERE bitand(sde.table_registry.object_flags,8) = 8 
  and table_name not in (select NVL(tr3.IMV_VIEW_NAME,'NONAME') from sde.table_registry tr3 group by tr3.IMV_VIEW_NAME)
  and table_name not in (select tr4.table_name from sde.table_registry tr4 where bitand(tr4.object_flags,128)=128 group by tr4.table_name) 
  ) tr
  left outer join (
  select * from sde.layers 
      where table_name not in (select tr4.table_name from sde.table_registry tr4 where bitand(tr4.object_flags,128)=128 group by tr4.table_name) 
	  )la on la.table_name=tr.table_name and la.owner=tr.owner
  left outer join (select column_name,table_name,owner from sde.column_registry where bitand(object_flags,32768)=32768 ) sgc3 on sgc3.table_name=tr.table_name and sgc3.owner=tr.owner
  where tr.owner='EDGIS'
  ORDER BY tr.table_name;
CURSOR non_versioned_tables IS 
  SELECT NVL(tr.registration_id,-1) registration_id, NVL(tr.rowid_column,'NONE') rowid_column,NVL(la.layer_id,-1) layer_id,tr.table_name,tr.owner,NVL(sgc3.column_name,'NOTST') column_name FROM (
    select * from sde.table_registry 
           WHERE bitand(sde.table_registry.object_flags,8) = 0 
		   and table_name not in (select NVL(tr3.IMV_VIEW_NAME,'NONAME') from sde.table_registry tr3 group by tr3.IMV_VIEW_NAME) 
		   and table_name not in (select tr4.table_name from sde.table_registry tr4 where bitand(tr4.object_flags,128)=128 group by tr4.table_name) 
     ) tr
  left outer join (
  select * from sde.layers 
      where table_name not in (select tr4.table_name from sde.table_registry tr4 where bitand(tr4.object_flags,128)=128 group by tr4.table_name) 
	  )la on la.table_name=tr.table_name and la.owner=tr.owner
  left outer join (select column_name,table_name,owner from sde.column_registry where bitand(object_flags,32768)=32768 ) sgc3 on sgc3.table_name=tr.table_name and sgc3.owner=tr.owner
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
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID';
    IF row_cnt = 1 then
        sqlstm := ' update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID=''0''||CIRCUITID where length(circuitid)=8 and circuitid is not null';
		dbms_output.put_line(sqlstm);
        execute immediate sqlstm ;               
        IF table_info.registration_id>-1 then
            sqlstm := ' update '||table_info.owner||'.A'||table_info.registration_id||' set CIRCUITID=''0''||CIRCUITID where length(circuitid)=8 and circuitid is not null';		
            dbms_output.put_line(sqlstm);
            execute immediate sqlstm ;  
        END IF;
    END IF;
	COMMIT;
	SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID2';
    IF row_cnt = 1 then
        sqlstm := ' update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID2=''0''||CIRCUITID2 where length(CIRCUITID2)=8 and CIRCUITID2 is not null';
		dbms_output.put_line(sqlstm);
        execute immediate sqlstm ;               
        IF table_info.registration_id>-1 then
            sqlstm := ' update '||table_info.owner||'.A'||table_info.registration_id||' set CIRCUITID2=''0''||CIRCUITID2 where length(CIRCUITID2)=8 and CIRCUITID2 is not null';		
            dbms_output.put_line(sqlstm);
            execute immediate sqlstm ;  
        END IF;
    END IF;
	COMMIT;
  END LOOP;
dbms_output.put_line('*****Checking Non-Versioned Tables****** ');  
  FOR table_info IN non_versioned_tables LOOP
    SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID';
    IF row_cnt = 1 then
        sqlstm := ' update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID=''0''||CIRCUITID where length(circuitid)=8 and circuitid is not null';
		dbms_output.put_line(sqlstm);
        execute immediate sqlstm ;
    END IF;
	SELECT COUNT(*) INTO row_cnt FROM sde.column_registry where TABLE_NAME = table_info.table_name and OWNER=table_info.owner and column_name='CIRCUITID2';
    IF row_cnt = 1 then
        sqlstm := ' update '||table_info.owner||'.'||table_info.table_name||' set CIRCUITID2=''0''||CIRCUITID2 where length(CIRCUITID2)=8 and CIRCUITID2 is not null';
		dbms_output.put_line(sqlstm);
        execute immediate sqlstm ;
    END IF;
  END LOOP;
END;
/
select sysdate from dual;
