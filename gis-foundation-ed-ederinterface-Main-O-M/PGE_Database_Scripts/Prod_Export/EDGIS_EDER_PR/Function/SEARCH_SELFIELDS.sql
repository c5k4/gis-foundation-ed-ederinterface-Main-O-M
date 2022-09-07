--------------------------------------------------------
--  DDL for Function SEARCH_SELFIELDS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE FUNCTION "EDGIS"."SEARCH_SELFIELDS" (vFields VARCHAR2,vSubtypeField VARCHAR2,vLookupTblName VARCHAR2,vAliasObjClsName VARCHAR2) RETURN CLOB AS
vSelectFields CLOB;
vFieldsLst varchar2(30000);
vField varchar2(200);
vFieldName varchar2(100);
vFieldAlias varchar2(100);
vDelimiterIndx number;
vSpaceIndx number;
BEGIN

vFieldsLst := vFields || ',';
vSelectFields := '';

WHILE vFieldsLst IS NOT NULL LOOP

  vDelimiterIndx := instr(vFieldsLst,',');
  vField := substr(vFieldsLst,1,vDelimiterIndx - 1);
  vField := TRIM(vField);
  vSpaceIndx := instr(vField,' ');
  
  IF (vSpaceIndx > 0) then
    vFieldName := substr(vField,1,vSpaceIndx-1);
    vFieldAlias := substr(vField,vSpaceIndx);
  ELSE
    vFieldName := vField;
    vFieldAlias := vField;
  END IF;
    
  IF(instr(vFieldName,'SHAPE') > 0) then
    vSelectFields := vSelectFields ||  vFieldName;
  ELSE IF(vFieldName = 'OBJECTID' OR vFieldName = 'GLOBALID') then
    vSelectFields := vSelectFields || vAliasObjClsName || '.' || vFieldName;
  ELSE IF(vFieldName = 'LABELTEXT' OR vFieldName = 'LABELTEXT2') then
    vSelectFields := vSelectFields || vAliasObjClsName || '.' || vFieldName;
  ELSE IF(vFieldName = vSubtypeField) then
    vSelectFields := vSelectFields || 'nvl((select stName from ' || vLookupTblName || ' where upper(fldName) = ''' || vFieldName || ''' and stCode = ' || vAliasObjClsName || '.' || vFieldName || ' and rownum = 1),'  || vAliasObjClsName || '.' || vFieldName || ')';
  ELSE 
    IF (vSubtypeField is null) then
      --vSelectFields := vSelectFields || 'nvl((select dmDesc from ' || vLookupTblName || ' where upper(fldName) = ''' || vFieldName || ''' and dmCode = ' || vAliasObjClsName || '.' || vFieldName || ' and (stCode = ' || vAliasObjClsName || '.' || vSubtypeField || ' or stCode is null) and rownum = 1),'  || vAliasObjClsName || '.' || vFieldName || ')';
      vSelectFields := vSelectFields || 'nvl((select dmDesc from ' || vLookupTblName || ' where upper(fldName) = ''' || vFieldName || ''' and dmCode = ' || vAliasObjClsName || '.' || vFieldName || ' and stCode is null and rownum = 1),'  || vAliasObjClsName || '.' || vFieldName || ')';
    ELSE
      vSelectFields := vSelectFields || 'nvl((select * from (select dmDesc from ' || vLookupTblName || ' where upper(fldName) = ''' || vFieldName || ''' and dmCode = ' || vAliasObjClsName || '.' || vFieldName || ' and (stCode = ' || vAliasObjClsName || '.' || vSubtypeField || ' or stCode is null) order by stCode) where rownum = 1),'  || vAliasObjClsName || '.' || vFieldName || ')';
    END IF;
  END IF;
  END IF;
  END IF;
  END IF; 
  
  vFieldsLst := substr(vFieldsLst,vDelimiterIndx + 1); 
  
  IF (vFieldsLst is not null) then
   vSelectFields := vSelectFields || vFieldAlias || ',';
  ELSE
   vSelectFields := vSelectFields || vFieldAlias;
  END IF;

END LOOP;

return vSelectFields;
END;
