using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;


namespace PGE.Interface.PNodeSync.PNodeSyncProcess
{
    public static class InitializeProcess
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool _blnErrOccured = false;

        #region Initialize functions
        public static bool InitializeVariables()
        {
            _logger.Debug("");
            try
            {
                _blnErrOccured = false;
                ConfigSettings.InitializeSettings();          

                //if(!ValidProperties())
                //    throw new Exception("One of the Property value in null/empty");
                //initialise fields
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            
            //return value.
            return (!ErrorOccured);
        }       
        //private bool ValidProperties()
        //{
        //    //Check the if the propeties of this class are valid or not. Return true if value, else false.
        //    bool validProperty = true;
        //    try
        //    {
        //        Type t = this.GetType();
        //        var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //        foreach (var prop in properties)
        //        {
        //            var value = prop.GetValue(this, null);
        //            if (prop.Name != "SDEWorkspaceConnection")
        //            {
        //                if (value == null || (string)value == "")
        //                {
        //                    _logger.Error("Null/empty value in property: " + prop.Name);
        //                    validProperty = false;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); validProperty = false; }
        //    return validProperty;
        //}      

        #endregion


        internal static void ErrorMessage(Exception ex, string strFunctionName)
        {
            ErrorOccured = true;
            _logger.Error(ex);
        }
        internal static bool ErrorOccured
        {
            get { return InitializeProcess._blnErrOccured; }
            set { InitializeProcess._blnErrOccured = value; }
        }
    }

}
