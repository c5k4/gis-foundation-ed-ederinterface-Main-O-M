using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Miner.Interop;
using Miner.Interop.Process;
using stdole;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// Base class for Tree Tools used to execute Px Tasks in either WorkflowManager or SessionManager. 
    /// You must provide your own COM Registration because they can be registered in multiple categories.
    /// </summary>
    public abstract class BasePxTreeTool : IMMTreeViewTool, IMMTreeViewToolEx, IMMTreeViewTool2, IMMTreeViewToolEx2
    {      
        /// <summary>
        /// The name of the task
        /// </summary>
        protected string _TaskName;
        /// <summary>
        /// The process framework application reference
        /// </summary>
        protected IMMPxApplication _PxApp;

        private int _Priority;
        private int _Category;
        private string _CategoryName;
        private string _ExtensionName;
        private IPictureDisp _Picture;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePxTreeTool"/> class.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="category">The category.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="extensionName">Name of the extension. (i.e MMSessionManager or MMWorkflowManager)</param>
        protected BasePxTreeTool(string taskName, int priority, int category, string categoryName, string extensionName)            
        {
            _TaskName = taskName;
            _Priority = priority;
            _Category = category;
            _CategoryName = categoryName;
            _ExtensionName = extensionName;            
        }

        #region Protected Members
        /// <summary>
        /// Executes the tree tool within error handling that reports all exceptions to the user via a <see cref="EventLogger"/>
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns><c>true</c> if the tool has executed successfully; otherwise <c>false</c></returns>
        protected virtual void InternalExecute(IMMTreeViewSelection selection)
        {
            // Only enable if 1 item is selected.
            if (selection.Count != 1) return;                

            // Execute the Task for the specified node.
            selection.Reset();
            IMMPxNode node = (IMMPxNode)selection.Next;
            IMMPxTask task = ((IMMPxNode3)node).GetTaskByName(this.Name);
            if (task == null) return;                

            task.Execute(node);
        }

        /// <summary>
        /// Determines of the tree tool is enabled for the specified selection of items.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns>
        /// Returns bitwise flag combination of the <see cref="mmToolState"/> to specify if enabled.
        /// </returns>
        protected virtual int InternalEnabled(IMMTreeViewSelection selection)
        {
            if (selection == null)
                return 0;

            if (selection.Count != 1)
                return 0;

            selection.Reset();
            IMMPxNode node = (IMMPxNode)selection.Next;
            IMMPxTask task = ((IMMPxNode3)node).GetTaskByName(this.Name);

            if (task == null)
                return 0;

            if (task.get_Enabled(node))
                return (int)(mmPxToolState.mmPxTSEnabled | mmPxToolState.mmPxTSMenuVisible);

            return 0;
        }

        /// <summary>
        /// Updates the bitmap image with the image from the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream of bitmap to load.</param>
        protected void UpdateBitmap(Stream stream)
        {
            try
            {
                Bitmap bitmap = new Bitmap(stream);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                _Picture = ActiveXConverter.ToPicture(bitmap);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, this.Name);
            }
        }
        #endregion

        #region IMMTreeViewTool Members

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public int Category
        {
            get { return _Category; }
        }

        /// <summary>
        /// Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="pSelection">The selection.</param>
        public void Execute(IMMTreeViewSelection pSelection)
        {
            try
            {                
                InternalExecute(pSelection);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, "Error Executing TreeTool " + this.Name);
            }            
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _TaskName; }
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
        /// Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="pSelection">The selection.</param>
        /// <returns>
        /// 	<c>true</c> if the tool should be enabled; otherwise <c>false</c>
        /// </returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            try
            {
                return InternalEnabled(pSelection);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex, "Error Enabling TreeTool " + this.Name);
            }

            return 0;
        }

        #endregion

        #region IMMTreeViewToolEx Members

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public IPictureDisp Bitmap
        {
            get { return _Picture; }
        }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName
        {
            get { return _CategoryName; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BasePxTreeTool"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="vSelection">The selection.</param>
        /// <returns>
        /// 	<c>true</c> if the tool should be enabled; otherwise <c>false</c>
        /// </returns>
        public int EnabledEx(object vSelection)
        {
            return get_Enabled((IMMTreeViewSelection)vSelection);
        }

        /// <summary>
        /// Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="vSelection">The selection.</param>
        /// <returns><c>true</c> if the tool executed; otherwise <c>false</c>.</returns>
        public bool ExecuteEx(object vSelection)
        {
            Execute((IMMTreeViewSelection)vSelection);
            return true;
        }

        /// <summary>
        /// Initializes the specified v init data.
        /// </summary>
        /// <param name="vInitData">The v init data.</param>
        public void Initialize(object vInitData)
        {
            if (vInitData is IMMPxApplication)
                _PxApp = (IMMPxApplication)vInitData;
        }

        /// <summary>
        /// Gets the sub category.
        /// </summary>
        /// <value>The sub category.</value>
        public int SubCategory
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip
        {
            get { return _TaskName; }
        }

        #endregion

        #region IMMTreeViewTool2 Members

        /// <summary>
        /// Gets a value indicating whether there are allow as default tool.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there are allow as default tool; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsDefaultTool
        {
            get { return false; }
        }

        #endregion

        #region IMMTreeViewToolEx2 Members

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        /// <value>The name of the extension.</value>
        public string ExtensionName
        {
            get { return _ExtensionName; }
        }

        #endregion
    }
}