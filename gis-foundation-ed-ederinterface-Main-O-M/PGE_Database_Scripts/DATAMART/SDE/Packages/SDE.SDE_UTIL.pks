Prompt drop Package SDE_UTIL;
DROP PACKAGE SDE.SDE_UTIL
/

Prompt Package SDE_UTIL;
--
-- SDE_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.sde_util
/***********************************************************************
*
*N  {sde_util.sps}  --  SDE PL/SQL utility package
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines useful constants and type
*   definitions for use by other SDE packages.  Now contains globally
*   useful functions, too.
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
*    Peter Aronson             12/09/98               Original coding.
*E
***********************************************************************/
IS

  /* Type definitions. */
  
   -- Standard identifier for an sde_tables_modified table. 
   
   SUBTYPE tlm_table_name_t IS SDE.sde_tables_modified.table_name%TYPE;
   SUBTYPE tlm_time_last_modified_t IS SDE.sde_tables_modified.time_last_modified%TYPE;
   SUBTYPE tlm_record_t IS SDE.sde_tables_modified%ROWTYPE;  
   
  /* Constants. */

   -- The following constants are used as return values from SQL-callable
   -- FUNCTIONs of type NUMBER, since Oracle doesn't allow FUNCTIONs of
   -- type BOOLEAN.

   C_false            CONSTANT NUMBER  := 0;
   C_true             CONSTANT NUMBER  := 1;

   -- The following constant is the name of the SDE DBA user.  This is kind
   -- redundent, since you need to know this in order to access this package!

   C_sde_dba          CONSTANT NVARCHAR2(32) := UPPER(N'SDE');
   C_sde_master       CONSTANT NVARCHAR2(32) := N'SDE';
   
   -- The following constant defines the number of rows in a PL/SQL table
   -- that after deleting, call for a call to the procedure
   -- DBMS_SESSION.FREE_UNUSED_USER_MEMORY.

   C_free_threshold   CONSTANT NUMBER := 2048;

   -- The following constant defines the release of sde_util, and is used 
   -- by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release  CONSTANT PLS_INTEGER := 1028;

   -- Standard SDE Error codes, -20000 to conform to Oracle convention
   -- that user-raised exceptions be in the range of -20999 to -20000.
   -- These codes must match their equivalent codes in sdeerno.h.

   SE_SUCCESS                 CONSTANT NUMBER := 0;

   SE_LAYER_EXISTS            CONSTANT NUMBER := -20019;
   SE_LAYER_NOEXIST           CONSTANT NUMBER := -20020;
   SE_NO_PERMISSIONS          CONSTANT NUMBER := -20025;
   SE_TABLE_NOEXIST           CONSTANT NUMBER := -20037;
   SE_NO_LOCKS                CONSTANT NUMBER := -20048;
   SE_LOCK_CONFLICT           CONSTANT NUMBER := -20049;
   SE_INVALID_PARAM_VALUE     CONSTANT NUMBER := -20066;
   SE_NOT_TABLE_OWNER         CONSTANT NUMBER := -20096;
   SE_PROCESS_NOT_FOUND       CONSTANT NUMBER := -20097;
   SE_VERSION_NOEXIST         CONSTANT NUMBER := -20126;
   SE_INVALID_SPATIAL_COLUMN  CONSTANT NUMBER := -20129;
   SE_NO_SDE_ROWID_COLUMN     CONSTANT NUMBER := -20169;
   SE_INVALID_VERSION_NAME    CONSTANT NUMBER := -20171;
   SE_STATE_NOEXIST           CONSTANT NUMBER := -20172;
   SE_VERSION_HAS_MOVED       CONSTANT NUMBER := -20174;
   SE_STATE_HAS_CHILDREN      CONSTANT NUMBER := -20175;
   SE_PARENT_NOT_CLOSED       CONSTANT NUMBER := -20176;
   SE_VERSION_EXIST           CONSTANT NUMBER := -20177;
   SE_STATE_USED_BY_VERSION   CONSTANT NUMBER := -20179;
   SE_TOO_MANY_STATES         CONSTANT NUMBER := -20184;
   SE_RASTERCOLUMN_EXISTS     CONSTANT NUMBER := -20194;
   SE_TABLE_REGISTERED        CONSTANT NUMBER := -20218;
   SE_TABLE_NOREGISTERED      CONSTANT NUMBER := -20220;
   SE_RASTERCOLUMN_NOEXIST    CONSTANT NUMBER := -20226;
   SE_SPATIALREF_EXISTS       CONSTANT NUMBER := -20254;
   SE_SPATIALREF_NOEXIST      CONSTANT NUMBER := -20255;
   SE_GEOMETRYCOL_NOEXIST     CONSTANT NUMBER := -20266;
   SE_METADATA_RECORD_NOEXIST CONSTANT NUMBER := -20267;
   SE_LOCATOR_NOEXIST         CONSTANT NUMBER := -20275;
   SE_LOCATOR_EXISTS          CONSTANT NUMBER := -20276;
   SE_VERSION_HAS_CHILDREN    CONSTANT NUMBER := -20285;
   SE_INVALID_VERSION_ID      CONSTANT NUMBER := -20298;
   SE_XML_INDEX_EXISTS        CONSTANT NUMBER := -20333;
   SE_XML_INDEX_NOEXIST       CONSTANT NUMBER := -20334;
   SE_XML_TAG_NOEXIST         CONSTANT NUMBER := -20339;
   SE_XML_TAG_EXISTS          CONSTANT NUMBER := -20359;
   SE_LAYER_OUTSIDE_SCHEMA    CONSTANT NUMBER := -20419;
   SE_TABLE_OUTSIDE_SCHEMA    CONSTANT NUMBER := -20420;
   SE_MVV_ROWID_UPDATE        CONSTANT NUMBER := -20499;
   SE_MVV_EDIT_DEFAULT        CONSTANT NUMBER := -20500;
   SE_MVV_IN_EDIT_MODE        CONSTANT NUMBER := -20501;
   SE_MVV_VERSION_NOT_DEFAULT CONSTANT NUMBER := -20502;
   SE_MVV_NAMEVER_NOT_CURRVER CONSTANT NUMBER := -20503;
   SE_MVV_NOT_STD_EDIT_MODE   CONSTANT NUMBER := -20504;
   SE_MVV_SET_DEFAULT         CONSTANT NUMBER := -20507;
   SE_MVV_VERSION_IN_USE      CONSTANT NUMBER := -20553;
   
   SE_DOMAINX_INVALID_TYPE    CONSTANT NUMBER := -20600;
   SE_DOMAINX_DELETE_ERR      CONSTANT NUMBER := -20601;
   SE_DOMAINX_INSERT_ERR      CONSTANT NUMBER := -20602;
   SE_DOMAINX_LAYER_NOT_FOUND CONSTANT NUMBER := -20603;

  /* Procedures and functions. */

   -- Returns current user, formed for SDE use.

   FUNCTION sde_user RETURN NVARCHAR2;

   -- Functions to update time last modified for our DMBS objects.

   PROCEDURE set_table_last_modified(tlm IN tlm_record_t);

   PRAGMA RESTRICT_REFERENCES (sde_util,WNDS,WNPS);
   PRAGMA RESTRICT_REFERENCES (sde_user,WNDS,WNPS);

END sde_util;

/


Prompt Grants on PACKAGE SDE_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SDE_UTIL TO PUBLIC WITH GRANT OPTION
/
