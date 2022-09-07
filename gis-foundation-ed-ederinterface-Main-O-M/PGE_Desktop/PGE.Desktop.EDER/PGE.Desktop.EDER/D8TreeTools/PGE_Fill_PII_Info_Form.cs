using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using System.Collections;
using ESRI.ArcGIS.ArcMapUI;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PGE.Desktop.EDER.D8TreeTools
{
    public partial class PGE_Fill_PII_Info_Form : Form
    {
        IObject CustAgrrementObj = null;
        bool dt_valuechanged = false;
        bool AgreementDateTextBox_changed = false;
        bool textChanged_OWNERNAME = false;
        bool textChanged_OWNERSTREETADDRESS = false;
        bool textChanged_OWNERSTREETADDRESS2 = false;
        bool textChanged_OWNERSTATE = false;
        bool textChanged_OWNERPHONE = false;
        bool textChanged_POLNUMBER = false;
        bool textChanged_AGREEMENTDATE = false;
        bool textChanged_PREMISEID = false;
        bool textChanged_OWNERCITY = false;
        bool textChanged_OWNERZIP = false;

        public PGE_Fill_PII_Info_Form(IObject pObj)
        {
            InitializeComponent();

            this.EnableEditingCheckBox.Checked = false;

            this.message.Visible = false;
            CustAgrrementObj = pObj;
            dt_valuechanged = false;
            AgreementDate.Format = DateTimePickerFormat.Custom;
            AgreementDate.CustomFormat = " ";
            this.Subtype.Text = "For " + pObj.Class.AliasName.ToString() + " Subtype : Privately Owned Lines";
            //this.RelatedTo.Text = "Related To : " +pObj.Class.AliasName.ToString() + " Object ID " + pObj.OID.ToString();

            IField PGE_OWNERNAME = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERNAME);
            IField PGE_OWNERSTREETADDRESS = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTREETADDRESS);
            IField PGE_OWNERSTREETADDRESS2 = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTREETADDRESS2);
            IField PGE_OWNERSTATE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTATE);
            IField PGE_OWNERCITY = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERCITY);
            IField PGE_OWNERPHONE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERPHONE);
            IField PGE_OWNERZIP = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERZIP);
            IField PGE_AGREEMENTDATE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_AGREEMENTDATE);
            IField PGE_PREMISEID = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_PREMISEID);
            IField PGE_POLNUMBER = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_POLNUMBER);

            object PGE_OWNERNAME_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERNAME.Name));
            object PGE_OWNERSTATE_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTATE.Name));
            object PGE_OWNERCITY_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERCITY.Name));
            object PGE_OWNERPHONE_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERPHONE.Name));
            object PGE_OWNERSTREETADDRESS_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS.Name));
            object PGE_OWNERSTREETADDRESS2_obj  = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS2.Name));
            object PGE_OWNERZIP_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERZIP.Name));
            object PGE_AGREEMENTDATE_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_AGREEMENTDATE.Name));
            object PGE_PREMISEID_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_PREMISEID.Name));
            object PGE_POLNUMBER_obj = CustAgrrementObj.get_Value(CustAgrrementObj.Class.Fields.FindField(PGE_POLNUMBER.Name));

            if (PGE_OWNERNAME_obj != null)
            {
                this.OwnerName.Text = PGE_OWNERNAME_obj.ToString();
            }
            if (PGE_OWNERSTATE_obj != null)
            {
                this.OwnerState.Text = PGE_OWNERSTATE_obj.ToString();
            }
            if (PGE_OWNERCITY_obj != null)
            {
                this.OwnerCity.Text = PGE_OWNERCITY_obj.ToString();
            }
            if (PGE_OWNERPHONE_obj != null)
            {
                this.OwnerPhone.Text = PGE_OWNERPHONE_obj.ToString();
            }
            if (PGE_OWNERSTREETADDRESS_obj != null)
            {
                this.OwnerStreetAddress.Text = PGE_OWNERSTREETADDRESS_obj.ToString();
            }
            if (PGE_OWNERSTREETADDRESS2_obj != null)
            {
                this.OwnerStreetAddress2.Text = PGE_OWNERSTREETADDRESS2_obj.ToString();
            }
            if (PGE_OWNERZIP_obj != null)
            {
                this.OwnerZip.Text = PGE_OWNERZIP_obj.ToString();
            }
            if (PGE_AGREEMENTDATE_obj != null)
            {
                //this.AgreementDate.Text = PGE_AGREEMENTDATE_obj.ToString();
                //if (this.AgreementDate.Equals(""))
                //{
                    AgreementDate.CustomFormat = " ";
                //}
                string date = PGE_AGREEMENTDATE_obj.ToString();
                //this.AgreementDateTextBox.Text = date;
                if(!string.IsNullOrEmpty(date)) { this.AgreementDateTextBox.Text = date.Remove(date.Length - 12);}
            }
            if (PGE_PREMISEID_obj != null)
            {
                this.PremiseID.Text = PGE_PREMISEID_obj.ToString();
            }
            if (PGE_POLNUMBER_obj != null)
            {
                this.PrivatelyOwnedLineNo.Text = PGE_POLNUMBER_obj.ToString();
            }

            this.EnableEditingCheckBox.Checked = false;
            this.Save.Enabled = false;
            this.Clear.Enabled = false;
            this.OwnerName.Enabled = false;
            this.OwnerStreetAddress.Enabled = false;
            this.OwnerStreetAddress2.Enabled = false;
            this.OwnerState.Enabled = false;
            this.OwnerCity.Enabled = false;
            this.OwnerPhone.Enabled = false;
            this.OwnerZip.Enabled = false;
            this.AgreementDate.Enabled = false;
            this.PremiseID.Enabled = false;
            this.PrivatelyOwnedLineNo.Enabled = false;
            this.AgreementDateTextBox.Enabled = false;
            //this.OwnerName.DeselectAll();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            bool dataSave = false;
            try
            {
                IField PGE_OWNERNAME = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERNAME);
                IField PGE_OWNERSTREETADDRESS = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTREETADDRESS);
                IField PGE_OWNERSTREETADDRESS2 = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTREETADDRESS2);
                IField PGE_OWNERSTATE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERSTATE);
                IField PGE_OWNERCITY = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERCITY);
                IField PGE_OWNERPHONE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERPHONE);
                IField PGE_OWNERZIP = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_OWNERZIP);
                IField PGE_AGREEMENTDATE = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_AGREEMENTDATE);
                IField PGE_PREMISEID = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_PREMISEID);
                IField PGE_POLNUMBER = ModelNameFacade.FieldFromModelName(CustAgrrementObj.Class, SchemaInfo.Electric.FieldModelNames.PGE_POLNUMBER);

                if (EnableEditingCheckBox.Checked || dt_valuechanged)
                {
                    if (textChanged_OWNERNAME) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERNAME.Name), this.OwnerName.Text.ToString()); }
                    if (textChanged_OWNERSTATE) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTATE.Name), this.OwnerState.Text.ToString()); }
                    if (textChanged_OWNERCITY) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERCITY.Name), this.OwnerCity.Text.ToString()); }
                    if (textChanged_OWNERPHONE) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERPHONE.Name), this.OwnerPhone.Text.ToString()); }
                    if (textChanged_OWNERSTREETADDRESS) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS.Name), this.OwnerStreetAddress.Text.ToString()); }
                    if (textChanged_OWNERSTREETADDRESS2) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS2.Name), this.OwnerStreetAddress2.Text.ToString()); }
                    if (textChanged_OWNERZIP) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERZIP.Name), this.OwnerZip.Text.ToString()); }
                    //if ((textChanged_AGREEMENTDATE) && dt_valuechanged && this.AgreementDate.Text.ToString() != " ") { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_AGREEMENTDATE.Name), this.AgreementDate.Text.ToString()); }

                    if (AgreementDateTextBox_changed) 
                    {
                        string dateSet = this.AgreementDateTextBox.Text.ToString();

                        
                            if (string.IsNullOrEmpty(dateSet))
                            {
                                CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_AGREEMENTDATE.Name), DBNull.Value);

                            }
                            else
                            {
                                CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_AGREEMENTDATE.Name), dateSet);
                            }
                        
                    
                    }
                    if (textChanged_PREMISEID) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_PREMISEID.Name), this.PremiseID.Text.ToString()); }
                    if (textChanged_POLNUMBER) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_POLNUMBER.Name), this.PrivatelyOwnedLineNo.Text.ToString()); }

                    //if (!String.IsNullOrEmpty(this.OwnerName.Text.ToString()) || textChanged_OWNERNAME) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERNAME.Name), this.OwnerName.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerState.Text.ToString()) || textChanged_OWNERSTATE) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTATE.Name), this.OwnerState.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerCity.Text.ToString()) || textChanged_OWNERCITY) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERCITY.Name), this.OwnerCity.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerPhone.Text.ToString()) || textChanged_OWNERPHONE) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERPHONE.Name), this.OwnerPhone.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerStreetAddress.Text.ToString()) || textChanged_OWNERSTREETADDRESS) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS.Name), this.OwnerStreetAddress.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerStreetAddress2.Text.ToString()) || textChanged_OWNERSTREETADDRESS2) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERSTREETADDRESS2.Name), this.OwnerStreetAddress2.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.OwnerZip.Text.ToString()) || textChanged_OWNERZIP) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_OWNERZIP.Name), this.OwnerZip.Text.ToString()); }
                    //if ((!String.IsNullOrEmpty(this.AgreementDate.Text.ToString()) || textChanged_AGREEMENTDATE) && dt_valuechanged && this.AgreementDate.Text.ToString() != " ") { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_AGREEMENTDATE.Name), this.AgreementDate.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.PremiseID.Text.ToString()) || textChanged_PREMISEID) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_PREMISEID.Name), this.PremiseID.Text.ToString()); }
                    //if (!String.IsNullOrEmpty(this.PrivatelyOwnedLineNo.Text.ToString()) || textChanged_POLNUMBER) { CustAgrrementObj.set_Value(CustAgrrementObj.Class.Fields.FindField(PGE_POLNUMBER.Name), this.PrivatelyOwnedLineNo.Text.ToString()); }
                
                    dataSave = true;
                    //if (String.IsNullOrEmpty(this.OwnerName.Text.ToString()) && String.IsNullOrEmpty(this.OwnerState.Text.ToString()) && String.IsNullOrEmpty(this.OwnerCity.Text.ToString()) && String.IsNullOrEmpty(this.OwnerPhone.Text.ToString()) && String.IsNullOrEmpty(this.OwnerStreetAddress.Text.ToString()) && String.IsNullOrEmpty(this.OwnerStreetAddress2.Text.ToString()) && String.IsNullOrEmpty(this.OwnerZip.Text.ToString()) && (!dt_valuechanged) && String.IsNullOrEmpty(this.PremiseID.Text.ToString()) && String.IsNullOrEmpty(this.PrivatelyOwnedLineNo.Text.ToString()))
                    //{
                    //    dataSave = false;
                    //}
                    CustAgrrementObj.Store();

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            if (dataSave)
            {
                //this.message.Visible = true;
                //this.message.Text = "Data Save Successfully";

                RefreshLayer();
                MessageBox.Show("Data Save Successfully");
                this.DialogResult = DialogResult.OK;
            }
            else { }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            this.OwnerName.Text = null;
            this.OwnerState.Text = null;
            this.OwnerCity.Text = null;
            
            this.OwnerPhone.Text = null;
            this.OwnerStreetAddress.Text = null;
            this.OwnerStreetAddress2.Text = null;
            this.OwnerZip.Text = null;
            this.AgreementDateTextBox.Text = null;
            AgreementDate.CustomFormat = " ";
            //this.AgreementDate.Value = System.DateTime.Now.Date;
            dt_valuechanged = true;
            AgreementDateTextBox_changed = false;
            this.PremiseID.Text = null;
            this.PrivatelyOwnedLineNo.Text = null;
            this.message.Visible = false;
            this.Save.Enabled = false;
            this.DialogResult = DialogResult.None;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Dispose();
            //this.DestroyHandle();
            this.DialogResult = DialogResult.Cancel;
        }

        private void AgreementDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime cDate = AgreementDate.Value;
            AgreementDate.Format = DateTimePickerFormat.Custom;
            AgreementDate.CustomFormat = "MM/dd/yyyy";
            textChanged_AGREEMENTDATE = true;
            dt_valuechanged = true;
            this.Save.Enabled = true;
            string date = cDate.Date.ToString();
            AgreementDateTextBox.Text = date.Remove(date.Length - 12);

        }

        private void RefreshLayer()
        {

            ESRI.ArcGIS.Carto.IMap pMAP = (ESRI.ArcGIS.Carto.IMap)((ESRI.ArcGIS.ArcMapUI.IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;

            ESRI.ArcGIS.Carto.ISelectionEvents pSelectionEvents;
            pSelectionEvents = pMAP as ESRI.ArcGIS.Carto.ISelectionEvents;

            //pSelectionEvents.SelectionChanged();

            ESRI.ArcGIS.ArcMapUI.IMxDocument pMxDoc = (ESRI.ArcGIS.ArcMapUI.IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document;

          
            pMxDoc.UpdateContents();
            pMxDoc.ActiveView.Refresh();

            ESRI.ArcGIS.ArcMapUI.ITableWindow pTableWindow2 = null;
            pTableWindow2 = new ESRI.ArcGIS.ArcMapUI.TableWindowClass();
            
            
            try
            {
               ESRI.ArcGIS.ArcMapUI.ITableWindow custAgreementTable = pTableWindow2.FindViaTable(CustAgrrementObj.Table as ITable, true);
               custAgreementTable.Refresh();

            }
            catch (Exception ex) { }
           
           
        }

        private void OwnerName_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERNAME = true;
            this.Save.Enabled = true;
        }

        private void OwnerStreetAddress_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERSTREETADDRESS = true;
            this.Save.Enabled = true;
        }

        private void OwnerStreetAddress2_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERSTREETADDRESS2 = true;
            this.Save.Enabled = true;
        }

        private void OwnerCity_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERCITY = true;
            this.Save.Enabled = true;
        }

        private void OwnerState_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERSTATE = true;
            this.Save.Enabled = true;
        }

        private void OwnerZip_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERZIP = true;
            this.Save.Enabled = true;
        }

        private void OwnerPhone_TextChanged(object sender, EventArgs e)
        {
            textChanged_OWNERPHONE = true;
            this.Save.Enabled = true;
        }

        private void PrivatelyOwnedLineNo_TextChanged(object sender, EventArgs e)
        {
            textChanged_POLNUMBER = true;
            this.Save.Enabled = true;
        }

        private void PremiseID_TextChanged(object sender, EventArgs e)
        {
            textChanged_PREMISEID = true;
            this.Save.Enabled = true;
        }

        private void EnableEditingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.EnableEditingCheckBox.Checked)
            {
                //this.EnableEditingCheckBox.Checked = true;
                //this.Save.Enabled = true;
                this.OwnerName.Enabled = true;
                this.Clear.Enabled = true;
                this.OwnerStreetAddress.Enabled = true;
                this.OwnerStreetAddress2.Enabled = true;
                this.OwnerState.Enabled = true;
                this.OwnerCity.Enabled = true;
                this.OwnerPhone.Enabled = true;
                this.OwnerZip.Enabled = true;
                this.AgreementDate.Enabled = true;
                this.PremiseID.Enabled = true;
                this.PrivatelyOwnedLineNo.Enabled = true;
                this.AgreementDateTextBox.Enabled = true;
            }
            else
            {
                this.EnableEditingCheckBox.Checked = false;
                this.Save.Enabled = false;
                this.Clear.Enabled = false;
                this.OwnerName.Enabled = false;
                this.OwnerStreetAddress.Enabled = false;
                this.OwnerStreetAddress2.Enabled = false;
                this.OwnerState.Enabled = false;
                this.OwnerCity.Enabled = false;
                this.OwnerPhone.Enabled = false;
                this.OwnerZip.Enabled = false;
                this.AgreementDate.Enabled = false;
                this.PremiseID.Enabled = false;
                this.PrivatelyOwnedLineNo.Enabled = false;
                this.AgreementDateTextBox.Enabled = false;
            }
        }

        private void AgreementDateTextBox_TextChanged(object sender, EventArgs e)
        {
            bool dateFormat = false;
            AgreementDateTextBox_changed = true;
            this.Save.Enabled = true;
            if (!string.IsNullOrEmpty(AgreementDateTextBox.Text))
            {
                
                DateTime dt;
                dateFormat = DateTime.TryParseExact(AgreementDateTextBox.Text, "M/d/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
                if (!dateFormat)
                {
                    //MessageBox.Show("Wrong Agreement Date formt");
                    this.message.Visible = true;
                    this.message.Text = "Wrong Agreement Date format";
                    this.message.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

                    this.message.ForeColor = System.Drawing.Color.Red;
                    this.Save.Enabled = false;
                    this.DialogResult = DialogResult.None;
                }
                else
                {
                    this.message.Visible = false;
                    this.message.Text = null;
                    this.Save.Enabled = true;
                }
            }
            else
            {
                this.message.Visible = false;
                this.message.Text = null;
                this.Save.Enabled = true;
            }

        }

        private void PGE_Fill_PII_Info_Form_Load(object sender, EventArgs e)
        {
            this.Subtype.Focus();
            
            
        }

       


    }
}
