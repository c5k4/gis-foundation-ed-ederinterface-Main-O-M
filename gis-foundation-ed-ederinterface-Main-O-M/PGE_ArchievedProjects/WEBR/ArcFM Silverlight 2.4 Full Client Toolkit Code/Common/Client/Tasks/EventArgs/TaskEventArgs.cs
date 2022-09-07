using System;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Base class for task event arguments.
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        // Methods
        public TaskEventArgs(object userToken)
        {
            this.UserState = userToken;
        }

        // Properties
        public object UserState { get; internal set; }
    }

}