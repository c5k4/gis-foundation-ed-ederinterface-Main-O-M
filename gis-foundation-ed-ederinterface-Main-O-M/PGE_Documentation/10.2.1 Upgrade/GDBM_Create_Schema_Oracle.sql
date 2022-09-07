/* GDBM_Create_SDE_Schema_Oracle

	Run this script as the SDE user to
	create the tables required by the GDBM.

*/

-- GDBM No Reconcile Versions table.
-- this table DOES need to be registered with the geodatabase.
create table GDBM_NO_RECONCILE_VERSIONS (
  OBJECTID NUMBER(38),
  OWNER VARCHAR2(32) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  VALID_TO_POST NUMBER(1));

grant select,insert,update,delete on GDBM_NO_RECONCILE_VERSIONS to MM_ADMIN;

-- GDBM Process Log table.
-- this table DOES need to be registered with the geodatabase.
create table GDBM_PROCESS_LOG (
  OBJECTID NUMBER(38),  
  SERVICE VARCHAR2(128) NOT NULL,
  COMPUTER VARCHAR2(128) NOT NULL,
  VERSION VARCHAR2(128) NOT NULL,
  ACTION VARCHAR2(1),
  POST_ID NUMBER(9));

grant select,insert,update,delete on GDBM_PROCESS_LOG to MM_ADMIN;

-- GDBM Version Delete queue.
-- this table DOES need to be registered with the geodatabase.
create table GDBM_VERSION_DELETE_QUEUE (
	OBJECTID NUMBER(38),
	VERSION VARCHAR2(128) NOT NULL,
	LAST_DELETE_ATTEMPT DATE,
	MESSAGE VARCHAR2(256));
  
grant select,insert,update,delete on GDBM_VERSION_DELETE_QUEUE to MM_ADMIN;

-- GDBM Post Queue table
-- this table DOES need to be registered with the geodatabase.
create table GDBM_POST_QUEUE (
	OBJECTID NUMBER(38),
	CURRENTUSER VARCHAR2(32) NOT NULL,
	VERSION_OWNER VARCHAR2(32) NOT NULL,
	VERSION_NAME VARCHAR2(64) NOT NULL,
	DESCRIPTION VARCHAR2(256),
	SUBMIT_TIME DATE NOT NULL,
	PRIORITY NUMBER(3) NOT NULL,
	PX_NODE_ID NUMBER(38) NOT NULL,
	NODE_TYPE_NAME VARCHAR2(64) NOT NULL,
	NODE_TYPE_ID NUMBER(38) NOT NULL,
	NON_PX_POST_CODE VARCHAR2(64)
);

grant select,insert,update,delete on GDBM_POST_QUEUE to MM_ADMIN;
grant select,insert,update,delete on GDBM_POST_QUEUE to MM_USER;


-- GDBM Reconcile History table
-- this table DOES need to be registered with the geodatabase.
create table GDBM_RECONCILE_HISTORY (
	OBJECTID NUMBER(38),
	VERSION_NAME VARCHAR2(128),
	RECONCILE_START_DT DATE,
	RECONCILE_END_DT DATE,
	RECONCILE_RESULT VARCHAR2(32),
	SERVICE_NAME VARCHAR2(32)
);

grant select,insert,update,delete on GDBM_RECONCILE_HISTORY to MM_ADMIN;
grant select,insert,update,delete on GDBM_RECONCILE_HISTORY to MM_USER;


-- GDBM Post History Table
-- this table DOES need to be registered with the geodatabase.
create table GDBM_POST_HISTORY (
	OBJECTID NUMBER(38),
	VERSION_NAME VARCHAR2(128),
	POST_START_DT DATE,
	POST_END_DT DATE,
	POST_TYPE VARCHAR2(256),
	POST_RESULT VARCHAR2(32),
	STATE_NAME VARCHAR2(96),
	SERVICE_NAME VARCHAR2(32)
);

grant select,insert,update,delete on GDBM_POST_HISTORY to MM_ADMIN;
grant select,insert,update,delete on GDBM_POST_HISTORY to MM_USER;
