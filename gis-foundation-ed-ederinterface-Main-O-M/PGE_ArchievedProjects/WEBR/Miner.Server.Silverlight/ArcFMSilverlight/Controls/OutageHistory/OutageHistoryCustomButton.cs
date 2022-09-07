using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ArcFMSilverlight.Controls.DeviceSettings;
using ESRI.ArcGIS.Client;
using Miner.Server.Client.Tasks;

namespace ArcFMSilverlight.Controls.OutageHistory
{
    public class OutageHistoryCustomButton : IDataGridCustomButton
    {
        public Button _outageHistoryButton = new Button();
        private bool _isEnabled;
        public static string OUTAGE_HISTORY_BUTTON_NAME = "Outage History...";
        private ResultSet _resultSet;

        private IDictionary<string, object> _attributes;
        public IDictionary<string, object> Attributes
        {
            set
            {
                _attributes = value;
            }
        }

        bool IDataGridCustomButton.ShowMe
        {
            get
            {
                return true;
            }
        }



        Button IDataGridCustomButton.CreateButton()
        {
            _outageHistoryButton.Visibility = Visibility.Collapsed;
            return _outageHistoryButton;
        }

        void IDataGridCustomButton.SetEnabled(string layerName)
        {
            _isEnabled = (layerName == "Transformer");
        }

        bool IDataGridCustomButton.Visible
        {
            get
            {
                return true;
            }
        }

        bool IDataGridCustomButton.IsEnabled
        {
            get
            {
                return _isEnabled;
            }
        }

        bool IDataGridCustomButton.IsManuallyEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        string IDataGridCustomButton.Name
        {
            get { return OUTAGE_HISTORY_BUTTON_NAME; }
        }

        Button IDataGridCustomButton.UnderlyingButton
        {
            get { return _outageHistoryButton; }
        }

        Action IDataGridCustomButton.ButtonClicked
        {
            get { return ButtonClickedAction; }
        }

        private void ButtonClickedAction()
        {
            OutageHistoryParametersWindow outageHistoryParametersWindow = new OutageHistoryParametersWindow(_attributes);

            outageHistoryParametersWindow.Closed += new EventHandler(outageHistoryParametersWindow_Closed);
            outageHistoryParametersWindow.Show();

        }
        void IDataGridCustomButton.SetEnabled(IDictionary<string, object> attributes, ResultSet resultSet, string layerName)
        {
            _isEnabled = true;
            _attributes = attributes;
            _resultSet = resultSet;
        }

        void IDataGridCustomButton.Open(string globalId, string layerName)
        {
            ButtonClickedAction();
        }

        void outageHistoryParametersWindow_Closed(object sender, EventArgs e)
        {
            OutageHistoryParametersWindow outageHistoryParametersWindow = sender as OutageHistoryParametersWindow;

            if (outageHistoryParametersWindow.DialogResult == true)
            {
                outageHistoryParametersWindow.OpenUrl();
            }
        }
    }
}
