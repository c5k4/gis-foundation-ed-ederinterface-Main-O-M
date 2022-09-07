#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// This class contains the definitions of constant values
    /// used by other components.
    /// </summary>
    internal static class Constants
    {
        internal const string LayerNameKey = "name";
        internal const string LayerIdKey = "id";
        internal const string ExceededThresholdKey = "exceededThreshold";
        internal const string ResultsKey = "results";
        internal const string LayersKey = "layers";
        internal const string IDKey = "id";
        internal const string NameKey = "name";
        internal const string AliasKey = "alias";
        internal const string ParentLayerIDKey = "parentLayerId";
        internal const string SubLayerIDsKey = "subLayerIds";
        internal const string IdentifyModelNamesKey = "IdentifyModelNames";
        internal const string LayerIDsKey = "layerIDs";
        internal const string TextKey = "text";
        internal const string ValueKey = "value";
        internal const string RelationshipsKey = "relationships";
        internal const string RelatedTableIdKey = "relatedTableId";
        internal const string LengthKey = "length";
        internal const string FieldTypeKey = "type";
        internal const string FieldNullableKey = "nullable";
        internal const string FieldEditableKey = "editable";
        internal const string DomainKey = "domain";
        internal const string DomainTypeKey = "type";
        internal const string DomainNameKey = "name";
        internal const string DomainTypeValueCV = "codedValue";
        internal const string DomainCodedValues = "codedValues";
        internal const string DomainCode = "code";
        internal const string TypeKey = "type";
        internal const string ElementID = "elementId";
        internal const string ObjectID = "objectId";
        internal const string ObjectClassName = "objectClassName";
        internal const string ProtectiveDevicesKey = "ProtectiveDevices";
    }
}
