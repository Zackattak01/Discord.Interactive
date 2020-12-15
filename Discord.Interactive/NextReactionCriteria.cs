using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class NextReactionCriteria : UserChannelCriteria<ReactionEventData>
    {
        public IReadOnlyCollection<IEmote> Reactions { get; }

        public NextReactionCriteria()
            : this(null, null, null) { }

        public NextReactionCriteria(ICommandContext context)
            : base(context) { }

        internal NextReactionCriteria(ulong? userId, ulong? channelId, IEnumerable<IEmote> reactions)
            : base(userId, channelId)
        {

            Reactions = reactions?.ToList();
            //Reactions ??= new List<IEmote>();
        }

        public override NextReactionCriteria EnsureUser(ulong id)
            => new NextReactionCriteria(id, RequiredChannelId, Reactions);

        public override NextReactionCriteria EnsureUser(IUser user)
            => EnsureUser(user.Id);

        public override NextReactionCriteria EnsureChannel(ulong id)
            => new NextReactionCriteria(RequiredUserId, id, Reactions);

        public override NextReactionCriteria EnsureChannel(IChannel channel)
            => EnsureChannel(channel.Id);

        public NextReactionCriteria EnsureEmotes(params IEmote[] emotes)
            => EnsureEmotes(emotes);

        public NextReactionCriteria EnsureEmotes(IEnumerable<IEmote> emotes)
        {
            var newReactions = Reactions?.ToList();
            newReactions ??= new List<IEmote>();

            newReactions.AddRange(emotes);
            return new NextReactionCriteria(RequiredUserId, RequiredChannelId, newReactions);
        }

        public NextReactionCriteria EnsureEmote(IEmote emote)
        {
            var newReactions = Reactions?.ToList();
            newReactions ??= new List<IEmote>();

            newReactions.Add(emote);
            return new NextReactionCriteria(RequiredUserId, RequiredChannelId, newReactions);
        }



        public override Task<bool> ValidateAsync(ReactionEventData reactionData)
        {
            var reaction = reactionData.Reaction;

            if (!base.Validate(reaction.UserId, reaction.Channel.Id))
                return Task.FromResult(false);

            if (Reactions is not null)
            {
                var emoteMatched = Reactions.Any(x => x.Equals(reaction.Emote));

                return Task.FromResult(emoteMatched);

            }

            return Task.FromResult(true);
        }

    }
}
