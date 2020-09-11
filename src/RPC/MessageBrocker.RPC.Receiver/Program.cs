using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageBrocker.RPC.Receiver.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageBrocker.RPC.Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var subscriber = host.Services.GetRequiredService<ISubscriber>();

            new Thread(() => Interact(subscriber)).Start();

            host.Run();
        }

       public static async void Interact(ISubscriber subscriber)
       {
            Console.WriteLine("1.Subscribe. 2.Unsubscribe");
            Console.Write("Option: ");
            var option = int.Parse(Console.ReadLine());
            Console.Write("Topic: ");
            var topic = Console.ReadLine();
            bool result = true;
            if (option == 1)
            {
                result = await subscriber.Subscribe(topic);
            }
            else
            {
                result = await subscriber.Unsubscribe(topic);
            }
            Console.WriteLine("Result: {result}", result);

        }
        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
