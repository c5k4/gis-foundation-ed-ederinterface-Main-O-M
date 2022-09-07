using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;

using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.UI.Commands
{
    /// <summary>
    /// An abstract class for creating a command within ArcMap or ArcCatalog.
    /// </summary>
    public abstract class BaseArcGISCommand : BaseCommand
    {
        // private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        protected BaseArcGISCommand(string name, string caption, string category, string message, string toolTip)
            : base(null, caption, category, 0, null, message, name, toolTip)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the user clicks a command.
        /// </summary>
        /// <remarks>
        /// Note to inheritors: override OnClick and use this method to
        /// perform the actual work of the custom command.
        /// </remarks>
        public override void OnClick()
        {
            try
            {
                InternalClick();
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, this.Caption);
            }
        }

        /// <summary>
        /// Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">A reference to the application in which the command was created.
        /// The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        /// or an IHookHelper reference (for commands created on an Engine ToolbarControl).</param>
        /// <remarks>Note to inheritors: classes inheriting from BaseCommand must always
        /// override the OnCreate method. Use this method to store a reference to the host
        /// application, passed in via the hook parameter.</remarks>
        public override void OnCreate(object hook)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads specified bitmap for command.
        /// </summary>
        /// <param name="stream">Stream of bitmap to load.</param>
        protected void UpdateBitmap(Stream stream)
        {
            UpdateBitmap(stream, 0, 0);
        }

        /// <summary>
        /// Loads the specified bitmap for command and sets the transparence for the xy location.
        /// </summary>
        /// <param name="stream">Stream of bitmap to load.</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        protected void UpdateBitmap(Stream stream, int x, int y)
        {
            UpdateBitmap(new Bitmap(stream), x, y);
        }

        /// <summary>
        /// Loads the specified bitmap for command and sets the transparence for the xy location.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        protected void UpdateBitmap(Bitmap bitmap, int x, int y)
        {
            try
            {
                m_bitmap = bitmap;
                m_bitmap.MakeTransparent(m_bitmap.GetPixel(x, y));
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, this.Name);
            }
        }

        /// <summary>
        /// This method is called within the base class and is wrapped on error handling
        /// </summary>
        protected virtual void InternalClick()
        {
        }

        #endregion
    }
}