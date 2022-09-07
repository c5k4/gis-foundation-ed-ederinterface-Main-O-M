using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.ED;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;

namespace PGE.Interfaces.MapProduction
{
    /// <summary>
    /// Base implementation of the IMGDBDataManager interface that helps to interact with the Geodatabase that holds the Map Polygon tobe used for Map Production
    /// </summary>
    public class GDBDataManager : IMPGDBDataManager, IDisposable
    {
        #region Private Members
        private IFeatureClass _mapPolygonFeatureclass = null;
        private int _mapNumberFldIdx = -1;
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
        public virtual IFeatureClass MapPolygonFeatureClass
        {
            get
            {
                if (_mapPolygonFeatureclass == null)
                {
                    _mapPolygonFeatureclass = ModelNameFacade.FeatureClassByModelName(Workspace, GDBFieldLookUp.MapPolygonModelName);
                }
                return _mapPolygonFeatureclass;
            }
            private set
            {
                _mapPolygonFeatureclass = value;
            }
        }

        /// <summary>
        /// The field index of the field defined by IMPGDBFieldLookUp.MapNumberModelName property
        /// </summary>
        public virtual int MapNumberFieldIndex
        {
            get
            {
                if (_mapNumberFldIdx == -1)
                {
                    _mapNumberFldIdx = ModelNameFacade.FieldIndexFromModelName(MapPolygonFeatureClass, GDBFieldLookUp.MapNumberModelName);
                }
                return _mapNumberFldIdx;
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
                        _otherModelNameFldIdx.Add(modelName, ModelNameFacade.FieldIndexFromModelName(MapPolygonFeatureClass, modelName));
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
            List<string> mapNumbers = new List<string>();
            if (!isBulk)
            {
                foreach (string key in filterKeys)
                {
                    string[] keySplit = key.Split(':');
                    mapNumbers.Add(keySplit[0]);
                }
            }

            IFeatureCursor featCursor = null;
            IFeature mapFeature = null;
            try
            {
                string fieldNames = "";
                Dictionary<string, int> modelNameIndexes = new Dictionary<string, int>();
                fieldNames += ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, GDBFieldLookUp.MapNumberModelName).Name + ",";
                fieldNames += ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, GDBFieldLookUp.MapScaleModelName).Name + ",";
                modelNameIndexes.Add(GDBFieldLookUp.MapNumberModelName, MapPolygonFeatureClass.FindField(ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, GDBFieldLookUp.MapNumberModelName).Name));
                modelNameIndexes.Add(GDBFieldLookUp.MapScaleModelName, MapPolygonFeatureClass.FindField(ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, GDBFieldLookUp.MapScaleModelName).Name));



                foreach (string modelName in GDBFieldLookUp.OtherFieldModelNames)
                {
                    modelNameIndexes.Add(modelName, MapPolygonFeatureClass.FindField(ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, modelName).Name));
                    fieldNames += ModelNameFacade.FieldFromModelName(MapPolygonFeatureClass, modelName).Name + ",";













                }

                fieldNames += MapPolygonFeatureClass.ShapeFieldName;

                DateTime startTime = System.DateTime.Now;
                Console.WriteLine("Starting obtaining map data: " + System.DateTime.Now);
                List<IMPGDBData> retVal = new List<IMPGDBData>();
                IQueryFilter queryFilter = new QueryFilterClass();
                if (GDBFieldLookUp.Subtype != null && GDBFieldLookUp.Subtype.Count > 0)
                {
                    string whereClause = ((ISubtypes)MapPolygonFeatureClass).SubtypeFieldName + " in ({0})";
                    string subtypes = string.Empty;
                    foreach (int subtype in GDBFieldLookUp.Subtype)
                    {
                        subtypes = string.IsNullOrEmpty(subtypes) ? subtype.ToString() : subtypes + "," + subtype;
                    }
                    whereClause = string.Format(whereClause, subtypes);
                    queryFilter.WhereClause = string.IsNullOrEmpty(subtypes) ? string.Empty : whereClause;
                }

                queryFilter.SubFields = fieldNames;

                ////remove this extra filter, finally. Adding jus for debugging less amount of data
                //queryFilter.WhereClause += " and objectid<2";

                featCursor = MapPolygonFeatureClass.Search(queryFilter, true);
                string otherFieldValue = string.Empty;
                while ((mapFeature = featCursor.NextFeature()) != null)
                {
                    IMPGDBData gdbData = new MPGDBData();
                    gdbData.MapNumber = GetDomainValue(mapFeature, modelNameIndexes[GDBFieldLookUp.MapNumberModelName]);

                    //Check if this map number exists in our filter first.  If the count is 0 then this is a bulk load
                    if (!isBulk && !mapNumbers.Contains(gdbData.MapNumber)) { continue; }

                    gdbData.MapScale = Int32.Parse(GetDomainValue(mapFeature, modelNameIndexes[GDBFieldLookUp.MapScaleModelName]));
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
                        wkbPart += point.X + " " + point.Y + ")";
                        gdbData.PolygonCoordinates.Add(wkbPart);
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
            if (MapPolygonFeatureClass != null)
            {
                while (Marshal.ReleaseComObject(MapPolygonFeatureClass) > 0) { };
                MapPolygonFeatureClass = null;
            }
        }
    }
}