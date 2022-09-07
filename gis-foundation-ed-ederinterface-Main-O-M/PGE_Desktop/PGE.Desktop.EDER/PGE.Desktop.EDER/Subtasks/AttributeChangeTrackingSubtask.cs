using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
//using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop.Process;
using ESRI.ArcGIS.Framework;
using System.Xml.Serialization;
using System.IO;

namespace PGE.Desktop.EDER.Subtasks
{
    [ComVisible(true)]
    [Guid("AEFDB6A1-93AA-4A93-8EB9-032052E8DE63")]
    [ProgId("PGE.Desktop.EDER.Subtasks.AttributeChangeTrackingSubtask")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class AttributeChangeTrackingSubtask : IMMPxSubtask, IMMPxSubtask2
    {
        protected static PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        private static IApplication _app;
        private static IMMPxApplication _PxApp;

        protected IMMPxTask _Task;
        private IMMEnumExtensionNames _SupportedExtensions = null;
        private Miner.Interop.Process.IDictionary _SupportedParameters = null;
        private Miner.Interop.Process.IDictionary _Parameters = null;
        IMMExtension m_pSMExt = null;
        IMMEnumExtensionNames m_pPxExtNames = null;

        private static IWorkspace _loginWorkspace;
        private static string trackingFeaturesTableName = "PGEDATA.TRACKED_FEATURES";
        private static string recordSessionEditsTableName = "PGEDATA.SESSIONDATA";
        private static string sessionCurOwnerTableName = "PGEDATA.SESSIONCURRENTOWNER";
        private static List<FEATURE> featureClsList = null;
        private static List<ATTRIBUTE> featAttributeList = null;
        private static bool trackedFeatureFound = false;

        #region Component Registration

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction, ComVisible(false)]
        static void Register(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction, ComVisible(false)]
        static void Unregister(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Unregister(regKey);
        }
        #endregion

        public bool Enabled(IMMPxNode pPxNode)
        {
            return true;
        }

        public bool Execute(IMMPxNode pPxNode)
        {
            DisplayStatusBar("Edits Capturing started for session id - " + pPxNode.Id.ToString());

            featureClsList = new List<FEATURE>();
            string versionOwner = string.Empty;
            string changingVersionName = string.Empty;
            string referenceVersionName = "SDE.DEFAULT";
            try
            {                
                _loginWorkspace = GetWorkspace();
                if (_loginWorkspace != null)
                {
                    ITable versionTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable("SDE.VERSIONS");
                    if (versionTbl != null)
                    {
                        IQueryFilter vQf = new QueryFilterClass();
                        vQf.WhereClause = "NAME ='SN_" + pPxNode.Id + "'";
                        ICursor vCur = versionTbl.Search(vQf, false);
                        IRow vRw = vCur.NextRow();
                        if (vRw != null)
                        {
                            versionOwner = vRw.get_Value(vRw.Fields.FindField("OWNER")).ToString();
                        }
                        //release cursor
                        if (vCur != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(vCur);
                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(vCur);
                            vCur = null;
                        }
                    }

                    changingVersionName = versionOwner + ".SN_" + pPxNode.Id.ToString();

                    // Get references to the child and parent versions.
                    IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)_loginWorkspace;
                    IVersion changingVersion = versionedWorkspace.FindVersion(changingVersionName);
                    IVersion referenceVersion = versionedWorkspace.FindVersion(referenceVersionName);

                    // Cast to the IVersion2 interface to find the common ancestor.
                    IVersion2 childVersion2 = (IVersion2)changingVersion;
                    IVersion commonAncestorVersion = childVersion2.GetCommonAncestor(referenceVersion);

                    // Cast the child version to IFeatureWorkspace and open the table.
                    IFeatureWorkspace childFWS = (IFeatureWorkspace)changingVersion;
                    IFeatureWorkspace parentsFWS = (IFeatureWorkspace)referenceVersion;                                        
                    

                    ITable trackingFeaturesTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable(trackingFeaturesTableName);
                    if (trackingFeaturesTbl != null)
                    {
                        IQueryFilter pQf = new QueryFilterClass();
                        pQf.SubFields = "FEATURECLASSNAME,FEATURESUBTYPECD,ATTRIBUTES";
                        pQf.WhereClause = "UPPER(OBJECTTYPE) = 'FEATURECLASS'";
                        ICursor pCur = trackingFeaturesTbl.Search(pQf, true);
                        IRow pTableRow = null;
                        int fclsIndex = pCur.FindField("FEATURECLASSNAME");
                        int subtypeIndex = pCur.FindField("FEATURESUBTYPECD");
                        int fieldsIndex = pCur.FindField("ATTRIBUTES");
                        while ((pTableRow = pCur.NextRow()) != null)
                        {
                            string featClsName = pTableRow.get_Value(fclsIndex).ToString();
                            string[] fSubtypes = pTableRow.get_Value(subtypeIndex).ToString().Split(',');
                            string[] fields = pTableRow.get_Value(fieldsIndex).ToString().Split(',');
                            _logger.Info("Process started for : " + featClsName);

                            //open featureClass in Default Version
                            IFeatureClass parentWkspFeatureClass = parentsFWS.OpenFeatureClass(featClsName);
                            //open featureClass in current Version
                            IFeatureClass childWkspFeatureClass = childFWS.OpenFeatureClass(featClsName);
                            //if (firstFeatureClass == null)
                            //    return;
                            ITable firstFC = childWkspFeatureClass as ITable;
                            // Cast the common ancestor version to IFeatureWorkspace and open the table.
                            IFeatureWorkspace commonAncestorFWS = (IFeatureWorkspace)commonAncestorVersion;
                            //open featureClass in commonAncestor workspace
                            IFeatureClass commonAncestorfeatureClass = commonAncestorFWS.OpenFeatureClass(featClsName);
                            ITable commonAncestorFC = commonAncestorfeatureClass as ITable;
                            // Cast to the IVersionedTable interface to create a difference cursor.
                            IVersionedTable versionedTable = (IVersionedTable)firstFC;

                            recordSessionEdits_FeatureCls(_loginWorkspace, featClsName, fSubtypes, fields, changingVersionName, parentWkspFeatureClass, childWkspFeatureClass, commonAncestorFC, versionedTable);
                        }
                        //release cursor
                        if (pCur != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCur);
                        }

                        IQueryFilter pQt = new QueryFilterClass();
                        pQt.SubFields = "FEATURECLASSNAME,FEATURESUBTYPECD,ATTRIBUTES";
                        pQt.WhereClause = "UPPER(OBJECTTYPE) = 'TABLE'";
                        ICursor pCurT = trackingFeaturesTbl.Search(pQt, true);
                        IRow pRowTbl = null;
                        int fclsIndexTbl = pCurT.FindField("FEATURECLASSNAME");
                        int subtypeIndexTbl = pCurT.FindField("FEATURESUBTYPECD");
                        int fieldsIndexTbl = pCurT.FindField("ATTRIBUTES");
                        while ((pRowTbl = pCurT.NextRow()) != null)
                        {
                            string tblName = pRowTbl.get_Value(fclsIndexTbl).ToString();
                            string[] fSubtypes = pRowTbl.get_Value(subtypeIndexTbl).ToString().Split(',');
                            string[] fields = pRowTbl.get_Value(fieldsIndexTbl).ToString().Split(',');
                            _logger.Info("Process started for : " + tblName);


                            ITable parentWkspTbl = parentsFWS.OpenTable(tblName);
                            ITable childWkspTbl = childFWS.OpenTable(tblName);
                            // Cast the common ancestor version to IFeatureWorkspace and open the table.
                            IFeatureWorkspace commonAncestorFWS = (IFeatureWorkspace)commonAncestorVersion;
                            ITable commonAncestorTbl = commonAncestorFWS.OpenTable(tblName);
                            // Cast to the IVersionedTable interface to create a difference cursor.
                            IVersionedTable versionedTbl = (IVersionedTable)childWkspTbl;                

                            recordSessionEdits_Table(_loginWorkspace, tblName, fSubtypes, fields, changingVersionName, parentWkspTbl, childWkspTbl, commonAncestorTbl, versionedTbl);
                        }
                        //release cursor
                        if (pCurT != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCurT);
                        }

                        //Inserting Session Data in Table
                        if (trackedFeatureFound)
                        {
                            ITable mmSessionTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable("PROCESS.MM_SESSION");
                            if (mmSessionTbl != null)
                            {
                                IQueryFilter sQf = new QueryFilterClass();
                                sQf.WhereClause = "SESSION_ID ='" + pPxNode.Id + "'";
                                ICursor sCur = mmSessionTbl.Search(sQf, false);
                                IRow sRw = sCur.NextRow();
                                if (sRw != null)
                                {
                                    string sessionId = pPxNode.Id.ToString();
                                    string sessionNm = sRw.get_Value(sRw.Fields.FindField("SESSION_NAME")).ToString();
                                    string sessionDesc = sRw.get_Value(sRw.Fields.FindField("DESCRIPTION")).ToString();
                                    string createdBy = sRw.get_Value(sRw.Fields.FindField("CREATE_USER")).ToString();
                                    //string postedBy = sRw.get_Value(sRw.Fields.FindField("CURRENT_OWNER")).ToString();
                                    string postedBy = getPostedBy(sessionId);
                                    //string taskname = this.ExecutingTask.Name;
                                    //if (taskname.ToUpper().Equals("POST SESSION"))
                                    //{
                                    //    postedBy = sRw.get_Value(sRw.Fields.FindField("CURRENT_OWNER")).ToString();
                                    //}
                                    //else
                                    //{
                                    //    postedBy = getPostedBy(sessionId);
                                    //}
                                    if (string.IsNullOrEmpty(postedBy))
                                    {
                                        postedBy = sRw.get_Value(sRw.Fields.FindField("CURRENT_OWNER")).ToString();
                                    }
                                    
                                    string creationDt = sRw.get_Value(sRw.Fields.FindField("CREATE_DATE")).ToString();
                                    creationDt = Convert.ToDateTime(creationDt).ToString("dd-MMM-yy hh:mm:ss");
                                    string postedDt = DateTime.Now.ToString("dd-MMM-yy hh:mm:ss");                                   

                                    string editedXML = string.Empty;
                                    if (featureClsList != null)
                                    {
                                        if (featureClsList.Count > 0)
                                        {
                                            editedXML = Serialize(featureClsList);
                                        }
                                    }
                                    // sqlInsert = "INSERT INTO " + recordSessionEditsTableName + " VALUES ('" + sessionId + "','" + sessionNm + "','" + sessionDesc + "','" + createdBy + "','" + creationDt + "','" + postedBy + "', TO_DATE('"+postedDt+"', 'dd-mon-yy hh:mi:ss') ,'" + editedXML + "')";
                                    string sqlInsert = "BEGIN PGEDATA.INSERTSESSIONDATA('" + sessionId + "','" + sessionNm + "','" + sessionDesc + "','" + createdBy + "','" + creationDt + "','" + postedBy + "','" + postedDt + "','" + editedXML + "'); END;";
                                   
                                   _loginWorkspace.ExecuteSQL(sqlInsert);
                                   featureClsList = null;
                                   editedXML = null;

                                }
                                //release cursor
                                if (sCur != null)
                                {
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
                                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(sCur);
                                    sCur = null;
                                }
                            }
                            DisplayStatusBar("Edits Captured successfully for session id - " + pPxNode.Id.ToString());
                            
                        }
                        //
                    }
                }
                if (!trackedFeatureFound)
                {
                    DisplayStatusBar("Tracked features weren't edited so Edits were not Captured for session id - " + pPxNode.Id.ToString());
                }
                trackedFeatureFound = false;

