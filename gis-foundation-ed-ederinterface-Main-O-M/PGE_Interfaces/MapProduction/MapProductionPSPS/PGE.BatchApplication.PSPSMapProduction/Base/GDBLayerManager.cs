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
using System.Data;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Base implementation of the IMGDBLayerManagerinterface that helps to read circuit data from sde geodatabase
    /// </summary>
    public class GDBLayerManager : IMPGDBLayerManager, IDisposable
    {
        #region Private Members
        private string _sdeConnectionFile = "";
        private string _pocFeatureClassName = "";
        private IFeatureClass _mapPrimaryOHConductorFeatureclass = null;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes an instance of the IMPGDBLayerManager object with the given SDE connection file and feature class name
        /// </summary>
        /// <param name="sdeConnectionFile">SDE Connection File</param>
        /// <param name="conductorFCName">Primary OH Conductor Feature Class Name</param>
        public GDBLayerManager(string sdeConnectionFile, string conductorFCName)
        {
            _sdeConnectionFile = sdeConnectionFile;
            _pocFeatureClassName = conductorFCName;
        }
        #endregion

        #region Public Methods

        public string SdeConnectionFile
        {
            get
            {
                return _sdeConnectionFile;
            }
        }

        public string FeatureClassName
        {
            get
            {
                return _pocFeatureClassName;
            }
        }
        
        /// <summary>
        /// The IFeatureclass instance of the Primary OH Conductor featureclass from the given sde connection.
        /// </summary>
        public virtual IFeatureClass ConductorFeatureClass
        {
            get
            {
                if (_mapPrimaryOHConductorFeatureclass == null)
                {
                    IWorkspaceFactory workspaceFactory = null;
                    IFeatureWorkspace destinationWorkspace = null;

                    try
                    {
                        workspaceFactory = new SdeWorkspaceFactoryClass();
                        
                        destinationWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(ReadEncryption.GetSDEPath(_sdeConnectionFile), 0);
                        _mapPrimaryOHConductorFeatureclass = destinationWorkspace.OpenFeatureClass(_pocFeatureClassName);
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

                }
                return _mapPrimaryOHConductorFeatureclass;
            }

            private set
            {
                _mapPrimaryOHConductorFeatureclass = value;
            }
        }

        /// <summary>
        /// Get a List of IMPGDBData from the Workspace using the IMPGDBFieldLookUp object
        /// </summary>
        /// <param name="filterKeys">List of Key to be used to filter the result</param>
        /// <returns>A List of IMPGDBData filled with all required data as defined by the IMPGDBData using the field mapping as defined by the IMPGDBFieldLookUp</returns>
        public List<PSPSSegmentRecord> GetLayerData(string fieldNames, string circuitIds)
        {
            ISpatialFilter spatialFilter = null;
            IFeatureCursor aoCursor = null;
            int mapId = 1;

            try
            {
                // quewry FGDB instead of Oracle database
                // Create the query filter.
                spatialFilter = new SpatialFilterClass();
                // Select the fields to be returned - the name and address of the businesses.
                spatialFilter.SubFields = fieldNames + "," + ConductorFeatureClass.ShapeFieldName + ",OBJECTID";
                // Set the filter to return only restaurants.
                if (circuitIds != "")
                {
                    spatialFilter.WhereClause = string.Format("PSPS_SEGMENT is not null AND PSPS_SEGMENT != 'N/A' AND CIRCUITID in ('{0}')", String.Join("','", circuitIds.Split(',')));
                }
                else
                {
                    spatialFilter.WhereClause = "PSPS_SEGMENT is not null AND PSPS_SEGMENT != 'N/A'";
                }


                // Use the PostfixClause to alphabetically order the set by name. Although this
                // has no effect on a File geodatabase, this will work for Personal and SDE
                // geodatabases.
                IQueryFilterDefinition queryFilterDef = (IQueryFilterDefinition)spatialFilter;
                queryFilterDef.PostfixClause = "ORDER BY CIRCUITNAME,PSPS_SEGMENT";

                int total = ConductorFeatureClass.FeatureCount(spatialFilter);
                aoCursor = ConductorFeatureClass.Search(spatialFilter, true);
                IFeature aoFeature = null;
                List<PSPSSegmentRecord> pspsSegments = new List<PSPSSegmentRecord>();
                int pspsSegmentIndex = aoCursor.Fields.FindField("PSPS_SEGMENT");
                int circuitNameIndex = aoCursor.Fields.FindField("CIRCUITNAME");
                int circuitIdIndex = aoCursor.Fields.FindField("CIRCUITID");
                int objIdIndex = aoCursor.Fields.FindField("OBJECTID");
                string prevPSPSSegmentName = "";
                string prevCircuitId = "";
                string prevCircuitName = "";
                string circuitCircuitName = "";
                //PSPSSegmentRecord currSegmentRow = null;
                int currCircuitSegmentStart = 0;
                int layoutType = MapLayoutType.PortraitSegmentMap;
                int numberSplitMaps = 0;
                bool splitVertical = false;
                string currCircuitId = "";
                double currTotalMileages = 0.0;
                IEnvelope currExtent = null;
                IEnvelope lastExtent = null;
                IField pspsNameField = ConductorFeatureClass.Fields.get_Field(pspsSegmentIndex);
                // query the domain values
                IDictionary<string, string> pspsSegmentDomainDict = GetDomainDictionary(pspsNameField.Domain);

                aoFeature = aoCursor.NextFeature();

                while (aoFeature != null)
                {
                    string tmpPSPSSegmentName = pspsSegmentDomainDict[aoFeature.Value[pspsSegmentIndex].ToString()];
                    currCircuitId = aoFeature.Value[circuitIdIndex].ToString();
                    circuitCircuitName = aoFeature.Value[circuitNameIndex].ToString();

                    if (currExtent == null)
                    {
                        currExtent = new EnvelopeClass() as IEnvelope;
                        currExtent.PutCoords(aoFeature.Extent.XMin, aoFeature.Extent.YMin, aoFeature.Extent.XMax, aoFeature.Extent.YMax);
                    }
                    else if (prevCircuitId.Equals(currCircuitId) && prevPSPSSegmentName.Equals(tmpPSPSSegmentName))
                    {
                        currExtent.Union(aoFeature.Extent);
                    }
                    else
                    {
                        lastExtent = aoFeature.Extent;
                    }
                    currTotalMileages += CalculateSegmentLength(aoFeature.ShapeCopy);
                    aoFeature = aoCursor.NextFeature();

                    if (aoFeature == null || prevCircuitId != "" && !prevCircuitId.Equals(currCircuitId) || prevPSPSSegmentName != "" && !prevPSPSSegmentName.Equals(tmpPSPSSegmentName))
                    {
                        if (aoFeature == null || prevPSPSSegmentName != "" && !prevPSPSSegmentName.Equals(tmpPSPSSegmentName))
                        {
                            layoutType = GetLayoutType(currExtent, true, out numberSplitMaps, out splitVertical);
                            if (numberSplitMaps > 0)
                            {
                                mapId = SplitMaps(pspsSegments, currExtent, prevCircuitId, prevCircuitName, prevPSPSSegmentName, numberSplitMaps, splitVertical, layoutType, currTotalMileages, mapId);
                            }
                            else
                            {
                                AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, prevPSPSSegmentName, layoutType, currTotalMileages, currExtent, mapId++, 1);
                            }
                        }
                        if (aoFeature == null || prevCircuitId != "" && !prevCircuitId.Equals(currCircuitId))
                        {
                            // add overview map row
                            if (currCircuitSegmentStart < pspsSegments.Count)
                            {
                                currExtent = null;
                                // add the Overview Map Row
                                for (int i = currCircuitSegmentStart; i < pspsSegments.Count; i++)
                                {
                                    if (currExtent == null)
                                    {
                                        currExtent = new EnvelopeClass() as IEnvelope;
                                        currExtent.PutCoords(pspsSegments[i].Extent.XMin, pspsSegments[i].Extent.YMin, pspsSegments[i].Extent.XMax, pspsSegments[i].Extent.YMax);
                                    }
                                    else
                                    {
                                        currExtent.Union(pspsSegments[i].Extent);
                                    }
                                }
                                layoutType = GetLayoutType(currExtent, false, out numberSplitMaps, out splitVertical);
                                AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, "", layoutType, 0.0, currExtent, mapId++, 2);
                            }
                            else // single page Circuit map - add Segment map and Overview map
                            {
                                // add overview
                                layoutType = GetLayoutType(currExtent, false, out numberSplitMaps, out splitVertical);
                                AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, "", layoutType, 0.0, currExtent, mapId++, 2);
                                // add segment
                                layoutType = GetLayoutType(currExtent, true, out numberSplitMaps, out splitVertical);
                                if (numberSplitMaps > 0)
                                {
                                    mapId = SplitMaps(pspsSegments, currExtent, prevCircuitId, prevCircuitName, prevPSPSSegmentName, numberSplitMaps, splitVertical, layoutType, currTotalMileages, mapId);
                                }
                                else
                                {
                                    AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, prevPSPSSegmentName, layoutType, currTotalMileages, currExtent, mapId++, 1);
                                }
                            }
                            currCircuitSegmentStart = pspsSegments.Count;

                        }

                        currExtent = lastExtent;
                        if (lastExtent != null)
                        {
                            currExtent = new EnvelopeClass() as IEnvelope;
                            currExtent.PutCoords(lastExtent.XMin, lastExtent.YMin, lastExtent.XMax, lastExtent.YMax);
                        }
                        currTotalMileages = 0.0;
                    }
                    prevPSPSSegmentName = tmpPSPSSegmentName;
                    prevCircuitId = currCircuitId;
                    prevCircuitName = circuitCircuitName;
                }

                return pspsSegments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (aoCursor != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(aoCursor);
                }
                if (spatialFilter != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(spatialFilter);
                }
            }

        }
        
        #endregion

        /// <summary>
        /// Dispose data stored in memoery at the end of process.
        /// </summary>
        public void Dispose()
        {
            if (ConductorFeatureClass != null)
            {
                while (Marshal.ReleaseComObject(ConductorFeatureClass) > 0) { };
                ConductorFeatureClass = null;
            }
        }

        #region Private members

        private double CalculateSegmentLength(IGeometry segment)
        {
            // US Feet to mile
            return (((IPolyline)segment).Length / 5280.0);
        }

        private void AddMapRecord(List<PSPSSegmentRecord> pspsSegments, string circuitId, string circuitName, string pspsName, int layoutType, double mileages, IEnvelope env, int mapId, int mapType)
        {
            pspsSegments.Add(new PSPSSegmentRecord()
            {
                CircuitId = circuitId,
                CircuitName = circuitName,
                PSPSSegmentName = pspsName,
                LayoutIndex = layoutType,
                TotalMilage = mileages,
                Extent = env,
                MapId = mapId,
                MapType = mapType
            });
        }

        private int GetLayoutType(IEnvelope extent, bool isSegment, out int numberSplitMaps, out bool splitVertical)
        {
            int layoutType = MapLayoutType.PortraitSegmentMap;
            numberSplitMaps = 0;
            splitVertical = false;

            // get miles
            double width = extent.Width / 5280.0;
            double height = extent.Height / 5280.0;
            if (isSegment)
            {
                if (width < 8.25 && height < 11.66)
                {
                    if (width > height)
                    {
                        layoutType = MapLayoutType.LandscapeSegmentMap;
                    }
                    else
                    {
                        layoutType = MapLayoutType.PortraitSegmentMap;
                    }
                }
                else
                {
                    // need to calculate how many maps to split
                    if (width - 8.25 > height - 11.66)
                    {
                        splitVertical = false;
                        numberSplitMaps = (int) (width / 8.25) + 1;
                        if (width / 2.0 > height)
                        {
                            layoutType = MapLayoutType.LandscapeSegmentMap;
                        }
                        else {
                            layoutType = MapLayoutType.PortraitSegmentMap;
                        }
                    }
                    else
                    {
                        splitVertical = true;
                        numberSplitMaps = (int)(height / 11.66) + 1;
                        if (width > height / 2.0)
                        {
                            layoutType = MapLayoutType.LandscapeSegmentMap;
                        }
                        else
                        {
                            layoutType = MapLayoutType.PortraitSegmentMap;
                        }
                    }
                }
            }
            else
            {
                if (width > height)
                {
                    layoutType = MapLayoutType.LandscapeOverviewMap;
                }
                else
                {
                    layoutType = MapLayoutType.PortraitOverviewMap;
                }
            }
            return layoutType;
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

        private int SplitMaps(List<PSPSSegmentRecord> pspsSegments, IEnvelope currExtent, string prevCircuitId, string prevCircuitName, string prevPSPSSegmentName, int numberSplitMaps, bool splitVertical, int layoutType, double currTotalMileages, int mapId)
        {
            for (int i = 0; i < numberSplitMaps; i++)
            {
                if (splitVertical)
                {
                    IEnvelope env1 = new EnvelopeClass() as IEnvelope;
                    double deltaHeight = currExtent.Height / numberSplitMaps;
                    double overlapDeltaHeightMin = (i == 0) ? 0.0 : deltaHeight * 0.02;
                    double overlapDeltaHeightMax = (i == numberSplitMaps - 1) ? 0.0 : deltaHeight * 0.02;
                    env1.PutCoords(currExtent.XMin, currExtent.YMin + (i * deltaHeight) - overlapDeltaHeightMin, currExtent.XMax, currExtent.YMin + (i + 1) * deltaHeight + overlapDeltaHeightMax);

                    AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, String.Format("{0}{1}", prevPSPSSegmentName, i + 1), layoutType, currTotalMileages, env1, mapId++, 1);
                }
                else
                {
                    double deltaWidth = currExtent.Width / numberSplitMaps;
                    double overlapDeltaWidthMin = (i == 0) ? 0.0 : deltaWidth * 0.02;
                    double overlapDeltaWidthMax = (i == numberSplitMaps - 1) ? 0.0 : deltaWidth * 0.02;
                    IEnvelope env2 = new EnvelopeClass() as IEnvelope;
                    env2.PutCoords(currExtent.XMin + (i * deltaWidth) - overlapDeltaWidthMin, currExtent.YMin, currExtent.XMin + (i + 1) * deltaWidth + overlapDeltaWidthMax, currExtent.YMax);
                    AddMapRecord(pspsSegments, prevCircuitId, prevCircuitName, String.Format("{0}{1}", prevPSPSSegmentName, i + 1), layoutType, currTotalMileages, env2, mapId++, 1);
                }
            }
            return mapId;
        }

        #endregion Private members
    }
}
