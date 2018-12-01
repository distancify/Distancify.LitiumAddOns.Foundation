using System.Collections.Concurrent;
using System.Threading;
using Litium.Foundation.Security;
using Litium.Foundation.Tasks;

namespace Distancify.LitiumAddOns.Tasks
{
    /// <summary>
    /// <para>
    ///     This abstract base class makes sure that any derived Litium task is
    ///     prevented from running concurrently. Only a single execution is
    ///     allowed at any given time.
    /// </para>
    /// <para>
    ///     Note that any subsequent executions while the task is running will be
    ///     ignored. They will not be queued and run later.
    /// </para>
    /// <para>
    ///     Note that the task may still execute on another server, so make sure
    ///     your task is configured to only run on a single machine if you only
    ///     want a single task execution in the cluster.
    /// </para>
    /// </summary>
    public abstract class NonConcurrentTask : ITask
    {
        private static volatile ConcurrentDictionary<string, object> _taskSpecificLocks = new ConcurrentDictionary<string, object>();
        private object TaskSpecificLock
        {
            get
            {
                return _taskSpecificLocks.GetOrAdd(GetType().Name, new object());
            }
        }

        public void ExecuteTask(SecurityToken token, string parameters)
        {
            bool entered = false;

            try
            {
                if (Monitor.TryEnter(TaskSpecificLock))
                {
                    entered = true;
                    Run();
                }
            }            
            finally
            {
                if (entered)
                {
                    Monitor.Exit(TaskSpecificLock);
                }
            }
        }

        protected abstract void Run();
    }
}