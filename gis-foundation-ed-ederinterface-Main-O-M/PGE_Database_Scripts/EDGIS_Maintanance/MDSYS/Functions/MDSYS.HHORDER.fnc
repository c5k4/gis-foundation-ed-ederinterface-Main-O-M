Prompt drop Function HHORDER;
DROP FUNCTION MDSYS.HHORDER
/

Prompt Function HHORDER;
--
-- HHORDER  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhorder (hhc IN RAW)
    RETURN RAW IS
begin
 return md.hhorder(hhc);
end;
/


Prompt Synonym HHORDER;
--
-- HHORDER  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHORDER FOR MDSYS.HHORDER
/
