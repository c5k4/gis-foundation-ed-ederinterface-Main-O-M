set serveroutput on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
set timing on
select sysdate from dual;
set linesize 160
set pagesize 50000
column SOURCE_FC_NAME format A45
column DEST_FC_NAME format A45
drop table temp_table_devicegroup_map ;
create table temp_table_devicegroup_map(DEVICEGROUPGUID char(38),DEVICEGUID char(38),CIRCUITID NVARCHAR2(9));

set serveroutput on
set timing on
DECLARE
   cursor push_feeder_type_list is select SOURCEFC.PHYSICALNAME SOURCE_FC_NAME,REL_CLASS.REL_ID, DESTFC.PHYSICALNAME DEST_FC_NAME
from (
  SELECT gi.UUID,gi.PHYSICALNAME 
  from sde.gdb_items gi 
  where gi.physicalname ='EDGIS.DEVICEGROUP'
) SOURCEFC 
join
( 
select SOURCE_ID,DEST_ID,REL_ID from 
(
  select a.ORIGINID SOURCE_ID, b.ORIGINID DEST_ID, a.UUID REL_ID
  from 
  (
  select UUID,ORIGINID,DESTID
    from sde.gdb_itemrelationships
	where ORIGINID in
	(
	 SELECT gi.UUID 
     from sde.gdb_items gi 
     where gi.physicalname ='EDGIS.DEVICEGROUP'
	)
	and DESTID in 
    (	
	  select typechecka.UUID from SDE.GDB_ITEMS typechecka where typechecka.TYPE in (select UUID from SDE.GDB_ITEMTYPES where NAME='Relationship Class')
    ) 
  ) a
  left outer join 
  (
  select UUID,ORIGINID,DESTID
    from sde.gdb_itemrelationships
	where ORIGINID in
	(
	 SELECT gi2.UUID 
     from sde.gdb_items gi2 
     where gi2.physicalname in
      (select 
	   cr5.owner||'.'||cr5.table_name 
	   from sde.column_registry cr5 
	   where cr5.table_name<>'CIRCUITSOURCE'
       and   cr5.table_name<>'DEVICEGROUP'	   
	   and   cr5.table_name in (
	         select cr4.table_name 
			 from sde.column_registry cr4 
			 where cr4.column_name='CIRCUITID'
			 )
       and cr5.table_name in (
	         select cr4.table_name 
			 from sde.column_registry cr4 
			 where cr4.column_name='CIRCUITID2'
			 )			 
	   )
	 )
   and DESTID in 
    (	
      select typechecka.UUID from SDE.GDB_ITEMS typechecka where typechecka.TYPE in (select UUID from SDE.GDB_ITEMTYPES where NAME='Relationship Class')
    ) 
  ) b on 
  a.DESTID=b.DESTID
)  
group by SOURCE_ID,DEST_ID,REL_ID
) REL_CLASS
 on REL_CLASS.SOURCE_ID=SOURCEFC.UUID
join 
(
SELECT gi2.UUID,gi2.PHYSICALNAME 
  from sde.gdb_items gi2 
  where gi2.physicalname in
      (select 
	   cr5.owner||'.'||cr5.table_name 
	   from sde.column_registry cr5 
	   where cr5.table_name<>'CIRCUITSOURCE'
       and cr5.table_name<>'DEVICEGROUP'
       and cr5.table_name<>'VOLTAGEREGULATOR'	   
	   and cr5.table_name in (
	         select cr4.table_name 
			 from sde.column_registry cr4 
			 where cr4.column_name='CIRCUITID'
			 ) 
		and cr5.table_name in (
	         select cr4.table_name 
			 from sde.column_registry cr4 
			 where cr4.column_name='CIRCUITID2'
			 ) 	 
	  )
) DESTFC 
on REL_CLASS.DEST_ID=DESTFC.UUID
order by 1,2,3;
sql_stmt varchar2(2000);
feedertype_num number;
BEGIN
FOR tab_obj IN push_feeder_type_list LOOP
  sql_stmt:='insert into temp_table_devicegroup_map (DEVICEGROUPGUID,DEVICEGUID,CIRCUITID) select src.GLOBALID,dest.GLOBALID,dest.CIRCUITID from '||tab_obj.SOURCE_FC_NAME||' src join (select * from '||tab_obj.DEST_FC_NAME||' where structureguid is not null and circuitid is not null) dest on src.globalid=dest.structureguid where dest.circuitid is not null group by src.GLOBALID,dest.GLOBALID,dest.CIRCUITID';
  dbms_output.put_line(sql_stmt);
  execute immediate sql_stmt;
  sql_stmt:='insert into temp_table_devicegroup_map (DEVICEGROUPGUID,DEVICEGUID,CIRCUITID) select src.GLOBALID,dest.GLOBALID,dest.CIRCUITID2 from '||tab_obj.SOURCE_FC_NAME||' src join (select * from '||tab_obj.DEST_FC_NAME||' where structureguid is not null and circuitid2 is not null) dest on src.globalid=dest.structureguid where dest.circuitid2 is not null group by src.GLOBALID,dest.GLOBALID,dest.CIRCUITID2';
  dbms_output.put_line(sql_stmt);
  execute immediate sql_stmt;
  commit;
