--------------------------------------------------------
--  DDL for Procedure SEARCH_SAPID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_SAPID" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_CAPBANK_CURSOR OUT SYS_REFCURSOR,
                                                                                                                            P_DEVGROUP_CURSOR OUT SYS_REFCURSOR, 
																															P_DEVGROUPNOT_CURSOR OUT SYS_REFCURSOR,                                                                                                                            
																															P_DPDCLOSE_CURSOR OUT SYS_REFCURSOR,
                                                                                                                            P_DPDOPEN_CURSOR OUT SYS_REFCURSOR,
																															P_FAULT_CURSOR OUT SYS_REFCURSOR, 
																															P_PADMOUNT_CURSOR OUT SYS_REFCURSOR, 
																															P_STREET_CURSOR OUT SYS_REFCURSOR, 
																															P_SUB_CURSOR OUT SYS_REFCURSOR, 
																															P_SUPP_CURSOR OUT SYS_REFCURSOR, 
																															P_SWITCHCLO_CURSOR OUT SYS_REFCURSOR, 
																															P_SWITCHOP_CURSOR OUT SYS_REFCURSOR, 
																															P_ENC_CURSOR OUT SYS_REFCURSOR,
                                                                                                                            P_TRANSFORMER_CURSOR OUT SYS_REFCURSOR,
                                                                                                                            P_VR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                            P_SD_CURSOR OUT SYS_REFCURSOR,																															
																															P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  
  
  v_operator_query  varchar2(255);
  v_division_query  varchar2(255);
  v_query   varchar2(32767);  
  v_query1   varchar2(32767);
  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000);      
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
      v_division_query := ' and DIVISION  =' || P_DIVISION || '';      
    end if;    
    
    v_table_name := 'CAPACITORBANK'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from CAPACITORBANK m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and rownum<500';                 
    v_query1 :=  'select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','n') || ' from CAPACITORBANK n where GLOBALID IN (select DEVICEGUID from controller where upper(SAPEQUIPID) '|| v_operator_query  || ' ) and rownum<=500';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    v_query1 := REPLACE(v_query1,'$COMMA$',','); 
    open P_CAPBANK_CURSOR for 'SELECT * FROM (' || v_query || ' UNION '  || v_query1 || ') where rownum<=500'; 

    v_table_name := 'DEVICEGROUP'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DEVICEGROUP m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query ||  ' and SUBTYPECD = 3 and DEVICEGROUPTYPE in (1, 2, 3, 4, 5, 6, 29, 30, 33, 16, 17, 18, 19, 20, 21, 22, 23, 32,36,47) and (Status IN (5,30,1,2,3) or SUBTYPECD = 2) and Status IN (5,30,1,2,3) FETCH FIRST 300 ROWS ONLY';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DEVGROUP_CURSOR for v_query;  /* query executed */ 
        
    v_table_name := 'DEVICEGROUP'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';    
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DEVICEGROUP m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and SUBTYPECD = 3 and DEVICEGROUPTYPE in (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,24,25,26,27,28,29,30,31,34,35,37,38,39,40,41,42,43,44,45,46) and (Status IN (5,30,1,2,3) or SUBTYPECD = 2) and DEVICEGROUPTYPE in (7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,31,32,34,36) and (Status IN (5,30,1,2,3) or subtypecd = 1) and Status IN (5,30,1,2,3) FETCH FIRST 300 ROWS ONLY';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DEVGROUPNOT_CURSOR for v_query;  /* query executed */ 
    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';     
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;  
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';        
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DYNAMICPROTECTIVEDEVICE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';     
    v_query1:= 'select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','n') || 'from DYNAMICPROTECTIVEDEVICE n where GLOBALID IN (select DEVICEGUID from controller where SAPEQUIPID ' || v_operator_query  ||') and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';  /* build query string */                
    v_query := REPLACE(v_query,'$COMMA$',','); 
    v_query1 := REPLACE(v_query1,'$COMMA$',','); 
    open P_DPDCLOSE_CURSOR for 'SELECT * FROM (' || v_query || ' UNION '  || v_query1 || ') where rownum<=500';   
    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';     
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;  
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';        
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  DYNAMICPROTECTIVEDEVICE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';     
    v_query1:= 'select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','n') || 'from DYNAMICPROTECTIVEDEVICE n where GLOBALID IN (select DEVICEGUID from controller where SAPEQUIPID ' || v_operator_query  ||') and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';  /* build query string */                
    v_query := REPLACE(v_query,'$COMMA$',','); 
    v_query1 := REPLACE(v_query1,'$COMMA$',','); 
    open P_DPDOPEN_CURSOR for 'SELECT * FROM (' || v_query || ' UNION '  || v_query1 || ') where rownum<=500';

    v_table_name := 'FAULTINDICATOR'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';        
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  FAULTINDICATOR m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query  || ' and STATUS > 0 and rownum<=300';  /* build query string */
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FAULT_CURSOR for v_query;  /* query executed */ 

    v_table_name := 'PADMOUNTSTRUCTURE'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';            
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PADMOUNTSTRUCTURE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS > 0 and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PADMOUNT_CURSOR for v_query;  /* query executed */     
        
    
    v_table_name := 'STREETLIGHT'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  STREETLIGHT m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS > 0 and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_STREET_CURSOR for v_query;  /* query executed */     
    
        
    v_table_name := 'SUBSURFACESTRUCTURE'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from SUBSURFACESTRUCTURE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS > 0 and (SUBTYPECD = 2 OR SUBTYPECD = 3) and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SUB_CURSOR for v_query;  /* query executed */         
    
    
    v_table_name := 'SUPPORTSTRUCTURE'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SUPPORTSTRUCTURE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS IN (5,30,1,3) AND SYMBOLNUMBER > 73 and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SUPP_CURSOR for v_query;  /* query executed */         
    
    
    v_table_name := 'SWITCH'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SWITCH m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCHCLO_CURSOR for v_query;  /* query executed */     
        
    
    v_table_name := 'SWITCH'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SWITCH m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCHOP_CURSOR for v_query;  /* query executed */ 
    
        
    v_table_name := 'SUBSURFACESTRUCTURE'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2; 
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  SUBSURFACESTRUCTURE m where upper(SAPEQUIPID) '|| v_operator_query  || v_division_query || ' and (SUBTYPECD = 4 OR SUBTYPECD = 5 OR SUBTYPECD = 6 OR SUBTYPECD = 7) and status > 0 and rownum<=300';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_ENC_CURSOR for v_query;  /* query executed */ 
    
    v_table_name := 'TRANSFORMER'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where globalid in (select transformerguid from transformerunit where upper(SAPEQUIPID) '|| v_operator_query  || ')' || v_division_query || ' and Status > 0 and rownum <=300 ORDER BY OBJECTID';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TRANSFORMER_CURSOR for v_query; 
    
    v_table_name := 'VOLTAGEREGULATOR'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where globalid in (select REGULATORGUID from VoltageRegulatorUnit where upper(SAPEQUIPID) '|| v_operator_query  || ')' || v_division_query || ' and Status > 0 and rownum <=300 ORDER BY OBJECTID';  /* build query string */        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_VR_CURSOR for v_query;
    
    v_table_name := 'STEPDOWN'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where globalid in (select STEPDOWNGUID from StepDownUnit where upper(SAPEQUIPID) '|| v_operator_query  || ')' || v_division_query || ' and Status > 0 and rownum <=300 ORDER BY OBJECTID';  /* build query string */            
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SD_CURSOR for v_query;
              
    v_query:= 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''SAPID'' order by CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;

END;
