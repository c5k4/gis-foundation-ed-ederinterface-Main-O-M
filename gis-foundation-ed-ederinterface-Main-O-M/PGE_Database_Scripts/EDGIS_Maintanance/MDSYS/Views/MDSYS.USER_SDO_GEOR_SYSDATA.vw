Prompt drop View USER_SDO_GEOR_SYSDATA;
DROP VIEW MDSYS.USER_SDO_GEOR_SYSDATA
/

/* Formatted on 6/27/2019 02:51:48 PM (QP5 v5.313) */
PROMPT View USER_SDO_GEOR_SYSDATA;
--
-- USER_SDO_GEOR_SYSDATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_GEOR_SYSDATA
(
    TABLE_NAME,
    COLUMN_NAME,
    METADATA_COLUMN_NAME,
    RDT_TABLE_NAME,
    RASTER_ID,
    OTHER_TABLE_NAMES
)
AS
    SELECT TABLE_NAME,
           COLUMN_NAME,
           METADATA_COLUMN_NAME,
           RDT_TABLE_NAME,
           RASTER_ID,
           OTHER_TABLE_NAMES
      FROM ALL_SDO_GEOR_SYSDATA
     WHERE owner = SYS_CONTEXT ('userenv', 'SESSION_USER')
/


Prompt Trigger SDO_GEOR_TRIG_DEL1;
--
-- SDO_GEOR_TRIG_DEL1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOR_TRIG_DEL1
INSTEAD OF DELETE ON MDSYS.USER_SDO_GEOR_SYSDATA
FOR EACH ROW
DECLARE
  owner   VARCHAR2(32);
  valid   VARCHAR2(32);
BEGIN
  owner:=user;
  valid:=SDO_GEOR_DEF.isValidEntry(upper(owner),upper(:old.table_name),upper(:old.column_name),upper(:old.rdt_table_name),:old.raster_id);
  if(valid='TRUE')
  then
     mderr.raise_md_error('MD', 'SDO', -13391, 'A valid entry cannot be deleted directly.');
  end if;
  SDO_GEOR_DEF.deleteMetaEntry(user, :old.rdt_table_name, :old.raster_id);
END;
/


Prompt Trigger SDO_GEOR_TRIG_INS1;
--
-- SDO_GEOR_TRIG_INS1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOR_TRIG_INS1
INSTEAD OF INSERT ON MDSYS.USER_SDO_GEOR_SYSDATA
FOR EACH ROW
DECLARE
  owner   VARCHAR2(32);
  valid   VARCHAR2(32);
BEGIN
  owner:=user;
  valid:=SDO_GEOR_DEF.isValidEntry(upper(owner),upper(:new.table_name),upper(:new.column_name),upper(:new.rdt_table_name),:new.raster_id);

  if(valid='FALSE')
  then
     mderr.raise_md_error('MD', 'SDO', -13391, 'The inserted entry is not valid.');
  end if;
  SDO_GEOR_INT.insertUserSysEntry(user, :new.table_name, :new.column_name,
      :new.metadata_column_name, :new.rdt_table_name, :new.raster_id,
      :new.other_table_names);
END;
/


Prompt Trigger SDO_GEOR_TRIG_UPD1;
--
-- SDO_GEOR_TRIG_UPD1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOR_TRIG_UPD1
INSTEAD OF UPDATE ON MDSYS.USER_SDO_GEOR_SYSDATA
FOR EACH ROW
DECLARE
 stmt  varchar2(2048);
 cnt   NUMBER;
 owner VARCHAR2(32);
 rdt   VARCHAR2(32);
 valid   VARCHAR2(32);
BEGIN
  owner:=user;
  valid:=SDO_GEOR_DEF.isValidEntry(upper(owner),upper(:old.table_name),upper(:old.column_name),upper(:old.rdt_table_name),:old.raster_id);
  if(valid='TRUE')
  then
     mderr.raise_md_error('MD', 'SDO', -13391, 'A valid entry cannot be updated directly.');
  end if;
  valid:=SDO_GEOR_DEF.isValidEntry(upper(owner),upper(:new.table_name),upper(:new.column_name),upper(:new.rdt_table_name),:new.raster_id);
  if(valid='FALSE')
  then
     mderr.raise_md_error('MD', 'SDO', -13391, 'The updated entry is not valid.');
  end if;


  SDO_GEOR_DEF.deleteMetaEntry(user, :old.rdt_table_name, :old.raster_id);


  SDO_GEOR_INT.insertUserSysEntry(user, :new.table_name, :new.column_name,
      :new.metadata_column_name, :new.rdt_table_name, :new.raster_id,
      :new.other_table_names);
END;
/


Prompt Synonym USER_SDO_GEOR_SYSDATA;
--
-- USER_SDO_GEOR_SYSDATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_GEOR_SYSDATA FOR MDSYS.USER_SDO_GEOR_SYSDATA
/


Prompt Grants on VIEW USER_SDO_GEOR_SYSDATA TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_GEOR_SYSDATA TO PUBLIC
/
