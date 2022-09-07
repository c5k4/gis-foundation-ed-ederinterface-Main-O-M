spool "D:\DA_190401\Logs\domain_update.txt"
SET SERVEROUTPUT ON
prompt Updating DIVISION DOMAIN;
select current_timestamp from dual;



declare 

  P_BU_INSL_MATERIAL NVARCHAR2(60) ;         
  P_SAP_EQUIP_ID NVARCHAR2(60)  ;           
  P_AU_INSL_MATERIAL NVARCHAR2(60); 
  P_SAP_FUNC_LOC NVARCHAR2(60);
  P_FIELD_NAME1 NVARCHAR2(40) :='DIVISION_POLE';
  P_FIELD_NAME2 NVARCHAR2(40) :='DIVISION_TOWER';

  v_code  NUMBER;
  v_errm  VARCHAR2(64);
  version_name  NVARCHAR2(30) :='DIV_DOMAIN_UD';
  counter1 number :=0;
  counter2 NUMBER :=0;
  cursor rw1 is select * from t_polestructure_evw where  division ='ETS-NV';
  cursor rw2 is select * from t_TOWERstructure_evw where  division ='ETS-NV';
    
begin
sde.version_user_ddl.create_version ('sde.DEFAULT',version_name,sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'masss update et for division domain update');
sde.version_util.set_current_version('DIV_DOMAIN_UD');                                   
sde.version_user_ddl.edit_version('DIV_DOMAIN_UD',1);

DBMS_OUTPUT.PUT_LINE('Update Process started POLE');

for c1 in rw1 
LOOP
BEGIN 

update  t_polestructure_evw  set  DIVISION='NV' WHERE  SAP_EQUIP_ID =C1.SAP_EQUIP_ID;
commit;

counter1 :=counter1 + 1;

 insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name)
 values (C1.SAP_EQUIP_ID, P_FIELD_NAME1 ,'ETS-NV','NV',version_name);
  
exception when others then
    
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE (' error updating POLE data '||v_code || ' ' || v_errm);
   continue;
     end;
 END LOOP;    
 DBMS_OUTPUT.PUT_LINE('Update Process started TOWER');    
for c2 in rw2
LOOP
BEGIN 

update  t_TOWERstructure_evw  set  DIVISION='NV' WHERE  SAP_EQUIP_ID =C2.SAP_EQUIP_ID;

commit;
counter2 :=counter2 + 1;
 insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name)
 values (C2.SAP_EQUIP_ID, P_FIELD_NAME2 ,'ETS-NV','NV',version_name);
exception when others then
    
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE (' error updating TOWER data '||v_code || ' ' || v_errm);
   continue;
     end;
     
     END LOOP;
sde.version_user_ddl.edit_version('DIV_DOMAIN_UD',2);
DBMS_OUTPUT.PUT_LINE(counter1 ||' counts Processed Sucessfully');
DBMS_OUTPUT.PUT_LINE(counter2 ||' counts Processed Sucessfully');

DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');
end;
/
prompt Updating DIVISION DOMAIN is complete;
select current_timestamp from dual;
spool off;