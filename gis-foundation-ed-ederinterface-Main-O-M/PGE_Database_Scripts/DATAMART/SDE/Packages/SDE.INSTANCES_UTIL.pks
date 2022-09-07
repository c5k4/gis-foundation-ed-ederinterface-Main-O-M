Prompt drop Package INSTANCES_UTIL;
DROP PACKAGE SDE.INSTANCES_UTIL
/

Prompt Package INSTANCES_UTIL;
--
-- INSTANCES_UTIL  (Package) 
--
CREATE OR REPLACE PACKAGE SDE.instances_util
/***********************************************************************
*
*N  {instances_util.sps}  --  Interface for dbtune DDL package
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines procedures to perform
*   DDL operations on instances table. It should be compiled by the
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
*    Gautam Shanbhag         07/21/2004               Original coding.
*E ***********************************************************************/
IS

-- Standard identifier for a instances record.
SUBTYPE instanceid_t    IS   SDE.instances.instance_id%TYPE;
SUBTYPE instance_name_t IS	SDE.instances.instance_name%TYPE;
SUBTYPE status_t IS	SDE.instances.status%TYPE;
SUBTYPE layer_name_t IS	SDE.layers.table_name%TYPE;
SUBTYPE layer_owner_t IS  SDE.layers.owner%TYPE;
SUBTYPE table_name_t IS	SDE.table_registry.table_name%TYPE;
SUBTYPE table_owner_t IS  SDE.table_registry.owner%TYPE;

  /* Constants. */
-- The following constant defines the release of instance_util, and is
   -- used by the iomgr to determine if the most up to date version of the 
   -- package has been installed.

   C_package_release       CONSTANT PLS_INTEGER := 1003; 

  /* Procedures and functions. */

   -- The following functions perform DDL operations for dbtune table. 
-- objects stored in the <master>.instances table.
PROCEDURE insert_instances (in_instance_id        IN  instanceid_t,
					in_instance_name IN instance_name_t,
 					in_status	IN status_t);
PROCEDURE delete_instances (in_instance_id		IN instanceid_t);
PROCEDURE check_instance_table_conflicts (in_table_name IN  table_name_t,in_table_owner IN  table_owner_t,current_schema_name IN instance_name_t);
PRAGMA RESTRICT_REFERENCES (instances_util,WNDS,WNPS);

END instances_util;

/


Prompt Grants on PACKAGE INSTANCES_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.INSTANCES_UTIL TO PUBLIC WITH GRANT OPTION
/
