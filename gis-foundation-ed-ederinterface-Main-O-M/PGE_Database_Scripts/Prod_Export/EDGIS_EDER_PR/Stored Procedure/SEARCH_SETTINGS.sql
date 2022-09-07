--------------------------------------------------------
--  DDL for Procedure SEARCH_SETTINGS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."SEARCH_SETTINGS" (P_GLOBALID IN VARCHAR2, P_SETTINGS_CURSOR OUT SYS_REFCURSOR)
AS
  v_table_name VARCHAR2(255); 
  v_query_type VARCHAR2(255);
  v_column_name NVARCHAR2(5000); 
  v_query      varchar2(30000);    
  v_lookup_query varchar2(255);
   
BEGIN

    v_query_type := 'SETTINGS';
    v_table_name := 'NETWORKPROTECTOR';
   
    v_lookup_query := 'SELECT * FROM LOOKUP_CLSFLDDOMAINMAP fd LEFT OUTER JOIN LOOKUP_DOMAINLIST dl on fd.DOMAINNAME = dl.DOMAINNAME where UPPER(OBJCLSNAME) = ''EDGIS.' || v_table_name || '''';
    select query_params INTO v_column_name from PGE_SEARCH_CONFIGINFO where query_type= v_query_type and UPPER(table_name)= UPPER(v_table_name) and rownum<2;   
    v_query:= 'WITH T AS (' ||v_lookup_query|| ') select ' || SEARCH_SELFIELDS(v_column_name,'SUBTYPECD','T','m') || ' from ' || v_table_name || ' m where m.GLOBALID =''' || P_GLOBALID || '''';    
--    dbms_output.put_line(v_query);
    v_query := REPLACE(v_query,'$COMMA$',','); 
    open P_SETTINGS_CURSOR for v_query;  /* query executed */    
    
END;
