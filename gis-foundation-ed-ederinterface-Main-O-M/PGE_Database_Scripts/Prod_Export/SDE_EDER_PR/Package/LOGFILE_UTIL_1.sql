--------------------------------------------------------
--  DDL for Package Body LOGFILE_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."LOGFILE_UTIL" 
/***********************************************************************
*
*N  {logfile_util.spb}  --  Implementation for logfile DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on logfiles.  It should be compiled by the
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
*    Sanjay Magal             06/09/03               Original coding.
*E
***********************************************************************/
IS

   /* Local Subprograms. */

PROCEDURE logfile_pool_get_id (in_sde_id      IN  sde_id_t,
                               check_orphans  IN  PLS_INTEGER,
                               pool_id        OUT table_id_t)
/***********************************************************************
*
*N  {logfile_pool_get_id}  --  Get available logfile pool table 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure checks if there's an available logfile table in the 
*   sde owned pool. If so, it takes it, and returns its id.
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*     in_sde_id        <IN>  ==  (sde_id_t) Sde_id for which to get a 
*                                        logpool table.
*     check_orphans    <IN>  ==  (PLS_INTEGER)  Check orphaned processes?
*     pool_id          <OUT> ==  (table_id_t) logpool table id 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                06/09/03         Original coding.
*E
***********************************************************************/
IS
 
    PRAGMA AUTONOMOUS_TRANSACTION;

      V_tab_id           table_id_t := 0;
      V_logpool_tab_name VARCHAR2(256);
      check_yn           BOOLEAN;
      logpool_count      NUMBER := 0;

   BEGIN

      IF (check_orphans = C_check_orphans) THEN
        check_yn := TRUE;
   ELSIF (check_orphans = C_nocheck_orphans) THEN 
     check_yn := FALSE;
   END IF;
       

     LOCK TABLE SDE.sde_logfile_pool IN EXCLUSIVE MODE;

     SELECT count(*) into logpool_count FROM SDE.sde_logfile_pool 
     WHERE sde_id IS NULL; 
                  
     IF (logpool_count = 0 AND check_yn) THEN
       SELECT LP.table_id into V_tab_id
       FROM SDE.sde_logfile_pool LP 
       WHERE LP.sde_id NOT IN (SELECT sde_id FROM SDE.process_information)
       AND ROWNUM = 1;
     END IF;

     IF (logpool_count > 0) THEN
       SELECT table_id into V_tab_id FROM SDE.sde_logfile_pool 
       WHERE sde_id IS NULL 
       AND ROWNUM = 1; 
     END IF;
     
     IF (V_tab_id > 0) THEN          
       pool_id := V_tab_id;
    
    UPDATE SDE.sde_logfile_pool
    SET sde_id = in_sde_id 
           WHERE table_id = V_tab_id;
         
          --Truncate logpool table in case previous user
          --did not clean up
          V_logpool_tab_name := 'SDE.sde_logpool_' || V_tab_id;
    
   EXECUTE IMMEDIATE
            'TRUNCATE TABLE '|| V_logpool_tab_name || ' REUSE STORAGE' || '';
     ELSE
      pool_id := 0;
      ROLLBACK;
     END IF;


   EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

END logfile_pool_get_id;

   
      
PROCEDURE logfile_pool_rel_id (pool_id   IN table_id_t)
/***********************************************************************
*
*N  {logfile_pool_rel_id}  --  Release logfile pool table
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure releases the logpool table in the 
*   sde owned pool of the current process. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*
*     pool_id      <IN> ==  (table_id_t) logpool table id 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                06/09/03         Original coding.
*E
***********************************************************************/
IS     
     
   PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

     UPDATE SDE.sde_logfile_pool SET sde_id = NULL 
     WHERE table_id = pool_id;
    
     COMMIT;

    EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

END logfile_pool_rel_id;


PROCEDURE logpool_tab_trunc (in_tab_name IN NVARCHAR2)
/***********************************************************************
*
*N  {logpool_tab_trunc}  -- Truncate logpool table
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure truncates the given logpool table 
*   Required to be done in an SDE owned stored procedure because
*   the logfile_data table could be an SDE.sde_logpool_N
*   To truncate a table in another user's schema, the user requires
*   DROP ANY TABLE privilege. Hence, move this to a stored procedure  
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*
*     in_tab_name      <IN> ==  (NVARCHAR2) logfile_data_table name 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                06/09/03         Original coding.
*E
***********************************************************************/
IS     
   
  BEGIN

    EXECUTE IMMEDIATE
      'TRUNCATE TABLE '|| TO_CHAR(in_tab_name) || ' REUSE STORAGE' || '';   
   
   
END logpool_tab_trunc;


PROCEDURE logdata_tab_trunc (in_tab_name IN NVARCHAR2)
/***********************************************************************
*
*N  {logdata_tab_trunc}  -- Truncate logfile_data table
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure truncates the given logfile_data table 
*   Required to be done in an SDE owned stored procedure.
*   To truncate a table in another user's schema, the user requires
*   DROP ANY TABLE privilege. Hence, move this to a stored procedure  
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*
*     in_tab_name      <IN> ==  (NVARCHAR2) logfile_data_table name 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Josefina Santiago          01/15/16         Original coding.
*E
***********************************************************************/
IS     
   
  BEGIN

    EXECUTE IMMEDIATE
      'TRUNCATE TABLE '|| TO_CHAR(in_tab_name) || '';   
   
   
