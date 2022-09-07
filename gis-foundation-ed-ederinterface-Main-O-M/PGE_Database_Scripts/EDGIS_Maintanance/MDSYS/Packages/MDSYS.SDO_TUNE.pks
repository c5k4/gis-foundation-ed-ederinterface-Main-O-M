Prompt drop Package SDO_TUNE;
DROP PACKAGE MDSYS.SDO_TUNE
/

Prompt Package SDO_TUNE;
--
-- SDO_TUNE  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_tune AUTHID current_user AS

------------------------------------------------------------------------------
-- Purpose
--   This function can be used to determine which level to use when indexing
--   a layer using the fixed tile method. The level you choose to tile a
--   layer is a major factor in query performance. If you choose to use a
--   very small tile size then many tiles are generated for a large
--   area-of-interest. If you choose a very large tile size then the primary
--   filter will return too many candidate rows, and is less effective.
--
-- Syntax
--   ESTIMATE_TILING_LEVEL (layer, maxtiles, type_of_estimate)
--
-- Keywords and Parameters
--   layer             is the name of the spatial layer for which the tile
--                     level is to be determined. Datatype is VARCHAR2.
--   maxtiles          is the maximum number of valid tiles that can be used
--                     to index the type_of_estimate object defined below.
--                     Datatype is INTEGER.
--   type_of_estimate  There are three different type_of_estimate objects
--                     that can be defined:
--     LAYER_EXTENT    defines the type_of_estimate object to be the rectangle
--                     defined by the users coordinate system.
--     ALL_GID_EXTNET  defines the type_of_estimate object to be the rectangle
--                     that defines the bounds of all the SDO_GID's of a layer.
--     AVG_GID_EXTENT  defines the type_of_estimate object to be the rectangle
--                     that represents the average individual geometry bounding
--                     box. The average bounding box is determined by sampling
--                     every geometry in the specified layer. Datatype is
--                     VARCHAR2.
--
-- Returns
--   This function returns an integer representing the level used to index the
--   specified layer.
--
--   0 is returned if the data is inconsistent:
--   e.g. _sdodim does not match _sdogeom table.
--   This needs to be fixed by adding more error checking in future versions.
--
-- Usage Notes
--   Using the AVG_GID_EXTENT performs the most analysis of the three types.
--
/* DEPRECATED
  FUNCTION estimate_tiling_level (layer            VARCHAR2,
                                  num_tiles        INTEGER,
                                  type_of_estimate VARCHAR2 := 'ALL_GID_EXTENT')
  RETURN INTEGER;
*/


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates the extents of a layer by keeping track of the
--   max and min x and y values for all geometries in the layer. These numbers
--   also gives the MBR (minimal bounding rectangle) of all the geometries.
--
-- Syntax
--   EXTENT_OF (layer, min_x, max_x, min_y, max_y)
--
-- Keywords and Paramters
--   layer   is the name of the spatial layer for which the extent is to
--           be determined. Datatype is VARCHAR2.
--   min_x   smallest x value for all geometries. Datatype is out number.
--   max_x   largest x value for all geometries. Datatype is out number.
--   min_y   smallest y value for all geometries. Datatype is out number.
--   max_y   largest y value for all geometries. Datatype is out number.
--

  PROCEDURE extent_of (layer       VARCHAR2,
                       min_x       OUT NUMBER,
                       max_x       OUT NUMBER,
                       min_y       OUT NUMBER,
                       max_y       OUT NUMBER);


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates the average MBR (minimal bounding rectangle)
--   for a layer by keeping track of the max and min x and y values for all
--   geometries in the layer.
--
-- Syntax
--   AVERAGE_MBR (layer, width, height)
--
-- Keywords and Paramters
--   layer   is the name of the spatial layer for which the average MBR is to
--           be determined. Datatype is VARCHAR2.
--   width   width of the average MBR. Datatype is out number.
--   height  height of the average MBR. Datatype is out number.
--

  PROCEDURE average_mbr (layer      VARCHAR2,
                         width      OUT NUMBER,
                         height     OUT NUMBER);


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates statistical histograms for a spatial layer.
--   Information from such histograms could be helpful to determine proper
--   spatial index type (fixed, variable, or hybrid) and index parameters
--   for the layer.
--
-- Syntax
--   HISTOGRAM_ANALYSIS (layer, result_table, type_of_histogram,
--                       max_value, intervals)
--
-- Keywords and Paramters
--   layer               the name of the spatial layer for which the histogram
--                       is to be computed. Datatype is VARCHAR2.
--   result_table        result table name where the histogram is stored. User
--                       must create this table before this procedure is
--                       called. This table contains two columnsof type
--                       NUMBER: 'value' and 'count'. For an example,
--                       CREATE TABLE histogram (value NUMBER, count NUMBER);
--   type_of_histogram   There are three types of histograms:
--     TILES_VS_LEVEL    Number of tiles at different spatial index level.
--                       This histogram is used to evaluate the spatial index
--                       that is already built on the layer.
--     GEOMS_VS_TILES    Number of geometries in different #tiles range.
--                       This histogram is used to evaluate the spatial index
--                       that is already built on the layer.
--     GEOMS_VS_AREA     Number of geometries in different size range. The
--                       shape of this histogram could be hepful for users
--                       to choose a proper index type and index level.
--     GEOMS_VS_VERTICES A histogram of geometry count vs the number of
--                       vertices (complexity) could tell users if spatial
--                       index selectivity is important for the layer. Since
--                       the number of vertices determine the performance of
--                       secondary filter (RELATE), selectivity of the primary
--                       filter could be crucial for layer that contains many
--                       complicated geometries.
--   max_value           The upper limit of the histogram. i.e. The histogram
--                       runs in range (0, max_value).
--   intervals           Number of intervals between 0 and max_value.
--
/* DEPRECATED
  PROCEDURE histogram_analysis (layer              VARCHAR2,
                                result_table       VARCHAR2,
                                type_of_histogram  VARCHAR2 := 'TILES_VS_LEVEL',
                                max_value          NUMBER,
                                intervals          INTEGER);
*/


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates geometry type information for the layer. It
--   outputs the number of geometries of different type, as well as the
--   percentages of points, linestrings, polygons, and complex geometries.
--
-- Syntax
--   MIX_INFO (layer, total_geom, point_geom, line_geom, poly_geom,
--             complex_geom)
--
-- Keywords and Paramters
--   layer        the name of the spatial layer for which the geometry type
--                information is to be calculated. Datatype is VARCHAR2.
--   total_geom   the total number of geometries. Datatype is OUT NUMBER.
--   point_geom   the number of point geometries. Datatype is OUT NUMBER.
--   line_geom    the number of linestring geometries. Datatype is OUT NUMBER.
--   poly_geom    the number of polygon geometries. Datatype is OUT NUMBER.
--   complex_geom the number of complex geometries. Datatype is OUT NUMBER.
--

  PROCEDURE mix_info (layer         VARCHAR2,
                      total_geom    OUT INTEGER,
                      point_geom    OUT INTEGER,
                      line_geom     OUT INTEGER,
                      poly_geom     OUT INTEGER,
                      complex_geom  OUT INTEGER);


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates geometry type information for the layer. It
--   prints out the number of geometries of different type, as well as the
--   percentages of points, linestrings, polygons, and complex geometries.
--
-- Syntax
--   MIX_INFO (layer)
--
-- Keywords and Paramters
--   layer     the name of the spatial layer for which the geometry type
--             information is to be calculated. Datatype is VARCHAR2.
--

  PROCEDURE mix_info (layer     VARCHAR2);


