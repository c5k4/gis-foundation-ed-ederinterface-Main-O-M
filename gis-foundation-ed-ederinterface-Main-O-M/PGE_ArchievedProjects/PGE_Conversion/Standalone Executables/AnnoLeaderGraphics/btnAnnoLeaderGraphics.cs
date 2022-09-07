using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AnnoLeaderGraphics
{
    public class btnAnnoLeaderGraphics : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public btnAnnoLeaderGraphics()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;
            frmAnnoLeaderGraphics dialog = new frmAnnoLeaderGraphics();
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(dialog, 0);
            this_tool.Run();

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
