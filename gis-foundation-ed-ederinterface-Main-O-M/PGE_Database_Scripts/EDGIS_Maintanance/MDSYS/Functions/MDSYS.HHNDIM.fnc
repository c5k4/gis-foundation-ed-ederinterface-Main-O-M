Prompt drop Function HHNDIM;
DROP FUNCTION MDSYS.HHNDIM
/

Prompt Function HHNDIM;
--
-- HHNDIM  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhndim (hhc IN RAW) RETURN BINARY_INTEGER IS
begin
  return md.hhndim(hhc);
end;
/


Prompt Synonym HHNDIM;
--
-- HHNDIM  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHNDIM FOR MDSYS.HHNDIM
/
