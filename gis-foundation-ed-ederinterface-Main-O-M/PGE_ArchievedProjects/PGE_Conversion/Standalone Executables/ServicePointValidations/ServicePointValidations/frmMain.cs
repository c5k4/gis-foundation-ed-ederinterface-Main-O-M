using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace ServicePointValidations
{
    public partial class frmMain : Form
    {
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmLogIn login = new frmLogIn();
            //login.ShowDialog();
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //grbConnectionProperties.Visible = true;
            frmSpatialDatabaseConnection frmConnectionProperties = new frmSpatialDatabaseConnection();
            frmConnectionProperties.ShowDialog();
            if (clsTestWorkSpace.State) connectionToolStripMenuItem.Enabled = false;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //private void updateRatedMVASTransformerToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    frmClenupTasks objClenupTasks = null;
        //    try
        //    {
        //        objClenupTasks = new frmClenupTasks();
        //        objClenupTasks.updateComponents(77,"Update RatedMVA field Value for S_Transformer", "Please wait processing...");
        //        objClenupTasks.ShowDialog();
        //    }
        //    catch (Exception Ex)
        //    {
        //        MessageBox.Show(Ex.Message);
        //    }
        //    finally
        //    {
        //        objClenupTasks = null;
        //        Logs.close();
        //    }
        //}

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void NonGeoCoded_SP_Replacement_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmNonGeoCoded_SP_Replacement objForm = new frmNonGeoCoded_SP_Replacement();
            objForm.Show();
        }

        private void updConduitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmServicePnt_Input_preparation objForm = new frmServicePnt_Input_preparation();
            objForm.Show();

        }

        private void updateAnnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ServiceLocation_Check objForm = new ServiceLocation_Check();
            objForm.Show();
        }

        private void GeoCoading_Validations_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GeoCoading_Validations objForm = new GeoCoading_Validations();
            objForm.Show();
        }

        private void FindVersionChange_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmFindVersionChanges objForm = new frmFindVersionChanges();
            objForm.Show();
        }

        private void UpdateVersionChanges_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmUpdateVersionChanges objForm = new frmUpdateVersionChanges();
            objForm.Show();
        }

        private void CleanupCase1_2_Reports_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmCleanupCase1_2_Reports objForm = new frmCleanupCase1_2_Reports();
            objForm.Show();
        }

        private void Case1_2_InputX_updation_sq_Click(object sender, EventArgs e)
        {
            frmCase1_InputX_updation_sql objForm = new frmCase1_InputX_updation_sql();
            objForm.Show();
        }

        //private void copyFromToAttToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmCopyFromTo objForm = new frmCopyFromTo();
        //    objForm.Show();
        //}

        

        //private void generateConversionIDsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmGenerateConvIDs objForm = new frmGenerateConvIDs();
        //    objForm.Show();
        //}

        //private void checkSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    FrmCheckSequence objForm = new FrmCheckSequence();
        //    objForm.Show();
        //}

        //private void cheakDBStateToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmValidateConvIDS objForm = new frmValidateConvIDS();
        //    objForm.Show();
        //}

        //private void featureCountToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    FrmFeatureCount objForm = new FrmFeatureCount();
        //    objForm.Show();

        //}

        //private void streetLightDevisionToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmUpdDevision_streetlight objForm = new frmUpdDevision_streetlight();
        //    objForm.Show();
        //}

        //private void updConduitToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmUpdConduit objForm = new frmUpdConduit();
        //    objForm.Show();
        //}

        //private void updateAnnoToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmUpdate_Anno objForm = new frmUpdate_Anno();
        //    objForm.Show();
        //}

        //private void annoExpToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    Form1 objForm = new Form1();
        //    objForm.Show();
        //}

        //private void updBankcodeToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmTransformerUnitBankcode objForm = new frmTransformerUnitBankcode();
        //    objForm.Show();
        //}

        //private void updDivisionAllToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}

        //private void bRE6ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //if (clsTestWorkSpace.Workspace == null)
        //    //{
        //    //    MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //    return;
        //    //}

        //    frmBre6LogUpdate objForm = new frmBre6LogUpdate();
        //    objForm.Show();
        //}

        //private void bRE6LOGVALIDATEDOMAINToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //if (clsTestWorkSpace.Workspace == null)
        //    //{
        //    //    MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //    return;
        //    //}
        //    frmBRE6Validatecodes objForm = new frmBRE6Validatecodes();
        //    objForm.Show();
        //}

        //private void removeDuplicateConvIdsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmRemoveDuplicateConvIds objForm = new frmRemoveDuplicateConvIds();
        //    objForm.Show();

        //}

        //private void createInfoRecsForDupConvIdConductorsToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmCreateInfoRecords objForm = new frmCreateInfoRecords();
        //    objForm.Show();
        //}

        //private void createMissingInfoRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmCreateMissingInfoRecords objForm = new frmCreateMissingInfoRecords();
        //    objForm.Show();
        //}

        //private void createSpecialConditionRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    frmCreateSpecialConditions objForm = new frmCreateSpecialConditions();
        //    objForm.Show();
        //}

        //private void createMissingUnitRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmGenerateMissingUnitRecords objForm = new frmGenerateMissingUnitRecords();
        //    objForm.Show();
        //}

        //private void generateControllersToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    GenerateController objForm = new GenerateController();
        //    objForm.Show();
        //}

        //private void streetLightDevisionToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    EnableAnno objForm = new EnableAnno();
        //    objForm.Show();
        //}

        //private void updConduitToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmGenerateVoltageRegUnits objForm = new frmGenerateVoltageRegUnits();
        //    objForm.Show();
        //}

        //private void updateAnnoToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    GenerateController objForm = new GenerateController();
        //    objForm.Show();
        //}

        //private void ValidateNumOfPRI_Transformer_Click(object sender, EventArgs e)
        //{
        //    //if (clsTestWorkSpace.Workspace == null)
        //    //{
        //    //    MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //    return;
        //    //}
        //    //FrmValidateNumOfPRI_Transformer objForm = new FrmValidateNumOfPRI_Transformer();
        //    //objForm.Show();
        //}

        //private void Validate9999Elbow_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    FrmValElbowOperatingNum objForm = new FrmValElbowOperatingNum();
        //    objForm.Show();
        //}

        //private void ValidateTrans_Downstream_CondChk_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmTrans_DoenstreamPhaseChk objForm = new frmTrans_DoenstreamPhaseChk();
        //    objForm.Show();
        //}

        //private void RebuildDuctdefinition_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmRebuildDuctdefinition objForm = new frmRebuildDuctdefinition();
        //    objForm.Show();
        //}

        //private void updateLocalOfficeid_Click(object sender, EventArgs e)
        //{

        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    updateLocalOfficeid_device objForm = new updateLocalOfficeid_device();
        //    objForm.Show();
        //}

        //private void generateMissingInfoRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    GenerateMissingInfoRecords objForm = new GenerateMissingInfoRecords();
        //    objForm.Show();

        //}

        //private void frmupdLocalofficeid_servicelocation_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmupdLocalofficeid_servicelocation objForm = new frmupdLocalofficeid_servicelocation();
        //    objForm.Show();
        //}

        //private void updPolygonAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    updPolygonAttributes objForm = new updPolygonAttributes();
        //    objForm.Show();

        //}

        //private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}

        //private void toolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    if (clsTestWorkSpace.Workspace == null)
        //    {
        //        MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    frmUpdCircuitIDforDeviceGroup objForm = new frmUpdCircuitIDforDeviceGroup();
        //    objForm.Show();
        //}
    }
}