Prompt drop View GDB_ITEMRELATIONSHIPS_VW;
DROP VIEW SDE.GDB_ITEMRELATIONSHIPS_VW
/

/* Formatted on 7/2/2019 01:50:00 PM (QP5 v5.313) */
PROMPT View GDB_ITEMRELATIONSHIPS_VW;
--
-- GDB_ITEMRELATIONSHIPS_VW  (View)
--

CREATE OR REPLACE FORCE VIEW SDE.GDB_ITEMRELATIONSHIPS_VW
(
    OBJECTID,
    UUID,
    TYPE,
    ORIGINID,
    DESTID,
    PROPERTIES,
    ATTRIBUTES
)
AS
    SELECT objectid,
           uuid,
           TYPE,
           originid,
           destid,
           properties,
           sde.sdexmltotext (d1.xml_doc) AS attributes
      FROM GDB_ItemRelationships
           LEFT OUTER JOIN sde_xml_doc4 d1
               ON GDB_ItemRelationships.attributes = d1.sde_xml_id
/


Prompt Grants on VIEW GDB_ITEMRELATIONSHIPS_VW TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON SDE.GDB_ITEMRELATIONSHIPS_VW TO PUBLIC WITH GRANT OPTION
/
