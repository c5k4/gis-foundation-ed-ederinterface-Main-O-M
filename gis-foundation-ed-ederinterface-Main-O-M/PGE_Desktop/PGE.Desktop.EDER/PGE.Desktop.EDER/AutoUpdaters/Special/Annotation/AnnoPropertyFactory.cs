using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Diagnostics;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.Special.Annotation
{
    public class AnnoPropertyFactory
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Public static methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <returns></returns>
        public static ISymbol AnnoSymbolForFeature(IFeature feature)
        {
            //Return null if the feature is null or the feature passed in is not an annotation feature.
            if(feature==null || !(feature is AnnotationFeature)) 
                return null; 
            IFeatureClass annoFC = feature.Class as IFeatureClass;
            string symbolName = FeatureSymbolName(feature);
            //The Annotation FeatureClass does not have Sub Classes. So Return the first Symbol
            //Assumes the Symbol Name and Annotation Class Name matches - In real life this need not be the case
            ISymbol symbol = SymbolFromSymbolName(annoFC, symbolName);
            return symbol;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public static ISymbol SymbolFromSymbolName(IFeatureClass featureClass, string symbolName)
        {
            ISymbol retVal = null;
            //If the featurecalss passed in is not a Annotation Featureclass simply return null
            if (!(featureClass.Extension is IAnnotationClassExtension2)) return null;
            IAnnotationClassExtension2 pAnnoClsExtn = featureClass.Extension as IAnnotationClassExtension2;

            IAnnoClass pAnnoCls = pAnnoClsExtn as IAnnoClass;
            ISymbolCollection2 pSymColl2 = pAnnoCls.SymbolCollection as ISymbolCollection2;
            pSymColl2.Reset();
            ISymbolIdentifier2 pSymID2 = null;
            while ((pSymID2 = pSymColl2.Next() as ISymbolIdentifier2) != null)
            {
                if (pSymID2.Name == symbolName || string.IsNullOrEmpty(symbolName))
                {
                    retVal = pSymID2.Symbol;
                    break;
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static esriTextHorizontalAlignment HorizontalAlignment(IFeature feature, esriTextHorizontalAlignment defaultValue)
        {
            esriTextHorizontalAlignment retVal = defaultValue;
            ISymbol symbol = AnnoSymbolForFeature(feature);
            if (symbol != null && symbol is ITextSymbol)
            {
                retVal = ((ITextSymbol)symbol).HorizontalAlignment;
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static esriTextVerticalAlignment VerticalAlignment(IFeature feature, esriTextVerticalAlignment defaultValue)
        {
            esriTextVerticalAlignment retVal = defaultValue;
            ISymbol symbol = AnnoSymbolForFeature(feature);
            if (symbol != null && symbol is ITextSymbol)
            {
                retVal = ((ITextSymbol)symbol).VerticalAlignment;
            }
            return retVal;
        }
        #endregion Public static methods

        #region private methods
        /// <summary>
        /// Returns subtypes name.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <returns></returns>
        private static string FeatureSymbolName(IFeature feature)
        {
            string retVal = string.Empty;
            ISubtypes subtypes=(ISubtypes)feature.Class;
            _logger.Debug("Checking for Subtypes.");
            if (subtypes.HasSubtype)
            {
                _logger.Debug("Subtypes found, getting subtype code and subtype name.");
                retVal = subtypes.get_SubtypeName(((IRowSubtypes)feature).SubtypeCode);
            }

            _logger.Debug("Returning subtype name.");
            return retVal;
        }
        #endregion
    }
}
