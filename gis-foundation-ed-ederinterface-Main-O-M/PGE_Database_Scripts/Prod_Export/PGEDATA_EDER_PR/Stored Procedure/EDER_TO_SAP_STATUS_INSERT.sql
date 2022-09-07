--------------------------------------------------------
--  DDL for Procedure EDER_TO_SAP_STATUS_INSERT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."EDER_TO_SAP_STATUS_INSERT" 
(
    STATUS OUT VARCHAR,
    input_dm_di_flag IN VARCHAR2 )
AS
sp_id VARCHAR2(20):=null;
cgc12 NUMBER:=0;
circuitid VARCHAR2(20):=null;
h_count NUMBER :=0;
BEGIN
  IF input_dm_di_flag = 'DI' THEN --Run for Daily Interface
    FOR o IN
  (Select ACTION,SAP_EQUIPMENT_ID,GUID,SERVICE_POINT_ID,SAP_EGI_NOTIFICATION,STATUS,STATUS_MESSAGE from PGEDATA.gen_equipment_stage  )
  LOOP
  if o.guid is not null then
  cgc12 := null;
  circuitid := null;

begin
  select sp.servicepointid into sp_id from edgis.zz_mv_servicepoint sp,edgis.zz_mv_generationinfo gen
  where sp.globalid= gen.servicepointguid and gen.globalid=o.GUID  AND ROWNUM < 2;
   Exception
    When NO_DATA_FOUND Then
     dbms_output.put_line('No Data Found for the GUID:'|| o.GUID);
     sp_id:= o.service_point_id;
     end;

begin
  SELECT PM.CGC12 ,PM.CIRCUITID into cgc12,circuitid
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_PRIMARYMETER PM
        WHERE SP.primarymeterguid=PM.globalid and sp.servicepointid = sp_id  AND ROWNUM < 2;
    Exception
    When NO_DATA_FOUND Then
   -- DBMS_OUTPUT.PUT_LINE('ERROR: '||sqlerrm);
    --Raise;

  if circuitid is null then
  begin
  SELECT TR.CGC12 ,TR.CIRCUITID into cgc12,circuitid
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_TRANSFORMER TR
        WHERE SP.transformerguid=TR.globalid and sp.servicepointid = sp_id  AND ROWNUM < 2;
      Exception
    When NO_DATA_FOUND Then
    dbms_output.put_line('No Data Found for the SPID:'|| sp_id);
    end;
  end if;
  end;
  insert into pgedata.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by

      ) select o.SAP_EQUIPMENT_ID equipmentid,o.GUID guid,sp_id spid,o.SAP_EGI_NOTIFICATION current_project,cgc12 cgc,circuitid circuitid,
      CASE
        WHEN o.STATUS = 'F'
        THEN 'E'
        WHEN o.STATUS = 'S'
        THEN 'S'
	WHEN o.STATUS = 'W'
        THEN 'W'
        ELSE 'E'
      END AS STATUS,o.status_message status_message,
      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
      TRUNC(Sysdate) as creationdate,TRUNC(Sysdate) as date_inserted,
      'STAGE'        AS inserted_by from dual;


  else
  cgc12 := null;
  circuitid := null;
  if o.service_point_id is not null then

 begin
  SELECT PM.CGC12 ,PM.CIRCUITID into cgc12,circuitid
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_PRIMARYMETER PM
        WHERE SP.primarymeterguid=PM.globalid and sp.servicepointid = o.service_point_id  AND ROWNUM < 2;
    Exception
    When NO_DATA_FOUND Then
   -- DBMS_OUTPUT.PUT_LINE('ERROR: '||sqlerrm);
    --Raise;

  if circuitid is null then
  begin
  SELECT TR.CGC12 ,TR.CIRCUITID into cgc12,circuitid
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_TRANSFORMER TR
        WHERE SP.transformerguid=TR.globalid and sp.servicepointid = o.service_point_id  AND ROWNUM < 2;
      Exception
    When NO_DATA_FOUND Then
   dbms_output.put_line('No Data Found for the SPID:'||o.service_point_id);
    end;
  end if;
  end;

   insert into pgedata.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
		recordid,	
        creationdate,
        date_inserted,
        inserted_by
      ) values (o.SAP_EQUIPMENT_ID,o.GUID, o.service_point_id,o.SAP_EGI_NOTIFICATION,cgc12,circuitid,
       CASE
        WHEN o.STATUS = 'F'
        THEN 'E'
        WHEN o.STATUS = 'S'
        THEN 'S'
        WHEN o.STATUS = 'W'
        THEN 'W'
        ELSE 'E' END,
      o.status_message,
	  ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0)),	
      TRUNC(Sysdate),
	  TRUNC(Sysdate),
      'STAGE' );
