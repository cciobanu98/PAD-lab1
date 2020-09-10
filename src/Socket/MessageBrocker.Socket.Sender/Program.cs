using MessageBrocker.Core.Abstract;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MessageBrocker.Sockets.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton(config)
                    .AddTransient<Sender>();

            services.AddTransient<ISender>(provider => provider.GetService<Sender>());

            services.AddOptions<SocketOptions>()
              .Configure<IConfiguration>((settings, configuration) =>
              {
                  configuration.Bind(settings);
              });

            var provider = services.BuildServiceProvider();
            var sender = provider.GetRequiredService<Sender>();
            sender.Connect();
            if (sender.IsConnected())
            {
                Console.WriteLine("UUUU");
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
