using PGE.Common.Delivery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    public partial class frmConfirmation : Form
    {
        private PONSHomeScreen pPONSHomeScreen;
        private SearchType pSearchType = 0;
        private string sStartOrEndDevice = string.Empty;

        // Added by SB
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private enum SearchType
        {
            StartEndDeviceSearch = 0,
            TransformerSearch = 1,
            SubstationCircuitSearch = 2,
            None = 3
        }

        public PONSHomeScreen HomeScreen
        {
            set { pPONSHomeScreen = value; }
        }

        public frmConfirmation()
        {
            InitializeComponent();
            //dgConfirmation.Rows.Add("transformer", "512095778252", "182611108");
            //dgConfirmation.Rows.Add("transformer", "512095778243", "182611109");
        }
        public frmConfirmation(string cgcNo, List<TransformerDetails> circuitidsdetails)
        {
            try
            {
                InitializeComponent();
                // Added by SB
                pSearchType = SearchType.TransformerSearch;
                //

                foreach (TransformerDetails cirid in circuitidsdetails)
                {
                    dgConfirmation.Rows.Add("transformer", cgcNo, cirid.CIRCUITID, cirid.OID);
                }
                this.dgConfirmation.ClearSelection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private IList<Equipment> EquipmentsCollection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEquipments"></param>
        /// Added by SB
        public frmConfirmation(IList<Equipment> pEquipments, string StartOrEndDevice)
        {
            try
            {
                InitializeComponent();
                EquipmentsCollection = pEquipments;
                sStartOrEndDevice = StartOrEndDevice;

                pSearchType = SearchType.StartEndDeviceSearch;
                foreach (Equipment pEquipment in pEquipments)
                    dgConfirmation.Rows.Add(pEquipment.ObjectClassDisplayName, pEquipment.ObjectDisplayName, pEquipment.CircuitID, pEquipment.ObjectID);

                this.dgConfirmation.ClearSelection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConfirmationOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgConfirmation.SelectedRows.Count < 1)
                {
                    MessageBox.Show("Select a record", "Selection Mandatory");
                    return;
                }

                switch (pSearchType)
                {
                    case (SearchType.TransformerSearch):
                        {
                            PGEGlobal.SELECTED_CIRCUITID = dgConfirmation.SelectedRows[0].Cells[2].Value.ToString();
                            break;
                        }
                    case (SearchType.StartEndDeviceSearch):
                        {
                            // Added by SB
                            if (this.dgConfirmation.SelectedRows.Count == 1)
                            {
                                int selectedRowIndex = this.dgConfirmation.SelectedRows[0].Index;
                                if (sStartOrEndDevice.Equals("START"))
                                {
                                    PGEGlobal.START_EQUIPMENT = EquipmentsCollection[selectedRowIndex];
                                    //PGEGlobal.SELECTED_START_OBJECT_CLASS_NAME = 
                                    //PGEGlobal.SELECTED_START_DEVICE_CIRCUITID = dgConfirmation.SelectedRows[0].Cells[2].Value.ToString();
                                    //PGEGlobal.SELECTED_START_OBJECT_CLASS_NAME = dgConfirmation.SelectedRows[0].Cells[0].Value.ToString();
                                    //PGEGlobal.SELECTED_START_DEVICE_CIRCUITID = dgConfirmation.SelectedRows[0].Cells[2].Value.ToString();
                                }
                                else if (sStartOrEndDevice.Equals("END"))
                                {
                                    PGEGlobal.END_EQUIPMENT = EquipmentsCollection[selectedRowIndex];
                                    //PGEGlobal.SELECTED_END_OBJECT_CLASS_NAME = dgConfirmation.SelectedRows[0].Cells[0].Value.ToString();
                                    //PGEGlobal.SELECTED_END_DEVICE_CIRCUITID = dgConfirmation.SelectedRows[0].Cells[2].Value.ToString();
                                }
                            }
                            break;
                        }
                }


                this.Close();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
