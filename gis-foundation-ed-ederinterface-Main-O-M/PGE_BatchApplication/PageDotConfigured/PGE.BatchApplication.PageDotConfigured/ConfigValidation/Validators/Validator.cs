using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Json;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Xml;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.PageDotConfigured.ConfigValidation.Validators
{
    public abstract class Validator
    {
        protected static Log4NetLogger Logger;
        protected static JsonDataDictionaryCache JsonDataCache;
        protected List<XmlCheckItem> Errors;
        protected PositionXmlDocument XmlSchema;

        protected Validator()
        {
            JsonDataCache = JsonDataDictionaryCache.Instance;
            Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.PageDotConfigured.log4net.config");
        }

        protected abstract IEnumerable<XmlCheckItem> CompileFullNodeList();
        protected abstract IEnumerable<XmlCheckItem> ValidateNodes(IEnumerable<XmlCheckItem> items);

        public IEnumerable<XmlCheckItem> Run()
        {
            IEnumerable<XmlCheckItem> list = CompileFullNodeList();
            return ValidateNodes(list);
        }

        /// <summary>
        ///     Small helper method to iterate through all variants of tag names due to XML tag name inconsistency
        ///     and XML tag name case sensitivity
        /// </summary>
        /// <param name="node">The node we are searching through</param>
        /// <param name="variants">All variants of the tag name we are looking for</param>
        /// <returns>The value assigned to the first variant encountered</returns>
        protected static string GetValueForTagVariants(XmlElement node, IEnumerable<string> variants)
        {
            string val = null;

            foreach (string variant in variants)
            {
                val = node.GetAttribute(variant);
                if (!String.IsNullOrEmpty(val)) return val;
            }

            return val;
        }
    }
}
