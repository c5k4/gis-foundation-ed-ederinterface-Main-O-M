Prompt drop View USER_SDO_NETWORK_JAVA_OBJECTS;
DROP VIEW MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
/

/* Formatted on 6/27/2019 02:51:43 PM (QP5 v5.313) */
PROMPT View USER_SDO_NETWORK_JAVA_OBJECTS;
--
-- USER_SDO_NETWORK_JAVA_OBJECTS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
(
    NAME,
    DESCRIPTION,
    CLASS_NAME,
    CLASS,
    JAVA_INTERFACE
)
AS
    SELECT constraint name,
           description,
           class_name,
           class,
           java_interface
      FROM sdo_network_constraints
     WHERE sdo_owner = SYS_CONTEXT ('USERENV', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_NETWORK_JAVA_DEL_TRIG;
--
-- SDO_NETWORK_JAVA_DEL_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_java_del_trig
INSTEAD OF DELETE ON MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
REFERENCING OLD AS o
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  DELETE
    FROM  sdo_network_constraints
    WHERE NLS_UPPER(SDO_OWNER) = NLS_UPPER(user_name)
      AND constraint = :o.name;

END;
/


Prompt Trigger SDO_NETWORK_JAVA_INS_TRIG;
--
-- SDO_NETWORK_JAVA_INS_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_java_ins_trig
INSTEAD OF INSERT ON MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
REFERENCING NEW AS n
FOR EACH ROW
DECLARE
 user_name         VARCHAR2(32);
 dir_name          VARCHAR2(200);
 file_name         VARCHAR2(32);
 class_file_name   VARCHAR2(32);
BEGIN

  EXECUTE IMMEDIATE 'SELECT user FROM dual' INTO user_name;

  INSERT INTO
    sdo_network_constraints
	  (sdo_owner,
     constraint,
     description,
     class_name,
     class,
     java_interface)
  VALUES
	  (
	   NLS_UPPER(user_name),
	   :n.name,
     :n.description,
     :n.class_name,
     :n.class,
     :n.java_interface
	  );
END;
/


Prompt Trigger SDO_NETWORK_JAVA_UPD_TRIG;
--
-- SDO_NETWORK_JAVA_UPD_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_java_upd_trig
INSTEAD OF UPDATE ON MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
REFERENCING OLD AS o NEW AS n
FOR EACH ROW
DECLARE
  user_name    VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'SELECT USER FROM DUAL' INTO user_name;

  UPDATE sdo_network_constraints
    SET
	    (constraint,
       description,
       class_name,
       class,
       java_interface)
      =
	    (SELECT
	       :n.name,
         :n.description,
         :n.class_name,
         :n.class,
         :n.java_interface
       FROM DUAL)
    WHERE  NLS_UPPER(sdo_owner)  = NLS_UPPER(user_name)
      AND  constraint = :o.name;
END;
/


Prompt Synonym USER_SDO_NETWORK_JAVA_OBJECTS;
--
-- USER_SDO_NETWORK_JAVA_OBJECTS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_NETWORK_JAVA_OBJECTS FOR MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS
/


Prompt Grants on VIEW USER_SDO_NETWORK_JAVA_OBJECTS TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_NETWORK_JAVA_OBJECTS TO PUBLIC
/
