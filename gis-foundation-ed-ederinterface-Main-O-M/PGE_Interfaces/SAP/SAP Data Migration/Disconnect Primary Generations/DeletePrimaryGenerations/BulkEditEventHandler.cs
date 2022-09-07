using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Geodatabase.FeederManager
{
    internal class BulkEditEventHandler
    {
        private IMMFeederExt _feederExt;

        internal IMMElectricTraceSettings TraceSettings
        {
            get;
            private set;
        }

        internal IMMElectricTracing2 FeederTracer
        {
            get;
            private set;
        }

        internal IGeometricNetwork GeometricNetwork
        {
            get;
            set;
        }

        private IMMFeederExt FeederExt
        {
            get
            {
                if (this._feederExt == null)
                {
                    IExtensionManager extensionManager = Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.ExtensionManager")) as IExtensionManager;
                    this._feederExt = (extensionManager.FindExtension("mmFramework.MMFeederExt") as IMMFeederExt);
                }
                return this._feederExt;
            }
        }

        public BulkEditEventHandler()
        {
            this.FeederTracer = new MMFeederTracerClass();
            this.TraceSettings = new MMElectricTraceSettingsClass();
        }

        public bool BeforeEdits(IFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }
            HashSet<IFeature> hashSet = null;
            hashSet = this.GetCoincidentFeatures(hashSet, feature);
            if (!this.FeederManagerAUsEnabled())
            {
                return false;
            }
            this.TraceFeedersBeforeEdits(hashSet);
            return true;
        }

        public bool BeforeEdits(IEnumerable<IFeature> electricFeatures)
        {
            HashSet<IFeature> hashSet = new HashSet<IFeature>();
            foreach (IFeature current in electricFeatures)
            {
                hashSet = this.GetCoincidentFeatures(hashSet, current);
            }
            if (!this.FeederManagerAUsEnabled())
            {
                return false;
            }
            this.TraceFeedersBeforeEdits(hashSet);
            return true;
        }

        public bool AfterEdits(IFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }
            IEnumerable<int> sourcesForFeature = this.GetSourcesForFeature(feature);
            if (!this.FeederManagerAUsEnabled())
            {
                return false;
            }
            this.UpdateFeedersAfterEdits(sourcesForFeature);
            return true;
        }

        public bool AfterEdits(IEnumerable<IFeature> electricFeatures)
        {
            HashSet<int> hashSet = new HashSet<int>();
            foreach (IFeature current in electricFeatures)
            {
                IEnumerable<int> sourcesForFeature = this.GetSourcesForFeature(current);
                if (sourcesForFeature != null)
                {
                    foreach (int current2 in sourcesForFeature)
                    {
                        hashSet.Add(current2);
                    }
                }
            }
            if (!this.FeederManagerAUsEnabled())
            {
                return false;
            }
            this.UpdateFeedersAfterEdits(hashSet);
            return true;
        }

        public void UpdateFeederLevels(IEnumerable<IFeature> features, mmEditEvent editEvent)
        {
            IMMSpecialAUStrategyEx subsourceLvlAU = new MMFeederSubSourceLevelAUClass() as IMMSpecialAUStrategyEx;
            foreach (IFeature current in features)
            {
                this.UpdateFeederLevels(current, subsourceLvlAU, editEvent);
            }
        }

        public void UpdateFeederLevels(IFeature feature, IMMSpecialAUStrategyEx subsourceLvlAU, mmEditEvent editEvent)
        {
            if (feature != null && subsourceLvlAU != null)
            {
                subsourceLvlAU.Execute(feature, mmAutoUpdaterMode.mmAUMFeederManager, editEvent);
            }
        }

        public void Abort()
        {
            Utility.SignalConnectOrDisconnectInProgress(false);
        }

        private HashSet<IFeature> GetCoincidentFeatures(HashSet<IFeature> featureSet, IFeature feature)
        {
            if (featureSet == null)
            {
                featureSet = new HashSet<IFeature>();
            }
            INetworkFeature electricNetworkFeature = this.GetElectricNetworkFeature(feature);
            if (electricNetworkFeature != null)
            {
                HashSet<IFeature> coincidentFeatures = Utility.GetCoincidentFeatures(feature, this.GeometricNetwork);
                foreach (IFeature current in coincidentFeatures)
                {
                    featureSet.Add(current);
                }
            }
            return featureSet;
        }

        private void TraceFeedersBeforeEdits(HashSet<IFeature> features)
        {
            if (features == null)
            {
                return;
            }
            int[] junctionEIDs = Utility.GetJunctionEIDs(features);
            IEnumerable<int> sourcesForEIDs = this.GetSourcesForEIDs(junctionEIDs);
            Utility.SignalConnectOrDisconnectInProgress(true);
            IMMFeederBulkEditing2 iMMFeederBulkEditing = (IMMFeederBulkEditing2)this.FeederTracer;
            iMMFeederBulkEditing.TraceFeedersBeforeEdits(this.GeometricNetwork, this.TraceSettings, null, sourcesForEIDs.ToArray<int>());
        }

        private void UpdateFeedersAfterEdits(IEnumerable<int> sources)
        {
            if (sources != null)
            {
                try
                {
                    IMMFeederBulkEditing2 iMMFeederBulkEditing = (IMMFeederBulkEditing2)this.FeederTracer;
                    iMMFeederBulkEditing.UpdateFeedersAfterEdits(this.GeometricNetwork, this.TraceSettings, null, sources.ToArray<int>());
                }
                finally
                {
                    Utility.SignalConnectOrDisconnectInProgress(false);
                }
            }
        }

        public bool FeederManagerAUsEnabled()
        {
            return this.FeederManagerAUsEnabled(this.GeometricNetwork);
        }

        private bool FeederManagerAUsEnabled(IGeometricNetwork network)
        {
            bool result = false;
            if (network != null)
            {
                IMMFeederAdmin iMMFeederAdmin = this.FeederExt as IMMFeederAdmin;
                if (iMMFeederAdmin != null)
                {
                    IMMFeederSettings iMMFeederSettings = iMMFeederAdmin.get_FeederSettings(network, false);
                    result = iMMFeederSettings.TracingAutoupdatersEnabled;
                }
            }
            return result;
        }

        private IEnumerable<int> GetSourcesForFeature(IFeature feature)
        {
            if (this.GetElectricNetworkFeature(feature) == null)
            {
                return null;
            }
            int[] eids = new int[]
			{
				Utility.GetSingleJunctionEID(feature)
			};
            return this.GetSourcesForEIDs(eids);
        }

        private IEnumerable<int> GetSourcesForEIDs(IEnumerable<int> eids)
        {
            HashSet<int> hashSet = new HashSet<int>();
            if (eids != null)
            {
                foreach (int current in eids)
                {
                    if (this.IsValidJunctionEID(current))
                    {
                        int[] barrierJunctions = new int[0];
                        int[] barrierEdges = new int[0];
                        IMMFeedPath[] array;
                        IMMTraceStopper[] array2;
                        IMMTraceStopper[] array3;
                        this.FeederTracer.FindFeedPaths(this.GeometricNetwork, this.TraceSettings, null, current, esriElementType.esriETJunction, SetOfPhases.abc, barrierJunctions, barrierEdges, out array, out array2, out array3);
                        if (array != null)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                IMMFeedPath iMMFeedPath = array[i];
                                if (iMMFeedPath != null && iMMFeedPath.PathElements != null && iMMFeedPath.PathElements.Length > 0)
                                {
                                    int eID = iMMFeedPath.PathElements[iMMFeedPath.PathElements.Length - 1].EID;
                                    hashSet.Add(eID);
                                }
                            }
                        }
                    }
                }
            }
            return hashSet;
        }

        private bool IsValidJunctionEID(int eid)
        {
            bool result = false;
            if (this.GeometricNetwork != null)
            {
                INetElements netElements = (INetElements)this.GeometricNetwork.Network;
                result = netElements.IsValidElement(eid, esriElementType.esriETJunction);
            }
            return result;
        }

        private INetworkFeature GetElectricNetworkFeature(IFeature feature)
        {
            INetworkFeature networkFeature = feature as INetworkFeature;
            if (networkFeature != null)
            {
                IGeometricNetwork geometricNetwork = networkFeature.GeometricNetwork;
                if (this.GeometricNetwork == null)
                {
                    if (!NetworkIdentification.IsElectric(geometricNetwork))
                    {
                        networkFeature = null;
                    }
                    else
                    {
                        this.SetNetwork(networkFeature);
                    }
                }
                else if (NetworkIdentification.QualifiedName(this.GeometricNetwork) != NetworkIdentification.QualifiedName(geometricNetwork))
                {
                    networkFeature = null;
                }
            }
            return networkFeature;
        }

        private void SetNetwork(INetworkFeature result)
        {
            this.GeometricNetwork = result.GeometricNetwork;
            IMMFeederAdmin iMMFeederAdmin = this.FeederExt as IMMFeederAdmin;
            if (iMMFeederAdmin != null)
            {
                IMMFeederSettings iMMFeederSettings = iMMFeederAdmin.get_FeederSettings(this.GeometricNetwork, false);
                IMMFeederSettings2 iMMFeederSettings2 = iMMFeederSettings as IMMFeederSettings2;
                this.TraceSettings.RespectConductorPhasing = iMMFeederSettings.UseConductorPhasing;
                this.TraceSettings.RespectEnabledField = iMMFeederSettings2.RespectEnabledField;
            }
        }
    }
}