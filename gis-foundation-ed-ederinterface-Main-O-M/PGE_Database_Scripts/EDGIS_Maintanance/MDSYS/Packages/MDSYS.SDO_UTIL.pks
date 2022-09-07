Prompt drop Package SDO_UTIL;
DROP PACKAGE MDSYS.SDO_UTIL
/

Prompt Package SDO_UTIL;
--
-- SDO_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_util AUTHID current_user AS

-- CONSTANT DECLARATION
  SDO_GTYPE_POLYGON       CONSTANT  NUMBER := 3;
  SDO_GTYPE_MULTIPOLYGON  CONSTANT  NUMBER := 7;
  SDO_GTYPE_COLLECTION    CONSTANT  NUMBER := 4;


  Function expand_multi_point (geometry IN mdsys.sdo_geometry)
  RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

  FUNCTION expand_geom (geometry IN mdsys.sdo_geometry)
  RETURN mdsys.sdo_geometry DETERMINISTIC PARALLEL_ENABLE;

  FUNCTION extract(geometry IN mdsys.sdo_geometry,
                   element  IN NUMBER,
                   ring     IN NUMBER DEFAULT 0)
    RETURN mdsys.sdo_geometry DETERMINISTIC  PARALLEL_ENABLE;
    PRAGMA restrict_references(extract, wnds, rnps, wnps, trust);

  FUNCTION extract_all(geometry IN mdsys.sdo_geometry,
                       flatten  IN NUMBER DEFAULT 1)
    RETURN mdsys.sdo_geometry_array DETERMINISTIC;
    PRAGMA restrict_references(extract_all, wnds, rnps, wnps, trust);

  FUNCTION append(geometry1 IN MDSYS.sdo_geometry,
                  geometry2 IN MDSYS.sdo_geometry)
    RETURN mdsys.sdo_geometry DETERMINISTIC PARALLEL_ENABLE;
    PRAGMA restrict_references(append, wnds, wnps);


  FUNCTION ExtractVoids(geometry IN mdsys.sdo_geometry,
                        dim      IN mdsys.sdo_dim_array)
    RETURN mdsys.sdo_geometry DETERMINISTIC PARALLEL_ENABLE;
    PRAGMA restrict_references(ExtractVoids, rnds, wnds, rnps, wnps, trust);

 FUNCTION GetVertices(geometry IN mdsys.sdo_geometry)
 RETURN vertex_set_type;
