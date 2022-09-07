using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Display;
using Miner.Interop;
using Miner.ComCategories;
using PGE.Desktop.EDER.Utility;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER.AutoUpdaters.Attribute
{
    #region PNode Au
    /// <summary>
    /// Date: Dec-26-2018
	/// Module Name: Display PNode-IDs
	/// Description: This algorithm provides drop down combo box with CNODE-IDs, for user to choose from and
    ///              updates PNode field, and sets the relationship with FNM table.
	/// </summary>

    [ComponentCategory(ComCategory.AttrAutoUpdateStrategy)]
	[ComVisible(true)]
    [Guid("94F6E74D-5BE9-461A-968A-D9083A413A9B")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Attribute.PNodeIdDisplayAU")]
    public class PNodeIdDisplayAU : IMMAttrAUStrategy
	{
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        static public void Register(string regKey)
        {
            Miner.ComCategories.AttrAutoUpdateStrategy.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static public void UnRegister(string regKey)
        {
            Miner.ComCategories.AttrAutoUpdateStrategy.Unregister(regKey);
        }

        #endregion

        #region Constructor
        public PNodeIdDisplayAU()
		{
		}
		#endregion

		#region IMMAttrAUStrategy Members

		public string DomainName
		{
			get
			{
				return string.Empty;
			}
		}

		public string Name
		{
			get
			{
                return "PNode-ID Au";
            }
		}

		public object GetAutoValue(IObject pObj)
		{
			object autoValue = null;
			IFeature feature = null;
            PNodeInitialize.PNodeIDValue = null;
            PNodeInitialize.FnmGUIDValue = null;
            //string stringError = null;

			try
			{
                PNodeInitialize.Workspace = (IWorkspace)((IDataset)((IRow)pObj).Table).Workspace;
				// AppRef is a singleton
				IApplication application = GetAppRef();

				// This AU only is executed in ArcMap.  No need to surpress the message
				if(application == null)
				{
					return autoValue;
				}
                feature = pObj as IFeature;
				if(feature == null)
				{
                    throw new Exception(null);
				}

				// Check to see if the AutoUpdater is running in 
				// Feeder management mode. If so, exit.
				if(CheckAUMode(mmAutoUpdaterMode.mmAUMFeederManager))
				{
					return autoValue;
				}

                IObjectClass cls = feature.Class as IObjectClass;
                IDataset dset = cls as IDataset;
                // check for PNode feature class:
                if (!dset.Name.Equals(PNodeInitialize.PNODE_FC_NAME, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("The AU must be assigned to PNode point feature class.",
                        "PNODE ID AU",System.Windows.Forms.MessageBoxButtons.OK);
                    return autoValue;
                }

                //Call IDs form.
                PNodeIDDisplayForm displayform = new PNodeIDDisplayForm();
                displayform.ShowDialog();
                if (string.IsNullOrEmpty(PNodeInitialize.PNodeIDValue))
                {
                    throw new Exception(null);
                }

                if (string.IsNullOrEmpty(PNodeInitialize.FnmGUIDValue))
                {
                    MessageBox.Show("Failed to get FNM GUID.",
                        "PNODE ID AU",
                        System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(null);
                }
                PNodeInitialize.SetFldVl(feature as IRow, PNodeInitialize.FnmGUIDValue, PNodeInitialize.PNodeFNMGUIDFldIdx);
                return PNodeInitialize.PNodeIDValue; 
			}
			catch(Exception ex)
			{
                _logger.Error("PG&E Create New Sub-PNode feature autoupdater ", ex);
                throw new COMException("", (int)mmErrorCodes.MM_E_CANCELEDIT);
			}			
		}

		public ESRI.ArcGIS.Geodatabase.esriFieldType FieldType
		{
			get
			{
				return esriFieldType.esriFieldTypeString;
			}
		}

		#endregion

		#region Private methods
		// This function can be used to determine in which mode the AutoUpdater is
		// currently operating.  Certain AutoUpdaters might only be applicable when 
		// a certain mode is enabled.  See the definition of the mmAutoUpdaterMode
		// enumeration for AU modes.

		internal bool CheckAUMode(mmAutoUpdaterMode auMode)
		{
			bool bCheckAUMode = false;
			try
			{
				// MMAutoUpdater is a singleton
                IMMAutoUpdater autoUpdater = Miner.Geodatabase.AutoUpdater.Instance;
				bCheckAUMode = (autoUpdater.AutoUpdaterMode == auMode);
			}
			catch(Exception ex)
			{}

			return bCheckAUMode;
		}

		internal AppRef GetAppRef()
		{
			Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
			return Activator.CreateInstance(type) as AppRef;
        }
        #endregion
    }
    #endregion

    #region Initialize class
    internal class PNodeInitialize
    {
        /// <summary>
        /// This initialize class initializes the varibles like workspace, tables and classes.
        /// Also has function that loads all FNM data to a dictionary that will be used to populated drop-down list in the form.
        /// It also has some helper functions like 'Get Value' Set Value' and 'Get Cursor (search functionality).
        /// </summary>

        #region Private and internal variables
        private static IWorkspace _sdeWorkspace = null;        
        internal static IMouseCursor _gMouseCur = new MouseCursorClass();
        private static bool _blnErrOccured = false;
        private static ITable _FNMTable = null;
        private static IFeatureClass _PNodeFC = null;
        private static IRelationshipClass _fnm_pnode_relationshipClass = null;

        //Field Indices 
        private static int _FNM_GlobalIDFldIdx = -1;
        private static int _FNM_CNODEIDFldIdx = -1;
        private static int _FNM_BUSIDFldIdx = -1;
        private static int _PNode_FNM_GUIDFldIdx = -1;        
        private static int _PNode_CNODEIDFldIdx = -1;      
        private static string _IDValue = null;
        private static string _FNM_GUID = null;
        private static Int32 _FNM_OID = -1;        
        
        internal static Dictionary<string, string> _gCNODE_Guid_Dict = new Dictionary<string, string>();//Holds CNODEID and GUID form FNM table.
        private const String FNM_TABLE_NAME = "EDGIS.FNM";
        internal const String PNODE_FC_NAME = "EDGIS.SUBPNODE";
        internal const String GLOBALIDFldName = "GLOBALID";
        //PNode specific
        internal const string CNODEIDFldName = "CNODE_ID";
        internal const string PNODE_CNODEIDFldName = "CNODEID";
        internal const string BUSIDFldName = "BUS_ID";
        internal const string PNODE_FNM_GLOBALIDFldName = "FNMGUID";
        internal const string LATEST_FNM_RELEASEDATEFleName = "LATEST_FNM_RELEASEDATE";
        internal const string RelCls_FNM_PNODEName = "EDGIS.FNM_SUBPNODE";
        #endregion

        internal static bool InitialiseVariables()
        {
            try
            {
                //GetWorkspace(Initialize.SDEConnectionFile, out _sdeWorkspace);
                if (_sdeWorkspace == null)
                    throw new Exception("Failed to get SDE workspace");

                //initialise circuit source table.
                IObjectClass objectCls = null;
                _fnm_pnode_relationshipClass = null;

                //initialise FNM table.
                objectCls = null;
                OpenObjectClass(ref objectCls, _sdeWorkspace, FNM_TABLE_NAME);
                _FNMTable = objectCls as ITable;

                //initialise PNode feature class.
                objectCls = null;
                OpenObjectClass(ref objectCls, _sdeWorkspace, PNODE_FC_NAME, true);
                _PNodeFC = objectCls as IFeatureClass;

                //**Field Indices.
                _FNM_GlobalIDFldIdx = FieldManager.GetIndex((IObjectClass)_FNMTable, GLOBALIDFldName);
                _FNM_CNODEIDFldIdx = FieldManager.GetIndex((IObjectClass)_FNMTable, CNODEIDFldName);
                _FNM_BUSIDFldIdx = FieldManager.GetIndex((IObjectClass)_FNMTable, BUSIDFldName);
                _PNode_FNM_GUIDFldIdx = FieldManager.GetIndex((IObjectClass)_PNodeFC, PNODE_FNM_GLOBALIDFldName);
                _PNode_CNODEIDFldIdx = FieldManager.GetIndex((IObjectClass)_PNodeFC, PNODE_CNODEIDFldName);
                GetFNM_Values();
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            return (!ErrorOccured);
        }

        /// <summary>
        /// Search the FNM table and fill the results in a data dictionary.
        /// </summary>
        private static void GetFNM_Values()
        {
            //**Get access to FNM records, and grab CNODE and GUID.
            ICursor pCur = null;
            try
            {
                //PNODE_END_DATE >= TO_DATE('2009-06-24 00:00:00','YYYY-MM-DD HH24:MI:SS')
                //PNODE_START_DATE < = TO_DATE('2019-01-07 00:00:00','YYYY-MM-DD HH24:MI:SS')
                //PNODE_START_DATE < = TO_DATE('2019-01-07','YYYY-MM-DD')
                string todayDate = string.Format("{0:yyyy-MM-dd}", System.DateTime.Now);

                //Only get the latest date
                //string sqlDate = @"PNODE_START_DATE < =  TO_DATE('" + todayDate + "','YYYY-MM-DD')" +
                //"AND (PNODE_END_DATE IS NULL OR PNODE_END_DATE >= TO_DATE('" + todayDate + "','YYYY-MM-DD'))";
                //string sqlDate = @"LATEST_FNM_RELEASEDATE > = (SELECT MAX(LATEST_FNM_RELEASEDATE) FROM EDGIS.FNM WHERE LATEST_FNM_RELEASEDATE < =  TO_DATE('" + todayDate + "','YYYY-MM-DD'))";
                string sqlDate = LATEST_FNM_RELEASEDATEFleName + @" > = (SELECT MAX("+ LATEST_FNM_RELEASEDATEFleName + ") FROM " + FNM_TABLE_NAME + " WHERE " + 
                                 LATEST_FNM_RELEASEDATEFleName + " < =  TO_DATE('" + todayDate + "','YYYY-MM-DD'))";
                if (_gCNODE_Guid_Dict.Count > 0) return;
                pCur = null; 
                GetCursor(out pCur, _FNMTable,sqlDate);
                string CNODEVlue = null;
                string GuidVlu = null;
                string BusIdVlu = null;
                IRow FNMRow = pCur.NextRow();
                while (FNMRow != null)
                {
                    //**Get value/s and load to dictionary.                        
                    CNODEVlue = GetValue(FNMRow as IRow, _FNM_CNODEIDFldIdx);
                    BusIdVlu = GetValue(FNMRow as IRow, _FNM_BUSIDFldIdx);
                    GuidVlu = GetValue(FNMRow as IRow, _FNM_GlobalIDFldIdx);

                    if (string.IsNullOrEmpty(GuidVlu))
                    {
                        //generate new guid and also set it in FNM table, 
                        //so that relationship can be generated with PNODE table when used.
                        GuidVlu = Guid.NewGuid().ToString("b").ToUpper();
                        SetFldVl(FNMRow as IRow, GuidVlu, _FNM_GlobalIDFldIdx);
                        FNMRow.Store();
                    }

                    //add to dictionary.
                    //_gCNODE_Guid_Dict.Add(GuidVlu, CNODEVlue + " - " + BusIdVlu);
                    _gCNODE_Guid_Dict.Add(GuidVlu, CNODEVlue + " | " + BusIdVlu);
                    FNMRow = pCur.NextRow();
                }
                if (pCur != null)
                    while (Marshal.ReleaseComObject(pCur) != 0) { }; pCur = null;
            }
            catch (Exception Ex)
            { ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// Open an object class.
        /// </summary>
        /// <param name="objCl">parameter passed as reference, return the passed class name as object class.</param>
        /// <param name="workSpace">Workspace instance the class needs to be opened from.</param>
        /// <param name="className">class name of the object class to be opened</param>
        /// <param name="fc">if true then open feature class, else open table</param>
        internal static void OpenObjectClass(ref IObjectClass objCl, IWorkspace workSpace, string className, bool fc = false)
        {
            try
            {
                IFeatureWorkspace featureWorkSpace = (IFeatureWorkspace)workSpace;
                //ITable table = ModelNameFacade.ObjectClassByModelName(workspace, SchemaInfo.E.ClassModelNames.PGEProperties) as ITable;
                if (fc)
                    objCl = featureWorkSpace.OpenFeatureClass(className) as IObjectClass;
                else
                    objCl = featureWorkSpace.OpenTable(className) as IObjectClass;
                if (objCl == null)
                    throw (new Exception("Unable to opent the following objectClass: " + className));
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// Display the error on the screen.
        /// </summary>
        /// <param name="ex">Occured error execption</param>
        /// <param name="strFunctionName">Function name error occured at</param>
        internal static void ErrorMessage(Exception ex, string strFunctionName)
        {
            try
            {
                ErrorOccured = true;
                //put log for net message here.
                System.Windows.Forms.MessageBox.Show("[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception pException) { }
        }

        /// <summary>
        /// Will return value from the row.
        /// </summary>
        /// <param name="pRow">The row to read the value from.</param>
        /// <param name="intFldIndex">Index of the field.</param>
        /// <returns>"value fo the field of found or else null.</returns>
        internal static string GetValue(IRow pRow, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                    throw new Exception("Missing field index");

                if (pRow.get_Value(intFldIndex) != System.DBNull.Value)
                {
                    string strValue = pRow.get_Value(intFldIndex).ToString();
                    if (strValue != "")
                        return strValue;
                }
                return null;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Missing Field Index" ); return null; }
        }

        /// <summary>
        /// Will update the value of the field.
        /// </summary>
        /// <param name="pRow">The row  that needs to be updated.</param>
        /// <param name="intFldIndex">Index of the field.</param>
        /// <param name="strValue">Value that needs to be set.</param>
        internal static void SetFldVl(IRow pRow, string strValue, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                    throw new Exception("Missing field index");

                if(!string.IsNullOrEmpty(strValue))
                {
                    pRow.set_Value(intFldIndex, strValue);
                }
                else
                {
                    pRow.set_Value(intFldIndex, System.DBNull.Value);
                }
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Index: " + intFldIndex.ToString() + ", Field Value: " + strValue); }
        }

        /// <summary>
        /// Will return cursor of rows.
        /// </summary>
        /// <param name="pCur">Returns rows in a Cursor as out parameter.</param>
        /// <param name="pTable">Table that needs to be searched.</param>
        /// <param name="strWhereClause">where clause that searched is performed on</param>
        /// <returns> "True is executed successfully or else false.</returns>
        internal static bool GetCursor(out ICursor pCur, ITable pTable, string strWhereClause)
        {
            pCur = null;
            try
            {
                if (string.IsNullOrEmpty(strWhereClause))
                {
                    pCur = pTable.Search(null, false);
                    return true;
                }

                if (strWhereClause.Length == 0)
                    return false;
                IQueryFilter pQFilter = new QueryFilter();
                pQFilter.WhereClause = strWhereClause;
                pCur = pTable.Search(pQFilter, false);
                return true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }

        internal static void CreatePNodeFNMRelationship(Int32 nodeOId, Int32 fnmOID, IWorkspace workSpace)
        {
            try
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workSpace;
                IRow originRow = _FNMTable.GetRow(fnmOID) as IRow;
                IRow destRow = _PNodeFC.GetFeature(nodeOId) as IRow;

                if (_fnm_pnode_relationshipClass == null)
                    _fnm_pnode_relationshipClass = featureWorkspace.OpenRelationshipClass(RelCls_FNM_PNODEName);
                IRelationship relationship = _fnm_pnode_relationshipClass.CreateRelationship(originRow as IObject, destRow as IObject);
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);}

        }

        #region Encapsulated fields
        internal static bool ErrorOccured
        {
            get { return PNodeInitialize._blnErrOccured; }
            set { PNodeInitialize._blnErrOccured = value; }
        }
        internal static IWorkspace Workspace
        {
            get { return PNodeInitialize._sdeWorkspace; }
            set { PNodeInitialize._sdeWorkspace = value; }
        }
        internal static string PNodeIDValue
        {
            get { return PNodeInitialize._IDValue; }
            set { PNodeInitialize._IDValue = value; }
        }
        internal static string FnmGUIDValue
        {
            get { return PNodeInitialize._FNM_GUID; }
            set { PNodeInitialize._FNM_GUID = value; }
        }
        public static Int32 FNM_OID
        {
            get { return PNodeInitialize._FNM_OID; }
            set { PNodeInitialize._FNM_OID = value; }
        }

        internal static int FNMGlobalIDFldIdx
        {
            get { return PNodeInitialize._FNM_GlobalIDFldIdx; }
            set { PNodeInitialize._FNM_GlobalIDFldIdx = value; }
        }
        internal static int FNMCNODEIDFldIdx
        {
            get { return PNodeInitialize._FNM_CNODEIDFldIdx; }
            set { PNodeInitialize._FNM_CNODEIDFldIdx = value; }
        }
        internal static int FNMBUSIDFldIdx
        {
            get { return PNodeInitialize._FNM_BUSIDFldIdx; }
            set { PNodeInitialize._FNM_BUSIDFldIdx = value; }
        }
        internal static int PNodeFNMGUIDFldIdx
        {
            get { return PNodeInitialize._PNode_FNM_GUIDFldIdx; }
            set { PNodeInitialize._PNode_FNM_GUIDFldIdx = value; }
        }
        internal static int PNodeCNODEIDFldIdx
        {
            get { return PNodeInitialize._PNode_CNODEIDFldIdx; }
            set { PNodeInitialize._PNode_CNODEIDFldIdx = value; }
        }
        public static ITable FNMTable
        {
            get { return PNodeInitialize._FNMTable; }
            set { PNodeInitialize._FNMTable = value; }
        }
        
        #endregion 
    }
    #endregion
}
