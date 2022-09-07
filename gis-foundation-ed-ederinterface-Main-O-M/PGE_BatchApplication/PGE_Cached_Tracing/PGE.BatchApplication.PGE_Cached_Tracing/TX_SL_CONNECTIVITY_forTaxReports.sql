create table EDGIS.PGE_TX_SL_CONNECTIVITY (FEEDERID VARCHAR2(32), TRANSFORMEROID number(38), SERVICELOCATIONOID number(38), TRANSFORMER VARCHAR2(38), SERVICELOCATION VARCHAR2(38));

create or replace 
PROCEDURE TX_SL_CONNECTIVITY AS
	CURSOR ELECDISTFEEDERS is select feederid from edgis.PGE_ELECDISTNETWORK_TRACE
	group by feederid order by feederid;
	sqlstmt VARCHAR2(1024);
	XFMR_ID NUMBER;
	SL_ID NUMBER;
	FEEDER_TO_TEST VARCHAR2(32);
	SLNUM VARCHAR2(38);
	XFMRNUM VARCHAR2(38);
	rowcnt number;
BEGIN
	--Drop current table if it exists
	sqlstmt := 'select count(*) from EDGIS.PGE_TX_SL_CONNECTIVITY';
	execute immediate sqlstmt into rowcnt;
	if rowcnt>0 then
		sqlstmt:= 'truncate table EDGIS.PGE_TX_SL_CONNECTIVITY';
		execute immediate sqlstmt;
	END IF;
	--Get transformer and service location IDs
	sqlstmt := 'select OBJECTID from sde.gdb_items where PHYSICALNAME=''EDGIS.TRANSFORMER'' ';
	execute immediate sqlstmt into XFMR_ID ;
	sqlstmt := 'select OBJECTID from sde.gdb_items where PHYSICALNAME=''EDGIS.SERVICELOCATION'' ';
	execute immediate sqlstmt into SL_ID ;
	--Execute the TX to SL mapping process
	FOR FEEDERINFO in ELECDISTFEEDERS LOOP
		sqlstmt := 'INSERT INTO EDGIS.PGE_TX_SL_CONNECTIVITY(FEEDERID,TRANSFORMER,SERVICELOCATION) select '''||FEEDERINFO.feederid||''',r7.TO_FEATURE_GLOBALID transformer, r8.service from ( select * from edgis.PGE_ELECDISTNETWORK_TRACE r2 where r2.to_feature_fcid='||XFMR_ID||' and r2.feederid='''||FEEDERINFO.feederid||''' ) r7 inner join ( select  r6.service,min(r6.ORDER_NUM) order_num from ( select r5.FEEDERID, r5.ORDER_NUM, connect_by_root r5.TO_FEATURE_GLOBALID service, r5.TO_FEATURE_GLOBALID transformer from ( select * from edgis.PGE_ELECDISTNETWORK_TRACE r1 where feederid='''||FEEDERINFO.feederid||''' ) r5 start with r5.to_feature_fcid='||SL_ID||' and r5.feederid='''||FEEDERINFO.feederid||''' connect by nocycle prior FROM_FEATURE_EID=TO_FEATURE_EID and prior feederid=feederid and prior order_num<order_num and prior to_feature_type <> to_feature_type and prior to_feature_fcid<>'||XFMR_ID||') r6 where r6.transformer in (select r2.TO_FEATURE_GLOBALID from edgis.PGE_ELECDISTNETWORK_TRACE r2 where to_feature_fcid='||XFMR_ID||') group by r6.service ) r8 on r7.order_num=r8.order_num ';
		dbms_output.put_line('***************************************************************');
		dbms_output.put_line(sqlstmt);
		dbms_output.put_line('***************************************************************');
		execute immediate sqlstmt ;
		commit;
	END LOOP;
	--Update the global IDs for all values inserted into our table
	update EDGIS.PGE_TX_SL_CONNECTIVITY set SERVICELOCATIONOID =(select objectid from edgis.servicelocation where GLOBALID=SERVICELOCATION);
	commit;
	update EDGIS.PGE_TX_SL_CONNECTIVITY set TRANSFORMEROID =(select objectid from edgis.transformer where GLOBALID=TRANSFORMER);
	commit;
	---Create required indexes
	execute immediate 'create index xfmr_sl_connect_idx on EDGIS.PGE_TX_SL_CONNECTIVITY(TRANSFORMEROID,SERVICELOCATIONOID)';
	execute immediate 'create index xfmr_sl_slonly_idx on EDGIS.PGE_TX_SL_CONNECTIVITY(SERVICELOCATIONOID)';
	execute immediate 'create index xfmr_sl_xfmronly_idx on EDGIS.PGE_TX_SL_CONNECTIVITY(TRANSFORMEROID)';
END;
/

grant select on EDGIS.PGE_TX_SL_CONNECTIVITY to sde_viewer;
grant select on EDGIS.PGE_TX_SL_CONNECTIVITY to sde_editor;
grant select on EDGIS.PGE_TX_SL_CONNECTIVITY to gisinterface;
grant select on EDGIS.PGE_TX_SL_CONNECTIVITY to gis_interface;
grant select on EDGIS.PGE_TX_SL_CONNECTIVITY to edgisbo;
grant execute on EDGIS.TX_SL_CONNECTIVITY to gisinterface;
grant execute on EDGIS.TX_SL_CONNECTIVITY to gis_interface;

