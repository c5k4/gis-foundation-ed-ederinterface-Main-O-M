using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

using ESRI.ArcGIS.Geodatabase;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    public class Relationship : IEvaluationCriteria
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// relationship class with attribute [XmlIgnore].
        /// </summary>
        [XmlIgnore]
        private IRelationshipClass relationshipClass;
        #endregion Private

        #region Public
        #region Properties
        /// <summary>
        /// Gets or sets the field name of the feature attribute which will be compared to the Value list.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        [XmlAttribute("ClassName")]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets Count.
        /// </summary>
        [XmlElement("Count")]
        public Count Count { get; set; }

        /// <summary>
        /// Gets or sets RelatedObject.
        /// </summary>
        [XmlElement("RelatedObject")]
        public RelatedObject relatedRecord { get; set; }

        /// <summary>
        /// Gets or sets the feature attributes. Each feature attribute is defined using the 
        /// operator type assigned above.
        /// </summary>
        /// <value>
        /// The feature attributes.
        /// </value>
        [XmlElement(typeof(FeatureAttribute), ElementName = "Attribute")]
        public List<FeatureAttribute> FeatureAttributes { get; set; }
        #endregion Properties

        /// <summary>
        /// Initializes the specified object class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //get the relationshipclass from class name and objectclass workspace
            relationshipClass = ((objectClass as IDataset).Workspace as IFeatureWorkspace).OpenRelationshipClass(ClassName);
            _logger.Debug("Got the relationship class from current workspace:" + ClassName);
            //check if related records are not null
            if (relatedRecord != null)
            {
                //initialize the related recordsfor relationshipclass.DestinationClass/relationshipclass.OriginClass
                IObjectClass relatedClass = null;
                if (relationshipClass.OriginClass == objectClass)
                {
                    relatedClass = relationshipClass.DestinationClass;
                }
                else
                {
                    relatedClass = relationshipClass.OriginClass;
                }
                _logger.Debug("Initializing the related records for relationshipclass.Destinationclass:" + relatedClass.AliasName);
                relatedRecord.Initialize(relatedClass);
                _logger.Debug("Completed initialization of the related records for relationshipclass.Destinationclass:" + relatedClass.AliasName);
            }
        }

        /// <summary>
        /// Evaluates the specified feature, and returns true if the related record is present and matches attributes or
        /// true if the record record count matches the count elements
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public bool Evaluate(IObject feature)
        {
            bool rValue = false;

            // Find the related objects.
            ESRI.ArcGIS.esriSystem.ISet relatedObjects = RelatedObjectFacade.Instance.GetObjectsRelatedToObject(relationshipClass, feature as IObject);

            // Check to see if a related record element is present.
            if (relatedRecord != null)
            {
                _logger.Debug("Related records are not <Null>.");
                // A related record element was detected, return false if no related objects are present as the cannot match related criteria. 
                if (relatedObjects.Count == 0)
                {
                    _logger.Debug("Related objects count is 0.");
                    return false;
                }
                _logger.Debug(string.Format("Related objects count is {0}.", relatedObjects.Count));

                relatedObjects.Reset();

                IObject relatedObject;
                while ((relatedObject = relatedObjects.Next() as IObject) != null)
                {
                    // Evaluate the related records
                    rValue = relatedRecord.Evaluate(relatedObject);
                    if (rValue) break;
                }
            }
            else
            {
                // No related record element was found continue;
                _logger.Debug("Related record is <Null>.");
                rValue = true;
            }

            // Check the record count
            if (Count != null)
            {
                _logger.Debug("Count is not <Null>.");
                rValue = rValue && Count.Evaluate(relatedObjects.Count);
            }
            _logger.Debug(string.Format("Final value of evaluation is {0}", rValue));
            return rValue;
        }
        #endregion Public
    }
}
