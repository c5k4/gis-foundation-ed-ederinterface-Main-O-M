Prompt drop View USER_SDO_NETWORK_USER_DATA;
DROP VIEW MDSYS.USER_SDO_NETWORK_USER_DATA
/

/* Formatted on 6/27/2019 02:51:41 PM (QP5 v5.313) */
PROMPT View USER_SDO_NETWORK_USER_DATA;
--
-- USER_SDO_NETWORK_USER_DATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_NETWORK_USER_DATA
(
    NETWORK,
    TABLE_TYPE,
    DATA_NAME,
    DATA_TYPE,
    DATA_LENGTH,
    CATEGORY_ID
)
AS
    SELECT network,
           table_type,
           data_name,
           data_type,
           data_length,
           category_id
      FROM sdo_network_user_data
     WHERE sdo_owner = SYS_CONTEXT ('USERENV', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_NETWORK_UD_DEL_TRIG;
--
-- SDO_NETWORK_UD_DEL_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_ud_del_trig
INSTEAD OF DELETE ON MDSYS.USER_SDO_NETWORK_USER_DATA
REFERENCING OLD AS o
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  DELETE
    FROM  sdo_network_user_data
    WHERE NLS_UPPER(SDO_OWNER) = NLS_UPPER(user_name)
      AND network    = NLS_UPPER(:o.network)
      AND table_type = NLS_UPPER(:o.table_type)
      AND data_name  = NLS_UPPER(:o.data_name);

END;
/


Prompt Trigger SDO_NETWORK_UD_INS_TRIG;
--
-- SDO_NETWORK_UD_INS_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_ud_ins_trig
INSTEAD OF INSERT ON MDSYS.USER_SDO_NETWORK_USER_DATA
REFERENCING NEW AS n
FOR EACH ROW
DECLARE
 user_name         VARCHAR2(32);
 no		   number ;
 table_name	   VARCHAR2(32);
BEGIN

  EXECUTE IMMEDIATE 'SELECT user FROM dual' INTO user_name;

  -- check if network already exists in the networkwork metadata
  EXECUTE IMMEDIATE
	'SELECT COUNT(*) FROM sdo_network_metadata_table ' ||
	'  where sdo_owner = :owner AND NLS_UPPER(network) = :net ' into no using NLS_UPPER(user_name), NLS_UPPER(:n.network);
  IF ( no = 0 ) THEN
   mderr.raise_md_error('MD', 'SDO', -13385, user_name||'.'||:n.network || ' NOT IN NETWORK METADATA!');
  END IF;


  INSERT INTO
    sdo_network_user_data
    (
     sdo_owner,
     network,
     table_type,
     data_name,
     data_type,
     data_length,
     category_id
     )
  VALUES
     (
      NLS_UPPER(user_name),
      NLS_UPPER(:n.network),
      NLS_UPPER(:n.table_type),
      NLS_UPPER(:n.data_name),
      NLS_UPPER(:n.data_type),
      :n.data_length,
      :n.category_id
     );
END;
/


Prompt Trigger SDO_NETWORK_UD_UPD_TRIG;
--
-- SDO_NETWORK_UD_UPD_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_ud_upd_trig
INSTEAD OF UPDATE ON MDSYS.USER_SDO_NETWORK_USER_DATA
REFERENCING OLD AS o NEW AS n
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
  no	       number ;
  table_name	   VARCHAR2(32);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  -- check if network already exists in the networkwork metadata
  EXECUTE IMMEDIATE
	'SELECT COUNT(*) FROM sdo_network_metadata_table ' ||
	'  where sdo_owner = :owner AND NLS_UPPER(network) = :net ' into no using NLS_UPPER(user_name), NLS_UPPER(:n.network);

  IF ( no = 0 ) THEN
   mderr.raise_md_error('MD', 'SDO', -13385,
           user_name||'.'||:n.network || ' NOT IN NETWORK METADATA!');
  END IF;

  UPDATE sdo_network_user_data
    SET
      (
       network,
       table_type,
       data_name,
       data_type,
       data_length,
       category_id)
      =
      (SELECT
	 NLS_UPPER(:n.network),
         NLS_UPPER(:n.table_type),
         NLS_UPPER(:n.data_name),
         NLS_UPPER(:n.data_type),
         :n.data_length,
         :n.category_id
       FROM DUAL)
    WHERE  NLS_UPPER(sdo_owner)  = NLS_UPPER(user_name)
      AND  NLS_UPPER(network)    = NLS_UPPER(:o.network)
      AND  NLS_UPPER(table_type) = NLS_UPPER(:o.table_type)
      AND  NLS_UPPER(data_name)  = NLS_UPPER(:o.data_name);
END;
/


Prompt Synonym USER_SDO_NETWORK_USER_DATA;
--
-- USER_SDO_NETWORK_USER_DATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_NETWORK_USER_DATA FOR MDSYS.USER_SDO_NETWORK_USER_DATA
/


Prompt Grants on VIEW USER_SDO_NETWORK_USER_DATA TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_NETWORK_USER_DATA TO PUBLIC
/
