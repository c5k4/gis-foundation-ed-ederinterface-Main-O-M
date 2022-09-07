spool "D:\DQI_Scripts\Logs\SQL_POLE67890_COUNTY.txt"
SET SERVEROUTPUT ON
--set serveroutput on size 100000;
prompt Updating POLE intersect for county objectid like 67890  ;
select current_timestamp from dual ;

declare 

  v_cnt NVARCHAR2(64) :=NULL;

  version_name  NVARCHAR2(30) :='ME_POLE_STRUCT_67890_V3';
  v_code  NUMBER;
  v_errm  NVARCHAR2(64);
  p_new_value NVARCHAR2(64) :=null;
  p_sap_equip_id NVARCHAR2(64):=null ;
  p_sap_func_loc NVARCHAR2(64) :=null;
  p_old_vale NVARCHAR2(64) :=null; 
  counter1 number :=0;

    
begin
  sde.version_user_ddl.create_version ('sde.DEFAULT',version_name,sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'masss update et');
  sde.version_util.set_current_version('ME_POLE_STRUCT_67890_V3');                                   
  sde.version_user_ddl.edit_version('ME_POLE_STRUCT_67890_V3',1);
  DBMS_OUTPUT.PUT_LINE('Update Process started');

for cur in (select * from T_POLESTRUCTURE_evw WHERE objectid  like ('%6') or objectid  like ('%7') or objectid  like ('%8') or objectid  like ('%9') or objectid  like ('%0'))

loop

BEGIN
 
  

  select code into  v_cnt  from etgis.domain_lookup where CODE_DESC =(  select CNTY_name from LD_COUNTY_1 where sde.st_intersects(shape,cur.shape) = 1 and ROWNUM<2) and  UPPER(feature_name)= 'COUNTY'  ;
  
  if v_cnt <> cur.county then
  select sap_equip_id ,Sap_Func_Loc_No , county  into p_sap_equip_id,p_sap_func_loc, p_old_vale from t_polestructure_evw where  sap_equip_id=cur.sap_equip_id ;
  update  T_POLESTRUCTURE_evw set  county= v_cnt where objectid=cur.objectid ;
  counter1 :=counter1 +1;
  COMMIT;
  
  select  county into p_new_value from t_polestructure_evw  where sap_equip_id=cur.sap_equip_id;
  
  
  insert into  ME_SAP_DATA_LOG (sap_equip_id,Field_Name,Old_Value,Updated_Value,Version_Name) values (p_sap_equip_id,'POLE_COUNTY', p_old_vale,p_new_value,version_name);  
  end if;
  commit;

  
  exception when others then
   
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE ('Error in Pole Division update where error is '  || v_code || ' ' || v_errm || ' for sap_equip_id' || cur.sap_equip_id );
    continue;
     end;
     


  
  

end loop;

DBMS_OUTPUT.PUT_LINE(' Process Completed Sucessfully for ' || counter1 || ' record for county pole' );

sde.version_user_ddl.edit_version('ME_POLE_STRUCT_67890_V3',2);
commit;
sde.version_util.set_current_version('SDE.DEFAULT'); 
commit;
end;
/
prompt Updating POLE intersect FOR county and  objectid like 67890 is complete;
select current_timestamp from dual;
spool off;