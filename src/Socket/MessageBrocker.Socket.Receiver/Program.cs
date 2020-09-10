using MessageBrocker.Socket.Receiver.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MessageBrocker.Socket.Receiver
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

            services.AddOptions<ReceiverOptions>()
              .Configure<IConfiguration>((settings, configuration) =>
              {
                  configuration.Bind(settings);
              });

            var provider = services.BuildServiceProvider();
            var receiver = provider.GetRequiredService<Receiver>();
            receiver.Run();
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
