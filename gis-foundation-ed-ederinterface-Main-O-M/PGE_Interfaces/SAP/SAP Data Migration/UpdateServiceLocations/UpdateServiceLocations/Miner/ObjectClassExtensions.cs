using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.ModelNames;
using Miner.Interop;
using System;

namespace Miner.Geodatabase
{
    internal static class ObjectClassExtensions
    {
        internal static void AddModelName(this IObjectClass source, string fieldName, string fieldModelName)
        {
            source.AddModelName(ModelNameManager.Instance, fieldName, fieldModelName);
        }

        internal static void AddModelName(this IObjectClass source, IMMModelNameManager manager, string fieldName, string fieldModelName)
        {
            int fieldIndex = source.Fields.FindField(fieldName);
            source.AddModelName(manager, fieldIndex, fieldModelName);
        }

        internal static void AddModelName(this IObjectClass source, IMMModelNameManager manager, int fieldIndex, string fieldModelName)
        {
            if (fieldIndex < 0)
            {
                return;
            }
            IField pField = source.Fields.get_Field(fieldIndex);
            manager.AddFieldModelName(source, pField, fieldModelName);
        }

     

        public static bool IsElectricNetworkClass(this IClass source)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }
            bool result = false;
            INetworkClass networkClass = source as INetworkClass;
            if (networkClass != null)
            {
                result = NetworkIdentification.IsElectric(networkClass.GeometricNetwork);
            }
            return result;
        }

        public static bool IsGasNetworkClass(this IClass source)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }
            bool result = false;
            INetworkClass networkClass = source as INetworkClass;
            if (networkClass != null)
            {
                result = NetworkIdentification.IsGas(networkClass.GeometricNetwork);
            }
            return result;
        }

        public static bool IsWaterNetworkClass(this IClass source)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }
            bool result = false;
            INetworkClass networkClass = source as INetworkClass;
            if (networkClass != null)
            {
                result = NetworkIdentification.IsWater(networkClass.GeometricNetwork);
            }
            return result;
        }

        public static IWorkspace GetWorkspace(this IClass source)
        {
            IWorkspace workspace = null;
            IDataset dataset = source as IDataset;
            if (dataset != null)
            {
                workspace = dataset.Workspace;
                if (workspace == null)
                {
                    IRelQueryTable relQueryTable = source as IRelQueryTable;
                    if (relQueryTable != null)
                    {
                        return relQueryTable.SourceTable.GetWorkspace();
                    }
                }
            }
            return workspace;
        }

        public static IClass GetSourceTable(this IClass source)
        {
            IClass result = source;
            IRelQueryTable relQueryTable = source as IRelQueryTable;
            if (relQueryTable != null)
            {
                result = relQueryTable.SourceTable.GetSourceTable();
            }
            return result;
        }

        public static bool IsUltimateSourceClass(this IObjectClass objectClass)
        {
            return objectClass != null && ModelNameManager.Instance.ContainsClassModelName(objectClass, "CircuitSource");
        }

        public static string QualifyFieldName(this IClass source, string fieldName)
        {
            IRelQueryTable relQueryTable = source as IRelQueryTable;
            if (relQueryTable != null)
            {
                string text = source.QualifyFieldName(relQueryTable.SourceTable, fieldName);
                if (text == fieldName)
                {
                    text = source.QualifyFieldName(relQueryTable.DestinationTable, fieldName);
                }
                fieldName = text;
            }
            return fieldName;
        }

        private static string QualifyFieldName(this IClass source, ITable table, string fieldName)
        {
            if (table is IRelQueryTable)
            {
                fieldName = table.QualifyFieldName(fieldName);
            }
            else
            {
                IDataset dataset = table as IDataset;
                if (dataset != null)
                {
                    ISQLSyntax iSQLSyntax = dataset.Workspace as ISQLSyntax;
                    if (iSQLSyntax != null)
                    {
                        string text = iSQLSyntax.QualifyColumnName(dataset.Name, fieldName);
                        if (source.FindField(text) >= 0)
                        {
                            fieldName = text;
                        }
                    }
                }
            }
            return fieldName;
        }
    }
}
