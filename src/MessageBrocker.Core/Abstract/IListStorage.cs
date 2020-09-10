using System;
using System.Collections.Generic;

namespace MessageBrocker.Core.Abstract
{
    public interface IListStorage<T>
    {
        public T Add(T data);

        public IEnumerable<KeyValuePair<Guid, T>> GetAll();

        public void Remove(Guid id);
    }
}
