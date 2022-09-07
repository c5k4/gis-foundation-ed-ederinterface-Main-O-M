using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// A special AU that retains the last FeederID.
    /// </summary>
    [Guid("B7C02712-7B31-4846-B505-1141D37C83C7")]
    [ProgId("PGE.Desktop.EDER.CircuitID")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class CircuitID : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _warningMsg = "Field Model name :{0},{1} not found.";
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor, pass in AU name.
        /// </summary>
        public CircuitID()
            : base("PGE Retain CircuitID")
        {
        }

        #endregion

        #region Override
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName);
                _logger.Debug("Class model name: " + SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName + " Found -" + enabled);

            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMFeederManager || eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
           // IObjectClass objClass = obj.Class;
            //get IField from modelname facade
            IField circuitIdFld =ModelNameFacade.FieldFromModelName(obj.Class,SchemaInfo.Electric.FieldModelNames.FeederID); //ModelNameManager.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.FeederID);
            IField lastFeederIdFld =ModelNameFacade.FieldFromModelName(obj.Class,SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName); //ModelNameManager.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName);
           
            if (circuitIdFld != null && lastFeederIdFld != null)
            {
                //string fldName = circuitIdFld.Name;
                //int fldIndex = objClass.FindField(fldName);
                //object feederIdVal = obj.get_Value(fldIndex);
                //get feederID field value
                object feederIdVal = obj.GetFieldValue(circuitIdFld.Name,false,SchemaInfo.Electric.FieldModelNames.FeederID);
                _logger.Debug("FeederID field Value: " +feederIdVal);

                if (feederIdVal != null && feederIdVal != DBNull.Value)//if not null
                {
                    //fldName = lastFeederIdFld.Name;
                    //get field index
                    int fldIndex = obj.Class.FindField(lastFeederIdFld.Name);
                    //set field value to last feedrerID
                    obj.set_Value(fldIndex, feederIdVal);
                }
            }
            else
            {
                _logger.Debug(string.Format(_warningMsg,SchemaInfo.Electric.FieldModelNames.FeederID,SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName));
            }
        }
        #endregion
    }
}
