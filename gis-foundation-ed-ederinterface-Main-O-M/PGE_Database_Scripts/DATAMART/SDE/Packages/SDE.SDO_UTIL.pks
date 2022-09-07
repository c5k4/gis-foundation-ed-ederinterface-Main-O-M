Prompt drop Package SDO_UTIL;
DROP PACKAGE SDE.SDO_UTIL
/

Prompt Package SDO_UTIL;
--
-- SDO_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.sdo_util
/***********************************************************************
*
*N  {sdo_util.sps}  --  Oracle Spatial Utilities 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   ArcSDE server operations involving the Oracle Spatial type.  It 
*   should be compiled by the SDE DBA user.   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2004 ESRI
*
*   TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
*   Unpublished material - all rights reserved under the
*   Copyright Laws of the United States.
*
*   For additional information, contact:
*   Environmental Systems Research Institute, Inc.
*   Attn: Contracts Dept
*   380 New York Street
*   Redlands, California, USA 92373
*
*   email: contracts@esri.com
*   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Kevin Watt          12/07/00               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   -- Standard identifier for
   -- SDE.layers, SDE.table_registry and SDE.geometry_columns

   SUBTYPE layer_record_t        IS SDE.layers%ROWTYPE;
   SUBTYPE geocol_record_t       IS SDE.geometry_columns%ROWTYPE;
   SUBTYPE registration_record_t IS SDE.table_registry%ROWTYPE;
     
   -- Standard table name type

  /* Constants. */

   -- The following constant defines the release of sdo_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1004;

  /* Procedures and functions. */

   -- Procedure register_layer:
   --   * Checks the table is not already registered
   --   * Insert SDE.LAYERS record
   --   * Insert SDE.TABLE_REGISTRY record
   -- This procedure issues COMMIT on success.

   PROCEDURE register_layer (layer            IN  layer_record_t,
                             gcol             IN  geocol_record_t,
                             registration     IN  registration_record_t);
 
   -- Assert WNDS, WNPS to Compiler

   PRAGMA RESTRICT_REFERENCES (sdo_util,WNDS,WNPS);

END sdo_util;

/


Prompt Grants on PACKAGE SDO_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SDO_UTIL TO PUBLIC WITH GRANT OPTION
/
