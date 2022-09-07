using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Miner.ComCategories;
using Miner.Interop;

using log4net;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;



namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Base class for Tree Tools used to execute in ArcMap. 
    /// You must provide your own COM Registration because they can be registered in multiple categories 
    /// i.e <see cref="D8CUTreeViewTool"/>,<see cref="D8SelectedCuTreeTool"/>, <see cref="D8SelectionTreeTool"/> 
    /// and/or <see cref="D8DesignTreeViewTool"/>.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseTreeTool : IMMTreeTool, IDisposable
    {

        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        #region Fields

        /// <summary>
        /// The category the tool will appear within.
        /// </summary>
        protected int _Category;

        /// <summary>
        /// The name of the tool.
        /// </summary>
        protected string _Name;

        /// <summary>
        /// The priority of the tool
        /// </summary>
        protected int _Priority;

        private Bitmap _Bitmap;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTreeTool"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="category">The category.</param>
        protected BaseTreeTool(string name, int priority, int category)
        {
            _Name = name;
            _Priority = priority;
            _Category = category;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether there are allow as default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there are allow as default; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsDefault
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public int Bitmap
        {
            get
            {
                return (_Bitmap != null) ? _Bitmap.GetHbitmap().ToInt32() : 0;
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public int Category
        {
            get { return _Category; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return _Priority; }
        }

        /// <summary>
        /// Gets the short cut.
        /// </summary>
        /// <value>The short cut.</value>
        public mmShortCutKey ShortCut
        {
            get { return mmShortCutKey.mmShortcutNone; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_Bitmap != null)
                _Bitmap.Dispose();
        }

        /// <summary>
        /// Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="pEnumItems">The enumeration of selected items.</param>
        /// <param name="lItemCount">The number of items selected.</param>
        public virtual void Execute(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                InternalExecute(pEnumItems, lItemCount);
            }
            catch (Exception e)
            {
                _logger.Error("Error Executing TreeTool: " + _Name, e);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="pEnumItems">The enumeration of selected items.</param>
        /// <param name="lItemCount">The number of items selected.</param>
        /// <returns><c>true</c> if the tool should be enabled; otherwise <c>false</c></returns>
        public virtual int get_Enabled(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                return (int)InternalEnabled(pEnumItems, lItemCount);
            }
            catch (Exception ex)
            {
                _logger.Error("Error Enabling the TreeTool " + this.Name, ex);
            }

            return 0;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        protected void UpdateBitmap(Bitmap bitmap)
        {
            _Bitmap = bitmap;
        }

        /// <summary>
        /// Loads the specified bitmap for command and sets the transparence for the xy location.
        /// </summary>
        /// <param name="stream">Stream of bitmap to load.</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        protected void UpdateBitmap(Stream stream, int x, int y)
        {
            try
            {
                _Bitmap = new Bitmap(stream);
                _Bitmap.MakeTransparent(_Bitmap.GetPixel(x, y));
            }
            catch (Exception ex)
            {
                _logger.Error(this.Name, ex);
            }
        }

        /// <summary>
        /// Determines of the tree tool is enabled for the specified selection of items.
        /// </summary>
        /// <param name="enumItems">The enumeration of selected items.</param>
        /// <param name="itemCount">The number of items selected.</param>
        /// <returns>Returns bitwise flag combination of the <see cref="mmToolState"/> to specify if enabled.</returns>
        protected virtual mmToolState InternalEnabled(ID8EnumListItem enumItems, int itemCount)
        {
            return mmToolState.mmTSNone;
        }

        /// <summary>
        /// Executes the tree tool within error handling that reports all exceptions to the user via a <see cref="EventLogger"/>
        /// </summary>
        /// <param name="enumItems">The enumeration of selected items.</param>
        /// <param name="itemCount">The number of items selected.</param>
        protected virtual void InternalExecute(ID8EnumListItem enumItems, int itemCount)
        {
        }

        #endregion
    }
}