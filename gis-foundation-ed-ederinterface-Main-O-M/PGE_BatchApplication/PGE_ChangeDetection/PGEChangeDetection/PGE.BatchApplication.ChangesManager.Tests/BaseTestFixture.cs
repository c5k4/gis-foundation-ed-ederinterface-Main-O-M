using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core;
using Castle.Windsor;
using PGE.Common.ChangesManagerShared;

namespace PGE.BatchApplication.ChangeManager.Tests
{
    public class BaseTestFixture
    {
        private static LicenseHandler _licenseHandler = null;
        protected IWindsorContainer _container;
        private string _gdbConnection = @"C:\Users\p1pc\AppData\Roaming\ESRI\Desktop10.0\ArcCatalog\edgis@pge1.sde";

        protected SDEWorkspaceConnection SDEWorkspace { get; set; }

        public BaseTestFixture()
        {
            if (_licenseHandler == null)
            {
                _container = new WindsorContainer(new XmlInterpreter());
                _licenseHandler = new LicenseHandler();
                _licenseHandler.Bind();
                _licenseHandler.CheckOut();
                this.SDEWorkspace = new SDEWorkspaceConnection(_gdbConnection);
            }
        }

        public void Dispose()
        {
            if (_licenseHandler != null)
            {
                _licenseHandler.CheckIn();
            }
        }
    }
}
