Prompt drop View MAINTENANCEPOLYGON_FVW;
DROP VIEW EDGIS.MAINTENANCEPOLYGON_FVW
/

/* Formatted on 7/2/2019 01:18:03 PM (QP5 v5.313) */
PROMPT View MAINTENANCEPOLYGON_FVW;
--
-- MAINTENANCEPOLYGON_FVW  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.MAINTENANCEPOLYGON_FVW
(
    OBJECTID,
    MAPNUMBER,
    MAPTYPE,
    LEGACYMAPID,
    MAPOFFICE,
    LEGACYCOORDINATE,
    GLOBALID,
    CREATIONUSER,
    DATECREATED,
    DATEMODIFIED,
    LASTUSER,
    CONVERSIONID,
    CONVERSIONWORKPACKAGE,
    SUBTYPECD,
    AREANAME,
    URBANRURALCODE,
    PARENTMAPNUMBER,
    DISTRICT,
    LOCALOPOFFICE,
    COUNTY,
    LOCATION,
    SHAPE,
    GLOBALID_1
)
AS
    (SELECT OBJECTID,
            MAPNUMBER,
            MAPTYPE,
            LEGACYMAPID,
            MAPOFFICE,
            LEGACYCOORDINATE,
            GLOBALID,
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
            (CASE
                 WHEN a.LASTUSER IS NULL THEN a.CREATIONUSER
                 ELSE a.LASTUSER
             END)
                AS "LASTUSER",
            CONVERSIONID,
            CONVERSIONWORKPACKAGE,
            SUBTYPECD,
            AREANAME,
            URBANRURALCODE,
            PARENTMAPNUMBER,
            DISTRICT,
            LOCALOPOFFICE,
            COUNTY,
            LOCALOFFICEID
                AS "LOCATION",
            SHAPE,
            GLOBALID_1
       FROM edgis.maintenanceplat a)
/


Prompt Grants on VIEW MAINTENANCEPOLYGON_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.MAINTENANCEPOLYGON_FVW TO GIS_SAP_RECON
/
