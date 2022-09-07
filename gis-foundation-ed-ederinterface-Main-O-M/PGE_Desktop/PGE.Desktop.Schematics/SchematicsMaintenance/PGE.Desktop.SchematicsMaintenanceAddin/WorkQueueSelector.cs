// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Schematics Work Queue Manager Button to perform schematics Editing
// TCS V3SF (EDGISREARC-375) 02/24/2021               Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicControls;
using ESRI.ArcGIS.SchematicUI;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.SchematicsMaintenance.Configuration;
using PGE.Desktop.SchematicsMaintenance.Core;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;
using ESRI.ArcGIS.ArcMapUI;

namespace PGE.Desktop.SchematicsMaintenance
{
    /// <summary>
    /// This class opens Work Queue Form for performing Schematic Start and Stop Operations
    /// </summary>
    class WorkQueueSelector : Button
    {
        #region Data members
        private SchematicExtension m_schematicExtension;
        private PGESchematicsMaintenanceExtension m_maintenanceExt;
        public static bool isOpen = false; //Check if WorkQueue Form is open or not
        private bool isEditing = false; //Check if Schematic Diagram is being Edited or not
        #endregion

        /// <summary>
        /// Constructor to initialize value of SchematicExtension as null
        /// </summary>
        public WorkQueueSelector()
        {
            m_schematicExtension = null;
        }

        /// <summary>
        /// Main function of WorkQueueSelector Class to open Work Queue Form
        /// </summary>
        protected override void OnClick()
        {
            IMap map = default(IMap);
            try
            {
                map = ArcMap.Document.FocusMap;

                //isOpen is bool value to check if Window in Already open or not
                if (!isOpen)
                {
                    WorkQueueSelectorWindow WorkQueueWindow = new WorkQueueSelectorWindow(map, m_schematicExtension, m_maintenanceExt, isEditing);
                    WorkQueueWindow.Show(ArcMap.Application as System.Windows.Forms.IWin32Window);
                }

                ArcMap.Application.CurrentTool = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.  The current map document may not be configured properly.");
                ArcMap.Application.StatusBar.Message[0] = "";
                Application.DoEvents();
                Logger.Log.Error(ex);
                return;
            }
        }

        /// <summary>
        /// Function to Enable or disable WorkQueue Button in ArcMap
        /// </summary>
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
            if (m_schematicExtension == null)
                m_schematicExtension = Utils.GetSchematicExtension();

            if (m_maintenanceExt == null)
                m_maintenanceExt = PGESchematicsMaintenanceExtension.GetExtension();

            if (m_schematicExtension == null)
            {
                Enabled = false;
                return;
            }

            if (m_maintenanceExt == null)
            {
                Enabled = false;
                return;
            }

            Enabled = (Utils.IsSchematicMap(ArcMap.Document.ActiveView.FocusMap, false));

            // Check if schematic layer is edited
            ISchematicTarget m_schematicTarget = null;
            m_schematicTarget = m_schematicExtension as ISchematicTarget;

            if (m_schematicTarget != null)
            {
                ISchematicLayer schLayer = m_schematicTarget.SchematicTarget;
                if (schLayer != null && schLayer.IsEditingSchematicDiagram())
                {
                    Enabled = true;
                    //Disable Start Editing Button
                    //Enable Start Editing Button
                    isEditing = true;
                    return;
                }
            }

            //Enable Start Edting Button
            //Disable Stop Editing Button
            isEditing = false;
        }

    }
}
