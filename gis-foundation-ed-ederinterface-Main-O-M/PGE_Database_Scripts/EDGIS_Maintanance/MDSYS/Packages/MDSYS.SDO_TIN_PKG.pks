Prompt drop Package SDO_TIN_PKG;
DROP PACKAGE MDSYS.SDO_TIN_PKG
/

Prompt Package SDO_TIN_PKG;
--
-- SDO_TIN_PKG  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_tin_pkg authid current_user AS
  -- all tables as "schema.table" for simpler interface.
  FUNCTION INIT(basetable varchar2, basecol varchar2,
       blktable varchar2,
       ptn_params varchar2 default null,
       tin_extent            sdo_geometry default null,
       tin_tol               NUMBER default 0.000000000000005,
       tin_tot_dimensions    NUMBER default 2,
       tin_domain            sdo_orgscl_type default null,
       tin_break_lines       SDO_GEOMETRY default null,
       tin_stop_lines        SDO_GEOMETRY default null,
       tin_void_rgns         SDO_GEOMETRY default null,
       tin_val_attr_tables   SDO_STRING_ARRAY default null,
       tin_other_attrs       SYS.XMLTYPE default null)
    RETURN SDO_TIN ;

  PROCEDURE CREATE_TIN(inp sdo_tin, inptable varchar2,
		       clstpcdatatbl varchar2 default null);

  -- works as read if qry is null
  FUNCTION CLIP_TIN(inp sdo_tin,
                   qry sdo_geometry, qry_min_res number, qry_max_res number,
                   blkno number default null)
    RETURN MDSYS.SDO_TIN_BLK_TYPE PIPELINED ;

  PROCEDURE DROP_DEPENDENCIES(basetable varchar2, col varchar2);

  FUNCTION TO_GEOMETRY(pts BLOB, trs BLOB,
                       num_pts NUMBER, num_trs NUMBER,
		       tin_ind_dim NUMBER,
                       tin_tot_dim NUMBER, srid number default null,
                       blk_domain sdo_orgscl_type default null,
                       get_ids NUMBER default NULL)
    RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;
--  PRAGMA RESTRICT_REFERENCES(to_geometry, WNDS, WNPS, RNPS);



/*

  -- works as read if qry is null
  FUNCTION CLIP_Pts(inp sdo_tin,
	            qry sdo_geometry,
	            qry_min_res number, qry_max_res number,
	            blkno number default null)
    RETURN ANYDATASET;
  FUNCTION CLIP_PtIds(inp sdo_tin,
	            qry sdo_geometry,
	            qry_min_res number, qry_max_res number)
    RETURN ANYDATASET;

  -- works as read if qry is null
  FUNCTION CLIP_Triangles(inp sdo_tin, qry sdo_geometry,
	            qry_min_res number, qry_max_res number,
		    blkno number default null)
    RETURN ANYDATASET;
  FUNCTION CLIP_TriangleIds(inp sdo_tin, qry sdo_geometry,
	            qry_min_res number, qry_max_res number,
		    blkno number default null)
    RETURN ANYDATASET;
*/
END sdo_tin_pkg;
/


Prompt Synonym SDO_TIN_PKG;
--
-- SDO_TIN_PKG  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_TIN_PKG FOR MDSYS.SDO_TIN_PKG
/


Prompt Grants on PACKAGE SDO_TIN_PKG TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_TIN_PKG TO PUBLIC
/
