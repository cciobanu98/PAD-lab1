using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MessageBrocker.Sockets.Shared.Abstract
{
    public abstract class SocketBase
    {
        protected readonly ILogger<SocketBase> Logger;
        protected readonly Socket Socket;
        protected readonly IOptions<SocketOptions> Options;
        public SocketBase(ILogger<SocketBase> logger, IOptions<SocketOptions> options)
        {
            Logger = logger;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Options = options;
        }
        public void Receive()
        {
            try
            {
                var state = new SocketState(Socket);
                Socket.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Logger.LogError("Error on receive: {msg}", e.Message);
            }
        }

        public void Send<T>(Socket socket, T data) where T: class
        {
            var json = JsonSerializer.Serialize(data);
            Send(socket, json);
        }

        public void Send(Socket socket, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            socket.Send(bytes);
        }

        protected virtual void HandleMessage(string message, Socket socket)
        {

        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;
            Socket handler = state.Socket;
            try
            {
                // Read data from the client socket.  
                int read = handler.EndReceive(ar);

                // Data was read from the client socket.  
                if (read > 0)
                {
                    state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, read));
                }
                if (read < SocketState.BufferSize)
                {
                    var message = state.StringBuilder.ToString();
                    HandleMessage(message, handler);
                    state.StringBuilder = new StringBuilder();
                }
                handler.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0,
                     new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {msg}", e.Message);
                handler.Close();
            }
        }
    }
}
