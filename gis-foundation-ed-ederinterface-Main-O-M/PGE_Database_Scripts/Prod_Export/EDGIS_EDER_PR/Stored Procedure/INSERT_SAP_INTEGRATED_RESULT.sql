--------------------------------------------------------
--  DDL for Procedure INSERT_SAP_INTEGRATED_RESULT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."INSERT_SAP_INTEGRATED_RESULT" (p_versionname IN VARCHAR2)
AS
-- p_versionname VARCHAR2(20);
   p_error VARCHAR2(1000);
BEGIN

 -- set serveroutput on;
  -- exists in sap only (1)
  --p_versionname := 'edgis.ED_07_Test';
  --sde.version_user_ddl.create_version('sde.DEFAULT', p_versionname, sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'versioned view edit version');
  sde.version_util.set_current_version(p_versionname);
   sde.version_user_ddl.edit_version(p_versionname,1);
  EXECUTE immediate 'insert into edgis.SAP_INTEGRATED_RESULT (SAP_EQUIPMENT_TYPE, FEATURE_GUID, RESULT ) select sap_equipment_type,guid,1
from edgis.SAP_TO_GIS minus select sap_equipment_type,guid,1 from edgis.SAP_TO_GIS where
guid in (select FEATURE_GUID from edgis.GIS_GUID)';
  -- exists in gis and sap and matches (2)
  EXECUTE immediate 'insert into edgis.SAP_INTEGRATED_RESULT (SAP_EQUIPMENT_TYPE, FEATURE_GUID, RESULT )
select a.sap_equipment_type, a.guid,2 from EDGIS.SAP_TO_GIS a inner join edgis.gis_guid b on a.guid = b.feature_guid
and a.sap_equipment_id=b.equipment_id group by a.sap_equipment_type,b.fc_name,a.guid';
  -- exists in gis only (3)
  EXECUTE immediate 'insert into edgis.SAP_INTEGRATED_RESULT ( FC_NAME, FEATURE_GUID, RESULT ) select fc_name,FEATURE_GUID,3
from edgis.GIS_GUID minus select fc_name,FEATURE_GUID,3 from edgis.GIS_GUID where
FEATURE_GUID in (select guid from edgis.SAP_TO_GIS)';
  -- update 4 , need to update gis the values did not match and both objects exist.
  -- exists in gis and sap but Equipment ID dont matches (4)
   dbms_output.put_line('loop end');
  DECLARE
    CURSOR READ_SAP_TO_GIS
    IS
      SELECT a.sap_equipment_id,
        a.equipment_name,
        a.sap_equipment_type,
        a.guid,
        b.feature_guid,
        b.fc_name
      FROM edgis.SAP_TO_GIS a
      LEFT JOIN edgis.GIS_GUID b
      ON a.guid = b.feature_guid
      --Changes done by YXA6 to Resolve the ED07 DATA Replication Issue
      where  a.PROCESSEDFLAG='T'
      minus
    SELECT c.sap_equipment_id,
      c.equipment_name,
      c.sap_equipment_type,
      c.guid,
      d.feature_guid,
      ----Changes done by YXA6 to Resolve the ED07 DATA Replication Issue
      --'EDGIS.ZZ_MV_' || d.fc_name
      d.fc_name
    FROM edgis.SAP_TO_GIS c
    LEFT JOIN edgis.GIS_GUID d
    ON c.guid             = d.feature_guid
    AND c.sap_equipment_id=d.equipment_id
    WHERE c.guid
in

