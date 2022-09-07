Prompt drop View ALL_SDO_NETWORK_JAVA_OBJECTS;
DROP VIEW MDSYS.ALL_SDO_NETWORK_JAVA_OBJECTS
/

/* Formatted on 6/27/2019 02:52:03 PM (QP5 v5.313) */
PROMPT View ALL_SDO_NETWORK_JAVA_OBJECTS;
--
-- ALL_SDO_NETWORK_JAVA_OBJECTS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_NETWORK_JAVA_OBJECTS
(
    OWNER,
    NAME,
    DESCRIPTION,
    CLASS_NAME,
    CLASS,
    JAVA_INTERFACE
)
AS
    SELECT sdo_owner  owner,
           constraint name,
           description,
           class_name,
           class,
           java_interface
      FROM sdo_network_constraints
     WHERE EXISTS
               (SELECT NULL
                  FROM all_java_classes
                 WHERE owner = sdo_owner AND name = class_name)
/


Prompt Synonym ALL_SDO_NETWORK_JAVA_OBJECTS;
--
-- ALL_SDO_NETWORK_JAVA_OBJECTS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_NETWORK_JAVA_OBJECTS FOR MDSYS.ALL_SDO_NETWORK_JAVA_OBJECTS
/


Prompt Grants on VIEW ALL_SDO_NETWORK_JAVA_OBJECTS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_NETWORK_JAVA_OBJECTS TO PUBLIC
/
