Prompt drop Function HHDECODE;
DROP FUNCTION MDSYS.HHDECODE
/

Prompt Function HHDECODE;
--
-- HHDECODE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhdecode (
           hhc IN RAW, dim IN BINARY_INTEGER, lb IN NUMBER, ub IN NUMBER)
    RETURN NUMBER IS
begin
  return md.hhdecode(hhc,dim,lb,ub);
end;
/


Prompt Synonym HHDECODE;
--
-- HHDECODE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHDECODE FOR MDSYS.HHDECODE
/
