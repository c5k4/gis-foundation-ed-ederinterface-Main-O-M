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
    public partial class NewFavoriteWindow : ChildWindow
    {
        #region declarations

        public static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region ctor

        public NewFavoriteWindow()
        {
            InitializeComponent();
        }

        #endregion ctor

        #region events

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TxtFavorite_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.CreateFavoriteButton.IsEnabled = ((TextBox) sender).Text.Length > 0;
        }

        #endregion events

        #region properties

        public Map CurrentMap { get; set; }

        #endregion properties

        #region methods

        #endregion methods

        private void NewFavoriteWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton_Click(null, null);
            }
            if (e.Key == Key.Enter && CreateFavoriteButton.IsEnabled == true)
            {
                CreateFavoriteButton_OnClick(null, null);                
            }
        }

        private void CreateFavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Check for existing?
            this.DialogResult = true;
        }
    }
}

