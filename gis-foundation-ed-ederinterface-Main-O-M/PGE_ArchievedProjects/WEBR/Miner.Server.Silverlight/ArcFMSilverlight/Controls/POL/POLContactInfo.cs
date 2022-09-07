using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ESRI.ArcGIS.Client;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using System.Xml.Linq;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using NLog;
using System.Windows.Browser;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArcFMSilverlight
{
    public class POLContactInfo : Control
    {
        private ToggleButton _POLToggleButton;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private const string SelectionCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/NewSelection.png";
        private static bool isPOLContactInfoCurrentlyActive = false;
        private string _mapServiceURL;
        //private int _supportStructureLayerID;
        //private int _custAgreementRelID;
        private List<Graphic> _custAgreeGraphics;
        private int _currentIndex = 0;
        private string _custAgreeDefinitionExp = "SUBTYPECD = 2";
        private List<int> _layerIDsArr = new List<int>();
        private List<string> _layerNamesArr = new List<string>();
        private List<int> _relationshipIDsArr = new List<int>();

        public POLContactInfo(ToggleButton POLToggleBtn, Map map, XElement configElement)
        {
            DefaultStyleKey = typeof(POLContactInfo);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _POLToggleButton = POLToggleBtn;
            this.Map = map;
            OnApplyTemplate();
            _POLToggleButton.IsEnabled = true;
            ReadConfiguration(configElement);
            CustomerPIIInfo.customerPIIInfo.PreviousCmd.Click += new RoutedEventHandler(PreviousCmd_Click);
            CustomerPIIInfo.customerPIIInfo.NextCmd.Click += new RoutedEventHandler(NextCmd_Click);
        }

        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(POLContactInfo),
            new PropertyMetadata(OnMapChanged));

        [Category("Export Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        #region Public Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _POLToggleButton.Click += new RoutedEventHandler(_POLToggleButton_Click);

        }

        #endregion Public Overrides

        #region IActiveControl Members

        public bool IsPOLContactInfoActive
        {
            get
            {
                return (bool)GetValue(IsActivePOLContactInfoProperty);
            }
            set
            {
                SetValue(IsActivePOLContactInfoProperty, value);
                if (IsPOLContactInfoActive)
                {
                    if (this.Map != null)
                        this.Map.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickPOL);
                }
                else
                {
                    if (this.Map != null)
                        this.Map.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickPOL);
                }

                _POLToggleButton.IsChecked = value;
            }
        }


        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActivePOLContactInfoProperty = DependencyProperty.Register(
            "IsPOLContactInfoActive",
            typeof(bool),
            typeof(POLContactInfo),
            new PropertyMetadata(OnIsPOLContactInfoActiveChanged));

        private static void OnIsPOLContactInfoActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (POLContactInfo)d;
            isPOLContactInfoCurrentlyActive = (bool)e.NewValue;
            if (isPOLContactInfoCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;
                CursorSet.SetID(control.Map, SelectionCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.Map);
                if (currentMapCursor.Contains("NewSelection.png") == true)
                    CursorSet.SetID(control.Map, "Arrow");
                CustomerPIIInfo.customerPIIInfo.DisplayResults = false;
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsPOLContactInfoActive = false;
        }

        #endregion IActiveControl Members

        private void ReadConfiguration(XElement POLElement)
        {
            try
            {
                var attribute = "";
                if (POLElement.Name.LocalName == "POL")
                {
                    attribute = POLElement.Attribute("Url").Value;
                    if (attribute != null)
                    {
                        _mapServiceURL = attribute;
                    }

                    int intAttribute = 0;
                    if (POLElement.HasElements)
                    {
                        foreach(XElement layerElem in POLElement.Elements()){
                            intAttribute = Convert.ToInt32(layerElem.Attribute("LayerId").Value);
                            if (intAttribute != null && intAttribute != 0)
                            {
                                _layerIDsArr.Add(intAttribute);
                            }

                            attribute = layerElem.Attribute("LayerName").Value;
                            if (attribute != null)
                            {
                                _layerNamesArr.Add(attribute);
                            }

                            intAttribute = Convert.ToInt32(layerElem.Attribute("RelationshipIds").Value);
                            if (intAttribute != null && intAttribute != 0)
                            {
                                _relationshipIDsArr.Add(intAttribute);
                            }
                        }
                    }
                    //attribute = POLElement.Attribute("LayerId").Value;
                    //if (attribute != null)
                    //{
                    //    _supportStructureLayerID = Convert.ToInt16(attribute);
                    //}

                    //attribute = POLElement.Attribute("RelationshipIds").Value;
                    //if (attribute != null)
                    //{
                    //    _custAgreementRelID = Convert.ToInt16(attribute);
                    //}
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tool = d as POLContactInfo;
            if (tool == null) return;
        }

        void _POLToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(_POLToggleButton.IsChecked == true)
                IsPOLContactInfoActive = true;
            else
                IsPOLContactInfoActive = false;
        }

        private void Map_MouseClickPOL(object sender, Map.MouseEventArgs e)
        {
            CustomerPIIInfo.customerPIIInfo.hideCustomerPIIInfo();
            if (_custAgreeGraphics != null)
                _custAgreeGraphics.Clear();
            _currentIndex = 0;
            ConfigUtility.StatusBar.Text = "Processing....";
            ExecuteIdentifyTask(e.MapPoint);
        }

        private void ExecuteIdentifyTask(MapPoint mapPoint)
        {
            try
            {
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = mapPoint;
                //identifyParameters.LayerIds.Add(_supportStructureLayerID);
                identifyParameters.LayerIds.AddRange(_layerIDsArr.ToArray());
                identifyParameters.MapExtent = Map.Extent;
                identifyParameters.LayerOption = LayerOption.visible;
                identifyParameters.Height = (int)Map.ActualHeight;
                identifyParameters.Width = (int)Map.ActualWidth;
                identifyParameters.SpatialReference = Map.SpatialReference;
                identifyParameters.Tolerance = 12;

                var identifyTask = new IdentifyTask(_mapServiceURL);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTaskPole_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTaskPole_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception ex)
            {
                logger.Error("Identify on map has failed: " + ex.Message);
                ConfigUtility.StatusBar.Text = "Identify on map has failed. " + ex.Message;
            }
        }

        private void identifyTaskPole_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        CustomerPIIInfo.customerPIIInfo.hideCustomerPIIInfo();
                        int selectedLayerId = e.IdentifyResults[0].LayerId;
                        string selectedLayerName = e.IdentifyResults[0].LayerName;
                        int selectedObjectId = GetObjectIDValue(e.IdentifyResults[0].Feature.Attributes);
                        if (selectedObjectId >= 0)
                        {
                            //  BEGIN AG modified on 2020-08-24
                            //CustomerPIIInfo.customerPIIInfo.CustomerPIITitle.Text = "Customer PII for " + selectedLayerName + " " + selectedObjectId;
                            CustomerPIIInfo.customerPIIInfo.CustomerPIITitle.Text = "CONFIDENTIAL: FOR INTERNAL PG&E USE ONLY (" + selectedLayerName + " " + selectedObjectId + ")";
                            //  END AG modified on 2020-08-24
                        }
                        //find customeragreement record for selected Feature
                        int selectedRelationShipId = getRelationshipId(selectedLayerId);
                        if (selectedRelationShipId != 0)
                        {
                            string[] outFields = new string[1];
                            outFields[0] = "*";
                            QueryTask task = new QueryTask(_mapServiceURL + "/" + selectedLayerId);
                            RelationshipParameter relationshipParameter = new RelationshipParameter
                            {
                                ObjectIds = new int[] { selectedObjectId },
                                RelationshipId = selectedRelationShipId,
                                OutFields = outFields,
                                OutSpatialReference = this.Map.SpatialReference
                            };
                            relationshipParameter.DefinitionExpression = _custAgreeDefinitionExp;
                            //proxyurl
                            task.ExecuteRelationshipQueryCompleted += (s, e1) =>
                            {
                                if (e1 != null && e1.Result != null)
                                {
                                    if (e1.Result.RelatedRecordsGroup != null && e1.Result.RelatedRecordsGroup.Count > 0)
                                    {
                                        if (e1.Result.RelatedRecordsGroup.ContainsKey(selectedObjectId)) //this is very unlikely to happen
                                        {
                                            _custAgreeGraphics = e1.Result.RelatedRecordsGroup[selectedObjectId].ToList();
                                            EnbleDisablePrevNextBtns();
                                            ShowCustomerPIIInformation();
                                        }
                                    }
                                    else
                                    {
                                        ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
                                    }
                                }
                                else
                                {
                                    ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
                                }
                            };
                            task.Failed += RelationshipQuery_Failed;
                            task.ExecuteRelationshipQueryAsync(relationshipParameter);

                            //QueryTask custAgreeQueryTask = new QueryTask(_mapServiceURL + "/" + _custAgreementTableID);
                            //Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                            //query.OutSpatialReference = this.Map.SpatialReference;
                            //query.OutFields.Add("*");
                            //query.Where = "GLOBALID = '" + custAgreeGlobalID + "' and SUBTYPECD = 2";
                            //custAgreeQueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(custAgreeQueryTask_ExecuteCompleted);
                            //custAgreeQueryTask.Failed += new EventHandler<TaskFailedEventArgs>(custAgreeQueryTask_Failed);
                            //custAgreeQueryTask.ExecuteAsync(query);
                        }
                        else
                            ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
                    }
                    else
                        ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
                }
                else
                {
                    ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
                }
            }
            catch (Exception err)
            {
                ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
            }
        }

        private int getRelationshipId(int layerId)
        {
            for (var i = 0; i < _layerIDsArr.Count; i++)
            {
                if (_layerIDsArr[i] == layerId)
                    return _relationshipIDsArr[i];
            }
            return 0;
        }

        //void custAgreeQueryTask_Failed(object sender, TaskFailedEventArgs e)
        //{
        //    ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
        //}

        private void RelationshipQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
        }

        void ShowCustomerPIIInformation()
        {
            try
            {
                if (_custAgreeGraphics != null && _custAgreeGraphics.Count > 0 && _custAgreeGraphics[_currentIndex] != null && _custAgreeGraphics[_currentIndex].Attributes != null)
                {
                    CustomerPIIInfo.customerPIIInfo.OwnerName = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERNAME"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerStreetAddress = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERSTREETADDRESS"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerStreetAddress2 = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERSTREETADDRESS2"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerCity = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERCITY"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerState = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERSTATE"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerZip = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERZIP"]);
                    CustomerPIIInfo.customerPIIInfo.OwnerPhone = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["OWNERPHONE"]);
                    CustomerPIIInfo.customerPIIInfo.POLNumber = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["POLNUMBER"]);
                    CustomerPIIInfo.customerPIIInfo.PremiseID = Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["PREMISEID"]);
                    if (_custAgreeGraphics[_currentIndex].Attributes["AGREEMENTDATE"] != null)
                        CustomerPIIInfo.customerPIIInfo.AgreementDate = (Convert.ToString(_custAgreeGraphics[_currentIndex].Attributes["AGREEMENTDATE"])).Split(' ')[0];
                    CustomerPIIInfo.customerPIIInfo.showCustomerPIIInfo();
                }
                else
                    ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
            }
            catch (Exception ex)
            {
                ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
            }
        }

        private void PreviousCmd_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex--;
            EnbleDisablePrevNextBtns();
            ShowCustomerPIIInformation();
        }

        private void NextCmd_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex++;
            EnbleDisablePrevNextBtns();
            ShowCustomerPIIInformation();
        }

        private void EnbleDisablePrevNextBtns()
        {
            //handle Previous button
            if (_currentIndex > 0)
                CustomerPIIInfo.customerPIIInfo.PreviousCmd.IsEnabled = true;
            else
                CustomerPIIInfo.customerPIIInfo.PreviousCmd.IsEnabled = false;

            //handle Next button
            if (_custAgreeGraphics != null)
            {
                if (_currentIndex < _custAgreeGraphics.Count-1)
                    CustomerPIIInfo.customerPIIInfo.NextCmd.IsEnabled = true;
                else
                    CustomerPIIInfo.customerPIIInfo.NextCmd.IsEnabled = false;
            }
            else
                CustomerPIIInfo.customerPIIInfo.NextCmd.IsEnabled = false;
        }

        //void custAgreeQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        //{
        //    try
        //    {
        //        if (e.FeatureSet != null && e.FeatureSet.Features != null && e.FeatureSet.Features.Count > 0)
        //        {
        //            CustomerPIIInfo.customerPIIInfo.OwnerName = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERNAME"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerStreetAddress = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERSTREETADDRESS"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerStreetAddress2 = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERSTREETADDRESS2"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerCity = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERCITY"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerState = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERSTATE"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerZip = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERZIP"]);
        //            CustomerPIIInfo.customerPIIInfo.OwnerPhone = Convert.ToString(e.FeatureSet.Features[0].Attributes["OWNERPHONE"]);
        //            CustomerPIIInfo.customerPIIInfo.POLNumber = Convert.ToString(e.FeatureSet.Features[0].Attributes["POLNUMBER"]);
        //            CustomerPIIInfo.customerPIIInfo.PremiseID = Convert.ToString(e.FeatureSet.Features[0].Attributes["PREMISEID"]);
        //            if (e.FeatureSet.Features[0].Attributes["AGREEMENTDATE"] != null)
        //                CustomerPIIInfo.customerPIIInfo.AgreementDate = (Convert.ToString(e.FeatureSet.Features[0].Attributes["AGREEMENTDATE"])).Split(' ')[0];
        //            CustomerPIIInfo.customerPIIInfo.showCustomerPIIInfo();
        //        }
        //        else
        //            ConfigUtility.StatusBar.Text = "No POL contact information available for selected feature.";
        //    }
        //    catch (Exception ex)
        //    {
        //        ConfigUtility.StatusBar.Text = "Error in showing Customer PII Information.";
        //    }
        //}

        private void identifyTaskPole_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find the clicked feature: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of feature has failed. " + e.Error.Message;
        }

        private int GetObjectIDValue(IDictionary<string, object> attributes)
        {
            int objectID = -1;

            string oidFieldName = GetObjectIDFieldName(attributes);
            if (!string.IsNullOrEmpty(oidFieldName))
            {
                Int32.TryParse(attributes[oidFieldName].ToString(), out objectID);
            }
            return objectID;
        }

        private string GetObjectIDFieldName(IDictionary<string, object> attributes)
        {
            string oidFieldName = string.Empty;
            if (attributes != null)
            {
                if (attributes.ContainsKey("OBJECTID"))
                {
                    oidFieldName = "OBJECTID";
                }
                else if (attributes.ContainsKey("OBJECT ID"))
                {
                    oidFieldName = "OBJECT ID";
                }
                else if (attributes.ContainsKey("Object ID"))
                {
                    oidFieldName = "Object ID";
                }
                else if (attributes.ContainsKey("ObjectID"))
                {
                    oidFieldName = "ObjectID";
                }
                else if (attributes.ContainsKey("ObjectId"))
                {
                    oidFieldName = "ObjectId";
                }
                else if (attributes.ContainsKey("Object Id"))
                {
                    oidFieldName = "Object Id";
                }
                else if (attributes.ContainsKey("OID"))
                {
                    oidFieldName = "OID";
                }
            }
            return oidFieldName;
        }
    }
}
