using System;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// General interface for implementing tasks in ArcFM Server.
    /// </summary>
    public interface ITask
    {
        event EventHandler<TaskFailedEventArgs> Failed;
        string Url { get; set; }
        string ProxyURL { get; set; }
    }

    /// <summary>
    /// Interface for the result from a task.
    /// </summary>
    public interface ITaskResult : ITask
    {
        event EventHandler<TaskResultEventArgs> ExecuteCompleted;
    }

    /// <summary>
    /// Interface for implementing a locate. 
    /// </summary>
    public interface ILocateTask : ITaskResult
    {
        void ExecuteAsync(LocateParameters parameters, object userToken = null);
    }

    /// <summary>
    /// Interface for executing a query against an ArcFM map service for its individual layers
    /// </summary>
    public interface IArcFMLayerTask : ITask
    {
        event EventHandler<LayerEventArgs> ExecuteCompleted;
        void ExecuteAsync(object userToken = null);
    }

    /// <summary>
    /// Interface for implementing a task to get ArcFM field order on a map layer. 
    /// </summary>
    public interface IFieldOrderTask : ITask
    {
        event EventHandler<LayerInformationEventArgs> ExecuteCompleted;
        void ExecuteAsync(int index, object userToken = null);
    }

    /// <summary>
    /// Interface for implementing a task to return domain values for a map layer.
    /// </summary>
    public interface IDomainValuesTask : ITask
    {
        event EventHandler<LayerInformationEventArgs> ExecuteCompleted;
        void ExecuteAsync(int index, object userToken = null);
    }

    /// <summary>
    /// Interface for getting the existing relationships from a map layer. 
    /// </summary>
    public interface ILayerRelationshipInfoTask : ITask
    {
        event EventHandler<LayerRelationshipInfoEventArgs> ExecuteCompleted;
        void ExecuteAsync(int layerId, object userToken = null);
    }
}
