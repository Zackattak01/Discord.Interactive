using Discord;
using Discord.Commands;
using Discord.Interactive;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Name;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{
    public class Startup
    {
        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup();
            await startup.RunAsync();
        }

        public async Task RunAsync()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider provider = services.BuildServiceProvider();

            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<InteractiveService>();
            provider.GetRequiredService<LoggingService>();

            await provider.GetRequiredService<StartupService>().MainAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //adds singletons to the services
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       // Add discord to the collection
                LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
                MessageCacheSize = 1000,            // Cache 1,000 messages per channel
                GatewayIntents = (GatewayIntents?)0b111_1111_1111_1111, //specifies all intents // in binary cause im too lazy too type out all the intents
                //UseSystemClock = false
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {                                       // Add the command service to the collection
                LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
                DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
            }))
            .AddSingleton<StartupService>()
            .AddSingleton<InteractiveService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<LoggingService>();
        }
    }
}