------------------------------------------------------------------------------
-- Purpose
--   This function estimates tiling time for a layer. The estimate is based
--   on the tiling time of a small sample layer that is automatically
--   generated from the original layer. User must have create-table privilege
--   to call this function. The sample layer will be cleaned up before this
--   funtion exits.
--
-- Syntax
--   ESTIMATE_TILING_TIME (layer, sample_ratio, tiling_level, num_tiles)
--
-- Keywords and Paramters
--   layer         the name of the spatial layer for which the tiling time is
--                 to be estimated. Datatype is VARCHAR2.
--   sample_ratio  The size ratio between the original layer and the sample
--                 layer (to be generated). Larger ratio means faster but
--                 less accurate estimation. Datatype is INTEGER.
--   tiling_level  The spatial index level at which the layer is to be
--                 tessellated. Datatype is INTEGER.
--   num_tiles     The number of tiles for variable or hybrid tessellation.
--                 Should be NULL for fixed tessellation. Datatype is INTEGER.
--
-- Returns
--   This function returns a number representing estimated tiling time in
--   seconds, or 0 is returned on error.
--
/* DEPRECATED
  FUNCTION estimate_tiling_time (layer          VARCHAR2,
                                 sample_ratio   INTEGER := 20,
                                 tiling_level   INTEGER,
                                 num_tiles      INTEGER := NULL)
  RETURN NUMBER;
*/