END logdata_tab_trunc;

PROCEDURE drop_lf_data_table (in_table_name IN  NVARCHAR2)
/***********************************************************************
*
*N  {drop_logfile_table}  -- Drop session/standalone logfile tables
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure drops temporary logfile tables  
*   for session/standalone logfiles. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*
*     in_table_name       <IN> ==  (NVARCHAR2) table name  
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                10/24/08         Original coding.
*E
***********************************************************************/
IS     
     
   BEGIN

    EXECUTE IMMEDIATE
      'DROP TABLE '|| TO_CHAR(in_table_name) || '';   
    
    EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

END drop_lf_data_table;

                             
PROCEDURE delete_lf_data_table (in_table_name IN  NVARCHAR2,
                                in_lf_data_id IN  sde_id_t)
/***********************************************************************
*
*N  {delete_lf_data_table}  --  Delete from logfile data table
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure deletes from the logfile data table  
*   for the given logfile id. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
* 
*     in_table_name <IN> == (NVARCHAR2) 8.x logfile data table name
*     in_lf_data_id <IN> == (sde_id_t) logfile id to delete 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                10/24/08         Original coding.
*E
***********************************************************************/
IS     
     
   PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

       EXECUTE IMMEDIATE
       'DELETE FROM '|| TO_CHAR(in_table_name) || 
       ' WHERE LOGFILE_DATA_ID = :data_id' USING in_lf_data_id;   
        
     COMMIT;

    EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

END delete_lf_data_table;


PROCEDURE delete_logfiles_table (in_table_name IN  NVARCHAR2,
                                 in_lf_id IN  sde_id_t)                                  
/***********************************************************************
*
*N  {delete_logfiles_table}  --  Delete from logfile table
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure deletes entry from the logfiles table  
*   for the given logfile id. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
* 
*     in_table_name <IN> == (NVARCHAR2) logfiles table name
*     in_lf_id <IN> == (sde_id_t) logfile id to delete 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                10/24/08         Original coding.
*E
***********************************************************************/
IS     
     
   PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN

       EXECUTE IMMEDIATE
       'DELETE FROM '|| TO_CHAR(in_table_name) || 
       ' WHERE LOGFILE_ID = :log_id' USING in_lf_id;   
        
     COMMIT;

    EXCEPTION

      WHEN OTHERS THEN
        ROLLBACK;
        RAISE;

END delete_logfiles_table;

 

PROCEDURE purge_tmp_logs (in_sde_id IN  sde_id_t,
                          in_owner  IN  NVARCHAR2)
/***********************************************************************
*
*N  {purge_tmp_logs}  --  Release temp logfiles for session
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*         This procedure releases temporary logfiles in the 
*   sde owned pool of the current process. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*
*     in_sde_id      <IN> ==  (sde_id_t) sde (session) id
*     in_owner       <IN> ==  (NVARCHAR2) owner name  
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*     -20025                SE_NO_PERMISSIONS
*
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Sanjay Magal                10/24/08         Original coding.
*E
***********************************************************************/
IS     

    c1                  INTEGER; 
    c1_status           INTEGER;
    owner_loop_done     BOOLEAN := FALSE;
    log_id              INTEGER;
    log_data_table      VARCHAR2(256);
    log_table           VARCHAR2(256);
    delimiter           VARCHAR2(1) := '.';
    
 BEGIN
 
     IF NOT dbms_sql.is_open(c1) THEN
       c1 := dbms_sql.open_cursor;
     END IF;
     
     dbms_sql.parse(c1,
      'SELECT logfile_id,TO_CHAR(logfile_data_table) ' ||
      'FROM ' || TO_CHAR(in_owner) ||'.sde_logfiles ' ||
      'WHERE session_tag = :in_session_tag', dbms_sql.native);
          
     dbms_sql.bind_variable(c1, ':in_session_tag', in_sde_id);
     dbms_sql.define_column(c1, 1, log_id);    
     dbms_sql.define_column(c1, 2, log_data_table, 256);
     c1_status := dbms_sql.execute(c1);       

     LOOP 
      IF dbms_sql.fetch_rows(c1) = 0 THEN 
         owner_loop_done := TRUE;
      ELSE
       dbms_sql.column_value(c1, 1, log_id);
       dbms_sql.column_value(c1, 2, log_data_table);

       log_table := TO_CHAR(in_owner)||'.'||
                    substr(log_data_table,instr(log_data_table,delimiter)+1);
                    
         IF (upper(log_table) LIKE '%SDE_SESSION_%') OR
            (upper(log_table) LIKE '%SDE_LOGDATA_%') THEN
           SDE.logfile_util.drop_lf_data_table (log_table);
         ELSE   
           SDE.logfile_util.delete_lf_data_table (log_table,log_id);
         END IF;
         
         log_table := TO_CHAR(in_owner) ||'.sde_logfiles';
         SDE.logfile_util.delete_logfiles_table (log_table,log_id);
              
      END IF;
      
        EXIT WHEN owner_loop_done;
     END LOOP;
     
     dbms_sql.close_cursor(c1);
       
    EXCEPTION 
      WHEN OTHERS THEN
       RAISE;

END purge_tmp_logs;



END logfile_util;
