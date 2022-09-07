using System;
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
using Miner.Server.Client.Tasks;
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;

namespace ArcFMSilverlight
{
    public class CIT_CustomButton : IDataGridCustomButton
    {
        public Button _ConductorInTrenchButton = new Button();
        private bool _isEnabled = false;
        public static string CIT_FilledDuct_BUTTON_NAME = "Update FilledDuct";
        private ResultSet _resultSet;
        private IDictionary<string, object> _attributes;
        public Grid mapArea_grid = null;
        public Map map_cit = null;
        public static Conductor_in_Trench CIT_Window;
        //private string CircuitName = string.Empty;
        private object circuitname = null;
        private object globalid = null;

        public IDictionary<string, object> Attributes
        {
            set
            {
                _attributes = value;
                bool value_globalid = _attributes.TryGetValue("GLOBALID", out globalid);
                if (value_globalid == true)
                {
                    MainPage._currentGLOBALID = globalid.ToString();
                }
                query_Filter(globalid.ToString());
            }
        }

        public CIT_CustomButton(Conductor_in_Trench CIT_Object)
        {
            CIT_Window = CIT_Object;
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
            _ConductorInTrenchButton.Visibility = Visibility.Collapsed;
            return _ConductorInTrenchButton;
        }

        void IDataGridCustomButton.SetEnabled(string layerName)
        {
            _isEnabled = (layerName.ToUpper() == "PRIMARY UNDERGROUND CONDUCTOR");
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
            get { return CIT_FilledDuct_BUTTON_NAME; }
        }

        Button IDataGridCustomButton.UnderlyingButton
        {
            get { return _ConductorInTrenchButton; }
        }

        Action IDataGridCustomButton.ButtonClicked
        {
            get { return ButtonClickedAction; }
        }

        private void ButtonClickedAction()
        {
            object objectid = 0;            
            
            try
            {                
                bool value_objectid = _attributes.TryGetValue("OBJECTID", out objectid);
                if (value_objectid == true)
                {
                    MainPage._currentOBJECTID = Convert.ToInt32(objectid);
                }
                bool value_globalid = _attributes.TryGetValue("GLOBALID", out globalid);
                if (value_globalid == true)
                {
                    MainPage._currentGLOBALID = globalid.ToString();
                }
                //done for grid data-if all the attributes(fields)-69 are available
                if (_attributes.Keys.Count>50)
                {
                    bool value_CircuitName = _attributes.TryGetValue("CIRCUITNAME", out circuitname);
                    if (value_CircuitName == true)
                    {
                        MainPage._currentCircuitName = Convert.ToString(circuitname);                        
                    }
                }

                if (CIT_Window != null)
                {                    
                    //CIT_Window = new Conductor_in_Trench(mapArea_grid, map_cit);
                    //CIT_Window._fw.Close();
                    CIT_Window.busyIndicator.IsBusy = true;
                    CIT_Window.busyIndicator.BusyContent = "Processing...";
                    //CIT_Window.EditCIT_objectid(MainPage._currentOBJECTID);
                    //CIT_Window.EditCIT_filledduct(MainPage._currentOBJECTID);
                    //CIT_Window.EditCIT_citupdatedon(MainPage._currentOBJECTID);
                    MainPage._currentCircuitName = Convert.ToString(circuitname);                   
                    CIT_Window.comboDuctFilledSelection.ItemsSource = CIT_Window.fillDuctValue();
                    CIT_Window.OpenDialog(mapArea_grid, map_cit, CIT_Window);
                    //MainPage._currentCircuitName = "";
                }
                //else
                //{
                //    CIT_Window._fw.Close();
                //    //CIT_Window = new Conductor_in_Trench(mapArea_grid, map_cit);
                //    CIT_Window.comboDuctFilledSelection.ItemsSource = CIT_Window.fillDuctValue();
                //    CIT_Window.OpenDialog(mapArea_grid, map_cit, CIT_Window);
                //}
            }
            catch (Exception ee)
            {
                throw ee;
            }

        }

        void spQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if ((e.FeatureSet).Features.Count > 0)
            {
                circuitname = Convert.ToString(((e.FeatureSet).Features[0].Attributes)["CIRCUITNAME"]);
            }           
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

        void query_Filter(string globalid)
        {
            try
            {
                Query query = new Query(); //query on service point layer          
                query.OutFields.AddRange(new string[] { "CIRCUITNAME" }); //Adding City to Address 
                query.Where = "GLOBALID='" + globalid + "'";
                QueryTask queryTask = new QueryTask(MainPage._circuitname_CIT);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
                queryTask.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
