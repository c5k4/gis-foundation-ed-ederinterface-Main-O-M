using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using log4net;

using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER
{

    /// <summary>
    /// Contains the business logic for validating the connected conductors at a closed device
    /// </summary>
    internal class ValidateClosedDevice
    {
        #region Private Variables

        /// <summary>
        /// Logger to log the error/info
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Closed status of Normal Position field
        /// </summary>
        private const int NormalStatusClosed = 1;

        #endregion Private Variables

        #region Business Logic Methods

        /// <summary>
        /// Validates the object for specific fields
        /// </summary>
        /// <param name="obj">Object/DeviceFeature to validate</param>
        /// <param name="errorMessage">Error message to be displayed to the user if the object turns out to be invalid</param>
        /// <returns>Returns true if the object is valid; false otherwise</returns>
        public static bool IsValid(IObject obj, out string errorMessage)
        {
            bool valid = false;
            errorMessage = string.Empty;

            //Validate the Junction Feature
            ISimpleJunctionFeature deviceJuncFeature = obj as ISimpleJunctionFeature;
            if (deviceJuncFeature == null)
            {
                _logger.Debug("The input Object is not a Junction Feature."); return true;
            }

            //Get the number of lines connecting the junction feature
            if (deviceJuncFeature.EdgeFeatureCount < 2) return true;

            //Get the Conductor count
            int[] conductorEdgeIndexs = GetConductorsAtJunction(deviceJuncFeature);
            //Skip the validation if alteast two conductors are not connected by the switch
            if (conductorEdgeIndexs.Length < 2) return valid = true;
            //Throw error and stop edit task if more than 2 conductors are connected
            if (conductorEdgeIndexs.Length > 2)
            {
                errorMessage = "There should be only two conductors connected to a Switchable Device.";
                return valid = false;
            }

            //Proceed only if two conductors exist

            //Get the field values of both the conductors and validate for equality
            string invalidFields = string.Empty;
            string missingFields = string.Empty;
            string invalidConfigError = "Contact Administrator. Not found fields with the following model names: \r {0} on {1}.";
            string valueMismatchError = "The values on {0} (OID: {1}) do not match with {2} (OID: {3}) on the following fields: {4}.";

            //Get the Features connecting the Switch
            IFeature upFeature = deviceJuncFeature.get_EdgeFeature(conductorEdgeIndexs[0]) as IFeature;
            IFeature downFeature = deviceJuncFeature.get_EdgeFeature(conductorEdgeIndexs[1]) as IFeature;
            //string missingFields = "Not found a field assigned with {0} model name in either '" + upFeature.Class.AliasName + "' or '" + downFeature.Class.AliasName + "'";

            //Validate the NumberOfPhases field
            int upFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(upFeature.Class, SchemaInfo.Electric.FieldModelNames.NumberOfPhases);
            int downFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(downFeature.Class, SchemaInfo.Electric.FieldModelNames.NumberOfPhases);
            if (upFeatFieldIndex == -1 || downFeatFieldIndex == -1)
            {
                missingFields = SchemaInfo.Electric.FieldModelNames.NumberOfPhases + ", ";
            }
            else if (!object.Equals(upFeature.get_Value(upFeatFieldIndex), downFeature.get_Value(downFeatFieldIndex)))
            {
                invalidFields = upFeature.Class.Fields.get_Field(upFeatFieldIndex).Name + ", ";
            }

            //Validate the PhaseDesignation field
            upFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(upFeature.Class, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
            downFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(downFeature.Class, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
            if (upFeatFieldIndex == -1 || downFeatFieldIndex == -1)
            {
                missingFields += SchemaInfo.Electric.FieldModelNames.PhaseDesignation + ", ";
            }
            else if (!object.Equals(upFeature.get_Value(upFeatFieldIndex), downFeature.get_Value(downFeatFieldIndex)))
            {
                invalidFields += upFeature.Class.Fields.get_Field(upFeatFieldIndex).Name + ", ";
            }

            //Validate the OperatingVoltage field
            upFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(upFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
            downFeatFieldIndex = ModelNameFacade.FieldIndexFromModelName(downFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
            if (upFeatFieldIndex == -1 || downFeatFieldIndex == -1)
            {
                missingFields += SchemaInfo.Electric.FieldModelNames.OperatingVoltage + ", ";
            }
            else if (!object.Equals(upFeature.get_Value(upFeatFieldIndex), downFeature.get_Value(downFeatFieldIndex)))
            {
                invalidFields += upFeature.Class.Fields.get_Field(upFeatFieldIndex).Name + ", ";
            }

            //Prepapre proper error message if the features are not valid
            if (string.Compare(missingFields, string.Empty) != 0)
            {
                string classAliasNames = null;
                if (upFeature.Class == downFeature.Class) classAliasNames = upFeature.Class.AliasName;
                else classAliasNames = upFeature.Class.AliasName + " / " + downFeature.Class.AliasName;
                errorMessage = string.Format(invalidConfigError, missingFields.Remove(missingFields.Length - 2), classAliasNames);
                valid = false;
            }
            else if (string.Compare(invalidFields, string.Empty) == 0) { valid = true; }
            else
            {
                errorMessage = string.Format(valueMismatchError, upFeature.Class.AliasName, upFeature.OID, downFeature.Class.AliasName, downFeature.OID, invalidFields.Remove(invalidFields.Length - 2));
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Returns the indexes of the Conductor edge features connected to a junction feature
        /// </summary>
        /// <param name="deviceFeature">Junction Feature</param>
        /// <returns>Returns the indexes of the Conductor edge features connected to a junction feature</returns>
        /// <remarks>
        /// Conductors are identified by the class mode names
        /// </remarks>
        public static int[] GetConductorsAtJunction(ISimpleJunctionFeature deviceFeature)
        {
            IList<int> conductors = new List<int>();
            string[] conductorClassModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor, SchemaInfo.Electric.ClassModelNames.PGEBusBar };
            for (int edgeIndex = 0; edgeIndex < deviceFeature.EdgeFeatureCount; edgeIndex++)
            {
                //Get each edge feature and store the feature index if the feature is a conductor
                IFeature edgeFeature = deviceFeature.get_EdgeFeature(edgeIndex) as IFeature;
                if (ModelNameFacade.ContainsClassModelName(edgeFeature.Class, conductorClassModelNames))
                { conductors.Add(edgeIndex); }
            }

            return conductors.ToArray<int>();
        }

        /// <summary>
        /// Determines if NormalPostionA / NormalPostionB / NormalPostionC field value of the Device is closed
        /// </summary>
        /// <param name="obj">Device to check for NormalPostion field values</param>
        /// <param name="onlyOnUpdatedFields">Whether the validation should carried out only on updated fields</param>
        /// <returns>Returns true if either NormalPostionA / NormalPostionB / NormalPostionC field value is closed; false, otherwise</returns>
        public static bool IsDeviceClosed(IObject obj, bool onlyOnUpdatedFields)
        {
            bool isClosed = false;

            ////Checks whether shape is changed
            //if (onlyOnUpdatedFields)
            //{
            //    isClosed = GeometryFacade.ShapeChanged(obj);
            //    if (isClosed) return isClosed;
            //}

            //IRowChanges objChanges = obj as IRowChanges;

            //Checks whether the NormalPositionA is closed
            int normalPosFieldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.NormalpositionA);
            if (normalPosFieldIndex != -1)
            {
                //if (!onlyOnUpdatedFields || objChanges.get_ValueChanged(normalPosFieldIndex))
                //{
                isClosed = object.Equals(obj.get_Value(normalPosFieldIndex), NormalStatusClosed);
                if (isClosed) return isClosed;
                //}
            }

            //Checks whether the NormalPositionB is closed
            normalPosFieldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.NormalpositionB);
            if (normalPosFieldIndex != -1)
            {
                //if (!onlyOnUpdatedFields || objChanges.get_ValueChanged(normalPosFieldIndex))
                //{
                isClosed = object.Equals(obj.get_Value(normalPosFieldIndex), NormalStatusClosed);
                if (isClosed) return isClosed;
                //}
            }

            //Checks whether the NormalPositionC is closed
            normalPosFieldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.NormalPositionC);
            if (normalPosFieldIndex != -1)
            {
                //if (!onlyOnUpdatedFields || objChanges.get_ValueChanged(normalPosFieldIndex))
                //{
                isClosed = object.Equals(obj.get_Value(normalPosFieldIndex), NormalStatusClosed);
                if (isClosed) return isClosed;
                //}
            }

            return isClosed;
        }

        #endregion Business Logic Methods
    }
}
