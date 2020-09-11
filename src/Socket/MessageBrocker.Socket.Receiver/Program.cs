using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

namespace MessageBrocker.Sockets.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton(config)
                    .AddTransient<Receiver>();

            services.AddOptions<SocketOptions>()
              .Configure<IConfiguration>((settings, configuration) =>
              {
                  configuration.Bind(settings);
              });

            var provider = services.BuildServiceProvider();
            var receiver = provider.GetRequiredService<Receiver>();
            receiver.Connect();
            new Thread(receiver.Receive).Start();
            while(true)
            {
                Console.Write("Subscribe to topic: ");
                var topic = Console.ReadLine();
                receiver.Subscribe(topic);
            }
        }
        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
