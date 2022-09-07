

spool D:\Temp\StoredProceduresChanges.txt

---- Adding  new procedures --------------
--GIS_CIRCUIT_CIRCUIT_REFRESH
--GIS_CIRCUIT_FNM_LCA_REFRESH
--GIS_PNODE_CIRCUIT_INFO_REFRESH

CREATE OR REPLACE procedure gis_circuit_circuit_refresh as
/*
    add description 
*/

cursor c1 is
    select distinct
        circuitid
    from edgis.circuitsource@to_eddm
    where 
        regexp_like(circuitid,'^[0-9]*$')
        and status = 5
    order by 
        circuitid;

    -- declares local variables
    v_circuitid varchar2(9 byte);
    v_downstream_circuitid	varchar2(9 byte);
    v_upstream_circuitid	varchar2(9 byte);
    v_level_num	number;

begin
    
    --verify temporary table is empty
    delete from edgis.gis_circuit_circuit_temp;
    commit;

    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;
    
    -- assigns initial values
    v_circuitid := '0';
    v_downstream_circuitid	:= '0';
    v_upstream_circuitid	:= '0';
    v_level_num := 0;
    
    delete from edgis.gis_circuit_circuit_temp;
   
    open c1;
        loop
             
            fetch c1 into v_circuitid; -- fetches the cursor c1 and assigns values to variables.
                if c1%notfound -- if no more feeders then exit
                    then
                        exit;
                end if;
            
            v_level_num := 0;
            v_downstream_circuitid := v_circuitid;
            
                loop
                    
                    begin         
                        select distinct  
                            nvl(edcs2.circuitid, edcs1.circuitid) into v_upstream_circuitid
                        from edgis.circuitsource@to_eddm edcs1 
                        left outer join edgis.zz_mv_subelectricstitchpoint substpt1 on edcs1.deviceguid = substpt1.electricstitchpointguid
                        left outer join edgis.zz_mv_subelectricstitchpoint substpt2 on substpt1.circuitid = substpt2.circuitid 
                        left outer join edgis.electricstitchpoint@to_eddm stpt on substpt2.electricstitchpointguid = stpt.globalid
                        left outer join edgis.circuitsource@to_eddm edcs2 on nvl(stpt.circuitid, edcs1.circuitid) = edcs2.circuitid
                        where 
                            regexp_like(edcs1.circuitid,'^[0-9]*$')
                            and edcs1.status = 5
                            and (substpt1.subtypecd = 1) -- or substpt1.subtypecd is null)
                            and (substpt2.subtypecd = 2) -- or substpt2.subtypecd is null)
                            and edcs1.circuitid = v_downstream_circuitid;
                                                                                                                                                                                                                                                                                        
                        exception 
                            when no_data_found then exit;
                            when others then exit;
                    end;

                    insert into edgis.gis_circuit_circuit_temp
                        (
                            circuitid,
                            downstream_circuitid,
                            upstream_circuitid,
                            level_num
                        )
                        values
                        (              
                            v_circuitid,
                            v_downstream_circuitid,
                            v_upstream_circuitid,
                            v_level_num
                        );
                   
                    exit when v_downstream_circuitid = v_upstream_circuitid;
                    
                    v_level_num := v_level_num + 1;    
                    v_downstream_circuitid	:= v_upstream_circuitid;
                                
                end loop;
                commit;
                
                v_circuitid := '0';
                v_downstream_circuitid	:= '0';
                v_upstream_circuitid	:= '0';
                v_level_num := 0;

        end loop;

        commit; 
    close c1;

    -- the following updates the main table
    -- merge is not needed because rows are only inserted or deleted.  Rows are never updated.

    delete edgis.gis_circuit_circuit  -- this may not be neessary becuase of the delete statement below
        where circuitid not in (select circuitid from edgis.gis_circuit_circuit_temp);
    commit;

    delete edgis.gis_circuit_circuit
        where 
            (circuitid, downstream_circuitid, upstream_circuitid, level_num) not in (select circuitid, downstream_circuitid, upstream_circuitid, level_num from edgis.gis_circuit_circuit_temp);
    commit;    

    -- This insert must run after the delete above.
    insert into edgis.gis_circuit_circuit
    (
        circuitid,
        downstream_circuitid,
        upstream_circuitid,
        level_num
    )
    (
        select 
            circuitid,
            downstream_circuitid,
            upstream_circuitid,
            level_num
        from edgis.gis_circuit_circuit_temp
        where circuitid not in (select circuitid from edgis.gis_circuit_circuit)
    );
    commit;
    
    --empty the temporary table
    delete from edgis.gis_circuit_circuit_temp;
    commit;
    
end gis_circuit_circuit_refresh;
/



create or replace procedure       gis_circuit_fnm_lca_refresh as

-- This SP populates table edgis.gis_circuit_fnm_lca_temp with the current active feeders and populates the FNM values with all past, current,and future values.
-- The records with a 'Y' in the EXCLUDE field for table edgis.gis_pnode_circuit_info are not inserted into dgis.gis_circuit_fnm_lca_temp.
-- Delete the feeders that are in edgis.gis_circuit_fnm_lca but not in edgis.gis_circuit_fnm_lca_temp.
-- Delete those rows from dgis.gis_circuit_fnm_lca where the feeder is in both edgis.gis_circuit_fnm_lca and edgis.gis_circuit_fnm_lca_temp but certain values do not match.
-- Insert into edgis.gis_circuit_fnm_lca those records from edgis.gis_circuit_fnm_lca_temp where the circuit is in edgis.gis_circuit_fnm_lca_temp but not i9n edgis.gis_circuit_fnm_lca.

