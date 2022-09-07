Prompt drop Function HHAND;
DROP FUNCTION MDSYS.HHAND
/

Prompt Function HHAND;
--
-- HHAND  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhand ( hh1 IN RAW, hh2 IN RAW )
    RETURN RAW IS
begin
 return md.hhand(hh1, hh2);
end;
/


Prompt Synonym HHAND;
--
-- HHAND  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHAND FOR MDSYS.HHAND
/
