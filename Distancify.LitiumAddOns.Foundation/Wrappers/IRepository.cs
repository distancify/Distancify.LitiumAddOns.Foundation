using System;

namespace Distancify.LitiumAddOns.Wrappers
{
    public interface IRepository<T> where T : IEntity
    {        
        T Find(Guid id);   
    }
}
