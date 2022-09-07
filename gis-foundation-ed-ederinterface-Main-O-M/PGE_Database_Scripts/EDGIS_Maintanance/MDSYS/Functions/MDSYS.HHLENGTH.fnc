Prompt drop Function HHLENGTH;
DROP FUNCTION MDSYS.HHLENGTH
/

Prompt Function HHLENGTH;
--
-- HHLENGTH  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhlength (hhc IN RAW, dim IN BINARY_INTEGER := NULL)
    RETURN BINARY_INTEGER IS
begin
 if dim is NULL then
   return md.hhlength(hhc);
 end if;
 return md.hhlength(hhc,dim);
end;
/


Prompt Synonym HHLENGTH;
--
-- HHLENGTH  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHLENGTH FOR MDSYS.HHLENGTH
/


Prompt Grants on FUNCTION HHLENGTH TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.HHLENGTH TO PUBLIC
/
