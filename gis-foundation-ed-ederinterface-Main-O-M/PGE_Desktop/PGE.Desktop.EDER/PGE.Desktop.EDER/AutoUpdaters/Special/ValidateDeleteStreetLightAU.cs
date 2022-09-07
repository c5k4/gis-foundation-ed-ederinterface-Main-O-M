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

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("EA142233-4E4D-4042-9A16-5E132F7AC006")]
    [ProgId("PGE.Desktop.EDER.ValidateDeleteStreetLightAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ValidateDeleteStreetLightAU : BaseSpecialAU
    {
        #region Static Methods
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string V_SLCDX_DATA_ACTIVE_TblName = "PGEDATA.V_SLCDX_DATA_ACTIVE";
        private const string UNIQUE_SP_ID_FldName = "UNIQUE_SP_ID";
        private const string StreetL_UNIQUESPID_FldName = "UNIQUESPID";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public ValidateDeleteStreetLightAU()
            : base("PGE Delete StreetLight Validate")
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
            bool isEnabled = false;
            if (eEvent == mmEditEvent.mmEventFeatureDelete)
                isEnabled = true;
            return isEnabled;
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
            
            //1.Determine if the object has  active UNIQUESPID, and cancel edit if active UNIQUESPID found.

            string stringError = string.Empty;
            string stringErrorTitle = string.Empty;
            var featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)obj).Table).Workspace;
            ITable slcdUniqueView =featureWorkspace.OpenTable(V_SLCDX_DATA_ACTIVE_TblName);
            if(slcdUniqueView == null)
            {
                stringErrorTitle = "Table not accessable";
                stringError = "Table not accessable: Table Name: " + V_SLCDX_DATA_ACTIVE_TblName;
                        MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
            

            #region Check for Unique UNIQUESPID
            {
                //Check if UNIQUESPID is active. Cancel edit for active ones.
                string newSPIDValue = GetValue(obj as IRow, StreetL_UNIQUESPID_FldName);
                if (!string.IsNullOrEmpty(newSPIDValue))
                {
                    //Checks New Circuit ID value already exists   
                    string sql = UNIQUE_SP_ID_FldName + "='" + newSPIDValue + "'"; 
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = sql;
                    pFilter.SubFields = UNIQUE_SP_ID_FldName;
                    int rowCount = slcdUniqueView.RowCount(pFilter);

                    //Prompt user for active UNIQUESPID-delete Violation
                    if (rowCount > 0)
                    {
                        stringErrorTitle = "Active Unique SPID Violation";
                        stringError = "The Unique SPID is active. StreetLight with active Unique SPID cannot be deleted. SQL: " + sql;
                        //MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }
                //else
                //{
                //    stringError = "The UNIQUESPID must not be null.";
                //    _logger.Error(stringError);
                //    throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                //}
            }
            #endregion
                    

        }
        #endregion

        #region Private function
        private static string GetValue(IRow row, string fieldName)
        {
            try
            {
                int fieldIndex = row.Fields.FindField(fieldName);
                if (fieldIndex == -1)
                    throw new Exception("Missing field name: " + fieldName);

                if (row.get_Value(fieldIndex) != System.DBNull.Value)
                {
                    object obj = row.get_Value(fieldIndex);
                    string strValue = null;
                    if (obj != null)
                    {
                        strValue = row.get_Value(fieldIndex).ToString();
                    }
                    if (!string.IsNullOrEmpty(strValue))
                        return strValue;
                }
                return null;
            }
            catch (Exception ex)
            {
                //_logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Name: " + fieldName + "," + ex.Message);
                string stringErrorTitle = "Failed to get Field Value";
                string stringError = " Field Name: " + fieldName + ". Err Msg: " + ex.Message;
                _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + stringError);
                MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

    }
}