------------------------------------------------------------------------------
-- Purpose
--   This function estimates the total number of spatial tiles for a layer.
--   The estimate is based on the total number of tiles for a small sample
--   layer that is automatically generated from the original layer. User
--   must have create-table privilege to call this function. The sample
--   layer will be cleaned up before this funtion exits.
--
-- Syntax
--   ESTIMATE_TOTAL_NUMTILES (layer, sample_ratio, tiling_level, num_tiles,
--                            num_largetiles)
--
-- Keywords and Paramters
--   layer           the name of the spatial layer for which the tiling time
--                   is to be estimated. Datatype is VARCHAR2.
--   sample_ratio    The size ratio between the original layer and the sample
--                   layer (to be generated). Larger ratio means faster but
--                   less accurate estimation. Datatype is INTEGER.
--   tiling_level    The spatial index level at which the layer is to be
--                   tessellated. Datatype is INTEGER.
--   num_tiles       The number of tiles for variable or hybrid tessellation.
--                   Should be NULL for fixed tessellation. Datatype is
--                   INTEGER.
--   num_largetiles  An output parameter indicating how many spatial tiles
--                   are of the same size as group tiles for hybrid indexing.
--                   For fixed indexing, this value should always be the same
--                   as the returned value (total number of spatial tiles).
--                   Datatype is OUT INTEGER.
--
-- Returns
--   This function returns an integer representing estimated total number of
--   spatial tiles, or 0 is returned on error.
--
/* DEPRECATED
  FUNCTION estimate_total_numtiles (layer           VARCHAR2,
                                    sample_ratio    INTEGER := 20,
                                    tiling_level    INTEGER,
                                    num_tiles       INTEGER := NULL,
                                    num_largetiles  OUT INTEGER)
  RETURN INTEGER;
*/


