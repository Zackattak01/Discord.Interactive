using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class InteractiveService
    {
        private BaseSocketClient Client { get; }

        public TimeSpan DefaultTimeout { get; }

        public InteractiveService(BaseSocketClient client, TimeSpan? defaultTimeout = null)
        {
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(15);
        }

        public InteractiveService(DiscordSocketClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }

        public InteractiveService(DiscordShardedClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }


        public async Task<SocketMessage> NextMessageAsync(ICriteria<IMessage> criteria = null, TimeSpan? timeout = null)
        {

            var socketMessageSource = new TaskCompletionSource<SocketMessage>();

            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);
            var socketMessageTask = socketMessageSource.Task;


            Task MessageHandler(SocketMessage message)
            {
                if (message.Author.Id == Client.CurrentUser.Id)
                {
                    return Task.CompletedTask;
                }

                if (criteria is not null)
                {
                    if (criteria.Validate(message))
                        socketMessageSource.SetResult(message);
                }
                else
                    socketMessageSource.SetResult(message);

                return Task.CompletedTask;
            }

            try
            {
                Client.MessageReceived += MessageHandler;

                var firstTaskCompleted = await Task.WhenAny(timeoutTask, socketMessageTask).ConfigureAwait(false);

                if (firstTaskCompleted == timeoutTask)
                    return null;
                else
                    return await socketMessageTask.ConfigureAwait(false);
            }
            finally
            {
                Client.MessageReceived -= MessageHandler;
            }
        }

        public async Task<SocketReaction> NextReactionAsync(TimeSpan? timout = null)
        {
            var socketReactionSource = new TaskCompletionSource<SocketReaction>();

            var socketReactionTask = socketReactionSource.Task;
            var timeoutTask = Task.Delay(timout ?? DefaultTimeout);

            Task ReactionHandler(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
            {
                socketReactionSource.SetResult(reaction);
                return Task.CompletedTask;
            }

            try
            {
                Client.ReactionAdded += ReactionHandler;

                Task firstCompletedTask = await Task.WhenAny(socketReactionTask, timeoutTask).ConfigureAwait(false);

                if (firstCompletedTask == timeoutTask)
                    return null;
                else
                    return await socketReactionTask.ConfigureAwait(false);
            }
            finally
            {
                Client.ReactionAdded -= ReactionHandler;
            }
        }


    }
}
