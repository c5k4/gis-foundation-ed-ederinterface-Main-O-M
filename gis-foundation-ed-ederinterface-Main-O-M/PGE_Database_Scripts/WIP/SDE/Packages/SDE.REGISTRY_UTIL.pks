Prompt drop Package REGISTRY_UTIL;
DROP PACKAGE SDE.REGISTRY_UTIL
/

Prompt Package REGISTRY_UTIL;
--
-- REGISTRY_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.registry_util
/***********************************************************************
*
*N  {registry_util.sps}  --  Interface for table_registry DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on table_registry.  It should be compiled by the
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
*    Yung-Ting Chen          03/02/99               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   -- Standard identifiers and structures for registered tables and columns.

   SUBTYPE registration_id_t IS SDE.table_registry.registration_id%TYPE;
   SUBTYPE registration_record_t IS SDE.table_registry%ROWTYPE;
   SUBTYPE registered_column_record_t IS SDE.column_registry%ROWTYPE;

   -- An array of registration ids.

   TYPE reg_list_t IS TABLE OF registration_id_t INDEX BY BINARY_INTEGER;

   -- Array of registered column definitions.

   TYPE registered_column_list_t IS TABLE OF registered_column_record_t
      INDEX BY BINARY_INTEGER;

   -- Standard table name type

   DEF_table_name  NVARCHAR2(160);
   SUBTYPE table_name_t IS DEF_table_name%TYPE;

   -- Old column name type

   DEF_column_name  NVARCHAR2(32);
   SUBTYPE column_name_t IS DEF_column_name%TYPE;


  /* Constants. */

   -- The following constant defines the release of registry_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.
   
   C_package_release       CONSTANT PLS_INTEGER := 1015;

  /* Procedures and functions. */

   -- The following functions perform DDL operations for registration objects
   -- stored in the SDE.TABLE_REGISTRY table.  These procedures all issue a 
   -- COMMIT on success.

   PROCEDURE insert_registration (registration IN  registration_record_t);
   PROCEDURE delete_registration (old_reg_id   IN  registration_id_t); 
   PROCEDURE update_registration (registration IN  registration_record_t); 
   PROCEDURE update_registration (registration IN  registration_record_t,
                                  txn_commit   IN  NUMBER); 
   PROCEDURE change_registration_table_name 
                             (new_table_name   IN  table_name_t,
                              wanted_reg_id    IN  registration_id_t);
   PROCEDURE clear_registration_modified (reg_id  IN  registration_id_t); 

   -- The following functions perform DDL operations for registration objects
   -- stored in the SDE.COLUMN_REGISTRY table.  These procedures all issue a 
   -- COMMIT on success, except for insert_registered_column.

   PROCEDURE insert_registered_column 
                                 (column_entry  IN  registered_column_record_t);
   PROCEDURE delete_registered_column 
                                 (column_entry  IN  registered_column_record_t);
   PROCEDURE update_registered_column 
                                 (column_entry  IN  registered_column_record_t);   
   PROCEDURE update_registered_column 
                                 (column_entry  IN  registered_column_record_t,
                                  old_column_name IN column_name_t);

   PRAGMA RESTRICT_REFERENCES (registry_util,WNDS,WNPS);

END registry_util;

/


Prompt Grants on PACKAGE REGISTRY_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.REGISTRY_UTIL TO PUBLIC WITH GRANT OPTION
/
