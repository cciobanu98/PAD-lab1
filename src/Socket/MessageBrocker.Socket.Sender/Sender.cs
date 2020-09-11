using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Shared.Abstract;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MessageBrocker.Sockets.Sender
{
    public class Sender : SocketBase, ISender
    {
        private readonly ManualResetEvent _connectDone;
        public Sender(ILogger<Sender> logger,
                      IOptions<SocketOptions> options) : base(logger, options)
        {
            _connectDone = new ManualResetEvent(false);
        }

        public void Connect()
        {
            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Parse(Options.Value.Ip), Options.Value.Port);
                Socket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallBack), Socket);
            }
            catch (Exception e)
            {
                Logger.LogError("Error on connecting: {message}", e.Message);
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
                Logger.LogInformation("Socket connected to {0}", socket.RemoteEndPoint.ToString());
                _connectDone.Set();
            }
            catch (Exception e)
            {
                Logger.LogError("Error on connecting {message}", e.Message);
            }
        }

        public bool IsConnected()
        {
            return _connectDone.WaitOne();
        }

        public void Send<T>(T data) where T : Message
        {
            Send(Socket, data);
        }
    }
}
