--------------------------------------------------------
--  DDL for Procedure CCB_GEN_NOSP_GWOC_INSERT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCB_GEN_NOSP_GWOC_INSERT" 
AS
BEGIN
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
      DATEFIXED
    )
  SELECT sp1.servicepointid,
    spguid.globalid,
    NULL           AS SAPEGINOTIFICATION,
    TRUNC(sysdate) AS DATECREATED,
    sp1.meternumber ,
    sp1.streetnumber,
    sp1.streetname1,
    sp1.streetname2,
    sp1.city,
    sp1.state,
    sp1.zip,
    'NO'    AS RESOLVED,
    sysdate AS datefixed
  FROM
    (SELECT servicepointguid,
      globalid
    FROM edgis.zz_mv_generationinfo gn
    WHERE NOT EXISTS
      (SELECT 2
      FROM edgis.zz_mv_servicepoint sp
      WHERE sp.globalid =gn.servicepointguid
      )
    ) spguid,
    edgis.zz_mv_servicepoint sp1
  WHERE spguid.servicepointguid=sp1.globalid;
END CCB_GEN_NOSP_GWOC_INSERT;
