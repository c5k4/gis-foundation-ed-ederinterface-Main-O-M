using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
    public class ResourceConstants
    {
        /// <summary>
        /// Field Names
        /// </summary>
        public class FieldNames
        {
            /// <summary>
            /// JobOrder field name in PMOrder Database
            /// </summary>
            private static string _PLDBID;

            public static string PLDBID
            {
                get
                {
                    if (_PLDBID == null)
                    {
                        _PLDBID = AppConfiguration.getStringSetting("PLDBID", "PLDBID");
                    }
                    return FieldNames._PLDBID;
                }
            }
            ///// <summary>
            ///// JobOrder field name in WIPCloud Database
            ///// </summary>
            //private static string _WIPJOBORDER;

            //public static string WIPJOBORDER
            //{
            //    get
            //    {
            //        if (_WIPJOBORDER == null)
            //        {
            //            _WIPJOBORDER = AppConfiguration.getStringSetting("WIPJOBNUMBER", "INSTALLJOBNUMBER");
            //        }
            //        return FieldNames._WIPJOBORDER;
            //    }
            //}
            /// <summary>
            ///// WIPFOUND field name in PMOrder
            ///// </summary>
            //public const string WIPFound = "WIPFOUND";
            ///// <summary>
            ///// WIPMISSING field name in PMOrder
            ///// </summary>
            //public const string WIPMissing = "WIPMISSING";
            ///// <summary>
            ///// LastMessageDate field name in PMOrder
            ///// </summary>
            //public const string LastMessageDate = "LastMessageDate";
        }

        /// <summary>
        /// XML Node Names
        /// </summary>
        public class XMLNodeNames
        {
            /// <summary>
            /// JobOrder XML Node Name
            /// </summary>
            public const string PLDBID = "PLDBID";
        }

    }
}
