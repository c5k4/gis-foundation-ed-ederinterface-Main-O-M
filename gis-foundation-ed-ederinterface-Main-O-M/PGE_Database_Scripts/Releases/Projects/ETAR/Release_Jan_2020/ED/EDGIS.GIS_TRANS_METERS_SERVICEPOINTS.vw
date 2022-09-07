--Create this view in ED database as EDGIS user.
--Get all PRIMARYMETERS AND TRANSFORMERS, get their FeederBY, and join using guid, and get Service PointIDs
--Latest. Jan-2-2020

  CREATE OR REPLACE FORCE VIEW "EDGIS"."GIS_TRANS_METERS_SERVICEPOINTS" ("FEEDERID", "FEEDERFEDBY", "SERVICEPOINTID", "UNIQUESPID", "INSERVICEDATE", "STATUS", "STATUS_DESC", "CITY", "ZIP", "PREMISEID", "TOWNSHIPTERRITORYCODE", "SERVICEAGREEMENTID", "SERVICELOCATIONGUID", "CGC12", "PRIMARYMETERGUID", "TRANSFORMERGUID", "LOCALOFFICEID", "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL") AS 
  (SELECT TT.FEEDERID, TT.FEEDERFEDBY, SP.SERVICEPOINTID,
    sp.uniquespid,
    trunc(sp.inservicedate) inservicedate,
    sp.status,
    cd.description status_desc,
    sp.city,
    sp.zip,
    sp.premiseid,
    sp.townshipterritorycode,
    sp.serviceagreementid,
    sp.servicelocationguid,
    sp.cgc12,
    sp.primarymeterguid,
    sp.transformerguid,
    sp.localofficeid,
    tt.order_num,
    tt.min_branch,
    tt.max_branch,
    tt.treelevel
FROM  
    EDGIS.zz_mv_servicepoint SP, 
    EDGIS.PGE_FEEDERFEDNETWORK_TRACE TT,
    (select code, pge_codes_and_descriptions.description from pge_codes_and_descriptions where upper(domain_name) = 'CONSTRUCTION STATUS') cd
WHERE TT.TO_FEATURE_FCID = 
--This gets FCID of 'EDGIS.PRIMARYMETER' feature class
(select objectid from sde.gdb_items
where UPPER(PHYSICALNAME) = UPPER('EDGIS.TRANSFORMER') 
AND UPPER(PATH) LIKE UPPER('%EDGIS.electricDataset\EDGIS.TRANSFORMER%')) 
AND TT.TO_FEATURE_GLOBALID = SP.TRANSFORMERGUID
--AND TT.FEEDERFEDBY IS NOT NULL
and to_char(sp.status) = cd.code(+))
UNION
(SELECT TT.FEEDERID, TT.FEEDERFEDBY, SP.SERVICEPOINTID,
    sp.uniquespid,
    trunc(sp.inservicedate) inservicedate,
    sp.status,
    cd.description status_desc,
    sp.city,
    sp.zip,
    sp.premiseid,
    sp.townshipterritorycode,
    sp.serviceagreementid,
    sp.servicelocationguid,
    sp.cgc12,
    sp.primarymeterguid,
    sp.transformerguid,
    sp.localofficeid,
    tt.order_num,
    tt.min_branch,
    tt.max_branch,
    tt.treelevel
FROM  
    EDGIS.zz_mv_servicepoint SP, 
    EDGIS.PGE_FEEDERFEDNETWORK_TRACE TT,
    (select code, pge_codes_and_descriptions.description from pge_codes_and_descriptions where upper(domain_name) = 'CONSTRUCTION STATUS') cd
WHERE TT.TO_FEATURE_FCID = 
--This gets FCID of 'EDGIS.PRIMARYMETER' feature class
(select objectid from sde.gdb_items
where UPPER(PHYSICALNAME) = UPPER('EDGIS.PRIMARYMETER') 
AND UPPER(PATH) LIKE UPPER('%EDGIS.electricDataset\EDGIS.PRIMARYMETER%')) 
AND TT.TO_FEATURE_GLOBALID = SP.PRIMARYMETERGUID
--AND TT.FEEDERFEDBY IS NOT NULL
and to_char(sp.status) = cd.code(+));
/

Prompt Grants on VIEW GIS_TRANS_METERS_SERVICEPOINTS TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.GIS_TRANS_METERS_SERVICEPOINTS TO SDE_VIEWER;
/

Prompt Grants on VIEW GIS_TRANS_METERS_SERVICEPOINTS TO ETAR_DM_SUB_DBLNK_RO to ETAR_DM_SUB_DBLNK_RO;
GRANT SELECT ON EDGIS.GIS_TRANS_METERS_SERVICEPOINTS TO ETAR_DM_SUB_DBLNK_RO;
/

--This makes table accessable by our DB-Link.
Prompt Grants on table PGE_FEEDERFEDNETWORK_MAP TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.PGE_FEEDERFEDNETWORK_MAP TO SDE_VIEWER;
