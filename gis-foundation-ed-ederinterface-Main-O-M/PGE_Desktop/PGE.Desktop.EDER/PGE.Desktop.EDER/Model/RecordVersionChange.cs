using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.Model
{
    public static class RecordVersionChange
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// This method records the deleted object's information into either the PGE_VERSIONDELETEPOINT 
        /// or PGE_VERSIONDELETELINE table depending on what type of feature the deleted feature is. It creates
        /// a new row and populates all fields, including shape. If it cannot find the circuitID, it does
        /// not create a row.
        /// </summary>
        /// <param name="versionName">The name of the version that the object was deleted in</param>
        /// <param name="changed">The IFeature that was deleted</param>
        public static void RecordDelete(IFeature changed)
        {
            IDataset objClassToStore;
            IFeatureClass changedFc = changed.Class as IFeatureClass;
            IRow newRow;
            string modelName=string.Empty, featGuid, versionName;
            //Depending on the type of feature that we are working with, we can choose which table to save delete
            //information into
            //Bug#13758
            if (changed.FeatureType == esriFeatureType.esriFTSimpleJunction ||
                changed.FeatureType == esriFeatureType.esriFTSimple ||
                changed.FeatureType == esriFeatureType.esriFTComplexJunction ||
                changed.FeatureType == esriFeatureType.esriFTAnnotation)
            {
                modelName = SchemaInfo.General.ClassModelNames.VERSIONDELETESPOINT;
            }
            else if (changed.FeatureType == esriFeatureType.esriFTComplexEdge ||
                changed.FeatureType == esriFeatureType.esriFTSimpleEdge ||
                changed.FeatureType == esriFeatureType.esriFTDimension)
            {
                modelName = SchemaInfo.General.ClassModelNames.VERSIONDELETESLINE;
            }
            else
            {
                _logger.Warn("The feature (OID:" + changed.OID + ") is not valid for RecordVersionChange");
                return;
            }
           //else throw new Miner.CancelEditException("Not a valid feature for the Schematics change detection AU");


            featGuid = changed.get_Value(changed.Fields.FindField(SchemaInfo.Electric.GlobalID)).ToString();
            versionName = ((IVersion)((IDataset)changed.Class).Workspace).VersionName;

            //Retrieves the object class we would like to save into
            objClassToStore = ModelNameFacade.ObjectClassByModelName(((IDataset)changed.Class).Workspace, modelName) as IDataset;

            /*
                * ***********************
                * Create, populate, and store the new row of data into the feature class identified previously by
                * model name.
                * ***********************
                */

            newRow = ((ITable)objClassToStore).CreateRow();

            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.FeatureGUID), featGuid);
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.VersionName), versionName);
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.DateDeleted), DateTime.Now);

            //Dummy columns, to be removed soon
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.CircuitID), "123456789");
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.InstallJobNumber), "ABCD");
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.FeatureClassID), "1234");
            newRow.set_Value(newRow.Fields.FindField(SchemaInfo.Electric.Shape), changed.Shape);

            newRow.Store();
            
        }

        /// <summary>
        /// Updates the VersionName field of the object class to reference the version that the create/update
        /// took place
        /// </summary>
        /// <param name="versionName">The name of the version we are in. This will be written into the
        /// VersionName attribute</param>
        /// <param name="changed">The created/modified feature</param>
        public static void RecordInsertUpdate(IFeature changed)
        {
            string versionName = ((IVersion)((IDataset)changed.Class).Workspace).VersionName;
            changed.set_Value(changed.Fields.FindField(SchemaInfo.Electric.VersionName), versionName);
        }
    }
}
