set heading off;

-- This script needs to be run as part of the setup and config of the CC&B interface
-- The following three lines grant permissions to the SDE_EDITOR role for the three views created in the previous script
SELECT 'Granting permissions to SDE_EDITOR' FROM dual;
Grant all on zz_mv_transformer to sde_editor;
Grant all on zz_mv_cwot to sde_editor;
Grant all on zz_mv_servicepoint to sde_editor;

-- The following three lines grant permissions to the GIS_I user for the three views created in the previous script
SELECT 'Granting permissions to GIS_I' FROM dual;
Grant all on zz_mv_transformer to gis_i;
Grant all on zz_mv_cwot to gis_i;
Grant all on zz_mv_servicepoint to gis_i;

--Create a function to retrieve a unique GUID for use in the GIS application, may error out if the function is in use
CREATE OR REPLACE FUNCTION GDB_GUID
RETURN NCHAR
IS
guid NCHAR (38);
BEGIN
guid := upper(RAWTOHEX(SYS_GUID()));
RETURN
'{'||substr(guid,1,8)||'-'||substr(guid,9,4)||'-'||substr(guid,13,4)||'-'||substr(guid,17,4)||'-'||substr(guid,21,12)||'}';
END;
/

--Save changes and exit
commit;
exit;