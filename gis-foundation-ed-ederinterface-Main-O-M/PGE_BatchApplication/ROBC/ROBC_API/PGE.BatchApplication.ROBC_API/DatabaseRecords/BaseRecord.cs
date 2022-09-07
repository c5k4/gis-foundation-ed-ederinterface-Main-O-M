using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBC_API.DatabaseRecords
{
    public abstract class BaseRecord
    {
        private Dictionary<string, object> FieldValues = new Dictionary<string, object>();
        
        /// <summary>
        /// Add a new field to the ROBC Record
        /// </summary>
        /// <param name="FieldName">Name of the Field</param>
        public void AddField(string fieldName)
        {
            string FieldName = fieldName.ToUpper();
            FieldValues.Add(FieldName, null);
        }

        /// <summary>
        /// Add a new field to the ROBC Record.  If the field already exists, it will update the current field value
        /// </summary>
        /// <param name="FieldName">Field Name</param>
        /// <param name="FieldValue">Field Value</param>
        public void AddField(string fieldName, object FieldValue)
        {
            string FieldName = fieldName.ToUpper();
            if (FieldValues.ContainsKey(FieldName)) { FieldValues[FieldName] = FieldValue; }
            else { FieldValues.Add(FieldName, FieldValue); }
        }

        /// <summary>
        /// Sets the field to the specfied value
        /// </summary>
        /// <param name="FieldName">Field Name</param>
        /// <param name="FieldValue">Field Value</param>
        public void SetFieldValue(string FieldName, object FieldValue)
        {
            AddField(FieldName, FieldValue);
        }

        /// <summary>
        /// Obtains the value of the specified field
        /// </summary>
        /// <param name="fieldName">Field Name</param>
        /// <returns></returns>
        public object GetFieldValue(string fieldName)
        {
            if (FieldValues.ContainsKey(fieldName.ToUpper())) {return FieldValues[fieldName.ToUpper()];}
            return null;
        }

        public List<string> GetFieldList()
        {
            return FieldValues.Keys.ToList();
        }

        //Some unique fields that shouldn't be updated by the user.
        public object ObjectID
        {
            get
            {
                return GetFieldValue("OBJECTID");
            }
            set
            {
                SetFieldValue("OBJECTID", value);
            }
        }

        public object GlobalID
        {
            get
            {
                return GetFieldValue("GLOBALID");
            }
            set
            {
                SetFieldValue("GLOBALID", value);
            }
        }

        public object CreationUser
        {
            get
            {
                return GetFieldValue("CREATIONUSER");
            }
            set
            {
                SetFieldValue("CREATIONUSER", value);
            }
        }

        public object DateCreated
        {
            get
            {
                return GetFieldValue("DATECREATED");
            }
            set
            {
                SetFieldValue("DATECREATED", value);
            }
        }

        public object DateModified
        {
            get
            {
                return GetFieldValue("DATEMODIFIED");
            }
            set
            {
                SetFieldValue("DATEMODIFIED", value);
            }
        }

        public object LastUser
        {
            get
            {
                return GetFieldValue("LASTUSER");
            }
            set
            {
                SetFieldValue("LASTUSER", value);
            }
        }

    }
}
