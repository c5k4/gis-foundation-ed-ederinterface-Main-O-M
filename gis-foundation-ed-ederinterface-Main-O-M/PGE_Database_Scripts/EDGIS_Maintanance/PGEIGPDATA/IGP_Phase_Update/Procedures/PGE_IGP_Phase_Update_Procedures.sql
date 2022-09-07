spool D:\TEMP\PGE_IGP_PHASE_UPDATE_PROCEDURES.txt

select current_timestamp from dual;
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_CHECK_GUID_OCCURENCE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEIGPDATA"."PGE_IGP_CHECK_GUID_OCCURENCE" (sBatchNumber in varchar2) AS
  BEGIN

  EXECUTE IMMEDIATE 'update PGE_IGP_INPUT_METERS_DATA set ERROR_CODE=''IGPPHID_02'',ERROR_DESCRIPTION=''GLOBALID not found in EDGIS.ZZ_MV_SERVICELOCATION''
  where BATCHID IN (' || sBatchNumber || ') AND globalid in
  (select GLOBALID from PGE_IGP_INPUT_METERS_DATA
  minus
  select GLOBALID from EDGIS.ZZ_MV_SERVICELOCATION)';

  COMMIT;

  EXECUTE IMMEDIATE 'update PGE_IGP_INPUT_TRANSFORMER_DATA set ERROR_CODE=''IGPPHID_02'',ERROR_DESCRIPTION=''GLOBALID not found in EDGIS.ZZ_MV_TRANSFORMER''
  where BATCHID IN (' || sBatchNumber || ') AND globalid in
  (select GLOBALID from PGE_IGP_INPUT_TRANSFORMER_DATA
  minus
  select GLOBALID from EDGIS.ZZ_MV_TRANSFORMER)';
  COMMIT;

  EXECUTE IMMEDIATE 'update PGE_IGP_INPUT_CONDUCTORS_DATA set ERROR_CODE=''IGPPHID_02'',ERROR_DESCRIPTION=''GLOBALID not found in EDGIS CONDUCTORS''
  where BATCHID IN (' || sBatchNumber || ') AND globalid in
  (select GLOBALID from PGE_IGP_INPUT_CONDUCTORS_DATA
  minus (SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR UNION SELECT GLOBALID FROM EDGIS.ZZ_MV_DISTBUSBAR UNION SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR))';
  COMMIT;

END PGE_IGP_CHECK_GUID_OCCURENCE;

/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_CHECK_SUBTYPE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEIGPDATA"."PGE_IGP_CHECK_SUBTYPE" (sBatchNumber in varchar2) AS

BEGIN

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_INPUT_CONDUCTORS_DATA SET ERROR_CODE = ''IGPPHID_03'',ERROR_DESCRIPTION = ''DAPHIE Phase prediction did not match with GIS subtype''
    WHERE BATCHID IN (' || sBatchNumber || ') AND GLOBALID IN (SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR WHERE SUBTYPECD = 2 UNION
    SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR WHERE SUBTYPECD = 2)
    AND PHASE_PREDICTION NOT IN (3,5,6)';

  COMMIT;

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_INPUT_CONDUCTORS_DATA SET ERROR_CODE = ''IGPPHID_03'',ERROR_DESCRIPTION = ''DAPHIE Phase prediction did not match with GIS subtype''
    WHERE BATCHID IN (' || sBatchNumber || ') AND GLOBALID IN (SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR WHERE SUBTYPECD = 1 UNION
    SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR WHERE SUBTYPECD = 1)
    AND PHASE_PREDICTION NOT IN (1,2,4)';

  COMMIT;

    EXECUTE IMMEDIATE 'UPDATE PGE_IGP_INPUT_CONDUCTORS_DATA SET ERROR_CODE = ''IGPPHID_03'',ERROR_DESCRIPTION = ''DAPHIE Phase prediction did not match with GIS subtype''
    WHERE BATCHID IN (' || sBatchNumber || ') AND GLOBALID IN (SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIOHCONDUCTOR WHERE SUBTYPECD = 3 UNION
    SELECT GLOBALID FROM EDGIS.ZZ_MV_PRIUGCONDUCTOR WHERE SUBTYPECD = 3)
    AND PHASE_PREDICTION NOT IN (7)';

  COMMIT;

    EXECUTE IMMEDIATE 'update pge_igp_input_conductors_data set ERROR_CODE=''IGPPHID_03'',ERROR_DESCRIPTION=''Invalid Phase''
    where BATCHID IN (' || sBatchNumber || ') AND PHASE_PREDICTION NOT IN (1,2,3,4,5,6,7)';
  COMMIT;

END PGE_IGP_CHECK_SUBTYPE;

/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_UPDATE_OPENPOINT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEIGPDATA"."PGE_IGP_UPDATE_OPENPOINT" (
    sUnprocessedTableName VARCHAR2,
    sBatchNumber          VARCHAR2,
    sFeederNetworkTrace_Table IN VARCHAR2)
