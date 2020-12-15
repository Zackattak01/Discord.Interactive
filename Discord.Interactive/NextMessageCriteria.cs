using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Discord.Commands;

namespace Discord.Interactive
{
    public class NextMessageCriteria : UserChannelCriteria<IMessage>
    {

        public NextMessageCriteria()
            : this(null, null) { }

        public NextMessageCriteria(ICommandContext context)
            : this(context.Message.Author.Id, context.Channel.Id) { }

        internal NextMessageCriteria(ulong? userId, ulong? channelId)
            : base(userId, channelId) { }

        public override NextMessageCriteria EnsureUser(ulong id)
            => new NextMessageCriteria(id, RequiredChannelId);

        public override NextMessageCriteria EnsureUser(IUser user)
            => new NextMessageCriteria(user.Id, RequiredChannelId);

        public override NextMessageCriteria EnsureChannel(ulong id)
            => new NextMessageCriteria(RequiredUserId, id);

        public override NextMessageCriteria EnsureChannel(IChannel channel)
            => new NextMessageCriteria(RequiredUserId, channel.Id);

        public override Task<bool> ValidateAsync(IMessage message)
            => Task.FromResult(base.Validate(message.Author, message.Channel));


    }
}