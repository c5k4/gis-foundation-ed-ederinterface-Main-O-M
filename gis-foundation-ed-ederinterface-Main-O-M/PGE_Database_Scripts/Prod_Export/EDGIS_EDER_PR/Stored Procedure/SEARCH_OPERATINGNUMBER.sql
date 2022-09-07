--------------------------------------------------------
--  DDL for Procedure SEARCH_OPERATINGNUMBER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_OPERATINGNUMBER" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2,P_TRANSFORMER_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                                        P_SWITCH_CLOSED_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_FUSE_CLOSED_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_SWITCH_OPEN_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_FUSE_OPEN_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_OPENPOINT_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_DPDCL_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                                        P_DPDOP_CURSOR OUT SYS_REFCURSOR,  
                                                                                                                                        P_CAPACITORBANK_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_VOLTAGEREGULATOR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_STEPDOWN_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_DCRECTIFIER_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_FAULTINDICATOR_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                                        P_LOADCHECKPOINT_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_NETWORKPRO_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_PROCAP_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_PRODPD_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_PROPM_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_PROVR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                                        P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(30000);    
  v_division_query varchar2(255);      
  v_lookup_query varchar2(255);
  v_operator_query varchar2(255);
  

BEGIN
    dbms_output.put_line(systimestamp);         
    v_query_type := 'OPERATINGNUMBER';

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

    v_table_name := 'TRANSFORMER';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  TRANSFORMER m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 ' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    dbms_output.put_line(v_query);
    open P_TRANSFORMER_CURSOR for v_query;  /* query executed */  
        
    v_table_name := 'SWITCH';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SWITCH m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) ' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCH_CLOSED_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'FUSE';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  FUSE m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) ' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FUSE_CLOSED_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'SWITCH';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SWITCH m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCH_OPEN_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'FUSE';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  FUSE m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) ' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FUSE_OPEN_CURSOR for v_query;  /* query executed */    
    
    v_table_name := 'OPENPOINT';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  OPENPOINT m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 ' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_OPENPOINT_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DYNAMICPROTECTIVEDEVICE m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL))' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPDCL_CURSOR for v_query;  /* query executed */    
    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DYNAMICPROTECTIVEDEVICE m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0)' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPDOP_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'CAPACITORBANK';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  CAPACITORBANK m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_CAPACITORBANK_CURSOR for v_query;  /* query executed */    
    
    v_table_name := 'VOLTAGEREGULATOR';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  VOLTAGEREGULATOR m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_VOLTAGEREGULATOR_CURSOR for v_query;  /* query executed */
      
    v_table_name := 'STEPDOWN';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  STEPDOWN m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_STEPDOWN_CURSOR for v_query;  /* query executed */

    v_table_name := 'DCRECTIFIER';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DCRECTIFIER m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DCRECTIFIER_CURSOR for v_query;  /* query executed */    

    v_table_name := 'FAULTINDICATOR';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  FAULTINDICATOR m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FAULTINDICATOR_CURSOR for v_query;  /* query executed */  
    
    v_table_name := 'LOADCHECKPOINT';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  LOADCHECKPOINT m where  upper(operatingnumber) '  || v_operator_query || ' and rownum<=500 and STATUS > 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_LOADCHECKPOINT_CURSOR for v_query;  /* query executed */
     
    v_table_name := 'NETWORKPROTECTOR';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  NETWORKPROTECTOR m where  upper(operatingnumber) '  || v_operator_query || 'and rownum<=500 and STATUS IN (5,30,1,2,3)' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_NETWORKPRO_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'CAPACITORBANK';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  CAPACITORBANK m where  upper(operatingnumber) '  || v_operator_query || 'and rownum<=500 and STATUS = 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PROCAP_CURSOR for v_query;  /* query executed */    
         
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DYNAMICPROTECTIVEDEVICE m where  upper(operatingnumber) '  || v_operator_query || 'and rownum<=500 and STATUS = 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRODPD_CURSOR for v_query;  /* query executed */  

    v_table_name := 'PRIMARYMETER';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where  upper(operatingnumber) '  || v_operator_query || 'and rownum<=500 and STATUS = 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PROPM_CURSOR for v_query;  /* query executed */  
    
    v_table_name := 'VOLTAGEREGULATOR';   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  VOLTAGEREGULATOR m where  upper(operatingnumber) '  || v_operator_query || 'and rownum<=500 and STATUS = 0' || v_division_query; /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PROVR_CURSOR for v_query;  /* query executed */  
        
    v_query:='select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''OPERATINGNUMBER'' order by CURSEQ';   
    open P_ALL_LAYERS_CURSOR for v_query;
    --dbms_output.put_line(systimestamp);

END;
