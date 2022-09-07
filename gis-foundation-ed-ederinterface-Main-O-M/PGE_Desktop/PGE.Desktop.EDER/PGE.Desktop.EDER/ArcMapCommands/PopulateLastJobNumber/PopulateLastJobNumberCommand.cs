using System.Text;
using System.Runtime.InteropServices;
using System;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.BaseClasses;

using Miner.ComCategories;

using PGE.Desktop.EDER.ArcMapCommands;
using PGE.Common.Delivery.UI.Commands;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// ToolControl implementation
    /// </summary>
    [Guid("C78D8899-5BD9-4BF7-8C36-F78AB2C8F321")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.PopulateLastJobNumberCommand")]
    [ComVisible(true)]
    public class PopulateLastJobNumberCommand : BaseArcGISCommand, IToolControl, IDisposable
    {
       
        #region Private Members
        //private IHookHelper m_hookHelper = null;
        private PopulateLastJobNumberUC _populateLastJobNumberUC = null;
        private ICompletionNotify _complete = null;
        #endregion

        #region constructor

        public PopulateLastJobNumberCommand()
            : base("PGETools_JobNumberCommand", "PG&E Job Number","", "PG&E Job Number", "PG&E Job Number")
        {
            base.m_name = "PGETools_JobNumberCommand";
        }
        
        #endregion constructor

        #region Overriden ToolControl Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="barType"></param>
        /// <returns></returns>
        public bool OnDrop(esriCmdBarType barType)
        {
            // return barType == esriCmdBarType.esriCmdBarTypeToolbar 
            return true;
        }

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="complete"></param>
        public void OnFocus(ICompletionNotify complete)
        {
            _complete = complete;
        }

        public int hWnd
        {
            get {

                if (null == _populateLastJobNumberUC)
                {
                    _populateLastJobNumberUC = new PopulateLastJobNumberUC();
                    _populateLastJobNumberUC.CreateControl();
                    _populateLastJobNumberUC.JobNumberTxtBox.LostFocus += new EventHandler(JobNumberTxtBox_LostFocus);
                }
                return _populateLastJobNumberUC.Handle.ToInt32();
            }
        }

        /// <summary>
        /// Resets the job number to its last value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void JobNumberTxtBox_LostFocus(object sender, EventArgs e)
        {
            if (_populateLastJobNumberUC != null)
            {
                _populateLastJobNumberUC.JobNumberTxtBox.Text = PopulateLastJobNumberUC.jobNumber;
                if (_complete != null)
                {
                    _complete.SetComplete();
                    _complete = null;
                }
            }
        }
        #endregion Overriden ToolControl Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_populateLastJobNumberUC != null)
                    _populateLastJobNumberUC.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
