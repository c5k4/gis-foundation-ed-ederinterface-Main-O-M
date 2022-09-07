using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;

using ESRI.ArcGIS.esriSystem;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP.RowTransformers
{
    class OpenPointRowTransformer : SAPDeviceRowTransformer
    {
        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        //IVARA
        public override bool IsValid(IRow sourceRow, out string errorMsg)
        {
            bool retValue = true;
            errorMsg = "";
            retValue = (IsNetworkFeature(sourceRow) == false);
            if (retValue == false)
            {
                errorMsg = "Not a Network Feature";
                GISSAPIntegrator._logger.Info(sourceRow.OID + " A Network Feature ");
                GISSAPIntegrator._logger.Error(sourceRow.OID + " A Network Feature ");
            }
                return retValue; 
            //return (IsNetworkFeature(sourceRow) == false);
        }

        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        public override bool IsValid(DeleteFeat sourceRow, out string errorMsg)
        {
            bool retValue = true;
            errorMsg = "";
            retValue = (IsNetworkFeature(sourceRow) == false);
            if (retValue == false)
            {
                errorMsg = "Not a Network Feature";
                GISSAPIntegrator._logger.Info(sourceRow.OID + " A Network Feature ");
                GISSAPIntegrator._logger.Error(sourceRow.OID + " A Network Feature ");
            }
                return retValue;
            //return (IsNetworkFeature(sourceRow) == false);
        }
    }
}
