using Grpc.Core;
using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.RPC.Brocker.Abstract;
using MessageBrocker.RPC.Shared;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBrocker.RPC.Brocker
{
    public class Worker : IWorker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueStorage<Message> _messageStorage;
        private readonly IListStorage<Subscriber> _subscribersStorage;

        public Worker(ILogger<Worker> logger, IQueueStorage<Message> messageStorage, IListStorage<Subscriber> subscribersStorage)
        {
            _logger = logger;
            _messageStorage = messageStorage;
            _subscribersStorage = subscribersStorage;
        }

        public async Task Run()
        {
            while (true)
            {
                while (!_messageStorage.IsEmpty())
                {
                    var msg = _messageStorage.Get();
                    var subscribers = _subscribersStorage.GetAll().Where(x => x.Value.Topic == msg.Topic);
                    foreach (var subscriber in subscribers)
                    {
                        await Send(subscriber.Value.Host, msg);
                    }
                }
                Thread.Sleep(500);
            }
        }

        private async Task Send(string host, Message message)
        {
            Channel channel = new Channel(host, ChannelCredentials.Insecure);
            Client.ClientClient client = new Client.ClientClient(channel);

            await client.SendAsync(new SendRequest(){ Data = message.Data, Topic = message.Topic});
            await channel.ShutdownAsync();
        }
    }
}
