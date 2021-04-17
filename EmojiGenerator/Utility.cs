using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace EmojiGenerator
{
    class Utility
    {
        public static EmbedBuilder GetEmbedTemplate(SocketCommandContext context)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Color = new Color(255, 200, 0);

            string nickname = "";
            if (context.IsPrivate)
                nickname = context.User.Username;
            else
                nickname = (context.User as SocketGuildUser).Nickname;

            builder.Footer = new EmbedFooterBuilder
            {
                Text = string.IsNullOrEmpty(nickname) ? context.User.Username : nickname,
                IconUrl = context.User.GetAvatarUrl() == "" || context.User.GetAvatarUrl() == null ? context.User.GetDefaultAvatarUrl() : context.User.GetAvatarUrl()
            };

            return builder;
        }

        public static async Task<RestUserMessage> SendEmbedMessage(SocketCommandContext context, string content)
        {
            EmbedBuilder emb = GetEmbedTemplate(context);
            emb.WithDescription(content);

            return await context.Channel.SendMessageAsync("", false, emb.Build());
        }
    }
}