begin
    
    --verify that the temporary table is empty
    delete from edgis.gis_circuit_fnm_lca_temp;
    commit;
    
    insert into edgis.gis_circuit_fnm_lca_temp
        select distinct
            a.circuitid,
            a.upstream_circuitid source_feeder,
            c.bus_id,
            c.cnode_id,
            c.lap_id,
            c.lca_id,
            c.start_date,
            c.end_date
        from edgis.gis_circuit_circuit_compressed a 
        inner join edgis.gis_pnode_circuit_info b on a.upstream_circuitid = b.edcs_circuitid
        inner join edgis.gis_fnm_lca c on b.pinfo_bus_id = c.bus_id and b.pinfo_cnode_id = c.cnode_id
        where
            (b.edcs_circuitid, b.pinfo_treelevel) in (select edcs_circuitid, max(pinfo_treelevel) from edgis.gis_pnode_circuit_info group by edcs_circuitid)
            and (upper(b.exclude) <> 'Y' or b.exclude is null)
        order by 
            a.circuitid,
            c.start_date;
    
    commit;

    -- the following updates the main table
    -- merge is not needed because rows are only inserted or deleted.  Rows are never updated.

    delete edgis.gis_circuit_fnm_lca  -- this may not be neessary becuase of the delete statement below
        where circuitid not in (select circuitid from edgis.gis_circuit_fnm_lca_temp);
    commit;

    delete edgis.gis_circuit_fnm_lca
        where 
            (circuitid|| source_feeder|| bus_id|| cnode_id|| lap_id|| lca_id|| start_date|| end_date) 
            not in (select circuitid|| source_feeder|| bus_id|| cnode_id|| lap_id|| lca_id|| start_date|| end_date from edgis.gis_circuit_fnm_lca_temp);
    commit;    

    -- This insert must run after the delete above.
    insert into edgis.gis_circuit_fnm_lca
    (
        circuitid, 
        source_feeder, 
        bus_id, 
        cnode_id, 
        lap_id, 
        lca_id, 
        start_date, 
        end_date
    )
    (
        select 
            circuitid, 
            source_feeder, 
            bus_id, 
            cnode_id, 
            lap_id, 
            lca_id, 
            start_date, 
            end_date
        from edgis.gis_circuit_fnm_lca_temp
        where circuitid not in (select circuitid from edgis.gis_circuit_fnm_lca)
    );
    commit;
   
    --empty the temporary table
    delete from edgis.gis_circuit_fnm_lca_temp;
    commit;

end gis_circuit_fnm_lca_refresh;
/



create or replace PROCEDURE GIS_PNODE_CIRCUIT_INFO_REFRESH AS 
begin

--Update GIS_PNODE_CIRCUIT_INFO table with PNODE and Circuit info.  
--Determines the first feeder that is down load from the pnode.
--if more that one pnode on the circuit then it returns the last pnode (furthest down load) on the cirtcuit.

-- verify temporary tabel is empty
    delete from edgis.gis_pnode_circuit_info_temp;
    commit;

    insert into edgis.gis_pnode_circuit_info_temp

        select distinct
            edcs.circuitid edcs_circuitid,
            edcs.objectid edcs_objectid,
            edcs.globalid edcs_globalid,
            edcs.substationid edcs_substationid,
            edcs.deviceguid edcs_deviceguid,
            edcs.status edcs_status,

            substpt.objectid substpt_objectid,
            substpt.stitchpointid substpt_stitchpointid,
            substpt.globalid substpt_globalid,
            substpt.circuitid substpt_circuitid,
            substpt.electricstitchpointguid substpt_elecstitchptguid,

            gistrace.to_feature_globalid gistrace_to_feature_globalid,
            gistrace.feederid gistrace_feederid,
            gistrace.order_num gistrace_order_num,
            gistrace.min_branch gistrace_min_branch,
            gistrace.max_branch gistrace_max_branch,
            gistrace.treelevel gistrace_treelevel,

            pinfo.circuitid pinfo_circuit_id,
            pinfo.line_fc_name pinfo_line_fc_name,
            pinfo.line_guid pinfo_line_guid,
            pinfo.line_oid pinfo_line_oid,
            pinfo.subpnode_oid pinfo_subpnode_oid,
            pinfo.fnm_globalid pinfo_fnm_globalid,
            pinfo.bus_id pinfo_bus_id,
            pinfo.fnm_cnodeid pinfo_cnodeid,
            pinfo.order_num pinfo_order_num,
            pinfo.min_branch pinfo_min_branch,
            pinfo.max_branch pinfo_max_branch,
            pinfo.treelevel pinfo_treelevel, 
            
            null as pinfo_qa_flag,
            null as exclude

        from edgis.zz_mv_circuitsource@to_eddm edcs
        inner join edgis.zz_mv_subelectricstitchpoint substpt on edcs.deviceguid = substpt.electricstitchpointguid
        inner join edgis.pge_feederfednetwork_trace@to_eddm gistrace on substpt.globalid = gistrace.to_feature_globalid
        inner join edgis.gis_pnodeinfo pinfo on gistrace.feederid = pinfo.circuitid
        where
            regexp_like(edcs.circuitid,'^[0-9]*$')
            and edcs.status = 5
            and substpt.subtypecd = 1
            and gistrace.order_num < pinfo.order_num
            and gistrace.min_branch >=  pinfo.min_branch
            and gistrace.max_branch <= pinfo.max_branch
            and gistrace.treelevel > pinfo.treelevel
            and (upper(pinfo.qa_flag) not like '%ERROR%' or upper(pinfo.qa_flag) is null)
        order by
                 edcs.circuitid;

    commit;

    -- sets a flag indicating that there is a problem in determining the pnode / bus combination.
    update edgis.gis_pnode_circuit_info_temp
        set pinfo_qa_flag = 'Error: Same circuitid and pinfo_treelevel but differnt bus and pnode',
        exclude = 'Y'
    where 
        (edcs_circuitid, pinfo_treelevel) in 
            (
                select edcs_circuitid, pinfo_treelevel 
                from (select distinct edcs_circuitid, pinfo_treelevel, pinfo_bus_id, pinfo_cnode_id from edgis.gis_pnode_circuit_info_temp) 
                group by edcs_circuitid, pinfo_treelevel 
                having count(edcs_circuitid) > 1
            );
    commit;

    -- similar to above but the multiple rows return the same bus and pnode.  Really a warning.
    update edgis.gis_pnode_circuit_info_temp
        set pinfo_qa_flag = 'Warning: Same circuitid and pinfo_treelevel but same bus and pnode'
    where 
        (edcs_circuitid, pinfo_treelevel) in 
            (
                select edcs_circuitid, pinfo_treelevel 
                from edgis.gis_pnode_circuit_info_temp 
                group by edcs_circuitid, pinfo_treelevel 
                having count(edcs_circuitid) > 1
            )
        and exclude is null;
    commit;

    -- the following updates the main table
    -- merge is not needed because rows are only inserted or deleted.  Rows are never updated.

    delete edgis.gis_pnode_circuit_info  -- this may not be neessary becuase of the delete statement below
        where edcs_circuitid not in (select edcs_circuitid from edgis.gis_pnode_circuit_info_temp);
    commit;

    delete edgis.gis_pnode_circuit_info
        where 
        (
            edcs_circuitid||
            edcs_objectid||
            edcs_globalid||
            edcs_substationid||
            edcs_deviceguid||
            edcs_status||
            substpt_objectid||
            substpt_stitchpointid||
            substpt_globalid||
            substpt_circuitid||
            substpt_elecstitchptguid||
            gistrace_to_feature_globalid||
            gistrace_feederid||
            gistrace_order_num||
            gistrace_min_branch||
            gistrace_max_branch||
            gistrace_treelevel||
            pinfo_circuit_id||
            pinfo_line_fc_name||
            pinfo_line_guid||
            pinfo_line_oid||
            pinfo_subpnode_oid||
            pinfo_fnm_globalid||
            pinfo_bus_id||
            pinfo_cnode_id||
            pinfo_order_num||
            pinfo_min_branch||
            pinfo_max_branch||
            pinfo_treelevel||
            pinfo_qa_flag||
            exclude
        ) 
        not in 
        (
            select 
                edcs_circuitid||
                edcs_objectid||
                edcs_globalid||
                edcs_substationid||
                edcs_deviceguid||
                edcs_status||
                substpt_objectid||
                substpt_stitchpointid||
                substpt_globalid||
                substpt_circuitid||
                substpt_elecstitchptguid||
                gistrace_to_feature_globalid||
                gistrace_feederid||
                gistrace_order_num||
                gistrace_min_branch||
                gistrace_max_branch||
                gistrace_treelevel||
                pinfo_circuit_id||
                pinfo_line_fc_name||
                pinfo_line_guid||
                pinfo_line_oid||
                pinfo_subpnode_oid||
                pinfo_fnm_globalid||
                pinfo_bus_id||
                pinfo_cnode_id||
                pinfo_order_num||
                pinfo_min_branch||
                pinfo_max_branch||
                pinfo_treelevel||
                pinfo_qa_flag||
                exclude 
            from edgis.gis_pnode_circuit_info_temp
        );
    commit;    

    -- This insert must run after the delete above.
    insert into edgis.gis_pnode_circuit_info
    (
        edcs_circuitid,
        edcs_objectid,
        edcs_globalid,
        edcs_substationid,
        edcs_deviceguid,
        edcs_status,
        substpt_objectid,
        substpt_stitchpointid,
        substpt_globalid,
        substpt_circuitid,
        substpt_elecstitchptguid,
        gistrace_to_feature_globalid,
        gistrace_feederid,
        gistrace_order_num,
        gistrace_min_branch,
        gistrace_max_branch,
        gistrace_treelevel,
        pinfo_circuit_id,
        pinfo_line_fc_name,
        pinfo_line_guid,
        pinfo_line_oid,
        pinfo_subpnode_oid,
        pinfo_fnm_globalid,
        pinfo_bus_id,
        pinfo_cnode_id,
        pinfo_order_num,
        pinfo_min_branch,
        pinfo_max_branch,
        pinfo_treelevel,
        pinfo_qa_flag,
        exclude
    )
    (
        select 
            edcs_circuitid,
            edcs_objectid,
            edcs_globalid,
            edcs_substationid,
            edcs_deviceguid,
            edcs_status,
            substpt_objectid,
            substpt_stitchpointid,
            substpt_globalid,
            substpt_circuitid,
            substpt_elecstitchptguid,
            gistrace_to_feature_globalid,
            gistrace_feederid,
            gistrace_order_num,
            gistrace_min_branch,
            gistrace_max_branch,
            gistrace_treelevel,
            pinfo_circuit_id,
            pinfo_line_fc_name,
            pinfo_line_guid,
            pinfo_line_oid,
            pinfo_subpnode_oid,
            pinfo_fnm_globalid,
            pinfo_bus_id,
            pinfo_cnode_id,
            pinfo_order_num,
            pinfo_min_branch,
            pinfo_max_branch,
            pinfo_treelevel,
            pinfo_qa_flag,
            exclude
        from edgis.gis_pnode_circuit_info_temp
        where edcs_circuitid not in (select edcs_circuitid from edgis.gis_pnode_circuit_info)
    );
    commit;

