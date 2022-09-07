spool D:\TEMP\PGE_IGP_PHASE_UPDATE_PACKAGES.txt

select current_timestamp from dual;

CREATE OR REPLACE PACKAGE PGE_IGP_UPDATETRANSFORMERUNIT AS

  -- TODO enter package declarations (types, exceptions, methods etc) here 
  PROCEDURE UPDATE_PHASE_TRANS_UNIT(sCircuitIds in varchar2,sUnprocessedTableName in varchar2, sFeederNetworkTrace_Table in varchar2,sBatchNumber in varchar2,iSerialNo in Number);
  PROCEDURE UPDATE_PRICOND_STATUS(sCircuitIds in varchar2);
  PROCEDURE UPDATE_ADDITIONAL_FIELDS(sCircuitIds in varchar2);
  PROCEDURE UPDATE_MULTI_UNIT_TABLE(sCircuitIds in varchar2);
  PROCEDURE UPDATE_BUSBAR_OP_SW_COND(sCircuitIds in varchar2, sFeederNetworkTrace_Table in varchar2);
  PROCEDURE TRUNCATE_TABLES_CIRCUITWISE(sCircuitIds in varchar2,sUnprocessedTableName in varchar2);

END PGE_IGP_UPDATETRANSFORMERUNIT;
/


