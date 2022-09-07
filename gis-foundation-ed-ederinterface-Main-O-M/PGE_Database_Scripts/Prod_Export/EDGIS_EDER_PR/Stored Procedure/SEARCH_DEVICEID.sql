--------------------------------------------------------
--  DDL for Procedure SEARCH_DEVICEID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_DEVICEID" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_DPDC_CURSOR OUT SYS_REFCURSOR, P_DPDO_CURSOR OUT SYS_REFCURSOR, P_SC_CURSOR OUT SYS_REFCURSOR, P_SO_CURSOR OUT SYS_REFCURSOR, P_FC_CURSOR OUT SYS_REFCURSOR, P_FO_CURSOR OUT SYS_REFCURSOR, P_OP_CURSOR OUT SYS_REFCURSOR, P_TIE_CURSOR OUT SYS_REFCURSOR, P_CAP_CURSOR OUT SYS_REFCURSOR, P_VR_CURSOR OUT SYS_REFCURSOR, P_SD_CURSOR OUT SYS_REFCURSOR, P_TR_CURSOR OUT SYS_REFCURSOR, P_DC_CURSOR OUT SYS_REFCURSOR, P_SMND_CURSOR OUT SYS_REFCURSOR, P_FI_CURSOR OUT SYS_REFCURSOR, P_LC_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
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
    v_query_type := 'SAPID';
    
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

    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE'; 
    v_view_name := 'DYNAMICPROTECTIVEDEVICE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';        
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',',');
    open P_DPDC_CURSOR for v_query;  /* query executed */       
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPDO_CURSOR for v_query;  /* query executed */  
    

    v_table_name := 'SWITCH'; 
    v_view_name := 'SWITCH';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m  where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',',');
    open P_SC_CURSOR for v_query;  /* query executed */  
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SO_CURSOR for v_query;  /* query executed */  


    v_table_name := 'FUSE'; 
    v_view_name := 'FUSE';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',',');
    open P_FC_CURSOR for v_query;  /* query executed */  
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FO_CURSOR for v_query;  /* query executed */ 


    v_table_name := 'OPENPOINT';   
    v_view_name := 'OPENPOINT';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0 AND SUBTYPECD <> 11' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_OP_CURSOR for v_query;  /* query executed */  


    v_table_name := 'TIE';  
    v_view_name := 'TIE';
    v_query_type := 'WIPSR';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TIE_CURSOR for v_query;  /* query executed */ 


    v_table_name := 'CAPACITORBANK'; 
    v_view_name := 'CAPACITORBANK';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_CAP_CURSOR for v_query;  /* query executed */  


    v_table_name := 'VOLTAGEREGULATOR'; 
    v_view_name := 'VOLTAGEREGULATOR';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_VR_CURSOR for v_query;  /* query executed */  


    v_table_name := 'STEPDOWN';   
    v_view_name := 'STEPDOWN';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SD_CURSOR for v_query;  /* query executed */  


    v_table_name := 'TRANSFORMER';
    v_view_name := 'TRANSFORMER';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TR_CURSOR for v_query;  /* query executed */  


    v_table_name := 'DCRECTIFIER';
    v_view_name := 'DCRECTIFIER';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DC_CURSOR for v_query;  /* query executed */  


    v_table_name := 'SMARTMETERNETWORKDEVICE';   
    v_view_name := 'SMARTMETERNETWORKDEVICE';
    v_query_type := 'WIPSR';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SMND_CURSOR for v_query;  /* query executed */  


    v_table_name := 'FAULTINDICATOR';  
    v_view_name := 'FAULTINDICATOR';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FI_CURSOR for v_query;  /* query executed */  


    v_table_name := 'LOADCHECKPOINT';
    v_view_name := 'LOADCHECKPOINT';
    v_query_type := 'OPERATINGNUMBER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    if REGEXP_LIKE(P_SEARCHTEXT, '^[[:digit:]]+$') then
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID ' || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query;
    else
      v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_view_name || ' m where CEDSADEVICEID = 123456789';
    end if;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_LC_CURSOR for v_query;  /* query executed */  


    v_query := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''DEVICEID'' order by CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;

END;
