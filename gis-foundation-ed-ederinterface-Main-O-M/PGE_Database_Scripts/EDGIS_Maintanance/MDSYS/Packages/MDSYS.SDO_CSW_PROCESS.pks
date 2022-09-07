Prompt drop Package SDO_CSW_PROCESS;
DROP PACKAGE MDSYS.SDO_CSW_PROCESS
/

Prompt Package SDO_CSW_PROCESS;
--
-- SDO_CSW_PROCESS  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_CSW_PROCESS AUTHID current_user AS
	SPATIALCOLSEP varchar2(20) := '<>';
	SPATIALPATHELEMSEP varchar2(20) := '>';
	NS_SEP varchar2(20) := '<';
	NULL_NS varchar2(20) := 'null';
	function sortSpContentByColIndex (spatialPaths mdsys.StringList, spatialPathInfo sdo_geom_path_info,
		userName varchar2, tableName in varchar2) return sdo_geom_path_info;
	function genSpatialPathElemColIndex(spatialPaths mdsys.StringList, spatialPathElem sdo_geom_path_elem) return number ;
	function genIdValue(data xmltype, tNS varchar2, tName varchar2,  rtUserName varchar2) return varchar2 ;
	function getPath(path mdsys.StringList) return varchar2;
	function getParsedPath(path varchar2) return mdsys.StringList; -- (ns1, name1, ns2, name2....)
	procedure genRecordTypeProcessingLogic(tNS varchar2, tName varchar2) ;
	function isRowIdIn (r ROWID, rlist RowPointerList) return number ;
	procedure insertRtMDUpdated(ns varchar2, name varchar2, updateTs timestamp) ;
        procedure insertRtDataUpdated(ns varchar2, name varchar2, updatedRowList ROWPOINTERLIST, updateTs timestamp);
        procedure insertCapabilitiesInfo(capabilitiesInfo xmltype);
        procedure deleteCapabilitiesInfo;

	function getRecordTypeId(rtnsUrl IN VARCHAR2,
                                  rtName IN VARCHAR2) return number;
	procedure insertPluginMap(rtnsUrl IN VARCHAR2, rtName IN VARCHAR2,
					pluginpackageName varchar2);

	procedure deletePluginMap(rtnsUrl IN VARCHAR2, rtName IN VARCHAR2);
	procedure insertDomainInfo(recordTypeId NUMBER,
 				   propertyName VARCHAR2,
 				   parameterName VARCHAR2,
 				   pValue STRINGLIST);
	procedure deleteDomainInfo(recordTypeId NUMBER, propertyName VARCHAR2,
				   parameterName VARCHAR2);

	procedure insertRecordViewMap(recordTypeNs VARCHAR2,
 				      viewSrcName VARCHAR2,
 				      targetTypeName VARCHAR2,
 				      mapInfo XMLTYPE,
 				      mapType VARCHAR2);
	procedure deleteRecordViewMap (recordTypeNs VARCHAR2,
                                      viewSrcName VARCHAR2,
                                      targetTypeName VARCHAR2);

	function extractID(xmldata XMLType, idPaths mdsys.StringList) return varchar2;
	function processIdPath(srcPaths mdsys.StringList) return mdsys.StringList;
	function parsePath(path varchar2) return mdsys.StringList;
	function parsePathElimTypePrefix(path varchar2) return mdsys.StringList;
	function generateAliasString(aliasList mdsys.StringList) return varchar2;
	function getAlias(nsurl varchar2, aliasList mdsys.StringList) return varchar2;
	function putAlias (nsurl varchar2, alias varchar2, aliasList mdsys.StringList) return mdsys.StringList;
	function mergeSysAndUserDefinedGeomInfo(l_spatialPaths mdsys.StringList, sysInfo mdsys.sdo_geom_path_info, userInfo mdsys.sdo_geom_path_info)  return  mdsys.sdo_geom_path_info;
	function generateParsedSpatialPath(spatialPaths mdsys.StringList, typeNS varchar2, typeName varchar2) return mdsys.StringList;
	function extractSDO(xmldata XMLType, spatialPaths mdsys.StringList,  userInfo mdsys.sdo_geom_path_info,  srsNs varchar2, ISGML3 varchar2, typeNS varchar2, typeName varchar2, spPathsSRSNSList mdsys.StringListList) return sdo_geom_path_info;
	function generateParsedSpatialPath(spatialPaths mdsys.StringList) return mdsys.StringListList;
	function processPath(srcPaths mdsys.StringListList, genArrIdx number) return mdsys.StringListList;
	function getProcessedIndex(userInfo mdsys.sdo_geom_path_info, spatialPath varchar2, typeNS varchar2, typeName varchar2) return number;
	function isInList(l mdsys.StringList, item varchar2) return number ;
	function genSpatialArrInfo(tableNamePrefix varchar2, pkVal varchar2,
                                   l_spatialArrList mdsys.StringListList,
                                   data XMLType,  srsNs varchar2, spatialArrUpdMap varchar2, ISGML3 varchar2,
				   typeNS varchar2, typeName varchar2,  spArrPathsSRSNSList mdsys.StringListList)
        return mdsys.SDO_GEOM_PATH_INFO_LIST;
        function  getUpdateMap(spatialArrList mdsys.StringListList) return varchar2;
        function expandPath(aliasedPath varchar2, nsParam varchar2) return varchar2;
        function findNSUrl(nsAlias varchar2, nsAliasList mdsys.StringList, nsUrlList mdsys.StringList) return varchar2;
        function getCount(pathPattern varchar2, nsParam varchar2, data xmltype) return number;
        function enumerateArrPaths(pathPattern varchar2, nsParam varchar2, data xmltype) return mdsys.StringList;
	function isPKColIn (r varchar2, rlist RowPointerList) return number;


 	PROCEDURE initialize_after_import;

        FUNCTION prepare_for_export_user return varchar2;
        FUNCTION prepare_for_export_user(exportSDOMetadata varchar2) return varchar2;

        FUNCTION prepare_for_export_rts (rtList mdsys.StringListList) return varchar2;
        FUNCTION prepare_for_export_rts (rtList mdsys.StringListList,  exportSDOMetadata varchar2) return varchar2;
        PROCEDURE prepare_for_export_rt_m_int(rtName in varchar2, rtNsUrl in varchar2, exportSDOMetadata varchar2) ;
        PROCEDURE prepare_for_export_rt_int(rtName in varchar2, rtNsUrl in varchar2, exportSDOMetadata varchar2);

        FUNCTION prepare_for_export_all return varchar2;
        FUNCTION prepare_for_export_all(exportSDOMetadata varchar2) return varchar2;
        procedure drop_export_tables;
        procedure genUpdateStr(spatialColCount number, update_prefix varchar2, update_suffix varchar2);

	procedure preProcessInsert (xmldata xmltype,
				idPaths mdsys.StringList,
				origidPaths mdsys.StringList,
				extractorType varchar2,
				spatialPaths mdsys.StringList,
				parsedAliasSpPath mdsys.StringListList,
				spPathsSRSNSList mdsys.StringListList,
				spatialArrPathsList mdsys.StringListList,
				spArrPathsSRSNSList mdsys.StringListList,
				rtNS varchar2,
				rtName varchar2,
				rtId number,
				rtUserName varchar2,
				tableName varchar2,
				srsNs varchar2,
				ISGML3 varchar2,
				spatialPathsStgSrIds mdsys.sdoNumList,
				spatialArrPathsStgSrIds mdsys.sdoNumList,
				p_type varchar2,
				pkColVal OUT varchar2) ;

	procedure preProcessUpdate(xmldata xmltype,
				oldId varchar2,
				spColUpdMap mdsys.StringList,
				spArrColUpdMap mdsys.StringList,
				idPaths mdsys.StringList,
				extractorType varchar2,
				spatialPaths mdsys.StringList,
				parsedAliasSpPath mdsys.StringListList,
				spPathsSRSNSList mdsys.StringListList,
				spatialArrPathsList mdsys.StringListList,
				spArrPathsSRSNSList mdsys.StringListList,
				rtNS varchar2,
				rtName varchar2,
				rtId number,
				rtUserName varchar2,
				tableName varchar2,
				srsNs varchar2,
				ISGML3 varchar2,
				spatialPathsStgSrIds mdsys.sdoNumList,
				spatialArrPathsStgSrIds mdsys.sdoNumList,
				computeSp number,
				p_type varchar2,
				newId OUT varchar2);
	procedure grantMDAccessToAdminUser(userName IN varchar2) ;
	procedure revokeMDAccessFromAdminUser(userName IN varchar2) ;
	procedure grantMDAccessToUser(userName IN varchar2) ;
	procedure revokeMDAccessFromUser(userName IN varchar2) ;
	procedure grantRecordTypeToUser(rtnsUrl IN VARCHAR2, rtName IN VARCHAR2, userName IN varchar2);
	procedure revokeRecordTypeFromUser(rtnsUrl IN VARCHAR2, rtName IN VARCHAR2, userName IN varchar2);
end;
/


Prompt Grants on PACKAGE SDO_CSW_PROCESS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_CSW_PROCESS TO PUBLIC
/
