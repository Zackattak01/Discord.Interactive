using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Name
{
    public class LoggingService
    {
        //this service is responsible for sending log messages

        private readonly DiscordSocketClient _client;

        private readonly CommandService commandService;

        public LoggingService(DiscordSocketClient discord, CommandService commandService)
        {
            _client = discord;
            _client.Log += Log;

            this.commandService = commandService;
            this.commandService.CommandExecuted += CommandExecutedAsync;

        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                if (result.Error != CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

    }
}