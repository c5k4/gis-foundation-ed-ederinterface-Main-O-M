Prompt drop Procedure GET_CURSOR_GLOBALID_1;
DROP PROCEDURE EDGIS.GET_CURSOR_GLOBALID_1
/

Prompt Procedure GET_CURSOR_GLOBALID_1;
--
-- GET_CURSOR_GLOBALID_1  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.GET_CURSOR_GLOBALID_1(
       p_tab IN VARCHAR2 ,
             p_cur OUT NOCOPY sys_refcursor) IS
 query_str VARCHAR2(200);
 invalid_identifier_exception EXCEPTION;
BEGIN


   query_str := 'SELECT DISTINCT OBJECTID FROM ' || p_tab || ' where  GLOBALID_1 is null or GLOBALID_1 =''{00000000-0000-0000-0000-000000000000}''' ;

dbms_output.put_line(query_str);
    OPEN p_cur FOR query_str;
EXCEPTION
WHEN invalid_identifier_exception THEN
  DBMS_OUTPUT.PUT_LINE ('Unexpected error:invalid_identifier_exception ');
WHEN OTHERS THEN
  DBMS_OUTPUT.PUT_LINE ('Unexpected error other');
END ;
/
