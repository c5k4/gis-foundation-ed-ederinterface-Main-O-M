--------------------------------------------------------
--  DDL for Package Body PINFO_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."PINFO_UTIL" 
/***********************************************************************
*
*N  {pinfo_util.spb}  --  Implementation for pinfo DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on pinfo entries.  It should be compiled by the
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
*    Peter Aronson             01/25/00               Original coding.
*E
***********************************************************************/
IS

   /* Package Globals. */

   G_sde_dba               BOOLEAN NOT NULL DEFAULT FALSE;
   G_current_user          SDE.sde_util.identifier_t;
   G_sde_id                sde_id_t;


   /* Local Subprograms. */

   PROCEDURE L_pinfo_user_can_modify (sde_id  IN  sde_id_t)
  /***********************************************************************
  *
  *N  {L_pinfo_user_can_modify}  --  Can current user modify pinfo entry?
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure tests if the pinfo entry specified by pid exists and
  *   is modifiable by the current user (who must be owner or SDE DBA).
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_it  <IN>  ==  (sde_id_t) Identifying pid for process information
  *                        entry to be tested.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20097                SE_PROCESS_NOT_FOUND
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS

      CURSOR pinfo_owner_cursor (pinfo_wanted_sde_id  IN  sde_id_t) IS
        SELECT owner
        FROM   SDE.process_information p
        WHERE  p.sde_id = pinfo_wanted_sde_id;

      pinfo_owner           pinfo_owner_cursor%ROWTYPE;

   BEGIN

      -- Make sure that the pinfo entry exists, and that the current user 
      -- can write to it.

      OPEN pinfo_owner_cursor (sde_id);
      FETCH pinfo_owner_cursor INTO pinfo_owner;
      IF pinfo_owner_cursor%NOTFOUND THEN
         CLOSE pinfo_owner_cursor;
         raise_application_error (sde_util.SE_PROCESS_NOT_FOUND,
                                  'Process Record for ' || TO_CHAR (sde_id) ||
                                  ' not found.');
      END IF;
      CLOSE pinfo_owner_cursor;
      IF NOT G_sde_dba THEN
         IF G_current_user != pinfo_owner.owner THEN
            raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                     'Not owner of process record for ' ||
                                     TO_CHAR (sde_id) || '.');
         END IF;
      END IF;
    
   END L_pinfo_user_can_modify;

   
   /* Public Subprograms. */

   PROCEDURE insert_pinfo (sde_id          IN  sde_id_t,
                           server_id       IN  server_id_t,
                           direct_connect  IN  VARCHAR2,
                           sysname         IN  sysname_t,
                           nodename        IN  nodename_t,
                           xdr_needed      IN  VARCHAR2)
  /***********************************************************************
  *
  *N  {insert_pinfo}  --  Insert pinfo entry into PROCESS_INFORMATION table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the 
  *   SDE.PROCESS_INFORMATION table as an autonomous transaction.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id          <IN>  ==  (sde_id_t) The unique SDE session id for
  *                                this pinfo entry.
  *     server_id       <IN>  ==  (server_id_t) The pid (Unix) or process
  *                                id (Windows) of this connection's if this
  *                                iomgr-mediated connection, otherwise it
  *                                is the process id of the client if this
  *                                is a direct connection.
  *     direct_connect  <IN>  ==  (VARCHAR2) If 'Y' (true) then this pinfo
  *                                record is for a direct connection, and
  *                                should automatically be cleaned up.  If
  *                                'N' (false), then this pinfo record is
  *                                for an iomgr-mediated connection, and
  *                                should only be cleaned up by the iomgr.
  *     sysname         <IN>  ==  (sysname_t) The Client System  Name.
  *     nodename        <IN>  ==  (nodename_t) The Client Host Node Name.
  *     xdr_needed      <IN>  ==  (VARCHAR2) If 'Y' (true) then this pinfo
  *                                record is for xdr needed. If 'N' (false),
  *                                then this pinfo record is for an
  *                                iomgr-mediated connection.
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
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

     CURSOR session_audsid_cursor IS
       SELECT SYS_CONTEXT ('USERENV', 'SESSIONID') audsid
       FROM   DUAL;

     session_audsid  INTEGER;

   BEGIN
      
      -- Just in case there is a lingering entry, delete, but ignore
      -- SE_PROCESS_NOT_FOUND errors.

      BEGIN

         L_pinfo_user_can_modify (sde_id);
         DELETE FROM SDE.process_information
         WHERE  sde_id = insert_pinfo.sde_id;

      EXCEPTION

         WHEN OTHERS THEN
            IF SQLCODE <> sde_util.SE_PROCESS_NOT_FOUND THEN
               RAISE;
            END IF;

      END;

      -- Start by fetching the current auditing session id.

      OPEN session_audsid_cursor;
      FETCH session_audsid_cursor INTO session_audsid;
      IF session_audsid_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_PROCESS_NOT_FOUND,
              'audit session id for sde_id ' || TO_CHAR (sde_id) ||
              ' not found.');
      END IF;
      CLOSE session_audsid_cursor;

      
      -- Perform the actual insert.
      
      INSERT INTO SDE.process_information
        (sde_id,server_id,audsid,start_time,rcount,wcount,opcount,numlocks,fb_partial,
         fb_count,fb_fcount,fb_kbytes,owner,direct_connect,
         sysname,nodename,xdr_needed)
      VALUES
        (insert_pinfo.sde_id,insert_pinfo.server_id,session_audsid,SYSDATE,0,0,0,0,0,0,0,0,
         G_current_user,insert_pinfo.direct_connect,insert_pinfo.sysname,
         insert_pinfo.nodename,insert_pinfo.xdr_needed);

      -- Got this far without an exception, it's safe to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END insert_pinfo;


   PROCEDURE delete_pinfo (sde_id  IN  sde_id_t)
  /***********************************************************************
  *
  *N  {insert_pinfo}  --  Delete pinfo entry from PROCESS_INFORMATION table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an pinfo entry from the
  *   SDE.PROCESS_INFORMATION table as an autonomous transaction.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id   <IN>  ==  (sde_id_t) The identifying sde id for pinfo entry
  *                         to be deleted.      
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20097                SE_PROCESS_NOT_FOUND
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      -- Check permissions.

      L_pinfo_user_can_modify (sde_id);
      
      -- Perform the delete.
      
      DELETE FROM SDE.process_information
      WHERE  sde_id = delete_pinfo.sde_id;

      -- Got this far without an exception, it's safe to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END delete_pinfo;

  PROCEDURE delete_pinfo_by_pid (sde_id     IN  sde_id_t)
  /***********************************************************************
  *
  *N  {delete_pinfo_by_pid}  --  Delete pinfo entry from PROCESS_INFORMATION table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes pinfo entries from the
  *   SDE.PROCESS_INFORMATION table as an autonomous transaction.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id   <IN>  ==  (sde_id_t) The identifying sde_id for pinfo entries
  *                                to be deleted
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20097                SE_PROCESS_NOT_FOUND
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Sanjay Magal                 10/15/08           Original coding.
  *    Sanjay Magal                 01/04/10           Update logic.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
            
      -- Perform the delete.
      
      DELETE FROM SDE.process_information
      WHERE  sde_id = delete_pinfo_by_pid.sde_id
        OR (proxy_yn = 'Y' AND parent_sde_id = delete_pinfo_by_pid.sde_id);

      -- Got this far without an exception, it's safe to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END delete_pinfo_by_pid;

   PROCEDURE insert_pinfo_for_proxy (sde_id          IN  sde_id_t,
                                     server_id       IN  server_id_t,
                                     direct_connect  IN  VARCHAR2,
                                     sysname         IN  sysname_t,
                                     nodename        IN  nodename_t,
                                     xdr_needed      IN  VARCHAR2,
                                     proxy_yn        IN  VARCHAR2,
                                     parent_sde_id   IN  sde_id_t)
  /***********************************************************************
  *
  *N  {insert_pinfo_for_proxy}  -- Insert pinfo entry for proxy session
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a server-supplied entry into the 
  *   SDE.PROCESS_INFORMATION table as an autonomous transaction.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id          <IN>  ==  (sde_id_t) The unique SDE session id for
  *                                this pinfo entry.
  *     server_id       <IN>  ==  (server_id_t) The pid (Unix) or process
  *                                id (Windows) of this connection's if this
  *                                iomgr-mediated connection, otherwise it
  *                                is the process id of the client if this
  *                                is a direct connection.
  *     direct_connect  <IN>  ==  (VARCHAR2) If 'Y' (true) then this pinfo
  *                                record is for a direct connection, and
  *                                should automatically be cleaned up.  If
  *                                'N' (false), then this pinfo record is
  *                                for an iomgr-mediated connection, and
  *                                should only be cleaned up by the iomgr.
  *     sysname         <IN>  ==  (sysname_t) The Client System  Name.
  *     nodename        <IN>  ==  (nodename_t) The Client Host Node Name.
  *     xdr_needed      <IN>  ==  (VARCHAR2) If 'Y' (true) then this pinfo
  *                                record is for xdr needed. If 'N' (false),
  *                                then this pinfo record is for an
  *                                iomgr-mediated connection.
  *    proxy_yn        <IN>  ==   (VARCHAR2) If 'Y' (true) then this pinfo
  *                                record is for a proxy session
  *    parent_sde_id  <IN>   ==   (sde_id_t) parent sde_id for proxy session
  *
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
  *    Sanjay Magal                01/04/10           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

     CURSOR session_audsid_cursor IS
       SELECT SYS_CONTEXT ('USERENV', 'SESSIONID') audsid
       FROM   DUAL;

     session_audsid  INTEGER;

   BEGIN
      
      -- Just in case there is a lingering entry, delete, but ignore
      -- SE_PROCESS_NOT_FOUND errors.

      BEGIN

         L_pinfo_user_can_modify (sde_id);
         DELETE FROM SDE.process_information
         WHERE  sde_id = insert_pinfo_for_proxy.sde_id;

      EXCEPTION

         WHEN OTHERS THEN
            IF SQLCODE <> sde_util.SE_PROCESS_NOT_FOUND THEN
               RAISE;
            END IF;

      END;

      -- Start by fetching the current auditing session id.

      OPEN session_audsid_cursor;
      FETCH session_audsid_cursor INTO session_audsid;
      IF session_audsid_cursor%NOTFOUND THEN
         raise_application_error (sde_util.SE_PROCESS_NOT_FOUND,
              'audit session id for sde_id ' || TO_CHAR (sde_id) ||
              ' not found.');
      END IF;
      CLOSE session_audsid_cursor;

      
      -- Perform the actual insert.
      
      INSERT INTO SDE.process_information
        (sde_id,server_id,audsid,start_time,rcount,wcount,opcount,numlocks,fb_partial,
         fb_count,fb_fcount,fb_kbytes,owner,direct_connect,
         sysname,nodename,xdr_needed,proxy_yn,parent_sde_id)
      VALUES
        (insert_pinfo_for_proxy.sde_id,insert_pinfo_for_proxy.server_id,
         session_audsid,SYSDATE,0,0,0,0,0,0,0,0,
         G_current_user,insert_pinfo_for_proxy.direct_connect,
         insert_pinfo_for_proxy.sysname,insert_pinfo_for_proxy.nodename,
         insert_pinfo_for_proxy.xdr_needed,insert_pinfo_for_proxy.proxy_yn,
         insert_pinfo_for_proxy.parent_sde_id);

      -- Got this far without an exception, it's safe to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END insert_pinfo_for_proxy;


   PROCEDURE update_pinfo (pinfo  IN  pinfo_record_t)
  /***********************************************************************
  *
  *N  {update_pinfo}  --  Update pinfo entry inf PROCESS_INFORMATION table
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates selected fields of an pinfo entry in the
  *   SDE.PROCESS_INFORMATION table as an autonomous transaction.  Only
  *   the statistics fields are updated, using the PID in the pinfo structure
  *   as the key.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     pinfo   <IN>  ==  (pinfo_record_t) The pinfo record containing the
  *                        identifying pid and the statistics fields to be
  *                        updated.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *     -20025                SE_NO_PERMISSIONS
  *     -20097                SE_PROCESS_NOT_FOUND
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 01/25/00           Original coding.
  *E
  ***********************************************************************/
   IS
     
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      
      -- Check permissions.

      L_pinfo_user_can_modify (pinfo.sde_id);
      
      -- Perform the update.
      
      UPDATE SDE.process_information
      SET    rcount     = pinfo.rcount,
             wcount     = pinfo.wcount,
             opcount    = pinfo.opcount,
             numlocks   = pinfo.numlocks,
             fb_partial = pinfo.fb_partial,
             fb_count   = pinfo.fb_count,
             fb_fcount  = pinfo.fb_fcount,
             fb_kbytes  = pinfo.fb_kbytes
      WHERE  sde_id = pinfo.sde_id;

      -- Got this far without an exception, it's safe to commit.

      COMMIT;

   EXCEPTION

      WHEN OTHERS THEN
         ROLLBACK;
         RAISE;

   END update_pinfo;


   FUNCTION get_sde_id RETURN sde_id_t
  /***********************************************************************
  *
  *N  {get_sde_id}  --  Return session's unique SDE connection id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the current session's unique SDE connection
  *   id.  The first call to this function generates it, and subsequent 
  *   calls return it from session global memory.  If a new id is generated,
  *   then a flag lock is placed for it to indicate it is in use.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (sde_id_t) The unique SDE connection id for this
  *                        session.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 02/29/00           Original coding.
  *E
  ***********************************************************************/
   IS

      CURSOR connection_id_cursor IS 
         SELECT SDE.connection_id_generator.nextval
         FROM   DUAL;

      lock_name    VARCHAR2(30);
      lock_handle  SDE.sde_util.lock_handle_t;
      lock_status  INTEGER;
     
   BEGIN

      -- If we don't already have an SDE connection ID for this session
      -- We must create one, and get a lock on it.

      IF G_sde_id IS NULL THEN
   
         -- Generate the connection ID from it's sequence.

         OPEN connection_id_cursor;
         FETCH connection_id_cursor INTO G_sde_id;
         CLOSE connection_id_cursor;

         -- Place a lock for the connection ID.

         lock_name := C_connection_lock_prefix || TO_CHAR (G_sde_id);
         DBMS_LOCK.ALLOCATE_UNIQUE (lock_name,lock_handle);
         lock_status := DBMS_LOCK.REQUEST (lock_handle,
                                           DBMS_LOCK.X_MODE,
                                           0,
                                           FALSE);
         IF lock_status <> 0 THEN
            raise_application_error (sde_util.SE_LOCK_CONFLICT,
                                     'Unable to obtain lock for SDE ' ||
                                     'Connection Id#' || TO_CHAR (G_sde_id) ||
                                     ', Error = ' || TO_CHAR (lock_status));
         END IF;
      END IF;

      RETURN G_sde_id;

   END get_sde_id;


   FUNCTION get_current_sde_id RETURN sde_id_t
  /***********************************************************************
  *
  *N  {get_sde_id}  --  Return session's current unique SDE connection id
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This function returns the current session's unique SDE connection
  *   id.  If it hasn't been generated for this session, NULL is returned.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN <OUT>  ==  (sde_id_t) The unique SDE connection id for this
  *                        session.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Peter Aronson                 03/02/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

     If G_sde_id IS NULL THEN
       G_sde_id := get_sde_id;
     End If;
     
     RETURN G_sde_id;

   END get_current_sde_id;


   PROCEDURE logoff (sde_id          IN  sde_id_t,
                     direct_connect  IN  VARCHAR2)
  /***********************************************************************
  *
  *N  {logoff}  --  Remove connections's records from lock + process tables
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure is used by SDE connection BEFORE LOGOFF triggers to
  *   delete any lock or process records for the disconnecting connection.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     sde_id          <IN>  ==  (sde_id_t) The identifying sde id for
  *                                locks and pinfo entry to be deleted.      
  *     direct_connect  <IN>  ==  (VARCHAR2) If 'Y', then this is a direct
  *                                connection, and the pinfo record should
  *                                be deleted.  If 'N', then this is an
  *                                iomgr-brokered connection, and the pinfo
  *                                record should be left for the iomgr to
  *                                deal with.
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
  *    Peter Aronson                 03/01/00           Original coding.
  *E
  ***********************************************************************/
   IS

   BEGIN

      -- Perform the deletes.  Do them directly instead of using the
      -- appropriate package procedures since those procedures use
      -- autonomous transactions, which, combined with logoff triggers,
      -- seem to give Oracle 8.1.5 indigestion.

      DELETE FROM SDE.layer_locks WHERE sde_id = logoff.sde_id;
      DELETE FROM SDE.state_locks WHERE sde_id = logoff.sde_id;
      DELETE FROM SDE.table_locks WHERE sde_id = logoff.sde_id;
      DELETE FROM SDE.object_locks WHERE sde_id = logoff.sde_id;
      UPDATE SDE.sde_logfile_pool SET sde_id = null 
      WHERE sde_id = logoff.sde_id;
     
      IF direct_connect = SDE.pinfo_util.C_is_direct_connection THEN
         DELETE FROM SDE.process_information WHERE sde_id = logoff.sde_id;
      END IF;
      COMMIT;
   END logoff;


   PROCEDURE cleanup (start_token  IN  VARCHAR2)
  /***********************************************************************
  *
  *N  {cleanup}  --  Remove any records from the lock and process tables
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure checks a special token against one written to 
  *   ArcSDE_StartUpConfirmPipe pipe, and if they match, deletes all rows
  *   from the lock and process tables.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     start_token  <IN>  ==  (VARCHAR2) The start token read from the
  *                             ArcSDE_StartUpPipe pipe.  This used to
  *                             confirm that this routine is being called
  *                             legitimately.
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
  *    Peter Aronson                 03/01/00           Original coding.
  *E
  ***********************************************************************/
   IS
   
      PRAGMA AUTONOMOUS_TRANSACTION;
   
      pipe_result   INTEGER;
      start_token2  VARCHAR2(64) DEFAULT 'Invalid!';
   
   BEGIN
   
      -- Make sure that this is the first time this is run, and make sure
      -- the start token is legitimate.
   
      IF start_token = start_token2 THEN
        start_token2 := 'Really?';
      END IF;
   
      pipe_result := DBMS_PIPE.RECEIVE_MESSAGE ('ArcSDE_StartUpConfirmPipe',0);
      IF pipe_result = 0 THEN
         DBMS_PIPE.UNPACK_MESSAGE (start_token2);
      END IF;
      IF start_token <> start_token2 THEN
         raise_application_error (sde_util.SE_NO_PERMISSIONS,
                                  'Invalid attempt to execute SDE.pinfo_util.'
                                  || 'cleanup.');
      END IF;
   
      -- Lock all of the tables to be cleaned; this prevents unfortunate
      -- accidents where some other connection tries to write to them as
      -- we're clearing them.
    
      LOCK TABLE SDE.layer_locks,
                 SDE.state_locks,
                 SDE.table_locks,
                 SDE.object_locks,
                 SDE.process_information IN EXCLUSIVE MODE;
   
      -- Delete all rows from these table.  We use DELETE instead of
      -- TRUNCATE as TRUNCATE would cause a commit, releasing our locks
      -- before we are ready.
   
      DELETE FROM SDE.layer_locks;
      DELETE FROM SDE.state_locks;
      DELETE FROM SDE.table_locks;
      DELETE FROM SDE.object_locks;
      DELETE FROM SDE.process_information;
      UPDATE SDE.sde_logfile_pool SET sde_id = null;
      
      COMMIT;
   
   EXCEPTION
   
      WHEN OTHERS THEN
        COMMIT;
        RAISE;
   
   END cleanup;


   PROCEDURE purge_unused (new_count  OUT  INTEGER)
  /***********************************************************************
  *
  *N  {purge_unused}  --  Remove unused records from lock and process tables
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure removes any abandoned (lacking an Oracle lock) 
  *   entries in the PROCESS_INFORMATION and lock tables.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     new_count  <OUT>  ==  (INTEGER) The number of entries left in the 
  *                            PROCESS_INFORMATION table after removing
  *                            dead entries.
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
  *    Peter Aronson                 02/28/06           Original coding.
  *E
  ***********************************************************************/
   IS
   
      PRAGMA AUTONOMOUS_TRANSACTION;

     CURSOR pinfo_entry_cursor IS 
       SELECT p.sde_id 
       FROM   SDE.process_information p;

     lock_name    VARCHAR(30); 
     lock_handle  VARCHAR(128); 
     lock_status  INTEGER; 

   BEGIN 

      new_count := 0;
      FOR pinfo_entry IN pinfo_entry_cursor LOOP 
        IF pinfo_entry.sde_id = get_current_sde_id THEN
          new_count := new_count + 1;
        ELSE
          lock_name := C_connection_lock_prefix || TO_CHAR (pinfo_entry.sde_id); 
          DBMS_LOCK.ALLOCATE_UNIQUE (lock_name,lock_handle); 
          lock_status := DBMS_LOCK.REQUEST (lock_handle, 
                                            DBMS_LOCK.X_MODE, 
                                            0, 
                                            TRUE); 
          IF lock_status = 0 OR lock_status = 4 THEN 
            logoff (pinfo_entry.sde_id,C_is_direct_connection); 
          ELSE
            new_count := new_count + 1;
          END IF; 
        END IF;
      END LOOP; 
      
      DELETE FROM SDE.layer_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);

      DELETE FROM SDE.state_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);
      
      DELETE FROM SDE.table_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);

      DELETE FROM SDE.object_locks
      WHERE  sde_id NOT IN (SELECT sde_id FROM SDE.process_information);
      
      COMMIT;
   EXCEPTION
   
      WHEN OTHERS THEN
        COMMIT;
        RAISE;
   
   END purge_unused;


BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   G_current_user := sde_util.sde_user;
   G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END pinfo_util;
