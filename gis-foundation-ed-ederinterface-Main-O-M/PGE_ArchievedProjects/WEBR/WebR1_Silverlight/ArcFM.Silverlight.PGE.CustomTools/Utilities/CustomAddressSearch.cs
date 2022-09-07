using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;

using ESRI.ArcGIS.Client.Tasks;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using MinerTask = Miner.Server.Client.Tasks;

using NLog;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class CustomAddressSearch : SearchItem
    {

        #region member variables

        private MinerTask.AddressLocateParameters _lastParameters;
        private static int _addressIndex = 0;
        private Locator _locatorTask;
        private List<Miner.Server.Client.Tasks.IResultSet> _lastResult;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private string  matchAddress = "";

        #endregion member variables

        #region public properties

        public string Fields { get; set; }
        public string Service { get; set; }
        public int AddressScore { get; set; }
        public string _searchTitle { get; set; }
        public ESRI.ArcGIS.Client.Geometry.SpatialReference LocalMapSpatialRef {get; set;}
        public ESRI.ArcGIS.Client.LayerCollection layersInMap { get; set; }

        #endregion public properties

        #region Constructor

        public CustomAddressSearch(MinerTask.ILocateTask locate)
            : base(locate)
        {
            AddressScore = 50;
            Service = "";
            Fields = "";
        }

        #endregion
        
        #region public overrides

        /// <summary>
        /// LocateAsync(string query): Configures and Submits Address Locate using the user input single line address
        /// </summary>
        /// <param name="query">Input Where Clause</param>
        public override void LocateAsync(string query)
        {

            if (query == null)
            {
                logger.Warn("Address query value is invalid. Did not execute Address Locate Operation.");
                return;
            }

            if (query.Length < 1)
            {
                logger.Warn("Address query string has a length of Zero. Enter Address and Search again. Did not execute Address Locate Operation.");
                return;
            }

            query = query.Replace("\"", "");

            matchAddress = query;

            //Clear any existing Results
            this.Results.Clear();

            if (!PassesParameterChecks())
            {
                logger.Warn("Address Parameters are incorrect. Did not execute Address Locate Operation");
                return;
            }

                //Set Original Title
                _searchTitle = this.Title;
                _lastParameters = new MinerTask.AddressLocateParameters();
            
    
                _locatorTask = new Locator(Service);
                _locatorTask.AddressToLocationsCompleted += new EventHandler<AddressToLocationsEventArgs>(q_AddressToLocationsCompleted);
                _locatorTask.Failed += new EventHandler<TaskFailedEventArgs>(q_Failed);

                var addressParams = new AddressToLocationsParameters();
                LocalMapSpatialRef = layersInMap[0].SpatialReference;
                addressParams.OutSpatialReference = LocalMapSpatialRef;
                
                Dictionary<string, string> address;
                string SearchText = "";
                
                SearchString(query, out address, out SearchText);

                if (SearchText == "")
                {
                    addressParams.Address.Add("Address", address["Address"]);
                    addressParams.Address.Add("City", address["City"]);
                    addressParams.Address.Add("State", address["State"]);
                    addressParams.Address.Add("Zip", address["Zip"]);
                            
                }
                else
                {
                    string[] strRemoveCA = SearchText.Split(',');

                    if (strRemoveCA.Length > 1)
                    {
                        if (strRemoveCA[1].Trim().ToUpper() != "CA")
                        {
                            _lastResult = new List<Miner.Server.Client.Tasks.IResultSet>();
                            OnLocateComplete(new ResultEventArgs(Results));
                            return;
                        }
                        else
                            addressParams.Address.Add("SingleLine", SearchText);
                    }
                    else
                        addressParams.Address.Add("SingleLine", SearchText);
                }

                //Set the Last Parameters to be used if needed
                _lastParameters.SpatialReference = LocalMapSpatialRef;
                _lastParameters.AddressScore = AddressScore;
                _lastParameters.Fields = Fields;
                _lastParameters.UserQuery = query;

                
                _locatorTask.AddressToLocationsAsync(addressParams);

        }

        #endregion public overrides

        #region private methods

        private void SearchString(string searchParam, out Dictionary<string, string> address, out string searchText)
        {
            string[] searchParams = null;
            string zip = string.Empty;
            address = new Dictionary<string, string>();
            searchText = "";
            //Add code Q1-2018
            if (searchParam.Contains(","))
            {
                string[] ReplacesearchParams = null;
                ReplacesearchParams = searchParam.Split(',');
                searchParam = ReplacesearchParams[0] + "," + ReplacesearchParams[1];
            }
            //if (!searchParam.Contains(","))
            //{
            //    searchParams = searchParam.Split(' ');
            //    searchParam = searchParams[0].ToString() + " " + searchParams[1].ToString();
            //   zip= searchParams[searchParams.Length - 1].ToString();
            //}
            searchParams = searchParam.Split(',');
            int number2 = 0;
            address.Add("Address", "");
            address.Add("City", "");
            address.Add("State", "CA");
            address.Add("Zip", "");

            switch (searchParams.Length)
            {
                case 1:
                    if (int.TryParse(searchParam.Trim(), out number2))
                    {
                        address["Zip"] = searchParam.Trim();
                    }
                    else
                        searchText = searchParam.Trim() + ",CA";
                    // searchText = searchParam.Trim() + ",CA" + "," +zip ;
                    break;
                case 4:
                    if (int.TryParse(searchParams[3].Trim(), out number2) && searchParams[2].Trim().Length == 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }
                        if (searchParams[1].Trim() != "")
                            address["City"] = searchParams[1].Trim();
                        if (searchParams[3].Trim() != "")
                            address["Zip"] = searchParams[3].Trim();
                    }
                    break;
                case 3:
                    if (!int.TryParse(searchParams[2].Trim(), out number2) && searchParams[2].Trim().Length == 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {

                            address["Address"] = searchParams[0].Trim();
                        }

                        if (searchParams[1].Trim() != "" && !int.TryParse(searchParams[1].Trim(), out number2))
                            address["City"] = searchParams[1].Trim();


                    }
                    else if (int.TryParse(searchParams[2].Trim(), out number2))
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }

                        address["Zip"] = searchParams[2].Trim();
                    }
                    else
                        searchText = searchParam;
                    break;

                case 2:
                    if (!int.TryParse(searchParams[1].Trim(), out number2) && searchParams[1].Trim().Length != 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }

                        if (searchParams[1].Trim() != "" && !int.TryParse(searchParams[1].Trim(), out number2))
                            address["City"] = searchParams[1].Trim();

                    }
                    else
                        searchText = searchParam;

                    break;
            }
        }

        private void ConvertToResultset(List<AddressCandidate> returnedCandidates, object userToken = null)
        {

            logger.Info("Converting Address Candidate List to Miner Result Set");

            var candidateList = new List<AddressCandidate>();

            //Check for a null response -- Server Side Error on Locate
            if (returnedCandidates == null)
            {
                _lastResult = new List<Miner.Server.Client.Tasks.IResultSet>();
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }

            //Check for a collection with 0 candidates -- No Matches Found
            if (returnedCandidates.Count < 1)
            {
                _lastResult = new List<Miner.Server.Client.Tasks.IResultSet>();
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }


            FeatureSet fs = new FeatureSet();
            int candidateIndex = 0;

            if (returnedCandidates.Count > 0)
            {
                fs.GeometryType = GeometryType.Point;
                fs.Fields = new List<ESRI.ArcGIS.Client.Field>();
                fs.Fields.Add(new ESRI.ArcGIS.Client.Field { FieldName = "MATCHED", Alias = "MATCHED", Type = ESRI.ArcGIS.Client.Field.FieldType.String, Length = 50 });
                fs.Fields.Add(new ESRI.ArcGIS.Client.Field { FieldName = "SCORE", Alias = "Score", Type = ESRI.ArcGIS.Client.Field.FieldType.String, Length = 50 });
                fs.Fields.Add(new ESRI.ArcGIS.Client.Field { FieldName = "X", Alias = "X", Type = ESRI.ArcGIS.Client.Field.FieldType.String, Length = 50 });
                fs.Fields.Add(new ESRI.ArcGIS.Client.Field { FieldName = "Y", Alias = "Y", Type = ESRI.ArcGIS.Client.Field.FieldType.String, Length = 50 });
                fs.SpatialReference = LocalMapSpatialRef;

            }


            List<string> uniqueAddress = new List<string>();
            bool boolmathchaddress = false;

            foreach (var candidate in returnedCandidates)
            {
                if (candidate.Address.ToUpper().Contains("CALIFORNIA"))
                {
                    if (candidate.Score >= _lastParameters.AddressScore)
                    {
                        if (!uniqueAddress.Contains(candidate.Address) && !boolmathchaddress)
                        {
                            uniqueAddress.Add(candidate.Address);
                            candidateIndex++;

                            if (this.matchAddress == candidate.Address)
                                boolmathchaddress = true;

                            candidateList.Add(candidate);
                            if (fs != null)
                            {
                                var graphic = new GraphicPlus();
                                string oid = (_addressIndex++).ToString();
                                graphic.Attributes.Add("OBJECTID", oid);
                                graphic.Attributes.Add("MATCHED", candidate.Address);
                                graphic.Attributes.Add("SCORE", candidate.Score);
                                graphic.Attributes.Add("X", candidate.Location.X);
                                graphic.Attributes.Add("Y", candidate.Location.Y);
                                foreach (var kvp in candidate.Attributes)
                                {
                                    graphic.Attributes.Add(kvp.Key, kvp.Value);
                                }

                                var point = new ESRI.ArcGIS.Client.Geometry.MapPoint();
                                point.X = candidate.Location.X;
                                point.Y = candidate.Location.Y;

                                var geometry = point;
                                geometry.SpatialReference = LocalMapSpatialRef;
                                graphic.Geometry = geometry;
                                graphic.Attributes.Add("RowIndex", candidateIndex);

                                fs.Features.Add(graphic);
                            }

                        }
                    }
                }
            }

            _lastResult = new List<Miner.Server.Client.Tasks.IResultSet>();

            if (candidateList.Count > 0)
            {

                Miner.Server.Client.Tasks.IResultSet resultSetColl = new MinerTask.ResultSet(fs);

                resultSetColl.Name = "Address";
                resultSetColl.ID = 0;
                resultSetColl.ExceededThreshold = false;
                resultSetColl.DisplayFieldName = "MATCHED";
                resultSetColl.Service = Service;
                resultSetColl.FieldAliases.Add("MATCHED", "Matched Address");
                resultSetColl.FieldAliases.Add("SCORE", "Score");
                resultSetColl.FieldAliases.Add("X", "X");
                resultSetColl.FieldAliases.Add("Y", "Y");

                _lastResult.Add(resultSetColl);
                Results.Add(resultSetColl);
                // }
            }

            OnLocateComplete(new ResultEventArgs(Results));

        }

        private bool PassesParameterChecks()
        {
            bool passesChecks = true;

            if (Service.Length < 1) passesChecks = false;
            if (Fields.Length < 1) passesChecks = false;
            if ((AddressScore > 100) || (AddressScore < 0)) AddressScore = 50;
            
            return passesChecks;
        }
    
        #endregion private methods

        #region Events

        private void q_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Address Locate Operation Failed -- " + e.Error.Message);

            Results.Clear();
            OnLocateComplete(new ResultEventArgs(Results));
        }

        private void q_AddressToLocationsCompleted(object sender, AddressToLocationsEventArgs e)
        {
            
            if (e.Results != null)
            {
                logger.Info("Address Locate Operation Completed Successfully with " + e.Results.Count.ToString() + " Candidates Returned");
                ConvertToResultset(e.Results, e.UserState);
            }
        }

        #endregion Events
    }
}
