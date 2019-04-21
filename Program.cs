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
            if(welcomechan != null && !user.IsBot)
            {
                ITextChannel realchan = welcomechan as ITextChannel;

                var role = user.Guild.GetRole(568944460503711773);

                user.AddRoleAsync(role);
                
                //realchan.SendMessageAsync($":smile: Welcome to {user.Guild.Name}, **{user.Username}**! " +
                //"\nWe *highly* indorse you to introduce yourself :wink:");
            }
            return Task.CompletedTask;
        }
        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);

            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            await Task.Delay(0);
        }
    }
}