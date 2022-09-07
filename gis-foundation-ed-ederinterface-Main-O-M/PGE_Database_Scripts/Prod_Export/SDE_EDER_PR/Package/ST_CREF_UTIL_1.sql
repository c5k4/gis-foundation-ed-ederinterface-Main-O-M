--------------------------------------------------------
--  DDL for Package Body ST_CREF_UTIL
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."ST_CREF_UTIL" AS

PROCEDURE get_cref_id    (cref_name  IN cref_name_t,
                          cref_def   IN cref_def_t,
                          cref_id    IN OUT cref_id_t)
/***********************************************************************
*
*N  {get_cref}  --  Get ST_COORDINATE_SYSTEMS row 
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This procedure gets the current ST_COORDINATE_SYSTEMS row based
*  on ID. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*     id           <IN>      ==  (NUMBER) cref Id 
*     cref_r       <IN OUT>  ==  (cref_record_t) 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Exceptions:
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Kevin Watt          12/02/04           Original coding.
*E
***********************************************************************/       
  IS
  
    CURSOR C_get_cref_cursor (name_in IN cref_name_t,def_in IN cref_def_t) IS
           SELECT id
           FROM  SDE.st_coordinate_systems
           WHERE  name = name_in AND definition = def_in;
  BEGIN
  
    OPEN C_get_cref_cursor (cref_name,cref_def);
    FETCH C_get_cref_cursor INTO cref_id;
    IF C_get_cref_cursor%NOTFOUND THEN
   CLOSE C_get_cref_cursor;
      raise_application_error (SDE.st_type_util.ST_CREF_NOEXIST,
                                'Coordinate Reference Name  '|| cref_name||' and << '||
        cref_def||' >>'||
                                ' does not exist in ST_COORDINATE_SYSTEMS table.');
    END IF;
    CLOSE C_get_cref_cursor;
  
  END get_cref_id;

PROCEDURE insert_cref (cref_r  IN cref_record_t)
/***********************************************************************
*
*N  {insert_cref}  --  Insert CREF row
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This procedure inserts a CREF row. 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*A  Parameters:
*     id           <IN>      ==  (NUMBER) cref Id 
*     cref_r       <IN OUT>  ==  (cref_record_t) 
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  SDE Exceptions:
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*H  History:
*
*    Kevin Watt          12/02/04           Original coding.
*E
***********************************************************************/ 
  IS
  
  BEGIN
  
    INSERT INTO SDE.st_coordinate_systems
   (id,name,organization,definition,description)
 VALUES
   (cref_r.id,cref_r.name,cref_r.organization,
    cref_r.definition,cref_r.description);
    
  END insert_cref;

END st_cref_util;
