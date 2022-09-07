using System;
using ESRI.ArcGIS;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interface.Integration.DMS.Common
{
  internal partial class LicenseInitializer
  {
    private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
    public LicenseInitializer()
    {
      ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);
    }

    void BindingArcGISRuntime(object sender, EventArgs e)
    {
      //
      // TODO: Modify ArcGIS runtime binding code as needed; for example, 
      // the list of products and their binding preference order.
      //
      ProductCode[] supportedRuntimes = new ProductCode[] { 
        ProductCode.Engine, ProductCode.Desktop };
      foreach (ProductCode c in supportedRuntimes)
      {
        if (RuntimeManager.Bind(c))
          return;
      }

      //
      // TODO: Modify the code below on how to handle bind failure
      //

      // Failed to bind, announce and force exit
      _log4.Error("ArcGIS runtime binding failed. Application will shut down.");
      System.Environment.Exit(0);
    }
  }
}