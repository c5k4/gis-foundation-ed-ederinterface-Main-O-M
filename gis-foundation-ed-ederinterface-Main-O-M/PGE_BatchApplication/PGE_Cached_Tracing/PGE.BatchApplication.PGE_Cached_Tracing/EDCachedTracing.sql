drop table PGE_ElecDistNetwork_Trace;

Create table PGE_ElecDistNetwork_Trace
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
TO_FEATURE_SCHEM_FCID NUMBER(38),
TO_FEATURE_FEEDERINFO NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

drop table PGE_Trace_Temp;

Create table PGE_Trace_Temp
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
TO_FEATURE_SCHEM_FCID NUMBER(38),
TO_FEATURE_FEEDERINFO NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

drop table PGE_FeederFedNetwork_Trace;

Create table PGE_FeederFedNetwork_Trace
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
TO_FEATURE_SCHEM_FCID NUMBER(38),
TO_FEATURE_FEEDERINFO NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

drop table PGE_UndergroundNetwork_Trace;

Create table PGE_UndergroundNetwork_Trace
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
SUBTYPECD NUMBER(38),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

drop table PGE_UndergroundNetwork_Temp;

Create table PGE_UndergroundNetwork_Temp
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

drop table PGE_CachedTrace_ToProcess;

Create table PGE_CachedTrace_ToProcess
(FEEDERID NVARCHAR2(9),
CIRCUITSTATUS NUMBER(1),
CIRCUITTYPE NVARCHAR2(1),
ERROR VARCHAR2(4000));

drop table PGE.BatchApplication.PGE_SSD_Initialization;

Create table PGE.BatchApplication.PGE_SSD_Initialization
(FEEDERID NVARCHAR2(9),
CIRCUITSTATUS NUMBER(1),
CIRCUITTYPE NVARCHAR2(1),
ERROR VARCHAR2(4000));

Grant all on PGE.BatchApplication.PGE_SSD_Initialization to gisinterface;

drop table EDGIS.PGE_FEEDERFEDNETWORK_MAP;
CREATE TABLE "EDGIS"."PGE_FEEDERFEDNETWORK_MAP" 
   (           "FROM_CIRCUITID" NVARCHAR2(9), 
                "TO_CIRCUITID" NVARCHAR2(9)
   ) ;

Grant all on EDGIS.PGE_FEEDERFEDNETWORK_MAP to gisinterface;

create or replace 
PROCEDURE TruncateNetworkMapTable AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt:= 'truncate table EDGIS.PGE_FEEDERFEDNETWORK_MAP';
  execute immediate sqlstmt;
  COMMIT;
END;
/
GRANT EXECUTE ON TruncateNetworkMapTable TO gisinterface;

create or replace 
PROCEDURE TruncateSSDInitTable AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt:= 'truncate table EDGIS.PGE.BatchApplication.PGE_SSD_Initialization';
  execute immediate sqlstmt;
  COMMIT;
END;
/

GRANT EXECUTE ON TruncateSSDInitTable TO gisinterface;

create or replace 
PROCEDURE Drop_Cond_Trace_Indices AS
BEGIN
	BEGIN
	execute immediate 'drop index cond_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index cond_trace_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_Cond_Trace_Indices ;
/

create or replace 
PROCEDURE Drop_Elec_Trace_Indices AS
BEGIN
	BEGIN
	execute immediate 'drop index elec_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index electraceidx_globIDSchemFCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_trace_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_net_idx_fcid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_trace_idx_type_and_eid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_net_idx_fcid_feattype_eid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index trace_info_idx_glids';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_Elec_Trace_Indices ;
/

create or replace 
PROCEDURE Drop_FeederFed_Trace_Indices AS
BEGIN
	BEGIN
	execute immediate 'drop index feedfed_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feedfedidx_globIDSchemFCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feederfed_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feederfed_idx_Upstream2';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index temp_trace_idx_FCID_OID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_FeederFed_Trace_Indices ;
/

create or replace 
PROCEDURE PopulateFeederFedFeeder AS
BEGIN
	execute immediate 'Truncate table edgis.PGE_FeederFedNetwork_Trace'; 
	insert into edgis.PGE_FeederFedNetwork_Trace (select * from edgis.pge_elecdistnetwork_trace);
	insert into edgis.PGE_FeederFedNetwork_Trace (FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL) (select FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL from edgis.PGE_SubGeomNetwork_Trace);
