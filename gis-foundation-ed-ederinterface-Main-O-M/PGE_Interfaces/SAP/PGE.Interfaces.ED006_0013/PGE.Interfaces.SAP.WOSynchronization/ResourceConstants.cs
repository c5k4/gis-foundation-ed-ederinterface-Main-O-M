using PGE.Interfaces.Integration.Framework.Utilities;

namespace PGE.Interfaces.SAP.WOSynchronization
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
            private static string _JOBORDER;

            public static string JOBORDER
            {
                get
                {
                    if (_JOBORDER == null)
                    {
                        _JOBORDER = AppConfiguration.getStringSetting("PMJOBNUMBER", "JOBNUMBER");
                    }
                    return FieldNames._JOBORDER;
                }
            }
            /// <summary>
            /// JobOrder field name in WIPCloud Database
            /// </summary>
            private static string _WIPJOBORDER;

            public static string WIPJOBORDER
            {
                get
                {
                    if (_WIPJOBORDER == null)
                    {
                        _WIPJOBORDER = AppConfiguration.getStringSetting("WIPJOBNUMBER", "INSTALLJOBNUMBER");
                    }
                    return FieldNames._WIPJOBORDER;
                }
            }
            /// <summary>
            /// WIPFOUND field name in PMOrder
            /// </summary>
            public const string WIPFound = "WIPFOUND";
            /// <summary>
            /// WIPMISSING field name in PMOrder
            /// </summary>
            public const string WIPMissing = "WIPMISSING";
            /// <summary>
            /// LastMessageDate field name in PMOrder
            /// </summary>
            public const string LastMessageDate = "LastMessageDate";
        }

        /// <summary>
        /// XML Node Names
        /// </summary>
        public class XMLNodeNames
        {
            /// <summary>
            /// JobOrder XML Node Name
            /// </summary>
            public const string JOBORDERNODENAME = "ORDER";
        }

    }
}
