Prompt drop Function HHNCOMPARE;
DROP FUNCTION MDSYS.HHNCOMPARE
/

Prompt Function HHNCOMPARE;
--
-- HHNCOMPARE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhncompare (hh1 IN RAW, hh2 IN RAW, lv IN BINARY_INTEGER)
    RETURN BINARY_INTEGER IS
begin
 return md.hhncompare(hh1,hh2,lv);
end;
/


Prompt Synonym HHNCOMPARE;
--
-- HHNCOMPARE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHNCOMPARE FOR MDSYS.HHNCOMPARE
/
