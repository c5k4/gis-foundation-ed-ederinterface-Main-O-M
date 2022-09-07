using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CreateConduitAnno
{
    public class btnCreateConduitAnno : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public btnCreateConduitAnno()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;
            frmCreateConduitAnno dialog = new frmCreateConduitAnno();
            CreateConduitAnnoTool this_tool = new CreateConduitAnnoTool(dialog);
            this_tool.Run();

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
