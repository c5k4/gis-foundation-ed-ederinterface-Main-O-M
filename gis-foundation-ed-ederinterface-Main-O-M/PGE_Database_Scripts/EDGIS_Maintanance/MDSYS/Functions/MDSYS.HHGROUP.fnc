Prompt drop Function HHGROUP;
DROP FUNCTION MDSYS.HHGROUP
/

Prompt Function HHGROUP;
--
-- HHGROUP  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhgroup (hhc IN RAW)
    RETURN RAW IS
begin
 return md.hhgroup(hhc);
end;
/


Prompt Synonym HHGROUP;
--
-- HHGROUP  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHGROUP FOR MDSYS.HHGROUP
/
