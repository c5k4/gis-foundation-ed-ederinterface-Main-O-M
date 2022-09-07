Prompt drop Function HHSUBDIVIDE;
DROP FUNCTION MDSYS.HHSUBDIVIDE
/

Prompt Function HHSUBDIVIDE;
--
-- HHSUBDIVIDE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhsubdivide (hh1 IN RAW, cid IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhsubdivide(hh1, cid);
end;
/


Prompt Synonym HHSUBDIVIDE;
--
-- HHSUBDIVIDE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHSUBDIVIDE FOR MDSYS.HHSUBDIVIDE
/