END PopulateFeederFedFeeder ;
/

create or replace 
PROCEDURE Create_Cond_Trace_Indices AS
BEGIN
	execute immediate 'create index cond_trace_idx_globID_FCID on edgis.PGE_UndergroundNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index cond_trace_idx_Upstream on edgis.PGE_UndergroundNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_UNDERGROUNDNETWORK_TRACE');
END Create_Cond_Trace_Indices ;
/

create or replace 
PROCEDURE Create_Elec_Trace_Indices AS
BEGIN
	execute immediate 'create index elec_trace_idx_globID_FCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index electraceidx_globIDSchemFCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index elec_trace_idx_Upstream on edgis.pge_elecdistnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_ELECDISTNETWORK_TRACE');
END Create_Elec_Trace_Indices ;
/

create or replace 
PROCEDURE Create_Trace_Temp_Indices AS
BEGIN
	execute immediate 'create index temp_trace_idx_FCID_OID on edgis.PGE_TRACE_TEMP(TO_FEATURE_FCID, TO_FEATURE_OID)';
	dbms_stats.gather_table_stats('EDGIS','PGE_TRACE_TEMP');
END Create_Trace_Temp_Indices ;
/

create or replace 
PROCEDURE Create_FeederFed_Trace_Indices AS
BEGIN
	execute immediate 'create index feedfed_trace_idx_globID_FCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index feedfedidx_globIDSchemFCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index feederfed_idx_Upstream on edgis.PGE_FeederFedNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	execute immediate 'create index feederfed_idx_Upstream2 on edgis.PGE_FeederFedNetwork_Trace(FEEDERFEDBY,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_FeederFedNetwork_Trace');
END Create_FeederFed_Trace_Indices ;
/

create or replace 
PROCEDURE PopUndergroundNetworkTrace AS
--- get a list of feature classes used in the network for lines.
sqlstmt varchar2(4000);
BEGIN
	DBMS_OUTPUT.put_line (SYSTIMESTAMP);
	sqlstmt := 'TRUNCATE TABLE PGE_UndergroundNetwork_Trace';
	DBMS_OUTPUT.put_line (sqlstmt);
	execute immediate sqlstmt;
	sqlstmt := 'INSERT INTO PGE_UndergroundNetwork_Trace SELECT TRACERESULTS.FEEDERID,CONDUIT.GLOBALID,1021,TRACERESULTS.TO_FEATURE_TYPE,TRACERESULTS.ORDER_NUM,TRACERESULTS.MIN_BRANCH,TRACERESULTS.MAX_BRANCH,TRACERESULTS.TREELEVEL FROM (select RELTABLE.ULSOBJECTID,TRACE.FEEDERID,TRACE.TO_FEATURE_TYPE,TRACE.ORDER_NUM,TRACE.MIN_BRANCH,TRACE.MAX_BRANCH,TRACE.TREELEVEL from edgis.conduitsystem_priug RELTABLE join edgis.pge_elecdistnetwork_trace TRACE on TRACE.to_feature_fcid = 1021 and TRACE.to_feature_oid = RELTABLE.UGOBJECTID) TRACERESULTS JOIN EDGIS.CONDUITSYSTEM CONDUIT ON CONDUIT.OBJECTID = TRACERESULTS.ULSOBJECTID';
	DBMS_OUTPUT.put_line (sqlstmt);
	execute immediate sqlstmt;
	sqlstmt := 'INSERT INTO PGE_UndergroundNetwork_Trace SELECT TRACERESULTS.FEEDERID,CONDUIT.GLOBALID,1022,TRACERESULTS.TO_FEATURE_TYPE,TRACERESULTS.ORDER_NUM,TRACERESULTS.MIN_BRANCH,TRACERESULTS.MAX_BRANCH,TRACERESULTS.TREELEVEL FROM (select RELTABLE.ULSOBJECTID,TRACE.FEEDERID,TRACE.TO_FEATURE_TYPE,TRACE.ORDER_NUM,TRACE.MIN_BRANCH,TRACE.MAX_BRANCH,TRACE.TREELEVEL from edgis.conduitsystem_priug RELTABLE join edgis.pge_elecdistnetwork_trace TRACE on TRACE.to_feature_fcid = 1022 and TRACE.to_feature_oid = RELTABLE.UGOBJECTID) TRACERESULTS JOIN EDGIS.CONDUITSYSTEM CONDUIT ON CONDUIT.OBJECTID = TRACERESULTS.ULSOBJECTID';
	DBMS_OUTPUT.put_line (sqlstmt);
	execute immediate sqlstmt;
	commit;
END;
/

create or replace 
PROCEDURE UpdateElecGlobalIDs AS
--- get a list of feature classes used in the network for lines.
cursor net_fcids is 
select OBJECTID,PHYSICALNAME from sde.gdb_items where OBJECTID in (select distinct(to_feature_fcid) from EDGIS.PGE_Trace_Temp) AND PHYSICALNAME in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='PHASEDESIGNATION') ORDER BY PHYSICALNAME;
cursor net_fcids_noPhaseDesignation is 
select OBJECTID,PHYSICALNAME from sde.gdb_items where OBJECTID in (select distinct(to_feature_fcid) from EDGIS.PGE_Trace_Temp) AND PHYSICALNAME not in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='PHASEDESIGNATION') ORDER BY PHYSICALNAME;
cursor net_fcids_underground is 
select OBJECTID,PHYSICALNAME from sde.gdb_items where OBJECTID in (select distinct(to_feature_fcid) from EDGIS.PGE_UNDERGROUNDNETWORK_TEMP) ORDER BY PHYSICALNAME;
sqlstmt varchar2(4000);
BEGIN
	FOR CLASS_NAME in net_fcids LOOP
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		sqlstmt := 'INSERT INTO PGE_ElecDistNetwork_Trace (SELECT TRACE.FEEDERFEDBY, TRACE.FEEDERID, TRACE.FROM_FEATURE_EID, TRACE.TO_FEATURE_EID, TRACE.TO_FEATURE_OID, CLASS.GLOBALID, TRACE.TO_FEATURE_FCID, TRACE.TO_FEATURE_SCHEM_FCID, CLASS.PHASEDESIGNATION, TRACE.TO_FEATURE_TYPE, TRACE.ORDER_NUM, TRACE.MIN_BRANCH, TRACE.MAX_BRANCH, TRACE.TREELEVEL FROM EDGIS.PGE_Trace_Temp TRACE INNER JOIN '||CLASS_NAME.PHYSICALNAME||' CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID))';
		--sqlstmt := 'MERGE INTO EDGIS.PGE_ELECDISTNETWORK_TRACE TRACE USING ( SELECT OBJECTID, GLOBALID, PHASEDESIGNATION FROM '||CLASS_NAME.PHYSICALNAME||') CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID) WHEN MATCHED THEN UPDATE SET TRACE.TO_FEATURE_GLOBALID = CLASS.GLOBALID, TRACE.TO_FEATURE_FEEDERINFO = CLASS.PHASEDESIGNATION';
		execute immediate sqlstmt;
		--DBMS_OUTPUT.put_line (sqlstmt);
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		dbms_output.put_line('Processed: '||CLASS_NAME.PHYSICALNAME);
		commit;
	END LOOP; 
	FOR CLASS_NAME in net_fcids_noPhaseDesignation LOOP
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		sqlstmt := 'INSERT INTO PGE_ElecDistNetwork_Trace (SELECT TRACE.FEEDERFEDBY, TRACE.FEEDERID, TRACE.FROM_FEATURE_EID, TRACE.TO_FEATURE_EID, TRACE.TO_FEATURE_OID, CLASS.GLOBALID, TRACE.TO_FEATURE_FCID, TRACE.TO_FEATURE_SCHEM_FCID, 7, TRACE.TO_FEATURE_TYPE, TRACE.ORDER_NUM, TRACE.MIN_BRANCH, TRACE.MAX_BRANCH, TRACE.TREELEVEL FROM EDGIS.PGE_Trace_Temp TRACE INNER JOIN '||CLASS_NAME.PHYSICALNAME||' CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID))';
		--sqlstmt := 'MERGE INTO EDGIS.PGE_ELECDISTNETWORK_TRACE TRACE USING ( SELECT OBJECTID, GLOBALID FROM '||CLASS_NAME.PHYSICALNAME||') CLASS ON (TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||' AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID) WHEN MATCHED THEN UPDATE SET TRACE.TO_FEATURE_GLOBALID = CLASS.GLOBALID, TRACE.TO_FEATURE_FEEDERINFO = 7';
		execute immediate sqlstmt;
		--DBMS_OUTPUT.put_line (sqlstmt);
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		dbms_output.put_line('Processed: '||CLASS_NAME.PHYSICALNAME);
		commit;
	END LOOP; 
	FOR CLASS_NAME in net_fcids_underground LOOP
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		sqlstmt := 'INSERT INTO PGE_UndergroundNetwork_Trace (SELECT TRACE.FEEDERFEDBY, TRACE.FEEDERID, -1, TRACE.FROM_FEATURE_EID, TRACE.TO_FEATURE_EID, TRACE.TO_FEATURE_OID, CLASS.GLOBALID, TRACE.TO_FEATURE_FCID, TRACE.TO_FEATURE_TYPE, TRACE.ORDER_NUM, TRACE.MIN_BRANCH, TRACE.MAX_BRANCH, TRACE.TREELEVEL FROM EDGIS.PGE_UndergroundNetwork_Temp TRACE INNER JOIN '||CLASS_NAME.PHYSICALNAME||' CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID))';
		execute immediate sqlstmt;
		DBMS_OUTPUT.put_line (sqlstmt);
		sqlstmt := 'MERGE INTO EDGIS.PGE_UNDERGROUNDNETWORK_TRACE TRACE USING (SELECT GLOBALID,SUBTYPECD FROM EDGIS.SUBSURFACESTRUCTURE) S ON (TRACE.TO_FEATURE_GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET TRACE.SUBTYPECD = S.SUBTYPECD';
		execute immediate sqlstmt;
		DBMS_OUTPUT.put_line (sqlstmt);
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		dbms_output.put_line('Processed: '||CLASS_NAME.PHYSICALNAME);
		commit;
	END LOOP;
	commit;
