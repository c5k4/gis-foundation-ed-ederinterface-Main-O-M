Prompt drop Package LOCK_UTIL;
DROP PACKAGE SDE.LOCK_UTIL
/

Prompt Package LOCK_UTIL;
--
-- LOCK_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.lock_util
/***********************************************************************
*
*N  {lock_util.sps}  --  Interface for ArcSDE Shared Lock Package
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   operations on ArcSDE's shared layer, state, table and object locks.
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
*    Peter Aronson             01/25/00               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */

   SUBTYPE layer_lock_t IS SDE.layer_locks%ROWTYPE;
   SUBTYPE layer_id_t IS SDE.layer_locks.layer_id%TYPE;
   
   SUBTYPE state_lock_t IS SDE.state_locks%ROWTYPE;
   SUBTYPE state_id_t IS SDE.state_locks.state_id%TYPE;
   
   SUBTYPE table_lock_t IS SDE.table_locks%ROWTYPE;
   SUBTYPE table_id_t IS SDE.table_locks.registration_id%TYPE;
   
   SUBTYPE object_lock_t IS SDE.object_locks%ROWTYPE;

  /* Constants. */

   -- The following constant defines the release of lock_util and is used by
   -- the instance startup code to determine if the most up to date version of
   -- the package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1010;

   -- Constant names for autolock parameters.

   C_is_autolock           CONSTANT CHAR(1)  := 'Y';
   C_is_not_autolock       CONSTANT CHAR(1)  := 'N';
   
   -- Constant names for lock types.

   C_shared_lock           CONSTANT CHAR(1)  := 'S';
   C_exclusive_lock        CONSTANT CHAR(1)  := 'E';
   C_marked_lock           CONSTANT CHAR(1)  := 'M';

   C_shared_lock_all       CONSTANT CHAR(1)  := '-';
   C_exclusive_lock_all    CONSTANT CHAR(1)  := 'X';

  /* Procedures and Functions. */

   -- The following functions perform operations for layer locks stored in
   -- the SDE.LAYER_LOCKS table.  Each operation is an autonomous transaction.

   PROCEDURE add_layer_lock (layer_lock  IN  layer_lock_t);
   PROCEDURE delete_layer_lock (sde_id    IN  pinfo_util.sde_id_t,
                                layer_id  IN  layer_id_t,
                                autolock  IN  VARCHAR2);
   PROCEDURE delete_layer_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t);
   PROCEDURE update_layer_lock (layer_lock  IN  layer_lock_t);

   -- The following functions perform operations for state locks stored in
   -- the SDE.STATE_LOCKS table.  Each operation is an autonomous transaction.

   PROCEDURE add_state_lock (state_lock  IN  state_lock_t);
   PROCEDURE delete_state_lock (sde_id    IN  pinfo_util.sde_id_t,
                                state_id  IN  state_id_t,
                                autolock  IN  VARCHAR2);
   PROCEDURE delete_state_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t);

   -- The following functions perform operations for table locks stored in
   -- the SDE.TABLE_LOCKS table.  Each operation is an autonomous transaction.

   PROCEDURE add_table_lock (table_lock  IN  table_lock_t);
   PROCEDURE delete_table_lock (sde_id    IN  pinfo_util.sde_id_t,
                                table_id  IN  table_id_t);
   PROCEDURE delete_table_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t);

   -- The following functions perform operations for object locks stored in
   -- the SDE.OBJECT_LOCKS object.  Each operation is an autonomous transaction.

   PROCEDURE add_object_lock (object_lock  IN  object_lock_t);
   PROCEDURE delete_object_lock (object_lock  IN  object_lock_t);
   PROCEDURE delete_object_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t);

   -- The following procedures delete layer, table, state, object locks
   -- stored in SDE.LAYER_LOCKS, SDE.TABLE_LOCKS, SDE.STATE_LOCKS, 
   -- SDE.OBJECT_LOCKS respectively within a single autonomous transaction.

   PROCEDURE delete_all_locks_by_sde_id (sde_id  IN  pinfo_util.sde_id_t);
   PROCEDURE delete_all_locks_by_pid (sde_id  IN  pinfo_util.sde_id_t);
   PROCEDURE delete_all_orphaned_locks;

END lock_util;

/


Prompt Grants on PACKAGE LOCK_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.LOCK_UTIL TO PUBLIC WITH GRANT OPTION
/
