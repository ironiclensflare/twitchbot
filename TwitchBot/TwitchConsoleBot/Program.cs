using System;
using TwitchBot;

namespace TwitchConsoleBot
{
    internal class Program
    {
        private static void Main()
        {
            var bot = new TwitchClient("irc.chat.twitch.tv", "ironic_bot", "oauth:1nrbmhkdbbmutfh7k0vik49yhvupi9", "ironiclensflare");

            bot.BotReady += (sender, eventArgs) => { bot.SendMessage("The bot is alive!"); };
            bot.NewMessage += (sender, args) =>
            {
                Console.WriteLine($"{args.Timestamp.ToShortTimeString()} - New message from {args.Name}: {args.Message}");
                Console.WriteLine(bot.Active
                    ? "I'm currently active, so I'll deal with this message..."
                    : "I'm not active, so I'll ignore this message...");
            };

            bot.Connect();

            var exit = false;
            while (!exit)
            {
                var input = Console.ReadKey();
                if (input.KeyChar.Equals('q')) { exit = true; }
            }
        }
    }
}
