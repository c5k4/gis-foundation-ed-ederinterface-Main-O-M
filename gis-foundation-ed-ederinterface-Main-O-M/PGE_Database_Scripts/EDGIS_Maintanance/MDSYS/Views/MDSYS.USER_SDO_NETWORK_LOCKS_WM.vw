Prompt drop View USER_SDO_NETWORK_LOCKS_WM;
DROP VIEW MDSYS.USER_SDO_NETWORK_LOCKS_WM
/

/* Formatted on 6/27/2019 02:51:42 PM (QP5 v5.313) */
PROMPT View USER_SDO_NETWORK_LOCKS_WM;
--
-- USER_SDO_NETWORK_LOCKS_WM  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_NETWORK_LOCKS_WM
(
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
    SELECT lock_id,
           network,
           workspace,
           original_node_filter,
           original_link_filter,
           original_path_filter,
           adjusted_node_filter,
           adjusted_link_filter,
           adjusted_path_filter
      FROM sdo_network_locks_wm
     WHERE sdo_owner = SYS_CONTEXT ('USERENV', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_NETWORK_LOCKS_DEL_TRIG;
--
-- SDO_NETWORK_LOCKS_DEL_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_locks_del_trig
INSTEAD OF DELETE ON MDSYS.USER_SDO_NETWORK_LOCKS_WM
REFERENCING OLD AS o
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  DELETE
    FROM  sdo_network_locks_wm
    WHERE NLS_UPPER(SDO_OWNER) = NLS_UPPER(user_name)
      AND lock_id = :o.lock_id;

END;
/


Prompt Trigger SDO_NETWORK_LOCKS_INS_TRIG;
--
-- SDO_NETWORK_LOCKS_INS_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_locks_ins_trig
INSTEAD OF INSERT ON MDSYS.USER_SDO_NETWORK_LOCKS_WM
REFERENCING NEW AS n
FOR EACH ROW
DECLARE
 user_name         VARCHAR2(32);
BEGIN

  EXECUTE IMMEDIATE 'SELECT user FROM dual' INTO user_name;

  INSERT INTO
    sdo_network_locks_wm(sdo_owner, lock_id, network, workspace,
     original_node_filter, original_link_filter, original_path_filter,
     adjusted_node_filter, adjusted_link_filter, adjusted_path_filter)
  VALUES (NLS_UPPER(user_name), :n.lock_id, :n.network, :n.workspace,
     :n.original_node_filter,:n.original_link_filter,:n.original_path_filter,
     :n.adjusted_node_filter,:n.adjusted_link_filter,:n.adjusted_path_filter);
END;
/


Prompt Trigger SDO_NETWORK_LOCKS_UPD_TRIG;
--
-- SDO_NETWORK_LOCKS_UPD_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_locks_upd_trig
INSTEAD OF UPDATE ON MDSYS.USER_SDO_NETWORK_LOCKS_WM
REFERENCING OLD AS o NEW AS n
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  UPDATE sdo_network_locks_wm
    SET  (lock_id, network, workspace,
          original_node_filter, original_link_filter, original_path_filter,
          adjusted_node_filter, adjusted_link_filter, adjusted_path_filter)
      = (SELECT :n.lock_id, :n.network, :n.workspace,
          :n.original_node_filter, :n.original_link_filter,
          :n.original_path_filter,
          :n.adjusted_node_filter, :n.adjusted_link_filter,
          :n.adjusted_path_filter
       FROM DUAL)
    WHERE  NLS_UPPER(sdo_owner)  = NLS_UPPER(user_name)
      AND  lock_id = :o.lock_id;
END;
/


Prompt Synonym USER_SDO_NETWORK_LOCKS_WM;
--
-- USER_SDO_NETWORK_LOCKS_WM  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_NETWORK_LOCKS_WM FOR MDSYS.USER_SDO_NETWORK_LOCKS_WM
/


Prompt Grants on VIEW USER_SDO_NETWORK_LOCKS_WM TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_NETWORK_LOCKS_WM TO PUBLIC
/
