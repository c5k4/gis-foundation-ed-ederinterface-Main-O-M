using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

// Changes for ENOS to SAP migration - Code changes for ENOS to SAP migration
namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    /// A relationship AU process GWOC record if generation info record get unrelated from service point.
    /// </summary>
    [ComVisible(true)]
    [Guid("4A28976E-22C1-00A4-E053-0AF4574D489D")]
    [ProgId("PGE.Desktop.EDER.PGEProcessGWOCRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class PGEManageGWOCRelAU : BaseRelationshipAU
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _gwocTableName = "EDGIS.GWOC";
        private static string _FieldNameservicepointid = "SERVICEPOINTID";
        private static string _FieldNamegenGUID = "GENERATIONGUID";
        private static string _FieldNamesapeginotification = "SAPEGINOTIFICATION";
        private static string _FieldNamedatecreated = "DATECREATED";
        private static string _FieldNamemeternumber = "METERNUMBER";
        private static string _FieldNamestreetname1 = "STREETNAME1";
        private static string _FieldNamestreetname2 = "STREETNAME2";
        private static string _FieldNamecity = "CITY";
        private static string _FieldNamestate = "STATE";
        private static string _FieldNamezip = "ZIP";
        private static string _FieldNamestreetnumber = "STREETNUMBER";
        private static string _FieldNameresolved = "RESOLVED";
        private static string _FieldNamecomments = "COMMENTS";
        private static string _FieldNamelocalofficeid = "LOCALOFFICEID";
        private static string _FieldNamecgc12 = "CGC12";        
        private static string _FieldNameglobalid = "GLOBALID";

        private static string _FieldValueTest = "TST";    
        private static string _FieldValueNo = "N";
        private static string _FieldValueNewGWOC = "This is a new GWOC record.";  
        

        #endregion private

        #region Constructors
        /// <summary>
        /// constructor for the Populate Conductor Phase Rel AU
        /// </summary>
        public PGEManageGWOCRelAU()
            : base("PGE Manage GWOC On Unrelating Generation From Service Point.")
        {
        }

        #endregion Constructors

        #region Relationship AU overrides
        /// <summary>
        /// create a GWOC record if generation info record get unrelated from service point.
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {

            if (eEvent != mmEditEvent.mmEventRelationshipDeleted)
                return;

            //Get parent service point
            IObject origObject = relationship.OriginObject;
            IObject destObject = relationship.DestinationObject;

            //check for not null
            if (origObject == null || destObject == null)
            {
                //log the warning and return the control 
                _logger.Warn("Either OriginObject or DestinationObject is <Null>, exiting.");
                return;
            }

            if (ModelNameFacade.ContainsClassModelName(relationship.OriginObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint }) &&
                ModelNameFacade.ContainsClassModelName(relationship.DestinationObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo }))
            {
                //If the relatenship between Service point and generationinfo is broken then insert a record in GWOC   
                if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
                ProcessGWOCRecordOnGenSPRelDeleted(origObject, destObject);

                if (eEvent == mmEditEvent.mmEventRelationshipCreated)
                    ProcessGWOCRecordOnGenSPRelCreated(origObject, destObject);
            }            
        }

        /// <summary>
        /// Whenever a generationinfo will be unrelated from service point the below function will maintain GWOC record
        /// </summary>
        /// <param name="argOrigObject"></param>
        /// <param name="argDestObject"></param>
        private void ProcessGWOCRecordOnGenSPRelDeleted(IObject argOrigObject, IObject argDestObject)
        {
            IQueryFilter pQf = null;
            IQueryFilter pQf1 = null;
            try
            {
                IWorkspace pWS = ((IDataset)argOrigObject.Class).Workspace;
                IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;

                ITable pTable = pFWS.OpenTable(_gwocTableName);

                pQf = new QueryFilterClass();
                pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid))) + "' AND " + _FieldNamecomments + "='" + _FieldValueTest + "'";

                if (pTable.RowCount(pQf) > 0)
                {
                    pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid))) + "'";
                    pTable.DeleteSearchedRows(pQf);
                }
                else
                {
                    pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid))) + "' AND " + _FieldNameresolved + "='" + _FieldValueNo + "'";

                    // before creating GWOC check first that the record exist or not
                    if (pTable.RowCount(pQf) > 0)
                        return;

                    //Creating a GWOC record 

                    string servicePointID = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNameservicepointid)));
                    string genGUID = Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid)));
                    string sapegiNotification = Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNamesapeginotification)));

                    string meterNumber = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamemeternumber)));
                    string streetName1 = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamestreetname1)));
                    string streetName2 = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamestreetname2)));

                    string city = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamecity)));
                    string state = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamestate)));
                    string zip = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamezip)));

                    //Keep status as per for new GWOC record
                    string resolved = _FieldValueNo;
                    string streetNumber = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamestreetnumber)));
                    string localofficeid = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamelocalofficeid)));
                    string cgc12 = Convert.ToString(argOrigObject.get_Value(argOrigObject.Fields.FindField(_FieldNamecgc12)));
                    string comments = _FieldValueNewGWOC;

                    IRow pRow = pTable.CreateRow();

                    pRow.set_Value(pRow.Fields.FindField(_FieldNamegenGUID), genGUID);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamesapeginotification), sapegiNotification);

                    pRow.set_Value(pRow.Fields.FindField(_FieldNameservicepointid), servicePointID);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamemeternumber), meterNumber);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamestreetname1), streetName1);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamestreetname2), streetName2);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamecity), city);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamestate), state);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamezip), zip);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamestreetnumber), streetNumber);

                    pRow.set_Value(pRow.Fields.FindField(_FieldNamedatecreated), DateTime.Now);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNameresolved), resolved);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamelocalofficeid), localofficeid);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamecgc12), cgc12);
                    pRow.set_Value(pRow.Fields.FindField(_FieldNamecomments), comments);

                    //Check no. of service points from same address if count==2 then update Comment as 
                    //Multiple records exists for Service Points for same address combination (Streetnumber + Street name1 + City)
                    pQf1 = new QueryFilterClass();
                    pQf1.WhereClause = "UPPER(TRIM(streetnumber)) = '" + streetNumber.Trim().ToUpper() + "' AND UPPER(TRIM(streetName1))  = '" + streetName1.Trim().ToUpper() + "' AND UPPER(TRIM(city))  = '" + city.Trim().ToUpper() + "'";

                    if ((argOrigObject.Table).RowCount(pQf1) == 2)
                    {
                        comments = "Multiple records exist for Service Points for same address combination (Streetnumber + Street name1 + City)";
                        pRow.set_Value(pRow.Fields.FindField(_FieldNamecomments), comments);
                    }

                    pRow.Store();
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
            finally
            {
                if (pQf != null && Marshal.IsComObject(pQf))
                {
                    Marshal.FinalReleaseComObject(pQf);
                    pQf = null;
                }
                if (pQf1 != null && Marshal.IsComObject(pQf1))
                {
                    Marshal.FinalReleaseComObject(pQf1);
                    pQf1 = null;
                }
            }
        }


        /// <summary>
        /// Whenever a generationinfo will be related to a service point the below function will maintain GWOC record
        /// </summary>
        /// <param name="argOrigObject"></param>
        /// <param name="argDestObject"></param>
        private void ProcessGWOCRecordOnGenSPRelCreated(IObject argOrigObject, IObject argDestObject)
        {
            try
            {
                IWorkspace pWS = ((IDataset)argOrigObject.Class).Workspace;
                IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;

                ITable pTable = pFWS.OpenTable(_gwocTableName);

                IQueryFilter pQf = new QueryFilterClass();
                pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid))) + "'";

                if (pTable.RowCount(pQf) > 0)
                {
                    pQf.WhereClause = _FieldNamegenGUID + "='" + Convert.ToString(argDestObject.get_Value(argDestObject.Fields.FindField(_FieldNameglobalid))) + "'";
                    pTable.DeleteSearchedRows(pQf);
                }               
            }
            catch (Exception exp)
            {
                // throw;
            }
        }               

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint});
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo});
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ServicePoint, originClass.AliasName, originClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled && (eEvent == mmEditEvent.mmEventRelationshipDeleted));
        }

        #endregion Relationship AU overrides

    }
}
