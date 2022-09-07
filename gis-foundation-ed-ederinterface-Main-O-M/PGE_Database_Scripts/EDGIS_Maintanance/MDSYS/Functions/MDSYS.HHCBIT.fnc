Prompt drop Function HHCBIT;
DROP FUNCTION MDSYS.HHCBIT
/

Prompt Function HHCBIT;
--
-- HHCBIT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcbit (hhc IN RAW, bit_number IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhcbit(hhc,bit_number);
end;
/


Prompt Synonym HHCBIT;
--
-- HHCBIT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCBIT FOR MDSYS.HHCBIT
/
