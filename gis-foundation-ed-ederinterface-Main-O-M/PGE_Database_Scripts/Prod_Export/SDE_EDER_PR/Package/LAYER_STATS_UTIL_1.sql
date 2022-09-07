--------------------------------------------------------
--  DDL for Package Body LAYER_STATS_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."LAYER_STATS_UTIL" 
/***********************************************************************
*
*N  {layer_stats_util.spb}  --  Implementation for layer DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on layers.  It should be compiled by the
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
*    Josefina Santiago       06/23/2009             Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;

   CURSOR G_layer_stats_cursor (wanted_layer_id   IN  layer_id_t, 
                                  wanted_version_id IN version_id_t) IS
     SELECT layer_id, version_id
     FROM   SDE.sde_layer_stats 
     WHERE  layer_id = wanted_layer_id AND
            version_id = wanted_version_id;
            
    CURSOR G_layer_stats_nv_cursor (wanted_layer_id   IN  layer_id_t) IS
     SELECT layer_id
     FROM   SDE.sde_layer_stats 
     WHERE  layer_id = wanted_layer_id AND
            version_id IS NULL;          
            

   /* Local Subprograms. */

   PROCEDURE L_layer_stats_user_can_modify (wanted_layer_id   IN  layer_id_t, 
                                            wanted_version_id IN  version_id_t)
  /***********************************************************************
  *
  *N  {L_layer_stats_user_can_modify}  --  Can current user modify layer?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the layer_stats specified by layer_id  
  *   and version_id is modifiable by the current user 
  *   (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_layer_id  <IN>  ==  (layer_id_t)   layer to be tested.
  *     wanted_version_id <IN> ==  (version_id_t) version to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_LAYER_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago       06/23/2009           Original coding.
  *E
  ***********************************************************************/
   IS

      layer_id           SDE.sde_layer_stats.layer_id%TYPE;
      version_id         SDE.sde_layer_stats.version_id%TYPE;

   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      OPEN G_layer_stats_cursor (wanted_layer_id, wanted_version_id);
      FETCH G_layer_stats_cursor INTO layer_id,version_id;
      IF G_layer_stats_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_LAYER_NOEXIST,
                                  'Layer ' || TO_CHAR (layer_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_layer_stats_cursor;
    
   END L_layer_stats_user_can_modify;
   
   
   PROCEDURE L_layer_stats_user_can_modify (wanted_layer_id   IN  layer_id_t)
  /***********************************************************************
  *
  *N  {L_layer_stats_user_can_modify}  --  Can current user modify layer?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the layer_stats specified by layer_id  
  *   and a null version_id is modifiable by the current user 
  *   (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_layer_id  <IN>  ==  (layer_id_t)   layer to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_LAYER_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago       06/23/2009           Original coding.
  *E
  ***********************************************************************/
   IS

      layer_id           SDE.sde_layer_stats.layer_id%TYPE;

   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      OPEN G_layer_stats_nv_cursor (wanted_layer_id);
      FETCH G_layer_stats_nv_cursor INTO layer_id;
      IF G_layer_stats_nv_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_LAYER_NOEXIST,
                                  'Layer ' || TO_CHAR (layer_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_layer_stats_nv_cursor;
    
   END L_layer_stats_user_can_modify;  


   /* Public Subprograms. */

   PROCEDURE insert_layer_stats (layer_stats    IN  layer_stats_record_t)
  /***********************************************************************
  *
  *N  {insert_layer_stats}  --  insert a layer entry into the LAYER_STATS 
  *                             table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE 
  *   LAYER_STATS table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer_stats       <IN> == (layer_stats_record_t)   The new 
  *                                                  layer_stats to be
  *                                                  inserted. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago         06/23/2009           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a layer_stats is
      -- inserted.

      BEGIN

      -- Insert the record into the layer_stats table.

        INSERT INTO SDE.sde_layer_stats 
         (layer_id,version_id,
          minx,miny,maxx,maxy,minz,maxz,minm,maxm,
          total_features, total_points,last_analyzed)
        VALUES (layer_stats.layer_id,
                layer_stats.version_id,
                layer_stats.minx,
                layer_stats.miny,
                layer_stats.maxx,
                layer_stats.maxy,
                layer_stats.minz,
                layer_stats.maxz,
                layer_stats.minm,
                layer_stats.maxm,                
                layer_stats.total_features,
                layer_stats.total_points,
                layer_stats.last_analyzed);

      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error(sde_util.SE_LAYER_EXISTS,
                                     'Layer_Stats ' || TO_CHAR (layer_stats.layer_id) || 
                                     TO_CHAR (layer_stats.version_id) || 
                                     ' already exists.');
      END;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;


   END insert_layer_stats;


   PROCEDURE delete_layer_stats (old_layer_id          IN  layer_id_t,
                                 old_version_id        IN  version_id_t)
  /***********************************************************************
  *
  *N  {delete_layer_stats}  --  Delete an arbitary layer_stats.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary layer_stats entry.  
  *   All checking and locking should be performed by the gsrvr, except
  *   that we will check layer ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_layer_id  <IN>  ==  (layer_id_t)   layer to delete.
  *     old_version_id <IN> ==  (version_id_t) version to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_LAYER_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago      06/24/2009           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure layer_stats exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      IF old_version_id IS NULL THEN
        L_layer_stats_user_can_modify (old_layer_id);
      ELSE
        L_layer_stats_user_can_modify (old_layer_id, old_version_id);
      END IF;

      -- We've verified our permissions and the layer's existence, so it
      -- must be OK to delete them.  Do it.

      -- Delete the layer_stats.

      IF old_version_id IS NULL THEN
        DELETE FROM SDE.sde_layer_stats 
           WHERE layer_id = old_layer_id AND version_id IS NULL;
      ELSE
        DELETE FROM SDE.sde_layer_stats 
           WHERE layer_id = old_layer_id AND version_id = old_version_id;    
      END IF;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END delete_layer_stats;


   PROCEDURE update_layer_stats   (layer_stats   IN layer_stats_record_t)
  /***********************************************************************
  *
  *N  {update_layer_stats}  --  Update a layer_stats
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a layer's record.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check layer ownership if the invoking user is not 
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer        <IN>  ==  (layer_record_t) layer to be updated
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20020                SE_LAYER_NOEXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago        06/24/2009           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      IF layer_stats.version_id IS NULL THEN
        L_layer_stats_user_can_modify (layer_stats.layer_id);
      ELSE
        L_layer_stats_user_can_modify (layer_stats.layer_id, layer_stats.version_id);
      END IF;
    
      -- Update the layers.
      IF layer_stats.version_id IS NULL THEN
        UPDATE SDE.sde_layer_stats
        SET   minx = layer_stats.minx,
              miny = layer_stats.miny,
              maxx = layer_stats.maxx,
              maxy = layer_stats.maxy,
              minz = layer_stats.minz,
              minm = layer_stats.minm,
              maxz = layer_stats.maxz,
              maxm = layer_stats.maxm,
              total_features = layer_stats.total_features,
              total_points = layer_stats.total_points,
              last_analyzed = layer_stats.last_analyzed
        WHERE layer_id = layer_stats.layer_id and
              version_id IS NULL;
      ELSE
        UPDATE SDE.sde_layer_stats
        SET   minx = layer_stats.minx,
              miny = layer_stats.miny,
              maxx = layer_stats.maxx,
              maxy = layer_stats.maxy,
              minz = layer_stats.minz,
              minm = layer_stats.minm,
              maxz = layer_stats.maxz,
              maxm = layer_stats.maxm,
              total_features = layer_stats.total_features,
              total_points = layer_stats.total_points,
              last_analyzed = layer_stats.last_analyzed
        WHERE layer_id = layer_stats.layer_id and
              version_id = layer_stats.version_id;
      END IF;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_layer_stats;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END layer_stats_util;
