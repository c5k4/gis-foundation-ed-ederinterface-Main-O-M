using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;

namespace PGE.Desktop.EDER.FieldEditors
{
    public partial class CircuitIDSelectorForm : Form
    {
        private string _circuitID = "";
        private object _circuitDivision = null;
        private object _substationID = null;
        private object _feederID = null;
        private ICodedValueDomain CircuitDivisionDomain = null;
        private IFeatureClass SubstationFeatureClass = null;
        private Dictionary<string, object> SubstationNameIDMap = new Dictionary<string, object>();

        public CircuitIDSelectorForm(ICodedValueDomain circuitDivisionDomain, IFeatureClass substationFeatureClass)
        {
            InitializeComponent();

            txtFeederID.KeyUp += new KeyEventHandler(txtFeederID_KeyUp);

            CircuitDivisionDomain = circuitDivisionDomain;
            SubstationFeatureClass = substationFeatureClass;

            List<string> divisionList = new List<string>();
            for (int i = 0; i < CircuitDivisionDomain.CodeCount; i++)
            {
                string description = CircuitDivisionDomain.get_Name(i).ToString();
                divisionList.Add(description);                
            }
            divisionList.Sort();
            cboDivision.Items.AddRange(divisionList.ToArray());

            List<string> substationList = new List<string>();
            IQueryFilter qf = new QueryFilterClass();
            IField substationIDField = ModelNameManager.Instance.FieldFromModelName(substationFeatureClass, SchemaInfo.Electric.FieldModelNames.SubstationID);
            IField substationNameField = ModelNameManager.Instance.FieldFromModelName(substationFeatureClass, SchemaInfo.Electric.FieldModelNames.SubstationName);
            if (substationIDField == null)
            {
                MessageBox.Show("Unable to find the substation ID field on the " + ((IDataset)substationFeatureClass).BrowseName + " feature class");
            }
            else if (substationNameField == null)
            {
                MessageBox.Show("Unable to find the substation name field on the " + ((IDataset)substationFeatureClass).BrowseName + " feature class");
            }
            else
            {
                int subIDIdx = substationFeatureClass.FindField(substationIDField.Name);
                int subNameIdx = substationFeatureClass.FindField(substationNameField.Name);

                qf.AddField(substationIDField.Name);
                qf.AddField(substationNameField.Name);
                qf.WhereClause = substationNameField.Name + " is not null and " + substationIDField.Name + " is not null";
                IFeatureCursor substationFeatures = substationFeatureClass.Search(qf, false);
                IFeature substationFeature = null;
                while ((substationFeature = substationFeatures.NextFeature()) != null)
                {
                    object subID = substationFeature.get_Value(subIDIdx);
                    object subName = substationFeature.get_Value(subNameIdx);
                    if (!SubstationNameIDMap.ContainsKey(subName.ToString()))
                    {
                        SubstationNameIDMap.Add(subName.ToString(), subID);
                        substationList.Add(subName.ToString());
                    }

                }
                substationList.Sort();
                cboSubID.Items.AddRange(substationList.ToArray());
            }
        } 

        public string CircuitID
        {
            get
            {
                return _circuitID;
            }
            set
            {
                _circuitID = value;
            }
        }

        public object CircuitDivision
        {
            get
            {
                return _circuitDivision;
            }
            set
            {
                _circuitDivision = value;
            }
        }

        public object SubstationID
        {
            get
            {
                return _substationID;
            }
            set
            {
                _substationID = value;
            }
        }

        public object FeederID
        {
            get
            {
                return _feederID;
            }
            set
            {
                _feederID = value;
            }
        }

        void txtFeederID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                SaveSelection();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveSelection();
        }

        private void SaveSelection()
        {
            if (cboDivision.SelectedItem == null || string.IsNullOrEmpty(cboDivision.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Circuit Division");
                return;
            }
            if (cboSubID.SelectedItem == null || string.IsNullOrEmpty(cboSubID.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Substation ID");
                return;
            }
            if (txtFeederID.Text == null || string.IsNullOrEmpty(txtFeederID.Text))
            {
                MessageBox.Show("Please enter a valid Feeder ID");
                return;
            }

            string circuitDivisionSelected = "";
            string substationIDSelected = "";
            for (int i = 0; i < CircuitDivisionDomain.CodeCount; i++)
            {
                string description = CircuitDivisionDomain.get_Name(i).ToString();
                if (description == cboDivision.SelectedItem.ToString())
                {
                    circuitDivisionSelected = CircuitDivisionDomain.get_Value(i).ToString();
                    break;
                }
            }

            substationIDSelected = SubstationNameIDMap[cboSubID.SelectedItem.ToString()].ToString();

            _circuitID = circuitDivisionSelected + substationIDSelected + txtFeederID.Text;
            _substationID = substationIDSelected;
            _circuitDivision = circuitDivisionSelected;
            _feederID = txtFeederID.Text;

            if (_circuitID.Length > 9)
            {
                MessageBox.Show("The circuit ID (" + _circuitID + ") specified is too long: " + _circuitID);
                return;
            }

            while (_circuitID.Length < 9)
            {
                _circuitID = "0" + _circuitID;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }
    }
}
