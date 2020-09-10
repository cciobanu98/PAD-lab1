using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        protected T Get<T>(Socket socket) where T : Message
        {
            try
            {
                using (var stream = new NetworkStream(socket))
                {
                    IFormatter formatter = new BinaryFormatter();
                    T message = (T)formatter.Deserialize(stream);
                    return message;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Error on receive meessage {e.Message}");
            }
            return null;
        }

        protected void Send<T>(Socket socket, T message) where T : Message
        {
            try
            {
                using (var stream = new NetworkStream(socket))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, message);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("sendMessage exception: " + e.Message);
            }
        }
    }
}
