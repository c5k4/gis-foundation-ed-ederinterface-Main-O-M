--------------------------------------------------------
--  DDL for Procedure PGE_INT_DATA_BACKUP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "INTDATAARCH"."PGE_INT_DATA_BACKUP" (v_processname IN VARCHAR2)
  AS
  
   v_fileds NVARCHAR2(2000);
  
BEGIN
  FOR CUR_tx IN (SELECT *  FROM PGE_INT_DATA_RTNTN where ProcessName=v_processname ) 
    LOOP  
  BEGIN
      --1. Insert data from table1 to table 2 based on where clause in configinfo table  
  if CUR_tx.dblink is not null then
          IF v_processname = 'DMS'  THEN
           
             SELECT LISTAGG(COLUMN_NAME, ', ') into v_fileds FROM ALL_TAB_COLUMNS@DBLINKEDGMC
             WHERE  OWNER = 'DMSSTAGING' AND TABLE_NAME =substr(upper(CUR_tx.FROMtable),instr(upper(CUR_tx.FROMtable),'.')+1);
             
              dbms_output.put_line(CUR_tx.dblink);
              
              
          ElsIf v_processname = 'ED08'  THEN
           
             SELECT LISTAGG(COLUMN_NAME, ', ') into v_fileds FROM ALL_TAB_COLUMNS@DBLINKEDGMC
             WHERE  OWNER = 'PGEDATA' AND TABLE_NAME =substr(upper(CUR_tx.FROMtable),instr(upper(CUR_tx.FROMtable),'.')+1);
             
              dbms_output.put_line(CUR_tx.dblink);     
             
          ELSE
            
              SELECT LISTAGG(COLUMN_NAME, ', ') into v_fileds FROM ALL_TAB_COLUMNS@DBLINKEDER 
              WHERE  TABLE_NAME =substr(upper(CUR_tx.FROMtable),instr(upper(CUR_tx.FROMtable),'.')+1);
               dbms_output.put_line(CUR_tx.dblink);
          end if;
          
          v_fileds := replace(v_fileds,' ORDER,',' "ORDER",');
          
         --YXA6 - Add condition for GDBM -To move only those records where status='C'
          IF CUR_tx.WH_FOR_INSERTION is not null  THEN
            EXECUTE immediate 'insert into '|| CUR_tx.totable || '(' ||v_fileds || ') select ' || v_fileds || 
          ' from ' || CUR_tx.FROMtable || '@' ||CUR_tx.dblink || '  where ' ||  CUR_tx.WH_FOR_INSERTION ; 
          else
          
          EXECUTE immediate 'insert into '|| CUR_tx.totable || '(' ||v_fileds || ') select ' || v_fileds || 
          ' from ' || CUR_tx.FROMtable || '@' ||CUR_tx.dblink;   
          
          end if;
         
           --M4JF
          EXECUTE immediate 'UPDATE '|| CUR_tx.totable || ' SET BACKUPDATE = SYSDATE WHERE BACKUPDATE IS NULL';
          COMMIT;
  else
          SELECT LISTAGG(COLUMN_NAME, ', ') into v_fileds FROM ALL_TAB_COLUMNS WHERE 
          TABLE_NAME =substr(upper(CUR_tx.FROMtable),instr(upper(CUR_tx.FROMtable),'.')+1);
          
           IF CUR_tx.WH_FOR_INSERTION is not null  THEN
              EXECUTE immediate 'insert into '|| CUR_tx.totable || '(' ||v_fileds || ') select ' || v_fileds || 
              ' from ' || CUR_tx.FROMtable || '  where ' ||  CUR_tx.WH_FOR_INSERTION ; 
          else
          
             EXECUTE immediate 'insert into '|| CUR_tx.totable || '(' ||v_fileds || ') select ' || v_fileds || ' from ' || CUR_tx.FROMtable;    
          
          end if;
          
          
        
          EXECUTE immediate 'UPDATE '|| CUR_tx.totable || ' SET BACKUPDATE = SYSDATE WHERE BACKUPDATE IS NULL';
          COMMIT;
end if;
  dbms_output.put_line('process completed 1');
  
  --2. Delete data from source table:table1 based on deletesourcedata
  
  if  CUR_tx.TRUNCATEDATA = 'Y' then
    if CUR_tx.dblink is not null then
      if CUR_tx.WH_FOR_DELETION is not null then 
        EXECUTE immediate 'delete from ' ||CUR_tx.FROMtable || '@' ||CUR_tx.dblink || ' where  ('  || CUR_tx.WH_FOR_DELETION  || ') and 
          ( ' ||  CUR_tx.PRIMARYKEY || ' in (select ' ||  CUR_tx.PRIMARYKEY || ' from ' || CUR_tx.totable  || '))' ; 
      else
        EXECUTE immediate 'delete from ' ||CUR_tx.FROMtable || '@' ||CUR_tx.dblink || '  where ' ||  CUR_tx.PRIMARYKEY || ' in 
        (select ' ||  CUR_tx.PRIMARYKEY || ' from ' || CUR_tx.totable  || ')' ;     
        commit;
      end if;
    else
      if CUR_tx.WH_FOR_DELETION is not null then
          EXECUTE immediate 'delete from ' ||CUR_tx.FROMtable || ' where  ('  || CUR_tx.WH_FOR_DELETION  || ') and 
          ( ' ||  CUR_tx.PRIMARYKEY || ' in (select ' ||  CUR_tx.PRIMARYKEY || ' from ' || CUR_tx.totable  || '))' ;   
      else
        EXECUTE immediate 'delete from ' || CUR_tx.FROMtable || '  where ' ||  CUR_tx.PRIMARYKEY || ' in 
        (select ' ||  CUR_tx.PRIMARYKEY || ' from ' || CUR_tx.totable  || ')' ;    
        commit;
      end if;
    end if;
end if;
 dbms_output.put_line('process completed 2');  
  --3. Delete data from target table:table2 based on retentionperiod(retentionperiod is no of days)
   execute immediate 'delete FROM ' || CUR_tx.totable || ' WHERE Backupdate < sysdate - ' || CUR_tx.retentionperiod ;
   dbms_output.put_line('process completed 3');  
  commit;

         

EXCEPTION WHEN OTHERS THEN
dbms_output.put_line('ERROR:' || CUR_tx.FROMTABLE); 
end;
END LOOP;



END PGE_INT_DATA_BACKUP;
