using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Telvent.Delivery.Framework;
using System.Linq;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.Diagnostics;
using System;

namespace Telvent.PGE.ED
{
    public class FieldInstance
    {
        private readonly IField _fieldInterface;
        private readonly int _index;
        private readonly IRow _row;
        private HashSet<string> _modelNames = null;

        public FieldInstance(IRow row, IField field)
        {
            _row = row;
            _fieldInterface = field;
            _index = _row.Fields.FindField(_fieldInterface.Name);
        }

        FieldInstance(IObject obj, string fieldModelName)
        {
            _row = obj;
            _fieldInterface = ModelNameFacade.FieldFromModelName(obj.Class, fieldModelName);
            _index = _row.Fields.FindField(_fieldInterface.Name);
        }

        public static FieldInstance FromFieldName(IRow row, string fieldName)
        {
            try
            {
                fieldName = fieldName.ToUpperInvariant();
                var fieldIndex = row.Fields.FindField(fieldName);
                if (fieldIndex < 0 || fieldIndex > row.Fields.FieldCount)
                    return null;
                return new FieldInstance(row, row.Fields.Field[fieldIndex]);
            }
            catch
            {
                MethodBase callingMethod = new StackFrame(1).GetMethod();
                new Log4NetLogger(callingMethod.DeclaringType, "MapProduction1.0.log4net.config").Warn(string.Format("Error finding field named {0}", fieldName));
                return null;
            }
        }

        public static FieldInstance FromModelName(IObject obj, string fieldModelName)
        {
            try
            {
                return new FieldInstance(obj, fieldModelName);
            }
            catch
            {
                MethodBase callingMethod = new StackFrame(1).GetMethod();
                new Log4NetLogger(callingMethod.DeclaringType, "MapProduction1.0.log4net.config").Warn(string.Format("Error finding field with model {0}", fieldModelName));
                return null;
            }
        }

        public bool HasModel(params string[] modelNames)
        {
            return modelNames.All(name => ModelNames.Contains(name));
        }

        private HashSet<string> ModelNames
        {
            get
            {
                if (_modelNames == null)
                {
                    var obj = _row as IObject;
                    if (obj == null) return new HashSet<string>();
                    _modelNames = new HashSet<string>(ModelNameFacade.ModelNameManager.FieldModelNames(obj.Class, _fieldInterface).AsEnumerable());
                }
                return _modelNames;
            }
        }
        
        public string Name
        {
            get { return _fieldInterface.Name; }
        }

        public int Index
        {
            get { return _index; }
        }

        public object Value
        {
            get { return _row.Value[_index]; }
            set { _row.Value[_index] = value; }
        }
        public string Alias
        {
            get { return _fieldInterface.AliasName; }
        }

        public object StoreValue
        {
            set 
            { 
                _row.Value[_index] = value;
                _row.Store();
            }
        }
    }
}