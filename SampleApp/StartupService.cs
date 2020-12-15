using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SampleApp
{
    class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _provider = provider;
            _client = discord;
            _commands = commands;
        }

        public async Task MainAsync()
        {

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"), true);
            await _client.StartAsync();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);


            //block the main task from closing
            await Task.Delay(Timeout.Infinite);
        }
    }
}