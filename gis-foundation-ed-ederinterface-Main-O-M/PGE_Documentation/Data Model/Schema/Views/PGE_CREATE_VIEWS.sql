--version 1.3, added electricstitchpoint

set serveroutput on
DECLARE
CURSOR view_tables IS 
  SELECT registration_id, rowid_column,table_name,owner,SUBSTR(table_name,1,20) As table_name_trunc FROM sde.table_registry 
  WHERE owner = USER and TABLE_NAME in (
  'CAPACITORBANK', 'CONDUITSYSTEM', 'DCRECTIFIER', 'DELIVERYPOINT', 'DEVICEGROUP',
  'DYNAMICPROTECTIVEDEVICE', 'ELECTRICSTITCHPOINT', 'FAULTINDICATOR', 'FUSE', 'NEUTRALCONDUCTOR',
  'PRIMARYGENERATION', 'PRIOHCONDUCTOR', 'PRIUGCONDUCTOR', 'SECOHCONDUCTOR',
  'SECONDARYGENERATION', 'SECUGCONDUCTOR', 'STEPDOWN', 'STREETLIGHT', 'SUBSTATION',
  'SUBSURFACESTRUCTURE', 'SUPPORTSTRUCTURE', 'SWITCH', 'TRANSFORMER', 'VOLTAGEREGULATOR', 'DCDEVICE', 'NETWORKPROTECTOR'
  )  ORDER BY registration_id;
sqlstm VARCHAR2(1024);
row_cnt NUMBER;
tb_name VARCHAR2(32);
BEGIN
dbms_output.put_line('If any A Tables will be checked for '||USER||' they will be listed here: ');
  FOR table_info IN view_tables LOOP
    dbms_output.put_line('creating view on TABLE: '||table_info.table_name||' and TABLE: A'||table_info.registration_id);
    SELECT COUNT(*) INTO row_cnt FROM user_tables where TABLE_NAME = 'A'||table_info.registration_id;
    IF row_cnt = 1 then
        sqlstm := 'create or replace view ZPGEVW_'||table_info.table_name_trunc||' as (SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS IN  (1,2,3,5,30) minus select d_table.SDE_DELETES_ROW_ID from '||table_info.owner||
			'.D'||table_info.registration_id||' d_table ) union SELECT A_Table.OBJECTID FROM (SELECT ObjectID,STATUS from '||table_info.owner||
			'.A'||table_info.registration_id||' where (objectid,sde_state_id) in ( SELECT objectid, max(sde_state_id)  from '||table_info.owner||
			'.A'||table_info.registration_id||' group by objectid ) and STATUS IN (1,2,3,5,30) ) A_Table';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
        sqlstm := 'create or replace view ZPGEVWP_'||table_info.table_name_trunc||' as (SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS=0  minus select d_table.SDE_DELETES_ROW_ID from '||table_info.owner||
			'.D'||table_info.registration_id||' d_table )  union SELECT A_Table.OBJECTID FROM (SELECT ObjectID,STATUS from '||table_info.owner||
			'.A'||table_info.registration_id||' where (objectid,sde_state_id) in ( SELECT objectid, max(sde_state_id)  from '||table_info.owner||
			'.A'||table_info.registration_id||' group by objectid ) and STATUS=0 ) A_Table';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
    ELSE
        sqlstm := 'create or replace view ZPGEVW_'||table_info.table_name_trunc||' as SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS IN  (1,2,3,5,30)';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
        sqlstm := 'create or replace view ZPGEVWP_'||table_info.table_name_trunc||' as SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS=0';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
    END IF;
    sqlstm := 'select count(*) from ZPGEVW_'||table_info.table_name_trunc||' ';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||' to sde_viewer';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||' to sde_editor';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||' to public';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'select count(*) from ZPGEVWP_'||table_info.table_name_trunc||' ';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||' to sde_viewer';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||' to sde_editor';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||' to public';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
  END LOOP;
END;
/

