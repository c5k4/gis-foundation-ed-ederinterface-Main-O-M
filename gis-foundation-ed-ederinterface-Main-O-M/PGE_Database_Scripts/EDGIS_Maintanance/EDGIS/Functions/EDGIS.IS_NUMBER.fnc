Prompt drop Function IS_NUMBER;
DROP FUNCTION EDGIS.IS_NUMBER
/

Prompt Function IS_NUMBER;
--
-- IS_NUMBER  (Function) 
--
CREATE OR REPLACE FUNCTION EDGIS.IS_NUMBER(str in varchar2) return varchar2 IS
 dummy number;
begin
 dummy := TO_NUMBER(str);
 return ('TRUE');
Exception WHEN OTHERS then
 return ('FALSE');
end;
/
