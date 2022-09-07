using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using Miner.Server.Client;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;

using PageTemplates;
using System.Windows.Input;
using System.Windows.Browser;
using System.Windows.Media;


namespace PageTemplates
{
    [Export(typeof(IPageTemplate))]
    public partial class Circuit_11x17L : UserControl, IPageTemplate, IPageTemplateView, IPageTemplateExport
    {
        private const string _mapType = "Circuit";
        private Map _templateMap;


        private L11x17 _templateXaml;
        
        public Circuit_11x17L()
        {
            //Presenter = new TemplatePresenter();
            _templateXaml = new L11x17
            {
                MapType = "Circuit"
            };
            _templateMap = _templateXaml.TemplateMap;
            _templateXaml.TemplateMap.MinimumResolution = .0000001;
            
            //InitializeComponent();
            //Circuit1117L.DataContext = Presenter;
            this.Loaded += new RoutedEventHandler(Circuit_11x17L_Loaded);
        }

        void Circuit_11x17L_Loaded(object sender, RoutedEventArgs e)
        {
            /*var templatePresenter = new TemplatePresenter()
            {
                CountyName = "CountyUnwired",
                DivisionName = "DivisionUnwired",
                DateCreated = "DateUnwired",
                ThumbnailSource = "ThumbUnwired",
                MapTypeName = _mapType,
                ScaleText = "ScaleUnwired",
                GridNumberText = "CR" + (string)Application.Current.Resources["SelectedCellNumber"]
            };
            
            this.DataContext = templatePresenter;
            */
            
            
            TemplateOptions.ScaleTextBlock = _templateXaml.txtBlkScale;
            TemplateOptions.GridNumberTextBlock = _templateXaml.txtBlkGridNumber;
            TemplateOptions.TemplateMap_Loaded(_templateXaml.TemplateMap, _mapType);
            //TemplateOptions.ScaleTemplate(sender);
        }
        #region public properties

        public TemplatePresenter Presenter
        {
            get { return DataContext as TemplatePresenter; }
            set
            {
                if (value != null)
                {
                    DataContext = value;
                }
            }
        }

        #endregion public properties

        #region IPageTemplate Members

        public IPageTemplateView View
        {
            get { return this; }
        }

        string IPageTemplate.Name
        {
            get { return "Circuit 11x17 Landscape"; }
        }

        #endregion

        #region IPageTemplateView

        public Map Map { get { return _templateMap; } }

        #endregion IPageTemplateView

        #region IPageTemplateExport Methods

        /// <summary>
        /// Exports the UIElement to the given stream in PDF format.
        /// </summary>
        /// <param name="element">The UIElement to export.</param>
        /// <param name="file">The file stream to export to.</param>
        public void ExportToPdf(UIElement element, Stream file)
        {
            if (element == null) return;
            if (file == null) return;

            //if (MessageBoxResult.OK != MessageBox.Show("High Resolution?", "Choose Resolution", MessageBoxButton.OKCancel))
            //{
            //    PdfExporter.ExportToStream(element, file);
            //    return;
            //}

            Miner.Server.Client.Toolkit.PdfExporter.ExportToStream(element, file);

            //PageTemplateExporter.ExportToPdf(element, file);
        }

        #endregion IPageTemplate Methods
    }
}
