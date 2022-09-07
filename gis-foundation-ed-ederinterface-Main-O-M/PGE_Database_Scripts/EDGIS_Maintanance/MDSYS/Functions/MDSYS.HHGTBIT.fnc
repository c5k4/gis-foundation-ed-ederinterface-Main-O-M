Prompt drop Function HHGTBIT;
DROP FUNCTION MDSYS.HHGTBIT
/

Prompt Function HHGTBIT;
--
-- HHGTBIT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhgtbit (hhc IN RAW, topology IN BINARY_INTEGER)
    RETURN VARCHAR2 IS
begin
 return md.hhgtbit(hhc, topology);
end;
/


Prompt Synonym HHGTBIT;
--
-- HHGTBIT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHGTBIT FOR MDSYS.HHGTBIT
/
