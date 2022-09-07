Prompt drop Package SVR_CONFIG_UTIL;
DROP PACKAGE SDE.SVR_CONFIG_UTIL
/

Prompt Package SVR_CONFIG_UTIL;
--
-- SVR_CONFIG_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.svr_config_util
/***********************************************************************
*
*N  {svr_config_util.sps}  --  Interface for server config DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on server config table. It should be compiled by the
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
*    Jose Kuruvilla         04/14/2002               Original coding.
*E
***********************************************************************/
IS

   -- Standard identifier for a server config record.

   DEF_prop_name           NVARCHAR2(32);
   DEF_char_prop_value     NVARCHAR2(512);
   DEF_num_prop_value      INTEGER;
   SUBTYPE prop_name_t        IS DEF_prop_name%TYPE;
   SUBTYPE char_prop_value_t  IS DEF_char_prop_value%TYPE;
   SUBTYPE num_prop_value_t   IS DEF_num_prop_value%TYPE;

  /* Constants. */

   -- The following constant defines the release of svr_config_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1001; 

  /* Procedures and functions. */

   -- The following functions perform DDL operations for server_config table. 
   -- objects stored in the SDE.SERVER_CONFIG table. 
   -- These procedures do not issue the COMMIT.

   PROCEDURE insert_server_config (in_prop_name       IN prop_name_t,
                                   in_char_prop_value IN char_prop_value_t,
                                   in_num_prop_value  IN num_prop_value_t);
   PROCEDURE truncate_server_config;

   PROCEDURE update_server_config (in_prop_name       IN prop_name_t,
                                   in_char_prop_value IN char_prop_value_t,
                                   in_num_prop_value  IN num_prop_value_t);
   PROCEDURE delete_server_config (in_prop_name       IN prop_name_t);
   PRAGMA RESTRICT_REFERENCES (svr_config_util,WNDS,WNPS); 

END svr_config_util;

/


Prompt Grants on PACKAGE SVR_CONFIG_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.SVR_CONFIG_UTIL TO PUBLIC WITH GRANT OPTION
/
