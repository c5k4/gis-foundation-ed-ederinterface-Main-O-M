using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using PGE.BatchApplication.ROBCService;
using Xceed.Wpf.Toolkit;
namespace PGE.BatchApplication.ROBCApp
{
    /// <summary>
    /// Interaction logic for ROBCCircuitFinder.xaml
    /// </summary>
    public partial class ROBCCircuitFinder : Page
    {
        private const string CIRCUITID = "CIRCUITID";
        private const string CIRCUITNAME = "CIRCUITNAME";
        private const string SUBSTATIONNAME = "SUBSTATIONNAME";
        private System.Threading.AutoResetEvent resetEvent = new System.Threading.AutoResetEvent(false);
        private string feederName = string.Empty;
        private Dictionary<string, string> CircuitSourceROBCFieldValues;
        public ROBCCircuitFinder()
        {
            InitializeComponent();
            txtCircuitId.Focus();
        }
        public ROBCCircuitFinder(string CircuitId, string CircuitName, string SubstationName)
        {
            InitializeComponent();
            txtCircuitId.Focus();
            this.txtCircuitId.Text = CircuitId;
            this.txtCircuitName.Text = CircuitName;
            this.txtSubStation.Text = SubstationName;
        }
        private void btnSearchCircuit_Click(object sender, RoutedEventArgs e)
        {
            SearchRobc();
        }

        private object SearchRobc()
        {

            string circuitID = !string.IsNullOrEmpty(txtCircuitId.Text) ? txtCircuitId.Text.Trim():string.Empty;
            string circuitName = !string.IsNullOrEmpty(txtCircuitName.Text) ? txtCircuitName.Text.Trim() : string.Empty;
            string substationName = !string.IsNullOrEmpty(txtSubStation.Text) ? txtSubStation.Text.Trim() : string.Empty;
            
            Dictionary<string, string> searchFieldsList = new Dictionary<string, string>();
            searchFieldsList.Add(CIRCUITID, circuitID);
            searchFieldsList.Add(CIRCUITNAME, circuitName);
            searchFieldsList.Add(SUBSTATIONNAME, substationName);
            
            Dictionary<string, string> circuitSourceROBCFieldValues = new Dictionary<string, string>();
            try
            {
                var _robcManagementService = new ROBCManagementService();
                
                this.Cursor = Cursors.Wait;
                if (string.IsNullOrEmpty(circuitID) && string.IsNullOrEmpty(circuitName) && string.IsNullOrEmpty(substationName))
                {
                    System.Windows.MessageBox.Show("Circuit ID or combination of circuit name and substation name can't be empty!");
                }
                else
                {
                    if (!string.IsNullOrEmpty(circuitID))
                    {
                        ExecuteCircuitWorkerProcess(circuitID);
                        
                    }
                    else if (!string.IsNullOrEmpty(circuitName) && !string.IsNullOrEmpty(substationName))
                    {
                        feederName = string.Format("{0};{1}", circuitName, substationName);
                        ExecuteCircuitsWorkerProcess(feederName);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Circuit ID or combination of circuit name and substation name can't be empty!", "Error");
                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Following error has occurred:\nMessage: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error");
            }
            finally
                {
                    this.Cursor = Cursors.Arrow;
                }
            return null;

        }

        private void GetCircuitDetail()
        {

        }
        private void GetCircuitsDetail()
        {

        }

        private void btnClearCircuit_Click(object sender, RoutedEventArgs e)
        {
            txtSubStation.Text = string.Empty;
            txtCircuitId.Text = string.Empty;
            txtCircuitName.Text = string.Empty;
            txtCircuitId.Focus();
        }

        #region BackgroundWorker Events

       
        public void ExecuteCircuitWorkerProcess(string circuitID)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork_Circuit;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted_Circuit;
            if (!worker.IsBusy)
            {
                _busyIndicator.IsBusy = true;
                worker.RunWorkerAsync(circuitID);
            }
        }
        

        private void Worker_DoWork_Circuit(object sender, DoWorkEventArgs e)
        {
            try
            {
                string circuitID =  (string)e.Argument;
                e.Result = new CircuitService().FindCircuit(circuitID, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //resetEvent.Set();
            }

        }

        private void Worker_RunWorkerCompleted_Circuit(object sender, RunWorkerCompletedEventArgs e)
        {
            _busyIndicator.IsBusy = false;
            if (e.Result != null)
            {
                this.CircuitSourceROBCFieldValues = (Dictionary<string, string>)e.Result;
            }
            if (CircuitSourceROBCFieldValues == null || string.IsNullOrEmpty(CircuitSourceROBCFieldValues["CIRCUITID"]))
            {
                System.Windows.MessageBox.Show("No records found. The Circuit ID may not be valid!", "Error");
            }
            else
            {
                ManageROBC ObjManageROBC = new ManageROBC(CircuitSourceROBCFieldValues);
                this.NavigationService.Navigate(ObjManageROBC);
            }
        }







        public void ExecuteCircuitsWorkerProcess(string feederName)
        {

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork_Circuits;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted_Circuits;
            if (!worker.IsBusy)
            {
                _busyIndicator.IsBusy = true;
                worker.RunWorkerAsync(feederName);
            }
        }
        private void Worker_DoWork_Circuits(object sender, DoWorkEventArgs e)
        {
            try
            {
                var robcMgmtService = new ROBCManagementService();
                string feederName = (string)e.Argument;
                string[] circuitAndSubstationNames = feederName.Split(';');
                if (circuitAndSubstationNames.Count() == 2)
                {
                    string circuitName = circuitAndSubstationNames[0];
                    string substationName = circuitAndSubstationNames[1];
                    int totalItems = 0;

                    robcMgmtService.PopulateMultipleCircuitROBCs(substationName, circuitName);
                    e.Result = robcMgmtService.GetMultipleCircuitROBCs(0, 10, "CircuitId", true, out totalItems);
                
                }
                
                Dictionary<string, string> result = new Dictionary<string, string>();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
         }

        
        private void Worker_RunWorkerCompleted_Circuits(object sender, RunWorkerCompletedEventArgs e)
        {
            _busyIndicator.IsBusy = false;
            var MultipleCircuitROBCList = (System.Collections.ObjectModel.ObservableCollection<ROBCModel>) e.Result;
            if (MultipleCircuitROBCList == null || MultipleCircuitROBCList.Count <= 0)
            {
                System.Windows.MessageBox.Show("No records found. The combination of circuit name and substation name may not be valid!", "Error");
            }
            else
            {
                string[] circuitAndSubstationNames = this.feederName.Split(';');
                if (circuitAndSubstationNames.Count() == 2)
                {
                    string circuitName = circuitAndSubstationNames[0];
                    string substationName = circuitAndSubstationNames[1];

                    MultipleCircuitsROBC multipleCircuitsROBC = new MultipleCircuitsROBC(substationName, circuitName);
                    this.NavigationService.Navigate(multipleCircuitsROBC);
                }
            }
        }

        #endregion




        
    }
}
