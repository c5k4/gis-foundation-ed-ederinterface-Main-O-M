
spool D:\Temp\TableChanges.txt

--- Create all tables 

  DROP TABLE GIS_PNODE_CIRCUIT_INFO CASCADE CONSTRAINTS;

  CREATE TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT" 
   (	"CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"DOWNSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"UPSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"LEVEL_NUM" NUMBER NOT NULL ENABLE, 
	"CREATEUSER" VARCHAR2(20 BYTE) DEFAULT 'EDGIS' NOT NULL ENABLE, 
	"DATECREATED" DATE DEFAULT SYSDATE NOT NULL ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT"  IS 'This table lists the active feeders. Most feeders are feed direct from a substation.  These feeders are included in the table to simplify data processing.  In this case the supplying feeder is equal to the active feeder.  These are indicated by a single row with LEVEL_NUM = 0. Some substations (and hence its feeders) are fed by a feeder as compared to a transmission line. These appear as two rows.  The first upstream feeder is indicated by a single row with LEVEL_NUM = 1.The next upstream supplying feeder is indicated by a single row with LEVEL_NUM = 0. Some substations (and hence its feeders) are fed by a feeder that is fed by a substation that is fed by a feeder. These appear as three rows.  The first upstream feeder is indicated by a single row with LEVEL_NUM = 2. The next upstream supplying feeder is indicated by a single row with LEVEL_NUM = 1. The next upstream supplying feeder is indicated by a single row with LEVEL_NUM = 0.';

  CREATE UNIQUE INDEX "EDGIS"."GIS_CIRCUIT_CIRCUIT_CIRID_LVL" ON "EDGIS"."GIS_CIRCUIT_CIRCUIT" ("CIRCUITID", "LEVEL_NUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;
  
  CREATE INDEX "EDGIS"."GIS_CIRCUIT_CIRCUIT_CIRCUITID" ON "EDGIS"."GIS_CIRCUIT_CIRCUIT" ("CIRCUITID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

 

  CREATE TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT_HISTORY" 
   (	"CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"DOWNSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"UPSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"LEVEL_NUM" NUMBER NOT NULL ENABLE, 
	"CREATEUSER" VARCHAR2(20 BYTE) NOT NULL ENABLE, 
	"DATECREATED" DATE NOT NULL ENABLE, 
	"ACTION" VARCHAR2(20 BYTE) NOT NULL ENABLE, 
	"ACTIONDATE" DATE DEFAULT SYSDATE NOT NULL ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT_HISTORY"  IS 'This is a trigger driven table that tracks changes to GIS_CIRCUIT_CIRCUIT.';

  CREATE INDEX "EDGIS"."GIS_CIR_CIR_HIST_CIRCUITID" ON "EDGIS"."GIS_CIRCUIT_CIRCUIT_HISTORY" ("CIRCUITID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

 CREATE OR REPLACE TRIGGER "EDGIS"."GIS_CIRCUIT_CIRCUIT_HISTORY" 
    AFTER DELETE OR INSERT ON EDGIS.GIS_CIRCUIT_CIRCUIT 
    FOR EACH ROW
BEGIN

IF INSERTING THEN
    -- Insert record into HISTORY table
    INSERT INTO EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY
    ( 
        CIRCUITID,
        DOWNSTREAM_CIRCUITID,
        UPSTREAM_CIRCUITID,
        LEVEL_NUM,
        CREATEUSER,
        DATECREATED,
        ACTION,
        ACTIONDATE
    )
    VALUES
    ( 
        :NEW.CIRCUITID,
        :NEW.DOWNSTREAM_CIRCUITID,
        :NEW.UPSTREAM_CIRCUITID,
        :NEW.LEVEL_NUM,
        :NEW.CREATEUSER,
        :NEW.DATECREATED,
        'INSERT',
        SYSDATE 
    );
END IF;

IF DELETING THEN
    -- Insert deleted-record into HISTORY table 
    INSERT INTO EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY
    ( 
        CIRCUITID,
        DOWNSTREAM_CIRCUITID,
        UPSTREAM_CIRCUITID,
        LEVEL_NUM,
        CREATEUSER,
        DATECREATED,
        ACTION,
        ACTIONDATE
    )
    VALUES
    ( 
        :OLD.CIRCUITID,
        :OLD.DOWNSTREAM_CIRCUITID,
        :OLD.UPSTREAM_CIRCUITID,
        :OLD.LEVEL_NUM,
        :OLD.CREATEUSER,
        :OLD.DATECREATED,
        'DELETE',
        SYSDATE 
    );
END IF;
END;
/
ALTER TRIGGER "EDGIS"."GIS_CIRCUIT_CIRCUIT_HISTORY" ENABLE;

  CREATE TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT_TEMP" 
   (	"CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"DOWNSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"UPSTREAM_CIRCUITID" VARCHAR2(9 BYTE) NOT NULL ENABLE, 
	"LEVEL_NUM" NUMBER NOT NULL ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_CIRCUIT_CIRCUIT_TEMP"  IS 'This is a staging table for GIS_CIRCUIT_CIRCUIT.';

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
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
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
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

  

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
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
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

  CREATE TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO_HISTORY" 
   (	"EDCS_CIRCUITID" NVARCHAR2(9) NOT NULL ENABLE, 
	"EDCS_OBJECTID" NUMBER(38,0), 
	"EDCS_GLOBALID" CHAR(38 BYTE), 
	"EDCS_SUBSTATIONID" NVARCHAR2(30), 
	"EDCS_DEVICEGUID" CHAR(38 BYTE), 
	"EDCS_STATUS" NUMBER(5,0), 
	"SUBSTPT_OBJECTID" NUMBER(38,0), 
	"SUBSTPT_STITCHPOINTID" NVARCHAR2(32), 
	"SUBSTPT_GLOBALID" CHAR(38 BYTE), 
	"SUBSTPT_CIRCUITID" NVARCHAR2(9), 
	"SUBSTPT_ELECSTITCHPTGUID" CHAR(38 BYTE), 
	"GISTRACE_TO_FEATURE_GLOBALID" CHAR(38 BYTE), 
	"GISTRACE_FEEDERID" VARCHAR2(20 BYTE), 
	"GISTRACE_ORDER_NUM" NUMBER, 
	"GISTRACE_MIN_BRANCH" NUMBER, 
	"GISTRACE_MAX_BRANCH" NUMBER, 
	"GISTRACE_TREELEVEL" NUMBER, 
	"PINFO_CIRCUIT_ID" VARCHAR2(20 BYTE), 
	"PINFO_LINE_FC_NAME" VARCHAR2(6 BYTE), 
	"PINFO_LINE_GUID" CHAR(38 BYTE), 
	"PINFO_LINE_OID" NUMBER, 
	"PINFO_SUBPNODE_OID" NUMBER, 
	"PINFO_FNM_GLOBALID" CHAR(38 BYTE), 
	"PINFO_BUS_ID" NVARCHAR2(254), 
	"PINFO_CNODE_ID" NVARCHAR2(254), 
	"PINFO_ORDER_NUM" NUMBER, 
	"PINFO_MIN_BRANCH" NUMBER, 
	"PINFO_MAX_BRANCH" NUMBER, 
	"PINFO_TREELEVEL" NUMBER, 
	"PINFO_QA_FLAG" VARCHAR2(255 BYTE), 
	"EXCLUDE" VARCHAR2(20 BYTE), 
	"CREATEUSER" VARCHAR2(20 BYTE), 
	"DATECREATED" DATE, 
	"ACTION" VARCHAR2(20 BYTE), 
	"ACTIONDATE" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO_HISTORY"  IS 'This is a trigger driven table that tracks changes to GIS_PNODE_CIRCUIT_INFO.';

  CREATE INDEX "EDGIS"."GIS_PNODE_CIRCT_INFO_HIST_IDX" ON "EDGIS"."GIS_PNODE_CIRCUIT_INFO_HISTORY" ("EDCS_CIRCUITID", "PINFO_BUS_ID", "PINFO_CNODE_ID", "ACTION") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

  CREATE TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO_TEMP" 
   (	"EDCS_CIRCUITID" NVARCHAR2(9) NOT NULL ENABLE, 
	"EDCS_OBJECTID" NUMBER(38,0), 
	"EDCS_GLOBALID" CHAR(38 BYTE), 
	"EDCS_SUBSTATIONID" NVARCHAR2(30), 
	"EDCS_DEVICEGUID" CHAR(38 BYTE), 
	"EDCS_STATUS" NUMBER(5,0), 
	"SUBSTPT_OBJECTID" NUMBER(38,0), 
	"SUBSTPT_STITCHPOINTID" NVARCHAR2(32), 
	"SUBSTPT_GLOBALID" CHAR(38 BYTE), 
	"SUBSTPT_CIRCUITID" NVARCHAR2(9), 
	"SUBSTPT_ELECSTITCHPTGUID" CHAR(38 BYTE), 
	"GISTRACE_TO_FEATURE_GLOBALID" CHAR(38 BYTE), 
	"GISTRACE_FEEDERID" VARCHAR2(20 BYTE), 
	"GISTRACE_ORDER_NUM" NUMBER, 
	"GISTRACE_MIN_BRANCH" NUMBER, 
	"GISTRACE_MAX_BRANCH" NUMBER, 
	"GISTRACE_TREELEVEL" NUMBER, 
	"PINFO_CIRCUIT_ID" VARCHAR2(20 BYTE), 
	"PINFO_LINE_FC_NAME" VARCHAR2(6 BYTE), 
	"PINFO_LINE_GUID" CHAR(38 BYTE), 
	"PINFO_LINE_OID" NUMBER, 
	"PINFO_SUBPNODE_OID" NUMBER, 
	"PINFO_FNM_GLOBALID" CHAR(38 BYTE), 
	"PINFO_BUS_ID" NVARCHAR2(254), 
	"PINFO_CNODE_ID" NVARCHAR2(254), 
	"PINFO_ORDER_NUM" NUMBER, 
	"PINFO_MIN_BRANCH" NUMBER, 
	"PINFO_MAX_BRANCH" NUMBER, 
	"PINFO_TREELEVEL" NUMBER, 
	"PINFO_QA_FLAG" VARCHAR2(255 BYTE), 
	"EXCLUDE" VARCHAR2(20 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO_TEMP"  IS 'This is a staging table for GIS_PNODE_CIRCUIT_INFO.';



  CREATE TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO" 
   (	"EDCS_CIRCUITID" NVARCHAR2(9) NOT NULL ENABLE, 
	"EDCS_OBJECTID" NUMBER(38,0), 
	"EDCS_GLOBALID" CHAR(38 BYTE), 
	"EDCS_SUBSTATIONID" NVARCHAR2(30), 
	"EDCS_DEVICEGUID" CHAR(38 BYTE), 
	"EDCS_STATUS" NUMBER(5,0), 
	"SUBSTPT_OBJECTID" NUMBER(38,0), 
	"SUBSTPT_STITCHPOINTID" NVARCHAR2(32), 
	"SUBSTPT_GLOBALID" CHAR(38 BYTE), 
	"SUBSTPT_CIRCUITID" NVARCHAR2(9), 
	"SUBSTPT_ELECSTITCHPTGUID" CHAR(38 BYTE), 
	"GISTRACE_TO_FEATURE_GLOBALID" CHAR(38 BYTE), 
	"GISTRACE_FEEDERID" VARCHAR2(20 BYTE), 
	"GISTRACE_ORDER_NUM" NUMBER, 
	"GISTRACE_MIN_BRANCH" NUMBER, 
	"GISTRACE_MAX_BRANCH" NUMBER, 
	"GISTRACE_TREELEVEL" NUMBER, 
	"PINFO_CIRCUIT_ID" VARCHAR2(20 BYTE), 
	"PINFO_LINE_FC_NAME" VARCHAR2(6 BYTE), 
	"PINFO_LINE_GUID" CHAR(38 BYTE), 
	"PINFO_LINE_OID" NUMBER, 
	"PINFO_SUBPNODE_OID" NUMBER, 
	"PINFO_FNM_GLOBALID" CHAR(38 BYTE), 
	"PINFO_BUS_ID" NVARCHAR2(254), 
	"PINFO_CNODE_ID" NVARCHAR2(254), 
	"PINFO_ORDER_NUM" NUMBER, 
	"PINFO_MIN_BRANCH" NUMBER, 
	"PINFO_MAX_BRANCH" NUMBER, 
	"PINFO_TREELEVEL" NUMBER, 
	"PINFO_QA_FLAG" VARCHAR2(255 BYTE), 
	"EXCLUDE" VARCHAR2(20 BYTE), 
	"CREATEUSER" VARCHAR2(20 BYTE) DEFAULT 'EDGIS', 
	"DATECREATED" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

   COMMENT ON TABLE "EDGIS"."GIS_PNODE_CIRCUIT_INFO"  IS 'This table contains the relationship between a pnode and a feeder.  It contains data that is also in GIS_PNODEINFO but has the added feeder information.  A feeder may be included in multiple rows because a feeder can map to multiple pnodes.  This is OK if the PINFO_TREELEVEL is different.  This table includes fields PINFO_QA_FLAG and EXCLUDE which are used to flag issues and exclude from down stream process when appropriate.';

  CREATE INDEX "EDGIS"."GIS_PNODE_CIRCUIT_INFO_IDX" ON "EDGIS"."GIS_PNODE_CIRCUIT_INFO" ("EDCS_CIRCUITID", "PINFO_BUS_ID", "PINFO_CNODE_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "EDGIS" ;

  CREATE OR REPLACE TRIGGER "EDGIS"."GIS_PNODE_CIRCUIT_INFO_HISTORY" 
    after insert or delete on edgis.gis_pnode_circuit_info 
    for each row
    --when (new.pinfo_qa_flag is not null or new.exclude is not null)
begin
    if inserting then 
            -- Insert record into HISTORY table
            insert into edgis.gis_pnode_circuit_info_history
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
                exclude,
                createuser,
                datecreated,
                action,
                actiondate
            )
            
            values
            ( 
                :new.edcs_circuitid,
                :new.edcs_objectid,
                :new.edcs_globalid,
                :new.edcs_substationid,
                :new.edcs_deviceguid,
                :new.edcs_status,
                :new.substpt_objectid,
                :new.substpt_stitchpointid,
                :new.substpt_globalid,
                :new.substpt_circuitid,
                :new.substpt_elecstitchptguid,
                :new.gistrace_to_feature_globalid,
                :new.gistrace_feederid,
                :new.gistrace_order_num,
                :new.gistrace_min_branch,
                :new.gistrace_max_branch,
                :new.gistrace_treelevel,
                :new.pinfo_circuit_id,
                :new.pinfo_line_fc_name,
                :new.pinfo_line_guid,
                :new.pinfo_line_oid,
                :new.pinfo_subpnode_oid,
                :new.pinfo_fnm_globalid,
                :new.pinfo_bus_id,
                :new.pinfo_cnode_id,
                :new.pinfo_order_num,
                :new.pinfo_min_branch,
                :new.pinfo_max_branch,
                :new.pinfo_treelevel,
                :new.pinfo_qa_flag,
                :new.exclude,
                :new.createuser,
                :new.datecreated,
                'INSERT',
                sysdate
            );      
    end if;
    
    if deleting then 
        -- Insert deleted record into HISTORY table
        insert into edgis.gis_pnode_circuit_info_history
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
            exclude,
            createuser,
            datecreated,
            action,
            actiondate
        )
        
        values
        ( 
            :old.edcs_circuitid,
            :old.edcs_objectid,
            :old.edcs_globalid,
            :old.edcs_substationid,
            :old.edcs_deviceguid,
            :old.edcs_status,
            :old.substpt_objectid,
            :old.substpt_stitchpointid,
            :old.substpt_globalid,
            :old.substpt_circuitid,
            :old.substpt_elecstitchptguid,
            :old.gistrace_to_feature_globalid,
            :old.gistrace_feederid,
            :old.gistrace_order_num,
            :old.gistrace_min_branch,
            :old.gistrace_max_branch,
            :old.gistrace_treelevel,
            :old.pinfo_circuit_id,
            :old.pinfo_line_fc_name,
            :old.pinfo_line_guid,
            :old.pinfo_line_oid,
            :old.pinfo_subpnode_oid,
            :old.pinfo_fnm_globalid,
            :old.pinfo_bus_id,
            :old.pinfo_cnode_id,
            :old.pinfo_order_num,
            :old.pinfo_min_branch,
            :old.pinfo_max_branch,
            :old.pinfo_treelevel,
            :old.pinfo_qa_flag,
            :old.exclude,
            :old.createuser,
            :old.datecreated,
            'DELETE',
            sysdate
        );      
    end if;
    
    
    
end;
/
ALTER TRIGGER "EDGIS"."GIS_PNODE_CIRCUIT_INFO_HISTORY" ENABLE;

 CREATE TABLE GIS_PNODEINFO_TEMP
   (	        TREELEVEL NUMBER(38,0), 
              ORDER_NUM NUMBER(38,0), 
              MIN_BRANCH NUMBER(38,0), 
              MAX_BRANCH NUMBER(38,0), 
              TO_FEATURE_GLOBALID CHAR(38 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS;

 CREATE TABLE GIS_PNODEINFO_TEMP2
   (	  CNODE_ID NVARCHAR2(254), 
        BUS_ID NVARCHAR2(254), 
        LCA_ID NVARCHAR2(50), 
        LAP_ID NVARCHAR2(254), 
        FNM_GLOBALID CHAR(38 BYTE) NOT NULL ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS;

  
CREATE TABLE GIS_PNODEINFO_HISTORY
   (
    LINE_FC_NAME VARCHAR2(6 BYTE), 
    LINE_GUID CHAR(38 BYTE), 
    LINE_OID NUMBER, 
    SUBPNODE_OID NUMBER, 
    CIRCUITID NVARCHAR2(9), 
    PNODE_CNODEID NVARCHAR2(20), 
    FNM_CNODEID NVARCHAR2(254), 
    BUS_ID NVARCHAR2(254), 
    FNM_GLOBALID CHAR(38 BYTE), 
    PNODE_FNMGUID CHAR(38 BYTE), 
    PNODE_CREATIONUSER NVARCHAR2(20), 
    PNODE_DATECREATED DATE, 
    PNODE_LASTUSER NVARCHAR2(20), 
    PNODE_DATEMODIFIED DATE, 
    LCA_ID NVARCHAR2(50), 
    TREELEVEL NUMBER, 
    MIN_BRANCH NUMBER, 
    MAX_BRANCH NUMBER, 
    ORDER_NUM NUMBER, 
    LAP_ID NVARCHAR2(254), 
    QA_FLAG VARCHAR2(500),
    ACTIONDATE DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS;

  COMMENT ON TABLE EDGIS.GIS_PNODEINFO_HISTORY  IS 'This table contains a list of pnodes that have unusual data for some attributes.';

  CREATE INDEX EDGIS.GIS_PNODEINFO_H_LINE_GUID ON EDGIS.GIS_PNODEINFO_HISTORY (LINE_GUID) 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS ;
 
  CREATE INDEX EDGIS.GIS_PNODEINFO_H_FNM_PNODE_GUID ON EDGIS.GIS_PNODEINFO_HISTORY (PNODE_FNMGUID) 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS ;

  CREATE INDEX EDGIS.GIS_PNODEINFO_H_CIRCUITID ON EDGIS.GIS_PNODEINFO_HISTORY (CIRCUITID) 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE EDGIS ;

 ALTER TABLE edgis.gis_pnodeinfo DROP COLUMN QA_FLAG;
 ALTER TABLE edgis.gis_pnodeinfo ADD QA_FLAG varchar(500);


 COMMENT ON TABLE "EDGIS"."FNM"  IS 'This data contains a subset of the rows in FNM_COMPLETE.  The columns have been extended to include LCA_ID (from table GIS_SUBLAP_LCA), Object_Id, and Global_Id.  This table contains data for the most recent pnode bus combinations that are currently active and for the most recent version that was active.   This table does not include future pnode bus combinations. This table is used to populate the dropdown list box that the mappers use to place a pnode-bus in the mapping application.';
 COMMENT ON TABLE "EDGIS"."FNM_COMPLETE"  IS 'This table contains data that is pulled from GIS_FNM_LAP_TEMP.  The primary data columns are cnode_id, bus_id, and lap_id.  This table does not include the LCA_ID which is not part of the FNM. This table contains all past, current, and all know future changes to the primary data columns and their relationships.';
 COMMENT ON TABLE "EDGIS"."FNM_HISTORY"  IS 'This is a trigger driven table that tracks changes to FNM_COMPLETE.';
 
 COMMENT ON TABLE "EDGIS"."GIS_FNM_LAP_TEMP"  IS 'This is a staging table for FNM_COMPLETE. This table is a temporary table that is used to stored data that is imported from PGE’s Market Data Repository (MDR).  The source table is ZE_DATA.CAISO_FNM_PNM_LAP.  The source data is transformed prior to inserting.  This table is truncated after the job has completed.';
 
 COMMENT ON TABLE "EDGIS"."GIS_PNODEINFO"  IS 'This table contains a list of pnodes that have been placed by a mapper and that have been associated with an intersecting bus, an overhead conductor, or an underground conductor.';
 COMMENT ON TABLE "EDGIS"."GIS_SERVICEPOINTS_INFO"  IS 'This table maps a customer’s Service Point to its Feeder.  This table contains only current data.  There is no history table.  The data pulled from ED DataMart.  It is in SubGIS because the DBLink from SubGIS caused the queries to run too slow.  This table is refreshed daily.';
 COMMENT ON TABLE "EDGIS"."GIS_SPP_FEEDER_SUBLAP"  IS 'This table is updated daily.  It contains the current data that is in a format required by MDSS.  MDSS imports this data daily into its SPP_FEEDER_SUBLAP table.  This table will be dropped after the downstream data consumers have transitioned from DataMart to GeoMart.';
 COMMENT ON TABLE "EDGIS"."GIS_SPP_ZONE"  IS 'This table is updated daily.  It contains the current data that is in a format required by MDSS.  MDSS imports this data daily into its SPP_ZONE table.  This table will be dropped after the downstream data consumers have transitioned from DataMart to GeoMart.';
 COMMENT ON TABLE "EDGIS"."GIS_SUBLAP_LCA"  IS 'This table is a temporary table that is used to stored data that is imported from PGE’s Market Data Repository (MDR).  The source table is ZE_DATA.CAISO_SUB_LAP_LCL_CAP_A.  The source data is not transformed prior to inserting.  The primary data columns are lap_id and LCA_NAME (aka LCA_ID). This table contains all past, current, and all know future changes to the primary data columns and their relationships.';
 COMMENT ON TABLE "EDGIS"."MDSS_SPP_FEEDER_SUBLAP"  IS 'This table stores data imported from MDSS.  The source table is SPP_FEEDER_SUBLAP.  It is refreshed daily.  This table will be dropped after the downstream data consumers have transitioned from DataMart to GeoMart.';
 COMMENT ON TABLE "EDGIS"."MDSS_SPP_ZONE"  IS 'This table stores data imported from MDSS.  The source table is SPP_ZONE.  It is refreshed daily.  This table will be dropped after the downstream data consumers have transitioned from DataMart to GeoMart.';

 GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SDE_VIEWER;

 GRANT SELECT ON EDGIS.GIS_CIRCUIT_CIRCUIT TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_CIRCUIT_CIRCUIT_TEMP TO SDE_VIEWER;

 GRANT SELECT ON EDGIS.GIS_CIRCUIT_FNM_LCA TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_CIRCUIT_FNM_LCA_HISTORY TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_CIRCUIT_FNM_LCA_TEMP TO SDE_VIEWER;

 GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO_HISTORY TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_PNODE_CIRCUIT_INFO_TEMP TO SDE_VIEWER;
 
 GRANT SELECT ON EDGIS.GIS_PNODEINFO_TEMP TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_PNODEINFO_TEMP2 TO SDE_VIEWER;
 GRANT SELECT ON EDGIS.GIS_PNODEINFO_HISTORY TO SDE_VIEWER;

 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO GIS_SUB_MDSS_RW;

 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT TO GIS_SUB_MDSS_RW; 
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT_TEMP TO GIS_SUB_MDSS_RW;

 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA_HISTORY TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA_TEMP TO GIS_SUB_MDSS_RW;

 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO_HISTORY TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO_TEMP TO GIS_SUB_MDSS_RW;

 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODEINFO_TEMP TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODEINFO_TEMP2 TO GIS_SUB_MDSS_RW;
 GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.GIS_PNODEINFO_HISTORY TO GIS_SUB_MDSS_RW;

 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SDE_EDITOR;

 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT TO SDE_EDITOR; 
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT_HISTORY TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_CIRCUIT_TEMP TO SDE_EDITOR;

 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA_HISTORY TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_CIRCUIT_FNM_LCA_TEMP TO SDE_EDITOR;

 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO_HISTORY TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODE_CIRCUIT_INFO_TEMP TO SDE_EDITOR;

 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODEINFO_TEMP TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODEINFO_TEMP2 TO SDE_EDITOR;
 GRANT DELETE,INSERT,SELECT,UPDATE ON EDGIS.GIS_PNODEINFO_HISTORY TO SDE_EDITOR;

 spool off
