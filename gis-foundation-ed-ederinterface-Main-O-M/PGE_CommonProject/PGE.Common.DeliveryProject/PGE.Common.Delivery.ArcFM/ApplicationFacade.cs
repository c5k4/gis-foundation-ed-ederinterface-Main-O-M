using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Framework;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Singleton with helper methods to get the  
    /// </summary>
    public class ApplicationFacade
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IMMAttributeEditor AttributeEditor()
        {
            UID uid = new UID();
            uid.Value = "mmDesktop.MMAttributeEditor";
            IMMAttributeEditor ae = (IMMAttributeEditor)Editor.FindExtension(uid);
            return ae;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IMMAttributeEditorUI2 AttributeEditorUI()
        {
            IMMAttributeEditor ae = AttributeEditor();
            if (ae != null)
            {
                IMMAttributeEditorUI aeUI = ae.UI;
                return (IMMAttributeEditorUI2)aeUI;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeEditor"></param>
        /// <returns></returns>
        public static int GetDesignTabPosition(IMMAttributeEditorUI2 attributeEditor)
        {
            int retVal = -1;
            int pageCount = attributeEditor.PageCount();
            for (int pagePosition = 0; pagePosition < pageCount - 1; pagePosition++)
            {
                if (attributeEditor.PageCaption(pagePosition).ToUpper().Equals("DESIGN"))
                {
                    retVal = pagePosition;
                    break;
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        public static ID8List QAQCTopLevel
        {
            get
            {
                if (Application != null)
                {
                    return Application.FindExtensionByName("QAQCTopLevel") as ID8List;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IApplication Application
        {
            get
            {
                try
                {
                    Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object obj = Activator.CreateInstance(type);
                    IApplication app = (IApplication)obj;
                    return app;
                }
                catch
                {
                    // Couldn't create the AppRef.  Probably not in ArcMap or ArcCatalog.
                    return null;
                }
            }
        }
    }
}
