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
using ArcFMSilverlight.Controls.Butterfly;
using ArcFMSilverlight.Controls.DeviceSettings;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Tasks;
using TaskFailedEventArgs = Miner.Server.Client.Tasks.TaskFailedEventArgs;

namespace ArcFMSilverlight.Butterfly
{
    public class ButterflyCustomButton : IDataGridCustomButton
    {
        public Button _butterflyButton = new Button();
        private bool _isEnabled;
        public static string BUTTERFLY_BUTTON_NAME = "Butterfly Diagram";
        private IDictionary<string, object> _attributes;
        private ResultSet _resultSet;
        private ButterflyTool _butterflyTool;

        public ButterflyTool ButterflyTool { get; set; }

        static public IList<string> ButterflyLayerNames
        {
            get
            {
                return ButterflyTool.ButterflyRolloverLayerNames;
            }
        }

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

        public ButterflyCustomButton(ButterflyTool butterflyTool)
        {
            _butterflyTool = butterflyTool;
        }

        Button IDataGridCustomButton.CreateButton()
        {
            _butterflyButton.Visibility = Visibility.Collapsed;
            return _butterflyButton;
        }

        void IDataGridCustomButton.SetEnabled(string layerName)
        {
            _isEnabled = (ButterflyTool.ButterflyRolloverLayerNames.Contains(layerName));
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
            get { return BUTTERFLY_BUTTON_NAME; }
        }

        Button IDataGridCustomButton.UnderlyingButton
        {
            get { return _butterflyButton; }
        }

        Action IDataGridCustomButton.ButtonClicked
        {
            get { return ButtonClickedAction; }
        }

        private void ButtonClickedAction()
        {
            OpenButterflyDiagram(_attributes["GLOBALID"].ToString(), _resultSet.Name);
        }
        void IDataGridCustomButton.SetEnabled(IDictionary<string, object> attributes, ResultSet resultSet, string layerName)
        {
            _isEnabled = true;
            _attributes = attributes;
            _resultSet = resultSet;
        }

        void IDataGridCustomButton.Open(string globalId, string layerName)
        {
            OpenButterflyDiagram(globalId, layerName);
        }

        string GetFacilityId(string globalId, string layerName)
        {
            if (layerName.ToLower().StartsWith("vault"))
            {
                string structureGuid = _attributes["STRUCTUREGUID"].ToString();
                if (structureGuid.StartsWith("{"))
                {
                    return structureGuid;
                }
                else
                {
                    return "{" + structureGuid.ToUpper() + "}";
                }
            }
            return globalId;
        }
        void OpenButterflyDiagram(string globalId, string layerName)
        {
            //Grab the related UFMFloor, if there is one
//            _butterflyTool.IsActive = true;
            var queryTask = new QueryTask(_butterflyTool.UfmFloorMapServiceUrl);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(queryTask_Failed);
            Query query = new Query();
            query.Where += "FACILITYID='" + GetFacilityId(globalId, layerName) + "'";

            query.OutFields.Add("*");
            query.ReturnGeometry = true;
            queryTask.ExecuteAsync(query);

        }

        void queryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            MessageBox.Show("Failed to open Butterfly Diagram");
        }

        void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if (e.FeatureSet.Features == null || e.FeatureSet.Features.Count == 0)
            {
                MessageBox.Show("No Butterfly Diagram exists for this feature");
            }
            else
            {
                // Pass the geometry
                _butterflyTool.Show(e.FeatureSet.Features[0], _attributes["STRUCTURENUMBER"].ToString());
            }
        }

    }
}
