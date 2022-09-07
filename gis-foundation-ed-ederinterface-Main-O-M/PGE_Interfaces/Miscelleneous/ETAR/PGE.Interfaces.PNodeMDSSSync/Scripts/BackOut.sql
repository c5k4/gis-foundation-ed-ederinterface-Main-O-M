set echo on
spool D:\Temp\DBChanges_backout.txt


DROP TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_TEMP";

  CREATE TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_TEMP" 
   (	"CIRCUITID" VARCHAR2(9 BYTE), 
	"SOURCE_FEEDER" VARCHAR2(9 BYTE), 
	"BUS_ID" VARCHAR2(254 BYTE), 
	"CNODE_ID" VARCHAR2(254 BYTE), 
	"LAP_ID" VARCHAR2(254 BYTE), 
	"LCA_ID" VARCHAR2(254 BYTE), 
	"START_DATE" DATE, 
	"END_DATE" DATE
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
   (	"CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"SOURCE_FEEDER" VARCHAR2(9 BYTE), 
	"BUS_ID" VARCHAR2(254 BYTE), 
	"CNODE_ID" VARCHAR2(254 BYTE), 
	"LAP_ID" VARCHAR2(254 BYTE), 
	"LCA_ID" VARCHAR2(254 BYTE), 
	"START_DATE" DATE NOT NULL ENABLE, 
	"END_DATE" DATE, 
	"CREATEUSER" VARCHAR2(20 BYTE) DEFAULT 'EDGIS', 
	"DATECREATED" DATE DEFAULT SYSDATE, 
	 CONSTRAINT "GIS_TABLE1_PK" PRIMARY KEY ("CIRCUITID", "START_DATE")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS NOLOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS"  ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA"  IS 'This table maps a Feeder to its BUS_ID, PNODE_ID, SUBLAP_ID, and LCA_ID.  The feeder values are for current only.  Even though this table has start dates and end dates, these dates are directly from the GIS_FMN_LCA view.  So the dates are for the relationships between buses, pnodes, sublaps, and LCAs only.';
  
  CREATE INDEX "EDGIS"."GIS_CIRCUIT_FNM_LCA_CIRCUITID" ON "EDGIS"."GIS_CIRCUIT_FNM_LCA" ("CIRCUITID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS NOLOGGING 
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
	"ACTION" VARCHAR2(20 BYTE), 
	"ACTIONDATE" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY"  IS 'This is a trigger driven table that tracks changes to GIS_CIRCUIT_FNM_LCA.';

  CREATE INDEX "EDGIS"."GIS_CIRCUIT_FNM_LCA_HIST_IDX" ON "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" ("CIRCUITID", "BUS_ID", "CNODE_ID", "START_DATE") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS NOLOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;


CREATE OR REPLACE TRIGGER "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" 
after delete or insert on edgis.gis_circuit_fnm_lca
referencing old as old new as new
for each row

begin

    if inserting then
        -- Insert record into HISTORY table
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
            'INSERT',
            sysdate
        );
    end if;

    if deleting then
        -- Insert deleted record into HISTORY table
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
            'DELETE',
            sysdate
        );
    end if;
end;
/

ALTER TRIGGER "EDGIS"."GIS_CIRCUIT_FNM_LCA_HISTORY" ENABLE;


-- Keep procedure GIS_CIRCUIT_FNM_LCA_REFRESH defination from prod 


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

-- Keep procedure GIS_SPP_FEEDER_SUBLAP_PROC defination from prod 


create or replace PROCEDURE         "GIS_SPP_FEEDER_SUBLAP_PROC" AS

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

-- Keep procedure gis_circuit_circuit_refresh defination from prod 

create or replace procedure gis_circuit_circuit_refresh as
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
