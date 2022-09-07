--------------------------------------------------------
--  DDL for Procedure UPDATEFEATINFOTEMPTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."UPDATEFEATINFOTEMPTABLE" (temp_table_name IN VARCHAR2,index_name in VARCHAR2,startInsert in number,endInsert in number) AS
--- get a list of feature classes used in the network for lines.
cursor net_fcids is 
select g.physicalname,n1.fcid from (select OBJECTID,PHYSICALNAME from sde.gdb_items where PHYSICALNAME in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='FEEDERINFO')) g
inner join (
select distinct n3.userclassid "FCID" from edgis.N_1_DESC n3 
where 
(n3.elementtype,n3.eid)in(select TO_FEATURE_TYPE,TO_FEATURE_EID from EDGIS.PGE_ElecDistNetwork_Trace)
) n1 on n1.FCID=g.objectid ;
cursor net_fcids_no_feederinfo is 
select g.physicalname,n1.fcid from (select OBJECTID,PHYSICALNAME from sde.gdb_items where PHYSICALNAME not in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='FEEDERINFO')) g
inner join (
select distinct n3.userclassid "FCID" from edgis.N_1_DESC n3 
where 
(n3.elementtype,n3.eid)in(select TO_FEATURE_TYPE,TO_FEATURE_EID from EDGIS.PGE_ElecDistNetwork_Trace)
) n1 on n1.FCID=g.objectid ;
rowcnt number;
rowcurrent number;
sqlstmt varchar2(2000);
BEGIN
  sqlstmt := 'select count(*) from cat where table_name='''||temp_table_name||''' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'drop table '||temp_table_name;
                execute immediate sqlstmt;
  END IF;
  sqlstmt:= 'Create table '||temp_table_name||'(FEEDERFEDBY NVARCHAR2(9),FEEDERID NVARCHAR2(9),FROM_FEATURE_EID NUMBER(38),TO_FEATURE_EID NUMBER(38),TO_FEATURE_GLOBALID CHAR(38),TO_FEATURE_FCID NUMBER(38),TO_FEATURE_SCHEM_FCID NUMBER(38), TO_FEATURE_FEEDERINFO NUMBER(38),TO_FEATURE_TYPE NUMBER(1),ORDER_NUM NUMBER(38),MIN_BRANCH NUMBER(38),MAX_BRANCH NUMBER(38),TREELEVEL NUMBER(38))';
  execute immediate sqlstmt;  
  sqlstmt := 'insert into '||temp_table_name||' (select FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_SCHEM_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL from (select FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_SCHEM_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL,rownum as rowCounter from EDGIS.PGE_ElecDistNetwork_Trace order by feederid,order_num) where rowCounter >= ' || startInsert || ' AND rowCounter < ' || endInsert || ')';
  execute immediate sqlstmt;    
  COMMIT;
  --Create the following index to aid in the performance of this procedure
  execute immediate 'create index '||index_name||' on '||temp_table_name||'(TO_FEATURE_EID,TO_FEATURE_TYPE)';
  dbms_stats.gather_table_stats('EDGIS',temp_table_name);
  sqlstmt := 'MERGE INTO    '||temp_table_name||' t
  USING   (
        SELECT  t1.rowid AS rid, t2.userclassid "FCID"
        FROM    '||temp_table_name||' t1
        inner JOIN    N_1_DESC t2
        ON      t2.eid=t1.TO_FEATURE_EID and t2.elementtype=t1.to_feature_type
        ) v
   ON      (t.rowid = v.rid)
   WHEN MATCHED THEN
  UPDATE
  SET     t.TO_FEATURE_FCID = v.FCID';
  dbms_output.put_line('runing :'||sqlstmt);
  execute immediate sqlstmt;
  commit; 
  execute immediate 'drop index '||index_name||' ';
  
  --Create the following index to aid in the performance of this procedure
  execute immediate 'create index '||index_name||' on '||temp_table_name||'(TO_FEATURE_FCID,TO_FEATURE_TYPE,TO_FEATURE_EID)';
  dbms_stats.gather_table_stats('EDGIS',temp_table_name);
  FOR CLASS_NAME in net_fcids LOOP
    --rowcnt:= 0;
                                sqlstmt := 'select count(*) from '||temp_table_name||' where TO_FEATURE_FCID='||class_name.fcid||'';
                                execute immediate sqlstmt into rowcnt ;
                                if rowcnt>0 then
                sqlstmt := 'MERGE INTO '||temp_table_name||' t USING ( SELECT  t1.rowid AS rid, t3.GLOBALID,t3.FEEDERINFO FROM '||temp_table_name||' t1 inner JOIN N_1_DESC t2';
                                                                sqlstmt := sqlstmt||' ON t2.eid=t1.TO_FEATURE_EID and t2.elementtype=t1.to_feature_type';
                                                                sqlstmt := sqlstmt||' inner join '||class_name.physicalname||' t3 on t3.objectid=t2.userid where t1.TO_FEATURE_FCID='||CLASS_NAME.FCID||') v';
                                                                sqlstmt := sqlstmt||' ON (t.rowid = v.rid) WHEN MATCHED THEN UPDATE SET TO_FEATURE_GLOBALID=v.GLOBALID,t.TO_FEATURE_FEEDERINFO=v.FEEDERINFO';
                --dbms_output.put_line(sqlstmt);
                execute immediate sqlstmt;
                                                                commit;
                                END IF;
   END LOOP; 
   FOR CLASS_NAME in net_fcids_no_feederinfo LOOP
    --rowcnt:= 0;
                                sqlstmt := 'select count(*) from '||temp_table_name||' where TO_FEATURE_FCID='||class_name.fcid||'';
                                execute immediate sqlstmt into rowcnt ;
                                if rowcnt>0 then
                sqlstmt := 'MERGE INTO '||temp_table_name||' t USING ( SELECT  t1.rowid AS rid, t3.GLOBALID FROM '||temp_table_name||' t1 inner JOIN N_1_DESC t2';
                                                                sqlstmt := sqlstmt||' ON t2.eid=t1.TO_FEATURE_EID and t2.elementtype=t1.to_feature_type';
                                                                sqlstmt := sqlstmt||' inner join '||class_name.physicalname||' t3 on t3.objectid=t2.userid where t1.TO_FEATURE_FCID='||CLASS_NAME.FCID||') v';
                                                                sqlstmt := sqlstmt||' ON (t.rowid = v.rid) WHEN MATCHED THEN UPDATE SET TO_FEATURE_GLOBALID=v.GLOBALID';
                dbms_output.put_line(sqlstmt);
                execute immediate sqlstmt;
                                                                commit;
                                END IF;
   END LOOP;    
   execute immediate 'drop index '||index_name||' '; 
 sqlstmt := 'update '||temp_table_name||' set to_feature_schem_fcid = 
 DECODE(to_feature_fcid, 
           1008,4525,    /*Capacitor Bank*/   
           1007,4517,    /*DCRectifier*/
           1016,4498,    /*Delivery Point*/
           1019,4512,    /*Dist Busbar*/
           998,4513,     /*DynamicProtectiveDevice*/
           18720,4521,    /*Network Junctions*/
           997,4510,     /*ElectricStitchPoint*/
           1004,4523,    /*FaultIndicator*/
           1003,4499,     /*FUSE*/
           1010,4514,    /*OPENPOINT*/
           1013,4500,    /*PrimaryGeneration*/
           1014,4508,    /*PrimaryMeter*/    
           3849,4509,   /*PRIMARYRISER*/
           1023,4504,   /*PRIOHCONDUCTOR*/
           1021,4529,   /*PRIUGCONDUCTOR*/
           1024,4522,   /*SECOHCONDUCTOR*/
           1015,4524,   /*SECONDARYGENERATION*/
           999,4501,    /*SECONDARYLOADPOINT*/
           1022,4515,   /*SECUGCONDUCTOR*/
           1012,4507,   /*SERVICELOCATION*/
           1009,4502,   /*SMARTMETERNETWORKDEVICE*/
           1002,4528,   /*STEPDOWN*/
           1011,4516,   /*STREETLIGHT*/
           1005,4505,   /*SWITCH*/
           1006,4519,   /*TIE*/
           1001,4526,   /*TRANSFORMER*/
           1025,4511,   /*TRANSFORMERLEAD*/ 
           1000,4518)';   /*VoltageRegulator*/
 dbms_output.put_line(sqlstmt);
 execute immediate sqlstmt; 
 commit;
END;
