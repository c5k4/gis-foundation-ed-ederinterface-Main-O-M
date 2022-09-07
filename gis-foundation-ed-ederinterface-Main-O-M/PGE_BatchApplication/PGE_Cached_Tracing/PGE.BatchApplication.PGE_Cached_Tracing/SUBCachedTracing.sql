drop table PGE_SubGeomNetwork_Trace;

Create table PGE_SubGeomNetwork_Trace
(FEEDERFEDBY NVARCHAR2(9),
FEEDERID NVARCHAR2(9),
FROM_FEATURE_EID NUMBER(38),
TO_FEATURE_EID NUMBER(38),
TO_FEATURE_OID NUMBER(38),
TO_FEATURE_GLOBALID CHAR(38),
TO_FEATURE_FCID NUMBER(38),
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
TO_FEATURE_FEEDERINFO NUMBER(38),
TO_FEATURE_TYPE NUMBER(1),
ORDER_NUM NUMBER(38),
MIN_BRANCH NUMBER(38),
MAX_BRANCH NUMBER(38),
TREELEVEL NUMBER(38));

create or replace 
PROCEDURE Drop_Sub_Trace_Indices AS
BEGIN
	BEGIN
	execute immediate 'drop index sub_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index sub_trace_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index sub_net_idx_fcid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index sub_trace_idx_type_and_eid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index sub_net_idx_fcid_feattype_eid';
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
END Drop_Sub_Trace_Indices ;
/

create or replace 
PROCEDURE Create_Trace_Temp_Indices AS
BEGIN
	execute immediate 'create index temp_trace_idx_FCID_OID on edgis.PGE_TRACE_TEMP(TO_FEATURE_FCID, TO_FEATURE_OID)';
	dbms_stats.gather_table_stats('EDGIS','PGE_TRACE_TEMP');
END Create_Trace_Temp_Indices ;
/

create or replace 
PROCEDURE Create_Sub_Trace_Indices AS
BEGIN
	execute immediate 'create index sub_trace_idx_globID_FCID on edgis.pge_subgeomnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index sub_trace_idx_Upstream on edgis.pge_subgeomnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_SubGeomNetwork_Trace');
END Create_Sub_Trace_Indices ;
/

create or replace 
PROCEDURE TruncateTracingTables AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt := 'select count(*) from cat where table_name=''PGE_SUBGEOMNETWORK_TRACE'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_SUBGEOMNETWORK_TRACE';
                execute immediate sqlstmt;
  END IF;
  sqlstmt := 'select count(*) from cat where table_name=''PGE_TRACE_TEMP'' and table_type=''TABLE'' ';
  execute immediate sqlstmt into rowcnt;
  if rowcnt>0 then
     sqlstmt:= 'truncate table EDGIS.PGE_TRACE_TEMP';
                execute immediate sqlstmt;
  END IF;
  COMMIT;
END;
/