------------------------------------------------------------------------------
-- Purpose
--   This function estimates spatial index performance such as query
--   selectivity and window query time for a layer. The estimate is based
--   on the window query time of a small sample layer that is automatically
--   generated from the original layer. User must have create-table privilege
--   to call this function. The sample layer will be cleaned up before this
--   funtion exits.
--
-- Syntax
--   ESTIMATE_INDEX_PERFORMANCE (layer, sample_ratio, tiling_level, num_tiles,
--                               window_layer, window_gid, tiling_time,
--                               filter_time, query_time)
--
-- Keywords and Paramters
--   layer         the name of the spatial layer for which the tiling time is
--                 to be estimated. Datatype is VARCHAR2.
--   sample_ratio  The size ratio between the original layer and the sample
--                 layer (to be generated). Larger ratio means faster but
--                 less accurate estimation. Datatype is INTEGER.
--   tiling_level  The spatial index level at which the layer is to be
--                 tessellated. Datatype is INTEGER.
--   num_tiles     The number of tiles for variable or hybrid tessellation.
--                 Should be NULL for fixed tessellation. Datatype is INTEGER.
--   window_layer  The name of the spatial layer in which window geometry is
--                 stored. Datatype is VARCHAR2.
--   window_gid    The window geometry ID. Datatype is NUMBER.
--   tiling_time   The estimated tiling time in seconds. Datatype is OUT
--                 NUMBER.
--   filter_time   The estimated spatial index filter time in seconds.
--                 Datatype is OUT NUMBER.
--   query_time    The estimated window query time in seconds. Datatype is
--                 OUT NUMBER.
--
-- Returns
--   This function returns a number between 0.0 and 1.0 representing estimated
--   spatial index selectivity (large number means better selectivity, and 0.0
--   implies an error).
--
/*  DEPRECATED
  FUNCTION estimate_index_performance (layer          VARCHAR2,
                                       sample_ratio   INTEGER := 20,
                                       tiling_level   INTEGER,
                                       num_tiles      INTEGER := NULL,
                                       window_layer   VARCHAR2,
                                       window_gid     NUMBER,
                                       tiling_time    OUT NUMBER,
                                       filter_time    OUT NUMBER,
                                       query_time     OUT NUMBER)
  RETURN NUMBER;
*/


--==========================================================================--
-- For object schema:
--==========================================================================--

