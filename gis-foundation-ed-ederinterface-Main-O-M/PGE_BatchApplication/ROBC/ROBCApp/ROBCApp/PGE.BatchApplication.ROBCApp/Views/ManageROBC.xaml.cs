using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PGE.BatchApplication.ROBCService;
using Xceed.Wpf.Toolkit;
namespace PGE.BatchApplication.ROBCApp
{
    /// <summary>
    /// Interaction logic for ManageROBC.xaml
    /// </summary>
    public partial class ManageROBC : Page
    {
        Dictionary<string, string> circuitSourceROBCFieldValues = new Dictionary<string, string>();
        ManageROBCViewModel viewModel;
        public ManageROBC()
        {
            InitializeComponent();
        }

        public ManageROBC(string circuitID)
        {
            viewModel = new ManageROBCViewModel(circuitID);
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = viewModel; };

        }
        public ManageROBC(IViewModel parentViewModel)
        {
            viewModel = new ManageROBCViewModel(parentViewModel);
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = viewModel; };
        }
        public ManageROBC(Dictionary<string, string> circuitSourceROBCFieldValues)
        {
            viewModel = new ManageROBCViewModel(circuitSourceROBCFieldValues);
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = viewModel; };
        }
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                if (viewModel.ParentViewModel is MultipleCircuitsROBCViewModel)
                {
                    this.NavigationService.RemoveBackEntry();
                    MultipleCircuitsROBC multipleCircuitsROBC = new MultipleCircuitsROBC(viewModel.CircuitROBC.SubstationName, viewModel.CircuitROBC.CircuitName);
                    this.NavigationService.Navigate(multipleCircuitsROBC);
                }
                else if (viewModel.ParentViewModel is UnassignedROBCViewModel)
                {
                    this.NavigationService.RemoveBackEntry();
                    UnassignedROBCs unassignedROBCs = new UnassignedROBCs();
                    this.NavigationService.Navigate(unassignedROBCs);
                }
                else
                {
                    this.NavigationService.GoBack();
                }
            }
        }

        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            this.CmbSubBlockVal.IsEnabled = false;
            this.CmbROBCVal.IsEnabled = false;
            if (!viewModel.AssignROBC())
            {
                this.CmbSubBlockVal.IsEnabled = true;
                this.CmbROBCVal.IsEnabled = true;
            }
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            this.CmbSubBlockVal.IsEnabled = false;
            this.CmbROBCVal.IsEnabled = false;
            viewModel.ExecuteWorkerProcess();
            this.CmbSubBlockVal.IsEnabled = true;
            this.CmbROBCVal.IsEnabled = true;
            
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            viewModel.HideYesNo();
            this.CmbSubBlockVal.IsEnabled = true;
            this.CmbROBCVal.IsEnabled = true;
        }
    }
}