AS
TYPE DYN_CUR
IS
  REF
  CURSOR;
    CUR DYN_CUR;
    CUR1 DYN_CUR;
    vCnt           NUMBER;
    vCntCon        NUMBER;
    vLoopCnt       NUMBER;
    vFeatureOid1   VARCHAR2(38);
    vFeatureGuid1  VARCHAR2(255);
    vErrorMsg1     VARCHAR2(1000);
    vPhaseDesg1    VARCHAR2(10);
    vFeatureOid2   VARCHAR2(38);
    vFeatureGuid2  VARCHAR2(255);
    vErrorMsg2     VARCHAR2(1000);
    vPhaseDesg2    VARCHAR2(10);
    vfeatureOid_op VARCHAR2(38);
    vPhasedesg_op  VARCHAR2(10);
    vCircuitid_op  VARCHAR2(20);
    vToFeatureOid  VARCHAR2(38);
    vPhyName       VARCHAR2(50);
    vSql           VARCHAR2(2000);
    vErrCond1      NUMBER;
    vErrCond2      NUMBER;
  BEGIN
    OPEN CUR FOR 'select FEATURE_OID,VALUE,CIRCUITID from ' || sUnprocessedTableName || ' where NAME=''EDGIS.OPENPOINT'' and PROCESSED=''N'' AND BATCHID = ''' || sBatchNumber || '''';
    LOOP
      FETCH CUR INTO vfeatureOid_op,vPhasedesg_op,vCircuitid_op;
    EXIT
  WHEN CUR%notfound;
    BEGIN
      vCnt := 0;
      vSql := 'select count(1) from ' || sFeederNetworkTrace_Table || ' a left join SDE.GDB_ITEMS b
on a.to_feature_fcid = b.objectid where a.to_FEATURE_EID in ( select a.FROM_FEATURE_EID
from ' || sFeederNetworkTrace_Table || ' a left join SDE.GDB_ITEMS b
on a.to_feature_fcid = b.objectid where a.TO_FEATURE_OID = ' || vfeatureOid_op || ' and  (a.FEEDERID = ''' || vCircuitid_op || ''' OR a.FEEDERFEDBY = ''' || vCircuitid_op || '''))  and
(a.FEEDERID = ''' || vCircuitid_op || ''' OR a.FEEDERFEDBY = ''' || vCircuitid_op || ''')
and b.PHYSICALNAME in (''EDGIS.PRIOHCONDUCTOR'',''EDGIS.PRIUGCONDUCTOR'',''EDGIS.SECOHCONDUCTOR'',''EDGIS.SECUGCONDUCTOR'',''EDGIS.DISTBUSBAR'')';
      --DBMS_OUTPUT.PUT_LINE(vSql);
      EXECUTE IMMEDIATE vSql INTO vCnt;
      IF vCnt          = 2 THEN
        vFeatureOid1  := NULL;
        vFeatureGuid1 := NULL;
        vErrorMsg1    := NULL;
        vPhaseDesg1   := NULL;
        vFeatureOid2  := NULL;
        vFeatureGuid2 := NULL;
        vErrorMsg2    := NULL;
        vPhaseDesg2   := NULL;
        vLoopCnt      := 1;
        vErrCond1     := 0;
        vErrCond2     := 0;
        OPEN CUR1 FOR 'select a.TO_FEATURE_OID,b.PHYSICALNAME from ' || sFeederNetworkTrace_Table || ' a left join SDE.GDB_ITEMS b
on a.to_feature_fcid = b.objectid where a.to_FEATURE_EID in ( select a.FROM_FEATURE_EID
from ' || sFeederNetworkTrace_Table || ' a left join SDE.GDB_ITEMS b
on a.to_feature_fcid = b.objectid where a.TO_FEATURE_OID = ' || vfeatureOid_op || ' and  (a.FEEDERID = ''' || vCircuitid_op || ''' OR a.FEEDERFEDBY = ''' || vCircuitid_op || '''))  and
(a.FEEDERID = ''' || vCircuitid_op || ''' OR a.FEEDERFEDBY = ''' || vCircuitid_op || ''')
and b.PHYSICALNAME in (''EDGIS.PRIOHCONDUCTOR'',''EDGIS.PRIUGCONDUCTOR'',''EDGIS.SECOHCONDUCTOR'',''EDGIS.SECUGCONDUCTOR'',''EDGIS.DISTBUSBAR'')';
        LOOP
          FETCH CUR1 INTO vToFeatureOid,vPhyName;
          EXIT
        WHEN CUR1%notfound;
          vCntCon := 0;
          vSql    := 'select count(1) from ' || sUnprocessedTableName || ' where feature_oid = ' || vToFeatureOid || ' and batchid = ''' || sBatchNumber || '''';
          EXECUTE immediate vSql INTO vCntCon;
          IF vCntCon = 0 THEN
            vSql    := 'select objectid,decode(PHASEDESIGNATION,1,''C'',2,''B'',3,''BC'',4,''A'',5,''AC'',6,''AB'',7,''ABC'',NULL) PHASEDESIGNATION,NULL ERROR_MSG,globalid from ' || REPLACE(vPhyName,'EDGIS.','EDGIS.ZZ_MV_') || ' where objectid = ' || vToFeatureOid;
          END IF;
          IF vCntCon > 1 THEN
            vSql    := 'select feature_oid,decode(Value,1,''C'',2,''B'',3,''BC'',4,''A'',5,''AC'',6,''AB'',7,''ABC'',NULL) PHASEDESIGNATION,error_msg,FEATURE_GUID from ' || sUnprocessedTableName || ' where PROCESSED = ''N'' and feature_oid = ' || vToFeatureOid || ' and BATCHID = ''' || sBatchNumber || ''' and rownum = 1';
          END IF;
          IF vCntCon = 1 THEN
            vSql    := 'select feature_oid,decode(Value,1,''C'',2,''B'',3,''BC'',4,''A'',5,''AC'',6,''AB'',7,''ABC'',NULL) PHASEDESIGNATION,error_msg,FEATURE_GUID from ' || sUnprocessedTableName || ' where feature_oid = ' || vToFeatureOid || ' and BATCHID = ''' || sBatchNumber || '''';
          END IF;
          IF vLoopCnt = 1 THEN
            BEGIN
              EXECUTE immediate vSql INTO vFeatureOid1,
              vPhaseDesg1,
              vErrorMsg1,
              vFeatureGuid1;
            EXCEPTION
            WHEN OTHERS THEN
              vErrCond1 := 1;
            END;
          END IF;
          IF vLoopCnt = 2 THEN
            BEGIN
              EXECUTE immediate vSql INTO vFeatureOid2,
              vPhaseDesg2,
              vErrorMsg2,
              vFeatureGuid2;
            EXCEPTION
            WHEN OTHERS THEN
              vErrCond2 := 1;
            END;
            --Logic
            IF vErrCond1 = 1 OR vErrCond2 = 1 THEN
              --if both conductor not found
              IF vErrCond1 = vErrCond2 THEN
                EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''E'',ERROR_MSG = ''Adjacent conductors not found: Immediate Conductor1 GUID = ' || vFeatureGuid1 || ' :  Immediate Conductor2 GUID = ' || vFeatureGuid2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
              ELSE
                IF vErrCond1 <> vErrCond2 THEN
                  --if first conductor not found
                  IF vErrCond1 = 1 THEN
                    EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg2 || ''',ERROR_MSG = ''Immediate Conductor1 not found : Immediate Conductor1 GUID = ' || vFeatureGuid1 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                  END IF;
                  --if Second conductor not found
                  IF vErrCond2 = 1 THEN
                    EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg1 || ''',ERROR_MSG = ''Immediate Conductor2 not found : Immediate Conductor2 GUID = ' || vFeatureGuid2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                  END IF;
                END IF;
              END IF;
            ELSE
              IF vErrorMsg1 IS NOT NULL AND vErrorMsg2 IS NOT NULL THEN
                EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''E'',ERROR_MSG = ''Feature not updated due to errorenous adjacent conductor: Immediate Conductor1 GUID = ' || vFeatureGuid1 || ' and Phase = ' || vPhaseDesg1 || ' :  Immediate Conductor2 GUID = ' || vFeatureGuid2 || ' and Phase = ' || vPhaseDesg2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
              ELSE
                IF vErrorMsg1 IS NOT NULL OR vErrorMsg2 IS NOT NULL THEN
                  --if ErrorMsg is not null for first conductor
                  IF vErrorMsg1 IS NOT NULL THEN
                    EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg2 || ''',ERROR_MSG = ''Error in Conductor1 : Immediate Conductor1 GUID = ' || vFeatureGuid1 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                  END IF;
                  --if ErrorMsg is not null for second conductor
                  IF vErrorMsg2 IS NOT NULL THEN
                    EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg1 || ''',ERROR_MSG = ''Error in Conductor2 : Immediate Conductor2 GUID = ' || vFeatureGuid2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                  END IF;
                ELSE
                  IF LENGTH(vPhaseDesg1)  <> LENGTH(vPhaseDesg2) THEN
                    IF LENGTH(vPhaseDesg1) > LENGTH(vPhaseDesg2) THEN
                      EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg1 || ''',ERROR_MSG = ''Phase Mismatch Error : Immediate Conductor1 GUID = ' || vFeatureGuid1 || ' and Phase = ' || vPhaseDesg1 || ' :  Immediate Conductor2 GUID = ' || vFeatureGuid2 || ' and Phase = ' || vPhaseDesg2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                    END IF;
                    IF LENGTH(vPhaseDesg1) < LENGTH(vPhaseDesg2) THEN
                      EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg2 || ''',ERROR_MSG = ''Phase Mismatch Error : Immediate Conductor1 GUID = ' || vFeatureGuid1 || ' and Phase = ' || vPhaseDesg1 || ' :  Immediate Conductor2 GUID = ' || vFeatureGuid2 || ' and Phase = ' || vPhaseDesg2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                    END IF;
                  ELSE
                    IF (LENGTH(vPhaseDesg1) = LENGTH(vPhaseDesg2)) AND (vPhaseDesg1 = vPhaseDesg2) THEN
                      EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg1 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                    ELSE
                      IF (LENGTH(vPhaseDesg1) = LENGTH(vPhaseDesg2)) AND (vPhaseDesg1 <> vPhaseDesg2) THEN
                        EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''N'',value = ''' || vPhaseDesg1 || ''',ERROR_MSG = ''Phase Mismatch Error : Immediate Conductor1 GUID = ' || vFeatureGuid1 || ' and Phase = ' || vPhaseDesg1 || ' :  Immediate Conductor2 GUID = ' || vFeatureGuid2 || ' and Phase = ' || vPhaseDesg2 || ''' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
                      END IF;
                    END IF;
                  END IF;
                END IF;
              END IF;
            END IF;
          END IF;
          vLoopCnt := vLoopCnt + 1;
        END LOOP;
        CLOSE CUR1;
      END IF;
      IF vCnt > 2 THEN
        EXECUTE immediate 'update ' || sUnprocessedTableName || ' set PROCESSED = ''E'',ERROR_MSG = ''More than 2 conductor found'' where FEATURE_OID = ' || vfeatureOid_op || ' and BATCHID = ''' || sBatchNumber || '''';
      END IF;
      COMMIT;
    EXCEPTION
    WHEN OTHERS THEN
      NULL;
    END;
  END LOOP;
  CLOSE CUR;
END PGE_IGP_UPDATE_OPENPOINT;

/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_UPDATECONDUCTORINFO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEIGPDATA"."PGE_IGP_UPDATECONDUCTORINFO" (sCircuitIds in varchar2,sUnprocessedTableName in varchar2,sBatchNumber in varchar2,iSerialNo in Number) AS

 iGlbCnt number;
 phasevalue varchar2(2000);
 TYPE DYN_CUR is REF CURSOR;
  CUR1 DYN_CUR;

  vFeatGuid varchar2(100);
  vValuePhase varchar2(20);

BEGIN

    open CUR1 for 'select FEATURE_GUID,VALUE from ' || sUnprocessedTableName || ' where NAME=''EDGIS.PRIUGCONDUCTOR'' and PROCESSED=''N'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
    LOOP
    fetch  CUR1 into vFeatGuid,vValuePhase;
             exit when CUR1%notfound;
         --DBMS_OUTPUT.PUT_LINE(vObj || ' ** ' || vSubtyp);
 --END LOOP;
 --CLOSE CUR1;

-- PRIUGCONDUCTORINFO
 --FOR REC in (select FEATURE_GUID,VALUE from PGE_IGP_UNPROCESSED_REC where NAME='EDGIS.PRIUGCONDUCTOR' and PROCESSED='N')
 --LOOP

    iGlbCnt := 0;
    phasevalue:=null;
    phasevalue := TO_CHAR(vValuePhase);
    --GETTING COUNT OF RELATED RECORDS
    select count(CONDUCTORGUID) into iGlbCnt from EDGIS.ZZ_MV_PRIUGCONDUCTORINFO where CONDUCTORGUID = vFeatGuid and conductoruse in (1,4,6) ;

    ----INSERT INTO UNPROCESSED TABLE
    --INSERT INTO PGE_IGP_UNPROCESSED_REC(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID)
    --SELECT 'TABLE','EDGIS.ZZ_MV_PRIUGCONDUCTORINFO',TO_CHAR(A.OBJECTID),'R',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID
    --FROM EDGIS.ZZ_MV_PRIUGCONDUCTORINFO A
    --INNER JOIN
    --PGE_IGP_UNPROCESSED_REC B
    --ON A.CONDUCTORGUID=B.FEATURE_GUID
    --WHERE B.FEATURE_GUID=REC.FEATURE_GUID;

    --EXECUTE IMMEDIATE 'INSERT INTO ' || sUnprocessedTableName || '(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID)
    --SELECT ''TABLE'',''EDGIS.ZZ_MV_PRIUGCONDUCTORINFO'',TO_CHAR(A.OBJECTID),''R'',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID
    --FROM EDGIS.ZZ_MV_PRIUGCONDUCTORINFO A
    --INNER JOIN
    --' || sUnprocessedTableName || ' B
    --ON A.CONDUCTORGUID=B.FEATURE_GUID
    --WHERE B.FEATURE_GUID=''' || vFeatGuid || ''' AND B.CIRCUITID IN (' || sCircuitIds || ')';

    EXECUTE IMMEDIATE 'INSERT INTO ' || sUnprocessedTableName || '(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID,
    BATCHID,SERIAL_NO,PARENT_GUID,PARENT_CLASS)
    SELECT ''TABLE'',''EDGIS.PRIUGCONDUCTORINFO'',TO_CHAR(A.OBJECTID),''R'',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID,''' ||
    sBatchNumber || ''',' || iSerialNo || ',''' || vFeatGuid || ''',''EDGIS.PRIUGCONDUCTOR''
    FROM EDGIS.ZZ_MV_PRIUGCONDUCTORINFO A
    INNER JOIN
    ' || sUnprocessedTableName || ' B
    ON A.CONDUCTORGUID=B.FEATURE_GUID
    WHERE B.FEATURE_GUID=''' || vFeatGuid || ''' AND B.CIRCUITID IN (' || sCircuitIds || ') AND A.CONDUCTORUSE IN (1,4,6) AND B.BATCHID IN (''' || sBatchNumber || ''')';

    COMMIT;

    --If Related Objects = 1
    if (iGlbCnt = 1) then
        --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

        EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
        CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
        commit;

    --If Related Objects = 2
    elsif (iGlbCnt = 2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='7') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='6' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''6'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        end if;
    --If Related Objects > 2
    elsif (iGlbCnt >2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue = '5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='7') then
            if(iGlbCnt=3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           elsif (iGlbCnt > 3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           end if;
        end if;
    end if;
    END LOOP;
 CLOSE CUR1;

    open CUR1 for 'select FEATURE_GUID,VALUE from ' || sUnprocessedTableName || ' where NAME=''EDGIS.PRIOHCONDUCTOR'' and PROCESSED=''N'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
    LOOP
    fetch  CUR1 into vFeatGuid,vValuePhase;
             exit when CUR1%notfound;
-- PRIOHONDUCTORINFO
 --FOR REC in (select FEATURE_GUID,VALUE from PGE_IGP_UNPROCESSED_REC where NAME='EDGIS.PRIOHCONDUCTOR' and PROCESSED='N')
 --LOOP

    iGlbCnt := 0;
    phasevalue:=null;
    phasevalue := TO_CHAR(vValuePhase);
    --GETTING COUNT OF RELATED RECORDS
    select count(CONDUCTORGUID) into iGlbCnt from EDGIS.ZZ_MV_PRIOHCONDUCTORINFO where CONDUCTORGUID = vFeatGuid and conductoruse in (1,4,6);

    ----INSERT INTO UNPROCESSED TABLE
    --INSERT INTO PGE_IGP_UNPROCESSED_REC(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID)
    --SELECT 'TABLE','EDGIS.PRIOHCONDUCTORINFO',TO_CHAR(A.OBJECTID),'R',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID
    --FROM EDGIS.ZZ_MV_PRIOHCONDUCTORINFO A
    --INNER JOIN
    --PGE_IGP_UNPROCESSED_REC B
    --ON A.CONDUCTORGUID=B.FEATURE_GUID
    --WHERE B.FEATURE_GUID=REC.FEATURE_GUID;

    EXECUTE IMMEDIATE 'INSERT INTO ' || sUnprocessedTableName || '(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID,
    BATCHID,SERIAL_NO,PARENT_GUID,PARENT_CLASS)
    SELECT ''TABLE'',''EDGIS.PRIOHCONDUCTORINFO'',TO_CHAR(A.OBJECTID),''R'',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID,''' ||
    sBatchNumber || ''',' || iSerialNo || ',''' || vFeatGuid || ''',''EDGIS.PRIOHCONDUCTOR''
    FROM EDGIS.ZZ_MV_PRIOHCONDUCTORINFO A
    INNER JOIN
    ' || sUnprocessedTableName || ' B
    ON A.CONDUCTORGUID=B.FEATURE_GUID
    WHERE B.FEATURE_GUID=''' || vFeatGuid || ''' AND B.CIRCUITID IN (' || sCircuitIds || ') AND A.CONDUCTORUSE IN (1,4,6) AND B.BATCHID IN (''' || sBatchNumber || ''')';

    COMMIT;

    --If Related Objects = 1
    if (iGlbCnt = 1) then
        --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

        EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
        CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
        commit;

    --If Related Objects = 2
    elsif (iGlbCnt = 2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='7') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='6' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''6'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        end if;
    --If Related Objects > 2
    elsif (iGlbCnt >2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue = '6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='7') then
            if(iGlbCnt=3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           elsif (iGlbCnt > 3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           end if;
        end if;
    end if;
    end loop;
  CLOSE CUR1;

    open CUR1 for 'select FEATURE_GUID,VALUE from ' || sUnprocessedTableName || ' where NAME=''EDGIS.SECUGCONDUCTOR'' and PROCESSED=''N'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
    LOOP
    fetch  CUR1 into vFeatGuid,vValuePhase;
             exit when CUR1%notfound;
--SECUGCONDUCTOR
 --FOR REC in (select FEATURE_GUID,VALUE from PGE_IGP_UNPROCESSED_REC where NAME='EDGIS.SECUGCONDUCTOR' and PROCESSED='N')
 --LOOP

    iGlbCnt := 0;
    phasevalue:=null;
    phasevalue := TO_CHAR(vValuePhase);
    --GETTING COUNT OF RELATED RECORDS
    select count(CONDUCTORGUID) into iGlbCnt from EDGIS.ZZ_MV_SECUGCONDUCTORINFO where CONDUCTORGUID = vFeatGuid and conductoruse in (5,6,8);

    ----INSERT INTO UNPROCESSED TABLE
    --INSERT INTO PGE_IGP_UNPROCESSED_REC(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID)
    --SELECT 'TABLE','EDGIS.ZZ_MV_SECUGCONDUCTORINFO',TO_CHAR(A.OBJECTID),'R',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID
    --FROM EDGIS.ZZ_MV_SECUGCONDUCTORINFO A
    --INNER JOIN
    --PGE_IGP_UNPROCESSED_REC B
    --ON A.CONDUCTORGUID=B.FEATURE_GUID
    --WHERE B.FEATURE_GUID=REC.FEATURE_GUID;

    EXECUTE IMMEDIATE 'INSERT INTO ' || sUnprocessedTableName || '(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID,
    BATCHID,SERIAL_NO,PARENT_GUID,PARENT_CLASS)
    SELECT ''TABLE'',''EDGIS.SECUGCONDUCTORINFO'',TO_CHAR(A.OBJECTID),''R'',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID,''' ||
    sBatchNumber || ''',' || iSerialNo || ',''' || vFeatGuid || ''',''EDGIS.SECUGCONDUCTOR''
    FROM EDGIS.ZZ_MV_SECUGCONDUCTORINFO A
    INNER JOIN
    ' || sUnprocessedTableName || ' B
    ON A.CONDUCTORGUID=B.FEATURE_GUID
    WHERE B.FEATURE_GUID=''' || vFeatGuid || ''' AND B.CIRCUITID IN (' || sCircuitIds || ') AND A.CONDUCTORUSE IN (5,6,8) AND B.BATCHID IN (''' || sBatchNumber || ''')';

    COMMIT;

    --If Related Objects = 1
    if (iGlbCnt = 1) then
        --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

        EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
        CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
        commit;

    --If Related Objects = 2
    elsif (iGlbCnt = 2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='7') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='6' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''6'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        end if;
    --If Related Objects > 2
    elsif (iGlbCnt >2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue = '6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='7') then
            if(iGlbCnt=3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           elsif (iGlbCnt > 3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           end if;
        end if;
    end if;
    end loop;
    CLOSE CUR1;

    open CUR1 for 'select FEATURE_GUID,VALUE from ' || sUnprocessedTableName || ' where NAME=''EDGIS.SECOHCONDUCTOR'' and PROCESSED=''N'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
    LOOP
    fetch  CUR1 into vFeatGuid,vValuePhase;
             exit when CUR1%notfound;
--SECOHCONDUCTOR
 --FOR REC in (select FEATURE_GUID,VALUE from PGE_IGP_UNPROCESSED_REC where NAME='EDGIS.SECOHCONDUCTOR' and PROCESSED='N')
 --LOOP

    iGlbCnt := 0;
    phasevalue:=null;
    phasevalue := TO_CHAR(vValuePhase);
    --GETTING COUNT OF RELATED RECORDS
    select count(CONDUCTORGUID) into iGlbCnt from EDGIS.ZZ_MV_SECOHCONDUCTORINFO where CONDUCTORGUID = vFeatGuid and conductoruse in (5,6,8);

    ----INSERT INTO UNPROCESSED TABLE
    --INSERT INTO PGE_IGP_UNPROCESSED_REC(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID)
    --SELECT 'TABLE','EDGIS.ZZ_MV_SECOHCONDUCTORINFO',TO_CHAR(A.OBJECTID),'R',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID
    --FROM EDGIS.ZZ_MV_SECOHCONDUCTORINFO A
    --INNER JOIN
    --PGE_IGP_UNPROCESSED_REC B
    --ON A.CONDUCTORGUID=B.FEATURE_GUID
    --WHERE B.FEATURE_GUID=REC.FEATURE_GUID;

    EXECUTE IMMEDIATE 'INSERT INTO ' || sUnprocessedTableName || '(REC_TYPE,NAME,FEATURE_OID,PROCESSED,CIRCUITID,OLD_PHASE,FEATURE_GUID,
    BATCHID,SERIAL_NO,PARENT_GUID,PARENT_CLASS)
    SELECT ''TABLE'',''EDGIS.SECOHCONDUCTORINFO'',TO_CHAR(A.OBJECTID),''R'',TO_CHAR(B.CIRCUITID),TO_CHAR(A.PHASEDESIGNATION),A.GLOBALID,''' ||
    sBatchNumber || ''',' || iSerialNo || ',''' || vFeatGuid || ''',''EDGIS.SECOHCONDUCTOR''
    FROM EDGIS.ZZ_MV_SECOHCONDUCTORINFO A
    INNER JOIN
    ' || sUnprocessedTableName || ' B
    ON A.CONDUCTORGUID=B.FEATURE_GUID
    WHERE B.FEATURE_GUID=''' || vFeatGuid || ''' AND B.CIRCUITID IN (' || sCircuitIds || ') AND A.CONDUCTORUSE IN (5,6,8) AND B.BATCHID IN (''' || sBatchNumber || ''')';

    COMMIT;

    --If Related Objects = 1
    if (iGlbCnt = 1) then
        --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

        EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
        CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
        commit;

    --If Related Objects = 2
    elsif (iGlbCnt = 2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue='7') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='6' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''6'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        end if;
    --If Related Objects > 2
    elsif (iGlbCnt >2) then
        if(phasevalue = '4' or phasevalue ='2' or phasevalue ='1') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE=phasevalue WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=' || phasevalue || ' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            commit;
        elsif(phasevalue = '6') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '5') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue = '3') then
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
            --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

            EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
            CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
            commit;
        elsif(phasevalue='7') then
            if(iGlbCnt=3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           elsif (iGlbCnt > 3) then
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='1' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='2' WHERE REC_TYPE='TABLE' AND VALUE IS NULL AND ROWNUM=1;
                --UPDATE PGE_IGP_UNPROCESSED_REC SET VALUE='4' WHERE REC_TYPE='TABLE' AND VALUE IS NULL;

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''1'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''2'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND ROWNUM=1 AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';

                EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''4'' WHERE REC_TYPE=''TABLE'' AND VALUE IS NULL AND
                CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''')';
                commit;
           end if;
        end if;
    end if;
    end loop;
    CLOSE CUR1;