else
insert into pgedata.eder_to_sap_status (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
		recordid,	
        creationdate,
        date_inserted,
        inserted_by
      ) values (o.SAP_EQUIPMENT_ID,o.GUID, o.service_point_id,o.SAP_EGI_NOTIFICATION,0,'',
      'E',o.status_message,
	  ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0)),	
      TRUNC(Sysdate),
	  TRUNC(Sysdate),'STAGE' );
end if;
  end if ;

  END LOOP;



    INSERT
    INTO PGEDATA.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by
      )
    SELECT '' AS EQUIPMENTID,
      chng.GEN_GUID guid,
      chng.servicepointid spid,
      eqp.sapeginotification CURRENT_PROJECT,
      chng.CGC12 CGC,
      chng.CIRCUITID,
      'C'              AS STATUS,
      'Circuit Change' AS status_message,
      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
      TRUNC(Sysdate) as creationdate,
      TRUNC(Sysdate) as date_inserted,
      'SPID'           AS INSERTED_BY
    FROM
      (SELECT GEN_GUID,
        servicepointid,
        CGC12,
        CIRCUITID
      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
        pgedata.pgedata_executed pex
      WHERE udated_field                      ='SERVICEPOINTGUID'
      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
      AND pex.process_name                    ='ED16UPDATESTOSAP'
      ) chng,
      edgis.zz_mv_generationinfo eqp
    WHERE chng.gen_guid= eqp.globalid;
    INSERT
    INTO PGEDATA.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by
      )
    SELECT '' AS EQUIPMENTID,
      chng.GEN_GUID guid,
      chng.servicepointid spid,
      eqp.sapeginotification CURRENT_PROJECT,
      chng.CGC12 CGC,
      chng.CIRCUITID,
      'C'              AS STATUS,
      'Circuit Change' AS status_message,
      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
      TRUNC(Sysdate) as creationdate,
      TRUNC(Sysdate) as date_inserted,

      'SPID'           AS INSERTED_BY
    FROM
      (SELECT GEN_GUID,
        servicepointid,
        CGC12,
        CIRCUITID
      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
        pgedata.pgedata_executed pex
      WHERE udated_field                      ='SERVICEPOINTID'
      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
      AND pex.process_name                    ='ED16UPDATESTOSAP'
      ) chng,
      edgis.zz_mv_generationinfo eqp
    WHERE chng.gen_guid= eqp.globalid;
    INSERT
    INTO PGEDATA.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by
      )
    SELECT '' AS EQUIPMENTID,
      chng.GEN_GUID guid,
      chng.servicepointid spid,
      eqp.sapeginotification CURRENT_PROJECT,
      chng.CGC12 CGC,
      chng.CIRCUITID,
      'C'              AS STATUS,
      'Circuit Change' AS status_message,
      'ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0) as recordid,
      TRUNC(Sysdate) as creationdate,
      TRUNC(SYSDATE)   AS DATE_INSERTED,
      'CGC'            AS INSERTED_BY
    FROM
      (SELECT GEN_GUID,
        servicepointid,
        CGC12,
        CIRCUITID
      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
        pgedata.pgedata_executed pex
      WHERE udated_field                      ='CGC12'
      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
      AND pex.process_name                    ='ED16UPDATESTOSAP'
      ) chng,
      edgis.zz_mv_generationinfo eqp
    WHERE chng.gen_guid= eqp.globalid;
    INSERT
    INTO PGEDATA.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by
      )
    SELECT '' AS EQUIPMENTID,
      chng.GEN_GUID guid,
      chng.servicepointid spid,
      eqp.sapeginotification CURRENT_PROJECT,
      chng.CGC12 CGC,
      chng.CIRCUITID,
      'C'              AS STATUS,
      'Circuit Change' AS status_message,
      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0)) as recordid,
      TRUNC(Sysdate) as creationdate,
      TRUNC(SYSDATE)   AS DATE_INSERTED,
      'CID'            AS INSERTED_BY
    FROM
      (SELECT GEN_GUID,
        servicepointid,
        CGC12,
        CIRCUITID
      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
        pgedata.pgedata_executed pex
      WHERE udated_field                      ='CIRCUITID'
      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
      AND pex.process_name                    ='ED16UPDATESTOSAP'
      ) chng,
      edgis.zz_mv_generationinfo eqp
    WHERE chng.gen_guid= eqp.globalid;
    UPDATE Pgedata.pgedata_executed pex
    SET lastdate      =sysdate
    WHERE process_name='ED16UPDATESTOSAP';
    COMMIT;



    --Run for Data Migration
  ELSIF input_dm_di_flag = 'DM' THEN
    INSERT
    INTO pgedata.EDER_TO_SAP_STATUS
      (
        equipmentid,
        guid,
        spid,
        current_project,
        cgc,
        circuitid,
        status,
        status_message,
        recordid,
        creationdate,
        date_inserted,
        inserted_by
      )
    SELECT EQUIPMENT_ID equipmentid ,
      EQUIP1.GUID,
      EQUIP1.SPID ,
      EQUIP1.CURRENT_PROJECT ,
      EQUIP1.CGC,
      EQUIP1.CIRCUITID,
      CASE
        WHEN EQUIP1.STATUS = 'F'
        THEN 'E'
        WHEN EQUIP1.STATUS = 'S'
        THEN 'S'
        ELSE 'E'
      END AS STATUS,
      EQUIP1.STATUS_MESSAGE,
      'ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0) as recordid,
      TRUNC(Sysdate) as creationdate,
      TRUNC(Sysdate) AS Date_inserted,
      'STAGE'        AS inserted_by
    FROM
      (SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
        EQUIP.GUID,
        SP2.SERVICEPOINTID SPID,
        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
        SP2.CGC12 CGC,
        SP2.CIRCUITID,
        EQUIP.STATUS,
        EQUIP.STATUS_MESSAGE
      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP,
        (SELECT PM.CGC12,
          PM.CIRCUITID,
          SP.SERVICEPOINTID
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,
          EDGIS.ZZ_MV_PRIMARYMETER PM
        WHERE SP.primarymeterguid=PM.globalid
        ) SP2
      WHERE SP2.SERVICEPOINTID = EQUIP.Service_point_id

      UNION

      SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
        EQUIP.GUID,
        SP1.SERVICEPOINTID SPID,
        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
        SP1.CGC12 CGC,
        SP1.CIRCUITID,
        EQUIP.STATUS,
        EQUIP.STATUS_MESSAGE
      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP,
        (SELECT TR.CGC12,
          TR.CIRCUITID,
          SP.SERVICEPOINTID
        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,
          EDGIS.ZZ_MV_TRANSFORMER TR
        WHERE SP.transformerguid=TR.globalid
        ) SP1
      WHERE SP1.SERVICEPOINTID = EQUIP.Service_point_id

      UNION
       SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
        EQUIP.GUID,
        EQUIP.SERVICE_POINT_ID SPID,
        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
        0 as CGC,
        null as CIRCUITID,
        EQUIP.STATUS,
        EQUIP.STATUS_MESSAGE
      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP
      where status='F'

      ) EQUIP1;
  END IF;
  STATUS := 'SUCCESS';
  COMMIT;
