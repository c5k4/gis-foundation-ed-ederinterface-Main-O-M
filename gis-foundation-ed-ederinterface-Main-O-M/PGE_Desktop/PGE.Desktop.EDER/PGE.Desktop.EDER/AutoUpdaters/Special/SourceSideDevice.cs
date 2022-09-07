using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System.Reflection;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// A special AU that propagates the Operating Number.
    /// </summary>
    [ComVisible(true)]
    [Guid("20DFD68F-B3D1-4373-A809-18B53CC0288B")]
    [ProgId("PGE.Desktop.EDER.SourceSideDevice")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public partial class SourceSideDevice : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private Dictionary<int, IFeatureClass> _featureClassIDs = null;
        private List<int> _SSDFeatureClassIDs = null;
        private IFeatureClass m_SwitchFeatureClass = null;
        private int m_SwitchTypeFldIndex = -1;
        private List<int> _upstreamJuncs = null;
        private List<int> _upstreamEdges = null;
        private static string _warningMsgNoModelName = "Model name: {0} not found.";
        private List<int> _nonSSDSwitchType = new List<int>(new int[] { 7, 8, 99 });
        private const string _switchTpeFieldName = "SwitchType";
        private List<int> _barrierEdgeClassIDs = null;
        #endregion

        #region Constructors

        /// <summary>
        /// constructor for the source side device AU
        /// </summary>
        public SourceSideDevice()
            : base("PGE Source Side Device")
        {
        }

        #endregion

        #region Override
        /// <summary>
        /// Determines if the AU can execute
        /// </summary>
        /// <param name="eAUMode"></param>
        /// <returns></returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode != mmAutoUpdaterMode.mmAUMNoEvents);
        }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool benabled = false;
            try
            {
                var hasSsdId = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);

                var isSourceSideDevice = hasSsdId
                                         && ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.SourceSideDevice)
                                         && ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.OperatingNumber);

                // Non-SourceSideDevice setup
                // Field Models: PGE_SOURCESIDEDEVICEID
                // AU assignment: OnCreate, OnUpdate
                if (hasSsdId && !isSourceSideDevice
                    && (eEvent == mmEditEvent.mmEventFeatureCreate
                        || eEvent == mmEditEvent.mmEventFeatureUpdate))
                {
                    benabled = true;
                }
                // SourceSideDevice setup
                // Object Models: PGE_SOURCESIDEDEVICE
                // Field Models: PGE_SOURCESIDEDEVICEID, PGE_OPERATINGNUMBER
                // AU assignment: OnCreate, OnUpdate, OnDelete
                else if (isSourceSideDevice
                    && (eEvent == mmEditEvent.mmEventFeatureDelete
                        || eEvent == mmEditEvent.mmEventFeatureCreate
                        || eEvent == mmEditEvent.mmEventFeatureUpdate))
                {
                    benabled = true;
                }

            }
            catch { }
            return benabled;
        }

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            #region Check events
            bool isCreate = eEvent == mmEditEvent.mmEventFeatureCreate;
            bool isUpdate = eEvent == mmEditEvent.mmEventFeatureUpdate;
            bool isDelete = eEvent == mmEditEvent.mmEventFeatureDelete;
            #endregion

            //Let's first check to determine if the circuit ID and circuit ID 2 fields are null.  If they are null we should
            //do nothing.  This should greatly improves ArcFM Disconnects on the system.
            int circuitID = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.FeederID);
            string currentCircuitID = obj.get_Value(circuitID).ToString();
            if (String.IsNullOrEmpty(currentCircuitID))
            {
                return;
            }

            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            mmAutoUpdaterMode currentAUMode = eAUMode;
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

            try
            {
                //set feature classes to dictionary
                InitFeatureClassIDs(obj);
                if (m_SwitchFeatureClass == null)//if feature class is null
                {
                    _logger.Warn(_Name + ": No feature class with the model name Switch and field model name PGE_SourceSideDevice was found.");
                    return;
                }

                // check if this feature has the field model names of interest
                // this may not be necessary since this is taken care of in the InternalEnabled
                int sourceSideDeviceIDIdx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                if (sourceSideDeviceIDIdx == -1) //if field index is null
                {
                    _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                    return;
                }
                object operatingNumber = null;

                bool isSwitch = false;
                bool normalPostionChanged = false;
                bool isSSD = IsObjectSSD(obj, out isSwitch);
                bool isOpen = IsOpen(obj, out normalPostionChanged);

                

                _upstreamJuncs = new List<int>();
                _upstreamEdges = new List<int>();

                IGeometricNetwork geomNetwork = (obj as INetworkFeature).GeometricNetwork;
                int startEID = TraceFacade.GetFeatureEID(obj as IFeature);//GetFeatureEID(obj as IFeature, esriElementType.esriETJunction);
                object upstreamOperatingNumber = null;

                // is this a create, update, or delete?
                #region On Create
                if (isCreate)
                {
                    // bug 1458: if this is an open SSD, the source side device ID should be blank or null
                    if (isSSD && isOpen)
                    {
                        return;
                    }
                    // trace upstream and get the correct feature
                    IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                    if (upstreamFeature == null)
                    {
                        _logger.Debug(_Name + ": No upsteam device found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                        return;
                    }
                    // propogate the upstream feature operating number field onto this feature's source side device ID field
                    CopyUpsteamOperatingNumber(upstreamFeature, obj, sourceSideDeviceIDIdx, out upstreamOperatingNumber);
                    if (upstreamOperatingNumber == null)
                    {
                        _logger.Debug(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the upstream feature class : " + ((IDataset)upstreamFeature.Class).Name);
                        return;
                    }

                    if (isSSD)
                    {
                        operatingNumber = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                        if (operatingNumber == DBNull.Value)//if null
                        {
                            _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                            return;
                        }
                        // propogate the operating number onto the source side device ID downstream 
                        PushOperatingNumberDownstream(obj, startEID, geomNetwork, operatingNumber, upstreamFeature);
                    }
                }
                #endregion

                #region On Update
                if (isUpdate)
                {
                    // bug 1458: if this is an open SSD, the source side device ID should be blank or null
                    if (isSSD)
                    {
                        if (isOpen && HasBeenConnected(obj))
                        {
                            FieldInstance sourceSideDeviceID = obj.FieldInstanceFromModelName(SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                            sourceSideDeviceID.Value = DBNull.Value;
                            return;
                        }
                    }
                    
                    IRowChanges rowChange = obj as IRowChanges;
                    if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
                    {
                        if (isSSD || isSwitch)
                        {
                            // did the operating number change?
                            int operatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                            if (operatingNumberIdx == -1)
                            {
                                _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }
                            //Just checking if OperatingNumbre has changed is not enough./ In case of Switches we need to check if the SwitchType has changed  making it a SourceSideDevice
                            if (UpdateRequired(obj) || normalPostionChanged)
                            {
                                IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                                int upstreamOperatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(upstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                                if (upstreamFeature == null)
                                {
                                    _logger.Debug(_Name + ": No upsteam device found for object with OID :  " + obj.OID + " from the object class: " + ((IDataset)obj.Class).Name);
                                    return;
                                }
                                if (normalPostionChanged)
                                {
                                    // propogate the upstream feature operating number field onto this feature's source side device ID field
                                    CopyUpsteamOperatingNumber(upstreamFeature, obj, sourceSideDeviceIDIdx, out upstreamOperatingNumber);
                                    if (upstreamOperatingNumber == null)
                                    {
                                        _logger.Debug(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the upstream feature class : " + ((IDataset)upstreamFeature.Class).Name);
                                        return;
                                    }
                                }

                                object operatingNumberToPush = null;
                                // propogate the operating number onto the source side device ID downstream 
                                if (isSSD) { operatingNumberToPush = obj.get_Value(operatingNumberIdx); }
                                if (!isSSD && isSwitch) { operatingNumberToPush = upstreamFeature.get_Value(upstreamOperatingNumberIdx); }
                                _logger.Debug("OperatingNumber: " + operatingNumber + ". Upstream Operating Number: " + upstreamOperatingNumber);
                                PushOperatingNumberDownstream(obj, startEID, geomNetwork, operatingNumberToPush, upstreamFeature);
                            }
                        }
                        else
                        {
                            //This AU is being moved to a subtask on update and create.  So we should always execute.
                            
                            //int shapeIdx = obj.Fields.FindField(((IFeatureClass)obj.Class).ShapeFieldName);
                            //IRowChanges rowChanges = (IRowChanges)obj;
                            //if (rowChanges.get_ValueChanged(shapeIdx) == false) return;

                            // trace upstream and get the correct feature
                            IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                            if (upstreamFeature == null)
                            {
                                _logger.Debug(_Name + ": No upsteam device found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }
                            // propogate the upstream feature operating number field onto this feature's source side device ID field
                            CopyUpsteamOperatingNumber(upstreamFeature, obj, sourceSideDeviceIDIdx, out upstreamOperatingNumber);
                            if (upstreamOperatingNumber == null)
                            {
                                _logger.Debug(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the upstream feature class : " + ((IDataset)upstreamFeature.Class).Name);
                                return;
                            }
                        }
                    }

                    // Support Connecting and Disconecting from Network.
                    if (eAUMode == mmAutoUpdaterMode.mmAUMFeederManager)
                    {
                        // check if the feederid field has gone from null to non-null indicating a connect or from non null to null indicating a disconnect
                        int feederIDIdx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.FeederID);
                        if (feederIDIdx == -1)
                        {
                            _logger.Warn("PGE Source Side Device: The model name :  " + SchemaInfo.Electric.FieldModelNames.FeederID + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                            return;
                        }

                        bool feederIDChanged = rowChange.get_ValueChanged(feederIDIdx);
                        if (!feederIDChanged)
                        {
                            return;
                        }

                        // is this a disconnect from the network or connected?
                        bool isConnected = HasBeenConnected(obj);

                        if (isConnected)
                        {
                            // trace upstream and get the correct feature
                            IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                            if (upstreamFeature == null)
                            {
                                _logger.Warn("PGE Source Side Device: No upsteam device found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }
                            // propogate the upstream feature operating number field onto this feature's source side device ID field
                            CopyUpsteamOperatingNumber(upstreamFeature, obj, sourceSideDeviceIDIdx, out upstreamOperatingNumber);
                            if (upstreamOperatingNumber == null)
                            {
                                _logger.Warn("PGE Source Side Device: The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)upstreamFeature.Class).Name);
                                return;
                            }

                            if (isSSD)
                            {
                                operatingNumber = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                                if (operatingNumber == DBNull.Value)//if null
                                {
                                    _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                                    return;
                                }
                                // propogate the operating number onto the source side device ID downstream 
                                PushOperatingNumberDownstream(obj, startEID, geomNetwork, operatingNumber, upstreamFeature);
                            }
                        }

                        if (!isConnected)
                        {
                            // disconnecting a device, set SSD ID = null;
                            FieldInstance sourceSideDeviceID = obj.FieldInstanceFromModelName(SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                            sourceSideDeviceID.Value = DBNull.Value;

                            //for non-SSDs do nothing more.
                            if (!isSSD)
                            {
                                return;
                            }

                            IFeature edge1 = null;
                            IFeature edge2 = null;
                            
                            GetConnectedEdgeFeatures(obj, out edge1, out edge2);

                            // get the EID of the junction feature that is in the same spot as this disconnected feature.
                            if (edge1 == null || edge2 == null)
                            {
                                _logger.Warn(_Name + ": No connected edges found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }
                            // get the from and to junctionEIDs off both edges and find the one that has the same x.y as the obj being deleted
                            if (edge1 != null)
                            {
                                startEID = FindEndpointEID(edge1, obj);
                            }
                            else
                            {
                                startEID = FindEndpointEID(edge2, obj);
                            }

                            IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                            if (upstreamFeature == null)
                            {
                                _logger.Warn("PGE Source Side Device: No upsteam device found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }

                            // Get upstream SSD operating number
                            upstreamOperatingNumber = upstreamFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                            if (upstreamOperatingNumber == DBNull.Value)
                            {
                                _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                                return;
                            }
                            PushOperatingNumberDownstream(obj, startEID, geomNetwork, upstreamOperatingNumber, upstreamFeature);
                        }
                    }
                }
                #endregion

                #region On Delete
                if (isDelete)
                {
                    if (!isSSD)
                    {
                        return;
                    }

                    IFeature edge1 = null;
                    IFeature edge2 = null;
                    GetConnectedEdgeFeatures(obj, out edge1, out edge2);
                    // get the EID of the junction feature that is in the same spot as this deleting feature.
                    if (edge1 == null || edge2 == null)
                    {
                        _logger.Warn(_Name + ": No connected edges found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                        return;
                    }
                    // get the from and to junctionEIDs off both edges and find the one that has the same x.y as the obj being deleted
                    if (edge1 != null)
                    {
                        startEID = FindEndpointEID(edge1, obj);
                    }
                    else
                    {
                        startEID = FindEndpointEID(edge2, obj);
                    }
                    IFeature upstreamFeature = UpstreamSourceSideDevice(geomNetwork, startEID);
                    if (upstreamFeature == null)
                    {
                        _logger.Warn(_Name + ": No upsteam device found for object with OID :  " + obj.OID + " from the object class : " + ((IDataset)obj.Class).Name);
                        return;
                    }

                    // Get upstream SSD operating number
                    upstreamOperatingNumber = upstreamFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                    if (upstreamOperatingNumber == DBNull.Value)
                    {
                        _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)obj.Class).Name);
                        return;
                    }
                    PushOperatingNumberDownstream(obj, startEID, geomNetwork, upstreamOperatingNumber, upstreamFeature);
                }
                #endregion

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }

        }

        /// <summary>
        /// Get the endpoint EID that matches the deleting feature
        /// </summary>
        /// <param name="edge1">the edge</param>
        /// <returns>return feature EID</returns>
        private int FindEndpointEID(IFeature edge1, IObject obj)
        {
            int eid = 0;
            IFeature deletingFeature = obj as IFeature;
            IPoint deletingPoint = deletingFeature.Shape as IPoint;
            IEdgeFeature edgeFeature1 = edge1 as IEdgeFeature;
            IJunctionFeature toJunctionFeature = edgeFeature1.ToJunctionFeature;
            IJunctionFeature fromJunctionFeature = edgeFeature1.FromJunctionFeature;
            IFeature toJunction = toJunctionFeature as IFeature;
            IFeature fromJunction = fromJunctionFeature as IFeature;
            // which endpoint is actually where the deleted obj lies?
            IPoint toPoint = toJunction.Shape as IPoint;
            IPoint fromPoint = fromJunction.Shape as IPoint;
            if (toPoint.X == deletingPoint.X && toPoint.Y == deletingPoint.Y)
            {
                eid = edgeFeature1.ToJunctionEID;
                _logger.Debug("EID : " + eid);
            }
            else
            {
                eid = edgeFeature1.FromJunctionEID;
                _logger.Debug("EID : " + eid);
            }
            return eid;
        }

        /// <summary>
        /// Get the network edge features that are in the same spot as the feature being deleted
        /// </summary>
        /// <param name="obj">Esri object</param>
        /// <returns></returns>
        private void GetConnectedEdgeFeatures(IObject obj, out IFeature edge1, out IFeature edge2)
        {
            //Feature junctionFeature = null;
            edge1 = null;
            edge2 = null;
            IGeometricNetwork m_GeoNet = (obj as INetworkFeature).GeometricNetwork;
            IPoint location = ((IFeature)obj).Shape as IPoint;
            IEnumFeature enumFeature = m_GeoNet.SearchForNetworkFeature(location, esriFeatureType.esriFTComplexEdge);
            enumFeature.Reset();
            IFeature feat = enumFeature.Next();
            edge1 = feat;
            _logger.Debug(edge1.Class.AliasName);
            if (feat != null)
            {
                edge2 = enumFeature.Next();
                _logger.Debug(edge2.Class.AliasName);
            }
        }

        #endregion

        /// <summary>
        /// Initialize a dictionary with feature class IDs and IFeature
        /// </summary>
        /// <param name="obj"></param>
        private void InitFeatureClassIDs(IObject obj)
        {
            if (_SSDFeatureClassIDs == null)
            {
                _SSDFeatureClassIDs = new List<int>();
            }
            //get geometric network
            IGeometricNetwork m_GeoNet = (obj as INetworkFeature).GeometricNetwork;

            if (_featureClassIDs == null)
            {
                _featureClassIDs = new Dictionary<int, IFeatureClass>();

                IEnumFeatureClass pENumJnFCs = m_GeoNet.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                pENumJnFCs.Reset();
                IFeatureClass pJNFC = pENumJnFCs.Next();
                while (pJNFC != null) //if feature class not null
                {
                    if (ModelNameFacade.FieldNameFromModelName(pJNFC, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId) != string.Empty)//if model name not present
                    {
                        if (!_featureClassIDs.ContainsKey(pJNFC.FeatureClassID))
                        {
                            _featureClassIDs.Add(pJNFC.FeatureClassID, pJNFC);
                        }
                    }

                    if (ModelNameFacade.ContainsClassModelName(pJNFC, SchemaInfo.Electric.ClassModelNames.SourceSideDevice))
                    {
                        if (!_SSDFeatureClassIDs.Contains(pJNFC.FeatureClassID))
                        {
                            _SSDFeatureClassIDs.Add(pJNFC.FeatureClassID);
                        }
                    }

                    if ((ModelNameFacade.ContainsClassModelName(pJNFC, SchemaInfo.Electric.ClassModelNames.Switch)) & (ModelNameFacade.ContainsClassModelName(pJNFC, SchemaInfo.Electric.ClassModelNames.SourceSideDevice)))
                    {
                        m_SwitchFeatureClass = pJNFC;
                        m_SwitchTypeFldIndex = m_SwitchFeatureClass.FindField(_switchTpeFieldName);   ////Replace hard-code for field name?
                    }
                    pJNFC = pENumJnFCs.Next();
                }
            }

            //Get the Feature Class IDs of Edge features assigned with PGE_TRACEEDGEBARRIER model namesto recongnize them as barrier while tracing
            _barrierEdgeClassIDs = new List<int>();
            IEnumFeatureClass pENumEdgeFCs = null;
            IFeatureClass edgeClass = null;

            //Get the complex edge feature classes and check for EdgeBarrier model name assignment
            pENumEdgeFCs = m_GeoNet.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
            pENumEdgeFCs.Reset();
            while ((edgeClass = pENumEdgeFCs.Next()) != null)
            {
                if (ModelNameFacade.ContainsClassModelName(edgeClass, SchemaInfo.Electric.ClassModelNames.EdgeBarrierModelName))
                {
                    _barrierEdgeClassIDs.Add(edgeClass.FeatureClassID);
                }
            }

            //Get the simple edge feature classes and check for EdgeBarrier model name assignment
            pENumEdgeFCs = m_GeoNet.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
            pENumEdgeFCs.Reset();
            while ((edgeClass = pENumEdgeFCs.Next()) != null)
            {
                if (ModelNameFacade.ContainsClassModelName(edgeClass, SchemaInfo.Electric.ClassModelNames.EdgeBarrierModelName))
                {
                    _barrierEdgeClassIDs.Add(edgeClass.FeatureClassID);
                }
            }
        }

        /// <summary>
        /// Compare the FEEDERID value to determine if this is a connection or disconnect operation
        /// </summary>
        /// <param name="rowChange">esri row</param>
        /// <param name="obj">esri object</param>
        /// <param name="feederIDIdx">field index of feederID</param>
        /// <param name="isConnect">out boolean value</param>
        /// <param name="isDisconnect">out boolean isdisconnect</param>
        private bool HasBeenConnected(IObject obj)
        {
            bool retVal = true;

            int FeederIDIdx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.FeederID);
            IRowChanges rowChange = (IRowChanges)obj;
            object feederID_orig = rowChange.get_OriginalValue(FeederIDIdx);
            object feederID_current = obj.get_Value(FeederIDIdx);

            if (feederID_orig == null || (feederID_orig is System.DBNull == true))
            {
                if (feederID_current != null && (feederID_current is System.DBNull == false))
                {
                    retVal =  true;
                }
            }
            if (feederID_orig != null && (feederID_orig is System.DBNull == false))
            {
                if (feederID_current == null || (feederID_current is System.DBNull == true))
                {
                    ISimpleJunctionFeature simpJuncFeature = (ISimpleJunctionFeature)obj;
                    if (simpJuncFeature.EdgeFeatureCount == 0)
                    {
                        retVal = false;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get the operating number from the upstream feature and copy it into the source side device field on obj
        /// </summary>
        /// <param name="upstreamFeature">the upstream feature</param>
        /// <param name="obj">the target of the copy</param>
        /// <param name="sourceSideDeviceIDIdx">the index of the source side device id field</param>
        /// <param name="upstreamOperatingNumber">the operating number to copy</param>
        private void CopyUpsteamOperatingNumber(IFeature upstreamFeature, IObject obj, int sourceSideDeviceIDIdx, out object upstreamOperatingNumber)
        {
            // propogate the upstream feature operating number field onto this feature's source side device ID field
            upstreamOperatingNumber = null;
            int upstreamOperatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(upstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
            if (upstreamOperatingNumberIdx != -1)
            {
                upstreamOperatingNumber = upstreamFeature.get_Value(upstreamOperatingNumberIdx);
                obj.set_Value(sourceSideDeviceIDIdx, upstreamOperatingNumber);
            }
        }

        /// <summary>
        /// Copy the operating number from the source object onto all downstream objects in their source side device id field
        /// </summary>
        /// <param name="obj">the object to copy from</param>
        /// <param name="startEID">the EID of the object to copy from</param>
        /// <param name="geomNetwork">the network that the source object is in</param>
        /// <param name="operatingNumber">the operating number to copy</param>
        private void PushOperatingNumberDownstream(IObject obj, int startEID, IGeometricNetwork geomNetwork, object operatingNumber, IFeature upstreamFeature)
        {
            List<IFeature> downstreamFeatures = DownstreamSourceSideDeviceZone(obj, startEID, geomNetwork);
            foreach (IFeature downstreamFeature in downstreamFeatures)
            {
                // don't set the upstream SSD feature
                if (upstreamFeature != null && downstreamFeature.Class.AliasName == upstreamFeature.Class.AliasName && downstreamFeature.OID == upstreamFeature.OID)
                {
                    continue;
                }

                int sourceSideDeviceIDIdx = ModelNameFacade.FieldIndexFromModelName(downstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                if (sourceSideDeviceIDIdx != -1)
                {
                    if (downstreamFeature.get_Value(sourceSideDeviceIDIdx) != operatingNumber)
                    {
                        bool updateSourceSideDeviceID = true;
                        bool isSwitch = false;
                        bool normalPositionChanged = false;
                        if (IsObjectSSD(downstreamFeature as IObject, out isSwitch) && (IsOpen(downstreamFeature as IObject, out normalPositionChanged)))
                        {
                            updateSourceSideDeviceID = false;
                        }

                        if (updateSourceSideDeviceID)
                        {
                            downstreamFeature.set_Value(sourceSideDeviceIDIdx, operatingNumber);
                            downstreamFeature.Store();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine if the object is a SSD
        /// </summary>
        /// <param name="obj">the object to check</param>
        /// <returns>true if the object is SSD</returns>
        private bool IsObjectSSD(IObject obj, out bool isSwitch)
        {
            bool isSSD = ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.SourceSideDevice);
            bool isSwitchTemp = false;
            // if the device is a switch and the switch type is 7, 8, or 99 then it is not considered a SSD switch
            if (isSSD)//if true
            {
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGESwitch))//if true
                {
                    isSwitchTemp = true;

                    //get switch value
                    object switchTypeObj = obj.get_Value(m_SwitchTypeFldIndex);
                    _logger.Debug(SchemaInfo.Electric.ClassModelNames.Switch + " field value: " + switchTypeObj);
                    if (switchTypeObj != null && (switchTypeObj is System.DBNull == false))
                    {
                        int switchType = Convert.ToInt32(switchTypeObj);
                        //if (switchType == 7 || switchType == 8 || switchType == 99)//check if switchType value matches to the following values
                        if(_nonSSDSwitchType.Contains(switchType)) 
                        {
                            isSSD = false;
                        }
                    }
                }
                else
                {
                    _logger.Warn(string.Format(_warningMsgNoModelName,SchemaInfo.Electric.ClassModelNames.Switch));
                }
            }
            else
            {
                _logger.Warn(string.Format(_warningMsgNoModelName, SchemaInfo.Electric.ClassModelNames.SourceSideDevice));
            }
            isSwitch = isSwitchTemp;
            return isSSD;
        }

        /// <summary>
        /// Check the normal position fields and determine if this device is open
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool IsOpen(IObject obj, out bool normalPostionChanged)
        {
            //assign normal positions A,B,C to string array
            string[] normalPostionModelNames = { SchemaInfo.Electric.FieldModelNames.NormalpositionA,SchemaInfo.Electric.FieldModelNames.NormalpositionB, SchemaInfo.Electric.FieldModelNames.NormalPositionC};
            bool isOpen = true;
            normalPostionChanged = false;
            //int index = -1;

            foreach (string normalPostionModelName in normalPostionModelNames)
            {
                //get field value for normal position
                if (ModelNameFacade.ContainsFieldModelName(obj.Class, normalPostionModelName))
                {
                    int normalPostionIdx = obj.Fields.FindField(normalPostionModelName);
                    if (normalPostionIdx != -1)
                    {
                        IRowChanges rowChanges = (IRowChanges)obj;
                        if (rowChanges.get_ValueChanged(normalPostionIdx))
                        {
                            normalPostionChanged = true;
                            break;
                        }
                    }
                }
            }


            foreach (string normalPostionModelName in normalPostionModelNames)
            {
                int? normalstatus = (int?)obj.GetFieldValue(null, false, normalPostionModelName);
                _logger.Debug(normalPostionModelName + " value: " + normalstatus);
                if (normalstatus != 0)//if value not equal to zero
                {
                    isOpen = false;
                    break;
                }
            }
            return isOpen;
        }

        /// <summary>
        /// Checks if the SourceSide device attribtue needs to be updated by checking if the Operating number of the Source Device has changed or the SwitchType has changed if the device is Switch/
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private bool UpdateRequired(IObject feature)
        {
            bool retVal = false;
            IRowChanges rowChanges = (IRowChanges)feature;
            //First Check if Operating Number has changed
            int operatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
            int shapeIdx = feature.Fields.FindField(((IFeatureClass)feature.Class).ShapeFieldName);
            if (operatingNumberIdx == -1)
            {
                _logger.Warn(_Name + ": The model name :  " + SchemaInfo.Electric.FieldModelNames.OperatingNumber + " was not found on the object class : " + ((IDataset)feature.Class).Name);
                return retVal;
            }

            retVal = (rowChanges.get_ValueChanged(operatingNumberIdx)) || (rowChanges.get_ValueChanged(shapeIdx));

            //Check if the supplied feature is Switch and check if the SwitchType has changed.
            bool isSwitch = ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.PGESwitch);
            if (isSwitch && retVal == false)
            {
                int switchTypeFieldIdx = feature.Fields.FindField(_switchTpeFieldName);
                if (switchTypeFieldIdx > -1)
                {
                    retVal = rowChanges.get_ValueChanged(switchTypeFieldIdx);
                }
            }
            return retVal;
        }
    }
}
