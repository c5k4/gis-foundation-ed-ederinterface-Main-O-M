Prompt drop Function HHCOMMONCODE;
DROP FUNCTION MDSYS.HHCOMMONCODE
/

Prompt Function HHCOMMONCODE;
--
-- HHCOMMONCODE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcommoncode (hh1 IN RAW, hh2 IN RAW)
    RETURN RAW IS
begin
 return md.hhcommoncode(hh1,hh2);
end;
/


Prompt Synonym HHCOMMONCODE;
--
-- HHCOMMONCODE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCOMMONCODE FOR MDSYS.HHCOMMONCODE
/
