using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Interface to allow processors to get specific values from a feature without necessarily knowing what kind of feature it is
    /// </summary>
    public interface IFeatureValues
    {
        /// <summary>
        /// Populate values specific to the feature
        /// </summary>
        /// <param name="row">The row to populate with data</param>
        /// <param name="info">The feature to get the data from</param>
        void getValues(DataRow row, FeatureInfo info, ControlTable controlTable);
    }
}
