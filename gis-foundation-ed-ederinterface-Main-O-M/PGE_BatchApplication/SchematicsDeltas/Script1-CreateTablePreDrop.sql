set serveroutput on
declare
   v_str varchar2(2000);
   v_cnt number;
   v_layerid number;
   v_sql varchar2(2000);
   v_columns varchar2(2000);
   v_rowid varchar2(32);
   v_columns2 varchar2(2000);
   v_columnCnt number;
   v_regid number;
   v_myRegid number;
   cursor cur_schematics is select id,owner from sde.sch_dataset;
   cursor cur_rowid (pOwner in varchar2,pTableName in varchar2) is select rowid_column from sde.table_registry where owner = pOwner and table_name = pTableName;
   cursor cur_layer (pOwner in varchar2,pTableName in varchar2) is select layer_id from sde.layers where upper(table_name) = pTableName and upper(owner) = pOwner;
   cursor cur_reg (pOwner in varchar2,pTableName in varchar2) is select registration_id from sde.table_registry where upper(table_name) = pTableName and upper(owner) = pOwner;
   cursor cur_tables (pOwner in varchar2,pTableName in varchar2) is select owner,table_name from all_tables where owner = pOwner and table_name like pTableName;
   cursor cur_columns (pOwner in varchar2,pTableName in varchar2) is select column_name,data_type,data_length from all_tab_columns where owner = pOwner and table_name = pTableName 
   and column_name not in ('OID','UCID','UOID','USID','SHAPE','UGUID') and data_type not in ('BLOB');
begin
   execute immediate 'select count(*) from all_tables where owner = ''SDE'' and table_name = ''SCHEM_GUID_PRE_DROP_TEMP'' ' into v_cnt;
   if v_cnt > 0 then
      execute immediate 'drop table SDE.schem_guid_pre_drop_temp';
   end if;
   execute immediate 'create table SDE.schem_guid_pre_drop_temp (OID number(38), owner nvarchar2(160),SCH_TABLE NVARCHAR2(160),UCID NUMBER(38),UOID NUMBER(38), UGUID NVARCHAR2(38),USID NUMBER(38), EMINX FLOAT(64),EMINY FLOAT(64),EMAXX FLOAT(64),EMAXY FLOAT(64))';
   v_str:=' ';
   for c_schematics in cur_schematics loop
       for c_tables in cur_tables(c_schematics.owner,'SCH'||c_schematics.id||'E_%') loop
           open cur_layer (c_tables.owner,c_tables.table_name);
           fetch cur_layer into v_layerid;
           close cur_layer;
           for c_columns in cur_columns (c_tables.owner,c_tables.table_name) loop
                   if instr(v_str,'~'||c_columns.column_name||'~') = 0 then
                      v_str:=v_str||'~'||c_columns.column_name||'~'||','; 
                      v_sql:='alter table SDE.schem_guid_pre_drop_temp add ('||c_columns.column_name||' '||c_columns.data_type||'('||c_columns.data_length||'))';    
                      dbms_output.put_line (v_sql);
                      execute immediate v_sql;    
                      dbms_output.put_line(c_tables.owner||'.'||c_tables.table_name||','||v_layerid||','||c_columns.column_name||','||length(c_columns.column_name));
                   end if;
           end loop;
       end loop;
   end loop;
end;
/
