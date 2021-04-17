using Discord.Commands;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading.Tasks;
using KoreanText;
using Discord.WebSocket;
using System.Linq;
using Discord;
using Gif.Components;

namespace EmojiGenerator
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private static readonly int          style  = (int)FontStyle.Bold;
        private static readonly RectangleF   rect   = new RectangleF(0, 0, 512, 512);
        private static readonly StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        private static readonly Pen          pen    = new Pen(System.Drawing.Color.FromArgb(54, 57, 64), 30);


        private static readonly string[]     kor    = { "ㄱ", "ㄴ", "ㄷ", "ㄹ", "ㅁ", "ㅂ", "ㅅ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ", "ㅃ", "ㅉ", "ㄸ", "ㄲ", "ㅆ", "ㅏ", "ㅑ", "ㅓ", "ㅕ", "ㅗ", "ㅛ", "ㅜ", "ㅠ", "ㅡ", "ㅣ", "ㅐ", "ㅔ", "ㅒ", "ㅖ",
                                                        "ㄳ", "ㄵ", "ㄶ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅄ", "ㅘ", "ㅙ", "ㅚ", "ㅝ", "ㅞ", "ㅟ", "ㅢ" };

        private static readonly string[]     eng    = { "r", "s", "e", "f", "a", "q", "t", "d", "w", "c", "z", "x", "v", "g", "Q", "W", "E", "R", "T", "k", "i", "j", "u", "h", "y", "n", "b", "m", "l", "o", "p", "O", "P",
                                                        "rt", "sw", "sg", "fr", "fa", "fq", "ft", "fx", "fv", "fg", "qt", "hk", "ho", "hl", "nj", "np", "nl", "ml" };

        private static readonly string[]     alpha  = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~".ToCharArray().Select(c => c.ToString()).ToArray();

        [Command("생성")]
        public async Task Add([Remainder]string text)
        {
            if (!Program.Owners.Contains(Context.User.Id))
            {
                await Utility.SendEmbedMessage(Context, "이 커맨드를 실행할 권한이 없습니다.");
                return;
            }
            
            string name = GetName(text);

            if (name.Length == 0)
            {
                await Utility.SendEmbedMessage(Context, "특수문자를 사용할 수 없습니다.");
                return;
            }
            if (name.Length == 1)
            {
                name += '_';
            }

            await Custom(name, "NanumGothic", "white", "_", text);
        }

        [Command("커스텀")]
        public async Task Custom(string name, string fontName, string color, string fontSize, [Remainder] string text)
        {
            if (!Program.Owners.Contains(Context.User.Id))
            {
                await Utility.SendEmbedMessage(Context, "이 커맨드를 실행할 권한이 없습니다.");
                return;
            }

            name = new string(name.Where(c => alpha.Contains(c.ToString())).ToArray());
            if (name.Length == 1)
            {
                await Utility.SendEmbedMessage(Context, "이름이 한 글자 이하인 이모지를 만들 수 없습니다.");

                return;
            }

            FontFamily font = new FontFamily(fontName == "_" ? "NanumGothic" : fontName);
            
            Bitmap img = new Bitmap(512, 512);
            Graphics graphic = Graphics.FromImage(img);
            graphic.Clear(System.Drawing.Color.Transparent);

            graphic.InterpolationMode = InterpolationMode.High;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            
            int len = text.Length;
            float size = fontSize == "_" ? (len == 1 ? 300 : len <= 4 ? 225 : 175) : int.Parse(fontSize);

            GraphicsPath path = new GraphicsPath();
            path.AddString(text, font, style, size, rect, format);
            graphic.DrawPath(pen, path);
            graphic.FillPath(new SolidBrush(System.Drawing.Color.FromName(color == "_" ? "White" : color)), path);

            graphic.Dispose();

            SocketGuild guild = GetChoGuild(text[0]);
            if (guild == null)
            {
                await Utility.SendEmbedMessage(Context, "남은 이모지 슬롯이 없습니다.");

                return;
            }

            if (GetChoGuilds(text[0]).Where(g => g.Emotes.Where(e => e.Name == name).Count() != 0).Count() == 0)
            {
                if (guild.Emotes.Where(e => !e.Animated).Count() < 50)
                {
                    img.Save($"img/{name}.png");
                    img.Dispose();

                    GuildEmote emote = await guild.CreateEmoteAsync(name, new Discord.Image($"img/{name}.png"));
                    await ReplyAsync(emote.ToString());
                }
                else
                {
                    AnimatedGifEncoder e = new AnimatedGifEncoder();
                    e.SetTransparent(System.Drawing.Color.Black);
                    e.Start($"img/{name}.gif");

                    e.SetDelay(1);
                    e.SetRepeat(-1);

                    e.AddFrame(img);
                    e.AddFrame(img);

                    e.Finish();
                    img.Dispose();

                    GuildEmote emote = await guild.CreateEmoteAsync(name, new Discord.Image($"img/{name}.gif"));
                    await ReplyAsync(emote.ToString());
                }
            }
            else
            {
                await Utility.SendEmbedMessage(Context, "이미 존재하는 이모지입니다.");
            }
        }

        [Command("제거")]
        public async Task Delete(string text)
        {
            if (!Program.Owners.Contains(Context.User.Id))
            {
                await Utility.SendEmbedMessage(Context, "이 커맨드를 실행할 권한이 없습니다.");
                return;
            }

            string name = GetName(text);
            SocketGuild[] guildss = GetChoGuilds(text[0]);

            bool isDeleted = false;
            foreach (SocketGuild guild in guildss)
            {
                GuildEmote[] emotes = guild.Emotes.Where(e => e.Name == name).ToArray();
                if (emotes.Length > 0)
                {
                    await guild.DeleteEmoteAsync(emotes[0]);

                    isDeleted = true;
                }
            }

            if (isDeleted)
            {
                await Utility.SendEmbedMessage(Context, "제거되었습니다.");
            }
            else
            {
                await Utility.SendEmbedMessage(Context, "존재하지 않는 이모지입니다.");
            }
        }

        [Command("상태")]
        public async Task Status()
        {
            SocketGuild guild;

            EmbedBuilder emb = Utility.GetEmbedTemplate(Context);
            emb.Title = "이모지 서버 슬롯 사용량";

            foreach (ulong[] ids in Program.Guilds)
            {
                foreach (ulong id in ids)
                {
                    guild = Program.client.GetGuild(id);
                    emb.AddField(guild.Name, $"**{guild.Emotes.Count}** / 100", true);
                }
                emb.AddField("** **", "** **", false);
            }

            await ReplyAsync("", false, emb.Build());
        }

        [Command("도움말")]
        public async Task Help()
        {
            EmbedBuilder emb = Utility.GetEmbedTemplate(Context);
            emb.Title = "이모지 생성기 사용 방법";

            emb.AddField("-생성 [텍스트]", "텍스트가 쓰인 기본적인 이모지를 생성합니다.");
            emb.AddField("-커스텀 [이름] [폰트] [색] [크기] [텍스트]", "사용자 지정 이모지를 생성합니다.\n'_'를 적어 기본값을 사용할 수 있습니다.");
            emb.AddField("-제거 [텍스트]", "이모지를 제거합니다.");
            emb.AddField("-상태", "이모지 서버의 슬롯 사용량을 보여줍니다.");

            await ReplyAsync("", false, emb.Build());
        }

        public string GetName(string text)
        {
            string name = "";
            foreach (char c in text)
            {
                if (alpha.Contains(c.ToString()))
                {
                    name += c;

                    continue;
                }

                KoreanChar kc = new KoreanChar(c);

                foreach (KoreanChar sung in new[] { kc.GetChoSung(), kc.GetJoongSung(), kc.GetJongSung() })
                {
                    string s = sung.GetChar().ToString();
                    if (kor.Contains(s))
                    {
                        name += eng[Array.IndexOf(kor, s)];
                    }
                }
            }

            return name;
        }

        public SocketGuild GetChoGuild(char c)
        {
            SocketGuild[] guild;
            string cho = new KoreanChar(c).GetChoSung().GetChar().ToString();
            if ("ㄱㄲㄴㄷㄸㄹ".Contains(cho))
                guild = Program.Guilds[0].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅁㅂㅃㅅㅆ".Contains(cho))
                guild = Program.Guilds[1].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅇㅈㅉ".Contains(cho))
                guild = Program.Guilds[2].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅊㅋㅌㅍㅎ".Contains(cho))
                guild = Program.Guilds[3].Select(g => Program.client.GetGuild(g)).ToArray();
            else
                guild = Program.Guilds[4].Select(g => Program.client.GetGuild(g)).ToArray();

            SocketGuild grr = null;
            foreach(SocketGuild gl in guild)
            {
                if (gl.Emotes.Count < 100)
                {
                    grr = gl;
                    break;
                }
            }

            return grr;
        }

        public SocketGuild[] GetChoGuilds(char c)
        {
            SocketGuild[] guild;
            string cho = new KoreanChar(c).GetChoSung().GetChar().ToString();
            if ("ㄱㄲㄴㄷㄸㄹ".Contains(cho))
                guild = Program.Guilds[0].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅁㅂㅃㅅㅆ".Contains(cho))
                guild = Program.Guilds[1].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅇㅈㅉ".Contains(cho))
                guild = Program.Guilds[2].Select(g => Program.client.GetGuild(g)).ToArray();
            else if ("ㅊㅋㅌㅍㅎ".Contains(cho))
                guild = Program.Guilds[3].Select(g => Program.client.GetGuild(g)).ToArray();
            else
                guild = Program.Guilds[4].Select(g => Program.client.GetGuild(g)).ToArray();

            return guild;
        }
    }
}
