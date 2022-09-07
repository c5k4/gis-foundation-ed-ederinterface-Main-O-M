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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ArcFMSilverlight.Controls.Tracing
{
    public class TraceItem : INotifyPropertyChanged
    {
        private string itemName = "";
        private string globalID = "";
        private int classID = -1;
        public TraceItem(int classID, string globalID, string layerAliasName, string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                itemName = classID + ":" + globalID;
            }
            LayerAliasName = layerAliasName;
            ItemName = itemName;
            this.classID = classID;
            this.globalID = globalID;

            TracingItemInformation.Add(new TraceItemInfo("Querying for Information", ""));
        }
        
        private ObservableCollection<TraceItemInfo> tracingItemInformation = new ObservableCollection<TraceItemInfo>();
        /// <summary>
        /// Add additional TraceItemInfo records for this item
        /// </summary>
        public ObservableCollection<TraceItemInfo> TracingItemInformation
        {
            get
            {
                return tracingItemInformation;
            }
            set
            {
                tracingItemInformation = value;
                OnPropertyChanged("TracingItemInformation");
            }
        }

        private string _layerAliasName = "";
        public string LayerAliasName
        {
            get
            {
                return _layerAliasName;
            }
            set
            {
                _layerAliasName = value;
                OnPropertyChanged("LayerAliasName");
            }
        }

        private Visibility _childrenVisibleCurrently = Visibility.Collapsed;
        public Visibility ChildrenVisibleCurrently
        {
            get
            {
                return _childrenVisibleCurrently;
            }
            set
            {
                _childrenVisibleCurrently = value;
                OnPropertyChanged("ChildrenVisibleCurrently");
                ChildrenNotVisibleCurrently = value;
            }
        }

        private Visibility _childrenNotVisibleCurrently = Visibility.Visible;
        public Visibility ChildrenNotVisibleCurrently
        {
            get
            {
                return _childrenNotVisibleCurrently;
            }
            set
            {
                if (value == Visibility.Collapsed) { _childrenNotVisibleCurrently = Visibility.Visible; }
                else { _childrenNotVisibleCurrently = Visibility.Collapsed; }
                OnPropertyChanged("ChildrenNotVisibleCurrently");
            }
        }

        public string GlobalID
        {
            get
            {
                return globalID;
            }
        }

        public int ClassID
        {
            get
            {
                return classID;
            }
        }

        public TraceItem TraceItemSelf
        {
            get
            {
                return this;
            }
        }

        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                itemName = value;
                OnPropertyChanged("ItemName");
            }
        }

        private bool obtainedFieldInformation = false;
        public bool FieldInformationObtained
        {
            get
            {
                return obtainedFieldInformation;
            }
            set
            {
                obtainedFieldInformation = value;
            }
        }

        public override bool Equals(object obj)
        {
            TraceItem otherItem = obj as TraceItem;
            if (otherItem != null)
            {
                if (((otherItem.ClassID - ClassID) == 0) && otherItem.GlobalID == GlobalID)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(itemName))
            {
                return classID + ":" + globalID;
            }
            return itemName;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TraceItemInfo
    {
        /// <summary>
        /// Simple constructor to add new field name and value pairs
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TraceItemInfo(string fieldName, string fieldValue)
        {
            _fieldName = fieldName;
            _fieldValue = fieldValue;
        }

        private string _fieldName = "";
        public string FieldName
        {
            get
            {
                return _fieldName;
            }
        }

        private string _fieldValue = "";
        public string FieldValue
        {
            get
            {
                return _fieldValue;
            }
        }
    }
}
