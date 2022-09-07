using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Miner.Interop;
using Miner.ComCategories;


namespace PGE.Desktop.EDER.ArcMapCommands
{
	public abstract class TreeToolBase: IMMTreeTool
	{
		protected mmShortCutKey _shortCut = mmShortCutKey.mmShortcutNone;
		protected int _priority = 0;
		private Bitmap _bitmap;
		private IntPtr _hBitmap; 
		protected bool _allowAsDefault = true;
		protected string _name = "Unknown";
		protected int _category = 0;

		protected TreeToolBase()
		{
		}

		protected TreeToolBase	(string name, int category, int priority, bool allowAsDefault, mmShortCutKey shortCut)
		{
			_name = name;
			_category = category;
			_priority = priority ;
			_allowAsDefault = allowAsDefault;
			_shortCut = shortCut ;
		}

		protected TreeToolBase(string name, int category, int priority, bool allowAsDefault, mmShortCutKey shortCut, Bitmap bitmap):
			this(name, category, priority, allowAsDefault, shortCut)
		{
			InternalBitmap = bitmap;
		}

		protected virtual Bitmap InternalBitmap
		{
			get { return _bitmap;}
			set { 
				_bitmap = value;
				_bitmap.MakeTransparent(_bitmap.GetPixel(1,1));
				_hBitmap = _bitmap.GetHbitmap();
				// The GetHbitmap method creates an unmanaged Windows GDI bitmap object. 
				// You have to delete this object manually because the .NET runtime garbage
				// collector doesn't clear it up. See destructor.
			}
		}

		// Needed to clear up the Hbitmap unmanaged resource
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool DeleteObject(IntPtr hObject);

		~TreeToolBase()
		{
			// Must de-allocate HBitmap with Windows.DeleteObject
			if (_hBitmap.ToInt32() != 0)
			{
				DeleteObject(_hBitmap);
			}
		}


		#region Implementation of IMMTreeTool
		public abstract void Execute(Miner.Interop.ID8EnumListItem pEnumItems, int lItemCount);

		public virtual int get_Enabled(Miner.Interop.ID8EnumListItem pEnumItems, int lItemCount)
		{
			return (int)mmToolState.mmTSNone;
		}

		public virtual Miner.Interop.mmShortCutKey ShortCut
		{
			get { return _shortCut;}
		}

		public virtual int Priority
		{
			get {	return _priority;}
		}

		public virtual int Bitmap
		{
			get { return _hBitmap.ToInt32();}
		}

		public virtual bool AllowAsDefault
		{
			get { return _allowAsDefault;}
		}

		public virtual string Name
		{
			get { return _name;}
		}

		public virtual int Category
		{
			get { return _category;}
		}
		#endregion
	}
}
