using System;
using System.Windows.Input;
using Discord.Commands;

namespace Discord.Interactive
{
    public class NextMessageCriteria : ICriteria<IMessage>
    {
        public ulong? RequiredUserId { get; }

        public ulong? RequiredChannelId { get; }

        public NextMessageCriteria()
            : this(null, null) { }

        public NextMessageCriteria(ICommandContext context)
            : this(context.Message.Author.Id, context.Channel.Id) { }

        internal NextMessageCriteria(ulong? userId, ulong? channelId)
        {
            RequiredUserId = userId;
            RequiredChannelId = channelId;
        }

        public virtual NextMessageCriteria EnsureUser(ulong id)
            => new NextMessageCriteria(id, RequiredChannelId);

        public virtual NextMessageCriteria EnsureUser(IUser user)
            => new NextMessageCriteria(user.Id, RequiredChannelId);

        public virtual NextMessageCriteria EnsureChannel(ulong id)
            => new NextMessageCriteria(RequiredUserId, id);

        public virtual NextMessageCriteria EnsureChannel(IChannel channel)
            => new NextMessageCriteria(RequiredUserId, channel.Id);

        public virtual bool Validate(IMessage message)
        {
            if (RequiredUserId is not null && message.Author.Id != RequiredUserId)
                return false;

            if (RequiredChannelId is not null && message.Channel.Id != RequiredChannelId)
                return false;

            return true;
        }
    }
}