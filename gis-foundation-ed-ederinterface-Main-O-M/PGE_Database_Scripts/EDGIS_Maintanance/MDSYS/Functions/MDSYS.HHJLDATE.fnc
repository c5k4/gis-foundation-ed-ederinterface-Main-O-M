Prompt drop Function HHJLDATE;
DROP FUNCTION MDSYS.HHJLDATE
/

Prompt Function HHJLDATE;
--
-- HHJLDATE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhjldate (ds IN VARCHAR2, fmt IN VARCHAR2)
    RETURN NUMBER IS
begin
 return md.hhjldate(ds,fmt);
end;
/


Prompt Synonym HHJLDATE;
--
-- HHJLDATE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHJLDATE FOR MDSYS.HHJLDATE
/