------------------------------------------------------------------------------
-- Purpose
--   This procedure estimates which level to use when indexing geometry
--   objects using the fixed tile method.
--
-- Syntax
--   ESTIMATE_TILING_LEVEL (table_name, column_name, maxtiles,
--                          type_of_estimate)
--
-- Keywords and Parameters
--   table_name        is the name of the geometry object table. Datatype is
--                     VARCHAR2.
--   column_name       is the name of the geometry object column for which the
--                     tile level is to be determined. Datatype is VARCHAR2.
--   maxtiles          is the maximum number of valid tiles that can be used
--                     to index the type_of_estimate object defined below.
--                     Datatype is INTEGER.
--   type_of_estimate  There are three different type_of_estimate objects
--                     that can be defined:
--     LAYER_EXTENT    defines the type_of_estimate object to be the rectangle
--                     defined by the users coordinate system.
--     ALL_GID_EXTNET  defines the type_of_estimate object to be the rectangle
--                     that defines the bounds of all the SDO_GID's of a layer.
--     AVG_GID_EXTENT  defines the type_of_estimate object to be the rectangle
--                     that represents the average individual geometry bounding
--                     box. The average bounding box is determined by sampling
--                     every geometry in the specified layer. Datatype is
--                     VARCHAR2.
--
-- Returns
--   This function returns an integer representing the level used to index the
--   specified layer.
--
--   0 is returned if the data is inconsistent:
--   e.g. invalid combination of table name and column name.
--   This needs to be fixed by adding more error checking in future versions.
-- Usage Notes
--   Using the AVG_GID_EXTENT performs the most analysis of the three types.
--
/* DEPRECATED
  FUNCTION estimate_tiling_level(table_name       VARCHAR2,
                                 column_name      VARCHAR2,
                                 num_tiles        INTEGER,
                                 type_of_estimate VARCHAR2 := 'AVG_GID_EXTENT')
  RETURN INTEGER;
*/


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates the extents (known as Minimal Bounding
--   Rectangle, or MBR) of all geometry objects in the specified table column.
--
-- Syntax
--   EXTENT_OF (table_name, column_name)
--
-- Keywords and Paramters
--   table_name   is the name of the geometry object table. Datatype is
--                VARCHAR2.
--   column_name  is the name of the geometry object column for which the
--                extent is to be determined. Datatype is VARCHAR2.
--
-- Returns
--   This function returns a geometry object representing extent (MBR) of the
--   specified geometry object column.
--
--   NULL is returned if the data is inconsistent:
--   e.g. invalid combination of table name and column name.
--   This needs to be fixed by adding more error checking in future versions.
--

  FUNCTION extent_of(table_name      VARCHAR2,
                     column_name     VARCHAR2,
		     use_index       VARCHAR2 default 'TRUE')
  RETURN MDSYS.SDO_GEOMETRY;


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates the average MBR (minimal bounding rectangle)
--   for a geometry object column by keeping track of the max and min x and y
--   values for all geometry objects in the column.
--
-- Syntax
--   AVERAGE_MBR (table_name, column_name, width, height)
--
-- Keywords and Paramters
--   table_name   the name of the geometry object table. Datatype is VARCHAR2.
--   column_name  the name of the geometry object column for which the
--                average MBR is to be calculated. Datatype is VARCHAR2.
--   width        width of the average MBR. Datatype is OUT NUMBER.
--   height       height of the average MBR. Datatype is OUT NUMBER.
--

  PROCEDURE average_mbr (table_name      VARCHAR2,
                         column_name     VARCHAR2,
                         width           OUT NUMBER,
                         height          OUT NUMBER);


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates statistical histograms for a geometry object
--   column. Information from such histograms could be helpful to determine
--   proper spatial index type (fixed, variable, or hybrid) and index
--   parameters for the geometry column.
--
-- Syntax
--   HISTOGRAM_ANALYSIS (table_name, column_name, result_table,
--                       type_of_histogram, max_value, intervals)
--
-- Keywords and Paramters
--   table_name          the name of the geometry object table.
--   column_name         the name of the geometry object column for which the
--                       histogram is to be computed.
--   result_table        result table name where the histogram is stored. User
--                       must create this table before this procedure is
--                       called. This table contains two columnsof type
--                       NUMBER: 'value' and 'count'. For an example,
--                       CREATE TABLE histogram (value NUMBER, count NUMBER);
--   type_of_histogram   There are three types of histograms:
--     TILES_VS_LEVEL    Number of tiles at different spatial index level.
--                       This histogram is used to evaluate the spatial index
--                       that is already built on the geometry column.
--     GEOMS_VS_TILES    Number of geometries in different #tiles range.
--                       This histogram is used to evaluate the spatial index
--                       that is already built on the geometry column.
--     GEOMS_VS_AREA     Number of geometries in different size range. The
--                       shape of this histogram could be hepful for users
--                       to choose a proper index type and index level.
--     GEOMS_VS_VERTICES A histogram of geometry count vs the number of
--                       vertices (complexity) could tell users if spatial
--                       index selectivity is important for the column. Since
--                       the number of vertices determine the performance of
--                       secondary filter (RELATE), selectivity of the primary
--                       filter could be crucial for geometry column that
--                       contains many complicated geometries.
--   max_value           The upper limit of the histogram. i.e. The histogram
--                       runs in range (0, max_value).
--   intervals           Number of intervals between 0 and max_value.
--
/* DEPRECATED
  PROCEDURE histogram_analysis (table_name         VARCHAR2,
                                column_name        VARCHAR2,
                                result_table       VARCHAR2,
                                type_of_histogram  VARCHAR2 := 'TILES_VS_LEVEL',
                                max_value          NUMBER,
                                intervals          INTEGER);
*/


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates geometry type information for the geometry
--   object column. It outputs the number of geometries of different type,
--   as well as the percentages for various geometry types.
--
-- Syntax
--   MIX_INFO (table_name, column_name, total_geom, point_geom, curve_geom,
--             poly_geom, complex_geom)
--
-- Keywords and Paramters
--   table_name   the name of the geometry object table. Datatype is VARCHAR2.
--   column_name  the name of the geometry object column for which the
--                geometry type infomation is to be calculated. Datatype is
--                VARCHAR2.
--   total_geom   the total number of geometries. Datatype is OUT NUMBER.
--   point_geom   the number of point geometries. Datatype is OUT NUMBER.
--   curve_geom   the number of curvestring geometries. Datatype is OUT NUMBER.
--   poly_geom    the number of polygon geometries. Datatype is OUT NUMBER.
--   complex_geom the number of complex geometries. Datatype is OUT NUMBER.
--

  PROCEDURE mix_info (table_name    VARCHAR2,
                      column_name   VARCHAR2,
                      total_geom    OUT INTEGER,
                      point_geom    OUT INTEGER,
                      curve_geom    OUT INTEGER,
                      poly_geom     OUT INTEGER,
                      complex_geom  OUT INTEGER);