--delete rows from temporary table
    delete from edgis.gis_pnode_circuit_info_temp;
    commit;

end GIS_PNODE_CIRCUIT_INFO_REFRESH;
/



GRANT EXECUTE ON GIS_CIRCUIT_CIRCUIT_REFRESH TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_CIRCUIT_FNM_LCA_REFRESH TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_PNODE_CIRCUIT_INFO_REFRESH TO GIS_SUB_MDSS_RW;

---- Adding  new procedures --------------


---- Updating existing procedures --------------

create or replace PROCEDURE GIS_COMPLETE_PNODEINFO AS

--This SP runs after edgis.gis_pnodeinfo has been updated by the executable called SPATIAL INTERSECT PROCESS.
--This SP obtains data from tables that did not make sense to include in the executable.

BEGIN
--sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

--This gets TREELEVEL, order_num, min_branch, max_branch from TRACING TABLE IN ED.

/*update edgis.gis_pnodeinfo gsb
set (gsb.treelevel, gsb.order_num, gsb.min_branch, gsb.max_branch) =
    (
        select edv.treelevel, edv.order_num, edv.min_branch, edv.max_branch
        from edgis.pge_feederfednetwork_trace@to_eddm edv
        where edv.to_feature_globalid = gsb.line_guid
    )
where exists (select 1 from edgis.pge_feederfednetwork_trace@to_eddm where to_feature_globalid = gsb.line_guid);*/

delete from edgis.gis_pnodeinfo_temp;

insert into edgis.gis_pnodeinfo_temp
select distinct edv.treelevel, edv.order_num, edv.min_branch, edv.max_branch,edv.to_feature_globalid
from edgis.pge_feederfednetwork_trace@to_eddm edv
where edv.to_feature_globalid in (select line_guid from edgis.gis_pnodeinfo);

--------This part is to capture unusual data conditions ---------------------------------------------------------------------

update edgis.gis_pnodeinfo set qa_flag =NULL;

update edgis.gis_pnodeinfo set qa_flag ='Error: More than one row returned from trace table for line guid while updating treelevel,order_num,min_branch,max_branch'
where line_guid in
(
select to_feature_globalid from edgis.gis_pnodeinfo_temp
group by to_feature_globalid having count(*) >1
);

delete from edgis.gis_pnodeinfo_temp where to_feature_globalid in
(
select to_feature_globalid from edgis.gis_pnodeinfo_temp
group by to_feature_globalid having count(*) >1
);

