Prompt drop Package ARCHIVE_UTIL;
DROP PACKAGE SDE.ARCHIVE_UTIL
/

Prompt Package ARCHIVE_UTIL;
--
-- ARCHIVE_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.archive_util
/***********************************************************************
*
*N  {archive_util.sps}  --  Interface for archive DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on archive.  It should be compiled by the
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
*    Josefina Santiago          10/17/05               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   -- Standard identifier for an archive tables.

   SUBTYPE archiving_regid_t IS SDE.sde_archives.archiving_regid%TYPE;
   SUBTYPE archive_record_t   IS SDE.sde_archives%ROWTYPE;

   -- Standard table name type

   DEF_table_name  NVARCHAR2(160);
   SUBTYPE table_name_t  IS DEF_table_name%TYPE;

  /* Constants. */

   -- The following constant defines the release of archive_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1003;

  /* Procedures and functions. */

   -- The following functions perform DDL operations for layer objects
   -- stored in the SDE.sde_archives table.  These procedures all issue a COMMIT
   -- on success.

   PROCEDURE insert_archive (archive              IN  archive_record_t);
   PROCEDURE delete_archive (old_archiving_regid  IN  archiving_regid_t); 
  
   PRAGMA RESTRICT_REFERENCES (archive_util,WNDS,WNPS);

END archive_util;

/


Prompt Grants on PACKAGE ARCHIVE_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ARCHIVE_UTIL TO PUBLIC WITH GRANT OPTION
/
