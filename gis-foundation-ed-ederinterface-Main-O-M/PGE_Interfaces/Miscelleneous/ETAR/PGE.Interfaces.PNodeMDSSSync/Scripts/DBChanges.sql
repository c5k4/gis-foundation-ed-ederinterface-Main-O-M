set echo on
spool D:\Temp\DBChanges.txt


DROP TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_TEMP";

  CREATE TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_TEMP" 
   (	"CIRCUITID" VARCHAR2(9 BYTE), 
	"SOURCE_FEEDER" VARCHAR2(9 BYTE), 
	"BUS_ID" VARCHAR2(254 BYTE), 
	"CNODE_ID" VARCHAR2(254 BYTE), 
	"LAP_ID" VARCHAR2(254 BYTE), 
	"LCA_ID" VARCHAR2(254 BYTE), 
	"START_DATE" DATE, 
	"END_DATE" DATE, 
	"REFRESH_DATE" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_TEMP"  IS 'This is a staging table for GIS_PNODE_CIRCUIT_INFO.';

DROP TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA";


  CREATE TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA" 
   (	"CIRCUITID" VARCHAR2(9 BYTE), 
	"SOURCE_FEEDER" VARCHAR2(9 BYTE), 
	"BUS_ID" VARCHAR2(254 BYTE), 
	"CNODE_ID" VARCHAR2(254 BYTE), 
	"LAP_ID" VARCHAR2(254 BYTE), 
	"LCA_ID" VARCHAR2(254 BYTE), 
	"START_DATE" DATE, 
	"END_DATE" DATE, 
	"CREATEUSER" VARCHAR2(20 BYTE) DEFAULT NULL, 
	"DATECREATED" DATE DEFAULT SYSDATE, 
	"UPDATEUSER" VARCHAR2(20 BYTE), 
	"DATEUPDATED" DATE, 
	"REFRESH_DATE" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA"  IS 'This table maps a feeder to its BUS_ID, PNODE_ID, SUBLAP_ID, and LCA_ID.  The listed feeders includes those that are currently active and traceable, and those that were active and traceable in the past but are no longer active or traceable.  The start and end dates are for the FNM and LCA related values only.  These dates are not associated with the feeder.  The REFRESH_DATE is the last date that the CIRCUITID / START_DATE combination and their attributes were refreshed.';

  CREATE UNIQUE INDEX "EDGIS"."GIS_TABLE1_PK" ON "EDGIS"."GIS_CIRCUIT_FNM_LCA" ("CIRCUITID", "START_DATE") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;
  CREATE INDEX "EDGIS"."GIS_CIRCUIT_FNM_LCA_CIRCUITID" ON "EDGIS"."GIS_CIRCUIT_FNM_LCA" ("CIRCUITID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

 
DROP TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY"; 


  CREATE TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" 
   (	"CIRCUITID" VARCHAR2(9 BYTE), 
	"SOURCE_FEEDER" VARCHAR2(9 BYTE), 
	"BUS_ID" VARCHAR2(254 BYTE), 
	"CNODE_ID" VARCHAR2(254 BYTE), 
	"LAP_ID" VARCHAR2(254 BYTE), 
	"LCA_ID" VARCHAR2(254 BYTE), 
	"START_DATE" DATE, 
	"END_DATE" DATE, 
	"CREATEUSER" VARCHAR2(20 BYTE), 
	"DATECREATED" DATE, 
	"UPDATEUSER" VARCHAR2(20 BYTE), 
	"DATEUPDATED" DATE, 
	"REFRESH_DATE" DATE, 
	"ACTION" VARCHAR2(50 BYTE), 
	"ACTIONDATE" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY"  IS 'This is a trigger driven table that tracks changes to GIS_CIRCUIT_FNM_LCA.  The values for ACTION column are INSERT, UPDATE, DROP, RESTORE, RESTORE AND UPDATE.';

  CREATE INDEX "EDGIS"."GIS_CIRCUIT_FNM_LCA_HIST_IDX" ON "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" ("CIRCUITID", "BUS_ID", "CNODE_ID", "START_DATE") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

 CREATE OR REPLACE TRIGGER "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" 
    after update or insert or delete on edgis.gis_circuit_fnm_lca
    referencing old as old new as new
    for each row    

    declare    
        v_LastAction varchar2(50);  -- variable that stores the last action for a circuitid-start_date combination.
    
    begin
            
        if inserting then
            -- Insert record into HISTORY table when a record is isnserted into the main table.  The SP uses the circuit-start_date combination as the key for when to insert.
            insert into edgis.gis_circuit_fnm_lca_history
            (
                circuitid,
                source_feeder,
                bus_id,
                cnode_id,
                lap_id,
                lca_id,
                start_date,
                end_date,
                createuser,
                datecreated,
                refresh_date,
                action,
                actiondate
            )
            values
            (
                :new.circuitid,
                :new.source_feeder,
                :new.bus_id,
                :new.cnode_id,
                :new.lap_id,
                :new.lca_id,
                :new.start_date,
                :new.end_date,
                :new.createuser,
                :new.datecreated,
                :new.refresh_date,
                'INSERT',
                sysdate
            );
        end if;

        if updating then
            -- Insert record into HISTORY table when a record in the main table is updated.
            -- The SP uses the circuit-start_date combination as the key for when to insert.

            select 
                upper(action) into v_LastAction 
            from edgis.gis_circuit_fnm_lca_history 
            where (circuitid, start_date, actiondate) in
            (
                select circuitid, start_date, max(actiondate) from edgis.gis_circuit_fnm_lca_history                 
                where 
                    circuitid = :old.circuitid
                    and start_date = :old.start_date
                group by circuitid, start_date
            );

            -- The first update statement below filters on the primary data fields (i.e. excludes the refresh date, and auditig fields)
            -- this way a new record is not inserted into the history table when the refresh date is the only value being updated.
            if 
                :old.circuitid <> :new.circuitid 
                or :old.source_feeder <> :new.source_feeder
                or :old.bus_id <> :new.bus_id
                or :old.cnode_id <> :new.cnode_id
                or :old.lap_id <> :new.lap_id
                or :old.lca_id <> :new.lca_id
                or :old.start_date <> :new.start_date
                or :old.end_date <> :new.end_date
                
            then       
                insert into edgis.gis_circuit_fnm_lca_history
                (
                    circuitid,
                    source_feeder,
                    bus_id,
                    cnode_id,
                    lap_id,
                    lca_id,
                    start_date,
                    end_date,
                    createuser,
                    datecreated,
                    updateuser,
                    dateupdated,
                    refresh_date,
                    action,
                    actiondate
                )
                values
                (
                    :new.circuitid,
                    :new.source_feeder,
                    :new.bus_id,
                    :new.cnode_id,
                    :new.lap_id,
                    :new.lca_id,
                    :new.start_date,
                    :new.end_date,
                    :new.createuser,
                    :new.datecreated,
                    :new.updateuser,
                    :new.dateupdated,
                    :new.refresh_date,
                    decode(v_LastAction, 'DROP', 'RESTORE AND UPDATE', 'UPDATE'),  --if last action was 'DROP' then use 'RESTORE AND UPDATE', else use 'UPDATE' 
                    sysdate
                );
            end if;
            
            --  The following section writes to edgis.gis_circuit_fnm_lca_history to keep track of when a circuit-start date combination reappears in the temp table.
            --  This occurs when the prior action for the record was DROP (i.e. was not in the temp table on prior run), but then the record reappears with no chnages other than refresh_date.  Hence Restored.  
            --  The record in the main table has its refresh date updated.  A record is inserted into ther history table with ACTION set to RESTORED.
            if 
                v_LastAction = 'DROP'
                and :old.circuitid = :new.circuitid 
                and :old.source_feeder = :new.source_feeder
                and :old.bus_id = :new.bus_id
                and :old.cnode_id = :new.cnode_id
                and :old.lap_id = :new.lap_id
                and :old.lca_id = :new.lca_id
                and :old.start_date = :new.start_date
                and :old.end_date = :new.end_date
                and :old.refresh_date <> :new.refresh_date
                
            then       
                insert into edgis.gis_circuit_fnm_lca_history
                (
                    circuitid,
                    source_feeder,
                    bus_id,
                    cnode_id,
                    lap_id,
                    lca_id,
                    start_date,
                    end_date,
                    createuser,
                    datecreated,
                    updateuser,
                    dateupdated,
                    refresh_date,
                    action,
                    actiondate
                )
                values
                (
                    :new.circuitid,
                    :new.source_feeder,
                    :new.bus_id,
                    :new.cnode_id,
                    :new.lap_id,
                    :new.lca_id,
                    :new.start_date,
                    :new.end_date,
                    :new.createuser,
                    :new.datecreated,
                    :new.updateuser,
                    :new.dateupdated,
                    :new.refresh_date,
                    'RESTORE',
                    sysdate
                );
            end if;          
        end if;

        if deleting then
            -- Insert deleted record into HISTORY table when a record in the main table is deleted.  The SP never deltes.  This is here to track deletes made directly to the table.
            insert into edgis.gis_circuit_fnm_lca_history
            (
                circuitid,
                source_feeder,
                bus_id,
                cnode_id,
                lap_id,
                lca_id,
                start_date,
                end_date,
                createuser,
                datecreated,
                refresh_date,
                action,
                actiondate
            )
            values
            (
                :old.circuitid,
                :old.source_feeder,
                :old.bus_id,
                :old.cnode_id,
                :old.lap_id,
                :old.lca_id,
                :old.start_date,
                :old.end_date,
                :old.createuser,
                :old.datecreated,
                :old.refresh_date,
                'DELETE',
                sysdate
            );
        end if;
        

/*  
--============================  
        
    after statement is

    begin       
    
        if upper(v_Caller) = 'PROCEDURE' and upper(v_Name) = 'GIS_CIRCUIT_FNM_LCA_REFRESH' then    
            
            -- the following section writes to edgis.gis_circuit_fnm_lca_history to keep track of when a circuit-start_date combination is not in the current temp table but is in the main table.  Hence dropped.
            insert into edgis.gis_circuit_fnm_lca_history
            (
                circuitid,
                source_feeder,
                bus_id,
                cnode_id,
                lap_id,
                lca_id,
                start_date,
                end_date,
                createuser,
                datecreated,
                updateuser,
                dateupdated,
                refresh_date,
                action,
                actiondate
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
                    end_date,
                    createuser,
                    datecreated,
                    updateuser,
                    dateupdated,
                    refresh_date,
                    'DROP',
                    sysdate
                from edgis.gis_circuit_fnm_lca
                where 
                    (circuitid, start_date) not in 
                        (select circuitid, start_date from edgis.gis_circuit_fnm_lca_temp)
                    and (circuitid, start_date) not in 
                    (
                        select 
                            circuitid, start_date 
                        from edgis.gis_circuit_fnm_lca_history 
                        where 
                            (circuitid, start_date, actiondate)
                            in
                            (
                                select circuitid, start_date, max(actiondate) 
                                from edgis.gis_circuit_fnm_lca_history 
                                group by circuitid, start_date
                            )
                            and upper(action) = 'DROP'
                    )
            );
    
            -- the following section writes to edgis.gis_circuit_fnm_lca_history to keep track of when a circuit-start date combination reappears in the temp table.  Hence Restored.
            insert into edgis.gis_circuit_fnm_lca_history
            (
                circuitid,
                source_feeder,
                bus_id,
                cnode_id,
                lap_id,
                lca_id,
                start_date,
                end_date,
                createuser,
                datecreated,
                updateuser,
                dateupdated,
                refresh_date,
                action,
                actiondate
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
                    end_date,
                    createuser,
                    datecreated,
                    updateuser,
                    dateupdated,
                    refresh_date,
                    'RESTORE',
                    sysdate
                from edgis.gis_circuit_fnm_lca
                where 
                    (circuitid, start_date) in 
                        (select circuitid, start_date from edgis.gis_circuit_fnm_lca_temp)
                    and (circuitid, start_date) in 
                    (
                        select 
                            circuitid, start_date 
                        from edgis.gis_circuit_fnm_lca_history 
                        where 
                            (circuitid, start_date, actiondate)
                            in
                            (
                                select circuitid, start_date, max(actiondate) 
                                from edgis.gis_circuit_fnm_lca_history 
                                group by circuitid, start_date
                            )
                            and upper(action) = 'DROP'
                    )
            ); 
            
        end if;
        
    end after statement;
*/
end GIS_CIRCUIT_FNM_LCA_HISTORY;
/
ALTER TRIGGER "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" ENABLE;


create or replace PROCEDURE "GIS_CIRCUIT_FNM_LCA_REFRESH" as

-- This SP populates table edgis.gis_circuit_fnm_lca_temp with the current active feeders and populates the FNM values with all past, current, and future values.
-- The records with a 'Y' in the EXCLUDE field for table edgis.gis_pnode_circuit_info are not inserted into dgis.gis_circuit_fnm_lca_temp.

-- Update records in edgis.gis_circuit_fnm_lca with those records from edgis.gis_circuit_fnm_lca_temp (matching by circuitid and start_date) where the 
-- source_feeder, bus_id, cnode_id, lap_id, lca_id, end_date, refresh_date do not mnatch.

v_refresh_date date;

begin
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;
    
    -- used to store the most recent action date for a circuitid, start_date combination.
    v_refresh_date := sysdate;


--  verify that the temporary table is empty
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
            c.end_date,
            v_refresh_date refresh_date
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

    -- the following updates the main table for any fields that have changed except circuitid or start_date, which are used as a key
    update edgis.gis_circuit_fnm_lca a
    set
    (
        a.source_feeder,
        a.bus_id,
        a.cnode_id,
        a.lap_id,
        a.lca_id,
        a.end_date,
        a.updateuser,
        a.dateupdated,
        a.refresh_date
    )
    =
    (
        select 
            b.source_feeder,
            b.bus_id,
            b.cnode_id,
            b.lap_id,
            b.lca_id,
            b.end_date,
            'EDGIS',
            sysdate,
            b.refresh_date
        from edgis.gis_circuit_fnm_lca_temp b where a.circuitid = b.circuitid and a.start_date = b.start_date
    )
    where exists
    (
        select * from edgis.gis_circuit_fnm_lca_temp b
            where 
                a.circuitid = b.circuitid and a.start_date = b.start_date and 
                ( 
                    a.source_feeder <> b.source_feeder 
                    or a.bus_id <> b.bus_id 
                    or a.cnode_id <> b.cnode_id 
                    or a.lap_id <> b.lap_id 
                    or a.lca_id <> b.lca_id 
                    or a.end_date <> b.end_date
                    or a.refresh_date <> b.refresh_date
                )
    );
      
    -- Inserts records where feeders-start_date combination is in edgis.gis_circuit_fnm_lca_temp but not in edgis.gis_circuit_fnm_lca.  Typically a brand new feeder or new FNM assignment for an existinger feeder.
    insert into edgis.gis_circuit_fnm_lca
    (
        circuitid,
        source_feeder,
        bus_id,
        cnode_id,
        lap_id,
        lca_id,
        start_date,
        end_date,
        createuser,
        datecreated,
        refresh_date
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
            end_date,
            'EDGIS',
            sysdate,
            refresh_date
        from edgis.gis_circuit_fnm_lca_temp
        where (circuitid, start_date) not in (select circuitid, start_date from edgis.gis_circuit_fnm_lca)
    );   
        
    --  The following section writes to edgis.gis_circuit_fnm_lca_history to keep track of when a circuit-start_date combination is not in the current temp table but is in the main table.  Hence dropped.
    --  The circuit-start_date combination in the main table remains unchanged but a record is inserted into the history table with the ACTION set to DROP.
    --  This has to be in the SP, not the trigger, because there are situations when no records as changed, inserted, deleted in the main table (hence trigger is not fired) 
    --  but this record needs to be inserted in the history table.
    insert into edgis.gis_circuit_fnm_lca_history
    (
        circuitid,
        source_feeder,
        bus_id,
        cnode_id,
        lap_id,
        lca_id,
        start_date,
        end_date,
        createuser,
        datecreated,
        updateuser,
        dateupdated,
        refresh_date,
        action,
        actiondate
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
            end_date,
            createuser,
            datecreated,
            updateuser,
            dateupdated,
            refresh_date,
            'DROP',
            sysdate
        from edgis.gis_circuit_fnm_lca
        where 
            (circuitid, start_date) not in 
                (select circuitid, start_date from edgis.gis_circuit_fnm_lca_temp)
            and (circuitid, start_date) not in 
            (
                select 
                    circuitid, start_date 
                from edgis.gis_circuit_fnm_lca_history 
                where 
                    (circuitid, start_date, actiondate)
                    in
                    (
                        select circuitid, start_date, max(actiondate) 
                        from edgis.gis_circuit_fnm_lca_history 
                        group by circuitid, start_date
                    )
                    and upper(action) = 'DROP'
            )
    );

    commit;
      
    --empty the temporary table
    delete from edgis.gis_circuit_fnm_lca_temp;
    commit;
    
end gis_circuit_fnm_lca_refresh;
/




create or replace PROCEDURE "GIS_SPP_FEEDER_SUBLAP_PROC" AS

/*
The following tables are required for this procedure:
    EDGIS.GIS_SPP_ZONE
    EDGIS.MDSS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_SPP_FEEDER_SUBLAP
    EDGIS.GIS_CIRCUIT_FNM_LCA
*/

v_sysdate date; -- holds the truncated sysdate.
v_threshold int;  -- the maximum number of days allowed for sysdate - refresh_date.

begin

--sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
    commit;

    v_sysdate := trunc(sysdate); -- use truncated sysdate when comparing sysdate to end_date.  Do not use when comparing to refresh_date
    v_threshold := 7; -- this is the maximum allows 

--Deletes existing data in edgis.gis_spp_feeder_sublap
	delete from edgis.gis_spp_feeder_sublap;
    commit;

--copies the current data from spp_feeder_sublap to gis_spp_feeder_sublap
    insert into edgis.gis_spp_feeder_sublap
        select * from edgis.mdss_spp_feeder_sublap;
    commit;

-- deletes feeders that are no longer in edgis.gis_circuit_fnm_lca 
-- the remaining feeders in edgis.gis_spp_feeder_sublap are the one that are currently active
    delete from edgis.gis_spp_feeder_sublap where feeder_num not in
    (
        select to_number(circuitid) from edgis.gis_circuit_fnm_lca
        where
            v_sysdate between start_date and end_date
            and refresh_date between start_date and end_date
            and sysdate - refresh_date between 0 and v_threshold        
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
                                v_sysdate between start_date and end_date
                                and refresh_date between start_date and end_date
                                and sysdate - refresh_date between 0 and v_threshold   
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
                                v_sysdate between start_date and end_date
                                and refresh_date between start_date and end_date
                                and sysdate - refresh_date between 0 and v_threshold
                            )
                        where
                            to_number(circuitid) = tt.feeder_num
                            and
                            (
                                pnode_id <> tt.pnode_id or tt.pnode_id is null or sublap_id <> tt.sublap_id or tt.sublap_id is null
                            )
                    );
    commit;

-- inserts new feeders and their associated attributes into edgis.gis_spp_feeder_sublap.  A new feeder is a feeder that is
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
        and v_sysdate between start_date and end_date
        and refresh_date between start_date and end_date
        and sysdate - refresh_date between 0 and v_threshold;  
        
    commit;

end GIS_SPP_FEEDER_SUBLAP_PROC;
/




create or replace procedure gis_circuit_circuit_refresh as
/*
    add description
*/

cursor c1 is
    select distinct
        circuitid
    from edgis.zz_mv_circuitsource@to_eddm
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
                        from edgis.zz_mv_circuitsource@to_eddm edcs1
                        left outer join edgis.zz_mv_subelectricstitchpoint substpt1 on edcs1.deviceguid = substpt1.electricstitchpointguid
                        left outer join edgis.zz_mv_subelectricstitchpoint substpt2 on substpt1.circuitid = substpt2.circuitid
                        left outer join edgis.zz_mv_electricstitchpoint@to_eddm stpt on substpt2.electricstitchpointguid = stpt.globalid
                        left outer join edgis.zz_mv_circuitsource@to_eddm edcs2 on nvl(stpt.circuitid, edcs1.circuitid) = edcs2.circuitid
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

GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA_TEMP TO SDE_EDITOR;
GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA_TEMP TO GIS_SUB_MDSS_RW;
GRANT SELECT ON GIS_CIRCUIT_FNM_LCA_TEMP TO SDE_VIEWER;

GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA TO SDE_EDITOR;
GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA TO GIS_SUB_MDSS_RW;
GRANT SELECT ON GIS_CIRCUIT_FNM_LCA TO SDE_VIEWER;

GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA_HISTORY TO SDE_EDITOR;
GRANT SELECT,UPDATE,DELETE,INSERT ON GIS_CIRCUIT_FNM_LCA_HISTORY TO GIS_SUB_MDSS_RW;
GRANT SELECT ON GIS_CIRCUIT_FNM_LCA_HISTORY TO SDE_VIEWER;

GRANT EXECUTE ON GIS_SPP_ZONE_PROC TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_CIRCUIT_FNM_LCA_REFRESH TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_CIRCUIT_CIRCUIT_REFRESH TO GIS_SUB_MDSS_RW;

set echo off
spool off;
