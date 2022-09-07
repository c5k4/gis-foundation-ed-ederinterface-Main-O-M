using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using Miner.ComCategories;
using Miner.Interop;
using System.Linq;
using System.Reflection;
using log4net;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    [ProgId("PGE.Desktop.EDER.SupportStructureLabel")]
    [Guid("68194680-31BE-440C-BCC9-96A521DECA91")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SupportStructureLabel : BaseLabelTextAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //private const string _customerOwned = "CustomerOwned";
        //private const string  _jointOwner= "Joint Owner";
        private const string _capitalizedText = "Y";
        private const string _unkCodeValue = "UN";
        #endregion

        enum Subtypes
        {
            None,
            Pole
        }

        #region Constructor

        /// <summary>
        /// Constructor, pass in name and the number of label texts this AU will build.
        /// </summary>
        public SupportStructureLabel()
            : base("PGE SupportStructure LabelText AU")
        {
        }

        #endregion

        /// <summary>
        ///   Implementation of AutoUpdater Enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass"> The object class. </param>
        /// <param name="editEvent"> The edit event. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        /// <remarks>
        ///   This method will be called from IMMSpecialAUStrategy::get_Enabled and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent editEvent)
        {
            bool enabled = false;
            bool isSupportStrucureBankClass = ModelNameFacade.ContainsAllClassModelNames(objectClass, new string[] { SchemaInfo.Electric.ClassModelNames.SupportStructure, SchemaInfo.General.ClassModelNames.LabelTextBank });

            if (editEvent == mmEditEvent.mmEventFeatureCreate)
            {
                enabled = isSupportStrucureBankClass;
                _logger.Debug("Class model name :" + SchemaInfo.Electric.ClassModelNames.SupportStructure + "and Field model name:" + SchemaInfo.General.ClassModelNames.LabelTextBank + " Found-" + enabled);

            }
            else if (editEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                bool isJointOwnerUnitClass = ModelNameFacade.ContainsAllClassModelNames(objectClass, new string[] { SchemaInfo.Electric.ClassModelNames.JointOwner, SchemaInfo.General.ClassModelNames.LabelTextUnit });
                _logger.Debug("Class model name :" + SchemaInfo.Electric.ClassModelNames.JointOwner + " found-" + enabled);
                enabled = isSupportStrucureBankClass || isJointOwnerUnitClass;
            }

            return enabled;
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
        protected override string GetLabelText(IObject obj, mmAutoUpdaterMode autoUpdaterMode, mmEditEvent editEvent, int labelIndex)
        {


            var label = new StringBuilder();
            ////get value for custom owned
            string customerOwned = obj.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.CustomerOwned)
                                                       .Convert<string>()
                                                       .Contains("y", StringComparison.InvariantCultureIgnoreCase)
                                                    ? _capitalizedText
                                                    : string.Empty;
            //if customer owned not null
            if (!String.IsNullOrEmpty(customerOwned))
            {
                var subOwners = base
                      .GetRelatedObjects(obj, null, modelNames: SchemaInfo.Electric.ClassModelNames.JointOwner)
                      .Select(o => o.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.JOName).Convert<string>())
                      .Where(o => !string.IsNullOrEmpty(o))// && o != _unkCodeValue)
                      .GroupBy(o => new { value = o });

                //loop each group values
                foreach (var g in subOwners)
                {
                    if (label.Length > 0)
                    {
                        label.Append(", ");
                    }

                    int count = g.Count();
                    if (count > 1)
                    {
                        label.Append(count);
                        label.Append("-");
                    }
                    label.Append(g.Key.value);
                }
                _logger.Debug("Attribute value :" + label);
                return label.ToString();
            }
            return "";
            #region older code
            //var customerOwned = obj.GetFieldValue(_customerOwned).Convert(string.Empty);
            //_logger.Debug(_customerOwned + " : " + customerOwned);
            //if (customerOwned.ToString().ToUpper() != "YES")
            //{
            //    return null;
            //}

            //var subOwners = base
            //    .GetRelatedObjects(obj,_jointOwner, modelNames: SchemaInfo.General.ClassModelNames.LabelTextUnit)
            //    .Select(o => o.GetFieldValue(_jOName,false).Convert("<null>")) // TODO: Maybe, may change.
            //    .Concatenate(" ");
            //if (subOwners.IsNullOrWhitespace()) // if null or empty
            //{
            //    return null;
            //}
            //_logger.Debug("Attribute value :" + subOwners);
            //return subOwners;
            #endregion
        }
    }
}