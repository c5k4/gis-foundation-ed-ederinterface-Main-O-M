spool "D:\DQI_Scripts\Logs\SQL_POLE_DIVISION_12345_ETS-NV.txt"
SET SERVEROUTPUT ON
--set serveroutput on size 100000;
prompt Updating POLE domain division ;
select current_timestamp from dual ;

declare 


  P_BU_INSL_MATERIAL NVARCHAR2(60) ;         
  P_SAP_EQUIP_ID NVARCHAR2(60)  ;           
  P_AU_INSL_MATERIAL NVARCHAR2(60); 
  P_SAP_FUNC_LOC NVARCHAR2(60);
  P_FIELD_NAME1 NVARCHAR2(40) :='POLE_DIVISION';
  
  v_code  NUMBER;
  v_errm  VARCHAR2(64);
  version_name  NVARCHAR2(30) :='ME_POLE_STRUCT_12345_V3';
  counter1 NUMBER :=0;
  counter2 NUMBER :=0;

    
begin

  sde.version_util.set_current_version('ME_POLE_STRUCT_12345_V3');                                   
  sde.version_user_ddl.edit_version('ME_POLE_STRUCT_12345_V3',1);
  DBMS_OUTPUT.PUT_LINE('Update Process started');

for cur in (select * from T_POLESTRUCTURE_evw WHERE (objectid  like ('%1') or objectid  like ('%2') or objectid  like ('%3') or objectid  like ('%4') or objectid  like ('%5')) and UPPER(division) ='ETS-NV')

loop
  
BEGIN
  


  update  t_polestructure_evw  set  DIVISION='NV' WHERE  SAP_EQUIP_ID =cur.SAP_EQUIP_ID;
  commit;

   counter1 :=counter1 + 1;

 insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name)
 values (cur.SAP_EQUIP_ID, P_FIELD_NAME1 ,'ETS-NV','NV',version_name);

  commit;

  exception when others then
    -- DBMS_OUTPUT.PUT_LINE(i.sap_equip_id || ' NOT FOUND ');
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE ('Error in Pole LIKE  12345 Division update where error is '  || v_code || ' ' || v_errm || ' ' ||cur.sap_equip_id);
    continue;
     end;
 

end loop;

update  t_polestructure_evw  set  DIVISION='NV' WHERE (objectid  like ('%1') or objectid  like ('%2') or objectid  like ('%3') or objectid  like ('%4') or objectid  like ('%5')) AND UPPER(division) ='ETS-NV' AND SAP_EQUIP_ID IS NULL;

COMMIT;

DBMS_OUTPUT.PUT_LINE(' Process Completed Sucessfully for ' || counter1 || 'record for division pole like 12345' );

sde.version_user_ddl.edit_version('ME_POLE_STRUCT_12345_V3',2);
commit;
sde.version_util.set_current_version('SDE.DEFAULT'); 
commit;
DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully');

end;
/
prompt Updating POLE domain FOR division with  objectid  like 12345 is complete;
select current_timestamp from dual;
spool off;