using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Miner.Geodatabase.Utilities
{
    internal class FieldIndexHelper
    {
        public const int ObjectNameFieldIndex = 1;

        public const int ObjectPrimaryDisplayFieldIndex = 2;

        public const int ObjectCustomSplitPolicyFieldIndex = 3;

        public const int ObjectDisplayNamerFieldIndex = 4;

        public const int ObjectAliasFieldIndex = 5;

        public const int ObjectClassIDIndex = 6;

        public const int SubtypeCodeFieldIndex = 1;

        public const int SubtypeEditTaskFieldIndex = 2;

        public const int SubtypeEditTaskNameFieldIndex = 3;

        public const int SubtypeFieldOrderFieldIndex = 4;

        public const int SubtypeObjectClassIDFieldIndex = 5;

        public const int SubtypeAbandonFeatureFieldIndex = 6;

        public const int SubtypeAbandonSubtypeFieldIndex = 7;

        public const int SubtypeOnAbandonFieldIndex = 8;

        public const int SubtypeSplitOnEditFieldIndex = 9;

        public const int SubtypeOnCreateFieldIndex = 10;

        public const int SubtypeOnUpdateFieldIndex = 11;

        public const int SubtypeOnDeleteFieldIndex = 12;

        public const int SubtypeOnMobileCreateFieldIndex = 13;

        public const int SubtypeOnMobileUpdateFieldIndex = 14;

        public const int SubtypeOnMobileDeleteFieldIndex = 15;

        public const int SubtypeMetadataEditorFieldIndex = 16;

        public const int SubtypeConfigurationEditorFieldEditor = 17;

        public const int SubtypeExtendedDataTableFieldIndex = 18;

        public const int SubtypeValidationRulesFieldIndex = 19;

        public const int SubtypeCustomSettingsFieldIndex = 20;

        public const int FieldNameFieldIndex = 1;

        public const int FieldAliasFieldIndex = 2;

        public const int FieldIndexFieldIndex = 3;

        public const int FieldSubtypeIDFieldIndex = 4;

        public const int FieldVisibleFieldIndex = 5;

        public const int FieldEditableFieldIndex = 6;

        public const int FieldAllowNullsFieldIndex = 7;

        public const int FieldAllowMassUpdateFieldIndex = 8;

        public const int FieldCUDefiningFieldIndex = 9;

        public const int FieldClearOnCreateFieldIndex = 10;

        public const int FieldOnCreateFieldIndex = 11;

        public const int FieldOnUpdateFieldIndex = 12;

        public const int FieldCustomFieldEditorFieldIndex = 13;

        public const int FieldValidationRuleFieldIndex = 14;

        public const int RelationshipClassIDFieldIndex = 1;

        public const int RelationshipClassNameFieldIndex = 2;

        public const int RelationshipClassAliasFieldIndex = 3;

        public const int RelationshipClassValidateRelatedFieldIndex = 4;

        public const int AutoupdaterClassID = 1;

        public const int AutoupdaterSubtype = 2;

        public const int AutoupdaterNames = 3;

        public const int AutoupdaterEvent = 4;

        protected ITable _table;

        private readonly Dictionary<string, int> _cachedIndices = new Dictionary<string, int>();

        public FieldIndexHelper()
        {
        }

        public FieldIndexHelper(ITable table)
        {
            this._table = table;
        }

        public FieldIndexHelper(IFeatureClass featureClass)
        {
            this._table = (featureClass as ITable);
        }

        public int FieldIndexFromFieldModelName(string fieldModelName)
        {
            int num = -1;
            if (this._table != null)
            {
                if (this._cachedIndices.ContainsKey(fieldModelName))
                {
                    return this._cachedIndices[fieldModelName];
                }
                IField field = ModelNameManager.Instance.FieldFromModelName(this._table as IObjectClass, fieldModelName);
                if (field != null)
                {
                    num = this._table.FindField(field.Name);
                    if (Marshal.IsComObject(field))
                    {
                        Marshal.ReleaseComObject(field);
                    }
                    this._cachedIndices.Add(fieldModelName, num);
                }
            }
            return num;
        }

        public void ReleaseCOMObjects()
        {
            Marshal.ReleaseComObject(this._table);
        }
    }
}
