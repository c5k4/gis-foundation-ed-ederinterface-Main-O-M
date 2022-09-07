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
using System.Windows.Media;

using System.Windows.Input;
using System.Windows.Browser;

namespace PageTemplates
{
    [Export(typeof(IPageTemplate))]
    public partial class AdHoc_85x11_P : UserControl, IPageTemplate, IPageTemplateView, IPageTemplateExport
    {
        private const string _maptype = "Ad Hoc";

        public AdHoc_85x11_P()
        {
            Presenter = new TemplatePresenter();
            // Required to initialize variables
            InitializeComponent();

            TemplateScaleBar.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(TemplateScaleBar_MouseLeftButtonDown);
            TemplateScaleBar.GotFocus += new RoutedEventHandler(TemplateScaleBar_GotFocus);
            TemplateScaleBar.MouseEnter += new System.Windows.Input.MouseEventHandler(TemplateScaleBar_MouseEnter);
            TemplateScaleBar.MouseLeave += new MouseEventHandler(TemplateScaleBar_MouseLeave);

            this.Loaded += new RoutedEventHandler(TemplateMap_Loaded);
        }

        void TemplateMap_Loaded(object sender, RoutedEventArgs e)
        {
            TemplateOptions.ScaleTextBlock = txtblkScale;
            TemplateOptions.TemplateMap_Loaded(TemplateMap, _maptype);
            TemplateOptions.ScaleTemplate(sender);           
        }

        void TemplateScaleBar_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void TemplateScaleBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        void TemplateScaleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenScaleSelectionChildWindow();
        }

        void TemplateScaleBar_GotFocus(object sender, RoutedEventArgs e)
        {
            OpenScaleSelectionChildWindow();
        }

        private void OpenScaleSelectionChildWindow()
        {
            ScaleSelection ss = new ScaleSelection();

            ss.Closed += new EventHandler(ss_Closed);
            ss.Show();
        }

        void ss_Closed(object sender, EventArgs e)
        {
            ScaleSelection.ss_Closed(sender, e, TemplateMap, _maptype);
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
            get { return "Ad Hoc 8.5x11 Portrait"; }
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