--------This part is to capture unusual data conditions ---------------------------------------------------------------------

update edgis.gis_pnodeinfo gsb
set (gsb.treelevel, gsb.order_num, gsb.min_branch, gsb.max_branch) =
    (
        select tmptable.treelevel, tmptable.order_num, tmptable.min_branch, tmptable.max_branch
        from edgis.gis_pnodeinfo_temp tmptable
        where tmptable.to_feature_globalid = gsb.line_guid
    )
where exists (select 1 from edgis.gis_pnodeinfo_temp where to_feature_globalid = gsb.line_guid);

delete from edgis.gis_pnodeinfo_temp;

update edgis.gis_pnodeinfo set qa_flag=qa_flag||' Error: NULL value found for one or more columns - treelevel,order_num,min_branch,max_branch' 
where treelevel is null or order_num is null or min_branch is null or max_branch is null;

commit;

--Updates FNM_CNODEID, BUS_ID, FNM_GLOBALID, LCA_ID, LAP_ID from FNM.

/*update edgis.gis_pnodeinfo gsb
set (gsb.fnm_cnodeid, gsb.bus_id, gsb.fnm_globalid, gsb.lca_id, gsb.lap_id) =
    (
        select fn.cnode_id, fn.bus_id, fn.globalid, fn.lca_id, fn.lap_id
        from edgis.fnm fn
        where fn.globalid = gsb.pnode_fnmguid
    )
where exists (select 1 from edgis.fnm where globalid = gsb.pnode_fnmguid);
commit;*/


delete from edgis.gis_pnodeinfo_temp2;

insert into edgis.gis_pnodeinfo_temp2
select distinct fn.cnode_id, fn.bus_id,fn.lca_id, fn.lap_id,fn.globalid as FNM_GLOBALID from edgis.fnm fn
where globalid in (select pnode_fnmguid from edgis.gis_pnodeinfo)
group by fn.cnode_id, fn.bus_id,fn.lca_id, fn.lap_id,fn.globalid;

--------This part is to capture unusual data conditions ---------------------------------------------------------------------
update edgis.gis_pnodeinfo set qa_flag =qa_flag ||' Error: More than one row returned from FNM table for pnode_fnmguid while updating fnm_cnodeid,bus_id,fnm_globalid,lca_id,lap_id'
where pnode_fnmguid in
(
select FNM_GLOBALID from edgis.gis_pnodeinfo_temp2
group by FNM_GLOBALID having count(*) >1
);

delete from edgis.gis_pnodeinfo_temp2 where FNM_GLOBALID in
(
select FNM_GLOBALID from edgis.gis_pnodeinfo_temp2
group by FNM_GLOBALID having count(*) >1
);

--------This part is to capture unusual data conditions ---------------------------------------------------------------------

update edgis.gis_pnodeinfo gsb
set (gsb.fnm_cnodeid, gsb.bus_id, gsb.fnm_globalid, gsb.lca_id, gsb.lap_id) =
    (
        select fn.cnode_id, fn.bus_id, fn.FNM_GLOBALID, fn.lca_id, fn.lap_id
        from edgis.gis_pnodeinfo_temp2 fn
        where fn.FNM_GLOBALID = gsb.pnode_fnmguid
    )
where exists (select 1 from edgis.gis_pnodeinfo_temp2 where FNM_GLOBALID = gsb.pnode_fnmguid);

delete from edgis.gis_pnodeinfo_temp2;

update edgis.gis_pnodeinfo set qa_flag=qa_flag||' Error: NULL value found for one or more columns - fnm_cnodeid,bus_id,fnm_globalid,lca_id,lap_id' 
where fnm_cnodeid is null or bus_id is null or fnm_globalid is null or lca_id is null or lap_id is null;

commit;

/*
THE TWO STATEMENTS ABOVE REPLACE THOSE BELOW.

--This gets TREELEVEL from TRACING TABLE IN ED.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.TREELEVEL =
  (SELECT EDV.TREELEVEL
  FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE@To_EDDM EDV
  WHERE EDV.to_feature_globalid = GSB.LINE_GUID);
  COMMIT;

--This gets ORDER_NUM from TRACING TABLE IN ED.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.ORDER_NUM =
  (SELECT EDV.ORDER_NUM
  FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE@To_EDDM EDV
  WHERE EDV.to_feature_globalid = GSB.LINE_GUID);
  COMMIT;

--This gets MIN_BRANCH from TRACING TABLE IN ED.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.MIN_BRANCH =
  (SELECT EDV.MIN_BRANCH
  FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE@To_EDDM EDV
  WHERE EDV.to_feature_globalid = GSB.LINE_GUID);
  COMMIT;

--This gets MAX_BRANCH from TRACING TABLE IN ED.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.MAX_BRANCH =
  (SELECT EDV.MAX_BRANCH
  FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE@To_EDDM EDV
  WHERE EDV.to_feature_globalid = GSB.LINE_GUID);
  COMMIT;

--Updates FNM_CNODEID from FNM.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.FNM_CNODEID =
  (SELECT FN.CNODE_ID
  FROM EDGIS.FNM FN
  WHERE GSB.PNODE_FNMGUID = FN.GLOBALID);
  COMMIT;

--Updates BUS_ID from FNM.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.BUS_ID =
  (SELECT FN.BUS_ID
  FROM EDGIS.FNM FN
  WHERE GSB.PNODE_FNMGUID = FN.GLOBALID);
  COMMIT;

--Updates FNM_GLOBALID from FNM.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.FNM_GLOBALID =
  (SELECT FN.GLOBALID
  FROM EDGIS.FNM FN
  WHERE GSB.PNODE_FNMGUID = FN.GLOBALID);
  COMMIT;

--Updates LCA_ID from FNM.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.LCA_ID =
  (SELECT FN.LCA_ID
  FROM EDGIS.FNM FN
  WHERE GSB.PNODE_FNMGUID = FN.GLOBALID);
  COMMIT;

--Updates LAP_ID from FNM.
UPDATE EDGIS.GIS_PNODEINFO GSB
SET GSB.LAP_ID =
  (SELECT FN.LAP_ID
  FROM EDGIS.FNM FN
  WHERE GSB.PNODE_FNMGUID = FN.GLOBALID);
  COMMIT;
*/

