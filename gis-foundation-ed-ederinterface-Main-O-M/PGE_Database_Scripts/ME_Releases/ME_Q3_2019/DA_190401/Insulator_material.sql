spool "D:\DQI_Scripts\Logs\Insulator_Material_up.txt"
SET SERVEROUTPUT ON

prompt Updating iNSULATOR_MATERIAL;
select current_timestamp from dual;

declare 

  P_BU_INSL_MATERIAL NVARCHAR2(30) ;         
  P_SAP_EQUIP_ID NVARCHAR2(30)  ;           
  P_AU_INSL_MATERIAL NVARCHAR2(30); 
  P_FIELD_NAME NVARCHAR2(40):='INSL_MATERIAL';
  version_name  NVARCHAR2(20) :='INSL_MAT_UD';
  v_code  NUMBER;
  v_errm  VARCHAR2(64);
  counter number :=0; 
    
begin
sde.version_user_ddl.create_version ('sde.DEFAULT',version_name,sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'masss update et insulator material');
sde.version_util.set_current_version('INSL_MAT_UD');                                   
sde.version_user_ddl.edit_version('INSL_MAT_UD',1);

DBMS_OUTPUT.PUT_LINE('Update Process started');


FOR i IN (SELECT * FROM Me_Insulator_Material_Dm_Up WHERE SAP_EQUIP_ID IS NOT NULL)
  LOOP
  begin
     select INSULATOR_MATERIAL,SAP_EQUIP_ID into P_BU_INSL_MATERIAL ,P_SAP_EQUIP_ID 
     FROM t_INSULATOR_evw  WHERE sap_equip_id=i.sap_equip_id AND (Insulator_Material IS NULL OR   upper(Insulator_Material)='UNKNOWN') ;
     exception when others then
     --DBMS_OUTPUT.PUT_LINE(i.sap_equip_id);
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE (' Select error  for  '|| i.sap_equip_id || v_code || ' ' || v_errm);
      CONTINUE;
     end;
     -- 43600224
     
    UPDATE t_INSULATOR_evw SET INSULATOR_MATERIAL=i.PROPOSED_INSULATOR_MATERIAL where 
    sap_equip_id=i.sap_equip_id AND (Insulator_Material IS NULL OR   upper(Insulator_Material)='UNKNOWN') ;
     counter := counter+1; 
      begin
      select INSULATOR_MATERIAL into P_AU_INSL_MATERIAL   FROM t_INSULATOR_evw   WHERE 
    sap_equip_id=i.sap_equip_id    ;
    exception when others then
     --DBMS_OUTPUT.PUT_LINE(i.sap_equip_id);
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE (' Select error  for updating '|| i.sap_equip_id || v_code || ' ' || v_errm);
      CONTINUE;
     end;
      --DBMS_OUTPUT.PUT_LINE(@P_BU_INSL_MATERIAL,@P_SAP_EQUIP_ID,@P_AU_INSL_MATERIAL)
      begin
     insert into  ME_SAP_DATA_LOG (Sap_Equip_Id,Field_Name,Old_Value,Updated_Value,Version_Name) values (P_SAP_EQUIP_ID,P_FIELD_NAME,P_BU_INSL_MATERIAL,P_AU_INSL_MATERIAL,version_name);
     exception when others then
     --DBMS_OUTPUT.PUT_LINE(i.sap_equip_id);
     v_code := SQLCODE;
    v_errm := SUBSTR(SQLERRM, 1, 64);
    DBMS_OUTPUT.PUT_LINE (' Insert error  for  '|| i.sap_equip_id ||v_code || ' ' || v_errm);
    CONTINUE;
     end;
      COMMIT;

  END LOOP;

sde.version_user_ddl.edit_version('INSL_MAT_UD',2);
DBMS_OUTPUT.PUT_LINE('Process Completed Sucessfully for '|| counter || 'count' );
end;
/
prompt Updating iNSULATOR_MATERIAL is complete;
select current_timestamp from dual;

spool off;
