
spool D:\Temp\BackUp.txt

-- Tables ---------------

DROP TABLE EDGIS.GIS_CIRCUIT_CIRCUIT CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_CIRCUIT_CIRCUIT_TEMP CASCADE CONSTRAINTS;

DROP TABLE EDGIS.GIS_CIRCUIT_FNM_LCA CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_CIRCUIT_FNM_LCA_HISTORY CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_CIRCUIT_FNM_LCA_TEMP CASCADE CONSTRAINTS;

DROP TABLE EDGIS.GIS_PNODE_CIRCUIT_INFO_HISTORY CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_PNODE_CIRCUIT_INFO_TEMP CASCADE CONSTRAINTS;

DROP TABLE EDGIS.GIS_PNODE_CIRCUIT_INFO CASCADE CONSTRAINTS;

DROP TABLE EDGIS.GIS_PNODEINFO_TEMP CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_PNODEINFO_TEMP2 CASCADE CONSTRAINTS;
DROP TABLE EDGIS.GIS_PNODEINFO_HISTORY CASCADE CONSTRAINTS;

DROP TRIGGER EDGIS.GIS_PNODEINFO_HISTORY;


-- Create again table GIS_PNODE_CIRCUIT_INFO from current production table structure

CREATE TABLE EDGIS.GIS_PNODE_CIRCUIT_INFO
(
  EDCS1_CIRCUITID               NVARCHAR2(9)    NOT NULL,
  EDCS1_OBJECTID                NUMBER(38),
  EDCS1_GLOBALID                CHAR(38 BYTE),
  EDCS1_SUBSTATIONID            NVARCHAR2(30),
  EDCS1_DEVICEGUID              CHAR(38 BYTE),
  EDCS1_STATUS                  NUMBER(5),
  EDCS2_CIRCUITID               NVARCHAR2(9),
  EDCS2_OBJECTID                NUMBER(38),
  EDCS2_GLOBALID                CHAR(38 BYTE),
  EDCS2_SUBSTATIONID            NVARCHAR2(30),
  EDCS2_DEVICEGUID              CHAR(38 BYTE),
  EDCS2_STATUS                  NUMBER(5),
  SUBSTPT1_OBJECTID             NUMBER(38),
  SUBSTPT1_STITCHPOINTID        NVARCHAR2(32),
  SUBSTPT1_GLOBALID             CHAR(38 BYTE),
  SUBSTPT1_CIRCUITID            NVARCHAR2(9),
  SUBSTPT1_ELECSTITCHPTGUID     CHAR(38 BYTE),
  SUBSTPT2_OBJECTID             NUMBER(38),
  SUBSTPT2_STITCHPOINTID        NVARCHAR2(32),
  SUBSTPT2_GLOBALID             CHAR(38 BYTE),
  SUBSTPT2_CIRCUITID            NVARCHAR2(9),
  SUBSTPT2_ELECSTITCHPTGUID     CHAR(38 BYTE),
  GISTRACE_TO_FEATURE_GLOBALID  CHAR(38 BYTE),
  PINFO_LINE_FC_NAME            VARCHAR2(6 BYTE),
  PINFO_LINE_GUID               CHAR(38 BYTE),
  PINFO_LINE_OID                NUMBER,
  PINFO_SUBPNODE_OID            NUMBER,
  PINFO_FNM_GLOBALID            CHAR(38 BYTE),
  PINFO_BUS_ID                  NVARCHAR2(254),
  PINFO_FNM_CNODEID             NVARCHAR2(254),
  PINFO_LAP_ID                  NVARCHAR2(254),
  PINFO_LCA_ID                  NVARCHAR2(50),
  PINFO_TREELEVEL               NUMBER,
  PINFO_QA_FLAG                 CHAR(5 BYTE)
)
TABLESPACE EDGIS
RESULT_CACHE (MODE DEFAULT)
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MAXSIZE          UNLIMITED
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
            FLASH_CACHE      DEFAULT
            CELL_FLASH_CACHE DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO GIS_SUB_MDSS_RW;
GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SDE_VIEWER;
GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SJD5;

-- Tables ---------------

-- Views ---------------

DROP VIEW GIS_CIRCUIT_CIRCUIT_COMPRESSED;
DROP VIEW GIS_FNM_LCA;
DROP VIEW GIS_FNM_VERSION_DATES;

-- Create again view GIS_CIRCUIT_FNM_VALUES from current production view structure

CREATE OR REPLACE VIEW "EDGIS"."GIS_CIRCUIT_FNM_VALUES" ("CIRCUITID", "SUBSTATION_ID", "BUS_ID", "PNODE_ID", "SUBLAP_ID", "LCA_ID") AS 
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

-- Views ---------------

-- Indexes_Triggers_Constraint ---------------

DROP INDEX FNM_COMPLETE_PNODE_BUS;
DROP INDEX FNM_HISTORY_PNODE_BUS;
DROP INDEX FNM_PNODE_BUS;

DROP INDEX GIS_PNODEINFO_CIRCUITID;
DROP INDEX GIS_PNODEINFO_FNM_PNODE_GUID;
DROP INDEX GIS_PNODEINFO_LINE_GUID;

