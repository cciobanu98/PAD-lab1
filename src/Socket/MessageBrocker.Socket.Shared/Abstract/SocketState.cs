using System;
using System.Net.Sockets;
using System.Text;

namespace MessageBrocker.Sockets.Shared.Abstract
{
    public class SocketState
    {
        public const int BufferSize = 1024;

        public byte[] Buffer { get; set; }

        public StringBuilder StringBuilder { get; set; }

        public Socket Socket { get; private set; }

        public SocketState(Socket socket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            StringBuilder = new StringBuilder();
            Buffer = new byte[BufferSize];
        }
    }
}