--FOR NOW: removing this, since this is not valid for edge-cases.
----The same FeederFedBy / CircuitID is sometimes assigned to multiple substation banks and transmission / distribution interpace poins.
----This causes multiple pnodes to be returned for a given FeederFedBy / CircuitID when only one pnode is allowed.
----The following sql resolves the issues by deleting those pnodes that are not the first pnode found on up-stream trace.
----The pnode with the max TreeLevel is retained while the other pnodes are deleted from the table.
--DELETE FROM EDGIS.GIS_PNODEINFO PN1
--WHERE PN1.TREELEVEL <(
--	SELECT MAX(TREELEVEL)
--	FROM EDGIS.GIS_PNODEINFO PN2
--	WHERE PN1.CircuitID = PN2.CircuitID
--	AND PN1.BUS_ID != PN2.BUS_ID
--	AND PN1.FNM_CNODEID != PN2.FNM_CNODEID
--  );
--  COMMIT;

/* Removed because this can be obtained via a report
--Show records where same PNode is connected to different Feeders, JOING ON NODEID AND BUSID, AND set QA_FLAG TO ERROR.
UPDATE EDGIS.GIS_PNODEINFO UP
SET UP.QA_FLAG = 'ERROR'
WHERE SUBPNODE_OID IN(
Select T1.SUBPNODE_OID
from EDGIS.GIS_PNODEINFO t1
join
(select * from EDGIS.GIS_PNODEINFO) t2
on (t1.PNODE_CNODEID = t2.PNODE_CNODEID and
      t1.BUS_ID = t2.BUS_ID)
where (t1.CIRCUITID != t2.CIRCUITID));
COMMIT;
*/

END GIS_COMPLETE_PNODEINFO;
/



CREATE OR REPLACE PROCEDURE       GIS_COPY_MDSS_DATA AS
--This is for error handling case, if for some reason, 'GIS_SPP_ZONE' and 'GIS_SPP_FEEDER_SUBLAP' tables have zero records
--then copy the data from repective MDSS tables into those tables.
BEGIN
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

    INSERT INTO EDGIS.GIS_SPP_ZONE
    SELECT * FROM EDGIS.MDSS_SPP_ZONE;
    COMMIT;

    INSERT INTO EDGIS.GIS_SPP_FEEDER_SUBLAP
    SELECT * FROM EDGIS.MDSS_SPP_FEEDER_SUBLAP;
    COMMIT;
END GIS_COPY_MDSS_DATA;
/




CREATE OR REPLACE PROCEDURE       GIS_INSERT_FNM_COMP AS
BEGIN

--sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;
  
  insert into EDGIS.FNM_COMPLETE
  (
    UDC_ID,
    LAP_ID,
    RES_TYPE,
    CNODE_ID,
    BUS_ID,
    FIRST_FNM_VERSION,
    FIRST_FNM_RELEASEDATE,
    LATEST_FNM_VERSION,
    LATEST_FNM_RELEASEDATE,
    DATECREATED,
    CREATEUSER
  )
  (
    select
      udc_id,
      lap_id,
      res_type,
      cnode_id,
      bus_id,
      first_fnm_version,
      first_fnm_release_date,
      latest_fnm_version,
      latest_fnm_release_date,
      sysdate,
      'ED_GIS_USER'
    from EDGIS.GIS_FNM_LAP_TEMP
      where (lap_id, cnode_id, bus_id) not in
        (
          select lap_id, cnode_id, bus_id from EDGIS.FNM_COMPLETE
        )
  );
  commit;
END GIS_INSERT_FNM_COMP;
/




CREATE OR REPLACE PROCEDURE       GIS_INSERT_FNM_FROM_FNM_COMP AS
-- this will need to be modified or an additional sp will be needed to include lca
BEGIN
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

  insert into EDGIS.FNM
  (
    UDC_ID,
    LAP_ID,
    RES_TYPE,
    CNODE_ID,
    BUS_ID,
    FIRST_FNM_VERSION,
    FIRST_FNM_RELEASEDATE,
    LATEST_FNM_VERSION,
    LATEST_FNM_RELEASEDATE,
    DATECREATED,
    CREATEUSER,
    objectid, globalid
  )
  (
    select
      udc_id,
      lap_id,
      res_type,
      cnode_id,
      bus_id,
      first_fnm_version,
      FIRST_FNM_RELEASEDATE,
      latest_fnm_version,
      LATEST_FNM_RELEASEDATE,
      sysdate,
      'ED_GIS_USER',
      sde.gdb_util.next_rowid('EDGIS', 'FNM'),
      sde.gdb_util.next_globalid
    from EDGIS.FNM_COMPLETE FNMCOMP
      where (cnode_id, bus_id) not in
        (
          select cnode_id, bus_id from EDGIS.FNM
        )
        AND
        (
            LATEST_FNM_RELEASEDATE = (select max(LATEST_FNM_RELEASEDATE) from EDGIS.FNM_COMPLETE where LATEST_FNM_RELEASEDATE <= sysdate) -- or Most recent version that is less than today's date.
            --latest_fnm_releasedate > sysdate or -- All future versions
            --LATEST_FNM_RELEASEDATE is null -- Manual overrides
        )
  );
--   EXCEPTION
--    WHEN OTHERS THEN
--    RAISE_APPLICATION_ERROR(-20000, 'An error occured.');
--	--DBMS_OUTPUT.PUT_LINE('Table does not exist.');
  commit;
END GIS_INSERT_FNM_FROM_FNM_COMP;
/




CREATE OR REPLACE PROCEDURE       GIS_MERGE_FNM_COMP AS
BEGIN

    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

  MERGE INTO EDGIS.FNM_COMPLETE TARGET
   USING EDGIS.GIS_FNM_LAP_TEMP SOURCE
   on
    (
      target.lap_id = source.lap_id
      and TARGET.cnode_id = SOURCE.cnode_id
      and TARGET.bus_id = SOURCE.bus_id
      AND
          (
            TARGET.udc_id <> SOURCE.udc_id
            or TARGET.res_type <> SOURCE.res_type
            or TARGET.latest_fnm_version <> SOURCE.latest_fnm_version
            or TARGET.latest_fnm_releasedate <> SOURCE.latest_fnm_release_date
          )
    )
when matched
   then update
        set
          TARGET.udc_id = SOURCE.udc_id,
          TARGET.res_type = SOURCE.res_type,
          TARGET.latest_fnm_version = SOURCE.latest_fnm_version,
          TARGET.latest_fnm_releasedate = SOURCE.latest_fnm_release_date,
          TARGET.DATEMODIFIED = sysdate,
          TARGET.MODIFYUSER = 'ED_GIS_USER';
  COMMIT;
