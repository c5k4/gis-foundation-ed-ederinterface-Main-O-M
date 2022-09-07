// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Data Validation-Misc XFR - EDER Component Specification
// Shaikh Rizuan 10/11/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using System.Reflection;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    ///  used populate the As-Built SourceSide field on the Voltage Regulator featureclass.
    /// </summary>
    [ComVisible(true)]
    [Guid("B72F8734-8875-4D4D-B108-B2DA77D70A03")]
    [ProgId("PGE.Desktop.EDER.UpperCaseAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UpperCaseAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #region Constructor
         /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public UpperCaseAU() : base("PGE Upper Case AU")
        {

        }
       #endregion

        #region Base Special AU Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent ==  mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                string[] strModelNames = new string[] { SchemaInfo.Electric.FieldModelNames.UpperCase };
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, strModelNames);
                _logger.Debug("Class model name: " + SchemaInfo.Electric.FieldModelNames.UpperCase + " Found -" + enabled);
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
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
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
            List<int> fldIndices = new List<int>();

            //Get the field indices from model name
               fldIndices = ModelNameFacade.FieldIndicesFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.UpperCase);

            //Check for event
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                for (int i = 0; i < fldIndices.Count; i++)//loop through each field index
                {
                    // get field value
                    object fldValue = obj.GetFieldValue(obj.Fields.get_Field(fldIndices[i]).Name, false, SchemaInfo.Electric.FieldModelNames.UpperCase).Convert<object>(System.DBNull.Value);
                    
                    //check if field value is not null and don't have domain assigned and field type should be string
                    if (fldValue != System.DBNull.Value && obj.Fields.get_Field(fldIndices[i]).Domain==null && obj.Fields.get_Field(fldIndices[i]).Type==esriFieldType.esriFieldTypeString )//|| obj.Fields.get_Field(fldIndices[i]).Type==esriFieldType.esriFieldTypeGlobalID || obj.Fields.get_Field(fldIndices[i]).Type==esriFieldType.esriFieldTypeGUID)
                    {
                        //set the value to upper case
                        obj.set_Value(fldIndices[i], fldValue.ToString().ToUpper());
                    }
                }
            }
        }
        #endregion
    }
}
