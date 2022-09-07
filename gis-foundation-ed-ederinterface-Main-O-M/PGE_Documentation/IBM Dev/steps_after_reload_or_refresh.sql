sqlplus sys/sys@pge1 as sysdba

declare
    CURSOR owner_cur IS
        SELECT DISTINCT(OWNER) OWNER from SDE.LAYERS ORDER BY OWNER;
    CURSOR index_cur IS
        SELECT owner, index_name FROM dba_indexes WHERE owner in (select distinct(owner) from sde.layers) and INDEX_TYPE = 'NORMAL' ORDER BY owner, index_name;
    SQL_STMT VARCHAR2(200);
begin
--    dbms_output.enable (100000);
--    dbms_output.put_line ('Starting SDE tuning procedure...');
    FOR IndexRec in index_cur LOOP
        SQL_STMT := 'alter index ' || IndexRec.owner || '.' || IndexRec.index_name || ' rebuild';
--        dbms_output.put_line (SQL_STMT);
        EXECUTE IMMEDIATE SQL_STMT;
    END LOOP;
end;
/

exec dbms_stats.gather_schema_stats('EDGIS');

exec dbms_stats.gather_schema_stats('SDE');

alter system flush shared_pool;

alter sequence state_id_generator_nc increment by 1000000000;

select state_id_generator_nc.nextval from dual;

alter sequence state_id_generator_nc increment by 1;

