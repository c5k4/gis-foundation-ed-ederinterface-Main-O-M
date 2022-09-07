using System.Collections.Generic;
using System.Xml;
using log4net;


namespace PGE.Interfaces.SAP.WOSynchronization
{
    /// <summary>
    /// Class for converting XML Data into IRowData2
    /// </summary>
    public class XMLProcessor
    {
        #region Private Variables
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion Private Variables

        #region Public Methods

        /// <summary>
        /// Convert XmlNode childNodes to List of IRowData
        /// </summary>
        /// <param name="jobOrderTopNode">Top Node in XML</param>
        /// <returns>List of IRowData having the message information contained in the <paramref name="jobOrderTopNode"/> XML node</returns>
        public List<IRowData2> ConvertToIRowData(XmlNode jobOrderTopNode)
        {
            List<IRowData2> lstRowData = new List<IRowData2>();
            XmlNode jobOrderNode = null;
            IRowData2 rData = null;

            //Checks whether XMLNode have childNodes
            if (jobOrderTopNode.HasChildNodes)
            {
                for (int i = 0; i < jobOrderTopNode.ChildNodes.Count; i++)
                {
                    jobOrderNode = jobOrderTopNode.ChildNodes[i];
                    //Ignore the commented nodes
                    if (jobOrderNode is XmlComment) continue;
                    //Populate IRowData with JobOrder Values
                    rData = FillRowData(jobOrderNode);
                    //Validate and add the data
                    if (rData != null)
                    {
                        lstRowData.Add(rData);
                    }
                }
            }
            return lstRowData;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Convert XMLNode into IRowData2.
        /// </summary>
        /// <param name="jobOrderNode">XMLNode with JobOrder and its related message information.</param>
        /// <returns>IRowData2 with JobOrder information.</returns>
        private IRowData2 FillRowData(XmlNode jobOrderNode)
        {
            IRowData2 rData = new RowData2();
            //Nodes name and value collection for every JobOrderNumber
            Dictionary<string, string> rowDataValues = new Dictionary<string, string>();
            string jobOrderNo = string.Empty;
            XmlNode childNode;

            if (jobOrderNode.HasChildNodes)
            {
                //Get the name and value of each field/xml node
                for (int i = 0; i < jobOrderNode.ChildNodes.Count; i++)
                {
                    //Get node at index
                    childNode = jobOrderNode.ChildNodes[i];
                    //Ignore the commented node
                    if (childNode is XmlComment) continue;
                    //Add Name and value to the Dictionary
                    rowDataValues.Add(childNode.Name.ToUpper(), childNode.InnerText);
                }

                //Checks jobOrder for null values
                if (rowDataValues.ContainsKey(ResourceConstants.XMLNodeNames.JOBORDERNODENAME.ToUpper()))
                {
                    jobOrderNo = rowDataValues[ResourceConstants.XMLNodeNames.JOBORDERNODENAME.ToUpper()];
                    if (!string.IsNullOrEmpty(jobOrderNo))
                    {
                        //Assign FacilityID and Node Name/Value collection
                        rData.FacilityID = jobOrderNo;
                        rData.FieldValues = rowDataValues;
                    }
                }
                else
                {
                    _logger.Debug(string.Format("{0} found null or Empty", ResourceConstants.XMLNodeNames.JOBORDERNODENAME));
                    return null;
                }
            }
            else
            {
                _logger.Debug("No messages found in the XML node.");
                return null;
            }

            return rData;
        }

        #endregion Private Methods
    }
}
