Prompt drop Function HHSBIT;
DROP FUNCTION MDSYS.HHSBIT
/

Prompt Function HHSBIT;
--
-- HHSBIT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhsbit (hhc IN RAW, bit_number IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhsbit(hhc, bit_number);
end;
/


Prompt Synonym HHSBIT;
--
-- HHSBIT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHSBIT FOR MDSYS.HHSBIT
/
