using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGEElecCleanup
{
    class clsConnectionProperties
    {
        private string lUsername;
        private string Password;
        private string Server;
        private string Service;
        private string Database;
        private string Version;

        private IPropertySet pPropertySet = new PropertySetClass();

        public void initPropertySet()
        {
            try
            {
                pPropertySet.SetProperty("USER", lUsername);
                pPropertySet.SetProperty("PASSWORD", Password);
                pPropertySet.SetProperty("SERVER", Server);
                pPropertySet.SetProperty("INSTANCE", Service);
                pPropertySet.SetProperty("VERSION", Version);
                pPropertySet.SetProperty("DATABASE", Database);
            }
            catch (Exception Ex)
            {
                //throw exception;
            }
        }

        public IPropertySet getPropertySet
        {
            get
            {
                return pPropertySet;
            }
        }
        public string strUserName
        {
            get
            {
                return lUsername;
            }
            set
            {
                lUsername = value;
            }
        }

        public string strPassword
        {
            get
            {
                return Password;
            }
            set
            {
                Password = value;
            }
        }

        public string strServer
        {
            get
            {
                return Server;
            }
            set
            {
                Server = value;
            }
        }

        public string strService
        {
            get
            {
                return Service;
            }
            set
            {
                Service = value;
            }
        }

        public string strDatabase
        {
            get
            {
                return Database;
            }
            set
            {
                Database = value;
            }
        }

        public string strVersion
        {
            get
            {
                return Version;
            }
            set
            {
                Version = value;
            }
        }
    }
}
