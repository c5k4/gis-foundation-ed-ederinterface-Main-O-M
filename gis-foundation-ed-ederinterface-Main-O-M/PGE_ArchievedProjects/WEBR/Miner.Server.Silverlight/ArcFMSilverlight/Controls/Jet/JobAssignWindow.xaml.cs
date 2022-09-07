using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

using NLog;
using System.Collections.ObjectModel;

namespace ArcFMSilverlight
{
    public partial class JobAssignWindow : ChildWindow
    {
        #region declarations

        public static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region ctor

        public JobAssignWindow()
        {
            InitializeComponent();

        }

        #endregion ctor
        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        #region events

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion events

        #region properties

        public Map CurrentMap { get; set; }

        #endregion properties

        #region methods

        #endregion methods

        private void JobAssignWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton_Click(null, null);
            }
            if (e.Key == Key.Enter && OkButton.IsEnabled == true)
            {
                OkButton_OnClick(null, null);                
            }
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Check for existing?
            this.DialogResult = true;
        }


        private void JobAssignWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (usersListBox.SelectedItem != null)
            {
                usersListBox.UpdateLayout();
                usersListBox.ScrollIntoView(usersListBox.SelectedItem);
            }
        }
    }
}

