using System;
using System.Collections.Generic;
using System.ComponentModel;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{

    /// <summary>
    ///  Extends the Esri Graphic class with addition of useful properties like PrimaryDisplay, Aliases, and Relationships
    /// </summary>
    public class GraphicPlus : Graphic, INotifyPropertyChanged
    {
        private string _primaryDisplay;
        private bool _locked;

        #region Constructors

        public GraphicPlus() : base() { }

        public GraphicPlus(Graphic graphic)
            : this()
        {
            if (graphic == null)
            {
                throw new NullReferenceException("graphic is null.");
            }

            Geometry = graphic.Geometry;
            MapTip = graphic.MapTip;
            Selected = graphic.Selected;
            Symbol = graphic.Symbol;
            TimeExtent = graphic.TimeExtent;
            foreach (var attribute in graphic.Attributes)
            {
                Attributes.Add(attribute);
            }
        }

        #endregion Constructors

        #region Public Events

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Primary display field of this graphic
        /// </summary>
        public string PrimaryDisplay
        {
            get
            {
                if ((_primaryDisplay == null) && (FieldAliases != null) && (DisplayFieldName != null))
                {
                    _primaryDisplay = GetPrimaryDisplay();
                }
                return _primaryDisplay;
            }
            set
            {
                _primaryDisplay = value;
                OnPropertyChanged("PrimaryDisplay");
            }
        }

        /// <summary>
        /// The dictionary of field aliases from the field information service
        /// </summary>
        public IDictionary<string, string> FieldAliases { get; set; }

        /// <summary>
        /// The list of relationships retrieved by the result relationship service
        /// </summary>
        public IEnumerable<IResultSet> Relationships { get; set; }

        /// <summary>
        /// Whether the graphic is protected from being cleared
        /// </summary>
        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
                OnPropertyChanged("Locked");
            }
        }

        /// <summary>
        /// used by the GraphicPlus node to get the primary display 
        /// </summary>
        public string DisplayFieldName { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private string GetPrimaryDisplay()
        {
            if ((DisplayFieldName != null) && (FieldAliases.ContainsKey(DisplayFieldName)))
            {
                return GetPrimaryDisplay(DisplayFieldName);
            }
            else if((DisplayFieldName != null) && (FieldAliases.ContainsKey(DisplayFieldName.ToUpper())))
            {
                // ArcGIS stores field names in caps, so make sure we test for that
                return GetPrimaryDisplay(DisplayFieldName.ToUpper());
            }
            else
            {
                return GetObjectIdDisplay();
            }
        }

        private string GetPrimaryDisplay(string fieldName)
        {
            string fieldAlias = FieldAliases[fieldName];
            fieldName = Utility.FieldToBind(fieldName, fieldAlias, Attributes);
            if (Attributes.ContainsKey(fieldName))
            {
                object value = Attributes[fieldName];
                if ((value != null) && 
                    (string.IsNullOrWhiteSpace(value.ToString()) == false) && 
                    (string.Equals(value.ToString(), "Null") == false ))
                {
                    return value.ToString();
                }
                else
                {
                    return GetObjectIdDisplay();
                }
            }
            else
            {
                return GetObjectIdDisplay();
            }
        }

        #endregion Private Methods

        private string GetObjectIdDisplay()
        {
            int oid = Utility.GetObjectIDValue(Attributes);
            if (oid >= 0)
            {
                return "(" + oid + ")";
            }
            return null;
        }
    }
}
