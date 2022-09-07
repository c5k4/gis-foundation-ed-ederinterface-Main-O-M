using System;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Types of Telvent's REST endpoints.
    /// </summary>
    [Obsolete("Enum no longer used")]
    public enum FormatType
    {
        html,
        json,
        pjson
    }
}

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{

    #region public enums

    /// <summary>
    /// Types of identify operations.
    /// </summary>
    [Obsolete]
    public enum IdentifyType
    {
        VisibleLayers = 0,
        SingleLayer = 1,
        ModelName = 2,
        IdentifyModelNames = 3
    }
}

#if SILVERLIGHT
namespace Miner.Server.Client.Trace
#elif WPF
namespace Miner.Mobile.Client.Trace
#endif
{
    /// <summary>
    /// Trace barriers for gas/water isolation trace.
    /// </summary>
    public enum ValveIsolationTraceBarriers
    {
        AllValves,
        CriticalValves
    }

    /// <summary>
    /// Trace barriers for gas pressure trace.
    /// </summary>
    public enum GasPressureTraceBarriers
    {
        AllRegulators,
        RegulatorsWithDifferentInAndOutPressures
    }

    /// <summary>
    /// Trace barriers for water pressure trace.
    /// </summary>
    public enum WaterPressureTraceBarriers
    {
        AllRegulators,
        RegulatorsAndOtherDevices
    }

    /// <summary>
    /// Supported trace types.
    /// </summary>
    public enum ElectricTraceType
    {
        Downstream,
        Upstream,
        Distribution,
        DownstreamProtective,
        UpstreamProtective,
        NextUpstreamProtective,
        ProtectiveIsolation
    }

    /// <summary>
    /// Supported gas traces.
    /// </summary>
    public enum GasTraceType
    {
        ValveIsolation,
        CathodicProtection,
        PressureSystem
    }

    /// <summary>
    /// Supported water traces.
    /// </summary>
    public enum WaterTraceType
    {
        ValveIsolation,
        CathodicProtection,
        PressureSystem
    }

    /// <summary>
    /// Phases to Trace for Electric Traces
    /// </summary>
    public enum PhasesToTrace
    {
        Any,
        A,
        B,
        C,
        AB,
        AC,
        BC,
        ABC,
        AtLeastA,
        AtLeastB,
        AtLeastC,
        AtLeastAB,
        AtLeastAC,
        AtLeastBC,
        AnySinglePhase,
        AnyTwoPhase
    }

    /// <summary>
    /// The temporary marks that can be used in gas and water tracing.
    /// </summary>
    public enum TraceTemporaryMarksType
    {
        ExcludeValve,
        IncludeValve,
        SqueezeOff,
        TemporarySource
    }

    #endregion public enums
}
