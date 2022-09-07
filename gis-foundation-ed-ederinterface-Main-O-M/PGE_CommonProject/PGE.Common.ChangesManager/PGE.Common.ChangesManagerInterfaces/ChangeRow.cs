using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;

namespace PGE.Common.ChangesManagerShared
{
    public enum ChangeRowUpdateType
    {
        Unset = 0,
        GeometryOnly = 1,
        AttributesOnly = 2,
        GeometryAndAttributes = 3,
        Insert = 4,
        Delete = 5,
        SessionZero = 6,
        ProposedToInService = 7
    }
    /// <summary>
    /// Lightweight wrapper for row information.
    /// </summary>
    public class ChangeRow
    {
        #region Fields

        /// <summary>
        /// Type of Edit that marked this row as changed.
        /// </summary>
        public readonly esriDataChangeType DifferenceType;

        /// <summary>
        /// The object id.
        /// </summary>
        public readonly int OID;

        public ChangeRowUpdateType UpdateType
        {
            get; set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ChangeRow"/> instance from the specified row.
        /// </summary>
        /// <param name="oid">The OID of the IRow</param>
        /// <param name="diffType">ype of the difference.</param>
        public ChangeRow(int oid, esriDataChangeType diffType)
        {
            OID = oid;
            DifferenceType = diffType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the row using the specified workspace.
        /// </summary>
        /// <param name="workspace">Workspace to use to get the Object</param>
        /// <param name="table">The table.</param>
        /// <returns>
        /// The <see cref="ESRI.ArcGIS.Geodatabase.IRow"/> for the given workspace.
        /// </returns>
        public IRow GetIRow(IWorkspace workspace, ChangeTable table)
        {
            IFeatureWorkspace fws = (IFeatureWorkspace)workspace;
            ITable tempTable = fws.OpenTable(table.TableName);
            IQueryFilter qfTemp = new QueryFilter();
            qfTemp.WhereClause = tempTable.OIDFieldName + " = " + this.OID;
            ICursor tempCursor = tempTable.Search(qfTemp, false);
            IRow returnRow = tempCursor.NextRow();
            Marshal.ReleaseComObject(tempTable);
            Marshal.ReleaseComObject(qfTemp);
            Marshal.ReleaseComObject(tempCursor);
            
            return returnRow;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        #endregion
    }
}
