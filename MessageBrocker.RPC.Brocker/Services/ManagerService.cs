using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MessageBrocker.Core.Abstract;
using MessageBrocker.RPC.Shared;
using Microsoft.Extensions.Logging;

namespace MessageBrocker.RPC.Brocker
{
    public class ManagerService : Subscription.SubscriptionBase
    {
        private readonly ILogger<ManagerService> _logger;
        private readonly IListStorage<Subscriber> _storage;

        public ManagerService(ILogger<ManagerService> logger, IListStorage<Subscriber> storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public override Task<SubscriptionReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            try
            {
                var subscriber = _storage.GetAll().Where(x => x.Value.Topic == request.Topic && x.Value.Host == context.Host).Select(x => x.Value).FirstOrDefault();
                if (subscriber != null)
                {
                    _logger.LogWarning("Subscriber: {sub} it's already subscribed to topic: {topic}", context.Host, request.Topic);
                    return Task.FromResult(new SubscriptionReply() { Success = false });
                }
                _logger.LogInformation("Subscriber: {sub} it's added to topic: {topic}", context.Host, request.Topic);
                _storage.Add(new Subscriber(request.Topic, context.Host));
                return Task.FromResult(new SubscriptionReply() { Success = true });
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error: {msg}", e.Message);
                return Task.FromResult(new SubscriptionReply() { Success = false });
            }
        }

        public override Task<SubscriptionReply> Unsubscribe(UnsubscribeRequest request, ServerCallContext context)
        {
            try
            {
                var subscriber = _storage.GetAll().Where(x => x.Value.Topic == request.Topic && x.Value.Host == context.Host).FirstOrDefault();
                if (subscriber.Value == null)
                {
                    _logger.LogWarning("Subscriber: {sub} for topic {topic} not found: ", context.Host, request.Topic);
                    return Task.FromResult(new SubscriptionReply() { Success = false });
                }
                _logger.LogInformation("Subscriber: {sub} it's removed from topic: {topic}", context.Host, request.Topic);
                _storage.Remove(subscriber.Key);
                return Task.FromResult(new SubscriptionReply() { Success = true });
            }
            catch(Exception e)
            {
                _logger.LogInformation("Error: {msg}", e.Message);
                return Task.FromResult(new SubscriptionReply() { Success = false });
            }
        }
    }
}
