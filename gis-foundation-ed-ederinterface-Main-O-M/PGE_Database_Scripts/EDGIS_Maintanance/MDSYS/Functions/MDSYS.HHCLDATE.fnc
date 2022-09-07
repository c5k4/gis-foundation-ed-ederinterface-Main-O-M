Prompt drop Function HHCLDATE;
DROP FUNCTION MDSYS.HHCLDATE
/

Prompt Function HHCLDATE;
--
-- HHCLDATE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcldate (jd IN NUMBER, fmt IN VARCHAR2)
    RETURN VARCHAR2 IS
begin
  return md.hhcldate(jd,fmt);
end;
/


Prompt Synonym HHCLDATE;
--
-- HHCLDATE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCLDATE FOR MDSYS.HHCLDATE
/
