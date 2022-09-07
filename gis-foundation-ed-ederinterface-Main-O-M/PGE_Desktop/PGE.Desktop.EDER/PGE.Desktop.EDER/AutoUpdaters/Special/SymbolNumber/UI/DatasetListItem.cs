using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using log4net;

namespace PGE.Desktop.EDER.SymbolNumber.UI
{
    /// <summary>
    /// A wrapper for the DataSet class which can be added to ComboBox. 
    /// </summary>
    public class DatasetListItem
    {

        #region Private
        private string _name;
        private IDataset _dataset;
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Protected
        /// <summary>
        /// Protected constructor.  Use DatasetListItem(IDataset dataset) to initialize objects.
        /// </summary>
        protected DatasetListItem() { }
        #endregion Protected

        #region Public
        #region Properties

        /// <summary>
        /// Gets the dataset name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            private set { }
        }

        /// <summary>
        /// Gets the dataset object.
        /// </summary>
        public IDataset Dataset
        {
            get { return _dataset; }

            private set
            {
                _dataset = value;
                _name = _dataset.Name;
            }
        }
        #endregion Properties

        #region Overrides for DatasetListItem
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.  Used by the combo box control for display.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            //return the Name
            _logger.Debug("ToString:" + Name);
            return Name;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            //return hashcode of Name
            _logger.Debug("HashCode:" + Name.GetHashCode());
            return Name.GetHashCode();
        }
        #endregion Overrides for DatasetListItem
        #endregion Public

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetListItem"/> class.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public DatasetListItem(IDataset dataset)
        {
            Dataset = dataset;
        }
        #endregion Constructor
    }
}
