using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using PGE.Common.Delivery.Framework;
using System.Linq;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Diagnostics;
using System;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Provides a facade which combines a field definition with the row containing an instance of that field.
    /// </summary>
    public class FieldInstance
    {
        private readonly IField _fieldInterface;
        private readonly int _index;
        private readonly int _fieldLength;
        private readonly IRow _row;
        private HashSet<string> _modelNames = null;

        /// <summary>
        /// Creates a known good field instance.
        /// </summary>
        /// <param name="row">The row the field instance is a member of.</param>
        /// <param name="field">The field this instance will handle.</param>
        public FieldInstance(IRow row, IField field)
        {
            _row = row;
            _fieldInterface = field;
            _index = _row.Fields.FindField(_fieldInterface.Name);
            _fieldLength = field.Length;
        }

        /// <summary>
        /// Attempts to find and construct a FieldInstance from the first field found with a given model name.
        /// If the field is missing, throws an exception.
        /// </summary>
        /// <param name="row">The row the field instance is a member of.</param>
        /// <param name="fieldModelName">The model name of the field this instance will handle.</param>
        FieldInstance(IObject obj, string fieldModelName)
        {
            _row = obj;
            _fieldInterface = ModelNameFacade.FieldFromModelName(obj.Class, fieldModelName);
            _fieldLength = _fieldInterface.Length;
            _index = _row.Fields.FindField(_fieldInterface.Name);
        }

        /// <summary>
        /// Locates a field on the row with the given name and returns an instance.
        /// If no such field is found, returns null.
        /// </summary>
        /// <param name="row">The row to search.</param>
        /// <param name="fieldName">The name of the field to search for.</param>
        /// <returns>An instance if the field is found; otherwise null.</returns>
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
                new Log4NetLogger(callingMethod.DeclaringType, "EDERDesktop.log4net.config").Warn(string.Format("Error finding field named {0}", fieldName));
                return null;
            }
        }

        /// <summary>
        /// Locates a field on the row with the given model name and returns an instance.
        /// If no field is found with the given model name, returns null.
        /// </summary>
        /// <param name="row">The row to search.</param>
        /// <param name="fieldModelName">The model name of the field to search for.</param>
        /// <returns>An instance if a field with the model name is found; otherwise null.</returns>
        public static FieldInstance FromModelName(IObject obj, string fieldModelName)
        {
            try
            {
                return new FieldInstance(obj, fieldModelName);
            }
            catch
            {
                MethodBase callingMethod = new StackFrame(1).GetMethod();
                new Log4NetLogger(callingMethod.DeclaringType, "EDERDesktop.log4net.config").Warn(string.Format("Error finding field with model {0}", fieldModelName));
                return null;
            }
        }

        /// <summary>
        /// Returns true if this field has ALL of the given model names.
        /// </summary>
        /// <param name="modelNames">The model names to look for.</param>
        /// <returns>True if this field has all of these model names; otherwise false.</returns>
        public bool HasModel(params string[] modelNames)
        {
            return modelNames.All(name => ModelNames.Contains(name));
        }

        /// <summary>
        /// Gets the set of model names assigned to this field.
        /// </summary>
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
        
        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        public string Name
        {
            get { return _fieldInterface.Name; }
        }

        /// <summary>
        /// Gets the index of the field on its row.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets the field length of this instance.
        /// </summary>
        public int FieldLength
        {
            get { return _fieldLength; }
        }

        /// <summary>
        /// Gets the value of this field.
        /// Use Convert<T>() to cast it to the necessary type.
        /// </summary>
        public object Value
        {
            get { return _row.Value[_index]; }
            set { _row.Value[_index] = value; }
        }

        /// <summary>
        /// Gets the alias of the field or, if none is assigned, the name.
        /// </summary>
        public string Alias
        {
            get { return _fieldInterface.AliasName; }
        }

        /// <summary>
        /// Assigns a value to the field and stores it.
        /// Recommend using
        /// 
        ///     [this instance].Value = value;
        ///     [source row or object].Store();
        ///     
        /// instead.
        /// </summary>
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