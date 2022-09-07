--------------------------------------------------------
--  DDL for Package ST_CREF_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "SDE"."ST_CREF_UTIL" AS
/******************************************************************************
   NAME:       st_spref_util
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        4/23/2005             1. Created this package.
******************************************************************************/

  C_package_release       CONSTANT PLS_INTEGER := 1001;
  C_package_guid          CONSTANT VARCHAR2 (32):= 'E869B7EE610C4AAAB8000628B7851FF8';

  SUBTYPE cref_record_t    IS SDE.st_coordinate_systems%ROWTYPE;
  SUBTYPE cref_id_t        IS SDE.st_coordinate_systems.id%TYPE;
  SUBTYPE cref_name_t      IS SDE.st_coordinate_systems.name%TYPE;
  SUBTYPE cref_def_t       IS SDE.st_coordinate_systems.definition%TYPE;

  PROCEDURE get_cref_id    (cref_name  IN cref_name_t,
                            cref_def   IN cref_def_t,
                            cref_id    IN OUT cref_id_t);
  
  PROCEDURE insert_cref    (cref_r  IN cref_record_t);

END st_cref_util;
