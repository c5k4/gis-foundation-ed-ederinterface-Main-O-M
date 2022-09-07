using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System.Windows.Forms;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Attribute
{
    [Guid("e6a027b6-2800-469e-8bf4-60e2a50d6ff3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Attribute.UpdateDPDRecloserCCRatingAU")]
    [ComponentCategory(ComCategory.AttrAutoUpdateStrategy)]
    public class UpdateDPDRecloserAttributes:BaseAttributeAU
    {
        
        #region StaticMembers
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        const string strDPDRecloserLookupTableName = "EDGIS.PGE_DPD_RECLOSER_LOOKUP";
        #endregion
        #region Constructor
        public UpdateDPDRecloserAttributes() : base("PG&E Update DPD Recloser CCRating & Max Interrupting Current", "", esriFieldType.esriFieldTypeString) { }        
        #endregion
        #region Base Attribute AU Override
        protected override object InternalExecute(IObject pObj)
        {            
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)pObj).Table).Workspace;
            ITable DPDRecloserLookupTable = null;
            IRow codeRow = null;
            QueryFilter queryFilter = null;
            string strDeviceType=string.Empty;
            int iSubTypeCd = 0;
            double dblCCRatingValue=0;
            double dblMaxInterruptCurrent=0;
            String strIntrruptingMedium = string.Empty;
            object returnObject=null;
            strDeviceType = pObj.GetFieldValue("DEVICETYPE", false, "").ToString();
            iSubTypeCd = Convert.ToInt32(pObj.GetFieldValue("SUBTYPECD", false, ""));
            try
            {
                //strDeviceType = pObj.GetFieldValue("DEVICETYPE",false,"").ToString();
                DPDRecloserLookupTable = new MMTableUtilsClass().OpenTable(strDPDRecloserLookupTableName, featureWorkspace);
                if (DPDRecloserLookupTable == null)
                    throw new Exception("Failed to load table " + strDPDRecloserLookupTableName);
                queryFilter=new QueryFilterClass();
                queryFilter.WhereClause=string.Format("CODE='{0}'",strDeviceType); 
                codeRow = new Extensions.CursorEnumerator(() => DPDRecloserLookupTable.Search(queryFilter, false)).FirstOrDefault();
                if(codeRow==null) throw new Exception("CCRating and Maximum interrupting Current is not configured in "+strDPDRecloserLookupTableName+" table");
                
                if (iSubTypeCd == 3)
                {
                    dblCCRatingValue = Convert.ToDouble((codeRow.get_Value(codeRow.Fields.FindField("CCRATING"))));
                    dblMaxInterruptCurrent = Convert.ToDouble((codeRow.get_Value(codeRow.Fields.FindField("MAXINTERRUPTINGCURRENT"))));
                    strIntrruptingMedium = (codeRow.get_Value(codeRow.Fields.FindField("INTERRUPTINGMEDIUM"))).ToString();

                    FieldInstance fldCCRating = FieldInstance.FromFieldName(pObj as IRow, "CCRATING");
                    FieldInstance fldMaxInterruptCurrent = FieldInstance.FromFieldName(pObj as IRow, "MAXINTERRUPTINGCURRENT");
                    FieldInstance fldIntrruptingMedium = FieldInstance.FromFieldName(pObj as IRow, "INTERRUPTINGMEDIUM");

                    fldCCRating.Value = dblCCRatingValue;
                    fldMaxInterruptCurrent.Value = dblMaxInterruptCurrent;
                    fldIntrruptingMedium.Value = strIntrruptingMedium;
                }

                returnObject = strDeviceType;
                return strDeviceType;                
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E Update DPD Recloser CCRating , Max Interrupting Current & Interruptinc medium autoupdator ", ex);
            }            
            finally
            {
                //release all objects goes here
                ReleaseAllTheThings(codeRow,queryFilter);
            }
            return strDeviceType;
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
