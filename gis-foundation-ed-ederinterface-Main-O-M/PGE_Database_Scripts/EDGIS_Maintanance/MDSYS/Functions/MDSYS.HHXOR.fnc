Prompt drop Function HHXOR;
DROP FUNCTION MDSYS.HHXOR
/

Prompt Function HHXOR;
--
-- HHXOR  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhxor ( hh1 IN RAW, hh2 IN RAW )
    RETURN RAW IS
begin
 return md.hhxor(hh1, hh2);
end;
/


Prompt Synonym HHXOR;
--
-- HHXOR  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHXOR FOR MDSYS.HHXOR
/
