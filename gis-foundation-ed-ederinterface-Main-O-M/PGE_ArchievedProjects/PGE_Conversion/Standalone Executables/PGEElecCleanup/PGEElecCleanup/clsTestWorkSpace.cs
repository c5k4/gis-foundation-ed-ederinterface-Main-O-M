using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Collections;

namespace PGEElecCleanup
{
    class clsTestWorkSpace
    {
        private static IWorkspaceFactory _pWorkspaceFactory = new SdeWorkspaceFactoryClass();
        private static IWorkspace _pWorkspace;
        private static IFeatureWorkspace _pFeatureWorkspace;
        private static IWorkspaceEdit _WorkspaceEdit;

        //variables
        private static bool blnDbState = false;

        public static bool State
        {
            get
            {
                return blnDbState;
            }
            set
            {
                blnDbState = value;
            }
        }

        public static IWorkspaceFactory WorkspaceFactory
        {
            get
            {
                return _pWorkspaceFactory;
            }
        }
        public static IFeatureWorkspace FeatureWorkspace
        {
            get
            {
                return _pFeatureWorkspace;
            }
        }

        public static IWorkspace Workspace
        {
            get
            {
                return _pWorkspace;
            }
        }
        /// <summary>
        /// Stops editing operation of the work space 
        /// </summary>
        /// <returns></returns>
        public static bool StopEditOperation(bool blnSave)
        {
            try
            {
                if (_WorkspaceEdit.IsBeingEdited())
                {
                    _WorkspaceEdit.StopEditOperation();
                    _WorkspaceEdit.StopEditing(blnSave);
                }
                    return true;
            }
            catch(Exception Ex)
            {
                throw new Exception("Unable to start edit operation <" + Ex.Message + ">");
            }
        }
        /// <summary>
        /// Starts the editing operation of the work space 
        /// </summary>
        /// <returns></returns>
        public static bool StartEditOperation()
        {
            try
            {
                if (_WorkspaceEdit == null)
                    _WorkspaceEdit = _pFeatureWorkspace as IWorkspaceEdit;

                if (!(_WorkspaceEdit.IsBeingEdited()))
                {
                    _WorkspaceEdit.StartEditing(true);
                    _WorkspaceEdit.StartEditOperation();
                }
                return true;
            }
            catch(Exception Ex)
            {
                throw new Exception("Unable to start edit operation <" + Ex.Message + "> " + Ex.Message);
            }
        }

        /// <summary>
        /// get the edit work space 
        /// </summary>
        public static IWorkspaceEdit WorkSpaceEdit
        {
            set
            {
                _WorkspaceEdit = value;
            }
            get
            {
                return _WorkspaceEdit;
            }
        }
        internal static bool initSDEWorkspace(clsConnectionProperties clsConnectionProperties)
        {
            try
            {
                _pWorkspace = _pWorkspaceFactory.Open(clsConnectionProperties.getPropertySet, 0);

                if (_pWorkspace == null)
                {
                    return false;
                }

                _pFeatureWorkspace = _pWorkspace as IFeatureWorkspace;

                //initialize connection properties
                strUSER = clsConnectionProperties.strUserName;
                strPASSWORD = clsConnectionProperties.strPassword;
                strSERVER = clsConnectionProperties.strServer;
                strVERSION = clsConnectionProperties.strVersion;
                strINSTANCE = clsConnectionProperties.strService;
                strDATABASE = clsConnectionProperties.strDatabase;

                return true;
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
        }

        //Variables
        //connection properties
        //USER
        //PASSWORD
        //SERVER
        //INSTANCE
        //VERSION
        //DATABASE
        public static string strUSER
        {
            get;
            set;
        }
        public static string strPASSWORD
        {
            get;
            set;
        }
        public static string strSERVER
        {
            get;
            set;
        }
        public static string strINSTANCE
        {
            get;
            set;
        }
        public static string strVERSION
        {
            get;
            set;
        }
        public static string strDATABASE
        {
            get;
            set;
        }
    }
}
