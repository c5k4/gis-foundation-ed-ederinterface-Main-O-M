using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;

namespace PGE.ChangesManager
{
    #region Enumerations

    /// <summary>
    /// Enumeration of Filter Result types.
    /// </summary>
    public enum FilterResultType
    {
        /// <summary>
        /// Edits will only be valid if the table they are on contains a modelname in the model name list
        /// </summary>
        OnlyModelNames,
        /// <summary>
        /// Edits will not be valid if they contain edits to tables with one of the model names
        /// </summary>
        ExcludeModelNames,
		/// <summary>
        /// Edits to non-featureclasses will be removed from the existing result tables.
        /// </summary>
        ExcludeAllNonFeatures,
        /// <summary>
        /// Edits for Feature Classes will be removed from the existing result tables.
        /// </summary>
        ExcludeAllFeatures,
		/// <summary>
        /// Edits will not be valid for the specified FeatureTypes(e.g esriFTAnnotation, esriFTRasterCatalogItem)
        /// http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/esriFeatureType.htm
        /// </summary>
        ExcludeFeatureTypes,
        /// <summary>
        /// Edits will not be valid for the specified FeatureTypes(e.g esriFTAnnotation, esriFTRasterCatalogItem)
        /// http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/esriFeatureType.htm
        /// </summary>
        OnlyFeatureTypes,
        /// <summary>
        /// Edits will only be valid if they are for the list of edited tables
        /// </summary>
        OnlyTables,
        /// <summary>
        /// Edits will not be valid if they are for the list of provided tables
        /// </summary>
        ExcludeTables,
        /// <summary>
        /// Edits will not be valid if they are for the IDatasetType Specified.
        /// http://resources.esri.com/help/9.3/ArcGISEngine/ArcObjects/esriGeoDatabase/esriDatasetType.htm
        /// </summary>
        ExcludeDatasetTypes,
        /// <summary>
        /// Edits will only be valid if they are for the IDatasetType specified.
        /// http://resources.esri.com/help/9.3/ArcGISEngine/ArcObjects/esriGeoDatabase/esriDatasetType.htm
        /// </summary>
        OnlyDatasetTypes,
        /// <summary>
        /// No filters applied: all results will be returned no filters applied
        /// </summary>
        None
    }

    /// <summary>
    /// Enumeration of row (object) types.
    /// </summary>
    public enum ChangeRowTypes
    {
        /// <summary>
        /// Annotation Feature Class
        /// </summary>
        Annotation,
        /// <summary>
        /// Line Feature Class
        /// </summary>
        Line,
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Other
        /// </summary>
        Other,
        /// <summary>
        /// Point Feature Class
        /// </summary>
        Point,
        /// <summary>
        /// Polygon Feature Class
        /// </summary>
        Polygon,
        /// <summary>
        /// General Table.
        /// </summary>
        Table
    }

    #endregion

    /// <summary>
    /// Lightweight wrapper for row information.
    /// </summary>
    public struct ChangeRow
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
            qfTemp.WhereClause = tempTable.OIDFieldName +" = "+ this.OID;
            ICursor tempCursor = tempTable.Search(qfTemp, false);
            IRow returnRow = tempCursor.NextRow();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tempTable);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(qfTemp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tempCursor);
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

        #endregion
    }

    /// <summary>
    /// A dictionary mapping a list of features corresponding to the table name.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ChangeTable : KeyedCollection<int,ChangeRow>
    {
        #region Fields

        /// <summary>
        /// If the row is a feature class.
        /// </summary>
        public readonly bool IsFC;

        /// <summary>
        /// The row type.
        /// </summary>
        public readonly ChangeRowTypes RowType;

        /// <summary>
        /// The table name.
        /// </summary>
        public readonly string TableName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public ChangeTable(ITable table)
        {
            IsFC = (table is IFeatureClass);
            TableName = ((IDataset)table).Name;
            RowType = ChangeRowTypes.Table;
            if (IsFC)
            {
                IFeatureClass featClass = (IFeatureClass)table;
                if (featClass.FeatureType == esriFeatureType.esriFTAnnotation || featClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                    RowType = ChangeRowTypes.Annotation;
                else
                {
                    switch (featClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryLine:
                        case esriGeometryType.esriGeometryPolyline:
                            RowType = ChangeRowTypes.Line;
                            break;
                        case esriGeometryType.esriGeometryMultipoint:
                        case esriGeometryType.esriGeometryPoint:
                            RowType = ChangeRowTypes.Point;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            RowType = ChangeRowTypes.Polygon;
                            break;
                        default:
                            RowType = ChangeRowTypes.Other;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="IsFeatureClass">if set to <c>true</c> it is a feature class.</param>
        /// <param name="rowType">Type of the row.</param>
        public ChangeTable(string tableName, bool IsFeatureClass, ChangeRowTypes rowType)
        {
            TableName = tableName;
            IsFC = IsFeatureClass;
            RowType = rowType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the ChangeRow, but only if it is unique.
        /// </summary>
        /// <param name="row">Row to Check and Add</param>
        public void AddUnique(ChangeRow row)
        {
            if (!this.Contains(row.OID))
            {
                base.Add(row);
            }
        }

        /// <summary>
        /// Combines the rows from another ChangeTable. Tables must match and all rows will be checked to insure only new values are added to existing list.
        /// </summary>
        /// <param name="table">The ChangeTable containing the values to Check and Add</param>
        public void AddUnique(ChangeTable table)
        {
            if (this.IsFC == table.IsFC &&
                this.RowType == table.RowType &&
                this.TableName == table.TableName)
            {
                foreach (ChangeRow row in table)
                {
                    if (!this.Contains(row.OID))
                    {
                        base.Add(row);
                    }
                }
            }
            else
            {
                throw new Exception("Objects do not match all properties: " +
                                "\nTableName Equal: " + (this.TableName == table.TableName) + " , " +
                                "\nIsFC Equal: " + (this.IsFC == table.IsFC) + " , " +
                                "\nRowType Equal: " + (this.RowType == table.RowType) + " , ");
            }
        }

        /// <summary>
        /// Gets the row using the specified workspace.
        /// </summary>
        /// <param name="workspace">Workspace to use to get the Object</param>
        /// <param name="row">The row.</param>
        /// <returns>
        /// The <see cref="ESRI.ArcGIS.Geodatabase.IRow"/> for the given workspace.
        /// </returns>
        public IRow GetIRow(IWorkspace workspace, ChangeRow row)
        {
            IFeatureWorkspace fws = (IFeatureWorkspace)workspace;
            ITable tempTable = fws.OpenTable(this.TableName);
            IQueryFilter qfTemp = new QueryFilter();
            qfTemp.WhereClause = tempTable.OIDFieldName + " = " + row.OID;
            ICursor tempCursor = tempTable.Search(qfTemp, false);
            IRow returnRow = tempCursor.NextRow();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tempTable);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(qfTemp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tempCursor);
            return returnRow;
        }

        /// <summary>
        /// Adds the ChangeRow, but only if it is unique.
        /// </summary>
        /// <param name="row">Row to Check and Add</param>
        public new void Add(ChangeRow row)
        {
            if (!this.Contains(row.OID))
            {
                base.Add(row);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        protected override int GetKeyForItem(ChangeRow item)
        {
            return item.OID;
        }

        #endregion
    }
}