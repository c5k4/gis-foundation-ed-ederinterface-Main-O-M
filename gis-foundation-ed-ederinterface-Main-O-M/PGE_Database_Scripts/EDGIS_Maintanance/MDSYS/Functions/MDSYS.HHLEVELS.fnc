Prompt drop Function HHLEVELS;
DROP FUNCTION MDSYS.HHLEVELS
/

Prompt Function HHLEVELS;
--
-- HHLEVELS  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhlevels (lb IN NUMBER, ub IN NUMBER, pr IN BINARY_INTEGER)
    RETURN BINARY_INTEGER IS
begin
 return md.hhlevels(lb,ub,pr);
end;
/


Prompt Synonym HHLEVELS;
--
-- HHLEVELS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHLEVELS FOR MDSYS.HHLEVELS
/
