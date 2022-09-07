using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.UFM
{
    public class UfmSyncDuctAttrHelper
    {
        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // To keep track of which fields need sync'ing
        IList<String> _syncFields = null;

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the duct features from the supplied duct banks that match the supplied duct definition record
        /// with any values from the duct definition record that have been marked for synching.
        /// </summary>
        /// <param name="ductBanks"></param>
        /// <param name="ductDefinition"></param>
        public void UpdateDuctInDuctBanks(ISet ductBanks, IMMDuctDefinition ductDefinition)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            try
            {
                // Get the duct number for the current definition
                int ductNumber = ductDefinition.ductNumber;

                // And retreive the corresponding duct features representing this duct
                IList<IFeature> ductFeatures = GetDuctFeatures(ductBanks, ductNumber);

                // If some were found...
                if (ductFeatures.Count > 0)
                {
                    // Get the list of releted items for the duct
                    ID8List ductAttributes = ductDefinition as ID8List;
                    ductAttributes.Reset();

                    // While there are more items
                    ID8ListItem attr = ductAttributes.Next(false);
                    while (attr != null)
                    {
                        // If the related item is an attribute
                        if (attr is IMMAttribute)
                        {
                            // Get the attribute
                            IMMAttribute attrib = attr as IMMAttribute;

                            // If the attribute is one that we need to copy across...
                            if (FieldMarkedForUpdate(ductFeatures, attrib) == true)
                            {
                                // Copy the value to the duct features
                                UpdateDuctFeatures(ductFeatures, attrib);
                            }
                        }

                        // Get the next related item
                        attr = ductAttributes.Next(false);
                    }

                    // Store any edits made to the features
                    foreach (IFeature feature in ductFeatures)
                    {
                        feature.Store();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to sync duct: " + ex.ToString());
            }
        }

        /// <summary>
        /// Updates the duct feature with any values from the duct definition record that have 
        /// been marked for synching.
        /// </summary>
        /// <param name="duct"></param>
        /// <param name="ductDefinition"></param>
        public void UpdateDuct(IFeature duct, IMMDuctDefinition ductDefinition)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            try
            {
                IList<IFeature> ductFeatures = new List<IFeature>();
                ductFeatures.Add(duct);

                // Get the list of releted items for the duct
                ID8List ductAttributes = ductDefinition as ID8List;
                ductAttributes.Reset();

                // While there are more items
                ID8ListItem attr = ductAttributes.Next(false);
                while (attr != null)
                {
                    // If the related item is an attribute
                    if (attr is IMMAttribute)
                    {
                        // Get the attribute
                        IMMAttribute attrib = attr as IMMAttribute;

                        // If the attribute is one that we need to copy across...
                        if (FieldMarkedForUpdate(ductFeatures, attrib) == true)
                        {
                            // Copy the value to the duct features
                            UpdateDuctFeatures(ductFeatures, attrib);
                        }
                    }

                    // Get the next related item
                    attr = ductAttributes.Next(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to sync duct: " + ex.ToString());
            }
        }

        #endregion

        #region Private methods

        private IList<IFeature> GetDuctFeatures(ISet ductBanks, int ductPosition)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Create an empty list
            IList<IFeature> ductFeatures = new List<IFeature>();

            try
            {
                // While there are more duct banks in the set
                ductBanks.Reset();
                IFeature ductBank = ductBanks.Next() as IFeature;
                while (ductBank != null)
                {
                    // Get the duct feature from the current duct bank for the supplied position
                    IFeature duct = GetDuct(ductBank, ductPosition);

                    // If it existed, add it to the list
                    if (duct != null)
                    {
                        ductFeatures.Add(duct);
                    }

                    // And move to the next duct bank
                    ductBank = ductBanks.Next() as IFeature;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get ducts from duct banks: " + ex.ToString());
            }

            // Return the result
            return ductFeatures;
        }

        private IFeature GetDuct(IFeature ductBank, int ductNumber)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume we won't find one
            IFeature duct = null;

            // We'll need a cursor
            IFeatureCursor ductCursor = null;

            try
            {
                // Get the duct FC
                IFeatureClass ductFc = ModelNameFacade.FeatureClassByModelName((ductBank.Class as IDataset).Workspace,
                                                                                SchemaInfo.UFM.ClassModelNames.UfmDuct);

                if (ductFc != null)
                {
                    // Create the spatial filter - we want to check all ducts within the duct bank...
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = ductBank.Shape;
                    spatialFilter.GeometryField = ductFc.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                    // ... that match the supplied duct position
                    spatialFilter.WhereClause = "DUCTPOSITION=" + ductNumber.ToString();

                    // Execute the query.
                    ductCursor = ductFc.Search(spatialFilter, true);
                }

                // Get the first feature found (should only be one)
                if (ductCursor != null)
                {
                    duct = ductCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to find duct from duct bank: " + ex.ToString());
            }
            finally
            {
                // Clean up
                Marshal.ReleaseComObject(ductCursor);
            }

            // Return the result
            return duct;
        }

        private bool FieldMarkedForUpdate(IList<IFeature> ductFeatures, IMMAttribute ductAttribute)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume it isn't marked
            bool marked = false;

            // If we don't already have a list of sync fields
            if (_syncFields == null)
            {
                // Get the feature class
                IObjectClass oc = ductFeatures[0].Class;
                _syncFields = ModelNameFacade.FieldNamesFromModelName(oc, SchemaInfo.UFM.FieldModelNames.DuctSync);
            }

            // Is the supplied attribute marked for syncing
            marked = _syncFields.Contains(ductAttribute.Name);

            // Return the result
            return marked;
        }

        private void UpdateDuctFeatures(IList<IFeature> ductFeatures, IMMAttribute ductAttribute)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            try
            {
                // For each duct feature in the list...
                foreach (IFeature duct in ductFeatures)
                {
                    // Find the matching field in the duct feature
                    int fieldIndex = duct.Fields.FindField(ductAttribute.Name);

                    // And update it with the value from the blob
                    if (fieldIndex > 0)
                    {
                        duct.set_Value(fieldIndex, ductAttribute.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update ducts: " + ex.ToString());
            }
        }

        #endregion
    }
}