create or replace 
PROCEDURE UpdateSUBGlobalIDs AS
--- get a list of feature classes used in the network for lines.
cursor net_fcids is 
select OBJECTID,PHYSICALNAME from sde.gdb_items where OBJECTID in (select distinct(to_feature_fcid) from EDGIS.PGE_Trace_Temp) AND PHYSICALNAME in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='PHASEDESIGNATION') ORDER BY PHYSICALNAME;
cursor net_fcids_noPhaseDesignation is 
select OBJECTID,PHYSICALNAME from sde.gdb_items where OBJECTID in (select distinct(to_feature_fcid) from EDGIS.PGE_Trace_Temp) AND PHYSICALNAME not in (select OWNER||'.'||TABLE_NAME from sde.column_registry where column_name='PHASEDESIGNATION') ORDER BY PHYSICALNAME;
sqlstmt varchar2(4000);
BEGIN
	FOR CLASS_NAME in net_fcids LOOP
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		sqlstmt := 'INSERT INTO PGE_SubGeomNetwork_Trace (SELECT TRACE.FEEDERFEDBY, TRACE.FEEDERID, TRACE.FROM_FEATURE_EID, TRACE.TO_FEATURE_EID, TRACE.TO_FEATURE_OID, CLASS.GLOBALID, TRACE.TO_FEATURE_FCID, CLASS.PHASEDESIGNATION, TRACE.TO_FEATURE_TYPE, TRACE.ORDER_NUM, TRACE.MIN_BRANCH, TRACE.MAX_BRANCH, TRACE.TREELEVEL FROM EDGIS.PGE_Trace_Temp TRACE INNER JOIN '||CLASS_NAME.PHYSICALNAME||' CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID))';
		--sqlstmt := 'MERGE INTO EDGIS.PGE_SUBGEOMNETWORK_TRACE TRACE USING ( SELECT OBJECTID, GLOBALID, PHASEDESIGNATION FROM '||CLASS_NAME.PHYSICALNAME||') CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID) WHEN MATCHED THEN UPDATE SET TRACE.TO_FEATURE_GLOBALID = CLASS.GLOBALID, TRACE.TO_FEATURE_FEEDERINFO = CLASS.PHASEDESIGNATION';
		execute immediate sqlstmt;
		--DBMS_OUTPUT.put_line (sqlstmt);
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		dbms_output.put_line('Processed: '||CLASS_NAME.PHYSICALNAME);
		commit;
	END LOOP; 
	FOR CLASS_NAME in net_fcids_noPhaseDesignation LOOP
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		sqlstmt := 'INSERT INTO PGE_SubGeomNetwork_Trace (SELECT TRACE.FEEDERFEDBY, TRACE.FEEDERID, TRACE.FROM_FEATURE_EID, TRACE.TO_FEATURE_EID, TRACE.TO_FEATURE_OID, CLASS.GLOBALID, TRACE.TO_FEATURE_FCID, 7, TRACE.TO_FEATURE_TYPE, TRACE.ORDER_NUM, TRACE.MIN_BRANCH, TRACE.MAX_BRANCH, TRACE.TREELEVEL FROM EDGIS.PGE_Trace_Temp TRACE INNER JOIN '||CLASS_NAME.PHYSICALNAME||' CLASS ON ((TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||') AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID))';
		--sqlstmt := 'MERGE INTO EDGIS.PGE_SUBGEOMNETWORK_TRACE TRACE USING ( SELECT OBJECTID, GLOBALID FROM '||CLASS_NAME.PHYSICALNAME||') CLASS ON (TRACE.TO_FEATURE_FCID = '||CLASS_NAME.OBJECTID||' AND CLASS.OBJECTID = TRACE.TO_FEATURE_OID) WHEN MATCHED THEN UPDATE SET TRACE.TO_FEATURE_GLOBALID = CLASS.GLOBALID, TRACE.TO_FEATURE_FEEDERINFO = 7';
		execute immediate sqlstmt;
		--DBMS_OUTPUT.put_line (sqlstmt);
		DBMS_OUTPUT.put_line (SYSTIMESTAMP);
		dbms_output.put_line('Processed: '||CLASS_NAME.PHYSICALNAME);
		commit;
	END LOOP; 
	commit;
END;
/

Grant execute on TruncateTracingTables to gisinterface;
Grant execute on UpdateSubGlobalIDs to gisinterface;
Grant execute on UpdateSubFCIDs to gisinterface;
Grant execute on Create_Sub_Trace_Indices to gisinterface;
Grant execute on Drop_Sub_Trace_Indices to gisinterface;

Grant all on PGE_SubGeomNetwork_Trace to webr;
Grant all on PGE_SubGeomNetwork_Trace to gisinterface;
Grant select on PGE_SubGeomNetwork_Trace to edgisbo;
Grant select on PGE_SubGeomNetwork_Trace to datamart;
grant select on sde.gdb_items to edgisbo;