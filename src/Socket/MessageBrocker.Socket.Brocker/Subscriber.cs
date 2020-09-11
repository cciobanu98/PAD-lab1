using System;
using System.Net.Sockets;

namespace MessageBrocker.Sockets.Brocker
{
    public class Subscriber
    {
        public string Topic { get; private set; }

        public Socket Socket { get; private set; }

        public Subscriber(Socket socket, string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }
    }
}
