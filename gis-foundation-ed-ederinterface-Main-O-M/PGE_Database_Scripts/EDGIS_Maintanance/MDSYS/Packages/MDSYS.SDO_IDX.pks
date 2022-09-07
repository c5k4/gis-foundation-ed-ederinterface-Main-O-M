Prompt drop Package SDO_IDX;
DROP PACKAGE MDSYS.SDO_IDX
/

Prompt Package SDO_IDX;
--
-- SDO_IDX  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_idx AUTHID current_user AS

  PROCEDURE ie_crt_geom_metadata;

  PROCEDURE cmt_idx_chngs(schema varchar2, iname varchar2,
		          iptn varchar2, tnum number);

  FUNCTION ENDIANCONVERT(entry   IN BLOB,
                         fanout  IN NUMBER,
                         dim     IN NUMBER)
  RETURN BLOB DETERMINISTIC PARALLEL_ENABLE;

  PROCEDURE tts_index_initialize(
    schema_name               IN VARCHAR2,
    SDO_INDEX_TYPE            IN VARCHAR2,
    SDO_LEVEL                 IN NUMBER,
    SDO_NUMTILES              IN NUMBER,
    SDO_MAXLEVEL              IN NUMBER,
    SDO_COMMIT_INTERVAL       IN NUMBER,
    SDO_INDEX_TABLE           IN VARCHAR2,
    SDO_INDEX_NAME            IN VARCHAR2,
    SDO_INDEX_PRIMARY         IN NUMBER,
    SDO_TSNAME                IN VARCHAR2,
    SDO_COLUMN_NAME           IN VARCHAR2,
    SDO_RTREE_HEIGHT          IN NUMBER,
    SDO_RTREE_NUM_NODES       IN NUMBER,
    SDO_RTREE_DIMENSIONALITY  IN NUMBER,
    SDO_RTREE_FANOUT          IN NUMBER,
    SDO_RTREE_ROOT            IN VARCHAR2,
    SDO_RTREE_SEQ_NAME        IN VARCHAR2,
    SDO_FIXED_META            IN RAW,
    SDO_TABLESPACE            IN VARCHAR2,
    SDO_INITIAL_EXTENT        IN VARCHAR2,
    SDO_NEXT_EXTENT           IN VARCHAR2,
    SDO_PCTINCREASE           IN NUMBER,
    SDO_MIN_EXTENTS           IN NUMBER,
    SDO_MAX_EXTENTS           IN NUMBER,
    SDO_INDEX_DIMS            IN NUMBER,
    SDO_LAYER_GTYPE           IN VARCHAR2,
    SDO_RTREE_PCTFREE         IN NUMBER,
    SDO_INDEX_PARTITION       IN VARCHAR2,
    SDO_PARTITIONED           IN NUMBER,
    SDO_RTREE_QUALITY         IN NUMBER,
    SDO_INDEX_VERSION         IN NUMBER,
    SDO_INDEX_GEODETIC        IN VARCHAR2,
    SDO_INDEX_STATUS          IN VARCHAR2,
    SDO_NL_INDEX_TABLE        IN VARCHAR2,
    SDO_DML_BATCH_SIZE        IN NUMBER,
    SDO_RTREE_ENT_XPND        IN NUMBER,
    SDO_ROOT_MBR              IN MDSYS.SDO_GEOMETRY,
    ENDIAN_FLAG               IN VARCHAR2);

  FUNCTION  process_params(params IN varchar2, invert_mask IN number,
			   mask_str IN OUT varchar2, dst IN OUT number,
			   units_str IN OUT varchar2, mltpl_msks IN OUT number)
   	    RETURN number;


END sdo_idx;
/


Prompt Grants on PACKAGE SDO_IDX TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_IDX TO PUBLIC
/
