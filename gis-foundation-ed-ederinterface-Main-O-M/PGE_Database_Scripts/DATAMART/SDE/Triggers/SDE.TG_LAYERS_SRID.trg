Prompt drop Trigger TG_LAYERS_SRID;
DROP TRIGGER SDE.TG_LAYERS_SRID
/

Prompt Trigger TG_LAYERS_SRID;
--
-- TG_LAYERS_SRID  (Trigger) 
--
CREATE OR REPLACE TRIGGER SDE.TG_LAYERS_SRID BEFORE UPDATE OF SRID ON SDE.LAYERS FOR EACH ROW
DECLARE value    integer := 0; st_srid  integer := -1; BEGIN SELECT a.srid INTO st_srid FROM sde.st_geometry_columns a WHERE a.owner = :old.owner AND a.table_name = :old.table_name; IF st_srid >= 0 THEN SELECT b.srid into value FROM SDE.spatial_references b WHERE b.auth_srid = st_srid AND b.srid = :new.srid; END IF; EXCEPTION WHEN NO_DATA_FOUND THEN IF st_srid >= 0 THEN RAISE_APPLICATION_ERROR(-20085, 'Invalid SDE.LAYERS update. Update violates the SDE.ST_GEOMETRY_COLUMNS SRID referential integrity on '||:old.owner||'.'||:old.table_name,TRUE); END IF; END;
/
