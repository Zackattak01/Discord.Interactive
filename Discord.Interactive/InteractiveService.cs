﻿using System;
using System.Reflection;
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

        
        public async Task<SocketMessage> NextMessageAsync(MessageCriteria criteria = null, TimeSpan? timeout = null){
            criteria ??= MessageCriteria.Default;

            var socketMessageSource = new TaskCompletionSource<SocketMessage>();

            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);
            var socketMessageTask = socketMessageSource.Task;


            Task MessageHandler(SocketMessage message) {
                if(message.Author.Id == Client.CurrentUser.Id){
                    return Task.CompletedTask;
                }


                if(criteria.Validate(message))
                    socketMessageSource.SetResult(message);
                    
                return Task.CompletedTask;
            }

            try{
                Client.MessageReceived += MessageHandler;

                var firstTaskCompleted = await Task.WhenAny(timeoutTask, socketMessageTask).ConfigureAwait(false);

                if(firstTaskCompleted == timeoutTask)
                    return null;
                else
                    return await socketMessageTask.ConfigureAwait(false);
            }
            finally{
                Client.MessageReceived -= MessageHandler;
            }
        }



    }
}
