Prompt drop Package Body RASTERCOLUMNS_UTIL;
DROP PACKAGE BODY SDE.RASTERCOLUMNS_UTIL
/

Prompt Package Body RASTERCOLUMNS_UTIL;
--
-- RASTERCOLUMNS_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.rastercolumns_util
/***********************************************************************
*
*N  {rastercolumns_util.spb}  --  Implementation for rastercolumn DDL
*                                 package.
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on raster_columns.  It should be compiled by the
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
*    Yung-Ting Chen           03/08/99               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          NVARCHAR2(32);

   CURSOR G_rastercolumn_owner_cursor 
                        (wanted_rastercolumn_id IN rastercolumn_id_t) IS
     SELECT owner
     FROM   SDE.raster_columns
     WHERE  rastercolumn_id = wanted_rastercolumn_id;

   /* Local Subprograms. */

   PROCEDURE L_rastercolumn_user_can_modify 
                           (wanted_rastercolumn_id  IN  rastercolumn_id_t)
  /***********************************************************************
  *
  *N  {L_rastercolumn_user_can_modify}  --  Can current user modify 
  *                                         rastercolumn?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the rastercolumn specified by ID exists and
  *   is modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_rastercolumn_id  <IN>  ==  (rastercolumn_id_t) Rastercolumn
  *                                       to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen          03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

      rastercolumn_owner           SDE.raster_columns.owner%TYPE;

   BEGIN

      -- Make sure that the rastercolumn exists, and that the current user
      -- can write to it.

      OPEN G_rastercolumn_owner_cursor (wanted_rastercolumn_id);
      FETCH G_rastercolumn_owner_cursor INTO rastercolumn_owner;
      IF G_rastercolumn_owner_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_RASTERCOLUMN_NOEXIST,
                                  'raster column ' || 
                                  TO_CHAR (wanted_rastercolumn_id) ||
                                  ' not found');
      END IF;
      CLOSE G_rastercolumn_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != rastercolumn_owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                 'Not owner of raster column ' ||
                                 TO_CHAR (wanted_rastercolumn_id) || '.');
         END IF;
      END IF;
    
   END L_rastercolumn_user_can_modify;


   /* Public Subprograms. */

   PROCEDURE insert_rastercolumn (rastercolumn IN rastercolumn_record_t)
  /***********************************************************************
  *
  *N  {insert_rastercolumn}  --  insert a rastercolumn entry into the 
  *                              RASTER_COLUMNS table. 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the
  *   SDE.RASTER_COLUMNS table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     rastercolumn  <IN>  ==  (rastercolumn_record_t) The new rastercolumn
  *                              to be inserted. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen          03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a rastercolumn is
      -- inserted.

      BEGIN

      -- Insert the record into the raster_columns table.

        INSERT INTO SDE.raster_columns 
         (rastercolumn_id,description,database_name,owner,table_name,
          raster_column,cdate,config_keyword,minimum_id,
          base_rastercolumn_id,rastercolumn_mask,srid)
        VALUES (rastercolumn.rastercolumn_id,
                rastercolumn.description,
                rastercolumn.database_name,
                rastercolumn.owner,
                rastercolumn.table_name,
                rastercolumn.raster_column,
                rastercolumn.cdate,
                rastercolumn.config_keyword,
                rastercolumn.minimum_id,
                rastercolumn.base_rastercolumn_id,
                rastercolumn.rastercolumn_mask,
                rastercolumn.srid);

      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error(sde_util.SE_RASTERCOLUMN_EXISTS,
                              'raster column ' || 
                              TO_CHAR (rastercolumn.rastercolumn_id) ||
                              ' already exists');
      END;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;


   END insert_rastercolumn;


   PROCEDURE delete_rastercolumn (old_rastercolumn_id IN rastercolumn_id_t)
  /***********************************************************************
  *
  *N  {delete_rastercolumn}  --  Delete an arbitary rastercolumn.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary rastercolumn entry.  
  *   All checking and locking should be performed by the gsrvr, except
  *   that we will check rastercolumn ownership if the invoking user is
  *   not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_rastercolumn_id  <IN>  ==  (rastercolumn_id_t) rastercolumn
  *                                    to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen         03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure rastercolumn exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_rastercolumn_user_can_modify (old_rastercolumn_id);

      -- We've verified our permissions and the rastercolumn's existence,
      -- so it must be OK to delete them.  Do it.

      -- Delete the raster_columns.

      DELETE FROM SDE.raster_columns WHERE 
                  rastercolumn_id = old_rastercolumn_id;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END delete_rastercolumn;



   PROCEDURE update_rastercolumn  (rastercolumn IN rastercolumn_record_t)
  /***********************************************************************
  *
  *N  {update_rastercolumn}  --  Update a rastercolumn.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a rastercolumn's record.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check rastercolumn ownership if the invoking user
  *   is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     rastercolumn  <IN>  ==  (rastercolumn_record_t) rastercolumn to be 
  *                             updated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen           03/08/99           Original coding.
  *    Mark Harris              01/18/2008         Comment out commit.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the rastercolumn exists, and that the current user
      -- can write to it.

      L_rastercolumn_user_can_modify (rastercolumn.rastercolumn_id);
    
      -- Update the raster_columns.

      UPDATE SDE.raster_columns
      SET   description = rastercolumn.description,
            config_keyword = rastercolumn.config_keyword,
            minimum_id = rastercolumn.minimum_id,
            rastercolumn_mask = rastercolumn.rastercolumn_mask
      WHERE rastercolumn_id = rastercolumn.rastercolumn_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      --COMMIT;

   END update_rastercolumn;

   PROCEDURE update_rastercolumn  (r IN rastercolumn_record_t, flag IN number)
   IS
   BEGIN
      L_rastercolumn_user_can_modify (r.rastercolumn_id);

      UPDATE SDE.raster_columns
      SET   description = r.description,
            raster_column = r.raster_column,
            rastercolumn_mask = r.rastercolumn_mask
      WHERE rastercolumn_id = r.rastercolumn_id;

   END update_rastercolumn;


   PROCEDURE rename_rastercolumn (new_table_name         IN table_name_t,
                                  wanted_rastercolumn_id IN rastercolumn_id_t)
  /***********************************************************************
  *
  *N  {rename_rastercolumn}  --  Rename a rastercolumn
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure changes a raster layer's table name. All 
  *   checking and locking should be performed by the gsrvr, except 
  *   that we will check raster layer ownership if the invoking user 
  *   is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     new_table_name         <IN>  ==  (table_name_t) new table name.
  *     wanted_rastercolumn_id <IN>  ==  (rastercolumn_id_t) rastercolumn id
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *   Zhiguang Han            11/30/00          Adapted from layers_util 
  *                                             package.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure rastercolumn exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_rastercolumn_user_can_modify (wanted_rastercolumn_id);

      -- Update the raster_columns.

      UPDATE SDE.raster_columns SET table_name = new_table_name
           WHERE rastercolumn_id = wanted_rastercolumn_id;

      -- Since we've gotten this far without an exception, it must be
      -- OK to commit.

      COMMIT;

   END rename_rastercolumn;

   PROCEDURE update_rastercolumn_srid 
                             (wanted_rastercolumn_id IN rastercolumn_id_t, 
                              new_srid               IN srid_t)
  /***********************************************************************
  *
  *N  {update_rastercolumn_srid}  --  Update srid of a raster layer
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a raster column's srid. All checking and 
  *   locking should be performed by the gsrvr, except that we will check 
  *   raster column ownership if the invoking user is not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_rastercolumn_id <IN>  ==  (rastercolumn_id_t) raster column ID
  *     new_srid               <IN>  ==  (srid_t) spatial reference ID
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *   Zhiguang Han             06/05/02             Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the raster column exists, and that the current 
      -- user can write to it.

      L_rastercolumn_user_can_modify (wanted_rastercolumn_id);

      -- Update the raster_columns.

      UPDATE SDE.raster_columns
      SET    srid = new_srid
      WHERE  rastercolumn_id = wanted_rastercolumn_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_rastercolumn_srid;

   PROCEDURE update_spatial_references
                             (wanted_rastercolumn_id IN rastercolumn_id_t, 
                              new_srtext             IN srtext_t)
  /***********************************************************************
  *
  *N  {update_spatial_references}  --  Update a projection string of a 
  *                                    spatial_reference entry
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a spatial reference's projection string.  
  *   All checking and locking should be performed by the gsrvr, except 
  *   that we will check raster column ownership if the invoking user is
  *   not the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters
  *     wanted_rastercolumn_id <IN>  ==  (rastercolumn_id_t) raster column ID
  *     new_srtext             <IN>  ==  (srtext_t) projection string
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20226                SE_RASTERCOLUMN_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *   Zhiguang Han            06/05/02                Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the raster layer exists, and that the current user
      -- can write to it.

      L_rastercolumn_user_can_modify (wanted_rastercolumn_id);
    
      -- Update the spatial_references.

      UPDATE SDE.spatial_references
      SET   srtext = new_srtext
      WHERE srid = 
            (SELECT srid FROM SDE.raster_columns 
             WHERE rastercolumn_id = wanted_rastercolumn_id);

      -- Make sure that something was updated.

         IF SQL%NOTFOUND THEN
            raise_application_error (sde_util.SE_SPATIALREF_NOEXIST,
                                     'spatial reference not found');
         END IF;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_spatial_references;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END rastercolumns_util;

/


Prompt Grants on PACKAGE RASTERCOLUMNS_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.RASTERCOLUMNS_UTIL TO PUBLIC WITH GRANT OPTION
/
