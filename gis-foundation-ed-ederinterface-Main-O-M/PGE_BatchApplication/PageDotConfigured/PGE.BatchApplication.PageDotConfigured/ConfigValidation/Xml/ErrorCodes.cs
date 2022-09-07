namespace PGE.BatchApplication.PageDotConfigured.ConfigValidation.Xml
{
    public enum ErrorCodes
    {
        //All clear
        NoError = 0,

        //No specific error, validation failed. Likely a mismatch between numbers
        ValidationFailed = 1,

        //One layer name string is contained within the other. E.g. "Transformer" is within "OH Transformers"
        StringContained = 2,

        //Layername field is empty in the XML
        EmptyValue = 3,

        //The id list and name list do not line up correctly. The number of entries in each should match
        KeyToValListCountMismatch = 4,

        //Relationship id mismatch
        RelationshipIdMismatch = 5,

        //Not for page.config, but for templates.config
        TemplatesConfigUrlBroken = 6
    }
}