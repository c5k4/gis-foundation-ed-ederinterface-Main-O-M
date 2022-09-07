using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Collections.Generic;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <exclude/>
    public class ComposeExportToExcel : IComposable
    {
        #region public 

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<ICustomizeExportToExcel> Customizers { get; set; }

        public AggregateCatalog Catalog { get; set; }

#if SILVERLIGHT
        public void Compose()
        {
            foreach (var item in Application.Current.Host.InitParams)
#elif WPF
        public void Compose(Dictionary<string, string> uriRelatives)
        {
            foreach (var item in uriRelatives)
#endif
            {
                if (!item.Key.StartsWith("ExportFormats")) continue;

                var deploymentCat = new SuperDeploymentCatalog(item.Value);
                Catalog.Catalogs.Add(deploymentCat);
                deploymentCat.DownloadCompleted += DeploymentCat_DownloadCompleted;
                deploymentCat.DownloadAsync();
            }
        }

        public event EventHandler Loaded;

        #endregion public

        private void DeploymentCat_DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
            }
            else
            {
                OnLoaded(new EventArgs());
            }
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            EventHandler handler = Loaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}