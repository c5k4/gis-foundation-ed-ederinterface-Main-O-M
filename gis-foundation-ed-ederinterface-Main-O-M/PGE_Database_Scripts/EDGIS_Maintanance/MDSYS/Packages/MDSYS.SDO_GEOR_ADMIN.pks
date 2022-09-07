Prompt drop Package SDO_GEOR_ADMIN;
DROP PACKAGE MDSYS.SDO_GEOR_ADMIN
/

Prompt Package SDO_GEOR_ADMIN;
--
-- SDO_GEOR_ADMIN  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_GEOR_ADMIN AUTHID CURRENT_USER AS

-- ---------------------------------------------------------------------
--      procedure to create DML triggers on a georaster table
-- ---------------------------------------------------------------------

PROCEDURE createDMLTrigger
(
    tableName      varchar2,
    columnName     varchar2
);


-- ---------------------------------------------------------------------
-- NAME
--   listRegisteredRDT
--
-- DESCRIPTION
--   This function lists all registereded RDT names in the table
--   mdsys.sdo_geor_sysdata_table
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listRegisteredRDT return mdsys.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   isRDTNameUnique
--
-- DESCRIPTION
--   This function is used to check if the specific RDT name is unique
--   in the table mdsys.sdo_geor_sysdata_table
--
-- ARGUMENTS
--   rdtName.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function isRDTNameUnique(rdtName VARCHAR2) return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   getUniqueRDTName
--
-- DESCRIPTION
--   This function returns an unique RDT name.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function getUniqueRDTName return VARCHAR2 DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
-- NAME
--   listGeoRasterTables
--
-- DESCRIPTION
--   For user mdsys:  this function lists all GeoRaster tables defined in
--                    the database.
--                    output format is:
--                     SCHEMA    TABLE
--   For other users: this function lists all GeoRaster tables defined in
--                    the current schema.
--                    output format is:
--                      TABLE
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listGeoRasterTables return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   listGeoRasterColumns
--
-- DESCRIPTION
--   For user mdsys:  this function lists all GeoRaster columns defined in
--                    the database.
--                    output format is:
--                    SCHEMA   TABLE  COLUMN   STATUS
--     Note: STATUS is used to indicate if a DML trigger is created on this
--          GeoRaster column or not.
--   For other users: this function lists all GeoRaster columnss defined in
--                    the current schema.
--                    output format is:
--                    TABLE  COLUMN  STATUS
--     Note: STATUS is used to indicate if a DML trigger is created on this
--          GeoRaster column or not.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listGeoRasterColumns return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   registerGeoRasterColumns
--
-- DESCRIPTION
--   For user mdsys: this procedure creates dml triggers for all GeoRaster
--                   columns defined in the database.
--   For other users:this procedure creates dml triggers for all GeoRaster
--                   columns defined in current schema.
--
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This procedure should be run by any end user.
--
-- ---------------------------------------------------------------------
Procedure registerGeoRasterColumns;

-- ---------------------------------------------------------------------
-- NAME
--   listGeoRasterObjects
--
-- DESCRIPTION
--   For user mdsys:  this function lists all GeoRaster objects defined in
--                    the database.
--                    output format is:
--                    SCHEMA TABLE  COLUMN RDT RID STATUS
--     Note: STATUS is used to indicate if this georaster object is
--           registered in the mdsys.sdo_geor_sysdata_table or not.
--   For other users: this function lists all GeoRaster objects defined in
--                    the current schema.
--                    output foramt is:
--                    TABLE  COLUMN RDT RID STATUS
--     Note: STATUS is used to indicate if this georaster object is
--           registered in the mdsys.sdo_geor_sysdata_table or not.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listGeoRasterObjects return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   registerGeoRasterObjects
--
-- DESCRIPTION
--   For user mdsys: this procedure registers all GeoRaster objects defined
--                   in the database.
--   For other users:this procedure registers all GeoRaster objects defined
--                   in current schema.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This procedure should be run by any end user.
--
-- ---------------------------------------------------------------------
Procedure registerGeoRasterObjects;

