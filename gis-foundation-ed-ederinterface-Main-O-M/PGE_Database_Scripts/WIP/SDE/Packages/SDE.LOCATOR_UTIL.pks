Prompt drop Package LOCATOR_UTIL;
DROP PACKAGE SDE.LOCATOR_UTIL
/

Prompt Package LOCATOR_UTIL;
--
-- LOCATOR_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.locator_util
/***********************************************************************
*
*N  {locator_util.sps}  --  Interface for locator DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on SDE locator table.  It should be compiled by the
*   SDE DBA user; security is by user name.   
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
*    Yung-Ting Chen          05/03/99               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   -- Standard identifier for a locator.

   SUBTYPE locator_id_t IS SDE.locators.locator_id%TYPE;
   SUBTYPE locator_record_t IS SDE.locators%ROWTYPE;

   -- Array of sdemetadata columns.

   TYPE record_id_list_t IS TABLE OF SDE.metadata.record_id%TYPE 
                                  INDEX BY BINARY_INTEGER;
   TYPE property_list_t IS TABLE OF SDE.metadata.property%TYPE 
                                  INDEX BY BINARY_INTEGER;
   TYPE value_list_t IS TABLE OF SDE.metadata.prop_value%TYPE 
                                  INDEX BY BINARY_INTEGER;

   -- The metadata class name of locator.

   C_metadata_class_name_locator  CONSTANT NVARCHAR2(32) := N'SDE internal';

   -- The following constant defines the release of locator_util, and 
   -- is used by the iomgr to determine if the most up to date version
   -- of the package has been installed.

   C_package_release                 CONSTANT PLS_INTEGER := 1007;

  /* Procedures and functions. */

   -- The following functions perform DDL operations for locator objects
   -- stored in the SDE.SDELOCATORS table.  These procedures all issue 
   -- a COMMIT on success.

   PROCEDURE insert_locator (locator           IN  locator_record_t,
                             num_properties    IN  integer,
                             record_id_list    IN  record_id_list_t,
                             property_list     IN  property_list_t,
                             value_list        IN  value_list_t);
   PROCEDURE delete_locator (old_locator_id    IN  locator_id_t); 
   PROCEDURE update_locator (locator           IN  locator_record_t,
                             num_properties    IN  integer,
                             record_id_list    IN  record_id_list_t,
                             property_list     IN  property_list_t,
                             value_list        IN  value_list_t);

   PRAGMA RESTRICT_REFERENCES (locator_util,WNDS,WNPS);

END locator_util;

/


Prompt Grants on PACKAGE LOCATOR_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.LOCATOR_UTIL TO PUBLIC WITH GRANT OPTION
/
