--------------------------------------------------------
--  DDL for Procedure SEARCH_TRANSFORMER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_TRANSFORMER" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_TRANSFORMER_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  v_search_text VARCHAR2(255);
  v_table_name VARCHAR2(255);  
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(30000);    
  v_division_query varchar2(255);      
  v_lookup_query varchar2(255);
  v_operator_query varchar2(255);

BEGIN

    v_search_text := REPLACE(P_SEARCHTEXT,' ','%');
    v_operator_query := 'like upper(''%' || v_search_text || '%'')';
    --initializing operator type according to the input value
--    if P_OPERATORS = 'LIKE' then
--      v_operator_query := 'like upper(''%' || P_SEARCHTEXT || '%'')';
--    else
--      v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
--    end if;
    
    --initializing division type according to the input value
    if (UPPER(P_DIVISION) = 'ALL' OR P_DIVISION = '0' OR P_DIVISION IS NULL OR P_DIVISION = '') then
      v_division_query := '';      
    else
      v_division_query := ' and DIVISION  =' || P_DIVISION || '';      
    end if;
    
    
    v_table_name := 'TRANSFORMER';     
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_table_name and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from TRANSFORMER m where m.GLOBALID in (select TRANSFORMERGUID from EDGIS.T_CUSTADD where METERNUMBER ' || v_operator_query ||  ' OR SPADDRESS ' || v_operator_query || ' OR TXADDRESS ' || v_operator_query || ' OR SERVICEPOINTID ' || v_operator_query || ') and rownum<=300 ' || v_division_query;    
    --dbms_output.put_line(v_query);
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TRANSFORMER_CURSOR for v_query;  /* query executed */    
    
    v_query := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE = ''TRANSFORMER'' order by CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;
END;
