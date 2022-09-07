using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PGE.BatchApplication.AUConveyor.Autoupdaters;
using PGE.BatchApplication.AUConveyor.Processing.Annotation;
using PGE.BatchApplication.AUConveyor.Utilities;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;

namespace PGE.BatchApplication.AUConveyor.Processing
{
    /// <summary>
    /// Logic for processing object classes during execution.
    /// </summary>
    internal class QueuedObjectClass
    {
        #region Private Members
        IObjectClass _objectClass = null;
        List<IRelationshipClass> _eligibleRelationships;
        List<IRelationshipClass> _recreateCapableRelationships;
        List<int> _eligibleSubtypes;
        string _subtypeSqlList;
        bool _eligibleAuFound;
        #endregion

        #region Properties
        /// <summary>
        /// The object class for processing.
        /// </summary>
        internal IObjectClass ObjectClass
        {
            get { return _objectClass; }
            set { _objectClass = value; }
        }

        /// <summary>
        /// Used to indicate in processing whether or not an AU needs to be fired on the object class.
        /// If <c>false</c>, the execution will occur in an "update-related only" format.
        /// If <c>true</c>, related objects could still be updated depending on the configuration but only if the AU isn't
        ///     configured for a subtype.
        /// </summary>
        internal bool EligibleAuFound
        {
            get { return _eligibleAuFound; }
            set { _eligibleAuFound = value; }
        }

        /// <summary>
        /// The relationships to update, if necessary.
        /// </summary>
        internal List<IRelationshipClass> EligibleRelationships
        {
            get { return _eligibleRelationships; }
            set { _eligibleRelationships = value; }
        }

        /// <summary>
        /// The subtypes in which the AU (if specified) is enabled. Empty lists indicate that ALL subtypes are used.
        /// </summary>
        internal List<int> EligibleSubtypes
        {
            get { return _eligibleSubtypes; }
            set { _eligibleSubtypes = value; }
        }

