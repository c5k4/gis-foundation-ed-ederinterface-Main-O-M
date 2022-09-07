using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("31F3DEA4-4B12-4177-B359-0F74915097BF")]
    [ProgId("PGE.Desktop.EDER.UpdateLIDARConflatedDate_AU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class UpdateLIDARConflatedDate_AU : BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateDeleteLiDARCorrectionAU"/> class.  
        /// </summary>
        public UpdateLIDARConflatedDate_AU() : base("PGE Update LIDARConflatedDate AU") { }
        #endregion Constructors

        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string sFClassName_SupportStructure = "EDGIS.SupportStructure";
        private static string CurrentSourcAccrucy = string.Empty;
        private static string PreSourcAccrucy = string.Empty;
        private static int Count = 0;
        private static int preobjectId = 0;
        private static int currentobjectId = 0;


        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onUpdate event
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {

            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate) || (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate))
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;

        }

        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            //AU will run only on FeatureUpdate
            string sourceAccuracyVal = string.Empty;
            IField sourceAccuracyFld = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.SourceAccuracy);
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                sourceAccuracyVal = pObject.get_Value(pObject.Fields.FindField(sourceAccuracyFld.Name)).ToString();
                if (sourceAccuracyVal.Equals("37") || sourceAccuracyVal.Equals("31"))
                {
                    pObject.set_Value(pObject.Fields.FindField("LIDARConflatedDate"), DateTime.Now);

                }
            }

            else if (eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                IFeature feat = pObject as IFeature;
                IFeatureChanges featureChange = feat as IFeatureChanges;
                //If feature shape changes then re-create the LiDAR Corrections feature
                sourceAccuracyVal = pObject.get_Value(pObject.Fields.FindField(sourceAccuracyFld.Name)).ToString();
                if (featureChange.ShapeChanged)
                {
                    if (sourceAccuracyVal.Equals("37") || sourceAccuracyVal.Equals("31"))
                    {
                        //If support sytructure is moved then source accuracy will be set as Unknown : 999
                        pObject.set_Value(pObject.Fields.FindField(sourceAccuracyFld.Name), "999");
                    }
                }
                else
                {


                    currentobjectId = pObject.OID;

                    if (Count == 0)
                    {
                        PreSourcAccrucy = GetSourceAccValue(pObject);
                        CurrentSourcAccrucy = sourceAccuracyVal;
                        //preobjectId = currentobjectId;
                        Count = Count + 1;
                    }
                    else
                    {
                        //preobjectId = currentobjectId;
                        if (preobjectId != currentobjectId)
                        {
                            PreSourcAccrucy = GetSourceAccValue(pObject);
                            CurrentSourcAccrucy = sourceAccuracyVal;
                            //preobjectId = currentobjectId;
                            Count = Count + 1;
                        }
                        else
                        {
                            Count = Count + 1;

                            CurrentSourcAccrucy = sourceAccuracyVal;
                            //preobjectId = currentobjectId;
                        }
                    }
                    if (!PreSourcAccrucy.Equals(CurrentSourcAccrucy))
                    {
                        if (sourceAccuracyVal.Equals("37") || sourceAccuracyVal.Equals("31"))
                        {
                            pObject.set_Value(pObject.Fields.FindField("LIDARConflatedDate"), DateTime.Now);

                        }
                    }

                    preobjectId = currentobjectId;
                    //currentobjectId = pObject.OID;

                    PreSourcAccrucy = pObject.get_Value(pObject.Fields.FindField(sourceAccuracyFld.Name)).ToString();
                }
            }
        }

        #endregion

        //This method is added in this class. Added new method code is given below:-
        /// <summary>
        /// Get sourceAccuracy field value from default version 
        /// </summary>
        /// <param name="pObj">The selected object .</param>
        /// <returns> string </returns>
        private string GetSourceAccValue(IObject pObj)
        {

            string strgetSourceAccValue = string.Empty;
            IWorkspace pFWorkspace = (IWorkspace)((IDataset)((IRow)pObj).Table).Workspace;
            IFeatureClass pSupportFeatureclass = default(IFeatureClass);
            IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)pFWorkspace;
            IFeatureWorkspace pFeatureFWorkspace = (IFeatureWorkspace)versionedWorkspace.DefaultVersion;
            IFeatureCursor pCursor = default(IFeatureCursor);
            IFeature pfet = default(IFeature);
            QueryFilter pQFilter = null;
            string sCircuitID = string.Empty;
            try
            {
                if (pFWorkspace != null)
                {
                    pSupportFeatureclass = pFeatureFWorkspace.OpenFeatureClass(sFClassName_SupportStructure);
                    if (pSupportFeatureclass == null)
                        throw new Exception("Failed to load table " + pSupportFeatureclass);
                    pQFilter = new QueryFilterClass();
                    pQFilter.WhereClause = string.Format("OBJECTID={0}", pObj.OID);
                    pQFilter.SubFields = "SOURCEACCURACY";
                    pCursor = pSupportFeatureclass.Search(pQFilter, true);
                    if ((pfet = pCursor.NextFeature()) == null)
                        return strgetSourceAccValue;
                    strgetSourceAccValue = pfet.get_Value(pfet.Fields.FindField("SOURCEACCURACY")).ToString();
                }
            }
            catch (Exception exp)
            {
            }
            return strgetSourceAccValue;
        }

}
}
