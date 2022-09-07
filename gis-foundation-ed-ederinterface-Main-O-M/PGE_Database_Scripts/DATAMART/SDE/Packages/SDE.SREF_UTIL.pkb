Prompt drop Package Body SREF_UTIL;
DROP PACKAGE BODY SDE.SREF_UTIL
/

Prompt Package Body SREF_UTIL;
--
-- SREF_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.sref_util
/***********************************************************************
*
*N  {sref_util.spb}  --  Implementation for sreg DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on the spatial_references table.  It should be 
*   compiled by the SDE DBA user; security is by user name.   
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

   /* Package Globals. */

   /* Local Subprograms. */

   /* Public Subprograms. */

   PROCEDURE insert_spatial_references (sref IN sref_record_t)
  /***********************************************************************
  *
  *N  {insert_spatial_references}  --  Add record to spatial_references
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure adds an entry to the SDE.spatial_references table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    sref <IN>  ==  (spatial_record_t)  new spatial_references
  *                           record to be entered.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jerry L. Day            03/18/99               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch the DUP_VAL_ON_INDEX 
      -- exception that occurs when a sref is inserted that has a name
      -- already present in the SDE.spatial_references table, so that it 
      -- may be properly handled.
      
      BEGIN
      
        INSERT INTO SDE.spatial_references 
          (srid, 
           falsex, 
           falsey,
           xyunits,
           falsez,
           zunits, 
           falsem,
           munits,
           xycluster_tol,
           zcluster_tol,
           mcluster_tol,
           object_flags,
           srtext,
           description,
           auth_name,
           auth_srid)
         VALUES (sref.srid, 
               sref.falsex, 
               sref.falsey,
               sref.xyunits,
               sref.falsez,
               sref.zunits,
               sref.falsem,
               sref.munits,
               sref.xycluster_tol,
               sref.zcluster_tol,
               sref.mcluster_tol,
               sref.object_flags,
               sref.srtext,
               sref.description,
               sref.auth_name,
               sref.auth_srid);
      EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                  raise_application_error (sde_util.SE_SPATIALREF_EXISTS,
                                     'Spatial reference ' || 
                                     TO_CHAR (sref.srid) || 
                                     ' already exists.');
      END;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END insert_spatial_references;

   PROCEDURE delete_spatial_references (old_sref_id  IN  sref_id_t)
  /***********************************************************************
  *
  *N  {delete_spatial_references}  --  Delete an arbitary spatial reference.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary spatial reference.  All checking
  *   and locking should be performed by the gsrvr, except that we will
  *   check sref ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_sref_id  <IN>  ==  (sref_id_t)  sref to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_SPATIALREF_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jerry L. Day            03/18/99               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      DELETE FROM SDE.spatial_references WHERE srid = old_sref_id;

      -- Make sure that something was updated.

         IF SQL%NOTFOUND THEN
            raise_application_error (sde_util.SE_SPATIALREF_NOEXIST,
                                     'Spatial Reference entry not found.');
         END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_spatial_references;
   
   PROCEDURE alter_spatial_references  (sref IN sref_record_t)
  /***********************************************************************
  *
  *N  {alter_spatial_references}  --  Update the fields of a 
  *                                    spatial_reference entry
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a spatial reference's fields.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check sref ownership if the invoking user is not 
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters
  *    sref <IN>  ==  (spatial_record_t)  spatial_references
  *                           record to be updated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_SPATIALREF_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jerry L. Day            03/18/99               Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Update the spatial_references.

      UPDATE SDE.spatial_references
      SET  falsex = sref. falsex,
           falsey = sref.falsey,
           xyunits = sref.xyunits,
           falsez = sref.falsez,
           zunits = sref. zunits,
           falsem = sref.falsem,
           munits = sref.munits,
           xycluster_tol = sref.xycluster_tol,
           zcluster_tol = sref.zcluster_tol,
           mcluster_tol = sref.mcluster_tol,
           object_flags = sref.object_flags,
           srtext = sref.srtext,
           description = sref.description,
           auth_name = sref.auth_name,
           auth_srid = sref.auth_srid
      WHERE srid = sref.srid;
      
      -- Make sure that something was updated.

         IF SQL%NOTFOUND THEN
            raise_application_error (sde_util.SE_SPATIALREF_NOEXIST,
                                     'Spatial Reference entry not found.');
         END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END alter_spatial_references;
   
   PROCEDURE lock_spatial_references
  /***********************************************************************
  *
  *N  {lock_spatial_references}  --  Lock spatial_references
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure locks spatial_references table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Jerry L. Day            03/18/99               Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      LOCK TABLE SDE.spatial_references IN EXCLUSIVE MODE;
 
   END lock_spatial_references;

END sref_util;

/


Prompt Grants on PACKAGE SREF_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SREF_UTIL TO PUBLIC WITH GRANT OPTION
/