END GIS_MERGE_FNM_COMP;
/




CREATE OR REPLACE PROCEDURE       GIS_MERGE_FNM_FROM_FNM_COMP AS
-- this will need to be modified or an additional sp will be needed to include lca
BEGIN

    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

  MERGE INTO EDGIS.FNM TARGET
   USING
   (
        select * from edgis.fnm_complete
            where
            latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm_complete where latest_fnm_releasedate <= sysdate) --or Most recent version that is less than today's date.
            --latest_fnm_releasedate > sysdate or -- All future versions
            --latest_fnm_releasedate is null -- Manual overrides)
   ) SOURCE
   on
    (
      TARGET.cnode_id = SOURCE.cnode_id
      and TARGET.bus_id = SOURCE.bus_id
      AND
          (
            TARGET.udc_id <> SOURCE.udc_id
            or TARGET.lap_id <> SOURCE.lap_id
            or TARGET.res_type <> SOURCE.res_type
            or TARGET.latest_fnm_version <> SOURCE.latest_fnm_version
            or TARGET.latest_fnm_releasedate <> SOURCE.latest_fnm_releasedate
          )
    )
when matched
   then update
        set
          TARGET.udc_id = SOURCE.udc_id,
          TARGET.lap_id = SOURCE.lap_id,
          TARGET.res_type = SOURCE.res_type,
          TARGET.latest_fnm_version = SOURCE.latest_fnm_version,
          TARGET.latest_fnm_releasedate = SOURCE.latest_fnm_releasedate,
          TARGET.DATEMODIFIED = sysdate,
          TARGET.MODIFYUSER = 'ED_GIS_USER';
  COMMIT;
END GIS_MERGE_FNM_FROM_FNM_COMP;
/




CREATE OR REPLACE PROCEDURE       GIS_PNODE_SERVICEPOINT_DATA AS

BEGIN
--sets the version to default
sde.version_util.set_current_version('SDE.DEFAULT');
commit;

--Insert data into table to hold ED-ServicePoints-Feeder data.
DELETE FROM EDGIS.GIS_SERVICEPOINTS_INFO;
commit;

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

END GIS_PNODE_SERVICEPOINT_DATA;
/




CREATE OR REPLACE PROCEDURE         "GIS_SPP_FEEDER_SUBLAP_PROC" AS

/*
The following tables are required for this procedure:
    EDGIS.GIS_SPP_ZONE
    EDGIS.MDSS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_CIRCUIT_FNM_LCA
*/

begin

--sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

--Deletes existing data in edgis.gis_spp_feeder_sublap
	delete from edgis.gis_spp_feeder_sublap;
    commit;

--copies the current data from spp_feeder_sublap to gis_spp_feeder_sublap
    insert into edgis.gis_spp_feeder_sublap
        select * from edgis.mdss_spp_feeder_sublap;
    commit;

-- deletes feeders that are no longer active according to GIS
-- the remaining feeders in edgis.gis_spp_feeder_sublap are the one that are currently active
    delete from edgis.gis_spp_feeder_sublap where feeder_num not in
    (
        select to_number(circuitid) from edgis.gis_circuit_fnm_lca 
        where 
            start_date <= sysdate
            and end_date >= sysdate
    );
    commit;

