using System;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.GDBM;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("D4C788D5-4E59-45C1-AED0-FC32800F8F58")]
    [ProgId("PGE.Desktop.EDER.ValidateOperatingNumberExists")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateOperatingNumberExists : BaseValidationRule
    {
        private int _OPNUMIDX;                                                  // FIELD INDEX FOR OPERATING NUMBER
        private int _CGC12IDX;                                                  // FIELD INDEX FOR CGC12 
                        

        private static readonly Common.Delivery.Diagnostics.Log4NetLogger Logger =
            new Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType,
                "EDERDesktop.log4net.config");

        public ValidateOperatingNumberExists()
            : base("PGE Validate Operating Number Exists")
        {
        }

        protected override bool EnableByModelNames(object param)
        {
            if (!(param is IObjectClass))
            {
                Logger.Debug("Parameter is not type of IObjectClass, exiting");
                return false;
            }

            IObjectClass oclass = param as IObjectClass;
            Logger.Debug("ObjectClass:" + oclass.AliasName);

            //Check if ClassModelName exist on current ObjectClass
            bool enableForClassModel = ModelNameFacade.ContainsClassModelName(oclass,
                SchemaInfo.Electric.ClassModelNames.OperatingNumber);
            Logger.Debug("ClassModelName:" + SchemaInfo.Electric.ClassModelNames.OperatingNumber + ", in ObjectClass :" +
                          oclass.AliasName + "exist(" + enableForClassModel + ")");

            //Check if FieldModelName exist on current ObjectClass fields
            bool enableForFieldModel = ModelNameFacade.ContainsFieldModelName(oclass,
                SchemaInfo.Electric.FieldModelNames.OperatingNumber);
            Logger.Debug("FieldModelName:" + SchemaInfo.Electric.FieldModelNames.OperatingNumber + ", in ObjectClass :" +
                         oclass.AliasName + "exist(" + enableForFieldModel + ")");

            Logger.Debug(string.Format("Returning Visible:{0}", enableForClassModel && enableForFieldModel));
            return (enableForClassModel && enableForFieldModel);
        }

        protected override ID8List InternalIsValid(IRow row)
        {
#if DEBUG
            Debugger.Launch();
#endif

            try
            {
                // if (isEditInsert(row)) { return null; }                                                                                 // DO NOT PROCESS IF THE EDIT TYPE IS NOT AN INSRT

                // Nuisance rule modification (only works when version used, not map selection).
                // This rule should only run on inserts.
                //
                if (!CommonValidate.ExcludeRule("", esriDifferenceType.esriDifferenceTypeInsert, row))
                {
                    Logger.Debug("OperatingNumber Validation Exist excluded based on insert.");
                    return _ErrorList;
                }

                bool useOPNumber = true;

                _OPNUMIDX = row.Fields.FindField("OPERATINGNUMBER");                                                                    // GET THE FIELD INDEXES FOR OPERATING NUMBER AND CGC12
                _CGC12IDX = row.Fields.FindField("CGC12");

                ITable table = row.Table;
                IDataset dSet = (IDataset)table;

                if((dSet.Name.ToUpper() == "EDGIS.TRANSFORMER") && (Convert.ToInt16(row.get_Value(row.Fields.FindField("SUBTYPECD"))) == 1)) 
                { 
                    useOPNumber = false; 
                }
              

                string testValue = string.Empty;

                if ((_OPNUMIDX == -1) && (_CGC12IDX == -1)) { return null; }                                                            // EXIT IF THE REQUIRED DATA DOES NOT EXIST IN THE ROW BEING PROCESSED
                if ((DBNull.Value.Equals(row.Value[_OPNUMIDX])) && (DBNull.Value.Equals(row.Value[_CGC12IDX]))) { return null; }        // EXIT IF BOTH FIELDS ARE NULL

                if (!DBNull.Value.Equals(row.Value[_OPNUMIDX]))
                {
                    testValue = row.Value[_OPNUMIDX].ToString();
                    IFeature feat = OperatingNumberIsUnique(row, testValue);
                    if (feat != null)
                    {
                        //INC000004131856 
                        //This error is not applicable to overhead transformers 
                        if (useOPNumber) 
                            AddError("The Operating Number is already in use. ( Feature Class = " + feat.Class.AliasName + ", OID = " + feat.OID + ")");      // CHECK IF THE OPERATING NUMBER IS UNIQUE
                    }
                }
                else
                {
                    if ((_CGC12IDX != -1) && (!DBNull.Value.Equals(row.Value[_CGC12IDX]))) { testValue = row.Value[_CGC12IDX].ToString(); }
                }
              
                RunArcMapValidation(row, useOPNumber, testValue);                                                                                               // VALIDATE THE OPERATING NUMBER/CGC12 VALUE
            }
            catch (COMException e)
            {
                Logger.Error("Failed with exception " + e.InnerException + e.StackTrace);
            }

            //Debug.Print("_ErrorList.HasChildren:" + _ErrorList.HasChildren.ToString());  
            return _ErrorList;
        }


        /// <summary>
        /// DETERMINE IF THE EDIT TYPE OF THE FEATURE BEING VALIDATED IS AN INSERT (CREATE A NEW FEATURE)
        /// </summary>
        /// <param name="row">THE FEATURE BEING PROCESSED BY THE VALIDATION RULE</param>
        /// <returns>A BOOLEAN INDICATING IF THE EDIT TYPE IS AN INSERT</returns>
        /// 

        private bool isEditInsert(IRow row)
        {
            IWorkspace defWSpace = null;                        
            IFeatureWorkspace defFWSpce = null;

            try
            {
                int oid = (int)row.get_Value(row.Fields.FindField(row.Table.OIDFieldName));

                defWSpace = ValidationEngine.Instance.getWorkspace();                                   // GET THE DEFAULT (PARENT) WORKSPACE
                defFWSpce = (IFeatureWorkspace)defWSpace;

                IDataset defTableDSet = (IDataset)row.Table;
                string tableName = defTableDSet.Name;

                ITable defTable = defFWSpce.OpenTable(tableName);                                       // OPEN THE TABLE IN THE DEFAULT WORKSPACE

                IRow defRow = defTable.GetRow(oid);                                                     // TRY TO GET THE FEATURE IN THE DEFAULT WORKSPACE
                if (defRow != null) { return false; }                                                    // THE FEATURE IS NOT FOUND IN THE DEFAULT WORKSPACE IT WAS CREATED IN THE SESSION WORKSPACE

                Logger.Info("ValidateOperatingNumberExists: Edit of " + tableName + " (OID = " + oid + ") is Not an Insert and Not Validated.");
               
                return true;                                                                           // IF THE FEATURE WAS FOUND, IT IS NOT AN INSERT - IT EXISTED IN THE PARENT VERSION
            }
            catch (COMException e)
            {
                if (e.ErrorCode == -2147219118)
                {
                    return true;                                                                        // THIS ERROR IS GENERATED WHEN THE FEATURE IS NOT FOUND SO IT INDICATES AN INSERT
                }
                else
                {
                    Logger.Error("ValidateOperatingNumberExists: " + e.Message + " " + e.InnerException + e.StackTrace);
                    return true;
                }
            }
            finally
            {
                //if (defWSpace != null) { Marshal.ReleaseComObject(defWSpace); }
                if (defFWSpce != null) { Marshal.ReleaseComObject(defFWSpce); }
            }
        }


        /// <summary>
        /// DETERMINE IF ANOTHER FEATURE HAS THE OPERATING NUMBER BEING USED FOR THE FEATURE BEING CREATED
        /// </summary>
        /// <param name="workSpace">THE CURRENT GIS WORKSPACE</param>
        /// <param name="globalId">THE GUID OF THE FEATURE HAVING THE OPERATING NUMBER BEING TESTED</param>
        /// <param name="OPNumValue">THE OPERATING NUMBER BEING TESTED</param>
        /// <returns>A FEATURE WHICH HAS THE SAME OPERATING NUMBER</returns>
        /// 

        private IFeature OperatingNumberIsUnique(IRow row, String OPNumValue)
        {
            IFeatureCursor fCursor = null;
            IQueryFilter qFilter = null;

            try
            {
                IDataset dSet = (IDataset) row.Table;
                IWorkspace workSpace = dSet.Workspace;

                object globalID = row.Value[row.Fields.FindField("GLOBALID")];

                IEnumFeatureClass OPNumClasses = ModelNameManager.Instance.FeatureClassesFromModelNameWS(workSpace, SchemaInfo.Electric.ClassModelNames.OperatingNumber);
                OPNumClasses.Reset();

                IFeature selectFeat = null;

                for (IFeatureClass OPNumFc = OPNumClasses.Next(); OPNumFc != null; OPNumFc = OPNumClasses.Next())
                {
                    qFilter = new QueryFilterClass();
                    qFilter.WhereClause = "OPERATINGNUMBER = '" + OPNumValue + "'";

                    fCursor = OPNumFc.Search(qFilter, false);
                    selectFeat = fCursor.NextFeature();

                    while (selectFeat != null)
                    {
                        if (!string.IsNullOrEmpty(globalID.ToString()))
                        {
                            if (!selectFeat.Value[selectFeat.Fields.FindField("GLOBALID")].Equals(globalID)) { return selectFeat; }
                        }

                        selectFeat = fCursor.NextFeature();
                    }
                }
            }
            finally
            {
                if (fCursor != null) { Marshal.ReleaseComObject(fCursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }
        
            return null;
        }

        //private void RunGDBMValidation(String username, String password, String opNumValue, IRow row)
        //{
        //    OracleDbConnection conn = new OracleDbConnection(username, password, WipDbApi.WipDbTns);
        //    OracleDataReader rdr = null;
        //    WipDbApi api = new WipDbApi(username, password);
        //    try
        //    {
        //        rdr =
        //            conn.ExecuteCommand("select count(*) from " + SchemaInfo.Wip.JETEquipmentTable +
        //                                " where operatingnumber='" + opNumValue + "' and equiptypeid=" +
        //                                api.GetEquipmentId(row) + " and jobnumber='" +
        //                                row.Value[row.Fields.FindField("INSTALLJOBNUMBER")] + "'");

        //        //rdr =
        //        //   conn.ExecuteCommand("select count(*) from " + SchemaInfo.Wip.JETEquipmentTable +
        //        //                       " where operatingnumber='" + opNumValue + "' and equipmenttypeid=" +
        //        //                       api.GetEquipmentId(row) + " and jobnumber='" +
        //        //                       row.Value[row.Fields.FindField("INSTALLJOBNUMBER")] + "'");

        //        rdr.Read();
        //        int res = rdr.GetInt32(0);
        //        if (res == 0)
        //        {
        //            AddError("Operating number has not been reserved in the Job Editor Tool.");
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //        if (rdr != null && !rdr.IsClosed) rdr.Close();
        //    }
        //}


        /// <summary>
        /// VALIDATE THE OPERATING NUMBER / CGC12 IS RESERVED IN THE WIP DATABASE JET EQUIPMENT TABLE
        /// </summary>
        /// <param name="row">THE FEATURE (ROW) BEING VALIDATED</param>
        /// 

        private void RunArcMapValidation(IRow row, bool useOPNumber, string testValue)
        {
            if(DBNull.Value.Equals(row.Value[row.Fields.FindField("INSTALLJOBNUMBER")].ToString())) { return; }
            String jobNumValue = row.Value[row.Fields.FindField("INSTALLJOBNUMBER")].ToString();                                    // GET THE JOB NUMBER
            if (jobNumValue == "0") { return; }                                                                                     // DO NOT PROCESS IF THE JOB NUMBER IS ZERO

            ILayer WIPLayer = null;
            if (!FindLayerByName("Wip", out WIPLayer))                                                                              // GET THE WIP GROUP LAYER
            {
                AddError(
                    "Could not find Wip layer in current map document, unable to perform Operating Number / CGC validation.");
                return;
            }
          
            IWorkspace wSpace = null;
            IFeatureWorkspace fWSpace = null;
            IQueryFilter qFilter = null;
            ICursor cursor = null;

            try
            {
                IFeatureLayer fLayer = (IFeatureLayer)WIPLayer;
                IFeatureClass wipFClass = fLayer.FeatureClass;

                IDataset wipDSet = (IDataset)wipFClass;                                                             // OPEN THE WIP LAYER TO GET A WORKSPACE TO THE WIP DATABASE
                wSpace = wipDSet.Workspace;

                fWSpace = (IFeatureWorkspace)wSpace;

                IObjectClass oClass = (IObjectClass)row.Table;                      
                int oID = oClass.ObjectClassID;

                IFeatureClass fClass = (IFeatureClass)oClass;                                                       // GET THE OBJECTCLASS OF THE FEATURE BEING VALIDATED
                IDataset dSet = (IDataset)fClass;
                string fClassName = dSet.Name;
                
                ITable jetEquipTable = fWSpace.OpenTable("WEBR.Jet_Equipment");                                     // OPEN THE JET EQUIPMENT TABLE

                qFilter = new QueryFilterClass();
             
                if (useOPNumber)                                                                                    // TEST TO SEE IF THE OPERATING NUMBER OR CGC12 WILL BE USED - USE THE OPERATING NUMBER IF IT EXISTS 
                {
                    qFilter.WhereClause = "OPERATINGNUMBER ='" + testValue + "' and JOBNUMBER = '" + jobNumValue + "'";
                }
                else
                {
                    qFilter.WhereClause = "CGC12 ='" + testValue + "' and JOBNUMBER = '" + jobNumValue + "'";
                }


                cursor = (jetEquipTable.Search(qFilter, false));

                if (cursor.NextRow() == null)
                {
                    if (useOPNumber)                                                                                // IF A ROW MATCHES THE JOBNUMBER AND OPERATING NUMBER / CGC12 THEY ARE RESERVED IN JET
                    {
                        AddError("Operating Number Not Reserved in JET tool.");
                    }
                    else
                    {
                        AddError("CGC12 Number Not Reserved in JET tool.");
                    }
                } 
            }
            finally
            {
                if (wSpace != null) { Marshal.ReleaseComObject(wSpace); }
                if (fWSpace != null) { Marshal.ReleaseComObject(fWSpace); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
            }
            

            
            
            


                

            //for (IDataset ds = dsEnum.Next(); ds != null; ds = dsEnum.Next())
            //{
            //    if (!ds.Name.ToUpper().Equals(SchemaInfo.Wip.JETEquipmentTable)) continue;

                
            //}
        }

        private bool FindLayerByName(String name, out ILayer resLayer)
        {
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            IApplication arcMapApp = appRefObj as IApplication;
            if (arcMapApp == null)
            {
                AddError(
                    "ArcMap application not found and unable to locate/login to the WIP database. Operating number / CGC validation failed to execute.");
                resLayer = null;
                return false;
            }

            IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
            IMap map = mxDoc.FocusMap;

            resLayer = FindLayerHelper(map, null, name);
            return resLayer != null;
        }

        private static ILayer FindLayerHelper(IMap map, ICompositeLayer layers, string lyrName)
        {
            for (int i = 0; i < (map != null ? map.LayerCount : layers.Count) ; i++)
            {
                ILayer lyr = map == null ? layers.Layer[i] : map.Layer[i];

                if (lyr is ICompositeLayer) lyr = FindLayerHelper(null, (ICompositeLayer)lyr, lyrName);

                if (lyr != null && lyr.Name.Equals(lyrName))
                {
                    return lyr;
                }
            }

            return null;
        }

        private int getFeatureClassID(IFeatureClass fClass)
        {
            IObjectClass pOClass = (IObjectClass)fClass;

            int iClassID = pOClass.ObjectClassID;

            return iClassID;

        }
    }
}
