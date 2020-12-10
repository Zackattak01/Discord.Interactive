using System;
using System.Threading;
using Discord;
using Discord.WebSocket;

namespace Discord.Interactive
{
    public class InteractiveService
    {
        private BaseSocketClient Client { get; }

        public TimeSpan DefaultTimeout { get; }

        public InteractiveService(BaseSocketClient client, TimeSpan? defaultTimeout = null)
        {
            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(15);
        }

        public InteractiveService(DiscordSocketClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }

        public InteractiveService(DiscordShardedClient client, TimeSpan? defaultTimeout = null)
            : this(client as BaseSocketClient, defaultTimeout) { }



    }
}
