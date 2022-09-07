--This populates Tables required for Tracing Framework.
create or replace PROCEDURE EDGIS.GIS_PNODE_SERVICEPOINT_DATA AS 
BEGIN
COMMIT;

--Insert data into table to hold ED-ServicePoints-Feeder data.
DELETE FROM EDGIS.GIS_SERVICEPOINTS_INFO;
INSERT into EDGIS.GIS_SERVICEPOINTS_INFO
    select * from EDGIS.GIS_TRANS_METERS_SERVICEPOINTS@To_EDDM;
commit;

--Update FeederFedBy where Null
UPDATE EDGIS.GIS_SERVICEPOINTS_INFO GSB
SET GSB.FEEDERFEDBY =
  (SELECT EDV.FROM_CIRCUITID
  FROM EDGIS.PGE_FEEDERFEDNETWORK_MAP@To_EDDM EDV
  WHERE EDV.TO_CIRCUITID = GSB.FEEDERID
  AND EDV.FROM_CIRCUITID IS NOT NULL)
  WHERE GSB.FEEDERFEDBY IS NULL;


--Update GIS_PNODE_CIRCUIT_INFO table with PNODE and Circuit info.  Includes one level of FeederFed Subs.
-- Needs to be modified
delete from edgis.GIS_PNODE_CIRCUIT_INFO;
commit;

insert into edgis.GIS_PNODE_CIRCUIT_INFO

select distinct *

    from
    (
        select distinct

            edcs1.circuitid edcs1_circuitid,
            edcs1.objectid edcs1_objectid,
            edcs1.globalid edcs1_globalid,
            edcs1.substationid edcs1_substationid,
            edcs1.deviceguid edcs1_deviceguid,
            edcs1.status edcs1_status,

            nvl(stpt.circuitid, edcs1.circuitid) edcs2_circuitid,
            edcs2.objectid edcs2_objectid,
            edcs2.globalid edcs2_globalid,
            edcs2.substationid edcs2_substationid,
            edcs2.deviceguid edcs2_deviceguid,
            edcs2.status edcs2_status

        from edgis.circuitsource@to_eddm edcs1
        left outer join edgis.zz_mv_subelectricstitchpoint substpt1 on edcs1.deviceguid = substpt1.electricstitchpointguid
        left outer join edgis.zz_mv_subelectricstitchpoint substpt2 on substpt1.circuitid = substpt2.circuitid
        left outer join edgis.electricstitchpoint@to_eddm stpt on substpt2.electricstitchpointguid = stpt.globalid
        left outer join edgis.circuitsource@to_eddm edcs2 on nvl(stpt.circuitid, edcs1.circuitid) = edcs2.circuitid
        where
            regexp_like(edcs1.circuitid,'^[0-9]*$')
            and edcs1.status = 5
            and substpt1.subtypecd = 1
            and substpt2.subtypecd = 2
    ) edcs
    left outer join
    (
        select
            substpt1.objectid substpt1_objectid,
            substpt1.stitchpointid substpt1_stitchpointid,
            substpt1.globalid substpt1_globalid,
            substpt1.circuitid substpt1_circuitid,
            substpt1.electricstitchpointguid substpt1_elecstitchptguid,

            substpt2.objectid substpt2_objectid,
            substpt2.stitchpointid substpt2_stitchpointid,
            substpt2.globalid substpt2_globalid,
            substpt2.circuitid substpt2_circuitid,
            substpt2.electricstitchpointguid substpt2_elecstitchptguid

        from edgis.zz_mv_subelectricstitchpoint substpt1
        inner join edgis.zz_mv_subelectricstitchpoint substpt2 on substpt1.circuitid = substpt2.circuitid
        where
            substpt1.subtypecd = 1
            and substpt2.subtypecd = 2
    ) substpt on edcs.edcs2_deviceguid = substpt.substpt1_elecstitchptguid
    left outer join
    (
        select
            gistrace.to_feature_globalid gistrace_to_feature_globalid,

            pinfo.line_fc_name pinfo_line_fc_name,
            pinfo.line_guid pinfo_line_guid,
            pinfo.line_oid pinfo_line_oid,
            pinfo.subpnode_oid pinfo_subpnode_oid,
            pinfo.fnm_globalid pinfo_fnm_globalid,
            pinfo.bus_id pinfo_bus_id,
            pinfo.fnm_cnodeid pinfo_fnm_cnodeid,
            pinfo.lap_id pinfo_lap_id,
            pinfo.lca_id pinfo_lca_id,
            pinfo.treelevel pinfo_treelevel,
            pinfo.qa_flag pinfo_qa_flag

        from edgis.pge_feederfednetwork_trace@to_eddm gistrace
        inner join edgis.gis_pnodeinfo pinfo on gistrace.feederid = pinfo.circuitid

        where
            gistrace.order_num < pinfo.order_num
            and gistrace.min_branch >=  pinfo.min_branch
            and gistrace.max_branch <= pinfo.max_branch
            and gistrace.treelevel > pinfo.treelevel
    ) tp on substpt1_globalid = tp.gistrace_to_feature_globalid
    where
        substpt1_elecstitchptguid is not null
    order by
         edcs.edcs1_circuitid;

commit;

/* --Show records where same PNode is connected to different Feeders, JOING ON NODEID AND BUSID, AND set QA_FLAG TO ERROR.
UPDATE EDGIS.GIS_GTB_PNODEINFO UP
SET UP.QA_FLAG = 'ERROR'
WHERE SUBPNODE_OID IN(
Select T1.SUBPNODE_OID
from EDGIS.GIS_GTB_PNODEINFO t1
join
(select * from EDGIS.GIS_GTB_PNODEINFO) t2
on (t1.PNODE_CNODEID = t2.PNODE_CNODEID and
      t1.BUS_ID = t2.BUS_ID)
where (t1.CIRCUITID != t2.CIRCUITID));
COMMIT; */
END GIS_PNODE_SERVICEPOINT_DATA;
/

Prompt Grants on PROCEDURE EDGIS.GIS_PNODE_SERVICEPOINT_DATA TO EDGISSUB_BO_RO to EDGISSUB_BO_RO;
GRANT EXECUTE ON EDGIS.GIS_PNODE_SERVICEPOINT_DATA TO EDGISSUB_BO_RO
/

Prompt Grants on PROCEDURE EDGIS.GIS_PNODE_SERVICEPOINT_DATA TO GIS_SUB_MDSS_RW to GIS_SUB_MDSS_RW;
GRANT EXECUTE ON EDGIS.GIS_PNODE_SERVICEPOINT_DATA TO GIS_SUB_MDSS_RW
/