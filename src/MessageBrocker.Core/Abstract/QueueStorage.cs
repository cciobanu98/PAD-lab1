using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MessageBrocker.Core.Abstract
{
    public class QueueStorage<T> : IQueueStorage<T>
    {
        protected ConcurrentQueue<T> _storage;

        public QueueStorage()
        {
            _storage = new ConcurrentQueue<T>();
        }
        public void Add(T data)
        {
            _storage.Enqueue(data);
        }

        public T Get()
        {
            T data;
            _storage.TryDequeue(out data);
            return data;
        }

        public bool IsEmpty()
        {
            return _storage.IsEmpty;
        }
    }
}
