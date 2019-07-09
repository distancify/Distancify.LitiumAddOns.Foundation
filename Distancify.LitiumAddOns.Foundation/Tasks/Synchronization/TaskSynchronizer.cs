using System;
using System.Collections.Concurrent;

namespace Distancify.LitiumAddOns.Foundation.Tasks.Synchronization
{
    public class TaskSynchronizer : ITaskSynchronizer
    {
        private ConcurrentDictionary<string, object> _groupSpecificLocks = new ConcurrentDictionary<string, object>();
        private object GroupSpecificLock(string groupName)
        {
            return _groupSpecificLocks.GetOrAdd(groupName, new object());
        }        

        public void Synchronize(string groupName, Action action)
        {
            lock(GroupSpecificLock(groupName))
            {
                action();
            }
        }
    }
}
