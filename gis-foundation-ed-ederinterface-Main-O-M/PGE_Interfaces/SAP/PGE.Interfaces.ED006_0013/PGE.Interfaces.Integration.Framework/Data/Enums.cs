using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.Integration.Framework.Data
{
    class Enums
    {
    }
    /// <summary>
    /// Types of changes
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// Data was updated
        /// </summary>
        Update,
        /// <summary>
        /// Data was inserted
        /// </summary>
        Insert,
        /// <summary>
        /// Data was deleted
        /// </summary>
        Delete,
        /// <summary>
        /// Data is being resent to SAP
        /// </summary>
        Reprocess
    }
    /// <summary>
    /// How a field transformer gets its data. Not currently used.
    /// </summary>
    public enum FieldTransformerType
    {
        /// <summary>
        /// The name of the field
        /// </summary>
        FieldName,
        /// <summary>
        /// The index of the field
        /// </summary>
        FieldIndex,
        /// <summary>
        /// The model name assigned to the field
        /// </summary>
        ModelName
    }
}
