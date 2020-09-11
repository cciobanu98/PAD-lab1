using System;
using System.Threading.Tasks;
using Grpc.Core;
using MessageBrocker.RPC.Shared;
using Microsoft.Extensions.Logging;

namespace MessageBrocker.RPC.Receiver
{
    public class SenderService : Client.ClientBase
    {
        private readonly ILogger<SenderService> _logger;
        public SenderService(ILogger<SenderService> logger)
        {
            _logger = logger;
        }

        public override Task<SendReply> Send(SendRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Message Topic: {topic}. Message: {msg}", request.Topic, request.Data);
                return Task.FromResult(new SendReply() { Success = true });
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error: {msg}", e.Message);
                return Task.FromResult(new SendReply() { Success = false });
            }
        }
    }
}
