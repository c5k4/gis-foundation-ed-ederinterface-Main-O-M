Prompt drop Package SDO_WFS_PROCESS;
DROP PACKAGE MDSYS.SDO_WFS_PROCESS
/

Prompt Package SDO_WFS_PROCESS;
--
-- SDO_WFS_PROCESS  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_WFS_PROCESS AUTHID current_user AS
	SPATIALCOLSEP varchar2(40) := '<>';
        SPATIALPATHELEMSEP varchar2(20) := '>';
        NS_SEP varchar2(20) := '<';
        NULL_NS varchar2(20) := 'null';
        KEY_SEP varchar2(20) := ';';
        COMMIT_THRESHOLD number  := 1000;
        LARGE_THRESHOLD number  := 0;

	-- publish from relation table content
	procedure publishFeatureType(dataSrc IN VARCHAR2,
                                     ftnsUrl IN VARCHAR2,
                                     ftName IN VARCHAR2,
                                     ftnsAlias IN VARCHAR2,
                                     featureDesc IN XMLType,
                                     SCHEMALOCATION IN VARCHAR2,
                                     pkeyCol IN VARCHAR2,
                                     columnInfo IN mdsys.StringList, pSpatialCol varchar2, featureMemberNS varchar2, featureMemberName varchar2, srsNS varchar2, srsNSAlias varchar2, mandatoryColumnInfo IN mdsys.StringList default mdsys.StringList('$ALL$'), wfs_version IN varchar2 default '1.0.0') ;
	procedure publishFeatureType(dataSrc IN VARCHAR2,
				     ftnsUrl IN VARCHAR2,
				     ftName IN VARCHAR2,
				     ftnsAlias IN VARCHAR2,
				     featureDesc IN XMLType,
				     SCHEMALOCATION IN VARCHAR2,
				     pkeyCol IN VARCHAR2,
				     columnInfo IN mdsys.StringList, pSpatialCol varchar2, featureMemberNS varchar2, featureMemberName varchar2, srsNS varchar2, srsNSAlias varchar2, viewTableName varchar2, viewTablepkeyCol varchar2, mandatoryColumnInfo IN mdsys.StringList default mdsys.StringList('$ALL$'), wfs_version IN varchar2 default '1.0.0') ;
	-- publish from a external feature xsd
	procedure publishFeatureType(dataSrc IN VARCHAR2,
				     ftnsUrl IN VARCHAR2,
				     ftName IN VARCHAR2,
				     ftnsAlias IN VARCHAR2,
				     featureDesc IN XMLType,
				     SCHEMALOCATION IN VARCHAR2,
				     pkeyCol IN VARCHAR2,
				     columnInfo IN mdsys.StringList, pSpatialCol varchar2, featureMemberNS varchar2, featureMemberName varchar2, srsNS varchar2, srsNSAlias varchar2, featureCollectionNS varchar2, featureCollectionName varchar2, isGML3 varchar2, formattedkeyCols mdsys.StringList, viewTableName varchar2, viewTablepkeyCol varchar2, viewTableFmtKeyCols mdsys.StringList, mandatoryColumnInfo IN mdsys.StringList default mdsys.StringList('$ALL$'), wfs_version IN varchar2 default '1.0.0') ;
	-- publish from a external feature xsd
	procedure publishFeatureType(ftxsd IN XMLType, featureDesc IN XMLType, wfs_version IN varchar2 default '1.0.0');
	procedure processInLineComplexElement(ftxsd IN XMLType,
			       elementXml IN XMLType,
                               elemIndex IN number,
                               spatialCols IN OUT NOCOPY mdsys.StringList,
                               currGmlElementsCnt IN OUT number,
                               gmlElementsCnt IN number,
                               parentElemPath IN varchar2,
                               gmlAlias IN VARCHAR2);
	procedure processComplexElement(ftxsd IN XMLType,
                               elementType IN varchar2,
                               spatialCols IN OUT NOCOPY mdsys.StringList,
                               currGmlElementsCnt IN OUT number,
                               gmlElementsCnt IN number,
                               parentElemPath IN varchar2,
                               gmlAlias IN VARCHAR2);
	function isComplexObjectType(elemType IN VARCHAR2, gmlAlias IN VARCHAR2) return number ;
	function isInList(l mdsys.StringList, item varchar2) return number ;

	procedure  registerObject(objNs IN VARCHAR2, objName IN VARCHAR2, ftnsUrl IN VARCHAR2, currObjColPath varchar2, objNameIndex IN  NUMBER, ftId IN NUMBER, spatialColInfo mdsys.StringList, spatialColInfoIndex IN OUT NUMBER);
        function getXMLTagType(typeOwner IN VARCHAR2, type IN VARCHAR2) RETURN VARCHAR2;
	procedure dropFeatureType(ftnsUrl IN VARCHAR2, ftName IN VARCHAR2);
	procedure dropFeatureTypes(ftnsUrl IN VARCHAR2);
	procedure insertFtMDUpdated(ns varchar2, name varchar2, updateTs timestamp) ;
	procedure insertFtDataUpdated(ns varchar2, name varchar2, updatedRowList ROWPOINTERLIST, updateTs timestamp);
	procedure insertCapabilitiesInfo(capabilitiesInfo xmltype, wfs_version IN varchar2 default '1.0.0');
	procedure deleteCapabilitiesInfo;
	procedure  genCollectionProcs ;
	procedure genCollectionProcsHelper(objNs IN VARCHAR2, objName IN VARCHAR2) ;
	procedure populateFeatureTypeXMLInfo(xmlcoltypexsd XMLType,
			fTypeId number, columnName varchar2,
			objPathInfo mdsys.StringList) ;
	procedure deleteFeatureTypeXMLInfo(fTypeId number);
	function containsSDO(ns varchar2, name varchar2, ftId number) return number;
	function containsSDOInternal(rootns varchar2, rootname varchar2,
				ns varchar2, name varchar2, ftId number) return number ;
	procedure updateObjectRef(refObjNs varchar2, refObjName varchar2, featureTypeId number , refObjSeqNo number) ;
	procedure updateObjectRef(refObjNs varchar2, refObjName varchar2,
		refObjSeqNo number, nsUrl varchar2,
		containerObjNs varchar2, containerObjName varchar2, containerObjNameIndex number, featureTypeId number) ;
	function sortSpContentByColIndex (spatialPaths mdsys.StringList, spatialPathInfo sdo_geom_path_info,
		userName varchar2, tableName in varchar2) return sdo_geom_path_info;
        function genSpatialPathElemColIndex(spatialPaths mdsys.StringList, spatialPathElem sdo_geom_path_elem) return number ;
        function genIdValue(data xmltype, idPaths mdsys.StringList, tNS varchar2, tName varchar2, ftUserName varchar2) return varchar2 ;
        function getPath(path mdsys.StringList) return varchar2;
        function getParsedPath(path varchar2) return mdsys.StringList; -- (ns1, name1, ns2, name2....)
        procedure genRecordTypeProcessingLogic(tNS varchar2, tName varchar2) ;
        function extractSDO(xmldata XMLType, spatialPaths mdsys.StringList, srsNs varchar2, ISGML3 varchar2, spPathsSRSNSList mdsys.StringListList) return sdo_geom_path_info ;
        function parsePath(path varchar2) return mdsys.StringList ;
        function generateParsedSpatialPath(spatialPaths mdsys.StringList) return mdsys.StringListList ;
        function processPath(srcPaths mdsys.StringListList,  genArrIdx number) return mdsys.StringListList ;
        function generateAliasString(aliasList mdsys.StringList) return varchar2 ;
        function getAlias(nsurl varchar2, aliasList mdsys.StringList) return varchar2 ;
        function putAlias (nsurl varchar2, alias varchar2, aliasList mdsys.StringList) return mdsys.StringList;
	function isRowIdIn (r ROWID, rlist RowPointerList) return number ;
	function isPKColIn (r varchar2, rlist RowPointerList) return number;
	function extractID(xmldata XMLType, idPaths mdsys.StringList) return varchar2 ;
	function processIdPath(srcPaths mdsys.StringList) return mdsys.StringList ;

	function getFeatureTypeId(ftnsUrl IN VARCHAR2,
				  ftName IN VARCHAR2) return number;
	procedure grantMDAccessToAdminUser(userName IN varchar2) ;
	procedure grantMDAccessToUser(userName IN varchar2);
	procedure revokeMDAccessFromAdminUser(userName IN varchar2) ;
	procedure revokeMDAccessFromUser(userName IN varchar2);
	procedure grantFeatureTypeToUser(ftnsUrl IN VARCHAR2,
                                  ftName IN VARCHAR2,
				  userName IN varchar2);
	procedure revokeFeatureTypeFromUser(ftnsUrl IN VARCHAR2,
				  ftName IN VARCHAR2,
                                  userName IN varchar2);
	procedure registerMTableView(ftNSUrl varchar2,
                     ftName varchar2,
                     viewTableList mdsys.StringList,
                     viewTablePkeyColList mdsys.StringList,
                     tablePKeyColList mdsys.StringList);
	procedure registerMTableView(ftNSUrl varchar2,
                     ftName varchar2,
                     viewTableList mdsys.StringList,
                     viewTablePkeyColList mdsys.StringList,
                     formattedViewTableColList mdsys.StringListList,
                     tablePKeyColList mdsys.StringList);
	procedure unRegisterMTableView(ftNSUrl varchar2,
		       ftName varchar2);
	function notInArray(p varchar2, pList mdsys.RowPointerList) return number;

	function genSpatialArrInfo(tableNamePrefix varchar2, pkVal varchar2,
				   l_spatialArrList mdsys.StringListList,
				   data XMLType,  srsNs varchar2, spatialArrUpdMap varchar2, ISGML3 varchar2, spArrPathsSRSNSList mdsys.StringListList)
	return mdsys.SDO_GEOM_PATH_INFO_LIST;
	function  getUpdateMap(spatialArrList mdsys.StringListList) return varchar2;
	function expandPath(aliasedPath varchar2, nsParam varchar2) return varchar2;
	function findNSUrl(nsAlias varchar2, nsAliasList mdsys.StringList, nsUrlList mdsys.StringList) return varchar2;
	function getCount(pathPattern varchar2, nsParam varchar2, data xmltype) return number;
	function enumerateArrPaths(pathPattern varchar2, nsParam varchar2, data xmltype) return mdsys.StringList;
	procedure genTriggers(tNS varchar2, tName varchar2);
	function isListMember(featureTypeName varchar2, namespaceUrl varchar2, ftIdList mdsys.StringListList) return number;
	PROCEDURE initialize_after_import;

	FUNCTION prepare_for_export_user return varchar2;
	FUNCTION prepare_for_export_user(exportSDOMetadata varchar2) return varchar2;

	FUNCTION prepare_for_export_fts (ftList mdsys.StringListList) return varchar2;
	FUNCTION prepare_for_export_fts (ftList mdsys.StringListList,  exportSDOMetadata varchar2) return varchar2;
	PROCEDURE prepare_for_export_ft_m_int(ftName in varchar2, ftNsUrl in varchar2, exportSDOMetadata varchar2) ;
	PROCEDURE prepare_for_export_ft_int(ftName in varchar2, ftNsUrl in varchar2, exportSDOMetadata varchar2);

	FUNCTION prepare_for_export_all return varchar2;
	FUNCTION prepare_for_export_all(exportSDOMetadata varchar2) return varchar2;
	procedure drop_export_tables;
	procedure genUpdateStr(spatialColCount number, update_prefix varchar2, update_suffix varchar2);
	procedure preProcessInsert (xmldata xmltype,
				    idPaths mdsys.StringList,
				    spatialPaths mdsys.StringList,
				    parsedAliasSpPath mdsys.StringListList,
				    spPathsSRSNSList mdsys.StringListList,
				    spatialArrPathsList mdsys.StringListList,
				    spArrPathsSRSNSList mdsys.StringListList,
				    ftNS varchar2,
				    ftName varchar2,
			            ftId number,
				    ftUserName varchar2,
				    tableName varchar2,
				    srsNs varchar2,
				    ISGML3 varchar2,
				    spatialPathsStgSrIds mdsys.sdoNumList, -- spatialPaths storage srid
				    spatialArrPathsStgSrIds mdsys.sdoNumList, pkColVal OUT VARCHAR2); -- spatial Arr paths storage srids
	procedure preProcessUpdate(xmldata xmltype,
				   oldId varchar2,
				   spColUpdMap mdsys.StringList,
				   spArrColUpdMap mdsys.StringList,
				   idPaths mdsys.StringList,
				   spatialPaths mdsys.StringList,
				   parsedAliasSpPath mdsys.StringListList,
				   spPathsSRSNSList mdsys.StringListList,
				   spatialArrPathsList mdsys.StringListList,
				   spArrPathsSRSNSList mdsys.StringListList,
				   ftNS varchar2,
				   ftName varchar2,
			  	   ftId number,
				   ftUserName varchar2,
				   tableName varchar2,
				   srsNs varchar2,
				   ISGML3 varchar2,
				   spatialPathsStgSrIds mdsys.sdoNumList,  -- spatialPaths storage srid
				   spatialArrPathsStgSrIds mdsys.sdoNumList,   -- spatial Arr paths storage srids
				   computeSp number, newId OUT varchar2);

END SDO_WFS_PROCESS;
/


Prompt Synonym SDO_WFS_PROCESS;
--
-- SDO_WFS_PROCESS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_WFS_PROCESS FOR MDSYS.SDO_WFS_PROCESS
/


Prompt Grants on PACKAGE SDO_WFS_PROCESS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_WFS_PROCESS TO PUBLIC
/
