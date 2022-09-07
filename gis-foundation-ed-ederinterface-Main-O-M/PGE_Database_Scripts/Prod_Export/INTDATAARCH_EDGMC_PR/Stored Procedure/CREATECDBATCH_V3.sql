--------------------------------------------------------
--  DDL for Procedure CREATECDBATCH_V3
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "INTDATAARCH"."CREATECDBATCH_V3" (v_tableName IN VARCHAR2,v_sourceTable IN VARCHAR2,v_fields IN VARCHAR2,v_tblNames IN VARCHAR2,v_startDate IN VARCHAR2,v_endDate IN VARCHAR2,v_whereClause IN VARCHAR2,v_size IN VARCHAR2)
  AS
  v_sqlQuery NVARCHAR2(10000);
  v_tablesRep NVARCHAR2(2000);
  v_whrCls  NVARCHAR2(2000);
  vfeat NVARCHAR2(100);
  TYPE T_Ref_Cur IS  REF CURSOR;
  cv T_Ref_Cur;
  vCounter number;
  vPf number;
  begin
  --DBMS_OUTPUT.PUT_LINE('leSDn' || to_char(length(v_startDate)));
  --DBMS_OUTPUT.PUT_LINE('leEDn' || to_char(nvl(length(v_endDate),0)));
  --v_tablesRep := replace(v_tblNames,'''','''''');
 -- v_whrCls := replace(v_whereClause,'''','''''');

  v_sqlQuery := 'Truncate table ' || v_tableName;
  --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
  execute immediate v_sqlQuery;
  commit;

  /*
  INSERT INTO CD_ED06_TEMP (FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_REPLACEGUID_OLD,FEAT_OID,FEAT_SAPEQIPID_OLD,FEAT_OPERATINGNO_OLD,CAPTURE_DATE,
OBJECTID,FEAT_FIELDS_LIST,FEAT_SHAPE_OLD,processflag)
(select FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_REPLACEGUID_OLD,FEAT_OID,FEAT_SAPEQIPID_OLD,FEAT_OPERATINGNO_OLD,CAPTURE_DATE,OBJECTID,
FEAT_FIELDS_LIST,FEAT_SHAPE_OLD,rownum processflag from(SELECT FEAT_GLOBALID,FEAT_CLASSNAME,ACTION,FEAT_REPLACEGUID_OLD,FEAT_OID,FEAT_SAPEQIPID_OLD,FEAT_OPERATINGNO_OLD,CAPTURE_DATE,OBJECTID,
FEAT_FIELDS_LIST,FEAT_SHAPE_OLD FROM INTDATAARCH.PGE_GDBM_AH_Info WHERE FEAT_GLOBALID IS NOT NULL 
AND to_date(CAPTURE_DATE,'MM-DD-YYYY HH24:MI:SS') >= to_date('01-27-2022 22:08:05','MM-DD-YYYY HH24:MI:SS') 
AND to_date(CAPTURE_DATE,'MM-DD-YYYY HH24:MI:SS') <= to_date('02-17-2022 02:39:55','MM-DD-YYYY HH24:MI:SS') 
AND ( (( USAGE IS NULL OR USAGE NOT LIKE 'NOCD') AND STATUS = 'C' AND FEAT_CLASSNAME NOT LIKE '%ANNO' ) ) 
ORDER by FEAT_CLASSNAME ASC));
  */

  v_sqlQuery := 'INSERT INTO ' || v_tableName || '(' || v_fields || ',processflag ) (';
  v_sqlQuery := v_sqlQuery || 'SELECT ' || v_fields || ',rownum processflag FROM ( ';
  v_sqlQuery := v_sqlQuery || 'SELECT ' || v_fields || ' FROM ' || v_sourceTable || ' WHERE FEAT_GLOBALID IS NOT NULL';

  --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
    if nvl(length(v_tblNames),0) > 0 then
    v_sqlQuery := v_sqlQuery ||' AND FEAT_CLASSNAME in ( ' || v_tblNames || ' )';
    end if;
    --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
    if nvl(length(v_whereClause),0) > 0 then
    v_sqlQuery := v_sqlQuery ||' AND ' || v_whereClause;
    end if;

if UPPER(v_tableName) LIKE UPPER('INTDATAARCH.CD_ED06_TEMP') THEN 

  if nvl(length(v_startDate),0) > 0 then
  v_sqlQuery := v_sqlQuery ||' AND (( to_date(CAPTURE_DATE,''MM-DD-YYYY HH24:MI:SS'') >= to_date(''' || v_startDate ||''',''MM-DD-YYYY HH24:MI:SS'')';
  end if;

 if nvl(length(v_endDate),0) > 0 then
  v_sqlQuery := v_sqlQuery ||' AND to_date(CAPTURE_DATE,''MM-DD-YYYY HH24:MI:SS'') <= to_date(''' || v_endDate ||''',''MM-DD-YYYY HH24:MI:SS'')';
  end if;

    v_sqlQuery := v_sqlQuery || ') OR ( ED06_Picked is null AND to_date(CAPTURE_DATE,''MM-DD-YYYY HH24:MI:SS'') <= to_date(''' || v_startDate || ''',''MM-DD-YYYY HH24:MI:SS'') ))';

ELSE 

  if nvl(length(v_startDate),0) > 0 then
  v_sqlQuery := v_sqlQuery ||' AND to_date(CAPTURE_DATE,''MM-DD-YYYY HH24:MI:SS'') >= to_date(''' || v_startDate ||''',''MM-DD-YYYY HH24:MI:SS'')';
  end if;

 if nvl(length(v_endDate),0) > 0 then
  v_sqlQuery := v_sqlQuery ||' AND to_date(CAPTURE_DATE,''MM-DD-YYYY HH24:MI:SS'') <= to_date(''' || v_endDate ||''',''MM-DD-YYYY HH24:MI:SS'')';
  end if;

  end if;   
   v_sqlQuery := v_sqlQuery || ' ORDER by CAPTURE_DATE ASC ))';

   --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);

 execute immediate v_sqlQuery;
 COMMIT;

 -- ED06 Data retenstion [START]
 if UPPER(v_tableName) LIKE UPPER('INTDATAARCH.CD_ED06_TEMP') THEN 

    --SP to retain Data
    PGE_INT_DATA_BACKUP('ED06_RETEN');
    -- Last RunDate Setting
    v_sqlQuery := 'Update INTDATAARCH.CD_ED06_TEMP_RETEN SET LASTRUNDATE = ''' || v_endDate || '''' || ' where LASTRUNDATE is null ';
    --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
    execute immediate v_sqlQuery;

    -- Start RunDate Setting
    v_sqlQuery := 'Update INTDATAARCH.CD_ED06_TEMP_RETEN SET STARTDATE = ''' || v_startDate || '''' || ' where STARTDATE is null ';
    --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
    execute immediate v_sqlQuery;

    v_sqlQuery := 'Update INTDATAARCH.PGE_GDBM_AH_INFO SET ED06_Picked = ''Y'' where (FEAT_CLASSNAME,FEAT_OID,CAPTURE_DATE) in (Select FEAT_CLASSNAME,FEAT_OID,CAPTURE_DATE from INTDATAARCH.CD_ED06_TEMP)';
    --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
    execute immediate v_sqlQuery;
    COMMIT;

 end if;   
 -- ED06 Data retenstion [END]

 --Delete not to process records
 v_sqlQuery := 'Delete From ' || v_tableName || ' Where FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''I'' and B.ACTION = ''D'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
 execute immediate v_sqlQuery;
 --COMMIT;

-- Delete Updates if insert is present
v_sqlQuery := 'Delete From ' || v_tableName || ' where ACTION = ''U'' AND FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''I'' and B.ACTION = ''U'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;
--COMMIT;

-- Delete Updates if Delete is present
v_sqlQuery := 'Delete From ' || v_tableName || ' Where ACTION = ''U'' AND FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''U'' and B.ACTION = ''D'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID)';
--DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;
--COMMIT;

-- Delete Updates if multiple Update is present
v_sqlQuery := 'Delete From ' || v_tableName || ' C where FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''U'' and B.ACTION = ''U'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID AND A.CAPTURE_DATE <> B.CAPTURE_DATE) AND CAPTURE_DATE > (Select MIN(CAPTURE_DATE) from ' || v_tableName || ' where FEAT_GLOBALID = C.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;
--COMMIT;

v_sqlQuery := 'Delete From ' || v_tableName || ' C where FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''I'' and B.ACTION = ''I'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID AND A.CAPTURE_DATE <> B.CAPTURE_DATE) AND CAPTURE_DATE > (Select MIN(CAPTURE_DATE) from ' || v_tableName || ' where FEAT_GLOBALID = C.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;
--COMMIT;

v_sqlQuery := 'Delete From ' || v_tableName || ' C where FEAT_GLOBALID IN (Select A.FEAT_GLOBALID from  ' || v_tableName || ' A, ' || v_tableName || ' B where A.ACTION = ''D'' and B.ACTION = ''D'' and A.FEAT_GLOBALID = B.FEAT_GLOBALID AND A.CAPTURE_DATE <> B.CAPTURE_DATE) AND CAPTURE_DATE > (Select MIN(CAPTURE_DATE) from ' || v_tableName || ' where FEAT_GLOBALID = C.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;


v_sqlQuery := 'DELETE FROM ' || v_tableName || ' WHERE rowid not in (SELECT MIN(rowid) FROM ' || v_tableName || ' GROUP BY FEAT_GLOBALID, CAPTURE_DATE)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;


--Update SUB
--v_sqlQuery := 'Update ' || v_tableName || ' SET SUB = ''SUB'' where FEAT_CLASSNAME like ''%.SUB%''';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
--execute immediate v_sqlQuery;

--v_sqlQuery := 'update ' || v_tableName || ' set (PROCESSFLAG) = (select batch_id from (select FEAT_GLOBALID,ceil(row_number() over (order by FEAT_GLOBALID desc)/' || v_size || ') as batch_id from ' || v_tableName || ') x where x.FEAT_GLOBALID = ' || v_tableName || '.FEAT_GLOBALID)';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
--execute immediate v_sqlQuery;

-- Create Batch of Records
v_sqlQuery := 'Update ' || v_tableName || ' SET processflag = ceil(processflag/' || v_size || ')';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;

v_sqlQuery := 'Update ' || v_tableName || ' SET LASTRUNDATE = ''' || v_endDate || '''';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;

v_sqlQuery := 'Update ' || v_tableName || ' SET RECORDSTATUS = ''N'' ';
 --DBMS_OUTPUT.PUT_LINE(v_sqlQuery);
execute immediate v_sqlQuery;

/*
vCounter := 0;
vPf := 1;
OPEN cv FOR 'SELECT FEAT_GLOBALID FROM ' || v_tableName || ' ORDER BY FEAT_CLASSNAME';
  LOOP
  FETCH cv INTO vfeat;
  execute immediate 'update ' || v_tableName || ' set PROCESSFLAG = ''' || to_char(vPf) || ''' where FEAT_GLOBALID = ''' || vfeat || '''';
  vCounter := vCounter +1;
  if vCounter >= v_size then
    vCounter := 0;
    vPf := vPf + 1;
  end if;
  end loop;
*/

COMMIT;

end;
