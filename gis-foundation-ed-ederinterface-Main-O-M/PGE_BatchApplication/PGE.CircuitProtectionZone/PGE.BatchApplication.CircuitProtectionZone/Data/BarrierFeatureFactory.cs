using System;
using System.Collections.Generic;
using System.Configuration;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.BatchApplication.CircuitProtectionZone.Util;
using System.Runtime.InteropServices;
using PGE.BatchApplication.CircuitProtectionZone.Common;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    internal class BarrierFeatureFactory
    {

        public BarrierFeatureFactory()
        {
        }

        public BarrierFeature Create(IFeature feature)
        {
            try
            {
                string featureClassName = EsriUtil.GetFeatureClassName(feature);
                object recloserValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.ReloserFieldName);
                object globalIdValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.GlobalIdFieldName);
                object operatingNumberValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.OperatingNumberFieldName);
                string operatingNumberString = operatingNumberValue == null ? string.Empty : operatingNumberValue.ToString();
                object statusValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.StatusFieldName);
                object normalPositionValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.NormalPositionFieldName);
                object customerOwnedValue = EsriUtil.GetFieldValue(feature, Properties.Settings.Default.CustomerOwnedFieldName);

                int status;
                if (Int32.TryParse(statusValue.ToString(), out status) == false)
                {
                    status = -1;
                }

                int position;
                if (Int32.TryParse(normalPositionValue.ToString(), out position) == false)
                {
                    position = -1;
                }

                bool customerOwned = false;
                if (customerOwnedValue != DBNull.Value && customerOwnedValue != null && customerOwnedValue.ToString() == "Y") { customerOwned = true; }

                if (feature != null)
                {
                    return new BarrierFeature
                    {
                        FeatureClassName = featureClassName,
                        ObjectId = feature.OID,
                        FeatureClassId = feature.Class.ObjectClassID,
                        IsRecloser = (recloserValue.ToString() == Properties.Settings.Default.ReloserFieldValue),
                        GlobalId = new Guid(globalIdValue.ToString()),
                        OperatingNumber = operatingNumberString,
                        Status = status,
                        NormalPosition = position,
                        CustomerOwned = customerOwned
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to create BarrierFeature.", ex, true);
                throw ex;
            }

            return null;
        }

        internal List<BarrierFeature> Create(IEnumNetEID junctionElements, INetwork network, string featureClassName)
        {
            List<BarrierFeature> barrierFeatures = new List<BarrierFeature>();

            IFeatureClass featureClass = featureClass = Geodatabase.GetFeatureClass(featureClassName);

            if (featureClass != null)
            {
                var netElements = (INetElements)network;

                junctionElements.Reset();
                int eid = junctionElements.Next();

                while (eid > 0)
                {
                    BarrierFeature barrierFeature = null;

                    int userClassId = 0;
                    int userId = -1;
                    int userSubId = 0;

                    netElements.QueryIDs(eid, esriElementType.esriETJunction, out userClassId, out userId,
                                            out userSubId);

                    if (userId > -1)
                    {
                        if (userClassId == featureClass.FeatureClassID)
                        {
                            IFeature feature = featureClass.GetFeature(userId);
                            if (feature != null)
                            {
                                barrierFeature = Create(feature);
                                if (feature != null) { while (Marshal.ReleaseComObject(feature) > 0) { } }
                            }
                        }
                    }

                    if (barrierFeature != null)
                    {
                        barrierFeatures.Add(barrierFeature);
                    }

                    eid = junctionElements.Next();
                }
            }

            return barrierFeatures;
        }

        internal BarrierFeature Create(int startEid, INetwork network, string featureClassName)
        {
            BarrierFeature barrierFeature = null;

            IFeatureClass featureClass = featureClass = Geodatabase.GetFeatureClass(featureClassName);

            if (featureClass != null)
            {
                var netElements = (INetElements)network;

                if (startEid > 0)
                {
                    int userClassId = 0;
                    int userId = -1;
                    int userSubId = 0;
                    netElements.QueryIDs(startEid, esriElementType.esriETJunction, out userClassId, out userId,
                                            out userSubId);

                    if (userId > -1)
                    {
                        if (userClassId == featureClass.FeatureClassID)
                        {
                            IFeature feature = featureClass.GetFeature(userId);
                            if (feature != null)
                            {
                                barrierFeature = Create(feature);
                                if (feature != null) { while (Marshal.ReleaseComObject(feature) > 0) { } }
                            }
                        }
                    }
                }
            }

            return barrierFeature;
        }

        internal List<IMMNetworkFeatureID> Create(string feederId, string featureClassName)
        {
            List<IMMNetworkFeatureID> networkFeatureIds = new List<IMMNetworkFeatureID>();

            IFeatureClass featureClass = featureClass = Geodatabase.GetFeatureClass(featureClassName);

            if (featureClass != null)
            {
                string whereClause = FilterUtil.CreateSimpleWhereClause(featureClass, "CIRCUITID", feederId);
                IQueryFilter queryFilter = FilterUtil.CreateQueryFilter(featureClass, whereClause);

                IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
                IFeature feature = featureCursor.NextFeature();
                while (feature != null)
                {
                    networkFeatureIds.Add(new MMNetworkFeatureIDClass { FCID = featureClass.FeatureClassID, OID = feature.OID });
                    while (Marshal.ReleaseComObject(feature) > 0) { }
                    feature = featureCursor.NextFeature();
                }
                if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
                if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0) { } }
            }

            return networkFeatureIds;
        }
    }
}
