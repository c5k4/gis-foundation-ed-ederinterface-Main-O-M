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

using System.Windows.Browser;
using Miner.Server.Client;
using ESRI.ArcGIS.Client;
using System.IO;

namespace PageTemplates
{
    public partial class L11x17 : UserControl
    {
        private string _mapType;
        public string MapType
        {
            get { return _mapType; }
            set { _mapType = value; }
        }
        
        public L11x17()
        {
            InitializeComponent();
        }


        public string CountyName
        {
            get { return (string)GetValue(CountyNameProperty); }
            set { SetValue(CountyNameProperty, value); }
        }

        public static readonly DependencyProperty CountyNameProperty =
            DependencyProperty.Register("CountyName", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));

        public string DivisionName
        {
            get { return (string)GetValue(DivisionNameProperty); }
            set { SetValue(DivisionNameProperty, value); }
        }

        public static readonly DependencyProperty DivisionNameProperty =
            DependencyProperty.Register("DivisionName", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));

        public string DateCreated
        {
            get { return (string)GetValue(DateCreatedProperty); }
            set { SetValue(DateCreatedProperty, value); }
        }

        public static readonly DependencyProperty DateCreatedProperty =
            DependencyProperty.Register("DateCreated", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));

        public string ThumbnailSource
        {
            get { return (string)GetValue(ThumbnailSourceProperty); }
            set { SetValue(ThumbnailSourceProperty, value); }
        }

        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));

        public string MapTypeName
        {
            get { return (string)GetValue(MapTypeNameProperty); }
            set { SetValue(MapTypeNameProperty, value); }
        }

        public static readonly DependencyProperty MapTypeNameProperty =
            DependencyProperty.Register("MapTypeName", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));
        
        public string ScaleText
        {
            get { return (string)GetValue(ScaleTextProperty); }
            set { SetValue(ScaleTextProperty, value); }
        }

        public static readonly DependencyProperty ScaleTextProperty =
            DependencyProperty.Register("ScaleText", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));

        public string GridNumberText
        {
            get { return (string)GetValue(GridNumberTextProperty); }
            set { SetValue(GridNumberTextProperty, value); }
        }

        public static readonly DependencyProperty GridNumberTextProperty =
            DependencyProperty.Register("GridNumberText", typeof(string),
            typeof(L11x17), new PropertyMetadata(""));        
    }
}
