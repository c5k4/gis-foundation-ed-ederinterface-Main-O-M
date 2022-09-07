using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.BatchApplication.ED;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Base implementation of the IMGDBDataManager interface that helps to interact with the Geodatabase that holds the Map Polygon tobe used for Map Production
    /// </summary>
    public class GDBDataManager : IMPGDBDataManager, IDisposable
    {
        #region Private Members
        private IFeatureClass _priOHConductorFeatureclass = null;
        private int _circuitIdFldIdx = -1;
        private int _circuitNameFldIdx = -1;
        private int _pspsNameFldIdx = -1;
        private Dictionary<string, int> _otherModelNameFldIdx = null;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes an instance of the IMPGDBDataManager object with the given Workspace and IMPGDBFieldLookUpObject
        /// </summary>
        /// <param name="workspace">IWorkspace to use to get the Map Polygon featurecalss</param>
        /// <param name="gdbFieldLookUp">Definition of the Map Polygon featurecalss</param>
        public GDBDataManager(IWorkspace workspace, IMPGDBFieldLookUp gdbFieldLookUp)
        {
            Workspace = workspace;
            GDBFieldLookUp = gdbFieldLookUp;
        }
        #endregion

        #region Public Methods

        #region IMPGDBDataManager Implementation
        /// <summary>
        /// Data structure of the Map Polygon featureclass
        /// </summary>
        public virtual IMPGDBFieldLookUp GDBFieldLookUp
        {
            get;
            private set;
        }
        /// <summary>
        /// The IFeatureclass instance of the Map Polygon featureclass from the given workspace.
        /// The IMPGDBFieldLook.MapPolygonModelName property is used to get the featureclass with the given modelname
        /// </summary>
        public virtual IFeatureClass PriOHConductorFeatureClass
        {
            get
            {
                if (_priOHConductorFeatureclass == null)
                {
                    _priOHConductorFeatureclass = ModelNameFacade.FeatureClassByModelName(Workspace, GDBFieldLookUp.PriOHConductorModelName);
                }
                return _priOHConductorFeatureclass;
            }
            private set
            {
                _priOHConductorFeatureclass = value;
            }
        }

        /// <summary>
        /// The field index of the field defined by IMPGDBFieldLookUp.CircuitIdModelName property
        /// </summary>
        public virtual int CircuitIdFieldIndex
        {
            get
            {
                if (_circuitIdFldIdx == -1)
                {
                    _circuitIdFldIdx = ModelNameFacade.FieldIndexFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.PriOHConductorModelName);
                }
                return _circuitIdFldIdx;
            }
        }
        /// <summary>
        /// The field index of the field defined by IMPGDBFieldLookUp.CircuitNameModelName property
        /// </summary>
        public virtual int CircuitNameFieldIndex
        {
            get
            {
                if (_circuitNameFldIdx == -1)
                {
                    _circuitNameFldIdx = ModelNameFacade.FieldIndexFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.CircuitNameModelName);
                }
                return _circuitNameFldIdx;
            }
        }
        /// <summary>
        /// The field index of the field defined by IMPGDBFieldLookUp.PSPSSegmentNameModelName property
        /// </summary>
        public virtual int PSPSNameFieldIndex
        {
            get
            {
                if (_pspsNameFldIdx == -1)
                {
                    _pspsNameFldIdx = ModelNameFacade.FieldIndexFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.PSPSSegmentNameModelName);
                }
                return _pspsNameFldIdx;
            }
        }
        /// <summary>
        /// The field indices of the fields defined by IMPGDBFieldLookUp.OtherfieldsModelName property
        /// </summary>
        public virtual Dictionary<string, int> OtherFieldIndices
        {
            get
            {
                if (_otherModelNameFldIdx == null)
                {
                    _otherModelNameFldIdx = new Dictionary<string, int>();
                    foreach (string modelName in GDBFieldLookUp.OtherFieldModelNames)
                    {
                        _otherModelNameFldIdx.Add(modelName, ModelNameFacade.FieldIndexFromModelName(PriOHConductorFeatureClass, modelName));
                    }
                }
                return _otherModelNameFldIdx;
            }
        }

        /// <summary>
        /// Get a List of IMPGDBData from the Workspace using the IMPGDBFieldLookUp object
        /// </summary>
        /// <param name="filterKeys">List of Key to be used to filter the result</param>
        /// <returns>A List of IMPGDBData filled with all required data as defined by the IMPGDBData using the field mapping as defined by the IMPGDBFieldLookUp</returns>
        public virtual List<IMPGDBData> GetMapData(params string[] filterKeys)
        {
            bool isBulk = false;
            if (filterKeys[0] == "BULK") { isBulk = true; }
            List<string> circuitIds = new List<string>();
            if (!isBulk)
            {
                foreach (string key in filterKeys)
                {
                    string[] keySplit = key.Split(':');
                    circuitIds.Add(keySplit[0]);
                }
            }

            IFeatureCursor featCursor = null;
            IFeature mapFeature = null;
            try
            {
                string fieldNames = "";
                Dictionary<string, int> modelNameIndexes = new Dictionary<string, int>();
                fieldNames += ModelNameFacade.FieldFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.CircuitIdModelName).Name + ",";
                fieldNames += ModelNameFacade.FieldFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.CircuitNameModelName).Name + ",";
                fieldNames += ModelNameFacade.FieldFromModelName(PriOHConductorFeatureClass, GDBFieldLookUp.PSPSSegmentNameModelName).Name + ",";
                modelNameIndexes.Add(GDBFieldLookUp.CircuitIdModelName, CircuitIdFieldIndex);
                modelNameIndexes.Add(GDBFieldLookUp.CircuitNameModelName, CircuitNameFieldIndex);
                modelNameIndexes.Add(GDBFieldLookUp.PSPSSegmentNameModelName, PSPSNameFieldIndex);

                fieldNames += PriOHConductorFeatureClass.ShapeFieldName;
                string whereClause = "PSPS_SEGMENT is not null and PSPS_SEGMENT != 'A/N'";
                if (circuitIds.Count > 0)
                {
                    whereClause += string.Format(" and CIRCUITID in ('{0}')", string.Join("','", circuitIds.ToArray()));
                }

                DateTime startTime = System.DateTime.Now;
                Console.WriteLine("Starting obtaining map data: " + System.DateTime.Now);
                List<IMPGDBData> retVal = new List<IMPGDBData>();
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.SubFields = fieldNames;

                //remove this extra filter, finally. Adding jus for debugging less amount of data
                queryFilter.WhereClause += whereClause;
                // query the domain values
                IField pspsNameField = PriOHConductorFeatureClass.Fields.get_Field(PSPSNameFieldIndex);
                IDictionary<string, string> pspsSegmentDomainDict = GetDomainDictionary(pspsNameField.Domain);

                featCursor = PriOHConductorFeatureClass.Search(queryFilter, true);
                string otherFieldValue = string.Empty;
                while ((mapFeature = featCursor.NextFeature()) != null)
                {
                    IMPGDBData gdbData = new MPGDBData();
                    gdbData.PSPSName = pspsSegmentDomainDict[gdbData.PSPSName];

                    gdbData.OtherFieldValues = new Dictionary<string, string>();
                    foreach (string modelName in GDBFieldLookUp.OtherFieldModelNames)
                    {
                        otherFieldValue = GetDomainValue(mapFeature, modelNameIndexes[modelName]);
                        //if (!string.IsNullOrEmpty(otherFieldValue))
                        //{
                        gdbData.OtherFieldValues.Add(modelName, otherFieldValue);
                        //}
                    }
                    gdbData.KeyFieldValues = new Dictionary<string, string>();
                    foreach (string keyField in GDBFieldLookUp.KeyFieldModelNames)
                    {
                        if (gdbData.OtherFieldValues.ContainsKey(keyField))
                        {
                            gdbData.KeyFieldValues.Add(keyField, gdbData.OtherFieldValues[keyField]);
                        }
                    }
                    AddEnvelopeInfo(mapFeature, ref gdbData);
                    if ((filterKeys.Count() > 0) && !isBulk)
                    {
                        if (gdbData.IsValid && filterKeys.Contains(gdbData.Key))
                            retVal.Add(gdbData);
                    }
                    else
                    {
                        if (gdbData.IsValid)
                        { retVal.Add(gdbData); }
                    }
                }
                TimeSpan ts = System.DateTime.Now - startTime;
                Console.WriteLine("Finished obtaining map data: " + System.DateTime.Now);
                return retVal;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during execution: Message: " + e.Message + " StackTrace: " + e.StackTrace);
                throw e;
            }
            finally
            {
                if (featCursor != null)
                {
                    while (Marshal.ReleaseComObject(featCursor) > 0) { };
                }
                if (mapFeature != null)
                {
                    while (Marshal.ReleaseComObject(mapFeature) > 0) { };
                }
            }
        }
        #endregion

        public string GetDomainValue(IObject feature, int fieldIndex)
        {
            string featureValue = feature.get_Value(fieldIndex).ToString();
            string result = featureValue;

            try
            {
                IField pField = feature.Fields.get_Field(fieldIndex);
                ICodedValueDomain pCodedValueDomain = null;
                IDomain pDomain = pField.Domain;

                //Check for domain type
                if (pDomain.Type == esriDomainType.esriDTCodedValue)
                {
                    pCodedValueDomain = pDomain as ICodedValueDomain;
                    
                    //Get coded value name and value and check with given value
                    for (int j = 0; j < pCodedValueDomain.CodeCount; j++)
                    {
                        string codedValueName = pCodedValueDomain.get_Name(j);
                        string codedValueValue = pCodedValueDomain.get_Value(j).ToString();

                        if (codedValueValue.ToString().Equals(featureValue))
                        {
                            result = codedValueName;
                            break;
                        }
                    }
                }
            }
            catch (Exception e) { }

            return result;
        }

        private IDictionary<string, string> GetDomainDictionary(IDomain domain)
        {
            IDictionary<string, string> domainDict = new Dictionary<string, string>();
            ICodedValueDomain codedDomain = domain as ICodedValueDomain;

            for (int i = 0; i < codedDomain.CodeCount - 1; i++)
            {
                domainDict.Add(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i));
            }
            return domainDict;
        }
        /// <summary>
        /// Give a Map Polygon Feature will populate the XMin,YMin,XMax and YMax field values in the given IMPGDBData usign the Extent property of the Feature's shape
        /// </summary>
        /// <param name="mapFeature">IFeature defining the Map polygon</param>
        /// <param name="gdbData">IMPGDBData that should be populated with the Envelope information</param>
        public virtual void AddEnvelopeInfo(IFeature mapFeature, ref IMPGDBData gdbData)
        {
            IEnvelope envelope = mapFeature.Shape.Envelope;
            gdbData.XMin = envelope.XMin;
            gdbData.YMin = envelope.YMin;
            gdbData.XMax = envelope.XMax;
            gdbData.YMax = envelope.YMax;
            CollectVerticesInWKT(mapFeature.ShapeCopy, ref gdbData);
        }

        public virtual void CollectVerticesInWKT(IGeometry geometry, ref IMPGDBData gdbData)
        {
            IPoint point = null;
            int outIndex = -1;
            int outRingIndex = -1;
            IPointCollection ptCollection = null;
            IEnumVertex enumVertex = null;
            string wkbPart = "(";
            try
            {
                ptCollection = (IPointCollection)geometry;
                enumVertex = ptCollection.EnumVertices;
                enumVertex.Reset();
                enumVertex.Next(out point, out outRingIndex, out outIndex);
                while (point != null)
                {
                    if (enumVertex.IsLastInPart())
                    {
                        wkbPart+=point.X + " " + point.Y +")";
                        ////gdbData.PolygonCoordinates.Add(wkbPart);
                        wkbPart = "(";
                    }
                    else
                    {
                        wkbPart += point.X + " " + point.Y + ",";
                    }
                    enumVertex.Next(out point, out outRingIndex, out outIndex);
                }
            }
            finally
            {
                if (ptCollection != null)
                {
                    while (Marshal.ReleaseComObject(ptCollection) > 0) { } 
                }
                if (enumVertex != null)
                {
                    while (Marshal.ReleaseComObject(enumVertex) > 0) { }
                }
            }
        }

        /// <summary>
        /// Retrieve the feature class from an SDE Oracle connection file and the feature class name.
        /// </summary>
        /// <param name="sdeConnectionFile">SDE Connection file</param>
        /// <param name="layerName">Layer Name</param>
        /// <returns>FeatureClass</returns>
        public IFeatureClass GetGDBLayer(string sdeConnectionFile, string layerName)
        {
            IFeatureClass featureClass = null;
            IWorkspaceFactory workspaceFactory = null;
            IFeatureWorkspace destinationWorkspace = null;

            try
            {
                workspaceFactory = new SdeWorkspaceFactoryClass();
               
                destinationWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(ReadEncryption.GetSDEPath(sdeConnectionFile), 0);
                featureClass = destinationWorkspace.OpenFeatureClass(layerName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(destinationWorkspace);
            }

            return featureClass;
        }

        /// <summary>
        /// IWorkspace to use to perform the Operation
        /// </summary>
        public IWorkspace Workspace
        {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// Dispose data stored in memoery at the end of process.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
