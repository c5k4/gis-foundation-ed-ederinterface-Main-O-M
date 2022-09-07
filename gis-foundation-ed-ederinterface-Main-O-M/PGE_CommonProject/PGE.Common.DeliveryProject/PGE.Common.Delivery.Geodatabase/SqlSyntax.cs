using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Systems;

namespace PGE.Common.Delivery.Geodatabase
{
    #region Enumerations

    /// <summary>
    /// Enumeration of the various supported geodatabases.
    /// </summary>
    public enum SpatialDataEngine
    {
        /// <summary>
        /// A local Microsoft Access geodatabase.
        /// </summary>
        Access,
        /// <summary>
        /// A local ESR File geodatabase.
        /// </summary>
        File,
        /// <summary>
        /// A remote Oracle geodatabase.
        /// </summary>
        Oracle,
        /// <summary>
        /// A remote Microsoft SQL Server geodatabase.
        /// </summary>
        SqlServer
    }

    #endregion

    /// <summary>
    /// A supporting class used to handle SQL syntax specific to a workspace.
    /// </summary>
    public class SqlSyntax
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSyntax"/> class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public SqlSyntax(IWorkspace workspace)
        {
            this.Workspace = workspace;
            this.Syntax = (ISQLSyntax)workspace;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public List<string> Keywords
        {
            get
            {
                List<string> keywords = new List<string>();
                IEnumBSTR bstr = this.Syntax.GetKeywords();
                bstr.Reset();

                string s;
                while ((s = bstr.Next()) != null)
                {
                    keywords.Add(s);
                }

                return keywords;
            }
        }

        /// <summary>
        /// Gets or sets the type of the geodatabase
        /// </summary>
        /// <value>The type.</value>
        public SpatialDataEngine Type
        {
            get
            {
                // Determine the type of spatial data engine for the workspace.
                if (this.Workspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
                {
                    UID uid = this.Workspace.WorkspaceFactory.GetClassID();
                    if (uid.Value.ToString() == "{71FE75F0-EA0C-4406-873E-B7D53748AE7E}")
                    {
                        return SpatialDataEngine.File;
                    }

                    return SpatialDataEngine.Access;
                }

                object names;
                object values;

                // Use the connection properties to determine which remote workspace is being used.
                this.Workspace.ConnectionProperties.GetAllProperties(out names, out values);

                List<string> array = new List<string>(((object[])names).Cast<string>());
                if (!array.Contains("DATABASE"))
                {
                    return SpatialDataEngine.Oracle;
                }
                else
                {
                    string database = TypeCastFacade.Cast(this.Workspace.ConnectionProperties.GetProperty("DATABASE"), string.Empty);
                    if (string.IsNullOrEmpty(database))
                        return SpatialDataEngine.Oracle;
                }

                return SpatialDataEngine.SqlServer;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the syntax.
        /// </summary>
        /// <value>The syntax.</value>
        protected ISQLSyntax Syntax
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the workspace.
        /// </summary>
        /// <value>The workspace.</value>
        protected IWorkspace Workspace
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handle escaping quote characters by use two quotes for every one displayed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The escaped quoted string.</returns>
        public string Escape(string value)
        {
            // Handle escaping quote characters by use two quotes for every one displayed.
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("'", "''");
        }

        /// <summary>
        /// Gets the formatted date time for the current workspace.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The formatted date time.</returns>
        public string GetFormattedDate(DateTime dateTime)
        {
            // Depending on the workspace we need to format the date time differently.
            switch (this.Type)
            {
                case SpatialDataEngine.Access:
                    // Access - #3/11/2005#
                    return string.Format("#{0}#", dateTime.ToShortDateString());

                case SpatialDataEngine.File:
                    // FileGeodatabase - date '3/11/2005'
                    return string.Format("date '{0}'", dateTime.ToShortDateString());

                case SpatialDataEngine.Oracle:
                    // Oracle - 01-NOV-05
                    return string.Format("{0}", dateTime.ToString("dd-MMM-yy"));

                case SpatialDataEngine.SqlServer:
                    // SqlServer - '3/11/2005'
                    return string.Format("{0}", dateTime.ToShortDateString());
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the SQL reserved function name for specified enumeration value.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>The function name for the workspace.</returns>
        public string GetFunction(esriSQLFunctionName functionName)
        {
            return this.Syntax.GetFunctionName(functionName);
        }

        /// <summary>
        /// Gets the special character for the specified enumeration value.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>The special character for the workspace.</returns>
        public string GetSpecialCharacter(esriSQLSpecialCharacters character)
        {
            return this.Syntax.GetSpecialCharacter(character);
        }

        /// <summary>
        /// Determines whether the specified predicate is supported.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified predicate is supported; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSupported(esriSQLPredicates predicate)
        {
            // Use a bitwise AND to check if the predicate is supported.
            int supportedPredicates = this.Syntax.GetSupportedPredicates();
            int predicateCheck = supportedPredicates & (int)predicate;

            // If the result of a bitwise AND is greater than 0, the predicate is supported.
            return (predicateCheck > 0);
        }

        #endregion
    }
}