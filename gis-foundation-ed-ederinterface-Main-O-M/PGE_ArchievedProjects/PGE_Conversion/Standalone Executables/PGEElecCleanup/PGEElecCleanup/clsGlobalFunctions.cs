using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Miner.Interop;
using System.Collections;
using Oracle.DataAccess.Client;
using System.IO;

namespace PGEElecCleanup
{
    class clsGlobalFunctions
    {
        public static clsGlobalFunctions  _globalFunctions = new clsGlobalFunctions();

        public Boolean loadVersionInCombobox(string lUsername, string Password, string Server,
                                          string Service, string rVersion, string Database, ComboBox objCombo)
        {
            IWorkspace pWorkSpace = null;
            IWorkspaceFactory pWorkSpaceFacrory = null;
            IPropertySet pConnectionProperties = null;

            IVersionedWorkspace versionedWorkspace = null;
            IEnumVersionInfo enumVersionInfo = null;
            IVersionInfo versionInfo = null;

            try
            {
                pConnectionProperties = new PropertySet();
                pConnectionProperties.SetProperty("USER", lUsername);
                pConnectionProperties.SetProperty("PASSWORD", Password);
                pConnectionProperties.SetProperty("SERVER", Server);
                pConnectionProperties.SetProperty("INSTANCE", Service);
                pConnectionProperties.SetProperty("VERSION", rVersion);
                pConnectionProperties.SetProperty("DATABASE", Database);

                pWorkSpaceFacrory = new SdeWorkspaceFactoryClass();
                pWorkSpace = pWorkSpaceFacrory.Open(pConnectionProperties, 0);

                if (pWorkSpace == null)
                {
                    MessageBox.Show("Unable to connect to SDE Database..Please verify the details.", ConstantValues.conApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //load version names
                objCombo.Items.Clear();
                if (pWorkSpace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    versionedWorkspace = (IVersionedWorkspace)pWorkSpace;

                    //get a enumerator of all the versions on the versioned workspace    
                    enumVersionInfo = versionedWorkspace.Versions;
                    enumVersionInfo.Reset();

                    versionInfo = enumVersionInfo.Next();
                    while (versionInfo != null)
                    {
                        string versionName = versionInfo.VersionName;
                        objCombo.Items.Add(versionName);
                        versionInfo = enumVersionInfo.Next();
                    }

                    return true;
                }

                //MessageBox.Show("The details are not SDE..Please verify the details.", ConstantValues.conApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch(Exception ex)
            {
                //Logs.Error("Error occured in getsdeworkspace");
                return false;
            }
            finally
            {
                if (pWorkSpace != null)
                {
                    Marshal.ReleaseComObject(pWorkSpace);
                }
                if (pWorkSpaceFacrory != null)
                {
                    Marshal.ReleaseComObject(pWorkSpaceFacrory);
                }
                if (pConnectionProperties != null)
                {
                    Marshal.ReleaseComObject(pConnectionProperties);
                }

                if (versionedWorkspace != null)
                {
                    Marshal.ReleaseComObject(versionedWorkspace);
                }
                if (enumVersionInfo != null)
                {
                    Marshal.ReleaseComObject(enumVersionInfo);
                }
                if (versionInfo != null)
                {
                    Marshal.ReleaseComObject(versionInfo);
                }
            }
        }

        public bool gAppendTable(ref DataTable dtFirst, DataTable dtSecond)
        {
            try
            {
                if (dtSecond.Rows.Count == 0)
                {
                    return true;
                }

                //append both data
                foreach (DataRow pDataRow in dtSecond.Rows)
                {
                    DataRow pDataRow_Two = dtFirst.NewRow();
                    pDataRow_Two.ItemArray = pDataRow.ItemArray;
                    dtFirst.Rows.Add(pDataRow_Two);
                }

                return true;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Error while append two table " + Ex.Message);
                return false;
            }
        }

        public DataTable compareDataTables(DataTable First, DataTable Second)
        {
                //First.TableName = "FirstTable";
                //Second.TableName = "SecondTable";

               //Create Empty Table 
                DataTable table = new DataTable("Difference"); 
                DataTable table1 = new DataTable(); 
                try 
                { 
                    //Must use a Dataset to make use of a DataRelation object 
                    using (DataSet ds4 = new DataSet())
                    {
                        //Add tables 
                        ds4.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });

                        //Get Columns for DataRelation 
                        DataColumn[] firstcolumns = new DataColumn[ds4.Tables[0].Columns.Count];

                        for (int i = 0; i < firstcolumns.Length; i++)
                        {
                            firstcolumns[i] = ds4.Tables[0].Columns[i];
                        }

                        DataColumn[] secondcolumns = new DataColumn[ds4.Tables[1].Columns.Count];

                        for (int i = 0; i < secondcolumns.Length; i++)
                        {
                            secondcolumns[i] = ds4.Tables[1].Columns[i];
                        }

                        //Create DataRelation 
                        DataRelation r = new DataRelation(string.Empty, firstcolumns, secondcolumns, false);

                        ds4.Relations.Add(r);

                        //Create columns for return table 
                        for (int i = 0; i < First.Columns.Count; i++)
                        {
                            table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                        }

                        table1 = table.Copy();

                        //If First Row not in Second, Add to return table. 
                        table.BeginLoadData();

                        foreach (DataRow parentrow in ds4.Tables[0].Rows)
                        {
                            DataRow[] childrows = parentrow.GetChildRows(r);

                            if (childrows == null || childrows.Length == 0)
                            {
                                table.LoadDataRow(parentrow.ItemArray, true);
                            }
                            table1.LoadDataRow(childrows, false);
                        }

                        table.EndLoadData();
                    }
                } 
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.Message); 
                } 
                return table; 
       } 

