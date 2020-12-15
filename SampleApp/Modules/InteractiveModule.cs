using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactive;

namespace SampleApp
{
    public class InteractiveModule : ModuleBase<SocketCommandContext>
    {
        public InteractiveService Interactive { get; set; }

        [Command("NextMessage")]
        public async Task NextMessageAsync()
        {
            await ReplyAsync("Waiting for the next recieved message (no user or channel guarantee)");

            var message = await Interactive.NextMessageAsync();

            if (message is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }

            await ReplyAsync($"Recieved message: \"{message.Content}\"");
        }

        [Command("NextMessage")]
        public async Task NextMessageAsync(string useDefault)
        {


            await ReplyAsync("Waiting for the the next message with default critiera (user & channel guarantee)");
            var criteria = new NextMessageCriteria(Context); //gives default
            //var criteria = new NextMessageCriteria().EnsureUser(Context.User).EnsureChannel(Context.Channel); also default

            var message = await Interactive.NextMessageAsync(criteria);

            if (message is not null && (message.Author.Id != Context.User.Id || message.Channel.Id != Context.Channel.Id))
                await ReplyAsync("Default NextMessageCriteria is broken!");

            if (message is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }

            await ReplyAsync($"Recieved message: \"{message.Content}\"");
        }

        [Command("NextMessage")]
        public async Task NextMessageAsync(IUser user)
        {
            await ReplyAsync("Waiting for reply for tagged user!");

            var criteria = new NextMessageCriteria().EnsureUser(user);
            var message = await Interactive.NextMessageAsync(criteria);

            if (message is not null && message.Author.Id != user.Id)
                await ReplyAsync("EnsureUser on NextMessageCriteria is broken!");

            if (message is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }


            await ReplyAsync($"Recieved message: \"{message.Content}\"");
        }

        [Command("NextReaction")]
        public async Task NextReactionAsync()
        {
            await ReplyAsync("Waiting for the next reaction (no user or channel guarantee)");

            var reaction = await Interactive.NextReactionAsync();

            if (reaction is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }

            await ReplyAsync($"Recieved reaction: \"{reaction.Emote.Name}\"");
        }

        [Command("NextReaction")]
        public async Task NextReactionAsync(string useDefault)
        {


            await ReplyAsync("Waiting for the the next reaction with default critiera (user & channel guarantee)");
            var criteria = new NextReactionCriteria(Context); //gives default
            //var criteria = new NextMessageCriteria().EnsureUser(Context.User).EnsureChannel(Context.Channel); also default

            var reaction = await Interactive.NextReactionAsync(criteria);

            if (reaction is not null && (reaction.UserId != Context.User.Id || reaction.Channel.Id != Context.Channel.Id))
                await ReplyAsync("Default NextReactionCriteria is broken!");

            if (reaction is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }

            await ReplyAsync($"Recieved message: \"{reaction.Emote.Name}\"");
        }

        [Command("NextReaction")]
        public async Task NextReactionAsync(IUser user)
        {
            await ReplyAsync("Waiting for the the next reaction with default critiera + emote (user & channel & emote guarantee)");

            var emoji = new Emoji("\uD83D\uDC4D"); //:thumbsup:
            var criteria = new NextReactionCriteria(Context).EnsureEmote(emoji);

            var reaction = await Interactive.NextReactionAsync(criteria);

            if (reaction is not null && (reaction.UserId != Context.User.Id || reaction.Channel.Id != Context.Channel.Id || reaction.Emote.Equals(emoji)))
                await ReplyAsync("NextReactionCriteria is broken!");

            if (reaction is null)
            {
                await ReplyAsync("Operation Timed Out");
                return;
            }

            await ReplyAsync($"Recieved message: \"{reaction.Emote.Name}\"");
        }
    }
}
