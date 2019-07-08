using System;

namespace Distancify.LitiumAddOns.Tasks.Synchronization
{
    public interface ITaskSynchronizer
    {   
        void Synchronize(string groupName, Action action);
    }
}