-- PRAGMA restrict_references(GetVertices, wnds, rnps, wnps);

  FUNCTION GetNumElem(geometry IN mdsys.sdo_geometry)
    RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;
    PRAGMA restrict_references(GetNumElem, rnds, wnds, rnps, wnps, trust);

  FUNCTION GetNumRings(
    geom IN MDSYS.SDO_GEOMETRY)
      RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;
  PRAGMA restrict_references(GetNumRings, rnds, wnds, rnps, wnps, trust);

  FUNCTION GetNumVertices(geometry IN mdsys.sdo_geometry)
    RETURN NUMBER PARALLEL_ENABLE;
    PRAGMA restrict_references(GetNumVertices, rnds, wnds, rnps, wnps, trust);

  FUNCTION OuterLn(geometry IN mdsys.sdo_geometry,
                   dim      IN mdsys.sdo_dim_array)
    RETURN mdsys.sdo_geometry DETERMINISTIC PARALLEL_ENABLE;
    PRAGMA restrict_references(OuterLn,rnds,wnds,rnps,wnps,trust);

  FUNCTION RefineMGon(mgon IN mdsys.sdo_geometry,
                      gon  IN mdsys.sdo_geometry,
                      dim  IN mdsys.sdo_dim_array)
    RETURN mdsys.sdo_geometry DETERMINISTIC;
    PRAGMA restrict_references(RefineMGon,rnds,wnds,rnps,wnps,trust);

 -- truncate the original number up to no_of_digits
 -- no_of_digits positive:  truncate the number to no_of_digits AFTER the decimal point
 -- ex: truncate_number(1.123456789,5) returns 1.12345
 -- no_of_digits negative:  truncate the number up to no_of_digits BEFORE the decimal point
 -- ex: truncate_number(987654321.123456789,-5) returns 987600000.0

 FUNCTION truncate_number(value NUMBER, no_of_digits NUMBER)
    RETURN NUMBER PARALLEL_ENABLE;
 PRAGMA restrict_references(truncate_number, wnds, rnps, wnps);

 FUNCTION rectify_geometry(
    geometry     IN MDSYS.SDO_GEOMETRY,
    tolerance    IN NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(rectify_geometry, rnds, wnds, rnps, wnps, trust);

  /* simplify a geometry */
  FUNCTION simplify(
   geometry       IN mdsys.sdo_geometry,
   threshold      IN NUMBER,
   tolerance      IN NUMBER := 0.0000005)
    RETURN mdsys.sdo_geometry DETERMINISTIC PARALLEL_ENABLE;
    PRAGMA restrict_references(simplify, rnds, wnds, rnps, wnps, trust);

 FUNCTION polygontoline(geometry IN mdsys.sdo_geometry)
    return MDSYS.SDO_GEOMETRY deterministic PARALLEL_ENABLE;

 FUNCTION point_to_line(
   geom1 IN mdsys.sdo_geometry,
   geom2 IN mdsys.sdo_geometry,
   tol   IN number := 10e-16)
   RETURN MDSYS.SDO_GEOMETRY deterministic PARALLEL_ENABLE;

 FUNCTION remove_duplicates(geometry IN sdo_geometry, dim in sdo_dim_array)
    return MDSYS.SDO_GEOMETRY deterministic PARALLEL_ENABLE;

 FUNCTION remove_duplicate_vertices(geometry IN sdo_geometry,
                                                tolerance in NUMBER)
    return MDSYS.SDO_GEOMETRY deterministic PARALLEL_ENABLE;

 FUNCTION circle_polygon (center_longitude     number,
                          center_latitude      number,
                          radius               number,
                          arc_tolerance            number)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION ellipse_polygon (center_longitude                number,
                           center_latitude                 number,
                           semi_major_axis                 number,
                           semi_minor_axis                 number,
                           azimuth                         number,
                           arc_tolerance                       number)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION point_at_bearing(start_point sdo_geometry,
                   bearing number,
                   distance number)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 PROCEDURE bearing_tilt_for_points(
                   start_point sdo_geometry,
                   end_point sdo_geometry,
                   tol number,
                   bearing OUT number,
                   tilt OUT number) ;

 FUNCTION convert_unit(value NUMBER, in_unit varchar2, out_unit varchar2)
 RETURN number PARALLEL_ENABLE;

 FUNCTION convert_distance(srid  number, distance NUMBER, unit_spec  varchar2)
 RETURN number PARALLEL_ENABLE;

 PROCEDURE Prepare_For_TTS (table_space IN VARCHAR2);

 PROCEDURE Initialize_Indexes_For_TTS ;

 FUNCTION to_clob(Geometry IN MDSYS.SDO_GEOMETRY)
  RETURN CLOB DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION from_clob(ClobGeom IN CLOB)
  RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION to_gmlgeometry(Geometry IN MDSYS.SDO_GEOMETRY)
 RETURN CLOB DETERMINISTIC  PARALLEL_ENABLE AS LANGUAGE JAVA NAME
    'oracle.spatial.util.GML2.to_GMLGeometryCLOB(oracle.sql.STRUCT) return oracle.sql.CLOB' ;

 FUNCTION to_gmlgeometry(Geometry IN MDSYS.SDO_GEOMETRY,
                            srsNameSpace IN varchar2, srsNSAlias IN varchar2)
 RETURN CLOB DETERMINISTIC  PARALLEL_ENABLE AS LANGUAGE JAVA NAME
    'oracle.spatial.util.GML2.to_GMLGeometryCLOB(oracle.sql.STRUCT, java.lang.String, java.lang.String) return oracle.sql.CLOB' ;

 FUNCTION to_gml311geometry(Geometry IN MDSYS.SDO_GEOMETRY)
 RETURN CLOB DETERMINISTIC  PARALLEL_ENABLE AS LANGUAGE JAVA NAME
    'oracle.spatial.util.GML3.to_GML3GeometryCLOB(oracle.sql.STRUCT) return oracle.sql.CLOB' ;

 FUNCTION to_gml311geometry(Geometry IN MDSYS.SDO_GEOMETRY,
                            srsNameSpace IN varchar2, srsNSAlias IN varchar2)
 RETURN CLOB DETERMINISTIC  PARALLEL_ENABLE AS LANGUAGE JAVA NAME
    'oracle.spatial.util.GML3.to_GML3GeometryCLOB(oracle.sql.STRUCT, java.lang.String, java.lang.String) return oracle.sql.CLOB' ;

FUNCTION to_kmlgeometry(Geometry IN MDSYS.SDO_GEOMETRY)
 RETURN CLOB DETERMINISTIC  PARALLEL_ENABLE AS LANGUAGE JAVA NAME
    'oracle.spatial.util.KML2.to_KMLGeometryCLOB(oracle.sql.STRUCT) return oracle.sql.CLOB' ;

 FUNCTION to_wkbgeometry(geometry IN MDSYS.SDO_GEOMETRY)
 RETURN BLOB DETERMINISTIC PARALLEL_ENABLE;
-- AS LANGUAGE JAVA NAME
-- 'oracle.spatial.util.Adapters.structToWkb(oracle.sql.STRUCT) return oracle.sql.BLOB';

 FUNCTION to_wktgeometry(geometry IN MDSYS.SDO_GEOMETRY)
 RETURN CLOB DETERMINISTIC PARALLEL_ENABLE;
-- AS LANGUAGE JAVA NAME
-- 'oracle.spatial.util.Adapters.structToWkt(oracle.sql.STRUCT) return oracle.sql.CLOB';

 FUNCTION to_wktgeometry_varchar(geometry IN MDSYS.SDO_GEOMETRY)
 RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION from_wkbgeometry(geometry IN BLOB)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.wkbToSTRUCT(oracle.sql.BLOB) return oracle.sql.STRUCT';

 FUNCTION from_wktgeometry(geometry IN CLOB)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.wktToSTRUCT(oracle.sql.CLOB) return oracle.sql.STRUCT';

 FUNCTION from_wktgeometry(geometry IN VARCHAR2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.wktToSTRUCT(java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GMLgeometry(geometry IN CLOB)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gmlToSTRUCT(oracle.sql.CLOB) return oracle.sql.STRUCT';

 FUNCTION from_GMLgeometry(geometry IN CLOB, srsNameSpace IN varchar2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gmlToSTRUCT(oracle.sql.CLOB, java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GMLgeometry(geometry IN VARCHAR2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gmlToSTRUCT(java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GMLgeometry(geometry IN VARCHAR2, srsNameSpace IN varchar2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gmlToSTRUCT(java.lang.String, java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GML311geometry(geometry IN CLOB)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gml311ToSTRUCT(oracle.sql.CLOB) return oracle.sql.STRUCT';

 FUNCTION from_GML311geometry(geometry IN CLOB, srsNameSpace IN varchar2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gml311ToSTRUCT(oracle.sql.CLOB, java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GML311geometry(geometry IN VARCHAR2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gml311ToSTRUCT(java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_GML311geometry(geometry IN VARCHAR2, srsNameSpace IN varchar2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.gml311ToSTRUCT(java.lang.String, java.lang.String) return oracle.sql.STRUCT';

 FUNCTION from_KMLgeometry(geometry IN CLOB)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.kmlToSTRUCT(oracle.sql.CLOB) return oracle.sql.STRUCT';

 FUNCTION from_KMLgeometry(geometry IN VARCHAR2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.kmlToSTRUCT(java.lang.String) return oracle.sql.STRUCT';

 FUNCTION extrude(geometry IN MDSYS.SDO_GEOMETRY,
                  grdHeight IN SDO_NUMBER_ARRAY,
                  height IN SDO_NUMBER_ARRAY,
                  cond IN VARCHAR2,
                  tol IN NUMBER)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- AS LANGUAGE JAVA NAME
-- 'oracle.spatial.util.Adapters.extrusion1(oracle.sql.STRUCT, oracle.spatial.type.SdoNumberArray, oracle.spatial.type.SdoNumberArray, java.lang.String, oracle.sql.NUMBER) return oracle.sql.STRUCT';

 FUNCTION extrude(geometry IN MDSYS.SDO_GEOMETRY,
                  grdHeight IN SDO_NUMBER_ARRAY,
                  height IN SDO_NUMBER_ARRAY,
                  tol IN NUMBER,
                  optional3dSrid IN NUMBER DEFAULT NULL)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
-- AS LANGUAGE JAVA NAME
-- 'oracle.spatial.util.Adapters.extrusion2(oracle.sql.STRUCT, oracle.spatial.type.SdoNumberArray, oracle.spatial.type.SdoNumberArray, oracle.sql.NUMBER, oracle.sql.NUMBER) return oracle.sql.STRUCT';

-- FUNCTION append_to_collection(geometry1 IN MDSYS.SDO_GEOMETRY,
--                               geometry2 IN MDSYS.SDO_GEOMETRY )
-- RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC
-- AS LANGUAGE JAVA NAME
-- 'oracle.spatial.util.Adapters.appendToCollection(oracle.sql.STRUCT, oracle.sql.STRUCT) return oracle.sql.STRUCT';

 FUNCTION affinetransforms(geometry IN MDSYS.SDO_GEOMETRY,
                           translation IN VARCHAR2, tx IN NUMBER, ty IN NUMBER, tz IN NUMBER,
                           scaling IN VARCHAR2, Psc1 IN MDSYS.SDO_GEOMETRY, sx IN NUMBER, sy IN NUMBER, sz IN NUMBER,
                           rotation IN VARCHAR2, P1 IN MDSYS.SDO_GEOMETRY, line1 IN MDSYS.SDO_GEOMETRY, angle IN NUMBER, dir IN NUMBER,
                           shearing IN VARCHAR2, SHxy IN NUMBER, SHyx IN NUMBER, SHxz IN NUMBER, SHzx IN NUMBER, SHyz IN NUMBER, SHzy IN NUMBER,
                           reflection IN VARCHAR2, Pref IN MDSYS.SDO_GEOMETRY, lineR IN MDSYS.SDO_GEOMETRY, dirR IN NUMBER, planeR IN VARCHAR2, n IN SDO_NUMBER_ARRAY, bigD IN SDO_NUMBER_ARRAY)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.affineTransforms(oracle.sql.STRUCT, java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
java.lang.String, oracle.sql.STRUCT, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
java.lang.String, oracle.sql.STRUCT, oracle.sql.STRUCT, oracle.sql.NUMBER, oracle.sql.NUMBER,
java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
java.lang.String, oracle.sql.STRUCT, oracle.sql.STRUCT, oracle.sql.NUMBER, java.lang.String, oracle.spatial.type.SdoNumberArray, oracle.spatial.type.SdoNumberArray) return oracle.sql.STRUCT';

 FUNCTION extract3d(geometry IN MDSYS.SDO_GEOMETRY, label IN VARCHAR2)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.extract3d(oracle.sql.STRUCT, java.lang.String) return oracle.sql.STRUCT';

 FUNCTION getlabelbyelement(sourceGeometry IN MDSYS.SDO_GEOMETRY, queryElement IN MDSYS.SDO_GEOMETRY, tol IN NUMBER)
 RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.getlabelbyelement(oracle.sql.STRUCT, oracle.sql.STRUCT, oracle.sql.NUMBER) return java.lang.String';

 FUNCTION validate_wkbgeometry(geometry IN BLOB)
 RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.validateWkb(oracle.sql.BLOB) return java.lang.String';

 FUNCTION validate_wktgeometry(geometry IN CLOB)
 RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.validateWkt(oracle.sql.CLOB) return java.lang.String';

 FUNCTION validate_wktgeometry(geometry IN VARCHAR2)
 RETURN VARCHAR2 DETERMINISTIC PARALLEL_ENABLE
 AS LANGUAGE JAVA NAME
 'oracle.spatial.util.Adapters.validateWkt(java.lang.String) return java.lang.String';

 FUNCTION concat_lines (geometry1 IN MDSYS.SDO_GEOMETRY,
                        geometry2 IN MDSYS.SDO_GEOMETRY)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 PROCEDURE internal_ordinate_copy(src IN MDSYS.SDO_ORDINATE_ARRAY,
                            src_position IN INTEGER,
                            dst IN OUT NOCOPY MDSYS.SDO_ORDINATE_ARRAY,
                            dst_position IN INTEGER,
                            length IN INTEGER);

 FUNCTION reverse_linestring(geometry IN MDSYS.SDO_GEOMETRY)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION internal_merge_linestrings(geometry IN MDSYS.SDO_GEOMETRY)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION internal_merge_linestrings(geomArr IN MDSYS.SDO_GEOMETRY_ARRAY)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

 FUNCTION internal_make_line_out_of_elem(
     multilinestring IN MDSYS.SDO_GEOMETRY, element_index IN INTEGER)
 RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 PROCEDURE internal_reverse_line_points(
       ordinates IN OUT NOCOPY MDSYS.SDO_ORDINATE_ARRAY);

------------------------------------------------------------------
-- Name
--   Partition_Table
-- Purpose
--   Partitions the input "schema.tablename" into buckets of at
--   most ptn_capacity each. The partitioning is based on the
--   spatial extent or MBR stored as the intervals <min_di, max_di>
--   in each dimension di.  The data is written back with
--   the ptn_id into the "output_table" which is assumed to be
--   be pre-created by the user.
--   The input <tablename> table is expected to have the following columns:
--      "rid" -- unique id for each row (e.g., the table rowid)
--      min_d1, max_d1 -- minimum and maximum values in dimension 1
--      min_d2, max_d2 -- minimum and maximum values in dimension 2
--      ..
--      min_dn, max_dn -- minimum and maximum values in dimension n
--      where n is the dimensionality specified by inp arg "numdim"
--   The input "wrk_tblspc" specifies the tablespace where "scratch-pad"
--	tables are created and dropped. Keep this tablespace different from
--      the tablespace in which the input <tablename> and output_table are.
--      (typical usage: create wrk_tblspc and drop after this procedure)
--   The arg "output_table" specifies where to write the output partitions
--     This routine assumes the output_table is pre-created and has the
--     following columns:
--     ptn_id number, rid varchar2(24), min_d1 number, max_d1 number,
--     min_d2, max_d2, ...., min_dn, max_dn (all number columns).
--     This routine writes the rows from <tablename> back to <output_table>
--       with the ptn_id set.
--  The arg <output_ptn_table> specifies where to write ptn extent information
--     This table should have the following numeric columns:
--     ptn_id, min_d1, max_d1, min_d2, max_d2, ...., min_dn, max_dn.
--  Parameter "numdim" specifies the number of dimensions.
--  Parameter "commit_interval" "n" specifies that commits happen
--  after every batch of n rows that are written to the <output_table>.
--  Parameter "packed_ptns" tries to pack the partitions.


 PROCEDURE partition_table(schemaname in varchar2, tablename in varchar2,
                           output_data_table in varchar2,
                           output_ptn_table in varchar2,
                           ptn_capacity in number default 100,
                           numdim in number default 2,
		           wrk_tblspc in varchar2 default null,
		           ptn_type in varchar2 default null,
                           dop in number default 1);


------------------------------------------------------------------
-- Name
--   DROP_WORK_TABLES
-- Purpose
--   This function drops any work tables and views in the current schema
--   created as part of partition_table, index creation, or
--   TIN/Point Cloud utilities.
--
--   DROPS all tables/views that match 'M%_<oidstr>$$%'
--   Input oidstr has to contain only hexadecimal numbers w/o spaces

 PROCEDURE DROP_WORK_TABLES(oidstr varchar2);

 FUNCTION remove_inner_rings(inpgeom SDO_GEOMETRY, inptol number)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION quad_tiles(geom SDO_GEOMETRY, sdo_level number, tol number:=0.0000000005)
 RETURN mdsys.F81_index_obj_array DETERMINISTIC;

 FUNCTION interior_point (geom SDO_GEOMETRY, tol number := 0.00000000005)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION number_of_components(geometry SDO_GEOMETRY, requested_type varchar2)
 RETURN NUMBER PARALLEL_ENABLE;

 FUNCTION get_2d_footprint(geometry SDO_GEOMETRY, tolerance number := 0.05)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

END sdo_util;
/


Prompt Synonym SDO_UTIL;
--
-- SDO_UTIL  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_UTIL FOR MDSYS.SDO_UTIL
/


Prompt Grants on PACKAGE SDO_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_UTIL TO PUBLIC
/
