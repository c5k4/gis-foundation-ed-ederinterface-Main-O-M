using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Comm = PGE.Interfaces.CCBServicePointUpdater.Common;
namespace PGE.Interfaces.CCBServicePointUpdater
{
    /// <summary>
    /// Class to host SQL query wrappers
    /// </summary>
    static class SQLTableInterface
    {
        /// <summary>
        /// Method that queries a table in a given workspace.
        /// SELECT /columnsToGet/ FROM /table/ WHERE /whereColumn in (whereValue1,whereValue2,etc..)/
        /// </summary>
        /// <param name="featWksp">Workspace in which the table is located</param>
        /// <param name="table">The table that is being queried</param>
        /// <param name="columnsToGet">Columns to retrieve</param>
        /// <param name="whereColumn">Which column to filter on</param>
        /// <param name="whereValue">What values of the above column to filter</param>
        /// <returns>A Cursor to the results of the query. Traverse using cursorName.NextRow()</returns>
        public static ICursor GetQueryResultsWhereInClause(IFeatureWorkspace featWksp, string table, string columnsToGet, string whereColumn, string whereValue)
        {
            ITable tbl;
            IQueryFilter q = GetQueryFilterWhereInClause(columnsToGet, whereColumn, whereValue);

            try
            {
                tbl = featWksp.OpenTable(table);
                ICursor cursor = tbl.Search(q, true);
                return cursor;
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine("Where In clause, requesting rows with " + whereColumn + "=" + whereValue);
                Comm.LogManager.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Method that queries a table in a given workspace.
        /// SELECT /columnsToGet/ FROM /table/ WHERE /whereColumn/=/whereValue/
        /// </summary>
        /// <param name="featWksp">Workspace in which the table is located</param>
        /// <param name="table">The table that is being queried</param>
        /// <param name="columnsToGet">Columns to retrieve</param>
        /// <param name="whereColumn">Which column to filter on</param>
        /// <param name="whereValue">What values of the above column to filter</param>
        /// <returns>A Cursor to the results of the query. Traverse using cursorName.NextRow()</returns>
        public static ICursor GetQueryResultsWhereClause(IFeatureWorkspace featWksp, string table, string columnsToGet, string whereColumn, string whereValue)
        {
            ITable tbl;
            IQueryFilter q = GetQueryFilterWhereClause(columnsToGet, whereColumn, whereValue);

            try
            {
                tbl = featWksp.OpenTable(table);
                ICursor cursor = tbl.Search(q, true);
                return cursor;
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine("Where clause, requesting rows with " + whereColumn + "=" + whereValue);
                Comm.LogManager.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Method that queries a table in a given workspace. Expects a single row to return so will throw an exception if more than one row is found
        /// SELECT /columnsToGet/ FROM /table/ WHERE /whereColumn/=/whereValue/
        /// </summary>
        /// <param name="featWksp">Workspace in which the table is located</param>
        /// <param name="table">The table that is being queried</param>
        /// <param name="columnsToGet">Columns to retrieve</param>
        /// <param name="whereColumn">Which column to filter on</param>
        /// <param name="whereValue">What values of the above column to filter</param>
        /// <returns>A single row that the query returned. If there are multiple rows returned by the query, this method will return Null</returns>
        public static IRow GetRowResultWhereClause(IFeatureWorkspace featWksp, string table, string columnsToGet, string whereColumn, string whereValue)
        {
            IRow singleResult;
            ICursor results = GetQueryResultsWhereClause(featWksp, table, columnsToGet, whereColumn, whereValue);

            singleResult = results.NextRow();

            if (results.NextRow() != null)
            {
                return null;
            }

            return singleResult;
        }

        /// <summary>
        /// Returns the query filter used to do a search without executing the query
        /// </summary>
        /// <param name="columnsToGet">Which columns from the table</param>
        /// <param name="whereColumn">the column to filter on</param>
        /// <param name="whereValue">The values of the column specified in whereColumn to filter on</param>
        /// <returns>A filter that can be executed on a table to get the pre-configured query's results</returns>
        public static IQueryFilter GetQueryFilterWhereClause(string columnsToGet, string whereColumn, string whereValue)
        {
            // Create the query filter.
            IQueryFilter queryFilter = new QueryFilter();

            // Select the fields to be returned
            queryFilter.SubFields = columnsToGet;

            // Set the filter to return only restaurants.
            queryFilter.WhereClause = whereColumn + "='" + whereValue.ToUpper() + "'";

            return queryFilter;
        }

        public static IQueryFilter GetQueryFilterWhereInClause(string columnsToGet, string whereColumn, string whereValue)
        {
            // Create the query filter.
            IQueryFilter queryFilter = new QueryFilter();

            // Select the fields to be returned
            queryFilter.SubFields = columnsToGet;

            // Set the filter to return only restaurants.
            queryFilter.WhereClause = whereColumn + " IN (" + whereValue.ToUpper() + ")";

            return queryFilter;
        }

    }

    
}
