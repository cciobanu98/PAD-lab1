using MessageBrocker.Core.Messages;

namespace MessageBrocker.Core.Abstract
{
    public interface ISender
    {
        void Send<T>(T data) where T : Message;
    }
}
