using System;

namespace Distancify.LitiumAddOns.Foundation.Tasks.Synchronization
{
    public interface ITaskSynchronizer
    {   
        void Synchronize(string groupName, Action action);
    }
}