EXCEPTION
WHEN OTHERS THEN
  STATUS := SQLERRM;

END EDER_TO_SAP_STATUS_INSERT;
---------------------------------------------------------------------------
--Backup of older code
---------------------------------------------------------------------------
--create or replace PROCEDURE         "EDER_TO_SAP_STATUS_INSERT"
--(
--    STATUS OUT VARCHAR,
--    input_dm_di_flag IN VARCHAR2 )
--AS
--sp_id VARCHAR2(20):=null;
--cgc12 NUMBER:=0;
--circuitid VARCHAR2(20):=null;
--h_count NUMBER :=0;
--BEGIN
--  IF input_dm_di_flag = 'DI' THEN --Run for Daily Interface
--    FOR o IN
--  (Select ACTION,SAP_EQUIPMENT_ID,GUID,SERVICE_POINT_ID,SAP_EGI_NOTIFICATION,STATUS,STATUS_MESSAGE from PGEDATA.gen_equipment_stage  )
--  LOOP
--  if o.guid is not null then
--  cgc12 := null;
--  circuitid := null;
--
--begin
--  select sp.servicepointid into sp_id from edgis.zz_mv_servicepoint sp,edgis.zz_mv_generationinfo gen
--  where sp.globalid= gen.servicepointguid and gen.globalid=o.GUID  AND ROWNUM < 2;
--   Exception
--    When NO_DATA_FOUND Then
--     dbms_output.put_line('No Data Found for the GUID:'|| o.GUID);
--     sp_id:= o.service_point_id;
--     end;
--
--begin
--  SELECT PM.CGC12 ,PM.CIRCUITID into cgc12,circuitid
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_PRIMARYMETER PM
--        WHERE SP.primarymeterguid=PM.globalid and sp.servicepointid = sp_id  AND ROWNUM < 2;
--    Exception
--    When NO_DATA_FOUND Then
--   -- DBMS_OUTPUT.PUT_LINE('ERROR: '||sqlerrm);
--    --Raise;
--
--  if circuitid is null then
--  begin
--  SELECT TR.CGC12 ,TR.CIRCUITID into cgc12,circuitid
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_TRANSFORMER TR
--        WHERE SP.transformerguid=TR.globalid and sp.servicepointid = sp_id  AND ROWNUM < 2;
--      Exception
--    When NO_DATA_FOUND Then
--    dbms_output.put_line('No Data Found for the SPID:'|| sp_id);
--    end;
--  end if;
--  end;
--  insert into pgedata.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--
--      ) select o.SAP_EQUIPMENT_ID equipmentid,o.GUID guid,sp_id spid,o.SAP_EGI_NOTIFICATION current_project,cgc12 cgc,circuitid circuitid,
--      CASE
--        WHEN o.STATUS = 'F'
--        THEN 'E'
--        WHEN o.STATUS = 'S'
--        THEN 'S'
--	WHEN o.STATUS = 'W'
--        THEN 'W'
--        ELSE 'E'
--      END AS STATUS,o.status_message status_message,
--      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
--      TRUNC(Sysdate) as creationdate,TRUNC(Sysdate) as date_inserted,
--      'STAGE'        AS inserted_by from dual;
--
--
--  else
--  cgc12 := null;
--  circuitid := null;
--  if o.service_point_id is not null then
--
-- begin
--  SELECT PM.CGC12 ,PM.CIRCUITID into cgc12,circuitid
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_PRIMARYMETER PM
--        WHERE SP.primarymeterguid=PM.globalid and sp.servicepointid = o.service_point_id  AND ROWNUM < 2;
--    Exception
--    When NO_DATA_FOUND Then
--   -- DBMS_OUTPUT.PUT_LINE('ERROR: '||sqlerrm);
--    --Raise;
--
--  if circuitid is null then
--  begin
--  SELECT TR.CGC12 ,TR.CIRCUITID into cgc12,circuitid
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,EDGIS.ZZ_MV_TRANSFORMER TR
--        WHERE SP.transformerguid=TR.globalid and sp.servicepointid = o.service_point_id  AND ROWNUM < 2;
--      Exception
--    When NO_DATA_FOUND Then
--   dbms_output.put_line('No Data Found for the SPID:'||o.service_point_id);
--    end;
--  end if;
--  end;
--
--   insert into pgedata.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        date_inserted,
--        inserted_by
--      ) values (o.SAP_EQUIPMENT_ID,o.GUID, o.service_point_id,o.SAP_EGI_NOTIFICATION,cgc12,circuitid,
--       CASE
--        WHEN o.STATUS = 'F'
--        THEN 'E'
--        WHEN o.STATUS = 'S'
--        THEN 'S'
--        WHEN o.STATUS = 'W'
--        THEN 'W'
--        ELSE 'E' END
--      ,o.status_message,TRUNC(Sysdate),
--      'STAGE' );
--else
--insert into pgedata.eder_to_sap_status (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        date_inserted,
--        inserted_by
--      ) values (o.SAP_EQUIPMENT_ID,o.GUID, o.service_point_id,o.SAP_EGI_NOTIFICATION,0,'',
--      'E',o.status_message,TRUNC(Sysdate),'STAGE' );
--end if;
--  end if ;
--
--  END LOOP;
--
--
--
--    INSERT
--    INTO PGEDATA.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--      )
--    SELECT '' AS EQUIPMENTID,
--      chng.GEN_GUID guid,
--      chng.servicepointid spid,
--      eqp.sapeginotification CURRENT_PROJECT,
--      chng.CGC12 CGC,
--      chng.CIRCUITID,
--      'C'              AS STATUS,
--      'Circuit Change' AS status_message,
--      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
--      TRUNC(Sysdate) as creationdate,
--      TRUNC(Sysdate) as date_inserted,
--      'SPID'           AS INSERTED_BY
--    FROM
--      (SELECT GEN_GUID,
--        servicepointid,
--        CGC12,
--        CIRCUITID
--      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
--        pgedata.pgedata_executed pex
--      WHERE udated_field                      ='SERVICEPOINTGUID'
--      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
--      AND pex.process_name                    ='ED16UPDATESTOSAP'
--      ) chng,
--      edgis.zz_mv_generationinfo eqp
--    WHERE chng.gen_guid= eqp.globalid;
--    INSERT
--    INTO PGEDATA.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--      )
--    SELECT '' AS EQUIPMENTID,
--      chng.GEN_GUID guid,
--      chng.servicepointid spid,
--      eqp.sapeginotification CURRENT_PROJECT,
--      chng.CGC12 CGC,
--      chng.CIRCUITID,
--      'C'              AS STATUS,
--      'Circuit Change' AS status_message,
--      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(edgis.ED16_RECORDID_SEQ.nextval,7,0))  as recordid,
--      TRUNC(Sysdate) as creationdate,
--      TRUNC(Sysdate) as date_inserted,
--
--      'SPID'           AS INSERTED_BY
--    FROM
--      (SELECT GEN_GUID,
--        servicepointid,
--        CGC12,
--        CIRCUITID
--      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
--        pgedata.pgedata_executed pex
--      WHERE udated_field                      ='SERVICEPOINTID'
--      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
--      AND pex.process_name                    ='ED16UPDATESTOSAP'
--      ) chng,
--      edgis.zz_mv_generationinfo eqp
--    WHERE chng.gen_guid= eqp.globalid;
--    INSERT
--    INTO PGEDATA.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--      )
--    SELECT '' AS EQUIPMENTID,
--      chng.GEN_GUID guid,
--      chng.servicepointid spid,
--      eqp.sapeginotification CURRENT_PROJECT,
--      chng.CGC12 CGC,
--      chng.CIRCUITID,
--      'C'              AS STATUS,
--      'Circuit Change' AS status_message,
--      'ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0) as recordid,
--      TRUNC(Sysdate) as creationdate,
--      TRUNC(SYSDATE)   AS DATE_INSERTED,
--      'CGC'            AS INSERTED_BY
--    FROM
--      (SELECT GEN_GUID,
--        servicepointid,
--        CGC12,
--        CIRCUITID
--      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
--        pgedata.pgedata_executed pex
--      WHERE udated_field                      ='CGC12'
--      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
--      AND pex.process_name                    ='ED16UPDATESTOSAP'
--      ) chng,
--      edgis.zz_mv_generationinfo eqp
--    WHERE chng.gen_guid= eqp.globalid;
--    INSERT
--    INTO PGEDATA.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--      )
--    SELECT '' AS EQUIPMENTID,
--      chng.GEN_GUID guid,
--      chng.servicepointid spid,
--      eqp.sapeginotification CURRENT_PROJECT,
--      chng.CGC12 CGC,
--      chng.CIRCUITID,
--      'C'              AS STATUS,
--      'Circuit Change' AS status_message,
--      ('ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0)) as recordid,
--      TRUNC(Sysdate) as creationdate,
--      TRUNC(SYSDATE)   AS DATE_INSERTED,
--      'CID'            AS INSERTED_BY
--    FROM
--      (SELECT GEN_GUID,
--        servicepointid,
--        CGC12,
--        CIRCUITID
--      FROM pgedata.pge_ed16_gis_changes_to_sap ed161,
--        pgedata.pgedata_executed pex
--      WHERE udated_field                      ='CIRCUITID'
--      AND to_date(ed161.postdate,'dd/mm/yyyy')> TO_DATE(pex.lastdate,'dd/mm/yyyy')
--      AND pex.process_name                    ='ED16UPDATESTOSAP'
--      ) chng,
--      edgis.zz_mv_generationinfo eqp
--    WHERE chng.gen_guid= eqp.globalid;
--    UPDATE Pgedata.pgedata_executed pex
--    SET lastdate      =sysdate
--    WHERE process_name='ED16UPDATESTOSAP';
--    COMMIT;
--
--
--
--    --Run for Data Migration
--  ELSIF input_dm_di_flag = 'DM' THEN
--    INSERT
--    INTO pgedata.EDER_TO_SAP_STATUS
--      (
--        equipmentid,
--        guid,
--        spid,
--        current_project,
--        cgc,
--        circuitid,
--        status,
--        status_message,
--        recordid,
--        creationdate,
--        date_inserted,
--        inserted_by
--      )
--    SELECT EQUIPMENT_ID equipmentid ,
--      EQUIP1.GUID,
--      EQUIP1.SPID ,
--      EQUIP1.CURRENT_PROJECT ,
--      EQUIP1.CGC,
--      EQUIP1.CIRCUITID,
--      CASE
--        WHEN EQUIP1.STATUS = 'F'
--        THEN 'E'
--        WHEN EQUIP1.STATUS = 'S'
--        THEN 'S'
--        ELSE 'E'
--      END AS STATUS,
--      EQUIP1.STATUS_MESSAGE,
--      'ED.0.16.' || TO_CHAR( SYSDATE, 'YYYYMMDD.hh24miss.' ) || lpad(EDGIS.ED16_RECORDID_SEQ.nextval,7,0) as recordid,
--      TRUNC(Sysdate) as creationdate,
--      TRUNC(Sysdate) AS Date_inserted,
--      'STAGE'        AS inserted_by
--    FROM
--      (SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
--        EQUIP.GUID,
--        SP2.SERVICEPOINTID SPID,
--        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
--        SP2.CGC12 CGC,
--        SP2.CIRCUITID,
--        EQUIP.STATUS,
--        EQUIP.STATUS_MESSAGE
--      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP,
--        (SELECT PM.CGC12,
--          PM.CIRCUITID,
--          SP.SERVICEPOINTID
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,
--          EDGIS.ZZ_MV_PRIMARYMETER PM
--        WHERE SP.primarymeterguid=PM.globalid
--        ) SP2
--      WHERE SP2.SERVICEPOINTID = EQUIP.Service_point_id
--
--      UNION
--
--      SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
--        EQUIP.GUID,
--        SP1.SERVICEPOINTID SPID,
--        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
--        SP1.CGC12 CGC,
--        SP1.CIRCUITID,
--        EQUIP.STATUS,
--        EQUIP.STATUS_MESSAGE
--      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP,
--        (SELECT TR.CGC12,
--          TR.CIRCUITID,
--          SP.SERVICEPOINTID
--        FROM EDGIS.ZZ_MV_SERVICEPOINT SP,
--          EDGIS.ZZ_MV_TRANSFORMER TR
--        WHERE SP.transformerguid=TR.globalid
--        ) SP1
--      WHERE SP1.SERVICEPOINTID = EQUIP.Service_point_id
--
--      UNION
--       SELECT EQUIP.SAP_EQUIPMENT_ID EQUIPMENT_ID,
--        EQUIP.GUID,
--        EQUIP.SERVICE_POINT_ID SPID,
--        EQUIP.SAP_EGI_NOTIFICATION CURRENT_PROJECT,
--        0 as CGC,
--        null as CIRCUITID,
--        EQUIP.STATUS,
--        EQUIP.STATUS_MESSAGE
--      FROM PGEDATA.GEN_EQUIPMENT_STAGE EQUIP
--      where status='F'
--
--      ) EQUIP1;
--  END IF;
--  STATUS := 'SUCCESS';
--  COMMIT;
--EXCEPTION
--WHEN OTHERS THEN
--  STATUS := SQLERRM;
--
--END EDER_TO_SAP_STATUS_INSERT;
