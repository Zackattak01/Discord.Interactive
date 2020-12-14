using System;
using System.Data;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class MessageCriteria
    {
        public static MessageCriteria Default { get; } = new MessageCriteria(null, null);

        public ulong? RequiredUserId { get; }

        public ulong? RequiredChannelId { get; }

        public MessageCriteria()
            : this(null, null) { }


        internal MessageCriteria(ulong? userId, ulong? requiredChannelId)
        {
            RequiredUserId = userId;
            RequiredChannelId = requiredChannelId;

        }

        public bool Validate(SocketMessage message)
        {
            if (RequiredUserId is not null)
            {
                if (message.Id != RequiredUserId)
                    return false;
            }

            if (RequiredChannelId is not null)
            {
                if (message.Channel is SocketTextChannel channel)
                {
                    if (channel.Id != RequiredChannelId)
                        return false;
                }
            }

            return true;
        }

        public MessageCriteria EnsureUser(IUser user)
            => new MessageCriteria(user.Id, RequiredChannelId);

        public MessageCriteria EnsureUser(ulong id)
            => new MessageCriteria(id, RequiredChannelId);

        public MessageCriteria EnsureChannel(ITextChannel channel)
            => new MessageCriteria(RequiredUserId, channel.Id);

        public MessageCriteria EnsureChannel(ulong id)
            => new MessageCriteria(RequiredUserId, id);
    }
}