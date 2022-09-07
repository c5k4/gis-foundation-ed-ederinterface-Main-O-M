Prompt drop Function HHSTYPE;
DROP FUNCTION MDSYS.HHSTYPE
/

Prompt Function HHSTYPE;
--
-- HHSTYPE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhstype (hhc IN RAW, type_id IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhstype(hhc, type_id);
end;
/


Prompt Synonym HHSTYPE;
--
-- HHSTYPE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHSTYPE FOR MDSYS.HHSTYPE
/