        public IFeatureClass getFeatureclassByName(IWorkspace _pWorkspace, string strFCname)
        {
            try
            {
                IFeatureWorkspace pFeatWorkSpace = _pWorkspace as IFeatureWorkspace;
                IFeatureClass pFeatclass = pFeatWorkSpace.OpenFeatureClass(strFCname);
                return pFeatclass;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Unable to open featureclass : " + strFCname);
                return null;
            }
        }

        public ITable getTableByName(IWorkspace _pWorkspace, string strTablename)
        {
            try
            {
                IFeatureWorkspace pFeatWorkSpace = _pWorkspace as IFeatureWorkspace;
                ITable pTable = pFeatWorkSpace.OpenTable(strTablename);
                return pTable;
            }
            catch (Exception eX)
            {
                Logs.writeLine("Unable to open Table : " + strTablename);
                return null;
            }
        }

        internal System.Data.DataTable getDSname(IWorkspace pWorkspace)
        {
            try
            {
                DataTable dtDataSet = new DataTable("FeatureDataset");
                IEnumDataset pEnumDSname = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);

                string strFDSName = "FEATUREDATASET";
                string strDSColumnName = "NAME";
                string strAliasName = "ALIASNAME";

                dtDataSet.Columns.Add(strFDSName);
                dtDataSet.Columns.Add(strDSColumnName);
                dtDataSet.Columns.Add(strAliasName);

                dtDataSet = getNameinGDB(pEnumDSname, strFDSName, dtDataSet.TableName, dtDataSet);
                return dtDataSet;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while geting DataSet names :" + ex.Message);
                return null;
            }
        }

        internal System.Data.DataTable getRSName(IWorkspace pWorkspace)
        {
            try
            {
                DataTable objRSDataTable = new DataTable("RelationshipClass");
                IEnumDataset pEnumRSname = pWorkspace.get_Datasets(esriDatasetType.esriDTRelationshipClass);

                string strFDSColumnName = "FEATUREDATASET";
                string strRScolumnName = "NAME";
                string strAliasName = "ALIASNAME";

                objRSDataTable.Columns.Add(strFDSColumnName);
                objRSDataTable.Columns.Add(strRScolumnName);
                objRSDataTable.Columns.Add(strAliasName);

                IDataset pDataset = pEnumRSname.Next();
                while (pDataset != null)
                {
                    DataRow objDatarow = objRSDataTable.NewRow();
                    objDatarow[strFDSColumnName] = "NODATASET";
                    objDatarow[strRScolumnName] = splitName(pDataset.Name);
                    objRSDataTable.Rows.Add(objDatarow);
                    pDataset = pEnumRSname.Next();
                }

                IEnumDataset pEnumDSname = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                IDataset objFDS = pEnumDSname.Next();
                while (objFDS != null)
                {
                    IFeatureDataset pFDataset = (IFeatureDataset)objFDS;
                    IEnumDataset penumDS = pFDataset.Subsets;
                    IDataset dataset = penumDS.Next();
                    while (dataset != null)
                    {
                        if (dataset.Type == esriDatasetType.esriDTRelationshipClass)
                        {
                            IRelationshipClass pRelationshipClass = (IRelationshipClass)dataset;
                            DataRow objRSDatarow = objRSDataTable.NewRow();

                            objRSDatarow[strFDSColumnName] = splitName(objFDS.Name);
                            objRSDatarow[strRScolumnName] = splitName(dataset.Name);
                            objRSDataTable.Rows.Add(objRSDatarow);
                        }
                        dataset = penumDS.Next();
                    }
                    objFDS = pEnumDSname.Next();
                }
                return objRSDataTable;

            }
            catch (Exception ex)
            {
                //throw ex;
                Logs.writeLine("Error while geting Relationships names :" + ex.Message);
                return null;
            }

        }

        public static DataTable getNameinGDB(IEnumDataset pEnumDS, string strColumnsName, string dtname, DataTable objdt)
        {
            try
            {
                IDataset pDataset = pEnumDS.Next();
                DataTable objDataTable = objdt;

                // objDataTable.Columns.Add(strColumnsName);
                while (pDataset != null)
                {
                    DataRow objDatarow = objDataTable.NewRow();
                    objDatarow["FEATUREDATASET"] = "NODATASET";
                    objDatarow[strColumnsName] = splitName(pDataset.Name);
                    objdt.Rows.Add(objDatarow);
                    pDataset = pEnumDS.Next();
                }

                return objDataTable;

            }
            catch (Exception ex)
            {
                //throw ex;
                Logs.writeLine("Error while geting data from GDB :" + ex.Message);
                return null;
            }
        }

        public static DataTable getFCName(IWorkspace pWorkspace)
        {
            try
            {
                IEnumDataset pEnumFeatureDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                DataTable objDataTable = new DataTable("FeatureClass");

                objDataTable.Columns.Add("FEATUREDATASET");
                objDataTable.Columns.Add("NAME");
                objDataTable.Columns.Add("ALIASNAME");
                //objDataTable.Columns.Add("FIELDNAME");
                //objDataTable.Columns.Add("FIELDALISNAME");

                IDataset pdataset = pEnumFeatureDataset.Next();

                while (pdataset != null)
                {

                    IFeatureDataset pFDataset = (IFeatureDataset)pdataset;
                    IEnumDataset penumDS = pFDataset.Subsets;
                    IDataset dataset = penumDS.Next();

                    while (dataset != null)
                    {
                        if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                        {
                            IFeatureClass pfeaureclass = (IFeatureClass)dataset;
                            if (pfeaureclass.FeatureType != esriFeatureType.esriFTAnnotation)
                            {
                                DataRow objFCDatarow = objDataTable.NewRow();

                                objFCDatarow["FEATUREDATASET"] = splitName(pdataset.Name);
                                objFCDatarow["NAME"] = splitName(dataset.Name);
                                objFCDatarow["ALIASNAME"] = splitName(pfeaureclass.AliasName);

                                objDataTable.Rows.Add(objFCDatarow);
                            }
                        }
                        dataset = penumDS.Next();
                    }
                    pdataset = pEnumFeatureDataset.Next();
                }
                return objDataTable;
            }
            catch (Exception ex)
            {
                //throw ex;
                Logs.writeLine("Error while geting Featureclass names :" + ex.Message);
                return null;
            }

        }

        public static DataTable getTableName(IWorkspace pWorkspace)
        {
            try
            {

                DataTable objdtTable = new DataTable("Table");
                IEnumDataset pEnumTablename = pWorkspace.get_Datasets(esriDatasetType.esriDTTable);

                string strDSName = "FEATUREDATASET";
                string strTableName = "NAME";
                string strAliasName = "ALIASNAME";

                objdtTable.Columns.Add(strDSName);
                objdtTable.Columns.Add(strTableName);
                objdtTable.Columns.Add(strAliasName);

                //objdtTable = getNameinGDB(pEnumTablename, strTablecolumnName, objdtTable.TableName, objdtTable);

                IDataset pDataset = pEnumTablename.Next();
                

                while (pDataset != null)
                {
                    IObjectClass objTablname = (IObjectClass)pDataset;

                    DataRow objDatarow = objdtTable.NewRow();
                    objDatarow[strDSName] = "NODATASET";
                    objDatarow[strTableName] = splitName(pDataset.Name);
                    objDatarow[strAliasName] = objTablname.AliasName;
                    objdtTable.Rows.Add(objDatarow);
                    pDataset = pEnumTablename.Next();
                }

                // Use this bellow function When you want eveluate the tables that present inside of the featuredataset

                //IEnumDataset pEnumDSname = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                //IDataset objFDS = pEnumDSname.Next();
                //while (objFDS != null)
                //{
                //    IFeatureDataset pFDataset = (IFeatureDataset)objFDS;
                //    IEnumDataset penumDS = pFDataset.Subsets;
                //    IDataset dataset = penumDS.Next();
                //    while (dataset != null)
                //    {
                //        if (dataset.Type == esriDatasetType.esriDTTable)
                //        {
                //            ITable pTable = (ITable)dataset;
                //            DataRow objRSDatarow = objdtTable.NewRow();

                //            objRSDatarow[strDSName] = splitName(objFDS.Name);
                //            objRSDatarow[strTablecolumnName] = splitName(dataset.Name);
                //            objdtTable.Rows.Add(objRSDatarow);
                //        }
                //        dataset = penumDS.Next();
                //    }
                //    objFDS = pEnumDSname.Next();
                //}


                return objdtTable;

            }
            catch (Exception ex)
            {
                //throw ex;
                Logs.writeLine("Error while geting object table names :" + ex.Message);
                return null;
            }
        }

        public static DataTable getAnnoName(IWorkspace pWorkspace)
        {
            try
            {
                DataTable objdtAnno = new DataTable("Annotation");
                IEnumDataset pEnumDSname = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                string strDSName = "FEATUREDATASET";
                string strAnnoName = "NAME";
                string strAliasName = "ALIASNAME";

                objdtAnno.Columns.Add(strDSName);
                objdtAnno.Columns.Add(strAnnoName);
                objdtAnno.Columns.Add(strAliasName);
                IDataset objFDS = pEnumDSname.Next();
                while (objFDS != null)
                {
                    IFeatureDataset pFDataset = (IFeatureDataset)objFDS;
                    IEnumDataset penumDS = pFDataset.Subsets;
                    IDataset dataset = penumDS.Next();
                    while (dataset != null)
                    {
                        if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                        {
                            IFeatureClass pFeatureClass = (IFeatureClass)dataset;
                            if (pFeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                DataRow objRSDatarow = objdtAnno.NewRow();

                                objRSDatarow[strDSName] = splitName(objFDS.Name);
                                objRSDatarow[strAnnoName] = splitName(dataset.Name);
                                objRSDatarow[strAliasName] = splitName(pFeatureClass.AliasName);
                                objdtAnno.Rows.Add(objRSDatarow);
                            }
                        }
                        dataset = penumDS.Next();
                    }
                    objFDS = pEnumDSname.Next();
                }
                return objdtAnno;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Error while geting Annotation class names :" + Ex.Message);
                return null;
            }

        }

        public static string splitName(string itemname)
        {
            string strTempname = itemname;

            if (strTempname.Contains(".") == true)
            {
                strTempname = strTempname.Substring(strTempname.IndexOf('.') + 1);
            }
            return strTempname;
        }       

        public void DataTable2CSV(DataTable table, string filename,bool blnHeader)
        {
            string seprater = ",";
            DataTable2CSV(table, filename, seprater, blnHeader);
        }

        public void DataTable2CSV(DataTable table, string filename, string sepChar,bool blnHeader)
        {
            System.IO.StreamWriter writer = default(System.IO.StreamWriter);
            try
            {
                writer = new System.IO.StreamWriter(filename, !blnHeader);

                // first write a line with the columns name
                string sep = "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                if (blnHeader)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        builder.Append(sep).Append(col.ColumnName);
                        sep = sepChar;
                    }
                }
                writer.WriteLine(builder.ToString());

                // then write all the rows
                foreach (DataRow row in table.Rows)
                {
                    sep = "";
                    builder = new System.Text.StringBuilder();

                    foreach (DataColumn col in table.Columns)
                    {
                        builder.Append(sep).Append(row[col.ColumnName]);
                        sep = sepChar;
                    }
                    writer.WriteLine(builder.ToString());
                }
            }
            finally
            {
                if ((writer != null))
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public bool isFCExist(string strFcName, ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace)
        {
            try
            {
                IWorkspace2 pWS2 = (IWorkspace2)pWorkspace;
                if (pWS2.get_NameExists(esriDatasetType.esriDTFeatureClass, strFcName)) return true;
                Logs.writeLine("The featureclass name is not found in the database FCName: " + strFcName);
                return false;
            }
            catch (Exception Ex)
            {
                Logs.writeLine("The featureclass name is not found in the database FCName: " + strFcName + "\n Error" + Ex.Message.ToString());
                return false;
            }
        }

        public string getStringVal(object p, string strFC_OID, string strField)
        {
            try
            {
                if (p == DBNull.Value || p == null)
                {
                    return string.Empty;
                }
                return p.ToString().Trim();
            }
            catch (Exception Ex)
            {
                Logs.writeLine("Invalid values found Feature objectid-FieldName : " + strFC_OID + "-" + strField);
                return "";
            }
        }

        private static Dictionary<string, IRelationshipClass> relClassMapping = new Dictionary<string, IRelationshipClass>();
        /// <summary>
        /// Get the related feature set by passing Feature and realted feature name.
        /// </summary>
        /// <param name="pOrginFeature">Orgine Feature</param>
        /// <param name="realtedFCAliasName">Related FeatureClass Alias Name.</param>
        /// <returns>ISet</returns>
        public static ISet getRealtedFeature(IFeature pOrginFeature, string realtedFCAliasName, bool OrigineClass)
        {
            ESRI.ArcGIS.esriSystem.ISet relSet = null;
            try
            {
                IRelationshipClass relClass = null;
                string relClassKey = pOrginFeature.Class.AliasName + "-" + realtedFCAliasName;
                if (relClassMapping.ContainsKey(relClassKey))
                {
                    relClass = relClassMapping[relClassKey];
                }
                else
                {
                    IEnumRelationshipClass enumRelClass = pOrginFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    relClass = enumRelClass.Next();
                    if (OrigineClass == true)
                    {
                        while (relClass != null)
                        {

                            if (relClass.OriginClass.AliasName != realtedFCAliasName)
                            {
                                relClass = enumRelClass.Next();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        while (relClass != null)
                        {
                            if (relClass.DestinationClass.AliasName != realtedFCAliasName)
                            {
                                relClass = enumRelClass.Next();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    relClassMapping.Add(relClassKey, relClass);
                }

                if (relClass == null)
                {
                    return new SetClass();
                }

                relSet = relClass.GetObjectsRelatedToObject((IObject)pOrginFeature);

                return relSet;
            }
            catch (Exception ex)
            {
                return relSet;
            }
        }

        /// <summary>
        /// Update the attribute value given features.
        /// </summary>
        /// <param name="featureClass">Featureclass tha want to update.</param>
        /// <param name="Querystring">used to fileter the value</param>
        /// <param name="subFiled">Sub Filed names</param>
        /// <param name="FiledName">File that want to update.</param>
        /// <param name="UpdateValue">Provide the value taht want to update.</param>
        public void UpdateFCAttributeValues(IFeatureClass featureClass, string Querystring, string subFiled, string FiledName, string UpdateValue)
        {
            IFeatureCursor featureCursor = null;
            try
            {
                clsTestWorkSpace.StartEditOperation();
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = Querystring;
                queryFilter.SubFields = subFiled;
                featureCursor = featureClass.Update(queryFilter, false);
                IFields fields = featureCursor.Fields;
                int fieldIndex = fields.FindField(FiledName);
                IFeature feature = null;
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    feature.set_Value(fieldIndex, FiledName);
                    featureCursor.UpdateFeature(feature);
                }
                clsTestWorkSpace.StopEditOperation(true);
                Marshal.ReleaseComObject(featureCursor);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while Updating Mutiple features attribute value. Details:" + ex.Message.ToString());
            }
            finally
            {
                clsTestWorkSpace.StopEditOperation(true);
                Marshal.ReleaseComObject(featureCursor);
            }

        }

        public void validateWorkspace()
        {
            try
            {
                if (clsTestWorkSpace.Workspace == null)
                    MessageBox.Show("Please connect to the database.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information); return ;
            }
            catch(Exception ex)
            { }
        }


        public string getCodedDomainDescription(IWorkspace pWorkspace, string strDomainName, int intVal)
        {
            try
            {
                IWorkspaceDomains2 pWorkspaceDomain = (IWorkspaceDomains2)pWorkspace;

                IDomain pDomain = pWorkspaceDomain.get_DomainByName(strDomainName);
                ICodedValueDomain pCodeDomain = (ICodedValueDomain)pDomain;

                string strVal = string.Empty;
                for (int i = 0; i < pCodeDomain.CodeCount; i++)
                {
                    if (pCodeDomain.get_Value(i).ToString() == intVal.ToString())
                        strVal = pCodeDomain.get_Name(i);
                }

                //MessageBox.Show(strVal);
                return strVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getCodedDomainDescription(IWorkspace pWorkspace, string strDomainName, string strValue)
        {
            try
            {
                IWorkspaceDomains2 pWorkspaceDomain = (IWorkspaceDomains2)pWorkspace;

                IDomain pDomain = pWorkspaceDomain.get_DomainByName(strDomainName);
                ICodedValueDomain pCodeDomain = (ICodedValueDomain)pDomain;

                string strVal = string.Empty;
                for (int i = 0; i < pCodeDomain.CodeCount; i++)
                {
                    if (pCodeDomain.get_Value(i).ToString() == strValue.ToString())
                        strVal = pCodeDomain.get_Name(i);
                }

                //MessageBox.Show(strVal);
                return strVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Common_addColumnToReportTable(DataTable ObjReportTable, string strReportFldNames)
        {

            try
            {
                string[] strFldNames = strReportFldNames.Split(',');

                for (int x = 0; x < strFldNames.Length; x++)
                {
                    ObjReportTable.Columns.Add(strFldNames[x]);
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("FeatureCalss Name: getting error due to " + ex.Message);
            }
        }
        public void Common_addRowstoReportTable(DataTable ObjReportTable, string strReportValues)
        {
            try
            {
                DataRow pDataRow = ObjReportTable.NewRow();

                string[] strFldVals = strReportValues.Split(',');

                for (int x = 0; x < strFldVals.Length; x++)
                {
                    pDataRow[x] = strFldVals[x];
                }
                ObjReportTable.Rows.Add(pDataRow);
            }
            catch (Exception ex)
            {
                Logs.writeLine("getting error due to " + ex.Message);
            }
        }
        public bool Common_addRowstoReportTable(DataTable ObjReportTable, List<PGEElecCleanup.updPolygonAttributes.FeatureInfo> featInfoRecords, string featClassName, string ATableName, string csvFileName, string strTime)
        {
            bool firstWrite = true;
            string strSqlFilePath = string.Empty;
            string strLogFilePath = string.Empty;
            string strSpoolStatement = string.Empty;
            int intIterateCnt = 0;
            try
            {
                strSpoolStatement = "SPOOL C:\\UDC\\Logs\\" + csvFileName + ".txt";
                strLogFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
                if (Directory.Exists(strLogFilePath) == false)
                {
                    Directory.CreateDirectory(strLogFilePath);
                }
                strSqlFilePath = strLogFilePath + strTime + "_" + csvFileName + ".sql";
                StreamWriter objTxtWriter = new StreamWriter(strSqlFilePath);

                objTxtWriter.WriteLine(strSpoolStatement);
                objTxtWriter.WriteLine("SET DEFINE OFF");


                foreach (PGEElecCleanup.updPolygonAttributes.FeatureInfo featInfo in featInfoRecords)
                {
                    intIterateCnt++;
                    DataRow pDataRow = ObjReportTable.NewRow();

                    pDataRow[0] = featClassName;
                    pDataRow[1] = featInfo.globalID;
                    pDataRow[2] = featInfo.County;
                    pDataRow[3] = featInfo.District;
                    pDataRow[4] = featInfo.Division;
                    pDataRow[5] = featInfo.Zip;
                    pDataRow[6] = featInfo.City;

                    string setQuery = GetUpdateQuery(featInfo.globalID, featInfo.County, featInfo.District, featInfo.Division, featInfo.Zip, featInfo.City);
                    pDataRow[7] = "update " + featClassName + " " + setQuery;
                    pDataRow[8] = "update " + ATableName + " " + setQuery;


                    objTxtWriter.WriteLine(pDataRow[7].ToString().Replace('|', ','));
                    objTxtWriter.WriteLine(pDataRow[8].ToString().Replace('|', ','));

                    if (intIterateCnt == 10000)
                    {
                        objTxtWriter.WriteLine("COMMIT;");
                        intIterateCnt = 0;
                    }

                    ObjReportTable.Rows.Add(pDataRow);

                    if (ObjReportTable.Rows.Count > 1000)
                    {
                        if (firstWrite)
                        {
                            clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(ObjReportTable, csvFileName, true, strTime);
                            firstWrite = false;
                        }
                        else
                        {
                            clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(ObjReportTable, csvFileName, false, strTime);
                        }
                        ObjReportTable.Rows.Clear();
                    }
                }
                objTxtWriter.WriteLine("COMMIT;");
                objTxtWriter.WriteLine("SPOOL OFF");
                objTxtWriter.Close();
            }
            catch (Exception ex)
            {
                Logs.writeLine("getting error due to " + ex.Message);
            }
            return firstWrite;
            //bool firstWrite = true;
            //try
            //{                
            //    foreach (PGEElecCleanup.updPolygonAttributes.FeatureInfo featInfo in featInfoRecords)
            //    {
            //        DataRow pDataRow = ObjReportTable.NewRow();

            //        pDataRow[0] = featClassName;
            //        pDataRow[1] = featInfo.globalID;
            //        pDataRow[2] = featInfo.County;
            //        pDataRow[3] = featInfo.District;
            //        pDataRow[4] = featInfo.Division;
            //        pDataRow[5] = featInfo.Zip;
            //        pDataRow[6] = featInfo.City;

            //        string setQuery = GetUpdateQuery(featInfo.globalID, featInfo.County, featInfo.District, featInfo.Division, featInfo.Zip, featInfo.City);
            //        pDataRow[7] = "update " + featClassName + " " + setQuery;
            //        pDataRow[8] = "update " + ATableName + " " + setQuery;

            //        ObjReportTable.Rows.Add(pDataRow);

            //        if (ObjReportTable.Rows.Count > 1000)
            //        {
            //            if (firstWrite)
            //            {
            //                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(ObjReportTable, csvFileName, true, strTime);
            //                firstWrite = false;
            //            }
            //            else
            //            {
            //                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(ObjReportTable, csvFileName, false, strTime);
            //            }
            //            ObjReportTable.Rows.Clear();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logs.writeLine("getting error due to " + ex.Message);
            //}
            //return firstWrite;
        }
        private string GetUpdateQuery(string globalID, string county, string district, string division, string zip, string city)
        {
            string setQuery = "";
            if (!string.IsNullOrEmpty(county))
            {
                AppendQuery("COUNTY", county, ref setQuery);
            }
            if (!string.IsNullOrEmpty(district))
            {
                AppendQuery("DISTRICT", district, ref setQuery);
            }
            if (!string.IsNullOrEmpty(division))
            {
                AppendQuery("DIVISION", division, ref setQuery);
            }
            if (!string.IsNullOrEmpty(zip))
            {
                AppendQuery("ZIP", zip, ref setQuery);
            }
            if (!string.IsNullOrEmpty(city))
            {
                AppendQuery("CITY", city, ref setQuery);
            }

            setQuery += " where GLOBALID = '" + globalID + "';";
            return setQuery;
        }
        private void AppendQuery(string fieldName, string value, ref string setString)
        {
            if (string.IsNullOrEmpty(setString))
            {
                setString = "set " + fieldName + " = '" + value + "'";
            }
            else
            {
                setString += "|" + fieldName + " = '" + value + "'";
            }
        }
        public void Common_initSummaryTable(string LogName, string AppName)
        {
            try
            {
                //initialize the datatable
                //Update project details in log file.
                Logs.createLogfile(LogName + ".log");
                Logs.writeLine("Project Name :" + "PG&E ELECTRIC");
                Logs.writeLine("Application Name :" + AppName);
                Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                Logs.writeLine("******************************************************************************");
                Logs.writeLine("Database details");
                Logs.writeLine("******************************************************************************");

                //report database details
                Logs.writeLine("Databse : " + clsTestWorkSpace.strDATABASE + " INSTANCE :" + clsTestWorkSpace.strINSTANCE);
                Logs.writeLine("USER : " + clsTestWorkSpace.strUSER);
                Logs.writeLine("VERSION :" + clsTestWorkSpace.strVERSION);
                Logs.writeLine("******************************************************************************");

            }
            catch (Exception Ex)
            {
                Logs.writeLine("Error on initialize the summary table method : " + Ex.Message);
            }
        }

        /// <summary>
        /// Get the related feature set by passing Feature and realted feature name.
        /// </summary>
        /// <param name="pOrginFeature">Orgine Feature</param>
        /// <param name="realtedFCAliasName">Related FeatureClass Name.</param>
        /// <returns>ISet</returns>
        public ISet getRealtedFeature_TableName(IFeature pOrginFeature, string realtedFCName, bool OrigineClass)
        {
            ESRI.ArcGIS.esriSystem.ISet relSet = null;
            try
            {
                IEnumRelationshipClass enumRelClass = pOrginFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass relClass = enumRelClass.Next();
                if (OrigineClass == true)
                {
                    while (relClass != null)
                    {
                        if (((IDataset)relClass.OriginClass).Name.ToUpper() != realtedFCName.ToUpper())
                        {
                            relClass = enumRelClass.Next();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    while (relClass != null)
                    {
                        if (((IDataset)relClass.DestinationClass).Name.ToUpper() != realtedFCName.ToUpper())
                        {
                            relClass = enumRelClass.Next();
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (relClass == null)
                {
                    return relSet;
                }

                relSet = relClass.GetObjectsRelatedToObject((IObject)pOrginFeature);

                return relSet;
            }
            catch (Exception ex)
            {
                Logs.writeLine("FeatureCalss Name: " + pOrginFeature.Class.AliasName+ " :getting error for feat Objectid: " + pOrginFeature.OID.ToString() + " " + ex.Message);
                return relSet;
            }
        }

        public T Cast<T>(object obj, T defaultValue)
        {
            if (obj != null)
            {
                if (obj is T)
                {
                    if (IsEmpty(obj))
                        return defaultValue;

                    return (T)obj;
                }

                if (IsEmpty(obj))
                    return defaultValue;

                if (!Convert.IsDBNull(obj))
                {
                    return (T)Convert.ChangeType(obj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return defaultValue;
        }
        private static bool IsEmpty(object obj)
        {
            if (obj is string)
                if (string.IsNullOrEmpty(obj.ToString().Trim()))
                    return true;

            return false;
        }

        public static int GetFieldIndex(IObjectClass objClass, string columnName)
        {
            int fieldIndex = -1;

            try
            {
                fieldIndex = ClassIDToFieldIDMapping[objClass.ObjectClassID][columnName];
            }
            catch
            {
                //We haven't cached this field index yet
                if (!ClassIDToFieldIDMapping.ContainsKey(objClass.ObjectClassID))
                {
                    ClassIDToFieldIDMapping.Add(objClass.ObjectClassID, new Dictionary<string, int>());
                }
                fieldIndex = objClass.FindField(columnName);
                ClassIDToFieldIDMapping[objClass.ObjectClassID].Add(columnName, fieldIndex);
            }
            return fieldIndex;
        }

        private static Dictionary<int, Dictionary<string, int>> ClassIDToFieldIDMapping = new Dictionary<int, Dictionary<string, int>>();


        public OracleConnection GetOracleConnection(string strUserName, string strPword, string strDbSource)
        {
            OracleConnection orclDbConn = null;
            try
            {

                orclDbConn = new OracleConnection("Data Source = " + strDbSource + "; User Id = " + strUserName + "; Password = " + strPword + ";");
                orclDbConn.Open();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Unable to connect conn " + strDbSource + " " + ex.Message);

            }
            return orclDbConn;
        }















    }

    


}
