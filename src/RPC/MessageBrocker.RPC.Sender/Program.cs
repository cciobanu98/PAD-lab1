using Grpc.Core;
using MessageBrocker.RPC.Shared;
using System;

namespace MessageBrocker.RPC.Senders
{
    class Program
    {
        static async void MainAsync(string[] args)
        {
            Channel channel = new Channel("https://localhost:5001", ChannelCredentials.Insecure);
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
            MainAsync(args);
        }
    }
}
