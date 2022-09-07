using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Microsoft.Win32;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Reads XML cofiguration documents from the database and selects a symbol number for a updated object by evaluating the rules defined within.
    /// </summary>
    [Guid("905EDCE4-BC77-4888-9EE6-FD791F5EF9FA")]
    [ProgId("PGE.Desktop.EDER.RelateDeviceGroupAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class RelateDeviceGroupAU : BaseSpecialAU
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RelateDeviceGroupAU"/> class.  
        /// </summary>
        public RelateDeviceGroupAU() : base("PGE Relate Device Group AU") { }
        #endregion Constructors

        #region Privates

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Used to store Destination Feature Classs Field name which need to be updated.
        /// </summary>
        private string _destFeatureClassFieldName= "STRUCTUREGUID";

        /// <summary>
        /// Used to store registry key name where Tolerance value in map units will be stored.
        /// </summary>
        private string _registryKey = "DeviceGroupSearchTolerance";
        /// <summary>
        /// If registry access throws some error or registry key doesnt exist at all. Then this default value will be used.
        /// </summary>
        private double _registryDefaultVlue = 5;

        /// <summary>
        /// Returns registry value from abovedefined path and key.
        /// If registry value is not retrieved then this method will return defined default value.
        /// </summary>
        /// <returns>registry value</returns>
        private double GetRegistryValue()
        {
            double retValue = _registryDefaultVlue;
            //Read the config location from the Registry Entry
            SystemRegistry sysRegistry = new SystemRegistry("PGE");
            retValue=sysRegistry.GetSetting<double>(_registryKey, _registryDefaultVlue);
            //object value =  Registry.GetValue(_registryPath, _registryKey, -1);
            //double retValue=-1;
            //if (value==null || value.ToString() == "-1")
            //{
            //    _logger.Warn("Registry value not found for Device Group Search Tolerance.");
            //    //Value not found in registry now assign the default value and return defaultvalue.
            //    retValue = _registryDefaultVlue;
            //}
            //else if (!double.TryParse(value.ToString(), out retValue))
            //{
            //    retValue = _registryDefaultVlue;
            //}
            return retValue;
        }

        /// <summary>
        /// Executes when DeviceGroup is created/updated.
        /// </summary>
        /// <param name="pObject">Object being created/updated.</param>
        /// <param name="eEvent">Eventobject which clarifies that object is created/updated.</param>
        private void ExecuteForDeviceGroup(IObject pObject, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                _logger.Debug("Executing DeviceGroupCreated.");
                DeviceGroupCreated(pObject);
                _logger.Debug("Executed DeviceGroupCreated.");
            }
            else if (eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                _logger.Debug("Executing DeviceGroupUpdated.");
                DeviceGroupUpdated(pObject);
                _logger.Debug("Executed DeviceGroupUpdated.");
            }
        }

        /// <summary>
        /// Executes when DeviceGroupChild is created/updated.
        /// </summary>
        /// <param name="pObject">Object being created/updated.</param>
        /// <param name="eEvent">Eventobject which clarifies that object is created/updated.</param>
        private void ExecuteForDeviceGroupChild(IObject pObject, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                _logger.Debug("Executing DeviceGroupChildCreated.");
                DeviceGroupChildCreated(pObject);
                _logger.Debug("Executed DeviceGroupChildCreated.");
            }
            else if (eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                _logger.Debug("Executing DeviceGroupChildUpdated.");
                DeviceGroupChildUpdated(pObject);
                _logger.Debug("Executed DeviceGroupChildUpdated.");
            }
        }

        /// <summary>
        /// Executes when DeviceGroupChild is created.
        /// </summary>
        /// <param name="pObject">Object being created.</param>
        private void DeviceGroupChildCreated(IObject pObject)
        {
            IFeature deviceGroupChildFeature = pObject as IFeature;
            IDataset devideGroupChildDS = deviceGroupChildFeature.Class as IDataset;

            IEnumFeatureClass deviceGroupFeatureClassses = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(devideGroupChildDS.Workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
            IFeatureClass currentFC = null;
            while ((currentFC = deviceGroupFeatureClassses.Next()) != null)
            {
                //No needto pass onlyUnrelated as true DeviceGroup may or may not been related doesnt matter to DeviceGroupChild
                _logger.Debug("Getting IFeatureCursor for " + currentFC.AliasName +".");
                IFeatureCursor deviceGroupCursor = GetIFeatureCursor(deviceGroupChildFeature, currentFC, false);
                _logger.Debug("Got IFeatureCursor for " + currentFC.AliasName + ".");
                try
                {
                    _logger.Debug("Getting closest IFeature of " + currentFC.AliasName + ".");
                    IFeature closestDeviceGroup = GetClosestFeature(deviceGroupChildFeature, deviceGroupCursor, true);
                    _logger.Debug("Got closest IFeature of " + currentFC.AliasName + ".");
                    if (closestDeviceGroup != null)
                    {
                        _logger.Debug("Relating closest feature.");
                        RelateObjects(closestDeviceGroup, deviceGroupChildFeature);
                        
                    }
                }
                finally
                {
                    _logger.Debug("Releasing IfeatureCursor.");
                    while (Marshal.ReleaseComObject(deviceGroupCursor) > 0)
                    {
                    }
                    _logger.Debug("Released IfeatureCursor.");
                }
            }
        }

        /// <summary>
        /// Executes when DeviceGroupChild is updated.
        /// </summary>
        /// <param name="pObject">Object being updated.</param>
        private void DeviceGroupChildUpdated(IObject pObject)
        {
            IFeatureChanges objectFeatureChanges = pObject as IFeatureChanges;
            if (objectFeatureChanges != null)
            {
                _logger.Debug("Checking if Geometry is updated.");
                if (objectFeatureChanges.ShapeChanged)
                {
                    _logger.Debug("Geometry is updated so calling CreateGroupChild.");
                    DeviceGroupChildCreated(pObject);
                }
            }
        }

        /// <summary>
        /// Executes when DeviceGroup is created.
        /// </summary>
        /// <param name="pObject">Object being created.</param>
        private void DeviceGroupCreated(IObject pObject)
        {
            IFeature deviceGroupFeature = pObject as IFeature;
            IDataset devideGroupDS = deviceGroupFeature.Class as IDataset;

            IEnumFeatureClass deviceGroupChildFeatureClassses = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(devideGroupDS.Workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroupChild);
            IFeatureClass currentFC=null;
            while ((currentFC = deviceGroupChildFeatureClassses.Next()) != null)
            {
                //Needto pass onlyUnrelated as true as already related DeviceGroupChild doesnt required.
                _logger.Debug("Getting IFeatureCursor for " + currentFC.AliasName + ".");
                IFeatureCursor deviceGroupChildCursor = GetIFeatureCursor(deviceGroupFeature, currentFC, true);
                _logger.Debug("Got IFeatureCursor for " + currentFC.AliasName + ".");
                try
                {
                    _logger.Debug("Getting closest IFeature of " + currentFC.AliasName + ".");
                    IFeature closestDeviceGroupChild = GetClosestFeature(deviceGroupFeature, deviceGroupChildCursor);
                    _logger.Debug("Got closest IFeature of " + currentFC.AliasName + ".");
                    if (closestDeviceGroupChild != null)
                    {
                        _logger.Debug("Relating closest feature.");
                        RelateObjects(deviceGroupFeature, closestDeviceGroupChild);
                    }
                }
                finally
                {
                    _logger.Debug("Releasing IfeatureCursor.");
                    while (Marshal.ReleaseComObject(deviceGroupChildCursor) > 0) 
                    {
                    }
                    _logger.Debug("Released IfeatureCursor.");
                }
            }
        }

        /// <summary>
        /// Executes when DeviceGroup is updated.
        /// </summary>
        /// <param name="pObject">Object being updated.</param>
        private void DeviceGroupUpdated(IObject pObject)
        {
            IFeatureChanges objectFeatureChanges = pObject as IFeatureChanges;
            if (objectFeatureChanges != null)
            {
                _logger.Debug("Checking if Geometry is updated.");
                if (objectFeatureChanges.ShapeChanged)
                {
                    _logger.Debug("Geometry is updated so calling CreateGroup.");
                    DeviceGroupCreated(pObject);
                }
            }
        }

        /// <summary>
        /// Returns All features from passed Featureclass where Created/Updated 
        /// feature is within buffer of registry/default value.
        /// </summary>
        /// <param name="DeviceFeature">Feature being created/updated.</param>
        /// <param name="currentFc">Featureclass from which features will be retrieved using spatial filter.</param>
        /// <param name="onlyUnrelated">True if only unrelated features required.</param>
        /// <returns>
        /// Returns FeatureCursor from passed Featureclass where Created/Updated 
        /// feature is within buffer of registry/default value.
        /// </returns>
        private IFeatureCursor GetIFeatureCursor(IFeature DeviceFeature, IFeatureClass currentFc, bool onlyUnrelated)
        {
            ISpatialFilter sfilter = new SpatialFilter();
            ITopologicalOperator iTopo = (ITopologicalOperator)DeviceFeature.Shape;
            IGeometry queryGeom = iTopo.Buffer(GetRegistryValue());
            sfilter.Geometry = queryGeom;
            sfilter.GeometryField = currentFc.ShapeFieldName;
            sfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            //get features that already are not related.
            if (onlyUnrelated)
            {
                sfilter.WhereClause = _destFeatureClassFieldName + " is null";
            }
            else
            {
                sfilter.WhereClause ="";
            }
            _logger.Debug("Ready to execute SpatialFilter");
            int count = currentFc.FeatureCount(sfilter);
            _logger.Debug("Execute SpatialFilter("+ count+")");
            IFeatureCursor SelectedCursor = currentFc.Search(sfilter, true);
            _logger.Debug("Ready to return IFeatureCursor");
            return SelectedCursor;
        }
        
/// <summary>
/// 
/// </summary>
/// <param name="DeviceFeature"></param>
/// <param name="SelectedCursor"></param>
/// <param name="isCursorDeviceGroup">Default false true only if PassedCursor </param>
/// <returns></returns>
        private IFeature GetClosestFeature(IFeature DeviceFeature, IFeatureCursor SelectedCursor, bool isCursorDeviceGroup=false)
        {
            IProximityOperator proximityOperator = DeviceFeature.Shape as IProximityOperator;
            IFeature currentFeature = SelectedCursor.NextFeature();
            IFeature closestFeature = null;
            double distance = 100000;//keep this as default higher side value
            _logger.Debug("Finding Closest feature.");
            do
            {
                if (currentFeature != null)
                {
                    if (isCursorDeviceGroup)
                    {
                        _logger.Debug("This is DeviceGroup Cursor so need to take care if this DeviceGroup is already related to same type of DeviceGroupChild if so then we will not return that device group and continue the search for another DeviceGroup.");
                        IRelationshipClass relationshipClass= GetIRelationshipClass(DeviceFeature, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
                        ESRI.ArcGIS.esriSystem.ISet tempRelatedObjectSet = relationshipClass.GetObjectsRelatedToObject(currentFeature);
                        if(tempRelatedObjectSet.Count>0)
                        {
                            currentFeature = SelectedCursor.NextFeature();
                            continue;
                        }
                    }
                    double currentDistance = proximityOperator.ReturnDistance(currentFeature.Shape);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestFeature = currentFeature;
                    }
                    currentFeature = SelectedCursor.NextFeature();
                }
            } while (currentFeature != null);
            _logger.Debug("Found Closest feature.");
            return closestFeature;
        }

        /// <summary>
        /// Returns Original Feature (most probably DeviceGroup) and Destination Feature (DeviceGroupChild) relationship class object
        /// </summary>
        /// <param name="ChildFeature">Child Feature from relationship(most probably DeviceGroupChild).</param>
        /// <param name="ModelName">Parent Feature Model Name from relationship(most probably DeviceGroup).</param>
        /// <returns>IRelationshipClass object between Parent Feature and Child Feature using passed ModelName.</returns>
        private IRelationshipClass GetIRelationshipClass(IFeature ChildFeature, string ModelName)
        {
            _logger.Debug("Geting RelationshipClass.");
            IObjectClass childFeatureClass = ChildFeature.Class as IObjectClass;
            IRelationshipClass relClass = ChildFeature.GetRelationships(ModelName).FirstOrDefault();
            _logger.Debug("Got RelationshipClass.");
            return relClass;
        }

        /// <summary>
        /// Returns List of IRelationshipClass.
        /// </summary>
        /// <param name="ChildFeature">The Feature</param>
        /// <param name="ModelName">The Model Name</param>
        /// <returns>Returns List of IRelationshipClass.</returns>
        private List<IRelationshipClass> GetIRelationshipClasses(IFeature ChildFeature, string ModelName)
        {
            _logger.Debug("Getting all RelationshipClasses.");
            IObjectClass childFeatureClass = ChildFeature.Class as IObjectClass;
            List<IRelationshipClass> relClasses = ChildFeature.GetRelationships(ModelName).ToList();
            _logger.Debug("Got all RelationshipClasses."); 
            return relClasses;
        }

        /// <summary>
        /// Relate Original Feature (most probably DeviceGroup) with Destination Feature (DeviceGroupChild)
        /// </summary>
        /// <param name="OrigFeature">Parent Feature from relationship(most probably DeviceGroup).</param>
        /// <param name="DestFeature">Child Feature from relationship(most probably DeviceGroupChild).</param>
        private void RelateObjects(IFeature OrigFeature, IFeature DestFeature)
        {
            _logger.Debug("Relating Objects.");
            bool isDeviceGroup = ModelNameFacade.ContainsClassModelName(OrigFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.DeviceGroup });
            bool isDeviceGroupChild = ModelNameFacade.ContainsClassModelName(OrigFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.DeviceGroupChild });
            IRelationshipClass RelationClass = null;
            //Take care if DeviceGroup and Device GroupChild is passed incorrectly.
            if(isDeviceGroup)
            {
                RelationClass = GetIRelationshipClass(DestFeature, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
                if (RelationClass != null)
                {
                    _logger.Debug("Create relationship.");
                    RelationClass.CreateRelationship(OrigFeature, DestFeature);
                    _logger.Debug("Relationship created.");
                }
            }
            else if(isDeviceGroupChild)
            {
                RelationClass = GetIRelationshipClass(OrigFeature, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
                if (RelationClass != null)
                {
                    _logger.Debug("Create relationship."); 
                    RelationClass.CreateRelationship(DestFeature, OrigFeature);
                    _logger.Debug("Relationship created.");
                }
            }
            _logger.Debug("Related Objects.");
        }

        /// <summary>
        /// Unrelated the Feature from the Modelname.
        /// </summary>
        /// <param name="OrigFeature">The Feature</param>
        private void UnRelateAllObjects(IFeature OrigFeature)
        {
            bool isDeviceGroup = ModelNameFacade.ContainsClassModelName(OrigFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.DeviceGroup });
            bool isDeviceGroupChild = ModelNameFacade.ContainsClassModelName(OrigFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.DeviceGroupChild });
            string OppModelName = "";

            if (isDeviceGroup)
            {
                OppModelName = SchemaInfo.Electric.ClassModelNames.DeviceGroupChild;
            }
            else if (isDeviceGroupChild)
            {
                OppModelName = SchemaInfo.Electric.ClassModelNames.DeviceGroup;
            }

            IFeatureClass OrigFeatureClass = OrigFeature.Class as IFeatureClass;
            IDataset OrigFeatureDS = OrigFeatureClass as IDataset;
            List<IRelationshipClass> ListIrelationClasses = GetIRelationshipClasses(OrigFeature, OppModelName);
            IEnumerator<IRelationshipClass> EnumRelClass = ListIrelationClasses.GetEnumerator();
            
            EnumRelClass.Reset();
            while (EnumRelClass.MoveNext())
            {
                IRelationshipClass RelClass =EnumRelClass.Current;
                RelClass.DeleteRelationshipsForObject(OrigFeature);
            }
        }
        
        #endregion Privates

        #region SpecialAU Overrides
        /// <summary>
        /// Decide if AUMode is other than AUArcMap do not execute
        /// </summary>
        /// <param name="eAUMode"></param>
        /// <returns></returns>
        protected override bool CanExecute(Miner.Interop.mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == Miner.Interop.mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enable the AU when the Create/Update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.DeviceGroup, SchemaInfo.Electric.ClassModelNames.DeviceGroupChild };
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, _modelNames);
            _logger.Debug(string.Format("ClassModelName : {0} or {1} Exist:{2}", SchemaInfo.Electric.ClassModelNames.DeviceGroup, SchemaInfo.Electric.ClassModelNames.DeviceGroupChild, enabled ));
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate ||
                eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate) && (enabled))
            {
                _logger.Debug("Returning Visible:true");
                return true;
            }
            _logger.Debug("Returning Visible:false");
            return false;
        }

        /// <summary>
        /// Executes the RelateDeviceGroup AU.
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            //Get the DeviceGroupSearchTolerance Registry value, if this value not found then use 5 feet as default value
            //double validDistance= GetRegistryValue();

            IRow FeatureClassRow = pObject as  IRow;
            IFeature DeviceFeature = FeatureClassRow as IFeature;
            ITable FeatureClassTable =FeatureClassRow.Table;
            IDataset FeatureClassDS = FeatureClassTable as IDataset;
            IFeatureClass DeviceFeatureClass = FeatureClassDS as IFeatureClass;
            //Check Object Placed is from which ModelName DeviceGroup or DeviceGroupChild

            bool isDeviceGroup = ModelNameFacade.ContainsClassModelName(DeviceFeatureClass, new string[]{SchemaInfo.Electric.ClassModelNames.DeviceGroup});
            bool isDeviceGroupChild = ModelNameFacade.ContainsClassModelName(DeviceFeatureClass, new string[]{SchemaInfo.Electric.ClassModelNames.DeviceGroupChild});
            if (isDeviceGroup)
            {
                ////if DeviceGroup ClassModelName then                
                ExecuteForDeviceGroup(pObject, eEvent);
            }
            else if (isDeviceGroupChild)
            {
                //else if DeviceGroupChildClassModelName then
                ExecuteForDeviceGroupChild(pObject, eEvent);
            }
        }
        #endregion SpecialAU Overrides

    }
}
