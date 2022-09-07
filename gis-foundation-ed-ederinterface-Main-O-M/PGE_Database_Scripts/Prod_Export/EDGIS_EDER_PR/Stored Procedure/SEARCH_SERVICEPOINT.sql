--------------------------------------------------------
--  DDL for Procedure SEARCH_SERVICEPOINT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_SERVICEPOINT" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_SERVICEPOINT_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000); 
  v_query1      varchar2(30000);    
  v_query2      varchar2(30000);    
  v_division_query varchar2(255);      
  v_lookup_query varchar2(255);
  v_operator_query varchar2(255);

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
      v_division_query := ' and DIVISION  =' || P_DIVISION || '';      
    end if;
	
	  v_table_name := 'SERVICELOCATION';           
    v_query_type := 'SERVICEPOINT';
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query1 := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where SERVICEPOINTID ' || v_operator_query || ' and rownum<=500 ' || v_division_query;
    v_query2 :=  'select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where GLOBALID IN (select SERVICELOCATIONGUID from EDGIS.ServicePoint where SERVICEPOINTID ' || v_operator_query || ') and rownum<=500 ' || v_division_query; 
    v_query1 := REPLACE(v_query1,'$COMMA$',','); 
    v_query2 := REPLACE(v_query2,'$COMMA$',',');
    open P_SERVICEPOINT_CURSOR for 'SELECT * FROM (' || v_query1 || ' UNION '  || v_query2 || ') where rownum<=500'; 
    
    v_query1 := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''' || v_query_type || ''' order by CURSEQ';        
    open P_ALL_LAYERS_CURSOR for v_query1;
END;
