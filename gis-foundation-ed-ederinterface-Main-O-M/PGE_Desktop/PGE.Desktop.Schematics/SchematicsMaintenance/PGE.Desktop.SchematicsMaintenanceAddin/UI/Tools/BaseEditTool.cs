// Copyright 2013 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/index.html#/Copyright_information/00010000009s000000/
// 

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SchematicControls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.SystemUI;
using PGE.Desktop.SchematicsMaintenance;
using ESRI.ArcGIS.Framework;
using System.Windows.Input;
using PGE.Desktop.SchematicsMaintenance.UI.DockableWindows;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;
using PGE.Desktop.SchematicsMaintenance.EditTools.ViewModels;
using System.Linq;
using PGE.Desktop.SchematicsMaintenance.Enums;
using PGE.Desktop.SchematicsMaintenance.Core;

namespace PGE.Desktop.SchematicsMaintenance.UI.Tools
{
    /// <summary>
    /// Base button used for editing schematic features that are already 
    /// in an schematic edit session.
    /// </summary>
    public abstract class BaseEditTool : BaseTool
    {
        #region Constants

        #endregion

        #region Members

        private IDockableWindow _dockableWindow;

        ///// <summary>
        ///// Indicates if an error has been displayed for OnUpdate failure.
        ///// </summary>
        //private bool _errorHasBeenDisplayed = false;

        /// <summary>
        /// Target of the extension.
        /// </summary>
        protected ISchematicTarget _schematicTarget = null;

        /// <summary>
        /// Schematic extension reference.
        /// </summary>
        protected SchematicExtension _schematicExtension = null;

        /// <summary>
        /// The view model associated with the tool.
        /// </summary>
        protected IEditToolViewModel _viewModel = null;

        #endregion

        #region Constructors / Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseEditTool(
            string toolName,
            string toolId,
            string toolDockWindowUid = null): base(toolName, toolId, toolDockWindowUid)
        {
            InitializeDockable();
        }

        #endregion

        #region Properties


        /// <summary>
        /// The associated view model of the tool.
        /// </summary>
        protected IEditToolViewModel ViewModel
        {
            get
            {
                if (this._viewModel == null)
                {
                    if (PGESchematicsMaintenanceExtension.EditorToolsViewModel != null)
                    {
                        this._viewModel = 
                            PGESchematicsMaintenanceExtension.EditorToolsViewModel.EditToolViewModels.
                            FirstOrDefault(p => p.ToolType == this.ToolType);
                    }
                }
                return this._viewModel;
            }
        }

        /// <summary>
        /// Gets the tool type associated with the button.
        /// </summary>
        protected abstract EditToolType ToolType
        {
            get;
        }


        #endregion

        #region Methods

        

        /// <summary>
        /// Initializes the dockable reference by querying
        /// the application for the associated UID.
        /// </summary>
        private void InitializeDockable()
        {
            try
            {
                UID editToolsDockableID = new UIDClass
                {
                    Value = ThisAddIn.IDs.PGE_Desktop_SchematicsMaintenance_UI_DockableWindows_EditToolsDockableWindow
                };

                _dockableWindow =
                    ArcMap.DockableWindowManager.GetDockableWindow(editToolsDockableID);
            }
            catch (Exception)
            {
                // Intentionally ignore
            }
        }

        /// <summary>
        /// Toggles the dockable window and initializes the UI 
        /// to correspond to the associated button that was clicked.
        /// </summary>
        protected override void OnActivate()
        {
            try
            {
                base.OnActivate();

                if (_dockableWindow == null)
                {
                    InitializeDockable();
                }

                if (_dockableWindow != null)
                {
                    // Retrieve if the dockable window is visible
                    bool isVisible = _dockableWindow.IsVisible();

                    if (!isVisible)
                    {
                        // Show the dockable window
                        _dockableWindow.Show(!isVisible);
                    }

                    if (this.ViewModel != null)
                    {
                        // Change the tab to the specific tool which was clicked
                        var isCurrent = PGESchematicsMaintenanceExtension.EditorToolsViewModel.SetCurrentTool(this.ViewModel);
                        if (!isCurrent)
                        {
                            Logger.Log.Warn("The Schematic Tool Editor Dockable window current tool did not switch to the current tool [" + this.ViewModel.Title + "]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and eat exception so it doesn't crash application
                Logger.Log.Error(ex.Message, ex);
            }
        }

        // TODO: Optimize updates since they occur often
        /// <summary>
        /// Overrides the update behavior of the button, to initialize
        /// class-level members to ensure that the extension is defined
        /// and schematic target meets the expected state.
        /// </summary>
        protected override void OnUpdate()
        {
            try
            {
                base.OnUpdate();

                // Verify the extension is found and on
                if (_schematicExtension == null)
                {
                    _schematicExtension = Utils.GetSchematicExtension();
                }
                if (_schematicExtension == null)
                {
                    // If the extension's not found, disable the tool
                    Enabled = false;
                }

                if (this.ViewModel != null)
                {
                    // Call the view model associated with the button,
                    // to determine if the system is in the correct state for 
                    // enabling the button
                    Enabled = this.ViewModel.IsEnabled;
                }
            }
            catch (Exception ex)
            {
                // Log and eat exception so it doesn't crash application
                Logger.Log.Error(ex.Message, ex);
            }
        }

        #endregion

    }
}
