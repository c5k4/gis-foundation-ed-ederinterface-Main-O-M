Prompt drop View SDO_RELATEMASK_TABLE;
DROP VIEW MDSYS.SDO_RELATEMASK_TABLE
/

/* Formatted on 6/27/2019 02:51:52 PM (QP5 v5.313) */
PROMPT View SDO_RELATEMASK_TABLE;
--
-- SDO_RELATEMASK_TABLE  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.SDO_RELATEMASK_TABLE
(
    SDO_MASK,
    SDO_RELATION
)
AS
    SELECT sdo_mask, sdo_relation FROM md$relate
/


Prompt Synonym SDO_RELATEMASK_TABLE;
--
-- SDO_RELATEMASK_TABLE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_RELATEMASK_TABLE FOR MDSYS.SDO_RELATEMASK_TABLE
/


Prompt Grants on VIEW SDO_RELATEMASK_TABLE TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.SDO_RELATEMASK_TABLE TO PUBLIC
/
