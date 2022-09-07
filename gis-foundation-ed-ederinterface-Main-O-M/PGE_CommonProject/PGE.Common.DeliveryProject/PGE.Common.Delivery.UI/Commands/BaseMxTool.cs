using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ESRI.ArcGIS.SystemUI;
using PGE.Common.Delivery.UI.Commands;

namespace PGE.Common.Delivery.UI.Tools
{
    /// <summary>
    /// Base class ArcMap tool
    /// </summary>
    public abstract class BaseMxTool : BaseArcGISCommand, ITool 
    {
        private const string DEFUALT_CURSOR_RES = "PGE.Common.Delivery.UI.Properties.Resources.DefaultCursor.cur";
        
        /// <summary>
        /// 
        /// </summary>
        protected Cursor _Cursor;

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Caption"></param>
        /// <param name="Category"></param>
        /// <param name="Message"></param>
        /// <param name="Tooltip"></param>
        /// <param name="Bitmap"></param>
        /// <param name="Cursor"></param>
        protected BaseMxTool(string Name, string Caption, string Category, string Message, string Tooltip, Bitmap Bitmap, Cursor Cursor)
            : base(Name, Caption, Category, Message, Tooltip)
        {
            m_bitmap = Bitmap;
            _Cursor = Cursor;
        }

        /// <summary>
        /// This constructor uses a default cursor from assembly resource 
        /// and no bitmap for the command - therefore displays the Name with an image
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Caption"></param>
        /// <param name="Category"></param>
        /// <param name="Message"></param>
        /// <param name="Tooltip"></param>
        protected BaseMxTool(string Name, string Caption, string Category, string Message, string Tooltip)
            : base(Name, Caption, Category, Message, Tooltip)
        {
            
            try
            {
                
                System.Reflection.Assembly ExecAssem = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream CurStream = ExecAssem.GetManifestResourceStream(DEFUALT_CURSOR_RES);
                _Cursor = new System.Windows.Forms.Cursor(CurStream);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
            


        }
        #endregion
        #region ITool overrides
        /// <summary>
        /// 
        /// </summary>
        public int Cursor
        {
            get
            {
                if (_Cursor == null) return 0;

                return _Cursor.Handle.ToInt32();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Deactivate()
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool OnContextMenu(int x, int y)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void OnDblClick()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="shift"></param>
        public virtual void OnKeyDown(int keyCode, int shift)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="shift"></param>
        public virtual void OnKeyUp(int keyCode, int shift)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void OnMouseDown(int button, int shift, int x, int y)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void OnMouseMove(int button, int shift, int x, int y)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void OnMouseUp(int button, int shift, int x, int y)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdc"></param>
        public virtual void Refresh(int hdc)
        {
            
        }
        #endregion
    }
}
