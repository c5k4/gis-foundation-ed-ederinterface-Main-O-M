Prompt drop View ALL_SDO_TOPO_INFO;
DROP VIEW MDSYS.ALL_SDO_TOPO_INFO
/

/* Formatted on 6/27/2019 02:52:01 PM (QP5 v5.313) */
PROMPT View ALL_SDO_TOPO_INFO;
--
-- ALL_SDO_TOPO_INFO  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_TOPO_INFO
(
    OWNER,
    TOPOLOGY,
    TOPOLOGY_ID,
    TOLERANCE,
    SRID,
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME,
    TG_LAYER_ID,
    TG_LAYER_TYPE,
    TG_LAYER_LEVEL,
    CHILD_LAYER_ID,
    DIGITS_RIGHT_OF_DECIMAL
)
AS
    SELECT SDO_OWNER        OWNER,
           Topology,
           Topology_id,
           Tolerance,
           SRID,
           b.owner          Table_Schema,
           b.Table_Name     Table_Name,
           b.Column_Name    Column_Name,
           b.Layer_ID       TG_Layer_ID,
           b.Layer_Type     TG_Layer_Type,
           b.Layer_Level    TG_Layer_Level,
           b.Child_Layer_id Child_Layer_id,
           Digits_Right_Of_Decimal
      FROM SDO_TOPO_METADATA_TABLE a, TABLE (a.Topo_Geometry_Layers) b
     WHERE (EXISTS
                (SELECT table_name
                   FROM all_tables
                  WHERE     table_name = topology || '_NODE$'
                        AND owner = sdo_owner
                 UNION ALL
                 SELECT view_name
                   FROM all_views
                  WHERE     view_name = topology || '_NODE$'
                        AND owner = sdo_owner))
/


Prompt Synonym ALL_SDO_TOPO_INFO;
--
-- ALL_SDO_TOPO_INFO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_TOPO_INFO FOR MDSYS.ALL_SDO_TOPO_INFO
/


Prompt Grants on VIEW ALL_SDO_TOPO_INFO TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_TOPO_INFO TO PUBLIC
/