DECLARE
CURSOR view_tables_50scale IS 
  SELECT registration_id, rowid_column,table_name,owner,SUBSTR(table_name,1,20) As table_name_trunc FROM sde.table_registry 
  WHERE owner = USER and TABLE_NAME in (
  'CAPACITORBANK', 'CONDUITSYSTEM', 'DCRECTIFIER', 'DELIVERYPOINT', 'DEVICEGROUP',
  'DYNAMICPROTECTIVEDEVICE', 'ELECTRICSTITCHPOINT', 'FAULTINDICATOR', 'FUSE', 'NEUTRALCONDUCTOR', 
  'PRIMARYGENERATION', 'PRIOHCONDUCTOR', 'PRIUGCONDUCTOR', 'SECOHCONDUCTOR',
  'SECONDARYGENERATION', 'SECUGCONDUCTOR', 'STEPDOWN', 'STREETLIGHT', 'SUBSTATION',
  'SUBSURFACESTRUCTURE', 'SUPPORTSTRUCTURE', 'SWITCH', 'TRANSFORMER', 'VOLTAGEREGULATOR', 'DCCONDUCTOR', 
  'DCDEVICE', 'NETWORKPROTECTOR'
  )  ORDER BY registration_id;
sqlstm VARCHAR2(1024);
row_cnt NUMBER;
tb_name VARCHAR2(32);
BEGIN
dbms_output.put_line('If any A Tables will be checked for '||USER||' they will be listed here: ');
  FOR table_info IN view_tables_50scale LOOP
    dbms_output.put_line('creating view on TABLE: '||table_info.table_name||' and TABLE: A'||table_info.registration_id);
    SELECT COUNT(*) INTO row_cnt FROM user_tables where TABLE_NAME = 'A'||table_info.registration_id;
    IF row_cnt = 1 then
        sqlstm := 'create or replace view ZPGEVW_'||table_info.table_name_trunc||'50 as (SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS IN  (1,2,3,5,30) minus select d_table.SDE_DELETES_ROW_ID from '||table_info.owner||
			'.D'||table_info.registration_id||' d_table ) union SELECT A_Table.OBJECTID FROM (SELECT ObjectID,STATUS from '||table_info.owner||
			'.A'||table_info.registration_id||' where (objectid,sde_state_id) in ( SELECT objectid, max(sde_state_id)  from '||table_info.owner||
			'.A'||table_info.registration_id||' group by objectid ) and STATUS IN (1,2,3,5,30) ) A_Table';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
        sqlstm := 'create or replace view ZPGEVWP_'||table_info.table_name_trunc||'50 as (SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS=0  minus select d_table.SDE_DELETES_ROW_ID from '||table_info.owner||
			'.D'||table_info.registration_id||' d_table )  union SELECT A_Table.OBJECTID FROM (SELECT ObjectID,STATUS from '||table_info.owner||
			'.A'||table_info.registration_id||' where (objectid,sde_state_id) in ( SELECT objectid, max(sde_state_id)  from '||table_info.owner||
			'.A'||table_info.registration_id||' group by objectid ) and STATUS=0 ) A_Table';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
    ELSE
        sqlstm := 'create or replace view ZPGEVW_'||table_info.table_name_trunc||'50 as SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS IN  (1,2,3,5,30)';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
        sqlstm := 'create or replace view ZPGEVWP_'||table_info.table_name_trunc||'50 as SELECT OBJECTID FROM '||table_info.owner||
			'.'||table_info.table_name||' WHERE STATUS=0';
        dbms_output.put_line(sqlstm);
        EXECUTE IMMEDIATE sqlstm;
    END IF;
    sqlstm := 'select count(*) from ZPGEVW_'||table_info.table_name_trunc||'50 ';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||'50 to sde_viewer';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||'50 to sde_editor';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVW_'||table_info.table_name_trunc||'50 to public';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'select count(*) from ZPGEVWP_'||table_info.table_name_trunc||'50 ';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||'50 to sde_viewer';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||'50 to sde_editor';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
    sqlstm := 'grant all on ZPGEVWP_'||table_info.table_name_trunc||'50 to public';
    dbms_output.put_line(sqlstm);
    EXECUTE IMMEDIATE sqlstm;
  END LOOP;
END;
/

CREATE OR REPLACE VIEW "EDGIS"."ZPGEVW_TRANSFORMER_OUTAGE" AS SELECT OBJECTID,GLOBALID,CGC12,SHAPE FROM edgis.zz_mv_transformer;
GRANT ALL ON "EDGIS"."ZPGEVW_TRANSFORMER_OUTAGE" TO SDE_EDITOR,SDE_VIEWER,MM_USER,MM_ADMIN,GIS_I,GIS_I_WRITE;

