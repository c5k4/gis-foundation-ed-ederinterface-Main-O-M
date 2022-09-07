using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Linq;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Text/Value pair for model names used to
    /// identify common features (e.g. CommonElectricFeatures
    /// </summary>
    public class ProtectiveDevice : INotifyPropertyChanged
    {
        private bool _isEnabled;

        #region public properties

        /// <summary>
        /// Gets the LayerID the protective device is associated with.
        /// </summary>
        public int LayerID { get; internal set; }

        /// <summary>
        /// Gets the name of the protective device.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the source service url that contained the protective device.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the state of the protective device.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnNotifyPropertyChanged("IsEnabled");
            }
        }

        #endregion public properties

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Notifies the listeners when a property changed
        /// </summary>
        /// <param name="p"></param>
        protected void OnNotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        /// <summary>
        /// Fires when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        /// <summary>
        /// Return name as the default implementation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        internal static List<ProtectiveDevice> FromResults(string json, string url)
        {
            List<ProtectiveDevice> protectiveDevices = new List<ProtectiveDevice>();
            if (string.IsNullOrEmpty(json) == false)
            {
                JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;

                if (jsonObject.ContainsKey(Constants.ProtectiveDevicesKey))
                {
                    JsonObject protectiveDevicesJsonObject = jsonObject[Constants.ProtectiveDevicesKey] as JsonObject;

                    var jsonArray = protectiveDevicesJsonObject[Constants.LayerIDsKey] as JsonArray;
                    if (jsonArray != null)
                    {
                        protectiveDevices.AddRange(from JsonPrimitive jsonProtectiveDevice in jsonArray
                                                   select new ProtectiveDevice
                                                   {
                                                       IsEnabled=true,
                                                       Url = url,
                                                       LayerID = Convert.ToInt32(jsonProtectiveDevice.ToString())
                                                   });
                    }
                }
            }

            return protectiveDevices;
        }
    }
}
