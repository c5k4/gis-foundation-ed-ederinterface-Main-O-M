using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework.FeederManager;

namespace PGE.Desktop.EDER.AutoUpdaters.Attribute
{
    [Guid("CCEFFBC4-295F-4180-88AA-682B29C9CC8D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Attribute.UpdateCircuitName")]
    [ComponentCategory(ComCategory.AttrAutoUpdateStrategy)]
    public class UpdateCircuitName : BaseAttributeAU
    {
        #region StaticMembers
        
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string sFClassName_CircuitSource = "EDGIS.CircuitSource";
        
        #endregion
        
        #region Constructor

        public UpdateCircuitName() : base("PG&E Update CircuitName in Feature", "", esriFieldType.esriFieldTypeString) { }        
        
        #endregion
        
        #region Base Attribute AU Override
        
        protected override object InternalExecute(IObject pObj)
        {            
            IFeatureWorkspace pFWorkspace = (IFeatureWorkspace)((IDataset)((IRow)pObj).Table).Workspace;
            ITable pTable_CircuitSource = default(ITable);
            ICursor pCursor = default(ICursor);
            IRow pRow = default(IRow);
            QueryFilter pQFilter = null;
            string sCircuitID=string.Empty;
            try
            {
                string[] sCircuitIDs = FeederManager2.GetCircuitIDs((IRow)pObj);
                if (sCircuitIDs.Length == 0) return null;
                sCircuitID = sCircuitIDs[0];
                //if (String.IsNullOrEmpty(sCircuitID = Convert.ToString(pObj.get_Value(pObj.Fields.FindField("CIRCUITID"))))) return null;
                //strDeviceType = pObj.GetFieldValue("DEVICETYPE",false,"").ToString();
                pTable_CircuitSource = new MMTableUtilsClass().OpenTable(sFClassName_CircuitSource, pFWorkspace);

                if (pTable_CircuitSource == null)
                    throw new Exception("Failed to load table " + sFClassName_CircuitSource);
                pQFilter=new QueryFilterClass();
                pQFilter.WhereClause = string.Format("CIRCUITID='{0}'", sCircuitID);
                pQFilter.SubFields = "SUBSTATIONNAME";
                pCursor= pTable_CircuitSource.Search(pQFilter, true);
                if ((pRow = pCursor.NextRow()) == null)
                    return null;
                return pRow.get_Value(pRow.Fields.FindField("SUBSTATIONNAME")) + " " + sCircuitID.Substring(sCircuitID.Length - 4);
                
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E Update CircuitName in Feature ", ex);
                return null;
            }            
            finally
            {
                //release all objects goes here
                ReleaseAllTheThings(pQFilter, pCursor, pRow);
            }
        }
        #endregion
        #region Helper Classes
        void ReleaseAllTheThings(params object[] comObjects)
        {
            foreach (var item in comObjects)
            {
                if (item is IDisposable)
                    ((IDisposable)item).Dispose();
                else if (item != null && Marshal.IsComObject(item))
                    while (Marshal.ReleaseComObject(item) > 0) ;
            }
        }
        #endregion

    }
}
