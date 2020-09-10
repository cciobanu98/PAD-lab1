namespace MessageBrocker.Core.Abstract
{
    public interface IReceiver
    {
        public T Receive<T>();
    }
}
