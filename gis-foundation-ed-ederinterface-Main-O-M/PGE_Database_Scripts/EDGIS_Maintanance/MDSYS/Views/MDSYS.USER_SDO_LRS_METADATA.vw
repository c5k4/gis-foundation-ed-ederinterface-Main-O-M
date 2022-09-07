Prompt drop View USER_SDO_LRS_METADATA;
DROP VIEW MDSYS.USER_SDO_LRS_METADATA
/

/* Formatted on 6/27/2019 02:51:44 PM (QP5 v5.313) */
PROMPT View USER_SDO_LRS_METADATA;
--
-- USER_SDO_LRS_METADATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_LRS_METADATA
(
    TABLE_NAME,
    COLUMN_NAME,
    DIM_POS,
    DIM_UNIT
)
AS
    SELECT SDO_TABLE_NAME  TABLE_NAME,
           SDO_COLUMN_NAME COLUMN_NAME,
           SDO_DIM_POS     DIM_POS,
           SDO_DIM_UNIT    DIM_UNIT
      FROM SDO_LRS_METADATA_TABLE,
           (SELECT SYS_CONTEXT ('userenv', 'CURRENT_SCHEMA') username
              FROM DUAL)
     WHERE sdo_owner = username
/


Prompt Trigger SDO_LRS_TRIG_DEL;
--
-- SDO_LRS_TRIG_DEL  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_LRS_TRIG_DEL
INSTEAD OF DELETE ON MDSYS.USER_SDO_LRS_METADATA
REFERENCING OLD AS n
FOR EACH ROW
declare
 user_name 	varchar2(32);
 stmt  		varchar2(2048);
 vcount 	INTEGER;
BEGIN

  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into user_name;

    DELETE FROM  sdo_lrs_metadata_table
    WHERE SDO_OWNER   = user_name
      AND SDO_TABLE_NAME  = UPPER(:n.table_name)
      AND SDO_COLUMN_NAME = UPPER(:n.column_name);
END;
/


Prompt Trigger SDO_LRS_TRIG_INS;
--
-- SDO_LRS_TRIG_INS  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_LRS_TRIG_INS
INSTEAD OF INSERT ON MDSYS.USER_SDO_LRS_METADATA
REFERENCING NEW AS n
FOR EACH ROW
declare
 user_name 	varchar2(32);
 stmt  		varchar2(2048);
 vcount 	INTEGER;
BEGIN

  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into user_name;

  if ( (instr(:n.table_name, ' ') > 0) OR
       (instr(:n.table_name, '''') > 0)  ) then
   mderr.raise_md_error('MD', 'SDO', -13223,
               :n.table_name||'.'||:n.column_name);
   end if;

  if ( (instr(:n.column_name, ' ') > 0) OR
   (instr(:n.column_name, '''') > 0)  ) then
   mderr.raise_md_error('MD', 'SDO', -13223,
               :n.table_name||'.'||:n.column_name);
   end if;

/*
  stmt :=  'SELECT count(*) FROM SDO_LRS_METADATA_TABLE ' ||
  ' WHERE sdo_owner = '''   || UPPER(user_name) || '''  ' ||
  '  AND  sdo_table_name = '''  || UPPER(replace(:n.table_name,'''',''))
   || ''' ' ||
 ' AND  sdo_column_name = ''' || UPPER(replace(:n.column_name,'''',''))|| ''' ';
  */

  stmt :=  'SELECT count(*) FROM SDO_LRS_METADATA_TABLE ' ||
  ' WHERE sdo_owner = :owner  AND  sdo_table_name =  :tab ' ||
  ' AND  sdo_column_name = :col ';

 EXECUTE IMMEDIATE stmt INTO vcount
    USING  UPPER(user_name), UPPER(:n.table_name), UPPER(:n.column_name) ;

  IF vcount = 0 THEN
    INSERT INTO sdo_lrs_metadata_table values
             (UPPER(user_name), UPPER(:n.table_name), UPPER(:n.column_name), :n.dim_pos, UPPER(:n.dim_unit));
  ELSE
   mderr.raise_md_error('MD', 'SDO', -13223,
           user_name||'.'||:n.table_name);
 END IF;
END;
/


Prompt Trigger SDO_LRS_TRIG_UPD;
--
-- SDO_LRS_TRIG_UPD  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_LRS_TRIG_UPD
INSTEAD OF UPDATE ON MDSYS.USER_SDO_LRS_METADATA
REFERENCING OLD AS old NEW AS n
FOR EACH ROW
declare
 user_name 	varchar2(32);
 stmt  		varchar2(2048);
 vcount 	INTEGER;
BEGIN

  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into user_name;

    UPDATE sdo_lrs_metadata_table
    SET (SDO_TABLE_NAME, SDO_COLUMN_NAME, SDO_DIM_POS, SDO_DIM_UNIT)  =
     (SELECT UPPER(:n.table_name), UPPER(:n.column_name),:n.dim_pos, UPPER(:n.dim_unit) FROM DUAL)
    WHERE SDO_OWNER   	  = UPPER(user_name)
      AND SDO_TABLE_NAME  = UPPER(:old.table_name)
      AND SDO_COLUMN_NAME = UPPER(:old.column_name);
END;
/


Prompt Synonym USER_SDO_LRS_METADATA;
--
-- USER_SDO_LRS_METADATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_LRS_METADATA FOR MDSYS.USER_SDO_LRS_METADATA
/


Prompt Grants on VIEW USER_SDO_LRS_METADATA TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_LRS_METADATA TO PUBLIC
/
