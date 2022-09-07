spool "D:\DQI_Scripts\Logs\SQL_TOWER_COUNTY.txt"
SET SERVEROUTPUT ON
--set serveroutput on size 100000;
prompt Updating tower intersect FOR COUNTY ;
select current_timestamp from dual ;

declare 

  v_cnt NVARCHAR2(64) :=NULL;

  version_name  NVARCHAR2(30) :='ME_TOWER_STRUCT_V3';
  v_code  NUMBER;
  v_errm  NVARCHAR2(64);
  p_new_value NVARCHAR2(64) :=null;
  p_sap_equip_id NVARCHAR2(64):=null ;
  p_sap_func_loc NVARCHAR2(64) :=null;
  p_old_vale NVARCHAR2(64) :=null; 
  counter1 number :=0;

    
begin
  sde.version_user_ddl.create_version ('sde.DEFAULT',version_name,sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'masss update et');
  sde.version_util.set_current_version('ME_TOWER_STRUCT_V3');                                   
  sde.version_user_ddl.edit_version('ME_TOWER_STRUCT_V3',1);
  DBMS_OUTPUT.PUT_LINE('Update Process started');

for cur in (select * from t_towerstructure_evw)

loop

BEGIN
  --select sap_equip_id , Sap_Func_Loc_No , county  into p_sap_equip_id,p_sap_func_loc, p_old_vale from t_towerstructure_evw 
  --where  Sap_Equip_Id=cur.sap_equip_id ;
  

  select code into  v_cnt  from etgis.domain_lookup where CODE_DESC =(  select CNTY_name 
  from LD_COUNTY_1 where sde.st_intersects(shape,cur.shape) = 1 and ROWNUM<2) and  UPPER(feature_name)= 'COUNTY'  ;
  
  if v_cnt <> cur.county then
  select sap_equip_id , Sap_Func_Loc_No , county  into p_sap_equip_id,p_sap_func_loc, p_old_vale from t_towerstructure_evw 
  where  Sap_Equip_Id=cur.sap_equip_id ;
  update  t_towerstructure_evw set  county= v_cnt where objectid=cur.objectid ;
  counter1 := counter1 +1;
  COMMIT;
  
  select  county into p_new_value from t_towerstructure_evw  where sap_equip_id=cur.sap_equip_id;
  

   insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name)
 values (p_sap_equip_id,'TOWER_COUNTY', p_old_vale,p_new_value,version_name);
  end if;
commit;
  
  exception when others then
    -- DBMS_OUTPUT.PUT_LINE(i.sap_equip_id|| ' NOT FOUND ');
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE ('Error in TOWER county update where error is '  || v_code || ' ' || v_errm || ' for sap_equip_id' || cur.sap_equip_id);
    continue;
     end;
     
     
end loop;

DBMS_OUTPUT.PUT_LINE(' Process Completed Sucessfully for ' || counter1 || 'record for county TOWER ' );


sde.version_user_ddl.edit_version('ME_TOWER_STRUCT_V3',2);
commit;
sde.version_util.set_current_version('SDE.DEFAULT'); 
commit;
DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');
end;
/
prompt Updating TOWER intersect FOR county  is complete;
select current_timestamp from dual;
spool off;