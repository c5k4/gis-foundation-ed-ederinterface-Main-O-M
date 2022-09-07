Prompt drop Function OGC_POINTONSURFACE;
DROP FUNCTION MDSYS.OGC_POINTONSURFACE
/

Prompt Function OGC_POINTONSURFACE;
--
-- OGC_POINTONSURFACE  (Function) 
--
CREATE OR REPLACE FUNCTION MDSYS.OGC_PointOnSurface(
  s ST_Geometry)
    RETURN ST_Point DETERMINISTIC IS
BEGIN
  IF(UPPER(OGC_GeometryType(s)) IN ('POLYGON')) THEN
    RETURN TREAT(s AS ST_Surface).ST_PointOnSurface();
  END IF;
  IF(UPPER(OGC_GeometryType(s)) IN ('MULTIPOLYGON')) THEN
    RETURN TREAT(s AS ST_MultiSurface).ST_PointOnSurface();
  END IF;
  RETURN NULL;
END OGC_PointOnSurface;
/


Prompt Synonym POINTONSURFACE;
--
-- POINTONSURFACE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM POINTONSURFACE FOR MDSYS.OGC_POINTONSURFACE
/


Prompt Grants on FUNCTION OGC_POINTONSURFACE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.OGC_POINTONSURFACE TO PUBLIC
/
