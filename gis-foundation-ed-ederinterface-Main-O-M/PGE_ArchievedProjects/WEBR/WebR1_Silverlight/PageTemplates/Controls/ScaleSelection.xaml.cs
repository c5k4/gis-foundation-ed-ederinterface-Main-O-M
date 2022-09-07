using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;

namespace PageTemplates
{
    public partial class ScaleSelection : ChildWindow
    {
        private string _selectedScale;
        
        public ScaleSelection()
        {
            InitializeComponent();

            comboScaleSelection.Loaded += new RoutedEventHandler(comboScaleSelection_Loaded);
            comboScaleSelection.SelectionChanged += new SelectionChangedEventHandler(comboScaleSelection_SelectionChanged);
        }

        void comboScaleSelection_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources.Contains("ScaleOptions"))
            {
                List<Tuple<string, string>> scaleSelectionItems = (List<Tuple<string, string>>)Application.Current.Resources["ScaleOptions"];

                foreach (var scaleItem in scaleSelectionItems)
                {                    
                    comboScaleSelection.Items.Add(new ComboBoxItem { Content = scaleItem.Item1, Tag = scaleItem.Item2 });
                }
            }
        }

        void comboScaleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            _selectedScale = ((ComboBoxItem)cb.SelectedItem).Tag.ToString();
            
            if (cb.SelectedIndex != -1) ApplyScaleButton.IsEnabled = true;
        }

        public static void ss_Closed(object sender, EventArgs e, Map templateMap, string mapType)
        {
            ScaleSelection ssWindow = (ScaleSelection)sender;

            if (ssWindow.DialogResult == true)
            {
                string selectedScale = ssWindow.SelectedScale;
                TemplateOptions.TemplateMap_Loaded(templateMap, mapType, selectedScale);                
            }
        }

        #region Public Properties

        public string SelectedScale
        {
            get { return _selectedScale; }
            set { _selectedScale = value; }
        }

        #endregion Public Properties

        #region Private Events

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ApplyScaleButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion Private Events
    }
}
