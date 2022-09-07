using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;


namespace PGE.Desktop.EDER.AutoUpdaters
{
    public class BaseSplitAU : BaseSpecialAU
    {
        protected static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Caches field values for the original object.
        /// </summary>
        protected static Dictionary<int, object> _originalObjectFields = null;
        protected static Hashtable _originalRelatedObjects = null;

        #region Constructor

        /// <summary>
        /// Constructor for BaseSplitAU
        /// </summary>
        /// <param name="name">Name of the AU</param>
        public BaseSplitAU(string name)
            : base(name)
        { }

        #endregion

        #region BaseSpecialAU overrides

        /// <summary>
        /// Determines in which class the AU will be enabled.
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool retVal = false;
            if (eEvent == mmEditEvent.mmEventBeforeFeatureSplit || eEvent == mmEditEvent.mmEventAfterFeatureSplit)
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventBeforeFeatureSplit)
            {
                _originalObjectFields = new Dictionary<int, object>();
                _originalRelatedObjects = new Hashtable();
                CollectFieldValues(obj);
                CollectRelatedObjects(obj);
            }
            InternalExecuteExtended(obj, eAUMode, eEvent);
            if (eEvent == mmEditEvent.mmEventAfterFeatureSplit)
            {
                _originalObjectFields.Clear();
                _originalRelatedObjects.Clear();
                _originalObjectFields = null;
                _originalRelatedObjects = null;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Extended implementation of InternalExecute method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected virtual void InternalExecuteExtended(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent) { }

        #endregion

        #region private methods

        /// <summary>
        /// Collects and caches original field values for the object about to be split.
        /// </summary>
        /// <param name="obj">The object being split.</param>
        private void CollectFieldValues(IObject obj)
        {
            try
            {
                for (int i = 0; i < obj.Fields.FieldCount; i++)
                {
                    object fieldValue = obj.get_Value(i);
                    if (fieldValue is IGeometry)
                    {
                        _originalObjectFields.Add(i, ((IFeature)obj).ShapeCopy);
                    }
                    else
                    {
                        _originalObjectFields.Add(i, fieldValue);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error collecting original field values.", ex);
                _originalObjectFields.Clear();
                _originalObjectFields = null;
            }
        }


        private void CollectRelatedObjects(IObject pTargetObject)
        {
            try
            {
                string datasetName = "";
                Hashtable hshOids = null;
                IEnumRelationshipClass pEnumRelClass = null;
                IObject pRelatedObj = null;
                pEnumRelClass = pTargetObject.Class.get_RelationshipClasses(
                        esriRelRole.esriRelRoleAny);
                pEnumRelClass.Reset();
                IRelationshipClass pRelClass = pEnumRelClass.Next();
                while (pRelClass != null)
                {
                    if (pTargetObject != null)
                    {
                        ESRI.ArcGIS.esriSystem.ISet relatedSet = pRelClass.GetObjectsRelatedToObject(pTargetObject);
                        relatedSet.Reset();
                        pRelatedObj = (IObject)relatedSet.Next();

                        while (pRelatedObj != null)
                        {
                            if (ModelNameFacade.ContainsClassModelName(
                                pRelatedObj.Class,
                                new string[] { SchemaInfo.Electric.ClassModelNames.RestoreRels }))
                            {

                                //Store the related object 
                                datasetName = ((IDataset)pRelatedObj.Class).Name.ToUpper();
                                if (!_originalRelatedObjects.ContainsKey(datasetName))
                                    _originalRelatedObjects.Add(datasetName, new Hashtable());
                                hshOids = (Hashtable)_originalRelatedObjects[datasetName];
                                if (!hshOids.ContainsKey(pRelatedObj.OID))
                                {
                                    hshOids.Add(pRelatedObj.OID, 0);
                                }
                                _originalRelatedObjects[datasetName] = hshOids;

                            }
                            else
                            {
                                break;
                            }
                            pRelatedObj = (IObject)relatedSet.Next();
                        }
                        Marshal.FinalReleaseComObject(relatedSet);
                    }
                    pRelClass = pEnumRelClass.Next();
                }
                Marshal.FinalReleaseComObject(pEnumRelClass);
            }
            catch (Exception ex)
            {
                _logger.Error("Error storing related objects. ", ex);
                _originalRelatedObjects.Clear();
                _originalRelatedObjects = null;
            }
        }

        #endregion

    }
}
