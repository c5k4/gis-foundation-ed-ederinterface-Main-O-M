Prompt drop View USER_ST_GEOM_INDEX_V;
DROP VIEW SDE.USER_ST_GEOM_INDEX_V
/

/* Formatted on 7/2/2019 01:50:00 PM (QP5 v5.313) */
PROMPT View USER_ST_GEOM_INDEX_V;
--
-- USER_ST_GEOM_INDEX_V  (View)
--

CREATE OR REPLACE FORCE VIEW SDE.USER_ST_GEOM_INDEX_V
(
    TABLE_NAME,
    COLUMN_NAME,
    GRID,
    SRID,
    COMMIT_INT,
    VERSION,
    STATUS,
    INDEX_NAME,
    UNIQUENESS,
    DISTINCT_KEYS,
    BLEVEL,
    LEAF_BLOCKS,
    CLUSTERING_FACTOR,
    DENSITY,
    NUM_ROWS,
    NUM_NULLS,
    SAMPLE_SIZE,
    LAST_ANALYZED,
    USER_STATS,
    ST_FUNCS
)
AS
    SELECT S.table_name,
           S.column_name,
           S.grid,
           S.srid,
           S.commit_int,
           S.version,
           S.status,
           S.index_name,
           S.uniqueness,
           S.distinct_keys,
           S.blevel,
           S.leaf_blocks,
           S.clustering_factor,
           S.density,
           S.num_rows,
           S.num_nulls,
           S.sample_size,
           S.last_analyzed,
           S.user_stats,
           S.st_funcs
      FROM ST_GEOMETRY_INDEX S
     WHERE S.OWNER IN (SELECT USER FROM DUAL)
/


Prompt Synonym USER_ST_GEOM_INDEX;
--
-- USER_ST_GEOM_INDEX  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_ST_GEOM_INDEX FOR SDE.USER_ST_GEOM_INDEX_V
/
