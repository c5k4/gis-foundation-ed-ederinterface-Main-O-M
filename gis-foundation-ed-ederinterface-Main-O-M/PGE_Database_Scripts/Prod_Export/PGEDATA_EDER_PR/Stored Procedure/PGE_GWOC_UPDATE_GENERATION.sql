--------------------------------------------------------
--  DDL for Procedure PGE_GWOC_UPDATE_GENERATION
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."PGE_GWOC_UPDATE_GENERATION" 
  (
    version_name IN VARCHAR2)
AS
  i            NUMBER;
  count_rcrd   NUMBER;
  sp_guid      VARCHAR2(100);
  h_label_text VARCHAR2(200);
  SLGUID       VARCHAR2(100);
BEGIN
  dbms_output.put_line('Performing updates to the GENERATIONINFO table');
  dbms_output.put_line('Switch to the created version.');
  --Setting the Version
  sde.version_util.set_current_version(version_name);
  -- START EDITING AGAINST THE VERSION
  dbms_output.put_line('Start editing.');
  sde.version_user_ddl.edit_version(version_name, 1);
  FOR o IN
  (SELECT servicepointid,
    meternumber,
    generationguid,
    streetnumber,
    streetname1,
    streetname2,
    city,
    state,
    zip,
    comments
  FROM edgis.zz_mv_gwoc
  WHERE UPPER(RESOLVED) = 'N'
  )
  LOOP
    count_rcrd:=0;
    SELECT COUNT(*)
    INTO count_rcrd
    FROM edgis.zz_mv_generationinfo gen
    WHERE gen.globalid = o.generationguid;
    IF count_rcrd      = 0 THEN
      -- Delete the GWOC record
      DELETE
      FROM edgis.zz_mv_gwoc
      WHERE generationguid=o.generationguid;
      --RETURN;
    ELSE
      SELECT COUNT(*)
      INTO count_rcrd
      FROM edgis.zz_mv_servicepoint
      WHERE globalid IN
        (SELECT servicepointguid
        FROM edgis.zz_mv_generationinfo
        WHERE globalid=o.generationguid
        );
      IF count_rcrd = 1 THEN
        --Resolve the GWOC record by setting the Status Y
        UPDATE edgis.zz_mv_gwoc
        SET RESOLVED        = 'Y',
          datefixed         =TRUNC(sysdate),
          comments          ='This GWOC record is resolved based on the generation already exist for the service point id'
        WHERE generationguid=o.generationguid;
        COMMIT;
      ELSE
      SELECT COUNT(*)
      INTO count_rcrd
      FROM edgis.zz_mv_generationinfo
      WHERE servicepointguid IN
        (SELECT globalid
        FROM edgis.zz_mv_servicepoint
        WHERE servicepointid=o.servicepointid
        );
         IF count_rcrd = 1 THEN
        --Resolve the GWOC record by setting the Status Y
        UPDATE edgis.zz_mv_gwoc
        SET RESOLVED        = 'N',
          comments          ='This GWOC is not resolved because the identified servicepoint already has generation'
        WHERE generationguid=o.generationguid;
        COMMIT;
        ELSE
        SELECT COUNT(*)
        INTO count_rcrd
        FROM edgis.zz_mv_servicepoint sp
        WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
        AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
        AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city))
        AND o.COMMENTS != 'Multiple records exist for Service Points for same address combination (Streetnumber + Street name1 + City)';
        IF count_rcrd                      = 1 THEN
          UPDATE edgis.zz_mv_generationinfo geninfo
          SET geninfo.servicepointguid =
            (SELECT sp.globalid
            FROM EDGIS.ZZ_MV_servicepoint sp
            WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
            AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
            AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city))
            )
          WHERE geninfo.globalid = o.generationguid;--(select sp.globalid from EDGIS.servicepoint sp where sp.servicepointid = o.servicepointid);
          i                     := sql%rowcount;
          COMMIT;
          IF i=1 THEN
            UPDATE edgis.zz_mv_gwoc
            SET RESOLVED        ='Y',
              datefixed         =TRUNC(sysdate),
              comments          = 'This GWOC record is resolved based on the unique match found in table servicepoint from combination STREETNUMBER, STREETNAME1, CITY, METERNUMBER'
            WHERE servicepointid=o.servicepointid;
            COMMIT;

          END IF;

        ELSE
          SELECT COUNT(*)
          INTO count_rcrd
          FROM edgis.zz_mv_servicepoint sp
          WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
          AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
          AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city))
          AND SP.METERNUMBER                 = o.meternumber
          AND o.COMMENTS != 'Multiple records exist for Service Points for same address combination (Streetnumber + Street name1 + City)';
          IF count_rcrd                      = 1 THEN
             UPDATE edgis.zz_mv_generationinfo geninfo
            SET geninfo.servicepointguid =
              (SELECT sp.globalid
              FROM EDGIS.ZZ_MV_servicepoint sp
              WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
              AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
              AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city))
              AND sp.meternumber                 = o.meternumber
              )
            WHERE geninfo.globalid = o.generationguid
            and geninfo.servicepointguid not in (SELECT sp.globalid
              FROM EDGIS.ZZ_MV_servicepoint sp
              WHERE UPPER(TRIM(sp.streetnumber)) = UPPER(TRIM(o.streetnumber))
              AND UPPER(TRIM(sp.streetname1))    = UPPER(TRIM(o.streetname1))
              AND UPPER(TRIM(sp.city))           = UPPER(TRIM(o.city))
              AND sp.meternumber                 = o.meternumber
              )
            ;--(select sp.globalid from EDGIS.servicepoint sp where sp.servicepointid = o.servicepointid);
            i                     := sql%rowcount;
            COMMIT;
            IF i=1 THEN
              UPDATE edgis.zz_mv_gwoc
              SET RESOLVED        ='Y',
                datefixed         =TRUNC(sysdate),
                comments          = 'This GWOC record is resolved based on the unique match found in table servicepoint from combination of METERNUMBER'
              WHERE servicepointid=o.servicepointid;
              COMMIT;
else  UPDATE edgis.zz_mv_gwoc
              SET RESOLVED        ='N',
                comments          = 'This GWOC is not resolved because the identified servicepoint already has generation'
              WHERE servicepointid=o.servicepointid;
              COMMIT;
            END IF;
          ELSE
            IF count_rcrd =0 THEN
              UPDATE edgis.zz_mv_gwoc
              SET RESOLVED        ='N',
                comments          = 'No Match found in table servicepoint from combination STREETNUMBER, STREETNAME1, CITY, METERNUMBER'
              WHERE servicepointid=o.servicepointid;
              COMMIT;
            ELSE
              UPDATE edgis.zz_mv_gwoc
              SET RESOLVED        ='N',
                comments          = 'Multiple matches found in service point from combination STREETNUMBER, STREETNAME1, CITY, METERNUMBER'
              WHERE servicepointid=o.servicepointid;
              COMMIT;
            END IF;
          END IF;
        END IF;
      END IF;
      END IF;
    END IF;
  END LOOP;
  COMMIT;
  dbms_output.put_line('Stop editing.');
  sde.version_user_ddl.edit_version(version_name, 2);
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line(SQLERRM);
END PGE_GWOC_UPDATE_GENERATION;
