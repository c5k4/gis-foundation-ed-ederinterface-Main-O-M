Prompt drop View ALL_SDO_NETWORK_LOCKS_WM;
DROP VIEW MDSYS.ALL_SDO_NETWORK_LOCKS_WM
/

/* Formatted on 6/27/2019 02:52:02 PM (QP5 v5.313) */
PROMPT View ALL_SDO_NETWORK_LOCKS_WM;
--
-- ALL_SDO_NETWORK_LOCKS_WM  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_NETWORK_LOCKS_WM
(
    OWNER,
    LOCK_ID,
    NETWORK,
    WORKSPACE,
    ORIGINAL_NODE_FILTER,
    ORIGINAL_LINK_FILTER,
    ORIGINAL_PATH_FILTER,
    ADJUSTED_NODE_FILTER,
    ADJUSTED_LINK_FILTER,
    ADJUSTED_PATH_FILTER
)
AS
    SELECT sdo_owner owner,
           lock_id,
           network,
           workspace,
           original_node_filter,
           original_link_filter,
           original_path_filter,
           adjusted_node_filter,
           adjusted_link_filter,
           adjusted_path_filter
      FROM sdo_network_locks_wm
/


Prompt Synonym ALL_SDO_NETWORK_LOCKS_WM;
--
-- ALL_SDO_NETWORK_LOCKS_WM  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_NETWORK_LOCKS_WM FOR MDSYS.ALL_SDO_NETWORK_LOCKS_WM
/


Prompt Grants on VIEW ALL_SDO_NETWORK_LOCKS_WM TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_NETWORK_LOCKS_WM TO PUBLIC
/
