/*
 *PreLoad info
* insert into sde.telvent_validation_severitymap values (sde.gdb_util.next_rowid('SDE', 'telvent_validation_severitymap'),'PGE Validate Orphan Unit Records - Error',0);
*commit;
*
*Check box in ArcFM Properties - Object Info - Rules ...
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;


namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("CAB8034E-A82B-4E4E-881F-A503425DAAB2")]
    [ProgId("PGE.Desktop.EDER.ValidateRelatedUnitRecordsError")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateOrphansError : BaseValidationRule
    {

        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.CircuitSource, SchemaInfo.Electric.ClassModelNames.ServicePoint, SchemaInfo.Electric.ClassModelNames.TransformerUnit};

        private static string _msgOrphanUnits = "{0} feature must participate in one composite relationship as a child.";
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// 
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateOrphansError()
            : base("PGE Validate Orphan Unit Records - Error", _enabledModelNames)
        {
        }
        #endregion Constructors

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {


                esriDifferenceType[] pDiffTypes = new esriDifferenceType[6];
                pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
                pDiffTypes[1] = esriDifferenceType.esriDifferenceTypeUpdateDelete;
                pDiffTypes[2] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
                pDiffTypes[3] = esriDifferenceType.esriDifferenceTypeUpdateUpdate;
                pDiffTypes[4] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
                pDiffTypes[5] = esriDifferenceType.esriDifferenceTypeDeleteUpdate;

                System.Collections.Hashtable h = ValidationEngine.Instance.
                    GetVersionDifferences(
                        (IVersionedWorkspace)Miner.Geodatabase.Edit.Editor.EditWorkspace,
                        pDiffTypes, false);

                IFeatureClass fc;
                IFeature feat;
                bool exist = false;

                if (h.Count > 0)
                {
                    for (int idx = 0; idx < h.Count; idx++)
                    {
                        if (h[idx] is VersionDiffObject)
                        {
                            VersionDiffObject vdo = h[idx] as VersionDiffObject;
                            // Only running this rule on insert only.
                            //if ((vdo.DifferenceType == esriDifferenceType.esriDifferenceTypeInsert) || (vdo.DifferenceType == esriDifferenceType.esriDifferenceTypeUpdateDelete) 
                            //    || (vdo.DifferenceType == esriDifferenceType.esriDifferenceTypeUpdateUpdate))
                            //{
                            if ((vdo.OID == row.OID) && (((ESRI.ArcGIS.Geodatabase.IDataset)row.Table).BrowseName.ToUpper() == vdo.DatasetName.ToUpper()))
                            {
                                string x = vdo.DatasetName;
                                if (x.Contains("TRANSFORMERUNIT"))
                                { 
                                    object one = row.Value[row.Fields.FindField("TRANSFORMERGUID")];
                                    if (one != null)
                                    {
                                        if (one.ToString().Length < 4)
                                        {
                                            AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                        else
                                        {
                                            // change made, check if parent exist
                                            IWorkspace workspace = (row.Table as IDataset).Workspace;
                                            exist = FeatureExist("EDGIS.TRANSFORMER", one.ToString(), workspace);
                                            if (!exist)
                                                AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                    }
                                    else
                                    {
                                        AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                    }
                                    
                                }
                                else if (x.Contains("SERVICEPOINT"))
                                {
                                    object one = row.Value[row.Fields.FindField("SERVICELOCATIONGUID")];
                                    if (one != null)
                                    {
                                        if (one.ToString().Length < 4)
                                        {
                                            AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                        else
                                        {
                                            // change made, check if parent exist
                                            IWorkspace workspace = (row.Table as IDataset).Workspace;
                                            exist = FeatureExist("EDGIS.SERVICELOCATION", one.ToString(), workspace);
                                            if (!exist)
                                                AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                    }
                                    else
                                    {
                                        AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                    }
                                }
                                else if (x.Contains("CIRCUITSOURCE"))
                                {
                                    object one = row.Value[row.Fields.FindField("DEVICEGUID")];
                                    if (one != null)
                                    {
                                        if (one.ToString().Length < 4)
                                        {
                                            AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                        else
                                        {
                                            // change made, check if parent exist
                                            IWorkspace workspace = (row.Table as IDataset).Workspace;
                                            exist = FeatureExist("EDGIS.ELECTRICSTITCHPOINT", one.ToString(), workspace);
                                            if (!exist)
                                                AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                        }
                                    }
                                    else
                                    {
                                        AddError(string.Format(_msgOrphanUnits, x + " (OID: " + row.OID + ") "));
                                    }
                                }
                            }
                           // }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Orhan Records Error rule.", ex);
            }
            return base.InternalIsValid(row);
        }
        #endregion

        private bool FeatureExist(string fcModelName, string guid, IWorkspace workspace)
        {
            bool result = false;
            IFeatureClass fc = GetFeatureClass(fcModelName, workspace);
            IQueryFilter qf = null;
            IFeatureCursor cursor = null;

            try
            {
                if (fc != null)
                {
                    qf= new QueryFilterClass();
                    qf.WhereClause = "GlobalID = '" + guid + "'";
                    cursor = fc.Search(qf, false);
                    IFeature feat = cursor.NextFeature();
                    if (feat != null)
                        result = true;
                }


                return result;
            }
            catch(System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
            }
        }
        private IFeatureClass GetFeatureClass(string featureClassName, IWorkspace workspace)
        {
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            IFeatureClass featClass = featWorkspace.OpenFeatureClass(featureClassName);
            if (featClass == null) { throw new Exception("Unable to find feature class " + featureClassName); }
            return featClass;
        }

        private List<string> getFieldsChanges()
        {
            List<string> result = new List<string>();
            try
            {
                IMMVersioningUtils versionUtils = new MMVersioningUtilsClass();
                
                return result;
            }
            catch (System.Exception ex)
            {
                return result;
            }

        }
    }
}
