using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Input;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class InteractiveBase<T> : ModuleBase<T>
        where T : class, ICommandContext
    {
        public InteractiveService Interactive { get; set; }

        public Task<SocketMessage> NextMessageAsync(ICriteria<IMessage> criteria = null, TimeSpan? timeout = null)
            => Interactive.NextMessageAsync(criteria, timeout);

        public Task<SocketMessage> NextMessageAsync()
            => Interactive.NextMessageAsync(Context);

        public Task<SocketReaction> NextReactionAsync(ICriteria<ReactionEventData> criteria = null, TimeSpan? timeout = null)
            => Interactive.NextReactionAsync(criteria, timeout);

        public Task<SocketReaction> NextReactionAsync()
            => Interactive.NextReactionAsync(Context);

        public void SendAndDelete(IMessageChannel channel, TimeSpan? timeout = null, string content = null,
                            bool isTTS = false, Embed embed = null, RequestOptions requestOptions = null,
                            AllowedMentions allowedMentions = null, MessageReference messageReference = null)
            => Interactive.SendAndDelete(channel, timeout, content, isTTS, embed, requestOptions, allowedMentions, messageReference);

        public void SendAndDelete(ICommandContext context, TimeSpan? timeout = null, string content = null,
                                bool isTTS = false, Embed embed = null, RequestOptions requestOptions = null,
                                AllowedMentions allowedMentions = null, MessageReference messageReference = null)
            => Interactive.SendAndDelete(context, timeout, content, isTTS, embed, requestOptions, allowedMentions, messageReference);

        public Task<IUserMessage> SendPaginatedMessage(ICommandContext context, PaginatedMessage paginatedMessage, TimeSpan? timeout = null, string content = null,
                                bool isTTS = false, RequestOptions requestOptions = null,
                                AllowedMentions allowedMentions = null, MessageReference messageReference = null)
            => Interactive.SendPaginatedMessage(context, paginatedMessage, timeout, content, isTTS, requestOptions, allowedMentions, messageReference);


    }
}