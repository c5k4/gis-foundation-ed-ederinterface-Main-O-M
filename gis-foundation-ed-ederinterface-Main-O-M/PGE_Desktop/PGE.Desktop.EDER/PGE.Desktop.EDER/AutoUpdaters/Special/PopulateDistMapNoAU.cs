using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;
using ESRI.ArcGIS.Carto;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Class to Manage PG&E Unique Circuit Id
    /// </summary>
    [ComVisible(true)]
    [Guid("3A7E021F-E60B-4549-9534-6A482107C258")]
    [ProgId("PGE.Desktop.EDER.PopulateDistMapNoAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PopulateDistMapNoAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes the new instance of <see cref="PopulateDistMapNoAU"/>
        /// </summary>
        public PopulateDistMapNoAU() : base("PGE Populate Distribution MapNumber") { }
        
        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");     
        /// <summary>
        /// Instance of <see cref="PopulateDistMapNo"/> class
        /// </summary>
        private PopulateDistMapNo _popDistMapNo = new PopulateDistMapNo();

        // Add a sneaky option allowing us to bypass ArcMap only check. The bypass is disabled by default but callers can turn it on via property
        private static bool _allowExecutionOutsideOfArcMap = false;
        public static bool AllowExecutionOutsideOfArcMap
        {
            get { return _allowExecutionOutsideOfArcMap; }
            set
            {
                _allowExecutionOutsideOfArcMap = value;
            }
        }

        #endregion Private Variables

        #region Override Methods
        /// <summary>
        /// Enable the AU when the Create/Update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            //Enable if Feature Event type is Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                //Checks for ObjectClass has PGE_DISTMAPUPDATE
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DistMapUpdate);
            }

            return enabled;
        }

        /// <summary>
        /// Execute the AU while working with ArcMap Application only.
        /// </summary>
        /// <param name="eAUMode">The AU application mode.</param>
        /// <returns>True if condition satisfied else false.</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap || _allowExecutionOutsideOfArcMap == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Execute Populate DistribuMapNoAU AU
        /// </summary>
        /// <param name="obj">The Object being Updated.</param>
        /// <param name="eAUMode">The AU Mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            bool objectChange = false;

            if (!ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.DistMapUpdate))
            {
                return;
            }

            //checks object class fields assigned with ModelName.
            _popDistMapNo.MapNoDeviceFieldIndex = ModelNameFacade.FieldIndexFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.ElecMapNumber);
            _popDistMapNo.LocalOfficeDeviceFieldIndex = ModelNameFacade.FieldIndexFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
            _popDistMapNo.MapOfficeDeviceFieldIndex = ModelNameFacade.FieldIndexFromModelName(pObject.Class, SchemaInfo.General.FieldModelNames.MapOfficeMN);
            if (_popDistMapNo.MapNoDeviceFieldIndex == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.ElecMapNumber + " - FieldModelname is not assigned for objectclass, exiting.");
                return;
            }

            if (_popDistMapNo.LocalOfficeDeviceFieldIndex == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.LocalOfficeID + " - FieldModelname is not assigned for objectclass, exiting.");
                return;
            }
            else
            {
                _popDistMapNo.LocalOfficeDeviceField  = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                if (_popDistMapNo.LocalOfficeDeviceField == null)
                {
                    _logger.Debug("Not found LocalOfficeIDField, exiting.");
                    return;
                }
            }
            
            //Checks Feature Edit Event for Update
            if (eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                //Checks If ObectClass field Shape or LocalOfficeId is changed.
                objectChange = IsRowAndFeatureChanged(pObject);
            }

            //Checks for Feature Edit Event for Create or ObectClass field Shape or LocalOfficeId is changed.
            if (objectChange || eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                //Execute populate Distribution MapNo to object Created or Updated.
               _popDistMapNo.ExecuteUpdateMapNoFromDistMap(pObject);
            }
        }
        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Checks the ObjectClass fields for changes on Shape or LocalOfficeId.
        /// </summary>
        /// <param name="pObject">The Object being created or updated.</param>
        /// <returns>Return as True if changed Else False.</returns>
        private bool IsRowAndFeatureChanged(IObject pObject)
        {
            bool objectChange = false;
            IRowChanges changeRow = pObject as IRowChanges;
            IFeatureChanges ChangeFea = pObject as IFeatureChanges;

            if (changeRow != null && ChangeFea != null)
            {
                //Checks ObjectClass LocalOfficeId field value is changed
                if (changeRow.get_ValueChanged(_popDistMapNo.LocalOfficeDeviceFieldIndex))
                {                    
                    objectChange = true;
                }

                //Checks ObjectClass shape field value is changed
                if (ChangeFea.ShapeChanged)
                {                    
                     objectChange = true;
                }
                
                //Checks LocalOfficeId and Shape is changed
                if (!objectChange)
                {
                    _logger.Debug("No change in " + pObject.Class.AliasName + " class, exiting.");
                }
            }
            else
            {
                if (changeRow != null && ChangeFea == null)
                {
                    _logger.Debug("ChangeFea is null, exiting.");
                }
                if (changeRow == null && ChangeFea != null)
                {
                    _logger.Debug("changeRow is null, exiting.");
                }
                if (changeRow == null && ChangeFea == null)
                {
                    _logger.Debug("changeRow and ChangeFea are null, exiting.");
                }
                return objectChange;
            }

            return objectChange;
        }

        #endregion Private Methods
    }
}
