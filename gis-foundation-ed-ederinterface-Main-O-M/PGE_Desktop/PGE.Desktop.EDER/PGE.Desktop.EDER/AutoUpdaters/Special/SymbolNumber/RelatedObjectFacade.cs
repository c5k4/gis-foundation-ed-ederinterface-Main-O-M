using System.Collections.Generic;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    public class RelatedObjectFacade
    {
        #region Private
        /// <summary>
        /// Instance of this class.
        /// </summary>
        private static RelatedObjectFacade _instance;
        /// <summary>
        /// Dictionary object to hold all the OID pending for deletion.
        /// </summary>
        private Dictionary<int, List<int>> oidsPendingDeletion;
        /// <summary>
        /// logger to log all the infomation, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        /// <summary>
        /// Prottected constructor.
        /// </summary>
        protected RelatedObjectFacade()
        {
            //initialize the dictionary object of pending oid for deletion.
            oidsPendingDeletion = new Dictionary<int, List<int>>();
        }
        #endregion Constructor

        #region Public
        /// <summary>
        /// Static property to Get and privately set initialized instance of this class.
        /// </summary>
        public static RelatedObjectFacade Instance
        {
            get
            {
                //before returning the instance check if same is already initialized
                if (_instance == null)
                {
                    //initialize the instance for the first time.
                    _logger.Debug("Creating new instance of RelatedObjectsFacade class.");
                    _instance = new RelatedObjectFacade();
                }
                //return the instance
                _logger.Debug("Returning the instance.");
                return _instance;
            }

            private set { }
        }

        /// <summary>
        /// Add the OID to oid pending for deletion dictionary.
        /// </summary>
        /// <param name="objClass">The Object class.</param>
        /// <param name="oid">The OID</param>
        public void addOID(IObjectClass objClass, int oid)
        {
            //lock the dictionary for thread safe
            _logger.Debug("Locking the oidsPendingDeletion object for thread safe.");
            lock (oidsPendingDeletion)
            {
                _logger.Debug("Thread safe section start.");
                //critical section starts
                List<int> oids = null;
                //get the oids from dictionary
                oidsPendingDeletion.TryGetValue(objClass.ObjectClassID, out oids);
                //failed to get the oids means it doesn't exist
                if (oids == null)
                {
                    //create new list and add the OID to it
                    _logger.Debug("Listof OID doesn't exist creating new.");
                    oids = new List<int>();
                    oids.Add(oid);
                    //add newly created list to dictionary
                    _logger.Debug("Added OID " + oid + " to the list of OID.");
                    oidsPendingDeletion.Add(objClass.ObjectClassID, oids);
                    _logger.Debug("Added list of OID to dictionary.");
                }
                else
                {
                    //already got the list of OID's so need to add current OID to it
                    _logger.Debug("Listof OID already present.");
                    oids.Add(oid);
                    _logger.Debug("Added OID " + oid + " to list of OID.");
                }
                _logger.Debug("Thread safe section end.");
                //critical section ends
            }
        }

        /// <summary>
        /// Remove the OID from dictionary object OID pending for deletion.
        /// </summary>
        /// <param name="objClass">The Object class.</param>
        /// <param name="oid">The OID.</param>
        public void removeOID(IObjectClass objClass, int oid)
        {
            //lock the object for thread safe
            _logger.Debug("Locking the oidsPendingDeletion object for thread safe.");
            lock (oidsPendingDeletion)
            {
                _logger.Debug("Thread safe section start.");
                //critical section starts
                List<int> oids = null;
                //get the list of oid for objectclass
                oidsPendingDeletion.TryGetValue(objClass.ObjectClassID, out oids);
                //check if list present
                if (oids != null)
                {
                    //remove the OID from list
                    _logger.Debug("Removed OID " + oid + " from list of OID.");
                    oids.Remove(oid);
                }
                else
                {
                    _logger.Debug("List of OID doesn't exist, no need to remove any OID.");
                }
                _logger.Debug("Thread safe section end.");
                //critical section ends
            }
        }

        /// <summary>
        /// Gets all related objects to the object except object pending for deletion.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <param name="obj">The object.</param>
        /// <returns>Related set of objects.</returns>
        public ESRI.ArcGIS.esriSystem.ISet GetObjectsRelatedToObject(IRelationshipClass relClass, IObject obj)
        {
            //get all the related objects
            _logger.Debug("Getting all the related objects.");
            ESRI.ArcGIS.esriSystem.ISet relatedObjects = relClass.GetObjectsRelatedToObject(obj);
            relatedObjects.Reset();

            object current = null;
            //iterate through all the related objects
            _logger.Debug("Looping through all the related objects.");
            while ((current = relatedObjects.Next()) != null)
            {
                //cast current object to IObject
                IObject relatedObject = current as IObject;
                //check if casting is successful
                if (relatedObject == null) continue;

                List<int> oids = null;
                //lock the dictionary object for thread safe
                lock (oidsPendingDeletion)
                {
                    _logger.Debug("Thread safe section start.");
                    //critical section starts
                    oidsPendingDeletion.TryGetValue(relatedObject.Class.ObjectClassID, out oids);
                    //check if list of oid is exist
                    if (oids != null)
                    {
                        _logger.Debug("List of OID present for ObjectClassID " + relatedObject.Class.ObjectClassID + ".");
                        //check if current OID present in list of OID's
                        if (oids.Contains(relatedObject.OID))
                        {
                            //just remove it from related objects
                            _logger.Debug("Removing the object from related set with OID " + relatedObject.OID + ".");
                            relatedObjects.Remove(current);
                            _logger.Debug("Removed successfully the object from related set with OID " + relatedObject.OID + ".");
                        }
                    }
                    else
                    {
                        _logger.Debug("List of OID doens't exist for ObjectClassID " + relatedObject.Class.ObjectClassID + ".");
                    }
                    _logger.Debug("Thread safe section end.");
                    //critical section ends
                }
            }

            return relatedObjects;
        }
        #endregion Public
    }
}
