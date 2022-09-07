Prompt drop Package Body XML_UTIL;
DROP PACKAGE BODY SDE.XML_UTIL
/

Prompt Package Body XML_UTIL;
--
-- XML_UTIL  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY SDE.xml_util
/***********************************************************************
*
*N  {xml_util.spb}  --  Implementation for XML DDL package 
*
*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
*
*P  Purpose:
*     This PL/SQL package body implements the procedures to perform
*   DDL operations on ArcSDE XML system tables.  It should be compiled 
*   by the SDE DBA user; security is by user name.   
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
*    Annette Locke            07/31/02               Original coding.
*E
***********************************************************************/
IS

  /* Package Globals. */

  G_sde_dba       BOOLEAN NOT NULL DEFAULT FALSE;
  G_current_user  NVARCHAR2(32);



   /* Public Subprograms. */

   PROCEDURE xml_index_tags_def_insert (indexIdVal  IN xml_index_tags_index_id_t,
                                        index_tag  IN xml_index_tags_record_t)
  /***********************************************************************
  *
  *N  {xml_index_tags_def_insert}  --  Insert an index tag entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a set of index tag entries into the 
  *   SDE.SDE_XML_INDEX_TAGS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     index_tag<IN>  ==  (xml_index_tags_record_t)  
  *                                      Index tag to insert.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *    Gautam Shanbhag       09/15/04           Insert into Modified index_tags table.
  *E
  ***********************************************************************/
   IS

     BEGIN

       -- Insert the index tags.

	   IF index_tag.tag_id IS NOT NULL
	   THEN INSERT INTO SDE.sde_xml_index_tags(index_id, tag_id, tag_name, data_type, 
			tag_alias, description, is_excluded)
	        VALUES(indexIdVal, index_tag.tag_id, index_tag.tag_name, index_tag.data_type,
			index_tag.tag_alias, index_tag.description, index_tag.is_excluded);
	   END IF;
	   -- Since we've gotten this far without an exception, it must be OK
	   -- to commit;

	   COMMIT;
	   
	 END xml_index_tags_def_insert;

   PROCEDURE xml_indexes_def_insert (xml_index IN xml_indexes_record_t)
  /***********************************************************************
  *
  *N  {xml_indexes_def_insert}  --  Insert an XML index entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a tag entry in the SDE.SDE_XML_INDEXES table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     indes <IN>  ==  (xml_indexes_record_t)  Index to insert.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

     -- Insert the record into the sde_xml_indexes table.

	 INSERT INTO SDE.sde_xml_indexes
	     (index_id, index_name, owner, index_type, description)
	 VALUES (xml_index.index_id, xml_index.index_name, 
	           xml_index.owner, xml_index.index_type, 
			   xml_index.description);
	 EXCEPTION
	   WHEN DUP_VAL_ON_INDEX THEN
		   raise_application_error(sde_util.SE_XML_INDEX_EXISTS,
		                           'Index ' || xml_index.owner || '.' ||
								   xml_index.index_name || ' already exists.');

	 -- Since we've gotten this far without an exception, it must be OK
	 -- to commit.

	 COMMIT;

   END xml_indexes_def_insert;

   PROCEDURE xml_indexes_def_delete (ownerVal IN xml_indexes_owner_t,
                                     indexNameVal IN xml_indexes_name_t)
  /***********************************************************************
  *
  *N  {xml_indexes_def_delete}  --  Delete an XML index entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes an index entry in the SDE.SDE_XML_INDEXES table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *  ownerVal     <IN>  ==  (xml_indexes_owner_t) Owner of index to delete.
  *  indexNameVal <IN>  ==  (xml_indexes_name_t) Name of index to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *E
  ***********************************************************************/
   IS
	BEGIN

	  -- Delete XML index record. Cascading constraint will delete from
	  -- index_tags_table;

	  DELETE FROM SDE.sde_xml_indexes
	  WHERE owner = ownerVal AND 
		    index_name = indexNameVal;

	  COMMIT;

	END xml_indexes_def_delete;

   
   PROCEDURE xml_columns_def_insert (column IN xml_columns_record_t)
  /***********************************************************************
  *
  *N  {xml_columns_def_insert}  --  Insert an XML column entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure inserts a column entry in the SDE.SDE_XML_COLUMNS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     column <IN>  ==  (xml_columns_record_t) Column entry to insert.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

	   -- Insert column entry.

	   INSERT INTO SDE.sde_xml_columns(column_id, registration_id, 
	                                   column_name, index_id, 
									   minimum_id, config_keyword,xflags)
	   VALUES(column.column_id, column.registration_id,
	          column.column_name, column.index_id, 
			  column.minimum_id, column.config_keyword,column.xflags);
	                               
	 END xml_columns_def_insert;


   PROCEDURE xml_columns_def_delete (columnIdVal IN xml_columns_column_id_t)
  /***********************************************************************
  *
  *N  {xml_columns_def_delete}  --  Delete an XML column entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a column entry in the SDE.SDE_XML_COLUMNS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     columnIdVal <IN>  ==  (xml_columns_column_id_t) Id of column to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

	   -- Delete column entry.

	   DELETE FROM SDE.sde_xml_columns WHERE column_id = columnIdVal;

	   COMMIT;
	   
	 END xml_columns_def_delete;
	 
   
   PROCEDURE xml_columns_def_update (column IN xml_columns_record_t)
  /***********************************************************************
  *
  *N  {xml_columns_def_update}  --  Update an XML column entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a column entry in the SDE.SDE_XML_COLUMNS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    column <IN>  ==  (xml_columns_record_t) Column entry to update.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *                     SE_XML_TAG_EXIST
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Annette Locke         07/31/02           Original coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

	   -- Update column entry.
	IF (column.xflags<>-1)
	THEN  UPDATE SDE.sde_xml_columns 
	   SET index_id = column.index_id,
	   minimum_id = column.minimum_id,
	   config_keyword = column.config_keyword,
	   xflags = column.xflags
	   WHERE column_id = column.column_id;
	ELSE
	   UPDATE SDE.sde_xml_columns 
	   SET index_id = column.index_id,
	   minimum_id = column.minimum_id,
	   config_keyword = column.config_keyword
	   WHERE column_id = column.column_id;
	END IF;
	   COMMIT;
	                               
	 END xml_columns_def_update;

   PROCEDURE xml_indexes_def_update (indexIdVal        IN xml_indexes_index_id_t, 
                                     indexNameVal         IN xml_indexes_name_t, 
                                     indexDescriptionVal  IN xml_indexes_description_t)
  /***********************************************************************
  *
  *N  {xml_indexes_def_update}  --  Update an XML index entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates an index entry in the SDE.SDE_XML_INDEXES table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *    indexIdVal  <IN>  ==  (xml_indexes_id_t) Index entry to update.
  *    indexName   <IN>  ==  (xml_indexes_name_t) Index name to update.
  *    indexDescription <IN>  ==  (xml_indexes_description_t) 
  *                                        Index description to update.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Josefina Santiago       10/25/07               Original Coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

	 -- Update index entry.
	 UPDATE SDE.sde_xml_indexes 
	 SET index_name = indexNameVal,
	     description = indexDescriptionVal
	 WHERE index_id = indexIdVal;
	
	 COMMIT;
	                               
 END xml_indexes_def_update;	 	 
	 

 PROCEDURE xml_index_tags_def_update (indexIdVal  IN xml_index_tags_index_id_t,
                                        index_tag  IN xml_index_tags_record_t)
  /***********************************************************************
  *
  *N  {xml_index_tags_def_update}  --  Updates an index tag entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure updates a set of index tag entries into the 
  *   SDE.SDE_XML_INDEX_TAGS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     index_tag<IN>  ==  (xml_index_tags_record_t)  
  *                                      Index tag to insert.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Gautam Shanbhag       01/10/05           Original coding.
  *E
  ***********************************************************************/
   IS

     BEGIN

       -- Insert the index tags.

	   IF index_tag.tag_name IS NOT NULL
	   THEN UPDATE SDE.sde_xml_index_tags
		SET tag_alias = index_tag.tag_alias,
	   description = index_tag.description,
	   is_excluded = index_tag.is_excluded
	   WHERE index_id = indexIdVal AND
		   tag_name = index_tag.tag_name;
	   END IF;
	   -- Since we've gotten this far without an exception, it must be OK
	   -- to commit;

	   COMMIT;
	   
	 END xml_index_tags_def_update;

 PROCEDURE xml_index_tags_def_delete (indexIdVal  IN xml_index_tags_index_id_t,
                                        tag_list  IN tagid_list_t)
  /***********************************************************************
  *
  *N  {xml_index_tags_def_delete}  --  Deletes an index tag entry.
  *
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *P  Purpose:
  *     This procedure deletes a set of index tag entries from the 
  *   SDE.SDE_XML_INDEX_TAGS table.  
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *A  Parameters:
  *     index_tag<IN>  ==  (xml_index_tags_record_t)  
  *                                      Index tag to delete.
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *X  SDE Exceptions:
  *                     SE_NO_PERMISSIONS
  *E
  *:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  *
  *H  History:
  *
  *    Gautam Shanbhag       01/10/05           Original coding.
  *E
  ***********************************************************************/
   IS
     tag_id_index       BINARY_INTEGER;

     BEGIN

       -- Delete the index tags.
       tag_id_index := tag_list.FIRST;
       WHILE tag_id_index IS NOT NULL LOOP
          DELETE FROM SDE.sde_xml_index_tags
	      WHERE index_id = indexIdVal AND
		  tag_id = tag_list(tag_id_index);
          tag_id_index := tag_list.NEXT(tag_id_index);
       END LOOP;
	   -- Since we've gotten this far without an exception, it must be OK
	   -- to commit;

	   COMMIT;
	   
END xml_index_tags_def_delete;

BEGIN

  G_current_user := sde_util.sde_user;
  G_sde_dba := (G_current_user = sde_util.C_sde_dba) OR 
                (G_current_user = sde_util.C_sde_master);

END xml_util;

/


Prompt Grants on PACKAGE XML_UTIL TO PUBLIC to PUBLIC;
GRANT EXECUTE ON SDE.XML_UTIL TO PUBLIC WITH GRANT OPTION
/
