using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Geodatabase.AutoUpdaters;
using Miner.Geodatabase.CathodicProtection;
using Miner.Geodatabase.FeederManager;
using Miner.Geodatabase.Network;
using Miner.Interop;
using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.Edit
{
    public class Disconnect
    {
        internal static bool isDisconnecting;

        public void DisconnectFeature(IFeature electricOrGasFeature)
        {
            if (electricOrGasFeature == null)
            {
                return;
            }
            INetworkFeature networkFeature = electricOrGasFeature as INetworkFeature;
            if (networkFeature == null)
            {
                return;
            }
            if (!Editor.IsOperationInProgress())
            {
                throw new InvalidOperationException("An edit operation must be started to disconnect feature.");
            }
            if (NetworkIdentification.IsElectric(networkFeature.GeometricNetwork))
            {
                this.DisconnectElectricFeature(electricOrGasFeature, networkFeature);
                return;
            }
        }

        private void DisconnectElectricFeature(IFeature feature, INetworkFeature networkFeature)
        {
            BulkEditEventHandler bulkEditEventHandler = new BulkEditEventHandler();
            bulkEditEventHandler.BeforeEdits(feature);
            try
            {
                Disconnect.isDisconnecting = true;
                FeatureKey key = new FeatureKey(feature.Class.ObjectClassID, feature.OID);
                IWorkspace workspace = feature.Class.GetWorkspace();
                //FeederWorkspaceExtension instance = FeederWorkspaceExtension.GetInstance(new ConnectionProperties(workspace));
                //instance.ClearCacheForFeature(key);
                networkFeature.Disconnect();
                if (bulkEditEventHandler.FeederManagerAUsEnabled())
                {
                    IEnumerable<IFeature> coincidentJunctions = Utility.GetCoincidentJunctions(feature, networkFeature.GeometricNetwork);
                    IMMSpecialAUStrategyEx subsourceLvlAU = new MMFeederSubSourceLevelAUClass() as IMMSpecialAUStrategyEx;
                    foreach (IFeature current in coincidentJunctions)
                    {
                        bulkEditEventHandler.UpdateFeederLevels(current, subsourceLvlAU, mmEditEvent.mmEventFeatureDelete);
                    }
                    bulkEditEventHandler.AfterEdits(feature);
                    this.FlagIslandsAfterFeatureDisconnected(feature, bulkEditEventHandler.TraceSettings);
                }
            }
            finally
            {
                Disconnect.isDisconnecting = false;
            }
        }



        private bool FlagIslandsAfterFeatureDisconnected(IFeature feature, IMMElectricTraceSettings traceSettings)
        {
            if (feature == null)
            {
                return false;
            }
            INetworkFeature networkFeature = feature as INetworkFeature;
            if (networkFeature == null)
            {
                return false;
            }
            bool result = false;
            Utility.SignalConnectOrDisconnectInProgress(true);
            HashSet<IFeature> coincidentFeatures = Utility.GetCoincidentFeatures(feature, networkFeature.GeometricNetwork);
            int[] junctionEIDs = Utility.GetJunctionEIDs(coincidentFeatures);
            try
            {
                IMMFeederBulkEditing2 iMMFeederBulkEditing = new MMFeederTracerClass() as IMMFeederBulkEditing2;
                iMMFeederBulkEditing.FlagIslandFeatures(networkFeature.GeometricNetwork, traceSettings, null, junctionEIDs);
                result = true;
            }
            finally
            {
                Utility.SignalConnectOrDisconnectInProgress(false);
            }
            return result;
        }
    }
}