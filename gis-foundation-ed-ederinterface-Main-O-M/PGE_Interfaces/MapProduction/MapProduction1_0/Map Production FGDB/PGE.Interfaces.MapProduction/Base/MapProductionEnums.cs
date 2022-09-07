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
    Failed = 4,
    FinishedTIFF = 5,
    FinishedPDF = 6, 
    Finished = 7 
}

public enum MapStatusUpdateType
{
    mapUpdateTypeProcessing = 1, 
    mapUpdateTypeFinished = 2, 
    mapUpdateTypeError = 3
}