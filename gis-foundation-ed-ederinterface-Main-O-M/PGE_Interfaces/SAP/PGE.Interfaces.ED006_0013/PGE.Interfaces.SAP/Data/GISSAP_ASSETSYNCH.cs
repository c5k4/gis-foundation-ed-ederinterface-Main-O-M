using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// This class encapsulates the GIS SAP data Table from DB.
    /// </summary>
    /// <KeyProperties>
    /// ASSETID
    /// </KeyProperties>

    public class GISSAP_ASSETSYNCH : INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new GISSAP_ASSETSYNCH object.
        /// </summary>
        /// <param name="aSSETID">Initial value of ASSETID.</param>
        /// <param name="aCTIONTYPE">Initial value of ACTIONTYPE.</param>
        /// <param name="tYPE">Initial value of TYPE.</param>
        /// <param name="dATEPROCESSED">Initial value of DATEPROCESSED.</param>
        /// <param name="sAPATTRIBUTES">Initial value of SAPATTRIBUTES.</param>
        public static GISSAP_ASSETSYNCH CreateGISSAP_ASSETSYNCH(string aSSETID, string aCTIONTYPE, short tYPE, global::System.DateTime dATEPROCESSED, string sAPATTRIBUTES)
        {
            GISSAP_ASSETSYNCH gISSAP_ASSETSYNCH = new GISSAP_ASSETSYNCH();
            gISSAP_ASSETSYNCH.ASSETID = aSSETID;
            gISSAP_ASSETSYNCH.ACTIONTYPE = aCTIONTYPE;
            gISSAP_ASSETSYNCH.TYPE = tYPE;
            gISSAP_ASSETSYNCH.DATEPROCESSED = dATEPROCESSED;
            gISSAP_ASSETSYNCH.SAPATTRIBUTES = sAPATTRIBUTES;
            return gISSAP_ASSETSYNCH;
        }
        /// <summary>
        /// ASSETID
        /// </summary>
        public string ASSETID
        {
            get
            {
                return this._ASSETID;
            }
            set
            {                
                this._ASSETID = value;
                OnPropertyChanged("ASSETID");
            }
        }
        private string _ASSETID;

        /// <summary>
        ///  ACTIONTYPE.
        /// </summary>

        public string ACTIONTYPE
        {
            get
            {
                return this._ACTIONTYPE;
            }
            set
            {                
                this._ACTIONTYPE = value;
                OnPropertyChanged("ACTIONTYPE");
            }
        }

        private string _ACTIONTYPE;

        /// <summary>
        /// property TYPE in the schema.
        /// </summary>
        
        public short TYPE
        {
            get
            {
                return this._TYPE;
            }
            set
            {
                this._TYPE = value;
                OnPropertyChanged("TYPE");
            }
        }
        
        private short _TYPE;        
        
        /// <summary>
        /// property DATEPROCESSED in the schema.
        /// </summary>
        
        public System.DateTime DATEPROCESSED
        {
            get
            {
                return this._DATEPROCESSED;
            }
            set
            {                
                this._DATEPROCESSED = value;
                OnPropertyChanged("DATEPROCESSED");
            }
        }
       
        private System.DateTime _DATEPROCESSED;
       
        /// <summary>
        /// property SAPATTRIBUTES in the schema.
        /// </summary>
        
        public string SAPATTRIBUTES
        {
            get
            {
                return this._SAPATTRIBUTES;
            }
            set
            {
                this._SAPATTRIBUTES = value;
                OnPropertyChanged("SAPATTRIBUTES");
            }
        }
       
        private string _SAPATTRIBUTES;
       
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Event to notify subscribers a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        /// <summary>
        /// Raises a property changed notification for the specified property name.
        /// </summary>
        /// <param name="propName">The name of the property that changed.</param>

        #region Code Added for EDGIS ReArch on 11/feb/2021
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rECORDID">Initial value of RECORDID.</param>
        /// <param name="bATCHID">Initial value of BATCHID.</param>
        /// <param name="cREATIONDATE">Initial value of CREATIONDATE.</param>
        /// <param name="pROCESSSEDFLAG">Initial value of PROCESSEDFLAG.</param>
        /// <param name="pROCESSSEDTIME">Initial value of PROCESSEDTIME.></param>
        /// <param name="eRRORDESCRIPTION">Initial value of ERRORDESCRIPTION.</param>
        /// <param name="rECORDTYPE">Initial value of RECORDTYPE.</param>
        /// <param name="gISDATA">Initial value of GISDATA.</param>
        /// <param name="gUID">Initial value of GUID.</param>
        /// <returns></returns>
        public static GISSAP_ASSETSYNCH CreateGISSAP_ASSETSYNCH1(string rECORDID, string bATCHID, global::System.DateTime cREATIONDATE, string pROCESSSEDFLAG, string pROCESSSEDTIME, string eRRORDESCRIPTION, string rECORDTYPE, string gISDATA, string gUID)
        {
            GISSAP_ASSETSYNCH gISSAP_ASSETSYNCH = new GISSAP_ASSETSYNCH();
            gISSAP_ASSETSYNCH.RECORDID = rECORDID;
            gISSAP_ASSETSYNCH.BATCHID = bATCHID;
            gISSAP_ASSETSYNCH.CREATIONDATE = cREATIONDATE;
            gISSAP_ASSETSYNCH.PROCESSSEDFLAG =pROCESSSEDFLAG;
            gISSAP_ASSETSYNCH.PROCESSSEDTIME = pROCESSSEDTIME;
            gISSAP_ASSETSYNCH.ERRORDESCRIPTION = eRRORDESCRIPTION;
            gISSAP_ASSETSYNCH.RECORDTYPE = rECORDTYPE;
            gISSAP_ASSETSYNCH.GISDATA = gISDATA;
            gISSAP_ASSETSYNCH.GUID = gUID;
            return gISSAP_ASSETSYNCH;
        }

        /// <summary>
        /// RECORDID
        /// </summary>
        public string RECORDID
        {
            get
            {
                return this._RECORDID;
            }
            set
            {
                this._RECORDID = value;
                OnPropertyChanged("RECORDID");
            }
        }
        private string _RECORDID;

        /// <summary>
        /// BATCHID
        /// </summary>
        public string BATCHID
        {
            get
            {
                return this._BATCHID;
            }
            set
            {
                this._BATCHID = value;
                OnPropertyChanged("BATCHID");
            }
        }
        private string _BATCHID;


        /// <summary>
        /// CREATIONDATE
        /// </summary>
        public System.DateTime CREATIONDATE
        {
            get
            {
                return this._CREATIONDATE;
            }
            set
            {
                this._CREATIONDATE = value;
                OnPropertyChanged("CREATIONDATE");
            }
        }
        private System.DateTime _CREATIONDATE;

        /// <summary>
        /// PROCESSSEDFLAG
        /// </summary>
        public string  PROCESSSEDFLAG
        {
            get
            {
                return this._PROCESSSEDFLAG;
            }
            set
            {
                this._PROCESSSEDFLAG = value;
                OnPropertyChanged("PROCESSSEDFLAG");
            }
        }
        private string  _PROCESSSEDFLAG;

        /// <summary>
        /// PROCESSSEDTIME
        /// </summary>
        public string PROCESSSEDTIME
        {
            get
            {
                return this._PROCESSSEDTIME;
            }
            set
            {
                this._PROCESSSEDTIME = value;
                OnPropertyChanged("PROCESSSEDTIME");
            }
        }
        private string _PROCESSSEDTIME;

        /// <summary>
        /// ERRORDESCRIPTION
        /// </summary>
        public string ERRORDESCRIPTION
        {
            get
            {
                return this._ERRORDESCRIPTION;
            }
            set
            {
                this._ERRORDESCRIPTION = value;
                OnPropertyChanged("ERRORDESCRIPTION");
            }
        }
        private string _ERRORDESCRIPTION;

        /// <summary>
        /// RECORDTYPE
        /// </summary>
        public string RECORDTYPE
        {
            get
            {
                return this._RECORDTYPE;
            }
            set
            {
                this._RECORDTYPE = value;
                OnPropertyChanged("RECORDTYPE");
            }
        }
        private string _RECORDTYPE;

        /// <summary>
        /// GISDATA
        /// </summary>
        public string GISDATA
        {
            get
            {
                return this._GISDATA;
            }
            set
            {
                this._GISDATA = value;
                OnPropertyChanged("GISDATA");
            }
        }
        private string _GISDATA;

        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return this._GUID;
            }
            set
            {
                this._GUID = value;
                OnPropertyChanged("GUID");
            }
        }
        private string _GUID;
        /// <summary>
        /// This is the type of asset
        /// </summary>
        public string ASSETTYPE
        {
            get
            {
                return this._ASSETTYPE;
            }
            set
            {
                this._ASSETTYPE = value;
                OnPropertyChanged("ASSETTYPE");
            }
        }

        private string  _ASSETTYPE;

        #endregion

        protected virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
