Prompt drop Materialized View STITCHPOINT_FVW;
DROP MATERIALIZED VIEW EDGIS.STITCHPOINT_FVW
/
Prompt Materialized View STITCHPOINT_FVW;
--
-- STITCHPOINT_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.STITCHPOINT_FVW (GLOBALID,OBJECTID,"GISFeatureClass",LOCALOFFICEID,"Location", CIRCUITID,CIRCUITNAME,SUBSTATIONID,SUBSTATIONNAME,STATUS, SUBTYPECD,DIVISION,DISTRICT,CREATIONUSER,DATECREATED, DATEMODIFIED,LASTUSER) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:34 PM (QP5 v5.313) */
(SELECT a.GlobalID,
        a.ObjectID,
        'STITCHPOINT'
            AS "GISFeatureClass",
        a.LocalOfficeID,
        a.LOCDESC
            AS "Location",
        a.CircuitID,
        b.CircuitName,
        b.SubstationID,
        b.SubstationName,
        a.Status,
        a.Subtypecd,
        a.Division,
        a.District,
        (CASE
             WHEN a.CREATIONUSER IS NULL THEN a.LASTUSER
             ELSE a.CREATIONUSER
         END)
            AS "CREATIONUSER",
        (CASE
             WHEN a.DATECREATED IS NULL THEN a.DATEMODIFIED
             ELSE a.DATECREATED
         END)
            AS "DATECREATED",
        (CASE
             WHEN a.DATEMODIFIED IS NULL THEN a.DATECREATED
             ELSE a.DATEMODIFIED
         END)
            AS "DATEMODIFIED",
        (CASE WHEN a.LASTUSER IS NULL THEN a.CREATIONUSER ELSE a.LASTUSER END)
            AS "LASTUSER"
   FROM edgis.zz_MV_ELECTRICSTITCHPOINT a, edgis.zz_MV_CIRCUITSOURCE b
  WHERE a.GLOBALID = b.deviceguid(+))
/


COMMENT ON MATERIALIZED VIEW EDGIS.STITCHPOINT_FVW IS 'snapshot table for snapshot EDGIS.STITCHPOINT_FVW'
/

Prompt Grants on MATERIALIZED VIEW STITCHPOINT_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.STITCHPOINT_FVW TO GIS_SAP_RECON
/
