namespace MapDocumentValidation.MapDocument
{
    public enum SymbolDataType
    {
        //For controlling attributes of symbology under a layer
        SymbolValueTemplate,
        SymbolValueKey,
        SymbolValue,

        //For marker symbols
        Color, //Also used for line symbols
        Angle,
        Size,
        XOffset,
        YOffset,

        //For character marker symbols
        FontName,
        CharacterIndex,

        //For cartographic marker symbols
        XScale,
        YScale,

        //For simple marker symbols
        Outline,
        OutlineColor,
        OutlineSize,
        Style, //also used for line symbols

        //For line symbols
        Width,

        //For cartographic line symbols
        Cap,
        Join,
        MiterLimit,

        //Font properties
        FontBold,
        FontSize,
        CharSet,
        Italic,
        Strikethrough,
        Underline,
        Weight,

        //Used for errors
        DataType,
        SymbolName,
        LayerCount,
        LayerName
    }
}