------------------------------------------------------------------------------
-- Purpose
--   This procedure caclulates geometry type information for the geometry
--   object column. It prints out the number of geometries of different type,
--   as well as the percentages for various geometry types.
--
-- Syntax
--   MIX_INFO (table_name, column_name)
--
-- Keywords and Paramters
--   table_name   the name of the geometry object table. Datatype is VARCHAR2.
--   column_name  the name of the geometry object column for which the
--                geometry type infomation is to be calculated. Datatype is
--                VARCHAR2.
--

  PROCEDURE mix_info (table_name      VARCHAR2,
                      column_name     VARCHAR2);


------------------------------------------------------------------------------
-- Purpose
--   This function estimates tiling time for a geometry column. The estimate
--   is based on the tiling time of a small sample geometry table that is
--   automatically generated from the original table column. User must have
--   create-table privilege to call this function. The sample table will be
--   cleaned up before this funtion exits.
--
-- Syntax
--   ESTIMATE_TILING_TIME (table_name, column_name, sample_ratio,
--                         tiling_level, num_tiles)
--
-- Keywords and Paramters
--   table_name    The name of the geometry object table. Datatype is VARCHAR2.
--   column_name   The name of the geometry object column for which the
--                 tiling time is to be estimated. Datatype is VARCHAR2.
--   sample_ratio  The size ratio between the original table and the sample
--                 table (to be generated). Larger ratio means faster but
--                 less accurate estimation. Datatype is INTEGER.
--   tiling_level  The spatial index level at which the geometry column is to
--                 be tessellated. Datatype is INTEGER.
--   num_tiles     The number of tiles for variable or hybrid tessellation.
--                 Should be 0 for fixed tessellation. Datatype is INTEGER.
--
-- Returns
--   This function returns a number representing estimated tiling time in
--   seconds, or 0 is returned on error.
--
/* DEPRECATING
  FUNCTION estimate_tiling_time (table_name      VARCHAR2,
                                 column_name     VARCHAR2,
                                 sample_ratio    INTEGER := 20,
                                 tiling_level    INTEGER,
                                 num_tiles       INTEGER := 0)
  RETURN NUMBER;
*/


------------------------------------------------------------------------------
-- Purpose
--   This function estimates the total number of spatial tiles for a geometry
--   column. The estimate is based on the total number of tiles for a small
--   sample geometry table that is automatically generated from the original
--   table column. User must have create-table privilege to call this
--   function. The sample table will be cleaned up before this funtion exits.
--
-- Syntax
--   ESTIMATE_TOTAL_NUMTILES (table_name, column_name, sample_ratio,
--                            tiling_level, num_tiles, num_largetiles)
--
-- Keywords and Paramters
--   table_name      The name of the geometry object table. Datatype is
--                   VARCHAR2.
--   column_name     The name of the geometry object column for which the
--                   tiling time is to be estimated. Datatype is VARCHAR2.
--   sample_ratio    The size ratio between the original layer and the sample
--                   layer (to be generated). Larger ratio means faster but
--                   less accurate estimation. Datatype is INTEGER.
--   tiling_level    The spatial index level at which the layer is to be
--                   tessellated. Datatype is INTEGER.
--   num_tiles       The number of tiles for variable or hybrid tessellation.
--                   Should be 0 for fixed tessellation. Datatype is INTEGER.
--   num_largetiles  An output parameter indicating how many spatial tiles
--                   are of the same size as group tiles for hybrid indexing.
--                   For fixed indexing, this value should always be the same
--                   as the returned value (total number of spatial tiles).
--                   Datatype is OUT INTEGER.
--
-- Returns
--   This function returns an integer representing estimated total number of
--   spatial tiles, or 0 is returned on error.
--
/* DEPRECATED
  FUNCTION estimate_total_numtiles (table_name      VARCHAR2,
                                    column_name     VARCHAR2,
                                    sample_ratio    INTEGER := 20,
                                    tiling_level    INTEGER,
                                    num_tiles       INTEGER := 0,
                                    num_largetiles  OUT INTEGER)
  RETURN INTEGER;
*/

