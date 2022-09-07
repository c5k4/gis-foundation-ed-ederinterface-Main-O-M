using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem; 
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)] 
    [Guid("dc463f28-c89b-4e6f-b18a-b791cee14036")]
    [ProgId("PGE.Desktop.EDER.DeleteOrphanRowsAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class DeleteOrphanRowsAU : BaseSpecialAU
    {
        #region Static Methods
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public DeleteOrphanRowsAU()
            : base("PGE Delete Orphan Rows")
        {

        }
        #endregion


        #region Base special AU Overrides
        /// <summary>
        /// Determines when the AU should be enabled 
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool isEnabled = false;
            if (eEvent == mmEditEvent.mmEventFeatureDelete)
                isEnabled = true;
            return isEnabled;
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
            
            if (eEvent != mmEditEvent.mmEventFeatureDelete)
                return; 
            string msg = "Deleting " + obj.Class.AliasName + " with oid: " + 
                obj.OID.ToString() + " in " + base._Name;
            _logger.Debug(msg);
            
            //1.Determine if the object has one or more related objects with 
            //model name PGE_ORPHANCLEANUP. 
            //2.For each of these PGE_ORPHANCLEANUP objects determine if the 
            //passed object to be deleted is the only related object. 
            //3. If passed object to be deleted is the only related object  
            //then delete the related PGE_ORPHANCLEANUP object 

            Hashtable hshObjectsToDelete = new Hashtable(); 
            IObjectClass pOtherOC = null;
            bool otherClassHasOrphanCleanup = false; 
            IEnumRelationshipClass pEnumRelClass = obj.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelClass = pEnumRelClass.Next();
            while (pRelClass != null)
            {
                otherClassHasOrphanCleanup = false;
                if (pRelClass.DestinationClass.AliasName == obj.Class.AliasName)
                    pOtherOC = pRelClass.OriginClass; 
                else
                    pOtherOC = pRelClass.DestinationClass;

                //Determine if the other class has model name PGE_ORPHANCLEANUP 
                if (ModelNameFacade.ContainsClassModelName(pOtherOC,
                        new string[] { SchemaInfo.Electric.ClassModelNames.OrphanCleanup }))
                    otherClassHasOrphanCleanup = true;

                if (otherClassHasOrphanCleanup)
                {
                    hshObjectsToDelete.Clear(); 
                    Debug.Print("Other class: " + ((IDataset)pOtherOC).Name + " has modelname: " + 
                        SchemaInfo.Electric.ClassModelNames.OrphanCleanup); 
 
                    //For each of the related features in this other class 
                    //check if they are orphan (other than the passed related record) 
                    //If so then delete the record
                    ISet pRelOrphanCleanups = pRelClass.GetObjectsRelatedToObject(obj); 
                    IRow pOrphanRow = (IRow)pRelOrphanCleanups.Next(); 
                    while (pOrphanRow != null)
                    {
                        if (!HasOtherRelatedObjects(pOrphanRow, obj))
                        {
                            //The related object which has PGE_ORPHANCLEANUP has no other 
                            //related objects other than the object being deleted so therefore 
                            //we can delete the PGE_ORPHANCLEANUP row 
                            hshObjectsToDelete.Add(pOrphanRow.OID, 0);
                        }
                        pOrphanRow = (IRow)pRelOrphanCleanups.Next(); 
                    }
                    Marshal.FinalReleaseComObject(pRelOrphanCleanups);  
                    
                    //If there are any objects to delete then delete them 
                    foreach (int oid in hshObjectsToDelete.Keys)
                    {
                        IRow pDelRow = ((ITable)pOtherOC).GetRow(oid);
                        pDelRow.Delete(); 
                    }
                }
                pRelClass = pEnumRelClass.Next();
            }
            Marshal.FinalReleaseComObject(pEnumRelClass);           

        }
        #endregion

        #region Private Methods

        private bool HasOtherRelatedObjects(IRow pOrphanCleanup, IRow pDeleteObj)
        {
            try
            {
                //Find one related object that is not the passed pDeleteObj and 
                //the routine must return true 
                bool hasOtherRelatedObjects = false; 
                IDataset pDS = (IDataset)pDeleteObj.Table;
                string deleteObjDS = pDS.Name.ToUpper(); 
                int deleteObjOId = pDeleteObj.OID; 
                IEnumRelationshipClass pEnumRelClass = ((IObjectClass)pOrphanCleanup.Table).
                    get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelClass = pEnumRelClass.Next();
                while (pRelClass != null)
                {
                    ISet pSet = pRelClass.GetObjectsRelatedToObject((IObject)pOrphanCleanup);
                    pSet.Reset(); 
                    IRow pRelRow = (IRow)pSet.Next();
                    while (pRelRow != null)
                    {
                        pDS = (IDataset)pRelRow.Table;
                        if ((pDS.Name.ToUpper() != deleteObjDS) ||
                            (deleteObjOId != pRelRow.OID))
                        {
                            Debug.Print("Found another related object other than one being deleted!"); 
                            hasOtherRelatedObjects = true;
                            break; 
                        }                        
                        pRelRow = (IRow)pSet.Next(); 
                    }
                    Marshal.FinalReleaseComObject(pSet);
                    if (hasOtherRelatedObjects)
                        break;
                    pRelClass = pEnumRelClass.Next(); 
                }
                return hasOtherRelatedObjects; 
            }
            catch (Exception ex)
            {
                _logger.Error("Entering error hander for HasOtherRelatedObjects with error: " + ex.Message); 
                throw new Exception("Error determining if OrphanCleanup row has other related objects"); 
            }
        }
        
        #endregion

    }
}
