using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// The interface to implement when creating custom relationships.
    /// </summary>
    public interface IRelationshipService
    {
        //ENOS2EDGIS Start, added Definition Expression to the interface definition
        string DefinitionExpression { get; set; }
        //**************ENOS2EDGIS End*********************************
        event EventHandler<RelationshipEventArgs> ExecuteCompleted;
        void GetRelationshipsAsync(IEnumerable<RelationshipInformation> data, int[] objectIDs, SpatialReference spatialReference = null);
    }
}
