spool "D:\DQI_Scripts\Logs\SQL_POLE12345_HFTD.txt"
SET SERVEROUTPUT ON
--set serveroutput on size 100000;
prompt Updating POLE intersect for hftd objectid like 12345 ;
select current_timestamp from dual ;

declare 

 
  v_hftd NVARCHAR2(64) :=NULL;
  version_name  NVARCHAR2(30) :='ME_POLE_STRUCT_12345_V3';
  v_code  NUMBER;
  v_errm  NVARCHAR2(64);
  p_new_value NVARCHAR2(64) :=null;
  p_sap_equip_id NVARCHAR2(64):=null ;
  p_sap_func_loc NVARCHAR2(64) :=null;
  p_old_vale NVARCHAR2(64) :=null; 

  counter4 NUMBER :=0;
    
begin

  sde.version_util.set_current_version('ME_POLE_STRUCT_12345_V3');                                   
  sde.version_user_ddl.edit_version('ME_POLE_STRUCT_12345_V3',1);
  DBMS_OUTPUT.PUT_LINE('Update Process started');

for cur in (select * from T_POLESTRUCTURE_evw WHERE objectid  like ('%1') or objectid  like ('%2') or objectid  like ('%3') or objectid  like ('%4') or objectid  like ('%5'))

loop
 BEGIN
  select code into  v_hftd  from etgis.domain_lookup where CODE_DESC =(  select CPUC_FIRE_MAP 
  from LD_hftd_1 where sde.st_intersects(shape,cur.shape) = 1 and ROWNUM<2) AND UPPER(FEATURE_NAME)= 'HFTD'  ;
  if v_hftd <> cur.hftd then
   select sap_equip_id , Sap_Func_Loc_No , hftd into p_sap_equip_id,p_sap_func_loc, p_old_vale from t_polestructure_evw 
  where  Sap_Equip_Id=cur.sap_equip_id ;
  update  T_POLESTRUCTURE_evw set  hftd= v_hftd where objectid=cur.objectid ;
   counter4 :=counter4 +1;
  COMMIT;
  select  hftd into p_new_value from t_polestructure_evw  where sap_equip_id=cur.sap_equip_id;
  
 
  insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name)
 values (p_sap_equip_id,'POLE_HFTD', p_old_vale,p_new_value,version_name);
  end if;
  commit;
  exception when others then
    -- DBMS_OUTPUT.PUT_LINE(i.sap_equip_id|| ' NOT FOUND ');
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE ('Error in Pole hftd update where error is '  || v_code || ' ' || v_errm || ' for sap_equip_id' || cur.sap_equip_id );
    continue;
     end;


  

end loop;


DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully for ' || counter4 || ' record for hftd pole');
sde.version_user_ddl.edit_version('ME_POLE_STRUCT_12345_V3',2);
commit;
sde.version_util.set_current_version('SDE.DEFAULT'); 
commit;
DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');
--DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');
end;
/
prompt Updating POLE intersect FOR hftd with objectid  12345 is complete;
select current_timestamp from dual;
spool off;