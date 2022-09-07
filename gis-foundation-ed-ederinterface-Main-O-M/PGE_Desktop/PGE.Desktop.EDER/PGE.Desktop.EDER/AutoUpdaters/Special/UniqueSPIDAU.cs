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
    /// <summary>
    /// Ensures the Unique SP_ID of Streetlight feature and OFFICE/STREETLIGHTNUMBER/FIXTURECODE Violation
    /// </summary>
    [ComVisible(true)]
    [Guid("DD56D304-E700-4F07-A27F-0057C9CEDC92")]
    [ProgId("PGE.Desktop.EDER.UniqueSPIDAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UniqueSPIDAU : BaseSpecialAU
    {
        #region Constructor

        /// <summary>
        /// Initializes new instance of <see cref="UniqueSPIDAU"/> class
        /// </summary>
        public UniqueSPIDAU() : base("PGE Unique SPID") { }

        #endregion Constructor

        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string UNIQUESPID_FldName = "UNIQUESPID";
        private const string STREETLIGHTNUMBER_FldName = "STREETLIGHTNUMBER";
        private const string OFFICE_FldName = "LOCALOFFICEID";
        private const string FIXTURECODE_FldName = "FIXTURECODE";
        #endregion Private Variables

        #region Overridden BaseSpecialAU Methods

        /// <summary>
        /// Determines whether the AU should be Enabled when the Create/Update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            //Enable only if Feature Event type is either Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = true;
            }

            return enabled;
        }

        /// <summary>
        /// Executes the AU while working with ArcMap Application only.
        /// </summary>
        /// <param name="eAUMode">The AU application mode.</param>
        /// <returns>Returns true if condition satisfied else false.</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
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
        /// Execute UniqueSPID AU
        /// </summary>
        /// <param name="obj">The Object being Updated.</param>
        /// <param name="eAUMode">The AU Mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

            ITable tableStreetLight = pObject.Class as ITable;
            string stringError = string.Empty;
            string stringErrorTitle = string.Empty;
            string sql = null;

            // Get the status value
            IField statusField = ModelNameManager.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.Status);
            string status = GetValue(pObject as IRow, statusField.Name);

            // If the status isn't proposed
            if (status != "0")
            {
                //Below lines commented for ME Release - DA Item 11 (Streetlight Process Changes)
                //#region Check for Unique UNIQUESPID
                //{
                //    //Gets New UNIQUESPID value from input object and Unique OFFICE/STREETLIGHTNUMBER/FIXTURECODE, and determines if they are unique. 

                //    //Gets New UNIQUESPID value from input object  
                //    string newSPIDValue = GetValue(pObject as IRow, UNIQUESPID_FldName);
                //    if (!string.IsNullOrEmpty(newSPIDValue))
                //    {
                //        if (newSPIDValue != "0")
                //        {
                //            //Checks New Circuit ID value already exists   
                //            sql = UNIQUESPID_FldName + "='" + newSPIDValue + "'";
                //            IQueryFilter pFilter = new QueryFilterClass();
                //            pFilter.WhereClause = sql;
                //            pFilter.SubFields = UNIQUESPID_FldName;
                //            int rowCount = tableStreetLight.RowCount(pFilter);

                //            //Prompt user for UNIQUESPID Violation
                //            if (rowCount > 1)
                //            {
                //                stringErrorTitle = "UNIQUESPID Violation";
                //                stringError = "The UNIQUESPID must be a unique value. SQL: " + sql;
                //                //MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        stringError = "The UNIQUESPID must not be null.";
                //        _logger.Error(stringError);
                //        throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                //    }
                //}
                //#endregion

                #region Check for Unique OFFICE/STREETLIGHTNUMBER/FIXTURECODE combination
                {

                    //Get values
                    string officeVlu = GetValue(pObject as IRow, OFFICE_FldName);
                    string streetLightVlu = GetValue(pObject as IRow, STREETLIGHTNUMBER_FldName);
                    string fixtureVlu = GetValue(pObject as IRow, FIXTURECODE_FldName);
                    string combinedVlu = officeVlu + streetLightVlu + fixtureVlu;

                    if (!string.IsNullOrEmpty(combinedVlu))
                    {

                        //Build sql
                        sql = null;
                        if (string.IsNullOrEmpty(officeVlu))
                            sql = OFFICE_FldName + " IS NULL AND ";
                        else
                            sql = OFFICE_FldName + "= '" + officeVlu + "' AND ";

                        if (string.IsNullOrEmpty(streetLightVlu))
                            sql = sql + STREETLIGHTNUMBER_FldName + " IS NULL AND ";
                        else
                            sql = sql + STREETLIGHTNUMBER_FldName + "= '" + streetLightVlu + "' AND ";

                        if (string.IsNullOrEmpty(fixtureVlu))
                            sql = sql + FIXTURECODE_FldName + " IS NULL";
                        else
                            sql = sql + FIXTURECODE_FldName + "= '" + fixtureVlu + "'";

                        //Checks OFFICE/STREETLIGHTNUMBER/FIXTURECODE combination value already exists                        
                        IQueryFilter pFilter = new QueryFilterClass();
                        pFilter.WhereClause = sql;
                        pFilter.SubFields = OFFICE_FldName + "," + STREETLIGHTNUMBER_FldName + "," + FIXTURECODE_FldName;
                        int rowCount = tableStreetLight.RowCount(pFilter);

                        //Prompt user for OFFICE/STREETLIGHTNUMBER/FIXTURECODE Violation
                        if (rowCount > 1)
                        {
                            stringErrorTitle = "OFFICE/STREETLIGHTNUMBER/FIXTURECODE Violation";
                            stringError = "The OFFICE/STREETLIGHTNUMBER/FIXTURECODE combination must be a unique value. SQL: " + sql;
                            //MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                        }
                    }
                    else
                    {
                        stringError = "The OFFICE/STREETLIGHTNUMBER/FIXTURECODE combination must not be null.";
                        _logger.Error(stringError);
                        throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }
                #endregion
            }
        }

        #endregion Overridden BaseSpecialAU Methods

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
