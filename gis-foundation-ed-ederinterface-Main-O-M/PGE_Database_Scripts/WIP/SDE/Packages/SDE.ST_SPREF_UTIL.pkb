Prompt drop Package Body ST_SPREF_UTIL;
DROP PACKAGE BODY SDE.ST_SPREF_UTIL
/

Prompt Package Body ST_SPREF_UTIL;
--
-- ST_SPREF_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.st_spref_util AS
/******************************************************************************
   NAME:       st_spref_util
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        4/23/2005             1. Created this package body.
******************************************************************************/

  Cursor g_get_srid_cursor (sr_name_in IN sr_name_t) IS
     SELECT srid
     FROM  SDE.st_spatial_references
     WHERE  sr_name = sr_name_in;

  Function get_srid(sr_name IN sr_name_t) Return number 
  IS
    srid     srid_t;
  Begin
  
    Open g_get_srid_cursor (sr_name);
    Fetch g_get_srid_cursor INTO srid;
    If g_get_srid_cursor%NOTFOUND THEN
       raise_application_error (SDE.st_type_util.st_spref_noexist,
                                'Spatial Reference name '|| sr_name||
                                  ' not found.');
    End If;
    Close g_get_srid_cursor;
   
    Return srid;
  End;
  
Procedure insert_spref (spref IN spatial_ref_record_t,
                        srid  IN Out srid_t)
  IS
    srid_out   srid_t;
  Begin
    INSERT INTO SDE.st_spatial_references 
	  (sr_name,x_offset,y_offset,xyunits,z_offset,z_scale,m_offset,m_scale,
	   min_x,max_x,min_y,max_y,min_z,max_z,min_m,max_m,cs_id,cs_name,cs_type,
	   organization,org_coordsys_id,Definition,description)
	VALUES
	  (spref.sr_name,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
	   spref.z_scale,spref.m_offset,spref.m_scale,spref.min_x,spref.max_x,
	   spref.min_y,spref.max_y,spref.min_z,spref.max_z,spref.min_m,spref.max_m,
	   spref.cs_id,spref.cs_name,spref.cs_type,spref.organization,spref.org_coordsys_id,
	   spref.Definition,spref.description) returning srid INTO srid_out;
	   
    srid := srid_out;   
  End insert_spref;

Procedure insert_spref (spref IN spatial_ref_record_t)
  IS
  Begin
    INSERT INTO SDE.st_spatial_references 
	  (srid,sr_name,x_offset,y_offset,xyunits,z_offset,z_scale,m_offset,m_scale,
	   min_x,max_x,min_y,max_y,min_z,max_z,min_m,max_m,cs_id,cs_name,cs_type,
	   organization,org_coordsys_id,definition,description)
	VALUES
	  (spref.srid,spref.sr_name,spref.x_offset,spref.y_offset,spref.xyunits,spref.z_offset,
	   spref.z_scale,spref.m_offset,spref.m_scale,spref.min_x,spref.max_x,
	   spref.min_y,spref.max_y,spref.min_z,spref.max_z,spref.min_m,spref.max_m,
	   spref.cs_id,spref.cs_name,spref.cs_type,spref.organization,spref.org_coordsys_id,
	   spref.definition,spref.description);
	   
  End insert_spref;

  Function select_spref (spref_r IN Out spatial_ref_record_t)
