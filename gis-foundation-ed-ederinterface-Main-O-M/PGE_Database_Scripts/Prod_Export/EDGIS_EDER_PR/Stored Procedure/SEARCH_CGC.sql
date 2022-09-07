--------------------------------------------------------
--  DDL for Procedure SEARCH_CGC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_CGC" (P_SEARCHTEXT IN VARCHAR2, P_DIVISION IN VARCHAR2, P_OPERATORS IN VARCHAR2, P_TRANSFORMER_CURSOR OUT SYS_REFCURSOR,  P_PRIMARYMETER_CURSOR OUT SYS_REFCURSOR, P_ALL_LAYERS_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255);  
  v_query_type VARCHAR2(255);  
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(30000);    
  v_division_query varchar2(255);      
  v_lookup_query varchar2(255);
  v_operator_query varchar2(255);
  v_textlength    number(30);  
  v_input_type varchar2(10);
  v_firstfour varchar2(50);
  v_lastfour varchar2(50); 

BEGIN

    --initializing operator type according to the v_input_type value
    if P_OPERATORS = 'LIKE' then
      v_operator_query := 'like upper(''%' || P_SEARCHTEXT || '%'')';
    else
      v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
    end if;
    
    --initializing division type according to the v_input_type value
    if (UPPER(P_DIVISION) = 'ALL' OR P_DIVISION = '0' OR P_DIVISION IS NULL OR P_DIVISION = '') then
      v_division_query := '';      
    else
      v_division_query := ' and DIVISION  =' || P_DIVISION || '';      
    end if;
    
    
    --Condition 1: cgc = '123456789012'---v_input_type - 123456789012
    v_table_name := 'TRANSFORMER';    
    v_query_type := 'CGC';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    SELECT LENGTH(P_SEARCHTEXT) Len INTO v_textlength FROM dual;      
    IF v_textlength = 12 then                
            v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  TRANSFORMER m where CGC12 '|| v_operator_query|| 'and rownum<=500 ' || v_division_query ;  /* build query string */                     
    ELSE    
    --Condition 2: cgc like '%1234%9012' ---v_input_type - 1234-9012
        v_input_type  := '-';
        SELECT INSTR(P_SEARCHTEXT,v_input_type) INTO v_textlength FROM DUAL;        
        --initializing operator type according to the v_input_type value
--        if P_OPERATORS = 'LIKE' then
--            v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
--        else
--            v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
--        end if;
        IF  v_textlength >0 then            
            select substr(P_SEARCHTEXT, 1, instr(P_SEARCHTEXT, '-') - 1), substr(P_SEARCHTEXT, instr(P_SEARCHTEXT, '-') + 1) INTO v_firstfour,v_lastfour from dual;
            if P_OPERATORS = 'LIKE' then
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  TRANSFORMER m where CGC12 '|| v_operator_query || ' and rownum<=500 ' || v_division_query ;  /* build query string */                 
            else  
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T1 AS (' ||v_lookup_query || ') select ' ||  SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T1','m') || ' from  TRANSFORMER m where CGC12 '|| v_operator_query ||' and rownum<=500 ' || v_division_query ;  /* build query string */             
            END IF;
        else        
            --Condition 3: cgc like '%1234%9012'----v_input_type - 12349012            
            select SUBSTR( P_SEARCHTEXT, 0, 4), SUBSTR( P_SEARCHTEXT, -4, 4) into v_firstfour, v_lastfour from dual;
            if P_OPERATORS = 'LIKE' then
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  TRANSFORMER m where CGC12 '|| v_operator_query || ' and rownum<=500 ' || v_division_query ;  /* build query string */                  
            else  
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  TRANSFORMER m where CGC12 '|| v_operator_query || ' and rownum<=500 ' || v_division_query ;  /* build query string */                 
            END IF;
        end if;
    End IF;
    v_query := REPLACE(v_query,'$COMMA$',','); 
    dbms_output.put_line(v_query);
    open P_TRANSFORMER_CURSOR for v_query;  /* query executed */  
    
    
    --For PRIMARYMETER feature class view            
    --initializing operator type according to the v_input_type value
    if P_OPERATORS = 'LIKE' then
      v_operator_query := 'like upper(''%' || P_SEARCHTEXT || '%'')';
    else
      v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
    end if;
    --Condition 1: cgc = '123456789012'---v_input_type - 123456789012
    v_table_name := 'PRIMARYMETER';    
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and table_name= v_table_name and rownum<2;  
    SELECT LENGTH(P_SEARCHTEXT) Len INTO v_textlength FROM dual;      
    IF v_textlength = 12 then                
            v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where CGC12 '|| v_operator_query||' and rownum<=500 ' || v_division_query ;  /* build query string */                                                  
    ELSE    
    --Condition 2: cgc like '%1234%9012' ---v_input_type - 1234-9012
        v_input_type  := '-';
        SELECT INSTR(P_SEARCHTEXT,v_input_type) INTO v_textlength FROM DUAL; 
        --initializing operator type according to the v_input_type value
--        if P_OPERATORS = 'LIKE' then
--            v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
--        else
--            v_operator_query := ' = upper(''' || P_SEARCHTEXT || ''')';
--        end if;
        IF  v_textlength >0 then            
            select substr(P_SEARCHTEXT, 1, instr(P_SEARCHTEXT, '-') - 1), substr(P_SEARCHTEXT, instr(P_SEARCHTEXT, '-') + 1) INTO v_firstfour,v_lastfour from dual;
            if P_OPERATORS = 'LIKE' then
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where CGC12 '|| v_operator_query ||' and rownum<=500 ' || v_division_query ;  /* build query string */ 
            else 
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where CGC12 '|| v_operator_query ||' and rownum<=500 ' || v_division_query ;  /* build query string */  
            END IF;
        else
        
    --Condition 3: cgc like '%1234%9012'----v_input_type - 12349012
            select SUBSTR( P_SEARCHTEXT, 0, 4), SUBSTR( P_SEARCHTEXT, -4, 4) into v_firstfour, v_lastfour from dual;
            if P_OPERATORS = 'LIKE' then
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where CGC12 '|| v_operator_query ||' and rownum<=500 ' || v_division_query ;  /* build query string */
            else
                v_operator_query := 'like upper(''%' || v_firstfour || '%' || v_lastfour || ''')';
                v_query := 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from  PRIMARYMETER m where CGC12 '|| v_operator_query ||' and rownum<=500 ' || v_division_query ;  /* build query string */
            END IF;
        end if;
    End IF;
    
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_PRIMARYMETER_CURSOR for v_query;  /* query executed */

    v_query:= 'SELECT OBJCLS_NAME AS "physicalName", LAYER_NAME AS "layerName", displayfieldname AS "displayFieldName", layerid AS "layerId", database_name AS "databaseName", LATESTWKID AS "latestWkid", WKID AS "wkid", GEOMETRYTYPE AS "geometryType" FROM PGE_SEARCH_LAYERS_ALL WHERE SEARCH_TYPE=''CGC'' ORDER BY CURSEQ';    
    open P_ALL_LAYERS_CURSOR for v_query;
END;