END;
/

create or replace 
PROCEDURE TruncateTracingTables AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt := 'select count(*) from cat where table_name=''PGE_ELECDISTNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_ELECDISTNETWORK_TRACE';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_TRACE_TEMP'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_TRACE_TEMP';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_UNDERGROUNDNETWORK_TEMP'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_UndergroundNetwork_Temp';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_UNDERGROUNDNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_UndergroundNetwork_Trace';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_FEEDERFEDNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_FEEDERFEDNETWORK_TRACE';
                execute immediate sqlstmt;
  END IF;

  sqlstmt := 'select count(*) from cat where table_name=''PGE_CACHEDTRACE_TOPROCESS'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_CACHEDTRACE_TOPROCESS';
                execute immediate sqlstmt;
  END IF;
  COMMIT;
END;
/

Grant execute on TruncateTracingTables to gisinterface;
Grant execute on UpdateElecGlobalIDs to gisinterface;
Grant execute on UpdateSubGlobalIDs to gisinterface;
Grant execute on UpdateSubFCIDs to gisinterface;
Grant execute on Create_Elec_Trace_Indices to gisinterface;
Grant execute on Drop_Elec_Trace_Indices to gisinterface;
Grant execute on Create_Sub_Trace_Indices to gisinterface;
Grant execute on Drop_Sub_Trace_Indices to gisinterface;
Grant execute on Drop_FeederFed_Trace_Indices to gisinterface;
Grant execute on Create_FeederFed_Trace_Indices to gisinterface;
Grant execute on PopulateFeederFedFeeder to gisinterface;

Grant all on PGE_CachedTrace_ToProcess to gisinterface;
Grant all on PGE_UndergroundNetwork_Trace to gisinterface;
Grant all on PGE_UndergroundNetwork_Temp to gisinterface;
Grant all on PGE_UndergroundNetwork_Trace to webr;
Grant all on PGE_UndergroundNetwork_Trace to edgisbo;
Grant all on PGE_ElecDistNetwork_Trace to webr;
Grant all on PGE_SubGeomNetwork_Trace to webr;
Grant all on PGE_ElecDistNetwork_Trace to gisinterface;
Grant all on PGE_SubGeomNetwork_Trace to gisinterface;
Grant all on PGE_FeederFedNetwork_Trace to webr;
Grant all on PGE_FeederFedNetwork_Trace to gisinterface;
Grant select on PGE_FeederFedNetwork_Trace to edgisbo;
Grant select on PGE_ElecDistNetwork_Trace to edgisbo;
Grant select on PGE_SubGeomNetwork_Trace to edgisbo;
Grant select on PGE_FeederFedNetwork_Trace to datamart;
Grant select on PGE_ElecDistNetwork_Trace to datamart;
Grant select on PGE_SubGeomNetwork_Trace to datamart;
grant select on sde.gdb_items to edgisbo;