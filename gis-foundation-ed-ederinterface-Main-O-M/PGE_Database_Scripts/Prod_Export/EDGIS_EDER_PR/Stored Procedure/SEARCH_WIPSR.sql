--------------------------------------------------------
--  DDL for Procedure SEARCH_WIPSR
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_WIPSR" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_JOBHISTORYNOTE_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_TIE_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_CAPACITORBANK_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_VOLTAGEREGULATOR_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_DPD_CL_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_DPD_OP_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_SWITCH_CL_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_SWITCH_OP_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_FUSE_CL_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_FUSE_OP_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_STEPDOWN_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_TRANSFORMER_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_STREETLIGHT_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_DCRECTIFIER_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_SMND_CURSOR OUT SYS_REFCURSOR, 
                                                                                                                        P_FAULTINDICATOR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_LOADCHECKPOINT_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_PRIOHCONDUCTOR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_PRIUGCONDUCTOR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_NEUTRALCONDUCTOR_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_OPENPOINT_CURSOR OUT SYS_REFCURSOR,
                                                                                                                        P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)

AS

  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(32727);    
  v_division_query varchar2(255);      
  v_lookup_query varchar2(255);
  v_operator_query varchar2(255);
  v_job_number  varchar2(100);



BEGIN
    --global sql statements
    v_query_type := 'WIPSR';
    v_job_number:='INSTALLJOBNUMBER ';
    
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
    
    
    v_table_name := 'JOBHISTORYNOTE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type = v_query_type and table_name= v_table_name and rownum<2;     
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  '|| v_table_name ||' m where ' ||v_job_number || v_operator_query || ' and rownum<=300' ;        
    v_query := REPLACE(v_query,'$COMMA$',','); 
    --dbms_output.put_line(v_query);
    open P_JOBHISTORYNOTE_CURSOR for v_query;  /* query executed */  
                
    v_table_name := 'TIE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type = v_query_type and table_name= v_table_name and rownum<2;     
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  '|| v_table_name ||' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300' ;    
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TIE_CURSOR for v_query;  /* query executed */      
        
    v_table_name := 'CAPACITORBANK';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_CAPACITORBANK_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'VOLTAGEREGULATOR';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300 ';    
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_VOLTAGEREGULATOR_CURSOR for v_query;  /* query executed */
    
    v_table_name := 'DYNAMICPROTECTIVEDEVICE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';    
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPD_CL_CURSOR for v_query;  /* query executed */            
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DPD_OP_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'SWITCH';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCH_CL_CURSOR for v_query;  /* query executed */    
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SWITCH_OP_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'FUSE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 AND (NOT (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) or (NORMALPOSITION_A IS NULL OR NORMALPOSITION_B IS NULL OR NORMALPOSITION_C IS NULL)) and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FUSE_CL_CURSOR for v_query;  /* query executed */    
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS IN (5,30,1,2,3) AND (NORMALPOSITION_A = 0 OR NORMALPOSITION_B=0 OR NORMALPOSITION_C=0) and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FUSE_OP_CURSOR for v_query;  /* query executed */
            
    v_table_name := 'STEPDOWN'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';    
    v_query := REPLACE(v_query,'$COMMA$',',');  
    open P_STEPDOWN_CURSOR for v_query;  /* query executed */
		
    v_table_name := 'TRANSFORMER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TRANSFORMER_CURSOR for v_query;  /* query executed */    
		 
    v_table_name := 'STREETLIGHT'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_STREETLIGHT_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'DCRECTIFIER';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_DCRECTIFIER_CURSOR for v_query;  /* query executed */
		
    v_table_name := 'SMARTMETERNETWORKDEVICE';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SMND_CURSOR for v_query;  /* query executed */
        
    v_table_name := 'FAULTINDICATOR';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_FAULTINDICATOR_CURSOR for v_query;  /* query executed */

    v_table_name := 'LOADCHECKPOINT'; 
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_LOADCHECKPOINT_CURSOR for v_query;  /* query executed */
   	
    v_table_name := 'PRIOHCONDUCTOR';
	  select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRIOHCONDUCTOR_CURSOR for v_query;  /* query executed */    
    	
    v_table_name := 'PRIUGCONDUCTOR';        
	  select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRIUGCONDUCTOR_CURSOR for v_query;  /* query executed */
    	
    v_table_name := 'NEUTRALCONDUCTOR'; 
	  select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_NEUTRALCONDUCTOR_CURSOR for v_query;  /* query executed */
   	    
    v_table_name := 'OPENPOINT';    
	  select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where ' ||v_job_number || v_operator_query || v_division_query || ' and STATUS > 0 and rownum<=300';
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_OPENPOINT_CURSOR for v_query;  /* query executed */
    
	  v_query := 'select OBJCLS_NAME as "physicalName", LAYER_NAME as "layerName", displayfieldname as "displayFieldName", layerid as "layerId", database_name as "databaseName", LATESTWKID as "latestWkid", WKID as "wkid", GEOMETRYTYPE as "geometryType" from PGE_SEARCH_LAYERS_ALL where SEARCH_TYPE=''WIP'' ORDER BY CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;
END;
