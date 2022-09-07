--------------------------------------------------------
--  DDL for Procedure LOAD_GIS_GUID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."LOAD_GIS_GUID" 

AS
BEGIN
  DECLARE
    CURSOR ALL_TABLES_WE_CARE_ABOUT

    IS
      SELECT a.owner,
        a.table_name
      FROM sde.column_registry a
      WHERE a.column_name='GLOBALID' and a.owner = 'EDGIS'
      AND a.table_name  IN
        (SELECT b.table_name
        FROM sde.column_registry b
        WHERE b.column_name='SAPEQUIPID'
        )and a.table_name NOT LIKE 'ZPGEVW%' and a.table_name NOT LIKE 'ZZ_MV%';

    SQLSTM    VARCHAR2(2000);
    reg_num   NUMBER;
    count_num NUMBER;
  BEGIN
    FOR tables IN ALL_TABLES_WE_CARE_ABOUT
    LOOP
      sqlstm := 'select count(registration_id) from sde.table_registry where table_name='''||tables.table_name||''' and owner='''||tables.owner||''' ';
      --dbms_output.put_line(sqlstm);
      EXECUTE immediate sqlstm INTO reg_num ;
      IF reg_num=1 THEN
        sqlstm := 'select registration_id from sde.table_registry where table_name='''||tables.table_name||''' and owner='''||tables.owner||''' ';
        --dbms_output.put_line(sqlstm);
        EXECUTE immediate sqlstm INTO reg_num ;
        sqlstm := 'select count(*) from all_tables where table_name=''A'||reg_num||''' and owner='''||tables.owner||''' ';
        EXECUTE immediate sqlstm INTO count_num ;
        IF count_num=1 THEN
          sqlstm   := 'insert into edgis.gis_guid (feature_guid,fc_name,EQUIPMENT_ID) select globalid,'''||tables.table_name||''',SAPEQUIPID from (select globalid,SAPEQUIPID from '||tables.owner||'.'||tables.table_name||' union select globalid,SAPEQUIPID from '||tables.owner||'.A'||reg_num||')  group by globalid,SAPEQUIPID ';
        ELSE
          sqlstm := 'insert into edgis.gis_guid (feature_guid,fc_name,EQUIPMENT_ID) select globalid,'''||tables.table_name||''',SAPEQUIPID from (select globalid,SAPEQUIPID from '||tables.owner||'.'||tables.table_name||' ) group by globalid,SAPEQUIPID  ';
        END IF;
        -- dbms_output.put_line(sqlstm);
        EXECUTE immediate sqlstm;
        COMMIT;
      ELSE
        dbms_output.put_line('Unable to find the table specified of '||tables.owner||'.'||tables.table_name);
      END IF ;
    END LOOP;
    UPDATE edgis.gis_guid
    SET EQUIPMENT_ID    ='DUPLICATE'
    WHERE feature_guid IN
      (SELECT b.feature_guid
      FROM edgis.gis_guid b
      GROUP BY feature_guid
      HAVING COUNT(*)>1
      );
    COMMIT;
  END;
END LOAD_GIS_GUID;
