using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Framework;
using Miner.Geodatabase;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{

    #region Enumerations

    /// <summary>
    /// ConduitSystem subtypes
    /// </summary>
    enum Subtype
    {
        //Enums are 0 based and the Subtype starts with 1
        None,
        DuctBank,
        Conduit,
        CIC
    }

    #endregion

    /// <summary>
    ///   An abstract Special Auto Updater (AU) that is used to building the label text which is set on the field assigned the 'LABELTEXT' field model name.
    /// </summary>
    public abstract class BaseLabelTextAU : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private int _labelCount;

        #endregion

        #region Fields

        /// <summary>
        ///   Gets the label text unit event.
        /// </summary>
        /// <value> The label text unit event. </value>
        protected static mmEditEvent LabelTextEditEvent;

        /// <summary>
        ///   Gets the label text unit.
        /// </summary>
        /// <value> The label text unit. </value>
        protected static IObject LabelTextUnit;

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseLabelTextAU" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        protected BaseLabelTextAU(string name, int labelCount = 1)
            : base(name)
        {
            _labelCount = labelCount;
        }

        #endregion

        #region Protected Methods

        protected void UpdateLabelCount(int labelCount)
        {
            _labelCount = labelCount;
        }

        /// <summary>
        ///   Gets the relationships.
        /// </summary>
        /// <param name="objClass"> The obj class. </param>
        /// <param name="modelName"> Name of the model. </param>
        /// <returns> </returns>
        protected List<IRelationshipClass> GetRelationships(IObjectClass objClass, string modelName)
        {
            _logger.Debug("ENTER: GetRelationships with ObjectClas: " + objClass.AliasName + " and Model Name: " + modelName);
            var relClasses = new List<IRelationshipClass>();

            var enumRelClass = objClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
            enumRelClass.Reset();
            IRelationshipClass relClass;
            while ((relClass = enumRelClass.Next()) != null)
            {
                _logger.Debug("Got Relationship: " + ((IDataset)relClass).Name);
                var destClass = relClass.DestinationClass;
                var origClass = relClass.OriginClass;

                if (ModelNameFacade.ContainsClassModelName(destClass, modelName)
                    || ModelNameFacade.ContainsClassModelName(origClass, modelName))
                    if (!relClasses.Contains(relClass))
                        relClasses.Add(relClass);
            }

            _logger.Debug("EXIT: GetRelationships with ObjectClas: " + objClass.AliasName + " and Model Name: " + modelName);
            return relClasses;
        }

        /// <summary>
        /// Gets the related objects based on the RelashipLabel static memeber variable.
        /// On Relationship delete the Store method on the parent object is called to reset the LabelText. 
        /// when the store is called the Relationship Delete Event is not complete yet so the LabelText will never get Reset to correct value. 
        /// This Method will filter the related object and will send only those that are still active.
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="aliasName"></param>
        /// <param name="relationshipRole"></param>
        /// <param name="modelNames"></param>
        /// <returns></returns>
        protected IEnumerable<IObject> GetRelatedObjects(IObject sourceObject, string aliasName = null, esriRelRole relationshipRole = esriRelRole.esriRelRoleAny, params string[] modelNames)
        {
            _logger.Debug("ENTER: GetRelatedObjects with source object: " + sourceObject.Class.AliasName + " | ObjectID: " + sourceObject.OID.ToString() +
                " modelNames: " + modelNames.Concatenate(","));

            //Get All related Objects
            var relatedObjects = sourceObject.GetRelatedObjects(aliasName, relationshipRole, modelNames);
            if (RelationshipLabel.PropogateRelationshipDelete && RelationshipLabel.DeletedOID != -1)
            {
                List<IObject> relatedObjList = relatedObjects.ToList();
                int position = relatedObjList.FindIndex(obj => obj.OID == RelationshipLabel.DeletedOID);
                if (position != -1)
                {
                    _logger.Debug("Located object to Remove at position: " + position.ToString());
                    relatedObjList.RemoveAt(position);
                    _logger.Debug("EXIT: GetRelatedObjects");
                    return relatedObjList.AsEnumerable();
                }
            }
            _logger.Debug("EXIT: GetRelatedObjects");
            return relatedObjects;
        }

        /// <summary>
        ///   Determines if the object is pending deletion.
        /// </summary>
        /// <param name="obj"> The obj. </param>
        /// <returns> <c>true</c> if the object is going to be deleted; otherwise <c>false</c> </returns>
        protected bool PendingDeletion(IObject obj)
        {
            _logger.Debug("ENTER: PendingDeletion with Object: " + obj.Class.AliasName + " | ObjectID: " + obj.OID.ToString());

            if (LabelTextEditEvent != mmEditEvent.mmEventFeatureDelete)
            {
                _logger.Debug("EXIT: PendingDeletion with mmEventFeatureDelete");
                return false;
            }
            if (LabelTextUnit == null)
            {
                _logger.Debug("EXIT: PendingDeletion with LabelTextUnit = NULL");
                return false;
            }

            _logger.Debug("EXIT: PendingDeletion with: " + (LabelTextUnit.OID == obj.OID).ToString());
            return (LabelTextUnit.OID == obj.OID);
        }

        /// <summary>
        ///   Gets the label text given the specific object.
        /// </summary>
        /// <param name="obj"> The object that triggered the event. </param>
        /// <param name="autoUpdaterMode"> The auto updater mode. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <param name="labelIndex">The index of the label that should be returned.</param>
        /// <returns> The label text that will be stored on the field assigned the <see
        ///    cref="SchemaInfo.General.FieldModelNames.LabelText" /> field model name, or null if no text should be assigned. </returns>
        protected abstract string GetLabelText(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex);

        /// <summary>
        ///   Determines whether this instance can execute using the specified AU mode.
        /// </summary>
        /// <param name="autoUpdaterMode"> The AU mode. </param>
        /// <returns> <c>true</c> if this instance can execute using the specified AU mode; otherwise, <c>false</c> . </returns>
        protected override bool CanExecute(mmAutoUpdaterMode autoUpdaterMode)
        {
            //Remedy Incident problem PBI000000011764 fix 12/03/2014 - this is 
            //addressing the performance issues with EDER - do not want LabelTextAUs 
            //firing when they are not necessary such as when in FeederManager mode 
            return ((autoUpdaterMode != mmAutoUpdaterMode.mmAUMNoEvents) && 
                (autoUpdaterMode != mmAutoUpdaterMode.mmAUMFeederManager));
        }

        /// <summary>
        ///   Implementation of Autoupdater Enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass"> The object class. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        /// <remarks>
        ///   This method will be called from IMMSpecialAUStrategy::get_Enabled and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent editEvent)
        {
            _logger.Debug("ENTER: InternalEnabled with Object Class: " + objectClass.AliasName + " EditEvent: " + editEvent.ToString());

            bool enabled = false;
            if (editEvent == mmEditEvent.mmEventFeatureCreate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.LabelTextBank);
            }// Added for INC INC000004199838 
            else if ((editEvent == mmEditEvent.mmEventFeatureUpdate) || (editEvent == mmEditEvent.mmEventFeatureDelete))
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.LabelTextUnit)
                          || ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.LabelTextBank);
            }

            _logger.Debug("EXIT: InternalEnabled with " + enabled.ToString());
            return enabled;
        }

        /// <summary>
        ///   Implementation of Autoupdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj"> The object that triggered the Auto Udpater. </param>
        /// <param name="autoUpdaterMode"> The auto updater mode. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <remarks>
        ///   This method will be called from IMMSpecialAUStrategy::ExecuteEx and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent)
        {
            if (editEvent == mmEditEvent.mmEventFeatureDelete) 
            { return; }
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);

            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            try
            {
                _logger.Debug("ENTER: InternalExecute with Object: " + obj.Class.AliasName + "|ObjectID: " + obj.OID.ToString() +
                    ", AutoupaterMode: " + autoUpdaterMode.ToString() + ", EditEvent: " + editEvent.ToString());
                
                //Do not run LabelText AUs again in response ot Rel AU or Unit AU .Store() call on parent feature.
                if ((BaseRelationshipAU.IsRelAUCallingStore == true) ||
                    (BaseLabelTextAU.IsRunningAsUnitAU == true && BaseSpecialAU.IsUnitCallingStore == true))
                {
                    _logger.Debug("EXIT: InternalExecute because another Rel AU or Unit AU is calling store.");
                    return;
                }

                // Determine if this AU needs to call this AU on it's bank.
                if (obj.HasModelName(SchemaInfo.General.ClassModelNames.LabelTextUnit))
                {
                    // Store the unit object that caused the bank to be triggered.
                    LabelTextUnit = obj;
                    LabelTextEditEvent = editEvent;

                    foreach (var relObj in obj.GetRelatedObjects(null, modelNames: SchemaInfo.General.ClassModelNames.LabelTextBank))
                    {
                        try
                        {
                            BaseLabelTextAU.IsRunningAsUnitAU = true;
                            _logger.Debug("STARTING: AU on Parent object: " + relObj.Class.AliasName + "|ObjectID: " + relObj.OID.ToString());
                            //Check if this feature has a DuctPosition field.  if it does, and it changed, the parent will need to update the cross
                            //section annotation.
                            IField ductPositionField = ModelNameManager.FieldFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.DuctPosition);
                            if (ductPositionField != null)
                            {
                                IRowChanges rowChanges = obj as IRowChanges;
                                if (rowChanges != null)
                                {
                                    int fieldIdx = obj.Fields.FindField(ductPositionField.Name);
                                    if (fieldIdx > -1)
                                    {
                                        if (rowChanges.get_ValueChanged(fieldIdx))
                                        {
                                            ExecuteCrossSectionAU = true;
                                        }
                                    }
                                }
                            }
                            // Added for null value For fields INC INC000004199838 
                            try
                            {

                                if (obj.get_Value(obj.Fields.FindField("CONDUCTORSIZE")).ToString().Contains("$U") == true)
                                {
                                    for (var i = 0; i < obj.Fields.FieldCount; i++)
                                    {
                                        var field = obj.Fields.Field[i];
                                        try
                                        {
                                            obj.set_Value(i, null);
                                        }
                                        catch { }
                                    }
                                    obj.Store();
                                }
                            }
                            catch { }
                            LabelTextHelper.ExecuteAUOnParent(relObj, autoUpdaterMode, editEvent);
                            _logger.Debug("FINISHED: AU on Parent object: " + relObj.Class.AliasName + "|ObjectID: " + relObj.OID.ToString());
                        }
                        finally
                        {
                            BaseLabelTextAU.IsRunningAsUnitAU = false;
                            ExecuteCrossSectionAU = false;
                        }
                    }

                    _logger.Debug("EXIT: InternalExecute - Finished updating ParentObjects.");
                    return;
                }

                bool labelChanged = false;

                bool isConduit = ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem);
                for (var labelIndex = 0; labelIndex < _labelCount; labelIndex++)
                {
                    var modelName = SchemaInfo.General.FieldModelNames.LabelText;
                    //CR#4937 -- Populating 

                    if (labelIndex > 0)//&& !isConduit) //-- prevents labeltext2. Surely a bug?
                    {
                        modelName += (labelIndex + 1).ToString("N0");
                    }

                    // Ask the label AU if it wants to override the model name to use
                    modelName = GetLabeltextField(obj, modelName);

                    _logger.Debug("Processing model name: " + modelName.ToString());

                    var fields = obj.GetFields(modelName);
                    foreach (var field in fields)
                    {
                        _logger.Debug("GetLabelText for " + field.Name);
                        var labelText = GetLabelText(obj, autoUpdaterMode, editEvent, labelIndex);
                        _logger.Debug("LabelText for " + field.Name + " = " + labelText.ToString());
                        string currentText = Convert.ToString(field.Value).Trim();
                        string newText = Convert.ToString(labelText).Trim();
                        if (currentText.Equals(newText) == false)
                        {
                            if (BaseRelationshipAU.IsRunningAsRelAU == true || BaseLabelTextAU.IsRunningAsUnitAU == true)
                            {
                                try
                                {
                                    BaseRelationshipAU.IsRelAUCallingStore = true;
                                    BaseSpecialAU.IsUnitCallingStore = true;
                                    _logger.Debug(field.Name + ".FieldLength = " + field.FieldLength.ToString() + " | LabelText.Length = " + newText.Length.ToString());
                                    int fieldLength = newText.Length < field.FieldLength ? newText.Length : field.FieldLength;
                                    newText = newText.Substring(0, fieldLength);
                                    _logger.Debug("Trim to fit into field:  New label = " + newText);
                                    _logger.Debug("START: Storing labelText on parent.");
                                    //Turn off AUs completely when setting the label text field.
                                    immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                                    field.StoreValue = newText;
                                    immAutoupdater.AutoUpdaterMode = currentAUMode;
                                    _logger.Debug("FINISHED: Storing labelText parent.");
                                    BaseRelationshipAU.IsRelAUCallingStore = false;
                                    BaseSpecialAU.IsUnitCallingStore = false;
                                }
                                finally
                                {
                                    BaseRelationshipAU.IsRelAUCallingStore = false;
                                    BaseSpecialAU.IsUnitCallingStore = false;
                                }
                            }
                            else
                            {
                                field.Value = newText;
                            }
                            labelChanged = true;
                        }
                    }
                }

                //If this is conduit we want to fire off the related PriUG and SecUG AUs.  In addition we need to execute the ArcFM Check Cross Section AU
                if (isConduit && labelChanged)
                {
                    // Only update conductors if were not a duct bank
                    var subtype = obj.SubtypeCodeAsEnum<Subtype>();
                    if (subtype != Subtype.DuctBank)
                    {
                        immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                        
                        //Need to potentially update the related priUG feature.
                        PriUGConductorLabel priUGLabelTextAU = new PriUGConductorLabel();
                        ConduitLabel.ExecuteOnRelatedObjects(obj, priUGLabelTextAU, mmAutoUpdaterMode.mmAUMArcMap, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor, ModelNameManager);

                        SecUGConductorLabel secUGConductorLabelTextAU = new SecUGConductorLabel();
                        ConduitLabel.ExecuteOnRelatedObjects(obj, secUGConductorLabelTextAU, mmAutoUpdaterMode.mmAUMArcMap, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor, ModelNameManager);

                        NeutralConductorLabel NeuConductorLabelTextAU = new NeutralConductorLabel();
                        ConduitLabel.ExecuteOnRelatedObjects(obj, NeuConductorLabelTextAU, mmAutoUpdaterMode.mmAUMArcMap, SchemaInfo.Electric.ClassModelNames.PGENeutralConductor, ModelNameManager);


                        immAutoupdater.AutoUpdaterMode = currentAUMode;
                    }
                }
                else if (labelChanged || ExecuteCrossSectionAU)
                {
                    ExecuteCrossSectionAU = false;
                    //If the label changed we want to execute the cross section anno AU
                    string modelName = "";
                    if (ModelNameManager.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor)) { modelName = SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor; }
                    else if (ModelNameManager.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor)) { modelName = SchemaInfo.Electric.ClassModelNames.SecondaryUGConductor; }
                    else if (ModelNameManager.ContainsClassModelName(obj.Class, SchemaInfo.UFM.ClassModelNames.DcConductor)) { modelName = SchemaInfo.UFM.ClassModelNames.DcConductor; }

                    Miner.Interop.IMMSpecialAUStrategy csAnnoAU = Activator.CreateInstance(Type.GetTypeFromProgID("mmUlsAUs.MMUpdateCrossSection")) as Miner.Interop.IMMSpecialAUStrategy;
                    if (!string.IsNullOrEmpty(modelName))
                    {
                        csAnnoAU.Execute(obj);
                    }
                }


                // Refresh any annotation features.
                if (labelChanged && autoUpdaterMode == mmAutoUpdaterMode.mmAUMArcMap)
                {
                    _logger.Debug("Refreshing Annotation.");
                    obj.RefreshAnnotation();

                    // Added for INC INC000004199838
                    Document.ActiveView.Refresh();
                }
            }
            finally
            {
                //Set our AU mode back to the original value
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }

        protected virtual string GetLabeltextField(IObject obj, string currentLabelField)
        {
            // default implementation just uses the supplied field
            return currentLabelField;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Refreshes the annotation on the given object.
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private static void RefreshAnnotation(IObject obj)
        {
            _logger.Debug("ENTER: RefreshAnnotation with object: " + obj.Class.AliasName + "|ObjectID: " + obj.OID.ToString());
            if (Document.ActiveView == null)
                return;

            // Look for related anno records that have a construction status field.
            foreach (var relationshipClass in obj.GetRelationships())
            {
                var destClass = relationshipClass.DestinationClass as IFeatureClass;
                _logger.Debug("Processing DestinationClass: " + destClass.AliasName);
                if (destClass == null || destClass.FeatureType != esriFeatureType.esriFTAnnotation)
                {
                    _logger.Debug("DestinationClass (" + destClass.AliasName + ") is not an annotation class.");
                    continue;
                }

                _logger.Debug("Getting related objects.");
                var relSet = obj.GetRelatedObjects()
                    .Cast<IFeature>()
                    .Where(feature => feature.ShapeCopy != null && !feature.ShapeCopy.IsEmpty);
                if (!relSet.Any())
                {
                    _logger.Debug("There are no related objects.");
                    continue; // There are no related objects continue.
                }

                // We need to get a combined extent of all the related annotations. (Typically there is only one).
                _logger.Debug("Combining extent of all related annoation.");
                IEnvelope extent = new EnvelopeClass();
                foreach (var feature in relSet)
                {
                    extent.Union(feature.ShapeCopy.Envelope);
                }

                // There are no annotation geometries.
                if (extent.IsEmpty) continue;

                // Call partial refresh
                _logger.Debug("Calling PartialRefesh.");
                Document.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, extent);
                _logger.Debug("EXIT: RefreshAnnotation.");
            }
        }

        #endregion

        #region Public Static Memebers
        internal static bool IsRunningAsUnitAU { get; set; }
        internal static bool ExecuteCrossSectionAU { get; set; }
        #endregion

    }
}
