using System.Windows;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <exclude/>
    public class ComposePageTemplates : IComposable
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IPageTemplate> PageTemplates { get; set; }

        public AggregateCatalog Catalog { get; set; }

#if SILVERLIGHT
        public void Compose()
        {
            foreach (var item in Application.Current.Host.InitParams)
            {
                if (!item.Key.StartsWith("Template")) continue;

                var deploymentCat = new DeploymentCatalog(item.Value);
#elif WPF
        public void Compose(Dictionary<string, string> uriRelatives)
        {
            foreach (var item in uriRelatives)
            {
                if (!item.Key.StartsWith("Templates")) continue;

                var deploymentCat = new SuperDeploymentCatalog(item.Value);
#endif
                Catalog.Catalogs.Add(deploymentCat);
                deploymentCat.DownloadCompleted += DeploymentCat_DownloadCompleted;
                deploymentCat.DownloadAsync();
            }
        }

        // Debugging
        static void DeploymentCat_DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
            }
        }

    }
}
