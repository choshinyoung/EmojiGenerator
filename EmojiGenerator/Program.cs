using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EmojiGenerator
{
    public class Program
    {
        // Your emoji server ids
        public static readonly ulong[][] Guilds = { new ulong[] { 123456789012345678, 123456789012345678 },   // ㄱㄴㄷㄹ
                                                    new ulong[] { 123456789012345678, 123456789012345678 },   // ㅁㅂㅅ
                                                    new ulong[] { 123456789012345678, 123456789012345678 },   // ㅇㅈ
                                                    new ulong[] { 123456789012345678, 123456789012345678 },   // ㅊㅋㅌㅍㅎ
                                                    new ulong[] { 123456789012345678, 123456789012345678 } }; // etc

        // Your user id
        public static readonly ulong[] Owners = { 123456789012345678 };

        private static readonly string token = "TOKEN";


        public static DiscordSocketClient client;
        public static CommandService Commands;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            Commands = new CommandService();

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await client.LoginAsync(TokenType.Bot, token);

            client.Ready += Client_Ready;
            client.GuildAvailable += Client_GuildAvailble;
            client.MessageReceived += Client_MessageReceived;

            await client.StartAsync();
            await client.SetGameAsync("-도움말", null, ActivityType.Playing);
            await client.SetStatusAsync(UserStatus.Online);

            await Task.Delay(-1);
        }

        private async Task Client_GuildAvailble(SocketGuild arg)
        {
            Console.WriteLine(arg.Name + "(" + arg.Id + ")" + "에 연결되었습니다.");

            await Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            Console.WriteLine("준비되었습니다.");

            await Task.CompletedTask;
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var message = MessageParam as SocketUserMessage;

            var context = new SocketCommandContext(client, message);

            if (context.Message == null || context.Message.Content == "" || context.User.IsBot) return;

            int ArgPos = 0;
            if (context.Message.HasStringPrefix("-", ref ArgPos))
            {
                await Commands.ExecuteAsync(context, ArgPos, null);
            }
        }
    }
}
