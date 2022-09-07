using System.ComponentModel.Composition;
using System.Windows;

using ESRI.ArcGIS.Client;

using Miner.Server.Client;

namespace ArcFMSilverlight.PageTemplates
{
    [Export(typeof(IPageTemplate))]
    public partial class SDGE_ArchD_P : IPageTemplate, IPageTemplateView
	{
		public SDGE_ArchD_P()
		{
            Presenter = new TemplatePresenter();
            // Required to initialize variables
			InitializeComponent();
            TemplateMap.MinimumResolution = 0.000000000001;

            this.Loaded += new System.Windows.RoutedEventHandler(SDGE_ArchD_P_Loaded);

            TemplateMapInformation.GotFocus += new System.Windows.RoutedEventHandler(TemplateMapInformation_GotFocus);
            TemplateMapInformation.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(TemplateMapInformation_MouseLeftButtonDown);

            TemplateMapDisclaimer.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(TemplateMapDisclaimer_MouseLeftButtonDown);
            TemplateMapDisclaimer.GotFocus += new System.Windows.RoutedEventHandler(TemplateMapDisclaimer_GotFocus);
        }

        void SDGE_ArchD_P_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TemplateMap.Focus();
        }

        void TemplateMapDisclaimer_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            DisclaimerPopup.Visibility = Visibility.Visible;
        }

        void TemplateMapDisclaimer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DisclaimerPopup.Visibility = Visibility.Visible;
        }

        void TemplateMapInformation_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SDGE_MapInfoPopup.PrintedByText.Text = TemplateMapInformation.PrintedByText.Text;
            MapInfoPopup.Visibility = Visibility.Visible;
        }

        void TemplateMapInformation_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            MapInfoPopup.Visibility = Visibility.Visible;
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
            get { return "ArchD_P_Sample"; }
        }

        #endregion

        #region IPageTemplateView Members

        public Map Map { get { return TemplateMap; } }

        #endregion

        private void SDGE_MapInfoPopupCloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MapInfoPopup.Visibility = Visibility.Collapsed;
            TemplateMapInformation.PrintedByText.Text = SDGE_MapInfoPopup.PrintedByText.Text;
        }

        private void SDGE_DisclaimerPopupCloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DisclaimerPopup.Visibility = Visibility.Collapsed;
        }
    }
}