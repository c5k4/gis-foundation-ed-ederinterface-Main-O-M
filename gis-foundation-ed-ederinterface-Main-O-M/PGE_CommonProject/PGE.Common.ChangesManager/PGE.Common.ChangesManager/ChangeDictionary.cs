using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace PGE.ChangesManager
{
    #region Enumerations

    /// <summary>
    /// Type of valid Difference Dictionary Workspaces.
    /// </summary>
    public enum ChangeWorkspace
    {
        /// <summary>
        /// Primary Workspace
        /// </summary>
        Primary,
        /// <summary>
        /// Secondary Workspace
        /// </summary>
        Secondary,
        /// <summary>
        /// Common Ancestor Workspace
        /// </summary>
        CommonAncestor,
        /// <summary>
        /// Automatically decide based on the usePrimary flag.
        /// </summary>
        Auto
    }

    #endregion

    /// <summary>
    /// A dictionary mapping a list of features corresponding to the table name.
    /// </summary>
    [ComVisible(false),
    ClassInterface(ClassInterfaceType.None)]
    public class ChangeDictionary : Dictionary<string, ChangeTable>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeDictionary"/> class.
        /// </summary>
        /// <param name="differenceTypes">The difference types.</param>
        public ChangeDictionary(params esriDataChangeType[] differenceTypes)
        {
            this.DifferenceTypes = differenceTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeDictionary"/> class.
        /// </summary>
        /// <param name="primaryWorkspace">The primary workspace.</param>
        /// <param name="differenceTypes">The difference types.</param>
        public ChangeDictionary(IWorkspace primaryWorkspace, params esriDataChangeType[] differenceTypes)
            : this(differenceTypes)
        {
            this.PrimaryWorkspace = primaryWorkspace;
            this.UsePrimaryWorkspace = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeDictionary"/> class.
        /// </summary>
        /// <param name="primaryWorkspace">The primary workspace.</param>
        /// <param name="secondaryWorkspace">The secondary workspace.</param>
        /// <param name="differenceTypes">The difference types.</param>
        public ChangeDictionary(IWorkspace primaryWorkspace, IWorkspace secondaryWorkspace, params esriDataChangeType[] differenceTypes)
            : this(primaryWorkspace, differenceTypes)
        {
            this.SecondaryWorkspace = secondaryWorkspace;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeDictionary"/> class.
        /// </summary>
        /// <param name="primaryWorkspace">The primary workspace.</param>
        /// <param name="secondaryWorkspace">The secondary workspace.</param>
        /// <param name="commonAncestor">The common ancestor workspace</param>
        /// <param name="differenceTypes">The difference types.</param>
        public ChangeDictionary(IWorkspace primaryWorkspace, IWorkspace secondaryWorkspace, IWorkspace commonAncestor, params esriDataChangeType[] differenceTypes)
            : this(primaryWorkspace, secondaryWorkspace, differenceTypes)
        {
            this.CommonAncestorWorkspace = commonAncestor;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ChangeDictionary"/> is reclaimed by garbage collection.
        /// </summary>
        ~ChangeDictionary()
        {
            try
            {
                foreach (string table in this.Keys)
                {
                    foreach (ChangeRow info in this[table])
                    {
                        this[table].Remove(info);
                    }
                    this.Remove(table);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the difference types.
        /// </summary>
        /// <value>The difference types.</value>
        public esriDataChangeType[] DifferenceTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the <see cref="System.Collections.Generic.IEnumerable{T}" /> of <see cref="ESRI.ArcGIS.Geodatabase.ITable"/> as the Keys.
        /// </summary>
        /// <value></value>
        public IEnumerable<ITable> KeysAsTable
        {
            get
            {
                IFeatureWorkspace workpace = this.GetWorkspace();
                foreach (string tableName in Keys)
                    yield return workpace.OpenTable(tableName);
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Collections.Generic.List{T}"/> of <see cref="ChangeTable"/> with the specified key.
        /// </summary>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
        ///   </returns>
        ///   
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="key"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="key"/> does not exist in the collection.
        ///   </exception>
        public ChangeTable this[ITable key]
        {
            get
            {
                string tableName = ((IDataset)key).Name;
                return this[tableName];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use primary workspace.
        /// </summary>
        /// <value><c>true</c> if using primary workspace; otherwise, <c>false</c>.</value>
        public bool UsePrimaryWorkspace
        {
            get;
            set;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the Common Ancestor between primary and secondary workspaces.
        /// </summary>
        protected IWorkspace CommonAncestorWorkspace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the primary workspace.
        /// </summary>
        /// <value>The primary workspace.</value>
        protected IWorkspace PrimaryWorkspace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the secondary workspace.
        /// </summary>
        /// <value>The secondary workspace.</value>
        protected IWorkspace SecondaryWorkspace
        {
            get;
            set;
        }

        #endregion

        #region Public Methods


        public void Dispose()
        {

        }

        /// <summary>
        /// Adds the specified table to the dictionary.
        /// </summary>
        /// <param name="key">The table.</param>
        /// <param name="value">The rows.</param>
        public void Add(ITable key, ChangeTable value)
        {
            if (key != null)
            {
                string tableName = ((IDataset)key).Name;
                base.Add(tableName, value);
            }
            else
            {
                if (value != null)
                {
                    string message = "ITable key is null, unable to add to dictionary! \n Debug information:";
                    foreach (ChangeRow row in value)
                    {
                        message = message + "\n Failed to add " + BuildRowDebugMessageInfo(row);
                    }
                    message = message + "\n End Debug Information";
                    throw new Exception(message);
                }
                else
                {
                    throw new Exception("Both the ITable key and ChangeTable value are null, unable to add to dictionary!");
                }
            }
        }

        /// <summary>
        /// Adds the contents of the specified key to the dictionary, no unique test.
        /// </summary>
        /// <param name="other">Dictionary of new unique values to add</param>        
        public void Add(ChangeDictionary other)
        {
            foreach (string testKey in other.Keys)
            {
                if (this.Keys.Contains(testKey))
                {
                    foreach (ChangeRow row in other[testKey])
                    {
                        this[testKey].Add(row);
                    }
                }
                else //new key does not exist so just add the whole thing.
                {
                    base.Add(testKey, other[testKey]);
                }
            }
        }

        /// <summary>
        /// Adds the contents of the specified key to the dictionary, if unique.
        /// </summary>
        /// <param name="other">Dictionary of new unique values to add</param>        
        public void AddUnique(ChangeDictionary other)
        {
            foreach (string testKey in other.Keys)
            {
                if (this.Keys.Contains(testKey))
                {
                    this[testKey].AddUnique(other[testKey]);
                }
                else //new key does not exist so just add the whole thing.
                {
                    base.Add(testKey, other[testKey]);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ESRI.ArcGIS.Geodatabase.ITable"/> for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The ITable object matching the key specified</returns>
        public ITable AsTable(string key)
        {
            IFeatureWorkspace workpace = this.GetWorkspace();
            return workpace.OpenTable(key);
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the dictionary contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(ITable key)
        {
            string tableName = ((IDataset)key).Name;
            return base.ContainsKey(tableName);
        }

        /// <summary>
        /// Get the isOrigin or Destination objects related to the IRow passed in, which is from a relationship class table.
        /// If you need both call the function twice!
        /// </summary>
        /// <param name="tempRelClassClass">Initialized ESRI.ArcGIS.Geodatabase.IRelationshipClass to use</param>
        /// <param name="isOrigin">specifying <c>true</c> for the isOrigin objects or <c>false</c> for Destination objects are to be returned</param>
        /// <param name="relTableObject">The ESRI.ArcGIS.Geodatabase.IRow object to use</param>
        /// <returns></returns>
        public IRow GetObjectFromRelationshipTableValue(IRelationshipClass tempRelClassClass, bool isOrigin, IRow relTableObject)
        {
            IRow returnRow = null;
            ITable iTableToUse = null;
            int relColumnID = -1;
            string tabColName = null;
            bool isQuoted = false;
            if (isOrigin)
            {
                relColumnID = relTableObject.Table.Fields.FindField(tempRelClassClass.OriginForeignKey);
                iTableToUse = (tempRelClassClass.OriginClass as ITable);
                tabColName = tempRelClassClass.OriginPrimaryKey;
            }
            else
            {
                relColumnID = relTableObject.Table.Fields.FindField(tempRelClassClass.DestinationForeignKey);
                iTableToUse = (tempRelClassClass.DestinationClass as ITable);
                tabColName = tempRelClassClass.DestinationPrimaryKey;
            }
            object valueToFind = relTableObject.get_Value(relColumnID);
            IField fieldInfo = relTableObject.Fields.get_Field(relColumnID);
            if (fieldInfo.Type == esriFieldType.esriFieldTypeGlobalID || fieldInfo.Type == esriFieldType.esriFieldTypeGUID || fieldInfo.Type == esriFieldType.esriFieldTypeString || fieldInfo.Type == esriFieldType.esriFieldTypeXML)
            {
                isQuoted = true;
            }
            //create a query for the table
            IQueryFilter qFilter = new QueryFilterClass();
            if (isQuoted)
            {
                qFilter.WhereClause = " " + tabColName + " = '" + valueToFind.ToString() + "'";
            }
            else
            {
                qFilter.WhereClause = " " + tabColName + " = " + valueToFind.ToString() + " ";
            }
            ICursor result = iTableToUse.Search(qFilter, false);
            returnRow = result.NextRow();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(iTableToUse);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(result);
            return returnRow;
        }

        /// <summary>
        /// Gets the workspace which can be either the primary or secondary workspace depending the <see cref="P:UsePrimaryWorkspace"/> parameter.
        /// </summary>
        /// <returns>The <see cref="IFeatureWorkspace"/> specified in the constructor.</returns>
        public IWorkspace GetWorkspace(ChangeWorkspace workspaceToUse)
        {
            switch (workspaceToUse)
            {
                case ChangeWorkspace.Auto:
                    // If not set, then we figured out if the secondary workspace is available.
                    // If the secondary workspace is empty we will default to the primary workspace.
                    if (this.UsePrimaryWorkspace == false)
                    {
                        this.UsePrimaryWorkspace = (this.SecondaryWorkspace == null);
                    }

                    // Use the property value to determine which workspace to obtain.
                    if (this.UsePrimaryWorkspace)
                    {
                        if (this.PrimaryWorkspace == null) { throw new Exception("Neither the Primary Workspace nor the Secondary Workspace exist, unable to return either workspace."); }
                        return this.PrimaryWorkspace;
                    }

                    return this.SecondaryWorkspace;
                case ChangeWorkspace.Primary:
                    if (this.PrimaryWorkspace == null) { throw new Exception("Primary Workspace on Dictionary does not exist, yet was explicitely asked for."); }
                    return this.PrimaryWorkspace;
                case ChangeWorkspace.Secondary:
                    if (this.SecondaryWorkspace == null) { throw new Exception("Secondary Workspace on Dictionary does not exist, yet was explicitely asked for."); }
                    return this.SecondaryWorkspace;
                case ChangeWorkspace.CommonAncestor:
                    if (this.CommonAncestorWorkspace == null) { throw new Exception("Common Ancestor Workspace on Dictionary does not exist, yet was explicitely asked for."); }
                    return this.CommonAncestorWorkspace;
                default:
                    throw new Exception("Unable to match the passed in value to a Workspace Request.");
            }
        }

        /// <summary>
        /// Checks the key to see if it is empty and if not, if it is a feature class.
        /// </summary>
        /// <param name="key">Name of the Table/Key to check</param>
        /// <returns><c>false</c> if empty or not a feature class. <c>true</c> returned only if both a feature class and more than one row present.</returns>
        public bool IsFCAndNotEmpty(string key)
        {
            bool result = false;
            if (this[key].Count > 0)
            {
                if (this[key].IsFC)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Removes values from dictionary, if they exist, based on another dictionary source.
        /// </summary>
        /// <param name="other">The ChangesDictionary2 that lists values to be removed</param>        
        public void Remove(ChangeDictionary other)
        {
            foreach (string removeKey in other.Keys)
            {
                if (this.Keys.Contains(removeKey))
                {
                    foreach (ChangeRow item in other[removeKey])
                    {
                        if (this[removeKey].Contains(item))
                        {
                            this[removeKey].Remove(item);
                            if (this[removeKey].Count == 0)
                            {
                                this.Remove(removeKey);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the workspace which can be either the primary or secondary workspace depending the <see cref="P:UsePrimaryWorkspace"/> parameter.
        /// </summary>
        /// <returns>The <see cref="IFeatureWorkspace"/> specified in the constructor.</returns>
        protected IFeatureWorkspace GetWorkspace()
        {
            return (IFeatureWorkspace)this.GetWorkspace(ChangeWorkspace.Auto);
        }

        #endregion

        #region Private Methods

        private string BuildRowDebugMessageInfo(ChangeRow row)
        {
            string message = "Row contents: ";
            message = message + "\t OID: " + row.OID.ToString();
            message = message + "\t DifferenceType: " + row.DifferenceType.ToString();
            return message;
        }

        #endregion
    }
}