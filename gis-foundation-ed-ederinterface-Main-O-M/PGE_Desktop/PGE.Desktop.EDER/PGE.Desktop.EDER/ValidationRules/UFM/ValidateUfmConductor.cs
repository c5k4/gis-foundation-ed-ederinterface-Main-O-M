using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER;
using PGE.Desktop.EDER.UFM;

using PGE.Desktop.EDER.AutoUpdaters.LabelText;


namespace PGE.Desktop.EDER.ValidationRules.UFM
{
    [ComVisible(true)]
    [Guid("334D005D-A929-46b3-BC3B-9916BDC4F9F1")]
    [ProgId("PGE.Desktop.EDER.ValidateUfmConductor")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateUfmConductor : BaseValidationRule
    {
        #region Constants

        // For managing errors
        private const string    VALIDATION_ERROR = "Conductor must be placed into a duct";
        private const int       VALIDATION_TYPE_WARN = 1;
        
        #endregion

        #region Class Variables

        // For error handling
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        #endregion

        #region Constructor

         /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateUfmConductor() : base("PGE Validate UFM Conductor", 
                                                SchemaInfo.UFM.ClassModelNames.PrimaryConductor, 
                                                SchemaInfo.UFM.ClassModelNames.SecondaryConductor, 
                                                SchemaInfo.UFM.ClassModelNames.DcConductor)
        {

        }

        #endregion

        #region Base Validation Rule Overrides

        /// <summary>
        /// Returns an ID8List of validation violations for the supplied row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            // Log entry
            _logger.Debug("Validating UFM Conductor");
            if (row != null)
            {
                _logger.Debug("Class: " + (row as IObject).Class.AliasName);
                _logger.Debug("OID  : " + row.OID.ToString());
            }

            // If this rule is being filtered out - do not run it 
            if (ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, VALIDATION_TYPE_WARN) == true)
            {
                // If the conductor should be associated with a duct...
                if (IsUfmConductor(row) == true)
                {
                    // Check that it is
                    if (HasDuct(row) == false)
                    {
                        // And report an error if it isn't
                        AddError(VALIDATION_ERROR);
                    }
                }
            }

            // Return the resulting list
            return _ErrorList;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns true if the supplied conductor row is subject to UFM validation.
        /// 
        /// A conductor is subject to UFM validation if its a DC conductor or, in the case of pri UG or 
        /// sec UG, if its IsDirectBuried field is set to true
        /// </summary>
        /// <param name="conductor"></param>
        /// <returns></returns>
        private bool IsUfmConductor(IRow conductor)
        {
            // Log entry
            _logger.Debug("Entering IsUfmConductor");

            // Assume we don't need to validate
            bool isUfmConductor = false;

            try
            {
                // We'll need the class for the supplied row
                //IObjectClass oc = (conductor as IObject).Class;
                // If its a DC conductor
                //if (ModelNameFacade.ContainsClassModelName(oc, SchemaInfo.UFM.ClassModelNames.DcConductor) == true)
                //{
                //    // DC always requires validation
                //    isUfmConductor = true;
                //}
                //else
                //{
                //    // Otherwise, get the field index for the direct buried field                
                //    int fieldIndex = ModelNameFacade.FieldIndexFromModelName(oc, SchemaInfo.UFM.FieldModelNames.DirectBuriedIdc);

                //    // If we found it...
                //    if (fieldIndex > 0)
                //    {
                //        // Get its value
                //        object value = conductor.get_Value(fieldIndex);
                //        if (value != null)
                //        {
                //            // If its set, we need to validate
                //            if (value.ToString() == "N")
                //            {
                //                isUfmConductor = true;
                //            }
                //        }
                //    }
                //}

                //Change for INC000004026598 
                //A UFM Conductor is where there is a conductor which has one or more 
                //related conduits of subtype 'Duct Bank' 

                // Get the relationship class between the supplied conductor and the conduit system
                ISet relatedConduits = UfmHelper.GetRelatedObjects(conductor, SchemaInfo.UFM.ClassModelNames.Conduit);

                // If we found the relationship class...
                if (relatedConduits != null)
                {
                    // If there is at least one related conduit (could be more than one if phase split)
                    if (relatedConduits.Count > 0)
                    {
                        // Loop through all of the conduits related to the conductor 
                        relatedConduits.Reset();
                        IRow relatedConduit = (IRow)relatedConduits.Next();

                        while (relatedConduit != null)
                        {
                            // Determine if they are of subtype duct bank 
                            var subtype = ((IObject)relatedConduit).SubtypeCodeAsEnum<Subtype>();
                            if (subtype == Subtype.DuctBank)
                            {
                                isUfmConductor = true;
                                break;
                            }
                            relatedConduit = (IRow)relatedConduits.Next();
                        }
                    }
                }

                //Release the ISet 
                Marshal.FinalReleaseComObject(relatedConduits);
            }
            catch (Exception ex)
            {
                // Log and ignore
                _logger.Warn("Failed to test if conductor is UFM Conductor: " + ex.ToString());
            }

            // Return the result
            return isUfmConductor;
        }

        /// <summary>
        /// Returns true if all of the related conduits for the supplied supplied conductor 
        /// that are of subtype 'Duct Bank' have a valid duct position, otherwise it will 
        /// return false 
        /// </summary>
        /// <param name="conductor"></param>
        /// <returns></returns>
        private bool HasDuct(IRow conductor)
        {
            // Log entry
            _logger.Debug("Entering HasDuct");

            // Assume that all related conduits of subtype 'Duct Bank' have a duct position 
            // until we find one for which the condition it untrue 
            bool hasDuct = true;

            try
            {
                // Get the relationship class between the supplied conductor and the conduit system
                ISet relatedConduits = UfmHelper.GetRelatedObjects(conductor, SchemaInfo.UFM.ClassModelNames.Conduit);

                // If we found the relationship class...
                if (relatedConduits != null)
                {
                    // If there is at least one related conduit (could be more than one if phase split)
                    if (relatedConduits.Count > 0)
                    {
                        // Loop through all of the conduits related to the conductor 
                        relatedConduits.Reset();
                        IRow relatedConduit = (IRow)relatedConduits.Next();

                        while (relatedConduit != null)
                        {
                            // Determine if they are of subtype duct bank 
                            var subtype = ((IObject)relatedConduit).SubtypeCodeAsEnum<Subtype>();
                            if (subtype == Subtype.DuctBank)
                            {
                                //IT MUST HAVE A DUCT POS 

                                // Get the duct position
                                int ductPosition = GetDuctPosition(conductor, relatedConduit);

                                // If we were able to determine a duct position
                                if (ductPosition == -1)
                                {
                                    // Then we do not have a duct pos for a related 
                                    // conduit of subtype 'Duct Bank' and the rule is 
                                    //broken 
                                    hasDuct = false;
                                    break;
                                }
                            }
                            relatedConduit = (IRow)relatedConduits.Next();
                        }
                    }
                }

                Marshal.FinalReleaseComObject(relatedConduits);
            }
            catch (Exception ex)
            {
                // Log and ignore
                _logger.Warn("Failed to test if conductor has a duct: " + ex.ToString());
            }

            // Return the result
            return hasDuct;
        }

        /// <summary>
        /// Returns the duct position for the supplied conductor in the supplied conduit
        /// </summary>
        /// <param name="conductor"></param>
        /// <param name="relatedConduits"></param>
        /// <returns>The duct position or -1 if the duct position could not be determined</returns>
        private int GetDuctPosition(IRow conductor, IRow relatedConduit)
        {
            // Can't use model names on a relationship, so we'll call out field names here
            const string FIELD_CONDUCTOR_OID    = "UGOBJECTID";
            const string FIELD_CONDUIT_OID      = "ULSOBJECTID";
            const string FIELD_DUCT_POSITION    = "ULS_POSITION";

            // Log entry
            _logger.Debug("Entering GetDuctPosition");

            // Assume we can't find a position
            int ductPosition = -1;

            // We'll need a cursor and we'll need to clean it up
            ICursor cursor = null;

            try
            {
                // Get the attributed relationship table
                ITable table =
                    ModelNameFacade.RelationshipClassFromModelName(((IObject) conductor).Class,
                        esriRelRole.esriRelRoleAny, SchemaInfo.UFM.ClassModelNames.Conduit) as ITable;

                // Filter to the row we care about
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = FIELD_CONDUCTOR_OID + "=" + conductor.OID.ToString() + " AND " + FIELD_CONDUIT_OID + "=" + relatedConduit.OID;
                cursor = table.Search(qf, false);
                IRow relRow = cursor.NextRow();

                // Get the duct position
                object val = relRow.get_Value(table.FindField(FIELD_DUCT_POSITION));
                if (val != null)
                {
                    ductPosition = Convert.ToInt32(val);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to get duct position: " + ex.ToString());
            }
            finally
            {
                Marshal.ReleaseComObject(cursor);
            }

            // Return the result
            return ductPosition;
        }

        #endregion
    }
}
