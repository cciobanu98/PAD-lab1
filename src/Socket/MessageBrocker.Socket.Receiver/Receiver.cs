using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Shared;
using MessageBrocker.Sockets.Shared.Abstract;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace MessageBrocker.Sockets.Receiver
{
    public class Receiver : SocketBase
    {
        private readonly ManualResetEvent _connectedEvent;
        public Receiver(ILogger<SocketBase> logger, 
            IOptions<SocketOptions> options) : base(logger, options)
        {
            _connectedEvent = new ManualResetEvent(false);
        }

        public void Connect()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(Options.Value.Ip), Options.Value.Port);
                Logger.LogInformation("Waiting for connection");
                Socket.Connect(endPoint);
                Logger.LogInformation("Connection established");
                _connectedEvent.Set();
            }
            catch (Exception e)
            {
                Logger.LogError("Connection error: {msg}", e.Message);
            }
        }

        public void Subscribe(string topic)
        {
            _connectedEvent.WaitOne();
            try
            {
                if (topic == null)
                {
                    throw new ArgumentNullException(nameof(topic));
                }
                if (!Socket.Connected)
                {
                    Logger.LogInformation("Socked it's not connected");
                    return;
                }
                Send(Socket, Constants.Subscribe + topic);
            }
            catch (Exception e)
            {
                Logger.LogError("Subscription error: {msg}", e.Message);
            }
        }

        protected override void HandleMessage(string message, Socket socket)
        {
            Message msg = JsonSerializer.Deserialize<Message>(message);
            Logger.LogInformation("Message received: {msg}", msg.ToString());
        }
    }
}
