using System;
using System.Collections.Generic;
using System.Text;

namespace PGEElecCleanup
{
    class clsGeoDBconnections
    {
        public static clsConnectionProperties pTestConProperties = new clsConnectionProperties();
        public static clsConnectionProperties pConversionConProperties = new clsConnectionProperties();

        public void initTestConProperties(string lUsername,
                                          string Password,
                                          string Server,
                                          string Service,
                                          string rVersion,
                                          string Database)
        {
            try
            {
                pTestConProperties.strUserName = lUsername;
                pTestConProperties.strPassword = Password;
                pTestConProperties.strServer = Server;
                pTestConProperties.strService = Service;
                pTestConProperties.strVersion = rVersion;
                pTestConProperties.strDatabase = Database;

                pTestConProperties.initPropertySet();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void initConversionConProperties(string lUsername,
                                          string Password,
                                          string Server,
                                          string Service,
                                          string rVersion,
                                          string Database)
        {
            try
            {
                pConversionConProperties.strUserName = lUsername;
                pConversionConProperties.strPassword = Password;
                pConversionConProperties.strServer = Server;
                pConversionConProperties.strService = Service;
                pConversionConProperties.strVersion = rVersion;
                pConversionConProperties.strDatabase = Database;

                pConversionConProperties.initPropertySet();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
