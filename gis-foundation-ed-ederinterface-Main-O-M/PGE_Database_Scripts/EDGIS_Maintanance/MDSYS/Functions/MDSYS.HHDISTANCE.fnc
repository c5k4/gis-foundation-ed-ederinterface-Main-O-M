Prompt drop Function HHDISTANCE;
DROP FUNCTION MDSYS.HHDISTANCE
/

Prompt Function HHDISTANCE;
--
-- HHDISTANCE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhdistance (type IN VARCHAR2, hh1 IN RAW, hh2 IN RAW,
           l01 IN NUMBER,       u01 IN NUMBER)
    RETURN NUMBER IS
begin
  return md.hhdistance(type,hh1,hh2,l01,u01);
end;
/


Prompt Synonym HHDISTANCE;
--
-- HHDISTANCE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHDISTANCE FOR MDSYS.HHDISTANCE
/
