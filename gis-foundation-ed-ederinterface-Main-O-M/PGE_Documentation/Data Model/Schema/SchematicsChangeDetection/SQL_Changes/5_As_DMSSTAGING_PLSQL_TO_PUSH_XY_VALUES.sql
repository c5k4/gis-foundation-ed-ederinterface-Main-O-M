/*
create table xy_device_temp as select UGUID GLOBALID, min(eminx) x,min(eminy) y from EDGIS.PGE_SCHEMGUIDTOTAL where UGUID in (select GUID from dmsstaging.device) group by UGUID ;

SQL> desc xy_device_temp
 Name                                      Null?    Type
 ----------------------------------------- -------- ----------------------------
 GLOBALID                                           NVARCHAR2(38)
 X                                                  NUMBER
 Y                                                  NUMBER
create index device_guid_temp2 on xy_device_temp(globalid);
-- create or replace index device_guid_temp on device(guid);
update device d set (d.XD_2,d.YD_2)=(select X,Y from xy_device_temp sch where d.GUID=sch.globalid) where d.guid is not null and d.guid in (select globalid from xy_device_temp);
commit;
drop table xy_device_temp ;
create table xy_node_temp as select UGUID GLOBALID, min(eminx) x,min(eminy) y from EDGIS.PGE_SCHEMGUIDTOTAL where UGUID in (select GUID from node) group by UGUID ;
SQL> desc xy_node_temp
 Name                                      Null?    Type
 ----------------------------------------- -------- -------------
 GLOBALID                                           NVARCHAR2(38)
 X                                                  NUMBER
 Y                                                  NUMBER
create index device_guid_temp2 on xy_node_temp(globalid);
-- create index node_guid_temp on node(guid);
update node n set (n.XN_2,n.YN_2)=(select X,Y from xy_node_temp sch where n.GUID=sch.globalid) where n.guid is not null and n.guid in (select globalid from xy_node_temp);
commit;
drop table xy_node_temp ;
*/
create table xy_device_temp (
GLOBALID NVARCHAR2(38),
 X NUMBER,
 Y NUMBER);
create index device_guid_temp on device(guid);
create index device_guid_temp2 on xy_device_temp(globalid);

create table xy_node_temp (
GLOBALID NVARCHAR2(38),
 X NUMBER,
 Y NUMBER);
create index node_guid_temp on node(guid); 
create index node_guid_temp2 on xy_node_temp(globalid);

CREATE or replace PROCEDURE UPDATE_SCHEM_XY AS
sqlstr VARCHAR2(3000);
BEGIN
    sqlstr := 'truncate table dmsstaging.xy_device_temp' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr ;
	sqlstr := 'insert into dmsstaging.xy_device_temp(GLOBALID,X,Y) select a.UGUID, (min(a.minx)*100),(min(a.miny)*100) from EDGIS.PGE_SCHEMGUIDTOTAL a where a.UGUID in (select b.GUID from dmsstaging.device b) group by a.UGUID ' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr ;
	sqlstr := 'update dmsstaging.device d set (d.XD_2,d.YD_2)=(select X,Y from dmsstaging.xy_device_temp sch where d.GUID=sch.globalid) where d.guid is not null and d.guid in (select globalid from dmsstaging.xy_device_temp)' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr ;
	dbms_output.put_line('commiting...');
    commit;
	sqlstr := 'truncate table dmsstaging.xy_node_temp' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr ; 
	sqlstr := 'insert into dmsstaging.xy_node_temp (GLOBALID,X,Y) select UGUID GLOBALID, (min(minx)*100) x,(min(miny)*100) y from EDGIS.PGE_SCHEMGUIDTOTAL where UGUID in (select GUID from dmsstaging.node) group by UGUID ' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr ;
	sqlstr := 'update dmsstaging.node n set (n.XN_2,n.YN_2)=(select X,Y from dmsstaging.xy_node_temp sch where n.GUID=sch.globalid) where n.guid is not null and n.guid in (select globalid from dmsstaging.xy_node_temp) ' ;
	dbms_output.put_line(sqlstr);
	execute immediate sqlstr;
	commit;
END;
/ 
grant all on UPDATE_SCHEM_XY to public;
grant all on xy_node_temp to public;
grant all on xy_device_temp to public;
grant all on UPDATE_SCHEM_XY to gisinterface;
grant all on UPDATE_SCHEM_XY to gis_interface;
grant all on UPDATE_SCHEM_XY to gis_i ;
grant all on UPDATE_SCHEM_XY to sde_editor ;
grant all on UPDATE_SCHEM_XY to sde_viewer ;

grant all on xy_node_temp to gisinterface;
grant all on xy_node_temp to gis_interface;
grant all on xy_node_temp to gis_i ;
grant all on xy_node_temp to sde_editor ;
grant all on xy_node_temp to sde_viewer ;

grant all on xy_device_temp to gisinterface;
grant all on xy_device_temp to gis_interface;
grant all on xy_device_temp to gis_i ;
grant all on xy_device_temp to sde_editor ;
grant all on xy_device_temp to sde_viewer ;

/*
update device d set (d.XD_2,d.YD_2)=(
  select sch.X,sch.Y from (
         select st.UGUID GLOBALID, min(st.eminx) x,min(st.eminy) y from schematics_guid_temp st where st.UGUID in 
                     (select dr.GUID from device dr) group by st.UGUID 
	     ) sch  
   where d.GUID=sch.globalid  
   )
	where d.guid is not null 
	and d.guid in (select sgt.UGUID GLOBALID from schematics_guid_temp sgt where sgt.UGUID in (select dr3.GUID from device dr3) group by sgt.UGUID);
   commit;
   -- drop table xy_device_temp ;
   -- create table xy_node_temp as select UGUID GLOBALID, min(eminx) x,min(eminy) y from schematics_guid_temp where UGUID in (select GUID from node) group by UGUID ;
   -- create index device_guid_temp2 on xy_node_temp(globalid);
-- create index node_guid_temp on node(guid);
   -- update node n set (n.XN_2,n.YN_2)=(select X,Y from xy_node_temp sch where n.GUID=sch.globalid) where n.guid is not null and n.guid in (select globalid from xy_node_temp);
   -- commit;
   -- drop table xy_node_temp ; 
   
*/

