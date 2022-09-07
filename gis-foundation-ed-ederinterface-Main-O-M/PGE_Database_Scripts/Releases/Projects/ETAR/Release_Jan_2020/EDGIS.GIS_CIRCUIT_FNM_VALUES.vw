CREATE OR REPLACE FORCE VIEW "EDGIS"."GIS_CIRCUIT_FNM_VALUES" ("CIRCUITID", "SUBSTATION_ID", "BUS_ID", "PNODE_ID", "SUBLAP_ID", "LCA_ID") AS 
  select distinct
    edcs1_circuitid circuitid,
    edcs1_substationid substation_id,
    pinfo_bus_id bus_id,
    pinfo_fnm_cnodeid pnode_id,
    pinfo_lap_id sublap_id,
    pinfo_lca_id lca_id  
from edgis.gis_pnode_circuit_info
where 
    (edcs1_circuitid, pinfo_treelevel) in 
    (
        select edcs1_circuitid, pinfo_treelevel from 
        (
            select distinct
                edcs1_circuitid,
                pinfo_bus_id,
                pinfo_fnm_cnodeid,
                pinfo_lap_id,
                pinfo_lca_id,
                pinfo_treelevel
            from edgis.gis_pnode_circuit_info
            where 
                (edcs1_circuitid, pinfo_treelevel) in (select edcs1_circuitid, max(pinfo_treelevel) from edgis.gis_pnode_circuit_info group by edcs1_circuitid)
        ) group by edcs1_circuitid, pinfo_treelevel having count(edcs1_circuitid) = 1
    )
    and pinfo_bus_id is not null
    and pinfo_fnm_cnodeid is not null
    and pinfo_lap_id is not null
    and pinfo_lca_id is not null    
order by 
    edcs1_circuitid;
/

Prompt Grants on VIEW EDGIS.GIS_CIRCUIT_FNM_VALUES TO GIS_SUB_MDSS_RW to GIS_SUB_MDSS_RW;
GRANT SELECT ON EDGIS.GIS_CIRCUIT_FNM_VALUES TO GIS_SUB_MDSS_RW
/

Prompt Grants on VIEW EDGIS.GIS_CIRCUIT_FNM_VALUES TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.GIS_CIRCUIT_FNM_VALUES TO SDE_VIEWER
/