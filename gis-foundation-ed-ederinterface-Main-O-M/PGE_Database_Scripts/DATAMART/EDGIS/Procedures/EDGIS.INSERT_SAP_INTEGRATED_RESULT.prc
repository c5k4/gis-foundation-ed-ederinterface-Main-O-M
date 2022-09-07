Prompt drop Procedure INSERT_SAP_INTEGRATED_RESULT;
DROP PROCEDURE EDGIS.INSERT_SAP_INTEGRATED_RESULT
/

Prompt Procedure INSERT_SAP_INTEGRATED_RESULT;
--
-- INSERT_SAP_INTEGRATED_RESULT  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.INSERT_SAP_INTEGRATED_RESULT
AS
BEGIN
  --set serveroutput on;
  -- exists in sap only (1)
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
      ON a.guid = b.feature_guid minus
    SELECT c.sap_equipment_id,
      c.equipment_name,
      c.sap_equipment_type,
      c.guid,
      d.feature_guid,
      d.fc_name
    FROM edgis.SAP_TO_GIS c
    LEFT JOIN edgis.GIS_GUID d
    ON c.guid             = d.feature_guid
    AND c.sap_equipment_id=d.equipment_id
    WHERE c.guid         IN
      (SELECT f.FEATURE_GUID FROM edgis.SAP_INTEGRATED_RESULT f
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
	  SQLSTM := 'select registration_id,owner from sde.table_registry where table_name='''||CUR_OBJ.FC_NAME||'''  ';
      EXECUTE immediate sqlstm INTO reg_num,tab_owner ;
	  sqlstm := 'select count(*) from all_tables where table_name='''||CUR_OBJ.FC_NAME||''' and owner='''||tab_owner||'''  ';
        -- dbms_output.put_line(sqlstm);
	  EXECUTE immediate sqlstm INTO count_num ;
      IF count_num=1 THEN
	  -- dbms_output.put_line(sqlstm);
        SQLSTM2 := 'INSERT INTO SAP_INTEGRATED_RESULT (SAP_EQUIPMENT_TYPE, FC_NAME, FEATURE_GUID, RESULT ) VALUES ('''||CUR_OBJ.SAP_EQUIPMENT_TYPE||''', '''||CUR_OBJ.FC_NAME||''', '''||CUR_OBJ.GUID||''', 4)';
        EXECUTE immediate SQLSTM2;
        IF CUR_OBJ.sap_equipment_id IS NOT NULL THEN
          SQLSTM  := 'UPDATE '||CUR_OBJ.FC_NAME||' SET SAPEQUIPID= '''||CUR_OBJ.sap_equipment_id||''' WHERE GLOBALID= '''|| CUR_OBJ.guid ||''' ';
          EXECUTE IMMEDIATE SQLSTM;
          -- dbms_output.put_line(sqlstm);
          sqlstm := 'select count(*) from all_tables where table_name=''A'||reg_num||''' and owner='''||tab_owner||'''  ';
          -- dbms_output.put_line(sqlstm);
		  EXECUTE immediate sqlstm INTO count_num ;
          IF count_num=1 THEN
            SQLSTM   := 'UPDATE '||tab_owner||'.A'||reg_num||' SET SAPEQUIPID= '''||CUR_OBJ.sap_equipment_id||''' WHERE GLOBALID= '''|| CUR_OBJ.guid ||''' ';
            EXECUTE immediate sqlstm;
            --dbms_output.put_line(sqlstm);
          END IF;
		  COMMIT;
        END IF;
	  ELSE
	     dbms_output.put_line('Unable to find the table specified of '||tab_owner||'.'||CUR_OBJ.FC_NAME);
	  END IF;
    END LOOP;
    COMMIT;
    --dbms_output.put_line('loop end');
  END;
END INSERT_SAP_INTEGRATED_RESULT;
/


Prompt Grants on PROCEDURE INSERT_SAP_INTEGRATED_RESULT TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON EDGIS.INSERT_SAP_INTEGRATED_RESULT TO GIS_I
/
