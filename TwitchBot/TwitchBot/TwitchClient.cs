using System;
using ChatSharp;
using ChatSharp.Events;

namespace TwitchBot
{
    public class TwitchClient
    {
        private readonly IrcClient _irc;
        private readonly string _botName;
        private readonly string _channelName;

        public TwitchClient(string server, string botName, string password, string channelName)
        {
            _irc = new IrcClient(server, new IrcUser(botName, botName, password));
            _botName = botName;
            _channelName = channelName.StartsWith("#") ? channelName : "#" + channelName;
        }

        public TwitchClient(IrcClient irc, string channelName)
        {
            _irc = irc;
            _botName = irc.User.Nick;
            _channelName = channelName.StartsWith("#") ? channelName : "#" + channelName;
        }

        /// <summary>
        /// Indicates whether the bot is active and will handle chat messages.
        /// </summary>
        public bool Active { get; set; }
        
        /// <summary>
        /// Connects the bot to IRC.
        /// </summary>
        public void Connect()
        {
            _irc.ConnectionComplete += (s, e) =>
            {
                Console.WriteLine("Connected successfully!");
                _irc.SendRawMessage("CAP REQ :twitch.tv/membership");
                _irc.SendRawMessage("CAP REQ :twitch.tv/tags");

                Console.WriteLine($"Joining  {_channelName}...");
                _irc.JoinChannel(_channelName);
            };

            _irc.ModeChanged += ModeChanged;
            _irc.TwitchMessageReceived += TwitchMessageReceived;

            Console.WriteLine($"Connecting to {_irc.ServerAddress}...");
            _irc.ConnectAsync();
        }

        private void TwitchMessageReceived(object sender, TwitchMessageEventArgs e)
        {
            OnMessageReceived(e);
        }

        /// <summary>
        /// Sends a message to the channel.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            _irc.SendMessage(message, _channelName);
        }

        private void ModeChanged(object sender, ModeChangeEventArgs e)
        {
            Console.WriteLine($"Mode change: {e.Change}");
            var regex = new System.Text.RegularExpressions.Regex(@"([\+-]\w)\s(.*)");
            var match = regex.Match(e.Change);
            if (!match.Success) return;
            var operation = match.Groups[1].Value;
            var username = match.Groups[2].Value;
            if (operation.Equals("+o") && username.Equals(_botName)) OnBotOpped();
            else if (operation.Equals("-o") && username.Equals(_botName)) OnBotDeopped();
        }

        /// <summary>
        /// Triggered when the bot has successfully connected to a channel
        /// and is ready to start moderating.
        /// </summary>
        public event EventHandler BotReady;

        internal void OnBotReady()
        {
            BotReady?.Invoke(null, null);
        }

        /// <summary>
        /// Triggered when the bot gains operator status in the channel.
        /// </summary>
        public event EventHandler BotOpped;

        internal void OnBotOpped()
        {
            BotOpped?.Invoke(null, null);
            if (!Active) BotReady?.Invoke(null, null);
            Active = true;
        }

        /// <summary>
        /// Triggered when the bot loses its operator status in the channel.
        /// </summary>
        public event EventHandler BotDeopped;

        internal void OnBotDeopped()
        {
            BotDeopped?.Invoke(null, null);
            Active = false;
        }

        /// <summary>
        /// Triggered when a new message is received in the channel.
        /// </summary>
        public event EventHandler<MessageEventArgs> NewMessage;

        internal void OnMessageReceived(TwitchMessageEventArgs e)
        {
            var messageArgs = new MessageEventArgs(e.Username, e.Message);
            messageArgs.IsMod = e.Mod;
            NewMessage?.Invoke(null, messageArgs);
        }
    }
}
