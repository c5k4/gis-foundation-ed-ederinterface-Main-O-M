using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using PGE.BatchApplication.AUConveyor.Utilities;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Microsoft.Win32;
using Miner.Interop;
using stdole;

namespace PGE.BatchApplication.AUConveyor.Autoupdaters
{
    //TODO: Add support for more AU Types if they exist.

    /// <summary>
    /// Wrapper class used for instantiating, casting, and executing a given AU.
    /// </summary>
    internal class AUWrapper : IDisposable
    {
        object _AU;
        string _auName;
        internal Guid GUID;

        internal string Name
        {
            get
            {
                return _auName;
            }
        }

        internal AUWrapper(string progID)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;

            string auName = null;

            Type t = Type.GetTypeFromProgID(progID);
            var path = Assembly.GetAssembly(t).Location;
            var assembly = Assembly.LoadFile(path);

            //Find the AU, ensure it exists, instantiate it.
            object AU = Activator.CreateInstance(assembly.GetType(Type.GetTypeFromProgID(progID).ToString()));
            Initialize(AU, out auName);

            //AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolveHandler;
        }

        private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs e)
        {
            try
            {
                string[] assemblyDetail = e.Name.Split(',');
                string assemblyBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly assembly = Assembly.LoadFrom(assemblyBasePath + @"\" + assemblyDetail[0] + ".dll");
                return assembly;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed resolving assembly", ex);
            }
        }

        internal AUWrapper(IUID autoGenID, out string auName)
        {
            auName = null;
            IMMUIDTools UIDT = null;

            try
            {
                UIDT = new MMUIDToolsClass();
                
                Type type = Type.GetTypeFromCLSID(new Guid(autoGenID.Value.ToString()), true);
                object obj = (IDispatch)Activator.CreateInstance(type);
                
                //Find the AU, ensure it exists, instantiate it.
                string genValue = autoGenID.Value.ToString();
                string test = UIDT.ProgIDFromCLSID(genValue);
                object AU = UIDT.CreateFromClsidString(genValue);
                Initialize(AU, out auName);
            }
            catch { }
            finally
            {
                if (UIDT != null) while (Marshal.ReleaseComObject(UIDT) > 0) { }
            }
        }

        private void Initialize(object AU, out string auName)
        {
            auName = "";
            _AU = AU;
            GUID = _AU.GetType().GUID;

            if (_AU is IMMSpecialAUStrategy)
            {
                IMMSpecialAUStrategy specialAU = _AU as IMMSpecialAUStrategy;
                _auName = specialAU.Name;
            }
            else if (_AU is IMMSpecialAUStrategyEx)
            {
                IMMSpecialAUStrategyEx specialExAU = _AU as IMMSpecialAUStrategyEx;
                _auName = specialExAU.Name;
            }
            else if (_AU is IMMAttrAUStrategy)
            {
                IMMAttrAUStrategy attrAU = _AU as IMMAttrAUStrategy;
                _auName = attrAU.Name;
            }

            if (_auName != null)
            {
                auName = _auName;
                LogManager.WriteLine("    Found AU. Name: " + _auName);
            }
        }

        internal void Execute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            //Call the AU's Execute() method.
            if (_AU is IMMSpecialAUStrategy)
                (_AU as IMMSpecialAUStrategy).Execute(pObject);
            else if (_AU is IMMSpecialAUStrategyEx)
                (_AU as IMMSpecialAUStrategyEx).Execute(pObject, eAUMode, eEvent);
            else if (_AU is IMMAttrAUStrategy)
                (_AU as IMMAttrAUStrategy).GetAutoValue(pObject);
        }

        public void Dispose()
        {
            if (_AU != null) while (Marshal.ReleaseComObject(_AU) > 0) { }
        }
    }
}
