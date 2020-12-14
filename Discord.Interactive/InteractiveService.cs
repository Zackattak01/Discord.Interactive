using System;
using System.Threading;
using System.Threading.Tasks;
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

        
        public Task<SocketMessage> NextMessageAsync(TimeSpan? timeout = null){
            var socketMessageSource = new TaskCompletionSource<SocketMessage>();

            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);
            var socketMessageTask = socketMessageSource.Task;


            Task MessageHandler(SocketMessage message) {
                if(message.Author.Id == Client.CurrentUser.Id){
                    return Task.CompletedTask;
                }

                socketMessageSource.SetResult(message);
                return Task.CompletedTask;
            }

            try{
                Client.MessageReceived += MessageHandler;

                var firstTaskCompleted = Task.WhenAny(timeoutTask, socketMessageTask);

                if(firstTaskCompleted == timeoutTask)
                    return null;
                else
                    return socketMessageTask;
            }
            finally{
                Client.MessageReceived -= MessageHandler;
            }
        }



    }
}
