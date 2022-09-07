--------------------------------------------------------
--  DDL for Package Body ST_TYPE_USER
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "SDE"."ST_TYPE_USER" 
/***********************************************************************
*
*N  {st_type_user.spb}  --  Implementation for globally useful functions
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

   G_current_user          SDE.st_type_user.spat_owner_t;
   tmp_user                SDE.st_type_user.spat_owner_t;

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

END st_type_user;