-- updates the pnode_id and sublap_id for the feeders in edgis.gis_spp_feeder_sublap
-- updates update_user and update_date
    update edgis.gis_spp_feeder_sublap tt
        set
            tt.update_date = sysdate,
            tt.update_user = 'ED_GIS_USER',
            (tt.pnode_id, tt.sublap_id) =
                (
                    select st.pnode_id, st.sublap_id from
                        (
                            select --distinct
                                a.circuitid,
                                b.zone_id pnode_id,
                                c.zone_id sublap_id
                            from edgis.gis_circuit_fnm_lca a
                            inner join
                                (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.cnode_id = b.zone_name
                            inner join
                                (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.lap_id = c.zone_name
                            where 
                                a.start_date <= sysdate
                                and a.end_date >= sysdate
                        ) st
                    where
                        to_number(st.circuitid) = tt.feeder_num
                        and
                            (
                                st.pnode_id <> tt.pnode_id or tt.pnode_id is null or st.sublap_id <> tt.sublap_id or tt.sublap_id is null
                            )
                )
                where exists
                    (
                        select 1
                        from
                            (
                                select --distinct
                                    a.circuitid,
                                    b.zone_id pnode_id,
                                    c.zone_id sublap_id
                                from edgis.gis_circuit_fnm_lca a
                                inner join
                                    (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.cnode_id = b.zone_name
                                inner join
                                    (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.lap_id = c.zone_name
                                where 
                                    a.start_date <= sysdate
                                    and a.end_date >= sysdate
                            )
                        where
                            to_number(circuitid) = tt.feeder_num
                            and
                            (
                                pnode_id <> tt.pnode_id or tt.pnode_id is null or sublap_id <> tt.sublap_id or tt.sublap_id is null
                            )
                    );
    commit;

-- inserts new feeders and their associated pnode_ids and sublap_ids.  A new feeder is a feeder that is
-- in gis' gis_circuit_fnm_lca but not in mdss' GIS_SPP_FEEDER_SUBLAP
-- sets create_user and create_date
    insert into edgis.gis_spp_feeder_sublap
    (
        feeder_num,
        pnode_id,
        sublap_id,
        create_user,
        create_date
    )
    select
        to_number(a.circuitid),
        b.zone_id pnode_id,
        c.zone_id sublap_id,
        'ED_GIS_USER',
        sysdate
    from edgis.gis_circuit_fnm_lca a
    inner join
        (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.cnode_id = b.zone_name
    inner join
        (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.lap_id = c.zone_name
    where
        to_number(a.circuitid) not in (select feeder_num from edgis.gis_spp_feeder_sublap)
        and a.start_date <= sysdate
        and a.end_date >= sysdate;
    commit;

end GIS_SPP_FEEDER_SUBLAP_PROC;
/




CREATE OR REPLACE PROCEDURE         "GIS_SPP_ZONE_PROC" as

/*
The following tables are required for this procedure:
    edgis.gis_spp_zone
    edgis.mdss_spp_zone
    edgis.fnm

    what happens if MDSS adds a new special sublap zone? ******
    What happens if MDSS adds a new special pnode zone?  ******

    Sublaps and pnodes that are in most currently effective FNM but not GIS_SPP_ZONE are assumed to be a new sublap or pnode
    that needs to be inserted in to GIS_SPP_ZONE so it can later be inserted in MDSS_SPP_ZONE.  The fields are being
    managed as follows:

        ZONE_ID --- MDSS has priority.  GIS will create and assign zone_ids for only new sublaps and pnodes.  MDSS can modify
        the zone_ids assigned by GIS.  GIS will import these changes as part of its daily job.

        ZONE_TYPE_ID --- GIS will set this value for new sublaps and pnodes.  Use 1501 for sublaps, and 1507 for pnodes.
        If MDSS changes these values, then GIS will not undo the change.  NEED TO VALIDATE because several queries use
        ZONE_TYPE_ID.

        ZONE_CODE --- GIS will set this to null for new ZONE_TYPE_ID in (1501, 1507).  If MDSS changes this value,
        then GIS will not undo to the change.

        ZONE_NAME --- GIS will set this value for new sublaps and pnodes.  If MDSS changes these values to a value not in the FNM,
        then GIS will treat the renamed zone as a special zone.  GIS would create a new zone (with the original zone name) to
        re-create to original zone.

        ZONE_DESC --- GIS will set this value for new sublaps and pnodes.  If MDSS changes these values, then GIS
        will not undo the change.  If MDSS changes this value, then GIS will not undo the change.      
            If ZONE_TYPE_ID is 1501, then set ZONE_DESC to LOOKUP TABLE FOR SUBLAPS.  
            If ZONE_TYPE_ID is 1507, then set ZONE_DESC to LOOKUP TABLE FOR PRICING NODES.

        ARCHIVE_DATE --- GIS sets this value when a sublap or pnode is no longer in use according to the FNM.  ***************
            If the sublap or pnode is inactive according to the FNM:
                If MDSS changes a date value back to null, then GIS will set the archive date to sysdate. *********
                If MDSS changes this date to another date, then GIS will not undo this change.  ********
            If the sublap or pnode is active according to the FNM:
                If MDSS populates the field with a date, then GIS will create a new zone to reactivate the zone.   *******

        PARENT_ZONE_ID
            Sublaps -- Value is always null for sublaps.  If MDSS changes this to a non-null value, then GIS
                        will change the value back to null.
            Pnodes -- GIS sets this value for new pnodes.  GIS maintains this value for pnodes.  So GIS will update when
                        necessary.  If MDSS changes this value for a pnode, then GIS will undo the change.

        RULE24_ZONE_ID_MAPPING --- Always set to ZONE_ID when ZONE_TYPE_ID in (1501, 1507) and when the pnode or sublap is
        in the FNM. If MDSS changes this value, GIS will not undo the change.

        DISPLAY_RULE24 --- If the pnode or sublap is in the FNM, then set to 1.

        CREATE_DATE --- Set by GIS for new sublaps and pnodes.   If MDSS changes this date or changes it to null, then GIS,
        will not undo MDSS' change.

        UPDATE_DATE --- Set by GIS for changes sublaps and pnodes.  If MDSS changes this date or changes it to null, then GIS,
        will not undo MDSS' change.

*/

-- Defines a cursor against edgis.gis_spp_zone that is used as part of the process to assign zone_id for sublaps or pnodes
-- that are not in the spp_zone table in MDSS.  Assumes that the zone_id will be null after the sublap or pnode has been
-- inserted into edgis.gis_spp_zone.  The cursor is created in the code below.

cursor c1 is
    select
      zone_name
    from
      edgis.gis_spp_zone
    where
      zone_id is null
    order by
      zone_type_id, zone_name;

-- declares local variables
v_zone_id number;
v_zone_name varchar2(50);
v_max_zone_id_in_use number;

begin

    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;
    
    -- assigns initial values to null
    --DBMS_OUTPUT.PUT_LINE('0');
    v_zone_id := null;
    v_zone_name := null;
    v_max_zone_id_in_use := null;

    -- deletes all rows from edgis.gis_spp_zone
    --execute immediate 'truncate table edgis.gis_spp_zone';
    --DBMS_OUTPUT.PUT_LINE('1');
    delete from edgis.gis_spp_zone;
    commit;

    -- copies the data from spp_zone to gis_spp_zone
    --DBMS_OUTPUT.PUT_LINE('2');
    insert into edgis.gis_spp_zone
        select * from edgis.mdss_spp_zone;
    commit;

    -- determines the greatest zone_id in MDSS spp_zone.
    --DBMS_OUTPUT.PUT_LINE('3');
    select nvl(max(zone_id), 0) into v_max_zone_id_in_use from edgis.gis_spp_zone;
    --DBMS_OUTPUT.PUT_LINE('max_zone_id_in_use = ' || TO_CHAR(v_max_zone_id_in_use));

    -- this eliminates all zone_type_ids that are not related to pnodes and sublaps.
    --DBMS_OUTPUT.PUT_LINE('4');
    delete from edgis.gis_spp_zone where zone_type_id not in (1501, 1507);  -- conider deleting all zones with an archive date and zone_id = 122, 123  ******************
    commit;

    -- sets an unpopulated archive_date in edgis.gis_spp_zone for those zones that are in FNM but the latest_fnm_releasedate is not the most recent version" *****************
    --DBMS_OUTPUT.PUT_LINE('5');
    update edgis.gis_spp_zone
    set
        update_date = sysdate,
        update_user = 'ED_GIS_USER',
        archive_date = trunc(sysdate)
    where
        archive_date is null
        and
            (
                zone_name in
                (
                    select distinct
                        lap_id
                    from edgis.fnm
                    where lap_id not in
                    (
                        select distinct lap_id
                            from edgis.fnm
                            where latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                    )
                )
                or zone_name in
                (
                    select distinct
                        cnode_id
                    from edgis.fnm
                    where cnode_id not in
                    (
                        select distinct cnode_id
                            from edgis.fnm
                            where latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                    )
                )
            );
    commit;

    -- clears a populated archive_date in edgis.gis_spp_zone for those zones that are in FNM and the latest_fnm_releasedate is the most recent version" *****************
    --DBMS_OUTPUT.PUT_LINE('6');
    update edgis.gis_spp_zone
    set
        update_date = sysdate,
        update_user = 'ED_GIS_USER',
        archive_date = null
    where
        archive_date is not null
        and
            (
                zone_name in
                (
                    select distinct lap_id
                        from edgis.fnm
                        where latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                )
                or zone_name in
                (
                    select distinct cnode_id
                        from edgis.fnm
                        where latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                )
            );
    commit;

    -- Inserts new pnodes and sublaps that are not already in edgis.gis_spp_zone
    -- need to verify that no duplicates are inserted ***********************************************************
    --DBMS_OUTPUT.PUT_LINE('7');
    insert into edgis.gis_spp_zone
    (
        zone_type_id,
        zone_name,
        zone_desc,
        display_rule24,
        create_date,
        create_user
    )

    with temp_current_fnm_table as
    (
        select
            fnm.lap_id,
            fnm.cnode_id,
            fnm.latest_fnm_version,
            fnm.latest_fnm_releasedate
        from edgis.fnm
        where
            upper(udc_id) in ('PGAE', 'SCE')
            and latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
    )

    select distinct
        fnm_zone.zone_type_id,
        fnm_zone.zone_name,
        fnm_zone.zone_desc,
        1 display_rule24,
        sysdate,
        'ED_GIS_USER'
    from
    (
        -- the following select block creates a dataset for sublaps that are in the most currently effective FNM
        select distinct
            1501 zone_type_id,
            lap_id zone_name,
            'LOOKUP TABLE FOR SUBLAPS' zone_desc
        from temp_current_fnm_table

        union

        -- the following select block creates a dataset for pnodes that are in the most currently effective FNM
        select
            1507 zone_type_id,
            cnode_id zone_name,
            'LOOKUP TABLE FOR PRICING NODES' zone_desc
        from temp_current_fnm_table
    ) fnm_zone
    left outer join edgis.gis_spp_zone t1 on fnm_zone.zone_name = t1.zone_name
    where t1.zone_name is null;

    commit;

    --DBMS_OUTPUT.PUT_LINE('8');
    open c1;
        loop
            fetch c1 into v_zone_name; -- fetches the cursor c1 and assigns values to variables.
                if c1%notfound
                    then
                        exit;
                end if;
            v_max_zone_id_in_use := v_max_zone_id_in_use + 1;
            --DBMS_OUTPUT.PUT_LINE('max_zone_id_in_use = ' || TO_CHAR(v_max_zone_id_in_use));
            --DBMS_OUTPUT.PUT_LINE('zone_name = ' || v_zone_name);

            -- updates the new pnodes and sublaps in gis_spp_zone with a zone_id and other values.
            update edgis.gis_spp_zone
                set
                    zone_id = v_max_zone_id_in_use,
                    rule24_zone_id_mapping = v_max_zone_id_in_use
                where zone_name = v_zone_name;

        end loop;
        commit;
    close c1;

    -- this section populates the parent code for new pnodes
    --DBMS_OUTPUT.PUT_LINE('9');
    update edgis.gis_spp_zone d
    set
        parent_zone_id =
        (
            select
                c.zone_id -- parent_zone_id
            from edgis.gis_spp_zone a
            inner join edgis.fnm b on a.zone_name = b.cnode_id
            inner join edgis.gis_spp_zone c on b.lap_id = c.zone_name
            where
                a.parent_zone_id is null
                and a.zone_type_id = 1507
                and b.latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                and a.zone_id = d.zone_id
        )
        where
            exists
            (
                select
                    c.zone_id -- parent_zone_id
                from edgis.gis_spp_zone a
                inner join edgis.fnm b on a.zone_name = b.cnode_id
                inner join edgis.gis_spp_zone c on b.lap_id = c.zone_name
                where
                    a.parent_zone_id is null
                    and a.zone_type_id = 1507
                    and b.latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                    and a.zone_id = d.zone_id
            );
    commit;

    -- updates the parent_zone_id for existing active pnodes.
    -- must be run after any new sublaps have been inserted.
    -- what other fields need to be updated?  ************************************
    -- need to address how to handle zones that where copied from mdss as active but are not active in GIS *******************
    --DBMS_OUTPUT.PUT_LINE('10');
    update edgis.gis_spp_zone d
        set
            update_date = sysdate,
            update_user = 'ED_GIS_USER',
            parent_zone_id =
            (
                select
                    c.zone_id
                from edgis.gis_spp_zone a
                inner join edgis.fnm b on a.zone_name = b.cnode_id
                inner join edgis.gis_spp_zone c on b.lap_id = c.zone_name
                where
                    b.latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                    and a.zone_id = d.zone_id
                    and a.archive_date is null
                    and c.zone_id <> d.parent_zone_id
            )
            where
                d.zone_type_id = 1507
                and exists
                (
                    select
                        c.zone_id
                    from edgis.gis_spp_zone a
                    inner join edgis.fnm b on a.zone_name = b.cnode_id
                    inner join edgis.gis_spp_zone c on b.lap_id = c.zone_name
                    where
                        b.latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm where latest_fnm_releasedate <= sysdate)
                        and a.zone_id = d.zone_id
                        and a.archive_date is null
                        and c.zone_id <> d.parent_zone_id
                );
    commit;

--DBMS_OUTPUT.PUT_LINE('11');
end GIS_SPP_ZONE_PROC;
/




CREATE OR REPLACE PROCEDURE       GIS_UPDATE_FNM_LCA_ID AS
BEGIN
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

 MERGE INTO EDGIS.FNM TARGET
   USING EDGIS.GIS_SUBLAP_LCA SOURCE
   on
    (
      TARGET.LAP_ID = SOURCE.LAP_ID
      AND
          (
            SOURCE.start_date <= sysdate AND
            (SOURCE.end_date is null OR SOURCE.end_date >=sysdate)
          )
    )
when matched
   then update
        set
          TARGET.LCA_ID = SOURCE.LCA_NAME;
  COMMIT;
END GIS_UPDATE_FNM_LCA_ID;
/

GRANT EXECUTE ON GIS_COMPLETE_PNODEINFO TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_COPY_MDSS_DATA TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_INSERT_FNM_COMP TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_INSERT_FNM_FROM_FNM_COMP TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_MERGE_FNM_COMP TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_MERGE_FNM_FROM_FNM_COMP TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_PNODE_SERVICEPOINT_DATA TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_PNODE_SERVICEPOINT_DATA TO EDGISSUB_BO_RO;

GRANT EXECUTE ON GIS_SPP_FEEDER_SUBLAP_PROC TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_SPP_ZONE_PROC TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_UPDATE_FNM_LCA_ID TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_COMPLETE_PNODEINFO TO EDGISSUB_BO_RO;

---- Updating existing procedures --------------

spool off