using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using Telvent.Delivery.Diagnostics;

namespace Telvent.PGE.MapProduction.PreProcessing
{
    /// <summary>
    /// Allows users to project the feature's geometry to new coordinate system and retrieves the feature's envelope in projected coordinate system.
    /// </summary>
    public class PGEGDBDataManager : GDBDataManager
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");
        #region Private Fields

        /// <summary>
        /// Collection of spatial references
        /// </summary>
        private Dictionary<string, ISpatialReference> _spatialReference = new Dictionary<string, ISpatialReference>();

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes the new instance of <see cref=""/>
        /// </summary>
        /// <param name="workspace">Workspace reference</param>
        /// <param name="gdbFieldLookUp">GDBLookUp instance refers to the fields in Maintenance Plat feature class</param>
        public PGEGDBDataManager(IWorkspace workspace, IMPGDBFieldLookUp gdbFieldLookUp)
            : base(workspace, gdbFieldLookUp)
        {
        }

        #endregion Constructor

        #region Public Overridden Methods

        /// <summary>
        /// Populates the <paramref name="gdbData"/> with project envelope of the <paramref name="mapFeature"/>
        /// </summary>
        /// <param name="mapFeature">Feature to get the projected envelope</param>
        /// <param name="gdbData">GDBData to populate the Envelope coordinates</param>
        /// <remarks>
        /// Retrieves the projected coordinate system SRID from  <paramref name="gdbData"/> and projects the <paramref name="mapFeature"/>
        /// feature to the new corrdinate system and assigns the new envelope of the projected feature to the GDBData.
        /// If Projected coordinate system SRID is invalid then envelope of the original map feature is used.
        /// </remarks>
        public override void AddEnvelopeInfo(IFeature mapFeature, ref IMPGDBData gdbData)
        {
            string coordinate = gdbData.OtherFieldValues[PGEGDBFieldLookUp.PGEGDBFields.LegacyCorrdSystemMN];
            _logger.Debug("Coordinate System Found:" + coordinate);
            if (string.IsNullOrEmpty(coordinate))//If SRID is null, then get envelop from the original feature
            {
                _logger.Debug("Coordinate system null. Using Base AddEnvelopeInfo"); 
                base.AddEnvelopeInfo(mapFeature, ref gdbData);
                return;
            }
            else
            {
                //Get the spatial reference from the SRID
                ISpatialReference spRef = GetSpatialReference(coordinate);
                if (spRef == null)
                {
                    _logger.Debug("Spatialreference object null:" + coordinate+". Using Base AddEnvelopeInfo");
                    base.AddEnvelopeInfo(mapFeature, ref gdbData);
                    return;
                }

                //Get the Projected envelope
                IGeometry projectedGeometry = GetProjectedGemoetry(mapFeature.ShapeCopy, spRef);
                //Set the Envelope
                gdbData.XMax = projectedGeometry.Envelope.XMax;
                gdbData.XMin = projectedGeometry.Envelope.XMin;
                gdbData.YMax = projectedGeometry.Envelope.YMax;
                gdbData.YMin = projectedGeometry.Envelope.YMin;
                CollectVerticesInWKT(projectedGeometry,ref gdbData);
            }
        }

        #endregion Public Overridden Methods

        #region Private Methods

        /// <summary>
        /// Get the projected envelop of the geometry
        /// </summary>
        /// <param name="fromGeometry">Geometry to be projected</param>
        /// <param name="toSpatialReference">To Spatial reference to be used for the projection of the geometry</param>
        /// <returns>Retusn the projected envelope of the geometry</returns>
        private IGeometry GetProjectedGemoetry(IGeometry fromGeometry, ISpatialReference toSpatialReference)
        {
            IGeometry retVal = null;
            if (toSpatialReference != null)
            {
                //Create Transformation to project the geometry
                IGeoTransformation datumConversion = new NADCONTransformationClass();

                //Project to the required spatial reference
                retVal = Project(fromGeometry, toSpatialReference, datumConversion);
            }
            return retVal;

        }

        /// <summary>
        /// Retrieves the mapped spatial reference/Coordinate System for the given Coordinate
        /// </summary>
        /// <param name="coordinate">SRID of the coordinate system</param>
        /// <returns>Retrieves the Coordinate System for the given SRID</returns>
        /// <remarks>
        /// Mapped SRID for the given coordinate ID is retrieved from the config file and returns the spatial reference using SRID
        /// </remarks>
        private ISpatialReference GetSpatialReference(string coordinate)
        {
            if (!_spatialReference.ContainsKey(coordinate))
            {
                ISpatialReference spRef = null;
                if (MapProductionConfigurationHandler.Settings.ContainsKey(coordinate))
                {
                    string prjFactoryCode = MapProductionConfigurationHandler.Settings[coordinate];
                    _logger.Debug("prjFactory Code:"+prjFactoryCode);
                    ISpatialReferenceFactory spRefFactory = new SpatialReferenceEnvironmentClass();
                    spRef = spRefFactory.CreateProjectedCoordinateSystem(Convert.ToInt32(prjFactoryCode));
                    if (spRef != null)
                    {
                        _logger.Debug("SpatialReference objetc created:" + spRef.Name);
                    }
                    else
                    {
                        _logger.Debug("Spatialreference Null");
                    }
                    _spatialReference.Add(coordinate, spRef);
                }
                else
                {
                    _spatialReference.Add(coordinate, null);
                    return null;
                }
            }
            return _spatialReference[coordinate];
        }

        /// <summary>
        /// Retrieves the spatial reference/Coordinate System for the given SRID
        /// </summary>
        /// <param name="srid">SRID of the coordinate system</param>
        /// <returns>Retrieves the Coordinate System for the given SRID</returns>
        private ISpatialReference GetSpatialReference(int srid)
        {
            if (!_spatialReference.ContainsKey(srid.ToString()))
            {
                ISpatialReference spRef = null;
                ISpatialReferenceFactory spRefFactory = new SpatialReferenceEnvironmentClass();
                spRef = spRefFactory.CreateProjectedCoordinateSystem(srid);
                _spatialReference.Add(srid.ToString(), spRef);
            }
            return _spatialReference[srid.ToString()];
        }

        /// <summary>
        /// Projects the Geometry to another Spatial reference using specified transformation
        /// </summary>
        /// <param name="geometryToProject">Geometry to be projected</param>
        /// <param name="toSpatialReference">Target Spatial reference</param>
        /// <param name="transformation">Transformation to be used to project the geometry</param>
        /// <returns>Returns the geometry in new coordinate system projected using the given transformation</returns>
        private IGeometry Project(IGeometry geometryToProject, ISpatialReference toSpatialReference, ITransformation transformation)
        {
            IGeometry2 geom2 = (IGeometry2)geometryToProject;
            geom2.ProjectEx(toSpatialReference, esriTransformDirection.esriTransformReverse, transformation as IGeoTransformation, false, 0, 0);
            return (IGeometry)geom2;
        }

        #endregion Private Methods
    }
}