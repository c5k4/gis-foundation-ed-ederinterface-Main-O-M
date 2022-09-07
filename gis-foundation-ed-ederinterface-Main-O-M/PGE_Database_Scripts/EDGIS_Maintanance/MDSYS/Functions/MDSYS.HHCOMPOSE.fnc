Prompt drop Function HHCOMPOSE;
DROP FUNCTION MDSYS.HHCOMPOSE
/

Prompt Function HHCOMPOSE;
--
-- HHCOMPOSE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcompose( hhc IN RAW,
           d01 IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhcompose(hhc,d01);
end;
/


Prompt Synonym HHCOMPOSE;
--
-- HHCOMPOSE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCOMPOSE FOR MDSYS.HHCOMPOSE
/
