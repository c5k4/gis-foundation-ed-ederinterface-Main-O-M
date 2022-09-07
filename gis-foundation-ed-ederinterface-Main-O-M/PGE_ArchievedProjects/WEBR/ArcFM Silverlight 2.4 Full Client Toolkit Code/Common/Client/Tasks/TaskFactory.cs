using System;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Class to create tasks of a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskFactory<T> : ITaskFactory<T> where T : ITask
    {
        private T _task;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TaskFactory()
        {
            try
            {

            }
            catch { }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="task"></param>
        public TaskFactory(T task)
        {
            if (task == null) throw new ArgumentNullException("task");

            _task = task;
        }

        /// <summary>
        /// Get the type of the tasks in this factory.
        /// </summary>
        public Type TaskType { get { return _task != null ? _task.GetType() : typeof(T); } }

        /// <summary>
        /// Create a task of type T.
        /// </summary>
        /// <returns></returns>
        public T CreateTask()
        {
            return (T)Activator.CreateInstance(TaskType);
        }
    }

    /// <summary>
    /// Interface to implement a task factory. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITaskFactory<T> where T : ITask
    {
        Type TaskType { get; }
        T CreateTask();
    }
}
