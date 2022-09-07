using System.Collections.Generic;
using System.Collections.ObjectModel;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Interface that represents a result set from an operation.
    /// </summary>
    public interface IResultSet
    {
        /// <summary>
        /// Name of the result set.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Layer Id associated with the result set.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Display field name.
        /// </summary>
        string DisplayFieldName { get; set; }

        /// <summary>
        /// Specifies whether there are more results from an operation.
        /// </summary>
        bool ExceededThreshold { get; set;  }

        /// <summary>
        /// Field aliases.
        /// </summary>
        Dictionary<string, string> FieldAliases { get; }

        /// <summary>
        /// Spatial reference.
        /// </summary>
        SpatialReference SpatialReference { get; }

        /// <summary>
        /// Geometry type.
        /// </summary>
        GeometryType GeometryType { get;  }

        /// <summary>
        /// Collection of features in nthe result set.
        /// </summary>
        ObservableCollection<Graphic> Features { get; }

        /// <summary>
        /// Service with which the result set is associated.
        /// </summary>
        string Service { get; set; }

        /// <summary>
        /// Relationship service with which the result set is associated.
        /// </summary>
        IRelationshipService RelationshipService { get; set; }
    }
}
