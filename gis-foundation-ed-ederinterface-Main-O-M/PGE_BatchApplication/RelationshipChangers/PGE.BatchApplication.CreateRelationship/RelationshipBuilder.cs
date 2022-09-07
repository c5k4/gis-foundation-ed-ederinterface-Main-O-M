using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Runtime.InteropServices;
using System.IO;

namespace PGE.BatchApplication.CreateRelationship
{
    /// <summary>
    /// Currently does not support attributed relationships. This can be added if desired.
    /// </summary>
    public class RelationshipBuilder : IDisposable
    {
        IWorkspace _ws = null;
        IRelationshipClass _relClass = null;
        IObjectClass _originClass = null;
        IObjectClass _destinationClass = null;
        string _relName = null;
        string _originTable;
        string _backwardPathLabel;
        string _originPK;
        string _originFK;
        string _destTable;
        string _forwardPathLabel;
        esriRelCardinality _relCardinality;
        esriRelNotification _messageDirection;
        bool _isComposite;

        public RelationshipBuilder(string databaseLocation, string relName, string originTable, string backwardPathLabel, 
            string originPK, string originFK, string destTable, string forwardPathLabel, string relCardinality, 
            string messageDirection, bool isComposite)
        {
            _relName = relName;
            _originTable = originTable;
            _backwardPathLabel = backwardPathLabel;
            _originPK = originPK;
            _originFK = originFK;
            _destTable = destTable;
            _forwardPathLabel = forwardPathLabel;
            _isComposite = isComposite;

            switch (relCardinality.Trim().ToUpper().Replace("_", ""))
            {
                case "ONETOONE":
                    _relCardinality = esriRelCardinality.esriRelCardinalityOneToOne;
                    break;
                case "MANYTOMANY":
                    _relCardinality = esriRelCardinality.esriRelCardinalityManyToMany;
                    break;
                default:
                    _relCardinality = esriRelCardinality.esriRelCardinalityOneToMany;
                    break;
            }

            _messageDirection = esriRelNotification.esriRelNotificationNone;
            switch (messageDirection.Trim().ToUpper().Replace("_", ""))
            {
                case "FORWARD":
                    _messageDirection = esriRelNotification.esriRelNotificationForward;
                    break;
                case "BACK":
                    _messageDirection = esriRelNotification.esriRelNotificationBackward;
                    break;
                case "BOTH":
                    _messageDirection = esriRelNotification.esriRelNotificationBoth;
                    break;
                default:
                    _messageDirection = esriRelNotification.esriRelNotificationNone;
                    break;
            }

            FileInfo fi = new FileInfo(databaseLocation);
            if (fi.Exists && fi.Extension.ToUpper() == ".SDE")
            {
                SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                _ws = wsFactory.OpenFromFile(databaseLocation, 0);
            }
            else
            {
                AccessWorkspaceFactory accessWSFactory = new AccessWorkspaceFactoryClass();
                _ws = accessWSFactory.OpenFromFile(databaseLocation, 0);
            }

            IFeatureWorkspace featWorkspace = _ws as IFeatureWorkspace;


            //Make sure it doesn't already exist, so we can error out more elegantly if so.
            IRelationshipClass target = null;
            try
            {
                target = featWorkspace.OpenRelationshipClass(_relName);
            }
            catch { }

            if (target != null)
            {
                //Table could not be found
                Console.WriteLine("\nERROR: That relationship class name already exists! (" + _relName + ")\n");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey(true);
                return;
            }

            try
            {
                _originClass = featWorkspace.OpenTable(_originTable) as IObjectClass;
                _destinationClass = featWorkspace.OpenTable(_destTable) as IObjectClass;
            }
            catch
            {
                //Table could not be found
                Console.WriteLine("\nERROR: One of the specified tables could not be found.\n");
                Console.WriteLine("Press a key to continue.");
                Console.ReadKey(true);
                return;
            }
        }

        public void Create()
        {
            IFeatureWorkspace featWorkspace = _ws as IFeatureWorkspace;

            _relClass = featWorkspace.CreateRelationshipClass(_relName, _originClass, _destinationClass, _forwardPathLabel, _backwardPathLabel, _relCardinality, _messageDirection, _isComposite, false, null, _originPK, null, _originFK, null);

            Console.WriteLine("Successfully added relationship rule: " + _relName);
        }

        public void Dispose()
        {
        }
    }
}
