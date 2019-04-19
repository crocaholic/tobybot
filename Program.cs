using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
namespace tobybot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.UserJoined += UserJoinedAsync;

            //Create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");            
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            //This is where we get the Token value from the configuration file
            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task UserJoinedAsync(SocketGuildUser user)
        {
            var welcomechan = user.Guild.GetChannel(568941228251545611);
            if(welcomechan != null)
            {
                ITextChannel realchan = welcomechan as ITextChannel;

                realchan.SendMessageAsync($":grinning: Hello, **{user.Username}**! Please be patient while our hamsters run a background check on you to ensure you aren't involved with the DEA or FBI! <:weed:568938819026681896> :spy:");
            }
            return Task.CompletedTask;
        }
        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        // This is called everytime a user sends a message!
        private async Task MessageReceivedAsync(SocketMessage message)
        {
           var msg = message as SocketUserMessage;
           var context = new SocketCommandContext(_client, msg);

            // this will make sure the bot doesn't try to eat itself
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            /*if(context.Message.Channel.Id == 566447876012769321)
            {
                var coolmsg = context.Message;

                // this is so the reactions can be automatically added
                IEmote up = new Emoji("👍");
                IEmote down = new Emoji("👎");
                await coolmsg.AddReactionsAsync(new[] {up, down});
            }*/
            if(message.Content.StartsWith(";;verify "))
            {
                string idString = message.Content.Replace(";;verify ", "");
                ulong realId;
                if(ulong.TryParse(idString, out realId))
                {
                    var welcomechan = context.Guild.GetChannel(538627968986251296);
                    var verifiedUser = context.Guild.GetUser(realId);
                    if(welcomechan != null && verifiedUser != null)
                    {
                        var role = context.Guild.GetRole(568944460503711773);
                        await verifiedUser.AddRoleAsync(role);
                        await context.Message.DeleteAsync();
                        ITextChannel realchan = welcomechan as ITextChannel;
                        await realchan.SendMessageAsync($"Welcome to {context.Guild.Name}, **{verifiedUser.Mention}**! Have fun and don't get caught :wink:");
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync(":x: Failed to detect a user ID from your message.");
                }
            }
        }
    }
}