DROP INDEX GIS_SERVICEPOINTS_INFO_CIRCUIT;
DROP INDEX GIS_SERVICEPOINTS_INFO_SA;
DROP INDEX GIS_SERVICEPOINTS_INFO_SP;

DROP INDEX GIS_SPP_FEEDER_SUBLAP_FEEDER;

-- Indexes_Triggers_Constraint ---------------

-- Procedures ---------------

DROP PROCEDURE GIS_CIRCUIT_CIRCUIT_REFRESH;
DROP PROCEDURE GIS_CIRCUIT_FNM_LCA_REFRESH;
DROP PROCEDURE GIS_PNODE_CIRCUIT_INFO_REFRESH;

-- Recreate all others procedures from the production back up 

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_COMPLETE_PNODEINFO" AS
-- Latest edits 3/4/2020 4:15 pm

BEGIN
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
COMMIT;

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
END GIS_COMPLETE_PNODEINFO;
/

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_COPY_MDSS_DATA" AS
--This is for error handling case, if for some reason, 'GIS_SPP_ZONE' and 'GIS_SPP_FEEDER_SUBLAP' tables have zero records
--then copy the data from repective MDSS tables into those tables.
  BEGIN
    INSERT INTO EDGIS.GIS_SPP_ZONE
    SELECT * FROM EDGIS.MDSS_SPP_ZONE;
    COMMIT;

    INSERT INTO EDGIS.GIS_SPP_FEEDER_SUBLAP
    SELECT * FROM EDGIS.MDSS_SPP_FEEDER_SUBLAP;
    COMMIT;
END GIS_COPY_MDSS_DATA;
/
  
CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_INSERT_FNM_COMP" AS
BEGIN
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
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_INSERT_FNM_FROM_FNM_COMP" AS
-- this will need to be modified or an additional sp will be needed to include lca
BEGIN
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
            LATEST_FNM_RELEASEDATE = (select max(LATEST_FNM_RELEASEDATE) from EDGIS.FNM_COMPLETE where LATEST_FNM_RELEASEDATE <= sysdate) OR -- Most recent version that is less than today's date.
            --latest_fnm_releasedate > sysdate or -- All future versions
            LATEST_FNM_RELEASEDATE is null -- Manual overrides
        )
  );
--   EXCEPTION
--    WHEN OTHERS THEN
--    RAISE_APPLICATION_ERROR(-20000, 'An error occured.');
--	--DBMS_OUTPUT.PUT_LINE('Table does not exist.');
  commit;
END GIS_INSERT_FNM_FROM_FNM_COMP;
/
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_MERGE_FNM_COMP" AS
BEGIN
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
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_MERGE_FNM_FROM_FNM_COMP" AS
-- this will need to be modified or an additional sp will be needed to include lca
BEGIN
  MERGE INTO EDGIS.FNM TARGET
   USING
   (
        select * from fnm_complete
            where
            latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm_complete where latest_fnm_releasedate <= sysdate) or -- Most recent version that is less than today's date.
            --latest_fnm_releasedate > sysdate or -- All future versions
            latest_fnm_releasedate is null -- Manual overrides)
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
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_PNODE_SERVICEPOINT_DATA" AS
-- Latest edits 3/4/2020 4:15 pm

BEGIN
    --sets the version to default
    sde.version_util.set_current_version('SDE.DEFAULT');
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

        from edgis.zz_mv_circuitsource@to_eddm edcs1
        left outer join edgis.zz_mv_subelectricstitchpoint substpt1 on edcs1.deviceguid = substpt1.electricstitchpointguid
        left outer join edgis.zz_mv_subelectricstitchpoint substpt2 on substpt1.circuitid = substpt2.circuitid
        left outer join edgis.zz_mv_electricstitchpoint@to_eddm stpt on substpt2.electricstitchpointguid = stpt.globalid
        left outer join edgis.zz_mv_circuitsource@to_eddm edcs2 on nvl(stpt.circuitid, edcs1.circuitid) = edcs2.circuitid
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
            substpt1.subtypecd = 2
            and substpt2.subtypecd = 1
    ) substpt on edcs.edcs2_deviceguid = substpt.substpt2_elecstitchptguid
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
    ) tp on substpt2_globalid = tp.gistrace_to_feature_globalid
    where
        pinfo_bus_id is not null
    order by
         edcs.edcs1_circuitid;


