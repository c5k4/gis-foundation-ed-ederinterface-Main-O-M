--------------------------------------------------------
--  DDL for Package Body LAYERS_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."LAYERS_UTIL" 
/***********************************************************************
*
*N  {layers_util.spb}  --  Implementation for layer DDL package 
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
*    Yung-Ting Chen           03/08/99               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;

   CURSOR G_layer_owner_cursor (wanted_layer_id  IN  layer_id_t) IS
     SELECT owner,base_layer_id
     FROM   SDE.layers 
     WHERE  layer_id = wanted_layer_id;

   /* Local Subprograms. */

   PROCEDURE L_layer_user_can_modify (wanted_layer_id  IN  layer_id_t)
  /***********************************************************************
  *
  *N  {L_layer_user_can_modify}  --  Can current user modify layer?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the layer specified by ID exists and is
  *   modifiable by the current user (who must be owner or SDE DBA).
  *   modifiable by the current user (who must be owner or sde DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_layer_id  <IN>  ==  (layer_id_t) Layer to be tested.
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
  *    Yung-Ting Chen          03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

      layer_owner           SDE.layers.owner%TYPE;
      layer_base_layer_id   SDE.layers.base_layer_id%TYPE;

   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      OPEN G_layer_owner_cursor (wanted_layer_id);
      FETCH G_layer_owner_cursor INTO layer_owner,layer_base_layer_id;
      IF G_layer_owner_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_LAYER_NOEXIST,
                                  'Layer ' || TO_CHAR (wanted_layer_id) ||
                                  ' not found.');
      END IF;
      CLOSE G_layer_owner_cursor;
      IF NOT G_sde_dba THEN
         IF layer_base_layer_id = 0 THEN
           IF G_current_user != layer_owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of layer ' ||
                                     TO_CHAR (wanted_layer_id) || '.');
           END IF;
         END IF;
      END IF;
    
   END L_layer_user_can_modify;


   /* Public Subprograms. */

   PROCEDURE insert_layer (layer            IN  layer_record_t,
                           gcol             IN  geocol_record_t,
                           new_g_table_name IN  table_name_t)
  /***********************************************************************
  *
  *N  {insert_layer}  --  insert a layer entry into the LAYERS table and
  *                       geometry_columns table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the SDE LAYERS
  *   table and SDE GEOMETRY_COLUMNS. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer             <IN> == (layer_record_t)   The new layer to be
  *                                                  inserted. 
  *     gcol              <IN> == (geocol_record_t)  GEOMETRY_COLUMNS row.
  *     new_g_table_name  <IN> == (table_name_T)     g_table_name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Yung-Ting Chen          03/08/99           Original coding.
  *    Kevin Watt              10/31/00           GEOMETRY_COLUMNS added.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a layer is
      -- inserted.

      BEGIN

      -- Insert the record into the layers table.

        INSERT INTO SDE.layers 
         (layer_id,description,database_name,table_name,owner,
          spatial_column,eflags,layer_mask,gsize1,gsize2,gsize3,minx,
          miny,maxx,maxy,cdate,layer_config,optimal_array_size,
          stats_date,minimum_id,srid,base_layer_id,minz,minm,maxz,maxm,
          secondary_srid)
        VALUES (layer.layer_id,
                layer.description,
                layer.database_name,
                layer.table_name,
                layer.owner,
                layer.spatial_column,
                layer.eflags,
                layer.layer_mask,
                layer.gsize1,
                layer.gsize2,
                layer.gsize3,
                layer.minx,
                layer.miny,
                layer.maxx,
                layer.maxy,
                layer.cdate,
                layer.layer_config,
                layer.optimal_array_size,
                layer.stats_date,
                layer.minimum_id,
                layer.srid,
                layer.base_layer_id,
                layer.minz,
                layer.minm,
                layer.maxz,
                layer.maxm,
                layer.secondary_srid);

      -- Insert the record into the geometry_columns table

        INSERT INTO SDE.geometry_columns
               (f_table_catalog,
                f_table_schema,
                f_table_name,
                f_geometry_column,
                g_table_catalog,
                g_table_schema,
                g_table_name,
                storage_type,
                geometry_type,
                coord_dimension,
                max_ppr,
                srid)
        VALUES (gcol.f_table_catalog,
                gcol.f_table_schema,
                gcol.f_table_name,
                gcol.f_geometry_column,
                gcol.g_table_catalog,
                gcol.g_table_schema,
                gcol.g_table_name,
                gcol.storage_type,
                gcol.geometry_type,
                gcol.coord_dimension,
                gcol.max_ppr,
                gcol.srid);

      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error(sde_util.SE_LAYER_EXISTS,
                                     'Layer ' || TO_CHAR (layer.layer_id) ||
                                     ' already exists.');
      END;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;


   END insert_layer;


   PROCEDURE delete_layer (old_layer_id          IN  layer_id_t)
  /***********************************************************************
  *
  *N  {delete_layer}  --  Delete an arbitary layer and geometry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary layer and geometry entry.  
  *   All checking and locking should be performed by the gsrvr, except
  *   that we will check layer ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_layer_id  <IN>  ==  (layer_id_t)  layer to delete.
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
  *    Yung-Ting Chen         03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure layer exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_layer_user_can_modify (old_layer_id);

      -- We've verified our permissions and the layer's existence, so it
      -- must be OK to delete them.  Do it.


      -- Delete the geometry_columns.

      DELETE FROM SDE.geometry_columns WHERE
            (f_table_schema, f_table_name, f_geometry_column) = 
            (SELECT owner, table_name,spatial_column FROM
             SDE.layers WHERE layer_id = old_layer_id); 

      -- Make sure that something was deleted.

       IF SQL%NOTFOUND THEN
          raise_application_error (sde_util.SE_GEOMETRYCOL_NOEXIST,
                                   'GEOMETRY_COLUMNS entry not found. ');
       END IF;

      -- Delete the layers.

      DELETE FROM SDE.layers WHERE layer_id = old_layer_id;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END delete_layer;



   PROCEDURE update_layer     (layer      IN layer_record_t)
  /***********************************************************************
  *
  *N  {update_layer}  --  Update a layer
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
  *    Yung-Ting Chen           03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      L_layer_user_can_modify (layer.layer_id);
    
      -- Update the layers.

      UPDATE SDE.layers
      SET   description = layer.description,
            gsize1 = layer.gsize1,
            gsize2 = layer.gsize2,
            gsize3 = layer.gsize3,
            minx = layer.minx,
            miny = layer.miny,
            maxx = layer.maxx,
            maxy = layer.maxy,
            eflags = layer.eflags,
            layer_mask = layer.layer_mask,
            layer_config = layer.layer_config,
            optimal_array_size = layer.optimal_array_size,
            stats_date = layer.stats_date,
            minimum_id = layer.minimum_id,
            minz = layer.minz,
            minm = layer.minm,
            maxz = layer.maxz,
            maxm = layer.maxm,
            secondary_srid = layer.secondary_srid
      WHERE layer_id = layer.layer_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_layer;

   PROCEDURE update_layer     (layer          IN layer_record_t,
                               new_spatial_column IN column_name_t)
  /***********************************************************************
  *
  *N  {update_layer}  --  Update a layer with the option to rename a 
  *                       spatial column.  To be used only with 
  *                       storage migration utility.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a layer's record with the option to 
  *   replace the spatial column.  This procedure should be used only 
  *   for the storage migration utility.
  *  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check layer ownership if the invoking user is not 
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer          <IN>  ==  (layer_record_t) layer to be updated
  *     new_spatial_column <IN>  ==  (column_name_t) spatial column
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
  *    Josefina Santiago   01/18/08   Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the layer exists, and that the current user can
      -- write to it.

      L_layer_user_can_modify (layer.layer_id);
    
      -- Update the layers.

      UPDATE SDE.layers
      SET   spatial_column = new_spatial_column, 
            description = layer.description,
            gsize1 = layer.gsize1,
            gsize2 = layer.gsize2,
            gsize3 = layer.gsize3,
            minx = layer.minx,
            miny = layer.miny,
            maxx = layer.maxx,
            maxy = layer.maxy,
            eflags = layer.eflags,
            layer_mask = layer.layer_mask,
            layer_config = layer.layer_config,
            optimal_array_size = layer.optimal_array_size,
            stats_date = layer.stats_date,
            minimum_id = layer.minimum_id,
            minz = layer.minz,
            minm = layer.minm,
            maxz = layer.maxz,
            maxm = layer.maxm,
            secondary_srid = layer.secondary_srid
      WHERE layer_id = layer.layer_id;

   END update_layer;


   PROCEDURE update_layer_extent(wanted_layer_id IN layer_id_t,
                                 new_minx        IN SDE.layers.minx%TYPE,
                                 new_miny        IN SDE.layers.miny%TYPE,
                                 new_maxx        IN SDE.layers.maxx%TYPE,
                                 new_maxy        IN SDE.layers.maxy%TYPE,
                                 new_minz        IN SDE.layers.minz%TYPE,
                                 new_minm        IN SDE.layers.minm%TYPE,
                                 new_maxz        IN SDE.layers.maxz%TYPE,
                                 new_maxm        IN SDE.layers.maxm%TYPE)
  /***********************************************************************
  *
  *N  {update_layer_extent}  --  Update a layer extent
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a layer's envelope in the layers table.  
  *   It does not perform a commit.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer    <IN>  ==  (layer_id_t) layer id of the layer to be updated
  *     new_minx <IN>  ==  (<schema>.layers.minx%TYPE) possible new value of minx 
  *     new_miny <IN>  ==  (<schema>.layers.miny%TYPE) possible new value of miny 
  *     new_maxx <IN>  ==  (<schema>.layers.maxx%TYPE) possible new value of maxx 
  *     new_maxy <IN>  ==  (<schema>.layers.maxy%TYPE) possible new value of maxy
  *     new_minz <IN>  ==  (<schema>.layers.minz%TYPE) possible new value of minz 
  *     new_minm <IN>  ==  (<schema>.layers.minm%TYPE) possible new value of minm 
  *     new_maxz <IN>  ==  (<schema>.layers.maxz%TYPE) possible new value of maxz 
  *     new_maxm <IN>  ==  (<schema>.layers.maxm%TYPE) possible new value of maxm  
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
  *    Annette Locke        04/08/99           Original coding.
  *E
  ***********************************************************************/
   IS
      PRAGMA AUTONOMOUS_TRANSACTION;

      eminz        SDE.LAYERS.minz%TYPE;
      emaxz        SDE.LAYERS.maxz%TYPE;
      min_measure  SDE.LAYERS.minm%TYPE;
      max_measure  SDE.LAYERS.maxm%TYPE;

     CURSOR find_zm_cursor (wanted_layer_id IN layer_id_t) IS
       SELECT minz, minm, maxz, maxm
       FROM   SDE.layers 
       WHERE  layer_id  = wanted_layer_id;

      zmextent     find_zm_cursor%ROWTYPE;

   BEGIN 

      OPEN find_zm_cursor(wanted_layer_id);
      FETCH find_zm_cursor INTO zmextent;
      IF find_zm_cursor%NOTFOUND THEN
         CLOSE find_zm_cursor;
         raise_application_error (sde_util.SE_LAYER_NOEXIST,
                                 'Layer id ' || TO_CHAR(wanted_layer_id) || 
                                 ' not found.');
      END IF;
      CLOSE find_zm_cursor;

      -- Update the layer envelope.
      IF zmextent.minz IS NULL THEN eminz := new_minz; 
      ELSIF new_minz IS NULL THEN eminz := zmextent.minz;
      ELSE eminz := LEAST(zmextent.minz, new_minz);
      END IF;

      IF zmextent.minm IS NULL THEN min_measure := new_minm; 
      ELSIF new_minm IS NULL THEN min_measure := zmextent.minm;
      ELSE min_measure := LEAST(zmextent.minm, new_minm);
      END IF;
      
      IF zmextent.maxz IS NULL THEN emaxz := new_maxz; 
      ELSIF new_maxz IS NULL THEN emaxz := zmextent.maxz;
      ELSE emaxz := GREATEST(zmextent.maxz, new_maxz);
      END IF;
      
      IF zmextent.maxm IS NULL THEN max_measure := new_maxm; 
      ELSIF new_maxm IS NULL THEN max_measure := zmextent.maxm;
      ELSE max_measure := GREATEST(zmextent.maxm, new_maxm);
      END IF; 

      -- Update the layer envelope and ZM extent.
      UPDATE SDE.layers
      SET
            minx = LEAST(minx, new_minx),
            miny = LEAST(miny, new_miny),
            maxx = GREATEST(maxx, new_maxx),
            maxy = GREATEST(maxy, new_maxy),
        minz = eminz,
            minm = min_measure,
            maxz = emaxz,
            maxm = max_measure
      WHERE layer_id = wanted_layer_id;

      -- Make sure that something was updated.

      IF SQL%NOTFOUND THEN
         raise_application_error (sde_util.SE_LAYER_NOEXIST,
                                  'Layer id ' || TO_CHAR(wanted_layer_id) || 
                                  ' not found.');
      END IF;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK; -- Release the lock.
         RAISE;

   END update_layer_extent;


   PROCEDURE change_layer_table_name (new_table_name  IN table_name_t,
                                      wanted_layer_id IN layer_id_t)
  /***********************************************************************
  *
  *N  {change_layer_table_name}  --  change an arbitary layer's and
  *                                  geometry's table name.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure changes an arbitary layer's  and gemetry's table name.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check layer ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     new_table_name  <IN>  ==  (table_name_t)  new table name.
  *     wanted_layer_id    <IN>  ==  (layer_id_t)  layer to change.
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
  *    Yung-Ting Chen      03/08/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Make sure layer exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_layer_user_can_modify (wanted_layer_id);

      -- We've verified our permissions and the layer's existence,
      -- so it must be OK to delete them.  Do it.

      -- Update the geometry_columns.

      UPDATE SDE.geometry_columns SET f_table_name = new_table_name
           WHERE (f_table_schema, f_table_name, f_geometry_column) =
                 (SELECT owner, table_name, spatial_column
                  FROM SDE.layers WHERE layer_id = wanted_layer_id);

      -- Make sure that something was updated.

       IF SQL%NOTFOUND THEN
          raise_application_error (sde_util.SE_GEOMETRYCOL_NOEXIST,
                                   'GEOMETRY_COLUMNS entry not found. ');
       END IF;

      -- Update the layers.
      
      UPDATE SDE.layers SET table_name = new_table_name
           WHERE layer_id = wanted_layer_id;

      -- Since we've gotten this far without an exception, it must be
      -- OK to commit.

      COMMIT;

   END change_layer_table_name;

   PROCEDURE change_g_table_name (new_table_name  IN table_name_t,
                                  wanted_layer_id IN layer_id_t)
   IS

   BEGIN

      -- Make sure layer exists, and (if we are not the SDE DBA)
      -- that the current user owns it.

      L_layer_user_can_modify (wanted_layer_id);

      -- We've verified our permissions and the layer's existence,
      -- so it must be OK to delete them.  Do it.

      -- Update the geometry_columns.

      UPDATE SDE.geometry_columns SET g_table_name = new_table_name
           WHERE (f_table_schema, f_table_name, f_geometry_column) =
                 (SELECT owner, table_name, spatial_column
                  FROM SDE.layers WHERE layer_id = wanted_layer_id);

      -- Make sure that something was updated.

       IF SQL%NOTFOUND THEN
          raise_application_error (sde_util.SE_GEOMETRYCOL_NOEXIST,
                                   'GEOMETRY_COLUMNS entry not found. ');
       END IF;

     -- Since we've gotten this far without an exception, it must be
      -- OK to commit.

      COMMIT;

   END change_g_table_name;


   PROCEDURE update_spatial_references (wanted_layer_id   IN layer_id_t,
                                        new_srtext        IN srtext_t,
                                        new_xycluster_tol IN spref_xycluster_t,
                                        new_zcluster_tol  IN spref_zcluster_t,
                                        new_mcluster_tol  IN spref_mcluster_t)
  /***********************************************************************
  *
  *N  {update_spatial_references}  --  Update a projection string of a 
  *                        spatial_reference entry, cluster tolerance values.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a spatial reference's projection string.  
  *   All checking and locking should be performed by the gsrvr, 
  *   except that we will check layer ownership if the invoking user is not 
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters
  *     wanted_layer_id     <IN>  ==  (layer_id_t) layer ID
  *     new_srtext          <IN>  ==  (srtext_t) projection string
  *     new_xycluster_tol   <IN>  ==  (spref_xycluster_t) New xycluster_tol
  *     new_zcluster_tol    <IN>  ==  (spref_zcluster_t)  New zcluster_tol
  *     new_mcluster_tol    <IN>  ==  (spref_mcluster_t)  New mcluster_tol
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
  *    Yung-Ting Chen              03/08/99           Original coding.
  *    K. Jonathan Shim            10/20/05           Added cluster_tol.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the layer exists, and that the current user
      -- can write to it.

      L_layer_user_can_modify (wanted_layer_id);
    
      -- Update the spatial_references.

      UPDATE SDE.spatial_references
      SET   srtext = new_srtext,
            xycluster_tol = new_xycluster_tol,
            zcluster_tol  = new_zcluster_tol,
            mcluster_tol  = new_mcluster_tol
      WHERE srid = 
            (SELECT srid from SDE.layers WHERE layer_id = wanted_layer_id);

      -- Make sure that something was updated.

         IF SQL%NOTFOUND THEN
            raise_application_error (sde_util.SE_SPATIALREF_NOEXIST,
                                     'Spatial Reference entry not found. ');
         END IF;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_spatial_references;


   PROCEDURE update_layer_srid   (wanted_layer_id IN layer_id_t,
                                  new_srid        IN srid_t)
  /***********************************************************************
  *
  *N  {update_layer_srid}  --  Update srid of the layer
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a layer's srid.
  *   All checking and locking should be performed by the gsrvr,
  *   except that we will check layer ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_layer_id   <IN>  ==  (layer_id_t) layer ID
  *     new_srid          <IN>  ==  (srid_t) spatial reference ID
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
  *    Yung-Ting Chen            03/08/99           Original coding.
  *    Peter Aronson           06/23/05           Also update GEOMETRY_COLUMNS.
  *E
  ***********************************************************************/
   IS

   wanted_g_table_name  VARCHAR2(16);

   BEGIN
     

      -- Make sure that the layer exists, and that the current user
      -- can write to it.

      L_layer_user_can_modify (wanted_layer_id);

      -- Update the LAYERS table.

      UPDATE SDE.layers
      SET   srid = new_srid
      WHERE layer_id = wanted_layer_id;

      -- Update the GEOMETRY_COLUMNS table.
 
      wanted_g_table_name := 'F' || TO_CHAR (wanted_layer_id);

      UPDATE SDE.geometry_columns
      SET   srid = new_srid
      WHERE g_table_name = TO_NCHAR(wanted_g_table_name);


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_layer_srid;


