using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class NextReactionCriteria : UserChannelCriteria<ReactionEventData>
    {
        public IReadOnlyCollection<IEmote> RequiredReactions { get; }

        public ulong? RequiredMessageId { get; }

        public NextReactionCriteria()
            : this(null, null, null, null) { }

        public NextReactionCriteria(ICommandContext context)
            : base(context) { }

        internal NextReactionCriteria(ulong? userId, ulong? channelId, ulong? messageId, IEnumerable<IEmote> reactions)
            : base(userId, channelId)
        {
            RequiredMessageId = messageId;
            RequiredReactions = reactions?.ToList();
        }

        public override NextReactionCriteria EnsureUser(ulong id)
            => new NextReactionCriteria(id, RequiredChannelId, RequiredMessageId, RequiredReactions);

        public override NextReactionCriteria EnsureUser(IUser user)
            => EnsureUser(user.Id);

        public override NextReactionCriteria EnsureChannel(ulong id)
            => new NextReactionCriteria(RequiredUserId, id, RequiredMessageId, RequiredReactions);

        public override NextReactionCriteria EnsureChannel(IChannel channel)
            => EnsureChannel(channel.Id);

        public NextReactionCriteria EnsureEmotes(params IEmote[] emotes)
            => EnsureEmotes(emotes);

        public NextReactionCriteria EnsureEmotes(IEnumerable<IEmote> emotes)
        {
            var newReactions = RequiredReactions?.ToList();
            newReactions ??= new List<IEmote>();

            newReactions.AddRange(emotes);
            return new NextReactionCriteria(RequiredUserId, RequiredChannelId, RequiredMessageId, newReactions);
        }

        public NextReactionCriteria EnsureEmote(IEmote emote)
        {
            var newReactions = RequiredReactions?.ToList();
            newReactions ??= new List<IEmote>();

            newReactions.Add(emote);
            return new NextReactionCriteria(RequiredUserId, RequiredChannelId, RequiredMessageId, newReactions);
        }

        public NextReactionCriteria EnsureMessage(IMessage message)
            => new NextReactionCriteria(RequiredUserId, message.Channel.Id, message.Id, RequiredReactions);

        public override Task<bool> ValidateAsync(ReactionEventData reactionData)
        {
            var reaction = reactionData.Reaction;

            if (!base.Validate(reaction.UserId, reaction.Channel.Id))
                return Task.FromResult(false);

            if (RequiredMessageId is not null)
            {
                if (reaction.MessageId != RequiredMessageId)
                    return Task.FromResult(false);
            }

            if (RequiredReactions is not null)
            {
                var emoteMatched = RequiredReactions.Any(x => x.Equals(reaction.Emote));

                return Task.FromResult(emoteMatched);

            }

            return Task.FromResult(true);
        }

    }
}
