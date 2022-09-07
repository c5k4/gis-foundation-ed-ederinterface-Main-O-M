namespace PGE.BatchApplication.PageDotConfigured.ConfigValidation.Json
{
    public static class JsonConstants
    {
        //Appended to a map service REST API URI to access the corresponding JSON text
        public const string JsonSuffix = "?f=json";

        public const string ArcFmMapServerSuffix = "/exts/ArcFMMapServer";
        public const string ArcFmMapServerIdPrefix = "/id";
    }

    public enum JsonTags
    {
        id,
        name,
        objectClassID
    }
}