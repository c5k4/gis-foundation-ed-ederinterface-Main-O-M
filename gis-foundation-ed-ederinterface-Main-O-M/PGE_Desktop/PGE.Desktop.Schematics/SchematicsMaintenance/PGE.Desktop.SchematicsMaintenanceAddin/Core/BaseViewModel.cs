// Copyright 2013 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/index.html#/Copyright_information/00010000009s000000/
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    /// <summary>
    /// Base object for view models
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Constants

        #endregion

        #region Members

        private string _error = string.Empty;

        #endregion

        #region Constructors/Destructor

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event for notifying of properties that change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Raises the PropertyChanged event for a changed property.
        /// </summary>
        /// <param name="propertyName">The name of property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            //CheckPropertyName(propertyName);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs only when debugging to isolate any propertyName issues quickly.
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        private void CheckPropertyName(string propertyName)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this)[propertyName];
            if (propertyDescriptor == null)
            {
                throw new InvalidOperationException(string.Format(null,
                    "The property with the propertyName '{0}' doesn't exist.", propertyName));
            }
        }
        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// Gets the current error, or string.empty if none.
        /// </summary>
        public virtual string Error 
        {
            get
            {
                return _error;
            }
            set
            {
                this._error = value;
                RaisePropertyChanged("Error");
            }
        }

        /// <summary>
        /// Tests to see if the given item has a validation error.
        /// </summary>
        /// <param name="propertyName">
        /// The property to check for validity.
        /// </param>
        /// <returns>The error associated with the item.</returns>
        public virtual string this[string propertyName] 
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
