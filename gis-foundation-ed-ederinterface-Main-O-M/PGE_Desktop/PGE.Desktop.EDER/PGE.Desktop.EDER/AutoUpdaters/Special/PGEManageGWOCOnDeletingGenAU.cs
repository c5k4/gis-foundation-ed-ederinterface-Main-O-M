using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

// Changes for ENOS to SAP migration - Code changes for ENOS to SAP migration
namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("4A28976E-22C2-00A4-E053-0AF4574D489D")]
    [ProgId("PGE.Desktop.EDER.ProcessGWOCOnDeletingGenAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEManageGWOCOnDeletingGenAU : BaseSpecialAU
    {
        #region Static Methods
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #endregion

        private static string _gwocTableName = "EDGIS.GWOC";
        private static string _FieldNamegenGUID = "GENERATIONGUID";
        private static string _FieldNameglobalid = "GLOBALID";
        private static string _FieldNameresolved = "RESOLVED";
        private static string _FieldNamereComments = "COMMENTS";
        

        #region Constructor
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public PGEManageGWOCOnDeletingGenAU()
            : base("PGE Manage GWOC on Deleting Generation")
        {

        }
        #endregion


        #region Base special AU Overrides
        /// <summary>
        /// Determines when the AU should be enabled 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {            
            bool ClassEnabled = ModelNameFacade.ContainsClassModelName(objectClass, new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo });

            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.GenerationInfo, objectClass.AliasName, ClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (ClassEnabled)));
            return (ClassEnabled && (eEvent == mmEditEvent.mmEventFeatureDelete));           
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

            if (eEvent != mmEditEvent.mmEventFeatureDelete)
                return;
            string msg = "Deleting " + obj.Class.AliasName + " with oid: " +
                obj.OID.ToString() + " in " + base._Name;
            _logger.Debug(msg);

            // Process GWOC when generation record is deleted 
            if (eEvent == mmEditEvent.mmEventFeatureDelete)
            GWOCRecordProcessOnGenDelete(obj);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This function will manage GWOC when a generationinfo record will be deleted
        /// </summary>
        /// <param name="argObject"></param>
        private void GWOCRecordProcessOnGenDelete(IObject argObject)
        {
            try
            {
                IWorkspace pWS = ((IDataset)argObject.Class).Workspace;
                IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;

                ITable pTable = pFWS.OpenTable(_gwocTableName);

                //Set the GWOC resolved to TST 
                IQueryFilter pQf = new QueryFilterClass();
                pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argObject.get_Value(argObject.Fields.FindField(_FieldNameglobalid))) + "'";

                if (pTable.RowCount(pQf) > 0)
                {
                    ICursor pCursor = pTable.Update(pQf, false);

                    if (pCursor != null)
                    {
                        IRow pRow = pCursor.NextRow();

                        while (pRow != null)
                        {
                            pRow.set_Value(pRow.Fields.FindField(_FieldNamereComments), "TST");
                            pRow.Store();
                            pRow = pCursor.NextRow();
                        }
                    }
                }
                else
                {
                    string genGUID = Convert.ToString(argObject.get_Value(argObject.Fields.FindField(_FieldNameglobalid)));
                    string comments = "TST";
                    IRow pnewRow = pTable.CreateRow();
                    pnewRow.set_Value(pnewRow.Fields.FindField(_FieldNamegenGUID), genGUID);
                    pnewRow.set_Value(pnewRow.Fields.FindField(_FieldNamereComments), comments);
                    pnewRow.Store();
                }

            }
            catch (Exception exp)
            {
                _logger.Error("Error occured while processing GWOC record. exp : "+exp.Message);                
            }
        }

        #endregion Private Methods

    }
}
