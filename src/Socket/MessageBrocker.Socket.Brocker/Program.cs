using MessageBrocker.Core.Abstract;
using MessageBrocker.Core.Messages;
using MessageBrocker.Sockets.Brocker.Options;
using MessageBrocker.Sockets.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;

namespace MessageBrocker.Sockets.Brocker
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton(config)
                    .AddSingleton<IQueueStorage<Message>, QueueStorage<Message>>()
                    .AddSingleton<IListStorage<Subscriber>, ListStorage<Subscriber>>()
                    .AddSingleton<Brocker>();

            services.AddOptions<BrockerOptions>()
              .Configure<IConfiguration>((settings, configuration) =>
              {
                  configuration.Bind(settings);
              });

            services.AddOptions<SocketOptions>()
              .Configure<IConfiguration>((settings, configuration) =>
              {
                  configuration.Bind(settings);
              });

            var provider = services.BuildServiceProvider();
            var brocker = provider.GetRequiredService<Brocker>();

            brocker.Start();
            new Thread(brocker.Accept).Start();
            new Thread(brocker.Send).Start();
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
