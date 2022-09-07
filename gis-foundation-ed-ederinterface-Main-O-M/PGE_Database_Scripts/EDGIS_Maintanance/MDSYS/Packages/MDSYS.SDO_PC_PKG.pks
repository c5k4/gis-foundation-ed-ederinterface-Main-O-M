Prompt drop Package SDO_PC_PKG;
DROP PACKAGE MDSYS.SDO_PC_PKG
/

Prompt Package SDO_PC_PKG;
--
-- SDO_PC_PKG  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_pc_pkg authid current_user AS
  -- all tables as "schema.table" for simpler interface.
  FUNCTION INIT(basetable varchar2, basecol varchar2,
       blktable             VARCHAR2,
       ptn_params varchar2,
       pc_extent            sdo_geometry default null,
       pc_tol               NUMBER default 0.0000000000005,
       pc_tot_dimensions    NUMBER default 2,
       pc_domain            sdo_orgscl_type default null,
       pc_val_attr_tables   SDO_STRING_ARRAY default null,
       pc_other_attrs             SYS.XMLTYPE default null)
    RETURN SDO_PC;

  PROCEDURE CREATE_PC(inp sdo_pc, inptable varchar2,
	              clstpcdatatbl varchar2 default null) ;

  -- works as read if qry params are null
  FUNCTION CLIP_PC(inp sdo_pc,
                   ind_dim_qry sdo_geometry,
                   other_dim_qry sdo_mbr,
		   qry_min_res number, qry_max_res number,
                   blkno number default null)
    RETURN MDSYS.SDO_PC_BLK_TYPE PIPELINED;

  PROCEDURE DROP_DEPENDENCIES(basetable varchar2, col varchar2);

  FUNCTION TO_GEOMETRY(pts BLOB, num_pts NUMBER,
                       pc_tot_dim NUMBER, srid NUMBER default null,
                       blk_domain sdo_orgscl_type default null,
                       get_ids  NUMBER default NULL)
    RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

  FUNCTION GET_PT_IDs(pts BLOB, num_pts NUMBER,
                      pc_tot_dim NUMBER,
                      blk_domain sdo_orgscl_type default null)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;


/*
PROCEDURE POPULATE_FROM_CLSTPCDATA(inp sdo_pc, clstpcdatatbl varchar2);

  PROCEDURE WRITE_CLSTPCDATA(inp sdo_pc, clstpcdatatbl varchar2);
*/


/*
  -- works as read if qry is null
  FUNCTION CLIP_PCAttrs(inp sdo_pc, qry sdo_geometry, blkno number default null)
    RETURN ANYDATASET;
  FUNCTION CLIP_PC_PtIds(inp sdo_pc, qry sdo_geometry, blkno number default null)
    RETURN ANYDATASET;
*/

END sdo_pc_pkg;
/


Prompt Synonym SDO_PC_PKG;
--
-- SDO_PC_PKG  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_PC_PKG FOR MDSYS.SDO_PC_PKG
/


Prompt Grants on PACKAGE SDO_PC_PKG TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_PC_PKG TO PUBLIC
/
