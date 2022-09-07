Prompt drop Function GDB_GUID;
DROP FUNCTION GIS_I.GDB_GUID
/

Prompt Function GDB_GUID;
--
-- GDB_GUID  (Function) 
--
CREATE OR REPLACE FUNCTION GIS_I.GDB_GUID
RETURN NCHAR
IS
guid NCHAR (38);
BEGIN
guid := upper(RAWTOHEX(SYS_GUID()));
RETURN
'{'||substr(guid,1,8)||'-'||substr(guid,9,4)||'-'||substr(guid,13,4)||'-'||substr(guid,17,4)||'-'||substr(guid,21,12)||'}';
END;
/
