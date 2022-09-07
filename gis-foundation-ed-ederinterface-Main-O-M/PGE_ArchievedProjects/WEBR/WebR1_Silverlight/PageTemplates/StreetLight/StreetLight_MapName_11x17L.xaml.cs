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
    public partial class StreetLight_MapName_11x17L : UserControl, IPageTemplate, IPageTemplateView, IPageTemplateExport
    {
        private const string _maptype = "Street Light";

        public StreetLight_MapName_11x17L()
        {
            Presenter = new TemplatePresenter
            {
                MapTitle = "Street Light Map Elements"
            };
            // Required to initialize variables
            InitializeComponent();
            TemplateMap.MinimumResolution = .0000001;
            this.Loaded += new RoutedEventHandler(TemplateMap_Loaded);
        }

        void TemplateMap_Loaded(object sender, RoutedEventArgs e)
        {
            TemplateOptions.ScaleTextBlock = txtblkScale;
            TemplateOptions.GridNumberTextBlock = txtBlkGridNumber;
            TemplateOptions.TemplateMap_Loaded(TemplateMap, _maptype);
            TemplateOptions.ScaleTemplate(sender);
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
            get { return "Street Light MapName 11x17 Landscape"; }
        }

        #endregion

        #region IPageTemplateView Members

        public Map Map { get { return TemplateMap; } }

        #endregion

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
