spool "D:\DQI_Scripts\Logs\SQL_POLE67890_DIVISION.txt"
SET SERVEROUTPUT ON
--set serveroutput on size 100000;
prompt Updating POLE intersect ;
select current_timestamp from dual ;

declare 


  v_div NVARCHAR2(64) :=NULL;

  version_name  NVARCHAR2(30) :='ME_POLE_STRUCT_67890_V3';
  v_code  NUMBER;
  v_errm  NVARCHAR2(64);
  p_new_value NVARCHAR2(64) :=null;
  p_sap_equip_id NVARCHAR2(64):=null ;
  p_sap_func_loc NVARCHAR2(64) :=null;
  p_old_vale NVARCHAR2(64) :=null; 
 
  counter2 NUMBER :=0;

    
begin

  sde.version_util.set_current_version('ME_POLE_STRUCT_67890_V3');                                   
  sde.version_user_ddl.edit_version('ME_POLE_STRUCT_67890_V3',1);
  DBMS_OUTPUT.PUT_LINE('Update Process started');

for cur in (select * from T_POLESTRUCTURE_evw WHERE objectid  like ('%6') or objectid  like ('%7') or objectid  like ('%8') or objectid  like ('%9') or objectid  like ('%0'))

loop
  
BEGIN
  select code into  v_div  from etgis.domain_lookup where UPPER(CODE_DESC) =(  select division from LD_division_1 where sde.st_intersects(shape,cur.shape) = 1  and ROWNUM<2) and   UPPER(feature_name)= 'DIVISION';
  if v_div <> cur.division then
  select sap_equip_id , Sap_Func_Loc_No , division into p_sap_equip_id,p_sap_func_loc, p_old_vale from t_polestructure_evw where  sap_equip_id=cur.sap_equip_id ;
  update  T_POLESTRUCTURE_evw set  division= v_div where objectid=cur.objectid ;
   counter2 :=counter2 +1;
  COMMIT;
  select  division into p_new_value from t_polestructure_evw  where sap_equip_id=cur.sap_equip_id;
  insert into  ME_SAP_DATA_LOG (sap_equip_id,Field_Name,Old_Value,Updated_Value,Version_Name) values (p_sap_equip_id,'POLE_DIVISION', p_old_vale,p_new_value,version_name);
  end if;
  commit;
  exception when others then
    -- DBMS_OUTPUT.PUT_LINE(i.sap_equip_id|| ' NOT FOUND ');
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE ('Error in Pole Division update where error is '  || v_code || ' ' || v_errm || ' for sap_equip_id ' || cur.sap_equip_id );
    continue;
     end;
  
 

  
  

end loop;

DBMS_OUTPUT.PUT_LINE(' Process Completed Sucessfully for ' || counter2 || 'record for division pole' );

sde.version_user_ddl.edit_version('ME_POLE_STRUCT_67890_V3',2);
commit;
sde.version_util.set_current_version('SDE.DEFAULT'); 
commit;
DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');

end;
/
prompt Updating POLE intersect FOR division with  objectid  like 67890 is complete;
select current_timestamp from dual;
spool off;