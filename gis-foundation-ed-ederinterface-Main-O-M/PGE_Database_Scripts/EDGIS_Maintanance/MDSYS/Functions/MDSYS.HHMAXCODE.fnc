Prompt drop Function HHMAXCODE;
DROP FUNCTION MDSYS.HHMAXCODE
/

Prompt Function HHMAXCODE;
--
-- HHMAXCODE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.hhmaxcode(hhc IN RAW, maxlen IN NUMBER)
    RETURN RAW IS
begin
 return md.hhmaxcode(hhc, maxlen);
end;
/


Prompt Synonym HHMAXCODE;
--
-- HHMAXCODE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM HHMAXCODE FOR MDSYS.HHMAXCODE
/


Prompt Grants on FUNCTION HHMAXCODE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.HHMAXCODE TO PUBLIC
/