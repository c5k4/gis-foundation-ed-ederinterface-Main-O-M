using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace PGEElecCleanup
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
        private void updateRatedMVASTransformerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClenupTasks objClenupTasks = null;
            try
            {
                objClenupTasks = new frmClenupTasks();
                objClenupTasks.updateComponents(77,"Update RatedMVA field Value for S_Transformer", "Please wait processing...");
                objClenupTasks.ShowDialog();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                objClenupTasks = null;
                Logs.close();
            }
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

        private void streetLightDevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            EnableAnno objForm = new EnableAnno();
            objForm.Show();
        }

        private void updConduitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmGenerateVoltageRegUnits objForm = new frmGenerateVoltageRegUnits();
            objForm.Show();
        }

        private void updateAnnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GenerateController objForm = new GenerateController();
            objForm.Show();
        }

        private void ValidateNumOfPRI_Transformer_Click(object sender, EventArgs e)
        {
            //if (clsTestWorkSpace.Workspace == null)
            //{
            //    MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //FrmValidateNumOfPRI_Transformer objForm = new FrmValidateNumOfPRI_Transformer();
            //objForm.Show();
        }

        private void Validate9999Elbow_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FrmValElbowOperatingNum objForm = new FrmValElbowOperatingNum();
            objForm.Show();
        }

        private void ValidateTrans_Downstream_CondChk_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmTrans_DoenstreamPhaseChk objForm = new frmTrans_DoenstreamPhaseChk();
            objForm.Show();
        }

        private void RebuildDuctdefinition_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmRebuildDuctdefinition objForm = new frmRebuildDuctdefinition();
            objForm.Show();
        }

        private void updateLocalOfficeid_Click(object sender, EventArgs e)
        {

            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            updateLocalOfficeid_device objForm = new updateLocalOfficeid_device();
            objForm.Show();
        }

        private void generateMissingInfoRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GenerateMissingInfoRecords objForm = new GenerateMissingInfoRecords();
            objForm.Show();

        }

        private void frmupdLocalofficeid_servicelocation_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmupdLocalofficeid_servicelocation objForm = new frmupdLocalofficeid_servicelocation();
            objForm.Show();
        }

        private void updPolygonAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            updPolygonAttributes objForm = new updPolygonAttributes();
            objForm.Show();

        }

        private void secondaryRiserMovementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmSecondaryRiserMovement objForm = new frmSecondaryRiserMovement();
            objForm.Show();
        }

        private void CreateMissingUgInfoRecords_Click(object sender, EventArgs e)
        {
            

            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmCreateMissingUgInfoRecords objForm = new frmCreateMissingUgInfoRecords();
            objForm.Show();
        }

        private void deleteAnnoForBypassSwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmDeleteAnno_bypassSwitches objForm = new frmDeleteAnno_bypassSwitches();
            objForm.Show();
        }

        private void featureValidateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmValidate_devices objForm = new frmValidate_devices();
            objForm.Show();
        }

        private void validateComplexDeviceIndicatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmComplexDeviceIndicatorChk objForm = new frmComplexDeviceIndicatorChk();
            objForm.Show();
        }

        private void searchRelatedStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RelatedStructureSearch objRelatedStructure = new RelatedStructureSearch();            
            objRelatedStructure.Show();    
        }

        private void serviceLocationPhaseDesignationComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ServiceLocationPhaseDesignationComparison objForm = new ServiceLocationPhaseDesignationComparison();
            objForm.Show();  
        }

        private void CreateDefaultJointOwnerToAnchor_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmCreateDefaultJointOwnerToAnchor objForm = new frmCreateDefaultJointOwnerToAnchor();
            objForm.Show();
        }

        private void ValidateConduitAnnoPosition_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmValidateConduitAnnoPosition objForm = new frmValidateConduitAnnoPosition();
            objForm.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            frmCustAgreementNumber_PosUpd objForm = new frmCustAgreementNumber_PosUpd();
            objForm.Show();
        }

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