using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;

namespace PGE.Common.Delivery.UI.Framework
{
    /// <summary>
    /// The form could be used to build a Query Expression window for Engine and Desktop application.
    /// Eg: QueryExpressionBuilder expressionBuilder = new QueryExpressionBuilder(ITable, initialExpression[this can be an empty string]);
    ///     expressionBuilder.ShowDialog();
    ///     retVal = expressionBuilder.QueryExpression;
    /// </summary>
    public partial class QueryExpressionBuilder : Form
    {
        ITable _table;
        IQueryPropertyPage2 _queryPropPage;
        string _queryExp, _initialExpression;
        bool _ignoreFlag = false;

        #region CTOR
        private QueryExpressionBuilder()
        {
            InitializeComponent();
        }
        //public QueryExpressionBuilder(IApplication application, ITable table,string initialExpression): this()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="initialExpression"></param>
        public QueryExpressionBuilder(ITable table,string initialExpression): this()
        {
            //if (null == application) throw new NullReferenceException("Application object cannot be null");
            if (null == table) throw new NullReferenceException("Table or Object class cannot be null");
            _queryExp = string.Empty; 
            //_app= application;
            _table = table;
            _initialExpression = initialExpression;
        }
        #endregion

        #region Destructor
        /// <summary>
        /// 
        /// </summary>
        ~QueryExpressionBuilder()
        {
            if (_queryPropPage != null)
            {
                ComReleaser.ReleaseCOMObject(_queryPropPage);  
            }
        }
        #endregion

        #region Form Events
        private void QueryExpressionBuilder_Load(object sender, EventArgs e)
        {
            IPropertyPage propPage;
            IPropertyPageSiteConfig propPageSiteConfig;
            _queryPropPage = new QueryPropertyPageClass();
            _queryPropPage.ExpressionLabel = "select from " + ((IDataset)_table).Name + " where ";

            if (String.IsNullOrEmpty (_initialExpression) == false) _ignoreFlag  = true;

            _queryPropPage.Expression = _initialExpression;  
            _queryPropPage.Table = _table;
            propPage = (IPropertyPage)_queryPropPage;
            propPageSiteConfig = new ComPropertyPageSiteClass();
            IntPtr inPtr = groupBox1.Handle;
            propPageSiteConfig.hWnd = inPtr.ToInt32();
            propPageSiteConfig.Page = propPage;
            propPage.SetPageSite((IPropertyPageSite)propPageSiteConfig);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _queryExp = _queryPropPage.QueryFilter.WhereClause;
            //PHI 041209
            if (_ignoreFlag == false)
            {
                // commented by Liren for testing

                //if (_queryExp.Length > 0)
                //{
                //    _queryExp = RemoveFirstOccur(_queryExp, "[", " ");
                //    _queryExp = RemoveFirstOccur(_queryExp, "]", " ");
                //}
            }

            //
            if(_queryPropPage!=null) _queryPropPage.FreeCursors();
            this.Close();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public string QueryExpression
        {
            get { return _queryExp; }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceFirstOccur(string original, string oldValue, string newValue) 
        {
            int loc = original.IndexOf(oldValue); 
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string RemoveFirstOccur(string original, string oldValue, string newValue)
        {
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length);
        }
    }
}