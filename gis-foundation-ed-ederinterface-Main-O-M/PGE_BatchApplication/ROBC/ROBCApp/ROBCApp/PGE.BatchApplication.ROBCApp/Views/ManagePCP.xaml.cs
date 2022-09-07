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
    /// Interaction logic for ManagePCP.xaml
    /// </summary>
    public partial class ManagePCP : Page
    {
        ManagePCPViewModel viewModel;
        public ManagePCP()
        {
            InitializeComponent();
            _busyIndicator.FocusAfterBusy = txtSummerKW;
        }

        public ManagePCP(PCPModel model)
        {
            viewModel = new ManagePCPViewModel(model);
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = viewModel; };
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        
    }
}
