--------------------------------------------------------
--  DDL for Procedure SEARCH_TLM
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_TLM" (P_GLOBALID IN VARCHAR2, P_TLM_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255); 
  v_query_type VARCHAR2(255);
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(30000);    
  v_lookup_query varchar2(255);
  v_count number;
  
  CURSOR c1 IS SELECT COUNT(globalid) FROM edgis.transformer WHERE globalid = P_GLOBALID;

BEGIN

    OPEN c1;
    FETCH c1 INTO v_count;
    CLOSE c1;
    v_query_type := 'TLM';
    IF v_count > 0 THEN
      v_table_name := 'TRANSFORMER'; 
    ELSE
      v_table_name := 'PRIMARYMETER';
    END IF;
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    
    IF v_table_name = 'TRANSFORMER' THEN
      v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m LEFT JOIN EDGIS.ZPGEVW_TRANSFORMER_CSOURCE cs
              on cs.OBJECTID = m.OBJECTID where m.GLOBALID =''' || P_GLOBALID || '''';  
    ELSE
      v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where m.GLOBALID =''' || P_GLOBALID || '''';
    END IF;
    
    dbms_output.put_line(v_query);
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_TLM_CURSOR for v_query;  /* query executed */    
    
END;
