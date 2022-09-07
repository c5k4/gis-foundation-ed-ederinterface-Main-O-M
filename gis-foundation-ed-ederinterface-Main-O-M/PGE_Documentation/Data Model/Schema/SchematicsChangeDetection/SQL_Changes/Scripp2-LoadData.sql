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
   v_myRowID varchar2(32);
   v_myRegid number;
   cursor cur_schematics is select id,owner from sde.sch_dataset;
   cursor cur_rowid (pOwner in varchar2,pTableName in varchar2) is select rowid_column from sde.table_registry where owner = pOwner and table_name = pTableName;
   cursor cur_layer (pOwner in varchar2,pTableName in varchar2) is select layer_id from sde.layers where upper(table_name) = pTableName and upper(owner) = pOwner;
   cursor cur_reg (pOwner in varchar2,pTableName in varchar2) is select registration_id from sde.table_registry where upper(table_name) = pTableName and upper(owner) = pOwner;
   cursor cur_tables (pOwner in varchar2,pTableName in varchar2) is select owner,table_name from all_tables where owner = pOwner and table_name like pTableName;
   cursor cur_columns (pOwner in varchar2,pTableName in varchar2) is select column_name,data_type,data_length from all_tab_columns where owner = pOwner and table_name = pTableName and column_name not in 
                     ('ID','DIAGRAMCLASSID','DIAGRAMOBJECTID','SCHEMATICID','ISINITIAL','ISDISPLAYED','RELATIONOBJECTID','RELATIONCLASSID','DATASOURCEID','UCID','UOID'
                      ,'USID','UPDATESTATUS','SUBTYPE','INITIALX','INITIALY','INITIALPOSITION','REFERENCELINK'
                      ,'ROTATION','FROMTID','SCHEMATICID','TOTID','SCHEMATICID','FROMPORT','TOPORT','FLOWDIRECTION','SHAPE','UGUID') and data_type not in ('BLOB');
begin
--- Truncate table and reset sequence.
   open cur_reg ('EDGIS','EDSCHEM_GUIDTOTAL');
   fetch cur_reg into v_myRegID;
   close cur_reg;
   open cur_rowid ('EDGIS','EDSCHEM_GUIDTOTAL');
   fetch cur_rowid into v_myRowID;
   close cur_rowid;
   execute immediate 'TRUNCATE TABLE EDGIS.EDSCHEM_GUIDTOTAL';
   execute immediate 'ALTER SEQUENCE R'||v_MyRegID||' minvalue 0';
-- Insert data for each table
   for c_schematics in cur_schematics loop
       for c_tables in cur_tables(c_schematics.owner,'SCH'||c_schematics.id||'E_%') loop
           dbms_output.put_line ('Table:'||c_tables.table_name);
           open cur_layer (c_tables.owner,c_tables.table_name);
           fetch cur_layer into v_layerid;
           close cur_layer;
           open cur_reg (c_tables.owner,c_tables.table_name);
           fetch cur_reg into v_regid;
           close cur_reg;
           open cur_rowid (c_tables.owner,c_tables.table_name);
           fetch cur_rowid into v_rowid;
           close cur_rowid;
           v_columnCnt:=0;
           v_columns:='';
           v_columns2:='';
           for c_columns in cur_columns (c_tables.owner,c_tables.table_name) loop
              dbms_output.put_line ('Columns:'||length(v_columns));
              if v_columnCnt = 0 then
                  v_columns:='NVL('||c_columns.column_name||','||''''||''''||')';
                  v_columns2:=c_columns.column_name;
              else
                  v_columns:=v_columns||',NVL('||c_columns.column_name||','||''''||''''||')';
                  v_columns2:=v_columns2||','||c_columns.column_name;
              end if;
              v_columnCnt:=v_columnCnt+1;
           end loop;
           v_str:='select '||''''||c_tables.owner||''''||','||''''||c_tables.table_name||''''||','||v_regid;
           v_str:=v_str||',A.'||v_rowid ||',A.UGUID,F.EMINX,F.EMINY,F.EMAXX,F.EMAXY,EDGIS.R'||v_myRegID||'.NEXTVAL ';
           -- dbms_output.put_line (v_columns);
           if length(v_columns) > 0 then
              v_str:=v_str||','||v_columns;
           end if; 
           v_str:=v_str||' from ';
           v_str:=v_str||c_tables.owner||'.'||c_tables.table_name||' A, '||c_tables.owner||'.F'||v_layerid||' F WHERE A.SHAPE=F.FID';           
           -- dbms_output.put_line (v_str);
           if length(v_columns) > 0 then
              v_sql:= 'insert into edgis.EDSCHEM_GUIDTOTAL (OWNER,SCH_TABLE,UCID,UOID,UGUID,MINX,MINY,MAXX,MAXY,OID,'||v_columns2||') '||v_str||' and ISDISPLAYED<>0';
              dbms_output.put_line (v_sql);
              execute immediate v_sql;
           else
			  execute immediate 'insert into edgis.EDSCHEM_GUIDTOTAL (OWNER,SCH_TABLE,UCID,UOID,UGUID,MINX,MINY,MAXX,MAXY,OID) '||v_str||' and ISDISPLAYED <>0';
           end if;
       end loop;
       commit;
   end loop;
end;
/
