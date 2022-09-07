Prompt drop Function HHOR;
DROP FUNCTION MDSYS.HHOR
/

Prompt Function HHOR;
--
-- HHOR  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhor ( hh1 IN RAW, hh2 IN RAW )
    RETURN RAW IS
begin
 return md.hhor(hh1, hh2);
end;
/


Prompt Synonym HHOR;
--
-- HHOR  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHOR FOR MDSYS.HHOR
/
