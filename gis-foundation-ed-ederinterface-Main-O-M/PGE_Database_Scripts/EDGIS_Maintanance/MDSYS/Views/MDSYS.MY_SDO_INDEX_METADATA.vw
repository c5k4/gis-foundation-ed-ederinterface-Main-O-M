Prompt drop View MY_SDO_INDEX_METADATA;
DROP VIEW MDSYS.MY_SDO_INDEX_METADATA
/

/* Formatted on 6/27/2019 02:51:58 PM (QP5 v5.313) */
PROMPT View MY_SDO_INDEX_METADATA;
--
-- MY_SDO_INDEX_METADATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.MY_SDO_INDEX_METADATA
(
    SDO_INDEX_OWNER,
    SDO_INDEX_TYPE,
    SDO_LEVEL,
    SDO_NUMTILES,
    SDO_MAXLEVEL,
    SDO_COMMIT_INTERVAL,
    SDO_INDEX_TABLE,
    SDO_INDEX_NAME,
    SDO_INDEX_PRIMARY,
    SDO_TSNAME,
    SDO_COLUMN_NAME,
    SDO_RTREE_HEIGHT,
    SDO_RTREE_NUM_NODES,
    SDO_RTREE_DIMENSIONALITY,
    SDO_RTREE_FANOUT,
    SDO_RTREE_ROOT,
    SDO_RTREE_SEQ_NAME,
    SDO_FIXED_META,
    SDO_TABLESPACE,
    SDO_INITIAL_EXTENT,
    SDO_NEXT_EXTENT,
    SDO_PCTINCREASE,
    SDO_MIN_EXTENTS,
    SDO_MAX_EXTENTS,
    SDO_INDEX_DIMS,
    SDO_LAYER_GTYPE,
    SDO_RTREE_PCTFREE,
    SDO_INDEX_PARTITION,
    SDO_PARTITIONED,
    SDO_RTREE_QUALITY,
    SDO_INDEX_VERSION,
    SDO_INDEX_GEODETIC,
    SDO_INDEX_STATUS,
    SDO_NL_INDEX_TABLE,
    SDO_DML_BATCH_SIZE,
    SDO_RTREE_ENT_XPND,
    SDO_ROOT_MBR
)
AS
    SELECT SDO_INDEX_OWNER,
           SDO_INDEX_TYPE,
           SDO_LEVEL,
           SDO_NUMTILES,
           SDO_MAXLEVEL,
           SDO_COMMIT_INTERVAL,
           SDO_INDEX_TABLE,
           SDO_INDEX_NAME,
           SDO_INDEX_PRIMARY,
           SDO_TSNAME,
           SDO_COLUMN_NAME,
           SDO_RTREE_HEIGHT,
           SDO_RTREE_NUM_NODES,
           SDO_RTREE_DIMENSIONALITY,
           SDO_RTREE_FANOUT,
           SDO_RTREE_ROOT,
           SDO_RTREE_SEQ_NAME,
           SDO_FIXED_META,
           SDO_TABLESPACE,
           SDO_INITIAL_EXTENT,
           SDO_NEXT_EXTENT,
           SDO_PCTINCREASE,
           SDO_MIN_EXTENTS,
           SDO_MAX_EXTENTS,
           SDO_INDEX_DIMS,
           SDO_LAYER_GTYPE,
           SDO_RTREE_PCTFREE,
           SDO_INDEX_PARTITION,
           SDO_PARTITIONED,
           SDO_RTREE_QUALITY,
           SDO_INDEX_VERSION,
           SDO_INDEX_GEODETIC,
           SDO_INDEX_STATUS,
           SDO_NL_INDEX_TABLE,
           SDO_DML_BATCH_SIZE,
           SDO_RTREE_ENT_XPND,
           SDO_ROOT_MBR
      FROM SDO_INDEX_METADATA_TABLE
     WHERE sdo_index_owner = SYS_CONTEXT ('userenv', 'CURRENT_SCHEMA')
/


Prompt Synonym MY_SDO_INDEX_METADATA;
--
-- MY_SDO_INDEX_METADATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM MY_SDO_INDEX_METADATA FOR MDSYS.MY_SDO_INDEX_METADATA
/


Prompt Grants on VIEW MY_SDO_INDEX_METADATA TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.MY_SDO_INDEX_METADATA TO PUBLIC
/
