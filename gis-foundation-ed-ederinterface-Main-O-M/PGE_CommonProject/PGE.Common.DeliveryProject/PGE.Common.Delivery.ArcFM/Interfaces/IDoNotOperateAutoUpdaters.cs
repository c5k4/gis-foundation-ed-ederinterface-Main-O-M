using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Changes for ENOS to SAP migration, Provides access to the auto updaters that are inoperable.
    /// </summary>
    public interface IDoNotOperateAutoUpdaters
    {
        /// <summary>
        /// Adds the specified type to specific object class ID.
        /// </summary>
        /// <param name="type">The type.</param>
        void AddAU(int ID, Type type);

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        void AddAU(IObjectClass source, Type type);
        /// <summary>
        /// Determines whether the AU type is inoperable for a specific object class ID.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        bool ContainsAU(int objectClassID,Type type);
        /// <summary>
        ///     Determines whether the type is inoperable.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <returns>Returns a <see cref="bool"/> representing <c>true</c> when the type is inoperable for the source.</returns>
        bool ContainsAU(IObjectClass source, Type type);
        /// <summary>
        /// Removes the specified type AU for a specific object class ID.
        /// </summary>
        /// <param name="type">The type.</param>
        void RemoveAU(int objectClassID,Type type);
        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        void RemoveAU(IObjectClass source, Type type);

    }
}
