Prompt drop Package Body ST_DOMAIN_OPERATORS;
DROP PACKAGE BODY SDE.ST_DOMAIN_OPERATORS
/

Prompt Package Body ST_DOMAIN_OPERATORS;
--
-- ST_DOMAIN_OPERATORS  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_domain_operators
/***********************************************************************
*
*N  {st_domain_operators.sps}  --  ST_GEOM domain operators.  
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package specification defines operators 
*    to support the ST_GEOMETRY type and domain index.
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
*    Kevin Watt          12/02/04               Original coding.
*E
***********************************************************************/
IS

--ST_EnvIntersects by envelope
Function st_envintersects_env_f(prim SDE.st_geometry,
                           minx number,miny number,maxx number,maxy number)
Return number 
IS
  Begin

     If prim.minx > maxx OR 
       prim.miny > maxy OR
       prim.maxx < minx OR
       prim.maxy < miny THEN
       Return 0;
    ELSE
	   Return 1;
    End If;
  End st_envintersects_env_f;

--ST_EnvIntersects by Shape
Function st_envintersects_shape_f(prim SDE.st_geometry,shape SDE.st_geometry)
Return number 
IS
  Begin

    If prim.minx > shape.maxx OR 
       prim.miny > shape.maxy OR
       prim.maxx < shape.minx OR
       prim.maxy < shape.miny THEN
       Return 0;
    ELSE
	   Return 1;
    End If;
  End st_envintersects_shape_f;


--ST_EnvIntersects by envelope in spatial index order
Function st_envintersects_env_ord_f(prim SDE.st_geometry,
                           minx number,miny number,maxx number,maxy number,
                           orderby varchar2)
Return number 
IS
  Begin

     If prim.minx > maxx OR 
       prim.miny > maxy OR
       prim.maxx < minx OR
       prim.maxy < miny THEN
       Return 0;
    ELSE
	   Return 1;
    End If;
  End st_envintersects_env_ord_f;

--ST_EnvIntersects by Shape in spatial index order
Function st_envintersects_shape_ord_f(prim SDE.st_geometry,
                                      shape SDE.st_geometry,
                                      orderby varchar2)
Return number 
IS
  Begin

    If prim.minx > shape.maxx OR 
       prim.miny > shape.maxy OR
       prim.maxx < shape.minx OR
       prim.maxy < shape.miny THEN
       Return 0;
    ELSE
	   Return 1;
    End If;
  End st_envintersects_shape_ord_f; 

End st_domain_operators;

/


Prompt Grants on PACKAGE ST_DOMAIN_OPERATORS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_DOMAIN_OPERATORS TO PUBLIC WITH GRANT OPTION
/
