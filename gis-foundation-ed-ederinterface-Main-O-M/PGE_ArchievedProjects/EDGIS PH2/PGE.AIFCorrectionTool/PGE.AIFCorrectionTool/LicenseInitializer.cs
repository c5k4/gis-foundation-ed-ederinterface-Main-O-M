using System;
using ESRI.ArcGIS;

namespace PGE.AIFCorrection
{
    /// <summary>
    /// Auto-generated logic to initialize Esri licenses.
    /// </summary>
    internal partial class LicenseInitializer
    {
        public LicenseInitializer()
        {
            ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);
        }

        private void BindingArcGISRuntime(object sender, EventArgs e)
        {
            ProductCode[] supportedRuntimes = new ProductCode[] { 
                ProductCode.Engine, ProductCode.Desktop };
            foreach (ProductCode c in supportedRuntimes)
            {
                if (RuntimeManager.Bind(c))
                return;
            }
    
            // Failed to bind, announce and force exit
            Console.WriteLine("ArcGIS runtime binding failed. Application will shut down.");
            System.Environment.Exit(0);
        }
    }
}