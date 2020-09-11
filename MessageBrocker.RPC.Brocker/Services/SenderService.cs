using Grpc.Core;
using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.RPC.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MessageBrocker.RPC.Brocker.Services
{
    public class SenderService : Sender.SenderBase
    {
        private readonly ILogger<ManagerService> _logger;
        private readonly IQueueStorage<Message> _storage;

        public SenderService(ILogger<ManagerService> logger, IQueueStorage<Message> storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public override Task<MessageReply> Send(MessageRequest request, ServerCallContext context)
        {
            try
            {
                _storage.Add(new Message(request.Topic, request.Data));
                return Task.FromResult(new MessageReply() { Success = true });
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error: {msg}", e.Message);
                return Task.FromResult(new MessageReply() { Success = false });
            }
        }
    }
}
