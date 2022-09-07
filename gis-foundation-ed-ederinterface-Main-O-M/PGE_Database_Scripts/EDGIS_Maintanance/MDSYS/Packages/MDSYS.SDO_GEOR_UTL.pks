Prompt drop Package SDO_GEOR_UTL;
DROP PACKAGE MDSYS.SDO_GEOR_UTL
/

Prompt Package SDO_GEOR_UTL;
--
-- SDO_GEOR_UTL  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_GEOR_UTL AUTHID CURRENT_USER AS

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
--   renameRDT
--
-- DESCRIPTION
--   This routine renames raster data table(s) owned by the current user.
--   It will update all related GeoRaster objects and GeoRaster system
--   data coorespondingly.
--
-- ARGUMENTS
--   oldRDTs - name(s) of existing raster data table(s) to be renamed
--   newRDTs - name(s) of new raster data table(s)
--
-- NOTES
--
--   This routine renames RDTs owned by the current user.
--
--   The RDT names in the strings are seperated with ','.
--   If an oldRDT does not appear in the GeoRaster sysdata, it is ignored.
--   If a newRDT is not unique in the GeoRaster sysdata, ORA-13403 is raised.
--   If a newRDT is NULL, a unique RDT name is automatically generated.
-- ---------------------------------------------------------------------
PROCEDURE renameRDT
(
   oldRDTs VARCHAR2,
   newRDTs VARCHAR2 DEFAULT NULL
);



-- ---------------------------------------------------------------------
-- NAME
--   makeRDTnamesUnique
--
-- DESCRIPTION
--   This routine resolves conflicting raster data table names by
--   automatically renaming some of them so that all raster data tables
--   are uniquely named afterwards.
--
-- ARGUMENTS
--   None.
--
-- NOTES
--   This routine is part of the fix for bug 3703288. In addition to applying
--   the necessary patchset that fixes this bug, an Oracle database of
--   version 10.1.0.2 or 10.1.0.3 needs to run this routine to resolve
--   potential RDT name conflicts. User may choose to run the above
--   renameRDT() to explicitly rename existing RDTs before run this
--   routine.
--
--   This routine should be run while connected as a user with the DBA role.
--
-- ---------------------------------------------------------------------
PROCEDURE makeRDTnamesUnique;


-- ---------------------------------------------------------------------
-- NAME
--   calcRasterNominalSize
--
-- DESCRIPTION
--   This routine calculates the total length of the raster blocks of
--   a GeoRaster object as if it is uncompressed and not sparse.
--
-- ARGUMENTS
--   geor       - the GeoRaster object
--   padding    - indicates whether to consider padding in the blocks
--   pyramid    - indicates whether to consider pyramids
--   bitmapMask - indicates whether to consider associated bitmap masks
--
-- NOTES
--   All the argument are case insensitive.
--   The result is in bytes.
--
-- ---------------------------------------------------------------------
FUNCTION calcRasterNominalSize
(
  geor       IN SDO_GEORASTER,
  padding    IN VARCHAR2 DEFAULT 'TRUE',
  pyramid    IN VARCHAR2 DEFAULT 'TRUE',
  bitmapMask IN VARCHAR2 DEFAULT 'TRUE'
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   calcRasterStorageSize
-- DESCRIPTION
--   This routine calculates the actual length of the raster blocks of
--   a GeoRaster object.
--
-- ARGUMENTS
--   geor - the GeoRaster object
--
-- NOTES
--   The result is in bytes.
--
-- ---------------------------------------------------------------------
FUNCTION calcRasterStorageSize
(
  geor       IN SDO_GEORASTER
)
RETURN NUMBER DETERMINISTIC PARALLEL_ENABLE;

-- ---------------------------------------------------------------------
-- NAME
--   calcOptimizedBlockSize
-- DESCRIPTION
--   This routine calculates the optimized blockSize based on dimensionSize and--   user input seed blockSize.
--
-- ARGUMENTS
--   dimensionSize - dimension size array, whose length must be 3.
--   blockSize - block size array, whose length must be 3.
--   pyramidLevel - pyramid level, default value is 0.
--
-- NOTES
--
-- ---------------------------------------------------------------------
PROCEDURE calcOptimizedBlockSize
(
  dimensionSize       IN MDSYS.SDO_NUMBER_ARRAY,
  blockSize           IN OUT MDSYS.SDO_NUMBER_ARRAY,
  pyramidLevel        IN number default 0
);

END SDO_GEOR_UTL;
/


Prompt Synonym SDO_GEOR_UTL;
--
-- SDO_GEOR_UTL  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_GEOR_UTL FOR MDSYS.SDO_GEOR_UTL
/


Prompt Grants on PACKAGE SDO_GEOR_UTL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_GEOR_UTL TO PUBLIC
/