CREATE OR REPLACE PACKAGE BODY PGE_IGP_UPDATETRANSFORMERUNIT AS

  PROCEDURE UPDATE_PHASE_TRANS_UNIT(sCircuitIds in varchar2,sUnprocessedTableName in varchar2, sFeederNetworkTrace_Table in varchar2,
    sBatchNumber in varchar2,iSerialNo in Number) AS
  BEGIN

    -- TODO: Implementation required for PROCEDURE UPDATE_TRANSFORMER_UNIT.UPDATE_PHASE_TRANS_UNIT
    TRUNCATE_TABLES_CIRCUITWISE(sCircuitIds,sUnprocessedTableName);

    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_SOURCEDATA(TRANSGUID,CONDUCTORGUID,TRANSCIRCUITID,PHASE_PREDICTION,CONDUCTORTYPE,TABLEDATA)
      select GLOBALID,PRICONDUCTORGUID,CIRCUITID,PHASE_PREDICTION,nvl(G1,nvl(g2,g3)) Table_name,''SOURCELINE'' from (
      select T.GLOBALID,T.PRICONDUCTORGUID,T.CIRCUITID,T.PHASE_PREDICTION,
      case when poh.GLOBALID is not null then ''EDGIS.PRIOHCONDUCTOR'' end G1,
      case when puh.GLOBALID is not null then ''EDGIS.PRIUGCONDUCTOR'' end G2,
      case when db.GLOBALID is not null then ''EDGIS.DISTBUSBAR'' end G3
      from  PGE_IGP_INPUT_TRANSFORMER_DATA T
      left join EDGIS.ZZ_MV_PRIOHCONDUCTOR poh on (T.PRICONDUCTORGUID = poh.GLOBALID)
      left join EDGIS.ZZ_MV_PRIUGCONDUCTOR puh on (T.PRICONDUCTORGUID = puh.GLOBALID)
      left join EDGIS.ZZ_MV_DISTBUSBAR db on (T.PRICONDUCTORGUID = db.GLOBALID)
      WHERE T.CIRCUITID IN (' || sCircuitIds || ') AND T.BATCHID IN (''' || sBatchNumber || '''))
      where GLOBALID in (select FEATURE_GUID from ' ||sUnprocessedTableName|| ' where PROCESSED in (''N'')
      AND CIRCUITID IN (' || sCircuitIds || '))';
    commit;

    --Using function IGP_TU_GET_PrimaryVolt in below query
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set
      (TRANSOID,TRANSOPVOLT,TRANSPHASEDESIG,TRANSDIVISION,TRANSNUMBEROFPHASES,TRANSSUBTYPECD)
      =
      (select OBJECTID,IGP_TU_GET_PrimaryVolt(OPERATINGVOLTAGE),PHASEDESIGNATION,DIVISION,NUMBEROFPHASES,SUBTYPECD from
      EDGIS.ZZ_MV_TRANSFORMER where GLOBALID = SL.TRANSGUID)
      WHERE TRANSCIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condfcid = (select MIN(OBJECTID) from sde.gdb_items where UPPER(PHYSICALNAME) = ''EDGIS.PRIOHCONDUCTOR'')
      where conductortype=''EDGIS.PRIOHCONDUCTOR'' and TRANSCIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condfcid = (select MIN(OBJECTID) from sde.gdb_items where UPPER(PHYSICALNAME) = ''EDGIS.PRIUGCONDUCTOR'')
      where conductortype=''EDGIS.PRIUGCONDUCTOR'' and TRANSCIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condfcid = (select MIN(OBJECTID) from sde.gdb_items where UPPER(PHYSICALNAME) = ''EDGIS.DISTBUSBAR'')
    where conductortype=''EDGIS.DISTBUSBAR'' and TRANSCIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

 ----------------------------------------------Script 2 starts ------------------------------------------------
    UPDATE_PRICOND_STATUS(sCircuitIds);
 ----------------------------------------------Script 2 ends ------------------------------------------------
 ----------------------------------------------Script 3 starts ------------------------------------------------

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set COMMENTS=''TR_COND_CASE''
    where SL.CONDUCTORTYPE in (''EDGIS.PRIOHCONDUCTOR'',''EDGIS.PRIUGCONDUCTOR'') and status is null and TRANSCIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    UPDATE_BUSBAR_OP_SW_COND(sCircuitIds, sFeederNetworkTrace_Table);
 ----------------------------------------------Script 3 ends ------------------------------------------------
 ----------------------------------------------Script 4 starts ------------------------------------------------
    UPDATE_ADDITIONAL_FIELDS(sCircuitIds);
 ----------------------------------------------Script 4 ends ------------------------------------------------
 ----------------------------------------------Script multi unit starts ------------------------------------------------
    UPDATE_MULTI_UNIT_TABLE(sCircuitIds);
 ----------------------------------------------Script multi ends ------------------------------------------------

    EXECUTE IMMEDIATE 'insert into ' || sUnprocessedTableName || '(PARENT_GUID,PARENT_CLASS,REC_TYPE,NAME,FEATURE_OID,VALUE,PROCESSED,CIRCUITID,OLD_PHASE,ERROR_MSG,FEATURE_GUID,BATCHID,SERIAL_NO)
    select DISTINCT TRANSGUID,''EDGIS.TRANSFORMER'',''TABLE'',''EDGIS.TRANSFORMERUNIT'',TRANSUNITOBJECTID,TRUNITPHDSGTOUPDATE,''R'',TRANSCIRCUITID,TRANSUNITPHASEDESIG,NULL,TRANSUNITGUID,''' || sBatchNumber || ''',' || iSerialNo || '
    from PGE_IGP_TU_TRANS_SOURCEDATA WHERE TRUNITPHDSGTOUPDATE IS NOT NULL AND (TRANSUNITCOUNT = 1 OR TRANSUNITCOUNT IS NULL) AND TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'insert into ' || sUnprocessedTableName || '(PARENT_GUID,PARENT_CLASS,REC_TYPE,NAME,FEATURE_OID,VALUE,PROCESSED,CIRCUITID,OLD_PHASE,ERROR_MSG,FEATURE_GUID,BATCHID,SERIAL_NO)
    select DISTINCT TRANSGUID,''EDGIS.TRANSFORMER'',''TABLE'',''EDGIS.TRANSFORMERUNIT'',TRANSUNITOBJECTID,TRUNITPHDSGTOUPDATE,''R'',TRANSCIRCUITID,TRANSUNITPHASEDESIG,NULL,TRANSUNITGUID,''' || sBatchNumber || ''',' || iSerialNo || '
    from PGE_IGP_TU_TRANS_SOURCEDATA WHERE TRUNITPHDSGTOUPDATE IS NOT NULL AND TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

    commit;
  END UPDATE_PHASE_TRANS_UNIT;


  PROCEDURE UPDATE_BUSBAR_OP_SW_COND(sCircuitIds in varchar2, sFeederNetworkTrace_Table in varchar2) AS
  BEGIN
      
      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(to_feature_oid,to_feature_globalid,from_feature_eid,CIRCUITID,TABLEDATA)
        select to_feature_oid,to_feature_globalid,from_feature_eid,feederid,''TR'' from ' || sFeederNetworkTrace_Table ||
        ' where to_feature_fcid=1001 and feederid in (' || sCircuitIds || ')'; --'EDGIS.TRANSFORMER'

      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,CIRCUITID,TABLEDATA)
        select to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,feederid,''BUS'' from ' || sFeederNetworkTrace_Table ||
        ' where to_feature_fcid=1019 and feederid in (' || sCircuitIds || ')'; --EDGIS.DISTBUSBAR
        commit;

      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,CIRCUITID,TABLEDATA)
        select to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,feederid,''OPENPOINT'' from ' || sFeederNetworkTrace_Table ||
        ' where to_feature_fcid=1010 and feederid in (' || sCircuitIds || ')'; --EDGIS.OPENPOINT

      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,CIRCUITID,TABLEDATA)
        select to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,feederid,''SWITCH'' from ' || sFeederNetworkTrace_Table ||
        ' where to_feature_fcid=1005 and feederid in (' || sCircuitIds || ')'; --EDGIS.SWITCH

      --EDGIS.PRIUGCONDUCTOR -1021,EDGIS.PRIOHCONDUCTOR - 1023
      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,CIRCUITID,TABLEDATA)
        select to_feature_oid,to_feature_eid,to_feature_fcid,from_feature_eid,feederid,''SWITCH_COND'' from ' || sFeederNetworkTrace_Table ||
        ' where to_feature_fcid in (1021,1023) and feederid in (' || sCircuitIds || ')';

    -------------------------------------------- Trace_Tr_Bus_open point_cond case starts---------------------------------------------------
    
    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(TRANSOID,TRANSGUID,BUSOID,CIRCUITID,TABLEDATA)
      select distinct a.to_feature_oid as TRANSOID, a.to_feature_globalid as TRANSGUID,
      b.to_feature_oid as BUSOID,b.CIRCUITID,''TR_BUS_OP_COND'' from PGE_IGP_TU_TRANS_TMP_DATA a
      join PGE_IGP_TU_TRANS_TMP_DATA b on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
      and a.TABLEDATA = ''TR'' and b.TABLEDATA = ''BUS''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set status=''MULTIPLE_BUS'' where transoid in
    (
    select transoid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''TR_BUS_OP_COND''
    group by transoid having count(*) >1
    ) and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';

    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(BUSOID,OPOID,CIRCUITID,TABLEDATA)
    select a.to_feature_oid as BUSOID, b.to_feature_oid as OPOID, a.CIRCUITID,''BUS_OP'' from PGE_IGP_TU_TRANS_TMP_DATA a
    join PGE_IGP_TU_TRANS_TMP_DATA b on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
    and a.TABLEDATA = ''BUS'' and b.TABLEDATA = ''OPENPOINT''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA BOC set opoid = (select opoid from PGE_IGP_TU_TRANS_TMP_DATA where busoid = BOC.busoid and TABLEDATA = ''BUS_OP'')
      where status is null and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';

    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(OPOID,CONDUCTOROID,CONDUCTORFCID,CIRCUITID,TABLEDATA)
      select a.to_feature_oid as OPOID, b.to_feature_oid as CONDUCTOROID, b.to_feature_fcid as CONDUCTORFCID,b.CIRCUITID,''OP_COND''
      from PGE_IGP_TU_TRANS_TMP_DATA a join PGE_IGP_TU_TRANS_TMP_DATA b
      on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
       and a.TABLEDATA = ''OPENPOINT'' and b.TABLEDATA = ''COND''';

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_TMP_DATA BOC set (CONDUCTOROID,CONDUCTORFCID,STATUS) = (select CONDUCTOROID,CONDUCTORFCID,''OP_CASE''
      from PGE_IGP_TU_TRANS_TMP_DATA where opoid = BOC.opoid and TABLEDATA = ''OP_COND'')
      where status is null and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set conductortype=''EDGIS.PRIOHCONDUCTOR'' where conductorfcid= 1023 and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';--'EDGIS.PRIOHCONDUCTOR'

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set conductortype=''EDGIS.PRIUGCONDUCTOR'' where conductorfcid=1021 and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';--'EDGIS.PRIUGCONDUCTOR'

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_TMP_DATA BOC set BUSPHASEDESIG = (select PHASEDESIGNATION from edgis.zz_mv_DISTBUSBAR where OBJECTID = BOC.busoid) WHERE CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_OP_COND''';


    -------------------------------------------- Trace_Tr_Bus_open point _cond case ends---------------------------------------------------

    -------------------------------------------- Trace_Tr_Bus_switch_cond case starts---------------------------------------------------
    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(TRANSOID,TRANSGUID,BUSOID,CIRCUITID,TABLEDATA)
    select distinct a.to_feature_oid as TRANSOID, a.to_feature_globalid as TRANSGUID,
    b.to_feature_oid as BUSOID,b.CIRCUITID,''TR_BUS_SW_COND'' from PGE_IGP_TU_TRANS_TMP_DATA a
    join PGE_IGP_TU_TRANS_TMP_DATA b on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
    and a.TABLEDATA = ''TR'' and b.TABLEDATA = ''BUS''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set status=''MULTIPLE_BUS'' where transoid in
    (
    select transoid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''TR_BUS_SW_COND''
    group by transoid having count(*) >1
    ) and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';

    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(BUSOID,SWOID,CIRCUITID,TABLEDATA)
    select a.to_feature_oid as BUSOID, b.to_feature_oid as SWOID,a.CIRCUITID,''BUS_SWITCH'' from PGE_IGP_TU_TRANS_TMP_DATA a
    join PGE_IGP_TU_TRANS_TMP_DATA b on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
    and a.TABLEDATA = ''BUS'' and b.TABLEDATA = ''SWITCH''';

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_TMP_DATA BSC SET swoid = (select swoid from PGE_IGP_TU_TRANS_TMP_DATA where busoid = BSC.busoid and TABLEDATA = ''BUS_SWITCH'')
    where status is null and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';

    EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_TMP_DATA(SWOID,CONDUCTOROID,CONDUCTORFCID,CIRCUITID,TABLEDATA)
    select a.to_feature_oid as SWOID, b.to_feature_oid as CONDUCTOROID, b.to_feature_fcid as CONDUCTORFCID,b.CIRCUITID,''SWITCH_COND'' from PGE_IGP_TU_TRANS_TMP_DATA a
    join PGE_IGP_TU_TRANS_TMP_DATA b on a.from_feature_eid =b.to_feature_eid where a.CIRCUITID in (' || sCircuitIds || ')
    and a.TABLEDATA = ''SWITCH'' and b.TABLEDATA = ''COND''';

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_TMP_DATA BSC SET (CONDUCTOROID,CONDUCTORFCID,STATUS)= (select CONDUCTOROID,CONDUCTORFCID,''SW_CASE''
    from PGE_IGP_TU_TRANS_TMP_DATA where SWOID = BSC.SWOID and TABLEDATA = ''SWITCH_COND'')
    where STATUS IS NULL and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set conductortype=''EDGIS.PRIOHCONDUCTOR'' where conductorfcid=1023 and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';--'EDGIS.PRIOHCONDUCTOR'
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set conductortype=''EDGIS.PRIUGCONDUCTOR'' where conductorfcid=1021 and CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';--'EDGIS.PRIUGCONDUCTOR'
    --error rownum
    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_TMP_DATA BSW SET BUSPHASEDESIG = (SELECT PHASEDESIGNATION FROM edgis.zz_mv_DISTBUSBAR
    WHERE busoid = BSW.busoid AND ROWNUM = 1) WHERE CIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''TR_BUS_SW_COND''';

    -------------------------------------------- Trace_Tr_Bus_sw_cond case ends---------------------------------------------------

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_SOURCEDATA SL SET (CONDUCTOROID,CONDFCID,CONDUCTORTYPE,COMMENTS,BUSPHASEDESIG) =
    (SELECT CONDUCTOROID,CONDFCID,CONDUCTORTYPE,concat(NVL(SL.COMMENTS,''''),''_TR_OP_CASE''),BUSPHASEDESIG
    FROM PGE_IGP_TU_TRANS_TMP_DATA WHERE TRANSOID = SL.TRANSOID and status=''OP_CASE'' and TABLEDATA = ''TR_BUS_OP_COND'') WHERE STATUS IS NULL
    AND CONDUCTORTYPE = ''EDGIS.DISTBUSBAR'' AND TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_SOURCEDATA SL SET (CONDUCTOROID,CONDFCID,CONDUCTORTYPE,COMMENTS,BUSPHASEDESIG) =
    (SELECT CONDUCTOROID,CONDFCID,CONDUCTORTYPE,concat(NVL(SL.COMMENTS,''''),''_TR_SW_CASE''),BUSPHASEDESIG
    FROM PGE_IGP_TU_TRANS_TMP_DATA WHERE TRANSOID = SL.TRANSOID and status=''SW_CASE'' and TABLEDATA = ''TR_BUS_SW_COND'') WHERE STATUS IS NULL
    AND CONDUCTORTYPE = ''EDGIS.DISTBUSBAR'' AND TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    commit;

  END UPDATE_BUSBAR_OP_SW_COND;

   PROCEDURE UPDATE_PRICOND_STATUS(sCircuitIds in varchar2) AS
  BEGIN

    --update  IGP_TU_PRIOHCOND_STATUS set STATUS=NULL;
    --Using function IGP_TU_GET_PrimaryVolt in below query
    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(OBJECTID,globalid, phasedesignation, nominalvoltage,circuitid,TABLEDATA)
      select objectid, globalid, phasedesignation,IGP_TU_GET_PrimaryVolt(nominalvoltage),circuitid,''STATUS''
      from edgis.zz_mv_priohconductor where CIRCUITID IN (' || sCircuitIds || ')';

    -- Single phase records without neutral
    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(conductorguid,CIRCUITID,TABLEDATA)
    select distinct a.conductorguid,b.CIRCUITID,''PRIOH_1'' from edgis.zz_mv_priohconductorinfo a
      join edgis.zz_mv_priohconductor b on a.conductorguid= b.globalid where b.CIRCUITID IN (' || sCircuitIds || ')
      group by a.conductorguid,b.CIRCUITID having count(*) = 1';

    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(conductorguid,TABLEDATA)
      select distinct conductorguid,''PRIOH_2'' from edgis.zz_mv_priohconductorinfo where conductorguid in
      (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''PRIOH_1'') and phasedesignation <=7';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA p set CIRCUITID = (select CIRCUITID from PGE_IGP_TU_TRANS_TMP_DATA where conductorguid = p.conductorguid and TABLEDATA = ''PRIOH_1'')
      where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''PRIOH_2''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set STATUS=CONCAT(NVL(STATUS,''''),''_WITHOUT_NEUTRAL'')
      where globalid IN (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''PRIOH_2'') AND CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

    -- Records with neutral
    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(CONDUCTORGUID,CIRCUITID,TABLEDATA)
      select distinct a.conductorguid,b.CIRCUITID,''MULTIPLE'' from edgis.zz_mv_priohconductorinfo a join edgis.zz_mv_priohconductor b
      on a.conductorguid= b.globalid where b.CIRCUITID IN (' || sCircuitIds || ') group by a.conductorguid,b.CIRCUITID having count(*) > 1';

    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(CONDUCTORGUID,TABLEDATA)
      select distinct conductorguid,''MULT_PHLESS7'' from edgis.zz_mv_priohconductorinfo where conductorguid in
      (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''MULTIPLE'') and phasedesignation <=7';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA p set CIRCUITID = (select CIRCUITID from PGE_IGP_TU_TRANS_TMP_DATA where conductorguid = p.conductorguid and TABLEDATA = ''MULTIPLE'')
      where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''MULT_PHLESS7''';

    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(CONDUCTORGUID,TABLEDATA)
      select distinct conductorguid,''MULT_PHMORE7'' from edgis.zz_mv_priohconductorinfo where conductorguid in
      (
      select  conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''MULTIPLE''
      ) and phasedesignation in (8,9,10,11)';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA p set CIRCUITID = (select CIRCUITID from PGE_IGP_TU_TRANS_TMP_DATA where conductorguid = p.conductorguid and TABLEDATA = ''MULTIPLE'')
      where CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''MULT_PHMORE7''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set STATUS = CONCAT(NVL(STATUS,''''),''_WITH_NEUTRAL'') where
      globalid IN
      (select distinct a.conductorguid from
      (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''MULT_PHLESS7'')a
      join
      (select distinct conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''MULT_PHMORE7'')b
      on a.conductorguid=b.conductorguid
      ) and CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

    -- without neutral
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set STATUS=CONCAT(NVL(STATUS,''''),''_WITHOUT_NEUTRAL'') where globalid IN
      (select a.conductorguid from
      (select distinct conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''MULT_PHMORE7'')a
      left join
      (select distinct conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''MULT_PHMORE7'')b
      on a.conductorguid=b.conductorguid
      where b.conductorguid is null
      ) and CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

      --- Exceptions
      
    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(conductorguid,CIRCUITID,TABLEDATA)
      select distinct a.conductorguid,b.CIRCUITID,''PRIOHINFO_EXP_1'' from edgis.zz_mv_priohconductorInfo a join edgis.zz_mv_priohconductor b
      on a.conductorguid= b.globalid where b.CIRCUITID IN (' || sCircuitIds || ') and b.phasedesignation >7';
    
     EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(conductorguid,CIRCUITID,TABLEDATA)
      select p.conductorguid,po.CIRCUITID,''PRIOHINFO_EXP_2'' from edgis.zz_mv_priohconductorInfo p join PGE_IGP_TU_TRANS_TMP_DATA po
      on (po.conductorguid = p.conductorguid) where po.CIRCUITID IN (' || sCircuitIds || ') and p.phasedesignation <=7 and po.TABLEDATA = ''PRIOHINFO_EXP_1''';


    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set STATUS=CONCAT(NVL(STATUS,''''),''_CN_OR_PN_ONLY'') where globalid IN
      (select a.conductorguid from
      (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''PRIOHINFO_EXP_1'')a
      left join
      (select conductorguid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''PRIOHINFO_EXP_2'')b
      on a.conductorguid=b.conductorguid
      where b.conductorguid is null) and CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_TMP_DATA set STATUS=CONCAT(NVL(STATUS,''''),''_CONDINFO_PHASE_NULL'') where globalid IN
      (select conductorguid from edgis.zz_mv_priohconductorinfo where phasedesignation is null) and CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

    
    EXECUTE IMMEDIATE 'insert into PGE_IGP_TU_TRANS_TMP_DATA(GLOBALID,CIRCUITID,TABLEDATA)
      select a.globalid,a.CIRCUITID,''PRIOHINFO_EXP_3'' from  edgis.zz_mv_priohconductor a left join edgis.zz_mv_priohconductorinfo b
      on a.globalid= b.conductorguid where b.conductorguid is null and a.CIRCUITID IN (' || sCircuitIds || ')';

    EXECUTE IMMEDIATE 'update  PGE_IGP_TU_TRANS_TMP_DATA set STATUS=CONCAT(NVL(STATUS,''''),''_CONDINFO_DOESNT_EXIST'') where globalid IN
    (select globalid from PGE_IGP_TU_TRANS_TMP_DATA where TABLEDATA = ''PRIOHINFO_EXP_3'') and CIRCUITID IN (' || sCircuitIds || ') and TABLEDATA = ''STATUS''';

    commit;
  END UPDATE_PRICOND_STATUS;


  PROCEDURE UPDATE_MULTI_UNIT_TABLE(sCircuitIds in varchar2) AS
    BEGIN
      EXECUTE IMMEDIATE 'INSERT INTO PGE_IGP_TU_TRANS_SOURCEDATA(TRANSOID,TRANSGUID,TRANSOPVOLT,TRANSPHASEDESIG,TRANSPHASE,TRANSCIRCUITID,
      TRANSDIVISION,TRANSNUMBEROFPHASES,TRANSSUBTYPECD,TRPHDSGTOUPDATE,PHASE_PREDICTION,TRANSUNITCOUNT,TRANSUNITOBJECTID,TRANSUNITGUID,
      TRANSUNITSUBTYPECD,TRANSUNITTRTYPE,TRANSUNITPHASEDESIG,TRUNITPHDSGTOUPDATE,CONDUCTOROID,CONDFCID,CONDUCTORTYPE,CONDOPVOLT,CONDNOMVOLT,
      CONDPHASEDESIG,CONDPHASE,CONDPHASESTATUS,RULE,STATUS,SOURCELINENOTFOUND,OPENPOINTSWCASE,COMMENTS,BUSPHASEDESIG,TABLEDATA)
        select TRANSOID  , a.transformerguid , b.TRANSOPVOLT , b.TRANSPHASEDESIG , b.TRANSPHASE , b.TRANSCIRCUITID, b.TRANSDIVISION, b.TRANSNUMBEROFPHASES,
          b.TRANSSUBTYPECD, b.TRPHDSGTOUPDATE, b.PHASE_PREDICTION, b.TRANSUNITCOUNT , a.OBJECTID as  TRANSUNITOBJECTID ,a.GLOBALID as  TRANSUNITGUID ,
          a.subtypecd as TRANSUNITSUBTYPECD , a.transformertype as  TRANSUNITTRTYPE ,a.phasedesignation as  TRANSUNITPHASEDESIG , b.TRUNITPHDSGTOUPDATE,
          b.CONDUCTOROID  , b.condfcid, b.CONDUCTORTYPE , b.CONDOPVOLT , b.CONDNOMVOLT, b.CONDPHASEDESIG , b.CONDPHASE , b.CONDPHASESTATUS , b.RULE ,
          b.STATUS  , b.SOURCELINENOTFOUND , b.OPENPOINTSWCASE , b.COMMENTS, b.BUSPHASEDESIG, ''SOURCELINE_MULTI''
        from edgis.zz_mv_transformerunit a
        join PGE_IGP_TU_TRANS_SOURCEDATA b
        on a.transformerguid=b.transguid
        where b.transunitcount > 1 and b.TRANSCIRCUITID in (' || sCircuitIds || ') and b.TABLEDATA = ''SOURCELINE''';

        EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set transphase = IGP_TU_GET_TRANS_PHASE(transunitsubtypecd, transunittrtype)
        where TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

        EXECUTE IMMEDIATE 'UPDATE PGE_IGP_TU_TRANS_SOURCEDATA SET STATUS=''FAIL_1'' where transguid in
        (
        select a.transguid from
        (
        select transguid,transphase,count(*) as Count from PGE_IGP_TU_TRANS_SOURCEDATA where TABLEDATA = ''SOURCELINE_MULTI''
        group by transguid,transphase
        ) a
        group by a.transguid
        having count(*) >1
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

        EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set rule= IGP_TU_GET_Rule_Multi (condphasestatus,transphase, transunitcount)
          where status is null and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

        -- Updating conductor unit phase designations to update

        ----- where transunitcount=2 and phase_prediction =7(ABC)------
         EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=2 and phase_prediction =7 and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig,count(*) from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =7
        and transunitphasedesig in (6,3) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig order by transguid
        )
        group by transguid having count(*)=2
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=2 and phase_prediction =7
       and STATUS IS NULL and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=3  where transunitcount=2 and phase_prediction =7
       and status=''FAIL'' and transunitphasedesig!=3 and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

        EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=6 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =7
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
        ----- where transunitcount=2 and phase_prediction =7(ABC)------
        --update IGP_TU_TRANS_SOURCELINE_MULTI set trphdsgtoupdate=7 where Rule='Rule_7A' and status='FAIL' and transphasedesig!=7;

        ----- where transunitcount=2 and phase_prediction =6(AB)------
       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=2 and phase_prediction =6 and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =6
        and transunitphasedesig in (4,2) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig
        )
        group by transguid having count(*)=2
        )and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=2 and phase_prediction =6 and STATUS IS NULL
       and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=2  where transunitcount=2 and phase_prediction =6 and status=''FAIL''
       and transunitphasedesig!=2and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=4 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =6
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        )and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
        ----- where transunitcount=2 and phase_prediction =6(AB)------
      -- update IGP_TU_TRANS_SOURCELINE_MULTI set  trphdsgtoupdate=6  where transunitcount=2 and phase_prediction =6 and status=''FAIL'' and transphasedesig!=6;


         ----- where transunitcount=2 and phase_prediction =3(BC)------------------
       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=2  and phase_prediction =3 and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2  and phase_prediction =3
        and transunitphasedesig in (2,1) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig
        )
        group by transguid having count(*)=2
        )and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=2 and phase_prediction =3 and STATUS IS NULL
       and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=1  where transunitcount=2 and phase_prediction =3 and status=''FAIL''
       and transunitphasedesig!=1and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=2 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =3
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
       ----- where transunitcount=2 and phase_prediction =3(BC)------------------
       --update IGP_TU_TRANS_SOURCELINE_MULTI set  trphdsgtoupdate=3  where transunitcount=2  and status=''FAIL'' and transphasedesig!=3;

       ----- where transunitcount=2 and phase_prediction =5(AC)------------------
       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=2 and phase_prediction =5 and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =5
        and transunitphasedesig in (4,1) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig
        )
        group by transguid having count(*)=2
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=2 and phase_prediction =5
       and STATUS IS NULL and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=1  where transunitcount=2 and phase_prediction =5 and status=''FAIL''
       and transunitphasedesig!=1 and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=4 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=2 and phase_prediction =5
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
       ----- where transunitcount=2 and phase_prediction =5(AC)------------------
       --update IGP_TU_TRANS_SOURCELINE_MULTI set  trphdsgtoupdate=5  where transunitcount=2 and phase_prediction =5 and status=''FAIL'' and transphasedesig!=5and TRANSCIRCUITID in (' || sCircuitIds || ')';

       ----- where transunitcount=3 and phase_prediction =7(ABC) and Conductor is ABC_WITHOUT_NEUTRAL ------------------
       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B'' and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B''
        and transunitphasedesig in (6,3,5) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig
        )
        group by transguid having count(*)=3
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B''
       and STATUS IS NULL and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=3  where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B''
       and status=''FAIL'' and transunitphasedesig!=3 and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=6 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B''
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=5 where transunitobjectid in
        (
        select max(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_7B''
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
      ----- where transunitcount=3 and phase_prediction =7(ABC) and Conductor is ABC_WITHOUT_NEUTRAL ------------------
      --EXECUTE IMMEDIATE 'update IGP_TU_TRANS_SOURCELINE_MULTI set  trphdsgtoupdate=7  where Rule='''Rule_7B''' and status=''FAIL'' and transphasedesig!=7and TRANSCIRCUITID in (' || sCircuitIds || ')';

       ----- where transunitcount=3 and phase_prediction =7(ABC) and Conductor is ABC_WITH_NEUTRAL ------------------
       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B'' and transguid in
        (
        select transguid from
        (
        select transguid,transunitphasedesig from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''
        and transunitphasedesig in (4,2,1) and TABLEDATA = ''SOURCELINE_MULTI'' group by transguid,transunitphasedesig
        )
        group by transguid having count(*)=3
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''
       and STATUS IS NULL and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=1  where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''
       and status=''FAIL'' and transunitphasedesig!=1 and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=4 where transunitobjectid in
        (
        select min(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';

       EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set  trunitphdsgtoupdate=2 where transunitobjectid in
        (
        select max(transunitobjectid) as transunitobjectid from PGE_IGP_TU_TRANS_SOURCEDATA where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''
        and status=''FAIL'' and TABLEDATA = ''SOURCELINE_MULTI'' group by transoid
        ) and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE_MULTI''';
         ----- where transunitcount=3 and phase_prediction =7(ABC) and Conductor is ABC_WITH_NEUTRAL ------------------
        --EXECUTE IMMEDIATE 'update IGP_TU_TRANS_SOURCELINE_MULTI set  trphdsgtoupdate=7  where transunitcount=3 and phase_prediction =7 and rule=''Rule_11B''  and status=''FAIL'' and transphasedesig!=7and TRANSCIRCUITID in (' || sCircuitIds || ')';

        commit;
  END UPDATE_MULTI_UNIT_TABLE;

  PROCEDURE UPDATE_ADDITIONAL_FIELDS(sCircuitIds in varchar2) AS
  BEGIN
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set (TRANSUNITCOUNT) =
      (select count from (select transformerguid,count(*) as Count from edgis.zz_mv_transformerunit GROUP by transformerguid)
      where transformerguid = SL.TRANSGUID) WHERE SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set (transunitobjectid,TRANSUNITGUID,transunitsubtypecd,transunittrtype,transunitphasedesig) = (
      select OBJECTID,GLOBALID,subtypecd,transformertype,phasedesignation from edgis.zz_mv_transformerunit where transformerguid = SL.TRANSGUID)
      where TRANSUNITCOUNT=1 and SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    --Using function IGP_TU_GET_PrimaryVolt in below query
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set (CONDUCTOROID,condopvolt,condphasedesig,CONDNOMVOLT) =
      (SELECT OBJECTID,IGP_TU_GET_PrimaryVolt(operatingvoltage),phasedesignation,IGP_TU_GET_PrimaryVolt(nominalvoltage)
      FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR where conductortype=''EDGIS.PRIOHCONDUCTOR'' and globalid = SL.CONDUCTORGUID)
      where conductortype=''EDGIS.PRIOHCONDUCTOR'' and SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    --remove nominal voltage as per script
    --Using function IGP_TU_GET_PrimaryVolt in below query
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set (CONDUCTOROID,condopvolt,condphasedesig,CONDNOMVOLT) =
      (SELECT OBJECTID,IGP_TU_GET_PrimaryVolt(operatingvoltage),phasedesignation,IGP_TU_GET_PrimaryVolt(nominalvoltage)
      FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR where conductortype=''EDGIS.PRIUGCONDUCTOR'' and globalid = SL.CONDUCTORGUID)
      where conductortype=''EDGIS.PRIUGCONDUCTOR'' and SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    --error in nominal voltage
    --ACTION ITEM : Need to check use of function IGP_TU_GET_PrimaryVolt in below query
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set (CONDUCTOROID,condopvolt,condphasedesig,CONDNOMVOLT,BUSPHASEDESIG) =
      (SELECT OBJECTID,operatingvoltage,phasedesignation,0,PHASEDESIGNATION
      FROM EDGIS.ZZ_MV_DISTBUSBAR where conductortype=''EDGIS.DISTBUSBAR'' and globalid = SL.CONDUCTORGUID)
      where conductortype=''EDGIS.DISTBUSBAR'' and SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set transphase = IGP_TU_GET_TRANS_PHASE(transunitsubtypecd, transunittrtype)
      where transunitcount = 1 and TRANSCIRCUITID in (' || sCircuitIds || ')';

  --ROWNUM Error
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA SL set condphase = (select status from PGE_IGP_TU_TRANS_TMP_DATA where OBJECTID = SL.conductoroid AND ROWNUM = 1 and TABLEDATA = ''STATUS'')
      where conductortype=''EDGIS.PRIOHCONDUCTOR'' and SL.TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condphase = ''_WITH_NEUTRAL''
      where conductortype=''EDGIS.PRIUGCONDUCTOR'' and transopvolt < condopvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condphase = ''_WITHOUT_NEUTRAL''
      where conductortype=''EDGIS.PRIUGCONDUCTOR'' and transopvolt = condopvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condphase = ''_EXCEPTION_UG''
      where conductortype=''EDGIS.PRIUGCONDUCTOR'' and transopvolt > condopvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    --work pending for comments,BUSPHASEDESIG,status

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condphasestatus= concat(IGP_TU_GET_Cond_Phase(condphasedesig), condphase)
      where comments not in (''_TR_SW_CASE'',''_TR_OP_CASE'') and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --DMS Issue : Change here and update condphasestatus based on Conductor (with neutral , without neutral) and Busbar phasedesignation
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set condphasestatus= concat(IGP_TU_GET_Cond_Phase(BUSPHASEDESIG), condphase)
      where comments in (''_TR_SW_CASE'',''_TR_OP_CASE'') and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    --work pending for comments,BUSPHASEDESIG,status

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set rule= IGP_TU_GET_Rule (condphasestatus,transphase) where transunitcount=1
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set rule= ''Exception'',status=''comments not in TR_COND_CASE,_TR_SW_CASE,_TR_OP_CASE''
      where comments not in (''TR_COND_CASE'',''_TR_SW_CASE'',''_TR_OP_CASE'') and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set rule= ''Exception'', status=''conductortype is null or conductortype is empty''
      where (conductortype is null or conductortype = '''') and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set Rule=''Exception'',status=''comments in _TR_OP_CASE,_TR_SW_CASE''
      where comments in (''_TR_OP_CASE'',''_TR_SW_CASE'') and conductortype !=''EDGIS.PRIUGCONDUCTOR'' and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set Rule=''Rule_11_0'' where Rule=''Rule_11'' and conductortype =''EDGIS.PRIUGCONDUCTOR'' and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set Rule=''Rule_11_1'' where Rule=''Rule_11'' and conductortype =''EDGIS.PRIOHCONDUCTOR'' and transopvolt= condnomvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set Rule=''Rule_11_2'' where Rule=''Rule_11'' and conductortype =''EDGIS.PRIOHCONDUCTOR'' and transopvolt< condnomvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set Rule=''Rule_11_3'',STATUS=''FAIL'',COMMENTS=''Rule_11_3 transopvolt > Sourcelinenomvolt''
    where Rule=''Rule_11'' and conductortype =''EDGIS.PRIOHCONDUCTOR'' and transopvolt> condnomvolt and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set sourcelinenotfound =''YES'' where conductoroid is null and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trunitphdsgtoupdate=phase_prediction where transunitcount=1 and phase_prediction!=transunitphasedesig
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =4 and transunitphasedesig=4 and transunitcount=1
    and rule=''Rule_1'' and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_1'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=4 where Rule=''Rule_1'' and status=''FAIL'' and transphasedesig!=4
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=4 where Rule='Rule_1' and status='FAIL' and transunitphasedesig!=4;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transphasedesig =2 and transunitphasedesig=2 and transunitcount=1 and rule=''Rule_2''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_2'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=2 where Rule=''Rule_2'' and status=''FAIL'' and transphasedesig!=2
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=2 where Rule='Rule_2' and status='FAIL' and transunitphasedesig!=2;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =1 and transunitphasedesig=1 and transunitcount=1 and rule=''Rule_3''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_3'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=1 where Rule=''Rule_3'' and status=''FAIL'' and transphasedesig!=1
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=1 where Rule='Rule_3' and status='FAIL' and transunitphasedesig!=1;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =6 and transunitphasedesig=6 and transunitcount=1 and rule=''Rule_4''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_4'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=6 where Rule=''Rule_4'' and status=''FAIL'' and transphasedesig!=6
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=6 where Rule='Rule_4' and status='FAIL' and transunitphasedesig!=6;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =3 and transunitphasedesig=3 and transunitcount=1 and rule=''Rule_5''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_5'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=3 where Rule=''Rule_5'' and status=''FAIL'' and transphasedesig!=3 and
    TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=3 where Rule='Rule_5' and status='FAIL' and transunitphasedesig!=3;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS'' where transphasedesig =5 and transunitphasedesig=5 and transunitcount=1 and rule=''Rule_6''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_6'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=5 where Rule=''Rule_6'' and status=''FAIL'' and transphasedesig!=5
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=5 where Rule='Rule_6' and status='FAIL' and transunitphasedesig!=5;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =6 and transunitphasedesig=6 and transunitcount=1 and rule=''Rule_7''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=1 and rule=''Rule_7'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=6 where Rule=''Rule_7'' and status=''FAIL'' and transphasedesig!=6
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=6 where Rule='Rule_7' and status='FAIL' and transunitphasedesig!=6;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =4 and transunitphasedesig=4 and transunitcount=1 and rule=''Rule_8''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_8'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=4 where Rule=''Rule_8'' and status=''FAIL'' and transphasedesig!=4
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=4 where Rule='Rule_8' and status='FAIL' and transunitphasedesig!=4;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =2 and transunitphasedesig=2 and transunitcount=1 and rule=''Rule_9''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_9'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=2 where Rule=''Rule_9'' and status=''FAIL'' and transphasedesig!=2
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=2 where Rule='Rule_9' and status='FAIL' and transunitphasedesig!=2;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =4 and transunitphasedesig=4 and transunitcount=1 and rule=''Rule_10''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=1 and rule=''Rule_10'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=4 where Rule=''Rule_10'' and status=''FAIL'' and transphasedesig!=4 and TRANSCIRCUITID in (' || sCircuitIds || ')';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=4 where Rule='Rule_10' and status='FAIL' and transunitphasedesig!=4;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =4 and transunitphasedesig=4 and transunitcount=1 and rule=''Rule_11_0''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =2 and transunitphasedesig=2 and transunitcount=1 and rule=''Rule_11_0''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =1 and transunitphasedesig=1 and transunitcount=1 and rule=''Rule_11_0''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=1 and rule=''Rule_11_0'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=4 where Rule=''Rule_11_0'' and status=''FAIL'' and transphasedesig!=4
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=4 where Rule='Rule_11_0' and status='FAIL' and transunitphasedesig!=4;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =6 and transunitphasedesig=6 and transunitcount=1 and rule=''Rule_11_1''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =3 and transunitphasedesig=3 and transunitcount=1 and rule=''Rule_11_1''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =5 and transunitphasedesig=5 and transunitcount=1 and rule=''Rule_11_1''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_11_1'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=6 where Rule=''Rule_11_1'' and status=''FAIL'' and transphasedesig!=6
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=6 where Rule='Rule_11_1' and status='FAIL' and transunitphasedesig!=6;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =4 and transunitphasedesig=4 and transunitcount=1 and rule=''Rule_11_2''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =2 and transunitphasedesig=2 and transunitcount=1 and rule=''Rule_11_2''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =1 and transunitphasedesig=1 and transunitcount=1 and rule=''Rule_11_2''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'' where transunitcount=1 and rule=''Rule_11_2'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=4 where Rule=''Rule_11_2'' and status=''FAIL'' and transphasedesig!=4
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=4 where Rule='Rule_11_2' and status='FAIL' and transunitphasedesig!=4;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL'', comments=''Rule_11_1 transunitcount >1'' where rule in (''Rule_11_1'',''Rule_11_2'')
    and transunitcount >1 and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =7 and transunitphasedesig=7 and transunitcount=1 and rule=''Rule_12''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=1 and rule=''Rule_12'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=7 where Rule=''Rule_12'' and status=''FAIL'' and transphasedesig!=7
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=7 where Rule='Rule_12' and status='FAIL' and transunitphasedesig!=7;

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''PASS''  where transphasedesig =7 and transunitphasedesig=7 and transunitcount=1 and rule=''Rule_13''
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set status=''FAIL''  where transunitcount=1 and rule=''Rule_13'' and STATUS IS NULL
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set trphdsgtoupdate=7 where Rule=''Rule_13'' and status=''FAIL'' and transphasedesig!=7
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';
    --update IGP_TU_TRANS_SOURCELINE set trunitphdsgtoupdate=7 where Rule='Rule_13' and status='FAIL' and transunitphasedesig!=7;

    --Updating status here to compare rules and daphie phase prediction
    EXECUTE IMMEDIATE 'update PGE_IGP_TU_TRANS_SOURCEDATA set COMPARISON_STATUS=''FAIL'' where transunitcount=1 and phase_prediction!=trphdsgtoupdate
    and TRANSCIRCUITID in (' || sCircuitIds || ') and TABLEDATA = ''SOURCELINE''';

  END UPDATE_ADDITIONAL_FIELDS;

  PROCEDURE TRUNCATE_TABLES_CIRCUITWISE(sCircuitIds in varchar2,sUnprocessedTableName in varchar2) AS
  BEGIN
    IF sCircuitIds = 'ALL' THEN
        DELETE FROM PGE_IGP_TU_TRANS_SOURCEDATA;
        DELETE FROM PGE_IGP_TU_TRANS_TMP_DATA;
        --EXECUTE IMMEDIATE 'DELETE FROM ' || sUnprocessedTableName || ' WHERE NAME = ''EDGIS.TRANSFORMERUNIT''';
        commit;
    else
        EXECUTE IMMEDIATE 'DELETE FROM PGE_IGP_TU_TRANS_SOURCEDATA WHERE TRANSCIRCUITID IN (' || sCircuitIds || ')';
        EXECUTE IMMEDIATE 'DELETE FROM PGE_IGP_TU_TRANS_TMP_DATA WHERE CIRCUITID IN (' || sCircuitIds || ')';
        --EXECUTE IMMEDIATE 'DELETE FROM ' || sUnprocessedTableName || ' WHERE CIRCUITID IN (' || sCircuitIds || ') AND NAME = ''EDGIS.TRANSFORMERUNIT''';
        commit;
    end if;

  END TRUNCATE_TABLES_CIRCUITWISE;

END PGE_IGP_UPDATETRANSFORMERUNIT;

/

grant all on PGEIGPDATA.PGE_IGP_UPDATETRANSFORMERUNIT to IGPEDITOR;
commit;

select current_timestamp from dual;
spool off