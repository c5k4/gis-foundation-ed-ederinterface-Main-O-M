using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace PGE.Common.Delivery.Systems.Xml
{
    /// <summary>
    /// A supporting class used to transform XML using an Style Sheet.
    /// </summary>
    public static class XmlTransform
    {
        #region Public Methods

        /// <summary>
        /// Transforms the specified input XML file using the stylesheet.
        /// </summary>
        /// <param name="stylesheetUri">The stylesheet URI.</param>
        /// <param name="inputUri">The input URI.</param>
        /// <param name="resultsFile">The results file.</param>
        public static void Transform(string stylesheetUri, string inputUri, string resultsFile)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(stylesheetUri);
            xsl.Transform(inputUri, resultsFile);
        }

        /// <summary>
        /// Transforms the specified input XML file using the stylesheet.
        /// </summary>
        /// <param name="stylesheetUri">The stylesheet URI.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="stylesheetResolver">The stylesheet resolver.</param>
        /// <param name="inputUri">The input URI.</param>
        /// <param name="resultsFile">The results file.</param>
        public static void Transform(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver, string inputUri, string resultsFile)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(stylesheetUri, settings, stylesheetResolver);
            xsl.Transform(inputUri, resultsFile);
        }

        #endregion
    }
}