END LOOP;
END;
/
insert into temp_table_devicegroup_map (DEVICEGROUPGUID,DEVICEGUID,CIRCUITID) select src.GLOBALID,dest.GLOBALID,dest.CIRCUITID from EDGIS.DEVICEGROUP src join (select * from EDGIS.VOLTAGEREGULATOR where DEVICEGROUPGUID is not null and circuitid is not null) dest on src.globalid=dest.DEVICEGROUPGUID where dest.circuitid is not null group by src.GLOBALID,dest.GLOBALID,dest.CIRCUITID ;
commit;
insert into temp_table_devicegroup_map (DEVICEGROUPGUID,DEVICEGUID,CIRCUITID) select src.GLOBALID,dest.GLOBALID,dest.CIRCUITID2 from EDGIS.DEVICEGROUP src join (select * from EDGIS.VOLTAGEREGULATOR where DEVICEGROUPGUID is not null and circuitid2 is not null) dest on src.globalid=dest.DEVICEGROUPGUID where dest.circuitid2 is not null group by src.GLOBALID,dest.GLOBALID,dest.CIRCUITID2 ;
commit;

select count(*) from temp_table_devicegroup_map ;
select count(*) from temp_table_devicegroup_map where circuitid is null;
select count(*) from (select devicegroupguid,deviceguid,circuitid from temp_table_devicegroup_map group by devicegroupguid,deviceguid,circuitid);
select max(count(*)) from temp_table_devicegroup_map group by devicegroupguid,circuitid;

drop table temp_update_devicegroup ;
create table temp_update_devicegroup(DEVICEGROUPGUID char(38),CIRCUITID NVARCHAR2(9),cnt_ids number, thisone number);
insert into temp_update_devicegroup(DEVICEGROUPGUID,CIRCUITID,cnt_ids,thisone) select  devicegroupguid,circuitid,count(*),0 from temp_table_devicegroup_map group by devicegroupguid,circuitid ;
commit;
update temp_update_devicegroup set thisone=1 where (DEVICEGROUPGUID,circuitid,cnt_ids) in ( 
select tud.DEVICEGROUPGUID,min(tud.circuitid), tud.cnt_ids from temp_update_devicegroup tud where 
(tud.DEVICEGROUPGUID,tud.cnt_ids) in (select DEVICEGROUPGUID,max(cnt_ids) from temp_update_devicegroup group by DEVICEGROUPGUID ) 
group by tud.DEVICEGROUPGUID,tud.cnt_ids);

select count(*) from temp_update_devicegroup;
select count(*) from (select devicegroupguid from temp_update_devicegroup group by DEVICEGROUPGUID);
select count(*) from temp_update_devicegroup where thisone=1;
select count(*) from (select devicegroupguid from temp_update_devicegroup where thisone=1 group by DEVICEGROUPGUID having count(*)>1);
create index temp_idx_tgug_thisone on temp_update_devicegroup(thisone);
create index temp_idx_tgug_circuitid on temp_update_devicegroup(CIRCUITID);

select count(*) from edgis.devicegroup where circuitid is null;

MERGE INTO edgis.devicegroup dg
USING (
    select dg1.rowid as rid, td.DEVICEGROUPGUID,td.CIRCUITID from edgis.devicegroup dg1 inner join (select * from temp_update_devicegroup where thisone=1) td on dg1.globalid=TD.devicegroupguid where dg1.circuitid is null
) TUGD
on (dg.rowid=tugd.rid)
when matched then update set dg.circuitid=TUGD.circuitid;

select count(*) from edgis.devicegroup where circuitid is null;
	
drop table temp_update_devicegroup ;
drop table temp_table_devicegroup_map ;

select sysdate from dual;