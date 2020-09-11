using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Brocker.Options;
using MessageBrocker.Sockets.Shared;
using MessageBrocker.Sockets.Shared.Abstract;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace MessageBrocker.Sockets.Brocker
{
    public class Brocker : SocketBase
    {
        private readonly IOptions<BrockerOptions> _brockerOptions;
        private readonly ManualResetEvent _allDone;
        private readonly IListStorage<Subscriber> _storage;
        private readonly IQueueStorage<Message> _messageStorage;

        public Brocker(ILogger<Brocker> logger,
                       IOptions<SocketOptions> options,
                       IOptions<BrockerOptions> brockerOptions,
                       IListStorage<Subscriber> storage,
                       IQueueStorage<Message> messageStorage) : base(logger, options)
        {
            _brockerOptions = brockerOptions;
            _allDone = new ManualResetEvent(false);
            _storage = storage;
            _messageStorage = messageStorage;
        }

        public void Start()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(Options.Value.Ip), Options.Value.Port);
            Socket.Bind(ipEndPoint);
            Socket.Listen(_brockerOptions.Value.Limit);
        }

        public void Accept()
        {
            while (true)
            {
                _allDone.Reset();
                Logger.LogInformation("Waiting for a connection...");
                Socket.BeginAccept(new AsyncCallback(AcceptCallBack), Socket);
                _allDone.WaitOne();
            }
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            _allDone.Set();

            Socket socket = (Socket)ar.AsyncState;
            var handler = socket.EndAccept(ar);
            //Logger.LogInformation("Clinet accepted: {0}", socket.RemoteEndPoint.ToString());
            var handlerInformation = new SocketState(handler);
            handler.BeginReceive(handlerInformation.Buffer, 0, SocketState.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), handlerInformation);
        }

        protected override void HandleMessage(string message, Socket socket)
        {
            Logger.LogInformation("Received message: {msg}", message);
            if (message.Contains(Constants.Subscribe))
            {
                var topic = message.Split(Constants.Subscribe).LastOrDefault();
                var existingSubscriber = _storage.GetAll().Where(x => x.Value.Topic == topic && x.Value.Socket == socket).Select(x => x.Value).FirstOrDefault();
                if (existingSubscriber != null)
                {
                    Logger.LogWarning("Subscriber already subscribed to topic {topic}", topic);
                    return;
                }
                var subscriber = new Subscriber(socket, topic);
                _storage.Add(subscriber);
            }
            else
            {
                Message msg = JsonSerializer.Deserialize<Message>(message);
                _messageStorage.Add(msg);
            }
        }

        public void Send()
        {
            while (true)
            {
                while (!_messageStorage.IsEmpty())
                {
                    var msg = _messageStorage.Get();
                    var subscribers = _storage.GetAll().Where(x => x.Value.Topic == msg.Topic);
                    foreach (var subscriber in subscribers)
                    {
                        Send(subscriber.Value.Socket, msg);
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