------------------------------------------------------------------------------
-- Purpose
--   This function estimates spatial index performance such as query
--   selectivity and window query time for a geometry column. The estimate
--   is based on the window query time of a small sample geometry table that
--   is automatically generated from the original table column. User must
--   have create-table privilege to call this function. The sample table will
--   be cleaned up before this funtion exits.
--
-- Syntax
--   ESTIMATE_INDEX_PERFORMANCE (table_name, column_name, sample_ratio,
--                               tiling_level, num_tiles, window_obj,
--                               tiling_time, filter_time, query_time)
--
-- Keywords and Paramters
--   table_name    The name of the geometry object table. Datatype is VARCHAR2.
--   column_name   The name of the geometry object column for which the
--                 tiling time is to be estimated. Datatype is VARCHAR2.
--   sample_ratio  The size ratio between the original table and the sample
--                 table (to be generated). Larger ratio means faster but
--                 less accurate estimation. Datatype is INTEGER.
--   tiling_level  The spatial index level at which the geometry column is to
--                 be tessellated. Datatype is INTEGER.
--   num_tiles     The number of tiles for variable or hybrid tessellation.
--                 Should be 0 for fixed tessellation. Datatype is INTEGER.
--   window_obj    The window geometry object. Datatype is MDSYS.SDO_GEOMETRY.
--   tiling_time   The estimated tiling time in seconds. Datatype is OUT
--                 NUMBER.
--   filter_time   The estimated spatial index filter time in seconds.
--                 Datatype is OUT NUMBER.
--   query_time    The estimated window query time in seconds. Datatype is
--                 OUT NUMBER.
--
-- Returns
--   This function returns a number between 0.0 and 1.0 representing estimated
--   spatial index selectivity (large number means better selectivity, and 0.0
--   implies an error).
--
/* DEPRECATED
  FUNCTION estimate_index_performance (table_name      VARCHAR2,
                                       column_name     VARCHAR2,
                                       sample_ratio    INTEGER := 20,
                                       tiling_level    INTEGER,
                                       num_tiles       INTEGER := 0,
                                       window_obj      MDSYS.SDO_GEOMETRY,
                                       tiling_time     OUT NUMBER,
                                       filter_time     OUT NUMBER,
                                       query_time      OUT NUMBER)
  RETURN NUMBER;
*/

