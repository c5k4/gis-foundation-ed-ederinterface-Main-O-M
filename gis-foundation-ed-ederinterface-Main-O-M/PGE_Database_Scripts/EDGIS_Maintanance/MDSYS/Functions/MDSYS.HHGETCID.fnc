Prompt drop Function HHGETCID;
DROP FUNCTION MDSYS.HHGETCID
/

Prompt Function HHGETCID;
--
-- HHGETCID  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhgetcid ( hhc IN RAW, lv IN BINARY_INTEGER )
    RETURN NUMBER IS
begin
 return md.hhgetcid(hhc, lv);
end;
/


Prompt Synonym HHGETCID;
--
-- HHGETCID  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHGETCID FOR MDSYS.HHGETCID
/
