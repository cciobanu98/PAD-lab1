using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Brocker.Options;
using MessageBrocker.Sockets.Brocker.Storage;
using MessageBrocker.Sockets.Shared.Abstract;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MessageBrocker.Sockets.Brocker
{
    public class Brocker : SocketBase
    {
        private readonly IOptions<BrockerOptions> _brockerOptions;
        private readonly ManualResetEvent _allDone;
        private readonly IListStorage<ReceiverInformation> _storage;
        private readonly IQueueStorage<Message> _messageStorage;
        public Brocker(ILogger<Brocker> logger,
                       IOptions<SocketOptions> options,
                       IOptions<BrockerOptions> brockerOptions,
                       IListStorage<ReceiverInformation> storage, 
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
            while(true)
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
            Logger.LogInformation("Clinet accepted: {0}", socket.RemoteEndPoint.ToString()); 
            socket.EndAccept(ar);
        }

        public void Receive()
        {

        }
    }
}
