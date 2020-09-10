using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBrocker.Core.Abstract
{
    public interface IQueueStorage<T>
    {
        public T Get();

        public void Add(T data);

        public bool IsEmpty();
    }
}
