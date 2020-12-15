using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace SampleApp
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider provider)
        {
            _commands = commands;
            _client = client;
            _provider = provider;
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            //the following logic confirms that the message is from a user and is used in the correct place (unless the user is an admin)
            if (!(messageParam is SocketUserMessage message))
                return;

            SocketGuildUser socketGuildUser = messageParam.Author as SocketGuildUser;

            //checks that the message is indeed a command to be handled by this bot
            int argPos = 0;
            if (!message.HasCharPrefix('!', ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.Author.IsBot)
                return;

            //creates command context and executes command
            SocketCommandContext context = new SocketCommandContext(_client, message);
            IResult result = await _commands.ExecuteAsync(context, argPos, _provider);
        }
    }
}