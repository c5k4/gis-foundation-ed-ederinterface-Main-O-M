// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing-Dates - EDER Component Specification
// Shaikh Rizuan 10/11/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.esriSystem;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{

    /// <summary>
    /// Populate  Install Job Year Field Value.
    /// </summary>
    [ComVisible(true)]
    [Guid("EA3D9736-B23B-4116-9897-B9E348CC3CCC")]
    [ProgId("PGE.Desktop.EDER.InstallJobYearAutoupdater")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
   public class InstallJobYearAutoupdater :BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private int _installJobFldIndex;
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        
        public InstallJobYearAutoupdater() : base("PGE Install Job Year")
        {

        }

        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            
            
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.InstallationJobYear);
                _logger.Debug("Field model name :" + SchemaInfo.Electric.FieldModelNames.InstallationJobYear + " Found-" + enabled);

            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

         /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            object installJobYrFldValue;
            int installDatefldIndex;
            Func<IObject, string, int> getFieldIndex = (objValueIndx, modelName) => ModelNameFacade.FieldIndexFromModelName(objValueIndx.Class, modelName);
            //check for model name assigned
            if ((_installJobFldIndex = getFieldIndex(obj, SchemaInfo.Electric.FieldModelNames.InstallationJobYear)) != -1)
            {
                installJobYrFldValue = obj.get_Value(_installJobFldIndex);
               _logger.Debug("Install job Year: " + installJobYrFldValue.ToString());
                //if job year field value is not null
               if (installJobYrFldValue == System.DBNull.Value && string.IsNullOrEmpty(installJobYrFldValue.ToString()))
               {
                   //if installation date present
                   if ((installDatefldIndex = getFieldIndex(obj, SchemaInfo.Electric.FieldModelNames.InstallationDate)) != -1)
                   {
                       //Set install job year field value
                       SetInstallJobYearFieldValue(obj,installDatefldIndex);
                   }
                   else
                   {
                       //Check if class model name present
                       if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.GetFieldFromChild))
                       {
                           //Get relationship class
                           IRelationshipClass relClass = ModelNameFacade.RelationshipClassFromModelName(obj.Class, esriRelRole.esriRelRoleOrigin, SchemaInfo.Electric.ClassModelNames.InstallationChildField);
                           if (relClass != null)
                           {
                               _logger.Debug("Related object name: " + relClass.OriginClass.AliasName);
                               //Get related objects
                               GetRelatedObjects(obj, relClass);
                           }
                       }
                   }
               }
            }
           
        }

        /// <summary>
        /// Get related objects
        /// </summary>
        /// <param name="obj">ESRI object</param>
        /// <param name="relClass">ESRI relationship class</param>
        private void GetRelatedObjects(IObject obj, IRelationshipClass relClass)
        {
            ISet relatedSet = relClass.GetObjectsRelatedToObject(obj);
            if (relatedSet != null && relatedSet.Count > 0) //if count greater than zero
            {
                _logger.Debug("Relatd Object Count: " + relatedSet.Count);
                IObject relObj = null;
                //loop through each object
                while ((relObj = relatedSet.Next() as IObject) != null)
                {
                    _logger.Debug("Related ObjectID" + relObj.OID);
                    //get install date field value
                    var relFieldInstallationDate = FieldInstance.FromModelName(relObj, SchemaInfo.Electric.FieldModelNames.InstallationDate);
                    if (relFieldInstallationDate != null && relFieldInstallationDate.Value != System.DBNull.Value)//if not null
                    {
                        //Set job year field value
                        SetInstallJobYearFieldValue(obj, relFieldInstallationDate.Index, relObj);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set Install job year field value
        /// </summary>
        /// <param name="obj">ESRI object</param>
        /// <param name="installDatefldIndex">Field index</param>
        /// <param name="relObj">ESRI object(related)</param>
        private void SetInstallJobYearFieldValue(IObject obj, int installDatefldIndex,IObject relObj=null)
        {
            object installDateFieldValue;
            installDateFieldValue = relObj != null ? relObj.get_Value(installDatefldIndex):obj.get_Value(installDatefldIndex);

            //check for field value is not null or empty
            if (installDateFieldValue != System.DBNull.Value && !string.IsNullOrEmpty(installDateFieldValue.ToString()))
            {
                int year = ((System.DateTime)(installDateFieldValue)).Year;
                _logger.Debug("Install job year value: " + year);
                //Assign field value to job year
                obj.set_Value(_installJobFldIndex, (object)year);
            }
            else
            {
                _logger.Warn("Installation date field value is <Null>.");

            }
        }
        #endregion
    }
}
