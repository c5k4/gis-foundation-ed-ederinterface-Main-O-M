Prompt drop Procedure CCB_GWOC_FROM_STG_INSERT;
DROP PROCEDURE PGEDATA.CCB_GWOC_FROM_STG_INSERT
/

Prompt Procedure CCB_GWOC_FROM_STG_INSERT;
--
-- CCB_GWOC_FROM_STG_INSERT  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA."CCB_GWOC_FROM_STG_INSERT"
  (
    version_name IN VARCHAR2)
AS
  j NUMBER;
BEGIN
  dbms_output.put_line('Performing updates to the GWOC table');
  dbms_output.put_line('Switch to the created version.');
  --Setting the Version
  sde.version_util.set_current_version(version_name);
  -- START EDITING AGAINST THE VERSION
  dbms_output.put_line('Start editing.');
  sde.version_user_ddl.edit_version(version_name, 1);
  FOR gw IN
  (SELECT servicepointid,
    meternumber,
    generationguid,
    streetnumber,
    streetname1,
    streetname2,
    city,
    state,
    zip
  FROM pgedata.ZZ_MV_gwoc_stage
  )
  LOOP
    SELECT COUNT(*)
    INTO j
    FROM edgis.ZZ_MV_servicepoint sp
    WHERE sp.servicepointid = gw.servicepointid;
    IF j                    =0 THEN
      INSERT
      INTO edgis.zz_mv_gwoc
        (
          SERVICEPOINTID,
          GENERATIONGUID,
          SAPEGINOTIFICATION,
          DATECREATED,
          METERNUMBER,
          STREETNUMBER,
          STREETNAME1,
          STREETNAME2,
          CITY,
          STATE,
          ZIP,
          RESOLVED,
          DATEFIXED,
          localofficeid,
          cgc12,
          comments
        )
      SELECT SERVICEPOINTID,
        GENERATIONGUID,
        SAPEGINOTIFICATION,
        DATECREATED,
        METERNUMBER,
        STREETNUMBER,
        STREETNAME1,
        STREETNAME2,
        CITY,
        STATE,
        ZIP,
        RESOLVED,
        DATEFIXED,
        localofficeid,
        cgc12,
        comments
      FROM pgedata.zz_mv_gwoc_stage
      WHERE servicepointid = gw.servicepointid;
      COMMIT;
      DELETE FROM pgedata.ZZ_MV_gwoc_stage WHERE servicepointid = gw.servicepointid;
      update edgis.zz_mv_generationinfo set servicepointguid=null where globalid=gw.generationguid;
      COMMIT;
    END IF;
  END LOOP;
  COMMIT;
  --dbms_output.put_line('Stop editing.');
  sde.version_user_ddl.edit_version(version_name, 2);
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line(SQLERRM);
END CCB_GWOC_FROM_STG_INSERT;
/


Prompt Grants on PROCEDURE CCB_GWOC_FROM_STG_INSERT TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_FROM_STG_INSERT TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCB_GWOC_FROM_STG_INSERT TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_FROM_STG_INSERT TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCB_GWOC_FROM_STG_INSERT TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_FROM_STG_INSERT TO IGPEDITOR
/
