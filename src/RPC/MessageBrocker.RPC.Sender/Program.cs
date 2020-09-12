using Grpc.Core;
using MessageBrocker.RPC.Shared;
using System;
using System.Threading.Tasks;

namespace MessageBrocker.RPC.Senders
{
    class Program
    {
        static async Task MainAsync(string[] args)
        {
            Channel channel = new Channel("localhost:5001", ChannelCredentials.Insecure);
            Sender.SenderClient sender = new Sender.SenderClient(channel);
            while (true)
            {
                Console.Write("Topic: ");
                var topic = Console.ReadLine();

                Console.Write("Data: ");
                var data = Console.ReadLine();

                await sender.SendAsync(new MessageRequest() { Topic = topic, Data = data });

            }
        }
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