( SELECT
    f.feature_guid
FROM
    edgis.sap_integrated_result f
);
    SAP_EQUIPMENT_ID   VARCHAR(200);
    EQUIPMENT_NAME     VARCHAR(200);
    SAP_EQUIPMENT_TYPE VARCHAR(200);
    GUID               VARCHAR2(200);
    SQLSTM             VARCHAR2(2000);
    SQLSTM2            VARCHAR2(2000);
    reg_num            NUMBER;
	tab_owner          VARCHAR2(2000);
    count_num          NUMBER;
  
  BEGIN
    FOR CUR_OBJ IN READ_SAP_TO_GIS
    LOOP
    BEGIN 
	  SQLSTM := 'select registration_id,owner from sde.table_registry where table_name='''||CUR_OBJ.FC_NAME||'''  ';
      EXECUTE immediate sqlstm INTO reg_num,tab_owner ;
	  sqlstm := 'select count(*) from all_tables where table_name='''||CUR_OBJ.FC_NAME||''' and owner='''||tab_owner||'''  ';
dbms_output.put_line(sqlstm);
	  EXECUTE immediate sqlstm INTO count_num ;
      IF count_num=1 THEN
dbms_output.put_line(sqlstm);
        SQLSTM2 := 'INSERT INTO SAP_INTEGRATED_RESULT (SAP_EQUIPMENT_TYPE, FC_NAME, FEATURE_GUID, RESULT ) 
        VALUES ('''||CUR_OBJ.SAP_EQUIPMENT_TYPE||''', '''||CUR_OBJ.FC_NAME||''', '''||CUR_OBJ.GUID||''', 4)';
        EXECUTE immediate SQLSTM2;
        IF CUR_OBJ.sap_equipment_id IS NOT NULL THEN
        --Changes done by YXA6 to Resolve the ED07 DATA Replication Issue
         --SQLSTM  := 'UPDATE '|| 'EDGIS.ZZ_MV_' || CUR_OBJ.FC_NAME||' SET SAPEQUIPID= '''||CUR_OBJ.sap_equipment_id||''',
         -- LASTUSER=user,DATEMODIFIED=sysdate WHERE GLOBALID= '''|| CUR_OBJ.guid ||''' ';
         SQLSTM  := 'UPDATE '|| 'EDGIS.ZZ_MV_' || CUR_OBJ.FC_NAME||' SET SAPEQUIPID= '''||CUR_OBJ.sap_equipment_id||'''
          WHERE GLOBALID= '''|| CUR_OBJ.guid ||''' ';
          
          EXECUTE IMMEDIATE SQLSTM;
          UPDATE EDGIS.SAP_TO_GIS  SET PROCESSEDFLAG = 'D' , PROCESSEDTIME = sysdate WHERE GUID = CUR_OBJ.guid; 
          dbms_output.put_line(sqlstm);

--Changes done by YXA6 to Resolve the ED07 DATA Replication Issue
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

          --sqlstm := 'select count(*) from all_tables where table_name=''A'||reg_num||''' and owner='''||tab_owner||'''  ';
 --dbms_output.put_line(sqlstm);
	---	  EXECUTE immediate sqlstm INTO count_num ;
     --     IF count_num=1 THEN
       --     SQLSTM   := 'UPDATE '||tab_owner||'.A'||reg_num||' SET SAPEQUIPID= '''||CUR_OBJ.sap_equipment_id||''', LASTUSER=user,DATEMODIFIED=sysdate WHERE GLOBALID= '''|| CUR_OBJ.guid ||''' ';
         --   EXECUTE immediate sqlstm;
           -- UPDATE EDGIS.SAP_TO_GIS  SET PROCESSEDFLAG = 'D' , PROCESSEDTIME = sysdate WHERE GUID = CUR_OBJ.guid; 
--dbms_output.put_line(sqlstm);
  --        END IF;
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		  COMMIT;
        END IF;
	  ELSE
      dbms_output.put_line('Unable to find the table specified of '||tab_owner||'.'||CUR_OBJ.FC_NAME);
	  END IF;
      exception when OTHERS then       
      p_error := SQLCODE ||':'|| SUBSTR(SQLERRM, 1 , 64);
      dbms_output.put_line(p_error);
     -- Update EDGIS.SAP_TO_GIS set ERRORDESCRIPTION= to_clob (utl_raw.cast_to_raw(p_error)) WHERE GUID=  CUR_OBJ.guid  ; 
      Update EDGIS.SAP_TO_GIS set ERRORDESCRIPTION= to_clob (utl_raw.cast_to_raw(p_error)) , PROCESSEDFLAG = 'F', PROCESSEDTIME = sysdate WHERE GUID=  CUR_OBJ.guid  ; 
      commit;
      End;      
    END LOOP;
    COMMIT;
    dbms_output.put_line('loop end');
  END;
 sde.version_user_ddl.edit_version(p_versionname,2);
END INSERT_SAP_INTEGRATED_RESULT;
