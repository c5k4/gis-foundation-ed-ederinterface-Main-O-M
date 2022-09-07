/// <summary>
/// ExportType enum used by the MapProduction Tool set
/// </summary>
public enum ExportType
{
    Bulk,
    Difference,
    Maps
}

/// <summary>
/// ExportState Enum used by MapProduction Toolset
/// </summary>
public enum ExportState
{
    ReadyToExport = 1,
    Processing = 2,
    Idle = 3,
    Failed = 4
}