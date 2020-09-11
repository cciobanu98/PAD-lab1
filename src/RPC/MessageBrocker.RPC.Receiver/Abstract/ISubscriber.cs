using System.Threading.Tasks;

namespace MessageBrocker.RPC.Receiver.Abstract
{
    public interface ISubscriber
    {
        Task<bool> Subscribe(string topic);

        Task<bool> Unsubscribe(string topic);
    }
}
