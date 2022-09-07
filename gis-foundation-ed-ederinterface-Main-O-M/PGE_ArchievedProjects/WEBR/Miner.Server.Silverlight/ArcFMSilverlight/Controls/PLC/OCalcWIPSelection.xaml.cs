using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;

namespace ArcFMSilverlight
{
    public partial class OCalcWIPSelectionControl : ChildWindow
    {
        #region Variables

        private string _selectedPLDBID;
        private Dictionary<string, string> _PMOrderNotificNoList;
        #endregion Variables

        #region Constructor

        public OCalcWIPSelectionControl()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region public methods
        public void SetWIPSelectionData(string selectedPLDBID, Dictionary<string,string> ocalcV6PMOrderNotificNoList)
        {
            _selectedPLDBID = selectedPLDBID;
            _PMOrderNotificNoList = ocalcV6PMOrderNotificNoList;
            cboPMOrderNumList.ItemsSource = null;
            cboPMOrderNumList.ItemsSource = ocalcV6PMOrderNotificNoList.Keys;
            EnableDisableOkBtn();
        }
        #endregion public methods

        #region private methods
        private void EnableDisableOkBtn()
        {
            if (cboPMOrderNumList.SelectedItem != null)
                OkButton.IsEnabled = true;
            else
                OkButton.IsEnabled = false;
        }
        #endregion private methods

        #region events
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedPMOrder = cboPMOrderNumList.SelectedItem.ToString();
                string selectedNotificationNumber = Convert.ToString(_PMOrderNotificNoList[selectedPMOrder]);
                if (selectedNotificationNumber != "")
                    HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + _selectedPLDBID + "?PMOrderNumber=" + selectedPMOrder + "&NotificationNumber=" + selectedNotificationNumber)), "_blank");
                else
                    HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + _selectedPLDBID + "?PMOrderNumber=" + selectedPMOrder)), "_blank");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open Line Design in O-Calc v6", "Message", MessageBoxButton.OK);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cboPMOrderNumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableDisableOkBtn();
        }
        #endregion events
    }
}