/***********************************************************************
  *
  *n  {select_Spref}  --  select from st_Spatial_References table. 
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *              select from st_Spatial_References tables. 
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
  Return number 
  IS
    srid_local      srid_t;
  
    Cursor c_sel_spref (srid_in IN srid_t) IS
	 SELECT sr_name,x_offset,y_offset,xyunits,z_offset,
              z_scale,m_offset,m_scale,min_x,max_x,min_y,max_y,
              min_z,max_z,min_m,max_m,cs_name,cs_type,organization,
              org_coordsys_id,Definition,description,cs_id
         FROM SDE.st_spatial_references
	  WHERE srid = srid_in;
  
  Begin

    srid_local := spref_r.srid;

    spref_r := check_spref_cache(spref_r.srid);

    If spref_r.srid = srid_local THEN
      Return(se_success);
    End If;

    spref_r.srid := srid_local;
    
    Open c_sel_spref (spref_r.srid);
    Fetch c_sel_spref INTO spref_r.sr_name,spref_r.x_offset,spref_r.y_offset,
	                       spref_r.xyunits,spref_r.z_offset,spref_r.z_scale,
						   spref_r.m_offset,spref_r.m_scale,spref_r.min_x,
						   spref_r.max_x,spref_r.min_y,spref_r.max_y,spref_r.min_z,
						   spref_r.max_z,spref_r.min_m,spref_r.max_m,spref_r.cs_name,
						   spref_r.cs_type,spref_r.organization,spref_r.org_coordsys_id,
						   spref_r.Definition,spref_r.description,spref_r.cs_id;
    If c_sel_spref%NOTFOUND THEN
	  Close c_sel_spref;
      Return SDE.st_type_util.spx_no_srid;
	End If;

	Close c_sel_spref;

    add_spref_cache_info  (spref_r);

	Return(se_success);
  
  End select_spref;

 Function check_spref_cache  (srid       IN srid_t)
  /***********************************************************************
  *
  *n  {check_Cache}  --  checks the cursor_Cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this function checks the cache for a layer match.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     spx_Record_T <in out> ==  (layer_Record_T) spatial index record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
  Return spatial_ref_record_t
  IS
    sp_ref    spatial_ref_record_t;
  Begin

    sp_ref.srid := -1;
    If nsp_refs > 0 THEN
	  FOR id IN 1 .. nsp_refs
      Loop
        If spref_cache(id).srid = srid THEN 
          sp_ref.srid                := srid;
	      sp_ref.sr_name             := spref_cache(id).sr_name;
	      sp_ref.x_offset            := spref_cache(id).x_offset;
	      sp_ref.y_offset            := spref_cache(id).y_offset;
	      sp_ref.xyunits             := spref_cache(id).xyunits;
	      sp_ref.z_offset            := spref_cache(id).z_offset;
	      sp_ref.z_scale             := spref_cache(id).zscale;
	      sp_ref.m_offset            := spref_cache(id).m_offset;
	      sp_ref.m_scale             := spref_cache(id).mscale;
	      sp_ref.min_x               := spref_cache(id).min_x;
	      sp_ref.max_x               := spref_cache(id).max_x;
	      sp_ref.min_y               := spref_cache(id).min_y;
	      sp_ref.max_y               := spref_cache(id).max_y;
	      sp_ref.min_z               := spref_cache(id).min_z;
	      sp_ref.max_z               := spref_cache(id).max_z;
	      sp_ref.min_m               := spref_cache(id).min_m;
	      sp_ref.max_m               := spref_cache(id).max_m;
	      sp_ref.cs_id               := spref_cache(id).csid;
	      sp_ref.cs_name             := spref_cache(id).cs_name;
	      sp_ref.cs_type             := spref_cache(id).cs_type;
	      sp_ref.organization        := spref_cache(id).organization;
	      sp_ref.org_coordsys_id     := spref_cache(id).org_coordsys_id;
	      sp_ref.Definition          := spref_cache(id).Definition;
	      sp_ref.description         := spref_cache(id).description;
		  
          Return(sp_ref);
        End If;
      End Loop;
    End If;
	
	Return(sp_ref);
	
  End check_spref_cache;

  Procedure add_spref_cache_info  (spref_r       IN spatial_ref_record_t)
  /***********************************************************************
  *
  *n  {add_Cache_Info}  --  adds layer/sp_Ref info to cache
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *p  purpose:
  *     this procedure adds the spatial_Ref info to the cache.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *a  parameters:
  *
  *     layer_R <in out> ==  (layer_Record_T) layer record.
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *x   exceptions:
  *
  *e
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *h  history:
  *
  *    kevin watt          12/02/04           original coding.
  *e
  ***********************************************************************/
  IS
  
  Begin
  					 
    nsp_refs := nsp_refs + 1;
    spref_cache(nsp_refs).srid               := spref_r.srid;
	spref_cache(nsp_refs).sr_name            := spref_r.sr_name;
	spref_cache(nsp_refs).x_offset           := spref_r.x_offset;
	spref_cache(nsp_refs).y_offset           := spref_r.y_offset;
	spref_cache(nsp_refs).xyunits            := spref_r.xyunits;
	spref_cache(nsp_refs).z_offset           := spref_r.z_offset;
	spref_cache(nsp_refs).zscale             := spref_r.z_scale;
	spref_cache(nsp_refs).m_offset           := spref_r.m_offset;
	spref_cache(nsp_refs).mscale             := spref_r.m_scale;
	spref_cache(nsp_refs).min_x              := spref_r.min_x;
	spref_cache(nsp_refs).max_x              := spref_r.max_x;
	spref_cache(nsp_refs).min_y              := spref_r.min_y;
	spref_cache(nsp_refs).max_y              := spref_r.max_y;
	spref_cache(nsp_refs).min_z              := spref_r.min_z;
	spref_cache(nsp_refs).max_z              := spref_r.max_z;
	spref_cache(nsp_refs).min_m              := spref_r.min_m;
	spref_cache(nsp_refs).max_m              := spref_r.max_m;
	spref_cache(nsp_refs).csid               := spref_r.cs_id;
	spref_cache(nsp_refs).cs_name            := spref_r.cs_name;
	spref_cache(nsp_refs).cs_type            := spref_r.cs_type;
	spref_cache(nsp_refs).organization       := spref_r.organization;
	spref_cache(nsp_refs).org_coordsys_id    := spref_r.org_coordsys_id;
	spref_cache(nsp_refs).Definition         := spref_r.Definition;
	spref_cache(nsp_refs).description        := spref_r.description;

  End add_spref_cache_info;

  Function exists_spref (spref IN spatial_ref_record_t)
  Return number 
  IS  

    Cursor c_sel_spref (x_offset_in IN float, y_offset_in IN float, xyunits_in IN float, z_offset_in IN float,
                        z_scale_in IN float, m_offset_in IN float, m_scale_in IN float, min_x_in IN float, 
                        max_x_in IN float, min_y_in IN float, max_y_in IN float, min_z_in IN float, 
                        max_z_in IN float, min_m_in IN float, max_m_in IN float, cs_id_in IN number) 
     IS
	SELECT srid
      FROM SDE.st_spatial_references
	  WHERE x_offset = x_offset_in AND y_offset = y_offset_in AND xyunits = xyunits_in AND z_offset = z_offset_in
      AND z_scale = z_scale_in AND m_offset = m_offset_in AND m_scale = m_scale_in AND min_x = min_x_in
      AND max_x = max_x_in AND min_y = min_y_in AND max_y = max_y_in AND min_z = min_z_in
      AND max_z = max_z_in AND min_m = min_m_in AND max_m = max_m_in AND cs_id = cs_id_in;

    Cursor c_sel_epsg_spref (x_offset_in IN float, y_offset_in IN float, xyunits_in IN float, z_offset_in IN float,
                             z_scale_in IN float, m_offset_in IN float, m_scale_in IN float, min_x_in IN float, 
                             max_x_in IN float, min_y_in IN float, max_y_in IN float, min_z_in IN float, 
                             max_z_in IN float, min_m_in IN float, max_m_in IN float, cs_id_in IN number) 
      IS
	SELECT srid
    FROM SDE.st_spatial_references
	WHERE x_offset = x_offset_in AND y_offset = y_offset_in AND xyunits = xyunits_in AND z_offset = z_offset_in
    AND z_scale = z_scale_in AND m_offset = m_offset_in AND m_scale = m_scale_in AND min_x = min_x_in
    AND max_x = max_x_in AND min_y = min_y_in AND max_y = max_y_in AND min_z = min_z_in
    AND max_z = max_z_in AND min_m = min_m_in AND max_m = max_m_in AND cs_id = cs_id_in
    AND srid >= epsg_min and srid <= epsg_max;

    Cursor c_sel_epsgxy_spref (x_offset_in IN float, y_offset_in IN float, xyunits_in IN float,
                               min_x_in IN float, max_x_in IN float, min_y_in IN float, max_y_in IN float,  
                               cs_id_in IN number) 
      IS
	SELECT srid
    FROM SDE.st_spatial_references
	WHERE x_offset = x_offset_in AND y_offset = y_offset_in AND xyunits = xyunits_in  
    AND min_x = min_x_in AND max_x = max_x_in AND min_y = min_y_in AND max_y = max_y_in 
    AND cs_id = cs_id_in AND srid >= epsg_min and srid <= epsg_max;
 
  srid  NUMBER;
  
  Begin
    
    If (spref.z_offset = 0.0 AND spref.z_scale = 1.0 
        AND spref.m_offset = 0.0 AND spref.m_scale = 1.0) THEN
    
      Open c_sel_epsgxy_spref (spref.x_offset, spref.y_offset, spref.xyunits, 
                               spref.min_x, spref.max_x, spref.min_y, spref.max_y, 
                               spref.cs_id);
      Fetch c_sel_epsgxy_spref INTO srid;
      If c_sel_epsgxy_spref%FOUND THEN
       If srid > 0 THEN
        Close c_sel_epsgxy_spref;
        Return srid;
       End if;
      Else
       Close c_sel_epsgxy_spref;
      End if;
             
    Else
      Open c_sel_epsg_spref (spref.x_offset, spref.y_offset, spref.xyunits, spref.z_offset, spref.z_scale, spref.m_offset,
                             spref.m_scale, spref.min_x, spref.max_x, spref.min_y, spref.max_y, spref.min_z, spref.max_z,
                             spref.min_m, spref.max_m, spref.cs_id);
      Fetch c_sel_epsg_spref INTO srid;
      If c_sel_epsg_spref%FOUND THEN
       If srid > 0 THEN
        Close c_sel_epsg_spref;
        Return srid;
       End if;
      Else
       Close c_sel_epsg_spref;
      End if;
      
    End if;
      
    Open c_sel_spref (spref.x_offset, spref.y_offset, spref.xyunits, spref.z_offset, spref.z_scale, spref.m_offset,
                      spref.m_scale, spref.min_x, spref.max_x, spref.min_y, spref.max_y, spref.min_z, spref.max_z,
                      spref.min_m, spref.max_m, spref.cs_id);
    Fetch c_sel_spref INTO srid;
    If c_sel_spref%NOTFOUND THEN
	Close c_sel_spref;
      Return 0;
    End If;
    Close c_sel_spref;
  
    Return srid;

  End exists_spref;

End st_spref_util;

/


Prompt Grants on PACKAGE ST_SPREF_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.ST_SPREF_UTIL TO PUBLIC WITH GRANT OPTION
/
