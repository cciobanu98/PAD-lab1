using Grpc.Core;
using MessageBrocker.RPC.Receiver.Abstract;
using MessageBrocker.RPC.Receiver.Options;
using MessageBrocker.RPC.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace MessageBrocker.RPC.Receiver.Implimentations
{
    public class Subscriber : ISubscriber
    {
        private readonly ILogger<Subscriber> _logger;
        private readonly IOptions<BrockerOptions> _options;

        public Subscriber(ILogger<Subscriber> logger, IOptions<BrockerOptions> options)
        {
            _logger = logger;
            _options = options;
        }
        public async Task<bool> Subscribe(string topic)
        {
            Channel channel = new Channel(_options.Value.Host, ChannelCredentials.Insecure);
            Subscription.SubscriptionClient client = new Subscription.SubscriptionClient(channel);

            var restult = await client.SubscribeAsync(new SubscribeRequest() { Topic = topic });

            return restult.Success;
        }

        public async Task<bool> Unsubscribe(string topic)
        {
            Channel channel = new Channel(_options.Value.Host, ChannelCredentials.Insecure);
            Subscription.SubscriptionClient client = new Subscription.SubscriptionClient(channel);

            var restult = await client.UnsubscribeAsync(new UnsubscribeRequest() { Topic = topic });

            return restult.Success;
        }
    }
}
