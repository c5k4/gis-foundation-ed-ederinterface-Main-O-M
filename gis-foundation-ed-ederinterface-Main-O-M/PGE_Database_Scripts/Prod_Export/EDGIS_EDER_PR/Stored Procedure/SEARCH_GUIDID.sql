--------------------------------------------------------
--  DDL for Procedure SEARCH_GUIDID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_GUIDID" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_SS_CURSOR OUT SYS_REFCURSOR, P_PRIOH_CURSOR OUT SYS_REFCURSOR, P_PRIUG_CURSOR OUT SYS_REFCURSOR, P_DPDC_CURSOR OUT SYS_REFCURSOR, P_DPDO_CURSOR OUT SYS_REFCURSOR, P_FC_CURSOR OUT SYS_REFCURSOR, P_FO_CURSOR OUT SYS_REFCURSOR, P_SC_CURSOR OUT SYS_REFCURSOR, P_SO_CURSOR OUT SYS_REFCURSOR, P_TRF_CURSOR OUT SYS_REFCURSOR, P_SSR_CURSOR OUT SYS_REFCURSOR, P_PDMS_CURSOR OUT SYS_REFCURSOR, P_ENC_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
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

    v_table_name := 'SUPPORTSTRUCTURE'; 
    v_view_name := 'SUPPORTSTRUCTURE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;    
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and Status IN (5,30,1,3) AND SYMBOLNUMBER > 73' || v_division_query;                  
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SS_CURSOR for v_query;  /* query executed */  

     
    v_table_name := 'PRIOHCONDUCTOR';    
    v_view_name := 'PRIOHCONDUCTOR';
    v_query_type := 'GLOBALID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;     
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and Status>0' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRIOH_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'PRIUGCONDUCTOR'; 
    v_view_name := 'PRIUGCONDUCTOR';
    v_query_type := 'GLOBALID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and Status>0' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRIUG_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE'; 
    v_view_name := 'DYNAMICPROTECTIVEDEVICE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPDC_CURSOR for v_query;  /* query executed */ 

        
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';    
    v_view_name := 'DYNAMICPROTECTIVEDEVICE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPDO_CURSOR for v_query;  /* query executed */ 

           
    v_table_name := 'FUSE'; 
    v_view_name := 'FUSE';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FC_CURSOR for v_query;  /* query executed */ 
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FO_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'SWITCH';  
    v_view_name := 'SWITCH';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SC_CURSOR for v_query;  /* query executed */ 
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SO_CURSOR for v_query;  /* query executed */ 


    v_table_name := 'TRANSFORMER'; 
    v_view_name := 'TRANSFORMER';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and STATUS > 0' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TRF_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'SUBSURFACESTRUCTURE'; 
    v_view_name := 'SUBSURFACESTRUCTURE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and status > 0 and (SUBTYPECD = 2 OR SUBTYPECD = 3)' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SSR_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'PADMOUNTSTRUCTURE';
    v_view_name := 'PADMOUNTSTRUCTURE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and status > 0' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PDMS_CURSOR for v_query;  /* query executed */ 

    
    v_table_name := 'SUBSURFACESTRUCTURE';  
    v_view_name := 'SUBSURFACESTRUCTURE';
    v_query_type := 'SAPID';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' || v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where GLOBALID ' || v_operator_query || ' and rownum<=1000 and (SUBTYPECD = 4 OR SUBTYPECD = 5 OR SUBTYPECD = 6 OR SUBTYPECD = 7) and status > 0' || v_division_query;              
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_ENC_CURSOR for v_query;  /* query executed */ 

    v_query := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''GUIDID'' order by CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;

END;
