using System.Collections.Generic;

namespace MapDocumentValidation.MapDocument
{
    public class MapDocuments : Dictionary<string, Layers>
    {
    }

    public class Layers : Dictionary<string, Symbols>
    {
    }

    public class Symbols : Dictionary<string, SymbolLayers>
    {
    }

    public class SymbolLayers : Dictionary<PropertyId, string>
    {
    }

    public class PropertyId
    {
        public PropertyId(SymbolDataType type, int symbolLayerId)
        {
            DataType = type;
            SymbolLayerSymbolLayerId = symbolLayerId;
        }

        public SymbolDataType DataType { get; private set; }
        public int SymbolLayerSymbolLayerId { get; private set; }

        public override int GetHashCode()
        {
            return DataType.GetHashCode() + SymbolLayerSymbolLayerId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PropertyId objAsId = obj as PropertyId;
            if (objAsId == null) return false;
            return objAsId.DataType.Equals(DataType) && objAsId.SymbolLayerSymbolLayerId.Equals(SymbolLayerSymbolLayerId);
        }

        public override string ToString()
        {
            return DataType + ", " + SymbolLayerSymbolLayerId;
        }
    }
}