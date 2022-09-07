Prompt drop Package LAYERS_UTIL;
DROP PACKAGE SDE.LAYERS_UTIL
/

Prompt Package LAYERS_UTIL;
--
-- LAYERS_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.layers_util
/***********************************************************************
*
*N  {layers_util.sps}  --  Interface for layers DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on Layers.  It should be compiled by the
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

   -- Standard identifier for a Layers.

   SUBTYPE layer_id_t IS SDE.layers.layer_id%TYPE;
   SUBTYPE layer_grid_t   IS SDE.layers.gsize1%TYPE;
   SUBTYPE layer_record_t IS SDE.layers%ROWTYPE;

   -- Standard identifier for a geometry.

   SUBTYPE srid_t          IS SDE.layers.srid%TYPE;
   SUBTYPE geocol_record_t IS SDE.geometry_columns%ROWTYPE;
   SUBTYPE layer_mask_t    IS SDE.layers.layer_mask%TYPE;

   -- Standard identifier for a spatial_reference record.
  
   SUBTYPE spatial_record_t  IS SDE.spatial_references%ROWTYPE;
   SUBTYPE spref_srid_t      IS SDE.spatial_references.srid%TYPE;
   SUBTYPE spref_authname_t  IS SDE.spatial_references.auth_name%TYPE;
   SUBTYPE spref_xycluster_t IS SDE.spatial_references.xycluster_tol%TYPE;
   SUBTYPE spref_zcluster_t  IS SDE.spatial_references.zcluster_tol%TYPE;
   SUBTYPE spref_mcluster_t  IS SDE.spatial_references.mcluster_tol%TYPE;

   -- Standard table name type

   DEF_table_name  NVARCHAR2(160);
   DEF_schema_name NVARCHAR2(32);
   DEF_srtext      VARCHAR2(1024);
   DEF_column_name NVARCHAR2(32);
   SUBTYPE table_name_t  IS DEF_table_name%TYPE;
   SUBTYPE schema_name_t IS DEF_schema_name%TYPE;
   SUBTYPE srtext_t      IS DEF_srtext%TYPE;
   SUBTYPE column_name_t IS DEF_column_name%TYPE;
   
  /* Constants. */

   -- The following constant defines the release of layers_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1020;

  /* Procedures and functions. */

   -- The following functions perform DDL operations for layer objects
   -- stored in the SDE.LAYERS table.  These procedures all issue a COMMIT
   -- on success.
   
   PROCEDURE insert_layer (layer            IN  layer_record_t,
                           gcol             IN  geocol_record_t,
                           new_g_table_name IN  table_name_t);
   PROCEDURE delete_layer (old_layer_id  IN  layer_id_t); 
   PROCEDURE update_layer (layer     IN  layer_record_t); 
   PROCEDURE update_layer (layer     IN  layer_record_t,
                           new_spatial_column IN column_name_t); 
   PROCEDURE change_layer_table_name 
                              (new_table_name    IN  table_name_t,
                               wanted_layer_id   IN  layer_id_t);
   PROCEDURE update_spatial_references (wanted_layer_id   IN layer_id_t,
                                        new_srtext        IN srtext_t,
                                        new_xycluster_tol IN spref_xycluster_t,
                                        new_zcluster_tol  IN spref_zcluster_t,
                                        new_mcluster_tol  IN spref_mcluster_t);
   PROCEDURE update_layer_srid (wanted_layer_id IN layer_id_t,
                                new_srid        IN srid_t);
   PROCEDURE insert_spatial_ref (spatial IN  spatial_record_t);
   PROCEDURE lock_spatial_ref;

   PROCEDURE update_layer_mask (wanted_layer_id IN layer_id_t,
                                new_layer_mask  IN layer_mask_t);

   PROCEDURE update_spref_auth_srid (srid_in      IN spref_srid_t,
                                     auth_srid_in IN spref_srid_t,
                                     auth_name_in IN spref_authname_t);
                                     
   PROCEDURE update_layer_grids (owner_in    IN  VARCHAR2,
                                 table_in    IN  VARCHAR2,
                                 column_in   IN  VARCHAR2,
                                 grid1       IN  layer_grid_t,
                                 grid2       IN  layer_grid_t,
                                 grid3       IN  layer_grid_t);
   
   -- The following functions perform DML operations for layer objects
   -- stored in the SDE.LAYERS table.  These procedures occur in an
   -- autonomous transaction.
 
   PROCEDURE update_layer_extent(wanted_layer_id IN layer_id_t,
                                new_minx        IN SDE.layers.minx%TYPE,
                                new_miny        IN SDE.layers.miny%TYPE,
                                new_maxx        IN SDE.layers.maxx%TYPE,
                                new_maxy        IN SDE.layers.maxy%TYPE,
                                new_minz        IN SDE.layers.minz%TYPE,
                                new_minm        IN SDE.layers.minm%TYPE,
                                new_maxz        IN SDE.layers.maxz%TYPE,
                                new_maxm        IN SDE.layers.maxm%TYPE);
                                
   PROCEDURE update_layer_eflags         (owner_in    IN varchar2, 
                                          table_in    IN varchar2,
                                          column_in   IN varchar2,
                                          new_eflags  IN number);
  
   PRAGMA RESTRICT_REFERENCES (layers_util,WNDS,WNPS);

END layers_util;

/


Prompt Grants on PACKAGE LAYERS_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.LAYERS_UTIL TO PUBLIC WITH GRANT OPTION
/
