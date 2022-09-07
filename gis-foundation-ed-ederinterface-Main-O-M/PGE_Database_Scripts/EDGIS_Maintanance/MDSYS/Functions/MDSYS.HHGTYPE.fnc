Prompt drop Function HHGTYPE;
DROP FUNCTION MDSYS.HHGTYPE
/

Prompt Function HHGTYPE;
--
-- HHGTYPE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhgtype (hhc IN RAW)
    RETURN BINARY_INTEGER IS
begin
 return md.hhgtype(hhc);
end;
/


Prompt Synonym HHGTYPE;
--
-- HHGTYPE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHGTYPE FOR MDSYS.HHGTYPE
/
