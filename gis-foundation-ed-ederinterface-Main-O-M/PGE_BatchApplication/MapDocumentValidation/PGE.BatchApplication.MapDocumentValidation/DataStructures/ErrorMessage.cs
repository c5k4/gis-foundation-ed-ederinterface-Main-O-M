namespace MapDocumentValidation.MapDocument
{
    public class ErrorMessage
    {
        public ErrorMessage(string sd1, string sd2, string lyrName, PropertyId error,
            string symbolName)
        {
            StoredDisplay1 = sd1;
            StoredDisplay2 = sd2;
            Error = error;
            LayerName = lyrName;
            SymbolName = symbolName;
        }

        public string StoredDisplay1 { get; private set; }
        public string StoredDisplay2 { get; private set; }
        public string LayerName { get; private set; }
        public string SymbolName { get; private set; }
        public PropertyId Error { get; private set; }

        public override bool Equals(object obj)
        {
            ErrorMessage other = (ErrorMessage) obj;

            return StoredDisplay1.Equals(other.StoredDisplay1) && StoredDisplay2.Equals(other.StoredDisplay2) &&
                   LayerName.Equals(other.LayerName) && SymbolName.Equals(other.SymbolName) && Error.Equals(other.Error);
        }
    }
}