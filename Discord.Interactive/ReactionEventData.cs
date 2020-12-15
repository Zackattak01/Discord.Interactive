using System;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class ReactionEventData
    {
        public Cacheable<IUserMessage, ulong> CachedMessage { get; }
        public ISocketMessageChannel OriginChannel { get; }
        public SocketReaction Reaction { get; }

        public ReactionEventData(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            CachedMessage = cachedMessage;
            OriginChannel = originChannel;
            Reaction = reaction;
        }
    }
}