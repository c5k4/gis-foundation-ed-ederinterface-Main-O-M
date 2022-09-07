Prompt drop View GDB_ITEMS_VW;
DROP VIEW SDE.GDB_ITEMS_VW
/

/* Formatted on 7/2/2019 01:17:55 PM (QP5 v5.313) */
PROMPT View GDB_ITEMS_VW;
--
-- GDB_ITEMS_VW  (View)
--

CREATE OR REPLACE FORCE VIEW SDE.GDB_ITEMS_VW
(
    OBJECTID,
    UUID,
    TYPE,
    NAME,
    PHYSICALNAME,
    PATH,
    URL,
    PROPERTIES,
    DEFAULTS,
    DATASETSUBTYPE1,
    DATASETSUBTYPE2,
    DATASETINFO1,
    DATASETINFO2,
    DEFINITION,
    DOCUMENTATION,
    ITEMINFO,
    SHAPE
)
AS
    SELECT objectid,
           uuid,
           TYPE,
           name,
           physicalname,
           PATH,
           url,
           properties,
           defaults,
           datasetsubtype1,
           datasetsubtype2,
           datasetinfo1,
           datasetinfo2,
           sde.sdexmltotext (d1.xml_doc) AS definition,
           sde.sdexmltotext (d2.xml_doc) AS documentation,
           sde.sdexmltotext (d3.xml_doc) AS iteminfo,
           shape
      FROM GDB_ITEMS
           LEFT OUTER JOIN sde_xml_doc1 d1
               ON gdb_items.definition = d1.sde_xml_id
           LEFT OUTER JOIN sde_xml_doc2 d2
               ON gdb_items.documentation = d2.sde_xml_id
           LEFT OUTER JOIN sde_xml_doc3 d3
               ON gdb_items.iteminfo = d3.sde_xml_id
/


Prompt Grants on VIEW GDB_ITEMS_VW TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON SDE.GDB_ITEMS_VW TO PUBLIC WITH GRANT OPTION
/
