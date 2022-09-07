Prompt drop Package Body ARCHIVE_UTIL;
DROP PACKAGE BODY SDE.ARCHIVE_UTIL
/

Prompt Package Body ARCHIVE_UTIL;
--
-- ARCHIVE_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.archive_util
/***********************************************************************
*
*N  {archive_util.spb}  --  Implementation for archive DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on sde_archive.  It should be compiled by the
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
*    Josefina Santiago         10/17/05               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          NVARCHAR2(32);

   /* Local Subprograms. */

   PROCEDURE insert_archive (archive IN archive_record_t)
  /***********************************************************************
  *
  *N  {insert_archive}  --  insert an archive entry into the 
  *                              SDE_archive table.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the
  *   SDE.SDE_archives table. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     archive  <IN>  ==  (archive_record_t) The new archive
  *                                           to be inserted. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago            10/17/05           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Insert the record into the sde_archive table; use a block to
      -- catch the DUP_VAL_ON_INDEX exception that occurs when an
      -- archive is inserted.

      INSERT INTO SDE.sde_archives
         (archiving_regid, history_regid,from_date, to_date,
          archive_date, archive_flags)
      VALUES (archive.archiving_regid,
                archive.history_regid,
                archive.from_date,
                archive.to_date,
                archive.archive_date,
                archive.archive_flags);
  
      COMMIT;


      EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
           raise_application_error(sde_util.SE_TABLE_REGISTERED,
                                  'Archiving ' ||
                                  TO_CHAR (archive.archiving_regid) ||
                                  ' already exists.');
  
   END insert_archive;


   PROCEDURE delete_archive (old_archiving_regid  IN  archiving_regid_t)
  /***********************************************************************
  *
  *N  {delete_archive}  --  Delete an arbitary archive
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an arbitary archive.  All checking
  *   and locking should be performed by the gsrvr. 
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     old_archiving_regid  <IN>  ==  (archiving_regid_t)  archive to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20220                SE_TABLE_NOREGISTERED
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago           10/17/05           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      DELETE FROM SDE.sde_archives WHERE archiving_regid = old_archiving_regid;

      -- Since we've gotten this far without an exception, it must be OK to
      -- commit.

      COMMIT;

   END delete_archive;

BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END archive_util;

/


Prompt Grants on PACKAGE ARCHIVE_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ARCHIVE_UTIL TO PUBLIC WITH GRANT OPTION
/