        /// <summary>
        /// A comma-separated list of subtypes if necessary.
        /// </summary>
        internal string SubtypeSqlList
        {
            get { return _subtypeSqlList; }
            set { _subtypeSqlList = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentOC">The object class for processing.</param>
        /// <param name="eligibleAuFound">Whether or not a configured AU was found for at least one subtype of this class.</param>
        internal QueuedObjectClass(IObjectClass currentOC, bool eligibleAuFound)
        {
            _objectClass = currentOC;
            _eligibleAuFound = eligibleAuFound;
            GetEligibleClasses();
            _eligibleSubtypes = new List<int>();
            _subtypeSqlList = string.Empty;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Executes the desired logic within the current object class, conveying the AU and performing
        /// any additional configured tasks.
        /// </summary>
        /// <param name="gisEditor">A reference to the editor object currently being used for these edits.</param>
        /// <param name="auWrapper">The wrapper class used for instantiating, casting, and executing the AU.</param>
        /// <param name="auHelper">The class containing information the AU relative to the current process.</param>
        /// <param name="whereClauses">Any amount of SQL WHERE clauses used for query filters. Each clause will be executed.</param>
        internal void ProcessClass(ref GISEditor gisEditor, AUWrapper auWrapper, AUHelper auHelper, List<string> whereClauses)
        {
            if (_eligibleAuFound)
            {
                LogManager.WriteLine("    Executing AU on features within the class \"" + _objectClass.AliasName + "\".");
                _eligibleSubtypes = auHelper.ConfiguredClasses.Where(kvp => kvp.Key.ObjectClassID == _objectClass.ObjectClassID).Select(kvp => kvp.Value).ToList();
                _subtypeSqlList = string.Join(",", _eligibleSubtypes.OrderBy(i => i).Select(i => i.ToString()).Distinct().ToArray());
                LogManager.WriteLine("      Configured class subtypes for this AU: " + _subtypeSqlList);
            }
            else
            {
                LogManager.WriteLine("    Updating related objects within the class \"" + _objectClass.AliasName + "\".");
            }

            //No need to loop through things if we're forcing updates and there are no eligible relationships.
            if (!_eligibleAuFound && _eligibleRelationships.Count == 0)
                throw new Exception("No relationships were found to update.");

            IQueryFilter qf = null;
            ICursor pCursor = null;
            try
            {
                //Loop through all features.
                qf = new QueryFilterClass();
                AnnotationManipulation.AnnoErrors = 0;
                int processedcnt = 0;
                int errorcnt = 0;
                int subquerycnt = 0;
                if (whereClauses.Count == 0)
                    whereClauses.Add("");
                foreach (string whereClause in whereClauses)
                {
                    LogManager.WriteLine("      Processing subquery " + ++subquerycnt + " of " + whereClauses.Count + ".");

                    //Add in custom where clause or Input File Reading
                    if (!string.IsNullOrEmpty(whereClause))
                        qf.WhereClause = whereClause;
                    //If the eligible subtype list is empty, everything works. But don't restrict the query if we want to update related anyway.
                    if (string.IsNullOrEmpty(ToolSettings.Instance.InputFile) && !ToolSettings.Instance.UpdateRelatedIfAuNotFound && _eligibleSubtypes.Where(st => st > 0).Count() > 0)
                        qf.WhereClause += (qf.WhereClause.Length > 0 ? " AND " : "") + (_objectClass as ISubtypes).SubtypeFieldName + " IN (" + _subtypeSqlList + ")";
                    int rowCnt = (_objectClass as ITable).RowCount(qf);
                    pCursor = (_objectClass as ITable).Search(qf, false);

                    LogManager.WriteLine("        Found " + rowCnt + " rows in this query for \"" + _objectClass.AliasName + "\".");
                    LogManager.ConsoleLogger.Write("        Rows processed: " + processedcnt);

                    for (IObject obj = pCursor.NextRow() as IObject; obj != null; obj = pCursor.NextRow() as IObject)
                    {
                        bool subtypeEnabled = (_eligibleSubtypes.Where(st => st > 0).Count() == 0 || ((_objectClass as ISubtypes) != null && _eligibleSubtypes.Contains(Convert.ToInt32(obj.get_Value((_objectClass as ISubtypes).SubtypeFieldIndex)))));

                        try
                        {
                            //Delete specified related features if configured.
                            Dictionary<int, AnnotationStore> annoStoreList = new Dictionary<int, AnnotationStore>();
                            bool recreateAllowed = RecreateEnabled(obj);
                            if (recreateAllowed)
                                DeleteRelated(obj, ref annoStoreList);

                            //If the store is forced, we know that the AU isn't enabled for the object.
                            //If the AU IS enabled, call the Execute() method on the AU.
                            if (_eligibleAuFound && subtypeEnabled)
                            {
                                auWrapper.Execute(obj, mmAutoUpdaterMode.mmAUMStandAlone, mmEditEvent.mmEventFeatureUpdate);
                                if (AuStoreAllowed(obj))
                                    obj.Store();
                            }
                            else
                            {
                                //The AU isn't enabled and the execution indicated to continue, so let's tell the related objects that their parent has changed.
                                foreach (IRelationshipClass rel in _eligibleRelationships)
                                {
                                    ISet relObjs = rel.GetObjectsRelatedToObject(obj);
                                    for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                                        (relObj as IRelatedObjectEvents).RelatedObjectChanged(rel, obj);
                                }
                            }

                            //Recreate specified related features if configured.
                            if (recreateAllowed)
                                CreateRelated(obj, ref annoStoreList);
                        }
                        catch(Exception e)
                        {
                            //Only write detailed error message to the file listener.
                            LogManager.FileLogger.IndentLevel++;
                            LogManager.FileLogger.WriteLine(e.ToString());
                            LogManager.FileLogger.IndentLevel--;

                            errorcnt++;
                            processedcnt--;
                        }

                        processedcnt++;

                        //Save after a certain number of rows (based on settings)
                        if (processedcnt % ToolSettings.Instance.SaveRowInterval == 0)
                        {
                            LogManager.ConsoleLogger.Write("\r");
                            LogManager.Write("        " + processedcnt + " rows processed for  \"" + _objectClass.AliasName + "\", saving edits.");
                            gisEditor.RestartEditing(true);
                            LogManager.Write("✓", ConsoleColor.Green, null);
                            LogManager.WriteLine();
                        }

                        LogManager.ConsoleLogger.Write("\r        Rows processed: " + processedcnt);

                        //Release the object obtained from the cursor.
                        Marshal.ReleaseComObject(obj);
                    }

                    LogManager.ConsoleLogger.Write("\r                                            ");
                    if (pCursor != null) while (Marshal.ReleaseComObject(pCursor) > 0) { }
                    pCursor = null;
                }
                LogManager.WriteLine();
                LogManager.WriteLine("        Successfully processed " + processedcnt + " rows.");
                if (errorcnt > 0)
                    Program.WriteWarning("**Warning: Errors were encountered in " + errorcnt + " rows.");
                if (AnnotationManipulation.AnnoErrors > 0)
                    Program.WriteWarning("**Warning: " + AnnotationManipulation.AnnoErrors + " annotation errors were encountered.");
            }
            finally
            {
                LogManager.WriteLine();
                if (qf != null) while (Marshal.ReleaseComObject(qf) > 0) { }
                if (pCursor != null) while (Marshal.ReleaseComObject(pCursor) > 0) { }
            }
        }

        /// <summary>
        /// Fills the <para>featureLoad</para> list with information indicating the objects that
        /// will be written to each input file.
        /// </summary>
        /// <param name="featureLoad">
        ///     A list of class and object ID information which will indicate the objects that
        ///     will be written to each input file.
        /// </param>
        /// <param name="auWrapper">The wrapper class used for instantiating, casting, and executing the AU.</param>
        /// <param name="auHelper">The class containing information the AU relative to the current process.</param>
        internal void SplitLoad(ref List<ProcessOIDInfo> featureLoad, AUWrapper auWrapper, AUHelper auHelper)
        {
            if (_eligibleAuFound)
            {
                _eligibleSubtypes = auHelper.ConfiguredClasses.Where(kvp => kvp.Key.ObjectClassID == _objectClass.ObjectClassID).Select(kvp => kvp.Value).ToList();
                _subtypeSqlList = string.Join(",", _eligibleSubtypes.Select(i => i.ToString()).Distinct().ToArray());
            }

            //No need to loop through things if we're forcing updates and there are no eligible relationships.
            if (!_eligibleAuFound && _eligibleRelationships.Count == 0)
                return;

            IQueryFilter qf = null;
            ICursor pCursor = null;
            try
            {
                //Loop through all features.
                qf = new QueryFilterClass();
                //Add in custom where clause
                if (!string.IsNullOrEmpty(ToolSettings.Instance.CustomWhereClause))
                    qf.WhereClause = ToolSettings.Instance.CustomWhereClause;
                //If the eligible subtype list is empty, everything works. But don't restrict the query if we want to update related anyway.
                if (!ToolSettings.Instance.UpdateRelatedIfAuNotFound && _eligibleSubtypes.Where(st => st > 0).Count() > 0)
                    qf.WhereClause += (qf.WhereClause.Length > 0 ? " AND " : "") + (_objectClass as ISubtypes).SubtypeFieldName + " IN (" + _subtypeSqlList + ")";
                int rowCnt = (_objectClass as ITable).RowCount(qf);
                pCursor = (_objectClass as ITable).Search(qf, false);

                LogManager.WriteLine("    Recording " + rowCnt + " rows for processes for \"" + _objectClass.AliasName + "\".");
                
                int classID = _objectClass.ObjectClassID;
                for (IObject o = pCursor.NextRow() as IObject; o != null; o = pCursor.NextRow() as IObject)
                {
                    featureLoad.Add(new ProcessOIDInfo(o.OID, classID));

                    //Release the object obtained from the cursor.
                    Marshal.ReleaseComObject(o);
                }
            }
            finally
            {
                if (qf != null) while (Marshal.ReleaseComObject(qf) > 0) { }
                if (pCursor != null) while (Marshal.ReleaseComObject(pCursor) > 0) { }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the eligible relationship classes on which to force an update based on the execution settings.
        /// </summary>
        private void GetEligibleClasses()
        {
            _eligibleRelationships = new List<IRelationshipClass>();

            if (ToolSettings.Instance.UpdateRelatedIfAuNotFound)
            {
                IEnumRelationshipClass relClasses = _objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                for (IRelationshipClass rel = relClasses.Next(); rel != null; rel = relClasses.Next())
                {
                    if (ToolSettings.Instance.ForceType.HasValue)
                    {
                        if ((rel.DestinationClass.ObjectClassID == _objectClass.ObjectClassID && (rel.OriginClass as IFeatureClass != null) && (rel.OriginClass as IFeatureClass).FeatureType == ToolSettings.Instance.ForceType) ||
                            (rel.OriginClass.ObjectClassID == _objectClass.ObjectClassID && (rel.DestinationClass as IFeatureClass != null) && (rel.DestinationClass as IFeatureClass).FeatureType == ToolSettings.Instance.ForceType))
                            _eligibleRelationships.Add(rel);
                    }
                    else if (ToolSettings.Instance.ForceClassNames.Count > 0)
                    {
                        foreach (string className in ToolSettings.Instance.ForceClassNames)
                        {
                            if ((rel.OriginClass.ObjectClassID == _objectClass.ObjectClassID && className.ToUpper() == (rel.DestinationClass as IDataset).Name.ToUpper())
                                || (rel.DestinationClass.ObjectClassID == _objectClass.ObjectClassID && className.ToUpper() == (rel.OriginClass as IDataset).Name.ToUpper()))
                            {
                                _eligibleRelationships.Add(rel);
                                break;
                            }
                        }
                    }
                    else
                        _eligibleRelationships.Add(rel);
                }

                //Ensure that extra -k values aren't passed in that weren't found. Output a warning if so.
                if (_eligibleRelationships.Count < ToolSettings.Instance.ForceClassNames.Count)
                {
                    Program.WriteWarning("More 'forceClass' flags were passed than eligible relationships were found. This may be in error - check the spelling of the configured flags if necessary.");
                    Program.WaitIfNotSeamless();
                }
            }

            //Now find out which relationship classes are configured for recreation.
            _recreateCapableRelationships = new List<IRelationshipClass>();
            foreach (IRelationshipClass relClass in _eligibleRelationships)
            {
                //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
                IRelatedObjectClassEvents originEvents = relClass.OriginClass.Extension as IRelatedObjectClassEvents;
                IRelatedObjectClassEvents destEvents = relClass.DestinationClass.Extension as IRelatedObjectClassEvents;

                if (destEvents != null || originEvents != null)
                {
                    _recreateCapableRelationships.Add(relClass);
                }
            }
        }

        /// <summary>
        /// Ensures that the proper tool settings are set to allow a feature store. If the "bypass" setting is set, this
        /// method loops through all data changes and checks whether or not a field (SHAPE fields excepted) has been modified.
        /// </summary>
        /// <param name="parentObject">The object used as the parent.</param>
        /// <returns><c>true</c> if the parent object should be stored, otherwise <c>false</c>.</returns>
        private bool AuStoreAllowed(IObject parentObject)
        {
            if (!ToolSettings.Instance.BypassAuStoreWhenFieldsEqual)
                return true;

            IRowChanges changes = parentObject as IRowChanges;

            for (int i = 0; i < parentObject.Fields.FieldCount; i++)
            {
                if (parentObject.Fields.get_Field(i).Name.ToUpper() != "SHAPE" && changes.get_ValueChanged(i))
                    return true;
            }

            return false;
        }


        #region Deletion/Recreation of Related Objects

        /// <summary>
        /// Ensures that the proper tool settings are set to allow recreation; additionally, if a "recreate only if placed"
        /// setting is enabled, loops through related objects of a specific object during processing and checks to see if 
        /// any object was placed at all (returning <c>true</c> only if a placed object was found).
        /// </summary>
        /// <param name="parentObject">The object used as the parent.</param>
        /// <returns><c>true</c> if related objects should be deleted and recreated, otherwise <c>false</c>.</returns>
        private bool RecreateEnabled(IObject parentObject)
        {
            if (!ToolSettings.Instance.RecreateRelated)
                return false;

            if (!ToolSettings.Instance.RecreateOnlyWhenExists)
                return true;

            foreach (IRelationshipClass rel in _recreateCapableRelationships)
            {
                ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                {
                    if (relObj is IFeature)
                    {
                        if ((relObj as IFeature).Shape != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Should only be run if the <see cref="RecreateEnabled"/> method returns <c>true</c>.
        /// Deletes related objects, and stores annotation details where necessary.
        /// Use <see cref="CreateRelated"/> to recreate the objects.
        /// <param name="parentObject">The object used as the parent.</param>
        /// <param name="annoStoreList">This list will be populated with annotation details from the deleted features.</param>
        /// </summary>
        private void DeleteRelated(IObject parentObject, ref Dictionary<int, AnnotationStore> annoStoreList)
        {
            //Loop through and delete.
            foreach (IRelationshipClass rel in _eligibleRelationships)
            {
                annoStoreList.Add(rel.RelationshipClassID, new AnnotationStore(rel, parentObject));

                //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
                IRelatedObjectClassEvents originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
                IRelatedObjectClassEvents destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

                if (destEvents != null || originEvents != null)
                {
                    ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                    for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                    {
                        annoStoreList[rel.RelationshipClassID].AddIfAnno(relObj);

                        relObj.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Should only be run if the <see cref="RecreateEnabled"/> method returns <c>true</c>.
        /// Typically requires <see cref="DeleteRelated"/> being run through first, but not necessary.
        /// Uses Esri events to create related objects. The objects must normally be configured to be created when
        /// the feature is first created.
        /// <param name="parentObject">The object used as the parent.</param>
        /// <param name="annoStoreList">A list will populated with annotation details from the deleted features.</param>
        /// </summary>
        private void CreateRelated(IObject parentObject, ref Dictionary<int, AnnotationStore> annoStoreList)
        {
            foreach (IRelationshipClass rel in _recreateCapableRelationships)
            {
                IRelatedObjectClassEvents originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
                IRelatedObjectClassEvents destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

                if (destEvents != null)
                    destEvents.RelatedObjectCreated(rel, parentObject);

                if (originEvents != null)
                    originEvents.RelatedObjectCreated(rel, parentObject);

                if (annoStoreList != null && annoStoreList.ContainsKey(rel.RelationshipClassID))
                    annoStoreList[rel.RelationshipClassID].ApplySettingsToNewAnno();
            }
        }

        #endregion

        #endregion
    }
}
