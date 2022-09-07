using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Common.Delivery.Geodatabase.ChangeDetection
{
    #region Enumerations

    /// <summary>
    /// Enumeration of Filter Result types.
    /// </summary>
    public enum FilterResultType
    {
        /// <summary>
        /// Edits to non-featureclasses will be removed from the existing result tables.
        /// </summary>
        ExcludeAllNonFeatures,
        /// <summary>
        /// Edits for Feature Classes will be removed from the existing result tables.
        /// </summary>
        ExcludeAllFeatures,
        /// <summary>
        /// Edits will only be valid if the table they are on contains a modelname in the model name list
        /// </summary>
        OnlyModelNames,
        /// <summary>
        /// Edits will not be valid if they contain edits to tables with one of the model names
        /// </summary>
        ExcludeModelNames,
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
        /// No filters applied: all results will be returned no filters applied
        /// </summary>
        None
    }

    /// <summary>
    /// Enumeration of row (object) types.
    /// </summary>
    public enum RowTypes
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
    public struct DiffRow
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
        /// Creates a new <see cref="DiffRow"/> instance from the specified row.
        /// </summary>
        /// <param name="oid">The OID of the IRow</param>
        /// <param name="diffType">ype of the difference.</param>
        public DiffRow(int oid, esriDataChangeType diffType)
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
        public IRow GetIRow(IWorkspace workspace, DiffTable table)
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
            // return fws.OpenTable(table.TableName).GetRow(this.OID); //this ran out of memory, after 20,000 edits or so.
        }

        ///// <summary>
        ///// Indicates whether this instance and a specified object are equal.
        ///// </summary>
        ///// <param name="obj">Another object to compare to.</param>
        ///// <returns>
        ///// true if obj and this instance are the same type and represent the same value; otherwise, false.
        ///// </returns>
        //public override bool Equals(object obj)
        //{
        //    if (!(obj is DiffRow)) return false;
        //    DiffRow other = (DiffRow)obj;
        //    if (other.OID == OID) return true;
        //    return false;
        //}
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
    public class DiffTable : KeyedCollection<int,DiffRow>
    {
        #region Fields

        /// <summary>
        /// If the row is a feature class.
        /// </summary>
        public readonly bool IsFeatureClass;

        /// <summary>
        /// The row type.
        /// </summary>
        public readonly RowTypes RowType;

        /// <summary>
        /// The table name.
        /// </summary>
        public readonly string TableName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public DiffTable(ITable table)
        {
            IsFeatureClass = (table is IFeatureClass);
            TableName = ((IDataset)table).Name;
            RowType = RowTypes.Table;
            if (IsFeatureClass)
            {
                IFeatureClass fclass = (IFeatureClass)table;
                if (fclass.FeatureType == esriFeatureType.esriFTAnnotation || fclass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                    RowType = RowTypes.Annotation;
                else
                {
                    switch (fclass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryLine:
                        case esriGeometryType.esriGeometryPolyline:
                            RowType = RowTypes.Line;
                            break;
                        case esriGeometryType.esriGeometryMultipoint:
                        case esriGeometryType.esriGeometryPoint:
                            RowType = RowTypes.Point;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            RowType = RowTypes.Polygon;
                            break;
                        default:
                            RowType = RowTypes.Other;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="isFeatureClass">if set to <c>true</c> [is feature class].</param>
        /// <param name="rowType">Type of the row.</param>
        public DiffTable(string tableName, bool isFeatureClass, RowTypes rowType)
        {
            TableName = tableName;
            IsFeatureClass = isFeatureClass;
            RowType = rowType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the DiffRow, but only if it is unique.
        /// </summary>
        /// <param name="row">Row to Check and Add</param>
        public void AddUnique(DiffRow row)
        {
            if (!this.Contains(row.OID))
            {
                base.Add(row);
            }
        }

        /// <summary>
        /// Combines the rows from another DiffTable. Tables must match and all rows will be checked to insure only new values are added to existing list.
        /// </summary>
        /// <param name="table">The DiffTable containing the values to Check and Add</param>
        public void AddUnique(DiffTable table)
        {
            if (this.IsFeatureClass == table.IsFeatureClass &&
                this.RowType == table.RowType &&
                this.TableName == table.TableName)
            {
                foreach (DiffRow row in table)
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
                                "\nIsFeatureClass Equal: " + (this.IsFeatureClass == table.IsFeatureClass) + " , " +
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
        public IRow GetIRow(IWorkspace workspace, DiffRow row)
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
        /// Adds the DiffRow, but only if it is unique.
        /// </summary>
        /// <param name="row">Row to Check and Add</param>
        public new void Add(DiffRow row)
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
        protected override int GetKeyForItem(DiffRow item)
        {
            return item.OID;
        }

        #endregion
    }
}