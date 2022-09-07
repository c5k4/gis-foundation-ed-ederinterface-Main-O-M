using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Populate  Install Job Year Field Value.
    /// </summary>
    [ComVisible(true)]
    [Guid("B67756A8-DB97-4061-8504-3419FFFDDCDC")]
    [ProgId("PGE.Desktop.EDER.PGEAllowableEdits")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEAllowableEdits : BaseSpecialAU
    {
        #region Static Methods
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string _warningMsg = "Class model name :{0} found";
        private static readonly string _cancelEdit ="Cancelling the edits.";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public PGEAllowableEdits(): base("PGE Allowable Edits")
        {

        }
        #endregion

        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //check create event and model name
            Func<mmEditEvent, bool> chkCreateEnabled = mmEvent =>mmEvent == mmEditEvent.mmEventFeatureCreate && ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DonotCreateModelName) == true ? true : false;
            //check update event and model name
            Func<mmEditEvent, bool> chkUpdateEnabled = mmEvent => mmEvent == mmEditEvent.mmEventFeatureUpdate && 
                ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DonotUpdateModelName) &&
                !ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.AllowFieldedit) == true ? true : false;
            //check delete event and model name
            Func<mmEditEvent, bool> chkDeleteEnabled = mmEvent => mmEvent == mmEditEvent.mmEventFeatureDelete && ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DonotDeleteModelName) == true ? true : false;
            //check update event and model name
            Func<mmEditEvent, bool> chkAllowEditEnabled = mmEvent => mmEvent == mmEditEvent.mmEventFeatureUpdate &&
                !ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DonotUpdateModelName) &&
                ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.AllowFieldedit) == true ? true : false;
            
            if (chkCreateEnabled(eEvent))
            {
                _logger.Debug(string.Format(_warningMsg, SchemaInfo.Electric.ClassModelNames.DonotCreateModelName));
                return true;
            }
            else if (chkUpdateEnabled(eEvent))
            {
                _logger.Debug(string.Format(_warningMsg, SchemaInfo.Electric.ClassModelNames.DonotUpdateModelName));
                return true;
            }
            else if (chkDeleteEnabled(eEvent))
            {
                _logger.Debug(string.Format(_warningMsg, SchemaInfo.Electric.ClassModelNames.DonotDeleteModelName));
                return true;
            }
            else if (chkAllowEditEnabled(eEvent))
            {
                _logger.Debug(string.Format(_warningMsg, SchemaInfo.Electric.FieldModelNames.AllowFieldedit));
                return true;
            }

            return false;
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
            

            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DonotCreateModelName))
                {
                    string cancelMsg = "Creating " + obj.Class.AliasName + " objects is not allowed. " + _cancelEdit;
                    _logger.Debug(cancelMsg);
                    throw new COMException(cancelMsg, (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
            }

            if (eEvent == mmEditEvent.mmEventFeatureDelete)
            {
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DonotCreateModelName))
                {
                    string cancelMsg = "Deleting " + obj.Class.AliasName + " objects is not allowed. " + _cancelEdit;
                    _logger.Debug(cancelMsg);
                    throw new COMException(cancelMsg, (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
            }

            if(eEvent == mmEditEvent.mmEventFeatureUpdate) 
            {
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DonotUpdateModelName) == true &&
                    ModelNameFacade.ContainsFieldModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.AllowFieldedit) == false)
                {
                    string cancelMsg = "Updating " + obj.Class.AliasName + " objects is not allowed. " + _cancelEdit;
                    _logger.Debug(cancelMsg);
                    throw new COMException(cancelMsg, (int)mmErrorCodes.MM_E_CANCELEDIT);
                }

                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DonotUpdateModelName) == false &&
                    ModelNameFacade.ContainsFieldModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.AllowFieldedit) == true)
                {
                    if (!CheckAllowEdits(obj))
                    {
                        string cancelMsg = "Updating " + obj.Class.AliasName + " objects is not allowed with some fields. " + _cancelEdit;
                        _logger.Debug(_cancelEdit);
                        throw new COMException(cancelMsg, (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }
            }
        }
          #endregion

        #region Private Methods
        /// <summary>
        /// check whtether the field editd 
        /// </summary>
        /// <param name="obj">esri object</param>
        /// <returns>boolean</returns>
        private bool CheckAllowEdits(IObject obj)
        {
            IRowChanges rowChanges = obj as IRowChanges;
            bool chkEdits = false;
            List<int> fieldIndices = new List<int>();
            for (int i = 0; i < obj.Fields.FieldCount; i++)
            {
                if (rowChanges.get_ValueChanged(i))
                {
                    if (ModelNameFacade.ModelNameManager.ContainsFieldModelName(obj.Class, obj.Fields.get_Field(i), SchemaInfo.Electric.FieldModelNames.AllowFieldedit))
                    {
                        chkEdits = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            #region
            ////get all field indices from object having PGE_ALLOWEDIT model name
            //fieldIndices= ModelNameFacade.FieldIndicesFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.AllowFieldedit);
            ////loop through each index
            //foreach(int fldIndex in fieldIndices)
            //{
            //    //if field value changed
            //    if (rowChanges.get_ValueChanged(fldIndex))
            //    {
            //        //return true
            //        chkEdits = true;
            //        _logger.Debug( obj.Fields.Field[fldIndex].Name + " is edited.");
            //    }
            //}
            #endregion

            return chkEdits;
        }
        #endregion
    }
}
