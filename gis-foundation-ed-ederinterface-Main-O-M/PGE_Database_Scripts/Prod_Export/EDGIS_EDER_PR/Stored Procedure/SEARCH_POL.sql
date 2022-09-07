--------------------------------------------------------
--  DDL for Procedure SEARCH_POL
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_POL" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_POLPOLYGON_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000);  
  v_view_name varchar2(255);
  v_query      varchar2(30000);
  v_operator_query varchar2(255);
  v_division_query varchar2(255);  
  v_lookup_query varchar2(255);
 
BEGIN

    --initializing operator type according to the input value    
    if P_OPERATORS = 'LIKE' then
      v_operator_query := 'like upper(''%' || P_SEARCHTEXT || '%'')';
    else
      v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
    end if;
    
    --initializing division type according to the input value
    if (UPPER(P_DIVISION) = 'ALL' OR P_DIVISION = '0' OR P_DIVISION IS NULL OR P_DIVISION = '') then
      v_division_query := '';      
    else
      v_division_query := 'and DIVISION  =' || P_DIVISION || '';      
    end if;
        
    v_table_name := 'POLPOLYGON';  
    v_view_name := 'POLPOLYGON';
    v_query_type := 'POL';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;     
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where DESCRIPTION ' || v_operator_query || ' FETCH FIRST 500 ROWS ONLY';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    dbms_output.put_line(v_query);
    open P_POLPOLYGON_CURSOR for v_query;  /* query executed */  
           
    v_query := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''POL'' order by CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;
END;
