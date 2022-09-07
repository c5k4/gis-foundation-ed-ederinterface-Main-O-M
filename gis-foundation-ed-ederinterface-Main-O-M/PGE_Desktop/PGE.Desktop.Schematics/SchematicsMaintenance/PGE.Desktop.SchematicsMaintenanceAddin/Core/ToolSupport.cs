using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    /// <summary>
    /// Static support class for ArcMap Tool classes. 
    /// Includes helper methods for programmatic tool activation/deactivation and 
    /// more robust map input (e.g. bounds checking).
    /// </summary>
    public static class ToolSupport
    {
        public static void SetCommandItemActivated(
            IApplication application,
            string toolID,
            bool enabled)
        {
            try
            {

                if (application == null)
                {
                    throw new ArgumentNullException("application");
                }
                if (toolID == null)
                {
                    throw new ArgumentNullException("toolID");
                }

                ICommandItem commandItem =
                    application.Document.CommandBars.Find(toolID);

                if (commandItem != null && commandItem.Command != null &&
                    commandItem.Command.Enabled)
                {
                    application.CurrentTool = enabled ? commandItem : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }

        public static void ZoomToLayerExtent(IActiveView activeView, ILayer layer)
        {
            try
            {
                if (activeView == null || layer == null)
                {
                    return;
                }
                // Refresh and Zoom to the layer
                activeView.Extent = layer.AreaOfInterest;
                activeView.Refresh();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }
}
