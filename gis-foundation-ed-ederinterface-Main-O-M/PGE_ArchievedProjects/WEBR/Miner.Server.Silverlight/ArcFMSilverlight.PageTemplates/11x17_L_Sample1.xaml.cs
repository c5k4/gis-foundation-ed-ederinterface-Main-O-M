using System.ComponentModel.Composition;
using System.Windows;

using ESRI.ArcGIS.Client;

using Miner.Server.Client;

namespace ArcFMSilverlight.PageTemplates
{
    [Export(typeof(IPageTemplate))]
    public partial class MainControl : IPageTemplate, IPageTemplateView
	{
		public MainControl()
		{
            Presenter = new TemplatePresenter();
            // Required to initialize variables
			InitializeComponent();
            Loaded += MainControlLoaded;
        }

        private void MainControlLoaded(object sender, RoutedEventArgs e)
        {
            TemplateMap.Focus();
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

		#endregion

        #region IPageTemplate Members

        public IPageTemplateView View
        {
            get { return this; }
        }

        string IPageTemplate.Name
        {
            get { return "11x17_L_Sample1"; }
        }

        #endregion

        #region IPageTemplateView Members

        public Map Map { get { return TemplateMap; } }

        #endregion

    }	
}