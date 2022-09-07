using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PGEElecCleanup
{
    public partial class frmClenupTasks : Form
    {
        public int intTask;

        public frmClenupTasks()
        {
            intTask = 0;
            InitializeComponent();
        }
        public void updateComponents(int Task, string strHeader, string strMessage)
        {
            intTask = Task;
            lblHeader.Text = strHeader;
            //lblMessage.Text = strMessage;
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            switch (intTask)
            {
                case 77:
                    //updRatedMVA();
                    break;
                case 64:
                    //updManualOpeIdcSwitch();
                    break;
                default:
                    break;
            }

        }

        //private void updManualOpeIdcSwitch()
        //{
        //    clsUpdManOpeIdcSwitch objUpdate = null;
        //    try
        //    {
        //        objUpdate = new clsUpdManOpeIdcSwitch();
        //        objUpdate.mStatusBar = sbClenupTask;
        //        objUpdate.startProcess(ConstantValues.switchFc);
        //        objUpdate = null;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw new ApplicationException(Ex.Message);
        //    }
        //    finally
        //    {
        //        objUpdate = null;
        //    }
        //}

        //private void updRatedMVA()
        //{
        //    clsUpdRatedMVA objUpdRatedMVA = null;
        //    try
        //    {
        //        objUpdRatedMVA = new clsUpdRatedMVA();
        //        objUpdRatedMVA.mStatusBar = sbClenupTask;
        //        objUpdRatedMVA.startProcess(ConstantValues.conSTransformer, ConstantValues.conSTransformerUnit);
        //        objUpdRatedMVA = null;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw new ApplicationException(Ex.Message);
        //    }
        //    finally
        //    {
        //        objUpdRatedMVA = null;
        //    }
        //}

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
