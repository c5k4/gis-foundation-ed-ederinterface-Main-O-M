--------------------------------------------------------
--  DDL for Package Body SDO_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."SDO_UTIL" 
/***********************************************************************
*
*N  {sdo_util.spb}  --  Oracle Spatial package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
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
*    Kevin Watt          12/08/00               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   /* Local Subprograms. */

   /* Public Subprograms. */

   PROCEDURE register_layer (layer            IN  layer_record_t,
                             gcol             IN  geocol_record_t,
                             registration     IN  registration_record_t)
  /***********************************************************************
  *
  *N  {register_layer}  --  Insert LAYERS and TABLE_REGISTRY for
  *                         Auto-Registered layers.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts into the LAYERS and TABLE_REGISTRY 
  *  for newly discovered Oracle Spatial tables. It check the 
  *  TABLE_REGISTRY first to see the table does not exist for
  *  unauthorized execution outside of the ArcSDE server.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     layer             <IN> == (layer_record_t)         The new layer to be
  *                                                        inserted. 
  *     gcol              <IN> == (geocol_record_t)        GEOMETRY_COLUMNS row.
  *     registration      <IN> == (registration_record_t)  g_table_name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt              12/08/00           Original code.
  *E
  ***********************************************************************/
   IS

     PRAGMA AUTONOMOUS_TRANSACTION;
     cnt   INTEGER := 0;

   BEGIN
     
   -- The following statement checks to see if the layer 
   -- owner and table exist in the ALL_SDO_GEOM_METADATA view. 
   -- This is preset to avoid inserting non-existent tables 
   -- into the ArcSDE metadata tables. 
      BEGIN
        EXECUTE IMMEDIATE
          'SELECT count(*) '||
          'FROM all_sdo_geom_metadata ' ||
          'WHERE owner = ' || '''' || TO_CHAR(layer.owner) || '''' ||
          ' AND table_name = ' || '''' || TO_CHAR(layer.table_name) || ''''
        INTO cnt;

        IF cnt = 0 THEN
     raise_application_error(SDE.sde_util.SE_TABLE_NOEXIST,
                                       'Owner ' || TO_CHAR(layer.owner) ||
                                       ' Table ' || TO_CHAR(layer.table_name) ||
                                       ' does not exist in ALL_SDO_GEOM_METADATA view.');
        END IF;


      -- Insert the record into the layers table.

        INSERT INTO SDE.layers 
           (layer_id,description,database_name,table_name,owner,
            spatial_column,eflags,layer_mask,gsize1,gsize2,gsize3,minx,
            miny,maxx,maxy,cdate,layer_config,optimal_array_size,
            stats_date,minimum_id,srid,base_layer_id)
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
                layer.base_layer_id);

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

      -- Insert the record into TABLE_REGISTRY table.  
      
        BEGIN   

            INSERT INTO SDE.table_registry
               (registration_id,table_name,owner,rowid_column,description,
                object_flags,registration_date,config_keyword,minimum_id,
                imv_view_name)
              VALUES (registration.registration_id,
                      registration.table_name,
                      registration.owner,
                      registration.rowid_column,
                      registration.description,
                      registration.object_flags,
                      registration.registration_date,
                      registration.config_keyword,
                      registration.minimum_id,
                      registration.imv_view_name);
         
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
              UPDATE SDE.table_registry 
                SET rowid_column = registration.rowid_column,
                    description  = registration.description,
                    object_flags = registration.object_flags,
                    config_keyword = registration.config_keyword,
                    minimum_id     = registration.minimum_id,
                    imv_view_name  = registration.imv_view_name 
                 WHERE table_name = registration.table_name AND
                             owner = registration.owner; 
             WHEN OTHERS THEN
               RAISE;
        END;                       
                                                                   
      EXCEPTION 
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE;
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
      END;

      -- Since we've gotten this far without an exception, it must be OK
      -- to commit.

      COMMIT;

   END register_layer;
  
END sdo_util;
