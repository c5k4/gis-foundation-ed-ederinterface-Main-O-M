Prompt drop Procedure UPDATE_SCHEM_XY;
DROP PROCEDURE DMSSTAGING.UPDATE_SCHEM_XY
/

Prompt Procedure UPDATE_SCHEM_XY;
--
-- UPDATE_SCHEM_XY  (Procedure) 
--
CREATE OR REPLACE PROCEDURE DMSSTAGING.UPDATE_SCHEM_XY AS
sqlstr VARCHAR2(3000);
BEGIN
commit;
END;
/


Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO GISINTERFACE
/

Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO GIS_I
/

Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO GIS_INTERFACE to GIS_INTERFACE;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO GIS_INTERFACE
/

Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO PUBLIC to PUBLIC;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO PUBLIC
/

Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO SDE_EDITOR to SDE_EDITOR;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO SDE_EDITOR
/

Prompt Grants on PROCEDURE UPDATE_SCHEM_XY TO SDE_VIEWER to SDE_VIEWER;
GRANT EXECUTE, DEBUG ON DMSSTAGING.UPDATE_SCHEM_XY TO SDE_VIEWER
/
