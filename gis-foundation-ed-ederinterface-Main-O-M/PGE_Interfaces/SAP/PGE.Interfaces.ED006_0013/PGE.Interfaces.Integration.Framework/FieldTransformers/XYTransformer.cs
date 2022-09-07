using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using ESRI.ArcGIS.Geodatabase;


using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Exceptions;
using ESRI.ArcGIS.Geometry;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.Integration.Framework.FieldTransformers
{
    /// <summary>
    /// Field Transformer that finds a rows GIS X or Y coordinate in WGC
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public partial class XYTransformer : BaseFieldTransformer
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public XYTransformer()
        {
        }

        private string fieldNameField;
        private int _decimals;
        private string valueField;

        /// <summary>
        /// The name of the field to return, either X or Y
        /// </summary>
        [XmlAttributeAttribute()]
        public string FieldName
        {
            get
            {
                return this.fieldNameField;
            }
            set
            {
                this.fieldNameField = value;
            }
        }
        [XmlAttributeAttribute()]
        public int Decimals
        {
            get
            {
                return this._decimals;
            }
            set
            {
                this._decimals = value;
            }
        }
        /// <summary>
        /// Not used
        /// </summary>
        [XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        #region IConfigurable Members
        /// <summary>
        /// Initializes the node so it has the proper FieldName
        /// </summary>
        /// <param name="config">The XML that contains the config</param>
        /// <returns>True if successful otherwise false</returns>
        public override bool Initialize(XmlNode config)
        {
            XYTransformer typeMapper = base.DeserializeXML<XYTransformer>(config.OuterXml);
            this.FieldName = typeMapper.FieldName;
            this.Value = typeMapper.Value;
            this.Decimals = typeMapper.Decimals;
            if (this.Decimals == 0)
            {
                this.Decimals = 10;
            }
            return true;
        }

        #endregion

        #region IFieldTransformer Members
        /// <summary>
        /// Given a row, returns its GIS Coordinates
        /// </summary>
        /// <typeparam name="T">The type to be returned, i.e. String, Decimal. etc</typeparam>
        /// <param name="row">The row to analyze</param>
        /// <returns>Either the X or Y coordinate based on configuration</returns>
        public override T GetValue<T>(IRow row)
        {
            T val = default(T);
            IFeature feat = (IFeature)row;

            if (feat.Shape == null)
            {
                throw new InvalidFieldTranformationException(string.Format("Row {0} for {1} could not have field {2} determined due to null shape", row.OID, ((IDataset)row.Table).Name, FieldName), null);
            }

            if (!(feat.Shape is IPoint))
            {
                // exception or message as return value so the process continues?
                throw new InvalidConfigurationException(string.Format("Field {0} for row {1} not found in table {2}.", FieldName, row.OID, ((IDataset)row.Table).Name));
            }

            //comment this if they do not want WGS lat/lon
            IPoint reproj = ProjectPoint((IPoint)feat.Shape);
            //uncomment this if they want state plane coordinates
            //IPoint reproj = (IPoint)feat.Shape;
            double powValue = Math.Pow(10, _decimals);
            switch (FieldName.ToUpper())
            {
                case "X":
                    
                    double x = Math.Truncate(reproj.X * powValue) / powValue;
                    return (T)Convert.ChangeType(x, typeof(T));
                case "Y":
                    double y = Math.Truncate(reproj.Y * powValue) / powValue;
                    return (T)Convert.ChangeType(y, typeof(T));
            }

            return val;
        }

        /// <summary>
        /// Given a row, returns its GIS Coordinates
        /// </summary>
        /// <typeparam name="T">The type to be returned, i.e. String, Decimal. etc</typeparam>
        /// <param name="row">The row to analyze</param>
        /// <returns>Either the X or Y coordinate based on configuration</returns>
        public override T GetValue<T>(DeleteFeat row,ITable FCName)
        {
            T val = default(T);
            //IFeature feat = (IFeature)row;

            if (row.geometry_Old == null)
            {
                throw new InvalidFieldTranformationException(string.Format("Row {0} for {1} could not have field {2} determined due to null shape", row.OID, ((IDataset)FCName).Name, FieldName), null);
            }

            if (!(row.geometry_Old is IPoint))
            {
                // exception or message as return value so the process continues?
                throw new InvalidConfigurationException(string.Format("Field {0} for row {1} not found in table {2}.", FieldName, row.OID, ((IDataset)FCName).Name));
            }

            //comment this if they do not want WGS lat/lon
            IPoint reproj = ProjectPoint((IPoint)row.geometry_Old);
            //uncomment this if they want state plane coordinates
            //IPoint reproj = (IPoint)feat.Shape;
            double powValue = Math.Pow(10, _decimals);
            switch (FieldName.ToUpper())
            {
                case "X":
                    Console.WriteLine("X"+reproj.X);
                    double x = Math.Truncate(reproj.X * powValue) / powValue;
                    Console.WriteLine("X new" + x);
                    return (T)Convert.ChangeType(x, typeof(T));
                case "Y":
                    Console.WriteLine("Y:"+reproj.Y);
                    double y = Math.Truncate(reproj.Y * powValue) / powValue;
                    Console.WriteLine("Y new" + y);
                    return (T)Convert.ChangeType(y, typeof(T));
            }

            return val;
        }

        /// <summary>
        /// Returns a new point with WGS lat/lon coordinates
        /// </summary>
        /// <param name="thePoint"></param>
        /// <returns></returns>
        /// <remarks>In performance testing this method increased this transformers percent of the overall process from 0.06 to 0.13 wich is still not significant</remarks>
        public static IPoint ProjectPoint(IPoint thePoint)
        {

            int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;

            ESRI.ArcGIS.Geometry.ISpatialReferenceFactory pSRF = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();

            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem m_GeographicCoordinateSystem;

            m_GeographicCoordinateSystem = pSRF.CreateGeographicCoordinateSystem(geoType);

            ESRI.ArcGIS.Geometry.IPoint pPoint = new ESRI.ArcGIS.Geometry.PointClass();
            pPoint.X = thePoint.X;
            pPoint.Y = thePoint.Y;
            pPoint.SpatialReference = thePoint.SpatialReference;
            pPoint.Project(m_GeographicCoordinateSystem);

            return pPoint;
        }

        #endregion
    }

}
