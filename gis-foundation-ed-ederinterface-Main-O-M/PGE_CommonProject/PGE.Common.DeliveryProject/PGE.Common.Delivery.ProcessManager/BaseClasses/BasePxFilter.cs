using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop.Process;
using Miner.ComCategories;
using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    [ComVisible(true)]
    public class BasePxFilter : IMMPxFilter, IMMPxFilterEx
    {
        private string _Category;
        private string _Name;
        private string _ProgID;
        private int _Priority;
        private string _NodeTypeName;
        private string _TopLevelName;
        private string _ExtensionName;
        protected IMMPxApplication _PxApp;

        

        protected BasePxFilter(string name, string progID, int priority, string nodeTypeName, string topLevelName, string category, string extensionName)
        {

            _Name = name;
            _ProgID = progID;
            _Priority = priority;
            _NodeTypeName = nodeTypeName;
            _TopLevelName = topLevelName;
            _Category = category;
            _ExtensionName = extensionName;

        }


        #region Component Registration
        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction, ComVisible(false)]
        static void Register(string regKey)
        {
            MMFilter.Register(regKey);
        }

        /// <summary>
        /// Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction, ComVisible(false)]
        static void Unregister(string regKey)
        {
            MMFilter.Unregister(regKey);
        }
        #endregion


        public string Category
        {
            get { return _Category; }
        }

        public virtual Miner.Interop.ID8ListItem Execute()
        {
            return null;
        }

        public string FilterProgID()
        {
            

            return _ProgID;
        }

        public void Initialize(object vInitData)
        {
            if (vInitData is IMMPxApplication)
                _PxApp = (IMMPxApplication)vInitData;
        }

        public stdole.IPictureDisp LargeImage
        {
            get { return null; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public int Priority
        {
            get { return _Priority; }
        }

        public stdole.IPictureDisp SmallImage
        {
            get { return null; }
        }

        public bool Visible
        {
            get
            {
                if (_PxApp == null) return false;

                return _PxApp.FilterVisible(_NodeTypeName);
            }
        }




        public string ExtensionName
        {
            get { return _ExtensionName; }
        }
    }
}
