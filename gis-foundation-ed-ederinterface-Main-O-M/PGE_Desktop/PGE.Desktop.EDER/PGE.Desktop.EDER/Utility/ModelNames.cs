using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;

namespace PGE.Desktop.EDER.Utility
{
    /// <summary>
    /// Contains a singleton for the model name manager and constants for frequently-used model names.
    /// </summary>
    internal static class ModelNames
    {
        internal const string F_LABELTEXT = "LabelText";
        internal const string UNIT = "PGE_LabelTextUnit";
        internal const string SUBRATING = "PGE_SUBTRANSFORMERRATING";
        internal const string SUBLOADTAPCHANGER = "PGE_SUBLOADTAPCHANGER";
        internal const string SUBTRANSFORMERBANK = "PGE_SUBTRANSFORMERBANK";
        internal const string VOLTAGEREGULATOR = "PGE_SUBVOLTAGEREGULATOR";
        internal const string SUBSTATIONTRANSFORMER = "PGE_SUBSTATIONTRANSFORMER";
        internal const string ANNO = "PGE_ANNOTATION";

        private static IMMModelNameManager _instance = null;

        internal static IMMModelNameManager Manager
        {
            get
            {
                if (_instance == null)
                    _instance = (IMMModelNameManager)Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMModelNameManager"));
                return _instance;
            }
        }
    }
}
