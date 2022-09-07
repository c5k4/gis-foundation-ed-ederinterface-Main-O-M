using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;

using PGE.Desktop.EDER.AutoUpdaters;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("EB6119B2-67FB-4C14-996F-E8A094E3DA0B")]
    [ProgId("PGE.Desktop.EDER.ValidateRelatedRecordStatus")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateRelatedRecordStatus : BaseValidationRule
    {
        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedOrigin,
            SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedDestination,
            SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedOrigin,
            SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedDestination

        };

        private static readonly string _statusModelName = SchemaInfo.Electric.FieldModelNames.Status;

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateRelatedRecordStatus()
            : base("PGE Validate Related Record Status", _enabledModelNames)
        {
        }
        #endregion Constructors

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid. Errors are Mixed Statuses (Proposed-Install/In-Service) between the parent 
        /// and child tables, or in case of feature relationships (DeviceGroup or Strcuture featureclasses) the Parent must be 
        /// Inservice when they have a related child that's Inservice, Idle features are not validated or considered.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                if (row == null)
                {
                    _logger.Debug("Primary Line row was null when Validating Related Record Status.");
                    return _ErrorList;
                }

                IObject feature = (IObject)row;
                if (feature == null)
                {
                    _logger.Warn(string.Format("Primary Line {0} OID: {1} couldn't be converted to a Feature when Validating Related Record Status.", row.Table, row.OID));
                    return _ErrorList;
                }
                // if (StatusHelper.IsIdle(feature))
                // {
                //    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating Related Record Status.", feature.Class.AliasName, feature.OID));
                //    return _ErrorList;
                // }
                bool isOriginMatchStatus = ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedOrigin);
                bool isDestinationMatchStatus = ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedDestination);
                bool isOriginMatchInService = ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedOrigin);
                bool isDestinationMatchInService = ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedDestination);
                int featureStatusValue = feature.GetFieldValue(null, false, _statusModelName).Convert<int>(-1);
                string featureStatusDescription = feature.GetFieldValue(null, true, _statusModelName).Convert<string>(string.Empty);

                // If this is a Origin row and it must match it's status must match it's related child records.
                if (isOriginMatchStatus)
                {

                    // IEnumerable<IObject> relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedDestination);
                    // Don't use the extention, limit the modelnames to the 'other' role specified
                    // (e.g. OriginRole will only return Destination objects with modelname).
                    IEnumerable<IObject> relatedObjects = GetRelatedObjects(feature, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedDestination);

                    foreach (IObject obj in relatedObjects)
                    {

                        int relatedStatusValue = obj.GetFieldValue(null, false, _statusModelName).Convert(-1);

                        if ((!StatusHelper.IsIdle(featureStatusValue)) && (relatedStatusValue != featureStatusValue))
                        {
                            AddError(string.Format("The Status attribute must match between related records: {0} OID: {1}", obj.Class.AliasName, obj.OID));
                        }
                    }


                }
                // If this is a Destination row and it must match it's status must match it's related parent record.
                if (isDestinationMatchStatus)
                {

                    // IEnumerable<IObject> relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleDestination, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedOrigin);
                    IEnumerable<IObject> relatedObjects = GetRelatedObjects(feature, esriRelRole.esriRelRoleDestination, SchemaInfo.Electric.ClassModelNames.ValidateStatusRelatedOrigin);
                    foreach (IObject obj in relatedObjects)
                    {
                        int relatedStatusValue = obj.GetFieldValue(null, false, _statusModelName).Convert(-1);

                        if ((!StatusHelper.IsIdle(obj)) && (relatedStatusValue != featureStatusValue))
                        {
                            AddError(string.Format("The Status attribute must match between related records: {0} OID: {1}", obj.Class.AliasName, obj.OID));
                        }
                    }


                }
                // If this is a Origin row and it must match it's status must be In-Service if any of it's children are.
                if ((isOriginMatchInService) && (StatusHelper.IsProposed(feature)))
                {

                    // IEnumerable<IObject> relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedDestination);
                    IEnumerable<IObject> relatedObjects = GetRelatedObjects(feature, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedDestination);
                    foreach (IObject obj in relatedObjects)
                    {

                        if (StatusHelper.IsInService(obj))
                        {
                            string relatedStatusDescription = obj.GetFieldValue(null, true, _statusModelName).Convert<string>(string.Empty);
                            AddError(string.Format("{0} with {1} Status cannot have related records with {2} Status: {3} OID: {4}", feature.Class.AliasName, featureStatusDescription, relatedStatusDescription, obj.Class.AliasName, obj.OID));
                        }
                    }

                }

                // If this is a Destination row and it must match it's status must match it's related parent record.
                if ((isDestinationMatchInService) && (StatusHelper.IsInService(feature)))
                {

                    // IEnumerable<IObject> relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleDestination, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedOrigin);
                    IEnumerable<IObject> relatedObjects = GetRelatedObjects(feature, esriRelRole.esriRelRoleDestination, SchemaInfo.Electric.ClassModelNames.ValidateInserviceRelatedOrigin);
                    foreach (IObject obj in relatedObjects)
                    {
                        if (StatusHelper.IsProposed(obj))
                        {
                            int relatedStatusValue = obj.GetFieldValue(null, false, _statusModelName).Convert(-1);
                            string relatedStatusDescription = obj.GetFieldValue(null, true, _statusModelName).Convert<string>(string.Empty);
                            if ((!StatusHelper.IsIdle(obj)) && (relatedStatusValue != featureStatusValue))
                            {
                                AddError(string.Format("{0} with {1} Status cannot have a related parent feature with {2} Status: {3} OID: {4}", feature.Class.AliasName, featureStatusDescription, relatedStatusDescription, obj.Class.AliasName, obj.OID));
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while Validating Related Record Status rule.", ex);

            }

            return base.InternalIsValid(row);
        }

        #endregion Override for validation rule
        
        #region Private
        /// <summary>
        /// Gets the relationships that the object itself plays the specified role and filtered by class model name 
        /// for the opposite role.
        /// </summary>
        /// <param name="modelName"> Model Name of origin/destination object class. </param>
        /// <returns> IEnumerable of RelationshipClass. </returns>
        private IEnumerable<IRelationshipClass> GetRelationships(IObject feature, esriRelRole relationshipRole, params string[] modelNames)
        {
            IObjectClass selfClass = FeederManager2.GetObjectClassFromObject(feature);
            var objClass = selfClass;

            if (modelNames == null)
            {
                return objClass.RelationshipClasses[relationshipRole].AsEnumerable();
            }
            // If using an Origin Role, then we are looking for the modelnames only on the destination.
            if (relationshipRole == esriRelRole.esriRelRoleOrigin)
            {
                return objClass.RelationshipClasses[relationshipRole]
                           .AsEnumerable()
                           .Where(relationshipClass => ModelNameFacade.ContainsClassModelName(relationshipClass.DestinationClass, modelNames));
            }
            // If using an Desination Role, then we are looking for the modelnames only on the Origin.
            else if (relationshipRole == esriRelRole.esriRelRoleDestination)
            {
                return objClass.RelationshipClasses[relationshipRole]
                           .AsEnumerable()
                           .Where(relationshipClass => ModelNameFacade.ContainsClassModelName(relationshipClass.OriginClass, modelNames));
            }
            // If using AnyRole then look for the modelname on both the origin and the desination.
            return objClass.RelationshipClasses[relationshipRole]
                           .AsEnumerable()
                           .Where(relationshipClass => (ModelNameFacade.ContainsClassModelName(relationshipClass.OriginClass, modelNames))
                                        || (ModelNameFacade.ContainsClassModelName(relationshipClass.DestinationClass, modelNames)));
        }

        /// <summary>
        /// Gets objects related to the obj, optionally filtered by model name, alias, and/or role.
        /// </summary>
        /// <param name="obj">The instance the objects are related to.</param>
        /// <param name="modelName">The model name to filter by.</param>
        /// <param name="relationshipRole">The relationship role to filter by.</param>
        /// <returns>An enumeration of IObjects related to this instance.</returns>
        public IEnumerable<IObject> GetRelatedObjects(IObject obj, esriRelRole relationshipRole = esriRelRole.esriRelRoleAny, params string[] modelNames)
        {
            var relatedObjects = GetRelationships(obj, relationshipRole, modelNames)
                                     .Select(relationship => relationship.GetObjectsRelatedToObject(obj).AsEnumerable())
                                     .SelectMany(relSet => relSet);


            return relatedObjects;

        }
        #endregion Private

    }
}
