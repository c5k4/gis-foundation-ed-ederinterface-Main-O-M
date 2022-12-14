Prompt drop Function HHCELLBNDRY;
DROP FUNCTION MDSYS.HHCELLBNDRY
/

Prompt Function HHCELLBNDRY;
--
-- HHCELLBNDRY  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcellbndry (hhc IN RAW, dim IN BINARY_INTEGER,
           lb IN NUMBER, ub IN NUMBER,lv IN BINARY_INTEGER, mm IN VARCHAR2)
    RETURN NUMBER IS
begin
 return md.hhcellbndry(hhc,dim,lb,ub,lv,mm);
end;
/


Prompt Synonym HHCELLBNDRY;
--
-- HHCELLBNDRY  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCELLBNDRY FOR MDSYS.HHCELLBNDRY
/


Prompt Grants on FUNCTION HHCELLBNDRY TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.HHCELLBNDRY TO PUBLIC
/
