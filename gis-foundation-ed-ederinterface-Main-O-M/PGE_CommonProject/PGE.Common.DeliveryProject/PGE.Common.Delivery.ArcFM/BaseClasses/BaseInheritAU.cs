using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// An abstract special auto updater that provides helper methods to inherit information from connected features.
    /// </summary>
    public abstract class BaseInheritAU : BaseSpecialAU
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInheritAU"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BaseInheritAU(string name)
            : base(name)
        {
        }

        #region Protected Members
        /// <summary>
        /// Finds the first connected feature based on the specified network feature.
        /// </summary>
        /// <param name="netFeature">The net feature.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        /// The connected edge if the network feature is a junction or the connected junction if the network feature is an edge; otherwise null.
        /// </returns>
        protected IFeature FindConnected(INetworkFeature netFeature, string modelName)
        {
            if (netFeature is IEdgeFeature)
            {
                IFeature junction = ConnectedJunction(netFeature, modelName);
                return (junction == null) ? ConnectedEdge(netFeature, modelName) : junction;
            }
            else
            {
                return ConnectedEdge(netFeature, modelName);
            }
        }

        /// <summary>
        /// Gets the first connected edge with the specified field model name
        /// </summary>
        /// <param name="netFeature">The net feature.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        /// The first connected edge with the specified class model name; otherwise null.
        /// </returns>
        protected IFeature ConnectedEdge(INetworkFeature netFeature, string modelName)
        {
            List<IEdgeFeature> edges = ConnectedEdges(netFeature);
            if (string.IsNullOrEmpty(modelName))
            {
                return edges.FirstOrDefault() as IFeature;
            }

            return edges.FirstOrDefault(w => ModelNameFacade.ContainsFieldModelName(((IFeature)w).Class, modelName)) as IFeature;
        }

        /// <summary>
        /// Gets the first connected junction with the specified field model name
        /// </summary>
        /// <param name="netFeature">The net feature.</param>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>
        /// The first connected junction with the specified class model name; otherwise null.
        /// </returns>
        protected IFeature ConnectedJunction(INetworkFeature netFeature, string modelName)
        {
            List<IJunctionFeature> junctions = ConnectedJunctions(netFeature);
            if (string.IsNullOrEmpty(modelName))
            {
                return junctions.FirstOrDefault() as IFeature;
            }

            return junctions.FirstOrDefault(w => ModelNameFacade.ContainsFieldModelName(((IFeature)w).Class, modelName)) as IFeature;
        }

        /// <summary>
        /// Gets the connected edges to the specified network feature.
        /// </summary>
        /// <param name="netFeature">The network feature.</param>
        /// <returns>A list of connected edge features.</returns>
        protected List<IEdgeFeature> ConnectedEdges(INetworkFeature netFeature)
        {
            if (netFeature is IEdgeFeature)
            {
                return ConnectedEdges((IEdgeFeature)netFeature);
            }
            else
            {
                return ConnectedEdges((ISimpleJunctionFeature)netFeature);
            }
        }

        /// <summary>
        /// Gets the connected junctions to the specified network feature.
        /// </summary>
        /// <param name="netFeature">The net feature.</param>
        /// <returns>A list of connected junction features.</returns>
        protected List<IJunctionFeature> ConnectedJunctions(INetworkFeature netFeature)
        {
            if (netFeature is IEdgeFeature)
            {
                return ConnectedJunctions((IEdgeFeature)netFeature);
            }
            else
            {
                return ConnectedJunctions((ISimpleJunctionFeature)netFeature);
            }
        }

        /// <summary>
        /// Checks the domain to make sure the value is valid.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the </returns>
        protected bool CheckDomain(IObject obj, string fieldName, object value)
        {
            IRowSubtypes rowSubtypes = (IRowSubtypes)obj;
            ISubtypes subtypes = (ISubtypes)obj.Class;

            IDomain domain = subtypes.get_Domain(rowSubtypes.SubtypeCode, fieldName);
            if (domain == null) return true;
            
            ICodedValueDomain cvDomain = domain as ICodedValueDomain;
            if(cvDomain == null) return true;

            for (int i = 0; i < cvDomain.CodeCount; i++)
            {
                object code = cvDomain.get_Value(i);
                if (code.Equals(value)) return true;
            }

            return false;
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Gets the connected edges to the specified edge feature.
        /// </summary>
        /// <param name="edgeFeature">The edge feature.</param>
        /// <returns>A list of connected edge features.</returns>
        private List<IEdgeFeature> ConnectedEdges(IEdgeFeature edgeFeature)
        {
            List<IEdgeFeature> edges = new List<IEdgeFeature>();
            List<IJunctionFeature> junctions = new List<IJunctionFeature>();

            if (edgeFeature is IComplexEdgeFeature)
            {
                IComplexEdgeFeature cef = (IComplexEdgeFeature)edgeFeature;
                for (int i = 0; i < cef.JunctionFeatureCount; i++)
                {
                    IJunctionFeature jf = cef.get_JunctionFeature(i);
                    junctions.Add(jf);
                }
            }
            else
            {
                junctions.Add(edgeFeature.FromJunctionFeature);
                junctions.Add(edgeFeature.ToJunctionFeature);
            }

            // Get the connected edges to the junctions.
            foreach (IJunctionFeature jf in junctions)
            {
                ISimpleJunctionFeature sjf = (ISimpleJunctionFeature)jf;
                for (int j = 0; j < sjf.EdgeFeatureCount; j++)
                {
                    IEdgeFeature ef = sjf.get_EdgeFeature(j);
                    if (ef == edgeFeature) continue;

                    if (!edges.Contains(ef))
                        edges.Add(ef);
                }
            }

            return edges;
        }

        /// <summary>
        /// Gets the connected edges to the specified simple junction feature.
        /// </summary>
        /// <param name="sjf">The simple junction feature.</param>
        /// <returns>A list of connected edge features.</returns>
        private List<IEdgeFeature> ConnectedEdges(ISimpleJunctionFeature sjf)
        {
            List<IEdgeFeature> edges = new List<IEdgeFeature>();
            for (int j = 0; j < sjf.EdgeFeatureCount; j++)
            {
                IEdgeFeature ef = sjf.get_EdgeFeature(j);
                edges.Add(ef);
            }

            return edges;
        }

        /// <summary>
        /// Gets the connected junctions to the specified junction feature.
        /// </summary>
        /// <param name="sjf">The simple junction feature.</param>
        /// <returns>A list of connected junction features.</returns>
        private List<IJunctionFeature> ConnectedJunctions(ISimpleJunctionFeature sjf)
        {
            List<IJunctionFeature> junctions = new List<IJunctionFeature>();

            IJunctionFeature jf = (IJunctionFeature)sjf;
            for (int i = 0; i < sjf.EdgeFeatureCount; i++)
            {
                IEdgeFeature ef = sjf.get_EdgeFeature(i);

                if (ef.ToJunctionFeature != jf)
                    junctions.Add(ef.ToJunctionFeature);

                if (ef.FromJunctionFeature != jf)
                    junctions.Add(ef.FromJunctionFeature);
            }

            return junctions;
        }

        /// <summary>
        /// Gets the connected junctions to the specified edge feature.
        /// </summary>
        /// <param name="edgeFeature">The edge feature.</param>
        /// <returns>A list of connected junction features.</returns>
        private List<IJunctionFeature> ConnectedJunctions(IEdgeFeature edgeFeature)
        {
            List<IJunctionFeature> junctions = new List<IJunctionFeature>();

            try
            {
                if (edgeFeature is IComplexEdgeFeature)
                {
                    IComplexEdgeFeature cef = (IComplexEdgeFeature)edgeFeature;
                    for (int i = 0; i < cef.JunctionFeatureCount; i++)
                    {
                        IJunctionFeature jf = cef.get_JunctionFeature(i);
                        junctions.Add(jf);
                    }
                }
                else
                {
                    junctions.Add(edgeFeature.FromJunctionFeature);
                    junctions.Add(edgeFeature.ToJunctionFeature);
                }

            }
            catch 
            {
                
            }
            
            return junctions;
        }
        #endregion
    }
}
