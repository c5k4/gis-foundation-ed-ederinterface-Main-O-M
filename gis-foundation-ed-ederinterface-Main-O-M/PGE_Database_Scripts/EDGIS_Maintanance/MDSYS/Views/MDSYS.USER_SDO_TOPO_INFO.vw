Prompt drop View USER_SDO_TOPO_INFO;
DROP VIEW MDSYS.USER_SDO_TOPO_INFO
/

/* Formatted on 6/27/2019 02:51:40 PM (QP5 v5.313) */
PROMPT View USER_SDO_TOPO_INFO;
--
-- USER_SDO_TOPO_INFO  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_TOPO_INFO
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
     WHERE sdo_owner = SYS_CONTEXT ('userenv', 'CURRENT_SCHEMA')
/


Prompt Synonym USER_SDO_TOPO_INFO;
--
-- USER_SDO_TOPO_INFO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_TOPO_INFO FOR MDSYS.USER_SDO_TOPO_INFO
/


Prompt Grants on VIEW USER_SDO_TOPO_INFO TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.USER_SDO_TOPO_INFO TO PUBLIC
/
