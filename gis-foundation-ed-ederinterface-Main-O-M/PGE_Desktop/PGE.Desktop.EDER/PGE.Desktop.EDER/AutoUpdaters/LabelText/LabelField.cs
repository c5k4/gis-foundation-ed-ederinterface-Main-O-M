#region

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

#endregion

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.LabelText
{
    public enum RelationAccessor
    {
        Name,
        Alias,
        Model
    }

    public class LabelField
    {
        private readonly Func<IEnumerable<IObject>, object> _aggregator;
        private readonly Func<IObject, int> _getFieldIndex;
        private readonly Func<IObject, bool> _hasField;
        private int? _fieldIndex;

        private LabelField(RelationAccessor fieldAccessor, string name)
        {
            Prefix = "";
            Postfix = "";

            switch (fieldAccessor)
            {
                case RelationAccessor.Name:
                    _getFieldIndex = o => o.Class.FindField(name);
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                case RelationAccessor.Alias:
                    _getFieldIndex = o => o.Fields.FindFieldByAliasName(name);
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                case RelationAccessor.Model:
                    _getFieldIndex = o => o.Fields.FindField(new ModelNameResolver().FieldNameFromModelName(o, name));
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fieldAccessor");
            }
        }

        private LabelField(RelationAccessor fieldAccessor, string name, Func<IEnumerable<IObject>, object> aggregator)
            : this(fieldAccessor, name)
        {
            Prefix = "";
            Postfix = "";

            _aggregator = aggregator;

            switch (fieldAccessor)
            {
                case RelationAccessor.Name:
                    _getFieldIndex = o => o.Class.FindField(name);
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                case RelationAccessor.Alias:
                    _getFieldIndex = o => o.Fields.FindFieldByAliasName(name);
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                case RelationAccessor.Model:
                    _getFieldIndex = o => o.Fields.FindField(new ModelNameResolver().FieldNameFromModelName(o, name));
                    _hasField = o => _getFieldIndex(o) != -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fieldAccessor");
            }
        }


        public string Prefix { get; set; }
        public string Postfix { get; set; }

        public static LabelField CreateFromObject(RelationAccessor fieldAccessor, string name)
        {
            return new LabelField(fieldAccessor, name);
        }

        public static LabelField CreateFromRelatedObjects(RelationAccessor fieldAccessor, string name,
                                                          Func<IEnumerable<IObject>, object> aggregator)
        {
            return new LabelField(fieldAccessor, name, aggregator);
        }

        public bool IsMemberOf(IObject obj)
        {
            return _hasField(obj);
        }

        public object GetValue(IObject obj)
        {
            return obj.Value[(int) (_fieldIndex ?? (_fieldIndex = _getFieldIndex(obj)))];
        }
    }
}