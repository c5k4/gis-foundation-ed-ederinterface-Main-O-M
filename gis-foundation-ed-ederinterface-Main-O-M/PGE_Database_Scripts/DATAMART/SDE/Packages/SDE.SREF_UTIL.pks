Prompt drop Package SREF_UTIL;
DROP PACKAGE SDE.SREF_UTIL
/

Prompt Package SREF_UTIL;
--
-- SREF_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.sref_util
/***********************************************************************
*
*N  {sref_util.sps}  --  Interface for sref DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on spatial references  It should be compiled by the
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
*    Jerry L. Day            03/18/99               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   -- Standard identifier for a spatial_references record.

   SUBTYPE sref_id_t     IS SDE.spatial_references.srid%TYPE;
   SUBTYPE sref_record_t IS SDE.spatial_references%ROWTYPE;

  /* Constants. */

   -- The following constant defines the release of sref_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1003;

  /* Procedures and functions. */

   -- The following functions perform DDL operations for spatial reference
   -- objects stored in the SDE.SPATIAL_REFERENCES table.  These procedures 
   -- all issue a COMMIT on success.

   PROCEDURE alter_spatial_references  (sref    IN  sref_record_t);
   PROCEDURE insert_spatial_references (sref    IN  sref_record_t);
   PROCEDURE delete_spatial_references (old_sref_id IN  sref_id_t);
   PROCEDURE lock_spatial_references;

   PRAGMA RESTRICT_REFERENCES (sref_util,WNDS,WNPS);

END sref_util;

 
/


Prompt Grants on PACKAGE SREF_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SREF_UTIL TO PUBLIC WITH GRANT OPTION
/
