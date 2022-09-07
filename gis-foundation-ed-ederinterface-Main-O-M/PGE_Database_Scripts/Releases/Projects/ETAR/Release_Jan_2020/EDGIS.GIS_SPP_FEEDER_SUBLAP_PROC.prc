--------------------------------------------------------
--  File created - Monday-September-16-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_SPP_FEEDER_SUBLAP_PROC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDGIS"."GIS_SPP_FEEDER_SUBLAP_PROC" AS 
begin

/*
The following tables are required for this procedure:
    EDGIS.GIS_SPP_ZONE
    EDGIS.MDSS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_PNODE_CIRCUIT_INFO

The following views are required for this procedure:
    EDGIS.GIS_CIRCUIT_FNM_VALUES
*/

--execute immediate 'truncate table edgis.gis_spp_feeder_sublap';
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
        select to_number(circuitid) from gis_circuit_fnm_values 
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
                            select 
                                a.circuitid, 
                                b.zone_id pnode_id,
                                c.zone_id sublap_id
                            from edgis.gis_circuit_fnm_values a 
                            inner join 
                                (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.pnode_id = b.zone_name
                            inner join 
                                (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.sublap_id = c.zone_name  
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
                                select 
                                    a.circuitid, 
                                    b.zone_id pnode_id,
                                    c.zone_id sublap_id
                                from edgis.gis_circuit_fnm_values a 
                                inner join 
                                    (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.pnode_id = b.zone_name
                                inner join 
                                    (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.sublap_id = c.zone_name  
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
-- in gis' gis_circuit_fnm_values but not in mdss' GIS_SPP_FEEDER_SUBLAP
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
    from edgis.gis_circuit_fnm_values a 
    inner join 
        (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1507) b on a.pnode_id = b.zone_name
    inner join 
        (select zone_id, zone_name, zone_type_id from edgis.gis_spp_zone where zone_type_id = 1501) c on a.sublap_id = c.zone_name  
    where 
        to_number(a.circuitid) not in (select feeder_num from edgis.gis_spp_feeder_sublap); 
    commit;
    
end GIS_SPP_FEEDER_SUBLAP_PROC;
/

Prompt Grants on PROCEDURE EDGIS.GIS_SPP_FEEDER_SUBLAP_PROC TO GIS_SUB_MDSS_RW to GIS_SUB_MDSS_RW;
GRANT EXECUTE ON EDGIS.GIS_SPP_FEEDER_SUBLAP_PROC TO GIS_SUB_MDSS_RW
/

