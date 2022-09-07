// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Schematics Work Queue Manager Form to perform schematics Editing
// TCS V3SF (EDGISREARC-375) 22/11/2021                             Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGE.Desktop.SchematicsMaintenance
{
    /// <summary>
    /// Stop Editing Menu Selection
    /// </summary>
    public partial class WorkQueueStopMenu : Form
    {
        /// <summary>
        /// Return Value of the Form/Dialog
        /// </summary>
        public WQStopEditMenu returnValue = default;

        /// <summary>
        /// Initialize Components
        /// </summary>
        public WorkQueueStopMenu()
        {
            InitializeComponent();
            returnValue = WQStopEditMenu.Cancel;
        }

        /// <summary>
        /// Cancel the Stop Editing Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkQueueStopMenu_Load(object sender, EventArgs e)
        {
            this.returnValue = WQStopEditMenu.Cancel;
        }

        /// <summary>
        /// Save and Submit Edits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SaveNSubmit_Click(object sender, EventArgs e)
        {
            this.returnValue = WQStopEditMenu.SaveNSubmit;
            this.Close();
        }

        /// <summary>
        /// Stop Editing but don't save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StopNoSave_Click(object sender, EventArgs e)
        {
            this.returnValue = WQStopEditMenu.StopNoSave;
            this.Close();
        }

        /// <summary>
        /// Stop Editing but save Edits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StopSave_Click(object sender, EventArgs e)
        {
            this.returnValue = WQStopEditMenu.StopSave;
            this.Close();
        }

        /// <summary>
        /// Cancel the Stop Editing Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.returnValue = WQStopEditMenu.Cancel;
            this.Close();
        }
    }

    /// <summary>
    /// Enum to define Stop Editing Operation
    /// </summary>
    public enum WQStopEditMenu
    {
        SaveNSubmit,
        StopNoSave,
        StopSave,
        Cancel
    }
}