                //Deleting record from sessionCurOwnerTableName after reading current owner for this session
                string sqlDelete = "DELETE FROM " + sessionCurOwnerTableName + " WHERE SESSIONID ='" + pPxNode.Id.ToString() + "'";
                _loginWorkspace.ExecuteSQL(sqlDelete);

            }
            catch (Exception ex) 
            {
                DisplayStatusBar("Error while Capturing Edits for session id - " + pPxNode.Id.ToString());
                _logger.Info("Exception Caught : "+ex.ToString());
                trackedFeatureFound = false;
            }
            return true;
        }

        private void DisplayStatusBar(string message)
        {
            IMMArcGISRuntimeEnvironment environment = new Miner.Framework.ArcGISRuntimeEnvironment();
            if (environment != null)
            {
                environment.SetStatusBarMessage(message);
            }
        }

        //Get user id of session posted by user
        private string getPostedBy(string sessionId)
        {
            string postedBy = string.Empty;
            try
            {
                ITable sessionCurOwnerTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable(sessionCurOwnerTableName);
                IQueryFilter sQCurOwner = new QueryFilterClass();
                sQCurOwner.WhereClause = "SESSIONID ='" + sessionId + "' and DATETIME= (select max(DATETIME) from "+sessionCurOwnerTableName+" where SESSIONID ='" + sessionId + "')";
                ICursor sCurOwner = sessionCurOwnerTbl.Search(sQCurOwner, false);
                IRow sRwCurOwner = sCurOwner.NextRow();

                if (sRwCurOwner != null)
                {
                    postedBy = sRwCurOwner.get_Value(sRwCurOwner.Fields.FindField("CURRENTOWNER")).ToString();                   
                     
                }
                if (sCurOwner != null) { Marshal.ReleaseComObject(sCurOwner); }
                

                //ITable mmSessionHstryTbl = ((IFeatureWorkspace)_loginWorkspace).OpenTable("PROCESS.MM_PX_HISTORY");
                //IQueryFilter sQCurOwner = new QueryFilterClass();
                //sQCurOwner.WhereClause = "NODE_ID ='" + sessionId + "' and DESCRIPTION like 'Session owner changed from%' and DATE_TIME= (select max(DATE_TIME) from PROCESS.MM_PX_HISTORY where NODE_ID ='" + sessionId + "' and DESCRIPTION like 'Session owner changed from%')";
                //ICursor sCurOwner = mmSessionHstryTbl.Search(sQCurOwner, false);
                //IRow sRwCurOwner = sCurOwner.NextRow();

                //if (sRwCurOwner != null)
                //{
                //    string desc = sRwCurOwner.get_Value(sRwCurOwner.Fields.FindField("DESCRIPTION")).ToString();
                //    //Session owner changed from "GIS_I"  to "akmb".
                //    desc = desc.Substring(27);
                //    string[] post = desc.Split(' ');
                //    postedBy = post[0].Trim(new Char[] { ' ', '"' }).ToString();
                //}
                //if (sCurOwner != null) { Marshal.ReleaseComObject(sCurOwner); }
            }
            catch (Exception ex) { _logger.Info("Error in method getPostedBy" + ex.ToString()); }
            return postedBy;
        }
                
        public bool Initialize(IMMPxApplication pPxApp)
        {
            _PxApp = pPxApp;
            if (_PxApp != null)
            {
                return true;
            }
            else return false;
        }

        public string Name
        {
            get { return "PGE Attribute Change Tracking SubTask"; }
        }

        public bool Rollback(IMMPxNode pPxNode)
        {
            throw new NotImplementedException();
        }

        
        //get workspace
        private static IWorkspace GetWorkspace()
        {
            _loginWorkspace = ((IMMPxApplicationEx2)_PxApp).Workspace;
            
            return _loginWorkspace;
        }
        //record session edits for feature class
        private static void recordSessionEdits_FeatureCls(IWorkspace wksp, string featureClsName, string[] subtypecd, string[] fields, string changingSessionId,IFeatureClass ParentWkspFeatureClass, IFeatureClass ChildWkspFeatureClass, ITable CommonAncestorFC, IVersionedTable VersionedTable)
        {
            string changingVersion = changingSessionId;
            FindVersionDifferences_FeatureClass(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeUpdateNoChange, ParentWkspFeatureClass,  ChildWkspFeatureClass,  CommonAncestorFC, VersionedTable);
            FindVersionDifferences_FeatureClass(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeInsert, ParentWkspFeatureClass, ChildWkspFeatureClass, CommonAncestorFC, VersionedTable);
            FindVersionDifferences_FeatureClass(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeDeleteNoChange, ParentWkspFeatureClass, ChildWkspFeatureClass, CommonAncestorFC, VersionedTable);

        }

        //record session edits for Table
        public static void FindVersionDifferences_FeatureClass(IWorkspace workspace, String changingVersionName, String referenceVersionName, String featureclassName, String[] subtypecd, String[] fields, esriDifferenceType differenceType, IFeatureClass ParentWkspFeatureClass, IFeatureClass ChildWkspFeatureClass, ITable CommonAncestorFC, IVersionedTable VersionedTable)
        {
            #region  private variables

            IDifferenceCursor differenceCursor = null;           
            string objectId = string.Empty;
            #endregion

            try
            {                
                differenceCursor = VersionedTable.Differences(CommonAncestorFC, differenceType, null);

                IRow differenceRow = null;
                int objectID = -1;
                bool differenceFound = false;
                // Step through the cursor, showing the ID of each modified row.
                differenceCursor.Next(out objectID, out differenceRow);

                
                while (objectID != -1)
                {
                    differenceFound = true;                    
                        
                    //foreach (string subtype in subtypecd)
                    //{
                        //if(subtype.Equals(subtypeCode))
                        //{
                            trackedFeatureFound = true;

                            #region for difference type Delete
                            if (differenceType == esriDifferenceType.esriDifferenceTypeDeleteNoChange)
                            {
                                if (differenceRow == null)
                                {
                                    differenceRow = CommonAncestorFC.GetRow(objectID);

                                    int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                                    string subtypeCode = "None";
                                    if (indexSubtype != -1)
                                    {
                                        subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                                    }
                                    foreach (string subtype in subtypecd)
                                    {
                                        if (subtype.Equals(subtypeCode))
                                        {
                                            FEATURE fClsObj = new FEATURE();
                                            featAttributeList = new List<ATTRIBUTE>();
                                            fClsObj.featureClsNm = featureclassName;
                                            if (indexSubtype != -1)
                                            {
                                                fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                            }
                                            else
                                            {
                                                fClsObj.featureSubType = "None";
                                            }
                                
                                            fClsObj.featureOID = differenceRow.OID.ToString();
                                            fClsObj.featureEditType = "DELETE";
                                            fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();

                                            foreach (string field in fields)
                                            {
                                                //Value in current session is null b/c feature is deleted
                                                string FldNewValue = string.Empty;
                                                //Value in Default Version b/c this feature is available in the default version                               
                                                string FldOldValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();

                                                ATTRIBUTE featAtrObj = new ATTRIBUTE();
                                                
                                                featAtrObj.FieldName = field;
                                                featAtrObj.OldValue = FldOldValue;
                                                featAtrObj.NewValue = FldNewValue;
                                                featAttributeList.Add(featAtrObj);
                                            }
                                            fClsObj.fieldValue = featAttributeList;
                                            featureClsList.Add(fClsObj);
                                            featAttributeList = null;
                                        }
                                    }
                                }

                            }
                            #endregion

                            #region for difference type update
                            else if (differenceType == esriDifferenceType.esriDifferenceTypeUpdateNoChange)
                            {
                                int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                                string subtypeCode = "None";
                                if (indexSubtype != -1)
                                {
                                    subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                                }
                                foreach (string subtype in subtypecd)
                                {
                                    if (subtype.Equals(subtypeCode))
                                    {                                        
                                        FEATURE fClsObj = new FEATURE();
                                        featAttributeList = new List<ATTRIBUTE>();
                                        fClsObj.featureClsNm = featureclassName;
                                        if (indexSubtype != -1)
                                        {
                                            fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                        }
                                        else
                                        {
                                            fClsObj.featureSubType = "None";
                                        }
                                
                                        fClsObj.featureOID = differenceRow.OID.ToString();
                                        fClsObj.featureEditType = "UPDATE";
                                        fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();

                                        foreach (string field in fields)
                                        {
                                            //Value in current session
                                            // FldNewValue = differenceRow.get_Value(differenceRow.Fields.FindField(field)).ToString();
                                             string FldNewValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();
                                            //Value in Default Version                                
                                            string FldOldValue = GetValueofAttribute_Feature(ParentWkspFeatureClass, field, "OBJECTID='" + differenceRow.OID.ToString() + "'");

                                            ATTRIBUTE featAtrObj = new ATTRIBUTE();
                                            
                                            featAtrObj.FieldName = field;
                                            featAtrObj.OldValue = FldOldValue;
                                            featAtrObj.NewValue = FldNewValue;
                                            featAttributeList.Add(featAtrObj);
                                        }
                                        fClsObj.fieldValue = featAttributeList;
                                        featureClsList.Add(fClsObj);
                                        featAttributeList = null;                                        
                                    }
                                }

                            }
                            #endregion

                            #region for difference type insert
                            else if (differenceType == esriDifferenceType.esriDifferenceTypeInsert)
                            {
                                int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                                string subtypeCode = "None";
                                if (indexSubtype != -1)
                                {
                                    subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                                }
                                foreach (string subtype in subtypecd)
                                {
                                    if (subtype.Equals(subtypeCode))
                                    {
                                        FEATURE fClsObj = new FEATURE();
                                        featAttributeList = new List<ATTRIBUTE>();
                                        fClsObj.featureClsNm = featureclassName;
                                        if (indexSubtype != -1)
                                        {
                                            fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                        }
                                        else
                                        {
                                            fClsObj.featureSubType = "None";
                                        }
                                
                                        fClsObj.featureOID = differenceRow.OID.ToString();
                                        fClsObj.featureEditType = "INSERT";
                                        fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();

                                        foreach (string field in fields)
                                        {
                                            //Value in current session
                                            string FldNewValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();
                                            //Value in Default Version b/c this is new feature created in the current version                               
                                            string FldOldValue = string.Empty;

                                            ATTRIBUTE featAtrObj = new ATTRIBUTE();
                                            
                                            featAtrObj.FieldName = field;
                                            featAtrObj.OldValue = FldOldValue;
                                            featAtrObj.NewValue = FldNewValue;
                                            featAttributeList.Add(featAtrObj);
                                        }
                                        fClsObj.fieldValue = featAttributeList;
                                        featureClsList.Add(fClsObj);
                                        featAttributeList = null;
                                    }
                                }

                            }
                            #endregion
                       
                    differenceCursor.Next(out objectID, out differenceRow);

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }
               
        public static string GetValueofAttribute_Feature(IFeatureClass fc, string attributename, string whereclause)
        {
            #region Private Variables
            string value = "";
            IFeatureCursor featurecursor = null;
            IFeature tmpfeature = null;
            #endregion
            IQueryFilter queryfilter = new QueryFilterClass();
            queryfilter.WhereClause = whereclause;
            try
            {
                attributename = attributename.Trim();
                featurecursor = fc.Search(queryfilter, true);
                tmpfeature = featurecursor.NextFeature();
                if (tmpfeature != null)
                {
                    if (tmpfeature.Fields.FindField(attributename) == -1)
                        return "";
                    if (tmpfeature.get_Value(tmpfeature.Fields.FindField(attributename)) == DBNull.Value)
                        return "";
                    //return tmpfeature.get_Value(tmpfeature.Fields.FindField(attributename)).ToString();
                    return ReadFieldValue(tmpfeature as IObject, tmpfeature.Fields.FindField(attributename), false).ToString(); 
                }
            }
            catch (Exception exp)
            {
                //Logger.Error("GetValueofAttribute()" + exp.Message.ToString());
            }
            finally
            {
                if (featurecursor != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(featurecursor);
            }
            return value;

        }

        //record session edits for feature class
        private static void recordSessionEdits_Table(IWorkspace wksp,string featureClsName,string[] subtypecd,string[] fields,string changingVersion,ITable parentWkspTbl,ITable childWkspTbl,ITable commonAncestorTbl,IVersionedTable versionedTbl)
        {            
            FindVersionDifferences_Table(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeUpdateNoChange, parentWkspTbl, childWkspTbl, commonAncestorTbl, versionedTbl);
            FindVersionDifferences_Table(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeInsert, parentWkspTbl, childWkspTbl, commonAncestorTbl, versionedTbl);
            FindVersionDifferences_Table(wksp, changingVersion, "SDE.DEFAULT", featureClsName, subtypecd, fields, esriDifferenceType.esriDifferenceTypeDeleteNoChange, parentWkspTbl, childWkspTbl, commonAncestorTbl, versionedTbl);

        }

        public static void FindVersionDifferences_Table(IWorkspace workspace, String changingVersionName, String referenceVersionName, String TableName, String[] subtypecd, String[] fields, esriDifferenceType differenceType,  ITable parentWkspTbl, ITable childWkspTbl, ITable commonAncestorTbl, IVersionedTable versionedTable)
        {
            #region  private variables

            IDifferenceCursor differenceCursor = null;
            //ITable firstFC = null;
            string objectId = string.Empty;
            #endregion

            try
            {                
                differenceCursor = versionedTable.Differences(commonAncestorTbl, differenceType, null);
                IRow differenceRow = null;
                int objectID = -1;
                // end Code 
                // Step through the cursor, showing the ID of each modified row.
                differenceCursor.Next(out objectID, out differenceRow);
                bool differenceFound = false;
                
                while (objectID != -1)
                {
                    differenceFound = true;

                    trackedFeatureFound = true;

                    #region for difference type Delete
                    if (differenceType == esriDifferenceType.esriDifferenceTypeDeleteNoChange)
                    {
                        if (differenceRow == null)
                        {
                            differenceRow = commonAncestorTbl.GetRow(objectID);

                            int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                            string subtypeCode = "None";
                            if (indexSubtype != -1)
                            {
                                subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                            }
                            foreach (string subtype in subtypecd)
                            {
                                if (subtype.Equals(subtypeCode))
                                {
                                    FEATURE fClsObj = new FEATURE();
                                    featAttributeList = new List<ATTRIBUTE>();
                                    fClsObj.featureClsNm = TableName;
                                    if (indexSubtype != -1)
                                    {
                                        fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                    }
                                    else
                                    {
                                        fClsObj.featureSubType = "None";
                                    }
                                
                                    fClsObj.featureOID = differenceRow.OID.ToString();
                                    fClsObj.featureEditType = "DELETE";
                                    fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();

                                    foreach (string field in fields)
                                    {
                                        //Value in current session is null b/c feature is deleted
                                        string FldNewValue = string.Empty;
                                        //Value in Default Version b/c this feature is available in the default version                               
                                        string FldOldValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();

                                        ATTRIBUTE featAtrObj = new ATTRIBUTE();

                                        featAtrObj.FieldName = field;
                                        featAtrObj.OldValue = FldOldValue;
                                        featAtrObj.NewValue = FldNewValue;
                                        featAttributeList.Add(featAtrObj);
                                    }
                                    fClsObj.fieldValue = featAttributeList;
                                    featureClsList.Add(fClsObj);
                                    featAttributeList = null;
                                }
                            }
                        }

                    }
                    #endregion

                    #region for difference type update
                    else if (differenceType == esriDifferenceType.esriDifferenceTypeUpdateNoChange)
                    {
                        int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                        string subtypeCode = "None";
                        if (indexSubtype != -1)
                        {
                            subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                        }
                        foreach (string subtype in subtypecd)
                        {
                            if (subtype.Equals(subtypeCode))
                            {
                                FEATURE fClsObj = new FEATURE();
                                featAttributeList = new List<ATTRIBUTE>();
                                fClsObj.featureClsNm = TableName;
                                if (indexSubtype != -1)
                                {
                                    fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                }
                                else
                                {
                                    fClsObj.featureSubType = "None";
                                }
                                
                                fClsObj.featureOID = differenceRow.OID.ToString();
                                fClsObj.featureEditType = "UPDATE";
                                fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();

                                
                                foreach (string field in fields)
                                {
                                    //Value in current session
                                    string FldNewValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();
                                    //Value in Default Version                                
                                    string FldOldValue = GetValueofAttribute_Table(parentWkspTbl, field, "OBJECTID='" + differenceRow.OID.ToString() + "'");

                                    ATTRIBUTE featAtrObj = new ATTRIBUTE();

                                    featAtrObj.FieldName = field;
                                    featAtrObj.OldValue = FldOldValue;
                                    featAtrObj.NewValue = FldNewValue;
                                    featAttributeList.Add(featAtrObj);
                                }
                                fClsObj.fieldValue = featAttributeList;
                                featureClsList.Add(fClsObj);
                                featAttributeList = null;
                            }
                        }

                    }
                    #endregion

                    #region for difference type insert
                    else if (differenceType == esriDifferenceType.esriDifferenceTypeInsert)
                    {
                        int indexSubtype = differenceRow.Fields.FindField("SUBTYPECD");
                        string subtypeCode = "None";
                        if (indexSubtype != -1)
                        {
                            subtypeCode = differenceRow.get_Value(differenceRow.Fields.FindField("SUBTYPECD")).ToString();
                        }
                        foreach (string subtype in subtypecd)
                        {
                            if (subtype.Equals(subtypeCode))
                            {
                                FEATURE fClsObj = new FEATURE();
                                featAttributeList = new List<ATTRIBUTE>();
                                fClsObj.featureClsNm = TableName;
                                if (indexSubtype != -1)
                                {
                                    fClsObj.featureSubType = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField("SUBTYPECD"), false).ToString();
                                }
                                else
                                {
                                    fClsObj.featureSubType = "None";
                                }
                                
                                fClsObj.featureOID = differenceRow.OID.ToString();
                                fClsObj.featureEditType = "INSERT";
                                fClsObj.featureGlobalid = differenceRow.get_Value(differenceRow.Fields.FindField("GLOBALID")).ToString();


                                foreach (string field in fields)
                                {
                                    //Value in current session
                                    string FldNewValue = ReadFieldValue(differenceRow as IObject, differenceRow.Fields.FindField(field), false).ToString();
                                    //Value in Default Version b/c this is new feature created in the current version                               
                                    string FldOldValue = string.Empty;

                                    ATTRIBUTE featAtrObj = new ATTRIBUTE();

                                    featAtrObj.FieldName = field;
                                    featAtrObj.OldValue = FldOldValue;
                                    featAtrObj.NewValue = FldNewValue;
                                    featAttributeList.Add(featAtrObj);
                                }
                                fClsObj.fieldValue = featAttributeList;
                                featureClsList.Add(fClsObj);
                                featAttributeList = null;
                            }
                        }

                    }
                    #endregion

                    differenceCursor.Next(out objectID, out differenceRow);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Get value in table for whereclause
        public static string GetValueofAttribute_Table(ITable tbl, string attributename, string whereclause)
        {
            #region Private Variables
            string value = "";
            ICursor tblcursor = null;
            IRow tmpRow = null;
            #endregion
            IQueryFilter queryfilter = new QueryFilterClass();
            queryfilter.WhereClause = whereclause;
            try
            {
                attributename = attributename.Trim();
                tblcursor = tbl.Search(queryfilter, true);
                tmpRow = tblcursor.NextRow();
                if (tmpRow != null)
                {
                    if (tmpRow.Fields.FindField(attributename) == -1)
                        return "";
                    if (tmpRow.get_Value(tmpRow.Fields.FindField(attributename)) == DBNull.Value)
                        return "";
                   // return tmpRow.get_Value(tmpRow.Fields.FindField(attributename)).ToString();
                    return ReadFieldValue(tmpRow as IObject, tmpRow.Fields.FindField(attributename), false).ToString(); 
                }
            }
            catch (Exception exp)
            {
                //Logger.Error("GetValueofAttribute()" + exp.Message.ToString());
            }
            finally
            {
                if (tblcursor != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(tblcursor);
            }
            return value;

        }
        
        //XML Serialization of FEATURECLASS.cs
        private static string Serialize(List<FEATURE> details)
        {
            StringWriter sw;
            XmlSerializer serializer = new XmlSerializer(typeof(List<FEATURE>));
            using (sw = new StringWriter())
            {
                serializer.Serialize(sw, details);
            }

            return sw.ToString();
        }

        private static object ReadFieldValue(IObject obj, int fieldIndex, bool ignoreDescription = false)
        {
            object fieldValue = null;

            if (obj != null && fieldIndex > -1)
            {
                /// Default to the field value first,
                /// overwrite it if a subtype or domain
                /// is found.
                fieldValue = obj.Value[fieldIndex];
                if (fieldValue != DBNull.Value && ignoreDescription == false)
                {
                    ISubtypes subtypes = obj.Class as ISubtypes;
                    IRowSubtypes rowSubtypes = obj as IRowSubtypes;
                    IField2 field = obj.Fields.Field[fieldIndex] as IField2;

                    // Set the appropriate domain based on the subtype if a subtype exists.
                    IDomain domain = null;
                    if (field != null)
                    {
                        if (subtypes != null && subtypes.SubtypeFieldIndex > -1)
                        {
                            domain = subtypes.get_Domain(rowSubtypes.SubtypeCode, field.Name);
                        }
                        else
                        {
                            domain = field.Domain;
                        }
                    }

                    // Check for a subtype field first
                    if (subtypes != null && rowSubtypes != null && subtypes.SubtypeFieldIndex == fieldIndex)
                        fieldValue = subtypes.get_SubtypeName(rowSubtypes.SubtypeCode);
                    // If it is not a subtype field, check for a domain field
                    else if (domain != null)
                    {
                        ICodedValueDomain2 cvDomain = domain as ICodedValueDomain2;
                        if (cvDomain != null)
                        {
                            for (int i = 0; i < cvDomain.CodeCount; i++)
                            {
                                if (cvDomain.get_Value(i).Equals(fieldValue))
                                {
                                    fieldValue = cvDomain.get_Name(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return fieldValue;
        }
        

        public IDictionary Parameters
        {
            set { _Parameters = value; }
        }

        public IMMEnumExtensionNames SupportedExtensions
        {
            get
            {
                if (m_pPxExtNames == null)
                {
                    m_pPxExtNames = new PxExtensionNamesClass();
                    m_pPxExtNames.Add("MMSessionManager");
                    _SupportedExtensions = m_pPxExtNames;
                }
                return _SupportedExtensions;
            }
        }

        public IDictionary SupportedParameterNames
        {
            get { return _SupportedParameters; }
        }

        /// <summary>
        /// Sets the task.
        /// </summary>
        /// <value>The task.</value>
        public IMMPxTask Task
        {
            set { _Task = value; }
        }

        public IMMPxTask ExecutingTask
        {
            get { return _Task; }
        }

    }
}