-- the following is the old code which was adjusted to account for subtypecd 1 verus 2 and other related changes
/*
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
*/
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
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_SPP_FEEDER_SUBLAP_PROC" AS
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
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_SPP_ZONE_PROC" as

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

        ZONE_ID ¿¿¿ MDSS has priority.  GIS will create and assign zone_ids for only new sublaps and pnodes.  MDSS can modify
        the zone_ids assigned by GIS.  GIS will import these changes as part of its daily job.

        ZONE_TYPE_ID ¿¿¿ GIS will set this value for new sublaps and pnodes.  Use 1501 for sublaps, and 1507 for pnodes.
        If MDSS changes these values, then GIS will not undo the change.  NEED TO VALIDATE because several queries use
        ZONE_TYPE_ID.

        ZONE_CODE ¿¿¿ GIS will set this to null for new ZONE_TYPE_ID in (1501, 1507).  If MDSS changes this value,
        then GIS will not undo to the change.

        ZONE_NAME ¿¿¿ GIS will set this value for new sublaps and pnodes.  If MDSS changes these values to a value not in the FNM,
        then GIS will treat the renamed zone as a special zone.  GIS would create a new zone (with the original zone name) to
        re-create to original zone.

        ZONE_DESC ¿¿¿ GIS will set this value for new sublaps and pnodes.  If MDSS changes these values, then GIS
        will not undo the change.  If ZONE_TYPE_ID is 1501, then use ¿¿¿LOOKUP TABLE FOR SUBLAPS¿¿¿.  If ZONE_TYPE_ID is 1507,
        then use ¿¿¿LOOKUP TABLE FOR PRICING NODES¿¿¿.  If MDSS changes this value, then GIS will not undo the change.

        ARCHIVE_DATE ¿¿¿ GIS sets this value when a sublap or pnode is no longer in use according to the FNM.  ***************
            If the sublap or pnode is inactive according to the FNM:
                If MDSS changes a date value back to null, then GIS will set the archive date to sysdate. *********
                If MDSS changes this date to another date, then GIS will not undo this change.  ********
            If the sublap or pnode is active according to the FNM:
                If MDSS populates the field with a date, then GIS will create a new zone to reactivate the zone.   *******

        PARENT_ZONE_ID ¿¿¿
            Sublaps -- Value is always null for sublaps.  If MDSS changes this to a non-null value, then GIS
                        will change the value back to null.
            Pnodes -- GIS sets this value for new pnodes.  GIS maintains this value for pnodes.  So GIS will update when
                        necessary.  If MDSS changes this value for a pnode, then GIS will undo the change.

        RULE24_ZONE_ID_MAPPING ¿¿¿ Always set to ZONE_ID when ZONE_TYPE_ID in (1501, 1507) and when the pnode or sublap is
        in the FNM. If MDSS changes this value, GIS will not undo the change.

        DISPLAY_RULE24 ¿¿¿ If the pnode or sublap is in the FNM, then set to 1.

        CREATE_DATE ¿¿¿ Set by GIS for new sublaps and pnodes.   If MDSS changes this date or changes it to null, then GIS,
        will not undo MDSS' change.

        UPDATE_DATE ¿¿¿ Set by GIS for changes sublaps and pnodes.  If MDSS changes this date or changes it to null, then GIS,
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
    DBMS_OUTPUT.PUT_LINE('0');
    v_zone_id := null;
    v_zone_name := null;
    v_max_zone_id_in_use := null;

    -- deletes all rows from edgis.gis_spp_zone
    --execute immediate 'truncate table edgis.gis_spp_zone';
    DBMS_OUTPUT.PUT_LINE('1');
    delete from edgis.gis_spp_zone;
    commit;

    -- copies the data from spp_zone to gis_spp_zone
    DBMS_OUTPUT.PUT_LINE('2');
    insert into edgis.gis_spp_zone
        select * from edgis.mdss_spp_zone;
    commit;

    -- determines the greatest zone_id in MDSS spp_zone.
    DBMS_OUTPUT.PUT_LINE('3');
    select nvl(max(zone_id), 0) into v_max_zone_id_in_use from edgis.gis_spp_zone;
    --DBMS_OUTPUT.PUT_LINE('max_zone_id_in_use = ' || TO_CHAR(v_max_zone_id_in_use));

    -- this eliminates all zone_type_ids that are not related to pnodes and sublaps.
    DBMS_OUTPUT.PUT_LINE('4');
    delete from edgis.gis_spp_zone where zone_type_id not in (1501, 1507);  -- conider deleting all zones with an archive date and zone_id = 122, 123  ******************
    commit;

    -- sets an unpopulated archive_date in edgis.gis_spp_zone for those zones that are in FNM but the latest_fnm_releasedate is not the most recent version" *****************
    DBMS_OUTPUT.PUT_LINE('5');
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
    DBMS_OUTPUT.PUT_LINE('6');
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
    DBMS_OUTPUT.PUT_LINE('7');
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

    DBMS_OUTPUT.PUT_LINE('8');
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
    DBMS_OUTPUT.PUT_LINE('9');
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
    DBMS_OUTPUT.PUT_LINE('10');
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

DBMS_OUTPUT.PUT_LINE('11');
end GIS_SPP_ZONE_PROC;
/
  

CREATE OR REPLACE  PROCEDURE "EDGIS"."GIS_UPDATE_FNM_LCA_ID" AS
BEGIN
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
GRANT EXECUTE ON GIS_SPP_FEEDER_SUBLAP_PROC TO GIS_SUB_MDSS_RW;
GRANT EXECUTE ON GIS_SPP_ZONE_PROC TO GIS_SUB_MDSS_RW;

GRANT EXECUTE ON GIS_UPDATE_FNM_LCA_ID TO GIS_SUB_MDSS_RW;


-- Procedures ---------------


spool off