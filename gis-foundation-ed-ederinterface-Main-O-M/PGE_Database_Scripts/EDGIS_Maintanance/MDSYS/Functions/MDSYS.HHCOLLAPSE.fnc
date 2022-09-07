Prompt drop Function HHCOLLAPSE;
DROP FUNCTION MDSYS.HHCOLLAPSE
/

Prompt Function HHCOLLAPSE;
--
-- HHCOLLAPSE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhcollapse( hhc IN RAW,
           d01 IN BINARY_INTEGER)
    RETURN RAW IS
begin
 return md.hhcollapse(hhc, d01);
end;
/


Prompt Synonym HHCOLLAPSE;
--
-- HHCOLLAPSE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHCOLLAPSE FOR MDSYS.HHCOLLAPSE
/