-- ---------------------------------------------------------------------
-- NAME
--   listRDT
--
-- DESCRIPTION
--   For user mdsys:  this function lists all RDT tables defined in
--                    the database,which include registered and
--                    unregistered RDTs in the mdsys.sdo_geor_sysdata_table.
--                    output format is:
--                    SCHEMA TABLE
--   For other users: this function lists all RDT tables defined in
--                    the current schema,,which include registered and
--                    unregistered RDTs in the mdsys.sdo_geor_sysdata_table.
--                    output foramt is:
--                    TABLE
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listRDT return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   listUnregisteredRDT
--
-- DESCRIPTION
--   For user mdsys:  this function lists all unregistered RDT tables
--                    defined in the database.
--                    output format is:
--                    SCHEMA TABLE
--   For other users: this function lists all unregistered RDT tables
--                    defined in the current schema.
--                    output foramt is:
--                    TABLE
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listUnregisteredRDT return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   checkSysdataEntries
--
-- DESCRIPTION
--   For user mdsys:  this function checks all sysdata entries in the
--                    database, and prints out a report.
--                    the following is a list of invalid cases
--                    1)RDT name isn't unique
--                    2)GeoRaster table doesn't exist
--                    3)GeoRaster column doesn't exist
--                    4)GeoRaster object doesn't exist
--                    5)Non-empty or Non-blank GeoRaster object, but
--                      RDT doesn't exist
--                    6)Duplicate GeoRaster objects(RDT+RID non-unique)
--
--   For other users: this function checks all sysdata entries related to
--                    the current schema, and prints out a report.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function checkSysdataEntries return MDSYS.sdo_string2_array  DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   maintainSysdataEntries
--
-- DESCRIPTION
--   For user mdsys:  this function maintains all sysdata entries in the
--                    database, and make them valid.
--                    the following is a list of actions to invalid cases
--         1)RDT name isn't unique : rename non-unique RDT
--         2)GeoRaster table doesn't exist : remove entry
--         3)GeoRaster column doesn't exist : remove entry
--         4)GeoRaster object doesn't exist : remove entry
--         5)Non-empty or Non-blank GeoRaster object, but
--           RDT doesn't exist : remove entry
--         6)Duplicate GeoRaster objects(RDT+RID non-unique):remove entry .
--
--   For other users: this function  maintains all sysdata entries related to
--                    the current schema.
--         1)RDT name isn't unique : no action
--         2)GeoRaster table doesn't exist : remove entry
--         3)GeoRaster column doesn't exist : remove entry
--         4)GeoRaster object doesn't exist : remove entry
--         5)Non-empty or Non-blank GeoRaster object, but
--           RDT doesn't exist : remove entry
--         6)Duplicate GeoRaster objects(RDT+RID non-unique):remove entry .
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function maintainSysdataEntries return MDSYS.sdo_string2_array DETERMINISTIC PARALLEL_ENABLE;



-- ---------------------------------------------------------------------
-- NAME
--   isUpgradeNeeded
--
-- DESCRIPTION
--   For user mdsys:  this function checks whole database,and prints
--                    out a report about validation of whole GeoRaster
--                    inforstrcture/data
--                    the following is a list of invalid cases
--                    1)RDT name isn't unique
--                    2)GeoRaster table doesn't exist
--                    3)GeoRaster column doesn't exist
--                    4)Non-empty or Non-blank GeoRaster object, but
--                      RDT doesn't exist
--                    5)Duplicate GeoRaster objects(RDT+RID non-unique)
--                    6)Non-registered GeoRaster columns
--                    7)Non-unique RDT(Non-registered) table
--                    8)Non-registered GeoRaster objects
--                    9)Non-registered RDT table
--
--   For other users:  this function checks whole schema,and prints
--                    out a report about validation of whole GeoRaster
--                    inforstrcture/data
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function isUpgradeNeeded return MDSYS.sdo_string2_array DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
-- NAME
--   upgradeGeoRaster
--
-- DESCRIPTION
--   For user mdsys:  this function maintains whole GeoRaster
--                    infrastucture/data on the database level
--
--   For other users: this function maintains whole GeoRaster
--                    infrastucture/data in schema level
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function upgradeGeoRaster return MDSYS.sdo_string2_array DETERMINISTIC PARALLEL_ENABLE;


-- ---------------------------------------------------------------------
-- NAME
--   listDanglingRasterData
--
-- DESCRIPTION
--   For user mdsys:  this function lists all dangling data
--                    on the database level
--
--   For other users: this function lists dangling data
--                    in schema level
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This function should be run by any end user.
--
-- ---------------------------------------------------------------------
Function listDanglingRasterData return MDSYS.sdo_string2_arrayset DETERMINISTIC PARALLEL_ENABLE;



END SDO_GEOR_ADMIN;
/


Prompt Synonym SDO_GEOR_ADMIN;
--
-- SDO_GEOR_ADMIN  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_GEOR_ADMIN FOR MDSYS.SDO_GEOR_ADMIN
/


Prompt Grants on PACKAGE SDO_GEOR_ADMIN TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_GEOR_ADMIN TO PUBLIC
/
