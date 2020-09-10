using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBrocker.Core.Abstract
{
    public class ListStorage<T> : IListStorage<T>
    {
        private ConcurrentDictionary<Guid, T> _storage;

        public ListStorage()
        {
            _storage = new ConcurrentDictionary<Guid, T>();
        }

        public T Add(T data)
        {
            var id = Guid.NewGuid();
            var item = _storage.GetOrAdd(id, data);
            return item;

        }

        public IEnumerable<KeyValuePair<Guid, T>> GetAll()
        {
            return _storage.ToList();
        }

        public void Remove(Guid id)
        {
            T data;
            _storage.TryRemove(id, out data);
        }

    }
}
