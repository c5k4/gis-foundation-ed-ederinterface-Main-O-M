Prompt drop Procedure CCB_GWOC_INSERT;
DROP PROCEDURE PGEDATA.CCB_GWOC_INSERT
/

Prompt Procedure CCB_GWOC_INSERT;
--
-- CCB_GWOC_INSERT  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA."CCB_GWOC_INSERT"
  (
    version_name IN VARCHAR2)
AS
  count_rcrd NUMBER;
BEGIN
  dbms_output.put_line('Performing Insert to the GWOC_STAGE table');
  dbms_output.put_line('Switch to the created version.');
  --Setting the Version
  sde.version_util.set_current_version(version_name);
  -- START EDITING AGAINST THE VERSION
  dbms_output.put_line('Start editing.');
  sde.version_user_ddl.edit_version(version_name, 1);
  --Truncatin GWOC Stage Table
  dbms_output.put_line('Truncating the GWOC STAGE Table');
  --Delete from PGEDATA.ZZ_MV_GWOC_STAGE;
  --Inserting in GWOC Stage where Action is DELETE
  INSERT
  INTO PGEDATA.ZZ_MV_GWOC_STAGE
    (
      SERVICEPOINTID,
      GENERATIONGUID,
      METERNUMBER,
      STREETNUMBER,
      STREETNAME1,
      CITY,
      STATE,
      ZIP,
      RESOLVED,
      localofficeid,
      cgc12,
      datecreated,
      comments
    )
  SELECT b.servicepointid,
    b.globalid,
    b.meternumber,
    b.streetnumber,
    b.streetname1,
    b.city,
    b.state,
    b.zip,
    'N' AS RESOLVED,
    b.localofficeid,
    b.cgc12,
    sysdate                     AS DATECREATED,
    'This is a new GWOC record' AS comments
  FROM pgedata.pge_ccbtoedgis_stg a,
    (SELECT sp.servicepointid,
      gen.globalid,
      sp.meternumber,
      sp.streetname1,
      sp.streetname2,
      sp.streetnumber,
      sp.city,
      sp.zip,
      sp.state,
      sp.county,
      sp.localofficeid,
      sp.cgc12
    FROM edgis.zz_mv_generationinfo gen,
      edgis.zz_mv_servicepoint sp
    WHERE sp.globalid=gen.servicepointguid
    ) b
  WHERE action         ='D'
  AND a.servicepointid = b.servicepointid;
  COMMIT;
  FOR o IN
  (SELECT * FROM pgedata.zz_mv_gwoc_stage
  )
  LOOP
    count_rcrd:=0;
    SELECT COUNT(*)
    INTO count_rcrd
    FROM edgis.zz_mv_servicepoint sp
    WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
    AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
    AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city));
    IF count_rcrd                      =2 THEN
      UPDATE pgedata.zz_mv_gwoc_stage
      SET comments        ='Multiple records exist for Service Points for same address combination (Streetnumber + Street name1 + City)'
      WHERE servicepointid=o.servicepointid;
    END IF;
  END LOOP;
  COMMIT;
  dbms_output.put_line('Stop editing.');
  sde.version_user_ddl.edit_version(version_name, 2);
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line(SQLERRM);
END CCB_GWOC_INSERT;
/


Prompt Grants on PROCEDURE CCB_GWOC_INSERT TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_INSERT TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCB_GWOC_INSERT TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_INSERT TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCB_GWOC_INSERT TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCB_GWOC_INSERT TO IGPEDITOR
/
