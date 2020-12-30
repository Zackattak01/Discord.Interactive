using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class InteractiveService
    {
        private BaseSocketClient Client { get; }

        public TimeSpan DefaultTimeout { get; }

        public TimeSpan DefaultPaginatorTimeout { get; }

        public InteractiveService(BaseSocketClient client, TimeSpan? defaultTimeout = null, TimeSpan? defaultPaginatorTimeout = null)
        {
            Client = client;
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(15);
            DefaultPaginatorTimeout = defaultPaginatorTimeout ?? TimeSpan.FromMinutes(2);
        }

        public InteractiveService(DiscordSocketClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }

        public InteractiveService(DiscordShardedClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }


        public async Task<SocketMessage> NextMessageAsync(ICriteria<IMessage> criteria = null, TimeSpan? timeout = null)
        {
            criteria ??= new NextMessageCriteria();

            var socketMessageSource = new TaskCompletionSource<SocketMessage>();

            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);
            var socketMessageTask = socketMessageSource.Task;


            async Task MessageHandler(SocketMessage message)
            {
                if (message.Author.Id == Client.CurrentUser.Id)
                    return;


                if (await criteria.ValidateAsync(message).ConfigureAwait(false))
                    socketMessageSource.SetResult(message);
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

        public Task<SocketMessage> NextMessageAsync(ICommandContext context, TimeSpan? timeout = null)
        {
            var criteria = new NextMessageCriteria(context);
            return NextMessageAsync(criteria, timeout);
        }

        public async Task<SocketReaction> NextReactionAsync(ICriteria<ReactionEventData> criteria = null, TimeSpan? timeout = null)
        {
            criteria ??= new NextReactionCriteria();

            var socketReactionSource = new TaskCompletionSource<SocketReaction>();

            var socketReactionTask = socketReactionSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            async Task ReactionHandler(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
            {
                if (reaction.UserId == Client.CurrentUser.Id)
                    return;

                if (await criteria.ValidateAsync(new ReactionEventData(cachedMessage, originChannel, reaction)).ConfigureAwait(false))
                    socketReactionSource.SetResult(reaction);

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

        public Task<SocketReaction> NextReactionAsync(ICommandContext context, TimeSpan? timeout = null)
        {
            var criteria = new NextReactionCriteria(context);
            return NextReactionAsync(criteria, timeout);
        }

        public void SendAndDelete(ICommandContext context, TimeSpan? timeout = null, string content = null,
                                            bool isTTS = false, Embed embed = null, RequestOptions requestOptions = null,
                                            AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            SendAndDelete(context.Channel, timeout, content, isTTS, embed, requestOptions, allowedMentions, messageReference);
        }

        public void SendAndDelete(IMessageChannel channel, TimeSpan? timeout = null, string content = null,
                                        bool isTTS = false, Embed embed = null, RequestOptions requestOptions = null,
                                        AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            Task.Run(async () =>
            {
                var message = await channel.SendMessageAsync(content, isTTS, embed, requestOptions, allowedMentions, messageReference).ConfigureAwait(false);

                await Task.Delay(timeout ?? DefaultTimeout).ConfigureAwait(false);

                await message.DeleteAsync().ConfigureAwait(false);
            });
        }

        public async Task<IUserMessage> SendPaginatedMessage(ICommandContext context, PaginatedMessage paginatedMessage, TimeSpan? timeout = null, string content = null,
                                bool isTTS = false, RequestOptions requestOptions = null,
                                AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            if (!paginatedMessage.IsValidPaginatedMessage())
            {
                throw new Exception("Invalid Paginated Message!");
            }

            var message = await context.Channel.SendMessageAsync(content, isTTS, paginatedMessage.FirstPage(), requestOptions, allowedMentions, messageReference).ConfigureAwait(false);

            //dont want to block the gateway
            //I also dont want to move this to a serparate function
            _ = Task.Run(async () =>
            {
                var emojis = paginatedMessage.Emotes.Keys.ToArray();

                //this is slow and bound to hit a ratelimit.  I can probably do something about the ratelimit but it will still be slow
                _ = Task.Run(async () => await message.AddReactionsAsync(emojis).ConfigureAwait(false));

                TaskCompletionSource completionSource = new TaskCompletionSource();

                var completionTask = completionSource.Task;
                Task timeoutTask = Task.Delay(timeout ?? DefaultPaginatorTimeout);

                async Task ReactionChanged(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
                {

                    var emote = paginatedMessage.Emotes.Keys.FirstOrDefault(x => x.Name == reaction.Emote.Name);

                    if (paginatedMessage.Emotes.TryGetValue(emote, out var action)
                    && cachedMessage.Id == message.Id
                    && reaction.UserId != Client.CurrentUser.Id
                    && reaction.UserId == context.User.Id)
                    {

                        Embed newPage = null;
                        switch (action)
                        {
                            case PaginatorAction.FirstPage:
                                newPage = paginatedMessage.FirstPage();
                                break;
                            case PaginatorAction.PreviousPage:
                                newPage = paginatedMessage.PreviousPage();
                                break;
                            case PaginatorAction.NextPage:
                                newPage = paginatedMessage.NextPage();
                                break;
                            case PaginatorAction.LastPage:
                                newPage = paginatedMessage.LastPage();
                                break;
                            case PaginatorAction.Stop:
                                completionSource.SetResult();
                                break;

                        }

                        if (newPage is not null)
                        {
                            await message.ModifyAsync(x => x.Embed = newPage).ConfigureAwait(false);
                        }

                    }
                }

                try
                {
                    Client.ReactionAdded += ReactionChanged;
                    Client.ReactionRemoved += ReactionChanged;

                    var task = await Task.WhenAny(completionTask, timeoutTask).ConfigureAwait(false);

                }
                finally
                {
                    Client.ReactionAdded -= ReactionChanged;
                    Client.ReactionRemoved -= ReactionChanged;

                    await message.RemoveAllReactionsAsync().ConfigureAwait(false);
                }
            });


            return message;
        }
    }
}
