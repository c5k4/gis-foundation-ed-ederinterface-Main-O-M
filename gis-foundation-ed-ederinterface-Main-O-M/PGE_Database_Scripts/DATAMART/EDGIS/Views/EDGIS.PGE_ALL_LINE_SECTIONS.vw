Prompt drop View PGE_ALL_LINE_SECTIONS;
DROP VIEW EDGIS.PGE_ALL_LINE_SECTIONS
/

/* Formatted on 7/2/2019 01:18:05 PM (QP5 v5.313) */
PROMPT View PGE_ALL_LINE_SECTIONS;
--
-- PGE_ALL_LINE_SECTIONS  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.PGE_ALL_LINE_SECTIONS
(
    GLOBALID,
    CONSTRUCTIONTYPE,
    CUSTOMEROWNED,
    DIVISION,
    CIRCUITID,
    MEASUREDLENGTH,
    INSTALLJOBYEAR,
    INSTALLJOBNUMBER,
    LOCALOFFICEID,
    CEDSALINESECTIONID,
    CEDSANUMBEROFPHASES,
    PHASEDESIGNATION,
    OPERATINGVOLTAGE,
    WINDSPEEDRERATEYEAR,
    SOILCONDITION,
    SPACING,
    UGASTATUS,
    WINDSPEEDCODE,
    DAILYLOADFACTOR,
    TESTRESULT,
    TESTDATE,
    LOADID,
    GEMSDISTMAPNUM,
    PGE_CONDUCTORCODE,
    CONDUCTORUSE,
    SUBTYPECD,
    SOURCESIDEDEVICEID,
    REGION,
    INSTALLATIONDATE
)
AS
    (SELECT a.globalid,
            a.constructiontype,
            a.customerowned,
            a.division,
            a.circuitid,
            a.measuredlength,
            a.installjobyear,
            a.installjobnumber,
            a.localofficeid,
            a.cedsalinesectionid,
            a.cedsanumberofphases,
            a.phasedesignation,
            a.operatingvoltage,
            a.windspeedrerateyear,
            NULL AS soilcondition,
            a.spacing,
            NULL AS ugastatus,
            a.windspeedcode,
            NULL AS dailyloadfactor,
            NULL AS testresult,
            NULL AS testdate,
            s.loadid,
            p.gemsdistmapnum,
            i.pge_conductorcode,
            i.conductoruse,
            NULL AS subtypecd,
            P.sourcesidedeviceid,
            a.region,
            i.installationdate
       FROM edgis.zz_mv_priohconductor  a
            LEFT OUTER JOIN edgis.zz_mv_priohconductorinfo i
                ON a.globalid = i.conductorguid
            LEFT OUTER JOIN edgis.zz_mv_specialload s
                ON a.globalid = s.conductorguid
            LEFT OUTER JOIN edgis.zz_mv_loadcheckpoint p
                ON a.globalid = p.conductorguid)
    UNION
    (SELECT a.globalid,
            a.constructiontype,
            a.customerowned,
            a.division,
            a.circuitid,
            a.measuredlength,
            a.installjobyear,
            a.installjobnumber,
            a.localofficeid,
            a.cedsalinesectionid,
            a.cedsanumberofphases,
            a.phasedesignation,
            a.operatingvoltage,
            NULL AS windspeedrerateyear,
            a.soilcondition,
            NULL AS spacing,
            a.ugastatus,
            NULL AS windspeedcode,
            a.dailyloadfactor,
            c.testresult,
            c.testdate,
            s.loadid,
            p.gemsdistmapnum,
            i.pge_conductorcode,
            i.conductoruse,
            c.subtypecd,
            P.sourcesidedeviceid,
            a.region,
            i.installationdate
       FROM edgis.zz_mv_priugconductor  a
            LEFT OUTER JOIN edgis.zz_mv_priugconductorinfo i
                ON a.globalid = i.conductorguid
            LEFT OUTER JOIN edgis.zz_mv_cabletest c
                ON a.globalid = c.conductorguid
            LEFT OUTER JOIN edgis.zz_mv_specialload s
                ON a.globalid = s.conductorguid
            LEFT OUTER JOIN edgis.zz_mv_loadcheckpoint p
                ON a.globalid = p.conductorguid)
    UNION
    (SELECT a.globalid,
            NULL AS constructiontype,
            a.customerowned,
            a.division,
            a.circuitid,
            a.measuredlength,
            a.installjobyear,
            a.installjobnumber,
            a.localofficeid,
            NULL AS cedsalinesectionid,
            a.cedsanumberofphases,
            a.phasedesignation,
            a.operatingvoltage,
            NULL AS windspeedrerateyear,
            NULL AS soilcondition,
            NULL AS spacing,
            NULL AS ugastatus,
            NULL AS windspeedcode,
            NULL AS dailyloadfactor,
            NULL AS testresult,
            NULL AS testdate,
            NULL AS loadid,
            NULL AS gemsdistmapnum,
            NULL AS pge_conductorcode,
            i.conductoruse,
            NULL AS subtypecd,
            NULL AS sourcesidedeviceid,
            a.region,
            i.installationdate
       FROM edgis.zz_mv_secohconductor  a
            LEFT OUTER JOIN edgis.zz_mv_secohconductorinfo i
                ON a.globalid = i.conductorguid)
    UNION
    (SELECT a.globalid,
            NULL AS constructiontype,
            a.customerowned,
            a.division,
            a.circuitid,
            a.measuredlength,
            a.installjobyear,
            a.installjobnumber,
            a.localofficeid,
            NULL AS cedsalinesectionid,
            a.cedsanumberofphases,
            a.phasedesignation,
            a.operatingvoltage,
            NULL AS windspeedrerateyear,
            NULL AS soilcondition,
            NULL AS spacing,
            NULL AS ugastatus,
            NULL AS windspeedcode,
            NULL AS dailyloadfactor,
            NULL AS testresult,
            NULL AS testdate,
            NULL AS loadid,
            NULL AS gemsdistmapnum,
            NULL AS pge_conductorcode,
            i.conductoruse,
            NULL AS subtypecd,
            NULL AS sourcesidedeviceid,
            a.region,
            i.installationdate
       FROM edgis.zz_mv_secugconductor  a
            LEFT OUTER JOIN edgis.zz_mv_secugconductorinfo i
                ON a.globalid = i.conductorguid)
/


Prompt Grants on VIEW PGE_ALL_LINE_SECTIONS TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.PGE_ALL_LINE_SECTIONS TO BO_USER
/
