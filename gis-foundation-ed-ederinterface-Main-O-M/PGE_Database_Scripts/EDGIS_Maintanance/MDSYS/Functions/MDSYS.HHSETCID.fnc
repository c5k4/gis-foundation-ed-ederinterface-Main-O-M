Prompt drop Function HHSETCID;
DROP FUNCTION MDSYS.HHSETCID
/

Prompt Function HHSETCID;
--
-- HHSETCID  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhsetcid ( hhc IN RAW, lv IN BINARY_INTEGER, cid IN NUMBER )
    RETURN RAW IS
begin
 return md.hhsetcid(hhc, lv, cid);
end;
/


Prompt Synonym HHSETCID;
--
-- HHSETCID  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHSETCID FOR MDSYS.HHSETCID
/
