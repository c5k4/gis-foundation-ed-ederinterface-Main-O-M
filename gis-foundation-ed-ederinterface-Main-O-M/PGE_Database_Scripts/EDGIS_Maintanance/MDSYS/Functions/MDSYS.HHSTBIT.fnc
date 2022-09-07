Prompt drop Function HHSTBIT;
DROP FUNCTION MDSYS.HHSTBIT
/

Prompt Function HHSTBIT;
--
-- HHSTBIT  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhstbit (hhc IN RAW, topology IN BINARY_INTEGER, type IN VARCHAR2)
    RETURN RAW IS
begin
 return md.hhstbit(hhc, topology,type);
end;
/


Prompt Synonym HHSTBIT;
--
-- HHSTBIT  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHSTBIT FOR MDSYS.HHSTBIT
/
