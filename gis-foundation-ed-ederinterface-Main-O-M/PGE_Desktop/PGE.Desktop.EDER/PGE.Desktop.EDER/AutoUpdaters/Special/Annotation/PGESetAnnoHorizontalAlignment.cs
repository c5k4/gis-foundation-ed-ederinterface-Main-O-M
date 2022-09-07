using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using Telvent.Delivery.ArcFM;
using Telvent.Delivery.Diagnostics;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.Special.Annotation
{
    /// <summary>
    /// A special AU that retains the last FeederID.
    /// </summary>
    [Guid("C1A51ECC-16D6-4B91-A663-C803E7DD9B60")]
    [ProgId("Telvent.PGE.ED.PGESetAnnoHorizontalAlignment")]
    //[ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGESetAnnoHorizontalAlignment : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string _fieldName = "HORIZONTALALIGNMENT";
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor, pass in AU name.
        /// </summary>
        public PGESetAnnoHorizontalAlignment()
            : base("PGE Align Anno Horizontal Alignment")
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

            if (eEvent == mmEditEvent.mmEventFeatureUpdate && objectClass.Extension is IAnnotationClassExtension)
            {
                enabled = true;
                _logger.Debug("Set Anno Horizontal Alignment -" + enabled);
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
            _logger.Debug("Checking if ObjectClass.Extension is instance of an IAnnotationClassExtension.");
            if (obj.Class.Extension is IAnnotationClassExtension)
            {
                _logger.Debug("ObjectClass.Extension is instance of an IAnnotationClassExtension, Getting FieldIndex for FieldName: " + _fieldName);
                int horizontalAlignmentFldIdx = obj.Fields.FindField(_fieldName);
                _logger.Debug("FieldIndex for FieldName: " + _fieldName + " FieldIndex: " + horizontalAlignmentFldIdx);

                //If the user has changed the Horizontal Alignment we need not change this.
                _logger.Debug("Checking if user has changed horizontal alignment.");
                if (((IRowChanges)obj).get_ValueChanged(horizontalAlignmentFldIdx))
                {
                    _logger.Debug("No need to update Horizontal alignment as user has changed it.");
                    return;
                }
                _logger.Debug("Updating Horizontal alignment.");
                obj.set_Value(horizontalAlignmentFldIdx, AnnoPropertyFactory.HorizontalAlignment(obj as IFeature, esriTextHorizontalAlignment.esriTHALeft));
                _logger.Debug("Updated Horizontal alignment.");
            }
        }
        #endregion
    }
}
