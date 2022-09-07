Prompt drop View USER_SDO_NETWORK_CONSTRAINTS;
DROP VIEW MDSYS.USER_SDO_NETWORK_CONSTRAINTS
/

/* Formatted on 6/27/2019 02:51:44 PM (QP5 v5.313) */
PROMPT View USER_SDO_NETWORK_CONSTRAINTS;
--
-- USER_SDO_NETWORK_CONSTRAINTS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_NETWORK_CONSTRAINTS
(
    CONSTRAINT           , DESCRIPTION, CLASS_NAME, CLASS)
AS

SELECT constraint,
       description,
       class_name,
       class
  FROM sdo_network_constraints
 WHERE sdo_owner = SYS_CONTEXT ('USERENV', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_NETWORK_CONS_DEL_TRIG;
--
-- SDO_NETWORK_CONS_DEL_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_cons_del_trig
INSTEAD OF DELETE ON MDSYS.USER_SDO_NETWORK_CONSTRAINTS
FOR EACH ROW
DECLARE
  err_msg      VARCHAR2(200);
  num          NUMBER := -1;
  stmt         VARCHAR2(200);
BEGIN

  stmt := 'select count(*) from dba_java_classes where owner = :owner '||
   ' and name = :name';
  execute immediate stmt into num using user, :old.class_name;

  if (num=1) then
    err_msg := 'Java class schema object: '||:old.class_name||' exists. Please '||
      'drop it first.';
    mdsys.mderr.raise_md_error('MD', 'SDO', '13385', err_msg);
  else
    stmt := 'delete from sdo_network_constraints where sdo_owner = :owner '||
      ' and class_name = :name';
    execute immediate stmt using user, :old.class_name;
  end if;

EXCEPTION
  when others then raise;
END sdo_network_cons_del_trig;
/


Prompt Trigger SDO_NETWORK_CONS_INS_TRIG;
--
-- SDO_NETWORK_CONS_INS_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_cons_ins_trig
INSTEAD OF INSERT ON MDSYS.USER_SDO_NETWORK_CONSTRAINTS
REFERENCING NEW AS n
FOR EACH ROW
DECLARE
  err_msg         VARCHAR2(200);
  num             NUMBER := -1;
  stmt            VARCHAR2(200);
BEGIN

  stmt := 'select count(*) from dba_java_classes where owner = :owner '||
   ' and name = :name';
  execute immediate stmt into num using user, :n.class_name;

  if (num<>1) then
    err_msg := 'Could not find Java class schema object: ' || :n.class_name ||
        '. Did not load Java class properly.';
    mdsys.mderr.raise_md_error('MD', 'SDO','13385', err_msg);
  else
    stmt := 'insert into sdo_network_constraints(sdo_owner, constraint, ' ||
     'description, class_name, class) values (:owner, :constraint, '||
     ':description, :class_name, :class)';
    execute immediate stmt using NLS_UPPER(user),:n.constraint,
     :n.description, :n.class_name,:n.class;
  end if;

EXCEPTION
  when others then raise;
END sdo_network_cons_ins_trig;
/


Prompt Trigger SDO_NETWORK_CONS_UPD_TRIG;
--
-- SDO_NETWORK_CONS_UPD_TRIG  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.sdo_network_cons_upd_trig
INSTEAD OF UPDATE ON MDSYS.USER_SDO_NETWORK_CONSTRAINTS
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
       class)
      =
	    (SELECT
	       :n.constraint,
         :n.description,
         :n.class_name,
         :n.class
       FROM DUAL)
    WHERE  NLS_UPPER(sdo_owner)  = NLS_UPPER(user_name)
      AND  constraint = :o.constraint;
END;
/


Prompt Synonym USER_SDO_NETWORK_CONSTRAINTS;
--
-- USER_SDO_NETWORK_CONSTRAINTS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_NETWORK_CONSTRAINTS FOR MDSYS.USER_SDO_NETWORK_CONSTRAINTS
/


Prompt Grants on VIEW USER_SDO_NETWORK_CONSTRAINTS TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_NETWORK_CONSTRAINTS TO PUBLIC
/
