Prompt drop Function HHGBIT;
DROP FUNCTION MDSYS.HHGBIT
/

Prompt Function HHGBIT;
--
-- HHGBIT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhgbit (hhc IN RAW, bit_number IN BINARY_INTEGER)
    RETURN BINARY_INTEGER IS
begin
 return md.hhgbit(hhc, bit_number);
end;
/


Prompt Synonym HHGBIT;
--
-- HHGBIT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHGBIT FOR MDSYS.HHGBIT
/
