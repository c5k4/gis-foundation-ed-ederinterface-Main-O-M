using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.GDBM;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.ValidationRules
{
    static class CommonValidate
    {

        /// <summary>
        /// Nuisance rules being exluded based on action (insert/update/delete).
        ///   - Only works when version check is used, not when map selection us used.
        ///   
        /// 
        /// </summary>
        /// <param name="ruleName"></param>
        /// <returns></returns>
        public static bool ExcludeRule(string ruleName, ESRI.ArcGIS.Geodatabase.esriDifferenceType esriDiff, ESRI.ArcGIS.Geodatabase.IRow row)
        {
            bool result = false;

            // Get version differences.
            System.Collections.Hashtable h = ValidationEngine.Instance.VersionDifferences;

            if ((h != null) && (h.Count > 0))
            {
                for (int idx = 0; idx < h.Count; idx++)
                {
                    if (h[idx] is VersionDiffObject)
                    {
                        VersionDiffObject vdo = h[idx] as VersionDiffObject;
                        // Only running this rule on insert only.
                        if (vdo.DifferenceType == esriDiff)
                        {
                            if ((vdo.OID == row.OID) && (((ESRI.ArcGIS.Geodatabase.IDataset)row.Table).BrowseName.ToUpper() == vdo.DatasetName.ToUpper()))
                            {
                                //   _logger.Debug("OperatingNumber Validation on " + esriDiff);
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// DETERMINE IF THE EDIT TYPE OF THE FEATURE BEING VALIDATED IS AN INSERT (CREATE A NEW FEATURE).
        /// </summary>
        /// <param name="row">THE FEATURE BEING PROCESSED BY THE VALIDATION RULE</param>
        /// <returns>A BOOLEAN INDICATING IF THE EDIT TYPE IS AN INSERT</returns>
        /// 

        public static bool isEditInsert(IRow row)
        {
            IWorkspace defWSpace = null;
            IFeatureWorkspace defFWSpce = null;

            try
            {
                int oid = (int)row.get_Value(row.Fields.FindField(row.Table.OIDFieldName));

                defWSpace = ValidationEngine.Instance.getWorkspace();                                   // GET THE DEFAULT (PARENT) WORKSPACE
                defFWSpce = (IFeatureWorkspace)defWSpace;

                IDataset defTableDSet = (IDataset)row.Table;
                string tableName = defTableDSet.Name;

                ITable defTable = defFWSpce.OpenTable(tableName);                                       // OPEN THE TABLE IN THE DEFAULT WORKSPACE

                IRow defRow = defTable.GetRow(oid);                                                     // TRY TO GET THE FEATURE IN THE DEFAULT WORKSPACE
                if (defRow != null) { return false; }                                                    // THE FEATURE IS NOT FOUND IN THE DEFAULT WORKSPACE IT WAS CREATED IN THE SESSION WORKSPACE

                // Logger.Info("ValidateOperatingNumberExists: Edit of " + tableName + " (OID = " + oid + ") is Not an Insert and Not Validated.");

                return true;                                                                           // IF THE FEATURE WAS FOUND, IT IS NOT AN INSERT - IT EXISTED IN THE PARENT VERSION
            }
            catch (COMException e)
            {
                if (e.ErrorCode == -2147219118)
                {
                    return true;                                                                        // THIS ERROR IS GENERATED WHEN THE FEATURE IS NOT FOUND SO IT INDICATES AN INSERT
                }
                else
                {
                    // Logger.Error("ValidateOperatingNumberExists: " + e.Message + " " + e.InnerException + e.StackTrace);
                    return true;
                }
            }
            finally
            {
                //if (defWSpace != null) { Marshal.ReleaseComObject(defWSpace); }
                if (defFWSpce != null) { Marshal.ReleaseComObject(defFWSpce); }
            }
        }
    }
}
