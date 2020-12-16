using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Discord.Interactive
{
    public abstract class UserChannelCriteria<T> : ICriteria<T>
    {
        public ulong? RequiredUserId { get; }

        public ulong? RequiredChannelId { get; }

        public UserChannelCriteria()
            : this(null, null) { }

        public UserChannelCriteria(ICommandContext context)
            : this(context.Message.Author.Id, context.Channel.Id) { }

        internal UserChannelCriteria(ulong? userId, ulong? channelId)
        {
            RequiredUserId = userId;
            RequiredChannelId = channelId;
        }

        public abstract UserChannelCriteria<T> EnsureUser(ulong id);

        public abstract UserChannelCriteria<T> EnsureUser(IUser user);

        public abstract UserChannelCriteria<T> EnsureChannel(ulong id);

        public abstract UserChannelCriteria<T> EnsureChannel(IChannel channel);

        public abstract Task<bool> ValidateAsync(T obj);

        protected virtual bool Validate(IUser user, IChannel channel)
            => Validate(user.Id, channel.Id);

        protected virtual bool Validate(ulong userId, ulong channelId)
        {
            if (RequiredUserId is not null && userId != RequiredUserId)
                return false;

            if (RequiredChannelId is not null && channelId != RequiredChannelId)
                return false;

            return true;
        }

    }
}