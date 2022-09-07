Prompt drop Package KEYSET_UTIL;
DROP PACKAGE SDE.KEYSET_UTIL
/

Prompt Package KEYSET_UTIL;
--
-- KEYSET_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.keyset_util
/***********************************************************************
*
*N  {keyset_util.sps}  --  Interface for keyset table package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on keyset table. It should be compiled by the
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
*    Gautam Shanbhag		03/04/05               Original coding.
*E
***********************************************************************/
IS

  /* Constants. */

   -- The following constant defines the release of keyset_util, and is 
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1010; 

  /* Procedures and functions. */

   -- The following functions perform DDL operations for keyset table. 
   PROCEDURE create_keyset_table (tname       IN NVARCHAR2,
                				  current_user IN NVARCHAR2, 
                				  dbtune_str_param IN NCLOB);
   PROCEDURE delete_keyset (tname       IN NVARCHAR2,
				    keyset_id        IN INTEGER);
   PROCEDURE remove_keyset_table (tname       IN NVARCHAR2);
   PRAGMA RESTRICT_REFERENCES (keyset_util,WNDS,WNPS); 

END keyset_util;

 
/


Prompt Grants on PACKAGE KEYSET_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.KEYSET_UTIL TO PUBLIC WITH GRANT OPTION
/
