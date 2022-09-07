using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Collections.Generic;
using System.Configuration;
using ESRI.ArcGIS.Geometry;
using System.Linq;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace MoveFeatures
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new MoveFeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ".log";
        private static string sGlobalIDListPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\GlobalID_List_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ".csv";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static StreamWriter pGlobalIDListWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();

        private static string strSwitchGlobalIDs = string.Empty;

        /// <summary>
        /// Main Method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            (pSWriter = File.CreateText(sPath)).Close();
            (pGlobalIDListWriter = File.CreateText(sGlobalIDListPath)).Close();
            WriteCSV("DPD OBJECTID, DPD GLOBALID, SWITCH OBJECTID, SWITCH GLOBALID");

            List<string> issueList = new List<string>();

            Console.WriteLine("----Move Features from FC's-----");

            //string issueNumber = Console.ReadLine(); // Read string from console                

            //Console.WriteLine("\n");
            Console.WriteLine("\n");

            string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            string strSdeVerName = ConfigurationManager.AppSettings["SESSION_NAME"].ToString();
            string strFromFCName = ConfigurationManager.AppSettings["FROM_FEATURE_CLASS"].ToString();
            string strToFCName = ConfigurationManager.AppSettings["TO_FEATURE_CLASS"].ToString();
            string strWhereCondition = ConfigurationManager.AppSettings["WHERE_CONDITION"].ToString();
            Console.WriteLine("Sde Connection Parameter : " + strEDGISSdeConn);
            Console.WriteLine("Sde Version Name Parameter : " + strSdeVerName);
            Console.WriteLine("From Featureclass : " + strFromFCName);
            Console.WriteLine("To Featureclass : " + strToFCName);
            Console.WriteLine("Where Condition : " + strWhereCondition);

            Console.WriteLine("Please confirm all above important details, before proceed (Y/N):");
            string strConfirm = Console.ReadLine().ToUpper(); // Read string from console

            if (strConfirm == "N") // Try to parse the string as an integer
            {
                Console.Write("Change sde connection string in configuration file and try again");
                return;
            }
            else if (strConfirm != "N" && strConfirm != "Y") // Try to parse the string as an integer
            {
                Console.WriteLine("invalid input value, plz try again");
                return;
            }

            //ESRI License Initializer generated code.
            WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            mmLicenseStatus licenseStatus = InitializeLicenses(mmLicensedProductCode.mmLPArcFM);
            if (RuntimeManager.ActiveRuntime == null)
                RuntimeManager.BindLicense(ProductCode.Desktop);
            //Process feature class wise

            List<string> lstFCNames = new System.Collections.Generic.List<string>();
         
            MoveFeaturesReverseOrder();

            WriteLine(Environment.NewLine + DateTime.Now.ToLongTimeString() + " -- Execution completed successfully.");
        }

        /// <summary>
        /// Write message to log file
        /// </summary>
        /// <param name="sMsg">Message</param>
        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        /// <summary>
        /// Write CSV file contentC
        /// </summary>
        /// <param name="sMsg">Comma separated values</param>
        private static void WriteCSV(string sMsg)
        {
            pGlobalIDListWriter = File.AppendText(sGlobalIDListPath);
            pGlobalIDListWriter.WriteLine(sMsg);
            pGlobalIDListWriter.Close();
        }

        /// <summary>
        /// Initialize Licenses
        /// </summary>
        /// <param name="productCode">Product Code</param>
        /// <returns></returns>
        private static mmLicenseStatus InitializeLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = arcFMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }


        /// <summary>
        /// Move Features
        /// </summary>
        private static void MoveFeaturesReverseOrder()
        {
            IVersion pVersion = default(IVersion);
            IFeature pFeat = default(IFeature);
            IFeatureCursor pFCursor = default(IFeatureCursor);
            IQueryFilter pQFilter = default(IQueryFilter);
            IQueryFilter _queryFilter = default(IQueryFilter);
            IFeatureClass pFromFClass = default(IFeatureClass);
            IFeatureClass pToFClass = default(IFeatureClass);
            ITable pTable = default(ITable);
            //IFeature pFeat = default(IFeature);
            IFeatureCursor pCursor = default(IFeatureCursor);

            //List<string> lstTargetFCNames = new System.Collections.Generic.List<string>();
            IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
            mmAutoUpdaterMode oldMode;
            string strLastUser = string.Empty, strVersionName = string.Empty;

            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);

                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(ConfigurationManager.AppSettings["SESSION_NAME"]);
                string pFromFCName = ConfigurationManager.AppSettings["FROM_FEATURE_CLASS"];
                string pToFCName = ConfigurationManager.AppSettings["TO_FEATURE_CLASS"];
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();

                pFromFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFromFCName);
                pToFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pToFCName);
                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started moving features from " + pFromFCName + " to " + pToFCName);
                //To Write change records to GIS-SAP Staging table....

                string strGUIDValue = string.Empty;
                _queryFilter = new QueryFilter();
                string whereCondition = ConfigurationManager.AppSettings["WHERE_CONDITION"];//"SUBTYPECD = 8 AND DEVICETYPE = 'SCAD'";

             
                _queryFilter.WhereClause = whereCondition;
                pCursor = pFromFClass.Search(_queryFilter, false);
                int iCount = 0;
                string strDPDGlobalIDs = string.Empty;

                //if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                //{
                int indexGlobalID = pFromFClass.Fields.FindField("GLOBALID");

                WriteLine(DateTime.Now.ToLongTimeString() + " Getting relationship details of " + pFromFCName + " featureclass");
                //Get relationship details of FROM featureclass
                IEnumRelationshipClass enumFromRelationshipClass = null;
                List<RelationShipDetails> fromRelationships = new List<RelationShipDetails>();
                IDataset fromDataset, toDataset;
                enumFromRelationshipClass = pFromFClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                IRelationshipClass fromRelationshipClass = enumFromRelationshipClass.Next();
                while (fromRelationshipClass != null)
                {
                    fromDataset = (IDataset)fromRelationshipClass.DestinationClass;
                    fromRelationships.Add(new RelationShipDetails
                    {
                        ClassName = fromDataset.Name,
                        ClassType = fromDataset.Type,
                        ObjectClass = fromRelationshipClass.DestinationClass,// fromDataset,
                        OriginPrimaryKey = fromRelationshipClass.OriginPrimaryKey,
                        OriginForeignKey = fromRelationshipClass.OriginForeignKey
                    });

                    fromRelationshipClass = enumFromRelationshipClass.Next();
                }

                WriteLine(DateTime.Now.ToLongTimeString() + " Getting relationship details of " + pToFCName + " featureclass");
                //Get relationship details of TO featureclass
                IEnumRelationshipClass enumToRelationshipClass = null;
                List<RelationShipDetails> toRelationships = new List<RelationShipDetails>();
                enumToRelationshipClass = pToFClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                IRelationshipClass toRelationshipClass = enumToRelationshipClass.Next();
                while (toRelationshipClass != null)
                {
                    toDataset = (IDataset)toRelationshipClass.DestinationClass;
                    toRelationships.Add(new RelationShipDetails
                    {
                        ClassName = toDataset.Name,
                        ClassType = toDataset.Type,
                        ObjectClass = toRelationshipClass.DestinationClass,// toDataset,
                        OriginPrimaryKey = toRelationshipClass.OriginPrimaryKey,
                        OriginForeignKey = toRelationshipClass.OriginForeignKey
                    });

                    toRelationshipClass = enumToRelationshipClass.Next();
                }

                //string globalIDsForSymbol_16 = string.Empty;
                //string globalIDsForSymbol_32 = string.Empty;
                while ((pFeat = pCursor.NextFeature()) != null)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + " Processing OID: " + pFeat.OID);

                    string dpdGlobalID = pFeat.get_Value(indexGlobalID).ToString();
                    if (string.IsNullOrEmpty(strDPDGlobalIDs))
                        strDPDGlobalIDs += "'" + dpdGlobalID + "'";
                    else
                        strDPDGlobalIDs += ",'" + dpdGlobalID + "'";


                    object FLISRAutomationDeviceIdc = null;
                    var fromRelation = fromRelationships.Where(w => w.ClassName.Equals("EDGIS.SCADA")).FirstOrDefault();
                    FLISRAutomationDeviceIdc = GetFieldValue((ITable)fromRelation.ObjectClass, "FLISRAUTOMATIONDEVICEIDC", fromRelation.OriginForeignKey + " = '" + dpdGlobalID + "'");

                    Dictionary<int, object> fieldValues = new Dictionary<int, object>();

                    for (int i = 1; i < pFeat.Fields.FieldCount; i++)
                    {
                        IFeatureClass sourceFeatureClass = (IFeatureClass)pFeat.Class;

                        string fieldName = pFeat.Fields.Field[i].Name;
                        bool bCondition1 = fieldName == sourceFeatureClass.ShapeFieldName;
                        bool bCondition2 = (sourceFeatureClass.LengthField != null &&
                                            fieldName == sourceFeatureClass.LengthField.Name);
                        bool bCondition3 = (sourceFeatureClass.AreaField != null &&
                                            fieldName == sourceFeatureClass.AreaField.Name);

                        // Don't do shape fields
                        if (!(bCondition1 || bCondition2 || bCondition3))
                        {
                            int myTargetFieldId = pToFClass.FindField(fieldName);
                            if (myTargetFieldId > -1 && pToFClass.Fields.Field[myTargetFieldId].Editable)
                            {
                                if (!fieldName.Equals("SUBTYPECD") && !fieldName.Equals("TIESWITCHIDC"))
                                {
                                    fieldValues.Add(myTargetFieldId, pFeat.Value[i]);
                                }
                            }
                        }
                    }
                    IGeometry newShape = pFeat.ShapeCopy;


                    Relation rel = new Relation();
                    IList<Relation> relList = new List<Relation>();
                    string gblId = string.Empty;
                    string fldName = "GLOBALID";
                    //Get relationship details
                    foreach (var fromRel in fromRelationships)
                    {
                        foreach (var toRel in toRelationships)
                        {
                            gblId = string.Empty;
                            if (fromRel.ClassName.Equals(toRel.ClassName))
                            {
                                //WriteLine(DateTime.Now.ToLongTimeString() + " Setting relationship - Table: " + fromRel.Dataset.Name + " Field: " + toRel.OriginForeignKey + " Value: " + switchGlobalID + " Where: " + fromRel.OriginForeignKey + " = '" + dpdGlobalID + "'");

                                gblId = Convert.ToString(GetFieldValue((ITable)fromRel.ObjectClass, fldName, fromRel.OriginForeignKey + " = '" + dpdGlobalID + "'"));

                                if (!string.IsNullOrEmpty(gblId))
                                {
                                    rel = new Relation();
                                    rel.ClassName = toRel.ClassName;
                                    rel.FieldName = fldName;
                                    rel.Value = gblId;

                                    relList.Add(rel);
                                }
                            }
                        }
                    }


                    //Disconnect feature
                    WriteLine(DateTime.Now.ToLongTimeString() + " Disconnecting DPD Feature: " + pFeat.OID);
                    System.Type typeAU = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                    IMMAutoUpdater mmAutoUpdater = (IMMAutoUpdater)System.Activator.CreateInstance(typeAU);
                    INetworkFeature networkFeature = (INetworkFeature)pFeat;
                    //networkFeature.Disconnect();
                    mmAutoUpdater.FeatureDisconnected(networkFeature);

                    //Deleting DPD feature
                    WriteLine(DateTime.Now.ToLongTimeString() + " Deleting DPD feature: " + pFeat.OID);
                    pFeat.Delete();

                    int swithOID;
                    //Copy feature to SWITCH feature class and get new GLOBALID
                    //string switchGlobalID = InsertFeature(pToFClass, pFeat, out swithOID);
                    IFeature newSwitch = InsertFeature(pToFClass, dpdGlobalID, FLISRAutomationDeviceIdc, newShape, fieldValues, fromRelationships, toRelationships, relList, out swithOID);

                    string switchGlobalID = newSwitch.get_Value(pToFClass.FindField("GLOBALID")).ToString();


                    //Write OID and GLOBALID into CSV
                    WriteCSV(pFeat.OID + "," + dpdGlobalID + "," + swithOID + "," + switchGlobalID);
                   iCount++;
                 
                }
              

                WriteLine(Environment.NewLine + DateTime.Now.ToLongTimeString() + " Total " + pFromFCName + " features processed : " + iCount);

              
                WriteLine(Environment.NewLine + DateTime.Now.ToLongTimeString() + " Old DPD Global IDs: " + Environment.NewLine + strDPDGlobalIDs);
                WriteLine(Environment.NewLine + DateTime.Now.ToLongTimeString() + " New Switch Global IDs: " + Environment.NewLine + strSwitchGlobalIDs);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features: " + ex.Message);
            }
            finally
            {
                ((IWorkspaceEdit)pVersion).StopEditOperation();
                ((IWorkspaceEdit)pVersion).StopEditing(true);
                m_AOLicenseInitializer.ShutdownApplication();
                GC.Collect();

                if (pVersion != null) Marshal.ReleaseComObject(pVersion);
                if (pFeat != null) Marshal.ReleaseComObject(pFeat);
                if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (_queryFilter != null) Marshal.ReleaseComObject(_queryFilter);
                if (pFromFClass != null) Marshal.ReleaseComObject(pFromFClass);
                if (pToFClass != null) Marshal.ReleaseComObject(pToFClass);
                if (pTable != null) Marshal.ReleaseComObject(pTable);
                //if (pRow != null) Marshal.ReleaseComObject(pRow);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
                //if (autoupdater != null) Marshal.ReleaseComObject(autoupdater);
                //if (oldMode != null) Marshal.ReleaseComObject(oldMode);
            }
        }

       
        /// <summary>
        /// Insert new feature to the featureclass
        /// </summary>
        /// <param name="targetFeatureClass"></param>
        /// <param name="dpdGlobalID"></param>
        /// <param name="FLISRAutomationDeviceIdc"></param>
        /// <param name="shape"></param>
        /// <param name="fieldValues"></param>
        /// <param name="newOID"></param>
        /// <returns></returns>
        private static IFeature InsertFeature(IFeatureClass targetFeatureClass, string dpdGlobalID, object FLISRAutomationDeviceIdc, IGeometry shape, Dictionary<int, object> fieldValues, List<RelationShipDetails> fromRelationships, List<RelationShipDetails> toRelationships, IList<Relation> relList, out int newOID)
        {
            IFeature newfeature = targetFeatureClass.CreateFeature();
            //var simplifiedFeature = newfeature as IFeatureSimplify;
            IGeometry myGeometry = shape;
            //simplifiedFeature.SimplifyGeometry(myGeometry);
            newfeature.Shape = myGeometry;

            //foreach (var fieldVal in fieldValues)
            //{
            //    newfeature.set_Value(fieldVal.Key, fieldVal.Value);
            //}

            newfeature.set_Value(targetFeatureClass.FindField("SUBTYPECD"), 6);//SCADAMATESwitch
            newfeature.set_Value(targetFeatureClass.FindField("SWITCHTYPE"), 18);//SCADA Switch
            newfeature.set_Value(targetFeatureClass.FindField("SECTIONALIZINGFEATURE"), "CTIN");//Cut-in

            newfeature.set_Value(targetFeatureClass.FindField("ATTACHMENTTYPE"), 0);//Not Applicable
            newfeature.set_Value(targetFeatureClass.FindField("GANGOPERATED"), "Y");//Yes

            string switchGlobalID = newfeature.get_Value(targetFeatureClass.FindField("GLOBALID")).ToString();
            if (string.IsNullOrEmpty(strSwitchGlobalIDs))
                strSwitchGlobalIDs += "'" + switchGlobalID + "'";
            else
                strSwitchGlobalIDs += ",'" + switchGlobalID + "'";

            //Connect feature
            WriteLine(DateTime.Now.ToLongTimeString() + " Connecting DPD Feature: " + newfeature.OID);
            System.Type typeAU = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater mmAutoUpdater = (IMMAutoUpdater)System.Activator.CreateInstance(typeAU);
            INetworkFeature networkFeature = (INetworkFeature)newfeature;
            //networkFeature.Connect();
            mmAutoUpdater.FeatureConnected(networkFeature);


            var symbol = newfeature.get_Value(targetFeatureClass.FindField("SYMBOLNUMBER"));

            WriteLine(DateTime.Now.ToLongTimeString() + " Copying DPD feature " + dpdGlobalID + " to SWITCH " + switchGlobalID);
            newfeature.Store();

            var symbolN = newfeature.get_Value(targetFeatureClass.FindField("SYMBOLNUMBER"));

            foreach (var fieldVal in fieldValues)
            {
                if (string.IsNullOrEmpty(Convert.ToString(newfeature.Value[fieldVal.Key])))
                {
                    newfeature.set_Value(fieldVal.Key, fieldVal.Value);
                }
            }

            if (FLISRAutomationDeviceIdc != null && FLISRAutomationDeviceIdc.ToString().Equals("Y"))
            {
                newfeature.set_Value(targetFeatureClass.FindField("SYMBOLNUMBER"), 16);
            }
            else
            {
                newfeature.set_Value(targetFeatureClass.FindField("SYMBOLNUMBER"), 32);
            }

            newOID = newfeature.OID;


            //Update relationship tables
            foreach (var fromRel in fromRelationships)
            {
                foreach (var toRel in toRelationships)
                {
                    if (fromRel.ClassName.Equals(toRel.ClassName))
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + " Setting relationship - Table: " + fromRel.ClassName + " Field: " + toRel.OriginForeignKey + " Value: " + switchGlobalID + " Where: " + fromRel.OriginForeignKey + " = '" + dpdGlobalID + "'");

                        var rel = relList.Where(r => r.ClassName.Equals(toRel.ClassName)).FirstOrDefault();
                        if (rel != null)
                        {
                            if (fromRel.ClassName.Equals("EDGIS.Controller"))
                            {
                                SetFieldValue((ITable)fromRel.ObjectClass, "SUBTYPECD", 6, rel.FieldName + " = '" + rel.Value + "'");//SwitchControl
                            }

                            if (fromRel.ClassType.Equals(esriDatasetType.esriDTTable))
                            {
                                SetFieldValue((ITable)fromRel.ObjectClass, toRel.OriginForeignKey, switchGlobalID, rel.FieldName + " = '" + rel.Value + "'");
                            }
                            else if (fromRel.ClassType.Equals(esriDatasetType.esriDTFeatureClass))
                            {
                                SetFieldValue((IFeatureClass)fromRel.ObjectClass, toRel.OriginForeignKey, switchGlobalID, rel.FieldName + " = '" + rel.Value + "'");
                            }
                        }
                    }
                }
            }
          

            return newfeature;
            
        }

        /// <summary>
        /// Set attribute value in feature class
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        private static void SetFieldValue(IFeatureClass featureClass, string fieldName, object value, string condition = "")
        {
            // Restrict the number of features to be updated.
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;
            queryFilter.SubFields = fieldName;

            int objectID = 0;
            // Use IFeatureClass.Update to populate IFeatureCursor.
            IFeatureCursor updateCursor = featureClass.Update(queryFilter, false);
            int typeFieldIndex = featureClass.FindField(fieldName);
            //int modifiedDateFieldIndex = featureClass.FindField("MODIFIED_DATE");
            IFeature feature = null;
            try
            {
                while ((feature = updateCursor.NextFeature()) != null)
                {
                    objectID = feature.OID;
                    feature.set_Value(typeFieldIndex, value);
                    //feature.set_Value(modifiedDateFieldIndex, DateTime.Now);

                    updateCursor.UpdateFeature(feature);
                }
            }
            catch (COMException comExc)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in SetFieldValue(" + ((IDataset)featureClass).Name + ", " + fieldName + ", " + value.ToString() + ", " + condition + ") - OID: " + objectID + " : " + comExc.Message);
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.ReleaseComObject(queryFilter);
                if (feature != null) Marshal.ReleaseComObject(feature);
                if (updateCursor != null) Marshal.ReleaseComObject(updateCursor);
            }
        }

        /// <summary>
        /// Set attribute value in attribute table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        private static void SetFieldValue(ITable table, string fieldName, object value, string condition = "")
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;
            queryFilter.SubFields = fieldName;

            int objectID = 0;
            ICursor updateCursor = table.Update(queryFilter, false);
            int typeFieldIndex = table.FindField(fieldName);
            IRow row = null;
            try
            {
                while ((row = updateCursor.NextRow()) != null)
                {
                    objectID = row.OID;
                    row.set_Value(typeFieldIndex, value);
                    updateCursor.UpdateRow(row);
                }
            }
            catch (COMException comExc)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in SetFieldValue(" + ((IDataset)table).Name + ", " + fieldName + ", " + value.ToString() + ", " + condition + ") - OID: " + objectID + " : " + comExc.Message);
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.ReleaseComObject(queryFilter);
                if (row != null) Marshal.ReleaseComObject(row);
                if (updateCursor != null) Marshal.ReleaseComObject(updateCursor);
            }
        }

        /// <summary>
        /// Get attribute value from table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static object GetFieldValue(ITable table, string fieldName, string condition)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;
            queryFilter.SubFields = fieldName;

            int objectID = 0;
            ICursor updateCursor = table.Search(queryFilter, false);
            int typeFieldIndex = table.FindField(fieldName);
            IRow row = null;
            try
            {
                if ((row = updateCursor.NextRow()) != null)
                {
                    objectID = row.OID;
                    return row.get_Value(typeFieldIndex);
                }
            }
            catch (COMException comExc)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in GetFieldValue(" + ((IDataset)table).Name + ", " + fieldName + ", " + condition + ") - OID: " + objectID + " : " + comExc.Message);
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.ReleaseComObject(queryFilter);
                if (row != null) Marshal.ReleaseComObject(row);
                if (updateCursor != null) Marshal.ReleaseComObject(updateCursor);
            }
            return null;
        }

        /// <summary>
        /// Delete features from feature class based on condition
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="condition"></param>
        private static void DeleteFeatures(IFeatureClass featureClass, string condition)
        {
            // Restrict the number of features to be updated.
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;

            int objectID = 0;
            // Use IFeatureClass.Update to populate IFeatureCursor.
            IFeatureCursor updateCursor = featureClass.Update(queryFilter, false);
            IFeature feature = null;
            try
            {
                while ((feature = updateCursor.NextFeature()) != null)
                {
                    objectID = feature.OID;
                    updateCursor.DeleteFeature();
                }
            }
            catch (COMException comExc)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in DeleteFeatures(" + ((IDataset)featureClass).Name + ", " + condition + ") - OID: " + objectID + " : " + comExc.Message);
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.ReleaseComObject(queryFilter);
                if (feature != null) Marshal.ReleaseComObject(feature);
                if (updateCursor != null) Marshal.ReleaseComObject(updateCursor);
            }
        }

        /// <summary>
        /// Open workspace from SDE file
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <returns></returns>
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        //private static bool DisableAutoUpdaterFramework(out IMMAutoUpdater autoupdater, out mmAutoUpdaterMode oldMode)
        //{
        //    string strDisableAU = ConfigurationManager.AppSettings["DISABLE_AU_FRAMEWORK"].ToString();

        //    try
        //    {
        //        Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
        //        object obj = Activator.CreateInstance(type);
        //        autoupdater = obj as IMMAutoUpdater;
        //        oldMode = autoupdater.AutoUpdaterMode;
        //        //disable all ArcFM Autoupdaters 
        //        if (Convert.ToBoolean(strDisableAU))
        //        {

        //            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
        //            return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //WriteLine(DateTime.Now + "");
        //        //throw;
        //        WriteLine(DateTime.Now.ToLongTimeString() + " Error in disabling Auto Updaters. ");
        //    }
        //    autoupdater = null;
        //    oldMode = mmAutoUpdaterMode.mmAUMStandAlone;
        //    return false;
        //}
    }

    class RelationShipDetails
    {
        public string ClassName { get; set; }
        public esriDatasetType ClassType { get; set; }
        public IObjectClass ObjectClass { get; set; }
        public string OriginForeignKey { get; set; }
        public string OriginPrimaryKey { get; set; }
    }

    class Relation
    {
        public string ClassName { get; set; }
        public string FieldName { get; set; }
        public string Value { get; set; }
    }
}
