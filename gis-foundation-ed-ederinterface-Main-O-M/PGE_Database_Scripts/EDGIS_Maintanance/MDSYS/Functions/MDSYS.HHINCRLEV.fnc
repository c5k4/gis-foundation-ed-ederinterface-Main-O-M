Prompt drop Function HHINCRLEV;
DROP FUNCTION MDSYS.HHINCRLEV
/

Prompt Function HHINCRLEV;
--
-- HHINCRLEV  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhincrlev (hhc IN RAW, lv IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhincrlev(hhc, lv);
end;
/


Prompt Synonym HHINCRLEV;
--
-- HHINCRLEV  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHINCRLEV FOR MDSYS.HHINCRLEV
/
