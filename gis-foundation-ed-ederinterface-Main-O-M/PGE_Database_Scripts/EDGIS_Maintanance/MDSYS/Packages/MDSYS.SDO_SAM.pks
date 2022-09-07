Prompt drop Package SDO_SAM;
DROP PACKAGE MDSYS.SDO_SAM
/

Prompt Package SDO_SAM;
--
-- SDO_SAM  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_sam AUTHID current_user AS

----------------------------------------------------------------------
-- Name
--  Predicated_Join
--
-- Purpose
--   This routine allows partial predicate-based join of tables.
--   Useful in finding co-located items of different categories
--   from the same or different tables.
--
--  Syntax
--    Predicated_Join(table1, col1, table2, col2, pred1, pred2, dst_spec)
--    table1, col1, pred1: refer to first table, location column and
--                         associated predicate to filter rows from table1
--    table2, col2, pred2: refer to second table, location column and
--                         associated predicate to filter rows from table2
--    dst_spec: is of the form "distance=1 units=mile" (just as in
--              regular spatial query windows.
--
--  Usage
--    select a.state_abrv from states a,
--      table(sdo_sam.Predicated_Join('STATES', 'GEOM',
--        'STATES', 'GEOM',  'state_abrv=''NH''', null, null)) b
--          where a.rowid = b.rid2;


  FUNCTION Predicated_Join(table_name1 varchar2, column_name1 varchar2,
                           table_name2 varchar2, column_name2 varchar2,
                           tab1_predicate varchar2, tab2_predicate varchar2,
                           dst_spec in varchar2)
  RETURN MDSYS.SDO_ROWIDSET
  PIPELINED;

------------------------------------------------------------------------------
-- Name
--   Simplify_Geometry
--
-- Purpose
--   This procedure simplifies a geometry without actually having to specify
--   thresholds. This routine iteratively invokes sdo_util.simplify
--   starting with a threshold of tolerance and increasing it by a
--   factor of threshold_incrmnts in each iteration. The recursive
--   invocation stops when the area of the resulting geometry
--   decreases by at most the specified "pct_area_chng_limit", i.e., when
--   (area(orig_geom)-area(result_geom))/area(orig_geom) < pct_area_chng_limit/100.
--
-- Syntax
--   Simplify_Geometry (geom, dim, pct_area_chang_limit)
--
-- Keywords and Paramters
--   geom 	       	    geometry to be simplified. Datatype is MDSYS.sdo_geometry.
--   dim	            diminfo containing tolerance info.
--                          Datatype is mdsys.sdo_dim_array.
--   pct_area_change_limit: Stops simplifying  when area of resulting
--                          geometry changes by at most this percentage.
--
-- Usage
-- select sdo_sam.simplify_geometry(a.geometry, b.diminfo)
--   from poly_4pt_temp a, user_sdo_geom_metadata b
--     where b.table_name='POLY_4PT_TEMP' ;



  FUNCTION simplify_geometry(geom mdsys.sdo_geometry,
                             dim mdsys.sdo_dim_array,
			     pct_area_change_limit number default 2)
  RETURN mdsys.sdo_geometry  DETERMINISTIC PARALLEL_ENABLE;

-- Tolerance as a parameter
  FUNCTION simplify_geometry(geom mdsys.sdo_geometry,
                             tol number,
			     pct_area_change_limit number default 2)
  RETURN mdsys.sdo_geometry  DETERMINISTIC PARALLEL_ENABLE;


-- Name
--   Simplify_Layer
--
-- Purpose
--   The "simplify_geometry" is applied to all geometries in a layer.
--   The simplified geometry is re-inserted in the table in column
--   "smpl_geom_colname". The commit_interval specifies the interval
--   at which each batch of simplified-geom inserts are committed.
--
-- Syntax
--   Simplify_Layer (theme_tablename, theme_colname,
--                   smpl_geom_colname, commit_interval, pct_area_change_limit)
--
-- Keywords and Paramters
--   theme_tablename	Name of table. Datatype varchar2
--   theme_colname	Geometry column in table. Datatype varchar2
--   smpl_geom_colname  Name of column to store simplified_geometry.
--                      Datatype varchar2.
--
--   Rest of the parameters below are for controlling the # of iterations
--
-- Usage
-- execute sdo_sam.simplify_layer('POLY_4PT_TEMP', 'GEOMETRY', 'SMPL_GEOM');


  PROCEDURE simplify_layer(theme_tablename varchar2,
                           theme_colname varchar2,
                           smpl_geom_colname varchar2,
			   commit_interval number default 10,
			   pct_area_change_limit number default 2);



------------------------------------------------------------------------------
-- Name
--   Intersection_Ratio
--
-- Purpose
--   This routine first computes the intersection of location,thematic_boundary
--   geometries. The ratio of the  area of this intersection to the
--   area of the "thematic_boundary" is computed and returned.
--
--   If dist and units are specified, then the location geometry
--   is "buffered" using the specified distance and the intersection_ratio
--   of the buffered_location to thematic_boundary is returned.
--
-- Syntax
--   Intersection_Ratio(location, thematic_boundary, dim,
--                        dist,  units);
-- Usage
--   select sdo_sam.intersection_ratio(a.geometry, b.geometry,
--     c.diminfo) from poly_4pt_temp a, tst b, user_sdo_geom_metadata c
--       where c.table_name = 'POLY_4PT_TEMP';

  FUNCTION Intersection_Ratio(location mdsys.sdo_geometry,
                              thematic_boundary mdsys.sdo_geometry,
                              dim mdsys.sdo_dim_array,
                              dist number default null,
                              units varchar2 default null)
  RETURN number PARALLEL_ENABLE;


------------------------------------------------------------------------------
-- Name
--   Tiled_Bins
--
-- Purpose
--   This routine tiles a 2-d space and returns geometries corresponding
--   to those tiles. The returned sdo_region object has (the tile_id,
--   tile_code (quadtree-code), tile_geometry) corresponding to each tile.
--   If srid is non-null, it is set in the tile_geometry.
--   If xdivs, ydivs (# of divisions in each dimension) are specified,
--   then tiling_level  is ignored.
-- Syntax
--   Tiled_Bins(l1, u1, l2, u2, srid)
--
-- Usage
--   select * from TABLE(sdo_sam.tiled_bins(-180, 180, -90, 90, 1, 0));



  FUNCTION Tiled_Bins(l1 number, u1 number, l2 number, u2 number,
                      tiling_level number default null,
		      srid number default null,
		      xdivs number default null, ydivs number default null)
  RETURN MDSYS.SDO_REGIONSET
  PIPELINED;


------------------------------------------------------------------------------
-- Name
--   Bin_Geometry
--
-- Purpose
--   This routine bins the geometry to the most-intersecting bin
--   from a bin_tablename.
--
-- Syntax
--   Bin_Geometry(geom, dim, bin_tablename, bin_colname)
--
-- Usage
--   select sdo_sam.bin_geometry(a.geometry, b.diminfo, 'BINTBL', 'GEOMETRY')
--     from poly_4pt a, user_sdo_geom_metadata b
--       where b.table_name='POLY_4PT' and a.gid=1;

  FUNCTION Bin_Geometry(geom mdsys.sdo_geometry, dim mdsys.sdo_dim_array,
                        bin_tablename varchar2, bin_colname in varchar2)
  RETURN NUMBER PARALLEL_ENABLE;


-- Signature usign tolerance instead of dim
  FUNCTION Bin_Geometry(geom mdsys.sdo_geometry, tol number,
                        bin_tablename varchar2, bin_colname in varchar2)
  RETURN NUMBER PARALLEL_ENABLE;

-- Name
--  Bin_Layer
--
-- Purpose
--   This routine computes the most-intersecting bin for each geomtry row
--   in a specified layer using the bins in "bin_tablename".
--   The bin_id for each geometry is added in the "bin_id_colname".
--   Commit_intrvl specifies the batch-size for the inserts.
--
-- Syntax
--   Bin_Layer(tablename, colname, bin_tablename, bin_colname,
--             bin_id_colname, commit_intrvl)
--
-- Usage
--   execute sdo_sam.bin_layer('POLY_4PT_TEMP', 'GEOMETRY', 'BINTBL',
--                             'GEOMETRY', ' BIN_ID', 1);


  PROCEDURE Bin_Layer(tablename varchar2, colname varchar2,
                      bin_tablename varchar2, bin_colname varchar2,
                      bin_id_colname varchar2,
			commit_interval number default 20);



------------------------------------------------------------------------------
-- Name
--   Spatial_Clusters
--
-- Purpose
--   This routine computes clusters using existing R-tree index.
--   The parameter max_clusters specifies the number of clusters to
--   obtain. Returns sdo_region objects where the geometry field
--   specifies the cluster boundary of each cluster. (The geometry_key
--   is set to null).
--
-- Syntax
--  Spatial_Clusters(tablename, colname, max_clusters,
--                   allow_outliers, tablepartition)
--
-- Usage
--   select count(*) from
--     TABLE(sdo_sam.Spatial_Clusters('POLY_4PT_TEMP', 'GEOMETRY', 3));

  FUNCTION Spatial_Clusters(tablename varchar2, colname varchar2,
                            max_clusters number,
		            allow_outliers varchar2 default null,
                            tablepartition varchar2 default null)
  RETURN MDSYS.SDO_REGIONSET
  PIPELINED;

------------------------------------------------------------------------------
-- Name
--   Aggregates For Layer
--
-- Purpose
--   Compute thematic aggregates for a layer of rows.
--   For each geometry in  tablename, identify the geometries in
--   theme_name table, find their intersection_ratio, and multiply
--   the specified aggregate using this intersection_ratio and aggregate
--   it for each geometry in tablename.
--
--   Returns a table of geometry(from tablename table), aggregate_value using
--   the sdo_regaggr object.
--
-- Syntax
--   Aggregate_For_Layer(theme_name, theme_colname, tablename, colname,
--		         aggr_type_string, aggr_col_string, dst_spec)
--
-- Usage
--   select * from TABLE(sdo_sam.Aggregate_For_Layer('POLY_4PT_TEMP',
--                       'GEOMETRY', 'SUM', '1', 'TST', 'GEOMETRY'));
--   select * from TABLE(sdo_sam.Aggregate_For_Layer('POLY_4PT_TEMP',
--                      'GEOMETRY', 'SUM', '1','TST', 'GEOMETRY','distance=2'));
--   select * from TABLE(sdo_sam.Aggregate_For_Layer('POLY_4PT_TEMP',
--                       'GEOMETRY', 'SUM', '1',  'TST', 'GEOMETRY',
--                       'distance=2 UNIT=MILE'));

  FUNCTION Aggregates_For_Layer(theme_name varchar2,
                                theme_colname varchar2,
		                aggr_type_string varchar2,
			        aggr_col_string varchar2,
			        tablename varchar2, colname varchar2,
		                dst_spec varchar2 default null,
      dst_offset number default null, zero_agg_rows number default 0 )
  RETURN MDSYS.SDO_REGAGGRSET
  PIPELINED;

  FUNCTION Aggregates_For_Geometry(theme_name varchar2,
                                   theme_colname varchar2,
                                   aggr_type_string varchar2,
                                   aggr_col_string varchar2,
			           geom mdsys.sdo_geometry,
		                   dst_spec varchar2 default null,
		                   dst_offset number default null)
  RETURN number ;


------------------------------------------------------------------------------
-- Name
--   Tiled Aggregates
--
-- Purpose
--   Similar to thematic aggregates. Instead of explicit tablename
--   the table is dynamically generated using tiling information.
--
--   Given a theme_name table, the tiling domain is determined.
--   Based on the tiling_level, a number of tiles are generated.
--   For each tile geometry, thematic aggregates are computed
--   as described above.
--   Parameter zero_agg_tiles, if 0, skips (does not return)
--   tiles that have aggregate value of 0. Default is 0.
--   Parameter x_divs, y_divs specify the # of divisions if tiling_level
--   is not specified. If both are specified, then tiling_level is ignored.
--
--
--
--   Returns a table of tile_geometry, aggregate_value  using
--   the sdo_regaggr object.
--
-- Syntax
--   Tiled_Aggregates(theme_name, theme_colname,
--                    aggr_type_string, aggr_col_string,
--                    tiling_level, dst_spec, tiling_domain, zero_agg_tiles)
--
-- Usage
-- select * from TABLE(sdo_sam.Tiled_Aggregates('POLY_4PT_TEMP',
--                     'GEOMETRY', 2, 'SUM', '1'));

  FUNCTION Tiled_Aggregates(theme_name varchar2,
                            theme_colname varchar2,
                            aggr_type_string varchar2,
                            aggr_col_string varchar2,
                            tiling_level number default null,
                            tiling_domain mdsys.sdo_dim_array default null,
                            zero_agg_tiles number default 0,
                            xdivs number default null, ydivs number default null)
  RETURN MDSYS.SDO_REGAGGRSET
  PIPELINED;

------------------------------------------------------------------------------
-- Name
--   Colocated Reference Features
--
-- Purpose
--   Same as Predicated_Join. Materialize the join results into
--   a table "result_tablename".  The result_tablename should have
--   the columns (tid number, rid1 number, rid2 number).
--
--   This routine materializes  each pair of rowids returned from
--   Predicated_Join and stores them in the rid1, rid2 columns of the
--   result_tablename. The tid is a unique generated "interaction"
--   number corresponding to each "rid1".
--
  PROCEDURE Colocated_Reference_Features(theme_tablename varchar2,
                                         theme_colname varchar2,
                                         theme_predicate varchar2,
			                 tablename varchar2,
				         colname varchar2,
                                         ref_predicate varchar2,
                                         dst_spec varchar2,
                                         result_tablename varchar2,
                                         commit_interval number default 100);


END sdo_sam;
/


Prompt Synonym SDO_SAM;
--
-- SDO_SAM  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_SAM FOR MDSYS.SDO_SAM
/


Prompt Grants on PACKAGE SDO_SAM TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_SAM TO PUBLIC
/
