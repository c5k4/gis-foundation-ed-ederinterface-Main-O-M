using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System;

namespace Miner.Geodatabase.FeederManager
{
    internal class FeederManagerClassModelNames
    {
        public static string CircuitSourceClassModelName = "CircuitSource";

        public static string SubsourceClassModelName = "CircuitSubsource";

        private static string legacySubsourceClassModelName = "CircuitSourceID";

        public static ITable GetCircuitSourceTable(IWorkspace workspace)
        {
            return FeederManagerClassModelNames.GetTable(workspace, FeederManagerClassModelNames.CircuitSourceClassModelName);
        }

        public static ITable GetSubsourceTable(IWorkspace workspace)
        {
            ITable table = FeederManagerClassModelNames.GetTable(workspace, FeederManagerClassModelNames.SubsourceClassModelName);
            if (table == null)
            {
                table = FeederManagerClassModelNames.GetTable(workspace, FeederManagerClassModelNames.legacySubsourceClassModelName);
            }
            return table;
        }

        private static ITable GetTable(IWorkspace workspace, string classModelName)
        {
            ITable result = null;
            IMMModelNameManager instance = ModelNameManager.Instance;
            IMMEnumTable iMMEnumTable = instance.TablesFromModelNameWS(workspace, classModelName);
            if (iMMEnumTable != null)
            {
                iMMEnumTable.Reset();
                result = iMMEnumTable.Next();
            }
            return result;
        }
    }
}