PROCEDURE update_layer_mask (wanted_layer_id IN layer_id_t,
                             new_layer_mask  IN layer_mask_t)
  /***********************************************************************
  *
  *N  {update_layer_mask}  --  Updates layer_mask of the layer
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a layer's layer_mask.
  *   All checking and locking should be performed by the gsrvr,
  *   except that we will check layer ownership if the invoking user is not
  *   the SDE DBA.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     wanted_layer_id   <IN>  ==  (layer_id_t) layer ID
  *     layer_mask        <IN>  ==  new_layer_mask  IN layer_mask_t)
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
  *    Sanjay Magal           09/17/02           Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      -- Make sure that the layer exists, and that the current user
      -- can write to it.

      L_layer_user_can_modify (wanted_layer_id);

      -- Update the layers.

      UPDATE SDE.layers
      SET   layer_mask = new_layer_mask
      WHERE layer_id = wanted_layer_id;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_layer_mask;
 



   PROCEDURE insert_spatial_ref (spatial IN spatial_record_t)
  /***********************************************************************
  *
  *N  {insert_spatial_ref}  --  Add record to spatial_references
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure adds an entry to spatial_references table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    spatial <IN>  ==  (spatial_record_t)  new spatial_references
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
  *    Yung-Ting Chen         03/11/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a spatial reference entry is
      -- inserted.

      BEGIN

        INSERT INTO SDE.spatial_references 
          (srid, falsex, falsey,xyunits,falsez,zunits, falsem,munits,
           xycluster_tol,zcluster_tol,mcluster_tol, srtext,object_flags,
           auth_name,auth_srid)
         VALUES (spatial.srid, spatial.falsex, 
                 spatial.falsey,spatial.xyunits,
                 spatial.falsez,
                 spatial.zunits,
                 spatial.falsem,
                 spatial.munits,
                 spatial.xycluster_tol,
                 spatial.zcluster_tol,
                 spatial.mcluster_tol,
                 spatial.srtext,
                 spatial.object_flags,
                 spatial.auth_name,
                 spatial.auth_srid);

      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
             raise_application_error(sde_util.SE_SPATIALREF_EXISTS,
                                     'Spatial reference entry ' || 
                                      TO_CHAR (spatial.srid) ||
                                     ' already exists.');
      END;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END insert_spatial_ref;

   PROCEDURE update_spref_auth_srid (srid_in      IN spref_srid_t,
                                     auth_srid_in IN spref_srid_t,
                                     auth_name_in IN spref_authname_t)
  /***********************************************************************
  *
  *N  {update_spref_auth_srid}  --  Update authsrid/name in 
  *                                 spatial_references table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the auth_srid and auth_name for 
  *  an existing srid that has no auth information.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    srid_in      <IN> == (spref_srid_t)     SRID.
  *    auth_srid_in <IN> == (spref_srid_t)     New auth_srid.
  *    auth_name_in <IN> == (spref_authname_t) New auth_name.
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
  *    Yung-Ting Chen         03/11/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- The following block is present to catch DUP_VAL_ON_INDEX
      -- exception that occurs when a spatial reference entry is
      -- inserted.

      BEGIN

        UPDATE SDE.spatial_references 
          SET auth_srid = auth_srid_in,
              auth_name = auth_name_in 
        WHERE srid = srid_in;

      END;


      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END update_spref_auth_srid;


   PROCEDURE lock_spatial_ref
  /***********************************************************************
  *
  *N  {lock_spatial_refernces}  --  Lock spatial_references
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
  *    Yung-Ting Chen         03/11/99           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      LOCK TABLE SDE.spatial_references IN EXCLUSIVE MODE;
 
   END lock_spatial_ref;
   
PROCEDURE update_layer_grids (owner_in    IN  VARCHAR2,
                              table_in    IN  VARCHAR2,
                              column_in   IN  VARCHAR2,
                              grid1       IN  layer_grid_t,
                              grid2       IN  layer_grid_t,
                              grid3       IN  layer_grid_t)
  /***********************************************************************
  *
  *N  {update_layer_grid}  --  Update Layers grid
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates the LAYERS grid values and sets the tlm
  *   to allow sessions to pick up the change. 
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
  *    Yung-Ting Chen         03/11/99           Original coding.
  *E
  ***********************************************************************/
   IS
     tlm      SDE.sde_util.tlm_record_t;
   BEGIN

      UPDATE SDE.LAYERS set gsize1 = grid1, 
                            gsize2 = grid2,
                            gsize3 = grid3
      WHERE owner = owner_in AND table_name = table_in AND 
            spatial_column = column_in;
                            
     tlm.table_name := 'LAYERS';
     SDE.sde_util.set_table_last_modified(tlm);
 
   END update_layer_grids;
   
