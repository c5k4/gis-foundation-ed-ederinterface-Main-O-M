Prompt drop View USER_SDO_NETWORK_HISTORIES;
DROP VIEW MDSYS.USER_SDO_NETWORK_HISTORIES
/

/* Formatted on 6/27/2019 02:51:43 PM (QP5 v5.313) */
PROMPT View USER_SDO_NETWORK_HISTORIES;
--
-- USER_SDO_NETWORK_HISTORIES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_NETWORK_HISTORIES
(
    NETWORK,
    NODE_HISTORY_TABLE,
    LINK_HISTORY_TABLE,
    NODE_TRIGGER,
    LINK_TRIGGER
)
AS
    SELECT network,
           node_history_table,
           link_history_table,
           node_trigger,
           link_trigger
      FROM sdo_network_histories
     WHERE owner = SYS_CONTEXT ('USERENV', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_NETWORK_HIS_DEL_TRIG;
--
-- SDO_NETWORK_HIS_DEL_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_his_del_trig
INSTEAD OF DELETE ON MDSYS.USER_SDO_NETWORK_HISTORIES
REFERENCING OLD AS o
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  DELETE
    FROM  sdo_network_histories
    WHERE NLS_UPPER(OWNER) = NLS_UPPER(user_name)
      AND network = :o.network;

END;
/


Prompt Trigger SDO_NETWORK_HIS_INS_TRIG;
--
-- SDO_NETWORK_HIS_INS_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_his_ins_trig
INSTEAD OF INSERT ON MDSYS.USER_SDO_NETWORK_HISTORIES
REFERENCING NEW AS n
FOR EACH ROW
DECLARE
 user_name         VARCHAR2(32);
BEGIN

  EXECUTE IMMEDIATE 'SELECT user FROM dual' INTO user_name;

  INSERT INTO sdo_network_histories(
     owner, network, node_history_table, link_history_table, node_trigger,
     link_trigger)
  VALUES(
     NLS_UPPER(user_name),:n.network,:n.node_history_table,
     :n.link_history_table,:n.node_trigger,:n.link_trigger);

EXCEPTION WHEN OTHERS THEN RAISE;
END;
/


Prompt Trigger SDO_NETWORK_HIS_UPD_TRIG;
--
-- SDO_NETWORK_HIS_UPD_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_his_upd_trig
INSTEAD OF UPDATE ON MDSYS.USER_SDO_NETWORK_HISTORIES
REFERENCING OLD AS o NEW AS n
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  UPDATE sdo_network_histories
   SET(network,node_history_table,link_history_table,node_trigger,link_trigger)
      =
   (SELECT
      :n.network,:n.node_history_table,:n.link_history_table,:n.node_trigger,
      :n.link_trigger
    FROM DUAL)
    WHERE  NLS_UPPER(owner)  = NLS_UPPER(user_name)
      AND  network = :o.network;
END;
/


Prompt Synonym USER_SDO_NETWORK_HISTORIES;
--
-- USER_SDO_NETWORK_HISTORIES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_NETWORK_HISTORIES FOR MDSYS.USER_SDO_NETWORK_HISTORIES
/


Prompt Grants on VIEW USER_SDO_NETWORK_HISTORIES TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_NETWORK_HISTORIES TO PUBLIC
/
