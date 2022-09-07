using System;
using System.Runtime.InteropServices;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;


namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Abstract base class for Auto Text Element.
    /// </summary>
    public abstract class BaseAutoTextElement : IMMAutoTextSource
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        protected string _DefaultText;

        private string _Caption;
        private string _Message;
        private string _ProgID;

        #endregion

        #region Constructors

        /// <summary>
        /// This is the constructor for the BaseAutoTextElement class.
        /// </summary>
        /// <param name="ProgID"></param>
        /// <param name="DefaultText"></param>
        /// <param name="Caption"></param>
        /// <param name="Message"></param>
        public BaseAutoTextElement(string ProgID, string DefaultText, string Caption, string Message)
        {
            _ProgID = ProgID;
            _Caption = Caption;
            _Message = Message;
            _DefaultText = DefaultText;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public string Caption
        {
            get { return _Caption; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Message
        {
            get { return _Message; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProgID
        {
            get
            {
                return _ProgID;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is in charge of calling all the other 'Event' functions.  It will default using the Default Text for the ATE, and will automatically replace any empty strings with a string containing a single space, to prevent ArcGIS from deleting the text element from the page template.
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        public string TextString(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            string returnText = _DefaultText;
            try
            {
                _logger.Debug(DateTime.Now.ToLongTimeString() + " ==== BEGIN EXECUTE: " + this.Caption);

                switch (eTextEvent)
                {
                    case mmAutoTextEvents.mmPrint:
                    case mmAutoTextEvents.mmStartPlot:
                        returnText = OnStart(eTextEvent, pMapProdInfo);
                        break;
                    case mmAutoTextEvents.mmPlotNewPage:
                        returnText = GetText(eTextEvent, pMapProdInfo);
                        break;
                    case mmAutoTextEvents.mmCreate:
                        returnText = OnCreate(eTextEvent, pMapProdInfo);
                        break;
                    case mmAutoTextEvents.mmFinishPlot:
                        returnText = OnFinish(eTextEvent, pMapProdInfo);
                        break;
                    case mmAutoTextEvents.mmRefresh:
                        returnText = OnRefresh(eTextEvent, pMapProdInfo);
                        break;
                    case mmAutoTextEvents.mmDraw:
                        returnText = OnDraw(eTextEvent, pMapProdInfo);
                        break;
                    default:
                        break;
                }
                _DefaultText = returnText;
                //If you return an empty string,
                //ArcGIS will delete your autotext
                //element from the page template
                if (string.IsNullOrEmpty(returnText))
                    return " ";
                else
                    return returnText;
            }
            catch (Exception ex)
            {
                _logger.Error("Could not build Auto Text string for " + _ProgID, ex);
                return " ";
            }
            finally
            {
                endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                _logger.Debug("TOTAL EXECUTION TIME ==== " + ts.TotalSeconds.ToString());

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <returns></returns>
        public virtual bool NeedRefresh(mmAutoTextEvents eTextEvent)
        {
            return true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method will fire every time a page is plotted/printed.  This is ONLY used by the ArcFM framework.  It will not be called by Export Map or Print Preview.
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected abstract string GetText(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo);

        /// <summary>
        /// This method will be called when the ATE is created (i.e. placed on the page)
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected virtual string OnCreate(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return _DefaultText;
        }

        /// <summary>
        /// This method will be called whenever the ATE is drawn
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected virtual string OnDraw(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return _DefaultText;
        }

        /// <summary>
        /// This method will be called whenever the ATE has finished plotting.  This is ONLY used by the ArcFM framework.  It will not be called by Export Map or Print Preview.
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected virtual string OnFinish(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return _DefaultText;
        }

        /// <summary>
        /// This method will be called whenever the ATE is forcibly refreshed by ArcFM.
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected virtual string OnRefresh(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return _DefaultText;
        }

        /// <summary>
        /// This method will be called whenever the printing/plotting begins.
        /// </summary>
        /// <param name="eTextEvent"></param>
        /// <param name="pMapProdInfo"></param>
        /// <returns></returns>
        protected virtual string OnStart(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return _DefaultText;
        }

        #endregion

        #region Private Methods

        [ComRegisterFunction]
        static void RegisterFunction(string regKey)
        {
            MMCustomTextSources.Register(regKey);
        }

        [ComUnregisterFunction]
        static void UnregisterFunction(string regKey)
        {
            MMCustomTextSources.Unregister(regKey);
        }

        #endregion
    }
}