Procedure update_layer_eflags          (owner_in    IN varchar2, 
                                        table_in    IN varchar2,
                                        column_in   IN varchar2,
                                        new_eflags  IN number)
  /***********************************************************************
  *
  *n  {update_layer_eflags}  --  Updates the LAYERS eflags for 
  *                               Index create/drop.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     This function updates the SDE.Layers eflags field for 
  *  ODCIIndexCreate and ODCIIndexDrop operations. The eflags represents
  *  the properties of the object. 
  *  SE_LOAD_MODE_MASK and SE_LAYER_HAS_NO_SPATIAL_INDEX are removed
  *  when ODCIIndexCreate executes and added for ODCIIndexDrop.
  *  
  *  The new_eflags represents the 'complete' list of properties of
  *  the object. Changes have be XOR'd from the original. 
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     owner_in   <in> ==  (varchar2) owner 
  *     table_in   <in> ==  (varchar2) table name
  *     column_in  <in> ==  (varchar2) spatial column name
  *     new_eflags <in> ==  (number) new (update) eflags
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
  IS
    tlm      SDE.sde_util.tlm_record_t;
 Begin 
    Update SDE.Layers SET eflags = new_eflags 
    Where owner = owner_in and table_name = table_in and
          spatial_column = column_in;
          
    Commit;
    
    tlm.table_name := 'LAYERS';
    SDE.sde_util.set_table_last_modified(tlm);
    
  End update_layer_eflags;
  

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END layers_util;
