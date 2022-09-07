using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace PGE.BatchApplication.UDC_FAI_Tools
{
    public class FAIPhotoNameUpdater : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public FAIPhotoNameUpdater()
        {
        }

        protected override void OnClick()
        {
            FAIPhotoNameUpdaterForm cgc12UpdateForm = new FAIPhotoNameUpdaterForm();
            cgc12UpdateForm.ShowDialog(new ArcMapWindow(ArcMap.Application));
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}
