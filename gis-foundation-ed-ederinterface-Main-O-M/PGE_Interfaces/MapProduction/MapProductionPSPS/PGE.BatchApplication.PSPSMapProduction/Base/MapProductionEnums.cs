/// <summary>
/// ExportType enum used by the PSPSMapProduction Tool set
/// </summary>
public enum ExportType
{
    Bulk,
    Maps,
    Difference
}

/// <summary>
/// ExportState class used by PSPSMapProduction Toolset
/// </summary>
public static class ExportState
{
    public const int ReadyToExport = 1;
    public const int Processing = 2;
    public const int Idle = 2;
    public const int Failed = 2;
}

public static class MapLayoutType
{
    public const int PortraitSegmentMap = 1;
    public const int PortraitOverviewMap = 2;
    public const int LandscapeSegmentMap = 3;
    public const int LandscapeOverviewMap = 4;
}