------------------------------------------------------------------------------
-- Purpose
--   This function estimates the quality_degradation of an R-tree index.
--   This computes the ratio of the current quality and the quality
--   at the time when the R-tree is last built/rebuilt.
--   This number gives an estimate of how much the query performance
--   degrades (i.e., how slow queries become) in comparison to
--   queries at index creation time. The quality could have
--   degraded due to updates between tree-creation time and
--   current time.
--
--   The quality of an R-tree is estimated as the ratio of the
--   sum of the areas of all index_level mbrs to the ((sum of the  area
--   of all mbrs at leaf level, i.e., data mbrs) times the height
--   of the tree.
--
-- Syntax
--   QUALITY_DEGRADATAION(schema_name, index_name, index_table_name)
--
-- Keywords and Paramters
--   schema_name   The name of the user's schema. Datatype is VARCHAR2.
--   index_name    The name of the index to be analyzed. Datatype is VARCHAR2.
--   index_table   The name of the R-tree index table. The R-tree can
--                 be either a primary or a secondary index for
--                 the specified index_name. Datatype is VARCHAR2.
--
-- Returns
--   This function returns a number that represents the degradation
--   factor (small number close to 1 means index is very efficient;
--   large numbers may mean index-quality has degraded; rebuilding
--   the index may improve the quality

  FUNCTION quality_degradation(schemaname varchar2,
                               indexname varchar2,
                               indextable varchar2)
  RETURN number;

------------------------------------------------------------------------------
-- Purpose
--  Same as above but indextable is not specified. Whole index is analyzed.

  FUNCTION quality_degradation(schemaname varchar2,
                               indexname varchar2)
  RETURN number;


------------------------------------------------------------------------------
-- Purpose
--   This procedure prints the quality for a specified R-tree index
--
-- Syntax
--   RTREE_QUALITY(schema_name, index_name, index_table_name)
--
-- Keywords and Paramters
--   schema_name   The name of the user's schema.
--                 Datatype is VARCHAR2.
--
--   index_name    The name of the index to be analyzed.
--                 Datatype is VARCHAR2.
--
--   index_table   The specific R-tree index for the index.
--                 Datatype is VARCHAR2.
--


  PROCEDURE analyze_rtree(schemaname varchar2,
                          indexname varchar2);

------------------------------------------------------------------------------
-- Purpose
--   This procedure prints just the quality for a specified R-tree index
--   (does not compare with the previous quality in the metadata)
--
-- Syntax
--   RTREE_QUALITY(schema_name, index_name, index_table_name)
--
-- Keywords and Paramters
--   schema_name   The name of the user's schema.
--                 Datatype is VARCHAR2.
--
--   index_name    The name of the index to be analyzed.
--                 Datatype is VARCHAR2.
--
--   index_table   The specific R-tree index for the index.
--                 Datatype is VARCHAR2.
--

  FUNCTION rtree_quality(schemaname varchar2,
                         indexname varchar2,
                         indextable varchar2)
  RETURN number;

------------------------------------------------------------------------------
-- Purpose
--  Same as above but indextable is not specified. Whole index is analyzed.

  FUNCTION rtree_quality(schemaname varchar2,
                         indexname varchar2)
  RETURN number;


---------------------------------------------------------------------------
-- Purpose
--
--   Function to estimate size of an R-tree index (table) in MB.
--   (tablespace needed at creation times is 3 times this size)
--
-- Syntax
--
--   estimate_rtree_index_size(number_of_geoms, db_block_size,
-- 			       sdo_rtr_pctfree, num_dimensions, is_geodetic)
--
-- Keywords and Parameters:
--
--   number_of_geoms - number of non-null geometries to be indexed
--   db_block_size   - block size of the underlying tablespace/database
--   sdo_rtr_pctfree - the percentage of space to be left empty in each node.
--                     This empty space can be consumed by subsequent inserts.
--                     If unspecified, the default in Oracle Spatial is 10
--   num_dimensions  - the number of dimensions in the
--                     data - NOTE: if geodetic, use 3
--
-- Returns: Size in MB.

  FUNCTION estimate_rtree_index_size (number_of_geoms integer,
                                      db_block_size integer,
                                      sdo_rtr_pctfree integer default 10,
                                      num_dimensions  integer default 2,
				      is_geodetic number default 0)
  RETURN number;

---------------------------------------------------------------------------
-- Purpose
--
--   Function to approximately compute R-tree index size (in MB)
--
-- Syntax
--
--   estimate_rtree_index_size(schema, tabname, colname, partname)
--
-- Keywords and Parameters:
--
--   schema          - user's schema
--   tabname         - table on which R-tree index needs to be constructed
--   colname         - column on which R-tree index needs to be constructed
--   partname        - name of partition for which to build the index
--
-- Returns: Size in MB

  FUNCTION estimate_rtree_index_size (schema varchar2,
                                      tabname varchar2,
				      colname varchar2,
				      partname varchar2 default null)
  RETURN number;

END sdo_tune;
/


Prompt Synonym SDO_TUNE;
--
-- SDO_TUNE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_TUNE FOR MDSYS.SDO_TUNE
/


Prompt Grants on PACKAGE SDO_TUNE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_TUNE TO PUBLIC
/
