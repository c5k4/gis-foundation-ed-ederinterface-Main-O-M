Prompt drop Package Body ST_TYPE_UTIL;
DROP PACKAGE BODY SDE.ST_TYPE_UTIL
/

Prompt Package Body ST_TYPE_UTIL;
--
-- ST_TYPE_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_type_util 
/***********************************************************************
*
*N  {st_type_util.spb}  --  Implementation for globally useful functions
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements globally useful functions.  
*   It should be compiled by the ST_GEOEMTRY type DBA user; 
*   security is by user name.   
*E
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*X  Legalese:
*
*   COPYRIGHT 1992-2005 ESRI
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
*    Kevin Watt             04/22/05               Original coding.
*E
***********************************************************************/
IS

   G_current_user          NVARCHAR2(32);
   tmp_user                NVARCHAR2(32);
   
   /***********************************************************************
  *
  *n  {get_geom_index_rec}  --  Select spc_record_t from ST_GEOMETRY_INDEX
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x  exceptions:
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          04/20/05          original coding.
  *e
  ***********************************************************************/
  Function get_geom_index_rec    (owner          IN spx_owner_t,
                                  table_name     IN spx_table_t,
                                  column         IN spx_column_t,
                                  spx_rec        IN OUT spx_record_t)
  Return number IS
  
    Cursor c_spx_select (in_owner IN spx_owner_t,in_table IN spx_table_t,in_spcol IN spx_column_t) IS
           SELECT *
           FROM   SDE.st_geometry_index s
           WHERE  s.owner = in_owner AND s.table_name = in_table AND s.column_name = in_spcol;
  Begin
    
  	Open c_spx_select (owner,table_name,column);
    Fetch c_spx_select INTO spx_rec;
	  
    If c_spx_select%NOTFOUND THEN
      Close c_spx_select;
      return(SE_FAILURE);
    End If;
    
    return(SE_SUCCESS);  
    
  End get_geom_index_rec;
 

   FUNCTION type_user RETURN VARCHAR2
  /***********************************************************************
  *
  *N  {type_user}  --  Return the current user name, quoted if necessary
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     Return the current user's user name, quoted if it contains 
  *   that call for it characters.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     RETURN     <OUT>  ==  (VARCHAR2) The current user name.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  Type Exceptions:
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Kevin Watt            04/22/05                 Original coding.
  *E
  ***********************************************************************/
   IS
   BEGIN

      RETURN G_current_user;

   END type_user;


BEGIN
/***********************************************************************
 *
 *N  {Global Initialization}  --  Initialize Global state
 *
 ***********************************************************************/

   -- See if there are any characters in the user name that need to be quoted.

   tmp_user := TO_NCHAR(TRANSLATE (USER,
                          ' %&()*+,-./:;<=>?@[]\^`abcdefghijklmnopqrstuvwxyz{|}~',
                          '|!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!'));
   IF INSTR (tmp_user,TO_NCHAR('!')) = 0 THEN
      IF INSTR (TRANSLATE (SUBSTR (tmp_user,1,1),
                           N'123456789$#',
                           N'00000000000'),'0') = 0 THEN
         G_current_user := USER;
      END IF;
   END IF;

   IF G_current_user IS NULL THEN
      G_current_user := TO_NCHAR('"' || USER || '"');
   END IF;

END st_type_util;


/


Prompt Grants on PACKAGE ST_TYPE_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_TYPE_UTIL TO PUBLIC WITH GRANT OPTION
/
