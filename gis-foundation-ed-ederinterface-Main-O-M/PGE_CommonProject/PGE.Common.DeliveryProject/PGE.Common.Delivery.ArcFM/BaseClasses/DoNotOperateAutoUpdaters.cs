using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Common.Delivery.ArcFM
{
    //Changes for ENOS to SAP migration
    public sealed class DoNotOperateAutoUpdaters : IDoNotOperateAutoUpdaters
    {
        #region Fields

        private readonly Dictionary<int,List<Guid>> _GuidsToObjectClasses;
        private static DoNotOperateAutoUpdaters _Instance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="IDoNotOperateAutoUpdaters" /> class from being created.
        /// </summary>
        private DoNotOperateAutoUpdaters()
        {
            _GuidsToObjectClasses = new Dictionary<int, List<Guid>>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static DoNotOperateAutoUpdaters Instance
        {
            get { return _Instance ?? (_Instance = new DoNotOperateAutoUpdaters()); }
        }

        #endregion

        #region IDoNotOperateAutoUpdaters Members


        /// <summary>
        ///     Determines whether a AU type is inoperable for a specific Object class.
        /// </summary>
        /// <param name="objClass">The Object class.</param>
        /// <param name="type">The type AU.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the type is inoperable for the source.
        /// </returns>
        public bool ContainsAU(IObjectClass objClass, Type type)
        {
            return this.ContainsAU(objClass.ObjectClassID, type);
        }

        /// <summary>
        ///     Determines whether the type AU is inoperatable for a specific object class ID.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool ContainsAU(int objectClassID,Type type)
        {
            var list = new List<Guid>();

            if (_GuidsToObjectClasses.ContainsKey(objectClassID))
                list = _GuidsToObjectClasses[objectClassID];

            return list.Contains(type.GUID);
        }

        /// <summary>
        ///     Adds the specified AU type to specific object class.
        /// </summary>
        /// <param name="objClass">The Object class.</param>
        /// <param name="type">The AU type.</param>
        public void AddAU(IObjectClass objClass, Type type)
        {
            this.AddAU(objClass.ObjectClassID, type);
        }

        /// <summary>
        ///     Adds the specified type.
        /// </summary>
        /// <param name="objectClassID">The object class ID.</param>
        /// <param name="type">The type.</param>
        public void AddAU(int objectClassID,Type type)
        {
            var list = new List<Guid>();

            if (_GuidsToObjectClasses.ContainsKey(objectClassID))
                list = _GuidsToObjectClasses[objectClassID];
            else
                _GuidsToObjectClasses.Add(objectClassID, list);

            list.Add(type.GUID);
        }

        /// <summary>
        ///     Removes the specified type AU for a specific object class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        public void RemoveAU(IObjectClass objClass, Type type)
        {
            this.RemoveAU(objClass.ObjectClassID, type);
        }

        /// <summary>
        ///     Removes the specified type.
        /// </summary>
        /// <param name="objectClassID">The object class ID.</param>
        /// <param name="type">The type AU.</param>
        public void RemoveAU(int objectClassID,Type type)
        {
            var list = new List<Guid>();

            if (_GuidsToObjectClasses.ContainsKey(objectClassID))
                list = _GuidsToObjectClasses[objectClassID];

            list.Remove(type.GUID);
        }

        #endregion
    }
}
