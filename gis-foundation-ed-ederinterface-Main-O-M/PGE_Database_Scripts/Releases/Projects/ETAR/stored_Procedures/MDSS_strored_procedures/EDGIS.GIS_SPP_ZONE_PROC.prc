--------------------------------------------------------
--  File created - Monday-September-16-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_SPP_ZONE_PROC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "EDGIS"."GIS_SPP_ZONE_PROC" as

/*
The following tables are required for this procedure: 
    edgis.gis_spp_zone
    edgis.mdss_spp_zone
    edgis.fnm
               
*/

/* 
    what happens if MDSS adds a new special sublap zone? ******
    What happens if MDSS adds a new special pnode zone?  ******
    
    Sublaps and pnodes that are in most currently effective FNM but not GIS_SPP_ZONE are assumed to be a new sublap or pnode 
    that needs to be inserted in to GIS_SPP_ZONE so it can later be inserted in MDSS_SPP_ZONE.  The fields are being 
    managed as follows:

        ZONE_ID – MDSS has priority.  GIS will create and assign zone_ids for only new sublaps and pnodes.  MDSS can modify 
        the zone_ids assigned by GIS.  GIS will import these changes as part of its daily job.

        ZONE_TYPE_ID – GIS will set this value for new sublaps and pnodes.  Use 1501 for sublaps, and 1507 for pnodes.  
        If MDSS changes these values, then GIS will not undo the change.  NEED TO VALIDATE because several queries use 
        ZONE_TYPE_ID.
        
        ZONE_CODE – GIS will set this to null for new ZONE_TYPE_ID in (1501, 1507).  If MDSS changes this value,
        then GIS will not undo to the change.
        
        ZONE_NAME – GIS will set this value for new sublaps and pnodes.  If MDSS changes these values to a value not in the FNM, 
        then GIS will treat the renamed zone as a special zone.  GIS would create a new zone (with the original zone name) to 
        re-create to original zone.
        
        ZONE_DESC – GIS will set this value for new sublaps and pnodes.  If MDSS changes these values, then GIS 
        will not undo the change.  If ZONE_TYPE_ID is 1501, then use “LOOKUP TABLE FOR SUBLAPS”.  If ZONE_TYPE_ID is 1507, 
        then use “LOOKUP TABLE FOR PRICING NODES”.  If MDSS changes this value, then GIS will not undo the change.
        
        ARCHIVE_DATE – GIS sets this value when a sublap or pnode is no longer in use according to the FNM.  ***************
            If the sublap or pnode is inactive according to the FNM:
                If MDSS changes a date value back to null, then GIS will set the archive date to sysdate. *********
                If MDSS changes this date to another date, then GIS will not undo this change.  ********
            If the sublap or pnode is active according to the FNM:
                If MDSS populates the field with a date, then GIS will create a new zone to reactivate the zone.   *******
        
        PARENT_ZONE_ID – 
            Sublaps -- Value is always null for sublaps.  If MDSS changes this to a non-null value, then GIS 
                        will change the value back to null.  
            Pnodes -- GIS sets this value for new pnodes.  GIS maintains this value for pnodes.  So GIS will update when 
                        necessary.  If MDSS changes this value for a pnode, then GIS will undo the change.
        
        RULE24_ZONE_ID_MAPPING – Always set to ZONE_ID when ZONE_TYPE_ID in (1501, 1507) and when the pnode or sublap is 
        in the FNM. If MDSS changes this value, GIS will not undo the change.
        
        DISPLAY_RULE24 – If the pnode or sublap is in the FNM, then set to 1.
        
        CREATE_DATE – Set by GIS for new sublaps and pnodes.   If MDSS changes this date or changes it to null, then GIS,
        will not undo MDSS' change.
        
        UPDATE_DATE – Set by GIS for changes sublaps and pnodes.  If MDSS changes this date or changes it to null, then GIS,
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
    -- assigns initial values to null 
    v_zone_id := null;
    v_zone_name := null;
    v_max_zone_id_in_use := null;
    
    -- deletes all rows from edgis.gis_spp_zone
    --execute immediate 'truncate table edgis.gis_spp_zone';
    delete from edgis.gis_spp_zone;
    commit;
   
    -- copies the data from spp_zone to gis_spp_zone
    insert into edgis.gis_spp_zone
        select * from edgis.mdss_spp_zone;
    commit;
   
    -- determines the greatest zone_id in MDSS spp_zone.
    select nvl(max(zone_id), 0) into v_max_zone_id_in_use from edgis.gis_spp_zone;
    --DBMS_OUTPUT.PUT_LINE('max_zone_id_in_use = ' || TO_CHAR(v_max_zone_id_in_use));     
    
    -- this eliminates all zone_type_ids that are not related to pnodes and sublaps.
    execute immediate 'delete from edgis.gis_spp_zone where zone_type_id not in (1501, 1507)'; -- conider deleting all zones with an archive date and zone_id = 122, 123  ******************
        
    -- sets archive_date for those zones that are in FNM but the latest_fnm_releasedate is not the most recent version" ***************** 
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
     
    -- Inserts new pnodes and sublaps that are not already in edgis.gis_spp_zone
    -- need to verify that no duplicates are inserted ***********************************************************
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
    -- most be run after any new sublaps have been inserted.
    -- what other fields need to be updated?  ************************************
    -- need to address how to handle zones that where copied from mdss as active but are not active in GIS *******************
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
        
end GIS_SPP_ZONE_PROC;

/

Prompt Grants on PROCEDURE EDGIS.GIS_SPP_ZONE_PROC TO GIS_SUB_MDSS_RW to GIS_SUB_MDSS_RW;
GRANT EXECUTE ON EDGIS.GIS_SPP_ZONE_PROC TO GIS_SUB_MDSS_RW
/