using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICommand = System.Windows.Input.ICommand;
using PGE.Desktop.SchematicsMaintenance.Common.Extensions;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    /// <summary>
    /// Basic map tool that supports Tool activation command, snapping, and bounds checking.
    /// </summary>
    public abstract class BaseTool : ESRI.ArcGIS.Desktop.AddIns.Tool, IToolCommand
    {
        #region Fields

        private ICommand setCommandItemActivatedCommand;
        private readonly SnappingSupport snappingSupport;
        private readonly IDockableWindow toolDockWindow;

        #endregion

        #region Properties

        protected string ToolId { get; private set; }
        protected string ToolName { get; private set; }

        /// <summary>
        /// Represents the map location after snapping and bounds checking. 
        /// This value is null if the map location is out of bounds of the coordinate system.
        /// </summary>
        public IPoint MapPoint { get; protected set; }

        /// <summary>
        /// Identifies when a spatial reference is Unknown.
        /// </summary>
        public bool IsValidSpatialReference { get; protected set; }

        #region Snapping Support

        protected SnappingSupport SnappingSupport
        {
            get
            {
                return this.snappingSupport;
            }
        }

        #endregion

        #endregion

        protected BaseTool(
            string toolName,
            string toolId,
            string toolDockWindowUid = null)
        {
            this.ToolName = toolName;
            //this.Log = log;
            this.ToolId = toolId;

            if (toolDockWindowUid != null)
            {
                this.toolDockWindow =
                    ArcMap.DockableWindowManager.GetDockableWindow(
                        toolDockWindowUid.ToUID());
            }

            this.snappingSupport = new SnappingSupport();
        }

        #region Implementation of IToolCommand

        /// <summary>
        /// Command which activates the tool. Can be used to activate this tool from outside classes.
        /// </summary>
        public ICommand SetCommandItemActivatedCommand
        {
            get
            {
                return this.setCommandItemActivatedCommand ??
                       (this.setCommandItemActivatedCommand =
                        new RelayCommand(
                            param =>
                            ToolSupport.SetCommandItemActivated(
                                ArcMap.Application, this.ToolId, true)));
            }
        }

        #endregion

        #region Overrides of Tool

        protected override void OnUpdate()
        {
            base.OnUpdate();

            ISpatialReference sref = ArcMap.Document.FocusMap.SpatialReference;

            bool disabled = ArcMap.Application == null ||
                            ArcMap.Document.ActiveView is IPageLayout ||
                            sref == null || sref is IUnknownCoordinateSystem;

            Enabled = !disabled;
        }

        protected override void OnActivate()
        {
            try
            {
                base.OnActivate();

                Cursor = Cursors.Cross;

                this.SetToolDockWindowVisibility(true);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                Logger.Log.Error(ex);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);

                this.SnappingSupport.Uninitalize();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }
        }

        protected override bool OnDeactivate()
        {
            bool result = base.OnDeactivate();

            try
            {
                Cursor = Cursors.Default;
                this.SetSnappingPaused(false);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }

            return result;
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            try
            {
                base.OnMouseDown(arg);

                IPoint mapPoint =
                    ArcMap.ThisApplication.Display.DisplayTransformation.
                        ToMapPoint(arg.X, arg.Y);

                bool isValidSpatialReference = this.IsValidSpatialReference;

                if (arg.Button == MouseButtons.Right ||
                    !mapPoint.IsWithinBounds(ref isValidSpatialReference))
                {
                    this.MapPoint = null;
                    IsValidSpatialReference = isValidSpatialReference;
                    return;
                }
                IsValidSpatialReference = isValidSpatialReference;
                this.MapPoint = this.SnappingSupport.Snap(mapPoint);

                this.SetToolDockWindowVisibility(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            try
            {
                base.OnMouseMove(arg);

                IPoint mapPoint =
                    ArcMap.Document.ActiveView.ScreenDisplay.DisplayTransformation.
                        ToMapPoint(arg.X, arg.Y);
                bool isValidSpatialReference = IsValidSpatialReference;
                if (!mapPoint.IsWithinBounds(ref isValidSpatialReference))
                {
                    Cursor = Cursors.No;
                    this.MapPoint = null;
                    IsValidSpatialReference = isValidSpatialReference;
                    return;
                }
                IsValidSpatialReference = isValidSpatialReference;
                Cursor = Cursors.Cross;

                this.MapPoint = this.SnappingSupport.Snap(mapPoint);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }
        }

        protected override void OnKeyDown(KeyEventArgs arg)
        {
            try
            {
                base.OnKeyDown(arg);
                if (this.SnappingSupport.IsEnabled)
                {
                    if (arg.KeyCode == Keys.Space)
                    {
                        this.SetSnappingPaused(true);
                        return;
                    }
                }

                if (arg.KeyCode == Keys.Escape)
                {
                    this.SetCommandItemActivatedCommand.Execute(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }
        }

        protected override void OnKeyUp(KeyEventArgs arg)
        {
            try
            {
                base.OnKeyUp(arg);
                if (this.SnappingSupport.IsEnabled)
                {
                    if (arg.KeyCode == Keys.Space)
                    {
                        this.SetSnappingPaused(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                throw;
            }
        }

        #endregion

        #region Support Methods

        protected void SetToolDockWindowVisibility(bool visibility)
        {
            if (this.toolDockWindow != null &&
                this.toolDockWindow.IsVisible() != visibility)
            {
                this.toolDockWindow.Show(visibility);
            }
        }

        private void SetSnappingPaused(bool paused)
        {
            this.SnappingSupport.Paused = paused;
            ArcMap.Application.StatusBar.Message[0] = paused
                ? @"Snapping is paused. Release the spacebar to resume snapping."
                : string.Empty;
        }

        #endregion

    }
}

