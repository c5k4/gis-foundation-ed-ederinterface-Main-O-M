Prompt drop View USER_SDO_GEOM_METADATA;
DROP VIEW MDSYS.USER_SDO_GEOM_METADATA
/

/* Formatted on 6/27/2019 02:51:49 PM (QP5 v5.313) */
PROMPT View USER_SDO_GEOM_METADATA;
--
-- USER_SDO_GEOM_METADATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_GEOM_METADATA
(
    TABLE_NAME,
    COLUMN_NAME,
    DIMINFO,
    SRID
)
AS
    SELECT SDO_TABLE_NAME  TABLE_NAME,
           SDO_COLUMN_NAME COLUMN_NAME,
           SDO_DIMINFO     DIMINFO,
           SDO_SRID        SRID
      FROM SDO_GEOM_METADATA_TABLE
     WHERE sdo_owner = SYS_CONTEXT ('userenv', 'CURRENT_SCHEMA')
/


Prompt Trigger SDO_GEOM_TRIG_DEL1;
--
-- SDO_GEOM_TRIG_DEL1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOM_TRIG_DEL1
INSTEAD OF DELETE ON MDSYS.USER_SDO_GEOM_METADATA
REFERENCING OLD AS n
FOR EACH ROW
declare
 tname varchar2(32);
 stmt  varchar2(2048);
 vcount INTEGER;
BEGIN

  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into tname;

    DELETE FROM  sdo_geom_metadata_table
    WHERE SDO_OWNER = tname
      AND SDO_TABLE_NAME = upper(:n.table_name)
      AND SDO_COLUMN_NAME = upper(:n.column_name);
END;
/


Prompt Trigger SDO_GEOM_TRIG_INS1;
--
-- SDO_GEOM_TRIG_INS1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOM_TRIG_INS1
INSTEAD OF INSERT ON MDSYS.USER_SDO_GEOM_METADATA
REFERENCING NEW AS n
FOR EACH ROW
declare
 tname varchar2(32);
 stmt  varchar2(2048);
 vcount INTEGER;
 dimcount INTEGER;
 tolerance NUMBER;
 dimelement MDSYS.SDO_DIM_ELEMENT;
 idx  number;
BEGIN
  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into tname;

  if ( (instr(:n.table_name, ' ') > 0 ) OR
       (instr(:n.table_name, '''') > 0 ) )  then
   mderr.raise_md_error('MD', 'SDO', -13199,
               'wrong table name: ' || :n.table_name);
   end if;

  if ( (instr(:n.column_name, ' ') > 0 ) OR
       (instr(:n.column_name, '''') > 0 ) ) then
   mderr.raise_md_error('MD', 'SDO', -13199,
               'wrong column name: ' || :n.column_name);
   end if;

  stmt :=  'SELECT count(*) FROM SDO_GEOM_METADATA_TABLE ' ||
  'WHERE sdo_owner = :tname  AND sdo_table_name = :table_name  '||
  '  AND  sdo_column_name = :column_name  ';

EXECUTE IMMEDIATE stmt INTO vcount
   USING upper(tname), upper(:n.table_name), upper(:n.column_name);


  IF vcount = 0 THEN
    dimcount :=  :n.diminfo.count;
    FOR idx in 1 .. dimcount LOOP
      dimelement := :n.diminfo(idx);
      tolerance := dimelement.SDO_TOLERANCE;
      if ( (tolerance is NULL) OR (tolerance <= 0) ) then
          mderr.raise_md_error('MD', 'SDO', -13224,
                :n.table_name||'.'||:n.column_name);
      end if;
    END LOOP;
    INSERT INTO sdo_geom_metadata_table values
             (tname,
             upper(:n.table_name), upper(:n.column_name), :n.diminfo,
             :n.srid);
  ELSE
   mderr.raise_md_error('MD', 'SDO', -13223,
               :n.table_name||'.'||:n.column_name);
 END IF;
END;
/


Prompt Trigger SDO_GEOM_TRIG_UPD1;
--
-- SDO_GEOM_TRIG_UPD1  (Trigger) 
--
CREATE OR REPLACE TRIGGER MDSYS.SDO_GEOM_TRIG_UPD1
INSTEAD OF UPDATE ON MDSYS.USER_SDO_GEOM_METADATA
REFERENCING OLD AS old NEW AS n
FOR EACH ROW
declare
 tname varchar2(32);
 stmt  varchar2(2048);
 vcount INTEGER;
BEGIN

  EXECUTE IMMEDIATE
  'SELECT user FROM dual' into tname;

    UPDATE sdo_geom_metadata_table
    SET (SDO_TABLE_NAME, SDO_COLUMN_NAME, SDO_DIMINFO, SDO_SRID)  =
     (SELECT upper(:n.table_name), upper(:n.column_name), :n.diminfo,
      :n.srid  FROM DUAL)
    WHERE SDO_OWNER = tname
      AND SDO_TABLE_NAME = upper(:old.table_name)
      AND SDO_COLUMN_NAME = upper(:old.column_name);
END;
/


Prompt Synonym USER_SDO_GEOM_METADATA;
--
-- USER_SDO_GEOM_METADATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_GEOM_METADATA FOR MDSYS.USER_SDO_GEOM_METADATA
/


Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_GEOM_METADATA TO PUBLIC
/

Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO SPATIAL_CSW_ADMIN to SPATIAL_CSW_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.USER_SDO_GEOM_METADATA TO SPATIAL_CSW_ADMIN
/

Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO SPATIAL_CSW_ADMIN_USR to SPATIAL_CSW_ADMIN_USR;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.USER_SDO_GEOM_METADATA TO SPATIAL_CSW_ADMIN_USR
/

Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO SPATIAL_WFS_ADMIN to SPATIAL_WFS_ADMIN;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.USER_SDO_GEOM_METADATA TO SPATIAL_WFS_ADMIN
/

Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO SPATIAL_WFS_ADMIN_USR to SPATIAL_WFS_ADMIN_USR;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON MDSYS.USER_SDO_GEOM_METADATA TO SPATIAL_WFS_ADMIN_USR
/

Prompt Grants on VIEW USER_SDO_GEOM_METADATA TO WFS_USR_ROLE to WFS_USR_ROLE;
GRANT SELECT ON MDSYS.USER_SDO_GEOM_METADATA TO WFS_USR_ROLE
/
