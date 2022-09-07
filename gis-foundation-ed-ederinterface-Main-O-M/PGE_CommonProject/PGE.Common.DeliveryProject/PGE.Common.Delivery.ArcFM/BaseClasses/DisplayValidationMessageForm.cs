using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// A form that allows user to edit Symbol number configuration documents stored in the database.
    /// </summary>
    public partial class DisplayValidationMessageForm : Form
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayValidationMessageForm"/> class.
        /// Uses model name facade to locate the symbol number rules table and finds the index of the config 
        /// field.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public DisplayValidationMessageForm(PGEError[] errorArray)
        {
            _errorArray = errorArray;
            InitializeComponent();
        }
        #endregion Constructor

        #region Private
        /// <summary>
        /// logger to log all the information, warning, and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        /// <summary>
        /// Index of the XML field.  
        /// </summary>
        PGEError[] _errorArray; 
                
        /// <summary>
        /// Handles the Load event of the DisplayValidationMessageForm control.
        /// 
        /// Clear the UI and load feature class names.  
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DisplayValidationMessageForm_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Debug("Loading list of errors");
                PopulateDataGrid(); 
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); 
            }
        }


        #endregion Private

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private DataGridViewColumn AppendNewColumn(
            string columnType,
            string fieldName)
        {
            try
            {
                DataGridViewColumn newColumn = null;
                switch (columnType)
                {
                    case "Text":
                        newColumn = new DataGridViewTextBoxColumn();
                        newColumn.HeaderText = fieldName;
                        break;
                    //case "DateTime":
                    //    newColumn = new GridViewDateTimeColumn();
                    //    //newColumn.FormatString = "{0:MM-dd-yyyy}";
                    //    newColumn.FormatString = "{dd-MM-yyyy hh:mm}";
                    //    newColumn.TextAlignment = ContentAlignment.MiddleRight;
                    //    newColumn.HeaderText = fieldName;
                    //    break;
                    case "Image":
                        newColumn = new DataGridViewImageColumn();
                        ((DataGridViewImageColumn)newColumn).ImageLayout = DataGridViewImageCellLayout.Normal; 
                        newColumn.Width = 16;
                        newColumn.HeaderText = fieldName;
                        break;
                    //case "CheckBox":
                    //    newColumn = new GridViewCheckBoxColumn();
                    //    newColumn.Width = 10;
                    //    newColumn.HeaderText = fieldName;
                    //    newColumn.ReadOnly = false;
                    //    break;
                }
                return newColumn;
            }
            catch (Exception ex)
            {
                _logger.Debug("An error has occurred in the AppendNewColumn routine " +
                    "Error Details : " + ex.Message);
                throw new Exception("Unable to append column to data grid");
            }
        }

        private void PopulateDataGrid()
        {
            try
            {
                //Clear the grid 
                grdErrors.Rows.Clear();
                grdErrors.Columns.Clear();

                //Setup the column headers
                DataGridViewColumn col = null;
                
                col = AppendNewColumn("Image", " ");
                grdErrors.Columns.Add(col);

                col = AppendNewColumn("Text", "Dataset");
                grdErrors.Columns.Add(col);

                col = AppendNewColumn("Text", "ObjectId");
                grdErrors.Columns.Add(col);

                col = AppendNewColumn("Text", "Error Message");
                grdErrors.Columns.Add(col);

                //Loop through all of the errors 
                for (int i = 0; i < _errorArray.Length; i++)
                {
                    PGEError pError = _errorArray[i];

                    //Create a new row in grid 
                    int rowIdx = grdErrors.Rows.Add();
                    DataGridViewRow dRowInfo = grdErrors.Rows[rowIdx];

                    //IMAGE 
                    dRowInfo.Cells[0].Value = imgErrorImages.Images[pError.Type];

                    //DATASET   
                    dRowInfo.Cells[1].Value = pError.Dataset;

                    //OBJECTID   
                    dRowInfo.Cells[2].Value = pError.ObjectId;

                    //ERROR MESSAGE   
                    dRowInfo.Cells[3].Value = pError.ErrorMsg;                     
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error populating grid: " + ex.Message);
            }
        }

    }
}
