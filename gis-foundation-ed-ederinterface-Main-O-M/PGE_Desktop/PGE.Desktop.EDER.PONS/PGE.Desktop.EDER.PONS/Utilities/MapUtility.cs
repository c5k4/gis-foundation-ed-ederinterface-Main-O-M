using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using log4net;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    class PGEFeatureClass
    {
        #region private variables
        private IFeatureClass pFeatureClass;
        private bool bLayerAddedToMap;

        #endregion private variables

        #region Properties
        public IFeatureClass FeatureClass
        {
            get { return pFeatureClass; }
            set { pFeatureClass = value; }
        }

        public bool LayerAddedToMap
        {
            get { return bLayerAddedToMap; }
            set { bLayerAddedToMap = value; }
        }
        #endregion Properties

        #region Private constructor
        private PGEFeatureClass()
        { }
        #endregion Private constructor

        #region Public methods
        public PGEFeatureClass(IFeatureClass FeatureClass, bool LayerAddedToMap)
        {
            pFeatureClass = FeatureClass;
            bLayerAddedToMap = LayerAddedToMap;
        }

        public void Dispose()
        { 
            if(pFeatureClass != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureClass);
                pFeatureClass = null;
            }
        }
        #endregion Public methods
    }

    class MapUtility
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //public IFeatureLayer GetFeatureLayer(string FeatureClassName)
        //{
        //    IFeatureLayer fLayer = null;
        //    try
        //    {
        //        if(string.IsNullOrEmpty(FeatureClassName))
        //            FeatureClassName = "EDGIS.Transformer";
        //        IApplication pApp = ArcMap.Application;
        //        string featureclassname = FeatureClassName;
        //        fLayer = GetFeatureLayerfromFCName(pApp, featureclassname);
                
        //    }
        //    catch (Exception ex)
        //    {                
        //        //throw ex;
        //    }
        //    return fLayer;
        //}

        public PGEFeatureClass GetFeatureClassByName(string FeatureClassName)
        {
            PGEFeatureClass pReturnFeatClass = default(PGEFeatureClass);
            try
            {
                #region Commented
                //IMMPropertiesExt pExt; 
                //pExt = (IMMPropertiesExt)Application.FindExtensionByName("MMPropertiesExt");
                //IMMStoredDisplayManager pSDMan = pExt.StoredDisplayManager;

                ////create a stored display name for the current display
                //IMMStoredDisplayName pSDName= new MMStoredDisplayNameClass();
                //pSDName.Name = pSDMan.CurrentStoredDisplayName();
                //pSDName.Type = pSDMan.CurrentStoredDisplayType();

                ////get a fully hyrdated stored display name for the current display
                //pSDName = pSDMan.GetStoredDisplayByName(pSDName);

                ////retrieve the stored display itself
                //IMMStoredDisplay pSD = pSDMan.GetUnopenedStoredDisplay(pSDName);

                ////load the stored display
                //pSD.Load();

                ////get a reference to the map of the stored display
                //IMap pMap = pSD.Map;
                //if (pMap != null)
                //{
                //    pMap.layer
                //    MessageBox.Show(Convert.ToString(pMap.LayerCount));
                //}
                #endregion Commented

                IFeatureLayer pFeatLayer = GetFeatureLayerfromFCName(FeatureClassName);
                if (pFeatLayer != null)
                {
                    if (pFeatLayer.FeatureClass != null)
                    {
                        pReturnFeatClass = new PGEFeatureClass(pFeatLayer.FeatureClass, true);
                        return pReturnFeatClass;
                    }
                }

                try
                {
                    if (PGEGlobal.WORKSPACE_EDER != null)
                    {
                        IFeatureWorkspace pFeatWorkspace = PGEGlobal.WORKSPACE_EDER as IFeatureWorkspace;
                        if (pFeatWorkspace != null)
                        {
                            IFeatureClass pFeatClass = pFeatWorkspace.OpenFeatureClass(FeatureClassName);
                            if (pFeatClass != null)
                            {
                                pReturnFeatClass = new PGEFeatureClass(pFeatClass, false);
                                return pReturnFeatClass;
                            }
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException COMEx)
                { }
                catch (Exception ex)
                { throw ex; }

                try
                {
                    if (PGEGlobal.WORKSPACE_EDERSUB != null)
                    {
                        IFeatureWorkspace pFeatWorkspace = PGEGlobal.WORKSPACE_EDERSUB as IFeatureWorkspace;
                        if (pFeatWorkspace != null)
                        {
                            IFeatureClass pFeatClass = pFeatWorkspace.OpenFeatureClass(FeatureClassName);
                            if (pFeatClass != null)
                            {
                                pReturnFeatClass = new PGEFeatureClass(pFeatClass, false);
                                return pReturnFeatClass;
                            }
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException COMEx)
                { }
                catch (Exception ex)
                { throw ex; }
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { }
            return pReturnFeatClass;
        }
        public List<string> GetTransformerCircuitIds(string cgcno)
        {
            List<string> circuitids=new List<string>();
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                //IFeatureLayer fLayer = GetFeatureLayerfromFCName("EDGIS.Transformer");
                //if (fLayer == null)
                //{
                //    MessageBox.Show("Transformer layer not present on map");
                //    return circuitids;
                //}
                //IFeatureClass fc = fLayer.FeatureClass;

                PGEFeatureClass objPGEFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("Transformer"));

                if (objPGEFeatureClass == null)
                {
                    MessageBox.Show("Unable to retrieve Transformer data.");
                    return circuitids;
                }

                if (objPGEFeatureClass.FeatureClass == null)
                {
                    MessageBox.Show("Unable to retrieve Transformer data.");
                    return circuitids;
                }

                IFeatureClass fc = objPGEFeatureClass.FeatureClass;
                
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = "CGC12='" + cgcno + "'";
                IFeatureCursor fccur= fc.Search(queryFilter,  true);
                IFeature tmpfeature = null;
                tmpfeature = fccur.NextFeature();
                string sCircuitID = string.Empty;
                while (tmpfeature != null)
                {
                    if (tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                    {
                        sCircuitID = tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")).ToString();
                        if(!circuitids.Contains(sCircuitID))
                            circuitids.Add(sCircuitID);
                    }
                    tmpfeature = fccur.NextFeature();
                }
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return circuitids;
        }
        public List<TransformerDetails> GetTransformerCircuitIdsdetails(string cgcno, out string FeatureClass, out string sFeat_CGC)
        {
            List<TransformerDetails> transformers = new List<TransformerDetails>();
            FeatureClass = string.Empty;
            sFeat_CGC = string.Empty;
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();

                string Search2_EligibleClasses = objUtilityFunctions.ReadConfigurationValue("Search2_EligibleClasses"),
                    Search2_EligibleClassAttributes = objUtilityFunctions.ReadConfigurationValue("Search2_EligibleClassAttributes");

                string[] SearchClasses = Search2_EligibleClasses.Split(';'),
                    SearchAttributes = Search2_EligibleClassAttributes.Split(';');

                if (SearchClasses.Count() != SearchAttributes.Count())
                    throw new Exception("Missing or Invalid configuration parameters! Please check with your Administrator.");

                for(int index = 0; index < SearchClasses.Count(); index++)
                {
                    string sFCName = SearchClasses[index];
                    if(string.IsNullOrEmpty(sFCName))
                        continue;

                    PGEFeatureClass objPGEFeatureClass = GetFeatureClassByName(sFCName);

                    if (objPGEFeatureClass == null)
                    {
                        MessageBox.Show("Unable to retrieve " + sFCName + " data.");
                        //return transformers;
                        continue;
                    }

                    if (objPGEFeatureClass.FeatureClass == null)
                    {
                        MessageBox.Show("Unable to retrieve " + sFCName + " data.");
                        //return transformers;
                        continue;
                    }

                    IFeatureClass fc = objPGEFeatureClass.FeatureClass;

                    IQueryFilter queryFilter = new QueryFilterClass();
                    double cgcDouble = 0.0;
                    if (fc.Fields.get_Field(fc.FindField(SearchAttributes[index])).Type.Equals(esriFieldType.esriFieldTypeDouble))
                    {
                        if (double.TryParse(cgcno, out cgcDouble))
                            queryFilter.WhereClause = "DIVISION = '" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' AND " + SearchAttributes[index] + "=" + cgcno;
                        else
                            continue;
                    }
                    else if (fc.Fields.get_Field(fc.FindField(SearchAttributes[index])).Type.Equals(esriFieldType.esriFieldTypeString))
                    {
                        queryFilter.WhereClause = "DIVISION = '" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' AND " + SearchAttributes[index] + "='" + cgcno + "'";
                    }
                    IFeatureCursor fccur = fc.Search(queryFilter, true);
                    IFeature tmpfeature = null;
                    tmpfeature = fccur.NextFeature();
                    string sCircuitID = string.Empty;
                    string sGlobalId = "";
                    while (tmpfeature != null)
                    {
                        if (tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                        {
                            sFeat_CGC = tmpfeature.get_Value(tmpfeature.Fields.FindField("CGC12")).ToString();
                            sCircuitID = tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")).ToString();
                            sGlobalId = tmpfeature.get_Value(tmpfeature.Fields.FindField("GLOBALID")).ToString();
                            Int64 oidf = tmpfeature.OID;
                            if (!transformers.Exists(o => o.CIRCUITID == sCircuitID))
                            {
                                TransformerDetails td = new TransformerDetails();
                                td.CIRCUITID = sCircuitID;
                                td.GLOBALID = sGlobalId;
                                td.OID = oidf;
                                transformers.Add(td);
                            }
                        }
                        tmpfeature = fccur.NextFeature();
                    }

                    if (transformers.Count > 0)
                    {
                        FeatureClass = sFCName;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return transformers;
        }

        public string GetSelectedDivisionCode()
        {
            string code = "";
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                string fcname = objUtilityFunctions.ReadConfigurationValue("Substation");
                string fname = "DIVISION";
                //IFeatureLayer fl = GetFeatureLayerfromFCName(fcname);
                //if (fl == null)
                //{
                //    MessageBox.Show("Substation layer not present on map");
                //    return code;
                //}

                //IFeatureClass fc = fl.FeatureClass;
                PGEFeatureClass objPGEFeatureClass = GetFeatureClassByName(fcname);
                if (objPGEFeatureClass != null)
                {
                    if (objPGEFeatureClass.FeatureClass != null)
                    {
                        UtilityFunctions uf = new UtilityFunctions();
                        code = uf.ReadFieldDomaincodeforValue(objPGEFeatureClass.FeatureClass, fname, PGEGlobal.WIZARD_DIVISION);
                    }
                    objPGEFeatureClass.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return code;
        }
        public Dictionary<string, string> GetSubStations()
        {
            Dictionary<string, string> substations = new Dictionary<string, string>();
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                PGEFeatureClass objPGESSFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("Substation"));

                if (objPGESSFeatureClass == null)
                    return substations;

                if(objPGESSFeatureClass.FeatureClass == null)
                    return substations;

                IFeatureClass fc = objPGESSFeatureClass.FeatureClass;                
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = "DIVISION='" + Convert.ToInt32(PGEGlobal.WIZARD_DIVISION_CODE) + "' ORDER BY NAME";
                queryFilter.SubFields = "OBJECTID,STATIONNUMBER,NAME";
                IFeatureCursor fccur = fc.Search(queryFilter, true);
                IFeature tmpfeature = null;
                tmpfeature = fccur.NextFeature();
                string sSubstationID = string.Empty;
                string sName = string.Empty;
                while (tmpfeature != null)
                {
                    if (tmpfeature.get_Value(tmpfeature.Fields.FindField("STATIONNUMBER")) != DBNull.Value)
                    {
                        sSubstationID = tmpfeature.OID.ToString(); // + ";" + tmpfeature.get_Value(tmpfeature.Fields.FindField("SUBSTATIONID")).ToString();
                        sName = tmpfeature.get_Value(tmpfeature.Fields.FindField("STATIONNUMBER")).ToString() + "  " + tmpfeature.get_Value(tmpfeature.Fields.FindField("NAME")).ToString();
                        //Int64 oidf = tmpfeature.OID;
                        string val;
                        if (!substations.TryGetValue(sSubstationID, out val))
                        {
                            substations.Add(sSubstationID, sName);
                        }

                    }
                    tmpfeature = fccur.NextFeature();
                }
                objPGESSFeatureClass.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return substations;
        }

        private Dictionary<string, string> GetSubStationBanks(string substationid)
        {
            Dictionary<string, string> substationBanks = new Dictionary<string, string>();
            
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();

                //IFeatureLayer fLayer = GetFeatureLayerfromFCName("EDGIS.SUBTransformerBank");
                //if (fLayer == null)
                //{
                //    MessageBox.Show("SubTransformer bank layer not present on map");
                //    return substationBanks;
                //}
                //IFeatureClass fc = fLayer.FeatureClass;

                PGEFeatureClass objPGESSBankFeatClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("SUBTransformerBank"));

                if (objPGESSBankFeatClass == null)
                {
                    MessageBox.Show("Unable to retrieve SubTransformer bank data.");
                    return substationBanks;
                }

                if(objPGESSBankFeatClass.FeatureClass == null)
                {
                    MessageBox.Show("Unable to retrieve SubTransformer bank data.");
                    return substationBanks;
                }

                IFeatureClass fc = objPGESSBankFeatClass.FeatureClass;

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = "SUBSTATIONID='" + substationid + "'";
                queryFilter.SubFields = "BANKCD,CIRCUITID";
                IFeatureCursor fccur = fc.Search(queryFilter, true);
                IFeature tmpfeature = null;
                tmpfeature = fccur.NextFeature();
                string bankCode = string.Empty;
                string sName = "";
                while (tmpfeature != null)
                {
                    if (tmpfeature.get_Value(tmpfeature.Fields.FindField("BANKCD")) != DBNull.Value)
                    {
                        bankCode = tmpfeature.get_Value(tmpfeature.Fields.FindField("BANKCD")).ToString();
                        sName = tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")).ToString(); ;
                       
                        string val;
                        if (!substationBanks.TryGetValue(bankCode, out val))
                        {
                            substationBanks.Add(sName, bankCode);
                        }

                    }
                    tmpfeature = fccur.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return substationBanks;
        }

        public Dictionary<string, int> FindSpatiallyConnectedCircuits(int SubstationOID)
        {
            Dictionary<string, int> dConductors = new Dictionary<string, int>();

            int iFeatureCount = 0, val = -1;
            string CircuitID = string.Empty, sBufferValue = string.Empty;
                                
            UtilityFunctions    objUtilityFunctions = default(UtilityFunctions);
            PGEFeatureClass     objPGESSFeatureClass = default(PGEFeatureClass),
                                //obj_OHC_PGEFeatureClass = default(PGEFeatureClass),
                                //obj_UGC_PGEFeatureClass = default(PGEFeatureClass),
                                obj_ESP_PGEFeatureClass = default(PGEFeatureClass);
            IFeature            pSubstation = default(IFeature);
            ITopologicalOperator pTopoOp = default(ITopologicalOperator);
            IGeometry           pBufferedGeom = default(IGeometry);
            ISpatialFilter      pSpFilter = default(ISpatialFilter);
            IFeatureCursor      //pOHC_Cursor = default(IFeatureCursor),
                                //pUGC_Cursor = default(IFeatureCursor),
                                pESP_Cursor = default(IFeatureCursor);
            IFeature            //pOHFeature = default(IFeature),
                                //pUGFeature = default(IFeature),
                                pESPFeature = default(IFeature);

            try
            {
                iFeatureCount = 0;
                val = -1;
                CircuitID = string.Empty;
                                
                objUtilityFunctions = new UtilityFunctions();
                objPGESSFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("Substation"));

                pSubstation = default(IFeature);
                pSubstation = objPGESSFeatureClass.FeatureClass.GetFeature(SubstationOID);
                if (pSubstation != null)
                {
                    pTopoOp = default(ITopologicalOperator);
                    pTopoOp = pSubstation.ShapeCopy as ITopologicalOperator;
                    
                    sBufferValue = objUtilityFunctions.ReadConfigurationValue("Substation_Buffer");
                    if (!string.IsNullOrEmpty(sBufferValue))
                    {
                        pBufferedGeom = default(IGeometry);
                        pBufferedGeom = pTopoOp.Buffer(Convert.ToDouble(sBufferValue));

                        pSpFilter = default(ISpatialFilter);
                        pSpFilter = new SpatialFilterClass();
                        pSpFilter.SubFields = "CIRCUITID";
                        pSpFilter.Geometry = pBufferedGeom;
                        pSpFilter.WhereClause = "SUBTYPECD = 2";
                        //pSpFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        pSpFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                        obj_ESP_PGEFeatureClass = default(PGEFeatureClass);
                        obj_ESP_PGEFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("ElectricStitchPoint"));
                        if (obj_ESP_PGEFeatureClass != null)
                            if (obj_ESP_PGEFeatureClass.FeatureClass != null)
                            {
                                pESP_Cursor = obj_ESP_PGEFeatureClass.FeatureClass.Search(pSpFilter, false);
                                if (pESP_Cursor != null)
                                {
                                    pESPFeature = default(IFeature);
                                    pESPFeature = pESP_Cursor.NextFeature();
                                    while (pESPFeature != null)
                                    {
                                        if (pESPFeature.get_Value(pESPFeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                                        {
                                            CircuitID = Convert.ToString(pESPFeature.get_Value(pESPFeature.Fields.FindField("CIRCUITID")));
                                            if (!dConductors.TryGetValue(CircuitID, out val))
                                            {
                                                iFeatureCount++;
                                                dConductors.Add(CircuitID, iFeatureCount);
                                            }
                                        }

                                        pESPFeature = default(IFeature);
                                        pESPFeature = pESP_Cursor.NextFeature();
                                    }
                                }
                            }

                        //obj_OHC_PGEFeatureClass = default(PGEFeatureClass);
                        //obj_OHC_PGEFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("PriOHConductor"));
                        //if (obj_OHC_PGEFeatureClass != null)
                        //    if (obj_OHC_PGEFeatureClass.FeatureClass != null)
                        //    {
                        //        pOHC_Cursor = obj_OHC_PGEFeatureClass.FeatureClass.Search(pSpFilter, false);
                        //        if (pOHC_Cursor != null)
                        //        {
                        //            pOHFeature = default(IFeature);
                        //            pOHFeature = pOHC_Cursor.NextFeature();
                        //            while (pOHFeature != null)
                        //            {
                        //                if (pOHFeature.get_Value(pOHFeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                        //                {
                        //                    CircuitID = Convert.ToString(pOHFeature.get_Value(pOHFeature.Fields.FindField("CIRCUITID")));
                        //                    if (!dConductors.TryGetValue(CircuitID, out val))
                        //                    {
                        //                        iFeatureCount++;
                        //                        dConductors.Add(CircuitID, iFeatureCount);
                        //                    }
                        //                }

                        //                pOHFeature = default(IFeature);
                        //                pOHFeature = pOHC_Cursor.NextFeature();
                        //            }
                        //        }
                        //    }

                        //obj_UGC_PGEFeatureClass = default(PGEFeatureClass);
                        //obj_UGC_PGEFeatureClass = GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("PriUGConductor"));
                        //if (obj_UGC_PGEFeatureClass != null)
                        //    if (obj_UGC_PGEFeatureClass.FeatureClass != null)
                        //    {
                        //        pUGC_Cursor = obj_UGC_PGEFeatureClass.FeatureClass.Search(pSpFilter, false);
                        //        if (pUGC_Cursor != null)
                        //        {
                        //            pUGFeature = default(IFeature);
                        //            pUGFeature = pUGC_Cursor.NextFeature();
                        //            while (pUGFeature != null)
                        //            {
                        //                if (pUGFeature.get_Value(pUGFeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                        //                {
                        //                    CircuitID = Convert.ToString(pUGFeature.get_Value(pUGFeature.Fields.FindField("CIRCUITID")));
                        //                    if (!dConductors.TryGetValue(CircuitID, out val))
                        //                    {
                        //                        iFeatureCount++;
                        //                        dConductors.Add(CircuitID, iFeatureCount);
                        //                    }
                        //                }

                        //                pUGFeature = default(IFeature);
                        //                pUGFeature = pUGC_Cursor.NextFeature();
                        //            }
                        //        }
                        //    }
                    }

                    if (dConductors.Count > 0)
                    {
                        Dictionary<string, int> dOrderedDict = new Dictionary<string, int>();
                        dConductors = dConductors.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                        int iRecordCount = 0;
                        foreach(KeyValuePair<string, int> kvp in dConductors)
                        {
                            //dConductors[kvp.Key] = ++iRecordCount;
                            dOrderedDict.Add(kvp.Key, ++iRecordCount);
                        }

                        dConductors = new Dictionary<string, int>();
                        dConductors = dOrderedDict;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (pSubstation != null) { Marshal.ReleaseComObject(pSubstation); pSubstation = null; }


                objUtilityFunctions = default(UtilityFunctions);
                objPGESSFeatureClass.Dispose();
                obj_ESP_PGEFeatureClass.Dispose();
                //obj_OHC_PGEFeatureClass.Dispose();
                //obj_UGC_PGEFeatureClass.Dispose();

                if (pTopoOp != null) { Marshal.ReleaseComObject(pTopoOp); pTopoOp = default(ITopologicalOperator); }
                if (pBufferedGeom != null) { Marshal.ReleaseComObject(pBufferedGeom); pBufferedGeom = default(IGeometry); }
                if (pSpFilter != null) { Marshal.ReleaseComObject(pSpFilter); pSpFilter = default(ISpatialFilter); }
                //if (pOHC_Cursor != null) { Marshal.ReleaseComObject(pOHC_Cursor); pOHC_Cursor = default(IFeatureCursor); }
                //if (pUGC_Cursor != null) { Marshal.ReleaseComObject(pUGC_Cursor); pUGC_Cursor = default(IFeatureCursor); }
                //if (pOHFeature != null) { Marshal.ReleaseComObject(pOHFeature); pOHFeature = default(IFeature); }
                //if (pUGFeature != null) { Marshal.ReleaseComObject(pUGFeature); pUGFeature = default(IFeature); }
                if (pESP_Cursor != null) { Marshal.ReleaseComObject(pESP_Cursor); pESP_Cursor = default(IFeatureCursor); }
                if (pESPFeature != null) { Marshal.ReleaseComObject(pESPFeature); pESPFeature = default(IFeature); }
            }

            return dConductors;
        }

        private List<string> GetCircuitIdsforbank(string substationid, string bankcode)
        {
            List<string> circuitids = new List<string>();
            UtilityFunctions objUtilityFunctions = default(UtilityFunctions);
            

            try
            {
                objUtilityFunctions = new UtilityFunctions();
                //IFeatureLayer fLayer = GetFeatureLayerfromFCName("EDGIS.SUBTransformerBank");
                //if (fLayer == null)
                //{
                //    MessageBox.Show("SubTransformer bank layer not present on map");
                //    return circuitids;
                //}
                //IFeatureClass fc = fLayer.FeatureClass;

                DataTable dtCircuitIDs = new DataTable();
                dtCircuitIDs = objUtilityFunctions.ExecuteQuery("SELECT FEEDERID FROM " +
                            objUtilityFunctions.ReadConfigurationValue("PGE_FEEDERFEDNETWORK_TRACE") +
                            " WHERE FEEDERFEDBY = '" + bankcode + "' AND FEEDERID IS NOT NULL", objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"));

                string circuitid = string.Empty;
                if (Convert.ToBoolean(objUtilityFunctions.ReadConfigurationValue("FirstCircuitID")))
                {
                    if (dtCircuitIDs.Rows.Count > 0)
                    {
                        circuitid = Convert.ToString(dtCircuitIDs.Rows[0][0]);

                        if (!circuitids.Contains(circuitid))
                        {
                            circuitids.Add(circuitid);
                        }
                    }
                }
                else
                {
                    foreach (DataRow drCircuitID in dtCircuitIDs.Rows)
                    {
                        circuitid = Convert.ToString(drCircuitID[0]);

                        if (!circuitids.Contains(circuitid))
                        {
                            circuitids.Add(circuitid);
                        }
                    }
                }

                #region Commented by SB: 02/01/2016
                //PGEFeatureClass objPGEFeatureClass = GetFeatureClassByName("EDGIS.SUBTransformerBank");

                //if (objPGEFeatureClass == null)
                //{
                //    MessageBox.Show("Unable to retrieve SubTransformer bank data.");
                //    return circuitids;
                //}

                //if (objPGEFeatureClass.FeatureClass == null)
                //{
                //    MessageBox.Show("Unable to retrieve SubTransformer bank data.");
                //    return circuitids;
                //}

                //IFeatureClass fc = objPGEFeatureClass.FeatureClass;

                //IQueryFilter queryFilter = new QueryFilterClass();
                //queryFilter.WhereClause = "SUBSTATIONID='" + substationid + "' AND BANKCD='" + bankcode + "'";
                //IFeatureCursor fccur = fc.Search(queryFilter, true);
                //IFeature tmpfeature = null;
                //tmpfeature = fccur.NextFeature();
                //string circuitid = string.Empty;
                //DataTable dtCircuitIDs = new DataTable();

                //while (tmpfeature != null)
                //{
                //    if (tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")) != DBNull.Value)
                //    {
                //        circuitid = tmpfeature.get_Value(tmpfeature.Fields.FindField("CIRCUITID")).ToString();
                //        dtCircuitIDs = objUtilityFunctions.ExecuteQuery("SELECT FEEDERID FROM " + 
                //            objUtilityFunctions.ReadConfigurationValue("PGE_FEEDERFEDNETWORK_TRACE") +
                //            " WHERE FEEDERFEDBY = '" + circuitid + "' AND FEEDERID IS NOT NULL", objUtilityFunctions.ReadConfigurationValue("WEBR_ConnectionString"));

                //        if (Convert.ToBoolean(objUtilityFunctions.ReadConfigurationValue("FirstCircuitID")))
                //        {
                //            if (dtCircuitIDs.Rows.Count > 0)
                //            {
                //                circuitid = Convert.ToString(dtCircuitIDs.Rows[0][0]);

                //                if (!circuitids.Contains(circuitid))
                //                {
                //                    circuitids.Add(circuitid);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            foreach (DataRow drCircuitID in dtCircuitIDs.Rows)
                //            {                                
                //                circuitid = Convert.ToString(drCircuitID[0]);

                //                if (!circuitids.Contains(circuitid))
                //                {
                //                    circuitids.Add(circuitid);
                //                }
                //            }
                //        }
                //    }
                //    tmpfeature = fccur.NextFeature();
                //}

                #endregion Commented by SB: 02/01/2016
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return circuitids;
        }

        public DataTable GetCircuitID_Bank_SS_Div()
        {
            DataTable pTable_All = new DataTable();
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();

                DataTable dtParams = new DataTable();
                dtParams.Columns.Add("Param_Name", typeof(string));
                dtParams.Columns.Add("Param_Type", typeof(string));
                dtParams.Columns.Add("Param_Value", typeof(string));
                dtParams.Rows.Add(new object[] { "P_DIVISION", "OracleType.Number", PGEGlobal.WIZARD_DIVISION_CODE });


                pTable_All = objUtilityFunctions.ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                       objUtilityFunctions.ReadConfigurationValue("SUBSTATION_FEEDER"), dtParams, string.Empty);


                for (int iCount_Row = 0; iCount_Row < pTable_All.Rows.Count; ++iCount_Row)
                {
                    try
                    {
                        pTable_All.Rows[iCount_Row]["TX_BANK_CODE"] = Convert.ToString(pTable_All.Rows[iCount_Row]["TX_BANK_CODE"]).Contains("BK") ?
                          "Bank " + Convert.ToString(pTable_All.Rows[iCount_Row]["TX_BANK_CODE"]).Split(new string[] { "BK" }, StringSplitOptions.None)[1] :
                            Convert.ToString(pTable_All.Rows[iCount_Row]["TX_BANK_CODE"]);
                    }
                    catch { }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pTable_All;
        }


        public IFeatureLayer GetFeatureLayerfromFCName(string featureclassName)
        {
            IFeatureLayer fLayer = null;
            try
            {
                IMap map = (PGEGlobal.Application.Document as IMxDocument).FocusMap;
                IFeatureLayer featureLayer;
                string fcName = "";
                IEnumLayer pEnumLayer;
                ILayer pLayer;
                UID pID = new UIDClass();
                pID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                pEnumLayer = map.Layers[pID, true];
                pEnumLayer.Reset();
                pLayer = pEnumLayer.Next();
                while (pLayer != null)
                {
                    if (fLayer != null)
                    {
                        break;
                    }
                    if (!(pLayer is IFeatureLayer))
                    {
                        pLayer = pEnumLayer.Next();
                        continue;
                    }
                    featureLayer = pLayer as IFeatureLayer;
                    IFeatureClass fetureClass = featureLayer.FeatureClass;
                    if (fetureClass == null)
                    {
                        pLayer = pEnumLayer.Next();
                        continue;
                    }

                    IDataset dataSet = (IDataset)fetureClass;
                    if (dataSet == null)
                    {
                        pLayer = pEnumLayer.Next();
                        continue;
                    }

                    fcName = dataSet.Name;
                    if (fcName == null)
                    {
                        pLayer = pEnumLayer.Next();
                        continue;
                    }

                    if (fcName.ToUpper() == featureclassName.ToUpper())
                    {
                        fLayer = featureLayer;
                    }
                    pLayer = pEnumLayer.Next();
                }

                if (fLayer == null)
                {
                    // {40A9E885-5533-11d0-98BE-00805F7CED21} IFeatureLayer

                    pID = new UIDClass();
                    pID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    pEnumLayer = map.Layers[pID, true];
                    pEnumLayer.Reset();
                    pLayer = pEnumLayer.Next();
                    while (pLayer != null)
                    {
                        if (fLayer != null)
                        {
                            break;
                        }
                        if (!(pLayer is IFeatureLayer))
                        {
                            continue;
                        }
                        featureLayer = pLayer as IFeatureLayer;
                        IFeatureClass fetureClass = featureLayer.FeatureClass;
                        IDataset dataSet = (IDataset)fetureClass;
                        fcName = dataSet.Name;
                        if (fcName.ToUpper() == featureclassName.ToUpper())
                        {
                            fLayer = featureLayer;
                        }
                        pLayer = pEnumLayer.Next();
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return fLayer;

        }
    }
}