end PGE_IGP_UPDATECONDUCTORINFO;

/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_UPDATE_UNPROCESS_PHASE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEIGPDATA"."PGE_IGP_UPDATE_UNPROCESS_PHASE" (sCircuitIds in varchar2,sUnprocessedTableName in varchar2,sBatchNumber in varchar2,iSerialNo in Number) AS

BEGIN

    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''C'' WHERE VALUE=''1'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''B'' WHERE VALUE=''2'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''BC'' WHERE VALUE=''3'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''A'' WHERE VALUE=''4'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''AC'' WHERE VALUE=''5'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''AB'' WHERE VALUE=''6'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET VALUE=''ABC'' WHERE VALUE=''7'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';

    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''C'' WHERE OLD_PHASE=''1'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''B'' WHERE OLD_PHASE=''2'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''BC'' WHERE OLD_PHASE=''3'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''A'' WHERE OLD_PHASE=''4'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''AC'' WHERE OLD_PHASE=''5'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''AB'' WHERE OLD_PHASE=''6'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';
    EXECUTE IMMEDIATE 'UPDATE ' || sUnprocessedTableName || ' SET OLD_PHASE=''ABC'' WHERE OLD_PHASE=''7'' AND CIRCUITID IN (' || sCircuitIds || ') AND BATCHID IN (''' || sBatchNumber || ''') AND SERIAL_NO IN (' || iSerialNo || ')';

    commit;

end PGE_IGP_UPDATE_UNPROCESS_PHASE;

/

--------------------------------------------------------
--  DDL for Procedure PGE_IGP_DMS_UPDATE
--------------------------------------------------------
set define off;

create or replace PROCEDURE PGE_IGP_DMS_UPDATE (iBatchID in  varchar2,sCircuitId in varchar2,versionFullName in varchar2) AS 

BEGIN

    Delete from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where BATCHID=   iBatchID ;
    
      -- Set Version to required version
    sde.version_util.set_current_version(versionFullName);

   --Insert Records from EDGIS.zz_mv_PriOHConductor
     Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.PRIOHCONDUCTOR', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_PriOHConductor.phasedesignation) ,iBatchID from EDGIS.zz_mv_PriOHConductor where CircuitID=
   sCircuitId and status = 5 );
     --Insert Records from EDGIS.zz_mv_PriUGConductor
      Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.PRIUGCONDUCTOR', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_PriUGConductor.phasedesignation) ,iBatchID from EDGIS.zz_mv_PriUGConductor where CircuitID=
   sCircuitId and status = 5 );
   --Insert Records from EDGIS.ZZ_MV_DISTBUSBAR
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.DISTBUSBAR', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_DISTBUSBAR.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_DISTBUSBAR where CircuitID=
   sCircuitId and status = 5 );
  --Insert Records from EDGIS.ZZ_MV_Transformer
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.TRANSFORMER', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_Transformer.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_Transformer where CircuitID=
   sCircuitId and status = 5 );
     --Insert Records from EDGIS.zz_mv_FUSE
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.FUSE', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_FUSE.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_FUSE where CircuitID=
   sCircuitId and status = 5 );
     --Insert Records from EDGIS.zz_mv_OPENPOINT
       
   Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   ( select distinct b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,sCircuitId, PGEIGPDATA.IGP_GET_PHASE_FROMCODE(a.TO_FEATURE_FEEDERINFO),iBatchID from PGEIGPDATA.PGE_FEEDERFEDNETWORK_TRACE a left join sde.GDB_ITEMS b
  on a.to_feature_fcid = b.objectid where (((a.FEEDERID = sCircuitId) OR (a.FEEDERFEDBY = sCircuitId) ) and( b.physicalname='EDGIS.OPENPOINT') ));
  
   --Insert Records from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.DYNAMICPROTECTIVEDEVICE', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_DYNAMICPROTECTIVEDEVICE.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE where CircuitID=
   sCircuitId and status = 5 );
  --Insert Records from EDGIS.ZZ_MV_SWITCH
   Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   ( select distinct b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,sCircuitId, PGEIGPDATA.IGP_GET_PHASE_FROMCODE(a.TO_FEATURE_FEEDERINFO),iBatchID from PGEIGPDATA.PGE_FEEDERFEDNETWORK_TRACE a left join sde.GDB_ITEMS b
  on a.to_feature_fcid = b.objectid where (((a.FEEDERID = sCircuitId) OR (a.FEEDERFEDBY = sCircuitId) ) and( b.physicalname='EDGIS.SWITCH') ));
  
     --Insert Records from EDGIS.zz_mv_STEPDOWN
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.STEPDOWN', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_STEPDOWN.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_STEPDOWN where CircuitID=
   sCircuitId and status = 5 );
     --Insert Records from EDGIS.zz_mv_VOLTAGEREGULATOR
      Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.VOLTAGEREGULATOR', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_VOLTAGEREGULATOR.phasedesignation) ,iBatchID from EDGIS.zz_mv_VOLTAGEREGULATOR where CircuitID=
   sCircuitId and status = 5 );
   --Insert Records from EDGIS.ZZ_MV_FAULTINDICATOR
    Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID) 
   (select 'EDGIS.FAULTINDICATOR', OBJECTID,GLOBALID,circuitid,PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_FAULTINDICATOR.phasedesignation) ,iBatchID from EDGIS.ZZ_MV_FAULTINDICATOR where CircuitID=
   sCircuitId and status = 5 );
   
   
    FOR CUR_tx IN (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class , 
    a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID, 
    (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
    b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMER') --1001
    and b.to_feature_fcid in (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR' -- 1022
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR' -- 1024
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMERLEAD' -- 1025
    ))
    LOOP
    
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_tx.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_tx.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_tx.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_tx.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    -- transformer end---
    
    -- open point end--
     FOR CUR_op IN (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
      b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=5772743
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT') --1010
      and b.to_feature_fcid in (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR' -- 1022
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR' -- 1024
      )
      and a.to_feature_oid not in 
      (
      -- Excluding open point which are breaking the network_for these there will be two upstream feature in trace table
      -- On Arc Map_ Arc FM upstream trace gives no results for these features 
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT') --1010
      and b.to_feature_fcid in (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR' -- 1022
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR' -- 1024
      )
      group by a.to_feature_oid having count(*) >1
      ))
    LOOP
    
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_op.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_op.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_op.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_op.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
    ---open point end --
    
    
    -- switch  start--
    
    FOR CUR_sw IN (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
    a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
    (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
    b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    --and a.to_feature_oid=3154752
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH') --1005
    and b.to_feature_fcid in 
    (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
     
    )
    and a.to_feature_oid not in 
    (
    -- Excluding features which are breaking the network_for these there will be two upstream feature in trace table
    -- On Arc Map_ Arc FM upstream trace gives no results for these features 
    select a.to_feature_oid
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    --and a.to_feature_oid=5772743
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH') --1005
    and b.to_feature_fcid in 
    (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
    )
    group by a.to_feature_oid having count(*) >1
    ))
   LOOP
    
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_sw.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_sw.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_sw.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_sw.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
    -- switch ended----
      -- stepdown started--
      FOR CUR_sd IN (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
      b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=242333
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN') --1002
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      ))
      
      LOOP
    
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_sd.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_sd.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_sd.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_sd.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
    --stepdown ended--
    
    
    -- FUSE  STARTED ---- 
    FOR CUR_fuse IN (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
    a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
    (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
    b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    --and a.to_feature_oid=2631315
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FUSE') --1003
    and b.to_feature_fcid in 
    (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
    )
    and a.to_feature_oid not in 
    (
    -- Excluding features which are breaking the network_for these there will be two upstream feature in trace table
    -- On Arc Map_ Arc FM upstream trace gives no results for these features
    select a.to_feature_oid
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FUSE') --1003
    and b.to_feature_fcid in 
    (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
    )
    group by a.to_feature_oid having count(*) >1
    ))
    
    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_fuse.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_fuse.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_fuse.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_fuse.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    -- FUSE ENDED---
    
    
    --voltageregulator started---
   FOR CUR_volr in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
    a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
    (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
    b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    --and a.to_feature_oid=1125803
    and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR') --1000
    and b.to_feature_fcid in 
    (
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
    ))
    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_volr.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_volr.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_volr.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_volr.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    --voltage regulator ended--
    
    -- dpd --
    FOR CUR_dpd in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as UpStream_Feature_Class,
      b.to_feature_oid as UpStream_Feature_OID,b.to_feature_globalid as UpStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=1562418
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE') --998
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are breaking the network_for these there will be two upstream feature in trace table
      -- On Arc Map_ Arc FM upstream trace gives no results for these features
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE') --998
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      group by a.to_feature_oid having count(*) > 1
      ))
    
      LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_dpd.UpStream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_dpd.UpStream_Feature_OID,
        UPSTREAM_FEATURE_GUID=CUR_dpd.UpStream_Feature_GUID 
        where FEATURE_OID= CUR_dpd.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
    -- dpd ended---
    
    -- busbar started --
    FOR CUR_dbar in (select c.Busbar_OID,(select name from sde.gdb_items WHERE objectid= d.to_feature_fcid ) as Req_Upstream_Feature_Class,d.to_feature_oid as Req_Upstream_Cond_OID,d.to_feature_globalid as Req_Upstream_Cond_GUID from 
    (select a.to_feature_oid as Busbar_OID,b.to_feature_fcid,b.to_feature_oid,b.to_feature_eid,b.from_feature_eid 
    from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
    on a.from_feature_eid= b.to_feature_eid
    where a.feederid=sCircuitId 
    --and a.to_feature_oid=5878773
    and a.to_feature_fcid=(select objectid from sde.gdb_items WHERE UPPER(name)='EDGIS.DISTBUSBAR')
    and b.to_feature_fcid in 
    (
    --998,1000,1001,1002,1003,1004,1005,1008,1010,1014,19469
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMER'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FUSE'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FAULTINDICATOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.CAPACITORBANK'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIMARYMETER'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.ELECTRICDISTNETWORK_JUNCTIONS'
    )
    )c
    join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE d
    on c.from_feature_eid=d.to_feature_eid
    and d.to_feature_fcid in 
    (
    --1019,1020,1021,1022,1023,1024,1025
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DCCONDUCTOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR'
    union
    select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMERLEAD'
    ))
    
    LOOP
        Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
        set UPSTREAM_FEATURE_CLASS = CUR_dbar.Req_Upstream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_dbar.Req_Upstream_Cond_OID,
        UPSTREAM_FEATURE_GUID=CUR_dbar.Req_Upstream_Cond_GUID 
        where FEATURE_OID= CUR_dbar.Busbar_OID and BATCHID = iBatchID;

    END LOOP;
    
    -- busbar ended---
    
    -- primary oh  started --
    
    FOR CUR_prioh in (select c.PriOHCond_OID,(select name from sde.gdb_items WHERE objectid= d.to_feature_fcid) as Req_Upstream_Feature_Class,d.to_feature_oid as Req_Upstream_Cond_OID,d.to_feature_globalid as Req_Upstream_Cond_GUID from 
      (
      select a.to_feature_oid as PriOHCond_OID,b.to_feature_fcid,b.to_feature_oid,b.to_feature_eid,b.from_feature_eid 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=9746333
      and a.to_feature_fcid=(select objectid from sde.gdb_items WHERE UPPER(name)='EDGIS.PRIOHCONDUCTOR')
      and b.to_feature_fcid in 
      (
      --3849,997,998,1000,1001,1002,1003,1004,1005,1006,1008,1010,1014,19469
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIMARYRISER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.ELECTRICSTITCHPOINT'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FUSE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FAULTINDICATOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TIE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.CAPACITORBANK'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIMARYMETER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.ELECTRICDISTNETWORK_JUNCTIONS'
      )
      )c
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE d
      on c.from_feature_eid=d.to_feature_eid
      and d.to_feature_fcid in 
      (
      --1019,1020,1021,1022,1023,1024,1025
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DCCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMERLEAD'
      ))
      
       LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_prioh.Req_Upstream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_prioh.Req_Upstream_Cond_OID,
        UPSTREAM_FEATURE_GUID=CUR_prioh.Req_Upstream_Cond_GUID 
        where FEATURE_OID= CUR_prioh.PriOHCond_OID and BATCHID = iBatchID;

    END LOOP;
    
    FOR CUR_priug in (select c.PriUGCond_OID,(select name from sde.gdb_items WHERE objectid= d.to_feature_fcid) as Req_Upstream_Feature_Class,d.to_feature_oid as Req_Upstream_Cond_OID,d.to_feature_globalid as Req_Upstream_Cond_GUID from 
      (
      select a.to_feature_oid as PriUGCond_OID,b.to_feature_fcid,b.to_feature_oid,b.to_feature_eid,b.from_feature_eid 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.from_feature_eid= b.to_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=6552914
      and a.to_feature_fcid=(select objectid from sde.gdb_items WHERE UPPER(name)='EDGIS.PRIUGCONDUCTOR')
      and b.to_feature_fcid in 
      (
      --3849,997,998,1000,1001,1002,1003,1004,1005,1006,1008,1010,1014,19469
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIMARYRISER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.ELECTRICSTITCHPOINT'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FUSE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FAULTINDICATOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TIE'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.CAPACITORBANK'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIMARYMETER'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.ELECTRICDISTNETWORK_JUNCTIONS'
      )
      )c
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE d
      on c.from_feature_eid=d.to_feature_eid
      and d.to_feature_fcid in 
      (
      --1019,1020,1021,1022,1023,1024,1025
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DCCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR'
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.TRANSFORMERLEAD'
      ))
    
      LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set UPSTREAM_FEATURE_CLASS = CUR_priug.Req_Upstream_Feature_Class, 
        UPSTREAM_FEATURE_OID= CUR_priug.Req_Upstream_Cond_OID,
        UPSTREAM_FEATURE_GUID=CUR_priug.Req_Upstream_Cond_GUID 
        where FEATURE_OID= CUR_priug.PriUGCond_OID and BATCHID = iBatchID;

    END LOOP;
    
    
    ---DOWNSTREAM  UPDATE---------
    
      For CUR_dwn_sw in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=3159061
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH') --1005
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=3159061
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SWITCH') --1005
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      group by a.to_feature_oid having count(*) >1
      ))

  LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_sw.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_sw.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_sw.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_sw.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
-- Downstream line features for all Switches ---)

-- Downstream line features for all Open Points ---
--Input - CircuitID

      fOR CUR_dwn_op in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=5776968
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT') --1010
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR' -- 1022
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR' -- 1024
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=5776968
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.OPENPOINT') --1010
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECUGCONDUCTOR' -- 1022
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.SECOHCONDUCTOR' -- 1024
      )
      group by a.to_feature_oid having count(*) >1
      ))
    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_op.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_op.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_op.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_op.Feature_OID and BATCHID = iBatchID;

    END LOOP;
    
-- Downstream line features for all Open Points ---

-- Downstream line features for all Voltage Regulators ---
--Input - CircuitID

      FOR CUR_dwn_vr in(select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=1126122
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR') --1000
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=1126122
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.VOLTAGEREGULATOR') --1000
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      )
      group by a.to_feature_oid having count(*) >1
      ))

    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_vr.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_vr.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_vr.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_vr.Feature_OID and BATCHID = iBatchID;

    END LOOP;
-- Downstream line features for all Voltage Regulators ---

-- Downstream line features for all Step down ---
--Input - CircuitID

      FOR CUR_dwn_sd in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=514609
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN') --1002
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=514609
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.STEPDOWN') --1002
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      group by a.to_feature_oid having count(*) >1
      ))

    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_sd.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_sd.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_sd.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_sd.Feature_OID and BATCHID = iBatchID;

    END LOOP;
-- Downstream line features for all Step down ---

-- Downstream line features for all Dynamic Protective Devices ---
--Input - CircuitID

      FOR CUR_dwn_dpd in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId
      --and a.to_feature_oid=4319306
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE') --998
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=4319306
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DYNAMICPROTECTIVEDEVICE') --998
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      group by a.to_feature_oid having count(*) >1
      ))
    LOOP
    Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
    set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_dpd.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_dpd.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_dpd.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_dpd.Feature_OID and BATCHID = iBatchID;

    END LOOP;

-- Downstream line features for all Dynamic Protective Devices ---

-- Downstream line features for all Fault Indicator ---
--Input - CircuitID

      FOR CUR_dwn_fi in (select (select name from sde.gdb_items WHERE objectid= a.to_feature_fcid ) as Feature_Class,
      a.to_feature_oid as Feature_OID,a.to_feature_globalid as Feature_GUID,
      (select name from sde.gdb_items WHERE objectid= b.to_feature_fcid ) as DownStream_Feature_Class,
      b.to_feature_oid as DownStream_Feature_OID,b.to_feature_globalid as DownStream_Feature_GUID 
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=1611038
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FAULTINDICATOR') --1004
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      and a.to_feature_oid not in 
      (
      -- Excluding features which are showing 2 downstream features_its a valid scenario
      -- Exculding these because no open device can have 2 downstream and our validation is for open devices
      select a.to_feature_oid
      from pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE a
      join pgeigpdata.PGE_FEEDERFEDNETWORK_TRACE b
      on a.to_feature_eid= b.from_feature_eid
      where a.feederid=sCircuitId 
      --and a.to_feature_oid=1611038
      and a.to_feature_fcid=(select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.FAULTINDICATOR') --1004
      and b.to_feature_fcid in 
      (
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.DISTBUSBAR' -- 1019
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIOHCONDUCTOR' -- 1023
      union
      select MIN(objectid) from sde.gdb_items WHERE upper(name) = 'EDGIS.PRIUGCONDUCTOR' --1021
      )
      group by a.to_feature_oid having count(*) >1
      ))                 
      LOOP
      Update PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA 
      set DOWNSTREAM_FEATURE_CLASS = CUR_dwn_fi.DownStream_Feature_Class, 
        DOWNSTREAM_FEATURE_OID= CUR_dwn_fi.DownStream_Feature_OID,
        DOWNSTREAM_FEATURE_GUID=CUR_dwn_fi.DownStream_Feature_GUID 
        where FEATURE_OID= CUR_dwn_fi.Feature_OID and BATCHID = iBatchID;

      END LOOP;
-- Downstream line features for all Fault Indicator ---
   
  
commit;


END PGE_IGP_DMS_UPDATE ;


/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_DMS_FC_ATTR_UPDATE
--------------------------------------------------------
set define off;
create or replace PROCEDURE PGE_IGP_DMS_FC_ATTR_UPDATE (iBatchID in varchar2,sCircuitId in varchar2,sUnprocessedTableName in varchar2,versionFullName in varchar2) AS 
BEGIN   
    
    -- Make all Null First 
    UPDATE PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA SET FEATURE_CURRENT_PHASE = NULL,NORMALPOSITION_A=NULL,NORMALPOSITION_B=NULL,
    NORMALPOSITION_C=NULL,UPSTREAM_FEATURE_PHASE=NULL,DOWNSTREAM_FEATURE_PHASE=NULL,NORMALPHASE=NULL WHERE BATCHID=iBatchID;
    commit;
    
    -- Set Version to required version
      sde.version_util.set_current_version(versionFullName);   
    
    --All DMS Point Features - Transformers,OpenPoint,Switch,Fuse,DynamicProtectiveDevice,FaultIndicator,StepDown,VoltageRegulator
        
    -- Updating current phase for Transformers
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_transformer
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_transformer.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_transformer.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_transformer.globalid;    
    commit;
    
    -- Updating current phase for Open Point
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_openpoint
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_openpoint.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_openpoint.phasedesignation),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_a = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_openpoint.normalposition_a),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_b = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_openpoint.normalposition_b),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_c = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_openpoint.normalposition_c)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_openpoint.globalid;    
    commit;
    
    -- Updating current phase for Switch
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_switch
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_switch.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_switch.phasedesignation),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_a = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_switch.normalposition_a),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_b = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_switch.normalposition_b),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_c = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_switch.normalposition_c)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_switch.globalid;    
    commit; 
    
    -- Updating current phase for Fuse
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_fuse
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_fuse.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_fuse.phasedesignation),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_a = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_fuse.normalposition_a),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_b = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_fuse.normalposition_b),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_c = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_fuse.normalposition_c)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_fuse.globalid;
    commit;
    
    -- Updating current phase for Dynamic ProtectiveDevice
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_dynamicprotectivedevice
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_dynamicprotectivedevice.phasedesignation),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_a = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_dynamicprotectivedevice.normalposition_a),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_b = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_dynamicprotectivedevice.normalposition_b),
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.normalposition_c = PGEIGPDATA.IGP_GET_NORMAL_STATUS(edgis.zz_mv_dynamicprotectivedevice.normalposition_c)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid;
    commit;
    
    -- Updating current phase for Fault Indicator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_faultindicator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_faultindicator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_faultindicator.phasedesignation)   
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_faultindicator.globalid;
    commit;
    
    
    -- Updating current phase for Step down
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_stepdown
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_stepdown.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_stepdown.phasedesignation)   
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_stepdown.globalid;
    commit;    
    
    -- Updating current phase for Voltage Regulator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_voltageregulator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_voltageregulator.phasedesignation)   
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid;
    commit;
    
    --All DMS Line Features - PriOHConductor, PriUGConductor,Busbar   
    
    -- Updating current phase for Pri OH conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priohconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_priohconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priohconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_priohconductor.globalid;
    commit;  
    
    -- Updating current phase for Pri UG conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priugconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_priugconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priugconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_priugconductor.globalid;
    commit;
    
    -- Updating current phase for Distribution Busbar
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_distbusbar
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_distbusbar.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_CURRENT_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_distbusbar.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.FEATURE_GUID = edgis.zz_mv_distbusbar.globalid;
    commit;    
    
    -- Update current phase for any other feature class that is left 
    
    --All DMS Point Features - Transformers,OpenPoint,Switch,Fuse,DynamicProtectiveDevice,FaultIndicator,StepDown,VoltageRegulator    
        
    -- Updating upstream feature phase for Transformers
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_transformer
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_transformer.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_transformer.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_transformer.globalid;    
    commit;    
    
    -- Updating upstream feature phase for Open Point
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_openpoint
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_openpoint.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_openpoint.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_openpoint.globalid;
    commit;
    
    -- Updating upstream feature phase for Switch
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_switch
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_switch.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_switch.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_switch.globalid;
    commit;  
    
    -- Updating upstream feature phase for Fuse
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_fuse
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_fuse.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_fuse.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_fuse.globalid;
    commit;
    
    -- Updating upstream feature phase for Dynamic ProtectiveDevice
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_dynamicprotectivedevice
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_dynamicprotectivedevice.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid;
    commit;
    
    -- Updating upstream feature phase for Fault Indicator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_faultindicator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_faultindicator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_faultindicator.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_faultindicator.globalid;
    commit;
    
    -- Updating upstream feature phase for StepDown
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_stepdown
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_stepdown.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_stepdown.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_stepdown.globalid;
    commit;
    
    -- Updating upstream feature phase for Voltage Regulator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_voltageregulator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_voltageregulator.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid;
    commit;
    
    --All DMS Line Features - PriOHConductor, PriUGConductor,Busbar   
    
    -- Updating upstream feature phase for Pri OH conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priohconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_priohconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priohconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_priohconductor.globalid;
    commit;
    
    -- Updating upstream feature phase for Pri UG conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priugconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_priugconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priugconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_priugconductor.globalid;
    commit;
    
    -- Updating upstream feature phase for Distribution Busbar
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_distbusbar
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_distbusbar.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_distbusbar.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.zz_mv_distbusbar.globalid; 
    commit;    
     -- Updating upstream feature phase for ZZ_MV_SECUGCONDUCTOR 
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using EDGIS.ZZ_MV_SECUGCONDUCTOR
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECUGCONDUCTOR.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_SECUGCONDUCTOR.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECUGCONDUCTOR.globalid; 
    commit;
     -- Updating upstream feature phase for ZZ_MV_SECOHGCONDUCTOR 
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.ZZ_MV_SECOHCONDUCTOR
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECOHCONDUCTOR.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_SECOHCONDUCTOR.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.UPSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECOHCONDUCTOR.globalid; 
    commit;
    -- Update upstream feature phase for any other feature class that is left    
    
    --All DMS Point Features - Transformers,OpenPoint,Switch,Fuse,DynamicProtectiveDevice,FaultIndicator,StepDown,VoltageRegulator 
    
    -- Updating downstream feature phase for Transformers
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_transformer
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_transformer.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_transformer.phasedesignation)    
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID  and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_transformer.globalid;    
    commit;   
    
    -- Updating downstream feature phase for Open Point
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_openpoint
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_openpoint.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_openpoint.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_openpoint.globalid;
    commit;
    
    -- Updating downstream feature phase for Switch
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_switch
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_switch.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_switch.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_switch.globalid; 
    commit;  
    
    -- Updating downstream feature phase for Fuse
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_fuse
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_fuse.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_fuse.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_fuse.globalid;
    commit;
    
    -- Updating downstream feature phase for Dynamic ProtectiveDevice
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_dynamicprotectivedevice
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_dynamicprotectivedevice.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_dynamicprotectivedevice.globalid;
    commit;
    
    -- Updating downstream feature phase for FaultIndicator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_faultindicator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_faultindicator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_faultindicator.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_faultindicator.globalid;
    commit;
    
    -- Updating downstream feature phase for StepDown
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_stepdown
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_stepdown.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_stepdown.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_stepdown.globalid;
    commit;
    
    -- Updating downstream feature phase for VoltageRegulator
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_voltageregulator
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_voltageregulator.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_voltageregulator.globalid;
    commit;
    
    --All DMS Line Features - PriOHConductor, PriUGConductor,Busbar 
    
     -- Updating downstream feature phase for Pri OH conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priohconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_priohconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priohconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_priohconductor.globalid;
    commit;
    
    -- Updating downstream feature phase for Pri UG conductor
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_priugconductor
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_priugconductor.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_priugconductor.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_priugconductor.globalid;
    commit;
    
     -- Updating downstream feature phase for Distribution Busbar
    merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using edgis.zz_mv_distbusbar
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_distbusbar.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.zz_mv_distbusbar.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.zz_mv_distbusbar.globalid; 
    commit;
     -- Updating downstream feature phase for  SecUGConductor
      merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using EDGIS.ZZ_MV_SECUGCONDUCTOR
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECUGCONDUCTOR.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_SECUGCONDUCTOR.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECUGCONDUCTOR.globalid; 
    commit;
    
     -- Updating downstream feature phase for  SecUGConductor
      merge into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA using EDGIS.ZZ_MV_SECOHCONDUCTOR
    on (PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECOHCONDUCTOR.globalid)
    when matched then update set PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_PHASE = PGEIGPDATA.IGP_GET_PHASE_FROMCODE(edgis.ZZ_MV_SECOHCONDUCTOR.phasedesignation)
    where PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.batchid=iBatchID   and
    PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA.DOWNSTREAM_FEATURE_GUID = edgis.ZZ_MV_SECOHCONDUCTOR.globalid; 
    commit;
    -- Update downstream feature phase for any other feature class that is left   
    
    -- Updating column NORMALPHASE for OpenPoint,Switch,Fuse,DynamicProtectiveDevice
    UPDATE PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA SET    
    NORMALPHASE=decode(normalposition_a,'Not Applicable','', 'A') ||decode(normalposition_b,'Not Applicable','','B') ||decode(normalposition_c,'Not Applicable','','C') 
    WHERE FEATURE_CLASS IN ('EDGIS.OPENPOINT','EDGIS.SWITCH','EDGIS.FUSE','EDGIS.DYNAMICPROTECTIVEDEVICE') and BATCHID=iBatchID;
    commit;
    
    -- Set Version to default version
       sde.version_util.set_current_version ('SDE.DEFAULT');

END PGE_IGP_DMS_FC_ATTR_UPDATE;



 


/
--------------------------------------------------------
--  DDL for Procedure PGE_IGP_CHECK_DMS_RULE
--------------------------------------------------------
set define off;

create or replace PROCEDURE PGE_IGP_CHECK_DMS_RULE (iBatchID in varchar2,p_ref  OUT sys_refcursor) AS  
    BEGIN
    OPEN p_ref FOR

--Rule 1 -Line to Line Phase Mismatch
select  'Line to Line Phase Mismatch',FEATURE_CLASS,FEATURE_OID,FEATURE_GUID ,FEATURE_CURRENT_PHASE,UPSTREAM_FEATURE_CLASS,UPSTREAM_FEATURE_OID,
UPSTREAM_FEATURE_GUID  ,upstream_feature_phase, DOWNSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE 
from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where feature_class in ('EDGIS.DISTBUSBAR','EDGIS.PRIOHCONDUCTOR','EDGIS.PRIUGCONDUCTOR') 
and batchid=iBatchID and (( UPSTREAM_FEATURE_PHASE='A' and  FEATURE_CURRENT_PHASE<>'A' ) or 
( UPSTREAM_FEATURE_PHASE='B' and  FEATURE_CURRENT_PHASE<>'B' ) or ( UPSTREAM_FEATURE_PHASE='C' and  FEATURE_CURRENT_PHASE<>'C' )  or 
( UPSTREAM_FEATURE_PHASE='AC' and  FEATURE_CURRENT_PHASE not in('A','C','AC')) or  
( UPSTREAM_FEATURE_PHASE='BC' and  FEATURE_CURRENT_PHASE not in('B','C','BC')) or  
( UPSTREAM_FEATURE_PHASE='AB' and  FEATURE_CURRENT_PHASE not in('A','B','AB')))
AND (feature_current_phase is not null And upstream_feature_phase is not null)
union
--Rule 2 -Line to Device Phase Mismatch
select 'Line to Device Phase Mismatch',FEATURE_CLASS,FEATURE_OID,FEATURE_GUID ,FEATURE_CURRENT_PHASE,UPSTREAM_FEATURE_CLASS,UPSTREAM_FEATURE_OID,
UPSTREAM_FEATURE_GUID , UPSTREAM_FEATURE_PHASE, DOWNSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE
from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where (feature_class in ('EDGIS.SWITCH','EDGIS.FUSE','EDGIS.OPENPOINT',
'EDGIS.DYNAMICPROTECTIVEDEVICE','EDGIS.STEPDOWN','EDGIS.VOLTAGEREGULATOR') and batchid=iBatchID)  and 
(( UPSTREAM_FEATURE_PHASE='A' and  FEATURE_CURRENT_PHASE<>'A' ) or ( UPSTREAM_FEATURE_PHASE='B' and  FEATURE_CURRENT_PHASE<>'B' ) or 
( UPSTREAM_FEATURE_PHASE='C' and  FEATURE_CURRENT_PHASE<>'C' )  or 
( UPSTREAM_FEATURE_PHASE='AC' and  FEATURE_CURRENT_PHASE not in('A','C','AC')) or 
( UPSTREAM_FEATURE_PHASE='BC' and  FEATURE_CURRENT_PHASE not in('B','C','BC')) or 
( UPSTREAM_FEATURE_PHASE='AB' and  FEATURE_CURRENT_PHASE not in('A','B','AB'))) 
union
--Rule 3 -Normal State Phase Mismatch of Devices
select 'Normal State Phase Mismatch of Devices',FEATURE_CLASS,FEATURE_OID,FEATURE_GUID ,FEATURE_CURRENT_PHASE,UPSTREAM_FEATURE_CLASS,UPSTREAM_FEATURE_OID,
UPSTREAM_FEATURE_GUID , UPSTREAM_FEATURE_PHASE, DOWNSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE
from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where  batchid=iBatchID
and feature_class in ('EDGIS.FUSE', 'EDGIS.DYNAMICPROTECTIVEDEVICE', 'EDGIS.SWITCH', 'EDGIS.OPENPOINT')
AND NORMALPHASE IS NOT NULL and feature_current_phase is not null
AND ((NORMALPHASE <> 'AC' AND instr(feature_current_phase, NORMALPHASE) = 0 )
OR (NORMALPHASE =  'AC' AND feature_current_phase NOT IN ('ABC','AC')))
union
--Rule 4-Phase Mismatch Across Normal Open Devices
select 'Phase Mismatch Across Normal Open Devices',FEATURE_CLASS,FEATURE_OID,FEATURE_GUID ,FEATURE_CURRENT_PHASE,UPSTREAM_FEATURE_CLASS,UPSTREAM_FEATURE_OID,
UPSTREAM_FEATURE_GUID , UPSTREAM_FEATURE_PHASE, DOWNSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE
from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where  batchid=iBatchID
AND feature_class in ('EDGIS.DYNAMICPROTECTIVEDEVICE','EDGIS.SWITCH','EDGIS.OPENPOINT')
AND (upstream_feature_phase is not null and downstream_feature_phase is not null and 
normalposition_a is not null and normalposition_b is not null and normalposition_c is not null)
AND (upstream_feature_phase <> downstream_feature_phase)
AND (normalposition_a <> 'Closed')
AND (normalposition_b <> 'Closed')
AND (normalposition_c <> 'Closed')
union 
--Rule 5 -Transformer Phase Mismatch
select 'Transformer Phase Mismatch',FEATURE_CLASS,FEATURE_OID,FEATURE_GUID ,FEATURE_CURRENT_PHASE,UPSTREAM_FEATURE_CLASS,UPSTREAM_FEATURE_OID,
UPSTREAM_FEATURE_GUID , UPSTREAM_FEATURE_PHASE, DOWNSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE
from PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA where (feature_class='EDGIS.TRANSFORMER' and batchid=iBatchID)   
AND feature_current_phase is not null And upstream_feature_phase is not null
AND ( (FEATURE_CURRENT_PHASE   <> 'AC'
AND instr(UPSTREAM_FEATURE_PHASE, FEATURE_CURRENT_PHASE) = 0 )
OR (FEATURE_CURRENT_PHASE  = 'AC'
AND UPSTREAM_FEATURE_PHASE NOT IN ('ABC','AC') ) );

END PGE_IGP_CHECK_DMS_RULE;


/
grant all on PGEIGPDATA.PGE_IGP_CHECK_GUID_OCCURENCE to IGPEDITOR ;
grant all on PGEIGPDATA.PGE_IGP_CHECK_SUBTYPE to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_UPDATECONDUCTORINFO to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_UPDATE_UNPROCESS_PHASE to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_UPDATE_OPENPOINT to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_DMS_UPDATE to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_DMS_FC_ATTR_UPDATE to IGPEDITOR;
grant all on PGEIGPDATA.PGE_IGP_CHECK_DMS_RULE to IGPEDITOR;
commit;

select current_timestamp from dual;
spool off