using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Common.Delivery.ArcFM
{
    //This is working but requires the PGE.Common.IREA.Framework and PGE.Common.Delivery.Framework dlls placed in the ArcGIS bin folder which is not good. 
    //Should write some piece of code that would load the dlls from different folder so that Custom Config Sections can be read even if the dll is not the ArcMap.exe folder.
    //or the dlls to be GAC'ed
    /// <summary>
    /// 
    /// </summary>
    public class OracleSequenceFacilityIDGenerator:IFacilityIDGenerator
    {
        #region IFacilityIDGenerator Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetFacilityID(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            FacilityIDConfigSection section = (FacilityIDConfigSection)config.GetSection("facilityiddefinitions");
            IDataset ds = (IDataset)obj.Class;
            FacilityIDDefinition facIdDefinition = section.FacilityIDDefinition[ds.Name.ToLower()];
            IFeatureWorkspace fws = (IFeatureWorkspace)ds.Workspace;
            IQueryDef querydef = fws.CreateQueryDef();
            querydef.SubFields = facIdDefinition.SequenceName+".NextVal";
            querydef.Tables = "SYS.DUAL";
            ICursor cursor = null;
            try
            {
                cursor = querydef.Evaluate();
                IRow row = cursor.NextRow();
                if (row != null)
                {
                    return facIdDefinition.Prefix + (string)row.get_Value(0);
                }
            }
            catch (Exception ex)
            {
                throw new COMException("Failed getting facility id  - /n" + ex.StackTrace, (int)mmErrorCodes.MM_E_CANCELEDIT); ;
            }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }; 
                }
            }
            return string.Empty;
        }

        #endregion
    }
}
